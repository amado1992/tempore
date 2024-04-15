namespace Tempore.Tests.Tempore.Client.WorkforceMetricClient
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    public partial class WorkforceMetricClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetWorkforceMetricsSchemaAsync_Method : EnvironmentTestBase
        {
            public The_GetWorkforceMetricsSchemaAsync_Method(
                DockerEnvironment dockerEnvironment, TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            public static IEnumerable<object[]> SchemaTypes()
            {
                yield return new object[] { SchemaType.Display };
                yield return new object[] { SchemaType.Export };
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var workforceMetricClientClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(
                                       () => workforceMetricClientClient.GetWorkforceMetricsSchemaAsync(
                                           Guid.Empty, SchemaType.Display));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status400BadRequest_StatusCode_With_Empty_WorkforceCollectionId_Async()
            {
                var workforceMetricClientClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(
                                       () => workforceMetricClientClient.GetWorkforceMetricsSchemaAsync(
                                           Guid.Empty, SchemaType.Display));
                apiException.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_The_WorkforceMetricCollectionId_Not_Found_Async()
            {
                var workforceMetricClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var apiException =
                    await Assert.ThrowsAsync<ApiException>(
                        () => workforceMetricClient.GetWorkforceMetricsSchemaAsync(Guid.NewGuid(), SchemaType.Display));

                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Theory]
            [MemberData(nameof(SchemaTypes))]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_The_Schema_From_A_Valid_WorkforceMetricCollectionId_Async(SchemaType schemaType)
            {
                var workforceMetricClient =
                    await this.TemporeServerApplicationFactory!.CreateClientAsync<WorkforceMetricClient>(
                        TestEnvironment.Tempore.Username,
                        TestEnvironment.Tempore.Password);

                var getWorkforceMetricCollectionsRequest = new GetWorkforceMetricCollectionsRequest
                {
                    Skip = 0,
                    Take = 1,
                };

                var workforceMetricCollectionDtoPagedResponse = await workforceMetricClient.GetWorkforceMetricCollectionsAsync(
                                                                    getWorkforceMetricCollectionsRequest);

                var workforceMetricCollectionDto = workforceMetricCollectionDtoPagedResponse.Items.FirstOrDefault();
                workforceMetricCollectionDto.Should().NotBeNull();

                var schema = await workforceMetricClient.GetWorkforceMetricsSchemaAsync(workforceMetricCollectionDto!.Id, schemaType);
                schema.Should().NotBeEmpty();
            }
        }
    }
}