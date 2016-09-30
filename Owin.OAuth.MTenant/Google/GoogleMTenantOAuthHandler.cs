﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin.OAuth.MTenant.Google;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Owin.OAuth.MTenant.Infrastructure;

namespace Owin.OAuth.MTenant.Google
{
    internal class GoogleMTenantOAuthHandler : AuthenticationHandler<GoogleOAuth2AuthenticationOptions>
    {
        private const string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";
        private const string UserInfoEndpoint = "https://www.googleapis.com/plus/v1/people/me";
        private const string AuthorizeEndpoint = "https://accounts.google.com/o/oauth2/auth";

        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IMTenantOAuthKeySecrectProvider _keySecrectProvider;

        public GoogleMTenantOAuthHandler(HttpClient httpClient, IMTenantOAuthKeySecrectProvider keySecrectProvider, ILogger logger)
        {
            _keySecrectProvider = keySecrectProvider;

            _httpClient = httpClient;
            _logger = logger;

        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;

            try
            {
                string code = null;
                string state = null;

                IReadableStringCollection query = Request.Query;
                IList<string> values = query.GetValues("code");
                if (values != null && values.Count == 1)
                {
                    code = values[0];
                }
                values = query.GetValues("state");
                if (values != null && values.Count == 1)
                {
                    state = values[0];
                }

                properties = Options.StateDataFormat.Unprotect(state);
                if (properties == null)
                {
                    return null;
                }

                // OAuth2 10.12 CSRF
                if (!ValidateCorrelationId(properties, _logger))
                {
                    return new AuthenticationTicket(null, properties);
                }

                string requestPrefix = Request.Scheme + "://" + Request.Host;
                string redirectUri = requestPrefix + Request.PathBase + Options.CallbackPath;

                // Build up the body for the token request
                var body = new List<KeyValuePair<string, string>>();
                body.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                body.Add(new KeyValuePair<string, string>("code", code));
                body.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));

                //give a change to the key/secrect provider to 

                if (_keySecrectProvider.Enabled)
                {
                    var tenantVal = _keySecrectProvider.GetTeant(properties);

                    var provider = _keySecrectProvider.GetProvider(properties);

                    var keySecrect = await _keySecrectProvider.GetOAuthClientKeySecrect(tenantVal, provider);
                    if (keySecrect != null)
                    {
                        body.Add(new KeyValuePair<string, string>("client_id", keySecrect.Item1));
                        body.Add(new KeyValuePair<string, string>("client_secret", keySecrect.Item2));
                    }
                    else
                    {
                        body.Add(new KeyValuePair<string, string>("client_id", Options.ClientId));
                        body.Add(new KeyValuePair<string, string>("client_secret", Options.ClientSecret));
                    }
                }
                else
                {
                    body.Add(new KeyValuePair<string, string>("client_id", Options.ClientId));
                    body.Add(new KeyValuePair<string, string>("client_secret", Options.ClientSecret));
                }

                // Request the token
                HttpResponseMessage tokenResponse =
                    await _httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(body));
                tokenResponse.EnsureSuccessStatusCode();
                string text = await tokenResponse.Content.ReadAsStringAsync();

                // Deserializes the token response
                JObject response = JObject.Parse(text);
                string accessToken = response.Value<string>("access_token");

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.WriteWarning("Access token was not found");
                    return new AuthenticationTicket(null, properties);
                }

                // Get the Google user
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage graphResponse = await _httpClient.SendAsync(request, Request.CallCancelled);
                graphResponse.EnsureSuccessStatusCode();
                text = await graphResponse.Content.ReadAsStringAsync();
                JObject user = JObject.Parse(text);

                var context = new GoogleOAuth2AuthenticatedContext(Context, user, response);
                context.Identity = new ClaimsIdentity(
                    Options.AuthenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                if (!string.IsNullOrEmpty(context.Id))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.Id,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.GivenName))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.GivenName, context.GivenName,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.FamilyName))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.Surname, context.FamilyName,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Name))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.Name, context.Name, ClaimValueTypes.String,
                        Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Email))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.Email, context.Email, ClaimValueTypes.String,
                        Options.AuthenticationType));
                }

                if (!string.IsNullOrEmpty(context.Profile))
                {
                    context.Identity.AddClaim(new Claim("urn:google:profile", context.Profile, ClaimValueTypes.String,
                        Options.AuthenticationType));
                }
                context.Properties = properties;

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                _logger.WriteError("Authentication failed", ex);
                return new AuthenticationTicket(null, properties);
            }
        }

        protected override async Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return;

            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge != null)
            {
                string baseUri =
                    Request.Scheme +
                    Uri.SchemeDelimiter +
                    Request.Host +
                    Request.PathBase;

                string currentUri =
                    baseUri +
                    Request.Path +
                    Request.QueryString;

                string redirectUri =
                    baseUri +
                    Options.CallbackPath;

                AuthenticationProperties properties = challenge.Properties;
                if (string.IsNullOrEmpty(properties.RedirectUri))
                {
                    properties.RedirectUri = currentUri;
                }

                // OAuth2 10.12 CSRF
                GenerateCorrelationId(properties);

                var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                queryStrings.Add("response_type", "code");

                //give a change to get the client id from the provider
                if (_keySecrectProvider.Enabled)
                {

                    var tenant = _keySecrectProvider.GetTeant(properties);

                    var provider = _keySecrectProvider.GetProvider(properties);

                    var keySecrect = await _keySecrectProvider.GetOAuthClientKeySecrect(tenant, provider);

                    if (keySecrect != null)
                    {
                        queryStrings.Add("client_id", keySecrect.Item1);
                    }
                    else
                    {
                        queryStrings.Add("client_id", Options.ClientId);
                    }
                }
                else
                    queryStrings.Add("client_id", Options.ClientId);

                queryStrings.Add("redirect_uri", redirectUri);

                // space separated
                string scope = string.Join(" ", Options.Scope);
                if (string.IsNullOrEmpty(scope))
                {
                    // Google OAuth 2.0 asks for non-empty scope. If user didn't set it, set default scope to 
                    // "openid profile email" to get basic user information.
                    scope = "openid profile email";
                }
                AddQueryString(queryStrings, properties, "scope", scope);

                AddQueryString(queryStrings, properties, "access_type", Options.AccessType);
                AddQueryString(queryStrings, properties, "approval_prompt");
                AddQueryString(queryStrings, properties, "login_hint");

                string state = Options.StateDataFormat.Protect(properties);
                queryStrings.Add("state", state);

                string authorizationEndpoint = WebUtilities.AddQueryString(AuthorizeEndpoint,
                    queryStrings);

                var redirectContext = new GoogleOAuth2ApplyRedirectContext(
                    Context, Options,
                    properties, authorizationEndpoint);
                Options.Provider.ApplyRedirect(redirectContext);
            }

            return;
        }

        public override async Task<bool> InvokeAsync()
        {
            return await InvokeReplyPathAsync();
        }

        private async Task<bool> InvokeReplyPathAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                // TODO: error responses

                AuthenticationTicket ticket = await AuthenticateAsync();
                if (ticket == null)
                {
                    _logger.WriteWarning("Invalid return state, unable to redirect.");
                    Response.StatusCode = 500;
                    return true;
                }

                var context = new GoogleOAuth2ReturnEndpointContext(Context, ticket);
                context.SignInAsAuthenticationType = Options.SignInAsAuthenticationType;
                context.RedirectUri = ticket.Properties.RedirectUri;

                await Options.Provider.ReturnEndpoint(context);

                if (context.SignInAsAuthenticationType != null &&
                    context.Identity != null)
                {
                    ClaimsIdentity grantIdentity = context.Identity;
                    if (!string.Equals(grantIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                    {
                        grantIdentity = new ClaimsIdentity(grantIdentity.Claims, context.SignInAsAuthenticationType, grantIdentity.NameClaimType, grantIdentity.RoleClaimType);
                    }
                    Context.Authentication.SignIn(context.Properties, grantIdentity);
                }

                if (!context.IsRequestCompleted && context.RedirectUri != null)
                {
                    string redirectUri = context.RedirectUri;
                    if (context.Identity == null)
                    {
                        redirectUri = WebUtilities.AddQueryString(redirectUri, "error", "access_denied");
                    }
                    Response.Redirect(redirectUri);
                    context.RequestCompleted();
                }

                return context.IsRequestCompleted;
            }
            return false;
        }

        private static void AddQueryString(IDictionary<string, string> queryStrings, AuthenticationProperties properties,
            string name, string defaultValue = null)
        {
            string value;
            if (!properties.Dictionary.TryGetValue(name, out value))
            {
                value = defaultValue;
            }
            else
            {
                properties.Dictionary.Remove(name);
            }

            if (value == null)
            {
                return;
            }

            queryStrings[name] = value;
        }
    }
}
