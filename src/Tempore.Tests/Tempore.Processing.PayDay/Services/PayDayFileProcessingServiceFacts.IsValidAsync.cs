namespace Tempore.Tests.Tempore.Processing.PayDay.Services
{
    extern alias TemporeServer;

    using System.Collections.Generic;
    using System.IO;
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
        public class The_IsValidAsync_Method
        {
            public static IEnumerable<object[]> Files()
            {
                var assembly = typeof(PayDayFileProcessingServiceFacts).Assembly;
                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees.xlsx"),
                                 },
                                 true,
                             };

                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees-missing-optional-columns.xlsx"),
                                 },
                                 true,
                             };

                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees-missing-optional-columns-exchanged-columns-positions.xlsx"),
                                 },
                                 true,
                             };

                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees-missing-fullname.xlsx"),
                                 },
                                 false,
                             };

                yield return new object[]
                             {
                                 new DataFile
                                 {
                                     Data = assembly.GetFileContent("Tempore.Tests.Resources.employees-missing-code.xlsx"),
                                 },
                                 false,
                             };
            }

            [Theory]
            [MemberData(nameof(Files))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Returns_Expected_Value_Async(DataFile dataFile, bool isValidExpectedValue)
            {
                var payDayFileProcessingService = new PayDayFileProcessingService(NullLogger<PayDayFileProcessingService>.Instance);

                using var memoryStream = new MemoryStream(dataFile.Data);
                var isValid = await payDayFileProcessingService.IsValidAsync(memoryStream);

                isValid.Should().Be(isValidExpectedValue);
            }
        }
    }
}