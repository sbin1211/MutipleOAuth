using MutipleOAuth.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace MutipleOAuth.Infrastructure
{
    public class MustBeTenant : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var tenantVal = values[parameterName].ToString().ToLower();
            var db = httpContext.GetOwinContext().Get<AppDbContext>();
            return db.OAuthApps.FirstOrDefault(x => x.Tenant.ToLower() == tenantVal) != null;


        }
    }
}