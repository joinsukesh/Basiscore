namespace Basiscore.CmsAudit.Services
{
    using Basiscore.CmsAudit.Models;
    using Newtonsoft.Json;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Publishing.Pipelines.Publish;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Text;
    using static Basiscore.CmsAudit.Constants;

    public class CmsAuditService
    {
        public CmsAuditService()
        {
            IsAuditLoggingEnabled = Configurations.EnableCmsAuditLogging;
        }

        public bool IsAuditLoggingEnabled { get; set; }

        /// <summary>
        /// determine if the audit logging should happen for this item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ProceedToInsertItemAuditLog(Item item)
        {
            bool proceedToInsertAuditLog = false;

            /// on CMS page refresh the OnItemSaving & OnItemSaved events will be triggered for items of "Schedule" template
            /// e.g: /sitecore/system/Settings/Email/Instance Tasks/Content Management Primary/Message Statistics/Today
            /// So, proceed to log only if it is not those items
            if (item.TemplateID != Templates.System.Tasks.Schedule)
            {
                List<ID> auditItemTemplateIds = new Configurations().SpecifiedTemplateIds;

                ///check if the created item's template id matches any from the defined list
                if (auditItemTemplateIds.Count <= 0 || auditItemTemplateIds.Any(x => x == item.TemplateID))
                {
                    proceedToInsertAuditLog = true;
                }
            }

            return proceedToInsertAuditLog;
        }

        public void BuildItemAuditInfo(Item originalItem, Item changedItem, ItemEventType itemEvent, DateTime logTime,
             string itemEventAuditLabel, string changeLog = "", string username = "")
        {
            List<KeyValuePair<string, string>> itemFieldsAndValues_Old = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> itemFieldsAndValues_New = new List<KeyValuePair<string, string>>();
            IEnumerable<Field> fields = null;
            List<string> allFieldNamesForAudit = new List<string>();
            List<string> validStandardFieldNames = new List<string>();

            /// collect fields & values of original item
            originalItem.Fields.ReadAll();
            fields = originalItem.Fields.Where(x => !x.Name.StartsWith(Constants.DoubleUnderscore));
            allFieldNamesForAudit.AddRange(fields.Select(x => x.Name));

            foreach (Field field in fields)
            {
                itemFieldsAndValues_Old.Add(new KeyValuePair<string, string>(field.Name, field.Value));
            }

            List<string> specifiedStandardFieldNames = new Configurations().SpecifiedStandardFieldNames;

            foreach (string fieldName in specifiedStandardFieldNames)
            {
                try
                {
                    if (originalItem.Fields[fieldName] != null)
                    {
                        itemFieldsAndValues_Old.Add(new KeyValuePair<string, string>(fieldName, originalItem.Fields[fieldName].Value));
                        validStandardFieldNames.Add(fieldName);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(Constants.ModuleName, ex, this);
                }
            }

            /// collect fields & values of changed item
            changedItem.Fields.ReadAll();
            fields = changedItem.Fields.Where(x => !x.Name.StartsWith(Constants.DoubleUnderscore));
            allFieldNamesForAudit.AddRange(fields.Select(x => x.Name));

            foreach (Field field in fields)
            {
                itemFieldsAndValues_New.Add(new KeyValuePair<string, string>(field.Name, field.Value));
            }

            foreach (string fieldName in validStandardFieldNames)
            {
                try
                {
                    itemFieldsAndValues_New.Add(new KeyValuePair<string, string>(fieldName, changedItem.Fields[fieldName].Value));
                }
                catch (Exception ex)
                {
                    Log.Error(Constants.ModuleName, ex, this);
                }
            }

            if ((itemEvent != ItemEventType.ITEM_SAVED && itemEvent != ItemEventType.SITE_PUBLISHED) ||
               IsDataDifferentForOldAndNewItems(originalItem, changedItem, itemFieldsAndValues_Old,
               itemFieldsAndValues_New))
            {
                ///collect all fieldnames to compare values of old & new item
                if (validStandardFieldNames.Count > 0)
                {
                    allFieldNamesForAudit.AddRange(validStandardFieldNames);
                }

                allFieldNamesForAudit = allFieldNamesForAudit.Distinct().ToList();
                AuditLog_Item ciai = PrepareAuditLog(originalItem, changedItem, itemEvent, logTime,
                    allFieldNamesForAudit, itemFieldsAndValues_Old, itemFieldsAndValues_New, itemEventAuditLabel,
                    changeLog, username);
                new DbService().InsertItemAuditLog(ciai);
            }
        }

        /// <summary>
        /// check if both old item data & new item data are the same
        /// </summary>
        /// <returns></returns>
        private bool IsDataDifferentForOldAndNewItems(Item originalItem, Item changedItem,
            List<KeyValuePair<string, string>> itemFieldsAndValues_Old,
            List<KeyValuePair<string, string>> itemFieldsAndValues_New)
        {
            bool isDataDifferentForOldAndNewItems = true;

            if (itemFieldsAndValues_Old.Count == itemFieldsAndValues_New.Count)
            {
                itemFieldsAndValues_Old = itemFieldsAndValues_Old.OrderBy(x => x.Key).ToList();
                itemFieldsAndValues_New = itemFieldsAndValues_New.OrderBy(x => x.Key).ToList();

                if (itemFieldsAndValues_Old.SequenceEqual(itemFieldsAndValues_New) &&
                    originalItem.ID == changedItem.ID && originalItem.TemplateID == changedItem.TemplateID &&
                           originalItem.Name == changedItem.Name && originalItem.Paths.Path == changedItem.Paths.Path &&
                           originalItem.Language == changedItem.Language && originalItem.Version == changedItem.Version)
                {
                    isDataDifferentForOldAndNewItems = false;
                }
            }

            return isDataDifferentForOldAndNewItems;
        }

        private AuditLog_Item PrepareAuditLog(Item originalItem, Item changedItem, ItemEventType itemEvent,
            DateTime logTime, List<string> allFieldNamesForAudit, List<KeyValuePair<string, string>> itemFieldsAndValues_Old,
            List<KeyValuePair<string, string>> itemFieldsAndValues_New, string itemEventAuditLabel, string changeLog, string username)
        {
            ItemInfo itemDataBeforeSave = null;
            ItemInfo itemDataAfterSave = null;
            StringBuilder sbAnnotations = new StringBuilder(string.Empty);
            string originalItemPath = originalItem.Paths.Path;
            string changedItemPath = changedItem.Paths.Path;

            if (itemEvent == ItemEventType.ITEM_MOVED)
            {
                originalItemPath = changeLog;
                sbAnnotations.AppendLine("Previous Path: " + changeLog);
            }
            else if (itemEvent == ItemEventType.ITEM_COPIED || itemEvent == ItemEventType.ITEM_CLONE_ADDED)
            {
                sbAnnotations.AppendLine("Source Item: " + changeLog);
            }
            else if (itemEvent == ItemEventType.ITEM_DELETED)
            {
                originalItemPath = string.Empty;
                changedItemPath = string.Empty;
            }
            else if (itemEvent == ItemEventType.ITEM_PUBLISHED)
            {
                sbAnnotations.AppendLine(changeLog);
            }
            else
            {
                sbAnnotations.AppendLine(GetChangeLog(allFieldNamesForAudit, itemFieldsAndValues_Old,
                        itemFieldsAndValues_New));
            }

            itemDataBeforeSave = new ItemInfo
            {
                ItemId = originalItem.ID.Guid,
                ItemName = originalItem.Name,
                ItemPath = originalItemPath,
                TemplateId = originalItem.TemplateID.Guid,
                ItemLanguage = originalItem.Language.Name,
                ItemVersion = originalItem.Version.Number,
                FieldData = itemFieldsAndValues_Old
            };

            itemDataAfterSave = new ItemInfo
            {
                ItemId = changedItem.ID.Guid,
                ItemName = changedItem.Name,
                ItemPath = changedItemPath,
                TemplateId = changedItem.TemplateID.Guid,
                ItemLanguage = changedItem.Language.Name,
                ItemVersion = changedItem.Version.Number,
                FieldData = itemFieldsAndValues_New
            };
            
            AuditLog_Item ciai = new AuditLog_Item
            {
                ItemId = changedItem.ID.Guid,
                ItemName = changedItem.Name,
                ItemPath = changedItemPath,
                TemplateId = changedItem.TemplateID.Guid,
                ItemLanguage = changedItem.Language.Name,
                ItemVersion = changedItem.Version.Number,
                Event = itemEventAuditLabel,
                ActionedBy = string.IsNullOrWhiteSpace(username) ? changedItem.Statistics.UpdatedBy : username,
                ItemDataBeforeSave = WebUtility.HtmlEncode(JsonConvert.SerializeObject(itemDataBeforeSave, Formatting.Indented)),
                ItemDataAfterSave = WebUtility.HtmlEncode(JsonConvert.SerializeObject(itemDataAfterSave, Formatting.Indented)),
                Comments = sbAnnotations.ToString(),
                LoggedTime = logTime
            };
            return ciai;
        }

        private string GetChangeLog(List<string> allFieldNamesForAudit,
            List<KeyValuePair<string, string>> itemFieldsAndValues_Old,
            List<KeyValuePair<string, string>> itemFieldsAndValues_New)
        {
            string changeLog = string.Empty;

            /// compare field values of original & changed items and collect those field names
            IEnumerable<string> differences = allFieldNamesForAudit
                            .Where(f => itemFieldsAndValues_Old.FirstOrDefault(x => x.Key == f).Value != itemFieldsAndValues_New.FirstOrDefault(x => x.Key == f).Value)
                            .Select(pair => pair)
                            .ToList();
            if (differences.Any())
            {
                changeLog = String.Format("Content changed in these fields [{0}]", differences.Aggregate((s1, s2) => s1 + ", " + s2));
            }

            return changeLog;
        }

        public void BuildPublishAuditInfo(PublishContext context)
        {
            DateTime logTime = DateTime.Now;

            if (context?.PublishOptions != null)
            {
                Item contextItem = context?.PublishOptions.RootItem;
                StringBuilder sb = new StringBuilder(string.Empty);

                /// if it is an item publish, the publish mode will always be SingleItem
                string selectedPublishMode = context.PublishOptions.Mode.ToString();

                if (context.PublishOptions.Mode == Sitecore.Publishing.PublishMode.Full ||
                    context.PublishOptions.Mode == Sitecore.Publishing.PublishMode.SingleItem)
                {
                    selectedPublishMode = context.PublishOptions.CompareRevisions ? "Smart Publish" : "Republish";
                }

                sb.AppendLine(String.Format("Mode: {0}; Published Subitems: {1}, Published Related Items: {2}",
                    selectedPublishMode, context.PublishOptions.Deep, context.PublishOptions.PublishRelatedItems));
                sb.AppendLine("Languages: " + string.Join(Constants.Comma.ToString(), context.Languages.Select(x => x.Name)));
                sb.AppendLine(String.Format("Created: {0}; Updated: {1}; Deleted: {2}; Skipped: {3}",
                    context.Statistics.Created, context.Statistics.Updated, context.Statistics.Deleted,
                    context.Statistics.Skipped));
                sb.AppendLine("Source Database: " + context.PublishOptions.SourceDatabase.Name + "; " + "Target Database: " + context.PublishOptions.TargetDatabase.Name);
                sb.AppendLine("Published Date: " + context.PublishOptions.PublishDate.ToString("dd-MMM-yyyy HH:mm:ss.fff"));

                if (contextItem == null)
                {
                    /// contextItem will be null if it is a site publish
                    AuditLog_Item ciai = new AuditLog_Item
                    {
                        ItemId = Guid.Empty,
                        ItemName = string.Empty,
                        ItemPath = string.Empty,
                        TemplateId = Guid.Empty,
                        ItemLanguage = string.Empty,
                        ItemVersion = 0,
                        Event = Constants.EventAuditLabels.Item.SITE_PUBLISHED,
                        ActionedBy = context?.PublishOptions.UserName,
                        ItemDataBeforeSave = string.Empty,
                        ItemDataAfterSave = string.Empty,
                        Comments = sb.ToString(),
                        LoggedTime = logTime
                    };

                    new DbService().InsertItemAuditLog(ciai);
                }
                else
                {
                    BuildItemAuditInfo(contextItem, contextItem, ItemEventType.ITEM_PUBLISHED, logTime, Constants.EventAuditLabels.Item.ITEM_PUBLISHED, sb.ToString(), context.PublishOptions.UserName);
                }
            }
        }
    }
}