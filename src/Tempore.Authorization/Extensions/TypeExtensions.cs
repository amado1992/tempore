// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Authorization.Extensions
{
    using Tempore.Common.Extensions;

    /// <summary>
    /// The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The role based policies.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{RoleBasedPolicyInfo}"/>.
        /// </returns>
        public static IEnumerable<RoleBasedPolicyInfo> RoleBasedPolicies(this Type type)
        {
            return type.Constants<RoleBasedPolicyInfo>();
        }

        /// <summary>
        /// The sorted role based policies.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{RoleBasedPolicyInfo}"/>.
        /// </returns>
        public static IEnumerable<RoleBasedPolicyInfo> SortedRoleBasedPolicies(this Type type)
        {
            var pendingRoleBasedPolicies = new Stack<(int RoleIdx, RoleBasedPolicyInfo Policy)>();
            var roleBasedPolicies = type.RoleBasedPolicies().ToList();
            do
            {
                if (pendingRoleBasedPolicies.TryPeek(out var peek))
                {
                    bool found = false;
                    int count = peek.Policy.Roles.Count;
                    if (peek.RoleIdx < count)
                    {
                        for (int roleIdx = peek.RoleIdx; roleIdx < count; roleIdx++)
                        {
                            peek.RoleIdx = roleIdx + 1;
                            string role = peek.Policy.Roles[roleIdx];
                            int idx = roleBasedPolicies.FindIndex(info => info.Name == role);
                            if (idx >= 0)
                            {
                                pendingRoleBasedPolicies.Push((0, roleBasedPolicies[idx]));
                                roleBasedPolicies.RemoveAt(idx);
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        yield return pendingRoleBasedPolicies.Pop().Policy;
                    }
                }
                else if (roleBasedPolicies.Count > 0)
                {
                    pendingRoleBasedPolicies.Push((0, roleBasedPolicies[0]));
                    roleBasedPolicies.RemoveAt(0);
                }
            }
            while (pendingRoleBasedPolicies.Any() || roleBasedPolicies.Any());
        }
    }
}