namespace Tempore.Tests.Tempore.Client.AgentCommandClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Client.Services.Interfaces;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Mapster;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.SignalR;

    using Moq;

    using Xunit;

    public partial class AgentCommandClientFact
    {
        [Collection(nameof(DockerCollection))]
        public class The_UploadEmployeesFromDevicesAsync_Method : EnvironmentTestBase
        {
            public The_UploadEmployeesFromDevicesAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var agentCommandClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentCommandClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => agentCommandClient.UploadEmployeesFromDevicesAsync(new UploadEmployeesFromDevicesRequest()));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_If_DeviceIds_Is_Empty_Async()
            {
                var agentCommandClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentCommandClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => agentCommandClient.UploadEmployeesFromDevicesAsync(new UploadEmployeesFromDevicesRequest { DeviceIds = new List<Guid>() }));
                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_If_DeviceIds_Is_Null_Async()
            {
                var agentCommandClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentCommandClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => agentCommandClient.UploadEmployeesFromDevicesAsync(new UploadEmployeesFromDevicesRequest { DeviceIds = null }));
                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Calls_IsAlive_From_AgentHubLifetimeManager_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var devices = new List<DeviceDto>
                              {
                                  TestEnvironment.Device_00.Instance,
                                  TestEnvironment.Device_01.Instance,
                              }.Adapt<List<DeviceRegistrationDto>>();

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

                var registeredDevices = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, int.MaxValue);

                var agentCommandClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentCommandClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                this.TemporeServerApplicationFactory.AgentHubLifetimeManagerMock.Reset();

                await agentCommandClient.UploadEmployeesFromDevicesAsync(UploadEmployeesFromDevicesRequest.Create(
                    registeredDevices.Items.Select(d => d.Id).ToArray()));

                this.TemporeServerApplicationFactory.AgentHubLifetimeManagerMock.Verify(manager => manager.IsAlive(agentDto.ConnectionId), Times.AtLeastOnce);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Calls_UploadEmployeesAsync_Of_AgentReceiver_For_Each_Device_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var devices = new List<DeviceRegistrationDto>
                              {
                                  TestEnvironment.Device_00.RegistrationInstance,
                                  TestEnvironment.Device_01.RegistrationInstance,
                              };

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

                var registeredDevices = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, int.MaxValue);

                this.TemporeServerApplicationFactory.AgentHubLifetimeManagerMock.Setup(manager => manager.IsAlive(agentDto.ConnectionId)).Returns(true);
                var hubClientsMock = new Mock<IHubClients<IAgentReceiver>>();
                var agentReceiverMock = new Mock<IAgentReceiver>();

                hubClientsMock.Setup(clients => clients.Client(agentDto.ConnectionId)).Returns(agentReceiverMock.Object);
                this.TemporeServerApplicationFactory.AgentHubContextMock.Setup(hubContext => hubContext.Clients).Returns(hubClientsMock.Object);

                var agentCommandClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentCommandClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                await agentCommandClient.UploadEmployeesFromDevicesAsync(UploadEmployeesFromDevicesRequest.Create(
                    registeredDevices.Items.Select(d => d.Id).ToArray()));

                foreach (var deviceDto in registeredDevices.Items)
                {
                    agentReceiverMock.Verify(agentReceiver => agentReceiver.UploadEmployeesAsync(deviceDto.Id), Times.Once);
                }
            }
        }
    }
}