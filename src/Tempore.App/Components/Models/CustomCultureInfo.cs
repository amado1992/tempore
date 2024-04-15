// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomCultureInfo.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Components.Models
{
    /// <summary>
    /// The custom culture info.
    /// </summary>
    public class CustomCultureInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCultureInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        public CustomCultureInfo(string name, string displayName)
        {
            this.Name = name;
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }
    }
}