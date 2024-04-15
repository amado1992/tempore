// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationInitializationService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The application initialization service.
    /// </summary>
    public class ApplicationInitializationService : BackgroundService, IApplicationInitializationService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ApplicationInitializationService> logger;

        private readonly IHostApplicationLifetime applicationLifetime;

        /// <summary>
        /// The database initialization service.
        /// </summary>
        private readonly IDatabaseInitializationService databaseInitializationService;

        /// <summary>
        /// The keycloak initialization service.
        /// </summary>
        private readonly IKeycloakInitializationService keycloakInitializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInitializationService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="applicationLifetime">
        /// The application life time.
        /// </param>
        /// <param name="databaseInitializationService">
        /// The database initialization service.
        /// </param>
        /// <param name="keycloakInitializationService">
        /// The keycloak initialization service.
        /// </param>
        public ApplicationInitializationService(
            ILogger<ApplicationInitializationService> logger,
            IHostApplicationLifetime applicationLifetime,
            IDatabaseInitializationService databaseInitializationService,
            IKeycloakInitializationService keycloakInitializationService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(applicationLifetime);
            ArgumentNullException.ThrowIfNull(databaseInitializationService);
            ArgumentNullException.ThrowIfNull(keycloakInitializationService);

            this.logger = logger;
            this.applicationLifetime = applicationLifetime;
            this.databaseInitializationService = databaseInitializationService;
            this.keycloakInitializationService = keycloakInitializationService;
        }

        /// <summary>
        /// Gets a value indicating whether is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The wait async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task WaitAsync()
        {
            while (!this.IsInitialized)
            {
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// The initialize async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            try
            {
                await this.databaseInitializationService.InitializeAsync();
                await this.keycloakInitializationService.InitializeAsync();

                this.IsInitialized = true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occurred initializing the application");

                this.applicationLifetime.StopApplication();
            }
        }

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="stoppingToken">
        /// The stopping token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.InitializeAsync();
        }
    }
}