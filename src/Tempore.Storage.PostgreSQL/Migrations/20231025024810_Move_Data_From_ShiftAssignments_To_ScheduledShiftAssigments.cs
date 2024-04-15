// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231025024810_Move_Data_From_ShiftAssignments_To_ScheduledShiftAssigments.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Move_Data_From_ShiftAssignments_To_ScheduledShiftAssigments : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO \"ScheduledShiftAssignments\"(\"EmployeeId\", \"ShiftId\", \"StartDate\", \"ExpireDate\", \"LastGeneratedDayDate\", \"EffectiveWorkingTime\") SELECT \"EmployeeId\", \"ShiftId\", \"StartDate\", \"ExpireDate\", \"LastGeneratedDayDate\", \"EffectiveWorkingTime\" FROM \"ShiftAssignments\"");
            migrationBuilder.Sql("INSERT INTO \"ScheduledShifts\"(\"ShiftId\", \"StartDate\", \"ExpireDate\", \"EffectiveWorkingTime\") SELECT DISTINCT \"ShiftId\", \"StartDate\", \"ExpireDate\", \"EffectiveWorkingTime\" FROM \"ShiftAssignments\"");
            migrationBuilder.Sql("UPDATE \"ScheduledShiftAssignments\" SET \"ScheduledShiftId\" = \"ScheduledShifts\".\"Id\" FROM \"ScheduledShifts\" WHERE \"ScheduledShiftAssignments\".\"ShiftId\" = \"ScheduledShifts\".\"ShiftId\" AND \"ScheduledShiftAssignments\".\"StartDate\" = \"ScheduledShifts\".\"StartDate\" AND \"ScheduledShiftAssignments\".\"ExpireDate\" = \"ScheduledShifts\".\"ExpireDate\" AND \"ScheduledShiftAssignments\".\"EffectiveWorkingTime\" = \"ScheduledShifts\".\"EffectiveWorkingTime\"");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledDays_ShiftAssignments_ShiftAssignmentId",
                table: "ScheduledDays");

            migrationBuilder.DropTable(
                name: "ShiftAssignments");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}