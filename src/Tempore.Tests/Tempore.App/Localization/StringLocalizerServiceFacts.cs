namespace Tempore.Tests.Tempore.App.Localization
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using FluentAssertions;

    using global::Tempore.Tests.Infraestructure;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;

    using TemporeServer::Tempore.Server.Services;
    using TemporeServer::Tempore.Server.Services.Interfaces;

    using Xunit;

    public class StringLocalizerServiceFacts
    {
        public static IEnumerable<object[]> GetServiceProvider_StringLocalizerServiceType_ResourceString_And_CultureInfo()
        {
            var fileExtensions = new[] { "cs", "razor", "razor.cs" };

            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var directoryInfo = new DirectoryInfo(directory).Parent!.Parent!.Parent!.Parent;
            var directoryInfoFullName = directoryInfo!.FullName;

            var regex = new Regex(
                @"StringLocalizer!?\[""([^""]+)""\]",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

            foreach (var supportedCulture in SupportedCultures.All)
            {
                var types = typeof(global::Tempore.App.App).Assembly.GetTypes().Where(type => typeof(ComponentBase).IsAssignableFrom(type)).ToList();
                foreach (Type type in types)
                {
                    var assemblyName = type.Assembly.GetName().Name;
                    var relativeNamespace = string.IsNullOrWhiteSpace(type.Namespace) ? string.Empty : type.Namespace!.Substring(assemblyName!.Length).TrimStart('.');
                    var fileNameWithoutExtension = Path.Combine(directoryInfoFullName, assemblyName!, relativeNamespace.Replace('.', Path.DirectorySeparatorChar), type.Name);

                    var processedResourceStrings = new HashSet<string>();
                    foreach (var fileExtension in fileExtensions)
                    {
                        var file = $"{fileNameWithoutExtension}.{fileExtension}";
                        if (File.Exists(file))
                        {
                            var matchCollection = regex.Matches(File.ReadAllText(file));
                            foreach (Match match in matchCollection)
                            {
                                var serviceCollection = new ServiceCollection();

                                serviceCollection
                                    .AddLogging()
                                    .AddLocalization(options => options.ResourcesPath = "Resources");

                                var stringLocalizerServiceType = typeof(IStringLocalizerService<>).MakeGenericType(type);
                                var stringLocalizerImplementationType = typeof(StringLocalizerService<>).MakeGenericType(type);
                                serviceCollection.AddSingleton(stringLocalizerServiceType, stringLocalizerImplementationType);

                                var resourceString = match.Groups[1].Value;
                                if (!processedResourceStrings.Contains(resourceString))
                                {
                                    yield return new object[]
                                                 {
                                                     serviceCollection,
                                                     stringLocalizerServiceType,
                                                     resourceString,
                                                     supportedCulture,
                                                 };
                                }
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetServiceProvider_StringLocalizerServiceType_ResourceString_And_Cultures()
        {
            var fileExtensions = new[] { "cs", "razor", "razor.cs" };

            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var directoryInfo = new DirectoryInfo(directory).Parent!.Parent!.Parent!.Parent;
            var directoryInfoFullName = directoryInfo!.FullName;

            var regex = new Regex(
                @"StringLocalizer!?\[""([^""]+)""\]",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

            var resourceStringsToBeIgnored = new Dictionary<string, HashSet<string>>();
            for (var i = 0; i < SupportedCultures.All.Count; i++)
            {
                var supportedCultureI = SupportedCultures.All[i];
                for (var j = i + 1; j < SupportedCultures.All.Count; j++)
                {
                    var supportedCultureJ = SupportedCultures.All[j];
                    resourceStringsToBeIgnored[$"{supportedCultureI.Name}-{supportedCultureJ.Name}"] = resourceStringsToBeIgnored[$"{supportedCultureJ.Name}-{supportedCultureI.Name}"] = new HashSet<string>();
                }
            }

            //// TODO: Update ignore list if required like this.
            //// ignoresList[$"{SupportedCultures.EnglishCultureInfo.Name}-{SupportedCultures.SpanishCultureInfo.Name}"].Add("Alcohol");
            //// ignoresList[$"{SupportedCultures.EnglishCultureInfo.Name}-{SupportedCultures.SpanishCultureInfo.Name}"].Add("Agenda");
            //// ignoresList[$"{SupportedCultures.EnglishCultureInfo.Name}-{SupportedCultures.SpanishCultureInfo.Name}"].Add("Angel");

            for (var i = 0; i < SupportedCultures.All.Count; i++)
            {
                var supportedCultureI = SupportedCultures.All[i];
                for (var j = i + 1; j < SupportedCultures.All.Count; j++)
                {
                    var supportedCultureJ = SupportedCultures.All[j];

                    var types = typeof(global::Tempore.App.App).Assembly.GetTypes().Where(type => typeof(ComponentBase).IsAssignableFrom(type)).ToList();
                    foreach (Type type in types)
                    {
                        var assemblyName = type.Assembly.GetName().Name;
                        var relativeNamespace = string.IsNullOrWhiteSpace(type.Namespace)
                                                    ? string.Empty
                                                    : type.Namespace!.Substring(assemblyName!.Length).TrimStart('.');
                        var fileNameWithoutExtension = Path.Combine(
                            directoryInfoFullName,
                            assemblyName!,
                            relativeNamespace.Replace('.', Path.DirectorySeparatorChar),
                            type.Name);

                        var processedResourceStrings = new HashSet<string>();
                        foreach (var fileExtension in fileExtensions)
                        {
                            var file = $"{fileNameWithoutExtension}.{fileExtension}";
                            if (File.Exists(file))
                            {
                                var matchCollection = regex.Matches(File.ReadAllText(file));
                                foreach (Match match in matchCollection)
                                {
                                    var serviceCollection = new ServiceCollection();

                                    serviceCollection.AddLogging().AddLocalization(options => options.ResourcesPath = "Resources");

                                    var stringLocalizerServiceType = typeof(IStringLocalizerService<>).MakeGenericType(type);
                                    var stringLocalizerImplementationType = typeof(StringLocalizerService<>).MakeGenericType(type);
                                    serviceCollection.AddSingleton(stringLocalizerServiceType, stringLocalizerImplementationType);

                                    var resourceString = match.Groups[1].Value;
                                    var key = $"{supportedCultureI.Name}-{supportedCultureJ.Name}";
                                    if (!resourceStringsToBeIgnored[key].Contains(resourceString) && !processedResourceStrings.Contains(resourceString))
                                    {
                                        yield return new object[]
                                                     {
                                                         serviceCollection,
                                                         stringLocalizerServiceType,
                                                         resourceString,
                                                         supportedCultureI,
                                                         supportedCultureJ,
                                                     };

                                        processedResourceStrings.Add(resourceString);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetServiceProvider_StringLocalizerServiceType_ResourceString_And_CultureInfo))]
        [Trait(Traits.Category, Category.Unit)]
        public void Resolves_LocalizedString_Successfully(IServiceCollection serviceCollection, Type stringLocalizerServiceType, string resourceStringName, CultureInfo cultureInfo)
        {
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var stringLocalizerService = serviceProvider.GetRequiredService(stringLocalizerServiceType).As<IStringLocalizerService>();

            var localizedString = stringLocalizerService[resourceStringName, cultureInfo];

            localizedString.ResourceNotFound.Should().BeFalse($"The required resource string '{resourceStringName}' not found in Culture '{cultureInfo.Name}' for '{stringLocalizerService.GetType().FullName}'");
        }

        [Theory]
        [MemberData(nameof(GetServiceProvider_StringLocalizerServiceType_ResourceString_And_CultureInfo))]
        [Trait(Traits.Category, Category.Unit)]
        public void Resolved_LocalizedString_Must_Have_A_Value(IServiceCollection serviceCollection, Type stringLocalizerServiceType, string resourceStringName, CultureInfo cultureInfo)
        {
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var stringLocalizerService = serviceProvider.GetRequiredService(stringLocalizerServiceType).As<IStringLocalizerService>();
            var localizedString = stringLocalizerService[resourceStringName, cultureInfo];

            localizedString.Value.Should().NotBeNullOrEmpty($"The required resource string '{resourceStringName}' doesn't have a value in Culture '{cultureInfo.Name}' for '{stringLocalizerService.GetType().FullName}'");
        }

        [Theory]
        [MemberData(nameof(GetServiceProvider_StringLocalizerServiceType_ResourceString_And_Cultures))]
        [Trait(Traits.Category, Category.Unit)]
        public void Resolves_LocalizedString_Must_Have_Different_Value_In_Different_Cultures(IServiceCollection serviceCollection, Type stringLocalizerServiceType, string resourceStringName, CultureInfo cultureInfoI, CultureInfo cultureInfoJ)
        {
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var stringLocalizerService = serviceProvider.GetRequiredService(stringLocalizerServiceType).As<IStringLocalizerService>();

            var localizedStringI = stringLocalizerService[resourceStringName, cultureInfoI];
            var localizedStringJ = stringLocalizerService[resourceStringName, cultureInfoJ];

            localizedStringI.Value.Should().NotBe(
                localizedStringJ.Value,
                $"The resource string '{resourceStringName}' have the same value '{localizedStringI.Value}' in '{cultureInfoI.Name}' and '{cultureInfoJ.Name}' cultures for '{stringLocalizerService.GetType().FullName}'");
        }
    }
}