
namespace Basiscore.ContentAuthoringGuide
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            ///To load default page when the url is '<instance>/ContentAuthoringGuide'
            routes.MapRoute(
                "ContentAuthoringGuide_Home",
                "ContentAuthoringGuide/{id}",
                new { controller = "ContentAuthoringGuide", action = "Index", area = "ContentAuthoringGuide", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "ContentAuthoringGuide_default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
