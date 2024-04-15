// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagedResponse.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests
{
    /// <summary>
    /// The pagination response.
    /// </summary>
    /// <typeparam name="TResult">
    /// The result.
    /// </typeparam>
    public class PagedResponse<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResponse{TResult}"/> class.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        public PagedResponse(int count, IEnumerable<TResult>? items)
        {
            this.Count = count;
            this.Items = items ?? Array.Empty<TResult>();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEnumerable<TResult> Items { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count { get; }
    }
}