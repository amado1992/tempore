namespace Tempore.Tests.Tempore.Server.Services
{
    extern alias TemporeServer;

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Fixtures;
    using global::Tempore.Tests.Fixtures.Postgres;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using TemporeServer::Tempore.Server.Services;

    using Xunit;

    public class DatabaseInitializationServiceFacts
    {
        [Collection(nameof(DockerCollection))]
        public class The_InitializeAsync_Method : EnvironmentTestBase
        {
            public The_InitializeAsync_Method(
                DockerEnvironment dockerEnvironment,
                TemporeServerWebApplicationFactory temporeServerApplicationFactory,
                TestEnvironmentInitializer testEnvironmentInitializer)
                : base(dockerEnvironment, testEnvironmentInitializer, temporeServerApplicationFactory, null)
            {
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Seeds_The_Default_Shift_Async()
            {
                var shiftRepository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<Shift, ApplicationDbContext>>();

                var shift = await shiftRepository.SingleOrDefaultAsync(new ShiftSpecByName(DefaultValues.DefaultShiftName));

                var indexes = new System.Collections.Generic.List<int>();
                shift.Should().NotBeNull();
                shift!.Days.Should().NotBeNullOrEmpty();
                shift.Days.Should().AllSatisfy(
                    day =>
                    {
                        day.Id.Should().NotBeEmpty();
                        if (day.Index is >= 1 and <= 5)
                        {
                            day.Timetable.Should().NotBeNull();
                            day.Timetable!.Id.Should().NotBeEmpty();
                            day.Timetable.Name.Should().Be(DefaultValues.WeekdaysTimetableName);
                        }
                        else if (day.Index == 6)
                        {
                            day.Timetable.Should().NotBeNull();
                            day.Timetable!.Id.Should().NotBeEmpty();
                            day.Timetable.Name.Should().Be(DefaultValues.SaturdayTimetableName);
                        }
                        else
                        {
                            day.Timetable.Should().BeNull();
                        }

                        indexes.Should().NotContain(day.Index);
                        indexes.Add(day.Index);
                    });

                for (int i = 0; i < indexes.Count - 1; i++)
                {
                    indexes[i].Should().BeLessThan(indexes[i + 1]);
                }
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Seeds_WorkforceMetricCollection_Async()
            {
                var workforceMetricCollectionRepository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<WorkforceMetricCollection, ApplicationDbContext>>();

                workforceMetricCollectionRepository.All().Count().Should().BeGreaterThan(0);
            }

            [Fact]
            [Trait(Traits.Category, Category.Integration)]
            public async Task Seeds_WorkforceMetrics_Foreach_Registered_Collections_Async()
            {
                var workforceMetricCollectionRepository = this.TemporeServerApplicationFactory!.Services
                    .GetRequiredService<IRepository<WorkforceMetricCollection, ApplicationDbContext>>();

                var metricCollections = await workforceMetricCollectionRepository.All().Include(collection => collection.WorkforceMetrics).ToListAsync();
                metricCollections.Should().AllSatisfy(collection => collection.WorkforceMetrics.Count.Should().BeGreaterThan(0));
            }
        }

        public class ShiftSpecByName : ISpecification<Shift>
        {
            private readonly string name;

            public ShiftSpecByName(string name)
            {
                this.name = name;
            }

            public Func<IQueryable<Shift>, IQueryable<Shift>> Build()
            {
                return shifts => shifts
                    .Include(shift => shift.Days.OrderBy(day => day.Index))
                    .ThenInclude(journey => journey.Timetable)
                    .Where(shift => shift.Name == this.name);
            }
        }
    }
}