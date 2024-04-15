// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Severity.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client.Services.Interfaces
{
    /// <summary>
    /// The severity.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// The normal severity.
        /// </summary>
        Normal,

        /// <summary>
        /// The information severity.
        /// </summary>
        Information,

        /// <summary>
        /// The success severity.
        /// </summary>
        Success,

        /// <summary>
        /// The warning severity.
        /// </summary>
        Warning,

        /// <summary>
        /// The error severity.
        /// </summary>
        Error,
    }
}