// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSQLApplicationDbContext.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.PostgreSQL
{
    using EntityFramework.Exceptions.PostgreSQL;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The postgres sql application db context.
    /// </summary>
    public class PostgreSQLApplicationDbContext : ApplicationDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSQLApplicationDbContext"/> class.
        /// </summary>
        /// <param name="logger">
        ///  The logger.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public PostgreSQLApplicationDbContext(ILogger<PostgreSQLApplicationDbContext> logger, IConfiguration configuration)
            : base(logger, configuration)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseExceptionProcessor();
            optionsBuilder.UseNpgsql(this.ConnectionString);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder = modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgreSQLApplicationDbContext).Assembly);
        }
    }
}