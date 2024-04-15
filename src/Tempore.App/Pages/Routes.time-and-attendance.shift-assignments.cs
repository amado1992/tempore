// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Routes.time-and-attendance.shift-assignments.cs" company="Port Hope Investment S.A.">
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
        /// The employees.
        /// </summary>
        public static partial class TimeAndAttendance
        {
            /// <summary>
            /// The shift assignments.
            /// </summary>
            public const string ShiftAssignmentTemplate = $"{Root}/shift-assignments/{{Id:guid}}";

            /// <summary>
            /// View of employe details.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <returns>
            /// The <see cref="Guid"/>.
            /// </returns>
            public static string ShiftAssignment(Guid? id)
            {
                return ShiftAssignmentTemplate.Replace("{Id:guid}", id.ToString());
            }
        }
    }
}