// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MudTableComponentService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services
{
    using MudBlazor;

    using Tempore.App.Services.Interfaces;

    /// <summary>
    /// The MudTableComponentService interface.
    /// </summary>
    /// <typeparam name="TItem">
    /// The item type.
    /// </typeparam>
    public class MudTableComponentService<TItem> : ComponentServiceBase<MudTable<TItem>>, IMudTableComponentService<TItem>
    {
        /// <inheritdoc />
        public async Task ReloadServerDataAsync()
        {
            await this.TypedComponent!.ReloadServerData();
        }
    }
}