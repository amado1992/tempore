// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanExtension.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Common.Extensions
{
    using System;

    /// <summary>
    /// The time span extension.
    /// </summary>
    public static class TimeSpanExtension
    {
        /// <summary>
        /// Gets a time only.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="TimeOnly"/>.
        /// </returns>
        public static TimeOnly GetTimeOnly(this TimeSpan time)
        {
            return new TimeOnly(time.Hours, time.Minutes, time.Seconds);
        }
    }
}