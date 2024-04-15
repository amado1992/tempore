namespace Tempore.Tests.Tempore.Client.ShiftClient
{
    extern alias TemporeServer;

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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Services;

    using Xunit;

    public partial class ShiftClientFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_GetShiftByIdAsync_Method : EnvironmentTestBase
        {
            public The_GetShiftByIdAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_In_GetShiftByIdAsync_Async()
            {
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>();

                var apiException = await Assert.ThrowsAsync<ApiException>(() => shiftClient.GetShiftByIdAsync(Guid.NewGuid()));

                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_When_Shift_Not_Found_Async()
            {
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var apiException = await Assert.ThrowsAsync<ApiException>(() => shiftClient.GetShiftByIdAsync(Guid.NewGuid()));

                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Succeeds_When_The_Shift_Exist_Async()
            {
                var shiftClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<ShiftClient>(
                                      TestEnvironment.Tempore.Username,
                                      TestEnvironment.Tempore.Password);

                var shiftRepository = this.TemporeServerApplicationFactory.Services.GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.Find(shift => shift.Name == DefaultValues.DefaultShiftName).FirstOrDefaultAsync();
                var shiftDto = await shiftClient.GetShiftByIdAsync(shift!.Id);

                shiftDto.Should().NotBeNull();
            }
        }
    }
}