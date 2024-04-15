// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkEmployees.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Employees
{
    using System;
    using System.ComponentModel;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Extensions;
    using Tempore.App.Pages;
    using Tempore.App.Services;
    using Tempore.App.Services.EventArgs;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// The link employees.
    /// </summary>
    [Route(Routes.Employees.LinkEmployees)]
    public sealed partial class LinkEmployees
    {
        private HashSet<EmployeeFromDeviceDto>? selectedEmployeesFromDevices;

        private EmployeeDto? selectedEmployee;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkEmployees"/> class.
        /// </summary>
        public LinkEmployees()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the Divices.
        /// </summary>
        public ICollection<DeviceDto>? Devices { get; set; }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<LinkEmployees>? StringLocalizer { get; set; }

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
        /// Gets or sets the IDeviceClient DeviceClient.
        /// </summary>
        [Inject]
        public IDeviceClient? DeviceClient { get; set; }

        /// <summary>
        /// Gets or sets the dialog service.
        /// </summary>
        [Inject]
        public IDialogService? DialogService { get; set; }

        /// <summary>
        /// Gets or sets the notification center.
        /// </summary>
        [Inject]
        public INotificationCenter? NotificationCenter { get; set; }

        /// <summary>
        /// Gets or sets Employees table component service.
        /// </summary>
        public IMudTableComponentService<EmployeeDto>? EmployeesTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets Employees from devices table component service.
        /// </summary>
        public IMudTableComponentService<EmployeeFromDeviceDto>? EmployeesFromDevicesTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the filtered user must be linked or not.
        /// </summary>
        public bool? EmployeeFromDevicesLinkedStatus
        {
            get => this.GetPropertyValue<bool?>(nameof(this.EmployeeFromDevicesLinkedStatus));
            set => this.SetPropertyValue(nameof(this.EmployeeFromDevicesLinkedStatus), value);
        }

        /// <summary>
        /// Gets or sets the value id of the device to filter.
        /// </summary>
        public Guid? EmployeeFromDevicesDeviceId
        {
            get => this.GetPropertyValue<Guid?>(nameof(this.EmployeeFromDevicesDeviceId));
            set => this.SetPropertyValue(nameof(this.EmployeeFromDevicesDeviceId), value);
        }

        /// <summary>
        /// Gets the Selected device ids to filter employees from devices.
        /// </summary>
        public List<Guid>? EmployeeFromDevicesDeviceIds =>
            this.EmployeeFromDevicesDeviceId is not null
                ? new List<Guid> { this.EmployeeFromDevicesDeviceId.Value }
                : null;

        /// <summary>
        /// Gets or sets a the search text for employee from device.
        /// </summary>
        public string SearchTextEmployeeFromDevice
        {
            get => this.GetPropertyValue<string>(nameof(this.SearchTextEmployeeFromDevice));
            set => this.SetPropertyValue(nameof(this.SearchTextEmployeeFromDevice), value);
        }

        /// <summary>
        /// Gets or sets a the search text for employee.
        /// </summary>
        public string SearchTextEmployee
        {
            get => this.GetPropertyValue<string>(nameof(this.SearchTextEmployee));
            set => this.SetPropertyValue(nameof(this.SearchTextEmployee), value);
        }

        /// <summary>
        /// Gets or sets NavigationManager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the link action is disabled.
        /// </summary>
        public bool IsLinkEmployeesProcessRunning
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsLinkEmployeesProcessRunning));
            set => this.SetPropertyValue(nameof(this.IsLinkEmployeesProcessRunning), value);
        }

        /// <summary>
        /// Gets a value indicating whether the link action disabled.
        /// </summary>
        public bool IsLinkActionDisabled =>
            this.IsLinkEmployeesProcessRunning
            || this.selectedEmployee is null
            || this.selectedEmployeesFromDevices is null
            || this.selectedEmployeesFromDevices.Count == 0
            || this.selectedEmployeesFromDevices.Any(dto => dto.IsLinked);

        /// <summary>
        /// Gets a value indicating whether the unlink action disabled.
        /// </summary>
        public bool IsUnlinkActionDisabled =>
            this.IsLinkEmployeesProcessRunning
            || this.selectedEmployeesFromDevices is null
            || this.selectedEmployeesFromDevices.Count == 0
            || this.selectedEmployeesFromDevices.Any(dto => !dto.IsLinked);

        /// <summary>
        /// Gets or sets a value indicating whether filter employees from devices.
        /// </summary>
        public bool IsFilterEmployeesFromDevicesEnabled
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsFilterEmployeesFromDevicesEnabled));
            set => this.SetPropertyValue(nameof(this.IsFilterEmployeesFromDevicesEnabled), value);
        }

        /// <summary>
        /// Gets or sets  a value indicating whether filter employees.
        /// </summary>
        public bool IsFilterEmployeesEnabled
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsFilterEmployeesEnabled));
            set => this.SetPropertyValue(nameof(this.IsFilterEmployeesEnabled), value);
        }

        /// <summary>
        /// Gets or sets the count employees from devices.
        /// </summary>
        private int CountEmployeesFromDevices { get; set; }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.IsLinkEmployeesProcessRunning):
                case nameof(this.IsFilterEmployeesFromDevicesEnabled):
                case nameof(this.SearchTextEmployeeFromDevice):
                case nameof(this.EmployeeFromDevicesLinkedStatus):
                case nameof(this.EmployeeFromDevicesDeviceId):
                    this.InvokeAsync(this.ResetEmployeesFromDevicesAsync);
                    break;
                case nameof(this.IsFilterEmployeesEnabled):
                case nameof(this.SearchTextEmployee):
                    this.InvokeAsync(this.ResetEmployeeAsync);
                    break;
            }
        }

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            this.NotificationCenter!.Subscribe(NotificationTypes.QueueTaskStarted, this.OnNotificationReceived);
            this.NotificationCenter!.Subscribe(NotificationTypes.QueueTaskCompleted, this.OnNotificationReceived);

            try
            {
                this.Devices = await this.DeviceClient!.GetAllDevicesAsync();
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred loading devices"]);
            }

            await this.UpdateLinkEmployeesProcessStateAsync();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.NotificationCenter!.Unsubscribe(NotificationTypes.QueueTaskStarted, this.OnNotificationReceived);
                this.NotificationCenter!.Unsubscribe(NotificationTypes.QueueTaskCompleted, this.OnNotificationReceived);
                this.Snackbar?.Dispose();
            }
        }

        private void OnNotificationReceived(NotificationReceivedEventArgs obj)
        {
            this.InvokeAsync(async () => await this.UpdateLinkEmployeesProcessStateAsync());
        }

        private async Task UpdateLinkEmployeesProcessStateAsync()
        {
            try
            {
                this.IsLinkEmployeesProcessRunning = await this.EmployeeClient!.IsLinkEmployeesProcessRunningAsync();
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred determining if link employee process is running"]);
            }

            this.StateHasChanged();
        }

        private async Task<TableData<EmployeeFromDeviceDto>> GetEmployeesFromDevicesAsync(TableState tableState)
        {
            this.selectedEmployeesFromDevices?.Clear();
            TableData<EmployeeFromDeviceDto> tableData = new TableData<EmployeeFromDeviceDto>
            {
                Items = Array.Empty<EmployeeFromDeviceDto>(),
            };

            try
            {
                var request = new GetEmployeesFromDevicesRequest
                {
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                    IncludeDevice = true,
                    DeviceIds = this.EmployeeFromDevicesDeviceIds,
                };

                if (this.IsFilterEmployeesFromDevicesEnabled)
                {
                    request.SearchText = this.SearchTextEmployeeFromDevice;
                    request.IsLinked = this.EmployeeFromDevicesLinkedStatus;
                }

                var paginationResponse = await this.EmployeeClient!.GetEmployeesFromDevicesAsync(request);
                tableData.Items = paginationResponse.Items;
                tableData.TotalItems = paginationResponse.Count;

                this.CountEmployeesFromDevices = paginationResponse.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred loading employees from devices"]);
            }

            return tableData;
        }

        private async Task<TableData<EmployeeDto>> GetEmployeesAsync(TableState tableState)
        {
            this.selectedEmployee = null;

            TableData<EmployeeDto> tableData = new TableData<EmployeeDto>
            {
                Items = Array.Empty<EmployeeDto>(),
            };

            try
            {
                var request = new GetEmployeesRequest
                {
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                };

                if (this.IsFilterEmployeesEnabled)
                {
                    request.SearchText = this.SearchTextEmployee;
                }

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

        private async Task AutoLinkAsync()
        {
            await this.EmployeeClient!.LinkEmployeesAsync();
            await this.UpdateLinkEmployeesProcessStateAsync();
        }

        private async Task LinkAsync()
        {
            this.Snackbar!.Clear();

            try
            {
                await this.EmployeeClient!.LinkEmployeeAsync(EmployeeLinkRequest.Create(this.selectedEmployee!.Id, this.selectedEmployeesFromDevices!.Select(dto => dto.Id).ToArray()));

                using var snackbar = this.Snackbar!.Add(
                    this.StringLocalizer!["Link created successfully"],
                    Severity.Success);
            }
            catch (ApiException ex)
            {
                this.Logger?.LogError(ex, "An error occurred while linking employee");
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred while linking employee"], Severity.Error);
            }
            finally
            {
                await this.ResetAsync();
            }
        }

        private async Task ResetAsync()
        {
            await this.ResetEmployeesFromDevicesAsync();
            await this.ResetEmployeeAsync();
        }

        private async Task ResetEmployeesFromDevicesAsync()
        {
            this.selectedEmployeesFromDevices?.Clear();
            await this.EmployeesFromDevicesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task ResetEmployeeAsync()
        {
            this.selectedEmployee = null;
            await this.EmployeesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task UnlinkAsync()
        {
            this.Snackbar!.Clear();

            try
            {
                if (await this.DialogService!.ConfirmAsync(this.StringLocalizer!["Are you sure you want to unlink the selected employees?"]))
                {
                    await this.EmployeeClient!.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(this.selectedEmployeesFromDevices!.Select(dto => dto.Id).ToArray()));

                    using var snackbar = this.Snackbar!.Add(
                        this.StringLocalizer!["Unlink employees executed successfully"],
                        Severity.Success);
                }
            }
            catch (ApiException ex)
            {
                this.Logger?.LogError(ex, "An error occurred while un-linking employee");
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred while un-linking employees"], Severity.Error);
            }
            finally
            {
                await this.ResetEmployeesFromDevicesAsync();
            }
        }

        private async Task UnLinkAsync(EmployeeFromDeviceDto employeeFromDevice)
        {
            ArgumentNullException.ThrowIfNull(employeeFromDevice);

            this.Snackbar!.Clear();
            try
            {
                if (await this.DialogService!.ConfirmAsync(this.StringLocalizer!["Are you sure you want to unlink the employee '{0}'?", employeeFromDevice.FullName]))
                {
                    await this.EmployeeClient!.UnlinkEmployeeAsync(EmployeeUnlinkRequest.Create(employeeFromDevice.Id));
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
                await this.ResetEmployeesFromDevicesAsync();
            }
        }

        private async Task RefreshEmployeesFromDevicesAsync()
        {
            await this.EmployeesFromDevicesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task RefreshEmployeesAsync()
        {
            await this.EmployeesTableComponentService!.ReloadServerDataAsync();
        }

        private void EmployeeDetails(EmployeeFromDeviceDto employeeFromDeviceDto)
        {
            ArgumentNullException.ThrowIfNull(employeeFromDeviceDto);

            this.NavigationManager?.NavigateTo(Routes.Employees.EmployeeDetails(employeeFromDeviceDto.EmployeeId));
        }

        private void ViewEmployeeDetails(EmployeeDto employeeDto)
        {
            ArgumentNullException.ThrowIfNull(employeeDto);

            this.NavigationManager?.NavigateTo(Routes.Employees.EmployeeDetails(employeeDto.Id));
        }

        private string EmployeeTableRowClassFunc(EmployeeDto? employee, int index)
        {
            if (employee == this.selectedEmployee)
            {
                return "selected";
            }

            return string.Empty;
        }

        private string EmployeeFromDeviceTableRowClassFunc(EmployeeFromDeviceDto employeeFromDevice, int index)
        {
            if (this.selectedEmployeesFromDevices?.Contains(employeeFromDevice) ?? false)
            {
                return "selected";
            }

            return string.Empty;
        }

        private void OnEmployeeTableClick(TableRowClickEventArgs<EmployeeDto> args)
        {
            this.selectedEmployee = this.selectedEmployee != args.Item ? args.Item : null;
        }
    }
}