namespace Tempore.Tests.Infraestructure.Extensions
{
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// The thread extensions.
    /// </summary>
    public static class ThreadExtensions
    {
        /// <summary>
        /// The set culture.
        /// </summary>
        /// <param name="thread">
        /// The thread.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        public static void SetCulture(this Thread thread, string culture)
        {
            var cultureInfo = new CultureInfo(culture, false);
            thread.SetCulture(cultureInfo);
        }

        /// <summary>
        /// The set culture.
        /// </summary>
        /// <param name="thread">
        /// The thread.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        /// <param name="uICultureInfo">
        /// The u i culture info.
        /// </param>
        public static void SetCulture(this Thread thread, CultureInfo cultureInfo, CultureInfo? uICultureInfo = null)
        {
            thread.CurrentCulture = cultureInfo;
            thread.CurrentUICulture = uICultureInfo ?? cultureInfo;
        }
    }
}