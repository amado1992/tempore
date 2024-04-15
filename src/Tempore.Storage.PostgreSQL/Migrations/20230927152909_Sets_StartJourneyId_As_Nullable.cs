// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230927152909_Sets_StartJourneyId_As_Nullable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Sets_StartJourneyId_As_Nullable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Journeys_StartJourneyId",
                table: "Shifts");

            migrationBuilder.AlterColumn<Guid>(
                name: "StartJourneyId",
                table: "Shifts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Journeys_StartJourneyId",
                table: "Shifts",
                column: "StartJourneyId",
                principalTable: "Journeys",
                principalColumn: "Id");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Journeys_StartJourneyId",
                table: "Shifts");

            migrationBuilder.AlterColumn<Guid>(
                name: "StartJourneyId",
                table: "Shifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Journeys_StartJourneyId",
                table: "Shifts",
                column: "StartJourneyId",
                principalTable: "Journeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}