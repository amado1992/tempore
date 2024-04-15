// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using Microsoft.AspNetCore.SignalR;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Hubs;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The notification service.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// The hub context.
        /// </summary>
        private readonly IHubContext<NotificationHub, INotificationReceiver> hubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="hubContext">
        /// The hub context.
        /// </param>
        public NotificationService(IHubContext<NotificationHub, INotificationReceiver> hubContext)
        {
            ArgumentNullException.ThrowIfNull(hubContext);

            this.hubContext = hubContext;
        }

        /// <summary>
        /// Sends a success notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SuccessAsync<TNotification>(string userId, string message)
        {
            await this.hubContext.Clients.User(userId).SuccessAsync(typeof(TNotification).Name, message);
        }

        /// <summary>
        /// Sends an error notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ErrorAsync<TNotification>(string userId, string message)
        {
            await this.hubContext.Clients.User(userId).ErrorAsync(typeof(TNotification).Name, message);
        }

        /// <summary>
        /// Sends a normal notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task NormalAsync<TNotification>(string userId, string message)
        {
            await this.hubContext.Clients.User(userId).NormalAsync(typeof(TNotification).Name, message);
        }

        /// <summary>
        /// Sends a warning notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task WarningAsync<TNotification>(string userId, string message)
        {
            await this.hubContext.Clients.User(userId).WarningAsync(typeof(TNotification).Name, message);
        }

        /// <summary>
        /// Sends an information notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InformationAsync<TNotification>(string userId, string message)
        {
            await this.hubContext.Clients.User(userId).InformationAsync(typeof(TNotification).Name, message);
        }

        /// <summary>
        /// Broadcasts a message.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task BroadcastAsync(string notificationType, string message = "")
        {
            await this.hubContext.Clients.All.NormalAsync(notificationType, message);
        }
    }
}