// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230905053146_Adds_Unique_Index_EmployeeIdOnDevice_DeviceId_To_EmployeeFromDevice.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Unique_Index_EmployeeIdOnDevice_DeviceId_To_EmployeeFromDevice : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmployeesFromDevices_EmployeeIdOnDevice_DeviceId",
                table: "EmployeesFromDevices",
                columns: new[] { "EmployeeIdOnDevice", "DeviceId" },
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeesFromDevices_EmployeeIdOnDevice_DeviceId",
                table: "EmployeesFromDevices");
        }
    }
}