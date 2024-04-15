// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTimestampsFromDevicesRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.DeviceCommands
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.DeviceCommands;

    /// <summary>
    /// The upload employees from devices request validator.
    /// </summary>
    public class UploadTimestampsFromDevicesRequestValidator : AbstractValidator<UploadTimestampsFromDevicesRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadTimestampsFromDevicesRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public UploadTimestampsFromDevicesRequestValidator(IStringLocalizer<UploadTimestampsFromDevicesRequestValidator> stringLocalizer)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer);

            this.RuleFor(request => request.DeviceIds)
                .NotEmpty();
        }
    }
}