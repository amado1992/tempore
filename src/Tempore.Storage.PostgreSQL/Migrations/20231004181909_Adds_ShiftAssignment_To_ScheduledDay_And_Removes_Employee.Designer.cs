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
    [Migration("20231004181909_Adds_ShiftAssignment_To_ScheduledDay_And_Removes_Employee")]
    partial class Adds_ShiftAssignment_To_ScheduledDay_And_Removes_Employee
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
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

                    b.Property<DateTimeOffset>("ProcessingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ProcessingState")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DataFiles");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Day", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<Guid>("ShiftId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("TimetableId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ShiftId");

                    b.HasIndex("TimetableId");

                    b.ToTable("Days");
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

                    b.Property<DateTime?>("AdmissionDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<float?>("BaseHours")
                        .HasColumnType("real");

                    b.Property<string>("CostCenter")
                        .HasColumnType("text");

                    b.Property<string>("Department")
                        .HasColumnType("text");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IdentificationCard")
                        .HasColumnType("text");

                    b.Property<string>("SocialSecurity")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Employees");
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

                    b.Property<bool>("IsLinked")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("boolean")
                        .HasComputedColumnSql("\"EmployeeId\" IS NOT NULL", true);

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("EmployeeIdOnDevice", "DeviceId")
                        .IsUnique();

                    b.ToTable("EmployeesFromDevices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledDay", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<Guid>("DayId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("EmployeeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ShiftAssignmentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DayId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ShiftAssignmentId");

                    b.ToTable("ScheduledDays");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Shift", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<Guid?>("StartDayId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("StartDayId")
                        .IsUnique();

                    b.ToTable("Shifts");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ShiftAssignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly?>("ExpireDate")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("LastGeneratedDayDate")
                        .HasColumnType("date");

                    b.Property<Guid>("ShiftId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("ShiftId");

                    b.HasIndex("EmployeeId", "ShiftId", "StartDate")
                        .IsUnique();

                    b.ToTable("ShiftAssignments");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Timestamp", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTimeOffset>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EmployeeFromDeviceId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ScheduledDayId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ScheduledDayId");

                    b.HasIndex("EmployeeFromDeviceId", "DateTime")
                        .IsUnique();

                    b.ToTable("Timestamp");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Timetable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("ValidCheckInTimeDuration")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("ValidCheckInTimeStart")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("ValidCheckOutTimeDuration")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("ValidCheckOutTimeStart")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Timetables");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Day", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Shift", "Shift")
                        .WithMany("Days")
                        .HasForeignKey("ShiftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tempore.Storage.Entities.Timetable", "Timetable")
                        .WithMany("Days")
                        .HasForeignKey("TimetableId");

                    b.Navigation("Shift");

                    b.Navigation("Timetable");
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

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledDay", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Day", "Day")
                        .WithMany("ScheduledDays")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tempore.Storage.Entities.Employee", null)
                        .WithMany("ScheduledDays")
                        .HasForeignKey("EmployeeId");

                    b.HasOne("Tempore.Storage.Entities.ShiftAssignment", "ShiftAssignment")
                        .WithMany("ScheduledDays")
                        .HasForeignKey("ShiftAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Day");

                    b.Navigation("ShiftAssignment");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Shift", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Day", "StartDay")
                        .WithOne()
                        .HasForeignKey("Tempore.Storage.Entities.Shift", "StartDayId");

                    b.Navigation("StartDay");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ShiftAssignment", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tempore.Storage.Entities.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("ShiftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Shift");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Timestamp", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.EmployeeFromDevice", "EmployeeFromDevice")
                        .WithMany("Timestamps")
                        .HasForeignKey("EmployeeFromDeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tempore.Storage.Entities.ScheduledDay", "ScheduledDay")
                        .WithMany("Timestamp")
                        .HasForeignKey("ScheduledDayId");

                    b.Navigation("EmployeeFromDevice");

                    b.Navigation("ScheduledDay");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Agent", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Day", b =>
                {
                    b.Navigation("ScheduledDays");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Device", b =>
                {
                    b.Navigation("EmployeesFromDevices");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Employee", b =>
                {
                    b.Navigation("EmployeeFromDevice");

                    b.Navigation("ScheduledDays");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.EmployeeFromDevice", b =>
                {
                    b.Navigation("Timestamps");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledDay", b =>
                {
                    b.Navigation("Timestamp");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Shift", b =>
                {
                    b.Navigation("Days");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ShiftAssignment", b =>
                {
                    b.Navigation("ScheduledDays");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Timetable", b =>
                {
                    b.Navigation("Days");
                });
#pragma warning restore 612, 618
        }
    }
}
