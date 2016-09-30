using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MutipleOAuth.Data
{
    public class OAuthApp :BaseEntity
    {
        public string ClientKey { get; set; }
        public string ClientSecrect { get; set; }
        public string Tenant { get; set; }
        public string Provider { get; set; }
    }
}