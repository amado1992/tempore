// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.shifts.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization.Roles
{
    /// <summary>
    /// The roles.
    /// </summary>
    public static partial class Roles
    {
        /// <summary>
        /// The shift roles.
        /// </summary>
        public static class Shifts
        {
            /// <summary>
            /// The shifts administrator.
            /// </summary>
            public const string Administrator = "Shifts Administrator";

            /// <summary>
            /// The viewer.
            /// </summary>
            public const string Viewer = "Shifts Viewer";

            /// <summary>
            /// The editor.
            /// </summary>
            public const string Editor = "Shifts Editor";

            /// <summary>
            /// The shift manager.
            /// </summary>
            public const string Manager = "Shifts Manager";
        }
    }
}