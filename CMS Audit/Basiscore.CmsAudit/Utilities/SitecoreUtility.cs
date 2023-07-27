namespace Basiscore.CmsAudit.Utilities
{
    using Sitecore.Collections;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Globalization;
    using Sitecore.Security.Accounts;
    using Sitecore.SecurityModel;
    using System.Collections.Generic;
    using System.Linq;

    public class SitecoreUtility
    {
        public static bool IsAuthenticated()
        {
            return Sitecore.Context.User.IsAuthenticated;
        }

        public static bool IsAdministrator()
        {
            return IsAuthenticated() && Sitecore.Context.User.IsAdministrator;
        }

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

        public static User GetUserByUsername(string usernameWithDomain, bool isAuthenticated)
        {
            User user = User.FromName(usernameWithDomain, isAuthenticated);
            return user;
        }

        public static List<Language> GetSystemLanguages()
        {
            List<Language> languages = null;

            Database db = Factory.GetDatabase(Configurations.MasterDbName);

            if (db != null)
            {
                using (new SecurityDisabler())
                {
                    LanguageCollection languageCollection = LanguageManager.GetLanguages(db);

                    if (languageCollection != null)
                    {
                        languages = languageCollection.ToArray().ToList();
                    }
                }
            }

            return languages;
        }

    }
}