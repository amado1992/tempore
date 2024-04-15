namespace Tempore.Tests.Tempore.Server.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Infrastructure.Keycloak.Services.Interfaces;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    public class KeycloakInitializationServiceFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_InitializeAsync_Method : EnvironmentTestBase
        {
            public The_InitializeAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory, null)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Creates_The_Tempore_User_Async()
            {
                var keycloakService = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IKeycloakClientFactory>();
                var (keycloakClient, realmName) = await keycloakService.CreateAsync();

                var user = (await keycloakClient.GetUsersAsync(realmName, username: TestEnvironment.Tempore.Username))
                    .FirstOrDefault(user => user.UserName == TestEnvironment.Tempore.Username);

                user.Should().NotBeNull();
            }
        }
    }
}