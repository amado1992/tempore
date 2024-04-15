// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAgentByIdRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Agents
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.Agents;

    /// <summary>
    /// The agent by id request validator.
    /// </summary>
    public class GetAgentByIdRequestValidator : AbstractValidator<GetAgentByIdRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAgentByIdRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public GetAgentByIdRequestValidator(IStringLocalizer<GetAgentByIdRequestValidator> stringLocalizer)
        {
            this.RuleFor(request => request.AgentId)
                .NotNull()
                .NotEmpty();
        }
    }
}