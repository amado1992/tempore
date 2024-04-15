using System;

namespace Tempore.Server.DataTransferObjects
{
    public partial class FileProcessingDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}