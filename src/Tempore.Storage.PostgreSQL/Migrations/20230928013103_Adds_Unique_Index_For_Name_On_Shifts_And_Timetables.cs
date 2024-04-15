// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230928013103_Adds_Unique_Index_For_Name_On_Shifts_And_Timetables.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_Unique_Index_For_Name_On_Shifts_And_Timetables : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Timetables_Name",
                table: "Timetables",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_Name",
                table: "Shifts",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timetables_Name",
                table: "Timetables");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_Name",
                table: "Shifts");
        }
    }
}