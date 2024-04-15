// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Routes.analytics.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages
{
    using System.Globalization;

    /// <summary>
    /// The routes.
    /// </summary>
    public static partial class Routes
    {
        /// <summary>
        /// The Analytics.
        /// </summary>
        public static partial class Analytics
        {
            /// <summary>
            /// The root.
            /// </summary>
            public const string Root = "/analytics";

            /// <summary>
            /// The workforce metrics.
            /// </summary>
            public const string WorkforceMetrics = $"{Root}/workforce-metrics";

            /// <summary>
            /// The workforce metrics with date range.
            /// </summary>
            public const string WorkforceMetricsTemplate = $"{Root}/workforce-metrics/{{StartDate:datetime}}/{{EndDate:datetime}}";

            /// <summary>
            /// Gets the workforce metrics page url with date range parameters.
            /// </summary>
            /// <param name="startDate">
            /// The start date.
            /// </param>
            /// <param name="endDate">
            /// The end date.
            /// </param>
            /// <returns>
            /// The url.
            /// </returns>
            public static string ComputeWorkforceMetrics(DateTime startDate, DateTime endDate)
            {
                return WorkforceMetricsTemplate
                    .Replace("{StartDate:datetime}", startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                    .Replace("{EndDate:datetime}", endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
        }
    }
}