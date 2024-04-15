// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    /// <summary>
    /// The NotificationService interface.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Sends a success notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SuccessAsync<TNotification>(string userId, string message);

        /// <summary>
        /// Sends an error notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ErrorAsync<TNotification>(string userId, string message);

        /// <summary>
        /// Sends a normal notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task NormalAsync<TNotification>(string userId, string message);

        /// <summary>
        /// Sends a warning notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task WarningAsync<TNotification>(string userId, string message);

        /// <summary>
        /// Sends an information notification async.
        /// </summary>
        /// <typeparam name="TNotification">
        /// The notification type.
        /// </typeparam>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task InformationAsync<TNotification>(string userId, string message);

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
        Task BroadcastAsync(string notificationType, string message = "");
    }
}