using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace Owin.OAuth.MTenant.Infrastructure
{
    public class MTenantOAuthKeySecrectProvider : IMTenantOAuthKeySecrectProvider
    {
        public MTenantOAuthKeySecrectProvider()
        {
            Enabled = true;
            OnGetOauthClientKeySecrect = (x, y) => Task.FromResult<Tuple<string, string>>(null);
        }
        Task<Tuple<string, string>> IMTenantOAuthKeySecrectProvider.GetOAuthClientKeySecrect(string tenant, string provider)
        {
            return OnGetOauthClientKeySecrect(tenant, provider);
        }

        public string GetTeant(AuthenticationProperties properties)
        {
            string value;
            var tenantVal = properties.Dictionary.TryGetValue(MTenantConstants.AuthenticationTenantKey, out value) ? value : null;

            return tenantVal;


        }

        public string GetProvider(AuthenticationProperties properties)
        {
            string value;
            var provider = properties.Dictionary.TryGetValue(MTenantConstants.AuthenticationProviderKey, out value) ? value : null;
            return provider;
        }

        public virtual Func<string, string, Task<Tuple<string, string>>> OnGetOauthClientKeySecrect { get; set; }

        public bool Enabled
        {
            get; set;
        }
    }
}
