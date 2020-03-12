
namespace SitecoreCustomTools.Models
{
    using Sitecore.Globalization;
    using SitecoreCustomTools.Utilities;
    using System.Collections.Generic;
    using System.Linq;

    public class CustomPublishDataModel
    {
        public string ItemPathsToPublishWithSubitems { get; set; }

        public string ItemPathsToPublish { get; set; }

        public string CommaSeparatedLanguageCodes { get; set; }
        public string CommaSeparatedDatabaseNames { get; set; }

        public bool ExcludeItemsWithWorkflow { get; set; }

        public List<string> SelectedDatabaseNames
        {
            get
            {
                List<string> selectedDatabaseNames = new List<string>();
                string[] arrDbNames = CommaSeparatedDatabaseNames.Split(',');

                if (arrDbNames != null && arrDbNames.Length > 0)
                {
                    selectedDatabaseNames.AddRange(arrDbNames.Where(x => !string.IsNullOrEmpty(x.Trim())));
                }

                return selectedDatabaseNames;
            }
        }

        public List<Language> SelectedLanguages
        {
            get
            {
                return SctHelper.GetTargetLanguages(CommaSeparatedLanguageCodes);
            }
        }

        public List<string> GetItemPathsToPublishWithSubitems()
        {
            List<string> lstItemPathsToPublishWithSubitems = new List<string>();
            string[] arrItemPaths = this.ItemPathsToPublishWithSubitems != null ? ItemPathsToPublishWithSubitems.Split('\n') : null;
            string validItemPath = "";

            if (arrItemPaths != null && arrItemPaths.Length > 0)
            {
                ///for each string remove trailing '/' and add only unique paths to the list.
                ///Also, check if there is a parent item & also its descendants. If yes, then exclude descendants
                foreach (string itemPath in arrItemPaths)
                {
                    validItemPath = itemPath.Trim().TrimEnd('/');

                    if (!string.IsNullOrEmpty(validItemPath)
                        && !lstItemPathsToPublishWithSubitems.Any(x => validItemPath.StartsWith(x + "/"))
                        && !lstItemPathsToPublishWithSubitems.Any(x => x == itemPath))
                    {
                        lstItemPathsToPublishWithSubitems.Add(validItemPath);
                    }
                }                
            }

            return lstItemPathsToPublishWithSubitems;
        }

        public List<string> GetItemPathsToPublish()
        {
            List<string> lstItemPathsToPublish = new List<string>();
            string[] arrItemPaths = ItemPathsToPublish != null ? ItemPathsToPublish.Split('\n') : null;
            string validItemPath = "";

            if (arrItemPaths != null && arrItemPaths.Length > 0)
            {
                ///for each string remove trailing / and add only unique paths to the list.
                ///If this path exists in 'ItemPathsToPublishWithSubitems', then exclude it.
                ///If this item's parent exists in 'ItemPathsToPublishWithSubitems', then exclude it.
                List<string> lstItemPathsToPublishWithSubitems = GetItemPathsToPublishWithSubitems();

                foreach (string itemPath in arrItemPaths)
                {
                    validItemPath = itemPath.Trim().TrimEnd('/');

                    if (!string.IsNullOrEmpty(validItemPath) 
                        && !lstItemPathsToPublish.Any(x => x == itemPath)
                         && !lstItemPathsToPublishWithSubitems.Any(x => x == itemPath)
                         && !lstItemPathsToPublishWithSubitems.Any(x => validItemPath.StartsWith(x + "/")))
                    {
                        lstItemPathsToPublish.Add(validItemPath);
                    }
                }
            }

            return lstItemPathsToPublish;
        }
    }
}