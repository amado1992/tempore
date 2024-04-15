// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHealthCheckService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The HealthCheckService interface.
    /// </summary>
    public interface IHealthCheckService
    {
        /// <summary>
        /// Determines whether the system is healthy.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> IsHealthyAsync();
    }
}