// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseHealthCheckServiceBase.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.HealthChecks.Services
{
    using System.Data;
    using System.Data.Common;

    using Microsoft.Extensions.Logging;

    using Tempore.Infrastructure.HealthChecks.Interfaces;

    /// <summary>
    /// The DatabaseHealthCheckServiceBase class.
    /// </summary>
    public abstract class DatabaseHealthCheckServiceBase : HealthCheckServiceBase, IDatabaseHealthCheckService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DatabaseHealthCheckServiceBase> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHealthCheckServiceBase"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        protected DatabaseHealthCheckServiceBase(ILogger<DatabaseHealthCheckServiceBase> logger, string connectionString)
        {
            this.logger = logger;
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        protected string ConnectionString { get; }

        /// <inheritdoc />
        public override async Task<bool> IsHealthyAsync()
        {
            await using var connection = this.CreateConnection();
            try
            {
                await connection.OpenAsync(CancellationToken.None);
            }
            catch (Exception)
            {
                // _logger.LogWarning(
                //    ex,
                //    $"Database is not ready '{ConnectionString}'",
                //    connection.GetPasswordProtectedConnectionString());
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a database connection.
        /// </summary>
        /// <returns>
        /// The <see cref="DbConnection"/>.
        /// </returns>
        public abstract DbConnection CreateConnection();
    }
}