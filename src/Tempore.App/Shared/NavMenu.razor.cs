// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavMenu.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Shared
{
    using System.ComponentModel;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Routing;
    using Microsoft.Extensions.Localization;

    using Tempore.App.Pages;

    /// <summary>
    /// Nav Menu.
    /// </summary>
    public partial class NavMenu
    {
        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<NavMenu>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the employees group is expanded.
        /// </summary>
        public bool IsEmployeesGroupExpanded
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsEmployeesGroupExpanded));
            set => this.SetPropertyValue(nameof(this.IsEmployeesGroupExpanded), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the time & attendance group is expanded.
        /// </summary>
        public bool IsTimeAndAttendanceGroupExpanded
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsTimeAndAttendanceGroupExpanded));
            set => this.SetPropertyValue(nameof(this.IsTimeAndAttendanceGroupExpanded), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the analytics group is expanded.
        /// </summary>
        public bool IsAnalyticsGroupExpanded
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsAnalyticsGroupExpanded));
            set => this.SetPropertyValue(nameof(this.IsAnalyticsGroupExpanded), value);
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            this.NavigationManager!.LocationChanged += this.OnNavigationManagerLocationChanged;
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.StateHasChanged();
        }

        private void OnNavigationManagerLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            // TODO: Review this behavior later.
            var path = new Uri(e.Location).LocalPath;
            this.IsEmployeesGroupExpanded = path.StartsWith(Routes.Employees.Root);
            this.IsTimeAndAttendanceGroupExpanded = path.StartsWith(Routes.TimeAndAttendance.Root);
            this.IsAnalyticsGroupExpanded = path.StartsWith(Routes.Analytics.Root);
        }
    }
}