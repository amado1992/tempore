// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using Tempore.Storage.Entities;

    /// <summary>
    /// The TimestampExtensions.
    /// </summary>
    public static class TimestampExtensions
    {
        /// <summary>
        /// Computes the distance between <see cref="Timestamp"/> and a <see cref="ScheduledDay"/>.
        /// </summary>
        /// <param name="timestamp">
        /// The timestamp.
        /// </param>
        /// <param name="scheduledDay">
        /// The scheduledDay.
        /// </param>
        /// <returns>
        /// The distance in miliseconds.
        /// </returns>
        public static double Distance(this Timestamp timestamp, ScheduledDay scheduledDay)
        {
            if (scheduledDay.StartDateTime is null || scheduledDay.EndDateTime is null)
            {
                return double.MaxValue;
            }

            var startDateTime = scheduledDay.StartDateTime.Value;
            var endDateTime = scheduledDay.EndDateTime.Value;

            return Math.Min(Math.Abs(timestamp.DateTime.Subtract(startDateTime).TotalMilliseconds), Math.Abs(timestamp.DateTime.Subtract(endDateTime).TotalMilliseconds));
        }
    }
}