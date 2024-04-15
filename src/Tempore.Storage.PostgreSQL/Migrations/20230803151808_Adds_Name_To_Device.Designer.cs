﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tempore.Storage.PostgreSQL;

#nullable disable

namespace Tempore.Storage.PostgreSQL.Migrations
{
    [DbContext(typeof(PostgreSQLApplicationDbContext))]
    [Migration("20230803151808_Adds_Name_To_Device")]
    partial class Adds_Name_To_Device
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tempore.Storage.Entities.Agent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("AgentId")
                        .HasColumnType("uuid");

                    b.Property<string>("DeviceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MacAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("MacAddress")
                        .IsUnique();

                    b.HasIndex("SerialNumber")
                        .IsUnique();

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.EmployeeFromDevice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("EmployeeId")
                        .HasColumnType("uuid");

                    b.Property<string>("EmployeeIdOnDevice")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("EmployeesFromDevices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Timestamp", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<TimeSpan>("DateTime")
                        .HasColumnType("interval");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Timestamp");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Device", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Agent", "Agent")
                        .WithMany("Devices")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.EmployeeFromDevice", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Device", "Device")
                        .WithMany("EmployeesFromDevices")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tempore.Storage.Entities.Employee", "Employee")
                        .WithMany("EmployeeFromDevice")
                        .HasForeignKey("EmployeeId");

                    b.Navigation("Device");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Timestamp", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Employee", "Employee")
                        .WithMany("Timestamps")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Agent", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Device", b =>
                {
                    b.Navigation("EmployeesFromDevices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Employee", b =>
                {
                    b.Navigation("EmployeeFromDevice");

                    b.Navigation("Timestamps");
                });
#pragma warning restore 612, 618
        }
    }
}
