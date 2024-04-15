// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Tempore client services.
        /// </summary>
        /// <param name="serviceCollection">
        /// The service collection/.
        /// </param>
        /// <param name="httpClientAction">
        /// The http configuration action.
        /// </param>
        /// <param name="httpClientBuilderAction">
        /// The http client builder configuration action.
        /// </param>
        public static void AddTemporeHttpClients(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, HttpClient>? httpClientAction = null,
            Action<IHttpClientBuilder>? httpClientBuilderAction = null)
        {
            var registrationFunctions = new List<Func<Action<IServiceProvider, HttpClient>, IHttpClientBuilder>>
                            {
                                serviceCollection.AddHttpClient<IAgentClient, AgentClient>,
                                serviceCollection.AddHttpClient<IAgentCommandClient, AgentCommandClient>,
                                serviceCollection.AddHttpClient<IDeviceClient, DeviceClient>,
                                serviceCollection.AddHttpClient<IEmployeeClient, EmployeeClient>,
                                serviceCollection.AddHttpClient<ITimestampClient, TimestampClient>,
                                serviceCollection.AddHttpClient<IFileProcessingClient, FileProcessingClient>,
                                serviceCollection.AddHttpClient<ISystemClient, SystemClient>,
                                serviceCollection.AddHttpClient<IShiftClient, ShiftClient>,
                                serviceCollection.AddHttpClient<IScheduledDayClient, ScheduledDayClient>,
                                serviceCollection.AddHttpClient<IWorkforceMetricClient, WorkforceMetricClient>,
                            };

            foreach (var function in registrationFunctions)
            {
                var httpClientBuilder = function.Invoke(
                    (serviceProvider, httpClient) => httpClientAction?.Invoke(serviceProvider, httpClient));
                httpClientBuilderAction?.Invoke(httpClientBuilder);
            }
        }

        /// <summary>
        /// Adds Tempore client services.
        /// </summary>
        /// <param name="serviceCollection">
        /// The service collection/.
        /// </param>
        /// <param name="httpClientAction">
        /// The http configuration action.
        /// </param>
        /// <param name="httpClientBuilderAction">
        /// The http client builder configuration action.
        /// </param>
        public static void AddTemporeHttpClients(
            this IServiceCollection serviceCollection,
            Action<HttpClient>? httpClientAction = null,
            Action<IHttpClientBuilder>? httpClientBuilderAction = null)
        {
            serviceCollection.AddTemporeHttpClients(
                (_, httpClient) => httpClientAction?.Invoke(httpClient),
                httpClientBuilderAction);
        }
    }
}