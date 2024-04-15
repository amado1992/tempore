// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.employees.cs" company="Port Hope Investment S.A.">
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
        /// The employees policies.
        /// </summary>
        public class Employees
        {
            /// <summary>
            /// The administrator.
            /// </summary>
            public static readonly PolicyInfo Administrator = new RoleBasedPolicyInfo(
                Roles.Employees.Administrator,
                "User has administrator access over employees",
                roles: GetAdministratorRoles());

            /// <summary>
            /// The editor.
            /// </summary>
            public static readonly PolicyInfo Editor = new RoleBasedPolicyInfo(
                Roles.Employees.Editor,
                "User has edit access of employees");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Viewer = new RoleBasedPolicyInfo(
                Roles.Employees.Viewer,
                "User has view access of employees");

            /// <summary>
            /// The linker.
            /// </summary>
            public static readonly PolicyInfo Linker = new RoleBasedPolicyInfo(
                Roles.Employees.Linker,
                "User has permission link employees with from employees from devices");

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
                           Roles.Employees.Editor,
                           Roles.Employees.Viewer,
                           Roles.Employees.Linker,
                       }.ToImmutableList();
            }
        }
    }
}