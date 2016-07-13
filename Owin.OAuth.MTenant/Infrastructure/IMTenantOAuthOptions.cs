using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.OAuth.MTenant.Infrastructure
{
    public interface IMTenantOAuthOptions
    {
        string ClientId { get; }
        string ClientSecret { get; }
    }
}
