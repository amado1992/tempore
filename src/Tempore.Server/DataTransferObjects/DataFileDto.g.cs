using System;
using Tempore.Storage.Entities;

namespace Tempore.Server.DataTransferObjects
{
    public partial class DataFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public FileType FileType { get; set; }
        public DateTimeOffset ProcessingDate { get; set; }
        public FileProcessingState ProcessingState { get; set; }
    }
}