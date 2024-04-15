// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Routes.employees.import-from-devices.cs" company="Port Hope Investment S.A.">
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
        /// <summary>
        /// The employees.
        /// </summary>
        public static partial class Employees
        {
            /// <summary>
            /// The import from devices.
            /// </summary>
            public static class ImportFromDevices
            {
                /// <summary>
                /// The agents.
                /// </summary>
                public const string Agents = $"{Root}/agents";

                /// <summary>
                /// View agent devices template.
                /// </summary>
                public const string AgentDevicesTemplate = $"{Agents}/agent-devices/{{AgentId:guid}}";

                /// <summary>
                /// View agent devices.
                /// </summary>
                /// <param name="id">
                /// The id.
                /// </param>
                /// <returns>
                /// The <see cref="string"/>.
                /// </returns>
                public static string ViewAgentDevices(Guid id)
                {
                    return AgentDevicesTemplate.Replace("{AgentId:guid}", id.ToString());
                }
            }
        }
    }
}