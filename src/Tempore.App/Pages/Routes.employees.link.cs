// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Routes.employees.link.cs" company="Port Hope Investment S.A.">
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
            /// The link employees.
            /// </summary>
            public const string LinkEmployees = $"{Root}/link-employees";

            /// <summary>
            /// The employee details.
            /// </summary>
            public const string EmployeeDetailsTemplate = $"{Root}/employee-details/{{Id:guid}}";

            /// <summary>
            /// The import file.
            /// </summary>
            public const string ImportFromFile = $"{Root}/import-from-file";

            /// <summary>
            /// The preview file.
            /// </summary>
            public const string PreviewFileTemplate = $"{Root}/preview-file/{{FileId:guid}}";

            /// <summary>
            /// Preview a file.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public static string PreviewFile(Guid id)
            {
                return PreviewFileTemplate.Replace("{FileId:guid}", id.ToString());
            }

            /// <summary>
            /// View of employe details.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <returns>
            /// The <see cref="Guid"/>.
            /// </returns>
            public static string EmployeeDetails(Guid? id)
            {
                return EmployeeDetailsTemplate.Replace("{Id:guid}", id.ToString());
            }
        }
    }
}