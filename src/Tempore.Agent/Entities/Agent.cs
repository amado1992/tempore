// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Agent.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The agent.
    /// </summary>
    public class Agent
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
        /// Gets or sets the devices.
        /// </summary>
        public virtual List<Device> Devices { get; set; }
    }
}