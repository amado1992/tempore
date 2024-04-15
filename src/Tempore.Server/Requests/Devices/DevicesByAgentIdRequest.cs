// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DevicesByAgentIdRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Devices
{
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;

    /// <summary>
    /// The devices by agent id request.
    /// </summary>
    public class DevicesByAgentIdRequest : PagedRequest<DeviceDto>
    {
        /// <summary>
        /// Gets or sets the agent id.
        /// </summary>
        public Guid AgentId { get; set; }
    }
}