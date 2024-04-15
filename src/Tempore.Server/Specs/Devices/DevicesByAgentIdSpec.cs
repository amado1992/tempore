// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DevicesByAgentIdSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Devices
{
    using Tempore.Server.Specs;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The DevicesByAgentIdSpec.
    /// </summary>
    public class DevicesByAgentIdSpec : Specification<Device>
    {
        /// <summary>
        /// The agent id.
        /// </summary>
        private readonly Guid agentId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesByAgentIdSpec"/> class.
        /// </summary>
        /// <param name="agentId">
        /// The agent id.
        /// </param>
        /// <param name="paginationOptions">
        /// The pagination options.
        /// </param>
        public DevicesByAgentIdSpec(Guid agentId, PaginationOptions paginationOptions)
            : base(paginationOptions)
        {
            this.agentId = agentId;
        }

        /// <inheritdoc />
        protected override Func<IQueryable<Device>, IQueryable<Device>> BuildSpec()
        {
            return devices => devices.Where(device => device.AgentId == this.agentId);
        }
    }
}