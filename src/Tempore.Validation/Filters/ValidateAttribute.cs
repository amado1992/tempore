// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateAttribute.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Validation.Filters
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The validate attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateAttribute : ServiceFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateAttribute"/> class.
        /// </summary>
        public ValidateAttribute()
            : base(typeof(ValidateAsyncActionFilter))
        {
        }
    }
}