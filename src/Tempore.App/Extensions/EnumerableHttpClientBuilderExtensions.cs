// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableHttpClientBuilderExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Extensions;

using Blorc.OpenIdConnect;

/// <summary>
/// The enumerable http client builder extensions.
/// </summary>
public static class EnumerableHttpClientBuilderExtensions
{
    /// <summary>
    /// The add access token.
    /// </summary>
    /// <param name="clientBuilders">
    /// The client builders.
    /// </param>
    public static void AddAccessToken(this IEnumerable<IHttpClientBuilder> clientBuilders)
    {
        foreach (var httpClientBuilder in clientBuilders)
        {
            httpClientBuilder.AddAccessToken();
        }
    }
}