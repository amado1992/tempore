// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessTokenDelegatingHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services
{
    using System.Net.Http.Headers;

    using Tempore.Agent.Services.Interfaces;

    /// <summary>
    /// The access token delegating handler.
    /// </summary>
    public class AccessTokenDelegatingHandler : DelegatingHandler
    {
        /// <summary>
        /// The token resolver.
        /// </summary>
        private readonly ITokenResolver tokenResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenDelegatingHandler"/> class.
        /// </summary>
        /// <param name="tokenResolver">
        /// The token resolver.
        /// </param>
        public AccessTokenDelegatingHandler(ITokenResolver tokenResolver)
        {
            this.tokenResolver = tokenResolver;
        }

        /// <summary>
        /// The send async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await this.tokenResolver.ResolveAsync());

            return await base.SendAsync(request, cancellationToken);
        }
    }
}