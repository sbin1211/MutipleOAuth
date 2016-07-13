// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

// OpenID is obsolete
#pragma warning disable 618

using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Google;

namespace Owin.OAuth.MTenant.Google
{
    public static class GoogleMTenantAuthenticationExtensions
    {

        public static IAppBuilder UseGoogleMTenantAuthentication(this IAppBuilder app, GoogleOAuth2AuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(GoogleMTenantOAuthMiddleware), app, options);
            return app;
        }
    }
}