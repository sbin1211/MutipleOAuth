// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Owin.OAuth.MTenant.Google
{

    public class GoogleMTenantOAuthProvider : IGoogleMTenantOAuthProvider
    {

        public GoogleMTenantOAuthProvider()
        {
            OnAuthenticated = context => Task.FromResult<object>(null);
            OnReturnEndpoint = context => Task.FromResult<object>(null);
            OnApplyRedirect = context =>
                context.Response.Redirect(context.RedirectUri);
        }

        public Func<GoogleMTenantOAuthAuthenticatedContext, Task> OnAuthenticated { get; set; }


        public Func<GoogleMTenantOAuthReturnEndpointContext, Task> OnReturnEndpoint { get; set; }


        public Action<GoogleMTenantOAuthApplyRedirectContent> OnApplyRedirect { get; set; }

        public virtual Task Authenticated(GoogleMTenantOAuthAuthenticatedContext context)
        {
            return OnAuthenticated(context);
        }


        public virtual Task ReturnEndpoint(GoogleMTenantOAuthReturnEndpointContext context)
        {
            return OnReturnEndpoint(context);
        }

        public virtual void ApplyRedirect(GoogleMTenantOAuthApplyRedirectContent context)
        {
            OnApplyRedirect(context);
        }


    }
}
