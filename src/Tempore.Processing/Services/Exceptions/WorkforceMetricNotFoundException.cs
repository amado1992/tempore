// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricNotFoundException.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.Exceptions
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The workforce metric not found exception.
    /// </summary>
    public class WorkforceMetricNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkforceMetricNotFoundException"/> class.
        /// </summary>
        public WorkforceMetricNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkforceMetricNotFoundException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public WorkforceMetricNotFoundException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkforceMetricNotFoundException"/> class.
        /// </summary>
        /// <param name="innerException">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public WorkforceMetricNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkforceMetricNotFoundException"/> class.
        /// </summary>
        /// <param name="context">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected WorkforceMetricNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}