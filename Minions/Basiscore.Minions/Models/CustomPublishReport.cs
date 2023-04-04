
namespace Basiscore.Minions.Models
{
    using System.Collections.Generic;

    public class CustomPublishReport
    {
        public List<ItemPublishedStatus> LstItemPublishStatus { get; set; }

        public string Error { get; set; }

        public int PublishStatus { get; set; }

        public List<string> LstPackageItemPaths { get; set; }

        public int GetItemPathsStatus { get; set; }

    }
}