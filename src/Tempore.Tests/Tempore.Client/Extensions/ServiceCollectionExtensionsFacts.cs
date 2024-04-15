namespace Tempore.Tests.Tempore.Client.Extensions
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using global::Tempore.Client.Extensions;
    using global::Tempore.Client.Services.Interfaces;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    public class ServiceCollectionExtensionsFacts
    {
        public class The_AddTemporeHttpClients_Method
        {
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Registers_All_Services_As_Expected_Using_The_HttpClient_ServiceProvider_Overload()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddTemporeHttpClients((_, _) => { });

                var httpClientInterfaceType = typeof(IHttpClient);
                var assembly = httpClientInterfaceType.Assembly;
                var types = assembly.GetTypes().ToList();

                var count = types.Count(type => httpClientInterfaceType.IsAssignableFrom(type) && type.IsInterface && type != httpClientInterfaceType);
                serviceCollection.Count(descriptor => descriptor.ServiceType.Assembly == assembly).Should().Be(count);
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Registers_All_Services_As_Expected_Using_The_HttpClient_Only_Overload()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddTemporeHttpClients(
                    _ =>
                    {
                    });

                var httpClientInterfaceType = typeof(IHttpClient);
                var assembly = httpClientInterfaceType.Assembly;
                var types = assembly.GetTypes().ToList();

                var count = types.Count(type => httpClientInterfaceType.IsAssignableFrom(type) && type.IsInterface && type != httpClientInterfaceType);

                serviceCollection.Count(descriptor => descriptor.ServiceType.Assembly == assembly).Should().Be(count);
            }
        }
    }
}