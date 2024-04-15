// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230928041150_Adds_Unique_Index_To_ShiftAssignment.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Unique_Index_To_ShiftAssignment : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_EmployeeId",
                table: "ShiftAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmployeeId_ShiftId",
                table: "ShiftAssignments",
                columns: new[] { "EmployeeId", "ShiftId" },
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_EmployeeId_ShiftId",
                table: "ShiftAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmployeeId",
                table: "ShiftAssignments",
                column: "EmployeeId");
        }
    }
}