// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnAssignShiftToEmployeesRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Employees;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The unassign shift to employees request handler.
    /// </summary>
    public class UnAssignShiftToEmployeesRequestHandler : IRequestHandler<UnAssignShiftToEmployeesRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AssignEmployeesToScheduledShiftRequestHandler> logger;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnAssignShiftToEmployeesRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="unitOfWork">
        /// The repository employee.
        /// </param>
        public UnAssignShiftToEmployeesRequestHandler(
            ILogger<AssignEmployeesToScheduledShiftRequestHandler> logger,
            IUnitOfWork<ApplicationDbContext> unitOfWork)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(unitOfWork);

            this.logger = logger;
            this.unitOfWork = unitOfWork;
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
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task Handle(UnAssignShiftToEmployeesRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            await using var transaction = this.unitOfWork.BeginTransaction();

            var shiftAssignmentRepository = this.unitOfWork.GetRepository<ScheduledShiftAssignment>();
            foreach (var shiftToEmployeeId in request.ShiftToEmployeeIds)
            {
                var shiftAssignment = await shiftAssignmentRepository.SingleOrDefaultAsync(shiftAssignment => shiftAssignment.Id == shiftToEmployeeId);

                if (shiftAssignment is null)
                {
                    throw this.logger.LogErrorAndCreateException<NotFoundException>($"Shift assignment with Id '{shiftToEmployeeId}' not found");
                }

                shiftAssignmentRepository.Delete(shiftAssignment);
                await shiftAssignmentRepository.SaveChangesAsync();
            }

            try
            {
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error unassigning employees to a shift. Rolling back.");
                await transaction.RollbackAsync(CancellationToken.None);
            }
        }
    }
}