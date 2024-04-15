namespace Tempore.Tests.Tempore.Processing.PayDay.Extensions
{
    using System.Linq;

    using FluentAssertions;

    using global::Tempore.Processing.PayDay;
    using global::Tempore.Processing.PayDay.Extensions;
    using global::Tempore.Processing.Services.WorkforceMetricCalculators.Interfaces;
    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Xunit;

    public class ServiceCollectionExtensionsFacts
    {
        public class The_AddPayDay_Method
        {
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Registers_All_Expected_Processors()
            {
                var serviceCollection = new ServiceCollection();
                var mock = new Mock<IRepository<WorkforceMetric, ApplicationDbContext>>();

                serviceCollection.AddLogging();
                serviceCollection.AddScoped(_ => mock.Object);
                serviceCollection.AddPayDay();

                using var buildServiceProvider = serviceCollection.BuildServiceProvider();
                var workforceMetricCalculators = buildServiceProvider.GetServices<IWorkforceMetricCalculator>().ToList();

                PayDayWorkforceMetrics.All.Should().AllSatisfy(metricName => workforceMetricCalculators.Any(calculator => calculator.WorkforceMetricName == metricName).Should().BeTrue());
            }
        }
    }
}