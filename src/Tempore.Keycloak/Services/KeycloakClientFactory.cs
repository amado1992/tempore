// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeycloakClientFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Keycloak.Services
{
    using Flurl.Http;

    using global::Keycloak.Net;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Tempore.HealthChecks.Services.Interfaces;
    using Tempore.Hosting.Services.Interfaces;
    using Tempore.Infrastructure.Keycloak.Services;
    using Tempore.Infrastructure.Keycloak.Services.Interfaces;
    using Tempore.Keycloak.Extensions;

    /// <summary>
    /// The keycloak client factory.
    /// </summary>
    public class KeycloakClientFactory : IKeycloakClientFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<KeycloakClientFactory> logger;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The http health check service factory.
        /// </summary>
        private readonly IHttpHealthCheckServiceFactory httpHealthCheckServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeycloakClientFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="serviceDiscovery">
        /// The service discovery.
        /// </param>
        /// <param name="httpHealthCheckServiceFactory">
        /// The http Health Check Service Factory.
        /// </param>
        public KeycloakClientFactory(ILogger<KeycloakClientFactory> logger, IConfiguration configuration, IServiceDiscovery serviceDiscovery, IHttpHealthCheckServiceFactory httpHealthCheckServiceFactory)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.httpHealthCheckServiceFactory = httpHealthCheckServiceFactory;
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// When some required configuration parameters are missing.
        /// </exception>
        public async Task<(KeycloakClient KeycloakClient, string RealmName)> CreateAsync()
        {
            var identityServerAuthority = this.configuration.GetSection("IdentityServer")?["Authority"];
            if (string.IsNullOrWhiteSpace(identityServerAuthority) || !Uri.TryCreate(identityServerAuthority, UriKind.Absolute, out var identityServerAuthorityUri))
            {
                throw new ArgumentException("The expected Keycloak Authority configuration value is empty or wrong formatted.");
            }

            var identityServerUsername = this.configuration.GetSection("IdentityServer")?["Username"];
            if (string.IsNullOrWhiteSpace(identityServerUsername))
            {
                throw new ArgumentException("The expected Keycloak Username configuration value is empty.");
            }

            var identityServerPassword = this.configuration.GetSection("IdentityServer")?["Password"];
            if (string.IsNullOrWhiteSpace(identityServerPassword))
            {
                throw new ArgumentException("The expected Keycloak Password configuration value is empty.");
            }

            var keycloakWebApiUrl = identityServerAuthorityUri.GetRootUrl();
            var keycloakAbsolutePathParts = identityServerAuthorityUri?.AbsolutePath?.Split("/");
            if (keycloakAbsolutePathParts is null || keycloakAbsolutePathParts.Length != 4)
            {
                throw new ArgumentException("Keycloak url is wrong formatted.");
            }

            var realmName = keycloakAbsolutePathParts[3];
            if (string.IsNullOrWhiteSpace(realmName))
            {
                throw new ArgumentException("Can't determine Keycloak realm from the url.");
            }

            var httpHealthCheckService = this.httpHealthCheckServiceFactory.Create(keycloakWebApiUrl.TrimEnd('/') + "/auth");
            await httpHealthCheckService.WaitAsync();

            FlurlHttp.ConfigureClient(
                keycloakWebApiUrl,
                client =>
                {
                    var allowUntrustedCertificatesStringValue = this.configuration.GetSection("IdentityServer")?["AllowUntrustedCertificates"];
                    if (!string.IsNullOrWhiteSpace(allowUntrustedCertificatesStringValue) && bool.TryParse(allowUntrustedCertificatesStringValue, out var allowUntrustedCertificates) && allowUntrustedCertificates)
                    {
                        this.logger.LogInformation("Allowing untrusted certificates for the Keycloak client");

                        client.Settings.HttpClientFactory = new UntrustedCertClientFactory();
                    }
                });

            return (new KeycloakClient(keycloakWebApiUrl, identityServerUsername, identityServerPassword), realmName);
        }
    }
}