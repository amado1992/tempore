// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.shifts.cs" company="Port Hope Investment S.A.">
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
        /// The shifts policies.
        /// </summary>
        public class Shifts
        {
            /// <summary>
            /// The administrator.
            /// </summary>
            public static readonly PolicyInfo Administrator = new RoleBasedPolicyInfo(
                Roles.Shifts.Administrator,
                "User has administrator access over shifts",
                roles: GetAdministratorRoles());

            /// <summary>
            /// The editor.
            /// </summary>
            public static readonly PolicyInfo Editor = new RoleBasedPolicyInfo(
                Roles.Shifts.Editor,
                "User has edit access of shifts");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Viewer = new RoleBasedPolicyInfo(
                Roles.Shifts.Viewer,
                "User has view access of shifts");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Manager = new RoleBasedPolicyInfo(
                Roles.Shifts.Manager,
                "User can manage shifts");

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
                           Roles.Shifts.Editor,
                           Roles.Shifts.Viewer,
                           Roles.Shifts.Manager,
                       }.ToImmutableList();
            }
        }
    }
}