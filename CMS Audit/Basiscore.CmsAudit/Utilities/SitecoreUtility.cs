namespace Basiscore.CmsAudit.Utilities
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.SecurityModel;

    public class SitecoreUtility
    {
        public static Item GetItem(string itemPathOrId, string database = "")
        {
            Item item = null;

            if (!string.IsNullOrWhiteSpace(itemPathOrId))
            {
                Database db = string.IsNullOrWhiteSpace(database) ? Sitecore.Context.Database : Factory.GetDatabase(database);

                if (db != null)
                {
                    using (new SecurityDisabler())
                    {
                        item = db.GetItem(itemPathOrId);
                    }
                }
            }

            return item;
        }
    }
}