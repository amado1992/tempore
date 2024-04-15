// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.system.cs" company="Port Hope Investment S.A.">
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
        /// The system.
        /// </summary>
        public class System
        {
            // TODO: Add a test for missing roles in administrator?

            /// <summary>
            /// The administrator.
            /// </summary>
            public static readonly PolicyInfo Administrator = new RoleBasedPolicyInfo(
                Roles.System.Administrator,
                "User has system administrator access",
                roles: GetAdministratorRoles(),
                groupName: Groups.Groups.TemporeAdministrators);

            /// <summary>
            /// Gets the administrator roles.
            /// </summary>
            /// <returns>
            /// The <see cref="ImmutableList"/>.
            /// </returns>
            private static IImmutableList<string> GetAdministratorRoles()
            {
                return new[]
                       {
                           Roles.Devices.Administrator,
                           Roles.Agents.Administrator,
                           Roles.Files.Administrator,
                           Roles.Employees.Administrator,
                           Roles.Timestamps.Administrator,
                           Roles.Shifts.Administrator,
                           Roles.Metrics.Administrator,
                       }.ToImmutableList();
            }
        }
    }
}