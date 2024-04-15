// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationReceivedEventArgs.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services.EventArgs
{
    using Tempore.Client.Services.Interfaces;

    using EventArgs = System.EventArgs;

    /// <summary>
    /// The notification received event args.
    /// </summary>
    public class NotificationReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="severity">
        /// The severity.
        /// </param>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public NotificationReceivedEventArgs(Severity severity, string notificationType, string message)
        {
            this.Severity = severity;
            this.NotificationType = notificationType;
            this.Message = message;
        }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        public Severity Severity { get; }

        /// <summary>
        /// Gets the notification type.
        /// </summary>
        public string NotificationType { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; }
    }
}