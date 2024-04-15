// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Common.Extensions
{
    /// <summary>
    /// The date only extensions.
    /// </summary>
    public static class DateOnlyExtensions
    {
        /// <summary>
        /// Adds a <see cref="TimeSpan"/> to the <see cref="DateOnly"/>.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="timeSpan">
        /// The time span.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeOffset"/>.
        /// </returns>
        public static DateTimeOffset Add(this DateOnly date, TimeSpan timeSpan)
        {
            return date.ToDateTime(TimeOnly.MinValue).Add(timeSpan);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeOffset"/>.
        /// </returns>
        public static DateTimeOffset Add(this DateOnly date, TimeOnly time)
        {
            return date.ToDateTime(time);
        }
    }
}