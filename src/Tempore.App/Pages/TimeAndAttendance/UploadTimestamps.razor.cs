// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTimestamps.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.TimeAndAttendance
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Newtonsoft.Json;

    using Tempore.App.Pages;
    using Tempore.Client;

    /// <summary>
    /// The upload timestamps.
    /// </summary>
    [Route(Routes.TimeAndAttendance.UploadEmployeesTimestamps)]
    public sealed partial class UploadTimestamps
    {
        /// <summary>
        /// The selected date range.
        /// </summary>
        private DateRange selectedDateRange = new(DateTime.Now.Date.AddDays(-1), DateTime.Now.Date);

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadTimestamps"/> class.
        /// </summary>
        public UploadTimestamps()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<UploadTimestamps>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the Snackbar.
        /// </summary>
        [Inject]
        public ISnackbar? Snackbar { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [Inject]
        public ILogger<UploadTimestamps>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the agent command client.
        /// </summary>
        [Inject]
        public IAgentCommandClient? AgentCommandClient { get; set; }

        /// <summary>
        /// Gets or sets the agent client.
        /// </summary>
        [Inject]
        public IAgentClient? AgentClient { get; set; }

        /// <summary>
        /// Gets or sets the device client.
        /// </summary>
        [Inject]
        public IDeviceClient? DeviceClient { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public HashSet<TreeItem> Items { get; set; } = new HashSet<TreeItem>();

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        public HashSet<TreeItem>? SelectedItems { get; set; }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            var treeItem = new TreeItem
            {
                Id = Guid.Empty,
                Title = this.StringLocalizer!["All"],
                Icon = Icons.Material.Filled.DeviceHub,
                CanExpand = true,
            };

            this.Items.Add(treeItem);
        }

        /// <summary>
        /// The upload timestamps async.
        /// </summary>
        /// <param name="arg">
        /// The arg.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UploadTimestampsAsync(MouseEventArgs arg)
        {
            // TODO: How to properly select the devices?
            var request = new UploadTimestampsFromDevicesRequest
            {
                DeviceIds = this.SelectedItems?.Where(item => !item.CanExpand).Select(item => item.Id).ToList(),
                From = new DateTimeOffset(this.selectedDateRange.Start!.Value),
                To = new DateTimeOffset(this.selectedDateRange.End!.Value),
            };

            try
            {
                await this.AgentCommandClient!.UploadTimestampsFromDevicesAsync(request);
            }
            catch (JsonSerializationException ex)
            {
                // Report validation errors?
                using var _ = this.Snackbar!.Add(ex.Message, Severity.Error);
            }
            catch (ApiException)
            {
                using var _ = this.Snackbar!.Add(this.StringLocalizer!["Error occurred requesting to upload the timestamps"], Severity.Error);
            }
        }

        private async Task<HashSet<TreeItem>> LoadServerData(TreeItem parentItem)
        {
            var items = new HashSet<TreeItem>();
            if (parentItem.Id == Guid.Empty)
            {
                // Buffer
                var response = await this.AgentClient!.GetAgentsAsync(0, int.MaxValue);
                foreach (var agent in response.Items)
                {
                    var item = new TreeItem
                    {
                        Id = agent.Id,
                        Title = agent.Name,
                        Icon = Icons.Material.Filled.DeviceHub,
                        CanExpand = true,
                        Selected = parentItem.Selected,
                    };

                    items.Add(item);
                }
            }
            else
            {
                // Buffer
                var response = await this.DeviceClient!.GetDevicesByAgentIdAsync(parentItem.Id, 0, int.MaxValue);
                foreach (var device in response.Items)
                {
                    var item = new TreeItem
                    {
                        Id = device.Id,
                        Title = device.Name,
                        Icon = Icons.Material.Filled.Devices,
                        CanExpand = false,
                        Selected = parentItem.Selected,
                    };

                    items.Add(item);
                }
            }

            return items;
        }
    }

    public class TreeItem
    {
        public string Icon { get; set; }

        public bool CanExpand { get; set; }

        public string Title { get; set; }

        public Guid Id { get; set; }

        public bool Selected { get; set; }
    }
}