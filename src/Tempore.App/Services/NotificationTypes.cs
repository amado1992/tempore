// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationTypes.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services
{
    /// <summary>
    /// The notification types.
    /// </summary>
    public static class NotificationTypes
    {
        /// <summary>
        /// The ComputeWorkforceMetricsProcessCompletedNotification.
        /// </summary>
        public const string ComputeWorkforceMetricsProcessCompletedNotification = nameof(ComputeWorkforceMetricsProcessCompletedNotification);

        /// <summary>
        /// The EmployeesLinkProcessCompletedNotification.
        /// </summary>
        public const string EmployeesLinkProcessCompletedNotification = nameof(EmployeesLinkProcessCompletedNotification);

        /// <summary>
        /// The ScheduleDaysProcessCompletedNotification.
        /// </summary>
        public const string ScheduleDaysProcessCompletedNotification = nameof(ScheduleDaysProcessCompletedNotification);

        /// <summary>
        /// The ProcessFileProcessCompletedNotification.
        /// </summary>
        public const string ProcessFileProcessCompletedNotification = nameof(ProcessFileProcessCompletedNotification);

        /// <summary>
        /// The QueueTaskStarted.
        /// </summary>
        public static string QueueTaskStarted = nameof(QueueTaskStarted);

        /// <summary>
        /// The QueueTaskCompleted.
        /// </summary>
        public static string QueueTaskCompleted = nameof(QueueTaskCompleted);
    }
}