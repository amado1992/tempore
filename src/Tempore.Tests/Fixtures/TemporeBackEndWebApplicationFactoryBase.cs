namespace Tempore.Integration.Tests.Fixtures
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect;

    using Catel.Caching;
    using Catel.Caching.Policies;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using Newtonsoft.Json.Linq;

    using Tempore.Client.Services.Interfaces;
    using Tempore.HealthChecks.Services;
    using Tempore.Hosting.Services.Interfaces;
    using Tempore.Tests.Fixtures;
    using Tempore.Tests.Fixtures.Interfaces;

    using TemporeServer::Tempore.Server.Hubs;
    using TemporeServer::Tempore.Server.Services.Interfaces;

    public abstract class TemporeBackEndWebApplicationFactoryBase<TProgram> : WebApplicationFactory<TProgram>, ITemporeBackEndWebApplicationFactory
        where TProgram : class
    {
        private readonly ICacheStorage<string, HttpClient> httpClientCache = new CacheStorage<string, HttpClient>();

        public Mock<IEnvironmentVariableService> EnvironmentVariableServiceMock { get; } = new Mock<IEnvironmentVariableService>();

        public Mock<INotificationService> NotificationServiceMock { get; } = new Mock<INotificationService>();

        public Mock<IHubLifetimeManager<AgentHub>> AgentHubLifetimeManagerMock { get; } = new Mock<IHubLifetimeManager<AgentHub>>();

        public Mock<IHubContext<AgentHub, IAgentReceiver>> AgentHubContextMock { get; } = new Mock<IHubContext<AgentHub, IAgentReceiver>>();

        public async Task<HttpClient> CreateHttpClientAsync(string username = "", string password = "", bool @override = false, TimeSpan timeout = default)
        {
            string key = $"username:{username};password:{password}";
            return await this.httpClientCache.GetFromCacheOrFetchAsync(
                       key,
                       async () =>
                       {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                           var httpClient = this.CreateClient();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                           string? accessToken = null;
                           if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                           {
                               var healthCheckService = new HttpHealthCheckService(
                                   NullLogger<HttpHealthCheckService>.Instance,
                                   TestEnvironment.Keycloak.Url);
                               await healthCheckService.WaitAsync();

                               using var keycloakHttpClient = new HttpClient();

                               var formData = new Dictionary<string, string>
                                              {
                                                  { "client_id", "tempore-client" },
                                                  { "username", username },
                                                  { "password", password },
                                                  { "grant_type", "password" },
                                                  { "scope", "openid profile tempore-api" },
                                              };

                               using var formUrlEncodedContent = new FormUrlEncodedContent(formData);
                               using var httpResponseMessage = await keycloakHttpClient.PostAsync(
                                                                   TestEnvironment.Keycloak.TokenUrl,
                                                                   formUrlEncodedContent);
                               var content = await httpResponseMessage.Content.ReadAsStringAsync();
                               var parsedContent = JObject.Parse(content);
                               accessToken = parsedContent.GetValue("access_token")?.Value<string>();
                           }

                           if (!string.IsNullOrWhiteSpace(accessToken))
                           {
                               httpClient.SetBearerToken(accessToken);
                           }

                           httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
                           var twoLetterIsoLanguageName = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                           httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(twoLetterIsoLanguageName);
                           if (timeout != default)
                           {
                               httpClient.Timeout = timeout;
                           }

                           return httpClient;
                       },
                       ExpirationPolicy.Sliding(TimeSpan.FromSeconds(20)),
                       @override);
        }

        public async Task<TClient> CreateClientAsync<TClient>(string username = "", string password = "", bool @override = false, TimeSpan timeout = default)
        {
#pragma warning disable IDISP001 // Dispose created
            var httpClient = await this.CreateHttpClientAsync(username, password, @override, timeout);
#pragma warning restore IDISP001 // Dispose created

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
            return (TClient)Activator.CreateInstance(typeof(TClient), httpClient);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
    }
}