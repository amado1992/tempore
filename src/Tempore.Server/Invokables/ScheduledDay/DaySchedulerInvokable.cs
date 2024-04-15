// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DaySchedulerInvokable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.ScheduledDay
{
    using System;

    using Coravel.Invocable;

    using Mapster;

    using MediatR;

    using MethodTimer;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Notifications.ScheduledDay;
    using Tempore.Server.Requests.ScheduledDay;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Server.Specs.ScheduledDay;
    using Tempore.Server.Specs.ScheduledShift;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The day scheduler invokable.
    /// </summary>
    public class DaySchedulerInvokable : IInvocable, IInvocableWithPayload<IInvocationContext<ScheduleDaysRequest>>, ICancellableInvocable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DaySchedulerInvokable> logger;

        /// <summary>
        /// The scheduled day repository.
        /// </summary>
        private readonly IRepository<ScheduledDay, ApplicationDbContext> scheduledDayRepository;

        /// <summary>
        /// The scheduled shift repository.
        /// </summary>
        private readonly IRepository<ScheduledShift, ApplicationDbContext> scheduledShiftRepository;

        /// <summary>
        /// The scheduled shift assignment repository.
        /// </summary>
        private readonly IRepository<ScheduledShiftAssignment, ApplicationDbContext> scheduledShiftAssignmentRepository;

        /// <summary>
        /// The day scheduler service.
        /// </summary>
        private readonly IDaySchedulerService daySchedulerService;

        /// <summary>
        /// The publisher.
        /// </summary>
        private readonly IPublisher publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DaySchedulerInvokable"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="scheduledDayRepository">
        /// The repository.
        /// </param>
        /// <param name="scheduledShiftRepository">
        /// The shift assignment repository.
        /// </param>
        /// <param name="scheduledShiftAssignmentRepository">
        /// The scheduled shift assignment repository.
        /// </param>
        /// <param name="daySchedulerService">
        /// The day scheduler service.
        /// </param>
        /// <param name="publisher">
        /// The publisher.
        /// </param>
        public DaySchedulerInvokable(
            ILogger<DaySchedulerInvokable> logger,
            IRepository<ScheduledDay, ApplicationDbContext> scheduledDayRepository,
            IRepository<ScheduledShift, ApplicationDbContext> scheduledShiftRepository,
            IRepository<ScheduledShiftAssignment, ApplicationDbContext> scheduledShiftAssignmentRepository,
            IDaySchedulerService daySchedulerService,
            IPublisher publisher)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(scheduledDayRepository);
            ArgumentNullException.ThrowIfNull(scheduledShiftRepository);
            ArgumentNullException.ThrowIfNull(scheduledShiftAssignmentRepository);
            ArgumentNullException.ThrowIfNull(daySchedulerService);
            ArgumentNullException.ThrowIfNull(publisher);

            this.logger = logger;
            this.scheduledDayRepository = scheduledDayRepository;
            this.scheduledShiftRepository = scheduledShiftRepository;
            this.scheduledShiftAssignmentRepository = scheduledShiftAssignmentRepository;
            this.daySchedulerService = daySchedulerService;
            this.publisher = publisher;
        }

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public IInvocationContext<ScheduleDaysRequest> Payload { get; set; } = default!;

        /// <inheritdoc />
        [Time]
        public async Task Invoke()
        {
            var request = this.Payload.Request;

            this.logger.LogInformation("Scheduling days for shift '{ScheduledShiftId}' - Force '{Force}'", request.ScheduledShiftId, request.Force);

            try
            {
                var scheduledShift = await this.scheduledShiftRepository.SingleAsync(new ScheduledShiftByIdSpec(request.ScheduledShiftId));

                foreach (var shiftAssignment in scheduledShift.ScheduledShiftAssignments)
                {
                    var currentDate = shiftAssignment.LastGeneratedDayDate?.AddDays(1) ?? DateOnly.MinValue;
                    if (request.Force || currentDate < shiftAssignment.ScheduledShift.StartDate || currentDate > shiftAssignment.ScheduledShift.ExpireDate)
                    {
                        currentDate = shiftAssignment.ScheduledShift.StartDate;
                    }

                    while (currentDate <= shiftAssignment.ScheduledShift.ExpireDate)
                    {
                        this.logger.LogInformation("Scheduling day '{Date}' for shift assignment '{ShiftAssignmentId}' ", currentDate, shiftAssignment.Id);

                        var scheduledDay = await this.daySchedulerService.ScheduleDayAsync(currentDate, shiftAssignment);

                        var scheduledDayByDateSpec = new ScheduledDayByDateSpec(shiftAssignment.Id, currentDate);
                        var storedScheduledDay = await this.scheduledDayRepository.SingleOrDefaultAsync(scheduledDayByDateSpec);
                        if (storedScheduledDay is null)
                        {
                            this.logger.LogInformation("Adding new  day '{Date}' for shift assignment '{ShiftAssignmentId}' ", currentDate, shiftAssignment.Id);

                            this.scheduledDayRepository.Add(scheduledDay);
                            await this.scheduledDayRepository.SaveChangesAsync();

                            this.logger.LogInformation("Adding new  day '{Date}' for shift assignment '{ShiftAssignmentId}' - '{ScheduledDayId}' ", currentDate, shiftAssignment.Id, scheduledDay.Id);
                        }
                        else
                        {
                            this.logger.LogInformation("Updating existing scheduled day '{Date}' for shift assignment '{ShiftAssignmentId}' - '{ScheduledDayId}'", currentDate, shiftAssignment.Id, storedScheduledDay.Id);

                            var typeAdapterConfig = TypeAdapterConfig<ScheduledDay, ScheduledDay>
                                               .NewConfig()
                                               .Ignore(day => day.Id)
                                               .Ignore(day => day.Day)
                                               .Ignore(day => day.Timestamps)
                                               .Ignore(day => day.ScheduledShiftAssignment!).Config;
                            scheduledDay.Adapt(storedScheduledDay, typeAdapterConfig);

                            this.scheduledDayRepository.Update(storedScheduledDay);
                            await this.scheduledDayRepository.SaveChangesAsync();

                            this.logger.LogInformation("Updated existing scheduled day '{Date}' for shift assignment '{ShiftAssignmentId}' - '{ScheduledDayId}'", currentDate, shiftAssignment.Id, storedScheduledDay.Id);
                        }

                        shiftAssignment.LastGeneratedDayDate = currentDate;
                        this.scheduledShiftAssignmentRepository.Update(shiftAssignment);
                        await this.scheduledShiftAssignmentRepository.SaveChangesAsync();

                        currentDate = currentDate.AddDays(1);

                        this.logger.LogInformation("Scheduled day '{Date}' for shift assignment '{ShiftAssignmentId}' ", currentDate, shiftAssignment.Id);
                    }

                    this.logger.LogInformation("Removing out of date range scheduled days for shift '{ShiftId}' from '{StartDate}' to '{ExpireDate}'", scheduledShift.ShiftId, scheduledShift.StartDate, scheduledShift.ExpireDate);

                    this.scheduledDayRepository.Delete(scheduledDay => scheduledDay.ScheduledShiftAssignmentId == shiftAssignment.Id && (scheduledDay.Date < shiftAssignment.ScheduledShift.StartDate || scheduledDay.Date > shiftAssignment.ScheduledShift.ExpireDate));
                    await this.scheduledDayRepository.SaveChangesAsync();

                    this.logger.LogInformation("Removed out of date range scheduled days for shift '{ShiftId}' from '{StartDate}' to '{ExpireDate}'", scheduledShift.ShiftId, scheduledShift.StartDate, scheduledShift.ExpireDate);
                }

                this.logger.LogInformation("Scheduled days for shift '{ShiftId}' from '{StartDate}' to '{ExpireDate}'", scheduledShift.ShiftId, scheduledShift.StartDate, scheduledShift.ExpireDate);
                await this.publisher.Publish(new ScheduleDaysProcessCompletedNotification(this.Payload, Severity.Success));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred scheduling days for shift '{ScheduledShiftId}' - Force '{Force}'", request.ScheduledShiftId, request.Force);

                await this.publisher.Publish(new ScheduleDaysProcessCompletedNotification(this.Payload, Severity.Error));
            }
        }
    }
}