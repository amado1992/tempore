// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerExtensions.exceptions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Logging.Extensions
{
    using System;

    using Catel;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The logger extensions.
    /// </summary>
    public static partial class LoggerExtensions
    {
        /// <summary>
        /// The log error and create exception.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        /// <typeparam name="TException">
        /// The exception type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Exception"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// When can create the exception.
        /// </exception>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, string message, Exception? innerException = null)
            where TException : Exception
        {
#pragma warning disable CA2254 // Template should be a static expression
            logger.LogError(message);
#pragma warning restore CA2254 // Template should be a static expression

            var exception = ExceptionFactory.CreateException<TException>(message, innerException);
            if (exception is null)
            {
                var error =
                    $"Exception type '{typeof(TException).Name}' does not have a constructor accepting a string";

#pragma warning disable CA2254 // Template should be a static expression
                logger.LogError(error);
#pragma warning restore CA2254 // Template should be a static expression

                throw new NotSupportedException(error);
            }

            return exception;
        }
    }
}