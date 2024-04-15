// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.metrics.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization.Roles
{
    /// <summary>
    /// The roles.
    /// </summary>
    public static partial class Roles
    {
        /// <summary>
        /// The metrics roles.
        /// </summary>
        public static class Metrics
        {
            /// <summary>
            /// The employee administrator.
            /// </summary>
            public const string Administrator = "Metrics Administrator";

            /// <summary>
            /// The viewer.
            /// </summary>
            public const string Viewer = "Metrics Viewer";

            /// <summary>
            /// The calculator.
            /// </summary>
            public const string Calculator = "Metrics Calculator";

            /// <summary>
            /// The exporter.
            /// </summary>
            public const string Exporter = "Metrics Exporter";
        }
    }
}