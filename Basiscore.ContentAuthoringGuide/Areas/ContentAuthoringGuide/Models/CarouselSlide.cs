
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models
{
    using Sitecore.Data.Items;
    using System.Web.Mvc;
    using Utilities;

    public class CarouselSlide : CustomItem
    {
        public CarouselSlide(Item innerItem) : base(innerItem) { }

        public string Image
        {
            get
            {
                return SitecoreUtility.GetMediaAshxUrl(InnerItem, Templates.CarouselSlide.Fields.Image);
            }
        }

        public MvcHtmlString Description
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.CarouselSlide.Fields.Description].Value);
            }
        }

        public bool IsActive
        {
            get
            {
                return InnerItem.Fields[Templates.CarouselSlide.Fields.IsActive].Value == "1";
            }
        }
    }

}
