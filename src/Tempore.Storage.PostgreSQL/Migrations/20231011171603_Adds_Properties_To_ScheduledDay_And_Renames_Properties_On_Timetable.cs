// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231011171603_Adds_Properties_To_ScheduledDay_And_Renames_Properties_On_Timetable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Properties_To_ScheduledDay_And_Renames_Properties_On_Timetable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidCheckOutTimeStart",
                table: "Timetables",
                newName: "CheckOutTimeStart");

            migrationBuilder.RenameColumn(
                name: "ValidCheckOutTimeDuration",
                table: "Timetables",
                newName: "CheckOutTimeDuration");

            migrationBuilder.RenameColumn(
                name: "ValidCheckInTimeStart",
                table: "Timetables",
                newName: "CheckInTimeStart");

            migrationBuilder.RenameColumn(
                name: "ValidCheckInTimeDuration",
                table: "Timetables",
                newName: "CheckInTimeDuration");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CheckInEndDateTime",
                table: "ScheduledDays",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CheckInStartDateTime",
                table: "ScheduledDays",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CheckOutEndDateTime",
                table: "ScheduledDays",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CheckOutStartDateTime",
                table: "ScheduledDays",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDateTime",
                table: "ScheduledDays",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDateTime",
                table: "ScheduledDays",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInEndDateTime",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "CheckInStartDateTime",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "CheckOutEndDateTime",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "CheckOutStartDateTime",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "ScheduledDays");

            migrationBuilder.RenameColumn(
                name: "CheckOutTimeStart",
                table: "Timetables",
                newName: "ValidCheckOutTimeStart");

            migrationBuilder.RenameColumn(
                name: "CheckOutTimeDuration",
                table: "Timetables",
                newName: "ValidCheckOutTimeDuration");

            migrationBuilder.RenameColumn(
                name: "CheckInTimeStart",
                table: "Timetables",
                newName: "ValidCheckInTimeStart");

            migrationBuilder.RenameColumn(
                name: "CheckInTimeDuration",
                table: "Timetables",
                newName: "ValidCheckInTimeDuration");
        }
    }
}