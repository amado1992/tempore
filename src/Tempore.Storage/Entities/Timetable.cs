// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timetable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The timetable.
    /// </summary>
    public class Timetable
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
        /// Gets or sets the start work time.
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the check in time start.
        /// </summary>
        public TimeSpan CheckInTimeStart { get; set; }

        /// <summary>
        /// Gets or sets the check in time duration.
        /// </summary>
        public TimeSpan CheckInTimeDuration { get; set; }

        /// <summary>
        /// Gets or sets the check out time start.
        /// </summary>
        public TimeSpan CheckOutTimeStart { get; set; }

        /// <summary>
        /// Gets or sets the check out time duration.
        /// </summary>
        public TimeSpan CheckOutTimeDuration { get; set; }

        /// <summary>
        /// Gets or sets the effective working time.
        /// </summary>
        public TimeSpan EffectiveWorkingTime { get; set; }

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        public List<Day> Days { get; set; }
    }
}