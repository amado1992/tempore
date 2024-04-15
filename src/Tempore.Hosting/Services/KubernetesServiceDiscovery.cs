// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KubernetesServiceDiscovery.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Hosting.Services
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Tempore.Hosting.Services.Interfaces;

    /// <summary>
    /// The kubernetes service discovery.
    /// </summary>
    public class KubernetesServiceDiscovery : IServiceDiscovery
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<IServiceDiscovery> logger;

        /// <summary>
        /// The environment variable service.
        /// </summary>
        private readonly IEnvironmentVariableService environmentVariableService;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesServiceDiscovery"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="environmentVariableService">
        /// The environment variable.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public KubernetesServiceDiscovery(
            ILogger<IServiceDiscovery> logger,
            IEnvironmentVariableService environmentVariableService,
            IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(environmentVariableService);
            ArgumentNullException.ThrowIfNull(configuration);

            this.logger = logger;
            this.environmentVariableService = environmentVariableService;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<string> GetServiceEndPointAddressAsync(string serviceName, string protocol)
        {
            this.logger.LogDebug("Getting service endpoint address for '{ServiceName}' and '{Protocol}'", serviceName, protocol);

            var endPoint = await this.GetServiceEndPointAsync(serviceName);

            var value = this.CreateEndpointString(protocol, endPoint, string.Empty);

            this.logger.LogDebug("Determined service endpoint address '{Endpoint}' using '{ServiceName}' and '{Protocol}'", value, serviceName, protocol);

            return value;
        }

        /// <inheritdoc/>
        public async Task<string> GetServiceEndPointAddressAsync(string serviceName, string bindingName, string protocol)
        {
            this.logger.LogDebug("Getting service endpoint address for '{ServiceName}' and '{BindingName}' and '{Protocol}'", serviceName, bindingName, protocol);

            var endPoint = await this.GetServiceEndPointAsync(serviceName, bindingName);

            var value = this.CreateEndpointString(protocol, endPoint, string.Empty);

            this.logger.LogDebug("Determined service endpoint address '{Endpoint}' using '{ServiceName}' and '{BindingName}' and '{Protocol}'", value, serviceName, bindingName, protocol);

            return value;
        }

        /// <inheritdoc/>
        public async Task<string> GetServiceEndPointAsync(string serviceName)
        {
            var endpoint = await this.GetServiceEndPointInternalAsync(serviceName);
            if (endpoint == ":")
            {
                endpoint = await this.GetServiceEndPointInternalAsync(serviceName.Replace("-", "_"));
            }

            return endpoint;
        }

        /// <inheritdoc/>
        public async Task<string> GetServiceEndPointAsync(string serviceName, string bindingName)
        {
            var endpoint = await this.GetServiceEndPointInternalAsync(serviceName, bindingName);
            if (endpoint == ":")
            {
                endpoint = await this.GetServiceEndPointInternalAsync(serviceName.Replace("-", "_"), bindingName);
            }

            return endpoint;
        }

        /// <summary>
        /// Creates endpoint string.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="servicePort">
        /// The service port.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected virtual string CreateEndpointString(string? protocol, string? serviceName, string? servicePort)
        {
            ArgumentNullException.ThrowIfNull(serviceName);

            var endpointBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(protocol) && !serviceName.StartsWith(protocol, StringComparison.InvariantCultureIgnoreCase))
            {
                endpointBuilder.Append($"{protocol}://");
            }

            endpointBuilder.Append(serviceName);

            if (!string.IsNullOrWhiteSpace(servicePort))
            {
                endpointBuilder.Append($":{servicePort}");
            }

            var endpoint = endpointBuilder.ToString();
            return endpoint;
        }

        /// <summary>
        /// Gets service end point internal async.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private Task<string> GetServiceEndPointInternalAsync(string serviceName)
        {
            this.logger.LogDebug("Getting service endpoint for '{ServiceName}'", serviceName);

            var serviceHost = this.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_HOST");
            var servicePort = this.GetEnvironmentVariable($"{serviceName.ToUpper()}_SERVICE_PORT");

            var value = this.CreateEndpointString(string.Empty, serviceHost, servicePort);

            this.logger.LogDebug("Determined service endpoint '{Endpoint}' using '{ServiceName}'", value, serviceName);

            return Task.FromResult(value);
        }

        /// <summary>
        /// Gets service end point internal async.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="bindingName">
        /// The binding name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private Task<string> GetServiceEndPointInternalAsync(string serviceName, string bindingName)
        {
            ArgumentNullException.ThrowIfNull(serviceName);
            ArgumentNullException.ThrowIfNull(bindingName);

            this.logger.LogDebug("Getting service endpoint for '{ServiceName}' and '{BindingName}'", serviceName, bindingName);

            // Note: order is important
            var serviceHost = this.GetEnvironmentVariable(
                $"{serviceName.ToUpper()}_SERVICE_HOST_{bindingName.ToUpper()}",
                $"{serviceName.ToUpper()}_{bindingName.ToUpper()}_SERVICE_HOST",
                $"{serviceName.ToUpper()}_SERVICE_HOST");

            var servicePort = this.GetEnvironmentVariable(
                $"{serviceName.ToUpper()}_SERVICE_PORT_{bindingName.ToUpper()}",
                $"{serviceName.ToUpper()}_{bindingName.ToUpper()}_SERVICE_PORT",
                $"{serviceName.ToUpper()}_SERVICE_PORT");

            var value = this.CreateEndpointString(string.Empty, serviceHost, servicePort);

            this.logger.LogDebug("Determined service endpoint '{Endpoint}' using '{ServiceName}' and '{BindingName}'", value, serviceName, bindingName);

            return Task.FromResult(value);
        }

        /// <summary>
        /// Gets environment variable.
        /// </summary>
        /// <param name="names">
        /// The names.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string? GetEnvironmentVariable(params string[] names)
        {
            var finalVariableNames = new List<string>();

            foreach (var name in names)
            {
                finalVariableNames.Add(name);
                finalVariableNames.Add(name.Replace("TMP_", string.Empty));
                finalVariableNames.Add(name.Replace("TEMPORE-", string.Empty));
                finalVariableNames.Add(name.Replace("TEMPORE-", "TMP_"));
            }

            foreach (var variableName in finalVariableNames)
            {
                var value = this.environmentVariableService.GetValue(variableName);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }
    }
}