// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231025034226_Removes_Obsolete_Relationships.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Removes_Obsolete_Relationships : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledDays_ScheduledShiftAssignments_ShiftAssignmentId",
                table: "ScheduledDays");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledShiftAssignments_ScheduledShifts_ScheduledShiftId",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledShiftAssignments_Shifts_ShiftId",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledShiftAssignments_EmployeeId_ShiftId_StartDate",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledShiftAssignments_ShiftId",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropColumn(
                name: "EffectiveWorkingTime",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropColumn(
                name: "LastGeneratedDayDate",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ScheduledShiftAssignments");

            migrationBuilder.RenameColumn(
                name: "ShiftAssignmentId",
                table: "ScheduledDays",
                newName: "ScheduledShiftAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduledDays_ShiftAssignmentId",
                table: "ScheduledDays",
                newName: "IX_ScheduledDays_ScheduledShiftAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduledDays_Date_ShiftAssignmentId",
                table: "ScheduledDays",
                newName: "IX_ScheduledDays_Date_ScheduledShiftAssignmentId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduledShiftId",
                table: "ScheduledShiftAssignments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShiftAssignments_EmployeeId_ScheduledShiftId",
                table: "ScheduledShiftAssignments",
                columns: new[] { "EmployeeId", "ScheduledShiftId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledDays_ScheduledShiftAssignments_ScheduledShiftAssig~",
                table: "ScheduledDays",
                column: "ScheduledShiftAssignmentId",
                principalTable: "ScheduledShiftAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledShiftAssignments_ScheduledShifts_ScheduledShiftId",
                table: "ScheduledShiftAssignments",
                column: "ScheduledShiftId",
                principalTable: "ScheduledShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledDays_ScheduledShiftAssignments_ScheduledShiftAssig~",
                table: "ScheduledDays");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledShiftAssignments_ScheduledShifts_ScheduledShiftId",
                table: "ScheduledShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledShiftAssignments_EmployeeId_ScheduledShiftId",
                table: "ScheduledShiftAssignments");

            migrationBuilder.RenameColumn(
                name: "ScheduledShiftAssignmentId",
                table: "ScheduledDays",
                newName: "ShiftAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduledDays_ScheduledShiftAssignmentId",
                table: "ScheduledDays",
                newName: "IX_ScheduledDays_ShiftAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduledDays_Date_ScheduledShiftAssignmentId",
                table: "ScheduledDays",
                newName: "IX_ScheduledDays_Date_ShiftAssignmentId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduledShiftId",
                table: "ScheduledShiftAssignments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EffectiveWorkingTime",
                table: "ScheduledShiftAssignments",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpireDate",
                table: "ScheduledShiftAssignments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastGeneratedDayDate",
                table: "ScheduledShiftAssignments",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "ScheduledShiftAssignments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "ScheduledShiftAssignments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShiftAssignments_EmployeeId_ShiftId_StartDate",
                table: "ScheduledShiftAssignments",
                columns: new[] { "EmployeeId", "ShiftId", "StartDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShiftAssignments_ShiftId",
                table: "ScheduledShiftAssignments",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledDays_ScheduledShiftAssignments_ShiftAssignmentId",
                table: "ScheduledDays",
                column: "ShiftAssignmentId",
                principalTable: "ScheduledShiftAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledShiftAssignments_ScheduledShifts_ScheduledShiftId",
                table: "ScheduledShiftAssignments",
                column: "ScheduledShiftId",
                principalTable: "ScheduledShifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledShiftAssignments_Shifts_ShiftId",
                table: "ScheduledShiftAssignments",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}