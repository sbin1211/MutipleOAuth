using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MutipleOAuth.Data
{
    public class OAuthApp :BaseEntity
    {
        public string AppId { get; set; }
        public string ClientSecrect { get; set; }
        public string StoreKey { get; set; }
        public string Provider { get; set; }
    }
}