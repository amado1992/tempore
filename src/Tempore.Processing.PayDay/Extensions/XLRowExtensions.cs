// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XLRowExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Extensions
{
    using System.Collections.Generic;

    using ClosedXML.Excel;

    /// <summary>
    /// The xl row extensions.
    /// </summary>
    public static class XLRowExtensions
    {
        /// <summary>
        /// Fast and safe gets a cell value.
        /// </summary>
        /// <typeparam name="TValue">
        /// The value type.
        /// </typeparam>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The TValue value.
        /// </returns>
        public static TValue? FastGetCellValue<TValue>(this IXLRow row, Dictionary<string, int> schema, string columnName)
        {
            return schema.TryGetValue(columnName, out int idx) && idx > 0 ? row.Cell(idx).GetValue<TValue>() : default;
        }
    }
}