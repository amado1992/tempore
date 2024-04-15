// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeycloakClientRoleMappingConfig.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Infrastructure.Keycloak.Models
{
    using global::Keycloak.Net.Models.Clients;

    using Newtonsoft.Json;

    /// <summary>
    /// The keycloak client role mapping config.
    /// </summary>
    public class KeycloakClientRoleMappingConfig : ClientConfig
    {
#pragma warning disable 8618
        /// <summary>
        /// Gets or sets the multi valued.
        /// </summary>
        [JsonProperty("multivalued")]
        public string MultiValued { get; set; }

        /// <summary>
        /// Gets or sets the user model client role mapping client id.
        /// </summary>
        [JsonProperty("usermodel.clientRoleMapping.clientId")]
        public string UserModelClientRoleMappingClientId { get; set; }

        /// <summary>
        /// Gets or sets the custom user info token claim.
        /// </summary>
        [JsonProperty("userinfo.token.claim")]
        public string CustomUserInfoTokenClaim { get; set; }

        /// <summary>
        /// Gets or sets the custom claim name.
        /// </summary>
        [JsonProperty("claim.name")]
        public string CustomClaimName { get; set; }

        /// <summary>
        /// Gets or sets the custom access token claim.
        /// </summary>
        [JsonProperty("access.token.claim")]
        public string CustomAccessTokenClaim { get; set; }

        /// <summary>
        /// Gets or sets the custom id token claim.
        /// </summary>
        [JsonProperty("id.token.claim")]
        public string CustomIdTokenClaim { get; set; }

        /// <summary>
        /// Gets or sets the json type label.
        /// </summary>
        [JsonProperty("jsonType.label")]
        public string JsonTypeLabel { get; set; }
#pragma warning restore 8618
    }
}