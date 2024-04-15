namespace Tempore.Tests.Tempore.Server.Invokables.ScheduledDay
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

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Invokables.Interfaces;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using ScheduleDaysRequest = TemporeServer::Tempore.Server.Requests.ScheduledDay.ScheduleDaysRequest;

    public partial class DaySchedulerInvokableFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_Invoke_Method_Integration : EnvironmentTestBase
        {
            public The_Invoke_Method_Integration(
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

            [Theory]
            [MemberData(nameof(Schedules_Days_Data))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Schedules_Days_As_Expected_Async(DateTime startDateTime, DateTime expireDateTime, DateOnly? lastGeneratedDayDate, bool force, int expectedScheduledDays)
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

                var storedShiftAssignment = await scheduledShiftAssignmentRepository.All().Include(assignment => assignment.ScheduledShift).SingleAsync(assignment => assignment.EmployeeId == employee.Id
                                                   && assignment.ScheduledShift.ShiftId == assignShiftToEmployeesRequest.ShiftId
                                                   && assignment.ScheduledShift.StartDate == startDate
                                                   && assignment.ScheduledShift.ExpireDate == expireDate
                                                   && assignment.ScheduledShift.EffectiveWorkingTime == assignShiftToEmployeesRequest.EffectiveWorkingTime);

                if (lastGeneratedDayDate is not null)
                {
                    storedShiftAssignment.LastGeneratedDayDate = lastGeneratedDayDate;
                    scheduledShiftAssignmentRepository.Update(storedShiftAssignment);
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
                    ScheduledShiftAssignmentId = storedShiftAssignment.Id,
                });

                scheduledDayRepository.Add(new ScheduledDay
                {
                    Date = startDate.AddDays(3),
                    DayId = day.Id,
                    ScheduledShiftAssignmentId = storedShiftAssignment.Id,
                });

                scheduledDayRepository.Add(new ScheduledDay
                {
                    Date = expireDate.AddDays(+3),
                    ScheduledShiftAssignmentId = storedShiftAssignment.Id,
                    DayId = day.Id,
                });

                await scheduledDayRepository.SaveChangesAsync();

                // Act
                var daySchedulerInvokable = this.TemporeServerApplicationFactory!.Services.GetRequiredService<TemporeServer::Tempore.Server.Invokables.ScheduledDay.DaySchedulerInvokable>();

                var request = new ScheduleDaysRequest
                {
                    ScheduledShiftId = storedShiftAssignment.ScheduledShift.Id,
                    Force = force,
                };

                var scheduleDaysRequestMock = new Mock<IInvocationContext<ScheduleDaysRequest>>();
                scheduleDaysRequestMock.SetupGet(context => context.Request).Returns(request);
                daySchedulerInvokable.Payload = scheduleDaysRequestMock.Object;

                await daySchedulerInvokable.Invoke();

                // Assert
                var shiftAssignment = await scheduledShiftAssignmentRepository.All()
                                          .Include(scheduledShiftAssignment => scheduledShiftAssignment.ScheduledDays)
                                          .Include(scheduledShiftAssignment => scheduledShiftAssignment.ScheduledShift)
                                          .SingleOrDefaultAsync(scheduledShiftAssignment => scheduledShiftAssignment.EmployeeId == employee.Id
                                                                     && scheduledShiftAssignment.ScheduledShift.ShiftId == assignShiftToEmployeesRequest.ShiftId
                                                                     && scheduledShiftAssignment.ScheduledShift.StartDate == startDate
                                                                     && scheduledShiftAssignment.ScheduledShift.ExpireDate == expireDate
                                                                     && scheduledShiftAssignment.ScheduledShift.EffectiveWorkingTime == assignShiftToEmployeesRequest.EffectiveWorkingTime);

                shiftAssignment.Should().NotBeNull();
                shiftAssignment!.ScheduledDays.Should().NotBeEmpty();
                shiftAssignment.ScheduledDays.Count.Should().Be(expectedScheduledDays);
                shiftAssignment.LastGeneratedDayDate.Should().NotBeNull();

                shiftAssignment.ScheduledDays.Any(scheduledDay => scheduledDay is
                {
                    StartDateTime: not null,
                    CheckInStartDateTime: not null,
                    CheckInEndDateTime: not null,
                    EndDateTime: not null,
                    CheckOutStartDateTime: not null,
                    CheckOutEndDateTime: not null,
                }).Should().BeTrue();
            }
        }
    }
}