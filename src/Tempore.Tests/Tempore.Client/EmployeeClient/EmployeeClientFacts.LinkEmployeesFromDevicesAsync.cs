namespace Tempore.Tests.Tempore.Client.EmployeeClient;

extern alias TemporeServer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using global::Tempore.Client;
using global::Tempore.Processing.Services.Interfaces;
using global::Tempore.Storage;
using global::Tempore.Storage.Entities;
using global::Tempore.Tests.Fixtures;
using global::Tempore.Tests.Fixtures.Postgres;
using global::Tempore.Tests.Infraestructure;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

using TemporeServer::Tempore.Server.Notifications.Employees;

using Xunit;

using AgentState = global::Tempore.Client.AgentState;
using FileType = global::Tempore.Client.FileType;
using TaskHelper = global::Tempore.Tests.Infraestructure.Helpers.TaskHelper;
using TimeSpan = System.TimeSpan;

public partial class EmployeeClientFacts
{
    [Collection(nameof(DockerCollection))]
    public class The_LinkEmployeesFromDevicesAsync_Method : EnvironmentTestBase
    {
        public The_LinkEmployeesFromDevicesAsync_Method(
            DockerEnvironment dockerEnvironment,
            TemporeServerWebApplicationFactory temporeServerApplicationFactory,
            TestEnvironmentInitializer testEnvironmentInitializer)
            : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
        {
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode__When_Invalid_Credentials_Are_Used_Async()
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.LinkEmployeesAsync());

            apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Succeeds_And_Returns_Non_Empty_JobId_With_Valid_Credentials_Async()
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
            var jobId = await employeeClient.LinkEmployeesAsync();

            jobId.Should().NotBeEmpty();
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Notifies_Success_Upon_Completion_Via_NotificationService_Async()
        {
            var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                  TestEnvironment.Tempore.Username,
                                  TestEnvironment.Tempore.Password);

            var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                   TestEnvironment.Tempore.Username,
                                   TestEnvironment.Tempore.Password);

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

            var devicesOfAgent = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, 100);
            var device = devicesOfAgent.Items.FirstOrDefault();
            device.Should().NotBeNull();

            var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
            await using var manifestResourceStream = typeof(EmployeeClientFacts).Assembly.GetManifestResourceStream(
                "Tempore.Tests.Resources.employees.xlsx");

            var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
            fileId.Should().NotBeEmpty();

            await TaskHelper.RepeatAsync(
                () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

            await fileProcessingClient.ProcessAsync(fileId);

            await TaskHelper.RepeatAsync(
                () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

            var fileProcessingServiceFactory = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IFileProcessingServiceFactory>();
            var fileProcessingService = fileProcessingServiceFactory.Create(Storage.Entities.FileType.PayDay);

            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                     TestEnvironment.Tempore.Username,
                                     TestEnvironment.Tempore.Password);

            var fileRepository = this.TemporeServerApplicationFactory!.Services
                .GetRequiredService<IRepository<DataFile, ApplicationDbContext>>();

            var dataFile = await fileRepository.SingleOrDefaultAsync(file => file.Id == fileId);
            dataFile.Should().NotBeNull();

            await foreach (var employee in fileProcessingService.GetEmployeesAsync(dataFile!))
            {
                var employeeFromDeviceDto = new EmployeeFromDeviceDto
                {
                    DeviceId = device!.Id,
                    EmployeeIdOnDevice = employee.ExternalId,
                    FullName = employee.FullName,
                };

                await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));
            }

            await TaskHelper.RepeatAsync(() => employeeClient.IsLinkEmployeesProcessRunningAsync());

            // Act
            this.TemporeServerApplicationFactory.NotificationServiceMock.Reset();

            await employeeClient.LinkEmployeesAsync();

            // Assert
            await this.TemporeServerApplicationFactory.NotificationServiceMock.WaitAndVerifyAsync(TimeSpan.FromSeconds(30), service => service.SuccessAsync<EmployeesLinkProcessCompletedNotification>(TestEnvironment.Tempore.Username, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status409Conflict_StatusCode_The_Job_Is_Already_Scheduled_Async()
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
            var device = devicesOfAgent.Items.FirstOrDefault();
            device.Should().NotBeNull();

            var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
            await using var manifestResourceStream = typeof(EmployeeClientFacts).Assembly.GetManifestResourceStream(
                "Tempore.Tests.Resources.employees.xlsx");

            var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
            fileId.Should().NotBeEmpty();

            await TaskHelper.RepeatAsync(
                () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

            await fileProcessingClient.ProcessAsync(fileId);

            await TaskHelper.RepeatAsync(
                () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

            var fileProcessingServiceFactory = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IFileProcessingServiceFactory>();
            var fileProcessingService = fileProcessingServiceFactory.Create(Storage.Entities.FileType.PayDay);

            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                     TestEnvironment.Tempore.Username,
                                     TestEnvironment.Tempore.Password);

            var fileRepository = this.TemporeServerApplicationFactory!.Services
                .GetRequiredService<IRepository<DataFile, ApplicationDbContext>>();

            var dataFile = await fileRepository.SingleOrDefaultAsync(file => file.Id == fileId);
            dataFile.Should().NotBeNull();

            await foreach (var employee in fileProcessingService.GetEmployeesAsync(dataFile!))
            {
                var employeeFromDeviceDto = new EmployeeFromDeviceDto
                {
                    DeviceId = device!.Id,
                    EmployeeIdOnDevice = employee.ExternalId,
                    FullName = employee.FullName,
                };

                await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));
            }

            await TaskHelper.RepeatAsync(
                () => employeeClient.IsLinkEmployeesProcessRunningAsync());

            await employeeClient.LinkEmployeesAsync();
            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.LinkEmployeesAsync());
            apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }
    }
}