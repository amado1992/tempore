// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpHealthCheckService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The http health check service.
    /// </summary>
    public class HttpHealthCheckService : HealthCheckServiceBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HttpHealthCheckService> logger;

        /// <summary>
        /// The health check url.
        /// </summary>
        private readonly string healthCheckUrl;

        /// <summary>
        /// The expected response.
        /// </summary>
        private readonly string expectedResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHealthCheckService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="healthCheckUrl">
        /// The health check url.
        /// </param>
        /// <param name="expectedResponse">
        /// The expected response.
        /// </param>
        public HttpHealthCheckService(ILogger<HttpHealthCheckService> logger, string healthCheckUrl, string expectedResponse = "")
        {
            this.logger = logger;
            this.healthCheckUrl = healthCheckUrl;
            this.expectedResponse = expectedResponse;
        }

        /// <inheritdoc />
        public override async Task<bool> IsHealthyAsync()
        {
            try
            {
                var httpMessageHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
                };

                using var httpClient = new HttpClient(httpMessageHandler);
                using var responseMessage = await httpClient.GetAsync(this.healthCheckUrl);

                responseMessage.EnsureSuccessStatusCode();

                if (string.IsNullOrWhiteSpace(this.expectedResponse))
                {
                    this.logger.LogInformation("Http-based service at '{Url}' endpoint is ready.", this.healthCheckUrl);

                    return true;
                }

                var content = await responseMessage.Content.ReadAsStringAsync();
                if (this.expectedResponse.Equals(content, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.logger.LogInformation("Http-based service at '{Url}' endpoint is ready.", this.healthCheckUrl);

                    return true;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogDebug(ex, "An error has thrown health checking of Http-based service at '{Url}' endpoint.", this.healthCheckUrl);
            }

            this.logger.LogWarning("Http-based service at '{0}' endpoint is not ready yet.", this.healthCheckUrl);

            return false;
        }
    }
}