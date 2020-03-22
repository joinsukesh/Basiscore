

namespace SitecoreCustomTools.Utilities
{
    using Sitecore.Collections;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Globalization;
    using Sitecore.Publishing;
    using Sitecore.Publishing.Pipelines.Publish;
    using Sitecore.SecurityModel;
    using SitecoreCustomTools.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SctHelper
    {
        public struct Databases
        {
            public static Database masterDb = Factory.GetDatabase(SctConstants.DatabaseNames.Master);
        }

        public static Language DefaultLanguage
        {
            get
            {
                return LanguageManager.GetLanguages(Databases.masterDb)
                                .Where(x => x.Name.ToLower() == "en").FirstOrDefault();
            }
        }

        public static bool IsUserLoggedIn()
        {
            return Sitecore.Context.IsLoggedIn;
        }

        public static bool IsValidID(ID id)
        {
            return (!string.IsNullOrEmpty(System.Convert.ToString(id)) && !id.IsNull && !id.IsGlobalNullId);
        }

        /// <summary>
        /// checks if an item has a field
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool ItemHasField(Item contextItem, ID fieldID)
        {
            contextItem.Fields.ReadAll();
            return contextItem.Fields.Any(x => x.ID == fieldID);
        }

        public static Database GetDatabase(string databaseName)
        {
            return Factory.GetDatabase(databaseName);
        }

        public static Item GetItem(ID itemID, Language language = null, string databaseName = "")
        {
            Item item = null;
            Database db = string.IsNullOrEmpty(databaseName) ? GetDatabase(SctConstants.DatabaseNames.Master) : GetDatabase(databaseName);

            if (db != null)
            {
                if (language == null)
                {
                    language = DefaultLanguage;
                }

                if (language != null)
                {
                    using (new SecurityDisabler())
                    {
                        using (new LanguageSwitcher(language.Name))
                        {
                            item = db.GetItem(itemID, language);
                        }
                    }
                }
            }

            return item;
        }

        public static Item GetItem(string itemPath, Language language = null, string databaseName = "")
        {
            Item item = null;
            Database db = string.IsNullOrEmpty(databaseName) ? GetDatabase(SctConstants.DatabaseNames.Master) : GetDatabase(databaseName);

            if (db != null)
            {
                if (language == null)
                {
                    language = DefaultLanguage;
                }

                if (language != null)
                {
                    using (new SecurityDisabler())
                    {
                        using (new LanguageSwitcher(language.Name))
                        {
                            item = db.GetItem(itemPath, language);
                        }
                    }
                }
            }

            return item;
        }

        public static List<string> GetTargetDatabases()
        {
            List<string> lstTargetDatabases = null;
            Item publishingTargetsItem = GetItem(SctConstants.Items.PublishingTargets);

            if (publishingTargetsItem != null)
            {
                List<Item> lstPublishingTargets = GetItemsByTemplate(publishingTargetsItem, SctConstants.Templates.PublishingTarget.ID);

                if (lstPublishingTargets != null && lstPublishingTargets.Count > 0)
                {
                    lstTargetDatabases = new List<string>();
                    IEnumerable<string> allTargetDatabases = lstPublishingTargets
                        .Where(x => !string.IsNullOrEmpty(System.Convert.ToString(x.Fields[SctConstants.Templates.PublishingTarget.Fields.TargetDatabase].Value).Trim()))
                    .Select(x => System.Convert.ToString(x.Fields[SctConstants.Templates.PublishingTarget.Fields.TargetDatabase].Value).Trim());

                    if (allTargetDatabases != null && allTargetDatabases.Count() > 0)
                    {
                        lstTargetDatabases.AddRange(allTargetDatabases);
                    }
                }
            }

            return lstTargetDatabases;
        }

        public static List<KeyValuePair<string, string>> GetPublishingTargets()
        {
            List<KeyValuePair<string, string>> lstTargetDatabases = null;
            Item publishingTargetsItem = GetItem(SctConstants.Items.PublishingTargets);

            if (publishingTargetsItem != null)
            {
                List<Item> lstPublishingTargets = GetItemsByTemplate(publishingTargetsItem, SctConstants.Templates.PublishingTarget.ID);

                if (lstPublishingTargets != null && lstPublishingTargets.Count > 0)
                {
                    lstTargetDatabases = new List<KeyValuePair<string, string>>();
                    string dbName = "";

                    foreach (Item item in lstPublishingTargets)
                    {
                        dbName = System.Convert.ToString(item.Fields[SctConstants.Templates.PublishingTarget.Fields.TargetDatabase].Value).Trim();

                        if (!string.IsNullOrEmpty(dbName))
                        {
                            lstTargetDatabases.Add(new KeyValuePair<string, string>(dbName + " (" + item.Name + ")", dbName));
                        }
                    }
                }
            }

            return lstTargetDatabases;
        }

        /// <summary>
        /// get all the user selected target languages
        /// </summary>
        /// <param name="commaSeparatedLanguageCodes"></param>
        /// <returns></returns>
        public static List<Language> GetTargetLanguages(string commaSeparatedLanguageCodes)
        {
            List<Language> lstLanguages = new List<Language>();

            if (!string.IsNullOrEmpty(commaSeparatedLanguageCodes))
            {
                string[] targetLanguageCodes = commaSeparatedLanguageCodes.Split(',');

                if (targetLanguageCodes != null && targetLanguageCodes.Length > 0)
                {
                    Language language = null;

                    foreach (string languageCode in targetLanguageCodes)
                    {
                        if (!string.IsNullOrEmpty(languageCode))
                        {
                            language = LanguageManager.GetLanguages(Databases.masterDb)
                                .Where(x => x.Name.ToLower() == languageCode.ToLower()).FirstOrDefault();

                            if (language != null)
                            {
                                lstLanguages.Add(language);
                            }
                        }
                    }
                }
            }

            return lstLanguages;
        }

        /// <summary>
        /// get the list of installed languages
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetInstalledLanguages()
        {
            //Dictionary<string, string> installedLanguages = new Dictionary<string, string>();
            List<KeyValuePair<string, string>> lstInstalledLanguages = new List<KeyValuePair<string, string>>();
            LanguageCollection languageCollection = LanguageManager.GetLanguages(Databases.masterDb);

            if (languageCollection != null && languageCollection.Count > 0)
            {
                foreach (Language language in languageCollection)
                {
                    lstInstalledLanguages.Add(new KeyValuePair<string, string>(language.Name, language.CultureInfo.DisplayName));
                }
                //lstInstalledLanguages = languageCollection.ToList();
            }

            return lstInstalledLanguages;
        }

        public static PublishResult PublishItem(Item sourceItem, string targetDatabaseName, bool publishSubItems)
        {
            PublishResult publishResult = null;

            if (sourceItem != null)
            {
                Database targetDatabase = GetDatabase(targetDatabaseName);

                if (targetDatabase != null)
                {
                    /// Create a publisher with the publishoptions
                    /// The publishOptions determine the source and target database,
                    /// the publish mode and language, and the publish date
                    PublishOptions publishOptions = new PublishOptions(Databases.masterDb, targetDatabase,
                                                         PublishMode.SingleItem, sourceItem.Language,
                                                         System.DateTime.Now);

                    if (publishOptions != null)
                    {
                        publishOptions.UserName = Sitecore.Context.User.Name;
                        MyPublisher publisher = new MyPublisher(publishOptions);

                        if (publisher != null)
                        {
                            /// Choose where to publish from
                            publisher.Options.RootItem = sourceItem;

                            /// Publish children as well?
                            publisher.Options.Deep = publishSubItems;

                            /// Do the publish!
                            using (new SecurityDisabler())
                            {
                                //using (new EventDisabler())
                                //{
                                publishResult = publisher.PublishWithResult();
                                //}
                            }

                            sourceItem.Publishing.ClearPublishingCache();
                        }
                    }
                }
            }

            return publishResult;
        }

        public static void UpdateFieldValues(Item contextItem, List<KeyValuePair<ID, string>> listOfItemFieldsWithValues)
        {
            if (contextItem != null && listOfItemFieldsWithValues != null && listOfItemFieldsWithValues.Count > 0)
            {
                using (new SecurityDisabler())
                {
                    contextItem.Editing.BeginEdit();

                    foreach (KeyValuePair<ID, string> kvp in listOfItemFieldsWithValues)
                    {
                        if (ItemHasField(contextItem, kvp.Key))
                        {
                            contextItem[kvp.Key] = kvp.Value;
                        }
                    }

                    contextItem.Editing.EndEdit();
                }
            }
        }

        public static Item RemoveWorkflow(Item contextItem, Language language)
        {
            Item processedItem = null;

            if (contextItem != null)
            {
                ID itemId = contextItem.ID;
                string dbName = contextItem.Database.Name;

                using (new SecurityDisabler())
                {
                    contextItem.Editing.BeginEdit();
                    contextItem[SctConstants.Templates.Workflow.Fields.Workflow] = string.Empty;
                    contextItem.Editing.EndEdit();
                }

                processedItem = GetItem(contextItem.ID, language, dbName);
            }

            return processedItem;
        }

        public static bool HasWorkflow(Item contextItem)
        {
            bool hasWorkflow = false;
            hasWorkflow = contextItem != null &&
                contextItem[SctConstants.Templates.Workflow.Fields.Workflow] != null &&
                !string.IsNullOrEmpty(contextItem[SctConstants.Templates.Workflow.Fields.Workflow]);
            return hasWorkflow;
        }

        public static List<Item> GetItemsByTemplate(Item parentItem, ID templateID, bool checkBaseTemplates = false)
        {
            List<Item> itemsByTemplate = new List<Item>();

            if (parentItem != null && IsValidID(templateID))
            {
                List<ID> usages = new List<ID>();
                TemplateItem selectedTemplateItem = Databases.masterDb.GetItem(templateID);

                if (checkBaseTemplates)
                {
                    itemsByTemplate = parentItem.Axes.GetDescendants().Where(x => x.TemplateID == templateID ||
                    (x.Template != null && x.Template.BaseTemplates.Any(b => b.ID == templateID))).ToList();
                }
                else
                {
                    itemsByTemplate = parentItem.Axes.GetDescendants().Where(x => x.TemplateID == templateID).ToList();
                }
            }

            return itemsByTemplate;

        }

        public static bool IsValidName(string name)
        {
            bool isValidName = true;

            if (!string.IsNullOrEmpty(name))
            {
                char[] charsInName = name.ToCharArray();

                foreach (char character in charsInName)
                {
                    if (!SctConstants.ValidCharacters.Contains(character))
                    {
                        isValidName = false;
                        break;
                    }
                }
            }
            else
            {
                isValidName = false;
            }

            return isValidName;
        }

        public static string ReplaceFirstInstance(string source, string keyword, string replaceWith)
        {
            int keywordPosition = source.IndexOf(keyword, 0, StringComparison.CurrentCultureIgnoreCase);

            if (keywordPosition < 0)
            {
                return source;
            }

            string result = source.Substring(0, keywordPosition) + replaceWith + source.Substring(keywordPosition + keyword.Length);
            return result;
        }

        public static string ReplaceLastInstance(string source, string keyword, string replaceWith)
        {
            int keywordPosition = source.LastIndexOf(keyword, source.Length, StringComparison.CurrentCultureIgnoreCase);

            if (keywordPosition == -1)
                return source;

            string result = source.Remove(keywordPosition, keyword.Length).Insert(keywordPosition, replaceWith);
            return result;
        }
    }
}