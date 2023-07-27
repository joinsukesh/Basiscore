namespace Basiscore.CmsAudit
{
    using Sitecore.Data;

    public class Constants
    {
        public static readonly string ModuleName = "Basiscore.CmsAudit";
        public static readonly char Comma = ',';
        public static readonly string ForwardSlash = "/";
        public static readonly string Space = " ";
        public static readonly string One = "1";
        public static readonly string None = "none";
        public static readonly string True = "true";
        public static readonly string DoubleUnderscore = "__";
        public static readonly string Hash = "#";
        public static readonly string GenericErrorMessage = "A problem occurred while processing your equest. Please try again later.";
        public static readonly string LoginPagePath = "~/sitecore/login";

        public struct Sessions
        {
            public static readonly string AUDIT_ITEM_TEMPLATES = "AUDIT_ITEM_TEMPLATES";
            public static readonly string AUDIT_STANDARD_FIELDS = "AUDIT_STANDARD_FIELDS";
            public static readonly string AUDIT_DB_NAMES = "AUDIT_DB_NAMES";
        }

        public enum ItemEventType
        {
            ITEM_CREATED,            
            ITEM_VERSION_REMOVED,
            ITEM_MOVED,
            ITEM_COPIED,
            ITEM_CLONE_ADDED,
            ITEM_DELETED,
            ITEM_SAVED,
            ITEM_PUBLISHED,
            SITE_PUBLISHED
        }

        public enum UserEventType
        {
            USER_CREATED,
            USER_UPDATED,
            USER_DELETED,
            ROLES_ADDED_FOR_USER,
            ROLES_REMOVED_FOR_USER
        }

        public struct EventAuditLabels
        {
            public struct Item
            {
                public static readonly string ITEM_CREATED = "Item Created";
                public static readonly string ITEM_RENAMED = "Item Renamed";
                public static readonly string ITEM_VERSION_ADDED = "Item Version Added";
                public static readonly string ITEM_VERSION_REMOVED = "Item Version Removed";
                public static readonly string ITEM_MOVED = "Item Moved";
                public static readonly string ITEM_COPIED = "Item Copied";
                public static readonly string ITEM_CLONE_ADDED = "Item Cloned";
                public static readonly string ITEM_DELETED = "Item Deleted";
                public static readonly string ITEM_SAVED = "Item Saved";                
                public static readonly string ITEM_TEMPLATE_CHANGED = "Item Template Changed";                
                public static readonly string ITEM_PUBLISHED = "Item Published";
                public static readonly string SITE_PUBLISHED = "Site Published";                
            }

            public struct User
            {
                public static readonly string USER_CREATED = "User Created";
                public static readonly string USER_UPDATED = "User Updated";
                public static readonly string USER_DELETED = "User Deleted";
                public static readonly string ROLES_ADDED_FOR_USER = "Role(s) Added for User";
                public static readonly string ROLES_REMOVED_FOR_USER = "Role(s) Removed for User";
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

        public struct ColumnNames
        {
            public static readonly string Row_Id = "Row_Id";
            public static readonly string Item_Info = "Item_Info";
            public static readonly string Event = "Event";
            public static readonly string Actioned_By = "Actioned_By";
            public static readonly string Item_Content = "Item_Content";
            public static readonly string Comments = "Comments";
            public static readonly string Logged_Time = "Logged_Time";
            public static readonly string FirstLogDate = "FirstLogDate";
            public static readonly string RecentLogDate = "RecentLogDate";
            public static readonly string TotalRows = "TotalRows";
        }
    }
}