// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogServiceExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Extensions
{
    using Microsoft.AspNetCore.Components;

    using MudBlazor;

    using Tempore.App.Dialogs;

    /// <summary>
    /// The dialog service extensions.
    /// </summary>
    public static class DialogServiceExtensions
    {
        /// <summary>
        /// Confirm async using <see cref="Confirmation"/> dialog.
        /// </summary>
        /// <param name="dialogService">
        /// The dialog service.
        /// </param>
        /// <param name="confirmText">
        /// The confirm text.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<bool> ConfirmAsync(this IDialogService dialogService, string confirmText)
        {
            ArgumentNullException.ThrowIfNull(dialogService);

            var parameters = new DialogParameters<Confirmation>
                             {
                                 { x => x.ContentText, confirmText },
                             };

            var dialogReference = await dialogService.ShowAsync<Confirmation>(string.Empty, parameters);
            return await dialogReference.GetReturnValueAsync<bool?>() ?? false;
        }

        /// <summary>
        /// The show async.
        /// </summary>
        /// <param name="dialogService">
        /// The dialog service.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="TDialog">
        /// The dialog type.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The dialog result type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<TResult> ShowAsync<TDialog, TResult>(
            this IDialogService dialogService, DialogParameters<TDialog> parameters)
            where TDialog : ComponentBase
        {
            ArgumentNullException.ThrowIfNull(dialogService);

            var dialogReference = await dialogService.ShowAsync<TDialog>(string.Empty, parameters);
            return await dialogReference.GetReturnValueAsync<TResult>();
        }
    }
}