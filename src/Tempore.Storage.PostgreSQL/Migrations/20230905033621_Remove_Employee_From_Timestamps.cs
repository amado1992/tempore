// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230905033621_Remove_Employee_From_Timestamps.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Remove_Employee_From_Timestamps : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timestamp_Employee_EmployeeId",
                table: "Timestamp");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Timestamp");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}