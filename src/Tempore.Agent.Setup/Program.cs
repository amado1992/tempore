// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Setup
{
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Parsing;

    using Microsoft.Extensions.Configuration;

    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main async.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<int> Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel
                .Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configurationFile = Path.Combine(currentDirectory, "appsettings.json");
            if (File.Exists(configurationFile))
            {
                var configuration = new ConfigurationBuilder().AddJsonFile(configurationFile, false, true).Build();
                loggerConfiguration = loggerConfiguration.ReadFrom.Configuration(configuration);
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            var logger = Log.ForContext(typeof(Program));

            var command = new RootCommand
            {
                Description = "Tempore Agent Setup Tool.",
            };

            command.AddCommand(Install.CreateInstallCommand());

            var builder = new CommandLineBuilder(command);
            builder.UseExceptionHandler(
                (exception, context) =>
                {
                    logger.Error(exception, "Error executing command '{command}'.", context.ParseResult.CommandResult.Token.Value);
                });

            builder.UseHelp();
            builder.UseVersionOption();
            builder.UseParseErrorReporting();
            builder.CancelOnProcessTermination();

            var parser = builder.Build();
            return parser.InvokeAsync(args);
        }
    }
}