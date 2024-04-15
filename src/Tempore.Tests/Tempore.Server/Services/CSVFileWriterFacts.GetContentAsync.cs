namespace Tempore.Tests.Tempore.Server.Services
{
    extern alias TemporeServer;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Tests.Infraestructure;

    using TemporeServer::Tempore.Server.Services;

    using Xunit;

    public partial class CSVFileWriterFacts
    {
        public class The_GetContentAsync_Method
        {
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Returns_The_Expected_Content_Async()
            {
                await using var fileWriter = new CsvFileContentWriter();
                var values = new List<object> { Guid.NewGuid(), 99.7d, 0.0d };
                await fileWriter.WriteAsync(values);
                await fileWriter.WriteLineAsync();
                await fileWriter.WriteLineAsync(values);

                await fileWriter.FlushAsync();

                var count = 0;
                var bytes = await fileWriter.GetContentAsync();
                await using var memoryStream = new MemoryStream(bytes);
                using StreamReader reader = new StreamReader(memoryStream);
                while (await reader.ReadLineAsync() is { })
                {
                    count++;
                }

                count.Should().Be(2);
            }
        }
    }
}