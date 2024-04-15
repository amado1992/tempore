namespace Tempore.Tests.Tempore.Server.Handlers.Agents
{
    extern alias TemporeServer;

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
    using TemporeServer::Tempore.Server.Requests.Agents;
    using TemporeServer::Tempore.Server.Specs;

    using Xunit;

    /// <summary>
    /// The agents request handler facts.
    /// </summary>
    public class AgentsRequestHandlerFacts
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
                var agentRepositoryMock = new Mock<IRepository<Agent, ApplicationDbContext>>();

                var agentsRequestHandler = new GetAgentsRequestHandler(
                    NullLogger<GetAgentsRequestHandler>.Instance,
                    agentRepositoryMock.Object);

                var agentsRequest = new GetAgentsRequest
                {
                    Skip = 0,
                    Take = 100,
                };

                var isPaginationOptionsEnabled = true;
                agentRepositoryMock.Setup(repository => repository.CountAsync(It.Is<Specification<Agent>>(specification => !specification.PaginationOptions.IsEnable)))
                    .Callback<ISpecification<Agent>>(
                        _ => isPaginationOptionsEnabled = false);

                // Act
                await agentsRequestHandler.Handle(agentsRequest, CancellationToken.None);

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
                var agentRepositoryMock = new Mock<IRepository<Agent, ApplicationDbContext>>();

                var agentsRequestHandler = new GetAgentsRequestHandler(
                    NullLogger<GetAgentsRequestHandler>.Instance,
                    agentRepositoryMock.Object);
                var agentsRequest = new GetAgentsRequest
                {
                    Skip = 0,
                    Take = 100,
                };

                agentRepositoryMock.Setup(repository => repository.CountAsync(It.IsAny<ISpecification<Agent>>()))
                    .ReturnsAsync(100);

                var isPaginationOptionDisabled = false;
                agentRepositoryMock.Setup(repository => repository.FindAsync(It.IsAny<ISpecification<Agent>>()))
                    .Callback<ISpecification<Agent>>(
                        specification =>
                        {
                            if (specification is Specification<Agent> agentSpecification)
                            {
                                isPaginationOptionDisabled = agentSpecification.PaginationOptions.IsEnable;
                            }
                        });

                // Act
                await agentsRequestHandler.Handle(agentsRequest, CancellationToken.None);

                // Assert
                isPaginationOptionDisabled.Should().BeTrue();
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
                var agentRepositoryMock = new Mock<IRepository<Agent, ApplicationDbContext>>();

                var agentsRequestHandler = new GetAgentsRequestHandler(
                    NullLogger<GetAgentsRequestHandler>.Instance,
                    agentRepositoryMock.Object);

                var agentsRequest = new GetAgentsRequest
                {
                    Skip = 0,
                    Take = 100,
                };

                agentRepositoryMock.Setup(repository => repository.CountAsync(It.Is<Specification<Agent>>(specification => !specification.PaginationOptions.IsEnable)))
                    .ReturnsAsync(0);

                // Act
                await agentsRequestHandler.Handle(agentsRequest, CancellationToken.None);

                // Assert
                agentRepositoryMock.Verify(
                    repository => repository.FindAsync(It.IsAny<ISpecification<Agent>>()),
                    Times.Never());
            }
        }
    }
}