// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringLocalizerService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using System.Globalization;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The string localizer service.
    /// </summary>
    /// <typeparam name="T">
    /// The type.
    /// </typeparam>
    public class StringLocalizerService<T> : IStringLocalizerService<T>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<StringLocalizerService<T>> logger;

        /// <summary>
        /// The srtring localizer.
        /// </summary>
        private readonly IStringLocalizer<T> stringLocalizer;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLocalizerService{T}"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public StringLocalizerService(ILogger<StringLocalizerService<T>> logger, IStringLocalizer<T> stringLocalizer, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(stringLocalizer);
            ArgumentNullException.ThrowIfNull(serviceProvider);

            this.logger = logger;
            this.stringLocalizer = stringLocalizer;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public LocalizedString this[string name] => this.stringLocalizer[name];

        /// <inheritdoc />
        public LocalizedString this[string name, params object[] arguments] => this.stringLocalizer[name, arguments];

        /// <inheritdoc />
        public LocalizedString this[string name, CultureInfo cultureInfo, params object[] arguments] => this.GetString(cultureInfo, name, arguments);

        /// <inheritdoc />
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.stringLocalizer.GetAllStrings(includeParentCultures);
        }

        /// <inheritdoc />
        public LocalizedString GetString(CultureInfo cultureInfo, string name, params object[] arguments)
        {
            return Task.Run(
                () =>
                {
                    var currentThreadCurrentCulture = Thread.CurrentThread.CurrentCulture;
                    var currentThreadCurrentUiCulture = Thread.CurrentThread.CurrentUICulture;

                    Thread.CurrentThread.CurrentCulture = cultureInfo;
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;

                    try
                    {
                        var localizer = (IStringLocalizer)this.serviceProvider.GetRequiredService(typeof(IStringLocalizer<>).MakeGenericType(typeof(T)));
                        return localizer[name, arguments];
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentThreadCurrentCulture;
                        Thread.CurrentThread.CurrentUICulture = currentThreadCurrentUiCulture;
                    }
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}