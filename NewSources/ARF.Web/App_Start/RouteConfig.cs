using System.Web.Mvc;
using System.Web.Routing;

namespace ARF.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "GetFileDefault",
                url: "f/{id}",
                defaults: new { controller = "GetFile", action = "Index", shortUrl = UrlParameter.Optional }
          );

            routes.MapRoute(
                  name: "Default1",
                  url: "l/{key}",
                  defaults: new { controller = "TempLinks", action = "Index", shortUrl = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "FakeAPI",
                url: "FakeAPI/{controller}/{action}/{nin}",
                defaults: new { controller = "Gosi", action = "Index", nin = UrlParameter.Optional }
            );
        }
    }
}