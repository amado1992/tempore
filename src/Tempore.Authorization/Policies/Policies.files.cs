// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.files.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization.Policies
{
    using System.Collections.Immutable;

    using Tempore.Authorization;

    using Roles = Tempore.Authorization.Roles.Roles;

    /// <summary>
    /// The policies.
    /// </summary>
    public static partial class Policies
    {
        /// <summary>
        /// The agents.
        /// </summary>
        public class Files
        {
            /// <summary>
            /// The administrator.
            /// </summary>
            public static readonly PolicyInfo Administrator = new RoleBasedPolicyInfo(
                Roles.Files.Administrator,
                "User has administrator access to files",
                roles: GetAdministratorRoles());

            /// <summary>
            /// The editor.
            /// </summary>
            public static readonly PolicyInfo Editor = new RoleBasedPolicyInfo(
                Roles.Files.Editor,
                "User has edit access to files");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Viewer = new RoleBasedPolicyInfo(
                Roles.Files.Viewer,
                "User has view access to files");

            /// <summary>
            /// The content viewer.
            /// </summary>
            public static readonly PolicyInfo ContentViewer = new RoleBasedPolicyInfo(
                Roles.Files.ContentViewer,
                "User has view access to view content of files");

            /// <summary>
            /// The file uploader.
            /// </summary>
            public static readonly PolicyInfo Uploader = new RoleBasedPolicyInfo(
                Roles.Files.Uploader,
                "User has upload file access");

            /// <summary>
            /// The file content approver.
            /// </summary>
            public static readonly PolicyInfo Approver = new RoleBasedPolicyInfo(
                Roles.Files.ContentApprover,
                "File content approver");

            /// <summary>
            /// Gets administrator roles.
            /// </summary>
            /// <returns>
            /// The <see cref="ImmutableList"/>.
            /// </returns>
            private static ImmutableList<string> GetAdministratorRoles()
            {
                return new[]
                       {
                           Roles.Files.Editor,
                           Roles.Files.Viewer,
                           Roles.Files.ContentViewer,
                           Roles.Files.Uploader,
                           Roles.Files.ContentApprover,
                       }.ToImmutableList();
            }
        }
    }
}