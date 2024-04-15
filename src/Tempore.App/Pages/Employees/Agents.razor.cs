// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Agents.razor.cs" company="Port Hope Investment S.A.">
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

    /// <summary>
    /// The agents.
    /// </summary>
    [Route(Routes.Employees.ImportFromDevices.Agents)]
    public partial class Agents
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Agents"/> class.
        /// </summary>
        public Agents()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the agents table component service.
        /// </summary>
        public IMudTableComponentService<AgentDto>? AgentsTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets the agent client.
        /// </summary>
        [Inject]
        public IAgentClient? AgentClient { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<Agents>? StringLocalizer { get; set; }

        /// <summary>
        /// Get items async.
        /// </summary>
        /// <param name="tableState">
        /// The table state.
        /// </param>
        /// <returns>
        /// The <see cref="Task{TableData}"/>.
        /// </returns>
        private async Task<TableData<AgentDto>> GetAgentsAsync(TableState tableState)
        {
            var paginationResponse = await this.AgentClient!.GetAgentsAsync(tableState.Skip(), tableState.PageSize);
            var tableData = new TableData<AgentDto>
            {
                Items = paginationResponse.Items,
                TotalItems = paginationResponse.Count,
            };

            return tableData;
        }

        private Task ViewDevicesAsync(Guid agentId)
        {
            this.NavigationManager!.NavigateTo(Routes.Employees.ImportFromDevices.ViewAgentDevices(agentId));

            return Task.CompletedTask;
        }

        private async Task RefreshAsync(MouseEventArgs obj)
        {
            await this.AgentsTableComponentService!.ReloadServerDataAsync();
        }
    }
}