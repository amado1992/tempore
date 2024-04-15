// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderDescription.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Components
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Header description.
    /// </summary>
    public partial class HeaderDescription
    {
        /// <summary>
        /// Gets or sets content text.
        /// </summary>
        [Parameter]
        public string ContentText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        [Parameter]
        public string Icon { get; set; } = string.Empty;
    }
}