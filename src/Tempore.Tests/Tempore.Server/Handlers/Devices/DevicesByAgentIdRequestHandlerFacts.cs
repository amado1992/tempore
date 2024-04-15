namespace Tempore.Tests.Tempore.Server.Handlers.Devices
{
    extern alias TemporeAgent;
    extern alias TemporeServer;

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using TemporeServer::Tempore.Server.Handlers.Agents;
    using TemporeServer::Tempore.Server.Handlers.Devices;
    using TemporeServer::Tempore.Server.Requests.Devices;
    using TemporeServer::Tempore.Server.Specs;

    using Xunit;

    /// <summary>
    /// The agents request handler facts.
    /// </summary>
    public class DevicesByAgentIdRequestHandlerFacts
    {
        /// <summary>
        /// The handle method.
        /// </summary>
        public class The_Handle_Method
        {
            /// <summary>
            /// Calls count async of agent repository with pagination options disabled.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Calls_CountAsync_Of_Agent_Repository_With_PaginationOptions_Disabled()
            {
                // Arrange
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();
                var devicesByAgentIdRequestHandler = new DevicesByAgentIdRequestHandler(
                    NullLogger<DevicesByAgentIdRequestHandler>.Instance,
                    deviceRepositoryMock.Object);
                var agentsRequest = new DevicesByAgentIdRequest()
                {
                    AgentId = Guid.NewGuid(),
                    Skip = 0,
                    Take = 100,
                };

                var isPaginationOptionsEnabled = true;
                deviceRepositoryMock
                    .Setup(
                        repository => repository.CountAsync(
                            It.Is<Specification<Device>>(specification => !specification.PaginationOptions.IsEnable)))
                    .Callback<ISpecification<Device>>(_ => isPaginationOptionsEnabled = false);

                // Act
                await devicesByAgentIdRequestHandler.Handle(agentsRequest, CancellationToken.None);

                // Assert
                isPaginationOptionsEnabled.Should().BeFalse();
            }

            /// <summary>
            /// Calls find async of agent repository with pagination options enabled.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Calls_FindAsync_Of_Agent_Repository_With_PaginationOptions_Enabled()
            {
                // Arrange
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                var devicesByAgentIdRequestHandler = new DevicesByAgentIdRequestHandler(
                    NullLogger<DevicesByAgentIdRequestHandler>.Instance,
                    deviceRepositoryMock.Object);

                var request = new DevicesByAgentIdRequest
                {
                    AgentId = Guid.NewGuid(),
                    Skip = 0,
                    Take = 100,
                };

                deviceRepositoryMock.Setup(
                        repository => repository.CountAsync(
                            It.Is<Specification<Device>>(specification => !specification.PaginationOptions.IsEnable)))
                    .ReturnsAsync(100);

                var isPaginationOptionsEnabled = false;
                deviceRepositoryMock
                    .Setup(
                        repository => repository.FindAsync(
                            It.Is<Specification<Device>>(specification => specification.PaginationOptions.IsEnable)))
                    .Callback<ISpecification<Device>>(_ => isPaginationOptionsEnabled = true);

                // Act
                await devicesByAgentIdRequestHandler.Handle(request, CancellationToken.None);

                // Assert
                isPaginationOptionsEnabled.Should().BeTrue();
            }

            /// <summary>
            /// Does not call find async of agent repository if count async returns 0.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Does_Not_Call_FindAsync_Of_Agent_Repository_If_CountAsync_Returns_0()
            {
                // Arrange
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                var devicesByAgentIdRequestHandler = new DevicesByAgentIdRequestHandler(
                    NullLogger<DevicesByAgentIdRequestHandler>.Instance,
                    deviceRepositoryMock.Object);

                var agentsRequest = new DevicesByAgentIdRequest
                {
                    AgentId = Guid.NewGuid(),
                    Skip = 0,
                    Take = 100,
                };

                deviceRepositoryMock.Setup(repository => repository.CountAsync(It.IsAny<ISpecification<Device>>()))
                    .ReturnsAsync(0);

                // Act
                await devicesByAgentIdRequestHandler.Handle(agentsRequest, CancellationToken.None);

                // Assert
                deviceRepositoryMock.Verify(
                    repository => repository.FindAsync(It.IsAny<ISpecification<Device>>()),
                    Times.Never());
            }
        }
    }
}