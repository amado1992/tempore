// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.agents.cs" company="Port Hope Investment S.A.">
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
        public class Agents
        {
            /// <summary>
            /// The administrator.
            /// </summary>
            public static readonly PolicyInfo Administrator = new RoleBasedPolicyInfo(
                Roles.Agents.Administrator,
                "User has administrator access to agents",
                roles: GetAdministratorRoles());

            /// <summary>
            /// The editor.
            /// </summary>
            public static readonly PolicyInfo Editor = new RoleBasedPolicyInfo(
                Roles.Agents.Editor,
                "User has edit access to agents");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Viewer = new RoleBasedPolicyInfo(
                Roles.Agents.Viewer,
                "User has view access to agents");

            /// <summary>
            /// The operator.
            /// </summary>
            public static readonly PolicyInfo Operator = new RoleBasedPolicyInfo(
                Roles.Agents.Operator,
                "User has permission to send commands to agents");

            /// <summary>
            /// The get administrator roles.
            /// </summary>
            /// <returns>
            /// The <see cref="ImmutableList"/>.
            /// </returns>
            private static ImmutableList<string> GetAdministratorRoles()
            {
                return new[] { Roles.Agents.Editor, Roles.Agents.Viewer, Roles.Agents.Operator }.ToImmutableList();
            }
        }
    }
}