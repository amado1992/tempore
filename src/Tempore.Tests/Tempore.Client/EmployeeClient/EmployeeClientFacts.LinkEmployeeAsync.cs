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
using global::Tempore.Tests.Infraestructure.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

using Xunit;

using AddEmployeeFromDeviceRequest = global::Tempore.Client.AddEmployeeFromDeviceRequest;
using AgentRegistrationRequest = global::Tempore.Client.AgentRegistrationRequest;
using AgentState = global::Tempore.Client.AgentState;
using FileType = global::Tempore.Client.FileType;

public partial class EmployeeClientFacts
{
    [Collection(nameof(DockerCollection))]
    public class The_LinkEmployeeAsync_Method : EnvironmentTestBase
    {
        public The_LinkEmployeeAsync_Method(
            DockerEnvironment dockerEnvironment,
            TemporeServerWebApplicationFactory temporeServerApplicationFactory,
            TestEnvironmentInitializer testEnvironmentInitializer)
            : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
        {
        }

        public static IEnumerable<object[]> Request_With_Required_Data_Missing()
        {
            yield return new object[] { new EmployeeLinkRequest() };
            yield return new object[] { new EmployeeLinkRequest { EmployeeId = Guid.NewGuid(), EmployeeFromDeviceIds = new List<Guid>() } };
            yield return new object[] { new EmployeeLinkRequest { EmployeeId = Guid.NewGuid(), EmployeeFromDeviceIds = new List<Guid> { Guid.NewGuid(), Guid.Empty } } };
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(Guid.NewGuid(), Guid.NewGuid())));

            apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_EmployeeFromDevice_Not_Found_Async()
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                TestEnvironment.Tempore.Username,
                TestEnvironment.Tempore.Password);

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(Guid.NewGuid(), Guid.NewGuid())));

            apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_Employee_Not_Found_Async()
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

            await fileProcessingClient.ProcessAsync(fileId);

            var fileProcessingServiceFactory = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IFileProcessingServiceFactory>();
            var fileProcessingService = fileProcessingServiceFactory.Create(Storage.Entities.FileType.PayDay);

            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                     TestEnvironment.Tempore.Username,
                                     TestEnvironment.Tempore.Password);

            var fileRepository = this.TemporeServerApplicationFactory!.Services
                .GetRequiredService<IRepository<DataFile, ApplicationDbContext>>();

            var dataFile = await fileRepository.SingleOrDefaultAsync(file => file.Id == fileId);
            dataFile.Should().NotBeNull();

            var employeeFromFile = await fileProcessingService.GetEmployeesAsync(dataFile!).OrderBy(e => new Random().Next(1000)).FirstAsync();
            var employeeFromDeviceDto = new EmployeeFromDeviceDto
            {
                DeviceId = device!.Id,
                EmployeeIdOnDevice = employeeFromFile.ExternalId,
                FullName = employeeFromFile.FullName,
            };

            var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(Guid.NewGuid(), employeeFromDeviceId)));
            apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Succeeds_When_The_EmployeeFromDevice_And_Employee_Exist_Async()
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

            var employeeFromFile = await fileProcessingService.GetEmployeesAsync(dataFile!).OrderBy(e => new Random().Next(1000)).FirstAsync();
            var employeeRepository = this.TemporeServerApplicationFactory!.Services
                .GetRequiredService<IRepository<Employee, ApplicationDbContext>>();
            var employee = await employeeRepository.SingleAsync(e => e.FullName == employeeFromFile.FullName);

            var employeeFromDeviceDto = new EmployeeFromDeviceDto
            {
                DeviceId = device!.Id,
                EmployeeIdOnDevice = Guid.NewGuid().ToString(),
                FullName = employeeFromFile.FullName,
            };

            var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

            await employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(employee.Id, employeeFromDeviceId));
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_The_EmployeeFromDevice_Exist_And_Is_Linked_Async()
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

            var employeeFromFile = await fileProcessingService.GetEmployeesAsync(dataFile!).OrderBy(e => new Random().Next(1000)).FirstAsync();
            var employeeRepository = this.TemporeServerApplicationFactory!.Services
                .GetRequiredService<IRepository<Employee, ApplicationDbContext>>();

            var employee = await employeeRepository.SingleAsync(e => e.FullName == employeeFromFile.FullName);

            var employeeFromDeviceDto = new EmployeeFromDeviceDto
            {
                DeviceId = device!.Id,
                EmployeeIdOnDevice = Guid.NewGuid().ToString(),
                FullName = employeeFromFile.FullName,
            };
            var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

            await employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(employee.Id, employeeFromDeviceId));

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(employee.Id, employeeFromDeviceId)));

            apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Theory]
        [MemberData(nameof(Request_With_Required_Data_Missing))]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_Required_Data_Is_Missing_Async(EmployeeLinkRequest request)
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                     TestEnvironment.Tempore.Username,
                                     TestEnvironment.Tempore.Password);

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.LinkEmployeeAsync(request));
            apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
    }
}