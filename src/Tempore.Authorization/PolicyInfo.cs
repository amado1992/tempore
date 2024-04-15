// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolicyInfo.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// The policy info.
    /// </summary>
    public class PolicyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="authorizationRequirement">
        /// The authorization requirement.
        /// </param>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        public PolicyInfo(
            string name,
            string? description,
            IAuthorizationRequirement authorizationRequirement,
            string? groupName = null)
        {
            this.Name = name;
            this.Description = description;
            this.AuthorizationRequirement = authorizationRequirement;
            this.GroupName = groupName;
        }

        /// <summary>
        /// Gets the authorization requirement.
        /// </summary>
        public IAuthorizationRequirement AuthorizationRequirement { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string? GroupName { get; set; }
    }
}