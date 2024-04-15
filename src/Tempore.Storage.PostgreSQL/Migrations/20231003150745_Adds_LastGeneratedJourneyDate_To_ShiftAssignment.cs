// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231003150745_Adds_LastGeneratedJourneyDate_To_ShiftAssignment.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_LastGeneratedJourneyDate_To_ShiftAssignment : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "LastGeneratedJourneyDate",
                table: "ShiftAssignments",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastGeneratedJourneyDate",
                table: "ShiftAssignments");
        }
    }
}