namespace Tempore.Tests.Tempore.Client.WorkforceMetricClient
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

    using FluentAssertions;

    using global::Tempore.Client;
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

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using FileFormat = global::Tempore.Client.FileFormat;

    public partial class WorkforceMetricClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_ExportWorkforceMetricsAsync_Method : EnvironmentTestBase
        {
            public The_ExportWorkforceMetricsAsync_Method(
                DockerEnvironment dockerEnvironment, TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> InvalidRequests()
            {
                yield return new object[]
                             {
                                 new ExportWorkforceMetricsRequest
                                 {
                                     WorkforceMetricCollectionId = default,
                                     StartDate = DateTimeOffset.Now,
                                     EndDate = DateTimeOffset.Now,
                                 },
                             };

                yield return new object[]
                             {
                                 new ExportWorkforceMetricsRequest
                                 {
                                     WorkforceMetricCollectionId = Guid.NewGuid(),
                                     StartDate = default,
                                     EndDate = DateTimeOffset.Now,
                                 },
                             };

                yield return new object[]
                             {
                                 new ExportWorkforceMetricsRequest
                                 {
                                     WorkforceMetricCollectionId = Guid.NewGuid(),
                                     StartDate = DateTimeOffset.Now,
                                     EndDate = default,
                                 },
                             };

                yield return new object[]
                             {
                                 new ExportWorkforceMetricsRequest
                                 {
                                     WorkforceMetricCollectionId = Guid.NewGuid(),
                                     StartDate = DateTimeOffset.Now,
                                     EndDate = DateTimeOffset.Now.AddDays(-10),
                                 },
                             };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var workforceMetricClientClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => workforceMetricClientClient.ExportWorkforceMetricsAsync(new ExportWorkforceMetricsRequest()));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Theory]
            [MemberData(nameof(InvalidRequests))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_Invalid_Data_For_The_Request_Async(ExportWorkforceMetricsRequest request)
            {
                var workforceMetricClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var apiException =
                    await Assert.ThrowsAsync<ApiException>(
                        () => workforceMetricClient.ExportWorkforceMetricsAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task
                Throws_ApiException_With_Status404NotFound_StatusCode_When_The_WorkforceMetricCollectionId_Not_Found_Async()
            {
                var workforceMetricClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var request = new ExportWorkforceMetricsRequest
                {
                    StartDate = DateTimeOffset.Now,
                    EndDate = DateTimeOffset.Now.AddDays(10),
                    WorkforceMetricCollectionId = Guid.NewGuid(),
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(() => workforceMetricClient.ExportWorkforceMetricsAsync(request));
                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Exports_WorkforceCollection_Metrics_In_CSV_FileFormat_As_Expected_Async()
            {
                // Devices
                var agentClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<AgentClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var deviceClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<DeviceClient>(
                                       TestEnvironment.Tempore.Username,
                                       TestEnvironment.Tempore.Password);

                var devices = new List<DeviceRegistrationDto> { TestEnvironment.Device_00.RegistrationInstance, };

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

                // Employees From Devices
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);
                var employeeIds = new List<Guid>();
                var employeeFromDeviceDtos = new List<EmployeeFromDeviceDto>();
                const int EmployeesCount = 10;

                for (int i = 0; i < EmployeesCount; i++)
                {
                    var employeeFromDeviceFaker = new Faker<EmployeeFromDeviceDto>();
                    employeeFromDeviceFaker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                    employeeFromDeviceFaker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());
                    employeeFromDeviceFaker.RuleFor(dto => dto.DeviceId, f => device!.Id);

                    var employeeFromDeviceDto = employeeFromDeviceFaker.Generate();
                    var employeeFromDeviceId =
                        await employeeClient.AddEmployeeFromDeviceAsync(
                            AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

                    employeeFromDeviceDtos.Add(employeeFromDeviceDto);

                    // Employees
                    var employeeRepository = this.TemporeServerApplicationFactory.Services
                        .GetRequiredService<IRepository<Employee, ApplicationDbContext>>();
                    var faker = new Faker<Employee>();
                    faker.RuleFor(dto => dto.FullName, () => employeeFromDeviceDto.FullName);
                    faker.RuleFor(dto => dto.ExternalId, () => employeeFromDeviceDto.EmployeeIdOnDevice);

                    var employee = faker.Generate();
                    employeeRepository.Add(employee);
                    await employeeRepository.SaveChangesAsync();

                    // Link Employees
                    var employeeLinkRequest = new EmployeeLinkRequest
                    {
                        EmployeeId = employee.Id,
                        EmployeeFromDeviceIds = new List<Guid> { employeeFromDeviceId },
                    };

                    await employeeClient.LinkEmployeeAsync(employeeLinkRequest);

                    employeeIds.Add(employee.Id);
                }

                // Shift Assignments
                var shiftRepository = this.TemporeServerApplicationFactory.Services
                    .GetRequiredService<IRepository<Shift, ApplicationDbContext>>();
                var shift = shiftRepository.All().First();

                var (startDateTime, expireDateTime) = DateRangeGenerator.Next();
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    StartDate = startDateTime,
                    ExpireDate = expireDateTime,
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                    EmployeeIds = employeeIds,
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);
                var shiftAssignmentRepository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();

                var startDate = new DateOnly(
                    assignShiftToEmployeesRequest.StartDate.Year,
                    assignShiftToEmployeesRequest.StartDate.Month,
                    assignShiftToEmployeesRequest.StartDate.Day);
                var expireDate = new DateOnly(
                    assignShiftToEmployeesRequest.ExpireDate.Year,
                    assignShiftToEmployeesRequest.ExpireDate.Month,
                    assignShiftToEmployeesRequest.ExpireDate.Day);

                var employeeId = employeeIds.First();
                var shiftAssignment = await shiftAssignmentRepository.All()
                                          .Include(assignment => assignment.ScheduledShift)
                                          .ThenInclude(scheduledShift => scheduledShift.Shift).ThenInclude(s => s.Days)
                                          .ThenInclude(day => day.Timetable).SingleAsync(
                                              assignment => assignment.EmployeeId == employeeId
                                                            && assignment.ScheduledShift.ShiftId
                                                            == assignShiftToEmployeesRequest.ShiftId
                                                            && assignment.ScheduledShift.StartDate == startDate
                                                            && assignment.ScheduledShift.ExpireDate == expireDate
                                                            && assignment.ScheduledShift.EffectiveWorkingTime
                                                            == assignShiftToEmployeesRequest.EffectiveWorkingTime);

                // ScheduledDays
                var scheduledDayClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<ScheduledDayClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var scheduleDaysRequest = new ScheduleDaysRequest
                {
                    ScheduledShiftId = shiftAssignment.ScheduledShiftId,
                    Force = true,
                };

                await scheduledDayClient.ScheduleDaysAsync(scheduleDaysRequest);
                await TaskHelper.RepeatAsync(() => scheduledDayClient.IsScheduleDaysProcessRunningAsync());

                var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>(
                                          TestEnvironment.Tempore.Username,
                                          TestEnvironment.Tempore.Password);

                var random = new Random();
                foreach (var employeeFromDeviceDto in employeeFromDeviceDtos)
                {
                    for (var currentDate = startDate; currentDate <= expireDate; currentDate = currentDate.AddDays(1))
                    {
                        await timestampClient.AddEmployeeFromDeviceTimestampAsync(
                            AddEmployeeFromDeviceTimestampRequest.Create(
                                device!.Id,
                                new DateTimeOffset(
                                    new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 8, 0, 0).AddMinutes(random.Next(-10, 10))),
                                employeeFromDeviceDto.EmployeeIdOnDevice));

                        if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                        {
                            await timestampClient.AddEmployeeFromDeviceTimestampAsync(
                                AddEmployeeFromDeviceTimestampRequest.Create(
                                    device.Id,
                                    new DateTimeOffset(
                                        new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 13, 0, 0).AddMinutes(random.Next(-10, 10))),
                                    employeeFromDeviceDto.EmployeeIdOnDevice));
                        }
                        else if (currentDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            await timestampClient.AddEmployeeFromDeviceTimestampAsync(
                                AddEmployeeFromDeviceTimestampRequest.Create(
                                    device.Id,
                                    new DateTimeOffset(
                                        new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 17, 0, 0).AddMinutes(random.Next(-10, 10))),
                                    employeeFromDeviceDto.EmployeeIdOnDevice));
                        }
                    }
                }

                var timestampScheduledDayAssociatorInvokable = this.TemporeServerApplicationFactory!.Services
                        .GetRequiredService<TemporeServer::Tempore.Server.Invokables.ScheduledDay.
                            TimestampScheduledDayAssociatorInvokable>();
                await timestampScheduledDayAssociatorInvokable.Invoke();

                var workforceMetricClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var getWorkforceMetricCollectionsRequest = new GetWorkforceMetricCollectionsRequest
                {
                    Skip = 0,
                    Take = 1,
                };
                var workforceMetricCollectionDtoPaginationResponse =
                    await workforceMetricClient.GetWorkforceMetricCollectionsAsync(
                        getWorkforceMetricCollectionsRequest);
                var workforceMetricCollectionDto = workforceMetricCollectionDtoPaginationResponse.Items.First();

                var computeWorkforceMetricsRequest = new ComputeWorkforceMetricsRequest
                {
                    StartDate = startDateTime,
                    EndDate = expireDateTime,
                    WorkForceMetricCollectionIds = new List<Guid> { workforceMetricCollectionDto.Id },
                };

                await workforceMetricClient.ComputeWorkforceMetricsAsync(computeWorkforceMetricsRequest);
                await TaskHelper.RepeatAsync(
                    () => workforceMetricClient.IsComputeWorkforceMetricsProcessRunningAsync(), TimeSpan.FromMinutes(5));

                var expectedColumnsCount = (await workforceMetricClient.GetWorkforceMetricsSchemaAsync(workforceMetricCollectionDto.Id, SchemaType.Export)).Count(info => info.Include);

                var request = new ExportWorkforceMetricsRequest
                {
                    StartDate = startDateTime,
                    EndDate = expireDateTime,
                    WorkforceMetricCollectionId = workforceMetricCollectionDto.Id,
                    FileFormat = FileFormat.CSV,
                };

                using var workforceMetricsFileResponse = await workforceMetricClient.ExportWorkforceMetricsAsync(request);
                workforceMetricsFileResponse.Should().NotBeNull();

                int count = 0;
                using var streamReader = new StreamReader(workforceMetricsFileResponse.Stream);
                while (await streamReader.ReadLineAsync() is { } line)
                {
                    var split = line.Split(',');
                    split.Length.Should().Be(expectedColumnsCount);
                    count++;
                }

                count.Should().Be(EmployeesCount);
            }
        }
    }
}