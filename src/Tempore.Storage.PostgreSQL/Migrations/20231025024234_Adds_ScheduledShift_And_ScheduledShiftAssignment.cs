// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231025024234_Adds_ScheduledShift_And_ScheduledShiftAssignment.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_ScheduledShift_And_ScheduledShiftAssignment : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveWorkingTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LastGeneratedDayDate = table.Column<DateOnly>(type: "date", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledShifts_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledShiftAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledShiftId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveWorkingTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LastGeneratedDayDate = table.Column<DateOnly>(type: "date", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledShiftAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledShiftAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledShiftAssignments_ScheduledShifts_ScheduledShiftId",
                        column: x => x.ScheduledShiftId,
                        principalTable: "ScheduledShifts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledShiftAssignments_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShiftAssignments_EmployeeId_ShiftId_StartDate",
                table: "ScheduledShiftAssignments",
                columns: new[] { "EmployeeId", "ShiftId", "StartDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShiftAssignments_ScheduledShiftId",
                table: "ScheduledShiftAssignments",
                column: "ScheduledShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShiftAssignments_ShiftId",
                table: "ScheduledShiftAssignments",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledShifts_ShiftId",
                table: "ScheduledShifts",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledDays_ScheduledShiftAssignments_ShiftAssignmentId",
                table: "ScheduledDays",
                column: "ShiftAssignmentId",
                principalTable: "ScheduledShiftAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}