// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitializationService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    /// <summary>
    /// The DatabaseInitializationService interface.
    /// </summary>
    public interface IInitializationService
    {
        /// <summary>
        /// Gets a value indicating whether is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Runs the initialization.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task InitializeAsync();

        /// <summary>
        /// Waits for the initialization.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task WaitAsync();
    }
}