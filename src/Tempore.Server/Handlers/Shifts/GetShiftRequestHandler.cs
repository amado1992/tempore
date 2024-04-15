// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetShiftRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Agents
{
    using System.Threading;
    using System.Threading.Tasks;

    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Shifts;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.Shifts;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The devices by agent id request handler.
    /// </summary>
    public class GetShiftRequestHandler : IRequestHandler<GetShiftRequest, PagedResponse<ShiftDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetShiftRequestHandler> logger;

        /// <summary>
        /// The employee repository.
        /// </summary>
        private readonly IRepository<Shift, ApplicationDbContext> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetShiftRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="repository">
        /// The device repository.
        /// </param>
        public GetShiftRequestHandler(ILogger<GetShiftRequestHandler> logger, IRepository<Shift, ApplicationDbContext> repository)
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
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<PagedResponse<ShiftDto>> Handle(GetShiftRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new ShiftBasicFilterSpec(request.SearchText, request.StartDate, new PaginationOptions(request.Skip, request.Take, false));
            var count = await this.repository.CountAsync(specification);
            if (count == 0)
            {
                return new PagedResponse<ShiftDto>(0, Array.Empty<ShiftDto>());
            }

            specification.PaginationOptions.IsEnable = true;
            var list = await this.repository.FindAsync(specification);

            var items = list.Adapt<List<ShiftDto>>();
            return new PagedResponse<ShiftDto>(count, items);
        }
    }
}