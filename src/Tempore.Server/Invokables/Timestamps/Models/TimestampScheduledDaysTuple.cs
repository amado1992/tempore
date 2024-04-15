// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampScheduledDaysTuple.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.Timestamps.Models
{
    using Tempore.Storage.Entities;

    public class TimestampScheduledDaysTuple
    {
        public Timestamp Timestamp { get; set; }

        public IEnumerable<ScheduledDay> ScheduledDays { get; set; }
    }
}