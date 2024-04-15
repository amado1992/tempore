// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231003164428_Renames_Journey_As_Day.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Renames_Journey_As_Day : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Journeys_StartJourneyId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Timestamp_ScheduledJourneys_ScheduledJourneyId",
                table: "Timestamp");

            migrationBuilder.DropTable(
                name: "ScheduledJourneys");

            migrationBuilder.DropTable(
                name: "Journeys");

            migrationBuilder.RenameColumn(
                name: "ScheduledJourneyId",
                table: "Timestamp",
                newName: "ScheduledDayId");

            migrationBuilder.RenameIndex(
                name: "IX_Timestamp_ScheduledJourneyId",
                table: "Timestamp",
                newName: "IX_Timestamp_ScheduledDayId");

            migrationBuilder.RenameColumn(
                name: "StartJourneyId",
                table: "Shifts",
                newName: "StartDayId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_StartJourneyId",
                table: "Shifts",
                newName: "IX_Shifts_StartDayId");

            migrationBuilder.RenameColumn(
                name: "LastGeneratedJourneyDate",
                table: "ShiftAssignments",
                newName: "LastGeneratedDayDate");

            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    TimetableId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Days_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Days_Timetables_TimetableId",
                        column: x => x.TimetableId,
                        principalTable: "Timetables",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScheduledDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledDays_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledDays_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Days_ShiftId",
                table: "Days",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Days_TimetableId",
                table: "Days",
                column: "TimetableId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledDays_DayId",
                table: "ScheduledDays",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledDays_EmployeeId",
                table: "ScheduledDays",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Days_StartDayId",
                table: "Shifts",
                column: "StartDayId",
                principalTable: "Days",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timestamp_ScheduledDays_ScheduledDayId",
                table: "Timestamp",
                column: "ScheduledDayId",
                principalTable: "ScheduledDays",
                principalColumn: "Id");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Days_StartDayId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Timestamp_ScheduledDays_ScheduledDayId",
                table: "Timestamp");

            migrationBuilder.DropTable(
                name: "ScheduledDays");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.RenameColumn(
                name: "ScheduledDayId",
                table: "Timestamp",
                newName: "ScheduledJourneyId");

            migrationBuilder.RenameIndex(
                name: "IX_Timestamp_ScheduledDayId",
                table: "Timestamp",
                newName: "IX_Timestamp_ScheduledJourneyId");

            migrationBuilder.RenameColumn(
                name: "StartDayId",
                table: "Shifts",
                newName: "StartJourneyId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_StartDayId",
                table: "Shifts",
                newName: "IX_Shifts_StartJourneyId");

            migrationBuilder.RenameColumn(
                name: "LastGeneratedDayDate",
                table: "ShiftAssignments",
                newName: "LastGeneratedJourneyDate");

            migrationBuilder.CreateTable(
                name: "Journeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimetableId = table.Column<Guid>(type: "uuid", nullable: true),
                    Index = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Journeys_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Journeys_Timetables_TimetableId",
                        column: x => x.TimetableId,
                        principalTable: "Timetables",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScheduledJourneys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JourneyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledJourneys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledJourneys_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledJourneys_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_ShiftId",
                table: "Journeys",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_TimetableId",
                table: "Journeys",
                column: "TimetableId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJourneys_EmployeeId",
                table: "ScheduledJourneys",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJourneys_JourneyId",
                table: "ScheduledJourneys",
                column: "JourneyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Journeys_StartJourneyId",
                table: "Shifts",
                column: "StartJourneyId",
                principalTable: "Journeys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timestamp_ScheduledJourneys_ScheduledJourneyId",
                table: "Timestamp",
                column: "ScheduledJourneyId",
                principalTable: "ScheduledJourneys",
                principalColumn: "Id");
        }
    }
}