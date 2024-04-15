// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231008055409_Adds_WorkforceMetric_Entities.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_WorkforceMetric_Entities : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkforceMetricCollections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforceMetricCollections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkforceMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    WorkforceMetricCollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforceMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkforceMetrics_WorkforceMetricCollections_WorkforceMetric~",
                        column: x => x.WorkforceMetricCollectionId,
                        principalTable: "WorkforceMetricCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkforceMetricDailySnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ScheduledDayId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkforceMetricId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforceMetricDailySnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkforceMetricDailySnapshots_ScheduledDays_ScheduledDayId",
                        column: x => x.ScheduledDayId,
                        principalTable: "ScheduledDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkforceMetricDailySnapshots_WorkforceMetrics_WorkforceMet~",
                        column: x => x.WorkforceMetricId,
                        principalTable: "WorkforceMetrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetricCollections_Name",
                table: "WorkforceMetricCollections",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetricDailySnapshots_ScheduledDayId_WorkforceMetri~",
                table: "WorkforceMetricDailySnapshots",
                columns: new[] { "ScheduledDayId", "WorkforceMetricId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetricDailySnapshots_WorkforceMetricId",
                table: "WorkforceMetricDailySnapshots",
                column: "WorkforceMetricId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetrics_Name",
                table: "WorkforceMetrics",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceMetrics_WorkforceMetricCollectionId",
                table: "WorkforceMetrics",
                column: "WorkforceMetricCollectionId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkforceMetricDailySnapshots");

            migrationBuilder.DropTable(
                name: "WorkforceMetrics");

            migrationBuilder.DropTable(
                name: "WorkforceMetricCollections");
        }
    }
}