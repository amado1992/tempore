﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnvironmentVariableService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Hosting.Services.Interfaces
{
    using System;

    public interface IEnvironmentVariableService
    {
        string? GetValue(string name);

        string? GetValue(string name, EnvironmentVariableTarget target);
    }
}