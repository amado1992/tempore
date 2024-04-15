// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeClient.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client
{
    using Newtonsoft.Json;

    /// <summary>
    /// The employee client.
    /// </summary>
    public partial class EmployeeClient
    {
        /// <summary>
        /// Updates json serializer settings.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}