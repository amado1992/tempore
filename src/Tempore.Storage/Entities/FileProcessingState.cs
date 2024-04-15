// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileProcessingState.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    /// <summary>
    /// The file processing state.
    /// </summary>
    public enum FileProcessingState
    {
        /// <summary>
        /// The not processed file processing state.
        /// </summary>
        NotProcessed,

        /// <summary>
        /// The completed file processing state.
        /// </summary>
        Completed,

        /// <summary>
        /// The incomplete file processing state.
        /// </summary>
        Incomplete,

        /// <summary>
        /// The failed file processing state.
        /// </summary>
        Failed,
    }
}