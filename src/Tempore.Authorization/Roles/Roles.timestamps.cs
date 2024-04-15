// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.timestamps.cs" company="Port Hope Investment S.A.">
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
        /// The timestamps roles.
        /// </summary>
        public static class Timestamps
        {
            /// <summary>
            /// The employee administrator.
            /// </summary>
            public const string Administrator = "Timestamps Administrator";

            /// <summary>
            /// The viewer.
            /// </summary>
            public const string Viewer = "Timestamps Viewer";

            /// <summary>
            /// The editor.
            /// </summary>
            public const string Editor = "Timestamps Editor";
        }
    }
}