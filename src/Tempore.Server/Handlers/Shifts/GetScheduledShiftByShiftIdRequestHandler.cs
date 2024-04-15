// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetScheduledShiftByShiftIdRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Shifts
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
    using Tempore.Server.Specs.ShiftAssignment;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The get shift scheduled shift by shift id request handler.
    /// </summary>
    public class GetScheduledShiftByShiftIdRequestHandler : IRequestHandler<GetScheduledShiftByShiftIdRequest,
        PagedResponse<ScheduledShiftOverviewDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetScheduledShiftByShiftIdRequestHandler> logger;

        /// <summary>
        /// The employee repository.
        /// </summary>
        private readonly IRepository<ScheduledShift, ApplicationDbContext> scheduledShiftRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetScheduledShiftByShiftIdRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="scheduledShiftRepository">
        /// The scheduled shift repository.
        /// </param>
        public GetScheduledShiftByShiftIdRequestHandler(
            ILogger<GetScheduledShiftByShiftIdRequestHandler> logger,
            IRepository<ScheduledShift, ApplicationDbContext> scheduledShiftRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(scheduledShiftRepository);

            this.logger = logger;
            this.scheduledShiftRepository = scheduledShiftRepository;
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
        public async Task<PagedResponse<ScheduledShiftOverviewDto>> Handle(
            GetScheduledShiftByShiftIdRequest request, CancellationToken cancellationToken)
        {
            var paginationOptions = new PaginationOptions(request.Skip, request.Take, false);
            var specification = new ScheduledShiftOverviewSpec(request.ShiftId, request.SearchParams, paginationOptions);
            var count = await this.scheduledShiftRepository.CountAsync(specification);
            if (count == 0)
            {
                return new PagedResponse<ScheduledShiftOverviewDto>(0, Enumerable.Empty<ScheduledShiftOverviewDto>());
            }

            paginationOptions.IsEnable = true;
            var shiftAssignments = await this.scheduledShiftRepository.FindAsync(specification);
            return new PagedResponse<ScheduledShiftOverviewDto>(0, shiftAssignments.Adapt<List<ScheduledShiftOverviewDto>>());
        }
    }
}