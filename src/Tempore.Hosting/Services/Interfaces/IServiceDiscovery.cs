// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceDiscovery.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Hosting.Services.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The ServiceDiscovery interface.
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// Gets service end point address async.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <returns>
        /// A <see cref="Task{String}"/> representing the result of the asynchronous operation.
        /// </returns>
        Task<string> GetServiceEndPointAddressAsync(string serviceName, string protocol);

        /// <summary>
        /// The get service end point address async.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="bindingName">
        /// The binding name.
        /// </param>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <returns>
        /// A <see cref="Task{String}"/> representing the result of the asynchronous operation.
        /// </returns>
        Task<string> GetServiceEndPointAddressAsync(string serviceName, string bindingName, string protocol);

        /// <summary>
        /// Gets service end point async.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <returns>
        /// A <see cref="Task{String}"/> representing the result of the asynchronous operation.
        /// </returns>
        Task<string> GetServiceEndPointAsync(string serviceName);

        /// <summary>
        /// The get service end point async.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="bindingName">
        /// The binding name.
        /// </param>
        /// <returns>
        /// A <see cref="Task{String}"/> representing the result of the asynchronous operation.
        /// </returns>
        Task<string> GetServiceEndPointAsync(string serviceName, string bindingName);
    }
}