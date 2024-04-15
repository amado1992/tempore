// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightTheme.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Themes
{
    using MudBlazor;

    /// <summary>
    /// The light theme.
    /// </summary>
    public class LightTheme : MudTheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightTheme"/> class.
        /// </summary>
        public LightTheme()
        {
            this.Palette = new PaletteLight
            {
                Primary = Colors.Light.Primary,
                Secondary = Colors.Light.Secondary,
                Tertiary = Colors.Light.Tertiary,
                Background = Colors.Light.Background,
                AppbarBackground = Colors.Light.AppbarBackground,
                AppbarText = Colors.Light.AppbarText,
                DrawerBackground = Colors.Light.Background,
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = Colors.Light.Primary,
                TableLines = "#e0e0e029",
                OverlayDark = "hsl(0deg 0% 0% / 75%)",
            };

            this.LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "5px",
                DrawerWidthLeft = "300px",
            };

            this.Typography = Themes.Typography.TemporeTypography;
            this.Shadows = new Shadow();
            this.ZIndex = new ZIndex
            {
                Drawer = 1300,
            };
        }
    }
}