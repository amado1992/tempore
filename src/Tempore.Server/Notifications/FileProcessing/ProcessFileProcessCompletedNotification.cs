// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFileProcessCompletedNotification.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Notifications.FileProcessing
{
    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Notifications;

    /// <summary>
    /// The ProcessFileProcessCompletedNotification class.
    /// </summary>
    public class ProcessFileProcessCompletedNotification : InvocationContextNotificationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessFileProcessCompletedNotification"/> class.
        /// </summary>
        /// <param name="context">
        /// The invocation context.
        /// </param>
        /// <param name="severity">
        /// The severity.
        /// </param>
        public ProcessFileProcessCompletedNotification(IInvocationContext context, Severity severity)
            : base(context, severity)
        {
        }
    }
}