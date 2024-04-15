// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationDbContext.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services
{
    using Microsoft.EntityFrameworkCore;

    using Tempore.Agent.Entities;

    /// <summary>
    /// The application db context.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ApplicationDbContext> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ApplicationDbContext(ILogger<ApplicationDbContext> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the agents.
        /// </summary>
        public DbSet<Agent>? Agent { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        public DbSet<Device>? Device { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agent.db")}");
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}