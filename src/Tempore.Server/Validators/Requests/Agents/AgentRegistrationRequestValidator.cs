// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentRegistrationRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Agents
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using AgentRegistrationRequest = Tempore.Server.Requests.Agents.AgentRegistrationRequest;

    /// <summary>
    /// The agent registration request validator.
    /// </summary>
    public class AgentRegistrationRequestValidator : AbstractValidator<AgentRegistrationRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRegistrationRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public AgentRegistrationRequestValidator(IStringLocalizer<AgentRegistrationRequestValidator> stringLocalizer)
        {
            this.RuleFor(request => request.Agent)
                .ChildRules(rules =>
                {
                    rules.RuleFor(dto => dto!.Name).NotEmpty();
                    rules.RuleFor(dto => dto!.ConnectionId).NotEmpty();
                })
                .NotNull();

            this.RuleForEach(request => request.Agent!.Devices)
                .ChildRules(rules => rules.RuleFor(dto => dto.Name)
                    .NotEmpty());
        }
    }
}