namespace Tempore.Tests.Tempore.Client.FileProcessingClient
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

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

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Xunit;

    using FileType = global::Tempore.Client.FileType;

    public partial class FileProcessingClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_ProcessFileAsync_Method : EnvironmentTestBase
        {
            public The_ProcessFileAsync_Method(
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
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>();
                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.ProcessAsync(Guid.NewGuid()));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_If_File_Not_Found_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.ProcessAsync(Guid.NewGuid()));
                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status409Conflict_StatusCode_If_A_File_Processing_Process_Is_Already_Running_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
                fileId.Should().NotBeEmpty();

                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                await fileProcessingClient.ProcessAsync(fileId);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.ProcessAsync(fileId));
                apiException.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Imports_Employees_Data_From_File_Into_The_Database_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
                fileId.Should().NotBeEmpty();

                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                await fileProcessingClient.ProcessAsync(fileId);

                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                var repository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<Employee, ApplicationDbContext>>();

                // TODO: Improve this later. It's is a good approximation for now.
                var employees = repository.All().ToList();
                employees.Should().NotBeEmpty();
                foreach (var employee in employees)
                {
                    employee.ExternalId.Should().NotBeNullOrEmpty();
                    employee.FullName.Should().NotBeNullOrEmpty();
                }
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Imports_Employees_Data_From_File_Into_The_Database_Without_Error_If_The_Method_Is_Called_More_Than_Once_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);
                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
                fileId.Should().NotBeEmpty();

                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                await fileProcessingClient.ProcessAsync(fileId);

                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                await fileProcessingClient.ProcessAsync(fileId);

                await TaskHelper.RepeatAsync(
                    () => fileProcessingClient.IsProcessingFileProcessRunningAsync());

                var repository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<Employee, ApplicationDbContext>>();

                // TODO: Improve this later. It's is a good approximation for now
                // TODO: Try with different files and override the existing content?
                var employees = repository.All().ToList();
                employees.Should().NotBeEmpty();
                foreach (var employee in employees)
                {
                    employee.ExternalId.Should().NotBeNullOrEmpty();
                    employee.FullName.Should().NotBeNullOrEmpty();
                }
            }
        }
    }
}