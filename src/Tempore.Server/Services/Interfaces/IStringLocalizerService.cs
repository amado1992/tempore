// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStringLocalizerService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    using System.Globalization;

    using Microsoft.Extensions.Localization;

    /// <summary>
    /// The StringLocalizerService interface.
    /// </summary>
    public interface IStringLocalizerService
    {
        /// <summary>Gets the string resource with the given name.</summary>
        /// <param name="name">The name of the string resource.</param>
        /// <returns>The string resource as a <see cref="T:Microsoft.Extensions.Localization.LocalizedString" />.</returns>
        LocalizedString this[string name] { get; }

        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns>The formatted string resource as a <see cref="T:Microsoft.Extensions.Localization.LocalizedString" />.</returns>
        LocalizedString this[string name, params object[] arguments] { get; }

        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns>The formatted string resource as a <see cref="T:Microsoft.Extensions.Localization.LocalizedString" />.</returns>
        LocalizedString this[string name, CultureInfo cultureInfo, params object[] arguments] { get; }

        /// <summary>Gets all string resources.</summary>
        /// <param name="includeParentCultures">
        /// A <see cref="T:System.Boolean" /> indicating whether to include strings from parent cultures.
        /// </param>
        /// <returns>The strings.</returns>
        IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);

        /// <summary>
        /// Gets an string in specific culture.
        /// </summary>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        /// <param name="name">
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="LocalizedString"/>.
        /// </returns>
        LocalizedString GetString(CultureInfo cultureInfo, string name, params object[] arguments);
    }
}