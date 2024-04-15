// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputeWorkforceMetricsProcessCompletedNotification.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Notifications.WorkforceMetrics
{
    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Notifications;

    /// <summary>
    /// The ScheduleDaysProcessCompletedNotification.
    /// </summary>
    public class ComputeWorkforceMetricsProcessCompletedNotification : InvocationContextNotificationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeWorkforceMetricsProcessCompletedNotification"/> class.
        /// </summary>
        /// <param name="context">
        /// The invocation context.
        /// </param>
        /// <param name="severity">
        /// The severity.
        /// </param>
        public ComputeWorkforceMetricsProcessCompletedNotification(IInvocationContext context, Severity severity)
            : base(context, severity)
        {
        }
    }
}