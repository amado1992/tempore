// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaginationOptions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs
{
    /// <summary>
    /// The pagination options.
    /// </summary>
    public class PaginationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationOptions"/> class.
        /// </summary>
        /// <param name="skip">
        ///     The skip.
        /// </param>
        /// <param name="take">
        ///     The take.
        /// </param>
        /// <param name="isEnable">
        ///     The is enable.
        /// </param>
        public PaginationOptions(int skip, int take, bool isEnable = true)
        {
            this.Skip = skip;
            this.Take = take;
            this.IsEnable = isEnable;
        }

        /// <summary>
        /// Gets the skip.
        /// </summary>
        public int Skip { get; }

        /// <summary>
        /// Gets the take.
        /// </summary>
        public int Take { get; }

        /// <summary>
        /// Gets or sets a value indicating whether is enable.
        /// </summary>
        public bool IsEnable { get; set; }
    }
}