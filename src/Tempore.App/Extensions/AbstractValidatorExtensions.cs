// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractValidatorExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Extensions
{
    using FluentValidation;

    /// <summary>
    /// The abstract validator extensions.
    /// </summary>
    public static class AbstractValidatorExtensions
    {
        /// <summary>
        /// The validate value.
        /// </summary>
        /// <param name="validator">
        /// The validator.
        /// </param>
        /// <typeparam name="TModel">
        /// The model type.
        /// </typeparam>
        /// <returns>
        /// The validation result.a.
        /// </returns>
        public static Func<object, string, Task<IEnumerable<string>>> ValidateValue<TModel>(this IValidator<TModel> validator)
        {
            return async (model, propertyName) =>
            {
                var result = await validator.ValidateAsync(
                                 ValidationContext<TModel>.CreateWithOptions(
                                     (TModel)model,
                                     x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}