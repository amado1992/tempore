// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeScheduledShiftAssignmentState.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Models
{
    using System.Collections.Immutable;

    using Tempore.Common.Extensions;

    /// <summary>
    /// The employee scheduled shift assignments state.
    /// </summary>
    public class EmployeeScheduledShiftAssignmentState : State<bool>
    {
        /// <summary>
        /// The assigned.
        /// </summary>
        public static readonly EmployeeScheduledShiftAssignmentState Assigned = new EmployeeScheduledShiftAssignmentState(true, nameof(Assigned));

        /// <summary>
        /// The unassigned.
        /// </summary>
        public static readonly EmployeeScheduledShiftAssignmentState Unassigned = new EmployeeScheduledShiftAssignmentState(false, nameof(Unassigned));

        /// <summary>
        /// All states.
        /// </summary>
        public static readonly IImmutableList<EmployeeScheduledShiftAssignmentState> All =
            typeof(EmployeeScheduledShiftAssignmentState).Constants<EmployeeScheduledShiftAssignmentState>().ToImmutableList();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeScheduledShiftAssignmentState"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private EmployeeScheduledShiftAssignmentState(bool value, string text)
            : base(value, text)
        {
        }
    }
}