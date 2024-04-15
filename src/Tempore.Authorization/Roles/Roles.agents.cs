// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.agents.cs" company="Port Hope Investment S.A.">
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
        /// The agents.
        /// </summary>
        public static class Agents
        {
            /// <summary>
            /// The administrator.
            /// </summary>
            public const string Administrator = "Agents Administrator";

            /// <summary>
            /// The editor.
            /// </summary>
            public const string Editor = "Agents Editor";

            /// <summary>
            /// The viewer.
            /// </summary>
            public const string Viewer = "Agents Viewer";

            /// <summary>
            /// The operator.
            /// </summary>
            public const string Operator = "Agents Operator";
        }
    }
}