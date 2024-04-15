// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateDeviceStateRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Devices
{
    using MediatR;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The update device state request.
    /// </summary>
    public class UpdateDeviceStateRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the device state.
        /// </summary>
        public DeviceState DeviceState { get; set; }
    }
}