// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeFromDevice.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The employee from a device.
    /// </summary>
    public class EmployeeFromDevice
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// Gets or sets the employee id on device.
        /// </summary>
        public string EmployeeIdOnDevice { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the employee.
        /// </summary>
        public Employee? Employee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is linked.
        /// </summary>
        public bool IsLinked { get; set; }

        /// <summary>
        /// Gets or sets the timestamps.
        /// </summary>
        public List<Timestamp>? Timestamps { get; set; }
    }
}