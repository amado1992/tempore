namespace Tempore.Tests.Fixtures.Interfaces
{
    extern alias TemporeServer;

    using global::Tempore.Client.Services.Interfaces;
    using global::Tempore.Hosting.Services.Interfaces;

    using Microsoft.AspNetCore.SignalR;

    using Moq;

    using TemporeServer::Tempore.Server.Hubs;
    using TemporeServer::Tempore.Server.Services.Interfaces;

    public interface ITemporeBackEndWebApplicationFactory : ITemporeWebApplicationFactory
    {
        /// <summary>
        /// Gets the environment variable service mock.
        /// </summary>
        Mock<IEnvironmentVariableService> EnvironmentVariableServiceMock { get; }

        /// <summary>
        /// Gets the notification service mock.
        /// </summary>
        Mock<INotificationService> NotificationServiceMock { get; }

        /// <summary>
        /// Gets the agent hub lifetime manager mock.
        /// </summary>
        Mock<IHubLifetimeManager<AgentHub>> AgentHubLifetimeManagerMock { get; }

        /// <summary>
        /// Gets the agent hub context mock.
        /// </summary>
        Mock<IHubContext<AgentHub, IAgentReceiver>> AgentHubContextMock { get; }
    }
}