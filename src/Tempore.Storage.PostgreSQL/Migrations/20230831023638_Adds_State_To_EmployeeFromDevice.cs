// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230831023638_Adds_State_To_EmployeeFromDevice.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_State_To_EmployeeFromDevice : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLinked",
                table: "EmployeesFromDevices",
                type: "boolean",
                nullable: false,
                computedColumnSql: "\"EmployeeId\" IS NOT NULL",
                stored: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLinked",
                table: "EmployeesFromDevices");
        }
    }
}