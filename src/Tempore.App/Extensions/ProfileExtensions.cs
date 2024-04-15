// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Extensions;

using Blorc.OpenIdConnect;

public static class ProfileExtensions
{
    public static string? GetNameOrPreferredUsername(this Profile profile)
    {
        return string.IsNullOrWhiteSpace(profile.Name) ? profile.PreferredUsername : profile.Name;
    }

    public static string GetInitials(this Profile profile, int maxLength = 0)
    {
        var name = profile.GetNameOrPreferredUsername();
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var split = name.Trim().Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
        if (maxLength > 0)
        {
            split = split.Take(maxLength).ToArray();
        }

        return split.Aggregate(string.Empty, (current, s) => current + s[0]);
    }
}