using MutipleOAuth.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MutipleOAuth
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Tenant",
             url: "{tenant}/{controller}/{action}/{id}",
             defaults: new { controller = "Tenant", action = "Index", id = UrlParameter.Optional },
             constraints: new { tenant = new MustBeTenant(),controller="Tenant"}
         );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
