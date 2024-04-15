// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleDaysRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.ScheduledDay
{
    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Invokables.ScheduledDay;
    using Tempore.Server.Requests.ScheduledDay;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The scheduled days request handler.
    /// </summary>
    public class ScheduleDaysRequestHandler : IRequestHandler<ScheduleDaysRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ScheduleDaysRequestHandler> logger;

        /// <summary>
        /// The queue service.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// The scheduled shift repository.
        /// </summary>
        private readonly IRepository<ScheduledShift, ApplicationDbContext> scheduledShiftRepository;

        /// <summary>
        /// The invocation context accessor.
        /// </summary>
        private readonly IInvocationContextAccessor invocationContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDaysRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        /// <param name="scheduledShiftRepository">
        /// The scheduled shift repository.
        /// </param>
        /// <param name="invocationContextAccessor">
        /// The scheduled context accessor.
        /// </param>
        public ScheduleDaysRequestHandler(
            ILogger<ScheduleDaysRequestHandler> logger,
            IQueueService queueService,
            IRepository<ScheduledShift, ApplicationDbContext> scheduledShiftRepository,
            IInvocationContextAccessor invocationContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);
            ArgumentNullException.ThrowIfNull(scheduledShiftRepository);
            ArgumentNullException.ThrowIfNull(invocationContextAccessor);

            this.logger = logger;
            this.queueService = queueService;
            this.scheduledShiftRepository = scheduledShiftRepository;
            this.invocationContextAccessor = invocationContextAccessor;
        }

        /// <inheritdoc />
        public async Task Handle(ScheduleDaysRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (this.queueService.IsScheduled<DaySchedulerInvokable>())
            {
                throw this.logger.LogErrorAndCreateException<ConflictException>("Scheduling day job is already scheduled");
            }

            var scheduledShift = await this.scheduledShiftRepository.SingleOrDefaultAsync(scheduledShift => scheduledShift.Id == request.ScheduledShiftId);
            if (scheduledShift is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"ScheduledShift with id {request.ScheduledShiftId} not found");
            }

            this.queueService.QueueInvocableWithPayload<DaySchedulerInvokable, IInvocationContext<ScheduleDaysRequest>>(this.invocationContextAccessor.Create(request));
        }
    }
}