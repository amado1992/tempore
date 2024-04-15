// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.agent.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization.Policies
{
    using System.Collections.Immutable;

    using Tempore.Authorization;

    /// <summary>
    /// The policies.
    /// </summary>
    public static partial class Policies
    {
        /// <summary>
        /// The agent.
        /// </summary>
        public class Agent
        {
            /// <summary>
            /// The Tempore Agent policy.
            /// </summary>
            public static readonly PolicyInfo TemporeAgent = new RoleBasedPolicyInfo(
                Authorization.Roles.Roles.Agent.TemporeAgent,
                "User is a tempore agent",
                roles: GetTemporeAgentRoles());

            /// <summary>
            /// Gets the tempore agent roles.
            /// </summary>
            /// <returns>
            /// The <see cref="IEnumerable{String}"/>.
            /// </returns>
            private static IImmutableList<string> GetTemporeAgentRoles()
            {
                return new[]
                       {
                           Roles.Roles.Agents.Administrator,
                           Roles.Roles.Devices.Administrator,
                           Roles.Roles.Timestamps.Administrator,
                           Roles.Roles.Employees.Administrator,
                       }.ToImmutableList();
            }
        }
    }
}