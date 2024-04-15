// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231022231757_Removes_ScheduledDays_From_Employee.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Removes_ScheduledDays_From_Employee : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledDays_Employees_EmployeeId",
                table: "ScheduledDays");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledDays_EmployeeId",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "ScheduledDays");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "ScheduledDays",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledDays_EmployeeId",
                table: "ScheduledDays",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledDays_Employees_EmployeeId",
                table: "ScheduledDays",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}