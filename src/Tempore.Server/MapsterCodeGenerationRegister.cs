// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapsterCodeGenerationRegister.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server
{
    using Mapster;

    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The mapster code generation register.
    /// </summary>
    public class MapsterCodeGenerationRegister : ICodeGenerationRegister
    {
        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public void Register(CodeGenerationConfig config)
        {
            config.AdaptTo("[name]Dto")
                .ForAllTypesInNamespace(typeof(ApplicationDbContext).Assembly, "Tempore.Storage.Entities")
                .ExcludeTypes(type => type.IsEnum)
                .IgnoreNullValues(true);

            config.AdaptTo("[name]RegistrationDto")
                .ForType<Agent>(builder => builder.Ignore(agent => agent.Id))
                .ForType<Device>(builder =>
                {
                    builder.Ignore(device => device.Id);
                    builder.Ignore(device => device.Agent);
                    builder.Ignore(device => device.AgentId);
                    builder.Ignore(device => device.EmployeesFromDevices);
                })
                .ExcludeTypes(type => type.IsEnum)
                .IgnoreNullValues(true);
        }
    }
}