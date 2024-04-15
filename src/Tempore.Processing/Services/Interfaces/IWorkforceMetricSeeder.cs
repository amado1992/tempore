// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkforceMetricSeeder.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.Interfaces
{
    /// <summary>
    /// The workforce metric seeder.
    /// </summary>
    public interface IWorkforceMetricSeeder
    {
        /// <summary>
        /// The seed async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SeedAsync();
    }
}