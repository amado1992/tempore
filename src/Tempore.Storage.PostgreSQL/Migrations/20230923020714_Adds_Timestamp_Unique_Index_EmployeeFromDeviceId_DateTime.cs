// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230923020714_Adds_Timestamp_Unique_Index_EmployeeFromDeviceId_DateTime.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Timestamp_Unique_Index_EmployeeFromDeviceId_DateTime : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timestamp_EmployeeFromDeviceId",
                table: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_EmployeeFromDeviceId_DateTime",
                table: "Timestamp",
                columns: new[] { "EmployeeFromDeviceId", "DateTime" },
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timestamp_EmployeeFromDeviceId_DateTime",
                table: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_EmployeeFromDeviceId",
                table: "Timestamp",
                column: "EmployeeFromDeviceId");
        }
    }
}