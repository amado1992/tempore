// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timestamp.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    using Tempore.Storage.Configurations;

    /// <summary>
    /// The timestamp type.
    /// </summary>
    public enum TimestampType
    {
        /// <summary>
        ///
        /// </summary>
        Automatic,

        Manual,
    }

    /// <summary>
    /// The timestamp.
    /// </summary>
    public class Timestamp
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the employee from device id.
        /// </summary>
        public Guid EmployeeFromDeviceId { get; set; }

        /// <summary>
        /// Gets or sets the employee from device.
        /// </summary>
        public EmployeeFromDevice EmployeeFromDevice { get; set; }

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// Gets or sets the scheduled day id.
        /// </summary>
        public Guid? ScheduledDayId { get; set; }

        /// <summary>
        /// Gets or sets the day journey.
        /// </summary>
        public ScheduledDay? ScheduledDay { get; set; }

        ///// <summary>
        ///// Gets or sets the type.
        ///// </summary>
        // public TimestampType Type { get; set; }
    }
}