// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseHealthCheckService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services
{
    using System.Data.Common;

    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    using Tempore.HealthChecks.Services.Interfaces;
    using Tempore.Infrastructure.HealthChecks.Interfaces;

    /// <summary>
    /// The database health check service factory.
    /// </summary>
    public class DatabaseHealthCheckServiceFactory : IDatabaseHealthCheckServiceFactory
    {
        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHealthCheckServiceFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public DatabaseHealthCheckServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="databaseFacade">
        /// The database facade.
        /// </param>
        /// <returns>
        /// The <see cref="IDatabaseHealthCheckService"/>.
        /// </returns>
        public IDatabaseHealthCheckService Create(DatabaseFacade databaseFacade)
        {
            return ActivatorUtilities.CreateInstance<DatabaseFacadeBasedHealthCheckService>(this.serviceProvider, databaseFacade);
        }
    }
}