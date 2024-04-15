// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App
{
    using System.ComponentModel;

    using Blorc.OpenIdConnect;

    using Microsoft.AspNetCore.Components;

    using Tempore.App.Services.Interfaces;

    /// <summary>
    /// The app.
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// The timer.
        /// </summary>
        private Timer? timer;

        /// <summary>
        /// Gets or sets the user manager.
        /// </summary>
        [Inject]
        private IUserManager? UserManager { get; set; }

        /// <summary>
        /// Gets or sets the health check service.
        /// </summary>
        [Inject]
        private IHealthCheckService? HealthCheckService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the app is healthy.
        /// </summary>
        private bool IsHealthy
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsHealthy));
            set => this.SetPropertyValue(nameof(this.IsHealthy), value);
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            this.timer?.Dispose();
            this.timer = new Timer(
                state => this.InvokeAsync(this.HealthCheckAsync),
                null,
                0,
                1000);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.IsHealthy) && this.IsHealthy)
            {
                this.InvokeAsync(this.AuthenticationCheckAsync);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.timer?.Dispose();
            }
        }

        /// <summary>
        /// Runs app health check.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task HealthCheckAsync()
        {
            this.IsHealthy = await this.HealthCheckService!.IsHealthyAsync();
        }

        /// <summary>
        /// Runs authentication state check.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task AuthenticationCheckAsync()
        {
            var user = await this.UserManager!.GetUserAsync<User<Profile>>();
            if (user is null)
            {
                await this.UserManager.SignInRedirectAsync();
            }

            this.StateHasChanged();
        }
    }
}