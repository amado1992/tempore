// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CultureSelector.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Components
{
    using System.Collections.Immutable;
    using System.Globalization;

    using Blazored.LocalStorage;

    using Microsoft.AspNetCore.Components;

    using Microsoft.Extensions.Localization;

    using Tempore.App.Components.Models;

    /// <summary>
    /// The culture selector.
    /// </summary>
    public partial class CultureSelector
    {
        /// <summary>
        /// The supported cultures.
        /// </summary>
        private static readonly ImmutableList<CustomCultureInfo> SupportedCultures = new List<CustomCultureInfo>
            {
                new CustomCultureInfo("en-US", "English"),
                new CustomCultureInfo("es-ES", "Español"),
            }.ToImmutableList();

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<CultureSelector>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the local storage.
        /// </summary>
        [Inject]
        public ILocalStorageService? LocalStorage { get; set; }

        /// <summary>
        /// Select culture.
        /// </summary>
        /// <param name="culture">
        /// The culture.
        /// </param>
        private async Task SetCultureAsync(CustomCultureInfo culture)
        {
            if (CultureInfo.CurrentCulture.Name != culture.Name)
            {
                await this.LocalStorage!.SetItemAsync("culture", culture.Name);
                this.NavigationManager!.NavigateTo(this.NavigationManager.Uri, true);
            }
        }
    }
}