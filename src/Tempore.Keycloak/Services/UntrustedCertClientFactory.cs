// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UntrustedCertClientFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Infrastructure.Keycloak.Services
{
    using Flurl.Http.Configuration;

    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        /// <inheritdoc/>
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true,
            };
        }
    }
}