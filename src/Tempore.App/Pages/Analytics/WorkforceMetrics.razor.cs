// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetrics.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Analytics
{
    using System.ComponentModel;
    using System.Net.Http.Headers;

    using Blorc.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;
    using MudBlazor.Extensions;

    using Tempore.App.Extensions;
    using Tempore.App.Pages.Employees;
    using Tempore.App.Services;
    using Tempore.App.Services.EventArgs;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;
    using Tempore.Client.Services.Interfaces;

    using Severity = MudBlazor.Severity;

    /// <summary>
    /// Workforce metrics.
    /// </summary>
    [Route(Routes.Analytics.WorkforceMetrics)]
    [Route(Routes.Analytics.WorkforceMetricsTemplate)]
    public partial class WorkforceMetrics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkforceMetrics"/> class.
        /// </summary>
        public WorkforceMetrics()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [Inject]
        public ILogger<WorkforceMetrics>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<WorkforceMetrics>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric client.
        /// </summary>
        [Inject]
        public IWorkforceMetricClient? WorkforceMetricClient { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric collections table component service.
        /// </summary>
        public IMudTableComponentService<WorkforceMetricCollectionDto>? WorkforceMetricCollectionsTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets the workforce metrics table component service.
        /// </summary>
        public IMudTableComponentService<IDictionary<string, object>>? WorkforceMetricsTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets the snackbar.
        /// </summary>
        [Inject]
        public ISnackbar? Snackbar { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INotificationCenter"/>.
        /// </summary>
        [Inject]
        public INotificationCenter? NotificationCenter { get; set; }

        [Inject]
        public IFileService FileService { get; set; }

        /// <summary>
        /// Gets or sets the start date parameter.
        /// </summary>
        [Parameter]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date parameter.
        /// </summary>
        [Parameter]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the selected date range.
        /// </summary>
        public DateRange SelectedDateRange { get; set; }

        /// <summary>
        /// Gets or sets the selected workforce metrics schema.
        /// </summary>
        public ICollection<ColumnInfo>? WorkforceMetricCollectionSchema { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workforce metric collections table filter is enabled.
        /// </summary>
        public bool IsWorkforceMetricCollectionsFilterEnabled
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsWorkforceMetricCollectionsFilterEnabled));
            set => this.SetPropertyValue(nameof(this.IsWorkforceMetricCollectionsFilterEnabled), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the compute workforce metric process is running.
        /// </summary>
        public bool IsComputeWorkforceMetricsProcessRunning
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsComputeWorkforceMetricsProcessRunning));
            set => this.SetPropertyValue(nameof(this.IsComputeWorkforceMetricsProcessRunning), value);
        }

        /// <summary>
        /// Gets or sets the workforce metric collection.
        /// </summary>
        public WorkforceMetricCollectionDto? SelectedWorkforceMetricCollection
        {
            get => this.GetPropertyValue<WorkforceMetricCollectionDto?>(nameof(this.SelectedWorkforceMetricCollection));
            set => this.SetPropertyValue(nameof(this.SelectedWorkforceMetricCollection), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether is workforce metric by workforce filter enabled.
        /// </summary>
        public bool IsWorkforceMetricFilterEnabled
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsWorkforceMetricFilterEnabled));
            set => this.SetPropertyValue(nameof(this.IsWorkforceMetricFilterEnabled), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether is workforce metric by workforce filter enabled.
        /// </summary>
        public bool IsExportingWorkforceMetrics
        {
            get => this.GetPropertyValue<bool>(nameof(this.IsExportingWorkforceMetrics));
            set => this.SetPropertyValue(nameof(this.IsExportingWorkforceMetrics), value);
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            if (this.StartDate == default)
            {
                this.StartDate = DateTime.Today.AddDays(-15);
            }

            if (this.EndDate == default)
            {
                this.EndDate = DateTime.Today;
            }

            this.SelectedDateRange = new DateRange(this.StartDate, this.EndDate);
        }

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            this.NotificationCenter!.Subscribe(NotificationTypes.QueueTaskStarted, this.OnNotificationReceived);
            this.NotificationCenter!.Subscribe(NotificationTypes.QueueTaskCompleted, this.OnNotificationReceived);

            await this.UpdateComputeWorkforceMetricsProcessStateAsync();
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.IsExportingWorkforceMetrics):
                case nameof(this.IsComputeWorkforceMetricsProcessRunning):
                    this.StateHasChanged();
                    break;
                case nameof(this.SelectedWorkforceMetricCollection):
                    this.InvokeAsync(
                        async () =>
                        {
                            var selectedWorkforceMetricCollection = this.SelectedWorkforceMetricCollection;
                            if (selectedWorkforceMetricCollection is not null)
                            {
                                this.WorkforceMetricCollectionSchema = await this.WorkforceMetricClient!.GetWorkforceMetricsSchemaAsync(selectedWorkforceMetricCollection.Id, SchemaType.Display);
                            }
                            else
                            {
                                this.WorkforceMetricCollectionSchema = null;
                            }

                            this.StateHasChanged();
                        });
                    break;
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.NotificationCenter!.Unsubscribe(NotificationTypes.QueueTaskStarted, this.OnNotificationReceived);
                this.NotificationCenter!.Unsubscribe(NotificationTypes.QueueTaskCompleted, this.OnNotificationReceived);
            }
        }

        private void OnNotificationReceived(NotificationReceivedEventArgs e)
        {
            this.InvokeAsync(async () => await this.UpdateComputeWorkforceMetricsProcessStateAsync());
        }

        private async Task UpdateComputeWorkforceMetricsProcessStateAsync()
        {
            try
            {
                this.IsComputeWorkforceMetricsProcessRunning = await this.WorkforceMetricClient!.IsComputeWorkforceMetricsProcessRunningAsync();
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred determining if compute workforce process is running"]);
            }

            this.StateHasChanged();
        }

        private async Task ComputeWorkforceMetricsAsync(WorkforceMetricCollectionDto workforceMetricCollectionDto)
        {
            var computeWorkforceMetricsRequest = new ComputeWorkforceMetricsRequest
            {
                StartDate = this.SelectedDateRange.Start!.Value,
                EndDate = this.SelectedDateRange.End!.Value,
                WorkForceMetricCollectionIds = new List<Guid>
                                               {
                                                   workforceMetricCollectionDto!.Id,
                                               },
            };

            try
            {
                await this.WorkforceMetricClient!.ComputeWorkforceMetricsAsync(computeWorkforceMetricsRequest);
                await this.UpdateComputeWorkforceMetricsProcessStateAsync();
            }
            catch (ApiException ex)
            {
                this.Logger?.LogError(ex, "An error occurred computing workforce metrics");

                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred computing workforce metrics"], Severity.Error);
            }
        }

        private async Task RefreshWorkforceMetricCollectionAsync()
        {
            await this.WorkforceMetricCollectionsTableComponentService!.ReloadServerDataAsync();
        }

        private async Task<TableData<WorkforceMetricCollectionDto>> GetWorkforceMetricCollectionsAsync(TableState tableState)
        {
            var tableData = new TableData<WorkforceMetricCollectionDto>
            {
                Items = Array.Empty<WorkforceMetricCollectionDto>(),
            };

            try
            {
                var request = new GetWorkforceMetricCollectionsRequest
                {
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                };

                var response = await this.WorkforceMetricClient!.GetWorkforceMetricCollectionsAsync(request);
                tableData.Items = response.Items;
                tableData.TotalItems = response.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred loading workforce metric collections"]);
            }

            return tableData;
        }

        private string SelectedWorkforceMetricTableRowClassFunc(WorkforceMetricCollectionDto workforceMetricCollection, int idx)
        {
            if (this.SelectedWorkforceMetricCollection == workforceMetricCollection)
            {
                return "selected";
            }

            return string.Empty;
        }

        private async Task<TableData<IDictionary<string, object>>> GetWorkforceMetricsAsync(TableState tableState)
        {
            var tableData = new TableData<IDictionary<string, object>>
            {
                Items = Array.Empty<Dictionary<string, object>>(),
            };

            try
            {
                var request = new GetWorkforceMetricsRequest
                {
                    WorkforceMetricCollectionId = this.SelectedWorkforceMetricCollection!.Id,
                    StartDate = this.SelectedDateRange.Start!.Value,
                    EndDate = this.SelectedDateRange.End!.Value,
                    Skip = tableState.Skip(),
                    Take = tableState.PageSize,
                };

                var response = await this.WorkforceMetricClient!.GetWorkforceMetricsAsync(request);

                tableData.Items = response.Items;
                tableData.TotalItems = response.Count;
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["An error occurred loading workforce metrics"], Severity.Error);
            }

            return tableData;
        }

        private async Task RefreshWorkforceMetricsAsync()
        {
            await this.WorkforceMetricsTableComponentService!.ReloadServerDataAsync();
        }

        private void OnWorkforceMetricCollectionRowClick(TableRowClickEventArgs<WorkforceMetricCollectionDto> args)
        {
            this.SelectedWorkforceMetricCollection = this.SelectedWorkforceMetricCollection != args.Item ? args.Item : null;
        }

        private async Task ExportWorkforceMetricsAsync()
        {
            this.IsExportingWorkforceMetrics = true;
            var exportWorkforceMetricsRequest = new ExportWorkforceMetricsRequest
            {
                WorkforceMetricCollectionId = this.SelectedWorkforceMetricCollection!.Id,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                FileFormat = FileFormat.CSV,
            };

            try
            {
                var fileResponse =
                    await this.WorkforceMetricClient!.ExportWorkforceMetricsAsync(exportWorkforceMetricsRequest);
                await using var memoryStream = new MemoryStream();
                await fileResponse.Stream.CopyToAsync(memoryStream);

                var fileResponseHeader = fileResponse.Headers["Content-Disposition"].First();
                var contentDispositionHeaderValue = ContentDispositionHeaderValue.Parse(fileResponseHeader);
                await this.FileService.SaveAsync(contentDispositionHeaderValue.FileName!, memoryStream.ToArray());

                using var snackbar = this.Snackbar!.Add(
                    this.StringLocalizer![
                        "The workforce metrics have been successfully exported to a file. Please check your browser’s download manager."],
                    Severity.Success);
            }
            catch (ApiException)
            {
                using var snackbar = this.Snackbar!.Add(
                    this.StringLocalizer!["An error occurred exporting workforce metrics"],
                    Severity.Error);
            }
            finally
            {
                IsExportingWorkforceMetrics = false;
            }
        }

    }
}