
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace Owin.OAuth.MTenant.Google
{
   
    public class GoogleMTenantOAuthReturnEndpointContext : ReturnEndpointContext
    {
       
        public GoogleMTenantOAuthReturnEndpointContext(
            IOwinContext context,
            AuthenticationTicket ticket)
            : base(context, ticket)
        {
        }
    }
}
