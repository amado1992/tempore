// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policies.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization.Policies
{
    using Tempore.Authorization;
    using Tempore.Common.Extensions;

    /// <summary>
    /// The policies.
    /// </summary>
    public static partial class Policies
    {
        /// <summary>
        /// Gets all policies.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{PolicyInfo}"/>.
        /// </returns>
        public static IEnumerable<PolicyInfo> All()
        {
            return typeof(Policies).Constants<PolicyInfo>();
        }
    }
}