// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeycloakAudienceConfig.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Infrastructure.Keycloak.Models
{
    using global::Keycloak.Net.Models.ProtocolMappers;

    using Newtonsoft.Json;

    public class KeycloakAudienceConfig : Config
    {
        [JsonProperty("access.token.claim")]
#pragma warning disable 8618
        public string AddToAccessToken { get; set; }

#pragma warning restore 8618
        [JsonProperty("included.client.audience")]
#pragma warning disable 8618
        public string IncludedClientAudience { get; set; }
#pragma warning restore 8618
    }
}