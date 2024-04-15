// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpHealthCheckServiceFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using Tempore.HealthChecks.Services.Interfaces;

    /// <summary>
    /// The http health check service factory.
    /// </summary>
    public class HttpHealthCheckServiceFactory : IHttpHealthCheckServiceFactory
    {
        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHealthCheckServiceFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public HttpHealthCheckServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates an instance of <see cref="HttpHealthCheckService"/>.
        /// </summary>
        /// <param name="healthCheckUrl">
        /// The healthCheckUrl.
        /// </param>
        /// <param name="expectedResponse">
        /// The expected response.
        /// </param>
        /// <returns>
        /// The <see cref="IHealthCheckService"/>.
        /// </returns>
        public IHealthCheckService Create(string healthCheckUrl, string expectedResponse = "")
        {
            return ActivatorUtilities.CreateInstance<HttpHealthCheckService>(this.serviceProvider, healthCheckUrl, string.Empty);
        }
    }
}