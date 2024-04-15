// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseHealthCheckServiceFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services.Interfaces
{
    using Microsoft.EntityFrameworkCore.Infrastructure;

    using Tempore.Infrastructure.HealthChecks.Interfaces;

    /// <summary>
    /// The DatabaseHealthCheckServiceFactory interface.
    /// </summary>
    public interface IDatabaseHealthCheckServiceFactory
    {
        /// <summary>
        /// Create a database health check service.
        /// </summary>
        /// <param name="databaseFacade">
        /// The database facade.
        /// </param>
        /// <returns>
        /// The <see cref="IDatabaseHealthCheckService"/>.
        /// </returns>
        IDatabaseHealthCheckService Create(DatabaseFacade databaseFacade);
    }
}