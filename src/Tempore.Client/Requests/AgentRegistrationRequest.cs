// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentRegistrationRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client
{
    /// <summary>
    /// The AgentRegistrationRequest class.
    /// </summary>
    public partial class AgentRegistrationRequest
    {
        /// <summary>
        /// Creates an instance of <see cref="AgentRegistrationRequest"/>.
        /// </summary>
        /// <param name="agent">
        /// The device id.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AgentRegistrationRequest"/>.
        /// </returns>
        public static AgentRegistrationRequest Create(AgentRegistrationDto agent)
        {
            return new AgentRegistrationRequest
            {
                Agent = agent,
            };
        }
    }
}