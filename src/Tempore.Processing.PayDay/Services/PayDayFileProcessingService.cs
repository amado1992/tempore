// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayDayFileProcessingService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Services
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    using ClosedXML.Excel;

    using Microsoft.Extensions.Logging;

    using Tempore.Common.Extensions;
    using Tempore.Processing.PayDay.Extensions;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The pay day file processing service.
    /// </summary>
    public class PayDayFileProcessingService : IFileProcessingService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<PayDayFileProcessingService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayDayFileProcessingService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public PayDayFileProcessingService(ILogger<PayDayFileProcessingService> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<bool> IsValidAsync(Stream stream)
        {
            XLWorkbook? workbook = null;
            try
            {
                workbook = new XLWorkbook(stream);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "The file is not a valid ");
            }

            if (workbook is null)
            {
                return Task.FromResult(false);
            }

            try
            {
                var worksheet = workbook.Worksheet(1);
                return Task.FromResult(ValidateRowHeader(worksheet));
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "The file is not a valid ");
                return Task.FromResult(false);
            }
            finally
            {
                workbook.Dispose();
            }
        }

        /// <inheritdoc/>
        public async Task<(int Count, List<Dictionary<string, string>> Items)> GetFileDataAsync(DataFile dataFile, int skip, int take)
        {
            using var stream = new MemoryStream();
            await stream.WriteAsync(dataFile.Data);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var rows = this.ReadContent(worksheet, dataFile.FileName);
            var items = rows.Skip(skip).Take(take).ToList();
            return (rows.Count, items);
        }

        /// <inheritdoc/>
        public async Task<List<string>> GetFileSchemeAsync(DataFile dataFile)
        {
            using var stream = new MemoryStream();
            await stream.WriteAsync(dataFile.Data);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            return this.ReadColumnNames(worksheet, dataFile.FileName);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Employee> GetEmployeesAsync(DataFile dataFile)
        {
            this.logger.LogInformation("Processing file {FileName}", dataFile.FileName);

            // Create a memory stream
            using var stream = new MemoryStream();

            // Write some data to the stream
            await stream.WriteAsync(dataFile.Data);

            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed().RowNumber();
            var schema = worksheet.FindColumnIndexes(PayDayFileColumnNames.All, PayDayFileColumnNames.ExternalId);
            var rowFirstData = worksheet.SearchFirstCellRowWithText(PayDayFileColumnNames.ExternalId) + 1;
            for (var idx = rowFirstData; idx < lastRow; idx++)
            {
                Employee? employee = null;
                try
                {
                    var row = worksheet.Row(idx);

                    var externalId = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.ExternalId);
                    var fullName = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.FullName);
                    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(externalId))
                    {
                        this.logger.LogWarning("Required data is missing in row {RowIdx} of file {FileIdx}. Skipped.", idx, dataFile.Id);
                        continue;
                    }

                    employee = new Employee
                    {
                        ExternalId = externalId,
                        FullName = fullName,
                        IdentificationCard = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.IdentificationCard),
                        SocialSecurity = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.SocialSecurity),
                        Department = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.Department),
                        CostCenter = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.CostCenter),
                        AdmissionDate = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.AdmissionDate)?.ParseAsDateTime(),
                        BaseHours = row.FastGetCellValue<string>(schema, PayDayFileColumnNames.BaseHours)?.ParseAsFloat(),
                    };
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, "Error processing row {RowIdx} of file {FileIdx}", idx, dataFile.Id);
                }

                if (employee is not null)
                {
                    yield return employee;
                }
            }

            // TODO: {...}
            this.logger.LogInformation("File {FileName} processed", dataFile.FileName);
        }

        /// <summary>
        /// The validate row header.
        /// </summary>
        /// <param name="worksheet">
        /// The worksheet.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ValidateRowHeader(IXLWorksheet worksheet)
        {
            var mandatoryColumns = new HashSet<string>
                          {
                              PayDayFileColumnNames.ExternalId,
                              PayDayFileColumnNames.FullName,
                          };

            int headerRowIdx = 0;
            foreach (var columnName in PayDayFileColumnNames.All)
            {
                var rowIdx = worksheet.SearchFirstCellRowWithText(columnName);
                if (rowIdx == 0)
                {
                    if (!mandatoryColumns.Contains(columnName))
                    {
                        continue;
                    }

                    return false;
                }

                if (headerRowIdx == 0)
                {
                    headerRowIdx = rowIdx;
                }
                else if (headerRowIdx != rowIdx)
                {
                    return false;
                }
            }

            return true;
        }

        private List<Dictionary<string, string>> ReadContent(IXLWorksheet worksheet, string fileName)
        {
            var content = new List<Dictionary<string, string>>();

            if (ValidateRowHeader(worksheet))
            {
                int lastRow = worksheet.LastRowUsed().RowNumber();
                int rowHeaderIndex = worksheet.SearchFirstCellRowWithText(PayDayFileColumnNames.ExternalId);
                for (int i = rowHeaderIndex + 1; i <= lastRow; i++)
                {
                    var row = worksheet.Row(i);
                    if (!row.IsEmpty())
                    {
                        int lastCellColumnIndex = row.LastCellUsed().Address.ColumnNumber;

                        Dictionary<string, string> cellPair = new Dictionary<string, string>();
                        for (int j = 1; j <= lastCellColumnIndex; j++)
                        {
                            var cellIndex = row.Cell(j).Address.ToString();
                            var cellValue = row.Cell(j).GetValue<string>();
                            cellPair.Add(cellIndex!, cellValue);
                        }

                        content.Add(cellPair);
                    }
                }

                this.logger.LogInformation("File {FileName} processed", fileName);
            }
            else
            {
                this.logger.LogWarning("File {FileName} invalid", fileName);
            }

            return content;
        }

        private List<string> ReadColumnNames(IXLWorksheet worksheet, string fileName)
        {
            List<string> result = new List<string>();
            if (ValidateRowHeader(worksheet))
            {
                var rowHeaderIndex = worksheet.SearchFirstCellRowWithText(PayDayFileColumnNames.ExternalId);
                var row = worksheet.Row(rowHeaderIndex);
                var lastCellColumnIndex = row.LastCellUsed().Address.ColumnNumber;
                for (var idx = 1; idx <= lastCellColumnIndex; idx++)
                {
                    var cellValue = row.Cell(idx).GetValue<string>();
                    result.Add(cellValue);
                }
            }
            else
            {
                this.logger.LogWarning("File {FileName} invalid", fileName);
            }

            return result;
        }

        /// <summary>
        /// The expected pay day file column names.
        /// </summary>
        public class PayDayFileColumnNames
        {
            /// <summary>
            /// The code.
            /// </summary>
            public const string ExternalId = "Código";

            /// <summary>
            /// The identification card.
            /// </summary>
            public const string IdentificationCard = "Cédula";

            /// <summary>
            /// The social security.
            /// </summary>
            public const string SocialSecurity = "Seg. Soc.";

            /// <summary>
            /// The name.
            /// </summary>
            public const string FullName = "Nombre";

            /// <summary>
            /// The cost center.
            /// </summary>
            public const string CostCenter = "Costo";

            /// <summary>
            /// The admission date.
            /// </summary>
            public const string AdmissionDate = "Ingreso";

            /// <summary>
            /// The base hours.
            /// </summary>
            public const string BaseHours = "Base";

            /// <summary>
            /// The department.
            /// </summary>
            public const string Department = "Depto.";

            /// <summary>
            /// Gets all payday file column name.
            /// </summary>
            public static readonly IImmutableList<string> All = typeof(PayDayFileColumnNames).Constants<string>().ToImmutableList();
        }
    }
}