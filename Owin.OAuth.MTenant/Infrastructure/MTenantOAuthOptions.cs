using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.OAuth.MTenant.Infrastructure
{
    public class MTenantOAuthOptions : AuthenticationOptions, IMTenantOAuthOptions
    {
        public MTenantOAuthOptions(string authenticationType) : base(authenticationType)
        {
        }



        public virtual string ClientId { get; }
        public virtual string ClientSecret { get; }
    }
}
