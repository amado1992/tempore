// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The date time service.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        public DateTimeService()
        {
        }

        /// <inheritdoc/>
        public DateTime Now()
        {
            return DateTime.Now;
        }

        /// <inheritdoc/>
        public DateOnly Today()
        {
            return DateOnly.FromDateTime(DateTime.Today);
        }
    }
}