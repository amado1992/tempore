// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231025072626_Adds_Unique_Index_To_ScheduledShift.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Unique_Index_To_ScheduledShift : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduledShifts_ShiftId",
                table: "ScheduledShifts");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShifts_ShiftId_StartDate_ExpireDate_EffectiveWorki~",
                table: "ScheduledShifts",
                columns: new[] { "ShiftId", "StartDate", "ExpireDate", "EffectiveWorkingTime" },
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduledShifts_ShiftId_StartDate_ExpireDate_EffectiveWorki~",
                table: "ScheduledShifts");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShifts_ShiftId",
                table: "ScheduledShifts",
                column: "ShiftId");
        }
    }
}