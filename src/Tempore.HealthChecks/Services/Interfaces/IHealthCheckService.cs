// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHealthCheckService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services.Interfaces
{
    /// <summary>
    /// The HealthCheckService interface.
    /// </summary>
    public interface IHealthCheckService
    {
        /// <summary>
        /// The is healthy async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> IsHealthyAsync();

        /// <summary>
        /// The wait async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task WaitAsync();
    }
}