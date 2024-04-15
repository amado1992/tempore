// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenResolver.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services
{
    using System.Text.Json;

    using Tempore.Agent.Services.Interfaces;

    /// <summary>
    /// The token resolver.
    /// </summary>
    public class TokenResolver : ITokenResolver
    {
        private readonly ILogger<TokenResolver> logger;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResolver"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public TokenResolver(ILogger<TokenResolver> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// The resolve async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<string> ResolveAsync()
        {
            var identityServerAuthorityUrl = this.configuration.GetSection("IdentityServer")["Authority"];
            var agentSecret = this.configuration.GetSection("IdentityServer")["AgentSecret"];

            this.logger.LogInformation("Resolving token from {Authority}", identityServerAuthorityUrl);

            using var httpClient = new HttpClient();

            // Read more data from configuration?
            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = "tempore-agent",
                ["grant_type"] = "client_credentials",
                ["scope"] = "openid profile tempore-api",
                ["client_secret"] = agentSecret, // This is a secret.
            };

            try
            {
                using var formUrlEncodedContent = new FormUrlEncodedContent(parameters);
                using var httpResponseMessage = await httpClient.PostAsync(
                                                    $"{identityServerAuthorityUrl}/protocol/openid-connect/token",
                                                    formUrlEncodedContent);

                httpResponseMessage.EnsureSuccessStatusCode();
                await using var stream = await httpResponseMessage.Content.ReadAsStreamAsync(CancellationToken.None);
                using var document = await JsonDocument.ParseAsync(stream);
                return document.RootElement.GetProperty("access_token").GetString()!;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex, "Error resolving token from {Authority}", identityServerAuthorityUrl);

                return string.Empty;
            }
        }
    }
}