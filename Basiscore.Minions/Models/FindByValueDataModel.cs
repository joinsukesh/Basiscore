
namespace Basiscore.Minions.Models
{
    using Sitecore.Globalization;
    using Basiscore.Minions.Utilities;
    using System.Collections.Generic;

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
    }
}