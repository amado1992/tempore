// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadEmployeesFromDevicesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.DeviceCommands
{
    using MediatR;

    /// <summary>
    /// The load device from user request.
    /// </summary>
    public class UploadEmployeesFromDevicesRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the device ids.
        /// </summary>
        public List<Guid> DeviceIds { get; set; }
    }
}