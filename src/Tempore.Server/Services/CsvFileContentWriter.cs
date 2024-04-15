// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvFileContentWriter.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using System.Globalization;
    using System.IO;
    using System.Threading;

    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The csv file content writer.
    /// </summary>
    public sealed class CsvFileContentWriter : IFileContentWriter
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        private readonly StreamWriter streamWriter;

        private readonly MemoryStream memoryStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFileContentWriter"/> class.
        /// </summary>
        public CsvFileContentWriter()
        {
            this.memoryStream = new MemoryStream();
            this.streamWriter = new StreamWriter(this.memoryStream);
        }

        /// <inheritdoc />
        public async Task WriteLineAsync(IEnumerable<object?>? values = null)
        {
            await this.semaphoreSlim.WaitAsync();

            try
            {
                await this.WriteValuesAsync(values);
                await this.streamWriter.WriteLineAsync();
            }
            finally
            {
                this.semaphoreSlim.Release();
            }
        }

        /// <inheritdoc />
        public async Task WriteAsync(IEnumerable<object?>? values = null)
        {
            await this.semaphoreSlim.WaitAsync();
            try
            {
                await this.WriteValuesAsync(values);
            }
            finally
            {
                this.semaphoreSlim.Release();
            }
        }

        /// <inheritdoc />
        public Task FlushAsync()
        {
            return this.streamWriter.FlushAsync();
        }

        /// <inheritdoc />
        public async Task<byte[]> GetContentAsync()
        {
            await using var destination = new MemoryStream();
            await this.semaphoreSlim.WaitAsync();
            try
            {
                var position = this.streamWriter.BaseStream.Position;
                this.streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                await this.streamWriter.BaseStream.CopyToAsync(destination);
                this.streamWriter.BaseStream.Seek(position, SeekOrigin.Begin);
            }
            finally
            {
                this.semaphoreSlim.Release();
            }

            return destination.ToArray();
        }

        /// <inheritdoc />
        public string GetFileName(string name)
        {
            return $"{name}.csv";
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.semaphoreSlim.Dispose();
            this.streamWriter.Dispose();
            this.memoryStream.Dispose();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            this.semaphoreSlim.Dispose();
            await this.streamWriter.DisposeAsync();
            await this.memoryStream.DisposeAsync();
        }

        private async Task WriteValuesAsync(IEnumerable<object?>? values)
        {
            var formattedValues = values?.Aggregate(
                string.Empty,
                (current, value) =>
                {
                    if (value is null)
                    {
                        return current + ",";
                    }

                    if (value is double doubleValue)
                    {
                        return current + doubleValue.ToString("F2", CultureInfo.InvariantCulture) + ",";
                    }

                    if (value is float floatValue)
                    {
                        return current + floatValue.ToString("F2", CultureInfo.InvariantCulture) + ",";
                    }

                    return current + value + ",";
                }).TrimEnd(',');

            await this.streamWriter.WriteAsync(formattedValues);
        }
    }
}