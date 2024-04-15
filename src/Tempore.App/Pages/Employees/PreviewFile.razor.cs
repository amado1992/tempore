// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreviewFile.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Employees
{
    using System;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Extensions;
    using Tempore.App.Pages;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// The preview file page.
    /// </summary>
    [Route(Routes.Employees.ImportFromFiles.PreviewFileTemplate)]
    public partial class PreviewFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewFile"/> class.
        /// </summary>
        public PreviewFile()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public ICollection<string>? Columns { get; set; }

        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        [Parameter]
        public Guid? FileId { get; set; }

        /// <summary>
        /// Gets or sets the file content component service.
        /// </summary>
        public IMudTableComponentService<IDictionary<string, string>>? FileContentTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<PreviewFile>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the file processing client.
        /// </summary>
        [Inject]
        public IFileProcessingClient? FileProcessingClient { get; set; }

        /// <summary>
        /// Called on initialized.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task OnInitializedAsync()
        {
            this.Columns = await this.FileProcessingClient!.GetFileSchemaAsync(this.FileId);
        }

        /// <summary>
        /// Gets file content async.
        /// </summary>
        /// <param name="tableState">
        /// The table state.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<TableData<IDictionary<string, string>>> GetFileDataAsync(TableState tableState)
        {
            var response = await this.FileProcessingClient!.GetFileDataByIdAsync(this.FileId, tableState.Skip(), tableState.PageSize);
            var tableData = new TableData<IDictionary<string, string>>
            {
                TotalItems = response.Count,
                Items = response.Items,
            };

            return tableData;
        }
    }
}