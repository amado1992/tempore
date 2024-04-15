// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseFacadeBasedHealthCheckService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Logging;

    using Tempore.Infrastructure.HealthChecks;
    using Tempore.Infrastructure.HealthChecks.Interfaces;

    public class DatabaseFacadeBasedHealthCheckService : HealthCheckServiceBase, IDatabaseHealthCheckService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DatabaseHealthCheckServiceBase> logger;

        /// <summary>
        /// The database facade.
        /// </summary>
        private readonly DatabaseFacade databaseFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFacadeBasedHealthCheckService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="databaseFacade">
        /// The database facade.
        /// </param>
        public DatabaseFacadeBasedHealthCheckService(ILogger<DatabaseHealthCheckServiceBase> logger, DatabaseFacade databaseFacade)
        {
            this.logger = logger;
            this.databaseFacade = databaseFacade;
        }

        /// <summary>
        /// The is healthy async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task<bool> IsHealthyAsync()
        {
            return await this.databaseFacade.CanConnectAsync();
        }
    }
}