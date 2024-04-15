// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceByNameSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Devices
{
    using System;
    using System.Linq;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The device by name spec.
    /// </summary>
    public class DeviceByNameSpec : ISpecification<Device>
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceByNameSpec"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public DeviceByNameSpec(string name)
        {
            this.name = name;
        }

        /// <inheritdoc />
        public Func<IQueryable<Device>, IQueryable<Device>> Build()
        {
            return devices => devices.Where(device => device.Name == this.name);
        }
    }
}