// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSQLApplicationDbContextDesignTimeDbContextFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.PostgreSQL
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;

    using Npgsql;

    using Tempore.Storage;

    /// <summary>
    /// The application db context design time db context factory.
    /// </summary>
    public class PostgreSQLApplicationDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PostgreSQLApplicationDbContext>
    {
        /// <summary>
        /// The create db context.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="ApplicationDbContext"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The expected 'environment' argument is not specified.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The specified environment {environment} is not supported.
        /// </exception>
        public PostgreSQLApplicationDbContext CreateDbContext(string[] args)
        {
            var idx = Array.IndexOf(args, "--environment");
            if (idx >= args.Length - 1 || idx == -1)
            {
                throw new ArgumentException("The expected 'environment' argument is not specified");
            }

            var environment = args[idx + 1];

            string connectionString;
            switch (environment)
            {
                case "tye":
                    var connectionStringBuilder = new NpgsqlConnectionStringBuilder
                    {
                        Port = 5002,
                        Host = "localhost",
                        Database = "tempore",
                        Username = "sa",
                        Password = "tempore-123!",
                    };

                    connectionString = connectionStringBuilder.ToString();
                    break;
                default:
                    throw new NotSupportedException($"The specified environment {environment} is not supported");
            }

            var optionsBuilder = new DbContextOptionsBuilder<PostgreSQLApplicationDbContext>();

            optionsBuilder.UseNpgsql(
                connectionString,
                builder =>
                {
                    builder.MigrationsAssembly(
                        typeof(PostgreSQLApplicationDbContextDesignTimeDbContextFactory).Assembly.GetName().Name);
                });

            var configurationBuilder = new ConfigurationBuilder();
            var initialData = new Dictionary<string, string>
            {
                ["ConnectionStrings:ApplicationDatabase"] = connectionString,
            };
            configurationBuilder.AddInMemoryCollection(initialData);

            return new PostgreSQLApplicationDbContext(NullLogger<PostgreSQLApplicationDbContext>.Instance, configurationBuilder.Build());
        }
    }
}