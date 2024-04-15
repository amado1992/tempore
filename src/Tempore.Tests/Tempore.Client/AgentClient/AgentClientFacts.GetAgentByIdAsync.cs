namespace Tempore.Tests.Tempore.Client.AgentClient
{
    extern alias TemporeAgent;

    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.AspNetCore.Http;

    using Xunit;

    public partial class AgentClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetAgentByIdAsync_Method : EnvironmentTestBase
        {
            public The_GetAgentByIdAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_When_Agent_Not_Found_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var exception = await Assert.ThrowsAsync<ApiException<ProblemDetails>>(() => agentClient.GetAgentByIdAsync(Guid.NewGuid()));
                exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }
        }
    }
}