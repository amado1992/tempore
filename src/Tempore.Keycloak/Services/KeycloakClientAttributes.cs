// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeycloakClientAttributes.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Keycloak.Services
{
    /// <summary>
    /// The keycloak client attributes.
    /// </summary>
    public static class KeycloakClientAttributes
    {
        /// <summary>
        /// The post logout redirect uris.
        /// </summary>
        public const string PostLogoutRedirectUris = "post.logout.redirect.uris";

        /// <summary>
        /// The pkce code challenge method.
        /// </summary>
        public const string PkceCodeChallengeMethod = "pkce.code.challenge.method";
    }
}