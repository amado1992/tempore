// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeesFromDevices.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Employees
{
    using System;
    using System.Collections.Immutable;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Dialogs;
    using Tempore.App.Extensions;
    using Tempore.App.Models;
    using Tempore.App.Pages;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// The link employees.
    /// </summary>
    [Route(Routes.Employees.EmployeesFromDevices)]
    public sealed partial class EmployeesFromDevices
    {
        private readonly ImmutableList<State<bool>> states = new List<State<bool>>
                                                                            {
                                                                                new State<bool>(true, "Linked"),
                                                                                new State<bool>(false, "Unlinked"),
                                                                            }.ToImmutableList();

        private EmployeeFromDeviceDto? selectedEmployeeFromDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesFromDevices"/> class.
        /// </summary>
        public EmployeesFromDevices()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<EmployeesFromDevices>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the snackbar.
        /// </summary>
        [Inject]
        public ISnackbar? Snackbar { get; set; }

        /// <summary>
        /// Gets or sets logger.
        /// </summary>
        [Inject]
        public ILogger<LinkEmployees>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the IEmployeeClient EmployeeClient.
        /// </summary>
        [Inject]
        public IEmployeeClient? EmployeeClient { get; set; }

        /// <summary>
        /// Gets or sets DialogService.
        /// </summary>
        [Inject]
        public IDialogService? DialogService { get; set; }

        /// <summary>
        /// Gets or sets Employees from devices table component service.
        /// </summary>
        public IMudTableComponentService<EmployeeFromDeviceDto>? EmployeesFromDevicesTableComponentService { get; set; }

        private bool? IsLinked { get; set; }

        private string SearchTextEmployeeFromDevice { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets NavigationManager.
        /// </summary>
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
        }

        private async Task<TableData<EmployeeFromDeviceDto>> GetEmployeesFromDevicesAsync(TableState tableState)
        {
            TableData<EmployeeFromDeviceDto> tableData = new TableData<EmployeeFromDeviceDto>
            {
                Items = Array.Empty<EmployeeFromDeviceDto>(),
            };

            try
            {
                var request = new GetEmployeesFromDevicesRequest
                {
                    SearchText = this.SearchTextEmployeeFromDevice,
                    IsLinked = this.IsLinked,
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                };

                var paginationResponse = await this.EmployeeClient!.GetEmployeesFromDevicesAsync(request);
                tableData.Items = paginationResponse.Items;
                tableData.TotalItems = paginationResponse.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred loading employees from devices"]);
            }

            return tableData;
        }

        private async Task ApplyFilterEmployeeFromDevice(MouseEventArgs arg)
        {
            await this.EmployeesFromDevicesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task ClearFilterEmployeeFromDevice(MouseEventArgs obj)
        {
            this.SearchTextEmployeeFromDevice = string.Empty;
            this.IsLinked = null;
            await this.EmployeesFromDevicesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task RefreshTables()
        {
            this.SearchTextEmployeeFromDevice = string.Empty;
            this.IsLinked = null;

            await this.EmployeesFromDevicesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task<bool> ConfirmAsync(string employeeName)
        {
            var parameters = new DialogParameters<Confirmation>
                             {
                                 { x => x.ContentText, this.StringLocalizer!["Are you sure you want to unlink the employee '{0}'?", employeeName] },
                             };

            var dialogReference = await this.DialogService!.ShowAsync<Confirmation>(this.StringLocalizer!["Confirm"], parameters);
            return await dialogReference.GetReturnValueAsync<bool?>() ?? false;
        }

        private void EmployeeDetails(Guid? id)
        {
            this.NavigationManager?.NavigateTo(Routes.Employees.EmployeeDetails(id));
        }
    }
}