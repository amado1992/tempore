// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportFile.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages.Employees
{
    using System;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Extensions;
    using Tempore.App.Pages;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// The import file page.
    /// </summary>
    [Route(Routes.Employees.ImportFromFiles.ImportFromFile)]
    public partial class ImportFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportFile"/> class.
        /// </summary>
        public ImportFile()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets files table component service.
        /// </summary>
        public IMudTableComponentService<DataFileDto>? FilesTableComponentService { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [Inject]
        public ILogger<ImportFile>? Logger { get; set; }

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<ImportFile>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the file processing client.
        /// </summary>
        [Inject]
        public IFileProcessingClient? FileProcessingClient { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the dialog service.
        /// </summary>
        [Inject]
        public IDialogService? DialogService { get; set; }

        /// <summary>
        /// Gets or sets the snackbar.
        /// </summary>
        [Inject]
        private ISnackbar? Snackbar { get; set; }

        private async Task UploadFilesAsync(IBrowserFile file)
        {
            this.Snackbar!.Clear();
            try
            {
                var fileParameter = new FileParameter(file.OpenReadStream(), file.Name);
                await this.FileProcessingClient!.UploadFileAsync(fileParameter, FileType.PayDay);
                using var snackbar = this.Snackbar!.Add(
                                this.StringLocalizer!["File uploaded successfully"],
                                Severity.Success);
            }
            catch (ApiException ex)
            {
                this.Logger!.LogInformation("Error occurred uploading file: ", ex.Message);
                this.Snackbar!.Clear();
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred uploading file"], Severity.Error);
            }
            finally
            {
                await this.FilesTableComponentService!.ReloadServerDataAsync();
            }
        }

        /// <summary>
        /// Gets the files async.
        /// </summary>
        /// <param name="tableState">
        /// The table state.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<TableData<DataFileDto>> GetFilesAsync(TableState tableState)
        {
            this.Snackbar!.Clear();
            try
            {
                var response = await this.FileProcessingClient!.GetListFilesAsync(tableState.Skip(), tableState.PageSize);
                var tableData = new TableData<DataFileDto>
                {
                    TotalItems = response.Count,
                    Items = response.Items,
                };

                return tableData;
            }
            catch (ApiException ex)
            {
                this.Snackbar!.Clear();
                this.Logger!.LogInformation("Error occurred showing file: ", ex.Message);
                using var _ = this.Snackbar!.Add(this.StringLocalizer!["Error occurred uploading file"], Severity.Error);
                return new TableData<DataFileDto>();
            }
        }

        private async Task RefreshAsync(MouseEventArgs arg)
        {
            await this.FilesTableComponentService!.ReloadServerDataAsync();
        }

        private void PreviewFileAsync(Guid fileId)
        {
            this.NavigationManager!.NavigateTo(Routes.Employees.ImportFromFiles.PreviewFile(fileId));
        }

        private async void DeleteFileAsync(Guid fileId, string filename)
        {
            this.Snackbar!.Clear();
            try
            {
                if (await this.DialogService!.ConfirmAsync(this.StringLocalizer!["Are you sure that you want to delete this file '{0}'?", filename]))
                {
                    await this.FileProcessingClient!.DeleteFileExistAsync(fileId);
                    using var snackbar = this.Snackbar!.Add(
                                    this.StringLocalizer!["File deleted successfully"],
                                    Severity.Success);
                }
            }
            catch (ApiException ex)
            {
                this.Snackbar!.Clear();
                this.Logger!.LogInformation("Error occurred deleting file: ", ex.Message);
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred deleting file"], Severity.Error);
            }
            finally
            {
                await this.FilesTableComponentService!.ReloadServerDataAsync();
            }
        }

        private async void ProcessAsync(Guid fileId, string filename)
        {
            this.Snackbar!.Clear();
            try
            {
                if (await this.DialogService!.ConfirmAsync(this.StringLocalizer!["Are you sure that you want to process this file '{0}'?", filename]))
                {
                    await this.FileProcessingClient!.ProcessAsync(fileId);
                    using var snackbar = this.Snackbar!.Add(
                                    this.StringLocalizer!["File processed successfully"],
                                    Severity.Success);
                }
            }
            catch (ApiException ex)
            {
                this.Snackbar!.Clear();
                this.Logger!.LogInformation(ex, "Error occurred processing file");
                using var snackbar = this.Snackbar!.Add(this.StringLocalizer!["Error occurred processing file"], Severity.Error);
            }
            finally
            {
                await this.FilesTableComponentService!.ReloadServerDataAsync();
            }
        }
    }
}