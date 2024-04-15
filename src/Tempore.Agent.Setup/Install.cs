// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Install.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Setup
{
    using System.CommandLine;
    using System.CommandLine.NamingConventionBinder;
    using System.Diagnostics;
    using System.IO.Compression;

    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    using Serilog;

    using Tempore.Agent.Setup.Logs;

    /// <summary>
    /// The install class.
    /// </summary>
    public static class Install
    {
        /// <summary>
        /// The create install command.
        /// </summary>
        /// <returns>
        /// The <see cref="Command"/>.
        /// </returns>
        public static Command CreateInstallCommand()
        {
            var logger = Log.ForContext(typeof(Install));
            var command = new Command("install", "Install Tempore Agent");

            command.AddOption(
                new Option<string>(new[] { "--directory", "-d" })
                {
                    IsRequired = true,
                });

            command.AddOption(
                new Option<string>(new[] { "--source", "-s" }, () => "https://nexus.tempore.io/repository/nuget/")
                {
                    IsRequired = false,
                });

            command.AddOption(
                new Option<string>(new[] { "--version", "-v" })
                {
                    IsRequired = false,
                });

            command.Handler = CommandHandler.Create(
                async (string directory, string source, string version) =>
                {
                    logger.Information("Installing Tempore.Agent in '{Directory}' from '{Source}'", directory, source);

                    var packageSource = new PackageSource(source);
                    var sourceRepository = Repository.Factory.GetCoreV3(packageSource);

                    var packageId = "Tempore.Agent";
                    var serilogLogger = new SerilogLogger(logger);

                    var findPackageByIdResource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>();
                    var versions = await findPackageByIdResource.GetAllVersionsAsync(
                                       packageId,
                                       NullSourceCacheContext.Instance,
                                       serilogLogger,
                                       CancellationToken.None);

                    NuGetVersion? versionToInstall;
                    if (string.IsNullOrWhiteSpace(version))
                    {
                        versionToInstall = versions.LastOrDefault();
                        if (versionToInstall is null)
                        {
                            logger.Error("There is not available version of 'Tempore.Agent' in '{Source}'", source);
                            return;
                        }
                    }
                    else
                    {
                        var package = new PackageDependency(packageId, VersionRange.Parse(version));
                        versionToInstall = package.VersionRange.FindBestMatch(versions);
                    }

                    logger.Information("Installing Service 'Tempore.Agent' - {Version}", versionToInstall);

                    using var downloader = await findPackageByIdResource.GetPackageDownloaderAsync(
                                               new PackageIdentity(packageId, versionToInstall),
                                               NullSourceCacheContext.Instance,
                                               serilogLogger,
                                               CancellationToken.None);

                    var cacheDirectory = Path.Combine(directory, "cache", "Tempore.Agent", versionToInstall?.OriginalVersion!);
                    if (!Directory.Exists(cacheDirectory))
                    {
                        Directory.CreateDirectory(cacheDirectory);
                    }

                    logger.Information("Stoping Service 'Tempore.Agent'");
                    var stopServiceProcessStartInfo = new ProcessStartInfo("sc.exe", "stop Tempore.Agent")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    };
                    using var stopServiceProcess = Process.Start(stopServiceProcessStartInfo);
                    if (stopServiceProcess is not null)
                    {
                        await stopServiceProcess.WaitForExitAsync();
                        logger.Information(await stopServiceProcess.StandardOutput.ReadToEndAsync());
                    }

                    var destinationFilePath = Path.Combine(
                        cacheDirectory,
                        $"{packageId}.{versionToInstall.OriginalVersion}.nupkg");
                    if (await downloader.CopyNupkgFileToAsync(destinationFilePath, CancellationToken.None))
                    {
                        ZipFile.ExtractToDirectory(destinationFilePath, cacheDirectory, true);

                        var contentDirectory = Path.Combine(cacheDirectory, "content");
                        var files = Directory.GetFiles(contentDirectory, "*.*", SearchOption.AllDirectories);

                        foreach (var file in files)
                        {
                            var relativePath = Path.GetRelativePath(contentDirectory, file);
                            var newFile = Path.Combine(directory, relativePath);
                            if (Path.GetFileName(file).Equals(
                                    "appsettings.json",
                                    StringComparison.InvariantCultureIgnoreCase) && File.Exists(newFile))
                            {
                                logger.Warning(
                                    "Configuration file '{NewFile}' already exists. Please check the file '{OriginalFile}' to update the configuration if necessary.",
                                    newFile,
                                    file);
                            }
                            else
                            {
                                var directoryName = Path.GetDirectoryName(newFile);
                                if (!Directory.Exists(directoryName))
                                {
                                    Directory.CreateDirectory(directoryName!);
                                }

                                logger.Information("Copying file '{NewFile}'", newFile);

                                File.Copy(file, newFile, true);
                            }
                        }
                    }

                    logger.Information("Creating Service 'Tempore.Agent'");
                    var installServiceProcessStartInfo = new ProcessStartInfo(
                        "sc.exe",
                        $"create \"Tempore.Agent\" binpath=\"{Path.Combine(directory, "Tempore.Agent.exe")}\"")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    };
                    using var installProcess = Process.Start(installServiceProcessStartInfo);
                    if (installProcess is not null)
                    {
                        await installProcess.WaitForExitAsync();
                        logger.Information(await installProcess.StandardOutput.ReadToEndAsync());
                    }

                    logger.Information("Starting Service 'Tempore.Agent'");
                    var startServiceProcessStartInfo = new ProcessStartInfo("sc.exe", "start Tempore.Agent")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    };
                    using var startProcess = Process.Start(startServiceProcessStartInfo);
                    if (startProcess is not null)
                    {
                        await startProcess.WaitForExitAsync();
                        logger.Information(await startProcess.StandardOutput.ReadToEndAsync());
                    }

                    logger.Information("Installed Service 'Tempore.Agent'");

                    logger.Warning("If 'Tempore.Agent' Service is not started successfully fix the configuration and start the service manually.");
                });

            return command;
        }
    }
}