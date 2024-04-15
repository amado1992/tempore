// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230827231019_Sets_ExternalId_As_Unique.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Sets_ExternalId_As_Unique : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employee_ExternalId",
                table: "Employee",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employee_ExternalId",
                table: "Employee");
        }
    }
}