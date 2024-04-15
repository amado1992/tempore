// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231004181909_Adds_ShiftAssignment_To_ScheduledDay_And_Removes_Employee.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_ShiftAssignment_To_ScheduledDay_And_Removes_Employee : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledDays_Employees_EmployeeId",
                table: "ScheduledDays");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "ScheduledDays",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftAssignmentId",
                table: "ScheduledDays",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledDays_ShiftAssignmentId",
                table: "ScheduledDays",
                column: "ShiftAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledDays_Employees_EmployeeId",
                table: "ScheduledDays",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledDays_ShiftAssignments_ShiftAssignmentId",
                table: "ScheduledDays",
                column: "ShiftAssignmentId",
                principalTable: "ShiftAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledDays_Employees_EmployeeId",
                table: "ScheduledDays");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledDays_ShiftAssignments_ShiftAssignmentId",
                table: "ScheduledDays");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledDays_ShiftAssignmentId",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "ShiftAssignmentId",
                table: "ScheduledDays");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "ScheduledDays",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledDays_Employees_EmployeeId",
                table: "ScheduledDays",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}