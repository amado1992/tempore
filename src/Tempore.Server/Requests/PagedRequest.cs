// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagedRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests
{
    using MediatR;

    /// <summary>
    /// The paged request.
    /// </summary>
    /// <typeparam name="TResult">
    /// The entity type.
    /// </typeparam>
    public class PagedRequest<TResult> : IRequest<PagedResponse<TResult>>
    {
        /// <summary>
        /// Gets or sets the skip.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int Take { get; set; }
    }

    /// <summary>
    /// The paged request.
    /// </summary>
    /// <typeparam name="TSearchParams">
    /// The filter type.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The result type.
    /// </typeparam>
    public class PagedRequest<TSearchParams, TResult> : PagedRequest<TResult>
    {
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        public TSearchParams? SearchParams { get; set; }
    }
}