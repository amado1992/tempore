// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricAggregation.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    public class WorkforceMetricAggregation
    {
        public double Value { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid WorkforceMetricId { get; set; }

        public string EmployeeName { get; set; }

        public string WorkforceMetricName { get; set; }

        public string ExternalId { get; set; }
    }
}