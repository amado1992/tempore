// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTimestampsFromDevicesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.DeviceCommands
{
    using MediatR;

    /// <summary>
    /// The execute upload employees timestamps request.
    /// </summary>
    public class UploadTimestampsFromDevicesRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the device ids.
        /// </summary>
        public List<Guid> DeviceIds { get; set; }

        /// <summary>
        /// Gets or sets the from.
        /// </summary>
        public DateTimeOffset From { get; set; }

        /// <summary>
        /// Gets or sets the from.
        /// </summary>
        public DateTimeOffset? To { get; set; }
    }
}