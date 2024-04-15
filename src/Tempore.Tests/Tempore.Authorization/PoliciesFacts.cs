namespace Tempore.Tests.Tempore.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Tempore.Authorization.Extensions;
    using global::Tempore.Common.Extensions;
    using global::Tempore.Tests.Infraestructure;

    using Xunit;

    using Policies = global::Tempore.Authorization.Policies.Policies;
    using Roles = global::Tempore.Authorization.Roles.Roles;

    public class PoliciesFacts
    {
        public class The_RoleBasedPolicies_Method
        {
            [Theory]
            [InlineData(typeof(Policies), typeof(Roles))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_A_List_With_Role_Based_Policies_For_Every_Existing_Role(Type policiesConstantType, Type rolesConstantType)
            {
                var policyInfos = policiesConstantType.RoleBasedPolicies().ToList();

                var methodInfo = rolesConstantType.GetMethod("All", BindingFlags.Static | BindingFlags.Public);
                var roles = methodInfo?.Invoke(rolesConstantType, Array.Empty<object>()) as IEnumerable<string> ?? Array.Empty<string>();

                foreach (var role in roles)
                {
                    var policyInfo = policyInfos.FirstOrDefault(info => info.Name == role);
                    Assert.True(policyInfo != null, $"{role} is not used as policy");
                }
            }

            [Theory]
            [InlineData(typeof(Policies))]
            [Trait(Traits.Category, Category.Unit)]
            public void CompositeRoles_Have_Unique_Roles(Type policiesConstantType)
            {
                var roleBasedPolicyInfos = policiesConstantType.RoleBasedPolicies().ToList();
                foreach (var roleBasedPolicyInfo in roleBasedPolicyInfos)
                {
                    Assert.Equal(roleBasedPolicyInfo.Roles.Count, roleBasedPolicyInfo.Roles.Distinct().Count());
                }
            }

            [Theory]
            [InlineData(typeof(Policies))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_A_List_With_Items_Count_Equals_To_SortedRoleBasePolicies_Items_Count(Type policiesConstantType)
            {
                var roleBasedPolicies = policiesConstantType.RoleBasedPolicies().ToList();
                var sortedRoleBasedPolicies = policiesConstantType.SortedRoleBasedPolicies().ToList();

                Assert.Equal(sortedRoleBasedPolicies.Count, roleBasedPolicies.Count);
            }

            [Theory]
            [InlineData(typeof(Policies))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_Same_Items_Of_SortedRoleBasePolicies(Type policiesConstantType)
            {
                var roleBasedPolicies = policiesConstantType.RoleBasedPolicies().ToList();
                var sortedRoleBasedPolicies = policiesConstantType.SortedRoleBasedPolicies().ToList();

                foreach (var roleBasedPolicy in roleBasedPolicies)
                {
                    Assert.Contains(roleBasedPolicy, sortedRoleBasedPolicies);
                }
            }
        }

        public class The_SortedRoleBasedPolicies_Method
        {
            [Theory]
            [InlineData(typeof(Policies), typeof(Roles))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_A_List_With_Role_Based_Policies_For_Every_Existing_Role(Type policiesConstantType, Type rolesConstantType)
            {
                var policyInfos = policiesConstantType.SortedRoleBasedPolicies().ToList();

                var methodInfo = rolesConstantType.GetMethod("All", BindingFlags.Static | BindingFlags.Public);
                var roles = methodInfo?.Invoke(rolesConstantType, Array.Empty<object>()) as IEnumerable<string> ?? Array.Empty<string>();
                foreach (var role in roles)
                {
                    var policyInfo = policyInfos.FirstOrDefault(info => info.Name == role);
                    Assert.True(policyInfo is not null, role);
                }
            }

            [Theory]
            [InlineData(typeof(Policies))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_Same_Items_Of_RoleBasePolicies(Type policiesConstantType)
            {
                var roleBasedPolicies = policiesConstantType.RoleBasedPolicies().ToList();
                var sortedRoleBasedPolicies = policiesConstantType.SortedRoleBasedPolicies().ToList();

                foreach (var roleBasedPolicy in sortedRoleBasedPolicies)
                {
                    Assert.Contains(roleBasedPolicy, roleBasedPolicies);
                }
            }

            [Theory]
            [InlineData(typeof(Policies))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_A_List_With_Items_Count_Equals_To_RoleBasePolicies_Items_Count(Type policiesConstantType)
            {
                var roleBasedPolicies = policiesConstantType.RoleBasedPolicies().ToList();
                var sortedRoleBasedPolicies = policiesConstantType.SortedRoleBasedPolicies().ToList();
                Assert.Equal(roleBasedPolicies.Count, sortedRoleBasedPolicies.Count);
            }

            [Theory]
            [InlineData(typeof(Policies))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_Roles_Sorted_By_Dependencies(Type policiesConstantType)
            {
                var roleBasedPolicies = policiesConstantType.SortedRoleBasedPolicies().ToList();
                for (var idx = 0; idx < roleBasedPolicies.Count; idx++)
                {
                    var roleBasedPolicy = roleBasedPolicies[idx];
                    foreach (var role in roleBasedPolicy.Roles)
                    {
                        int dependencyIndex = roleBasedPolicies.FindIndex(info => info.Name == role);
                        Assert.True(dependencyIndex < idx);
                    }
                }
            }
        }
    }
}