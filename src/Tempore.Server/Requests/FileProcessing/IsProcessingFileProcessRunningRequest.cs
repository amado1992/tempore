// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsProcessingFileProcessRunningRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.FileProcessing
{
    using MediatR;

    /// <summary>
    /// The IsProcessingFileProcessRunningRequest class.
    /// </summary>
    public class IsProcessingFileProcessRunningRequest : IRequest<bool>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IsProcessingFileProcessRunningRequest Instance = new IsProcessingFileProcessRunningRequest();

        private IsProcessingFileProcessRunningRequest()
        {
        }
    }
}