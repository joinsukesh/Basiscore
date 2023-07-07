namespace Basiscore.CmsAudit
{
    using Sitecore.Data;

    public class Constants
    {
        public static readonly string ModuleName = "Basiscore.CmsAudit";
        public static readonly char Comma = ',';
        public static readonly string None = "none";
        public static readonly string True = "true";
        public static readonly string DoubleUnderscore = "__";

        public struct Sessions
        {
            public static readonly string AUDIT_ITEM_TEMPLATES = "AUDIT_ITEM_TEMPLATES";
            public static readonly string AUDIT_STANDARD_FIELDS = "AUDIT_STANDARD_FIELDS";
        }

        public enum ItemEventType
        {
            ITEM_CREATED,
            ITEM_RENAMED,
            ITEM_SAVING,
            ITEM_SAVED,
            ITEM_VERSION_ADDING,
            ITEM_VERSION_ADDED,
            ITEM_VERSION_REMOVED,
            ITEM_TEMPLATE_CHANGED,
            ITEM_MOVED,
            ITEM_PUBLISHED,
            ITEM_DELETED
        }

        public struct ItemEventAuditLabels
        {
            public struct Item
            {
                public static readonly string ITEM_CREATED = "Item Created";
                public static readonly string ITEM_RENAMED = "Item Renamed";
                //public static readonly string ITEM_SAVING = "Item Saved";
                public static readonly string ITEM_SAVED = "Item Saved";
                public static readonly string ITEM_VERSION_ADDED = "Item Version Added";
                public static readonly string ITEM_VERSION_REMOVED = "Item Version Removed";
                public static readonly string ITEM_TEMPLATE_CHANGED = "Item Template Changed";
                public static readonly string ITEM_MOVED = "Item Moved";
                public static readonly string ITEM_PUBLISHED = "Item Published";
                public static readonly string ITEM_DELETED = "Item Deleted";
            }
        }

        public struct ItemIds
        {
            /// <summary>
            /// /sitecore/system/Settings/Email/Instance Tasks/Content Management Primary/Message Statistics/Today
            /// </summary>
            public static ID Today => new ID("{A6599689-3616-4938-A5BB-9EC65480D2F3}");

            /// <summary>
            /// /sitecore/system/Settings/Basiscore/CMS Audit/CMS URL
            /// </summary>
            public static ID CMS_URL => new ID("{4D992AC5-05BB-4703-B3FB-C7B204A74DB7}");
        }
    }
}