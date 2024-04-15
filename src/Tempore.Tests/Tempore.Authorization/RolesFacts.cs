namespace Tempore.Tests.Tempore.Authorization;

using System.Linq;

using global::Tempore.Authorization.Roles;
using global::Tempore.Tests.Infraestructure;

using Xunit;

public class RolesFacts
{
    public class The_All_Method
    {
        [Fact]
        [Trait(Traits.Category, Category.Unit)]

        public void Returns_Non_Repeated_Roles()
        {
            var roles = Roles.All().ToList();
            Assert.Equal(roles.Count, roles.Distinct().Count());
        }

        [Fact]
        [Trait(Traits.Category, Category.Unit)]
        public void Returns_Roles()
        {
            var roles = Roles.All().ToList();
            Assert.NotEmpty(roles);
        }
    }
}