// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableStateExtension.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Extensions
{
    using MudBlazor;

    /// <summary>
    /// The table state extension.
    /// </summary>
    public static class TableStateExtension
    {
        /// <summary>
        /// The skip.
        /// </summary>
        /// <param name="tableState">
        /// The table state.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Skip(this TableState tableState)
        {
            return tableState.Page * tableState.PageSize;
        }
    }
}