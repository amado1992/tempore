namespace Tempore.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect;

    using FluentAssertions;

    using Tempore.App.Extensions;
    using Tempore.Tests.Infraestructure;

    using Xunit;

    public class ProfileExtensionsFacts
    {
        public class The_GetInitials_Method
        {
            public static IEnumerable<object[]> Data()
            {
                yield return new object[] { "Igr Alexánder Fernández Saúco", -1, "IAFS" };
                yield return new object[] { "Yircy    , Diem Collazo   Marín  ", -1, "YDCM" };
                yield return new object[] { "  Igr, Alexánder Fernández Saúco", 2, "IA" };
                yield return new object[] { " ,  Yircy   Diem ,  Collazo   Marín  ", 2, "YD" };
            }

            [Theory]
            [MemberData(nameof(Data))]
            [Trait(Traits.Category, Category.Unit)]
            public void Returns_Initials_As_Expected(string name, int maxLength, string expectedInitials)
            {
                var profile = new Profile { Name = name };

                profile.GetInitials(maxLength).Should().Be(expectedInitials);
            }
        }
    }
}