// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMudTableComponentService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services.Interfaces
{
    using Blorc.Services;

    /// <summary>
    /// The MudTableComponentService interface.
    /// </summary>
    /// <typeparam name="TItem">
    /// The item type.
    /// </typeparam>
    public interface IMudTableComponentService<TItem> : IComponentService
    {
        /// <summary>
        /// Reloads the server data async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ReloadServerDataAsync();
    }
}