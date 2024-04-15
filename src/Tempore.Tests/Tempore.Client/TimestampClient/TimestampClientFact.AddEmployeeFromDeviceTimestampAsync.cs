namespace Tempore.Tests.Tempore.Client.TimestampClient
{
    extern alias TemporeAgent;

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

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Xunit;

    using AgentState = global::Tempore.Client.AgentState;

    public partial class TimestampClientFact
    {
        [Collection(nameof(DockerCollection))]
        public class The_AddEmployeeFromDeviceTimestampAsync_Method : EnvironmentTestBase
        {
            public The_AddEmployeeFromDeviceTimestampAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> GetAddEmployeeFromDeviceTimestampRequestsWithoutRequiredProperties()
            {
                yield return new object[]
                {
                    new AddEmployeeFromDeviceTimestampRequest(),
                };

                yield return new object[]
                {
                    AddEmployeeFromDeviceTimestampRequest.Create(new TimestampDto()),
                };

                var timestamp00 = new TimestampDto
                {
                    DateTime = DateTimeOffset.Now,
                };

                yield return new object[]
                             {
                                 AddEmployeeFromDeviceTimestampRequest.Create(timestamp00),
                             };

                var timestamp01 = new TimestampDto
                {
                    DateTime = DateTimeOffset.Now,
                    EmployeeFromDevice = new EmployeeFromDeviceDto(),
                };

                yield return new object[]
                             {
                                 AddEmployeeFromDeviceTimestampRequest.Create(timestamp01),
                             };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => timestampClient.AddEmployeeFromDeviceTimestampAsync(new AddEmployeeFromDeviceTimestampRequest()));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Theory]
            [Trait(Traits.Category, Category.Integration)]
            [MemberData(nameof(GetAddEmployeeFromDeviceTimestampRequestsWithoutRequiredProperties))]
            public async Task
                Throws_ApiException_With_StatusStatus400BadRequest_StatusCode_When_Required_Properties_Are_Missing_Async(
                    AddEmployeeFromDeviceTimestampRequest request)
            {
                var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>(
                                          TestEnvironment.Tempore.Username,
                                          TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(
                                       () => timestampClient.AddEmployeeFromDeviceTimestampAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_EmployeeFromDevice_DoesNot_Exist_Async()
            {
                var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var timestampDto = new TimestampDto
                {
                    DateTime = DateTimeOffset.Now,
                    EmployeeFromDevice = new EmployeeFromDeviceDto
                    {
                        EmployeeIdOnDevice = Guid.NewGuid().ToString(),
                    },
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(() => timestampClient.AddEmployeeFromDeviceTimestampAsync(AddEmployeeFromDeviceTimestampRequest.Create(timestampDto)));
                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status409Conflict_StatusCode_When_A_Timestamp_By_EmployeeFromDeviceId_And_Date_Already_Exist_Async()
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

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);
                var faker = new Faker<EmployeeFromDeviceDto>();
                faker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                faker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());
                faker.RuleFor(dto => dto.DeviceId, f => device!.Id);

                var employeeFromDeviceDto = faker.Generate();
                await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

                var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>(
                                          TestEnvironment.Tempore.Username,
                                          TestEnvironment.Tempore.Password);

                var timestampDto = new TimestampDto
                {
                    DateTime = DateTimeOffset.Now,
                    EmployeeFromDevice = new EmployeeFromDeviceDto
                    {
                        DeviceId = employeeFromDeviceDto.DeviceId,
                        EmployeeIdOnDevice = employeeFromDeviceDto.EmployeeIdOnDevice,
                    },
                };

                var request = AddEmployeeFromDeviceTimestampRequest.Create(timestampDto);
                await timestampClient.AddEmployeeFromDeviceTimestampAsync(request);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => timestampClient.AddEmployeeFromDeviceTimestampAsync(request));
                apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_When_EmployeeFromDevice_Exist_Async()
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

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);
                var faker = new Faker<EmployeeFromDeviceDto>();
                faker.RuleFor(dto => dto.FullName, f => f.Name.FullName());
                faker.RuleFor(dto => dto.EmployeeIdOnDevice, f => Guid.NewGuid().ToString());
                faker.RuleFor(dto => dto.DeviceId, f => device!.Id);

                var employeeFromDeviceDto = faker.Generate();
                var employeeFromDeviceId = await employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));

                var timestampClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<TimestampClient>(
                                          TestEnvironment.Tempore.Username,
                                          TestEnvironment.Tempore.Password);

                var timestampDto = new TimestampDto
                {
                    DateTime = DateTimeOffset.Now,
                    EmployeeFromDevice = new EmployeeFromDeviceDto
                    {
                        DeviceId = employeeFromDeviceDto.DeviceId,
                        EmployeeIdOnDevice = employeeFromDeviceDto.EmployeeIdOnDevice,
                    },
                };

                var timestampId = await timestampClient.AddEmployeeFromDeviceTimestampAsync(AddEmployeeFromDeviceTimestampRequest.Create(timestampDto));

                var repository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Timestamp, Storage.ApplicationDbContext>>();

                var storedTimestamp = await repository.SingleOrDefaultAsync(timestamp => timestamp.Id == timestampId);

                storedTimestamp.Should().NotBeNull();
                storedTimestamp!.DateTime.Subtract(timestampDto.DateTime).Should().BeLessThan(TimeSpan.FromSeconds(1));
                storedTimestamp.EmployeeFromDeviceId.Should().Be(employeeFromDeviceId);
            }
        }
    }
}