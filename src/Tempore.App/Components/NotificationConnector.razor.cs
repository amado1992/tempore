// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationConnector.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Components
{
    using System;

    using Blorc.OpenIdConnect;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.AspNetCore.SignalR.Client;

    using MudBlazor;

    using Polly;

    using Tempore.App.Services.EventArgs;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client.Services.Interfaces;

    using TypedSignalR.Client;

    using Severity = Tempore.Client.Services.Interfaces.Severity;

    /// <summary>
    /// The notifications component.
    /// </summary>
    public sealed partial class NotificationConnector : IDisposable
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [Inject]
        public ILogger<NotificationConnector>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HubConnectionBuilder"/>.
        /// </summary>
        [Inject]
        public IHubConnectionBuilder? HubConnectionBuilder { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UserManager"/>.
        /// </summary>
        [Inject]
        public IUserManager? UserManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INotificationReceiver"/>.
        /// </summary>
        [Inject]
        public INotificationReceiver? NotificationReceiver { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INotificationCenter"/>.
        /// </summary>
        [Inject]
        public INotificationCenter? NotificationCenter { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IWebAssemblyHostEnvironment"/>.
        /// </summary>
        [Inject]
        private IWebAssemblyHostEnvironment? HostEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISnackbar"/>.
        /// </summary>
        [Inject]
        private ISnackbar? Snackbar { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            this.UserManager?.Dispose();
            this.Snackbar?.Dispose();
            this.NotificationCenter!.NotificationReceived -= this.OnNotificationReceived;
        }

        /// <inheritdoc />
        protected override Task OnInitializedAsync()
        {
            this.NotificationCenter!.NotificationReceived += this.OnNotificationReceived;

            // TODO: Improve this to avoid allocations?
            var notificationHubUrl = new Uri(new Uri(this.HostEnvironment!.BaseAddress), "notificationHub");
            this.HubConnectionBuilder!.WithUrl(
                notificationHubUrl,
                options => options.AccessTokenProvider = async () =>
                {
                    var user = await this.UserManager!.GetUserAsync();
                    return user?.AccessToken;
                }).WithAutomaticReconnect();

            var hubConnection = this.HubConnectionBuilder!.Build();

#pragma warning disable IDISP004 // Don't ignore created IDisposable
            // TODO: Review later the implication of release the registration result.
            hubConnection.Register(this.NotificationReceiver);
#pragma warning restore IDISP004 // Don't ignore created IDisposable

            var waitAndRetryForever = Policy.Handle<Exception>().WaitAndRetryForeverAsync(
                _ => TimeSpan.FromSeconds(5),
                (exception, retryAttempt, _) =>
                {
                    this.Logger!.LogError(exception, "Error connecting to the notifications hub");
                });

            Task.Run(
                async () =>
                {
                    await waitAndRetryForever.ExecuteAsync(
                        async cancellationToken =>
                        {
                            await hubConnection.StartAsync(cancellationToken);

                            this.Logger!.LogInformation("Connected to the notifications hub");
                        },
                        CancellationToken.None);
                });

            return Task.CompletedTask;
        }

        private void OnNotificationReceived(object? sender, NotificationReceivedEventArgs args)
        {
            switch (args.Severity)
            {
                case Severity.Normal:
                    using (var snackbar = this.Snackbar!.Add(args.Message))
                    {
                        break;
                    }

                case Severity.Information:
                    using (var snackbar = this.Snackbar!.Add(args.Message, MudBlazor.Severity.Info))
                    {
                        break;
                    }

                case Severity.Success:
                    using (var snackbar = this.Snackbar!.Add(args.Message, MudBlazor.Severity.Success))
                    {
                        break;
                    }

                case Severity.Warning:
                    using (var snackbar = this.Snackbar!.Add(args.Message, MudBlazor.Severity.Warning))
                    {
                        break;
                    }

                case Severity.Error:
                    using (var snackbar = this.Snackbar!.Add(args.Message, MudBlazor.Severity.Error))
                    {
                        break;
                    }
            }
        }
    }
}