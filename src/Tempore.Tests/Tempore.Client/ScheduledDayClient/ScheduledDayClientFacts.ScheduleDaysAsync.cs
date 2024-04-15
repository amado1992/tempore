namespace Tempore.Tests.Tempore.Client.ScheduledDayClient
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
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

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Notifications.ScheduledDay;
    using TemporeServer::Tempore.Server.Specs;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using ScheduleDaysRequest = global::Tempore.Client.ScheduleDaysRequest;

    public partial class ScheduledDayClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_ScheduleDaysAsync_Method : EnvironmentTestBase
        {
            public The_ScheduleDaysAsync_Method(
                    DockerEnvironment dockerEnvironment,
                    TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                    TestEnvironmentInitializer testEnvironmentInitializer)
                    : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> Schedules_Days_Data()
            {
                yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 15), null!, false, 15 };
                yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), null!, false, 10 };
                yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), null!, true, 10 };
                yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), new DateOnly(2025, 1, 16), false, 10 };
                yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), new DateOnly(2024, 12, 31), false, 10 };
                yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), new DateOnly(2025, 1, 5), false, 6 };
                yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), new DateOnly(2025, 1, 5), true, 10 };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var scheduledDayClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ScheduledDayClient>();

                // Act & Assert
                var scheduleDaysRequest = new ScheduleDaysRequest
                {
                    ScheduledShiftId = Guid.NewGuid(),
                    Force = false,
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(() => scheduledDayClient.ScheduleDaysAsync(
                                       scheduleDaysRequest));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Theory]
            [MemberData(nameof(Schedules_Days_Data))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status409Conflict_StatusCode_If_ScheduledDays_Job_Already_Running_Async(DateTime startDateTime, DateTime expireDateTime, DateOnly? lastGeneratedDayDate, bool force, int expectedScheduledDays)
            {
                // Devices
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

                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    StartDate = startDateTime,
                    ExpireDate = expireDateTime,
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                    EmployeeIds = new List<Guid> { employee.Id },
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var scheduledShiftAssignmentRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();
                var startDate = new DateOnly(startDateTime.Year, startDateTime.Month, startDateTime.Day);
                var expireDate = new DateOnly(expireDateTime.Year, expireDateTime.Month, expireDateTime.Day);

                var storedScheduledShiftAssignment = await scheduledShiftAssignmentRepository.SingleAsync(SpecificationBuilder.Build<ScheduledShiftAssignment>(scheduledShiftAssignments => scheduledShiftAssignments.Include(assignment => assignment.ScheduledShift).Where(scheduledShiftAssignment => scheduledShiftAssignment.EmployeeId == employee.Id
                                                         && scheduledShiftAssignment.ScheduledShift.ShiftId == assignShiftToEmployeesRequest.ShiftId
                                                         && scheduledShiftAssignment.ScheduledShift.StartDate == startDate
                                                         && scheduledShiftAssignment.ScheduledShift.ExpireDate == expireDate
                                                         && scheduledShiftAssignment.ScheduledShift.EffectiveWorkingTime == assignShiftToEmployeesRequest.EffectiveWorkingTime)));

                if (lastGeneratedDayDate is not null)
                {
                    storedScheduledShiftAssignment.LastGeneratedDayDate = lastGeneratedDayDate;
                    scheduledShiftAssignmentRepository.Update(storedScheduledShiftAssignment);
                    await scheduledShiftAssignmentRepository.SaveChangesAsync();
                }

                // Adds extra out of range scheduled days.
                var scheduledDayRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<ScheduledDay, ApplicationDbContext>>();
                var dayRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<Day, ApplicationDbContext>>();
                var day = await dayRepository.All().FirstAsync();

                scheduledDayRepository.Add(new ScheduledDay
                {
                    Date = startDate.AddDays(-3),
                    DayId = day.Id,
                    ScheduledShiftAssignmentId = storedScheduledShiftAssignment.Id,
                });

                scheduledDayRepository.Add(new ScheduledDay
                {
                    Date = startDate.AddDays(3),
                    DayId = day.Id,
                    ScheduledShiftAssignmentId = storedScheduledShiftAssignment.Id,
                });

                scheduledDayRepository.Add(new ScheduledDay()
                {
                    Date = expireDate.AddDays(+3),
                    ScheduledShiftAssignmentId = storedScheduledShiftAssignment.Id,
                    DayId = day.Id,
                });

                await scheduledDayRepository.SaveChangesAsync();

                // Act
                var request = new ScheduleDaysRequest
                {
                    ScheduledShiftId = storedScheduledShiftAssignment.ScheduledShiftId,
                    Force = force,
                };

                var scheduledDayClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ScheduledDayClient>(
                                             TestEnvironment.Tempore.Username,
                                             TestEnvironment.Tempore.Password);

                await TaskHelper.RepeatAsync(() => scheduledDayClient.IsScheduleDaysProcessRunningAsync());

                await scheduledDayClient.ScheduleDaysAsync(request);
                var apiException = await Assert.ThrowsAsync<ApiException>(() => scheduledDayClient.ScheduleDaysAsync(request));
                apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            }

            [Theory]
            [MemberData(nameof(Schedules_Days_Data))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Notifies_Success_Upon_Completion_Via_NotificationService_Async(DateTime startDateTime, DateTime expireDateTime, DateOnly? lastGeneratedDayDate, bool force, int expectedScheduledDays)
            {
                // Devices
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

                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    StartDate = startDateTime,
                    ExpireDate = expireDateTime,
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                    EmployeeIds = new List<Guid> { employee.Id },
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var scheduledShiftAssignmentRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();
                var startDate = new DateOnly(startDateTime.Year, startDateTime.Month, startDateTime.Day);
                var expireDate = new DateOnly(expireDateTime.Year, expireDateTime.Month, expireDateTime.Day);

                var storedScheduledShiftAssignment = await scheduledShiftAssignmentRepository.SingleAsync(SpecificationBuilder.Build<ScheduledShiftAssignment>(scheduledShiftAssignments => scheduledShiftAssignments.Include(assignment => assignment.ScheduledShift).Where(scheduledShiftAssignment => scheduledShiftAssignment.EmployeeId == employee.Id
                                                         && scheduledShiftAssignment.ScheduledShift.ShiftId == assignShiftToEmployeesRequest.ShiftId
                                                         && scheduledShiftAssignment.ScheduledShift.StartDate == startDate
                                                         && scheduledShiftAssignment.ScheduledShift.ExpireDate == expireDate
                                                         && scheduledShiftAssignment.ScheduledShift.EffectiveWorkingTime == assignShiftToEmployeesRequest.EffectiveWorkingTime)));

                if (lastGeneratedDayDate is not null)
                {
                    storedScheduledShiftAssignment.LastGeneratedDayDate = lastGeneratedDayDate;
                    scheduledShiftAssignmentRepository.Update(storedScheduledShiftAssignment);
                    await scheduledShiftAssignmentRepository.SaveChangesAsync();
                }

                // Adds extra out of range scheduled days.
                var scheduledDayRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<ScheduledDay, ApplicationDbContext>>();
                var dayRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<Day, ApplicationDbContext>>();
                var day = await dayRepository.All().FirstAsync();

                scheduledDayRepository.Add(new ScheduledDay
                {
                    Date = startDate.AddDays(-3),
                    DayId = day.Id,
                    ScheduledShiftAssignmentId = storedScheduledShiftAssignment.Id,
                });

                scheduledDayRepository.Add(new ScheduledDay
                {
                    Date = startDate.AddDays(3),
                    DayId = day.Id,
                    ScheduledShiftAssignmentId = storedScheduledShiftAssignment.Id,
                });

                scheduledDayRepository.Add(new ScheduledDay()
                {
                    Date = expireDate.AddDays(+3),
                    ScheduledShiftAssignmentId = storedScheduledShiftAssignment.Id,
                    DayId = day.Id,
                });

                await scheduledDayRepository.SaveChangesAsync();

                // Act
                var request = new ScheduleDaysRequest
                {
                    ScheduledShiftId = storedScheduledShiftAssignment.ScheduledShiftId,
                    Force = force,
                };

                var scheduledDayClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ScheduledDayClient>(
                                             TestEnvironment.Tempore.Username,
                                             TestEnvironment.Tempore.Password);

                await TaskHelper.RepeatAsync(() => scheduledDayClient.IsScheduleDaysProcessRunningAsync());

                this.TemporeServerApplicationFactory.NotificationServiceMock.Reset();

                await scheduledDayClient.ScheduleDaysAsync(request);

                await this.TemporeServerApplicationFactory.NotificationServiceMock.WaitAndVerifyAsync(TimeSpan.FromSeconds(60), service => service.SuccessAsync<ScheduleDaysProcessCompletedNotification>(TestEnvironment.Tempore.Username, It.IsAny<string>()), Times.Once);
            }
        }
    }
}