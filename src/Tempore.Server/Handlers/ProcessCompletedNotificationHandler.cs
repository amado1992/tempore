// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessCompletedNotificationHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using System.Threading;

    using MediatR;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Notifications;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The process completed notification handler.
    /// </summary>
    /// <typeparam name="TNotification">
    /// The notification type.
    /// </typeparam>
    public class ProcessCompletedNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : InvocationContextNotificationBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ProcessCompletedNotificationHandler<TNotification>> logger;

        /// <summary>
        /// The localizer service.
        /// </summary>
        private readonly IStringLocalizerService<TNotification> localizerService;

        /// <summary>
        /// The notification service.
        /// </summary>
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCompletedNotificationHandler{TNotification}"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="localizerService">
        /// The string localizer service.
        /// </param>
        /// <param name="notificationService">
        /// The notification service.
        /// </param>
        public ProcessCompletedNotificationHandler(
            ILogger<ProcessCompletedNotificationHandler<TNotification>> logger,
            IStringLocalizerService<TNotification> localizerService,
            INotificationService notificationService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(localizerService);
            ArgumentNullException.ThrowIfNull(notificationService);

            this.logger = logger;
            this.localizerService = localizerService;
            this.notificationService = notificationService;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(TNotification notification, CancellationToken cancellationToken)
        {
            if (notification.InvocationContext.Username is null)
            {
                return;
            }

            if (notification.Severity == Severity.Success)
            {
                await this.notificationService.SuccessAsync<TNotification>(notification.InvocationContext.Username, this.localizerService["Success", notification.InvocationContext.CultureInfo]);
            }
            else
            {
                await this.notificationService.ErrorAsync<TNotification>(notification.InvocationContext.Username, this.localizerService["Error", notification.InvocationContext.CultureInfo]);
            }
        }
    }
}