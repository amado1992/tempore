// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignEmployeesToScheduledShiftRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using System.Threading;
    using System.Threading.Tasks;

    using EntityFramework.Exceptions.Common;

    using MediatR;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.Employees;
    using Tempore.Server.Specs.Shifts;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The assign employees to scheduled shift request handler.
    /// </summary>
    public class AssignEmployeesToScheduledShiftRequestHandler : IRequestHandler<AssignEmployeesToScheduledShiftRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AssignEmployeesToScheduledShiftRequestHandler> logger;

        /// <summary>
        /// The repository employee.
        /// </summary>
        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignEmployeesToScheduledShiftRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="unitOfWork">
        /// The repository employee.
        /// </param>
        public AssignEmployeesToScheduledShiftRequestHandler(
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
        public async Task<Guid> Handle(AssignEmployeesToScheduledShiftRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            ScheduledShift? scheduledShift;

            var shiftRepository = this.unitOfWork.GetRepository<Shift>();

            var shift = await shiftRepository.SingleOrDefaultAsync(new ShiftByIdSpec(request.ShiftId));
            if (shift is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Shift with Id '{request.ShiftId}' not found");
            }

            await using var transaction = this.unitOfWork.BeginTransaction();

            try
            {
                var scheduledShiftRepository = this.unitOfWork.GetRepository<ScheduledShift>();

                var scheduledShiftSpec = SpecificationBuilder.Build<ScheduledShift>(
                    scheduledShifts => scheduledShifts
                        .Where(s =>
                            s.ShiftId == request.ShiftId
                            && s.StartDate == request.StartDate
                            && s.ExpireDate == request.ExpireDate
                            && s.EffectiveWorkingTime == request.EffectiveWorkingTime));

                scheduledShift = await scheduledShiftRepository.SingleOrDefaultAsync(scheduledShiftSpec);
                if (scheduledShift is null)
                {
                    scheduledShift = new ScheduledShift
                    {
                        ShiftId = request.ShiftId,
                        StartDate = request.StartDate,
                        ExpireDate = request.ExpireDate,
                        EffectiveWorkingTime = request.EffectiveWorkingTime,
                    };

                    scheduledShiftRepository.Add(scheduledShift);
                    await scheduledShiftRepository.SaveChangesAsync();
                }

                var employeeRepository = this.unitOfWork.GetRepository<Employee>();
                var scheduledShiftAssignmentRepository = this.unitOfWork.GetRepository<ScheduledShiftAssignment>();
                foreach (var employeeId in request.EmployeeIds)
                {
                    var employeeByIdSpec = new EmployeeByIdSpec(employeeId);
                    var employee = await employeeRepository.SingleOrDefaultAsync(employeeByIdSpec);
                    if (employee is null)
                    {
                        throw this.logger.LogErrorAndCreateException<NotFoundException>($"Employee with Id '{employeeId}' not found");
                    }

                    var scheduledShiftAssignmentSpec = SpecificationBuilder.Build<ScheduledShiftAssignment>(
                        assignments => assignments.Include(assignment => assignment.ScheduledShift)
                            .Where(scheduledShiftAssignment =>
                                scheduledShiftAssignment.ScheduledShift.ShiftId == request.ShiftId
                                && scheduledShiftAssignment.EmployeeId == employeeId
                                && scheduledShiftAssignment.LastGeneratedDayDate >= request.StartDate));

                    if (await scheduledShiftAssignmentRepository.ContainsAsync(scheduledShiftAssignmentSpec))
                    {
                        throw this.logger.LogErrorAndCreateException<ConflictException>($"Scheduled days for employee '{employee.Id}' and shift '{shift.Id}' were already generated for dates after '{request.StartDate}'");
                    }

                    scheduledShiftAssignmentSpec = SpecificationBuilder.Build<ScheduledShiftAssignment>(assignment => assignment
                        .Include(shiftAssignment => shiftAssignment.ScheduledShift)
                        .Where(scheduledShiftAssignment => scheduledShiftAssignment.EmployeeId == employeeId
                                                           && scheduledShiftAssignment.ScheduledShift.ShiftId == request.ShiftId
                                                           && scheduledShiftAssignment.ScheduledShift.StartDate <= request.ExpireDate
                                                           && scheduledShiftAssignment.ScheduledShift.ExpireDate >= request.StartDate));

                    if (await scheduledShiftAssignmentRepository.ContainsAsync(scheduledShiftAssignmentSpec))
                    {
                        throw this.logger.LogErrorAndCreateException<ConflictException>($"The employee '{employee.Id}' is already assigned to the shift '{shift.Id}' in a date range that intercept with '{request.StartDate}' to '{request.ExpireDate}'.");
                    }

                    this.logger.LogInformation("Adding new shift assignment to employee '{EmployeeId}' to shift '{ShiftId}' from '{StartDate}' to '{ExpireDate}'", employee.Id, shift.Id, request.StartDate, request.ExpireDate);

                    var scheduledShiftAssignment = new ScheduledShiftAssignment
                    {
                        ScheduledShiftId = scheduledShift.Id,
                        EmployeeId = employeeId,
                    };

                    scheduledShiftAssignmentRepository.Add(scheduledShiftAssignment);

                    await scheduledShiftAssignmentRepository.SaveChangesAsync();
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error assigning employees to a shift. Rolling back.");

                await transaction.RollbackAsync(CancellationToken.None);

                if (ex is UniqueConstraintException)
                {
                    throw this.logger.LogErrorAndCreateException<ConflictException>(
                        "Error assigning employees to a shift",
                        ex);
                }

                throw;
            }

            return scheduledShift.Id;
        }
    }
}