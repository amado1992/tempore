// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetShiftByIdRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Shifts
{
    using Mapster;

    using MediatR;

    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Shifts;
    using Tempore.Server.Specs.Shifts;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The shift request handler.
    /// </summary>
    public class GetShiftByIdRequestHandler : IRequestHandler<GetShiftByIdRequest, ShiftDto>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetShiftByIdRequestHandler> logger;

        /// <summary>
        /// The shift repository.
        /// </summary>
        private readonly IRepository<Shift, ApplicationDbContext> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetShiftByIdRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public GetShiftByIdRequestHandler(ILogger<GetShiftByIdRequestHandler> logger, IRepository<Shift, ApplicationDbContext> repository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(repository);

            this.logger = logger;
            this.repository = repository;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<ShiftDto> Handle(GetShiftByIdRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new ShiftByIdSpec(request.Id);
            var shift = await this.repository.SingleOrDefaultAsync(specification);

            if (shift is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Shift with Id '{request.Id}' not found");
            }

            var shiftDto = shift.Adapt<ShiftDto>();
            return shiftDto;
        }
    }
}