﻿using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.OAuth.MTenant.Infrastructure
{
    public interface IMTenantOAuthKeySecrectProvider
    {
        Task<Tuple<string, string>> GetOAuthClientKeySecrect(string tenant, string provider);

        bool Enabled { get; set; }

        string GetTeant(AuthenticationProperties properties);
        string GetProvider(AuthenticationProperties properties);
    }
}
