// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskStartedListener.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Listeners
{
    using System.Threading.Tasks;

    using Coravel.Events.Interfaces;
    using Coravel.Queuing.Broadcast;

    using Tempore.App.Services;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The task started listener.
    /// </summary>
    public class TaskStartedListener : IListener<QueueTaskStarted>
    {
        /// <summary>
        /// The task tracker.
        /// </summary>
        private readonly IQueueService queueService;

        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskStartedListener"/> class.
        /// </summary>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        /// <param name="notificationService">
        /// The notification service.
        /// </param>
        public TaskStartedListener(IQueueService queueService, INotificationService notificationService)
        {
            ArgumentNullException.ThrowIfNull(queueService);
            ArgumentNullException.ThrowIfNull(notificationService);

            this.queueService = queueService;
            this.notificationService = notificationService;
        }

        /// <summary>
        /// The handle async.
        /// </summary>
        /// <param name="broadcasted">
        /// The event data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task HandleAsync(QueueTaskStarted broadcasted)
        {
            var type = this.queueService.GetTaskType(broadcasted.Guid);
            if (type is not null)
            {
                await this.notificationService.BroadcastAsync(NotificationTypes.QueueTaskStarted, string.Empty);
            }
        }
    }
}