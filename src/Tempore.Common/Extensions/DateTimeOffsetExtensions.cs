// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeOffsetExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Common.Extensions
{
    /// <summary>
    /// The date time offset extensions.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// The min.
        /// </summary>
        /// <param name="current">
        /// The date time offset.
        /// </param>
        /// <param name="dateTimeOffset">
        /// The date time offset b.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeOffset"/>.
        /// </returns>
        public static DateTimeOffset Min(this DateTimeOffset current, DateTimeOffset dateTimeOffset)
        {
            return current < dateTimeOffset ? current : dateTimeOffset;
        }

        /// <summary>
        /// The max.
        /// </summary>
        /// <param name="current">
        /// The date time offset.
        /// </param>
        /// <param name="dateTimeOffset">
        /// The date time offset b.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeOffset"/>.
        /// </returns>
        public static DateTimeOffset Max(this DateTimeOffset current, DateTimeOffset dateTimeOffset)
        {
            return current > dateTimeOffset ? current : dateTimeOffset;
        }
    }
}