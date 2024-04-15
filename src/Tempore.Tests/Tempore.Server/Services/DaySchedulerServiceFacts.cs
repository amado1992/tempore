namespace Tempore.Tests.Tempore.Server.Services;

extern alias TemporeServer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using global::Tempore.Storage.Entities;
using global::Tempore.Tests.Infraestructure;

using Microsoft.Extensions.Logging.Abstractions;

using TemporeServer::Tempore.Server.Services;

using Xunit;

public class DaySchedulerServiceFacts
{
    public class The_ScheduleDayAsync_Method
    {
        public static object[] MorningShift()
        {
            var timetable = new Timetable
            {
                Id = Guid.NewGuid(),
                Name = nameof(MorningShift),
                StartTime = new TimeSpan(8, 0, 0),
                Duration = new TimeSpan(8, 0, 0),
                CheckInTimeStart = new TimeSpan(0, -15, 0),
                CheckInTimeDuration = new TimeSpan(0, 30, 0),
                CheckOutTimeStart = new TimeSpan(0, -15, 0),
                CheckOutTimeDuration = new TimeSpan(0, 30, 0),
                EffectiveWorkingTime = TimeSpan.FromHours(8),
            };

            var shiftId = Guid.NewGuid();
            var scheduledShiftAssignment = new ScheduledShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                ScheduledShift = new ScheduledShift
                {
                    ShiftId = shiftId,
                    StartDate = new DateOnly(2023, 10, 2),
                    ExpireDate = new DateOnly(2023, 10, 2).AddDays(13),
                    Shift = new Shift
                    {
                        Id = shiftId,
                        Name = nameof(MorningShift),
                    },
                    EffectiveWorkingTime = TimeSpan.FromHours(8 * 12),
                },
            };

            var dayList = new List<Day>();
            for (var dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
            {
                var day = new Day
                {
                    Id = Guid.NewGuid(),
                    Index = (int)dayOfWeek,
                    TimetableId = dayOfWeek > DayOfWeek.Sunday ? timetable.Id : null,
                    Timetable = dayOfWeek > DayOfWeek.Sunday ? timetable : null,
                    Shift = scheduledShiftAssignment.ScheduledShift.Shift,
                    ShiftId = scheduledShiftAssignment.ScheduledShift.ShiftId,
                };

                dayList.Add(day);
            }

            scheduledShiftAssignment.ScheduledShift.Shift.Days = dayList;

            var date = new DateOnly(2023, 10, 12);
            var index = ((int)scheduledShiftAssignment.ScheduledShift.StartDate.DayOfWeek + date.DayNumber - scheduledShiftAssignment.ScheduledShift.StartDate.DayNumber) % scheduledShiftAssignment.ScheduledShift.Shift.Days.Count;
            var first = scheduledShiftAssignment.ScheduledShift.Shift.Days.First(d => d.Index == index);
            var expectedScheduledDay = new ScheduledDay
            {
                Date = date,
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                DayId = first.Id,
                StartDateTime = new DateTime(2023, 10, 12, 8, 0, 0),
                EndDateTime = new DateTime(2023, 10, 12, 16, 0, 0),
                CheckInStartDateTime = new DateTime(2023, 10, 12, 7, 45, 0),
                CheckInEndDateTime = new DateTime(2023, 10, 12, 8, 15, 0),
                CheckOutStartDateTime = new DateTime(2023, 10, 12, 15, 45, 0),
                CheckOutEndDateTime = new DateTime(2023, 10, 12, 16, 15, 0),
                EffectiveWorkingTime = first.Timetable?.EffectiveWorkingTime ?? TimeSpan.Zero,
                RelativeEffectiveWorkingTime = TimeSpan.FromHours(8),
            };

            return new object[] { date, scheduledShiftAssignment, expectedScheduledDay };
        }

        public static object[] AfternoonShift()
        {
            var timetable = new Timetable
            {
                Id = Guid.NewGuid(),
                Name = nameof(AfternoonShift),
                StartTime = new TimeSpan(12, 0, 0),
                Duration = TimeSpan.FromHours(6),
                CheckInTimeStart = TimeSpan.FromMinutes(-10),
                CheckInTimeDuration = TimeSpan.FromMinutes(20),
                CheckOutTimeStart = TimeSpan.FromMinutes(-5),
                CheckOutTimeDuration = TimeSpan.FromMinutes(10),
                EffectiveWorkingTime = TimeSpan.FromHours(6),
            };

            var shiftId = Guid.NewGuid();
            var scheduledShiftAssignment = new ScheduledShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                ScheduledShift = new ScheduledShift
                {
                    ShiftId = shiftId,
                    StartDate = new DateOnly(2023, 10, 2),
                    ExpireDate = new DateOnly(2023, 10, 2).AddDays(6),
                    Shift = new Shift
                    {
                        Id = shiftId,
                        Name = nameof(AfternoonShift),
                    },
                    EffectiveWorkingTime = TimeSpan.FromHours(6 * 6),
                },
            };

            var dayList = new List<Day>();
            for (var dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
            {
                var day = new Day
                {
                    Id = Guid.NewGuid(),
                    Index = (int)dayOfWeek,
                    TimetableId = dayOfWeek > DayOfWeek.Sunday ? timetable.Id : null,
                    Timetable = dayOfWeek > DayOfWeek.Sunday ? timetable : null,
                    Shift = scheduledShiftAssignment.ScheduledShift.Shift,
                    ShiftId = scheduledShiftAssignment.ScheduledShift.ShiftId,
                };

                dayList.Add(day);
            }

            scheduledShiftAssignment.ScheduledShift.Shift.Days = dayList;

            var date = new DateOnly(2023, 10, 5);
            var index = ((int)scheduledShiftAssignment.ScheduledShift.StartDate.DayOfWeek + date.DayNumber - scheduledShiftAssignment.ScheduledShift.StartDate.DayNumber) % scheduledShiftAssignment.ScheduledShift.Shift.Days.Count;
            var first = scheduledShiftAssignment.ScheduledShift.Shift.Days.First(d => d.Index == index);
            var expectedScheduledDay = new ScheduledDay
            {
                Date = date,
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                DayId = first.Id,
                StartDateTime = new DateTime(2023, 10, 5, 12, 0, 0),
                EndDateTime = new DateTime(2023, 10, 5, 18, 0, 0),
                CheckInStartDateTime = new DateTime(2023, 10, 5, 11, 50, 0),
                CheckInEndDateTime = new DateTime(2023, 10, 5, 12, 10, 0),
                CheckOutStartDateTime = new DateTime(2023, 10, 5, 17, 55, 0),
                CheckOutEndDateTime = new DateTime(2023, 10, 5, 18, 5, 0),
                EffectiveWorkingTime = first.Timetable?.EffectiveWorkingTime ?? TimeSpan.Zero,
                RelativeEffectiveWorkingTime = TimeSpan.FromHours(6),
            };

            return new object[] { date, scheduledShiftAssignment, expectedScheduledDay };
        }

        public static object[] NightShift()
        {
            var timetable = new Timetable
            {
                Id = Guid.NewGuid(),
                Name = nameof(NightShift),
                StartTime = new TimeSpan(20, 0, 0),
                Duration = TimeSpan.FromHours(7),
                CheckInTimeStart = new TimeSpan(0, -5, 0),
                CheckInTimeDuration = new TimeSpan(0, 10, 0),
                CheckOutTimeStart = new TimeSpan(0, -5, 0),
                CheckOutTimeDuration = new TimeSpan(0, 10, 0),
                EffectiveWorkingTime = TimeSpan.FromHours(7),
            };

            var shiftId = Guid.NewGuid();
            var scheduledShiftAssignment = new ScheduledShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                ScheduledShift = new ScheduledShift
                {
                    ShiftId = shiftId,
                    StartDate = new DateOnly(2023, 10, 2),
                    ExpireDate = new DateOnly(2023, 10, 2).AddDays(20),
                    Shift = new Shift
                    {
                        Id = shiftId,
                        Name = nameof(NightShift),
                    },
                    EffectiveWorkingTime = TimeSpan.FromHours(126),
                },
            };

            var dayList = new List<Day>();
            for (var dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
            {
                var day = new Day
                {
                    Id = Guid.NewGuid(),
                    Index = (int)dayOfWeek,
                    TimetableId = dayOfWeek > DayOfWeek.Sunday ? timetable.Id : null,
                    Timetable = dayOfWeek > DayOfWeek.Sunday ? timetable : null,
                    Shift = scheduledShiftAssignment.ScheduledShift.Shift,
                    ShiftId = scheduledShiftAssignment.ScheduledShift.ShiftId,
                };

                dayList.Add(day);
            }

            scheduledShiftAssignment.ScheduledShift.Shift.Days = dayList;

            var date = new DateOnly(2023, 10, 14);
            var index = ((int)scheduledShiftAssignment.ScheduledShift.StartDate.DayOfWeek + date.DayNumber - scheduledShiftAssignment.ScheduledShift.StartDate.DayNumber) % scheduledShiftAssignment.ScheduledShift.Shift.Days.Count;
            var expectedScheduledDay = new ScheduledDay
            {
                Date = date,
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                DayId = scheduledShiftAssignment.ScheduledShift.Shift.Days.First(d => d.Index == index).Id,
                StartDateTime = new DateTime(2023, 10, 14, 20, 0, 0),
                EndDateTime = new DateTime(2023, 10, 15, 3, 0, 0),
                CheckInStartDateTime = new DateTime(2023, 10, 14, 19, 55, 0),
                CheckInEndDateTime = new DateTime(2023, 10, 14, 20, 05, 0),
                CheckOutStartDateTime = new DateTime(2023, 10, 15, 2, 55, 0),
                CheckOutEndDateTime = new DateTime(2023, 10, 15, 3, 5, 0),
                EffectiveWorkingTime = TimeSpan.FromHours(7),
                RelativeEffectiveWorkingTime = TimeSpan.FromHours(7),
            };

            return new object[] { date, scheduledShiftAssignment, expectedScheduledDay };
        }

        public static object[] NoWorkingDayShift()
        {
            var timetable = new Timetable
            {
                Id = Guid.NewGuid(),
                Name = nameof(NoWorkingDayShift),
                StartTime = new TimeSpan(8, 0, 0),
                Duration = new TimeSpan(8, 0, 0),
                CheckInTimeStart = new TimeSpan(0, -15, 0),
                CheckInTimeDuration = new TimeSpan(0, 30, 0),
                CheckOutTimeStart = new TimeSpan(0, -15, 0),
                CheckOutTimeDuration = new TimeSpan(0, 30, 0),
            };

            var shiftId = Guid.NewGuid();
            var scheduledShiftAssignment = new ScheduledShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                ScheduledShift = new ScheduledShift()
                {
                    ShiftId = shiftId,
                    StartDate = new DateOnly(2023, 10, 2),
                    ExpireDate = new DateOnly(2023, 10, 2).AddDays(25),
                    Shift = new Shift
                    {
                        Id = shiftId,
                        Name = nameof(NoWorkingDayShift),
                    },
                },
            };

            var dayList = new List<Day>();
            for (var dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
            {
                var day = new Day
                {
                    Id = Guid.NewGuid(),
                    Index = (int)dayOfWeek,
                    TimetableId = dayOfWeek > DayOfWeek.Sunday ? timetable.Id : null,
                    Timetable = dayOfWeek > DayOfWeek.Sunday ? timetable : null,
                    Shift = scheduledShiftAssignment.ScheduledShift.Shift,
                    ShiftId = scheduledShiftAssignment.ScheduledShift.ShiftId,
                };

                dayList.Add(day);
            }

            scheduledShiftAssignment.ScheduledShift.Shift.Days = dayList;

            var date = new DateOnly(2023, 10, 15);
            var index = ((int)scheduledShiftAssignment.ScheduledShift.StartDate.DayOfWeek + date.DayNumber - scheduledShiftAssignment.ScheduledShift.StartDate.DayNumber) % scheduledShiftAssignment.ScheduledShift.Shift.Days.Count;
            var expectedScheduledDay = new ScheduledDay
            {
                Date = date,
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                DayId = scheduledShiftAssignment.ScheduledShift.Shift.Days.First(d => d.Index == index).Id,
            };

            return new object[] { date, scheduledShiftAssignment, expectedScheduledDay };
        }

        public static IEnumerable<object[]> RealisticShifts()
        {
            var weekdaysTimetable = new Timetable
            {
                Id = Guid.NewGuid(),
                Name = DefaultValues.WeekdaysTimetableName,
                StartTime = new TimeSpan(8, 0, 0),
                Duration = TimeSpan.FromHours(9),
                CheckInTimeStart = new TimeSpan(0, -15, 0),
                CheckInTimeDuration = new TimeSpan(0, 30, 0),
                CheckOutTimeStart = new TimeSpan(0, -15, 0),
                CheckOutTimeDuration = new TimeSpan(0, 30, 0),
                EffectiveWorkingTime = TimeSpan.FromHours(8),
            };

            var saturdayTimetable = new Timetable
            {
                Id = Guid.NewGuid(),
                Name = DefaultValues.SaturdayTimetableName,
                StartTime = new TimeSpan(8, 0, 0),
                Duration = TimeSpan.FromHours(5),
                CheckInTimeStart = new TimeSpan(0, -15, 0),
                CheckInTimeDuration = new TimeSpan(0, 30, 0),
                CheckOutTimeStart = new TimeSpan(0, -15, 0),
                CheckOutTimeDuration = new TimeSpan(0, 30, 0),
                EffectiveWorkingTime = TimeSpan.FromHours(5),
            };

            var shiftId = Guid.NewGuid();
            var scheduledShiftAssignment = new ScheduledShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                ScheduledShift = new ScheduledShift
                {
                    ShiftId = shiftId,
                    StartDate = new DateOnly(2023, 10, 2),
                    ExpireDate = new DateOnly(2023, 10, 2).AddDays(13),
                    Shift = new Shift
                    {
                        Id = shiftId,
                        Name = nameof(RealisticShifts),
                    },
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                },
            };

            var dayList = new List<Day>();
            for (var dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
            {
                var day = new Day
                {
                    Id = Guid.NewGuid(),
                    Index = (int)dayOfWeek,
                    Shift = scheduledShiftAssignment.ScheduledShift.Shift,
                    ShiftId = scheduledShiftAssignment.ScheduledShift.ShiftId,
                };

                if (dayOfWeek == DayOfWeek.Saturday)
                {
                    day.Timetable = saturdayTimetable;
                    day.TimetableId = saturdayTimetable.Id;
                }

                if (dayOfWeek is >= DayOfWeek.Monday and < DayOfWeek.Saturday)
                {
                    day.Timetable = weekdaysTimetable;
                    day.TimetableId = weekdaysTimetable.Id;
                }

                dayList.Add(day);
            }

            scheduledShiftAssignment.ScheduledShift.Shift.Days = dayList;

            var date = new DateOnly(2023, 10, 12);
            var index = ((int)scheduledShiftAssignment.ScheduledShift.StartDate.DayOfWeek + date.DayNumber - scheduledShiftAssignment.ScheduledShift.StartDate.DayNumber) % scheduledShiftAssignment.ScheduledShift.Shift.Days.Count;
            var firstDay = scheduledShiftAssignment.ScheduledShift.Shift.Days.First(d => d.Index == index);
            var expectedScheduledDay = new ScheduledDay
            {
                Date = date,
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                DayId = firstDay.Id,
                StartDateTime = new DateTime(2023, 10, 12, 8, 0, 0),
                EndDateTime = new DateTime(2023, 10, 12, 17, 0, 0),
                CheckInStartDateTime = new DateTime(2023, 10, 12, 7, 45, 0),
                CheckInEndDateTime = new DateTime(2023, 10, 12, 8, 15, 0),
                CheckOutStartDateTime = new DateTime(2023, 10, 12, 16, 45, 0),
                CheckOutEndDateTime = new DateTime(2023, 10, 12, 17, 15, 0),
                EffectiveWorkingTime = firstDay.Timetable?.EffectiveWorkingTime ?? TimeSpan.Zero,
                RelativeEffectiveWorkingTime = TimeSpan.FromHours(8.69067),
            };

            yield return new object[] { date, scheduledShiftAssignment, expectedScheduledDay };

            date = new DateOnly(2023, 10, 7);
            index = ((int)scheduledShiftAssignment.ScheduledShift.StartDate.DayOfWeek + date.DayNumber - scheduledShiftAssignment.ScheduledShift.StartDate.DayNumber) % scheduledShiftAssignment.ScheduledShift.Shift.Days.Count;
            firstDay = scheduledShiftAssignment.ScheduledShift.Shift.Days.First(d => d.Index == index);
            expectedScheduledDay = new ScheduledDay
            {
                Date = date,
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                DayId = firstDay.Id,
                StartDateTime = new DateTime(2023, 10, 7, 8, 0, 0),
                EndDateTime = new DateTime(2023, 10, 7, 13, 0, 0),
                CheckInStartDateTime = new DateTime(2023, 10, 7, 7, 45, 0),
                CheckInEndDateTime = new DateTime(2023, 10, 7, 8, 15, 0),
                CheckOutStartDateTime = new DateTime(2023, 10, 7, 12, 45, 0),
                CheckOutEndDateTime = new DateTime(2023, 10, 7, 13, 15, 0),
                EffectiveWorkingTime = firstDay.Timetable?.EffectiveWorkingTime ?? TimeSpan.Zero,
                RelativeEffectiveWorkingTime = TimeSpan.FromHours(5.43167),
            };

            yield return new object[] { date, scheduledShiftAssignment, expectedScheduledDay };
        }

        public static IEnumerable<object[]> Data()
        {
            yield return MorningShift();
            yield return AfternoonShift();
            yield return NightShift();
            yield return NoWorkingDayShift();

            foreach (var realisticShift in RealisticShifts())
            {
                yield return realisticShift;
            }
        }

        public static IEnumerable<object[]> OutOfShiftAssignmentDateRageData()
        {
            var shiftAssignment = new ScheduledShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                ScheduledShift = new ScheduledShift
                {
                    ShiftId = Guid.NewGuid(),
                    StartDate = new DateOnly(2023, 10, 2),
                    ExpireDate = new DateOnly(2023, 10, 10),
                },
            };

            yield return new object[] { shiftAssignment.ScheduledShift.ExpireDate.AddDays(1), shiftAssignment };
            yield return new object[] { shiftAssignment.ScheduledShift.StartDate.AddDays(-1), shiftAssignment };
        }

        [Theory]
        [MemberData(nameof(Data))]
        [Trait(Traits.Category, Category.Unit)]
        public async Task Creates_A_ScheduledDay_With_The_Expected_DateTime_Values_Async(
               DateOnly date, ScheduledShiftAssignment scheduledShiftAssignment, ScheduledDay expectedScheduledDay)
        {
            var daySchedulerService = new DaySchedulerService(NullLogger<DaySchedulerService>.Instance);
            var scheduledDay = await daySchedulerService.ScheduleDayAsync(date, scheduledShiftAssignment);

            scheduledDay.Should().BeEquivalentTo(
                       expectedScheduledDay,
                       options => options.Excluding(s => s.Id).Excluding(s => s.Day).Excluding(s => s.Timestamps)
                           .Excluding(s => s.ScheduledShiftAssignment).Excluding(s => s.RelativeEffectiveWorkingTime));

            scheduledDay.RelativeEffectiveWorkingTime.TotalHours.Should().BeApproximately(
                       expectedScheduledDay.RelativeEffectiveWorkingTime.TotalHours,
                       0.001);
        }

        [Theory]
        [MemberData(nameof(OutOfShiftAssignmentDateRageData))]
        [Trait(Traits.Category, Category.Unit)]
        public async Task Throws_ArgumentException_Async(DateOnly date, ScheduledShiftAssignment scheduledShiftAssignment)
        {
            var daySchedulerService = new DaySchedulerService(NullLogger<DaySchedulerService>.Instance);
            await Assert.ThrowsAsync<ArgumentException>(
                () => daySchedulerService.ScheduleDayAsync(date, scheduledShiftAssignment));
        }

        [Fact]
        [Trait(Traits.Category, Category.Unit)]
        public async Task Throws_ArgumentNullException_When_ShiftAssignment_Argument_Is_Null_Async()
        {
            var daySchedulerService = new DaySchedulerService(NullLogger<DaySchedulerService>.Instance);
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => daySchedulerService.ScheduleDayAsync(new DateOnly(2023, 12, 12), null!));
        }
    }
}