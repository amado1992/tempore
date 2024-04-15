// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileWriterFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces;

using Tempore.Storage.Entities;

public interface IFileContentWriterFactory
{
    IFileContentWriter Create(FileFormat fileFormat);

    void Register<TServiceType>(FileFormat fileFormat)
        where TServiceType : IFileContentWriter;
}