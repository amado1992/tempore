// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Device.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The device.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last transferred timestamp date time.
        /// </summary>
        public DateTimeOffset? LastTransferredTimestampDateTime { get; set; }

        /// <summary>
        /// Gets or sets the agent id.
        /// </summary>
        public Guid AgentId { get; set; }

        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        public Agent Agent { get; set; }
    }
}