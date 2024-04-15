// --------------------------------------------------------------------------------------------------------------------
// <copyright file="20230810150349_Adds_ConnectionId_To_Agent_And_Improves_Employee.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Adds_ConnectionId_To_Agent_And_Improves_Employee : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Employee",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionDate",
                table: "Employee",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "BaseHours",
                table: "Employee",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "CostCenter",
                table: "Employee",
                type: "text",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Employee",
                type: "text",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationCard",
                table: "Employee",
                type: "text",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "SocialSecurity",
                table: "Employee",
                type: "text",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Agents",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PayDayFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayDayFile", x => x.Id);
                });
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayDayFile");

            migrationBuilder.DropColumn(
                name: "AdmissionDate",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "BaseHours",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "CostCenter",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "IdentificationCard",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "SocialSecurity",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Agents");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Employee",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");
        }
    }
}