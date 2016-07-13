// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Owin.OAuth.MTenant.Google
{

    public interface IGoogleMTenantOAuthProvider
    {

        Task Authenticated(GoogleMTenantOAuthAuthenticatedContext context);

        Task ReturnEndpoint(GoogleMTenantOAuthReturnEndpointContext context);

        void ApplyRedirect(GoogleMTenantOAuthApplyRedirectContent context);
    }
}
