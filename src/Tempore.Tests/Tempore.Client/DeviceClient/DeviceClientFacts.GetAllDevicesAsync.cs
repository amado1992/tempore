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

using Microsoft.AspNetCore.Http;

using Xunit;

public partial class DeviceClientFacts
{
    [Collection(nameof(DockerCollection))]
    public class The_GetAllDevicesAsync_Method : EnvironmentTestBase
    {
        public The_GetAllDevicesAsync_Method(
            DockerEnvironment dockerEnvironment,
            TemporeServerWebApplicationFactory temporeServerApplicationFactory,
            TestEnvironmentInitializer testEnvironmentInitializer)
            : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
        {
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_In_GetAllDevices_Async()
        {
            var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>();

            var apiException = await Assert.ThrowsAsync<ApiException>(() => deviceClient.GetAllDevicesAsync());

            apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Returns_All_The_Registered_Devices_Async()
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

            await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

            var devicesAll = await deviceClient.GetAllDevicesAsync();

            devicesAll.Should().NotBeNull();
            devicesAll.Count.Should().Be(devices.Count);
        }
    }
}