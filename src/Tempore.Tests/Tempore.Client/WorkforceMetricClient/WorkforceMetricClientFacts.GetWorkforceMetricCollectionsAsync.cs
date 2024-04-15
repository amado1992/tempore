namespace Tempore.Tests.Tempore.Client.WorkforceMetricClient
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.AspNetCore.Http;

    using Xunit;

    public partial class WorkforceMetricClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetWorkforceMetricCollectionsAsync_Method : EnvironmentTestBase
        {
            public The_GetWorkforceMetricCollectionsAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> InvalidSkipAndTake()
            {
                yield return new object[] { new GetWorkforceMetricCollectionsRequest { Skip = -1, Take = 0 } };
                yield return new object[] { new GetWorkforceMetricCollectionsRequest { Skip = 0, Take = -1 } };
                yield return new object[] { new GetWorkforceMetricCollectionsRequest { Skip = -1, Take = -1 } };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var workforceMetricClientClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => workforceMetricClientClient.GetWorkforceMetricCollectionsAsync(new GetWorkforceMetricCollectionsRequest()));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Theory]
            [MemberData(nameof(InvalidSkipAndTake))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_With_Invalid_Request_Data_Async(GetWorkforceMetricCollectionsRequest request)
            {
                var workforceMetricClientClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => workforceMetricClientClient.GetWorkforceMetricCollectionsAsync(request));

                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_With_A_Valid_Request_Async()
            {
                var workforceMetricClientClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var request = new GetWorkforceMetricCollectionsRequest
                {
                    Skip = 0,
                    Take = int.MaxValue,
                };

                var response = await workforceMetricClientClient.GetWorkforceMetricCollectionsAsync(request);
                response.Should().NotBeNull();
                response.Count.Should().BeGreaterThan(0);
                response.Items.Should().NotBeEmpty();
            }
        }
    }
}