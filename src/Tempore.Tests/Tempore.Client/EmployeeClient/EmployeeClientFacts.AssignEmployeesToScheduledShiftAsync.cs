namespace Tempore.Tests.Tempore.Client.EmployeeClient
{
    extern alias TemporeAgent;
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

    using FluentAssertions;

    using global::Tempore.Client;
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
    using TemporeServer::Tempore.Server.Specs;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;
    using ApplicationDbContext = global::Tempore.Storage.ApplicationDbContext;
    using FileType = global::Tempore.Client.FileType;

    public partial class EmployeeClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_AssignEmployeesToScheduledShiftAsync_Method : EnvironmentTestBase
        {
            public The_AssignEmployeesToScheduledShiftAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> Request_With_Empty_Values()
            {
                yield return new object[] { new AssignEmployeesToScheduledShiftRequest(), };
                yield return new object[] { new AssignEmployeesToScheduledShiftRequest { ShiftId = Guid.NewGuid() }, };
                yield return new object[]
                {
                    new AssignEmployeesToScheduledShiftRequest
                                            {
                                                ShiftId = Guid.NewGuid(),
                                                EmployeeIds = new List<Guid>
                                                              {
                                                                  Guid.NewGuid(),
                                                                  Guid.Empty,
                                                              },
                                            },
                };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AssignEmployeesToScheduledShiftAsync(new AssignEmployeesToScheduledShiftRequest()));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Theory]
            [MemberData(nameof(Request_With_Empty_Values))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_Request_Includes_Empty_Values_Async(AssignEmployeesToScheduledShiftRequest request)
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AssignEmployeesToScheduledShiftAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_ExpireDay_Is_Lower_Than_StartDate_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var (startDate, expireDate) = DateRangeGenerator.Next(-15);
                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AssignEmployeesToScheduledShiftAsync(new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = Guid.NewGuid(),
                    EmployeeIds = new List<Guid> { Guid.NewGuid() },
                    StartDate = startDate,
                    ExpireDate = expireDate,
                }));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_Shift_Not_Found_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var (startDate, expireDate) = DateRangeGenerator.Next(15);
                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AssignEmployeesToScheduledShiftAsync(new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = Guid.NewGuid(),
                    EmployeeIds = new List<Guid> { Guid.NewGuid() },
                    StartDate = startDate,
                    ExpireDate = expireDate,
                }));

                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_Employee_Not_Found_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var repository = this.TemporeServerApplicationFactory.Services
                    .GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await repository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();
                shift.Should().NotBeNull();

                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = new List<Guid> { Guid.NewGuid() },
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AssignEmployeesToScheduledShiftAsync(
                                       assignShiftToEmployeesRequest));

                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_When_The_Employees_Are_Not_Assigned_To_The_Shift_Async()
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

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var (startDate, expireDate) = DateRangeGenerator.Next();
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = { employee.Id },
                    StartDate = startDate,
                    ExpireDate = expireDate,
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status409Conflict_StatusCode_Trying_Add_A_New_Assignment_That_Intercepts_With_Existing_Assignments_Async()
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

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var (startDate1, expireDate2) = DateRangeGenerator.Next(30);

                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = new List<Guid> { employee.Id },
                    StartDate = startDate1,
                    ExpireDate = expireDate2,
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var dateRanges = new[]
                                  {
                                      (assignShiftToEmployeesRequest.StartDate, assignShiftToEmployeesRequest.ExpireDate),
                                      (assignShiftToEmployeesRequest.StartDate.AddDays(-30), assignShiftToEmployeesRequest.ExpireDate),
                                      (assignShiftToEmployeesRequest.StartDate.AddDays(-30), assignShiftToEmployeesRequest.ExpireDate.AddDays(30)),
                                      (assignShiftToEmployeesRequest.StartDate.AddDays(-30), assignShiftToEmployeesRequest.ExpireDate.AddDays(-15)),
                                      (assignShiftToEmployeesRequest.StartDate, assignShiftToEmployeesRequest.ExpireDate.AddDays(-15)),
                                      (assignShiftToEmployeesRequest.StartDate.AddDays(10), assignShiftToEmployeesRequest.ExpireDate.AddDays(-10)),
                                  };

                foreach (var (startDate, expireDate) in dateRanges)
                {
                    assignShiftToEmployeesRequest.StartDate = startDate;
                    assignShiftToEmployeesRequest.ExpireDate = expireDate;
                    var apiException = await Assert.ThrowsAsync<ApiException>(
                                           () => employeeClient.AssignEmployeesToScheduledShiftAsync(
                                               assignShiftToEmployeesRequest));

                    apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
                }
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status409Conflict_StatusCode_Trying_Add_A_New_Assignment_With_StartDate_In_A_Range_Of_The_Last_Assignment_Async()
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

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();
                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var (startDate, expireDate) = DateRangeGenerator.Next(30);

                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = new List<Guid> { employee.Id },
                    StartDate = startDate,
                    ExpireDate = expireDate,
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                assignShiftToEmployeesRequest.StartDate = assignShiftToEmployeesRequest.StartDate.AddDays(-30);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest));
                apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status409Conflict_StatusCode_Trying_Add_A_New_Assignment_With_An_StartDate_Lower_Than_The_Date_Of_The_Last_Generated_Scheduled_Day_Async()
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

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();
                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();

                var (startDate1, expireDate1) = DateRangeGenerator.Next(30);
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = new List<Guid> { employee.Id },
                    StartDate = startDate1,
                    ExpireDate = expireDate1,
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var scheduledShiftAssignmentRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();

                var startDate = DateOnly.FromDateTime(assignShiftToEmployeesRequest.StartDate.DateTime);

                var shiftAssignment = await scheduledShiftAssignmentRepository.All().Include(assignment => assignment.ScheduledShift).SingleOrDefaultAsync(assignment => assignment.ScheduledShift.ShiftId == assignShiftToEmployeesRequest.ShiftId && assignment.EmployeeId == employee.Id && assignment.ScheduledShift.StartDate == startDate);
                shiftAssignment.Should().NotBeNull();
                shiftAssignment!.LastGeneratedDayDate = DateOnly.FromDateTime(startDate1);

                scheduledShiftAssignmentRepository.Update(shiftAssignment);
                await scheduledShiftAssignmentRepository.SaveChangesAsync();

                assignShiftToEmployeesRequest.StartDate = assignShiftToEmployeesRequest.StartDate.AddDays(-30);
                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest));
                apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            }
        }
    }
}