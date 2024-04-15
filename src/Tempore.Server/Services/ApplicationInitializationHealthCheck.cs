// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationInitializationHealthCheck.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The application initialization health check.
    /// </summary>
    public class ApplicationInitializationHealthCheck : IHealthCheck
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ApplicationInitializationHealthCheck> logger;

        /// <summary>
        /// The initialization service.
        /// </summary>
        private readonly IInitializationService applicationInitializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInitializationHealthCheck"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="applicationInitializationService">
        /// The application initialization service.
        /// </param>
        public ApplicationInitializationHealthCheck(ILogger<ApplicationInitializationHealthCheck> logger, IApplicationInitializationService applicationInitializationService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(applicationInitializationService);

            this.logger = logger;
            this.applicationInitializationService = applicationInitializationService;
        }

        /// <summary>
        /// The check health async.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.applicationInitializationService.IsInitialized ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
        }
    }
}