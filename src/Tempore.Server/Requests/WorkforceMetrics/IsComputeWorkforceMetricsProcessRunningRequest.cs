// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsComputeWorkforceMetricsProcessRunningRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.WorkforceMetrics
{
    using MediatR;

    /// <summary>
    /// The IsComputeWorkforceMetricsProcessRunningRequest.
    /// </summary>
    public class IsComputeWorkforceMetricsProcessRunningRequest : IRequest<bool>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IsComputeWorkforceMetricsProcessRunningRequest Instance = new IsComputeWorkforceMetricsProcessRunningRequest();

        private IsComputeWorkforceMetricsProcessRunningRequest()
        {
        }
    }
}