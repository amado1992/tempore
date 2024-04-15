// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleBasedPolicyInfo.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization
{
    using System.Collections.Immutable;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Infrastructure;

    /// <summary>
    /// The Role based policy info.
    /// </summary>
    public class RoleBasedPolicyInfo : PolicyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleBasedPolicyInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <param name="customAuthorizationRequirement">
        /// The custom authorization requirement.
        /// </param>
        public RoleBasedPolicyInfo(
            string name,
            string? description = null,
            string? groupName = null,
            IImmutableList<string>? roles = null,
            IAuthorizationRequirement? customAuthorizationRequirement = null)
            : base(
                name,
                description,
                customAuthorizationRequirement ?? new RolesAuthorizationRequirement(new[] { name }),
                groupName)
        {
            this.Roles = roles ?? Array.Empty<string>().ToImmutableArray();
        }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        public IImmutableList<string> Roles { get; }
    }
}