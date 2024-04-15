// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231018122057_Adds_EffectiveWorkingTime_To_Timetable_Fixes_Shift_Day_Relationship.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_EffectiveWorkingTime_To_Timetable_Fixes_Shift_Day_Relationship : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Days_StartDayId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_StartDayId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "StartDayId",
                table: "Shifts");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EffectiveWorkingTime",
                table: "Timetables",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveWorkingTime",
                table: "Timetables");

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Shifts",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "StartDayId",
                table: "Shifts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_StartDayId",
                table: "Shifts",
                column: "StartDayId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Days_StartDayId",
                table: "Shifts",
                column: "StartDayId",
                principalTable: "Days",
                principalColumn: "Id");
        }
    }
}