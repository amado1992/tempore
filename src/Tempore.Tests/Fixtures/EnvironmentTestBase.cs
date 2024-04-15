namespace Tempore.Tests.Fixtures
{
    using System.Threading.Tasks;

    using global::Tempore.Tests.Fixtures.Interfaces;

    using Xunit;

    /// <summary>
    /// The environment test base.
    /// </summary>
    public class EnvironmentTestBase : IAsyncLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentTestBase"/> class.
        /// </summary>
        /// <param name="dockerEnvironment">
        ///     The tempore docker environment.
        /// </param>
        /// <param name="testEnvironmentInitializer">
        ///     The test environment initializer.
        /// </param>
        /// <param name="temporeServerApplicationFactory">
        ///     The tempore api application factory.
        /// </param>
        /// <param name="temporeAgentFactory">
        ///     The tempore agent factory.
        /// </param>
        protected EnvironmentTestBase(
            ITemporeDockerEnvironment dockerEnvironment,
            ITestEnvironmentInitializer testEnvironmentInitializer,
            ITemporeBackEndWebApplicationFactory? temporeServerApplicationFactory,
            ITemporeBackEndWebApplicationFactory? temporeAgentFactory = default)
        {
            this.DockerEnvironment = dockerEnvironment;
            this.TemporeServerApplicationFactory = temporeServerApplicationFactory;
            this.TemporeAgentApplicationFactory = temporeAgentFactory;
            this.TestEnvironmentInitializer = testEnvironmentInitializer;
        }

        /// <summary>
        /// Gets the tempore docker environment.
        /// </summary>
        public ITemporeDockerEnvironment DockerEnvironment { get; }

        /// <summary>
        /// Gets the tempore api host application factory.
        /// </summary>
        public ITemporeBackEndWebApplicationFactory? TemporeServerApplicationFactory { get; }

        /// <summary>
        /// Gets or sets the tempore agent application factory.
        /// </summary>
        public ITemporeBackEndWebApplicationFactory? TemporeAgentApplicationFactory { get; set; }

        /// <summary>
        /// Gets the test environment initializer.
        /// </summary>
        public ITestEnvironmentInitializer TestEnvironmentInitializer { get; }

        /// <summary>
        /// The initialize async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            this.ResetMocks();

            this.TestEnvironmentInitializer.TemporeServerApplicationFactory = this.TemporeServerApplicationFactory;
            this.TestEnvironmentInitializer.TemporeAgentApplicationFactory = this.TemporeAgentApplicationFactory;

            await this.TestEnvironmentInitializer.InitializeAsync();
        }

        /// <summary>
        /// The dispose async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// The reset mocks.
        /// </summary>
        private void ResetMocks()
        {
        }
    }
}