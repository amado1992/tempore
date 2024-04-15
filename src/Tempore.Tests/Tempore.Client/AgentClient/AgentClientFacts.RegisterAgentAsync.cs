namespace Tempore.Tests.Tempore.Client.AgentClient
{
    extern alias TemporeAgent;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

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
        public class The_RegisterAgentAsync_Method : EnvironmentTestBase
        {
            public The_RegisterAgentAsync_Method(
                DockerEnvironment dockerEnvironment, TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> DeviceUpdateActions()
            {
                yield return new object[]
                             {
                                 (Action<DeviceRegistrationDto>)(device =>
                                                                    {
                                                                        device.Name = TestEnvironment.Device_00.NewName01;
                                                                    }),
                                 true,
                             };

                yield return new object[]
                             {
                                 (Action<DeviceRegistrationDto>)(device =>
                                                                    {
                                                                        device.Name = TestEnvironment.Device_00.NewName02;
                                                                        device.SerialNumber = null;
                                                                        device.MacAddress = null;
                                                                    }),
                                 false,
                             };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used()
            {
                // Arrange
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.InvalidUsername,
                                      TestEnvironment.Tempore.InvalidPassword);

                var devices = new List<DeviceRegistrationDto> { TestEnvironment.Device_00.RegistrationInstance };

                var agentDto = new AgentRegistrationDto
                {
                    Name = TestEnvironment.Agent.Name,
                    State = AgentState.Online,
                    Devices = devices,
                };

                // Act & Assert
                var apiException = await Assert.ThrowsAsync<ApiException>(
                                       () => agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto)));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_If_ConnectionId_Is_Empty_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var devices = new List<DeviceRegistrationDto> { TestEnvironment.Device_00.RegistrationInstance };

                var agentDto = new AgentRegistrationDto
                {
                    Name = TestEnvironment.Agent.Name,
                    State = AgentState.Online,
                    Devices = devices,
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(
                                       () => agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto)));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_The_Same_Id_For_Multiple_Registrations_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
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

                var agentI0d = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));
                var agentI1d = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                agentI0d.Should().Be(agentI1d);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Updates_The_ConnectionId_If_The_Agent_Is_Already_Registered_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
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

                agentDto.ConnectionId = Guid.NewGuid().ToString();
                await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                var agentByIdAsync = await agentClient.GetAgentByIdAsync(agentId);

                agentByIdAsync.ConnectionId.Should().Be(agentDto.ConnectionId);
            }

            [Theory]
            [MemberData(nameof(DeviceUpdateActions))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Synchronize_Agent_Devices_Properly_Async(
                Action<DeviceRegistrationDto> updateAction, bool useSameDeviceId)
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);
                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                       TestEnvironment.Tempore.Username,
                                       TestEnvironment.Tempore.Password);

                var deviceDto = TestEnvironment.Device_00.RegistrationInstance;

                var devices = new List<DeviceRegistrationDto> { deviceDto };

                var agentDto = new AgentRegistrationDto
                {
                    Name = TestEnvironment.Agent.Name,
                    State = AgentState.Online,
                    Devices = devices,
                    ConnectionId = Guid.NewGuid().ToString(),
                };

                var agentId = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));
                var deviceDtoPaginationResponse = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, int.MaxValue);
                var device = deviceDtoPaginationResponse.Items.FirstOrDefault();
                device.Should().NotBeNull();
                var deviceId = device!.Id;

                updateAction(deviceDto);

                await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                deviceDtoPaginationResponse = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, int.MaxValue);
                deviceDtoPaginationResponse.Items.Count.Should().Be(1);
                device = deviceDtoPaginationResponse.Items.FirstOrDefault();
                device.Should().NotBeNull();

                device!.Name.Should().Be(deviceDto.Name);
                useSameDeviceId.Should().Be(device.Id == deviceId);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_Registering_Offline_Devices_With_Null_Values_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceRegistrationDto01 = new DeviceRegistrationDto
                {
                    Name = "An Offline Device 01",
                    State = DeviceState.Offline,
                };

                var deviceRegistrationDto02 = new DeviceRegistrationDto
                {
                    Name = "An Offline Device 02",
                    State = DeviceState.Offline,
                };

                var deviceRegistrations = new List<DeviceRegistrationDto>
                                             {
                                                 deviceRegistrationDto01,
                                                 deviceRegistrationDto02,
                                             };

                var agentDto = new AgentRegistrationDto
                {
                    Name = TestEnvironment.Agent.Name,
                    State = AgentState.Online,
                    Devices = deviceRegistrations,
                    ConnectionId = Guid.NewGuid().ToString(),
                };

                var agentId = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                agentDto.ConnectionId = Guid.NewGuid().ToString();
                await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceDtoPaginationResponse = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, int.MaxValue);
                deviceDtoPaginationResponse.Items.Should().NotBeEmpty();

                deviceRegistrations.Should().AllSatisfy(
                    registration =>
                    {
                        var device = deviceDtoPaginationResponse.Items.FirstOrDefault(
                            deviceDto => deviceDto.Name == registration.Name
                                   && deviceDto.State == registration.State);
                        device.Should().NotBeNull();
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_Updating_Offline_Devices_With_Null_MacAddress_And_SerialNumber_First_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceRegistrationDto01 = new DeviceRegistrationDto
                {
                    Name = $"An Offline Device {Guid.NewGuid()}",
                    State = DeviceState.Offline,
                };

                var deviceRegistrations = new List<DeviceRegistrationDto>
                                             {
                                                 deviceRegistrationDto01,
                                             };

                var agentDto = new AgentRegistrationDto
                {
                    Name = TestEnvironment.Agent.Name,
                    State = AgentState.Online,
                    Devices = deviceRegistrations,
                    ConnectionId = Guid.NewGuid().ToString(),
                };

                var agentId = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                agentDto.ConnectionId = Guid.NewGuid().ToString();

                var faker = new Faker();
                deviceRegistrationDto01.MacAddress = faker.Internet.Mac();
                deviceRegistrationDto01.SerialNumber = Guid.NewGuid().ToString();
                deviceRegistrationDto01.State = DeviceState.Online;

                await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceDtoPaginationResponse = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, int.MaxValue);
                deviceDtoPaginationResponse.Items.Should().NotBeEmpty();

                deviceRegistrations.Should().AllSatisfy(
                    registration =>
                    {
                        var device = deviceDtoPaginationResponse.Items
                            .FirstOrDefault(deviceDto => deviceDto.Name == registration.Name
                                                         && deviceDto.State == registration.State
                                                         && deviceDto.MacAddress == registration.MacAddress
                                                         && deviceDto.SerialNumber == registration.SerialNumber);
                        device.Should().NotBeNull();
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Changes_The_Agent_State_Properly_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceRegistrationDto01 = new DeviceRegistrationDto
                {
                    Name = "An Offline Device 01",
                    State = DeviceState.Offline,
                };

                var deviceRegistrationDto02 = new DeviceRegistrationDto
                {
                    Name = "An Offline Device 02",
                    State = DeviceState.Offline,
                };

                var deviceRegistrations = new List<DeviceRegistrationDto>
                                             {
                                                 deviceRegistrationDto01,
                                                 deviceRegistrationDto02,
                                             };

                var agentDto = new AgentRegistrationDto
                {
                    Name = "An Offline Agent",
                    State = AgentState.Offline,
                    Devices = deviceRegistrations,
                    ConnectionId = Guid.NewGuid().ToString(),
                };

                var agentId = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                agentDto.State = AgentState.Online;
                await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                var agent = await agentClient.GetAgentByIdAsync(agentId);
                agent.State.Should().Be(AgentState.Online);
            }
        }
    }
}