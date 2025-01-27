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
    [Migration("20231025024234_Adds_ScheduledShift_And_ScheduledShiftAssignment")]
    partial class Adds_ScheduledShift_And_ScheduledShiftAssignment
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
                        .HasColumnType("text");

                    b.Property<string>("MacAddress")
                        .HasColumnType("text");

                    b.Property<string>("Model")
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

                    b.HasIndex("DeviceName");

                    b.HasIndex("MacAddress")
                        .IsUnique()
                        .HasFilter("\"MacAddress\" IS NOT NULL");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("SerialNumber")
                        .IsUnique()
                        .HasFilter("\"SerialNumber\" IS NOT NULL");

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

                    b.Property<DateTimeOffset?>("CheckInEndDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("CheckInStartDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("CheckOutEndDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("CheckOutStartDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<Guid>("DayId")
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("EffectiveWorkingTime")
                        .HasColumnType("interval");

                    b.Property<DateTimeOffset?>("EndDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeSpan>("RelativeEffectiveWorkingTime")
                        .HasColumnType("interval");

                    b.Property<Guid>("ShiftAssignmentId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("StartDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DayId");

                    b.HasIndex("ShiftAssignmentId");

                    b.HasIndex("Date", "ShiftAssignmentId")
                        .IsUnique();

                    b.ToTable("ScheduledDays");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledShift", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<TimeSpan>("EffectiveWorkingTime")
                        .HasColumnType("interval");

                    b.Property<DateOnly>("ExpireDate")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("LastGeneratedDayDate")
                        .HasColumnType("date");

                    b.Property<Guid>("ShiftId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("ShiftId");

                    b.ToTable("ScheduledShifts");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledShiftAssignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<TimeSpan>("EffectiveWorkingTime")
                        .HasColumnType("interval");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("ExpireDate")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("LastGeneratedDayDate")
                        .HasColumnType("date");

                    b.Property<Guid?>("ScheduledShiftId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ShiftId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("ScheduledShiftId");

                    b.HasIndex("ShiftId");

                    b.HasIndex("EmployeeId", "ShiftId", "StartDate")
                        .IsUnique();

                    b.ToTable("ScheduledShiftAssignments");
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

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Shifts");
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

                    b.Property<TimeSpan>("CheckInTimeDuration")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("CheckInTimeStart")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("CheckOutTimeDuration")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("CheckOutTimeStart")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("EffectiveWorkingTime")
                        .HasColumnType("interval");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Timetables");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.WorkforceMetric", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("WorkforceMetricCollectionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("WorkforceMetricCollectionId");

                    b.ToTable("WorkforceMetrics");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.WorkforceMetricCollection", b =>
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

                    b.ToTable("WorkforceMetricCollections");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.WorkforceMetricDailySnapshot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("ScheduledDayId")
                        .HasColumnType("uuid");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.Property<Guid>("WorkforceMetricId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WorkforceMetricId");

                    b.HasIndex("ScheduledDayId", "WorkforceMetricId")
                        .IsUnique();

                    b.ToTable("WorkforceMetricDailySnapshots");
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

                    b.HasOne("Tempore.Storage.Entities.ScheduledShiftAssignment", "ShiftAssignment")
                        .WithMany("ScheduledDays")
                        .HasForeignKey("ShiftAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Day");

                    b.Navigation("ShiftAssignment");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledShift", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("ShiftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shift");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledShiftAssignment", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.Employee", "Employee")
                        .WithMany("ShiftAssignments")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tempore.Storage.Entities.ScheduledShift", "ScheduledShift")
                        .WithMany("ScheduledShiftAssignments")
                        .HasForeignKey("ScheduledShiftId");

                    b.HasOne("Tempore.Storage.Entities.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("ShiftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("ScheduledShift");

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
                        .WithMany("Timestamps")
                        .HasForeignKey("ScheduledDayId");

                    b.Navigation("EmployeeFromDevice");

                    b.Navigation("ScheduledDay");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.WorkforceMetric", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.WorkforceMetricCollection", "WorkforceMetricCollection")
                        .WithMany("WorkforceMetrics")
                        .HasForeignKey("WorkforceMetricCollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WorkforceMetricCollection");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.WorkforceMetricDailySnapshot", b =>
                {
                    b.HasOne("Tempore.Storage.Entities.ScheduledDay", "ScheduledDay")
                        .WithMany()
                        .HasForeignKey("ScheduledDayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tempore.Storage.Entities.WorkforceMetric", "WorkforceMetric")
                        .WithMany()
                        .HasForeignKey("WorkforceMetricId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduledDay");

                    b.Navigation("WorkforceMetric");
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

                    b.Navigation("ShiftAssignments");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.EmployeeFromDevice", b =>
                {
                    b.Navigation("Timestamps");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledDay", b =>
                {
                    b.Navigation("Timestamps");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledShift", b =>
                {
                    b.Navigation("ScheduledShiftAssignments");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.ScheduledShiftAssignment", b =>
                {
                    b.Navigation("ScheduledDays");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Shift", b =>
                {
                    b.Navigation("Days");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.Timetable", b =>
                {
                    b.Navigation("Days");
                });

            modelBuilder.Entity("Tempore.Storage.Entities.WorkforceMetricCollection", b =>
                {
                    b.Navigation("WorkforceMetrics");
                });
#pragma warning restore 612, 618
        }
    }
}
