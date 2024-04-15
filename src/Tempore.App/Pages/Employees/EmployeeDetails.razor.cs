// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeDetails.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Employees
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Dialogs;
    using Tempore.App.Extensions;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// DetailsEmploye.
    /// </summary>
    [Route(Routes.Employees.EmployeeDetailsTemplate)]
    public partial class EmployeeDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeDetails"/> class.
        /// </summary>
        public EmployeeDetails()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<EmployeeDetails>? StringLocalizer { get; set; }

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

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Parameter]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets EmployeeData.
        /// </summary>
        private EmployeeDto? EmployeeData { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// OnInitializedAsync.
        /// </summary>
        /// <returns> Task. </returns>
        protected override async Task OnInitializedAsync()
        {
            if (this.Id is not null)
            {
                await this.GetEmployeeByIdAsync();
            }
        }

        private async Task GetEmployeeByIdAsync()
        {
            try
            {
                this.EmployeeData = await this.EmployeeClient!.GetEmployeeByIdAsync(this.Id);
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred loading employee"]);
            }
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
                    SearchText = string.Empty,
                    IsLinked = null,
                    IncludeDevice = true,
                    IncludeAgent = true,
                    EmployeeId = this.Id,
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

        private async Task UnLinkAsync(Guid id, string name)
        {
            this.Snackbar!.Clear();
            try
            {
                if (await this.ConfirmAsync(name))
                {
                    await this.EmployeeClient!.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(id));
                    using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Unlink created successfully"], Severity.Success);
                }
            }
            catch (ApiException ex)
            {
                this.Logger?.LogError(ex, "An error occurred while un-linking employee");

                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred un-linking the employee"], Severity.Error);
            }
            finally
            {
                await this.EmployeesFromDevicesTableComponentService!.ReloadServerDataAsync();
            }
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
    }
}