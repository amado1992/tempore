namespace Tempore.Tests.Fixtures
{
    extern alias TemporeAgent;
    extern alias TemporeServer;

    using System.Threading.Tasks;

    using global::Tempore.Infrastructure.Keycloak.Services.Interfaces;
    using global::Tempore.Tests.Fixtures.Interfaces;

    using Microsoft.Extensions.DependencyInjection;

    using TemporeAgent::Tempore.Agent.Services.Interfaces;

    using TemporeServer::Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The test environment initializer.
    /// </summary>
    public class TestEnvironmentInitializer : ITestEnvironmentInitializer
    {
        /// <summary>
        /// The _initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Gets or sets the tempore api host application factory.
        /// </summary>
        public ITemporeBackEndWebApplicationFactory? TemporeServerApplicationFactory { get; set; }

        public ITemporeBackEndWebApplicationFactory? TemporeAgentApplicationFactory { get; set; }

        /// <summary>
        /// The initialize async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            if (this.initialized)
            {
                return;
            }

            if (this.TemporeServerApplicationFactory is not null)
            {
                var initializationService = this.TemporeServerApplicationFactory.Services.GetRequiredService<IApplicationInitializationService>();
                await initializationService.WaitAsync();
            }

            if (this.TemporeAgentApplicationFactory is not null)
            {
                this.TemporeAgentApplicationFactory.Services.GetRequiredService<ITokenResolver>();
            }

            // TODO: Also waits for agent
            this.initialized = true;
        }
    }
}