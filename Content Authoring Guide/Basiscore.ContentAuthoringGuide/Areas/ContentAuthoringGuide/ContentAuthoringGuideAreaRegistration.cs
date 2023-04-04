using System.Web.Mvc;

namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide
{
    public class ContentAuthoringGuideAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ContentAuthoringGuide";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //to load default page when the url is '<instance>/ContentAuthoringGuide
            context.MapRoute(
                "ContentAuthoringGuide_Home",
                "ContentAuthoringGuide/{id}",
                new { controller = "ContentAuthoringGuide", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "ContentAuthoringGuide_default",
                "ContentAuthoringGuide/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );             
        }
    }
}