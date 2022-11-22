
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models
{
    using Sitecore.Data.Items;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Utilities;

    public class GuideRoot : CustomItem
    {
        public GuideRoot(Item innerItem) : base(innerItem) { }

        public List<MenuItem> MenuItems { get; set; }

        public List<Article> Articles { get; set; }

        public string TopLogo
        {
            get
            {
                return InnerItem != null ? SitecoreUtility.GetMediaAshxUrl(InnerItem, Templates.GuideRoot.Fields.TopLogo) : string.Empty;
            }
        }

        public MvcHtmlString TopLogoContent
        {
            get
            {
                return InnerItem != null ? new MvcHtmlString(InnerItem.Fields[Templates.GuideRoot.Fields.TopLogoContent].Value) : new MvcHtmlString(string.Empty);
            }
        }

        public string Title
        {
            get
            {
                return InnerItem != null ? InnerItem.Fields[Templates.GuideRoot.Fields.Title].Value : string.Empty;
            }
        }

        public string CoverPageImage
        {
            get
            {
                return InnerItem != null ? SitecoreUtility.GetMediaAshxUrl(InnerItem, Templates.GuideRoot.Fields.CoverPageImage) : string.Empty;
            }
        }

        public string BottomLogo
        {
            get
            {
                return InnerItem != null ? SitecoreUtility.GetMediaAshxUrl(InnerItem, Templates.GuideRoot.Fields.BottomLogo) : string.Empty;
            }
        }

        public MvcHtmlString BottomLogoContent
        {
            get
            {
                return InnerItem != null ? new MvcHtmlString(InnerItem.Fields[Templates.GuideRoot.Fields.BottomLogoContent].Value) : new MvcHtmlString(string.Empty);
            }
        }        
    }
}
