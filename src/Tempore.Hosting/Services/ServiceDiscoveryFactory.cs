// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDiscoveryFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Hosting.Services
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Tempore.Hosting.Exceptions;
    using Tempore.Hosting.Services.Interfaces;
    using Tempore.Logging.Extensions;

    /// <summary>
    /// The service discovery factory.
    /// </summary>
    public static class ServiceDiscoveryFactory
    {
        /// <summary>
        /// The sync obj.
        /// </summary>
        private static readonly object SyncObj = new();

        /// <summary>
        /// The service discovery.
        /// </summary>
        private static IServiceDiscovery? serviceDiscovery;

        /// <summary>
        /// Gets service discovery.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="environmentVariableService">
        /// The environment variable service.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceDiscovery"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static IServiceDiscovery GetServiceDiscovery(
            ILogger<IServiceDiscovery> logger,
            IEnvironmentVariableService environmentVariableService,
            IConfiguration configuration)
        {
            lock (SyncObj)
            {
                if (serviceDiscovery is null)
                {
                    if (!string.IsNullOrWhiteSpace(environmentVariableService.GetValue("KUBERNETES_SERVICE_HOST"))
                        || !string.IsNullOrWhiteSpace(environmentVariableService.GetValue("APP_INSTANCE")))
                    {
                        logger.LogInformation("Creating Kubernetes Service Discovery");

                        serviceDiscovery = new KubernetesServiceDiscovery(
                            logger,
                            environmentVariableService,
                            configuration);
                    }
                    else
                    {
                        throw logger.LogErrorAndCreateException<TemporeException>(
                            "Only hosting in kubernetes is supported");
                    }
                }
            }

            return serviceDiscovery;
        }

        public static IServiceDiscovery GetServiceDiscovery(IConfiguration configuration)
        {
            return GetServiceDiscovery(
                NullLogger<IServiceDiscovery>.Instance,
                new EnvironmentVariableService(NullLogger<EnvironmentVariableService>.Instance),
                configuration);
        }
    }
}