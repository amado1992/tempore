namespace Tempore.Tests.Tempore.Processing.PayDay.Services.WorkforceMetricCalculators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Common.Extensions;
    using global::Tempore.Processing.PayDay;
    using global::Tempore.Processing.PayDay.Services;
    using global::Tempore.Processing.PayDay.Services.WorkforceMetricCalculators;
    using global::Tempore.Processing.Specs;
    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Xunit;

    public class JorOrdDiurnaWorkforceMetricCalculatorFacts
    {
        public class The_CalculateDailySnapshot_Method
        {
            public static IEnumerable<object[]> Data()
            {
                var scheduledDayDate = DateOnly.FromDateTime(DateTime.Now);

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(5, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(9, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(15, 51, 0)),
                                                      },
                                                  },
                                 },
                                 0.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(20, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(9, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(15, 51, 0)),
                                                      },
                                                  },
                                 },
                                 0.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                  },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 0.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                  },
                                     WorkforceMetricConflictResolutions = new List<WorkforceMetricConflictResolution>
                                     {
                                         new WorkforceMetricConflictResolution
                                         {
                                             WorkforceMetric = new WorkforceMetric
                                                               {
                                                                   Name = PayDayWorkforceMetrics.JorOrdDiurna,
                                                                   WorkforceMetricCollection = new WorkforceMetricCollection
                                                                       {
                                                                           Name = PayDayWorkforceMetricCollection.Name,
                                                                       },
                                                               },
                                             Value = 8.0d,
                                         },
                                     },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 8.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                  },
                                     WorkforceMetricConflictResolutions = new List<WorkforceMetricConflictResolution>
                                     {
                                         new WorkforceMetricConflictResolution
                                         {
                                             WorkforceMetric = new WorkforceMetric
                                                               {
                                                                   Name = PayDayWorkforceMetrics.JorOrdDiurna,
                                                                   WorkforceMetricCollection = new WorkforceMetricCollection
                                                                       {
                                                                           Name = PayDayWorkforceMetricCollection.Name,
                                                                       },
                                                               },
                                             Value = 4.0d,
                                         },
                                         new WorkforceMetricConflictResolution
                                         {
                                             WorkforceMetric = new WorkforceMetric
                                                               {
                                                                   Name = PayDayWorkforceMetrics.JorOrdDiurna,
                                                                   WorkforceMetricCollection = new WorkforceMetricCollection
                                                                       {
                                                                           Name = PayDayWorkforceMetricCollection.Name,
                                                                       },
                                                               },
                                             Value = 4.0d,
                                         },
                                     },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 8.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                  },
                                     WorkforceMetricConflictResolutions = new List<WorkforceMetricConflictResolution>
                                     {
                                         new WorkforceMetricConflictResolution
                                         {
                                             WorkforceMetric = new WorkforceMetric
                                                               {
                                                                   Name = PayDayWorkforceMetrics.JorOrdDiurna,
                                                                   WorkforceMetricCollection = new WorkforceMetricCollection
                                                                       {
                                                                           Name = PayDayWorkforceMetricCollection.Name,
                                                                       },
                                                               },
                                             Value = 4.0d,
                                         },
                                         new WorkforceMetricConflictResolution
                                         {
                                             WorkforceMetric = new WorkforceMetric
                                                               {
                                                                   Name = PayDayWorkforceMetrics.AusenciaNOpagada,
                                                                   WorkforceMetricCollection = new WorkforceMetricCollection
                                                                       {
                                                                           Name = PayDayWorkforceMetricCollection.Name,
                                                                       },
                                                               },
                                             Value = 4.0d,
                                         },
                                     },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 4.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                  },
                                     WorkforceMetricConflictResolutions = new List<WorkforceMetricConflictResolution>
                                     {
                                         new WorkforceMetricConflictResolution
                                         {
                                             WorkforceMetric = new WorkforceMetric
                                                               {
                                                                   Name = PayDayWorkforceMetrics.AusenciaNOpagada,
                                                                   WorkforceMetricCollection = new WorkforceMetricCollection
                                                                       {
                                                                           Name = PayDayWorkforceMetricCollection.Name,
                                                                       },
                                                               },
                                             Value = 8.0d,
                                         },
                                     },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 0.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(9, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(15, 51, 0)),
                                                      },
                                                  },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 8.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(6, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(9, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(15, 51, 0)),
                                                      },
                                                  },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 8.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(6, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(9, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(17, 51, 0)),
                                                      },
                                                  },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 8.0d,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(9, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(15, 51, 0)),
                                                      },
                                                  },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 new TimeSpan(16, 0, 0).Subtract(new TimeSpan(9, 10, 0)).TotalHours,
                            };

                yield return new object[]
                            {
                                 new ScheduledDay
                                 {
                                     Date = scheduledDayDate,
                                     StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                                     EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                                     CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                                     CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                                     CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                                     CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                                     Timestamps = new List<Timestamp>
                                                  {
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(9, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                                      },
                                                      new Timestamp
                                                      {
                                                          DateTime = scheduledDayDate.Add(new TimeSpan(15, 38, 0)),
                                                      },
                                                  },
                                     EffectiveWorkingTime = TimeSpan.FromHours(8),
                                     RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
                                 },
                                 new TimeSpan(15, 38, 0).Subtract(new TimeSpan(9, 10, 0)).TotalHours,
                            };

                yield return new object[]
                {
                     new ScheduledDay
                     {
                         Date = scheduledDayDate,
                         StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                         EndDateTime = scheduledDayDate.Add(new TimeSpan(16, 0, 0)),
                         CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                         CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                         CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(15, 45, 0)),
                         CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(16, 15, 0)),
                         Timestamps = new List<Timestamp>
                                      {
                                          new Timestamp
                                          {
                                              DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                          },
                                          new Timestamp
                                          {
                                              DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                          },
                                          new Timestamp
                                          {
                                              DateTime = scheduledDayDate.Add(new TimeSpan(16, 10, 0)),
                                          },
                                      },
                         EffectiveWorkingTime = TimeSpan.FromHours(8),
                         RelativeEffectiveWorkingTime = TimeSpan.FromHours(8.69067),
                     },
                     8.69067,
                };

                yield return new object[]
                {
                     new ScheduledDay
                     {
                         Date = scheduledDayDate,
                         StartDateTime = scheduledDayDate.Add(new TimeSpan(8, 0, 0)),
                         EndDateTime = scheduledDayDate.Add(new TimeSpan(17, 0, 0)),
                         CheckInStartDateTime = scheduledDayDate.Add(new TimeSpan(7, 45, 0)),
                         CheckInEndDateTime = scheduledDayDate.Add(new TimeSpan(8, 15, 0)),
                         CheckOutStartDateTime = scheduledDayDate.Add(new TimeSpan(16, 45, 0)),
                         CheckOutEndDateTime = scheduledDayDate.Add(new TimeSpan(17, 15, 0)),
                         Timestamps = new List<Timestamp>
                                      {
                                          new Timestamp
                                          {
                                              DateTime = scheduledDayDate.Add(new TimeSpan(8, 10, 0)),
                                          },
                                          new Timestamp
                                          {
                                              DateTime = scheduledDayDate.Add(new TimeSpan(14, 10, 0)),
                                          },
                                          new Timestamp
                                          {
                                              DateTime = scheduledDayDate.Add(new TimeSpan(17, 10, 0)),
                                          },
                                      },
                         EffectiveWorkingTime = TimeSpan.FromHours(8),
                         RelativeEffectiveWorkingTime = TimeSpan.FromHours(8.69067),
                     },
                     8.69067,
                };
            }

            [Theory]
            [MemberData(nameof(Data))]
            [Trait(Traits.Category, Category.Unit)]
            [Trait(Traits.Category, Category.WorkInProgress)]
            public async Task Computes_Expected_Values_Async(ScheduledDay scheduledDay, double expectedValue)
            {
                // Arrange
                var workforceMetricRepositoryMock = new Mock<IRepository<WorkforceMetric, ApplicationDbContext>>();
                var workforceMetric = new WorkforceMetric
                {
                    Id = Guid.NewGuid(),
                };

                if (scheduledDay.WorkforceMetricConflictResolutions is { } workforceMetricConflictResolutions)
                {
                    foreach (var workforceMetricConflictResolution in workforceMetricConflictResolutions)
                    {
                        if (workforceMetricConflictResolution.WorkforceMetric is { Name: PayDayWorkforceMetrics.JorOrdDiurna, WorkforceMetricCollection.Name: PayDayWorkforceMetricCollection.Name })
                        {
                            workforceMetricConflictResolution.WorkforceMetricId = workforceMetric.Id;
                        }
                    }
                }

                workforceMetricRepositoryMock.Setup(repository => repository.SingleOrDefaultAsync(It.IsAny<SearchWorkforceMetricByNameAndCollectionSpecification>())).ReturnsAsync(workforceMetric);
                var workforceMetricCalculator = new JorOrdDiurnaWorkforceMetricCalculator(NullLogger<JorOrdDiurnaWorkforceMetricCalculator>.Instance, workforceMetricRepositoryMock.Object);

                // Act
                var workforceMetricDailySnapshot = await workforceMetricCalculator.CalculateDailySnapshot(scheduledDay);

                // Assert
                workforceMetricDailySnapshot.Value.Should().BeApproximately(expectedValue, 0.001);
            }
        }
    }
}