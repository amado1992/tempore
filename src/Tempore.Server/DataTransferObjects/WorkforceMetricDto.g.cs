using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class WorkforceMetricDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid WorkforceMetricCollectionId { get; set; }
        public WorkforceMetricCollectionDto WorkforceMetricCollection { get; set; }
        public List<WorkforceMetricConflictResolutionDto> WorkforceMetricConflictResolutions { get; set; }
    }
}