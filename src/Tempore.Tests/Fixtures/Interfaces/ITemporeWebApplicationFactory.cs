namespace Tempore.Tests.Fixtures.Interfaces
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The TemporeWebApplicationFactory interface.
    /// </summary>
    public interface ITemporeWebApplicationFactory
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// The create client.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpClient"/>.
        /// </returns>
        HttpClient CreateClient();

        /// <summary>
        /// The create http client async.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="override">
        /// The override.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<HttpClient> CreateHttpClientAsync(string username = "", string password = "", bool @override = false, TimeSpan timeout = default);

        /// <summary>
        /// The create client async.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="override">
        /// The override.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <typeparam name="TClient">
        /// The client type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<TClient> CreateClientAsync<TClient>(string username = "", string password = "", bool @override = false, TimeSpan timeout = default);
    }
}