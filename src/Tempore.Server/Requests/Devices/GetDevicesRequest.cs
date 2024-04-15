// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetDevicesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Devices
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;

    /// <summary>
    /// The devices.
    /// </summary>
    public class GetDevicesRequest : IRequest<List<DeviceDto>>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly GetDevicesRequest Instance = new GetDevicesRequest();

        private GetDevicesRequest()
        {
        }
    }
}