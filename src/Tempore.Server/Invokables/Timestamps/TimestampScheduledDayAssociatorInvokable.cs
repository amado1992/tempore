// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampScheduledDayAssociatorInvokable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.ScheduledDay
{
    using System;

    using Coravel.Invocable;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The TimestampScheduledDayAssociatorInvokable.
    /// </summary>
    public class TimestampScheduledDayAssociatorInvokable : IInvocable
    {
        private readonly ILogger<TimestampScheduledDayAssociatorInvokable> logger;

        private readonly IRepository<Timestamp, ApplicationDbContext> timestampRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimestampScheduledDayAssociatorInvokable"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="timestampRepository">
        /// The timestamp repository.
        /// </param>
        public TimestampScheduledDayAssociatorInvokable(
            ILogger<TimestampScheduledDayAssociatorInvokable> logger, IRepository<Timestamp, ApplicationDbContext> timestampRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timestampRepository);

            this.logger = logger;
            this.timestampRepository = timestampRepository;
        }

        /// <inheritdoc />
        public async Task Invoke()
        {
            var timestampScheduledDaysTuples = await this.timestampRepository.FindAsync(new TimestampScheduledDaysCandidatesSpec());
            foreach (var timestampScheduledDaysTuple in timestampScheduledDaysTuples)
            {
                var timestamp = timestampScheduledDaysTuple.Timestamp;
                var scheduledDayMatch = timestampScheduledDaysTuple.ScheduledDays
                    .FirstOrDefault(scheduledDay => scheduledDay.CheckInStartDateTime <= timestamp.DateTime && scheduledDay.CheckOutEndDateTime >= timestamp.DateTime);

                if (scheduledDayMatch is not null)
                {
                    timestamp.ScheduledDayId = scheduledDayMatch.Id;
                    this.timestampRepository.Update(timestamp);
                    await this.timestampRepository.SaveChangesAsync();
                }
            }
        }
    }
}