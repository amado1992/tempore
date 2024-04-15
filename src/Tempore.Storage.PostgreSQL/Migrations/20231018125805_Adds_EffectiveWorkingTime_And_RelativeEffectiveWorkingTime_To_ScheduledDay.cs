// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20231018125805_Adds_EffectiveWorkingTime_And_RelativeEffectiveWorkingTime_To_ScheduledDay.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_EffectiveWorkingTime_And_RelativeEffectiveWorkingTime_To_ScheduledDay : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "EffectiveWorkingTime",
                table: "ScheduledDays",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RelativeEffectiveWorkingTime",
                table: "ScheduledDays",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveWorkingTime",
                table: "ScheduledDays");

            migrationBuilder.DropColumn(
                name: "RelativeEffectiveWorkingTime",
                table: "ScheduledDays");
        }
    }
}