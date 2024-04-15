// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231022143651_Adds_Unique_Index_To_ScheduledDay_With_Date_And_ShiftAssignmentId.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Unique_Index_To_ScheduledDay_With_Date_And_ShiftAssignmentId : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ScheduledDays_Date_ShiftAssignmentId",
                table: "ScheduledDays",
                columns: new[] { "Date", "ShiftAssignmentId" },
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduledDays_Date_ShiftAssignmentId",
                table: "ScheduledDays");
        }
    }
}