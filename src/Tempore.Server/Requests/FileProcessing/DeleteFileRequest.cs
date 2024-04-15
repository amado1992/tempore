// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteFileRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.FileProcessing
{
    using MediatR;

    public class DeleteFileRequest : IRequest
    {
        public Guid FileId { get; set; }
    }
}