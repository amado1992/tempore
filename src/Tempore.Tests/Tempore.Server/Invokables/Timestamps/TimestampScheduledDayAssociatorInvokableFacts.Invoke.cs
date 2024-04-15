namespace Tempore.Tests.Tempore.Client.EmployeeClient;

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

using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

using TemporeServer::Tempore.Server.Services.Interfaces;

using Xunit;

using AgentState = global::Tempore.Client.AgentState;

public partial class TimestampScheduledDayAssociatorInvokableFacts
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

        public static IEnumerable<object[]> Data()
        {
            yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 15), new DateTimeOffset(new DateTime(2025, 1, 1).Add(new TimeSpan(8, 0, 0))), true };
            yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 15), new DateTimeOffset(new DateTime(2025, 1, 7).Add(new TimeSpan(12, 0, 0))), true };
            yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 15), new DateTimeOffset(new DateTime(2025, 1, 15).Add(new TimeSpan(16, 0, 0))), true };

            yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 15), new DateTimeOffset(new DateTime(2024, 12, 31).Add(new TimeSpan(16, 0, 0))), false };
            yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 15), new DateTimeOffset(new DateTime(2025, 1, 16).Add(new TimeSpan(16, 0, 0))), false };
            yield return new object[] { new DateTime(2025, 1, 1), new DateTime(2025, 1, 15), new DateTimeOffset(new DateTime(2025, 1, 15).Add(new TimeSpan(20, 0, 0))), false };
        }

        [Theory]
        [MemberData(nameof(Data))]
        [Trait(Traits.Category, Category.Integration)]
        public async Task Associates_Or_Not_A_ScheduledDay_To_The_Timestamp_As_Expected_Async(DateTime startDateTime, DateTime expireDateTime, DateTimeOffset dateTimeOffset, bool shouldAssign)
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
            var shiftAssignmentRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();

            var startDate = new DateOnly(assignShiftToEmployeesRequest.StartDate.Year, assignShiftToEmployeesRequest.StartDate.Month, assignShiftToEmployeesRequest.StartDate.Day);
            var expireDate = new DateOnly(assignShiftToEmployeesRequest.ExpireDate.Year, assignShiftToEmployeesRequest.ExpireDate.Month, assignShiftToEmployeesRequest.ExpireDate.Day);
            var shiftAssignment = await shiftAssignmentRepository.All()
                                      .Include(assignment => assignment.ScheduledShift)
                                      .ThenInclude(scheduledShift => scheduledShift.Shift)
                .ThenInclude(s => s.Days).ThenInclude(day => day.Timetable).SingleAsync(
                    assignment => assignment.EmployeeId == employee.Id
                                  && assignment.ScheduledShift.ShiftId == assignShiftToEmployeesRequest.ShiftId
                                  && assignment.ScheduledShift.StartDate == startDate && assignment.ScheduledShift.ExpireDate == expireDate
                                  && assignment.ScheduledShift.EffectiveWorkingTime == assignShiftToEmployeesRequest.EffectiveWorkingTime);

            var scheduledDayIds = new List<Guid>();
            for (var currentDate = startDate; currentDate <= expireDate; currentDate = currentDate.AddDays(1))
            {
                var daySchedulerService = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IDaySchedulerService>();
                var scheduledDay = await daySchedulerService.ScheduleDayAsync(currentDate, shiftAssignment);

                var scheduledDayRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<ScheduledDay, ApplicationDbContext>>();
                scheduledDayRepository.Add(scheduledDay);
                await scheduledDayRepository.SaveChangesAsync();

                scheduledDayIds.Add(scheduledDay.Id);
            }

            // Timestamps
            var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

            var employeeFromDeviceTimestampRequest = AddEmployeeFromDeviceTimestampRequest.Create(
                   device!.Id,
                   dateTimeOffset,
                   employeeFromDeviceDto.EmployeeIdOnDevice);

            var timestampId = await timestampClient.AddEmployeeFromDeviceTimestampAsync(employeeFromDeviceTimestampRequest);

            // Act
            var timestampScheduledDayAssociatorInvokable = this.TemporeServerApplicationFactory!.Services.GetRequiredService<TemporeServer::Tempore.Server.Invokables.ScheduledDay.TimestampScheduledDayAssociatorInvokable>();
            await timestampScheduledDayAssociatorInvokable.Invoke();

            // Assert
            var timestampRepository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<Timestamp, ApplicationDbContext>>();
            var storedTimestamp = timestampRepository.Find(timestamp => timestamp.Id == timestampId).FirstOrDefault();
            storedTimestamp.Should().NotBeNull();

            if (shouldAssign)
            {
                storedTimestamp!.ScheduledDayId.Should().NotBeNull();
                scheduledDayIds.Should().Contain(storedTimestamp!.ScheduledDayId!.Value);
            }
            else
            {
                storedTimestamp!.ScheduledDayId.Should().BeNull();
            }
        }
    }
}