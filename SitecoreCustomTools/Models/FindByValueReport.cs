
namespace SitecoreCustomTools.Models
{
    using System.Collections.Generic;

    public class FindByValueReport
    {
        public string Error { get; set; }

        public int TaskStatus { get; set; }

        public List<ValueMatchedItem> LstValueMatchedItems { get; set; }
    }
}