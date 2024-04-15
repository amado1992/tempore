// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainLayout.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Shared
{
    using Blorc.OpenIdConnect;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Dialogs;
    using Tempore.App.Themes;

    /// <summary>
    /// The main layout.
    /// </summary>
    public partial class MainLayout
    {
        /// <summary>
        /// The current theme.
        /// </summary>
        private readonly MudTheme currentTheme = new LightTheme();

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<MainLayout>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the user manager.
        /// </summary>
        [Inject]
        public IUserManager? UserManager { get; set; }

        /// <summary>
        /// Gets a value indicating whether is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is drawer open.
        /// </summary>
        public bool IsDrawerOpen { get; private set; } = true;

        [Inject]
        public IDialogService DialogService { get; set; }

        /// <summary>
        /// The drawer toggle.
        /// </summary>
        public void DrawerToggle()
        {
            this.IsDrawerOpen = !this.IsDrawerOpen;
        }

        /// <summary>
        /// The on initialized async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task OnInitializedAsync()
        {
            this.IsAuthenticated = await this.UserManager!.IsAuthenticatedAsync();
            this.UserManager.UserInactivity += this.OnUserManagerUserInactivity;
        }

        private void OnUserManagerUserInactivity(object? sender, UserInactivityEventArgs e)
        {
            this.UserManager!.UserInactivity -= this.OnUserManagerUserInactivity;
            this.InvokeAsync(
                async () =>
                {
                    if (e.SignOutTimeSpan <= TimeSpan.FromMinutes(5))
                    {
                        e.InactivityNotificationTimeSpan = TimeSpan.FromSeconds(e.SignOutTimeSpan <= TimeSpan.FromMinutes(1) ? 1 : 30);
                    }

                    var dialogParameters = new DialogParameters
                                           {
                                               { nameof(e.SignOutTimeSpan), e.SignOutTimeSpan },
                                           };

                    var dialogReference = await this.DialogService.ShowAsync<UserInactivity>(string.Empty, dialogParameters);
                    await dialogReference.GetReturnValueAsync<bool>();

                    this.UserManager!.UserInactivity += this.OnUserManagerUserInactivity;
                });
        }
    }
}