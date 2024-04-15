// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentInitializationService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services
{
    using System;

    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.DependencyInjection;

    using Polly;

    using StoneAssemblies.Hikvision.Models;
    using StoneAssemblies.Hikvision.Services.Interfaces;

    using Tempore.Agent.Services.Interfaces;
    using Tempore.Client.Services.Interfaces;

    using TypedSignalR.Client;

    /// <summary>
    /// The agent background service.
    /// </summary>
    public class AgentInitializationService : BackgroundService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AgentInitializationService> logger;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The hub connection builder.
        /// </summary>
        private readonly IHubConnectionBuilder hubConnectionBuilder;

        /// <summary>
        /// The token resolver.
        /// </summary>
        private readonly ITokenResolver tokenResolver;

        /// <summary>
        /// The device info repository.
        /// </summary>
        private readonly IDeviceInfoRepository deviceInfoRepository;

        /// <summary>
        /// The service scope.
        /// </summary>
        private readonly IServiceScope serviceScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentInitializationService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="hubConnectionBuilder">
        /// The hub connection builder.
        /// </param>
        /// <param name="tokenResolver">
        /// The token resolver.
        /// </param>
        /// <param name="deviceInfoRepository">
        /// The device info repository.
        /// </param>
        /// <param name="serviceScopeFactory">
        /// The service scope factory.
        /// </param>
        public AgentInitializationService(
            ILogger<AgentInitializationService> logger,
            IConfiguration configuration,
            IHubConnectionBuilder hubConnectionBuilder,
            ITokenResolver tokenResolver,
            IDeviceInfoRepository deviceInfoRepository,
            IServiceScopeFactory serviceScopeFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(hubConnectionBuilder);
            ArgumentNullException.ThrowIfNull(tokenResolver);
            ArgumentNullException.ThrowIfNull(deviceInfoRepository);
            ArgumentNullException.ThrowIfNull(serviceScopeFactory);

            this.logger = logger;
            this.configuration = configuration;
            this.hubConnectionBuilder = hubConnectionBuilder;
            this.tokenResolver = tokenResolver;
            this.deviceInfoRepository = deviceInfoRepository;
            this.serviceScope = serviceScopeFactory.CreateScope();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            this.serviceScope.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Executes async.
        /// </summary>
        /// <param name="stoppingToken">
        /// The stopping token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.SynchronizeDeviceTimeAsync(stoppingToken);
            await this.ConnectToTemporeServerAsync(stoppingToken);
        }

        private async Task SynchronizeDeviceTimeAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Synchronizing devices time.");

            var hikvisionDeviceConnectionFactory = this.serviceScope.ServiceProvider.GetRequiredService<IHikvisionDeviceConnectionFactory>();

            var deviceInfos = this.deviceInfoRepository.Devices.ToList();
            var synchronizedDevices = 0;
            foreach (var deviceInfo in deviceInfos)
            {
                if (deviceInfo.SynchronizeTime)
                {
                    this.logger.LogInformation("Synchronizing device '{DeviceName}' time.", deviceInfo.Name);

                    var hikvisionDeviceConnection = hikvisionDeviceConnectionFactory.Create(
                        deviceInfo.Url,
                        deviceInfo.Username!,
                        deviceInfo.Password!);

                    var systemClient = hikvisionDeviceConnection.GetClient<ISystemClient>();

                    try
                    {
                        var time = new Time
                        {
                            LocalTime = DateTime.Now,
                            TimeZone = TimeZoneInfo.Local.ToCST(),
                        };

                        await systemClient.SetTimeAsync(time);

                        synchronizedDevices++;
                        this.logger.LogInformation("Synchronized device '{DeviceName}' time. Time set to {Time} - {TimeZone}.", deviceInfo.Name, time.LocalTime, time.TimeZone);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation(ex, "Error synchronizing device '{DeviceName}' time.", deviceInfo.Name);
                    }
                }
            }

            this.logger.LogInformation("Synchronized '{Count}/{Total}' devices.", synchronizedDevices, deviceInfos.Count);
        }

        private async Task ConnectToTemporeServerAsync(CancellationToken stoppingToken)
        {
            var serverUrl = this.configuration["Server"];

            this.logger.LogInformation("Connecting to Tempore Server '{Url}'", serverUrl);

            this.hubConnectionBuilder.WithUrl(
                    $"{serverUrl}/agentHub",
                    options => options.AccessTokenProvider = async () => await this.tokenResolver.ResolveAsync())
                .WithAutomaticReconnect();

            var hubConnection = this.hubConnectionBuilder.Build();

#pragma warning disable IDISP004 // Don't ignore created IDisposable
            // TODO: Review later the implication of release the registration result.
            hubConnection.Register<IAgentReceiver>(
                ActivatorUtilities.CreateInstance<AgentReceiver>(this.serviceScope.ServiceProvider));
#pragma warning restore IDISP004 // Don't ignore created IDisposable

            var waitAndRetryForever = Policy.Handle<Exception>().WaitAndRetryForeverAsync(
                _ => TimeSpan.FromSeconds(5),
                (exception, retryAttempt, _) => this.logger.LogError(
                    exception,
                    "Error connecting to Tempore Server. Will retry again in 5 seconds. Retry attempt {RetryAttempt}.",
                    retryAttempt));

            await waitAndRetryForever.ExecuteAsync(
                async (context) => { await hubConnection.StartAsync(stoppingToken); },
                stoppingToken);

            this.logger.LogInformation("Connected to Tempore Server '{Url}'", serverUrl);
        }
    }
}