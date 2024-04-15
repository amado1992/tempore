// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationCenter.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services.Interfaces
{
    using Tempore.App.Services.EventArgs;

    /// <summary>
    /// The notification center interface.
    /// </summary>
    public interface INotificationCenter
    {
        /// <summary>
        /// The notification received event.
        /// </summary>
        event EventHandler<NotificationReceivedEventArgs>? NotificationReceived;

        /// <summary>
        /// Subscribes an action.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        void Subscribe(string notificationType, Action<NotificationReceivedEventArgs> action);

        /// <summary>
        /// Unsubscribe an action.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        void Unsubscribe(string notificationType, Action<NotificationReceivedEventArgs> action);
    }
}