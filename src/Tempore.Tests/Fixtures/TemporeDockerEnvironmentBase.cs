namespace Tempore.Tests.Fixtures
{
    using System.Threading.Tasks;

    using global::Tempore.Tests.Fixtures.Interfaces;

    using global::TestEnvironment.Docker;

    using Xunit;

    /// <summary>
    /// The tempore docker environment base.
    /// </summary>
    public abstract class TemporeDockerEnvironmentBase : ITemporeDockerEnvironment, IAsyncLifetime
    {
        /// <summary>
        /// The _docker environment.
        /// </summary>
        private IDockerEnvironment? dockerEnvironment;

        /// <summary>
        /// The initialize async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            this.dockerEnvironment ??= this.Build();

            await this.dockerEnvironment.UpAsync();
        }

        /// <summary>
        /// The dispose async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task DisposeAsync()
        {
            if (this.dockerEnvironment is null)
            {
                return;
            }

            await this.dockerEnvironment.DownAsync();
            await this.dockerEnvironment.DisposeAsync();

            this.dockerEnvironment = null;
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="dockerEnvironmentBuilder">
        /// The docker environment builder.
        /// </param>
        protected abstract void Configure(DockerEnvironmentBuilder dockerEnvironmentBuilder);

        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IDockerEnvironment"/>.
        /// </returns>
        private IDockerEnvironment Build()
        {
            var dockerEnvironmentBuilder = new DockerEnvironmentBuilder();

            this.Configure(dockerEnvironmentBuilder);

            return dockerEnvironmentBuilder.Build();
        }
    }
}