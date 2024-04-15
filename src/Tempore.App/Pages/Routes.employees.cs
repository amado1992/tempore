// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Routes.employees.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Pages
{
    /// <summary>
    /// The routes.
    /// </summary>
    public static partial class Routes
    {
        /// <summary>
        /// The employees.
        /// </summary>
        public static partial class Employees
        {
            /// <summary>
            /// The root.
            /// </summary>
            public const string Root = "/employees";

            /// <summary>
            /// The employees.
            /// </summary>
            public const string EmployeesList = $"{Root}/employees-list";

            /// <summary>
            /// The employees from devices.
            /// </summary>
            public const string EmployeesFromDevices = $"{Root}/employees-from-devices";
        }
    }
}