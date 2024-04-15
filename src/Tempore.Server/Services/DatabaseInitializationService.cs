// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseInitializationService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.HealthChecks.Services.Interfaces;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Extensions;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The database initialization service.
    /// </summary>
    public class DatabaseInitializationService : IDatabaseInitializationService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DatabaseInitializationService> logger;

        /// <summary>
        /// The environment.
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The database health check service factory.
        /// </summary>
        private readonly IDatabaseHealthCheckServiceFactory databaseHealthCheckServiceFactory;

        private readonly IEnumerable<IWorkforceMetricSeeder> workforceMetricSeeders;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseInitializationService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="environment">
        /// The environment.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <param name="databaseHealthCheckServiceFactory">
        /// The database health check factory.
        /// </param>
        /// <param name="workforceMetricSeeders">
        /// The workforce metric seeders.
        /// </param>
        public DatabaseInitializationService(
            ILogger<DatabaseInitializationService> logger,
            IWebHostEnvironment environment,
            IServiceProvider serviceProvider,
            IDatabaseHealthCheckServiceFactory databaseHealthCheckServiceFactory,
            IEnumerable<IWorkforceMetricSeeder> workforceMetricSeeders)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(environment);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(databaseHealthCheckServiceFactory);
            ArgumentNullException.ThrowIfNull(workforceMetricSeeders);

            this.logger = logger;
            this.environment = environment;
            this.serviceProvider = serviceProvider;
            this.databaseHealthCheckServiceFactory = databaseHealthCheckServiceFactory;
            this.workforceMetricSeeders = workforceMetricSeeders;
        }

        /// <summary>
        /// Gets a value indicating whether the service is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initializes a database.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            if (this.environment.IsSwaggerGen())
            {
                return;
            }

            await this.MigrateAsync();
            await this.SeedAsync();

            this.IsInitialized = true;
        }

        /// <summary>
        /// The wait async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task WaitAsync()
        {
            while (!this.IsInitialized)
            {
                await Task.Delay(1000);
            }
        }

        private async Task MigrateAsync()
        {
            this.logger.LogInformation("Migrating database");

            using var serviceScope = this.serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var databaseHealthCheckService = this.databaseHealthCheckServiceFactory.Create(applicationDbContext.Database);
            await databaseHealthCheckService.WaitAsync();

            await applicationDbContext.Database.MigrateAsync();

            this.logger.LogInformation("Database migrated");
        }

        private async Task SeedAsync()
        {
            this.logger.LogInformation("Seeding database");

            foreach (var workforceMetricSeeder in this.workforceMetricSeeders)
            {
                await workforceMetricSeeder.SeedAsync();
            }

            await this.SeedDefaultDataAsync();
        }

        private async Task SeedDefaultDataAsync()
        {
            using var serviceScope = this.serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork<ApplicationDbContext>>();

            var timetableRepository = unitOfWork.GetRepository<Timetable>();

            var weekdaysTimetable = await timetableRepository.SingleOrDefaultAsync(t => t.Name == DefaultValues.WeekdaysTimetableName);
            if (weekdaysTimetable is null)
            {
                weekdaysTimetable = new Timetable
                {
                    Name = DefaultValues.WeekdaysTimetableName,
                    StartTime = new TimeSpan(8, 0, 0),
                    Duration = new TimeSpan(9, 0, 0),
                    EffectiveWorkingTime = new TimeSpan(8, 0, 0),
                    CheckInTimeStart = new TimeSpan(0, -15, 0),
                    CheckInTimeDuration = new TimeSpan(0, 30, 0),
                    CheckOutTimeStart = new TimeSpan(0, -15, 0),
                    CheckOutTimeDuration = new TimeSpan(0, 30, 0),
                };

                timetableRepository.Add(weekdaysTimetable);
                await timetableRepository.SaveChangesAsync();
            }

            var saturdayTimetable = await timetableRepository.SingleOrDefaultAsync(t => t.Name == DefaultValues.SaturdayTimetableName);
            if (saturdayTimetable is null)
            {
                saturdayTimetable = new Timetable
                {
                    Name = DefaultValues.SaturdayTimetableName,
                    StartTime = new TimeSpan(8, 0, 0),
                    Duration = new TimeSpan(5, 0, 0),
                    EffectiveWorkingTime = new TimeSpan(5, 0, 0),
                    CheckInTimeStart = new TimeSpan(0, -15, 0),
                    CheckInTimeDuration = new TimeSpan(0, 30, 0),
                    CheckOutTimeStart = new TimeSpan(0, -15, 0),
                    CheckOutTimeDuration = new TimeSpan(0, 30, 0),
                };

                timetableRepository.Add(saturdayTimetable);
                await timetableRepository.SaveChangesAsync();
            }

            var shiftRepository = unitOfWork.GetRepository<Shift>();
            var shift = await shiftRepository.SingleOrDefaultAsync(shift => shift.Name == DefaultValues.DefaultShiftName);
            if (shift is null)
            {
                shift = new Shift
                {
                    Name = DefaultValues.DefaultShiftName,
                };

                shiftRepository.Add(shift);
                await shiftRepository.SaveChangesAsync();

                var dayRepository = unitOfWork.GetRepository<Day>();
                for (var dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
                {
                    var day = new Day
                    {
                        Index = (int)dayOfWeek,
                        ShiftId = shift.Id,
                    };

                    switch (dayOfWeek)
                    {
                        case > DayOfWeek.Sunday and < DayOfWeek.Saturday:
                            day.TimetableId = weekdaysTimetable.Id;
                            break;
                        case DayOfWeek.Saturday:
                            day.TimetableId = saturdayTimetable.Id;
                            break;
                    }

                    dayRepository.Add(day);
                }

                await dayRepository.SaveChangesAsync();
            }
        }
    }
}