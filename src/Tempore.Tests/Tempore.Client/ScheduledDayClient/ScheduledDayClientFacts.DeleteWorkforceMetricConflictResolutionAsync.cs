namespace Tempore.Tests.Tempore.Client.ScheduledDayClient
{
    extern alias TemporeServer;

    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.AspNetCore.Http;

    using Xunit;

    public partial class ScheduledDayClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_DeleteWorkforceMetricConflictResolutionAsync_Method : EnvironmentTestBase
        {
            public The_DeleteWorkforceMetricConflictResolutionAsync_Method(
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
                // Arrange
                var scheduledDayClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<ScheduledDayClient>();

                // Act
                var apiException = await Assert.ThrowsAsync<ApiException>(
                                       () => scheduledDayClient.DeleteWorkforceMetricConflictResolutionAsync(Guid.NewGuid()));

                // Assert
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }
        }
    }
}