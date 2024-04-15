namespace Tempore.Tests.Tempore.Client.WorkforceMetricClient
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
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Notifications.WorkforceMetrics;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;

    public partial class WorkforceMetricClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_ComputeWorkforceMetricsAsync_Method : EnvironmentTestBase
        {
            public The_ComputeWorkforceMetricsAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> Required_Fields_Empty_Data()
            {
                yield return new object[] { new ComputeWorkforceMetricsRequest() };
                yield return new object[] { new ComputeWorkforceMetricsRequest { StartDate = default } };
                yield return new object[]
                             {
                                 new ComputeWorkforceMetricsRequest
                                 {
                                     StartDate = DateTimeOffset.Now, EndDate = default,
                                 },
                             };
                yield return new object[]
                             {
                                 new ComputeWorkforceMetricsRequest
                                 {
                                     StartDate = DateTimeOffset.Now, EndDate = DateTimeOffset.Now.AddDays(-1),
                                 },
                             };

                yield return new object[]
                             {
                                 new ComputeWorkforceMetricsRequest
                                 {
                                     StartDate = DateTimeOffset.Now,
                                     EndDate = DateTimeOffset.Now.AddDays(1),
                                     WorkForceMetricCollectionIds = new List<Guid>
                                                                    {
                                                                        Guid.NewGuid(), Guid.Empty, Guid.NewGuid(),
                                                                    },
                                 },
                             };
                yield return new object[]
                             {
                                 new ComputeWorkforceMetricsRequest
                                 {
                                     StartDate = default,
                                     EndDate = DateTimeOffset.Now.AddDays(1),
                                     WorkForceMetricCollectionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                                 },
                             };

                yield return new object[]
                             {
                                 new ComputeWorkforceMetricsRequest
                                 {
                                     StartDate = DateTimeOffset.Now.AddDays(1),
                                     EndDate = default,
                                     WorkForceMetricCollectionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                                 },
                             };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var workforceMetricClientClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => workforceMetricClientClient.ComputeWorkforceMetricsAsync(new ComputeWorkforceMetricsRequest()));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Theory]
            [MemberData(nameof(Required_Fields_Empty_Data))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_Required_Fields_Are_Empty_Async(ComputeWorkforceMetricsRequest request)
            {
                var workforceMetricClientClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                                                      TestEnvironment.Tempore.Username,
                                                      TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => workforceMetricClientClient.ComputeWorkforceMetricsAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status409Conflict_StatusCode_The_Job_Is_Already_Scheduled_Async()
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

                var workforceMetricClientClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                                                      TestEnvironment.Tempore.Username,
                                                      TestEnvironment.Tempore.Password);

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

                var (startDate, expireDate) = DateRangeGenerator.Next();

                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    StartDate = startDate,
                    ExpireDate = expireDate,
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                    EmployeeIds = new List<Guid> { employee.Id },
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var getWorkforceMetricCollectionsRequest = new GetWorkforceMetricCollectionsRequest
                {
                    Skip = 0,
                    Take = int.MaxValue,
                };

                var response = await workforceMetricClientClient.GetWorkforceMetricCollectionsAsync(getWorkforceMetricCollectionsRequest);
                var metricCollection = response.Items.FirstOrDefault();
                metricCollection.Should().NotBeNull();

                // Act
                await TaskHelper.RepeatAsync(() => workforceMetricClientClient.IsComputeWorkforceMetricsProcessRunningAsync());

                var computeWorkforceMetricsRequest = new ComputeWorkforceMetricsRequest
                {
                    StartDate = startDate,
                    EndDate = expireDate,
                    WorkForceMetricCollectionIds = new List<Guid> { metricCollection!.Id },
                };

                this.TemporeServerApplicationFactory.NotificationServiceMock.Reset();

                await workforceMetricClientClient.ComputeWorkforceMetricsAsync(computeWorkforceMetricsRequest);

                var apiException = await Assert.ThrowsAsync<ApiException>(
                    () => workforceMetricClientClient.ComputeWorkforceMetricsAsync(computeWorkforceMetricsRequest));
                apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Notifies_Success_Upon_Completion_Via_NotificationService_Async()
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

                var workforceMetricClientClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                                                      TestEnvironment.Tempore.Username,
                                                      TestEnvironment.Tempore.Password);

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

                var (startDate, expireDate) = DateRangeGenerator.Next();

                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift.Id,
                    StartDate = startDate,
                    ExpireDate = expireDate,
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                    EmployeeIds = new List<Guid> { employee.Id },
                };

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var getWorkforceMetricCollectionsRequest = new GetWorkforceMetricCollectionsRequest
                {
                    Skip = 0,
                    Take = int.MaxValue,
                };

                var response = await workforceMetricClientClient.GetWorkforceMetricCollectionsAsync(getWorkforceMetricCollectionsRequest);
                var metricCollection = response.Items.FirstOrDefault();
                metricCollection.Should().NotBeNull();

                // Act
                await TaskHelper.RepeatAsync(() => workforceMetricClientClient.IsComputeWorkforceMetricsProcessRunningAsync());

                var computeWorkforceMetricsRequest = new ComputeWorkforceMetricsRequest
                {
                    StartDate = startDate,
                    EndDate = expireDate,
                    WorkForceMetricCollectionIds = new List<Guid> { metricCollection!.Id },
                };

                this.TemporeServerApplicationFactory.NotificationServiceMock.Reset();

                await workforceMetricClientClient.ComputeWorkforceMetricsAsync(computeWorkforceMetricsRequest);

                await this.TemporeServerApplicationFactory.NotificationServiceMock.WaitAndVerifyAsync(TimeSpan.FromSeconds(60), service => service.SuccessAsync<ComputeWorkforceMetricsProcessCompletedNotification>(TestEnvironment.Tempore.Username, It.IsAny<string>()), Times.Once);
            }
        }
    }
}