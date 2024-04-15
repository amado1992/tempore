// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsScheduleDaysProcessRunningRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.ScheduledDay
{
    using MediatR;

    using Tempore.Server.Invokables.ScheduledDay;
    using Tempore.Server.Requests.ScheduledDay;
    using Tempore.Server.Services.Interfaces;

    public class IsScheduleDaysProcessRunningRequestHandler : IRequestHandler<IsScheduleDaysProcessRunningRequest, bool>
    {
        private readonly ILogger<IsScheduleDaysProcessRunningRequestHandler> logger;

        /// <summary>
        /// The queue service.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsScheduleDaysProcessRunningRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        public IsScheduleDaysProcessRunningRequestHandler(ILogger<IsScheduleDaysProcessRunningRequestHandler> logger, IQueueService queueService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);

            this.logger = logger;
            this.queueService = queueService;
        }

        /// <inheritdoc/>
        public Task<bool> Handle(IsScheduleDaysProcessRunningRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            //// TODO: Some processes must be singletons for the app while others depends on parameters.
            //// This DaySchedulerInvokable process can be enqueued with different parameters.
            //// Task: https://dev.azure.com/Port-Hope-Investment/Tempore/_workitems/edit/458
            return Task.FromResult(this.queueService.IsScheduled<DaySchedulerInvokable>());
        }
    }
}