// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeycloakGroupMappingConfig.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Infrastructure.Keycloak.Models
{
    using global::Keycloak.Net.Models.Clients;

    using Newtonsoft.Json;

    public class KeycloakGroupMappingConfig : ClientConfig
    {
#pragma warning disable 8618
        [JsonProperty("full.path")]
        public string FullPath { get; set; }

        [JsonProperty("userinfo.token.claim")]
        public string CustomUserInfoTokenClaim { get; set; }

        [JsonProperty("claim.name")]
        public string CustomClaimName { get; set; }

        [JsonProperty("access.token.claim")]
        public string CustomAccessTokenClaim { get; set; }

        [JsonProperty("id.token.claim")]
        public string CustomIdTokenClaim { get; set; }

        [JsonProperty("user.attribute")]
        public new string UserAttribute { get; set; }

        [JsonProperty("jsonType.label")]
        public string JsonType { get; set; }

        [JsonProperty("multivalued")]
        public string MultiValued { get; set; }

        [JsonProperty("aggregate.attrs")]
        public string AggregateAttrs { get; set; }

#pragma warning restore 8618
    }
}