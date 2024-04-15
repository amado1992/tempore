// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAgentReceiver.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client.Services.Interfaces
{
    /// <summary>
    /// The TemporeAgentReceiver interface.
    /// </summary>
    public interface IAgentReceiver
    {
        /// <summary>
        /// The register async.
        /// </summary>
        /// <param name="connectionId">
        /// The connection id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task RegisterAsync(string connectionId);

        /// <summary>
        /// The load users async.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task UploadEmployeesAsync(Guid deviceId);

        /// <summary>
        /// Executes upload employees timestamps async.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <param name="from">
        /// The request from.
        /// </param>
        /// <param name="to">
        /// The request to.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task UploadEmployeesTimestampsAsync(Guid deviceId, DateTimeOffset from, DateTimeOffset? to);

        /// <summary>
        /// The is device alive async.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ReportDeviceStateAsync(Guid deviceId);
    }
}