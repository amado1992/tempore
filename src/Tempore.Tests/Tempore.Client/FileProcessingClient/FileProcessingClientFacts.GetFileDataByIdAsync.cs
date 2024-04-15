﻿namespace Tempore.Tests.Tempore.Client.FileProcessingClient
{
    using System;
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
        public class The_GetFileDataByIdAsync_Method : EnvironmentTestBase
        {
            public The_GetFileDataByIdAsync_Method(
                DockerEnvironment dockerEnvironment, TemporeServerWebApplicationFactory temporeServerApplicationFactory, TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status401Unauthorized_StatusCode_When_Invalid_Credentials_Are_Used_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>();

                await using var manifestResourceStream = typeof(Client.FileProcessingClient.FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                // Act & Assert
                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.GetFileDataByIdAsync(Guid.Empty, 0, 100));
                apiException.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Returns_The_File_Content_Properly_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var fileId = await fileProcessingClient.UploadFileAsync(new FileParameter(manifestResourceStream, "employees.xlsx"), FileType.PayDay);

                var paginationResponse = await fileProcessingClient.GetFileDataByIdAsync(fileId, 0, 100);
                paginationResponse.Count.Should().BeGreaterThan(0);
                paginationResponse.Items.Count.Should().BeGreaterThan(0);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Throws_ApiException_With_Status404NotFound_StatusCode_If_File_Not_Found_Async()
            {
                var fileProcessingClient = await this.TemporeServerApplicationFactory!.CreateClientAsync<FileProcessingClient>(TestEnvironment.Tempore.Username, TestEnvironment.Tempore.Password);

                await using var manifestResourceStream = typeof(FileProcessingClientFacts).Assembly.GetManifestResourceStream(
                    "Tempore.Tests.Resources.employees.xlsx");

                var apiException = await Assert.ThrowsAsync<ApiException>(() => fileProcessingClient.GetFileDataByIdAsync(Guid.NewGuid(), 0, 100));
                apiException.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            }
        }
    }
}