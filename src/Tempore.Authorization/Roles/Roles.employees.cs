// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.employees.cs" company="Port Hope Investment S.A.">
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
        /// The employee roles.
        /// </summary>
        public static class Employees
        {
            /// <summary>
            /// The employee administrator.
            /// </summary>
            public const string Administrator = "Employees Administrator";

            /// <summary>
            /// The viewer.
            /// </summary>
            public const string Viewer = "Employees Viewer";

            /// <summary>
            /// The editor.
            /// </summary>
            public const string Editor = "Employees Editor";

            /// <summary>
            /// The editor.
            /// </summary>
            public const string Linker = "Employees Linker";
        }
    }
}