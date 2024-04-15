// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MudDialogInstanceComponentService.cs" company="Port Hope Investment S.A.">
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
    public class MudDialogInstanceComponentService : ComponentServiceBase<MudDialogInstance>, IMudDialogInstanceComponentService
    {
        /// <inheritdoc />
        public void Close<TReturnValue>(TReturnValue returnValue)
        {
            this.TypedComponent!.Close(returnValue);
        }

        /// <inheritdoc />
        public void Cancel()
        {
            this.TypedComponent!.Cancel();
        }
    }
}