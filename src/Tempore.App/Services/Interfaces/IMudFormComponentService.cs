// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMudFormComponentService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services.Interfaces
{
    using Blorc.Services;

    /// <summary>
    /// The MudFormComponentService interface.
    /// </summary>
    public interface IMudFormComponentService : IComponentService
    {
        /// <summary>
        /// Validates form.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>. <c>true</c> if the model is valid, otherwise <c>false</c>.
        /// </returns>
        Task<bool> ValidateAsync();
    }
}