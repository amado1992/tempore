namespace Tempore.Tests.Tempore.Client.AgentClient
{
    extern alias TemporeAgent;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Xunit;

    public partial class AgentClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetAgentsAsync_Method : EnvironmentTestBase
        {
            public The_GetAgentsAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_The_Registered_Agents_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var devices = new List<DeviceRegistrationDto>
                              {
                                  TestEnvironment.Device_00.RegistrationInstance,
                              };

                var agentDto = new AgentRegistrationDto
                {
                    Name = TestEnvironment.Agent.Name,
                    State = AgentState.Online,
                    Devices = devices,
                    ConnectionId = Guid.NewGuid().ToString(),
                };

                var agentId = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                var response = await agentClient.GetAgentsAsync(0, 100);

                response.Items.Count.Should().Be(1);

                var registeredAgent = response.Items.First();
                registeredAgent.Id.Should().Be(agentId);
                registeredAgent.Should().BeEquivalentTo(agentDto, options => options
                    .Excluding(dto => dto.Devices));
            }
        }
    }
}