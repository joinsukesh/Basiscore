
namespace Basiscore.Minions.Models
{
    using Basiscore.Minions.Utilities;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System.Collections.Generic;

    public class ImportExcelDataItemInfo
    {
        public ImportExcelDataItemInfo(string itemName, string itemTemplate, string parentItem)
        {
            ITEM_NAME = itemName;
            ITEM_TEMPLATE = itemTemplate;
            PARENT_ITEM = parentItem;
        }

        public string ITEM_NAME { get; set; }

        public string ItemName
        {
            get
            {
                return !string.IsNullOrEmpty(this.ITEM_NAME) ? this.ITEM_NAME.Trim() : string.Empty;
            }
        }

        public string ITEM_TEMPLATE { get; set; }

        public TemplateItem ItemTemplate
        {
            get
            {
                TemplateItem templateItem = null;

                if (!string.IsNullOrEmpty(this.ITEM_TEMPLATE))
                {
                    Item item = MinionHelper.GetItem(this.ITEM_TEMPLATE);

                    if (item != null)
                    {
                        templateItem = MinionHelper.GetTemplateItem(item.Paths.FullPath);
                    }
                }

                return templateItem;
            }

        }

        public BranchItem BranchItem
        {
            get
            {
                BranchItem branchItem = null;

                if (this.ItemTemplate == null && !string.IsNullOrEmpty(this.ITEM_TEMPLATE))
                {
                    Item item = MinionHelper.GetItem(this.ITEM_TEMPLATE);

                    if (item != null && item.Paths.FullPath.ToLower().StartsWith(MinionConstants.Paths.BranchTemplate))
                    {
                        branchItem = item;
                    }
                }

                return branchItem;
            }
        }

        public string PARENT_ITEM { get; set; }

        public Item ParentItem
        {
            get
            {
                return MinionHelper.GetItem(this.PARENT_ITEM, "");
            }
        }

        public bool ItemToCreateExists
        {
            get
            {
                return this.ParentItem != null && MinionHelper.GetItem(this.ParentItem.Paths.FullPath + "/" + this.ItemName) != null;
            }
        }

        public Item CurrentItem
        {
            get
            {
                return this.ParentItem != null ? MinionHelper.GetItem(this.ParentItem.Paths.FullPath + "/" + this.ItemName) : null;
            }
        }

        public List<KeyValuePair<string, string>> FieldValueCollection { get; set; }

        public int ExcelSheetRowNumber { get; set; }

        public string FailureReason { get; set; }

    }
}