// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231115171700_Adds_WorkforceMetricConflictResolution_Unique_Index.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_WorkforceMetricConflictResolution_Unique_Index : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkforceMetricConflictResolutions_ScheduledDayId",
                table: "WorkforceMetricConflictResolutions");

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetricConflictResolutions_ScheduledDayId_Workforce~",
                table: "WorkforceMetricConflictResolutions",
                columns: new[] { "ScheduledDayId", "WorkforceMetricId" },
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkforceMetricConflictResolutions_ScheduledDayId_Workforce~",
                table: "WorkforceMetricConflictResolutions");

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetricConflictResolutions_ScheduledDayId",
                table: "WorkforceMetricConflictResolutions",
                column: "ScheduledDayId");
        }
    }
}