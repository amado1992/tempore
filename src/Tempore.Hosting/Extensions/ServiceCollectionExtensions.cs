// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Hosting.Extensions
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Tempore.Hosting.Services;
    using Tempore.Hosting.Services.Interfaces;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the service discovery.
        /// </summary>
        /// <param name="serviceCollection">
        /// The service collection.
        /// </param>
        public static void AddServiceDiscovery(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IServiceDiscovery>>();
                var environmentVariableService = sp.GetRequiredService<IEnvironmentVariableService>();
                var configuration = sp.GetRequiredService<IConfiguration>();

                return ServiceDiscoveryFactory.GetServiceDiscovery(logger, environmentVariableService, configuration);
            });
        }
    }
}