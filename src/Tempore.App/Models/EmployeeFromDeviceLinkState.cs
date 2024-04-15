// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeFromDeviceLinkState.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Models
{
    using System.Collections.Immutable;

    using Tempore.Common.Extensions;

    /// <summary>
    /// The employee from device link state.
    /// </summary>
    public class EmployeeFromDeviceLinkState : State<bool>
    {
        /// <summary>
        /// The linked.
        /// </summary>
        public static readonly EmployeeFromDeviceLinkState Linked = new EmployeeFromDeviceLinkState(true, nameof(Linked));

        /// <summary>
        /// The unlinked.
        /// </summary>
        public static readonly EmployeeFromDeviceLinkState Unlinked = new EmployeeFromDeviceLinkState(false, nameof(Unlinked));

        /// <summary>
        /// All states.
        /// </summary>
        public static readonly IImmutableList<EmployeeFromDeviceLinkState> All =
            typeof(EmployeeFromDeviceLinkState).Constants<EmployeeFromDeviceLinkState>().ToImmutableList();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeFromDeviceLinkState"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private EmployeeFromDeviceLinkState(bool value, string text)
            : base(value, text)
        {
        }
    }
}