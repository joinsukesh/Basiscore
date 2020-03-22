
namespace SitecoreCustomTools.Models
{
    using Sitecore.Globalization;
    using SitecoreCustomTools.Utilities;
    using System.Collections.Generic;

    public class FindByValueDataModel
    {
        public int TaskId { get; set; }

        public string ParentItemId { get; set; }

        public string TargetTemplateId { get; set; }

        public string Keyword { get; set; }

        public int MatchCondition { get; set; }

        public string ReplaceValue { get; set; }

        public string CommaSeparatedLanguageCodes { get; set; }

        public List<Language> SelectedLanguages
        {
            get
            {
                return SctHelper.GetTargetLanguages(CommaSeparatedLanguageCodes);
            }
        }
    }
}