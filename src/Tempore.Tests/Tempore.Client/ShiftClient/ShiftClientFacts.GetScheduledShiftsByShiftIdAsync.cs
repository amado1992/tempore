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

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using ApplicationDbContext = global::Tempore.Storage.ApplicationDbContext;
    using FileType = global::Tempore.Client.FileType;

    public partial class ShiftClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetScheduledShiftsByShiftIdAsync_Method : EnvironmentTestBase
        {
            public The_GetScheduledShiftsByShiftIdAsync_Method(
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

                var apiException = await Assert.ThrowsAsync<ApiException>(() => shiftClient.GetScheduledShiftsByShiftIdAsync(new GetScheduledShiftByShiftIdRequest()));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_The_ScheduledShifts_Async()
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

                var employeeIds = await employeeRepository.All().Include(employee => employee.ScheduledShifts).Where(employee => employee.ScheduledShifts.Count == 0).Select(employee => employee.Id).Take(10).ToListAsync();

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                employeeIds = employeeIds.OrderBy(guid => new Random().Next(100)).ToList();

                // ALERT: The code is duplicated on purpose to create multiple shift assignments.
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = employeeIds,
                    StartDate = DateTimeOffset.Now,
                    ExpireDate = DateTimeOffset.Now.AddDays(15),
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    EmployeeIds = employeeIds,
                    StartDate = DateTimeOffset.Now.AddDays(25),
                    ExpireDate = DateTimeOffset.Now.AddDays(25).AddDays(15),
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                // Assert
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var scheduledShifts = await shiftClient.GetScheduledShiftsByShiftIdAsync(
                    new GetScheduledShiftByShiftIdRequest
                    {
                        ShiftId = shift.Id,
                        Skip = 0,
                        Take = int.MaxValue,
                    });

                scheduledShifts.Items.Should().NotBeEmpty();
                scheduledShifts.Items.Count.Should().BeGreaterThan(1);
                scheduledShifts.Items.DistinctBy(dto => new { dto.StartDate, dto.ExpireDate, dto.EffectiveWorkingTime }).Count().Should().Be(scheduledShifts.Items.Count);
                scheduledShifts.Items.Should().AllSatisfy(
                    dto =>
                    {
                        dto.ExpireDate.Should().NotBe(default);
                        dto.StartDate.Should().NotBe(default);
                        dto.EffectiveWorkingTime.Should().NotBe(default);
                        dto.EmployeesCount.Should().NotBe(0);
                    });
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_Filtered_ScheduledShift_By_Id_ShiftId_StartDate_ExpireDate_And_EffectiveWorkingTime_Async()
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

                var employeeIds = await employeeRepository.All().Include(employee => employee.ScheduledShifts).Where(employee => employee.ScheduledShifts.Count == 0).Select(employee => employee.Id).Take(10).ToListAsync();

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                employeeIds = employeeIds.OrderBy(guid => new Random().Next(100)).ToList();

                // ALERT: The code is duplicated on purpose to create multiple shift assignments.
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = employeeIds,
                    StartDate = DateTimeOffset.Now,
                    ExpireDate = DateTimeOffset.Now.AddDays(15),
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    EmployeeIds = employeeIds,
                    StartDate = DateTimeOffset.Now.AddDays(25),
                    ExpireDate = DateTimeOffset.Now.AddDays(25).AddDays(15),
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                };

                var scheduledShiftId = await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                // Assert
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var request = new GetScheduledShiftByShiftIdRequest
                {
                    ShiftId = shift.Id,
                    Skip = 0,
                    Take = int.MaxValue,
                    SearchParams = new ScheduledShiftSearchParams
                    {
                        Id = scheduledShiftId,
                        StartDate = DateTimeOffset.Now.AddDays(25),
                        ExpireDate = DateTimeOffset.Now.AddDays(25).AddDays(15),
                        EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                    },
                };

                var shiftAssignments = await shiftClient.GetScheduledShiftsByShiftIdAsync(request);

                shiftAssignments.Items.Should().NotBeEmpty();
                shiftAssignments.Items.Count.Should().BeGreaterThan(0);
                shiftAssignments.Items.Should().AllSatisfy(
                    dto =>
                    {
                        dto.Id.Should().Be(request.SearchParams!.Id!.Value);
                        dto.ExpireDate.Date.Should().Be(request.SearchParams.ExpireDate?.Date);
                        dto.StartDate.Should().Be(request.SearchParams.StartDate?.Date);
                        dto.EffectiveWorkingTime.Should().Be(TimeSpan.FromHours(97.77));
                    });
            }
        }
    }
}