// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthCheckService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Tempore.App.Services.Interfaces;

    /// <summary>
    /// The health check service.
    /// </summary>
    public class HealthCheckService : IHealthCheckService
    {
        /// <summary>
        /// The http client.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckService"/> class.
        /// </summary>
        /// <param name="httpClient">
        /// The http client.
        /// </param>
        public HealthCheckService(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient);

            this.httpClient = httpClient;
        }

        /// <summary>
        /// Determines whether the system is healthy.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> IsHealthyAsync()
        {
            var isHealthy = true;

            try
            {
                using var responseMessage = await this.httpClient.GetAsync("/health");
                responseMessage.EnsureSuccessStatusCode();
            }
            catch
            {
                isHealthy = false;
            }

            return isHealthy;
        }
    }
}