
namespace Basiscore.Minions.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class GeneratePackageDataModel
    {
        public string DatabaseName { get; set; }

        public string PackageName { get; set; }

        public string Author { get; set; }

        public string Version { get; set; }

        public string ItemPathsToIncludeWithSubitems { get; set; }

        public string ItemPathsToInclude { get; set; }

        public List<string> GetItemPathsToIncludeWithSubitems()
        {
            List<string> lstItemPathsToIncludeWithSubitems = new List<string>();
            string[] arrItemPaths = this.ItemPathsToIncludeWithSubitems != null ? ItemPathsToIncludeWithSubitems.Split('\n') : null;
            string validItemPath = "";

            if (arrItemPaths != null && arrItemPaths.Length > 0)
            {
                ///for each string remove trailing '/' and add only unique paths to the list.
                ///Also, check if there is a parent item & also its descendants. If yes, then exclude descendants
                foreach (string itemPath in arrItemPaths)
                {
                    validItemPath = itemPath.Trim().TrimEnd('/');

                    if (!string.IsNullOrEmpty(validItemPath) 
                        && !lstItemPathsToIncludeWithSubitems.Any(x => validItemPath.StartsWith(x + "/"))
                        && !lstItemPathsToIncludeWithSubitems.Any(x => x == itemPath))
                    {
                        lstItemPathsToIncludeWithSubitems.Add(validItemPath);
                    }
                }
            }

            return lstItemPathsToIncludeWithSubitems;
        }

        public List<string> GetItemPathsToInclude()
        {
            List<string> lstItemPathsToInclude = new List<string>();
            string[] arrItemPaths = ItemPathsToInclude != null ? ItemPathsToInclude.Split('\n') : null;
            string validItemPath = "";

            if (arrItemPaths != null && arrItemPaths.Length > 0)
            {
                ///for each string remove trailing '/' and add only unique paths to the list.
                ///If this path exists in 'ItemPathsToIncludeWithSubitems', then exclude it.
                ///If this item's parent exists in 'ItemPathsToIncludeWithSubitems', then exclude it.
                List<string> lstItemPathsToIncludeWithSubitems = GetItemPathsToIncludeWithSubitems();

                foreach (string itemPath in arrItemPaths)
                {
                    validItemPath = itemPath.Trim().TrimEnd('/');

                    if (!string.IsNullOrEmpty(validItemPath) 
                         && !lstItemPathsToInclude.Any(x => x == itemPath)
                         && !lstItemPathsToIncludeWithSubitems.Any(x => x == itemPath)
                         && !lstItemPathsToIncludeWithSubitems.Any(x => validItemPath.StartsWith(x + "/")))
                    {
                        lstItemPathsToInclude.Add(validItemPath);
                    }
                }
            }

            return lstItemPathsToInclude;
        }

        public int ResultStatus { get; set; }

        public string ResultMessage { get; set; }

    }
}