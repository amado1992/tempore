// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInactivity.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Dialogs
{
    using System;

    using Blorc.OpenIdConnect;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Services.Interfaces;

    /// <summary>
    /// The user inactivity dialog.
    /// </summary>
    public partial class UserInactivity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInactivity"/> class.
        /// </summary>
        public UserInactivity()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the sign-out timeSpan.
        /// </summary>
        [Parameter]
        public TimeSpan? SignOutTimeSpan { get; set; }

        /// <summary>
        /// Gets or sets the remaining time span for sing-out.
        /// </summary>
        public TimeSpan? RemainingTimeSpan { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [Inject]
        public ILogger<UserInactivity>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<UserInactivity>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the user manager.
        /// </summary>
        [Inject]
        public IUserManager? UserManager { get; set; }

        /// <summary>
        /// Gets or sets the mud dialog instance component service.
        /// </summary>
        public IMudDialogInstanceComponentService? MudMudDialogInstanceComponentService { get; set; }

        /// <summary>
        /// Gets the sign-out progress color.
        /// </summary>
        public Color SignOutProgressColor
        {
            get
            {
                if (this.SignOutTimeSpan is null)
                {
                    return Color.Dark;
                }

                if (this.SignOutTimeSpan.Value <= TimeSpan.FromSeconds(10))
                {
                    return Color.Error;
                }

                if (this.SignOutTimeSpan.Value <= TimeSpan.FromSeconds(15))
                {
                    return Color.Warning;
                }

                return Color.Success;
            }
        }

        /// <summary>
        /// Gets the sign-out progress value.
        /// </summary>
        public double SignOutProgressValue
        {
            get
            {
                var remainingTimeSpan = this.RemainingTimeSpan;
                if (remainingTimeSpan is null)
                {
                    return 0;
                }

                var signOutTimeSpan = this.SignOutTimeSpan;
                if (signOutTimeSpan is null)
                {
                    return 0;
                }

                return remainingTimeSpan.Value.TotalMilliseconds / signOutTimeSpan.Value.TotalMilliseconds * 100;
            }
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            this.RemainingTimeSpan = this.SignOutTimeSpan;
            this.UserManager!.UserInactivity += this.OnUserManagerUserInactivity;
            this.UserManager.UserActivity += this.OnUserManagerUserActivity;
        }

        private void OnUserManagerUserActivity(object? sender, UserActivityEventArgs e)
        {
            this.UserManager!.UserInactivity -= this.OnUserManagerUserInactivity;
            this.UserManager.UserActivity -= this.OnUserManagerUserActivity;

            this.MudMudDialogInstanceComponentService!.Close(true);
        }

        private void OnUserManagerUserInactivity(object? sender, UserInactivityEventArgs e)
        {
            var signOutTimeSpan = e.SignOutTimeSpan;
            if (signOutTimeSpan <= TimeSpan.FromMinutes(5))
            {
                this.RemainingTimeSpan = signOutTimeSpan;
                e.InactivityNotificationTimeSpan = TimeSpan.FromSeconds(signOutTimeSpan <= TimeSpan.FromMinutes(1) ? 1 : 30);

                this.StateHasChanged();
            }
        }
    }
}