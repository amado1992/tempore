// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Colors.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Themes
{
    using System.Collections.Immutable;

    /// <summary>
    /// The custom colors.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// The theme colors.
        /// </summary>
        public static readonly ImmutableList<string> ThemeColors = new List<string>
                                                                   {
                                                                       Light.Primary,
                                                                       MudBlazor.Colors.Blue.Default,
                                                                       MudBlazor.Colors.BlueGrey.Default,
                                                                       MudBlazor.Colors.Purple.Default,
                                                                       MudBlazor.Colors.Orange.Default,
                                                                       MudBlazor.Colors.Red.Default,
                                                                       MudBlazor.Colors.Amber.Default,
                                                                       MudBlazor.Colors.DeepPurple.Default,
                                                                       MudBlazor.Colors.Pink.Default,
                                                                       MudBlazor.Colors.Indigo.Default,
                                                                       MudBlazor.Colors.LightBlue.Default,
                                                                       MudBlazor.Colors.Cyan.Default,
                                                                   }.ToImmutableList();

        /// <summary>
        /// The light.
        /// </summary>
        public static class Light
        {
            /// <summary>
            /// The primary.
            /// </summary>
            public const string Primary = "#003A5D";

            /// <summary>
            /// The secondary.
            /// </summary>
            public const string Secondary = "#2196f3";

            /// <summary>
            /// The tertiary.
            /// </summary>
            public const string Tertiary = "#2196f3";

            /// <summary>
            /// The background.
            /// </summary>
            public const string Background = "#FFF";

            /// <summary>
            /// The appbar background.
            /// </summary>
            public const string AppbarBackground = "#FFF";

            /// <summary>
            /// The appbar text.
            /// </summary>
            public const string AppbarText = "#6E6E6E";
        }

        /// <summary>
        /// The dark.
        /// </summary>
        public static class Dark
        {
            /// <summary>
            /// The primary.
            /// </summary>
            public const string Primary = "#003A5D";

            /// <summary>
            /// The secondary.
            /// </summary>
            public const string Secondary = "#2196f3";

            /// <summary>
            /// The tertiary.
            /// </summary>
            public const string Tertiary = "#2196f3";

            /// <summary>
            /// The background.
            /// </summary>
            public const string Background = "#1b1f22";

            /// <summary>
            /// The appbar background.
            /// </summary>
            public const string AppbarBackground = "#1b1f22";

            /// <summary>
            /// The drawer background.
            /// </summary>
            public const string DrawerBackground = "#121212";

            /// <summary>
            /// The surface.
            /// </summary>
            public const string Surface = "#202528";

            /// <summary>
            /// The disabled.
            /// </summary>
            public const string Disabled = "#545454";
        }
    }
}