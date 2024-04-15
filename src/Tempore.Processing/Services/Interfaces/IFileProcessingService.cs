// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileProcessingService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.Interfaces
{
    using Tempore.Storage.Entities;

    /// <summary>
    /// The FileProcessingService interface.
    /// </summary>
    public interface IFileProcessingService
    {
        /// <summary>
        /// Gets the employees.
        /// </summary>
        /// <param name="dataFile">
        /// The data file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        IAsyncEnumerable<Employee> GetEmployeesAsync(DataFile dataFile);

        /// <summary>
        /// The is valid async.
        /// </summary>
        /// <param name="stream">
        /// The form file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> IsValidAsync(Stream stream);

        /// <summary>
        /// Gets the file content async.
        /// </summary>
        /// <param name="dataFile">
        /// The file data.
        /// </param>
        /// <param name="skip">
        /// The skip.
        /// </param>
        /// <param name="take">
        /// The take.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<(int Count, List<Dictionary<string, string>> Items)> GetFileDataAsync(DataFile dataFile, int skip, int take);

        /// <summary>
        /// Gets the file scheme async.
        /// </summary>
        /// <param name="dataFile">
        /// The data file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<List<string>> GetFileSchemeAsync(DataFile dataFile);
    }
}