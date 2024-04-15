// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231025050240_Move_LastGeneratedDayDate_To_ScheduledShiftAssignment.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Move_LastGeneratedDayDate_To_ScheduledShiftAssignment : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastGeneratedDayDate",
                table: "ScheduledShifts");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastGeneratedDayDate",
                table: "ScheduledShiftAssignments",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastGeneratedDayDate",
                table: "ScheduledShiftAssignments");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastGeneratedDayDate",
                table: "ScheduledShifts",
                type: "date",
                nullable: true);
        }
    }
}