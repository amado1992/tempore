// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231115142110_Adds_WorkforceMetricConflictResolution.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_WorkforceMetricConflictResolution : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkforceMetricConflictResolutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ScheduledDayId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkforceMetricId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforceMetricConflictResolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkforceMetricConflictResolutions_ScheduledDays_ScheduledD~",
                        column: x => x.ScheduledDayId,
                        principalTable: "ScheduledDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkforceMetricConflictResolutions_WorkforceMetrics_Workfor~",
                        column: x => x.WorkforceMetricId,
                        principalTable: "WorkforceMetrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetricConflictResolutions_ScheduledDayId",
                table: "WorkforceMetricConflictResolutions",
                column: "ScheduledDayId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetricConflictResolutions_WorkforceMetricId",
                table: "WorkforceMetricConflictResolutions",
                column: "WorkforceMetricId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkforceMetricConflictResolutions");
        }
    }
}