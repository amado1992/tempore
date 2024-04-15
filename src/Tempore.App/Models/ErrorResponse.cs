// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorResponse.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Models
{
    /// <summary>
    /// Class to deserialize the JSON.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the detail.
        /// </summary>
        public string Detail { get; set; } = string.Empty;
    }
}