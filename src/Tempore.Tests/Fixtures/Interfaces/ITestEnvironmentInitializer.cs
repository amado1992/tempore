namespace Tempore.Tests.Fixtures.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The TestEnvironmentInitializer interface.
    /// </summary>
    public interface ITestEnvironmentInitializer
    {
        /// <summary>
        /// Gets or sets the tempore server application factory.
        /// </summary>
        ITemporeBackEndWebApplicationFactory? TemporeServerApplicationFactory { get; set; }

        /// <summary>
        /// Gets or sets the tempore agent application factory.
        /// </summary>
        ITemporeBackEndWebApplicationFactory? TemporeAgentApplicationFactory { get; set; }

        /// <summary>
        /// The initialize async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task InitializeAsync();
    }
}