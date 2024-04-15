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
    public class UpdateDeviceStateRequestHandlerFacts
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
            public async Task Updates_The_DeviceState_Async()
            {
                // Arrange
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();
                var devicesByAgentIdRequestHandler = new UpdateDeviceStateRequestHandler(
                    NullLogger<UpdateDeviceStateRequestHandler>.Instance,
                    deviceRepositoryMock.Object);

                var deviceId = Guid.NewGuid();
                var device = new Device
                {
                    Id = deviceId,
                    State = DeviceState.Online,
                };

                deviceRepositoryMock.Setup(repository => repository.SingleOrDefaultAsync(It.IsAny<ISpecification<Device>>()))
                    .ReturnsAsync(device);

                var deviceStateRequest = new UpdateDeviceStateRequest
                {
                    DeviceId = deviceId,
                    DeviceState = DeviceState.Offline,
                };

                // Act
                await devicesByAgentIdRequestHandler.Handle(deviceStateRequest, CancellationToken.None);

                // Assert
                deviceRepositoryMock.Verify(repository => repository.Update(It.Is<Device>(d => d.Id == deviceId && d.State == DeviceState.Offline)));
                deviceRepositoryMock.Verify(repository => repository.SaveChangesAsync());
            }
        }
    }
}