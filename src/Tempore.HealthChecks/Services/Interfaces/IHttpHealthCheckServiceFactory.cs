// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHttpHealthCheckServiceFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services.Interfaces
{
    /// <summary>
    /// The HttpHealthCheckServiceFactory interface.
    /// </summary>
    public interface IHttpHealthCheckServiceFactory
    {
        /// <summary>
        /// Create http health check service.
        /// </summary>
        /// <param name="healthCheckUrl">
        /// The health check url.
        /// </param>
        /// <param name="expectedResponse">
        /// The expected response.
        /// </param>
        /// <returns>
        /// The <see cref="IHealthCheckService"/>.
        /// </returns>
        IHealthCheckService Create(string healthCheckUrl, string expectedResponse = "");
    }
}