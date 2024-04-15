namespace Tempore.Tests.Tempore.Client.FileProcessingClient
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Xunit;

    public partial class FileProcessingClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetListFilesAsync_Method : EnvironmentTestBase
        {
            public The_GetListFilesAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_The_ListFiles_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                var fileList = await fileProcessingClient.GetListFilesAsync(0, 100);

                fileList.Should().NotBeNull();
                fileList.Count.Should().BeGreaterThanOrEqualTo(0);
            }
        }
    }
}