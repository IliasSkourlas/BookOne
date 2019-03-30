using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookOne
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
            name: "ChatRoom",
            url: "chat",
            defaults: new { controller = "Chat", action = "Index" });

            routes.MapRoute(
            name: "SendMessage",
            url: "send_message",
            defaults: new { controller = "Chat", action = "SendMessage" });

        }
    }
}
