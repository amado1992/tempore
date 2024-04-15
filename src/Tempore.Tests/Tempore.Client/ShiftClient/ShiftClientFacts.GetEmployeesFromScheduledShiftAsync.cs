namespace Tempore.Tests.Tempore.Client.ShiftClient
{
    extern alias TemporeAgent;
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Processing.Services.Interfaces;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;
    using global::Tempore.Tests.Infraestructure.Helpers;
    using global::Tempore.Tests.Tempore.Client.EmployeeClient;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Services;
    using TemporeServer::Tempore.Server.Specs;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using ApplicationDbContext = global::Tempore.Storage.ApplicationDbContext;
    using FileType = global::Tempore.Client.FileType;

    public partial class ShiftClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetEmployeesFromScheduledShiftAsync_Method : EnvironmentTestBase
        {
            public The_GetEmployeesFromScheduledShiftAsync_Method(
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
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => shiftClient.GetEmployeesFromScheduledShiftAsync(new GetEmployeesFromScheduledShiftRequest()));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_ScheduledShift_Async()
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

                await TaskHelper.RepeatAsync(() => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var employeeRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Employee, ApplicationDbContext>>();

                var employeeIds = (await employeeRepository.FindAsync(SpecificationBuilder.Build<Employee, Guid>(employees => employees
                                      .Include(employee => employee.ScheduledShifts)
                                      .Where(employee => employee.ScheduledShifts.Count == 0)
                                      .Select(employee => employee.Id).Take(10)))).ToList();

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var startDate = new DateTimeOffset(new DateTime(2030, 1, 1));
                var assignEmployeesToScheduledShiftRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = employeeIds,
                    StartDate = startDate,
                    ExpireDate = startDate.AddDays(15),
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                };

                var scheduledShiftId = await employeeClient.AssignEmployeesToScheduledShiftAsync(assignEmployeesToScheduledShiftRequest);

                // Assert
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var getEmployeesFromScheduledShiftRequest = new GetEmployeesFromScheduledShiftRequest
                {
                    ScheduledShiftId = scheduledShiftId,
                    Skip = 0,
                    Take = int.MaxValue,
                };

                var response = await shiftClient.GetEmployeesFromScheduledShiftAsync(getEmployeesFromScheduledShiftRequest);

                response.Items.Should().NotBeEmpty();
                response.Items.Should().AllSatisfy(
                    dto =>
                    {
                        dto.Id.Should().NotBeEmpty();
                        dto.FullName.Should().NotBeEmpty();
                        if (employeeIds.Contains(dto.Id))
                        {
                            dto.IsAssigned.Should().BeTrue();
                        }
                        else
                        {
                            dto.IsAssigned.Should().BeFalse();
                        }
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_From_ScheduledShift_As_Unassigned_When_ScheduledShiftId_Null_Async()
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

                await TaskHelper.RepeatAsync(() => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();
                shift.Should().NotBeNull();

                // Assert
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var employeesFromScheduledShiftRequest = new GetEmployeesFromScheduledShiftRequest
                {
                    Skip = 0,
                    Take = int.MaxValue,
                };

                var response = await shiftClient.GetEmployeesFromScheduledShiftAsync(employeesFromScheduledShiftRequest);

                response.Items.Should().NotBeEmpty();
                response.Items.Should().AllSatisfy(
                    dto =>
                    {
                        dto.Id.Should().NotBeEmpty();
                        dto.FullName.Should().NotBeEmpty();
                        dto.IsAssigned.Should().BeFalse();
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Employees_With_State_Assigned_Yes_Or_Not_Filtered_By_Employee_FullName_Async()
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

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var employeeRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Employee, ApplicationDbContext>>();

                var employeeIds = await employeeRepository.All().Include(employee => employee.ScheduledShifts).Where(employee => employee.ScheduledShifts.Count == 0).Select(employee => employee.Id).Take(10).ToListAsync();

                employeeIds = employeeIds.OrderBy(guid => new Random().Next(100)).ToList();
                var startDate = new DateTimeOffset(new DateTime(2030, 2, 1));
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = employeeIds,
                    StartDate = startDate,
                    ExpireDate = startDate.AddDays(15),
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                };

                var scheduledShift = await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                // Assert
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var employee = await employeeRepository.SingleAsync(e => e.FullName == employeeFromFile.FullName);

                var employeesFromScheduledShiftRequest = new GetEmployeesFromScheduledShiftRequest
                {
                    ScheduledShiftId = scheduledShift,
                    Skip = 0,
                    Take = int.MaxValue,
                    SearchParams = new EmployeesFromScheduledShiftSearchParams
                    {
                        SearchText = employee.FullName,
                    },
                };

                var response = await shiftClient.GetEmployeesFromScheduledShiftAsync(employeesFromScheduledShiftRequest);

                response.Items.Should().NotBeEmpty();
                response.Items.Should().AllSatisfy(
                    dto =>
                    {
                        dto.FullName.Contains(employeesFromScheduledShiftRequest.SearchParams.SearchText, StringComparison.InvariantCultureIgnoreCase).Should().Be(true);
                    });
            }
        }
    }
}