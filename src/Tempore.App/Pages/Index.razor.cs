// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Index.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// The index.
    /// </summary>
    [Route(Routes.Home.Index)]
    public partial class Index
    {
        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<Index>? StringLocalizer { get; set; }

        /// <summary>
        /// Device management.
        /// </summary>
        public void NavigateToDevices()
        {
            this.NavigationManager?.NavigateTo(Routes.Employees.ImportFromDevices.Agents);
        }

        /// <summary>
        /// Navigates to time and attendance.
        /// </summary>
        public void NavigateToFileProcessing()
        {
            this.NavigationManager?.NavigateTo(Routes.Employees.ImportFromFiles.ImportFromFile);
        }

        /// <summary>
        /// Navigates to employees page.
        /// </summary>
        public void NavigateToEmployees()
        {
            this.NavigationManager?.NavigateTo(Routes.Employees.ImportFromFile);
        }

        /// <summary>
        /// Navigates to ....
        /// </summary>
        public void NavigateToTimeAttendance()
        {
            this.NavigationManager?.NavigateTo(Routes.TimeAndAttendance.Shifts);
        }

        /// <summary>
        /// Navigates to analytics.
        /// </summary>
        public void NavigateToAnalytics()
        {
            this.NavigationManager?.NavigateTo(Routes.Analytics.WorkforceMetrics);
        }
    }
}