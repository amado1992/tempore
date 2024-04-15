// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shifts.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Shifts
{
    using System;
    using System.ComponentModel;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Extensions;
    using Tempore.App.Pages;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// The shifts.
    /// </summary>
    [Route(Routes.TimeAndAttendance.Shifts)]
    public sealed partial class Shifts
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shifts"/> class.
        /// </summary>
        public Shifts()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<Shifts>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the snackbar.
        /// </summary>
        [Inject]
        public ISnackbar? Snackbar { get; set; }

        /// <summary>
        /// Gets or sets logger.
        /// </summary>
        [Inject]
        public ILogger<Shifts>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the IShiftClient ShiftClient.
        /// </summary>
        [Inject]
        public IShiftClient? ShiftClient { get; set; }

        /// <summary>
        /// Gets or sets DialogService.
        /// </summary>
        [Inject]
        public IDialogService? DialogService { get; set; }

        /// <summary>
        /// Gets or sets shift table component service.
        /// </summary>
        public IMudTableComponentService<ShiftDto>? ShiftsTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets a the search text for shift.
        /// </summary>
        public string SearchText
        {
            get => this.GetPropertyValue<string>(nameof(this.SearchText));
            set => this.SetPropertyValue(nameof(this.SearchText), value);
        }

        /// <summary>
        /// Gets or sets NavigationManager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether filter employees from devices.
        /// </summary>
        public bool IsFilterEnabled
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsFilterEnabled));
            set => this.SetPropertyValue(nameof(this.IsFilterEnabled), value);
        }

        /// <summary>
        /// Gets or sets a the date.
        /// </summary>
        private DateTime? Date
        {
            get => this.GetPropertyValue<DateTime?>(nameof(this.Date));
            set => this.SetPropertyValue(nameof(this.Date), value);
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.IsFilterEnabled):
                case nameof(this.SearchText):
                case nameof(this.Date):
                    this.InvokeAsync(this.ResetShiftsAsync);
                    break;
            }
        }

        private async Task<TableData<ShiftDto>> GetShiftsAsync(TableState tableState)
        {
            TableData<ShiftDto> tableData = new TableData<ShiftDto>
            {
                Items = Array.Empty<ShiftDto>(),
            };

            try
            {
                var request = new GetShiftRequest
                {
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                };

                if (this.IsFilterEnabled)
                {
                    request.SearchText = this.SearchText;
                    request.StartDate = this.Date;
                }

                var paginationResponse = await this.ShiftClient!.GetShiftsAsync(request);
                tableData.Items = paginationResponse.Items;
                tableData.TotalItems = paginationResponse.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred loading shifts"]);
            }

            return tableData;
        }

        private async Task RefreshShiftsAsync()
        {
            await this.ShiftsTableComponentService!.ReloadServerDataAsync();
        }

        private void ShiftAssignment(ShiftDto shiftDto)
        {
            ArgumentNullException.ThrowIfNull(shiftDto);

            this.NavigationManager?.NavigateTo(Routes.TimeAndAttendance.ShiftAssignment(shiftDto.Id));
        }

        private async Task ResetShiftsAsync()
        {
            await this.ShiftsTableComponentService!.ReloadServerDataAsync();
        }
    }
}