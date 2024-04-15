// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230926204731_Adds_Attendance_Management_Entities.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Attendance_Management_Entities : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeesFromDevices_Employee_EmployeeId",
                table: "EmployeesFromDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_ExternalId",
                table: "Employees",
                newName: "IX_Employees_ExternalId");

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduledJourneyId",
                table: "Timestamp",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Timetables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ValidCheckInTimeStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ValidCheckInTimeDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ValidCheckOutTimeStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ValidCheckOutTimeDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timetables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Journeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    TimetableId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journeys", x => x.Id);
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
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JourneyId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartJourneyId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shifts_Journeys_StartJourneyId",
                        column: x => x.StartJourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_ScheduledJourneyId",
                table: "Timestamp",
                column: "ScheduledJourneyId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmployeeId",
                table: "ShiftAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_ShiftId",
                table: "ShiftAssignments",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_StartJourneyId",
                table: "Shifts",
                column: "StartJourneyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeesFromDevices_Employees_EmployeeId",
                table: "EmployeesFromDevices",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timestamp_ScheduledJourneys_ScheduledJourneyId",
                table: "Timestamp",
                column: "ScheduledJourneyId",
                principalTable: "ScheduledJourneys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Shifts_ShiftId",
                table: "Journeys",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeesFromDevices_Employees_EmployeeId",
                table: "EmployeesFromDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_Timestamp_ScheduledJourneys_ScheduledJourneyId",
                table: "Timestamp");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Shifts_ShiftId",
                table: "Journeys");

            migrationBuilder.DropTable(
                name: "ScheduledJourneys");

            migrationBuilder.DropTable(
                name: "ShiftAssignments");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Journeys");

            migrationBuilder.DropTable(
                name: "Timetables");

            migrationBuilder.DropIndex(
                name: "IX_Timestamp_ScheduledJourneyId",
                table: "Timestamp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ScheduledJourneyId",
                table: "Timestamp");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employee");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_ExternalId",
                table: "Employee",
                newName: "IX_Employee_ExternalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeesFromDevices_Employee_EmployeeId",
                table: "EmployeesFromDevices",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");
        }
    }
}