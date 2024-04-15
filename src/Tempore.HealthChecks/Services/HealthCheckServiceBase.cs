// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthCheckServiceBase.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services
{
    using Tempore.HealthChecks.Services.Interfaces;

    /// <summary>
    /// The health check service base.
    /// </summary>
    public abstract class HealthCheckServiceBase : IHealthCheckService
    {
        /// <summary>
        /// The is healthy async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public abstract Task<bool> IsHealthyAsync();

        /// <summary>
        /// The wait async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task WaitAsync()
        {
            while (!await this.IsHealthyAsync())
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}