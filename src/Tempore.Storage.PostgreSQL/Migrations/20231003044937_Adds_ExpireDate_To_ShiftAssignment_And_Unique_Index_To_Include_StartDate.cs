// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231003044937_Adds_ExpireDate_To_ShiftAssignment_And_Unique_Index_To_Include_StartDate.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_ExpireDate_To_ShiftAssignment_And_Unique_Index_To_Include_StartDate : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_EmployeeId_ShiftId",
                table: "ShiftAssignments");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpireDate",
                table: "ShiftAssignments",
                type: "date",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmployeeId_ShiftId_StartDate",
                table: "ShiftAssignments",
                columns: new[] { "EmployeeId", "ShiftId", "StartDate" },
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_EmployeeId_ShiftId_StartDate",
                table: "ShiftAssignments");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "ShiftAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmployeeId_ShiftId",
                table: "ShiftAssignments",
                columns: new[] { "EmployeeId", "ShiftId" },
                unique: true);
        }
    }
}