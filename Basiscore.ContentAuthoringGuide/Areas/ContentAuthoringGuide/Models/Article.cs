
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models
{
    using Sitecore.Data.Items;
    using System.Collections.Generic;
    using System.Web.Mvc;


    public class Article : CustomItem
    {
        public Article(Item innerItem) : base(innerItem) { }

        public string Title
        {
            get
            {
                return InnerItem.Fields[Templates.Article.Fields.Title].Value;
            }
        }

        public MvcHtmlString Description
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.Article.Fields.Description].Value);
            }
        }

        public MvcHtmlString ReferenceURL
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.Article.Fields.ReferenceURL].Value);
            }
        }

        public List<CarouselSlide> Slides { get; set; }

        public string Section1Header
        {
            get
            {
                return InnerItem.Fields[Templates.Article.Fields.Section1Header].Value;
            }
        }

        public MvcHtmlString Section1Description
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.Article.Fields.Section1Description].Value);
            }
        }

        public string Section2Header
        {
            get
            {
                return InnerItem.Fields[Templates.Article.Fields.Section2Header].Value;
            }
        }

        public MvcHtmlString Section2Description
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.Article.Fields.Section2Description].Value);
            }
        }

        public string Section3Header
        {
            get
            {
                return InnerItem.Fields[Templates.Article.Fields.Section3Header].Value;
            }
        }

        public MvcHtmlString Section3Description
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.Article.Fields.Section3Description].Value);
            }
        }

        public string Section4Header
        {
            get
            {
                return InnerItem.Fields[Templates.Article.Fields.Section4Header].Value;
            }
        }

        public MvcHtmlString Section4Description
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.Article.Fields.Section4Description].Value);
            }
        }

        public string Section5Header
        {
            get
            {
                return InnerItem.Fields[Templates.Article.Fields.Section5Header].Value;
            }
        }

        public MvcHtmlString Section5Description
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.Article.Fields.Section5Description].Value);
            }
        }

        public bool IsActive
        {
            get
            {
                return InnerItem.Fields[Templates.Article.Fields.IsActive].Value == "1";
            }
        }
    }

}
