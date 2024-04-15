// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentDevices.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Employees
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Extensions;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    [Route(Routes.Employees.ImportFromDevices.AgentDevicesTemplate)]
    public partial class AgentDevices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentDevices"/> class.
        /// </summary>
        public AgentDevices()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the device component service.
        /// </summary>
        public IMudTableComponentService<DeviceDto>? DevicesTableComponentService { get; set; }

        [Parameter]
        public Guid AgentId { get; set; }

        [Inject]
        public IStringLocalizer<AgentDevices>? StringLocalizer { get; set; }

        [Inject]
        public IDeviceClient? DeviceClient { get; set; }

        [Inject]
        public IAgentCommandClient? AgentCommandClient { get; set; }

        [Inject]
        public IAgentClient? AgentClient { get; set; }

        public AgentDto? CurrentAgent { get; set; }

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            this.CurrentAgent = await this.AgentClient!.GetAgentByIdAsync(this.AgentId);
        }

        private async Task<TableData<DeviceDto>> GetDevicesAsync(TableState tableState)
        {
            var paginationResponse = await this.DeviceClient!.GetDevicesByAgentIdAsync(this.AgentId, tableState.Skip(), tableState.PageSize);

            var tableData = new TableData<DeviceDto>
            {
                Items = paginationResponse.Items,
                TotalItems = paginationResponse.Count,
            };

            return tableData;
        }

        private async Task RefreshAsync(MouseEventArgs arg)
        {
            await this.DevicesTableComponentService!.ReloadServerDataAsync();
        }

        private async Task ExecuteUploadEmployeesAsync(DeviceDto device)
        {
            await this.AgentCommandClient!.UploadEmployeesFromDevicesAsync(UploadEmployeesFromDevicesRequest.Create(device.Id));
        }
    }
}