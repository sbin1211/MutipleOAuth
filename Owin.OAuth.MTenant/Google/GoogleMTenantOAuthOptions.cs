
using Microsoft.Owin;
using Owin.OAuth.MTenant.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Owin.Security;
namespace Owin.OAuth.MTenant.Google
{

    public class GoogleMTenantOAuthOptions : MTenantOAuthOptions
    {

        public GoogleMTenantOAuthOptions()
            : base(Constants.DefaultAuthenticationType)
        {
            Caption = Constants.DefaultAuthenticationType;
            CallbackPath = new PathString("/signin-google");
            AuthenticationMode = AuthenticationMode.Passive;
            Scope = new List<string>();
            BackchannelTimeout = TimeSpan.FromSeconds(60);

            
        }


        public ICertificateValidator BackchannelCertificateValidator { get; set; }

        public TimeSpan BackchannelTimeout { get; set; }


        public HttpMessageHandler BackchannelHttpHandler { get; set; }


        public string Caption
        {
            get { return Description.Caption; }
            set { Description.Caption = value; }
        }


        public PathString CallbackPath { get; set; }


        public string SignInAsAuthenticationType { get; set; }


        public IGoogleMTenantOAuthProvider Provider { get; set; }


        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }


        public IList<string> Scope { get; private set; }


        public string AccessType { get; set; }

        public override string ClientId
        {
            get
            {
                if (string.IsNullOrEmpty(base.ClientId))
                {
                    return OnGetClientId()
                }
                else
                    return base.ClientId;
            }

            set
            {
                base.ClientId = value;
            }
        }
        public Func<AuthenticationProperties, string> OnGetClientId { get; set; }
        public Func<AuthenticationProperties, string> OnGetClientSecrect { get; set; }
    }
}
