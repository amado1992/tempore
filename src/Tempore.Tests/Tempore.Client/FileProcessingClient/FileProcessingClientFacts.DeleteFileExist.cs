namespace Tempore.Tests.Tempore.Client.FileProcessingClient
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Xunit;

    public partial class FileProcessingClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_DeleteFileExistAsync_Method : EnvironmentTestBase
        {
            public The_DeleteFileExistAsync_Method(
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

                // Act & Assert
                Guid guid = Guid.NewGuid();
                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.DeleteFileExistAsync(guid));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Should_Delete_Existing_File()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var repository = this.TemporeServerApplicationFactory!.Services.GetRequiredService<IRepository<DataFile, ApplicationDbContext>>();

                await using var manifestResourceStream =
                    typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                        "Tempore.Tests.Resources.employees.xlsx");

                var dataFile = new DataFile
                {
                    CreatedDate = DateTime.UtcNow,
                    Data = new byte[manifestResourceStream!.Length],
                    FileName = Guid.NewGuid().ToString(),
                    FileType = Storage.Entities.FileType.PayDay,
                };

                repository.Add(dataFile);
                await repository.SaveChangesAsync();

                await fileProcessingClient.DeleteFileExistAsync(dataFile.Id);

                var dataFileDeleted = await repository.SingleOrDefaultAsync(x => x.FileName == dataFile.FileName);

                dataFileDeleted.Should().BeNull();
            }
        }
    }
}