namespace Tempore.Tests.Tempore.Client.EmployeeClient
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

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
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Services;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using FileType = global::Tempore.Client.FileType;

    public partial class EmployeeClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetEmployeesFromDevicesAsync_Method : EnvironmentTestBase
        {
            public The_GetEmployeesFromDevicesAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            // TODO: Move this to another place?
            public static IEnumerable<object[]> InvalidSkipAndTake()
            {
                yield return new object[] { -1, 0 };
                yield return new object[] { 0, -1 };
                yield return new object[] { -1, -1 };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode__When_Invalid_Credentials_Are_Used_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.GetEmployeesFromDevicesAsync(new GetEmployeesFromDevicesRequest { Take = 100, Skip = 0 }));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_With_Valid_Credentials_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(new GetEmployeesFromDevicesRequest { Take = 100, Skip = 0 });

                paginationResponse.Should().NotBeNull();
            }

            [Theory]
            [MemberData(nameof(InvalidSkipAndTake))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_For_Invalid_Values_Async(int skip, int take)
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);
                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.GetEmployeesFromDevicesAsync(new GetEmployeesFromDevicesRequest { Skip = skip, Take = take }));
                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Devices_Filtered_By_Employee_FullName_And_Linked_State_Async()
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
                    EmployeeIdOnDevice = employeeFromFile.ExternalId,
                    FullName = employeeFromFile.FullName,
                };

                var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

                await employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(employee.Id, employeeFromDeviceId));

                var employeeFromDeviceRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<EmployeeFromDevice, ApplicationDbContext>>();
                var employeeFromDevice = await employeeFromDeviceRepository.SingleAsync(e => e.Id == employeeFromDeviceId);

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = employeeFromDevice.FullName,
                    IsLinked = true,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);

                paginationResponse.Items.Should().NotBeEmpty().And.AllSatisfy(
                    dto =>
                    {
                        dto.FullName.Contains(request.SearchText, StringComparison.InvariantCultureIgnoreCase).Should().Be(true);
                        dto.IsLinked.Should().Be(request.IsLinked.Value);
                    });
            }

            [Fact(Skip = "Improve this test later")]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_Pagination_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                       TestEnvironment.Tempore.Username,
                                       TestEnvironment.Tempore.Password);

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
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

                var employeeIds = new List<Guid>();

                const int ExpectedEmployees = 100;
                var employeeFromDeviceDtos = new List<EmployeeFromDeviceDto>();
                for (int i = 0; i < ExpectedEmployees; i++)
                {
                    // TODO: Generate more employees
                    var employeeIdOnDevice = i.ToString().PadLeft(4, '0');
                    var employeeFromDeviceDto = new EmployeeFromDeviceDto()
                    {
                        EmployeeIdOnDevice = employeeIdOnDevice,
                        DeviceId = device!.Id,
                        FullName = $"Jane Doe {employeeIdOnDevice}",
                    };

                    employeeFromDeviceDtos.Add(employeeFromDeviceDto);

                    employeeIds.Add(await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto)));
                }

                var expectedTake = 10;
                var deviceDtoPaginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(new GetEmployeesFromDevicesRequest { Take = expectedTake, Skip = 0 });
                deviceDtoPaginationResponse.Count.Should().Be(ExpectedEmployees);
                deviceDtoPaginationResponse.Items.Count.Should().Be(expectedTake);

                // foreach (var employeeId in employeeIds)
                // {
                //    // await deviceClient.removeAsy(employeeFromDeviceRequest)
                // }
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_In_Employee_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

                GetEmployeesRequest request = new GetEmployeesRequest()
                {
                    SearchText = string.Empty,
                    Skip = 0,
                    Take = 10,
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.GetEmployeesAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Filtered_By_Employee_FullName_Async()
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

                GetEmployeesRequest request = new GetEmployeesRequest()
                {
                    SearchText = employee.FullName,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesAsync(request);
                paginationResponse.Items.Should().NotBeEmpty().And.AllSatisfy(
                    dto =>
                    {
                        dto.FullName.Contains(request.SearchText, StringComparison.InvariantCultureIgnoreCase).Should().Be(true);
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Filtered_By_Employee_ExternalId_Async()
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

                GetEmployeesRequest request = new GetEmployeesRequest()
                {
                    SearchText = employee.ExternalId,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesAsync(request);
                paginationResponse.Items.Should().NotBeEmpty().And.AllSatisfy(
                    dto =>
                    {
                        dto.ExternalId.Contains(request.SearchText, StringComparison.InvariantCultureIgnoreCase).Should().Be(true);
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

                GetEmployeesFromDevicesRequest request = new GetEmployeesFromDevicesRequest()
                {
                    SearchText = string.Empty,
                    IsLinked = null,
                    Skip = 0,
                    Take = 10,
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.GetEmployeesFromDevicesAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Filtered_By_EmployeeFromDevice_FullName_Async()
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

                await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(new EmployeeFromDeviceDto
                {
                    DeviceId = device!.Id,
                    EmployeeIdOnDevice = employeeFromFile.ExternalId,
                    FullName = employeeFromFile.FullName,
                }));

                GetEmployeesFromDevicesRequest request = new GetEmployeesFromDevicesRequest()
                {
                    SearchText = employeeFromFile.FullName,
                    IsLinked = null,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Should().NotBeEmpty().And.AllSatisfy(
                    dto =>
                    {
                        dto.FullName.Contains(request.SearchText, StringComparison.InvariantCultureIgnoreCase).Should().Be(true);
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Filtered_By_EmployeeFromDevice_FullName_And_False_IsLinked_Async()
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

                var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(new EmployeeFromDeviceDto
                {
                    DeviceId = device!.Id,
                    EmployeeIdOnDevice = Guid.NewGuid().ToString(),
                    FullName = employeeFromFile.FullName,
                }));

                await employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(employee.Id, employeeFromDeviceId));
                await employeeClient.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(employeeFromDeviceId));

                var employeeFromDeviceRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<EmployeeFromDevice, ApplicationDbContext>>();
                var employeeFromDevice = await employeeFromDeviceRepository.SingleAsync(e => e.Id == employeeFromDeviceId);

                GetEmployeesFromDevicesRequest request = new GetEmployeesFromDevicesRequest()
                {
                    SearchText = employeeFromDevice.FullName,
                    IsLinked = false,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Should().NotBeEmpty().And.AllSatisfy(
                    dto =>
                    {
                        dto.FullName.Contains(request.SearchText, StringComparison.InvariantCultureIgnoreCase).Should().Be(true);
                        dto.IsLinked.Should().Be(request.IsLinked.Value);
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Filtered_By_EmployeeFromDevice_True_IsLinked_Async()
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
                var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(
                                               AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

                await employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(employee.Id, employeeFromDeviceId));

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = string.Empty,
                    IsLinked = true,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Should().NotBeEmpty().And.AllSatisfy(
                    dto =>
                    {
                        dto.IsLinked.Should().Be(request.IsLinked.Value);
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Filtered_By_EmployeeFromDevice_False_IsLinked_Async()
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

                var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(
                                               AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

                await employeeClient.LinkEmployeeAsync(EmployeeLinkRequest.Create(employee.Id, employeeFromDeviceId));
                await employeeClient.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(employeeFromDeviceId));

                GetEmployeesFromDevicesRequest request = new GetEmployeesFromDevicesRequest()
                {
                    SearchText = string.Empty,
                    IsLinked = false,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Should().NotBeEmpty().And.AllSatisfy(
                    dto =>
                    {
                        dto.IsLinked.Should().Be(request.IsLinked.Value);
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Device_With_Device_Information_If_Is_Requested_Async()
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

                var agentId = await agentClient.RegisterAgentAsync(new AgentRegistrationRequest
                {
                    Agent = agentDto,
                });

                var devicesOfAgent = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, 100);
                var device = devicesOfAgent.Items.FirstOrDefault();
                device.Should().NotBeNull();

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var faker = new Faker<EmployeeFromDeviceDto>();
                faker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                faker.RuleFor(dto => dto.DeviceId, f => device!.Id);
                faker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());

                var addEmployeeFromDeviceRequest = new AddEmployeeFromDeviceRequest()
                {
                    Employee = faker.Generate(),
                };

                await employeeClient.AddEmployeeFromDeviceAsync(addEmployeeFromDeviceRequest);

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = addEmployeeFromDeviceRequest.Employee.FullName,
                    IncludeDevice = true,
                    IsLinked = false,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                var employeeFromDeviceDto = paginationResponse.Items.First();
                employeeFromDeviceDto.Should().NotBeNull();
                employeeFromDeviceDto!.Device.Should().NotBeNull();
                employeeFromDeviceDto.Device.Agent.Should().BeNull();
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Device_With_Device_And_Agent_Information_If_Is_Requested_Async()
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

                var agentId = await agentClient.RegisterAgentAsync(new AgentRegistrationRequest
                {
                    Agent = agentDto,
                });

                var devicesOfAgent = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, 100);
                var device = devicesOfAgent.Items.FirstOrDefault();
                device.Should().NotBeNull();

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var faker = new Faker<EmployeeFromDeviceDto>();
                faker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                faker.RuleFor(dto => dto.DeviceId, f => device!.Id);
                faker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());

                var addEmployeeFromDeviceRequest = new AddEmployeeFromDeviceRequest
                {
                    Employee = faker.Generate(),
                };

                await employeeClient.AddEmployeeFromDeviceAsync(addEmployeeFromDeviceRequest);

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = addEmployeeFromDeviceRequest.Employee.FullName,
                    IncludeDevice = true,
                    IncludeAgent = true,
                    IsLinked = false,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                var employeeFromDeviceDto = paginationResponse.Items.First();
                employeeFromDeviceDto.Should().NotBeNull();
                employeeFromDeviceDto!.Device.Should().NotBeNull();
                employeeFromDeviceDto.Device.Should().BeEquivalentTo(employeeFromDeviceDto.Device);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Device_Without_Device_And_Agent_Information_If_Only_Agent_Information_Is_Requested_Async()
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

                var agentId = await agentClient.RegisterAgentAsync(new AgentRegistrationRequest
                {
                    Agent = agentDto,
                });

                var devicesOfAgent = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, 100);
                var device = devicesOfAgent.Items.FirstOrDefault();
                device.Should().NotBeNull();

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var faker = new Faker<EmployeeFromDeviceDto>();
                faker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                faker.RuleFor(dto => dto.DeviceId, f => device!.Id);
                faker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());

                var addEmployeeFromDeviceRequest = new AddEmployeeFromDeviceRequest
                {
                    Employee = faker.Generate(),
                };

                await employeeClient.AddEmployeeFromDeviceAsync(addEmployeeFromDeviceRequest);

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = addEmployeeFromDeviceRequest.Employee.FullName,
                    IncludeDevice = false,
                    IncludeAgent = true,
                    IsLinked = false,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                var employeeFromDeviceDto = paginationResponse.Items.First();
                employeeFromDeviceDto.Should().NotBeNull();
                employeeFromDeviceDto!.Device.Should().BeNull();
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Device_With_Agent_Information_If_Requested_And_All_Properties_With_The_Expected_Values_Async()
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

                var agentId = await agentClient.RegisterAgentAsync(new AgentRegistrationRequest
                {
                    Agent = agentDto,
                });

                var devicesOfAgent = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, 100);
                var device = devicesOfAgent.Items.FirstOrDefault();
                device.Should().NotBeNull();

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var faker = new Faker<EmployeeFromDeviceDto>();
                faker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                faker.RuleFor(dto => dto.DeviceId, f => device!.Id);
                faker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());

                var addEmployeeFromDeviceRequest = new AddEmployeeFromDeviceRequest()
                {
                    Employee = faker.Generate(),
                };

                await employeeClient.AddEmployeeFromDeviceAsync(addEmployeeFromDeviceRequest);

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = addEmployeeFromDeviceRequest.Employee.FullName,
                    IncludeDevice = true,
                    IncludeAgent = true,
                    IsLinked = false,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                var employeeFromDeviceDto = paginationResponse.Items.First();
                employeeFromDeviceDto.Should().NotBeNull();
                employeeFromDeviceDto!.Device.Should().NotBeNull();
                employeeFromDeviceDto.Device.Agent.Should().NotBeNull();

                employeeFromDeviceDto.Device.Agent.Id.Should().Be(employeeFromDeviceDto.Device.AgentId);
                employeeFromDeviceDto.Device.Agent.Name.Should().Be(agentDto.Name);
                employeeFromDeviceDto.Device.Agent.State.Should().Be(agentDto.State);
                employeeFromDeviceDto.Device.Agent.ConnectionId.Should().Be(agentDto.ConnectionId);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Device_With_Agent_Information_If_Requested_By_The_EmployeeId_And_All_Properties_With_The_Expected_Values_Async()
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
                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = string.Empty,
                    IncludeDevice = true,
                    IncludeAgent = true,
                    EmployeeId = employee.Id,
                    IsLinked = null,
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                var responseEmployeeFromDeviceDto = paginationResponse.Items.First();
                responseEmployeeFromDeviceDto.Should().NotBeNull();
                responseEmployeeFromDeviceDto.Device.Should().NotBeNull();
                responseEmployeeFromDeviceDto.Device.Agent.Should().NotBeNull();

                responseEmployeeFromDeviceDto.Device.Agent.Id.Should().Be(agentId);
                responseEmployeeFromDeviceDto.Device.Agent.Name.Should().Be(agentDto.Name);
                responseEmployeeFromDeviceDto.Device.Agent.State.Should().Be(agentDto.State);
                responseEmployeeFromDeviceDto.Device.Agent.ConnectionId.Should().Be(agentDto.ConnectionId);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Device_Information_If_Requested_By_The_DeviceIds_With_The_Expected_Values_Async()
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

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = string.Empty,
                    IncludeDevice = true,
                    IsLinked = null,
                    DeviceIds = new List<Guid> { device!.Id },
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                var responseEmployeeFromDeviceDto = paginationResponse.Items.First();
                responseEmployeeFromDeviceDto.Should().NotBeNull();
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_Device_Information_With_Various_Devices_If_Requested_By_The_DeviceIds_With_The_Expected_Values_Async()
            {
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                       TestEnvironment.Tempore.Username,
                                       TestEnvironment.Tempore.Password);

                var devices = new List<DeviceRegistrationDto> { TestEnvironment.Device_00.RegistrationInstance, TestEnvironment.Device_01.RegistrationInstance };

                var agentDto = new AgentRegistrationDto
                {
                    Name = TestEnvironment.Agent.Name,
                    State = AgentState.Online,
                    Devices = devices,
                    ConnectionId = Guid.NewGuid().ToString(),
                };

                var agentId = await agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));

                var devicesOfAgent = await deviceClient.GetDevicesByAgentIdAsync(agentId, 0, 100);

                var deviceOne = devicesOfAgent.Items.ElementAt(0);
                var deviceTwo = devicesOfAgent.Items.ElementAt(1);

                deviceOne.Should().NotBeNull();
                deviceTwo.Should().NotBeNull();

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

                int count = 0;
                var deviceOneDto = new EmployeeFromDeviceDto
                {
                    DeviceId = deviceOne.Id,
                };

                var deviceTwoDto = new EmployeeFromDeviceDto
                {
                    DeviceId = deviceTwo.Id,
                };

                await foreach (var employee in fileProcessingService.GetEmployeesAsync(dataFile!))
                {
                    var deviceDto = (count >= 0 && count < 3) ? deviceOneDto : deviceTwoDto;

                    var employeeFromDeviceDto = new EmployeeFromDeviceDto
                    {
                        DeviceId = deviceDto.DeviceId,
                        EmployeeIdOnDevice = employee.ExternalId,
                        FullName = employee.FullName,
                    };

                    await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));
                    count++;
                }

                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = string.Empty,
                    IncludeDevice = true,
                    IsLinked = null,
                    DeviceIds = new List<Guid> { deviceTwo.Id },
                    Skip = 0,
                    Take = 100,
                };

                var paginationResponse = await employeeClient.GetEmployeesFromDevicesAsync(request);
                paginationResponse.Items.Should().NotBeNull();
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                foreach (var employeeFromDevice in paginationResponse.Items)
                {
                    employeeFromDevice.DeviceId.Should().Be(deviceTwo.Id);
                }
            }

            [Fact(Skip = "This is not working on build server, review if it is required")]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Include_Shifts_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
                await using var manifestResourceStream = typeof(EmployeeClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
                fileId.Should().NotBeEmpty();

                await fileProcessingClient.ProcessAsync(fileId);
                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var employeeRepository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<Employee, ApplicationDbContext>>();

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var employeeIds = await employeeRepository.All().Select(employee => employee.Id).Take(10).ToListAsync();
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = employeeIds,
                    StartDate = DateTimeOffset.Now,
                    ExpireDate = DateTimeOffset.Now.AddDays(30),
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var request = new GetEmployeesRequest
                {
                    SearchText = string.Empty,
                    Skip = 0,
                    Take = 100,
                    IncludeShifts = true,
                };

                var paginationResponse = await employeeClient.GetEmployeesAsync(request);
                paginationResponse.Items.Should().NotBeNull();
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                paginationResponse.Items.Where(dto => employeeIds.Contains(dto.Id)).Should().AllSatisfy(dto => dto.ScheduledShifts.Should().NotBeEmpty());
            }

            [Fact(Skip = "This is not working on build server, review if it is required")]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_Include_Shifts_And_ShiftAssignment_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
                await using var manifestResourceStream = typeof(EmployeeClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
                fileId.Should().NotBeEmpty();

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

                await fileProcessingService.GetEmployeesAsync(dataFile!).OrderBy(e => new Random().Next(1000)).FirstAsync();
                var employeeRepository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<Employee, ApplicationDbContext>>();

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();
                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var employeeIds = await employeeRepository.All().Select(employee => employee.Id).Take(10).ToListAsync();
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = employeeIds,
                    StartDate = DateTimeOffset.Now,
                    ExpireDate = DateTimeOffset.Now.AddDays(30),
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var request = new GetEmployeesRequest
                {
                    SearchText = string.Empty,
                    Skip = 0,
                    Take = 100,
                    IncludeShifts = true,
                    IncludeShiftAssignment = true,
                };

                var paginationResponse = await employeeClient.GetEmployeesAsync(request);
                paginationResponse.Items.Should().NotBeNull();
                paginationResponse.Items.Count.Should().BeGreaterThan(0);

                paginationResponse.Items.Where(dto => employeeIds.Contains(dto.Id)).Should().AllSatisfy(dto => dto.ScheduledShiftAssignments.Should().NotBeEmpty());
            }
        }
    }
}