// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationReceiver.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client.Services.Interfaces
{
    /// <summary>
    /// The NotificationReceiver interface.
    /// </summary>
    public interface INotificationReceiver
    {
        /// <summary>
        /// Sends a notification async.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="severity">
        /// The severity.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SendAsync(string notificationType, Severity severity, string message);

        /// <summary>
        /// Sends a normal notification async.
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
        Task NormalAsync(string notificationType, string message);

        /// <summary>
        /// Sends a success notification async.
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
        Task SuccessAsync(string notificationType, string message);

        /// <summary>
        /// Sends an error notification async.
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
        Task ErrorAsync(string notificationType, string message);

        /// <summary>
        /// Sends a warning notification async.
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
        Task WarningAsync(string notificationType, string message);

        /// <summary>
        /// Sends an information notification async.
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
        Task InformationAsync(string notificationType, string message);
    }
}