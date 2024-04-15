// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateAsyncActionFilter.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Validation.Filters
{
    using System.Collections.Concurrent;
    using System.Reflection;

    using FluentValidation;
    using FluentValidation.Results;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The validate async action filter.
    /// </summary>
    public class ValidateAsyncActionFilter : IAsyncActionFilter
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> ValidateAsyncMethodCache = new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ValidateAsyncActionFilter> logger;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateAsyncActionFilter"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public ValidateAsyncActionFilter(ILogger<ValidateAsyncActionFilter> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// The on action execution async.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelStateProperty = typeof(ControllerBase).GetProperty(nameof(ControllerBase.ModelState));
            if (modelStateProperty == null)
            {
                return;
            }

            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var parameterInfos = descriptor.MethodInfo.GetParameters();
                foreach (var parameterInfo in parameterInfos)
                {
                    var attribute = parameterInfo.GetCustomAttribute(typeof(FromServicesAttribute));
                    if (attribute is not null)
                    {
                        continue;
                    }

                    var value = context.ActionArguments[parameterInfo.Name!];
                    if (value == null)
                    {
                        continue;
                    }

                    object? validator = null;
                    var valueType = value.GetType();
                    try
                    {
                        validator = this.serviceProvider.GetRequiredService(typeof(IValidator<>).MakeGenericType(valueType));
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogWarning(ex, "Error activating validator for {Type}", valueType);
                    }

                    if (validator is null)
                    {
                        continue;
                    }

                    var validatorType = validator.GetType();
                    try
                    {
                        var validateAsyncMethod = ValidateAsyncMethodCache.GetOrAdd(
                            validatorType,
                            type => type.GetMethods().First(
                                info => info.Name == nameof(IValidator.ValidateAsync)
                                        && info.GetParameters()[0].ParameterType == valueType));
                        if (validateAsyncMethod?.Invoke(validator, new[] { value, CancellationToken.None }) is Task<ValidationResult> validationResultTask)
                        {
                            this.logger.LogInformation("Validating object of Type '{Type}' with validator '{ValidatorType}'", valueType, validatorType);

                            var validationResult = await validationResultTask;
                            if (validationResult.IsValid)
                            {
                                this.logger.LogInformation("Validation result of object of Type '{Type}' from validator '{ValidatorType}' indicates that object state is valid", valueType, validatorType);

                                continue;
                            }

                            this.logger.LogWarning("Validation result of object of Type '{Type}' from validator '{ValidatorType}' indicates that object state is not valid", valueType, validatorType);

                            if (modelStateProperty.GetValue(context.Controller) is ModelStateDictionary modelStateDictionary)
                            {
                                foreach (var validationResultError in validationResult.Errors)
                                {
                                    modelStateDictionary.AddModelError(validationResultError.PropertyName, validationResultError.ErrorMessage);
                                }
                            }

                            var options = this.serviceProvider.GetRequiredService<IOptions<ApiBehaviorOptions>>();
                            context.Result = options.Value.InvalidModelStateResponseFactory(context);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO: Also return an error an break the validation?
                        this.logger.LogInformation(ex, "Error executing validation of object of type '{Type}' with validator '{ValidatorType}'", valueType, validatorType);
                    }
                }
            }

            if (context.Result is null)
            {
                await next();
            }
        }
    }
}