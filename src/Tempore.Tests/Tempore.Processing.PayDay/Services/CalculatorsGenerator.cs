namespace Tempore.Tests.Tempore.Processing.PayDay.Services;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using global::Tempore.Processing.PayDay;
using global::Tempore.Processing.PayDay.Services.WorkforceMetricCalculators;
using global::Tempore.Processing.Services.WorkforceMetricCalculators.Interfaces;
using global::Tempore.Tests.Infraestructure;

using Xunit;

public class CalculatorsGenerator
{
    [Fact]
    [Trait(Traits.Category, Category.Development)]
    public async Task GenerateRegistrationAsync()
    {
        var sb = new StringBuilder();
        foreach (var metric in PayDayWorkforceMetrics.All)
        {
            var text = metric.Replace(" ", string.Empty);
            sb.AppendLine($"serviceCollection.AddTransient<{nameof(IWorkforceMetricCalculator)}, {text}{nameof(IWorkforceMetricCalculator).Substring(1)}>();");
        }

        var s = sb.ToString();
    }

    [Fact]
    [Trait(Traits.Category, Category.Development)]
    public async Task GenerateAsync()
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        var directoryInfo = new DirectoryInfo(directory).Parent!.Parent!.Parent!.Parent;
        var directoryInfoFullName = directoryInfo!.FullName;

        var outputDirectory = "Tempore.Processing.PayDay/Services/WorkforceMetricCalculators/";

        var code = @"namespace Tempore.Processing.PayDay.Services.WorkforceMetricCalculators
{{
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The <see cref=""PayDayWorkforceMetrics.{0}""/> processor.
    /// </summary>
    public sealed class {0}WorkforceMetricCalculator : {1}
    {{
        /// <summary>
        /// Initializes a new instance of the <see cref=""{0}WorkforceMetricCalculator""/> class.
        /// </summary>
        /// <param name=""logger"">
        /// The logger.
        /// </param>
        /// <param name=""workforceMetricRepository"">
        /// The workforce metric repository.
        /// </param>
        public {0}WorkforceMetricCalculator(
            ILogger<{0}WorkforceMetricCalculator> logger,
            IRepository<WorkforceMetric, ApplicationDbContext> workforceMetricRepository)
            : base(logger, workforceMetricRepository)
        {{
        }}

        /// <summary>
        /// Gets the workforce metric.
        /// </summary>
        public override string WorkforceMetricName => PayDayWorkforceMetrics.{0};
    }}
}}";

        foreach (var metric in PayDayWorkforceMetrics.All)
        {
            var text = metric.Replace(" ", string.Empty);
            var content = string.Format(code, text, nameof(PayDayWorkforceMetricCalculatorBase));
            var filePath = Path.Combine(directoryInfoFullName, outputDirectory, $"{text}WorkforceMetricCalculator.cs");
            if (!File.Exists(filePath))
            {
                await File.WriteAllTextAsync(
                    filePath,
                    content);
            }
        }
    }

    [Fact]
    [Trait(Traits.Category, Category.Development)]
    public async Task GenerateTestsAsync()
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        var directoryInfo = new DirectoryInfo(directory).Parent!.Parent!.Parent!.Parent;
        var directoryInfoFullName = directoryInfo!.FullName;

        var outputDirectory = "Tempore.Tests/Tempore.Processing.PayDay/Services/WorkforceMetricCalculators/";

        var code = @"namespace Tempore.Tests.Tempore.Processing.PayDay.Services.WorkforceMetricCalculators
{{
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

    public class {0}WorkforceMetricCalculatorFacts
    {{
        public class The_{1}_Method
        {{
            public static IEnumerable<object[]> Data()
            {{
                yield return new object[] {{ new ScheduledDay(), -1.0d }};
            }}

            [Theory]
            [MemberData(nameof(Data))]
            [Trait(Traits.Category, Category.WorkInProgress)]
            public async Task Computes_Expected_Values_Async(ScheduledDay scheduledDay, double expectedValue)
            {{
                // Arrange
                var workforceMetricRepositoryMock = new Mock<IRepository<WorkforceMetric, ApplicationDbContext>>();
                var workforceMetric = new WorkforceMetric
                {{
                    Id = Guid.NewGuid(),
                }};
                workforceMetricRepositoryMock.Setup(repository => repository.SingleOrDefaultAsync(It.IsAny<SearchWorkforceMetricByNameAndCollectionSpecification>())).ReturnsAsync(workforceMetric);
                var workforceMetricCalculator = new {0}WorkforceMetricCalculator(NullLogger<{0}WorkforceMetricCalculator>.Instance, workforceMetricRepositoryMock.Object);

                // Act
                var workforceMetricDailySnapshot = await workforceMetricCalculator.CalculateDailySnapshot(scheduledDay);

                // Assert
                workforceMetricDailySnapshot.Value.Should().Be(expectedValue);
            }}
        }}
    }}
}}";

        foreach (var metric in PayDayWorkforceMetrics.All)
        {
            var text = metric.Replace(" ", string.Empty);
            var content = string.Format(code, text, nameof(IWorkforceMetricCalculator.CalculateDailySnapshot));
            var filePath = Path.Combine(directoryInfoFullName, outputDirectory, $"{text}WorkforceMetricCalculatorFacts.cs");
            if (!File.Exists(filePath))
            {
                await File.WriteAllTextAsync(
                    filePath,
                    content);
            }
        }
    }
}