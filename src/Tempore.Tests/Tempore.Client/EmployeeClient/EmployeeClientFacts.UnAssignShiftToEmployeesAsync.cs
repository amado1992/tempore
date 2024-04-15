namespace Tempore.Tests.Tempore.Client.EmployeeClient
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

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Services;

    using Xunit;

    using ApplicationDbContext = global::Tempore.Storage.ApplicationDbContext;
    using FileType = global::Tempore.Client.FileType;

    public partial class EmployeeClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_UnAssignShiftToEmployeesAsync_Method : EnvironmentTestBase
        {
            public The_UnAssignShiftToEmployeesAsync_Method(
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
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.UnAssignShiftToEmployeesAsync(new UnAssignShiftToEmployeesRequest()));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_When_Request_Includes_Empty_Values_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var request = new UnAssignShiftToEmployeesRequest
                {
                    ShiftToEmployeeIds = new List<Guid> { Guid.NewGuid(), Guid.Empty },
                };

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.UnAssignShiftToEmployeesAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_Shift_Assignment_Not_Found_Async()
            {
                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => employeeClient.UnAssignShiftToEmployeesAsync(new UnAssignShiftToEmployeesRequest
                {
                    ShiftToEmployeeIds = new List<Guid> { Guid.NewGuid() },
                }));

                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Fact(Skip = "This test is incompleted")]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_When_The_Employees_Are_Assigned_To_The_Shift_Async()
            {
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

                var employeeIds = await employeeRepository.All().Take(10).Select(employee => employee.Id).ToListAsync();
                var assignShiftToEmployeesRequest = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = shift!.Id,
                    EmployeeIds = employeeIds,
                    StartDate = DateTimeOffset.Now,
                    ExpireDate = DateTimeOffset.Now,
                };

                var employeeClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<EmployeeClient>(
                                         TestEnvironment.Tempore.Username,
                                         TestEnvironment.Tempore.Password);

                await employeeClient.AssignEmployeesToScheduledShiftAsync(assignShiftToEmployeesRequest);

                var shiftAssignmentRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();
                var shiftAssignments = await shiftAssignmentRepository.All().Select(shiftAssignment => shiftAssignment.Id).Take(10).ToListAsync();
                shiftAssignments = shiftAssignments.OrderBy(guid => new Random().Next(100)).ToList();

                var request = new UnAssignShiftToEmployeesRequest
                {
                    ShiftToEmployeeIds = new List<Guid>
                                         {
                                             shiftAssignments.ElementAt(0), shiftAssignments.ElementAt(1),
                                         },
                };

                await employeeClient.UnAssignShiftToEmployeesAsync(request);
            }
        }
    }
}