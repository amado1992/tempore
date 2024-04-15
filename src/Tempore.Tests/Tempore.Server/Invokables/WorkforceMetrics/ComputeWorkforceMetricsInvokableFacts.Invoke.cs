namespace Tempore.Tests.Tempore.Server.Invokables.WorkforceMetrics
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Processing.PayDay;
    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Invokables.Employees;
    using TemporeServer::Tempore.Server.Invokables.Interfaces;
    using TemporeServer::Tempore.Server.Invokables.WorkforceMetrics;
    using TemporeServer::Tempore.Server.Specs;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using ComputeWorkforceMetricsRequest = TemporeServer::Tempore.Server.Requests.WorkforceMetrics.ComputeWorkforceMetricsRequest;

    public partial class ComputeWorkforceMetricsInvokableFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_Invoke_Method : EnvironmentTestBase
        {
            public The_Invoke_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Computes_All_Metrics_For_Requested_Date_Range_Async()
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

                // Employees From Devices
                var employeeFromDeviceFaker = new Faker<EmployeeFromDeviceDto>();
                employeeFromDeviceFaker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                employeeFromDeviceFaker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());
                employeeFromDeviceFaker.RuleFor(dto => dto.DeviceId, f => device!.Id);

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                            TestEnvironment.Tempore.Username,
                                            TestEnvironment.Tempore.Password);

                var employeeFromDeviceDto = employeeFromDeviceFaker.Generate();
                var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

                // Employees
                var employeeRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Employee, ApplicationDbContext>>();
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

                // Shift Assignments
                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();
                var shift = shiftRepository.All().First();

                (DateTime startDate, DateTime expireDate) = DateRangeGenerator.Next();
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    StartDate = startDate,
                    ExpireDate = expireDate,
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                    EmployeeIds = new List<Guid> { employee.Id },
                };

                var scheduledShiftId = await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                // Schedule
                var daySchedulerInvokable = this.TemporeServerApplicationFactory!.Services.GetRequiredService<TemporeServer::Tempore.Server.Invokables.ScheduledDay.DaySchedulerInvokable>();
                var request = new TemporeServer::Tempore.Server.Requests.ScheduledDay.ScheduleDaysRequest
                {
                    ScheduledShiftId = scheduledShiftId,
                    Force = true,
                };

                var scheduleDaysRequestMock = new Mock<IInvocationContext<TemporeServer::Tempore.Server.Requests.ScheduledDay.ScheduleDaysRequest>>();
                scheduleDaysRequestMock.SetupGet(context => context.Request).Returns(request);
                daySchedulerInvokable.Payload = scheduleDaysRequestMock.Object;
                await daySchedulerInvokable.Invoke();

                // Timestamps
                var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>(
                                          TestEnvironment.Tempore.Username,
                                          TestEnvironment.Tempore.Password);

                var random = new Random();
                for (var currentDate = startDate.Date; currentDate <= expireDate; currentDate = currentDate.AddDays(1))
                {
                    await timestampClient.AddEmployeeFromDeviceTimestampAsync(AddEmployeeFromDeviceTimestampRequest.Create(new TimestampDto
                    {
                        DateTime = currentDate.AddHours(8).AddMinutes(random.Next(-15, 15)),
                        EmployeeFromDevice = new EmployeeFromDeviceDto
                        {
                            DeviceId = employeeFromDeviceDto.DeviceId,
                            EmployeeIdOnDevice = employeeFromDeviceDto.EmployeeIdOnDevice,
                        },
                    }));

                    await timestampClient.AddEmployeeFromDeviceTimestampAsync(AddEmployeeFromDeviceTimestampRequest.Create(new TimestampDto
                    {
                        DateTime = currentDate.DayOfWeek == DayOfWeek.Saturday ?
                                       currentDate.AddHours(13).AddMinutes(random.Next(-15, 15)) :
                                       currentDate.AddHours(17).AddMinutes(random.Next(-15, 15)),
                        EmployeeFromDevice = new EmployeeFromDeviceDto
                        {
                            DeviceId = employeeFromDeviceDto.DeviceId,
                            EmployeeIdOnDevice = employeeFromDeviceDto.EmployeeIdOnDevice,
                        },
                    }));
                }

                // Association
                var timestampScheduledDayAssociatorInvokable = this.TemporeServerApplicationFactory!.Services.GetRequiredService<TemporeServer::Tempore.Server.Invokables.ScheduledDay.TimestampScheduledDayAssociatorInvokable>();
                await timestampScheduledDayAssociatorInvokable.Invoke();

                // Act: Workforce
                var workforceMetricCollectionRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<WorkforceMetricCollection, ApplicationDbContext>>();
                var workforceMetricCollection = await workforceMetricCollectionRepository.SingleOrDefaultAsync(SpecificationBuilder
                    .Build<WorkforceMetricCollection>(workforceMetricCollections => workforceMetricCollections
                            .Where(collection => collection.Name == PayDayWorkforceMetricCollection.Name)));

                workforceMetricCollection.Should().NotBeNull();

                var computeWorkforceMetricsInvokable = this.TemporeServerApplicationFactory!.Services.GetRequiredService<ComputeWorkforceMetricsInvokable>();
                var invocationContextMock = new Mock<TemporeServer::Tempore.Server.Invokables.Interfaces.IInvocationContext<ComputeWorkforceMetricsRequest>>();

                var computeWorkforceMetricsRequest = new ComputeWorkforceMetricsRequest
                {
                    StartDate = new DateOnly(startDate.Year, startDate.Month, startDate.Day),
                    EndDate = new DateOnly(expireDate.Year, expireDate.Month, expireDate.Day),
                    WorkForceMetricCollectionIds = new List<Guid>
                                                             {
                                                                 workforceMetricCollection!.Id,
                                                             },
                };

                invocationContextMock.SetupGet(context => context.Request).Returns(computeWorkforceMetricsRequest);
                computeWorkforceMetricsInvokable.Payload = invocationContextMock.Object;
                await computeWorkforceMetricsInvokable.Invoke();

                // Assert?
                var workforceMetricDailySnapshotRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<WorkforceMetricDailySnapshot, ApplicationDbContext>>();
                var workforceMetricDailySnapshotSpec = SpecificationBuilder.Build<WorkforceMetricDailySnapshot>(dailySnapshots => dailySnapshots.Include(snapshot => snapshot.ScheduledDay)
                    .ThenInclude(day => day.ScheduledShiftAssignment)
                    .Where(snapshot => snapshot.ScheduledDay.ScheduledShiftAssignment.EmployeeId == employee.Id));

                var count = await workforceMetricDailySnapshotRepository.CountAsync(workforceMetricDailySnapshotSpec);
                count.Should().NotBe(0);

                count.Should().Be(((int)expireDate.Subtract(startDate).TotalDays + 1) * PayDayWorkforceMetrics.All.Count);
            }
        }
    }
}