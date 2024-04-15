// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.FileProcessing
{
    using MediatR;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The upload file request.
    /// </summary>
    public class UploadFileRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// Gets or sets the file type.
        /// </summary>
        public FileType FileType { get; set; }
    }
}