// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFileRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.FileProcessing
{
    using MediatR;

    /// <summary>
    /// The process file request.
    /// </summary>
    public class ProcessFileRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        public Guid FileId { get; set; }
    }
}