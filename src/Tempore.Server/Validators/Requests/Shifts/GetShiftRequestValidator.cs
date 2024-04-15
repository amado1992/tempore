// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetShiftRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Shifts
{
    using FluentValidation;

    using Tempore.Server.Requests.Shifts;

    /// <summary>
    /// The shift request validator.
    /// </summary>
    public class GetShiftRequestValidator : AbstractValidator<GetShiftRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetShiftRequestValidator"/> class.
        /// </summary>
        public GetShiftRequestValidator()
        {
            this.RuleFor(request => request.Skip).GreaterThanOrEqualTo(0);
            this.RuleFor(request => request.Take).GreaterThanOrEqualTo(0);
        }
    }
}