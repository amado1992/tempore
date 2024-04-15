namespace Tempore.Tests.Tempore.Client.EmployeeClient;

extern alias TemporeServer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

using Xunit;

using AgentState = global::Tempore.Client.AgentState;
using FileType = global::Tempore.Client.FileType;

public partial class EmployeeClientFacts
{
    [Collection(nameof(DockerCollection))]
    public class The_UnlinkEmployeeAsync_Method : EnvironmentTestBase
    {
        public The_UnlinkEmployeeAsync_Method(
            DockerEnvironment dockerEnvironment,
            TemporeServerWebApplicationFactory temporeServerApplicationFactory,
            TestEnvironmentInitializer testEnvironmentInitializer)
            : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
        {
        }

        public static IEnumerable<object[]> Request_With_Missing_Required_Data()
        {
            yield return new object[] { new EmployeeUnlinkRequest() };
            yield return new object[] { EmployeeUnlinkRequest.Create(Guid.NewGuid(), Guid.Empty) };
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(Guid.NewGuid())));

            apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_EmployeeFromDevice_Not_Found_Async()
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                TestEnvironment.Tempore.Username,
                TestEnvironment.Tempore.Password);

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(Guid.NewGuid())));

            apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Theory]
        [MemberData(nameof(Request_With_Missing_Required_Data))]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_Required_Data_Is_Missing_Async(EmployeeUnlinkRequest request)
        {
            var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                     TestEnvironment.Tempore.Username,
                                     TestEnvironment.Tempore.Password);

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.UnlinkEmployeeAsync(request));
            apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Unlinks_Existing_Linked_EmployeeFromDevice()
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

            var agentRegistrationRequest = AgentRegistrationRequest.Create(agentDto);

            var agentId = await agentClient.RegisterAgentAsync(agentRegistrationRequest);

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
            await employeeClient.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(employeeFromDeviceId));
        }

        [Fact]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_The_EmployeeFromDevice_Is_Already_Unlinked_Async()
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

            var agentRegistrationRequest = AgentRegistrationRequest.Create(agentDto);

            var agentId = await agentClient.RegisterAgentAsync(agentRegistrationRequest);

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
            var employeeFromDeviceDto = new EmployeeFromDeviceDto
            {
                DeviceId = device!.Id,
                EmployeeIdOnDevice = Guid.NewGuid().ToString(),
                FullName = employeeFromFile.FullName,
            };

            var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

            var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(employeeFromDeviceId)));
            apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
    }
}