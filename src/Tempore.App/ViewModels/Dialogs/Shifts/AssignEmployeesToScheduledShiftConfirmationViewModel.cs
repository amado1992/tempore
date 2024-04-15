// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignEmployeesToScheduledShiftConfirmationViewModel.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.ViewModels.Dialogs.Shifts
{
    /// <summary>
    /// The assign shift to employees confirmation view model.
    /// </summary>
    public class AssignEmployeesToScheduledShiftConfirmationViewModel
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the expire date.
        /// </summary>
        public DateTime? ExpireDate { get; set; }

        /// <summary>
        /// Gets or sets the effective working hours.
        /// </summary>
        public double? EffectiveWorkingHours { get; set; }

        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Gets or sets the employee ids.
        /// </summary>
        public Guid[] EmployeeIds { get; set; }
    }
}