namespace Tempore.Tests.Tempore.App.Dialogs
{
    using System.Linq;

    using AngleSharp.Dom;

    using Blorc.Services;

    using Bunit;

    using FluentAssertions;

    using global::Tempore.App.Dialogs;
    using global::Tempore.App.Services.Interfaces;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using MudBlazor;
    using MudBlazor.Services;

    using Xunit;

    public partial class ConfirmationFacts
    {
        public class The_Confirm_Method
        {
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Calls_The_MudDialogInstanceComponentService_Close_With_True_Value_As_Result()
            {
                // Arrange
                using var ctx = new TestContext();

                ctx.JSInterop.SetupVoid("mudElementRef.saveFocus", _ => true);
                ctx.JSInterop.SetupVoid("mudScrollManager.lockScroll", _ => true);

                ctx.Services.AddMudServices();
                ctx.Services.AddBlorcCore();

                ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");

                using var dialogProvider = ctx.RenderComponent<MudDialogProvider>();
                using var dialog = ctx.RenderComponent<Confirmation>();

                var mudDialogInstanceComponentServiceMock = new Mock<IMudDialogInstanceComponentService>();
                dialog.Instance.MudDialogInstanceComponentService = mudDialogInstanceComponentServiceMock.Object;

                var confirmButton = dialogProvider.FindAll("button").FirstOrDefault(e => e.Text() == "Yes");
                confirmButton.Should().NotBeNull();

                // Act
                confirmButton!.Click();

                // Assert
                mudDialogInstanceComponentServiceMock.Verify(service => service.Close(true));
            }
        }
    }
}