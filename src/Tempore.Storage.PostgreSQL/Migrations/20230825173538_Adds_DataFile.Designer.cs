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
    [Migration("20230825173538_Adds_DataFile")]
    partial class Adds_DataFile
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tempore.Storage.Entities.Agent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.DataFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FileType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DataFiles");
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
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("MacAddress");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("SerialNumber");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("AdmissionDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<float>("BaseHours")
                        .HasColumnType("real");

                    b.Property<string>("CostCenter")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IdentificationCard")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SocialSecurity")
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
