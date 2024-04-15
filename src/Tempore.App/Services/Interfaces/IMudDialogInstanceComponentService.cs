// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMudDialogInstanceComponentService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services.Interfaces
{
    using Blorc.Services;

    using MudBlazor;

    /// <summary>
    /// The <see cref="MudDialogInstance"/> component service interface.
    /// </summary>
    public interface IMudDialogInstanceComponentService : IComponentService
    {
        /// <summary>
        /// Closes the dialog instance.
        /// </summary>
        /// <param name="returnValue">
        /// The return value.
        /// </param>
        /// <typeparam name="TReturnValue">
        /// The return value type.
        /// </typeparam>
        void Close<TReturnValue>(TReturnValue returnValue);

        /// <summary>
        /// The cancel the dialog instance.
        /// </summary>
        void Cancel();
    }
}