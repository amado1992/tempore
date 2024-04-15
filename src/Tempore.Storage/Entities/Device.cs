// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Device.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
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
        /// Gets or sets the device name.
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        public string? SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the mac address.
        /// </summary>
        public string? MacAddress { get; set; }

        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        public Agent Agent { get; set; }

        /// <summary>
        /// Gets or sets the agent id.
        /// </summary>
        public Guid AgentId { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public DeviceState State { get; set; }

        /// <summary>
        /// Gets or sets the employees from devices.
        /// </summary>
        public virtual List<EmployeeFromDevice> EmployeesFromDevices { get; set; }
    }
}