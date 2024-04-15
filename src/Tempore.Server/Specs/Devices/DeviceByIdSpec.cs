// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceByIdSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Devices
{
    using System;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The device by id spec.
    /// </summary>
    public class DeviceByIdSpec : ISpecification<Device>
    {
        /// <summary>
        /// The id.
        /// </summary>
        private readonly Guid id;

        private readonly bool includeAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceByIdSpec"/> class.
        /// </summary>
        /// <param name="id">
        ///     The device id.
        /// </param>
        /// <param name="includeAgent">
        /// The include agent option.
        /// </param>
        public DeviceByIdSpec(Guid id, bool includeAgent = false)
        {
            this.id = id;
            this.includeAgent = includeAgent;
        }

        /// <inheritdoc />
        public Func<IQueryable<Device>, IQueryable<Device>> Build()
        {
            return devices =>
            {
                if (this.includeAgent)
                {
                    devices = devices.Include(device => device.Agent);
                }

                return devices.Where(device => device.Id == this.id);
            };
        }
    }
}