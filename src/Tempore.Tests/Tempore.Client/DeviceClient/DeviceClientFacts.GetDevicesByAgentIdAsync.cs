namespace Tempore.Tests.Tempore.Client.DeviceClient;

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

public partial class DeviceClientFacts
{
    [Collection(nameof(DockerCollection))]
    public class The_GetDevicesByAgentIdAsync_Method : EnvironmentTestBase
    {
        public The_GetDevicesByAgentIdAsync_Method(
            DockerEnvironment dockerEnvironment,
            TemporeServerWebApplicationFactory temporeServerApplicationFactory,
            TestEnvironmentInitializer testEnvironmentInitializer)
            : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
        {
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Returns_The_Registered_Devices_For_An_Agent_Async()
        {
            var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                  TestEnvironment.Tempore.Username,
                                  TestEnvironment.Tempore.Password);
            var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                   TestEnvironment.Tempore.Username,
                                   TestEnvironment.Tempore.Password);

            var devices = new List<DeviceRegistrationDto> { TestEnvironment.Device_00.RegistrationInstance };

            var agentDto = new AgentRegistrationDto
            {
                Name = TestEnvironment.Agent.Name,
                State = AgentState.Online,
                Devices = devices,
                ConnectionId = Guid.NewGuid().ToString(),
            };

            var agentId = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

            var devicesOfAgent = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, 100);

            devicesOfAgent.Should().NotBeNull();
            devicesOfAgent.Count.Should().Be(devices.Count);

            foreach (var deviceOfAgent in devicesOfAgent.Items)
            {
                var deviceByName = devices.FirstOrDefault(d => d.Name == deviceOfAgent.Name);
                deviceByName.Should().BeEquivalentTo(
                    deviceOfAgent,
                    options => options.ComparingByMembers<DeviceDto>().Excluding(dto => dto.Id)
                        .Excluding(dto => dto.Agent).Excluding(dto => dto.AgentId)
                        .Excluding(dto => dto.EmployeesFromDevices));
            }
        }
    }
}