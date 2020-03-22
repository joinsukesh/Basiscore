
namespace SitecoreCustomTools.Models
{
    using System.Collections.Generic;

    public class ValueMatchedItem
    {
        public string ItemPath { get; set; }

        public string LanguageCode { get; set; }

        public List<string> Fields { get; set; }

        public string MatchLog { get; set; }
    }
}