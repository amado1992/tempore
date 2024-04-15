// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Routes.analytics.workforce-metrics.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages
{
    /// <summary>
    /// The routes.
    /// </summary>
    public static partial class Routes
    {
        public static partial class Analytics
        {
            /// <summary>
            /// The workload metric.
            /// </summary>
            public const string WorkloadMetricsTemplate = $"{Root}/workload-metrics/{{Id:guid}}";

            /// <summary>
            /// View of employe details.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <returns>
            /// The <see cref="Guid"/>.
            /// </returns>
            public static string WorkloadMetrics(Guid? id)
            {
                return WorkloadMetricsTemplate.Replace("{Id:guid}", id.ToString());
            }
        }
    }
}