// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests.FileProcessing;

    public interface IFileService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Guid> UploadFileAsync(UploadFileRequest file);
    }
}