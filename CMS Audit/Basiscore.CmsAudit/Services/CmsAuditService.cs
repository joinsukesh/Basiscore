namespace Basiscore.CmsAudit.Services
{
    using Basiscore.CmsAudit.Models;
    using Newtonsoft.Json;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
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
             string itemEventAuditLabel, string annotation = "")
        {
            List<KeyValuePair<string, string>> itemFieldsAndValues_Old = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> itemFieldsAndValues_New = new List<KeyValuePair<string, string>>();
            StringBuilder sbAnnotations = new StringBuilder(string.Empty);
            IEnumerable<Field> fields = null;
            List<string> allFieldNamesForAudit = new List<string>();
            List<string> validStandardFieldNames = new List<string>();

            if (!string.IsNullOrWhiteSpace(annotation))
            {
                sbAnnotations.AppendLine(annotation);
            }

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

            if (itemEvent == ItemEventType.ITEM_CREATED || itemEvent == ItemEventType.ITEM_VERSION_REMOVED ||
                itemEvent == ItemEventType.ITEM_DELETED ||
                IsDataDifferentForOldAndNewItems(originalItem, changedItem, itemFieldsAndValues_Old,
                itemFieldsAndValues_New))
            {
                ///collect all fieldnames to compare values of old & new item
                if (validStandardFieldNames.Count > 0)
                {
                    allFieldNamesForAudit.AddRange(validStandardFieldNames);
                }

                allFieldNamesForAudit = allFieldNamesForAudit.Distinct().ToList();
                sbAnnotations.AppendLine(GetChangeLog(allFieldNamesForAudit, itemFieldsAndValues_Old, 
                    itemFieldsAndValues_New));
                AuditLog_Item ciai = PrepareAuditLog(originalItem, changedItem, itemEvent, logTime,
                        itemFieldsAndValues_Old, itemFieldsAndValues_New, itemEventAuditLabel, sbAnnotations.ToString());
                new DbService().InsertCmsItemAuditLog(ciai);
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

            if (itemFieldsAndValues_Old.SequenceEqual(itemFieldsAndValues_New))
            {
                if (originalItem.ID == changedItem.ID && originalItem.TemplateID == changedItem.TemplateID &&
                       originalItem.Name == changedItem.Name && originalItem.Paths.Path == changedItem.Paths.Path &&
                       originalItem.Language == changedItem.Language && originalItem.Version == changedItem.Version)
                {
                    isDataDifferentForOldAndNewItems = false;
                }
            }

            return isDataDifferentForOldAndNewItems;
        }

        private AuditLog_Item PrepareAuditLog(Item originalItem, Item changedItem, ItemEventType itemEvent,
            DateTime logTime, List<KeyValuePair<string, string>> itemFieldsAndValues_Old,
            List<KeyValuePair<string, string>> itemFieldsAndValues_New, string itemEventAuditLabel, string annotations)
        {
            ItemInfo itemDataBeforeSave = null;
            ItemInfo itemDataAfterSave = null;

            itemDataBeforeSave = new ItemInfo
            {
                ItemId = originalItem.ID.Guid,
                ItemName = originalItem.Name,
                ItemPath = originalItem.Paths.Path,
                TemplateId = originalItem.TemplateID.Guid,
                ItemLanguage = originalItem.Language.Name,
                ItemVersion = originalItem.Version.Number,
                FieldData = itemFieldsAndValues_Old
            };

            itemDataAfterSave = new ItemInfo
            {
                ItemId = changedItem.ID.Guid,
                ItemName = changedItem.Name,
                ItemPath = changedItem.Paths.Path,
                TemplateId = changedItem.TemplateID.Guid,
                ItemLanguage = changedItem.Language.Name,
                ItemVersion = changedItem.Version.Number,
                FieldData = itemFieldsAndValues_New
            };

            AuditLog_Item ciai = new AuditLog_Item
            {
                ItemId = changedItem.ID.Guid,
                ItemName = changedItem.Name,
                ItemPath = changedItem.Paths.Path,
                TemplateId = changedItem.TemplateID.Guid,
                ItemLanguage = changedItem.Language.Name,
                ItemVersion = changedItem.Version.Number,
                Event = itemEventAuditLabel,
                ActionedBy = changedItem.Statistics.UpdatedBy,
                ItemDataBeforeSave = JsonConvert.SerializeObject(itemDataBeforeSave),
                ItemDataAfterSave = JsonConvert.SerializeObject(itemDataAfterSave),
                Comments = annotations,
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
                            .Where(f=> itemFieldsAndValues_Old.FirstOrDefault(x=>x.Key==f).Value != itemFieldsAndValues_New.FirstOrDefault(x => x.Key == f).Value)
                            .Select(pair => pair)
                            .ToList();
            //IEnumerable<string> differences1 = itemFieldsAndValues_Old
            //                .Where((pair, index) => pair.Value != itemFieldsAndValues_New[index].Value)
            //                .Select(pair => pair.Key)
            //                .ToList();

            if (differences.Any())
            {
                changeLog = String.Format("Content changed in these fields [{0}]", differences.Aggregate((s1, s2) => s1 + ", " + s2));
            }

            return changeLog;
        }
    }
}