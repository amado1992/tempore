// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DarkTheme.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Themes
{
    using MudBlazor;

    /// <summary>
    /// The dark theme.
    /// </summary>
    public class DarkTheme : MudTheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DarkTheme"/> class.
        /// </summary>
        public DarkTheme()
        {
            this.Palette = new PaletteDark
            {
                Primary = Colors.Dark.Primary,
                Secondary = Colors.Dark.Secondary,
                Tertiary = Colors.Dark.Tertiary,
                Success = Colors.Dark.Primary,
                Black = "#27272f",
                Background = Colors.Dark.Background,
                BackgroundGrey = "#27272f",
                Surface = Colors.Dark.Surface,
                DrawerBackground = Colors.Dark.DrawerBackground,
                DrawerText = "rgba(255,255,255, 0.50)",
                AppbarBackground = Colors.Dark.AppbarBackground,
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "rgba(255,255,255, 0.50)",
                TableLines = "#e0e0e036",
                Dark = Colors.Dark.DrawerBackground,
                Divider = "#e0e0e036",
                OverlayDark = "hsl(0deg 0% 0% / 75%)",
                TextDisabled = Colors.Dark.Disabled,
            };

            this.LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "5px",
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