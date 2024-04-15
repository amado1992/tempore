namespace Tempore.Tests.Fixtures.Postgres;

extern alias TemporeAgent;

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using global::Keycloak.Net;

using TemporeAgent::Tempore.Agent.Services.Interfaces;

/// <summary>
/// The token resolver.
/// </summary>
public class IntegrationTestsTokenResolver : ITokenResolver
{
    /// <summary>
    /// The resolve async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<string> ResolveAsync()
    {
        var keycloakClient = new KeycloakClient(TestEnvironment.Keycloak.BaseUrl, TestEnvironment.Keycloak.Username, TestEnvironment.Keycloak.Password);
        var credentials = await keycloakClient.GetClientSecretAsync("master", "tempore-agent");

        using var httpClient = new HttpClient();

        // Read more data from configuration?
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = "tempore-agent",
            ["grant_type"] = "client_credentials",
            ["scope"] = "openid profile tempore-api",
            ["client_secret"] = credentials.Value, // This is a secret.
        };

        using var formUrlEncodedContent = new FormUrlEncodedContent(parameters);
        using var httpResponseMessage = await httpClient.PostAsync(
                                      $"{TestEnvironment.Keycloak.Autority}/protocol/openid-connect/token",
                                      formUrlEncodedContent);
        httpResponseMessage.EnsureSuccessStatusCode();

        await using var stream = await httpResponseMessage.Content.ReadAsStreamAsync(CancellationToken.None);

        using var document = await JsonDocument.ParseAsync(stream);

        return document.RootElement.GetProperty("access_token").GetString()!;
    }
}