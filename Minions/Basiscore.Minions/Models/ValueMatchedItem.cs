
namespace Basiscore.Minions.Models
{
    using System.Collections.Generic;

    public class ValueMatchedItem
    {
        public string ItemId { get; set; }

        public string ItemPath { get; set; }

        public string LanguageCode { get; set; }

        public List<string> Fields { get; set; }

        public string MatchLog { get; set; }

        public string FieldNames { get; set; }

    }
}