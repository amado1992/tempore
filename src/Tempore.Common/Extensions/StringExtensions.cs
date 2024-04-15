// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Common.Extensions
{
    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The parse as date time.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime? ParseAsDateTime(this string text)
        {
            return DateTime.TryParse(text, out var value) ? value : null;
        }

        /// <summary>
        /// The parse as float.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float? ParseAsFloat(this string text)
        {
            return float.TryParse(text, out var value) ? value : null;
        }
    }
}