// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XLWorksheetExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Extensions
{
    using System.Collections.Immutable;
    using System.Globalization;

    using ClosedXML.Excel;

    using static Tempore.Processing.PayDay.Services.PayDayFileProcessingService;

    /// <summary>
    /// The ixl worksheet extensions.
    /// </summary>
    public static class XLWorksheetExtensions
    {
        /// <summary>
        /// Gets the row header.
        /// </summary>
        /// <param name="worksheet">
        /// The worksheet.
        /// </param>
        /// <param name="text">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int SearchFirstCellRowWithText(this IXLWorksheet worksheet, string text)
        {
            // TODO: Review if this is the best approach?
            var foundCells = worksheet.Search(text, CompareOptions.OrdinalIgnoreCase);

            var firstCell = foundCells
                .OrderBy(cell => cell.Address.RowNumber)
                .ThenBy(cell => cell.Address.ColumnNumber)
                .FirstOrDefault();

            if (firstCell is null)
            {
                return 0;
            }

            return firstCell.Address.RowNumber;
        }

        /// <summary>
        /// Find the column indexes by names.
        /// </summary>
        /// <param name="worksheet">
        /// The worksheet.
        /// </param>
        /// <param name="columnNames">
        /// The column names.
        /// </param>
        /// <param name="mandatoryColumnName">
        /// A mandatory column name.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary{String, Integer}"/>.
        /// The column indexes.
        /// </returns>
        public static Dictionary<string, int> FindColumnIndexes(this IXLWorksheet worksheet, IImmutableList<string> columnNames, string mandatoryColumnName)
        {
            var indexes = new Dictionary<string, int>();
            foreach (var columnName in columnNames)
            {
                indexes[columnName] = 0;
            }

            var rowHeader = worksheet.SearchFirstCellRowWithText(mandatoryColumnName);
            var colLastHeader = worksheet.Row(rowHeader).LastCellUsed().Address.ColumnNumber;
            for (var i = 1; i <= colLastHeader; i++)
            {
                var row = worksheet.Row(rowHeader);
                var value = row.Cell(i).GetValue<string>();
                var idx = columnNames.IndexOf(value);
                if (idx >= -1)
                {
                    indexes[value] = row.Cell(i).Address.ColumnNumber;
                }
            }

            return indexes;
        }
    }
}