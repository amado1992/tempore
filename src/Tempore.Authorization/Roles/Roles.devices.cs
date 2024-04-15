// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.devices.cs" company="Port Hope Investment S.A.">
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
        /// The devices roles.
        /// </summary>
        public static class Devices
        {
            /// <summary>
            /// The device administrator.
            /// </summary>
            public const string Administrator = "Devices Administrator";

            /// <summary>
            /// The editor.
            /// </summary>
            public const string Editor = "Devices Editor";

            /// <summary>
            /// The viewer.
            /// </summary>
            public const string Viewer = "Devices Viewer";
        }
    }
}