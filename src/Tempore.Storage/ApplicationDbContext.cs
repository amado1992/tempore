// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationDbContext.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Tempore.Storage.Entities;

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
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public ApplicationDbContext(ILogger<ApplicationDbContext> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.ConnectionString = configuration.GetConnectionString("ApplicationDatabase");
        }

        /// <summary>
        /// Gets or sets the employees from devices.
        /// </summary>
        public DbSet<EmployeeFromDevice>? EmployeesFromDevices { get; set; }

        /// <summary>
        /// Gets or sets the agents.
        /// </summary>
        public DbSet<Agent>? Agents { get; set; }

        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        public DbSet<Device>? Devices { get; set; }

        /// <summary>
        /// Gets or sets the data files.
        /// </summary>
        public DbSet<DataFile>? DataFiles { get; set; }

        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        public DbSet<Employee>? Employees { get; set; }

        /// <summary>
        /// Gets or sets the timetables.
        /// </summary>
        public DbSet<Timetable>? Timetables { get; set; }

        /// <summary>
        /// Gets or sets the shifts.
        /// </summary>
        public DbSet<Shift>? Shifts { get; set; }

        /// <summary>
        /// Gets or sets the scheduled shift assignments.
        /// </summary>
        public DbSet<ScheduledShiftAssignment>? ScheduledShiftAssignments { get; set; }

        /// <summary>
        /// Gets or sets the shifts.
        /// </summary>
        public DbSet<ScheduledShift>? ScheduledShifts { get; set; }

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        public DbSet<Day>? Days { get; set; }

        /// <summary>
        /// Gets or sets the scheduled days.
        /// </summary>
        public DbSet<ScheduledDay>? ScheduledDays { get; set; }

        /// <summary>
        /// Gets or sets the workforce metrics.
        /// </summary>
        public DbSet<WorkforceMetric>? WorkforceMetrics { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric collections.
        /// </summary>
        public DbSet<WorkforceMetricCollection>? WorkforceMetricCollections { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric daily snapshots.
        /// </summary>
        public DbSet<WorkforceMetricDailySnapshot>? WorkforceMetricDailySnapshots { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric conflict resolutions.
        /// </summary>
        public DbSet<WorkforceMetricConflictResolution>? WorkforceMetricConflictResolutions { get; set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        protected string ConnectionString { get; }

        /// <summary>
        /// The on model creating.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}