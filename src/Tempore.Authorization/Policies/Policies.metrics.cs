// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.metrics.cs" company="Port Hope Investment S.A.">
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
        /// The Metrics policies.
        /// </summary>
        public class Metrics
        {
            /// <summary>
            /// The administrator.
            /// </summary>
            public static readonly PolicyInfo Administrator = new RoleBasedPolicyInfo(
                Roles.Metrics.Administrator,
                "User has administrator access on metrics",
                roles: GetAdministratorRoles());

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Viewer = new RoleBasedPolicyInfo(
                Roles.Metrics.Viewer,
                "User has view access on metrics");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Calculator = new RoleBasedPolicyInfo(
                Roles.Metrics.Calculator,
                "User has calculation access on metrics");

            /// <summary>
            /// The viewer.
            /// </summary>
            public static readonly PolicyInfo Exporter = new RoleBasedPolicyInfo(
                Roles.Metrics.Exporter,
                "User has export access on metrics");

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
                           Roles.Metrics.Viewer,
                           Roles.Metrics.Exporter,
                           Roles.Metrics.Calculator,
                       }.ToImmutableList();
            }
        }
    }
}