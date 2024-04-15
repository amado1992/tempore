// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFileDataByIdRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.FileProcessing
{
    /// <summary>
    /// The get file content by id request.
    /// </summary>
    public class GetFileDataByIdRequest : PagedRequest<Dictionary<string, string>>
    {
        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        public Guid FileId { get; set; }
    }
}