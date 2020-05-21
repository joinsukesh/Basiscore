
namespace Basiscore.Minions.Models
{
    using Basiscore.Minions.Utilities;
    using Sitecore.Globalization;
    using System.Collections.Generic;
    using System.Linq;

    public class FindByValueDataModel
    {
        public int TaskId { get; set; }

        public string ParentItemId { get; set; }

        public string TargetTemplateId { get; set; }

        public string Keyword { get; set; }

        public int MatchCondition { get; set; }

        public string TargetFieldId { get; set; }

        public string ReplaceValue { get; set; }

        public bool CreateVersion { get; set; }

        public string CommaSeparatedLanguageCodes { get; set; }

        public List<Language> SelectedLanguages
        {
            get
            {
                return MinionHelper.GetTargetLanguages(CommaSeparatedLanguageCodes);
            }
        }

        public string TargetLanguageCode { get; set; }

        public Language SelectedLanguage
        {
            get
            {
                return MinionHelper.GetTargetLanguages(TargetLanguageCode).FirstOrDefault();
            }
        }

        public string TargetItemPaths { get; set; }

        public List<string> GetItemPaths()
        {
            List<string> lstItemPaths = new List<string>();
            string[] arrItemPaths = TargetItemPaths != null ? TargetItemPaths.Split('\n') : null;
            string validItemPath = "";

            if (arrItemPaths != null && arrItemPaths.Length > 0)
            {
                ///for each string remove trailing / and add only unique paths to the list.
                foreach (string itemPath in arrItemPaths)
                {
                    validItemPath = itemPath.Trim().TrimEnd('/');

                    if (!string.IsNullOrEmpty(validItemPath)
                        && !lstItemPaths.Any(x => x == itemPath))
                    {
                        lstItemPaths.Add(validItemPath);
                    }
                }
            }

            return lstItemPaths;
        }
    }
}