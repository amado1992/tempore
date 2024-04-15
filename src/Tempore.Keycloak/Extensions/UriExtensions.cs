// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UriExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Keycloak.Extensions
{
    /// <summary>
    /// The uri extensions.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Gets root url.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetRootUrl(this Uri uri)
        {
            var result = $"{uri.Scheme}://{uri.Host}";
            if (!uri.IsDefaultPort)
            {
                result += ":" + uri.Port;
            }

            return result;
        }
    }
}