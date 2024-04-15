namespace Tempore.Tests.Tempore.Processing.PayDay.Services.WorkforceMetricCalculators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Processing.PayDay.Services.WorkforceMetricCalculators;
    using global::Tempore.Processing.Specs;
    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Xunit;

    public class JorOrdDiuDiaFerWorkforceMetricCalculatorFacts
    {
        public class The_CalculateDailySnapshot_Method
        {
            public static IEnumerable<object[]> Data()
            {
                yield return new object[] { new ScheduledDay(), -1.0d };
            }

            [Theory]
            [MemberData(nameof(Data))]
            [Trait(Traits.Category, Category.WorkInProgress)]
            public async Task Computes_Expected_Values_Async(ScheduledDay scheduledDay, double expectedValue)
            {
                // Arrange
                var workforceMetricRepositoryMock = new Mock<IRepository<WorkforceMetric, ApplicationDbContext>>();
                var workforceMetric = new WorkforceMetric
                {
                    Id = Guid.NewGuid(),
                };
                workforceMetricRepositoryMock.Setup(repository => repository.SingleOrDefaultAsync(It.IsAny<SearchWorkforceMetricByNameAndCollectionSpecification>())).ReturnsAsync(workforceMetric);
                var workforceMetricCalculator = new JorOrdDiuDiaFerWorkforceMetricCalculator(NullLogger<JorOrdDiuDiaFerWorkforceMetricCalculator>.Instance, workforceMetricRepositoryMock.Object);

                // Act
                var workforceMetricDailySnapshot = await workforceMetricCalculator.CalculateDailySnapshot(scheduledDay);

                // Assert
                workforceMetricDailySnapshot.Value.Should().Be(expectedValue);
            }
        }
    }
}