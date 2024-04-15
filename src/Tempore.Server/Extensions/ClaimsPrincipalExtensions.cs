// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimsPrincipalExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using System.Security.Claims;

    /// <summary>
    /// The claims principal extensions.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the preferred username.
        /// </summary>
        /// <param name="principal">
        /// The principal.
        /// </param>
        /// <returns>
        /// The preferred username.
        /// </returns>
        public static string? GetPreferredUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("preferred_username");
        }

        /// <summary>
        /// Gets the name identifier.
        /// </summary>
        /// <param name="principal">
        /// The principal.
        /// </param>
        /// <returns>
        /// The name identifier.
        /// </returns>
        public static Guid GetNameIdentifier(this ClaimsPrincipal principal)
        {
            return Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}