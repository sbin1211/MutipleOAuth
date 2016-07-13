
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace Owin.OAuth.MTenant.Google
{


    public class GoogleMTenantOAuthApplyRedirectContent : BaseContext<GoogleMTenantOAuthOptions>
    {


        public GoogleMTenantOAuthApplyRedirectContent(IOwinContext context, GoogleMTenantOAuthOptions options,
            AuthenticationProperties properties, string redirectUri)
            : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        public string RedirectUri { get; private set; }


        public AuthenticationProperties Properties { get; private set; }
    }
}
