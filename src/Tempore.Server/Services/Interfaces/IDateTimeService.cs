// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDateTimeService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    /// <summary>
    /// The DateTimeService interface.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// The now.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        DateTime Now();

        /// <summary>
        /// The today.
        /// </summary>
        /// <returns>
        /// The <see cref="DateOnly"/>.
        /// </returns>
        DateOnly Today();
    }
}