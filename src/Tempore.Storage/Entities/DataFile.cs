// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataFile.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The data file .
    /// </summary>
    public class DataFile
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the file type.
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the processing date.
        /// </summary>
        public DateTimeOffset ProcessingDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the processing state.
        /// </summary>
        public FileProcessingState ProcessingState { get; set; }
    }
}