using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShipleySwine
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Breed",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Boars", action = "SelectBoar", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BredGiltId",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "BredGilts", action = "Email", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Authentication",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Authentication", action = "Login", id = UrlParameter.Optional }
            );

        }
    }
}
