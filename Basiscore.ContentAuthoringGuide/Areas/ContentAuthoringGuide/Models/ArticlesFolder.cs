
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models
{
    using Sitecore.Data.Items;
    using System.Collections.Generic;
    
    public class ArticlesFolder : CustomItem
    {
        public ArticlesFolder(Item innerItem) : base(innerItem) { }

        public string Title
        {
            get
            {
                return InnerItem.Fields[Templates.ArticlesFolder.Fields.Title].Value;
            }
        }

        public bool IsActive
        {
            get
            {
                return InnerItem.Fields[Templates.ArticlesFolder.Fields.IsActive].Value == "1";
            }
        }

        public List<Article> Articles { get; set; }
    }

}
