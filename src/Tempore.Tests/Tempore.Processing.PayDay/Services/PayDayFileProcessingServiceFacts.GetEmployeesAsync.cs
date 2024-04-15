namespace Tempore.Tests.Tempore.Processing.PayDay.Services
{
    extern alias TemporeServer;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Processing.PayDay.Services;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Infraestructure;
    using global::Tempore.Tests.Infraestructure.Extensions;

    using Microsoft.Extensions.Logging.Abstractions;

    using Xunit;

    public partial class PayDayFileProcessingServiceFacts
    {
        public class The_GetEmployeesAsync_Method
        {
            public static IEnumerable<object[]> ValidFiles()
            {
                var assembly = typeof(PayDayFileProcessingServiceFacts).Assembly;
                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees.xlsx"),
                                 },
                             };

                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees-missing-optional-columns.xlsx"),
                                 },
                             };

                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees-missing-optional-columns-exchanged-columns-positions.xlsx"),
                                 },
                             };
            }

            [Theory]
            [MemberData(nameof(ValidFiles))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Returns_Employees_With_Required_Fields_From_Valid_Files_Async(DataFile dataFile)
            {
                var payDayFileProcessingService = new PayDayFileProcessingService(NullLogger<PayDayFileProcessingService>.Instance);

                var employees = await payDayFileProcessingService.GetEmployeesAsync(dataFile).ToListAsync();
                employees.Should().AllSatisfy(
                    employee =>
                    {
                        employee.FullName.Should().NotBeEmpty();
                        employee.ExternalId.Should().NotBeEmpty();
                    });

                employees.Count.Should().BeGreaterThan(0);
            }
        }
    }
}