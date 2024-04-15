// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.timestamps.cs" company="Port Hope Investment S.A.">
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
        /// The timestamps policies.
        /// </summary>
        public class Timestamps
        {
            /// <summary>
            /// The administrator.
            /// </summary>
            public static readonly PolicyInfo Administrator = new RoleBasedPolicyInfo(
                Roles.Timestamps.Administrator,
                "User has administrator access ofemployees timestamps",
                roles: GetAdministratorRoles());

            /// <summary>
            /// The editor.
            /// </summary>
            public static readonly PolicyInfo Editor = new RoleBasedPolicyInfo(
                Roles.Timestamps.Editor,
                "User has edit access of employees timestamps");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Viewer = new RoleBasedPolicyInfo(
                Roles.Timestamps.Viewer,
                "User has view access of employees timestamps");

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
                           Roles.Timestamps.Editor,
                           Roles.Timestamps.Viewer,
                       }.ToImmutableList();
            }
        }
    }
}