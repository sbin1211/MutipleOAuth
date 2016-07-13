// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Owin.OAuth.MTenant.Infrastructure;

namespace Owin.OAuth.MTenant.Google
{
    public class GoogleMTenantOAuthMiddleware : GoogleOAuth2AuthenticationMiddleware
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        private readonly IMTenantOAuthKeySecrectProvider _keySecrectProvider;

        public GoogleMTenantOAuthMiddleware(
            OwinMiddleware next,
            IAppBuilder app,
            GoogleOAuth2AuthenticationOptions options, IMTenantOAuthKeySecrectProvider keySecrectProvider) :
            base(next, app, ByPassOptionsException(options))

        {
            _keySecrectProvider = keySecrectProvider;
        }
        protected override AuthenticationHandler<GoogleOAuth2AuthenticationOptions> CreateHandler()
        {
            return new GoogleMTenantOAuthHandler(_httpClient, _keySecrectProvider, _logger);
        }


        private static GoogleOAuth2AuthenticationOptions ByPassOptionsException(GoogleOAuth2AuthenticationOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ClientId))
                options.ClientId = "0";

            if (string.IsNullOrWhiteSpace(options.ClientSecret))
                options.ClientSecret = "0";

            return options;
        }
    }
}
