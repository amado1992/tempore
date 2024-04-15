// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Roles.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization.Roles
{
    using Tempore.Common.Extensions;

    /// <summary>
    /// The roles.
    /// </summary>
    public static partial class Roles
    {
        /// <summary>
        /// Return all roles.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public static IEnumerable<string> All()
        {
            return typeof(Roles).Constants<string>();
        }
    }
}