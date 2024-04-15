// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileContentWriter.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    /// <summary>
    /// The content file writer.
    /// </summary>
    public interface IFileContentWriter : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Writes data line.
        /// </summary>
        /// <param name="values">
        /// The values to be written.
        /// </param>
        /// <returns>
        /// A task.
        /// </returns>
        Task WriteLineAsync(IEnumerable<object?>? values = null);

        /// <summary>
        /// Writes data.
        /// </summary>
        /// <param name="values">
        /// The values to be written.
        /// </param>
        /// <returns>
        /// A task.
        /// </returns>
        Task WriteAsync(IEnumerable<object?>? values = null);

        /// <summary>
        /// Clears all buffers for this stream asynchronously and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <returns>
        /// A task.
        /// </returns>
        Task FlushAsync();

        /// <summary>
        /// Gets content.
        /// </summary>
        /// <returns>
        /// A task.
        /// </returns>
        Task<byte[]> GetContentAsync();

        /// <summary>
        /// Gets the full file name.
        /// </summary>
        /// <param name="name">
        /// The file name without extension.
        /// </param>
        /// <returns>
        /// The fullname.
        /// </returns>
        string GetFileName(string name);
    }
}