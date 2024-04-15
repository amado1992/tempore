// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DevicesSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Devices
{
    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The DevicesByAgentIdSpec.
    /// </summary>
    public class DevicesSpec : ISpecification<Device>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesSpec"/> class.
        /// </summary>
        public DevicesSpec()
        {
        }

        /// <summary>
        /// Devices.
        /// </summary>
        /// <returns>
        /// devices.
        /// </returns>
        public Func<IQueryable<Device>, IQueryable<Device>> Build()
        {
            return devices => devices;
        }
    }
}