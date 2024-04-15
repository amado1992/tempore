// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230905032504_Updates_Timestamps.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Updates_Timestamps : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timestamp_Employee_EmployeeId",
                table: "Timestamp");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Timestamp");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "Timestamp",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeFromDeviceId",
                table: "Timestamp",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_EmployeeFromDeviceId",
                table: "Timestamp",
                column: "EmployeeFromDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timestamp_Employee_EmployeeId",
                table: "Timestamp",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timestamp_EmployeesFromDevices_EmployeeFromDeviceId",
                table: "Timestamp",
                column: "EmployeeFromDeviceId",
                principalTable: "EmployeesFromDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timestamp_Employee_EmployeeId",
                table: "Timestamp");

            migrationBuilder.DropForeignKey(
                name: "FK_Timestamp_EmployeesFromDevices_EmployeeFromDeviceId",
                table: "Timestamp");

            migrationBuilder.DropIndex(
                name: "IX_Timestamp_EmployeeFromDeviceId",
                table: "Timestamp");

            migrationBuilder.DropColumn(
                name: "EmployeeFromDeviceId",
                table: "Timestamp");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "Timestamp",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateTime",
                table: "Timestamp",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddForeignKey(
                name: "FK_Timestamp_Employee_EmployeeId",
                table: "Timestamp",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}