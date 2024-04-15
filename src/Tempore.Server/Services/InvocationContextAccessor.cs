// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationContextAccessor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using System.Globalization;

    using Tempore.Server.Extensions;
    using Tempore.Server.Invokables;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Services.Interfaces;

    public class InvocationContextAccessor : IInvocationContextAccessor
    {
        private readonly ILogger<InvocationContextAccessor> logger;

        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationContextAccessor"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        public InvocationContextAccessor(ILogger<InvocationContextAccessor> logger, IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(httpContextAccessor);

            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public IInvocationContext Create()
        {
            var preferredUsername = this.httpContextAccessor.HttpContext?.User.GetPreferredUsername()!;
            return new InvocationContext(preferredUsername, CultureInfo.CurrentCulture);
        }

        /// <inheritdoc />
        public IInvocationContext<TRequest> Create<TRequest>(TRequest request)
        {
            var preferredUsername = this.httpContextAccessor.HttpContext?.User.GetPreferredUsername()!;
            return new InvocationContext<TRequest>(preferredUsername, CultureInfo.CurrentCulture, request);
        }
    }
}