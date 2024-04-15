// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing
{
    /// <summary>
    /// The column info class.
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The column name.
        /// </param>
        /// <param name="index">
        /// The column index.
        /// </param>
        /// <param name="include">
        /// The include.
        /// </param>
        public ColumnInfo(string name, int index, bool include = true)
        {
            this.Name = name;
            this.Index = index;
            this.Include = include;
        }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets a value indicating whether the column will be included in the output.
        /// </summary>
        public bool Include { get; }
    }
}