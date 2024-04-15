namespace Tempore.Tests.Tempore.Client.FileProcessingClient
{
    using System.IO;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.AspNetCore.Http;

    using Xunit;

    public partial class FileProcessingClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_UploadFileAsync_Method : EnvironmentTestBase
        {
            public The_UploadFileAsync_Method(
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

                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                // Act & Assert
                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.Unknown));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_For_Not_Supported_FileType_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!
                                           .CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.Unknown));
                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_For_Wrong_Format_File_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!
                                           .CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                // Wrong format file
                using var memoryStream = new MemoryStream();
                await memoryStream.WriteAsync(new byte[] { 1, 2, 3, 4, 5 });
                memoryStream.Seek(0, SeekOrigin.Begin);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.UploadFileAsync(new FileParameter(memoryStream, "employees.xlsx"), FileType.PayDay));
                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_NotEmpty_FileId_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);
                fileId.Should().NotBeEmpty();
            }
        }
    }
}