// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Employee.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The employee.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the external id.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the employee from device.
        /// </summary>
        public List<EmployeeFromDevice>? EmployeeFromDevice { get; set; }

        /// <summary>
        /// Gets or sets the identification card.
        /// </summary>
        public string? IdentificationCard { get; set; }

        /// <summary>
        /// Gets or sets the seg social.
        /// </summary>
        public string? SocialSecurity { get; set; }

        /// <summary>
        /// Gets or sets the dpto.
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Gets or sets the cost center.
        /// </summary>
        public string? CostCenter { get; set; }

        /// <summary>
        /// Gets or sets the admission date.
        /// </summary>
        public DateTime? AdmissionDate { get; set; }

        /// <summary>
        /// Gets or sets the base hours.
        /// </summary>
        public float? BaseHours { get; set; }

        /// <summary>
        /// Gets or sets the shift assignments.
        /// </summary>
        public List<ScheduledShiftAssignment> ScheduledShiftAssignments { get; set; }

        /// <summary>
        /// Gets or sets the scheduled shifts.
        /// </summary>
        public List<ScheduledShift> ScheduledShifts { get; set; }
    }
}