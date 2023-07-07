namespace Basiscore.CmsAudit.Handlers
{
    using Basiscore.CmsAudit.Services;
    using Sitecore.Data.Events;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Events;
    using System;
    using static Basiscore.CmsAudit.Constants;

    public class ItemEventHandler
    {
        private CmsAuditService caService = new CmsAuditService();

        /// <summary>
        /// item:created is followed by item:saving and item:versionAdded
        /// item:saving occurs twice. Because first, item is created only with name and template - that's then first set of events is triggered.
        /// then other item fields are set - that's when events are triggered again.
        /// So, when an item is created there will be 4 logs in the audit table - Item created, Ite saved, Ite saved & Item Version Added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnItemCreated(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, ItemEventType.ITEM_CREATED, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        /// item:saving event is executed before item is saved in the database.
        /// Item which is stored within event args is what will be saved to the database if none of the handlers abandons the process.
        /// And item which is in the database is the original one as nothing was saved to database yet. 
        /// Here I'm asking Sitecore to give me what is there still in the database, so that
        /// changed Item has all the new field values and originalItem has all the fields which are still in the database.
        protected void OnItemSaving(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, ItemEventType.ITEM_SAVED, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        protected void OnVersionRemoving(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, ItemEventType.ITEM_VERSION_REMOVED, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        protected void OnItemDeleted(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, ItemEventType.ITEM_DELETED, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        private void CommonEventHandler(object sender, EventArgs args, ItemEventType itemEvent, DateTime logTime)
        {
            if (caService.IsAuditLoggingEnabled)
            {
                Item originalItem = null;
                Item changedItem = null;
                string itemEventAuditLabel = string.Empty;

                if (itemEvent == ItemEventType.ITEM_CREATED)
                {
                    ItemCreatedEventArgs itemCreatedEventArgs = Event.ExtractParameter<ItemCreatedEventArgs>(args, 0);
                    changedItem = itemCreatedEventArgs?.Item;
                    originalItem = changedItem;
                    itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_CREATED;
                }                
                else
                {
                    changedItem = Event.ExtractParameter(args, 0) as Item;                    
                }

                if (changedItem == null || (!caService.ProceedToInsertItemAuditLog(changedItem)))
                {
                    return;
                }
                else
                {
                    switch (itemEvent)
                    {
                        case ItemEventType.ITEM_SAVED:
                            //originalItem = changedItem?.Database.GetItem(changedItem.ID, changedItem.Language, changedItem.Version);
                            originalItem = changedItem?.Database.GetItem(changedItem.ID, changedItem.Language);

                            if (originalItem.Name != changedItem.Name)
                            {
                                itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_RENAMED;
                            }
                            else if (originalItem.Paths.Path != changedItem.Paths.Path)
                            {
                                itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_MOVED;
                            }
                            else if (originalItem.TemplateID != changedItem.TemplateID)
                            {
                                itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_TEMPLATE_CHANGED;
                            }
                            else if (originalItem.Version != changedItem.Version)
                            {
                                itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_VERSION_ADDED;
                            }
                            else
                            {
                                itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_SAVED;
                            }
                            break;
                        case ItemEventType.ITEM_VERSION_REMOVED:
                            originalItem = changedItem;
                            itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_VERSION_REMOVED;
                            break;
                        case ItemEventType.ITEM_PUBLISHED:
                            originalItem = changedItem;
                            itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_PUBLISHED;
                            break;
                        case ItemEventType.ITEM_DELETED:
                            originalItem = changedItem;
                            itemEventAuditLabel = Constants.ItemEventAuditLabels.Item.ITEM_DELETED;
                            break;
                    }

                    caService.BuildItemAuditInfo(originalItem, changedItem, itemEvent, logTime, itemEventAuditLabel);
                }
            }
        }
    }
}