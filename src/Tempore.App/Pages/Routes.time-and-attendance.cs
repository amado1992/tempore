// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Routes.time-and-attendance.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages
{
    /// <summary>
    /// The routes.
    /// </summary>
    public static partial class Routes
    {
        /// <summary>
        /// The time and Attendance.
        /// </summary>
        public static partial class TimeAndAttendance
        {
            /// <summary>
            /// The root.
            /// </summary>
            public const string Root = "/time-and-attendance";

            /// <summary>
            /// The shifts.
            /// </summary>
            public const string Shifts = $"{Root}/shifts";

            /// <summary>
            /// The upload employees timestamps.
            /// </summary>
            public const string UploadEmployeesTimestamps = $"{Root}/upload-employees-timestamps";
        }
    }
}