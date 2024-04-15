namespace Tempore.Tests.Infraestructure
{
    using System.Collections.Immutable;
    using System.Globalization;

    /// <summary>
    /// The supported cultures.
    /// </summary>
    public static class SupportedCultures
    {
        /// <summary>
        /// The english.
        /// </summary>
        public const string English = "en";

        /// <summary>
        /// The spanish.
        /// </summary>
        public const string Spanish = "es";

        /// <summary>
        /// The english culture info.
        /// </summary>
        public static readonly CultureInfo EnglishCultureInfo = new CultureInfo(English);

        /// <summary>
        /// The spanish culture info.
        /// </summary>
        public static readonly CultureInfo SpanishCultureInfo = new CultureInfo(Spanish);

        /// <summary>
        /// The all.
        /// </summary>
        public static readonly IImmutableList<CultureInfo> All = ImmutableList.Create(EnglishCultureInfo, SpanishCultureInfo);
    }
}