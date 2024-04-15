// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMenu.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Components
{
    using Blorc.OpenIdConnect;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Localization;

    using Tempore.App.Shared;

    public partial class UserMenu
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [Inject]
        public ILogger<NotificationConnector>? Logger { get; set; }

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
        /// Gets the user.
        /// </summary>
        public User<Profile>? User { get; private set; }

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            this.User = await this.UserManager!.GetUserAsync<User<Profile>>();
        }

        private async Task SignOutAsync(MouseEventArgs arg)
        {
            await this.UserManager!.SignOutRedirectAsync();
        }
    }
}