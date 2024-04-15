// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogLogger.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Setup.Logs
{
    using NuGet.Common;

    /// <summary>
    /// The Serilog logger.
    /// </summary>
    internal class SerilogLogger : ILogger
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SerilogLogger(Serilog.ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Logs debug.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void LogDebug(string data)
        {
            this.logger.Debug(data);
        }

        /// <summary>
        /// Logs verbose.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void LogVerbose(string data)
        {
            this.logger.Verbose(data);
        }

        /// <summary>
        /// Logs information.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void LogInformation(string data)
        {
            this.logger.Information(data);
        }

        /// <summary>
        /// Logs minimal.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void LogMinimal(string data)
        {
            this.logger.Verbose(data);
        }

        /// <summary>
        /// Logs warning.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void LogWarning(string data)
        {
            this.logger.Warning(data);
        }

        /// <summary>
        /// Logs error.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void LogError(string data)
        {
            this.logger.Error(data);
        }

        /// <summary>
        /// Logs information summary.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void LogInformationSummary(string data)
        {
            this.logger.Information(data);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Minimal:
                case LogLevel.Debug:
                    this.LogDebug(data);
                    break;
                case LogLevel.Error:
                    this.LogError(data);
                    break;
                case LogLevel.Information:
                    this.LogInformation(data);
                    break;
                case LogLevel.Warning:
                    this.LogWarning(data);
                    break;
                case LogLevel.Verbose:
                    this.LogVerbose(data);
                    break;
            }
        }

        /// <summary>
        /// The log async.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// It's not implemented.
        /// </exception>
        public Task LogAsync(LogLevel level, string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// It's not implemented.
        /// </exception>
        public void Log(ILogMessage message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The log async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// It's not implemented.
        /// </exception>
        public Task LogAsync(ILogMessage message)
        {
            throw new NotImplementedException();
        }
    }
}