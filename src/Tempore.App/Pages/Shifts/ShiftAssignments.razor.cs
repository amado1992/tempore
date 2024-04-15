// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShiftAssignments.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Shifts
{
    using System.ComponentModel;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Dialogs.Shifts;
    using Tempore.App.Extensions;
    using Tempore.App.Services;
    using Tempore.App.Services.EventArgs;
    using Tempore.App.Services.Interfaces;
    using Tempore.App.ViewModels.Dialogs.Shifts;
    using Tempore.Client;

    /// <summary>
    /// DetailsEmploye.
    /// </summary>
    [Route(Routes.TimeAndAttendance.ShiftAssignmentTemplate)]
    public partial class ShiftAssignments
    {
        private const double DefaultEffectiveWorkingHours = 97.77d;

        private const int DefaultShiftAssignmentsDays = 15;

        /// <summary>
        /// The selected employee from shift assignment.
        /// </summary>
        private HashSet<ScheduledShiftEmployeeDto>? selectedScheduledShiftEmployees;

        /// <summary>
        /// The selected scheduled shift.
        /// </summary>
        private ScheduledShiftOverviewDto? selectedScheduledShift;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftAssignments"/> class.
        /// </summary>
        public ShiftAssignments()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<ShiftAssignments>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the snackbar.
        /// </summary>
        [Inject]
        public ISnackbar? Snackbar { get; set; }

        /// <summary>
        /// Gets or sets shift data.
        /// </summary>
        public ShiftDto? ShiftData { get; set; }

        /// <summary>
        /// Gets or sets logger.
        /// </summary>
        [Inject]
        public ILogger<ShiftAssignments>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the IEmployeeClient EmployeeClient.
        /// </summary>
        [Inject]
        public IEmployeeClient? EmployeeClient { get; set; }

        /// <summary>
        /// Gets or sets the IScheduledDayClient ScheduledDayClient.
        /// </summary>
        [Inject]
        public IScheduledDayClient? ScheduledDayClient { get; set; }

        /// <summary>
        /// Gets or sets DialogService.
        /// </summary>
        [Inject]
        public IDialogService? DialogService { get; set; }

        /// <summary>
        /// Gets or sets the IShiftClient ShiftClient.
        /// </summary>
        [Inject]
        public IShiftClient? ShiftClient { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the notification center.
        /// </summary>
        [Inject]
        public INotificationCenter? NotificationCenter { get; set; }

        /// <summary>
        /// Gets or sets employees table component service.
        /// </summary>
        public IMudTableComponentService<ScheduledShiftEmployeeDto>? ScheduledShiftEmployeeTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets employees table component service.
        /// </summary>
        public IMudTableComponentService<ScheduledShiftOverviewDto>? ScheduledShiftOverviewTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets employees table component service.
        /// </summary>
        public IMudTableComponentService<DayDto>? DayTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Parameter]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether the assign action disabled.
        /// </summary>
        public bool IsAssignActionDisabled =>
            this.selectedScheduledShiftEmployees?.Count == 0
            || this.selectedScheduledShiftEmployees is null
            || this.AssignActionDisabled()
            || (this.selectedScheduledShift is not null && this.selectedScheduledShiftEmployees?.Count > 0 && this.AssignActionDisabled());

        /// <summary>
        /// Gets or sets a value indicating whether filter employee from shift assignmentEnabled.
        /// </summary>
        public bool IsFilterEmployeeFromShiftAssignmentEnabled
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsFilterEmployeeFromShiftAssignmentEnabled));
            set => this.SetPropertyValue(nameof(this.IsFilterEmployeeFromShiftAssignmentEnabled), value);
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
        /// Gets or sets a value indicating whether filter shift assignment.
        /// </summary>
        public bool IsScheduledShiftFilterEnabled
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsScheduledShiftFilterEnabled));
            set => this.SetPropertyValue(nameof(this.IsScheduledShiftFilterEnabled), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the filtered user must be assigned or not.
        /// </summary>
        public bool? ScheduledShiftEmployeeAssignmentStatus
        {
            get => this.GetPropertyValue<bool?>(nameof(this.ScheduledShiftEmployeeAssignmentStatus));
            set => this.SetPropertyValue(nameof(this.ScheduledShiftEmployeeAssignmentStatus), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the schedule days action is disabled.
        /// </summary>
        public bool IsScheduleDaysProcessRunning
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsScheduleDaysProcessRunning));
            set => this.SetPropertyValue(nameof(this.IsScheduleDaysProcessRunning), value);
        }

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        private ICollection<DayDto>? Days { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field to read only the start date.
        /// </summary>
        private DateTime? StartDate
        {
            get => this.GetPropertyValue<DateTime?>(nameof(this.StartDate));
            set => this.SetPropertyValue(nameof(this.StartDate), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field to read only the expire date.
        /// </summary>
        private DateTime? ExpireDate
        {
            get => this.GetPropertyValue<DateTime?>(nameof(this.ExpireDate));
            set => this.SetPropertyValue(nameof(this.ExpireDate), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field to read only the effective working hours.
        /// </summary>
        private double? EffectiveWorkingHours
        {
            get => this.GetPropertyValue<double?>(nameof(this.EffectiveWorkingHours));
            set => this.SetPropertyValue(nameof(this.EffectiveWorkingHours), value);
        }

        /// <summary>
        /// OnInitializedAsync.
        /// </summary>
        /// <returns> Task. </returns>
        protected override async Task OnInitializedAsync()
        {
            this.NotificationCenter!.Subscribe(NotificationTypes.QueueTaskStarted, this.OnNotificationReceived);
            this.NotificationCenter!.Subscribe(NotificationTypes.QueueTaskCompleted, this.OnNotificationReceived);

            await this.UpdateScheduleDaysProcessRunningStateAsync();
            await this.GetShiftByIdAsync();
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

        /// <inheritdoc/>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.IsFilterEmployeeFromShiftAssignmentEnabled):
                case nameof(this.SearchTextEmployee):
                case nameof(this.ScheduledShiftEmployeeAssignmentStatus):
                    this.InvokeAsync(this.ResetEmployeesFromShiftAssignmentAsync);
                    break;
                case nameof(this.IsScheduledShiftFilterEnabled):
                    this.InvokeAsync(this.ResetScheduledShiftsAsync);
                    break;
                case nameof(this.StartDate):
                case nameof(this.ExpireDate):
                case nameof(this.EffectiveWorkingHours):
                    this.InvokeAsync(this.RefreshShiftAssignmentAsync);
                    break;
            }
        }

        private void OnNotificationReceived(NotificationReceivedEventArgs args)
        {
            this.InvokeAsync(async () => await this.UpdateScheduleDaysProcessRunningStateAsync());
        }

        private async Task UpdateScheduleDaysProcessRunningStateAsync()
        {
            try
            {
                this.IsScheduleDaysProcessRunning = await this.ScheduledDayClient!.IsScheduleDaysProcessRunningAsync();
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred determining whether the daily planning process is running"]);
            }

            this.StateHasChanged();
        }

        private async Task<TableData<ScheduledShiftOverviewDto>> GetScheduledShiftAsync(TableState tableState)
        {
            TableData<ScheduledShiftOverviewDto> tableData = new TableData<ScheduledShiftOverviewDto>
            {
                Items = Array.Empty<ScheduledShiftOverviewDto>(),
            };

            try
            {
                var request = new GetScheduledShiftByShiftIdRequest
                {
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                    ShiftId = this.Id,
                };

                if (this.selectedScheduledShift is not null)
                {
                    request.SearchParams = new ScheduledShiftSearchParams
                    {
                        StartDate = this.StartDate,
                        ExpireDate = this.ExpireDate,
                        EffectiveWorkingTime = this.selectedScheduledShift.EffectiveWorkingTime,
                    };
                }

                var response = await this.ShiftClient!.GetScheduledShiftsByShiftIdAsync(request);
                tableData.Items = response.Items;
                tableData.TotalItems = response.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred loading shift assignments"]);
            }

            return tableData;
        }

        private async Task<TableData<ScheduledShiftEmployeeDto>> GetEmployeesFromShiftAssignmentsAsync(TableState tableState)
        {
            TableData<ScheduledShiftEmployeeDto> tableData = new TableData<ScheduledShiftEmployeeDto>
            {
                Items = Array.Empty<ScheduledShiftEmployeeDto>(),
            };

            try
            {
                var request = new GetEmployeesFromScheduledShiftRequest
                {
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                    ScheduledShiftId = this.selectedScheduledShift?.Id,
                };

                if (this.IsFilterEmployeeFromShiftAssignmentEnabled)
                {
                    request.SearchParams = new EmployeesFromScheduledShiftSearchParams
                    {
                        SearchText = this.SearchTextEmployee,
                        IsAssigned = this.ScheduledShiftEmployeeAssignmentStatus,
                    };
                }

                var paginationResponse = await this.ShiftClient!.GetEmployeesFromScheduledShiftAsync(request);
                tableData.Items = paginationResponse.Items;
                tableData.TotalItems = paginationResponse.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred loading employees from shift assignments"]);
            }

            return tableData;
        }

        private async Task AssignEmployeesToShift()
        {
            this.Snackbar!.Clear();

            // TODO: Extract default values as configuration variables.
            var assignShiftToEmployeesConfirmationViewModel = new AssignEmployeesToScheduledShiftConfirmationViewModel
            {
                ShiftId = this.Id,
                EmployeeIds = this.selectedScheduledShiftEmployees!.Select(dto => dto.Id).ToArray(),
                StartDate = this.selectedScheduledShift?.StartDate is null ? DateTime.Now : this.selectedScheduledShift!.StartDate.DateTime,
                ExpireDate = this.selectedScheduledShift?.ExpireDate is null ? DateTime.Now.AddDays(DefaultShiftAssignmentsDays) : this.selectedScheduledShift!.ExpireDate.DateTime,
                EffectiveWorkingHours = this.selectedScheduledShift?.EffectiveWorkingTime is null ? DefaultEffectiveWorkingHours : this.selectedScheduledShift!.EffectiveWorkingTime.TotalHours,
            };

            var parameters = new DialogParameters<AssignEmployeesToScheduledShiftConfirmation>
                             {
                                 { dialog => dialog.ViewModel, assignShiftToEmployeesConfirmationViewModel },
                                 { dialog => dialog.ReadOnly, this.selectedScheduledShift is not null },
                             };

            var request = await this.DialogService!.ShowAsync<AssignEmployeesToScheduledShiftConfirmation, AssignEmployeesToScheduledShiftRequest?>(parameters);
            if (request is not null)
            {
                this.Snackbar!.Clear();
                try
                {
                    var scheduledShiftId = await this.EmployeeClient!.AssignEmployeesToScheduledShiftAsync(request);
                    using var snackbar = this.Snackbar!.Add(
                        this.StringLocalizer!["Successful assignment of employee(s) to the shift"],
                        Severity.Success);

                    // TODO: Improve this later by adding GetScheduledShiftById
                    var scheduledShiftByShiftIdRequest = new GetScheduledShiftByShiftIdRequest
                    {
                        ShiftId = request.ShiftId,
                        Skip = 0,
                        Take = 1,
                        SearchParams = new ScheduledShiftSearchParams
                        {
                            Id = scheduledShiftId,
                        },
                    };

                    var response = await this.ShiftClient!.GetScheduledShiftsByShiftIdAsync(scheduledShiftByShiftIdRequest);
                    this.selectedScheduledShift = response.Items.FirstOrDefault();
                    if (this.selectedScheduledShift is not null)
                    {
                        this.StartDate = this.selectedScheduledShift?.StartDate.DateTime;
                        this.ExpireDate = this.selectedScheduledShift?.ExpireDate.DateTime;
                        this.EffectiveWorkingHours = this.selectedScheduledShift?.EffectiveWorkingTime.TotalHours;
                        this.IsScheduledShiftFilterEnabled = true;
                    }
                }
                catch (ApiException ex)
                {
                    this.Logger?.LogError(ex, "An error occurred while assigning a shift to an employee");
                    using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred while assigning a shift to an employee"], Severity.Error);
                }
            }

            await this.RefreshEmployeeFromShiftAssignmentAsync();

            if (this.selectedScheduledShift is null)
            {
                await this.RefreshShiftAssignmentAsync();
            }
        }

        private async Task RefreshEmployeeFromShiftAssignmentAsync()
        {
            await this.ScheduledShiftEmployeeTableComponentService!.ReloadServerDataAsync();
        }

        private async Task RefreshShiftAssignmentAsync()
        {
            await this.ScheduledShiftOverviewTableComponentService!.ReloadServerDataAsync();
        }

        private async Task ResetEmployeesFromShiftAssignmentAsync()
        {
            this.selectedScheduledShiftEmployees?.Clear();
            await this.ScheduledShiftEmployeeTableComponentService!.ReloadServerDataAsync();
        }

        private async Task ResetScheduledShiftsAsync()
        {
            if (!this.IsScheduledShiftFilterEnabled)
            {
                this.selectedScheduledShift = null;

                this.StartDate = null;
                this.ExpireDate = null;
                this.EffectiveWorkingHours = null;

                await this.ScheduledShiftOverviewTableComponentService!.ReloadServerDataAsync();
                await this.ScheduledShiftEmployeeTableComponentService!.ReloadServerDataAsync();
            }
        }

        private string ShiftAssignmentTableRowClassFunc(ScheduledShiftOverviewDto? scheduledShiftExtendedDto, int index)
        {
            if (scheduledShiftExtendedDto is { Id: var id } && id == this.selectedScheduledShift?.Id)
            {
                return "selected";
            }

            return string.Empty;
        }

        private string EmployeeFromShiftAssignmentTableRowClassFunc(ScheduledShiftEmployeeDto scheduledShiftEmployee, int index)
        {
            if (this.selectedScheduledShiftEmployees?.Contains(scheduledShiftEmployee) ?? false)
            {
                return "selected";
            }

            return string.Empty;
        }

        private async Task GetShiftByIdAsync()
        {
            try
            {
                this.ShiftData = await this.ShiftClient!.GetShiftByIdAsync(this.Id);
                this.Days = this.ShiftData.Days;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred loading shift"]);
            }
        }

        private bool AssignActionDisabled()
        {
            return this.selectedScheduledShiftEmployees!.Any(employee => employee.IsAssigned);
        }

        private async Task OnShiftAssignmentTableClick(TableRowClickEventArgs<ScheduledShiftOverviewDto> args)
        {
            this.selectedScheduledShift = this.selectedScheduledShift != args.Item ? args.Item : null;
            await this.RefreshEmployeeFromShiftAssignmentAsync();
        }

        private Task NavigateToComputeMetrics(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            this.NavigationManager!.NavigateTo(Routes.Analytics.ComputeWorkforceMetrics(startDate.Date, endDate.Date));

            return Task.CompletedTask;
        }

        private async Task ScheduleDaysAsync(ScheduledShiftOverviewDto scheduledShiftOverviewDto, bool force)
        {
            this.Snackbar!.Clear();

            try
            {
                var request = new ScheduleDaysRequest
                {
                    ScheduledShiftId = scheduledShiftOverviewDto.Id,
                    Force = force,
                };

                await this.ScheduledDayClient!.ScheduleDaysAsync(request);

                await this.UpdateScheduleDaysProcessRunningStateAsync();
            }
            catch (ApiException ex)
            {
                this.Logger?.LogError(ex, "An error occurred while creating the day's schedule");
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred while creating the day's schedule"], Severity.Error);
            }
        }
    }
}