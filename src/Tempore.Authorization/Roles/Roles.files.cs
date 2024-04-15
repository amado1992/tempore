// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.files.cs" company="Port Hope Investment S.A.">
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
        public static class Files
        {
            /// <summary>
            /// The device administrator.
            /// </summary>
            public const string Administrator = "File Administrator";

            /// <summary>
            /// The editor.
            /// </summary>
            public const string Editor = "File Editor";

            /// <summary>
            /// The viewer.
            /// </summary>
            public const string Viewer = "File Viewer";

            /// <summary>
            /// The content viewer.
            /// </summary>
            public const string ContentViewer = "File Content Viewer";

            /// <summary>
            /// The uploader.
            /// </summary>
            public const string Uploader = "File Uploader";

            /// <summary>
            /// The file approver.
            /// </summary>
            public const string ContentApprover = "File Content Approver";
        }
    }
}