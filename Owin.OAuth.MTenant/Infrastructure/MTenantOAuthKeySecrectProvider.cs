using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.OAuth.MTenant.Infrastructure
{
    public class MTenantOAuthKeySecrectProvider : IMTenantOAuthKeySecrectProvider
    {
        public MTenantOAuthKeySecrectProvider()
        {
            OnGetOauthClientKeySecrect = (x, y) => Task.FromResult(new Tuple<string, string>("", ""));
        }
        Task<Tuple<string, string>> IMTenantOAuthKeySecrectProvider.GetOAuthClientKeySecrect(string tenant, string provider)
        {
            return OnGetOauthClientKeySecrect(tenant, provider);
        }

        public virtual Func<string, string, Task<Tuple<string, string>>> OnGetOauthClientKeySecrect { get; set; }
    }
}
