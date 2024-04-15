// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListFileRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.FileProcessing
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    using Tempore.Storage.Entities;

    public class ListFileRequest : PagedRequest<DataFileDto>
    {
    }
}