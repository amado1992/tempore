// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Day.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The day.
    /// </summary>
    public class Day
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the timetable id.
        /// </summary>
        public Guid? TimetableId { get; set; }

        /// <summary>
        /// Gets or sets the timetable.
        /// </summary>
        public Timetable? Timetable { get; set; }

        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        public Shift Shift { get; set; }

        /// <summary>
        /// Gets or sets the scheduled days.
        /// </summary>
        public List<ScheduledDay> ScheduledDays { get; set; }

        ///// <summary>
        ///// Gets or sets a value indicating whether is rest.
        ///// </summary>
        // public bool IsRest { get; set; }
    }
}