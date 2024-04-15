// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Employees.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Employees
{
    using System;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Dialogs;
    using Tempore.App.Extensions;
    using Tempore.App.Pages;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// The link employees.
    /// </summary>
    [Route(Routes.Employees.EmployeesList)]
    public sealed partial class Employees
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Employees"/> class.
        /// </summary>
        public Employees()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<Employees>? StringLocalizer { get; set; }

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
        /// Gets or sets Employees table component service.
        /// </summary>
        public IMudTableComponentService<EmployeeDto>? EmployeesTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets Employees from devices table component service.
        /// </summary>
        public IMudTableComponentService<EmployeeFromDeviceDto>? EmployeesFromDevicesTableComponentService { get; set; }

        public string SearchTextEmployee { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets DialogService.
        /// </summary>
        [Inject]
        public IDialogService? DialogService { get; set; }

        /// <summary>
        /// Gets or sets NavigationManager.
        /// </summary>
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
        }

        private async Task<TableData<EmployeeDto>> GetEmployeesAsync(TableState tableState)
        {
            TableData<EmployeeDto> tableData = new TableData<EmployeeDto>
            {
                Items = Array.Empty<EmployeeDto>(),
            };

            try
            {
                var request = new GetEmployeesRequest
                {
                    SearchText = this.SearchTextEmployee,
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                };

                var paginationResponse = await this.EmployeeClient!.GetEmployeesAsync(request);
                tableData.Items = paginationResponse.Items;
                tableData.TotalItems = paginationResponse.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred loading employees"]);
            }

            return tableData;
        }

        private async Task ApplyFilterEmployee(MouseEventArgs arg)
        {
            await this.EmployeesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task ClearFilterEmployee(MouseEventArgs obj)
        {
            this.SearchTextEmployee = string.Empty;
            await this.EmployeesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task RefreshTables()
        {
            this.SearchTextEmployee = string.Empty;
            await this.EmployeesTableComponentService!.ReloadServerDataAsync();
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