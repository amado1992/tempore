namespace Tempore.Tests.Tempore.Client.EmployeeClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using FluentAssertions;

using global::Tempore.Client;
using global::Tempore.Tests.Fixtures;
using global::Tempore.Tests.Fixtures.Postgres;
using global::Tempore.Tests.Infraestructure;

using Microsoft.AspNetCore.Http;

using Xunit;

public partial class EmployeeClientFacts
{
    [Collection(nameof(DockerCollection))]
    public class The_AddEmployeeFromDeviceAsync_Method : EnvironmentTestBase
    {
        public The_AddEmployeeFromDeviceAsync_Method(
            DockerEnvironment dockerEnvironment,
            TemporeServerWebApplicationFactory temporeServerApplicationFactory,
            TestEnvironmentInitializer testEnvironmentInitializer)
            : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
        {
        }

        public static IEnumerable<object[]> Data()
        {
            yield return new object[]
                         {
                             new EmployeeFromDeviceDto()
                             {
                                 EmployeeIdOnDevice = string.Empty,
                                 DeviceId = Guid.NewGuid(),
                                 FullName = "Jane Doe",
                             },
                         };

            yield return new object[]
                         {
                             new EmployeeFromDeviceDto()
                             {
                                 EmployeeIdOnDevice = Guid.NewGuid().ToString(),
                                 DeviceId = Guid.Empty,
                                 FullName = "Jane Doe",
                             },
                         };

            yield return new object[]
                         {
                             new EmployeeFromDeviceDto()
                             {
                                 EmployeeIdOnDevice = Guid.NewGuid().ToString(),
                                 DeviceId = Guid.Empty,
                                 FullName = string.Empty,
                             },
                         };
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode__When_Invalid_Credentials_Are_Used_Async()
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AddEmployeeFromDeviceAsync(new AddEmployeeFromDeviceRequest()));

            apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Returns_The_Same_Id_For_Multiple_Employee_Load_AsyncAsync()
        {
            var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                  TestEnvironment.Tempore.Username,
                                  TestEnvironment.Tempore.Password);

            var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                   TestEnvironment.Tempore.Username,
                                   TestEnvironment.Tempore.Password);

            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<global::Tempore.Client.EmployeeClient>(
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
            var device = devicesOfAgent.Items.FirstOrDefault();
            device.Should().NotBeNull();

            var employeeFromDeviceDto = new EmployeeFromDeviceDto
            {
                EmployeeIdOnDevice = "123456",
                DeviceId = device!.Id,
                FullName = "Jane Doe",
            };

            var employeeId01 = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));
            var employeeId02 = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

            employeeId02.Should().Be(employeeId01);
        }

        [Theory]
        [MemberData(nameof(Data))]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status400BadRequest_When_Required_Fields_Are_Empty_Async(EmployeeFromDeviceDto employeeFromDevice)
        {
            var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                  TestEnvironment.Tempore.Username,
                                  TestEnvironment.Tempore.Password);

            var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                   TestEnvironment.Tempore.Username,
                                   TestEnvironment.Tempore.Password);

            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<global::Tempore.Client.EmployeeClient>(
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
            var device = devicesOfAgent.Items.FirstOrDefault();
            device.Should().NotBeNull();

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDevice)));
            apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
    }
}