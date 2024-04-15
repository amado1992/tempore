// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MudFormComponentService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services
{
    using MudBlazor;

    using Tempore.App.Services.Interfaces;

    /// <summary>
    /// The mud dialog instance component service.
    /// </summary>
    public class MudFormComponentService : ComponentServiceBase<MudForm>, IMudFormComponentService
    {
        /// <inheritdoc />
        public async Task<bool> ValidateAsync()
        {
            await this.TypedComponent!.Validate();

            return this.TypedComponent.IsValid;
        }
    }
}