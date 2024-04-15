namespace Tempore.Tests.Tempore.Server.Invokables.ScheduledDay
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client.Services.Interfaces;
    using global::Tempore.Storage;
    using global::Tempore.Storage.Entities;
    using global::Tempore.Tests.Infraestructure;

    using MediatR;

    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using TemporeServer::Tempore.Server.Invokables.Interfaces;
    using TemporeServer::Tempore.Server.Invokables.ScheduledDay;
    using TemporeServer::Tempore.Server.Requests.ScheduledDay;
    using TemporeServer::Tempore.Server.Services;
    using TemporeServer::Tempore.Server.Services.Interfaces;
    using TemporeServer::Tempore.Server.Specs.ScheduledShift;
    using TemporeServer::Tempore.Server.Specs.ShiftAssignment;

    using Xunit;

    using ScheduleDaysProcessCompletedNotification = TemporeServer::Tempore.Server.Notifications.ScheduledDay.ScheduleDaysProcessCompletedNotification;

    public partial class DaySchedulerInvokableFacts
    {
        public class The_Invoke_Method
        {
            public static ScheduledShiftAssignment MorningShiftAssignment()
            {
                var now = new DateTimeService().Today().AddDays(5);
                var timetable = new Timetable
                {
                    Id = Guid.NewGuid(),
                    Name = "TimeTable Morning",
                    StartTime = new TimeSpan(8, 0, 0),
                    Duration = new TimeSpan(8, 0, 0),
                    CheckInTimeStart = new TimeSpan(0, -15, 0),
                    CheckInTimeDuration = new TimeSpan(0, 30, 0),
                    CheckOutTimeStart = new TimeSpan(0, -15, 0),
                    CheckOutTimeDuration = new TimeSpan(0, 30, 0),
                };

                var shiftId = Guid.NewGuid();
                var hours = 97.77m;
                var scheduledShiftAssignment = new ScheduledShiftAssignment
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = Guid.NewGuid(),
                    ScheduledShift = new ScheduledShift()
                    {
                        ShiftId = shiftId,
                        StartDate = new DateOnly(2023, 10, 2),
                        EffectiveWorkingTime = TimeSpan.FromHours((double)hours),
                        Shift = new Shift
                        {
                            Id = shiftId,
                            Name = "Shift Morning",
                        },
                    },
                    LastGeneratedDayDate = new DateOnly(2023, 10, 5),
                };

                scheduledShiftAssignment.ScheduledShift.ExpireDate = now;

                Day? firstDay = null;
                var dayList = new List<Day>();
                for (var i = 0; i < 7; i++)
                {
                    var day = new Day
                    {
                        Id = Guid.NewGuid(),
                        Index = i,
                        TimetableId = i < 6 ? timetable.Id : null,
                        Timetable = i < 6 ? timetable : null,
                        Shift = scheduledShiftAssignment.ScheduledShift.Shift,
                        ShiftId = scheduledShiftAssignment.ScheduledShift.ShiftId,
                    };

                    firstDay ??= day;
                    dayList.Add(day);
                }

                scheduledShiftAssignment.ScheduledShift.Shift.Days = dayList;

                return scheduledShiftAssignment;
            }

            public static ScheduledShiftAssignment AfternoonShiftAssignment()
            {
                var now = new DateTimeService().Today().AddDays(5);
                var timetable = new Timetable
                {
                    Id = Guid.NewGuid(),
                    Name = "TimeTable Afternoon",
                    StartTime = new TimeSpan(12, 0, 0),
                    Duration = new TimeSpan(6, 0, 0),
                    CheckInTimeStart = new TimeSpan(0, -10, 0),
                    CheckInTimeDuration = new TimeSpan(0, 20, 0),
                    CheckOutTimeStart = new TimeSpan(0, -5, 0),
                    CheckOutTimeDuration = new TimeSpan(0, 10, 0),
                };

                var shiftId = Guid.NewGuid();
                var hours = 97.77m;
                var shiftAssignment = new ScheduledShiftAssignment
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = Guid.NewGuid(),
                    ScheduledShift = new ScheduledShift()
                    {
                        ShiftId = shiftId,
                        StartDate = new DateOnly(2023, 9, 14),
                        EffectiveWorkingTime = TimeSpan.FromHours((double)hours),
                        Shift = new Shift
                        {
                            Id = shiftId,
                            Name = "Shift afternoon",
                        },
                    },
                    LastGeneratedDayDate = new DateOnly(2023, 10, 22),
                };
                shiftAssignment.ScheduledShift.ExpireDate = now;

                Day? firstDay = null;
                var dayList = new List<Day>();
                for (var i = 0; i < 7; i++)
                {
                    var day = new Day
                    {
                        Id = Guid.NewGuid(),
                        Index = i,
                        TimetableId = i < 6 ? timetable.Id : null,
                        Timetable = i < 6 ? timetable : null,
                        Shift = shiftAssignment.ScheduledShift.Shift,
                        ShiftId = shiftAssignment.ScheduledShift.ShiftId,
                    };

                    firstDay ??= day;
                    dayList.Add(day);
                }

                shiftAssignment.ScheduledShift.Shift.Days = dayList;

                return shiftAssignment;
            }

            public static IEnumerable<object[]> Data()
            {
                yield return new object[] { MorningShiftAssignment() };
                yield return new object[] { AfternoonShiftAssignment() };
            }

            [Theory]
            [MemberData(nameof(Data))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Save_A_ScheduledDay_At_Database_AtLeastOnce_Async(ScheduledShiftAssignment scheduledShiftAssignment)
            {
                var scheduledDayRepositoryMock = new Mock<IRepository<ScheduledDay, ApplicationDbContext>>();
                var scheduledShiftRepositoryMock = new Mock<IRepository<ScheduledShift, ApplicationDbContext>>();
                var scheduledShiftAssignmentRepositoryMock = new Mock<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();
                var dayScheduledService = new DaySchedulerService(NullLogger<DaySchedulerService>.Instance);
                var dayTimeServiceMock = new Mock<IDateTimeService>();
                dayTimeServiceMock.Setup(service => service.Now()).Returns(DateTime.Now);

                var shiftAssignments = new List<ScheduledShiftAssignment> { scheduledShiftAssignment, };
                scheduledShiftRepositoryMock.Setup(repository => repository.SingleAsync(It.IsAny<ScheduledShiftByIdSpec>()))
                    .ReturnsAsync(new ScheduledShift { ScheduledShiftAssignments = shiftAssignments });

                var publisherMock = new Mock<IPublisher>();

                var scheduledDayInvokable = new DaySchedulerInvokable(
                               NullLogger<DaySchedulerInvokable>.Instance,
                               scheduledDayRepositoryMock.Object,
                               scheduledShiftRepositoryMock.Object,
                               scheduledShiftAssignmentRepositoryMock.Object,
                               dayScheduledService,
                               publisherMock.Object);

                var request = new ScheduleDaysRequest
                {
                    ScheduledShiftId = Guid.NewGuid(),
                };

                var mock = new Mock<IInvocationContext<ScheduleDaysRequest>>();
                mock.SetupGet(invocationContext => invocationContext.Request).Returns(request);
                scheduledDayInvokable.Payload = mock.Object;

                await scheduledDayInvokable.Invoke();

                scheduledDayRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
            }

            [Theory(Skip = "This is not working properly")]
            [MemberData(nameof(Data))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Count_A_ScheduledDay_Save_At_Database_Async(ScheduledShiftAssignment scheduledShiftAssignment)
            {
                var scheduledShiftRepositoryMock = new Mock<IRepository<ScheduledShift, ApplicationDbContext>>();
                var scheduledDayRepositoryMock = new Mock<IRepository<ScheduledDay, ApplicationDbContext>>();
                var shiftAssignmentRepositoryMock = new Mock<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();
                var dayScheduledService = new DaySchedulerService(NullLogger<DaySchedulerService>.Instance);

                var shiftAssignments = new List<ScheduledShiftAssignment> { scheduledShiftAssignment, };
                scheduledShiftRepositoryMock.Setup(repository => repository.SingleAsync(It.IsAny<ScheduledShiftByIdSpec>()))
                    .ReturnsAsync(new ScheduledShift { ScheduledShiftAssignments = shiftAssignments });

                var lastGeneratedDayDate = scheduledShiftAssignment.LastGeneratedDayDate;

                var publisherMock = new Mock<IPublisher>();

                var saveScheduledDayInvokable = new DaySchedulerInvokable(
                               NullLogger<DaySchedulerInvokable>.Instance,
                               scheduledDayRepositoryMock.Object,
                               scheduledShiftRepositoryMock.Object,
                               shiftAssignmentRepositoryMock.Object,
                               dayScheduledService,
                               publisherMock.Object);

                var request = new ScheduleDaysRequest
                {
                    ScheduledShiftId = Guid.NewGuid(),
                };

                var payloadMock = new Mock<IInvocationContext<ScheduleDaysRequest>>();
                payloadMock.SetupGet(context => context.Request).Returns(request);
                saveScheduledDayInvokable.Payload = payloadMock.Object;
                await saveScheduledDayInvokable.Invoke();

                lastGeneratedDayDate.Should().NotBeNull();

                if (lastGeneratedDayDate != null)
                {
                    var nowDayNumber = new DateTimeService().Today().DayNumber;
                    var possibleDayGenerated = nowDayNumber - lastGeneratedDayDate.Value.DayNumber;
                    scheduledDayRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Exactly(possibleDayGenerated), $"{possibleDayGenerated} had to be generated");
                }
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Does_Not_Get_Stuck_In_An_Infinity_Loop_In_Case_Of_Error_And_Notifies_The_Error_Via_Publisher_Async()
            {
                var scheduledShiftRepositoryMock = new Mock<IRepository<ScheduledShift, ApplicationDbContext>>();
                var scheduledDayRepositoryMock = new Mock<IRepository<ScheduledDay, ApplicationDbContext>>();
                var shiftAssignmentRepositoryMock = new Mock<IRepository<ScheduledShiftAssignment, ApplicationDbContext>>();
                var dayScheduledService = new DaySchedulerService(NullLogger<DaySchedulerService>.Instance);
                var dayTimeServiceMock = new Mock<IDateTimeService>();
                dayTimeServiceMock.Setup(service => service.Now()).Returns(DateTime.Now);

                var scheduledShift = new ScheduledShift
                {
                    StartDate = new DateOnly(2025, 1, 1),
                    ExpireDate = new DateOnly(2025, 1, 15),
                    EffectiveWorkingTime = TimeSpan.FromHours(97.77),
                };

                var shiftAssignment = new ScheduledShiftAssignment
                {
                    ScheduledShift = scheduledShift,
                };

                var scheduledShiftAssignments = new List<ScheduledShiftAssignment> { shiftAssignment, };
                scheduledShift.ScheduledShiftAssignments = scheduledShiftAssignments;
                scheduledShiftRepositoryMock.Setup(repository => repository.SingleAsync(It.IsAny<ScheduledShiftByIdSpec>()))
                    .ReturnsAsync(scheduledShift);

                var publisherMock = new Mock<IPublisher>();

                var scheduledDayInvokable = new DaySchedulerInvokable(
                               NullLogger<DaySchedulerInvokable>.Instance,
                               scheduledDayRepositoryMock.Object,
                               scheduledShiftRepositoryMock.Object,
                               shiftAssignmentRepositoryMock.Object,
                               dayScheduledService,
                               publisherMock.Object);

                var request = new ScheduleDaysRequest
                {
                    ScheduledShiftId = Guid.NewGuid(),
                };

                var mock = new Mock<IInvocationContext<ScheduleDaysRequest>>();
                mock.SetupGet(invocationContext => invocationContext.Request).Returns(request);
                scheduledDayInvokable.Payload = mock.Object;

                await scheduledDayInvokable.Invoke();

                publisherMock.Verify(r => r.Publish(It.Is<ScheduleDaysProcessCompletedNotification>(notification => notification.Severity == Severity.Error), CancellationToken.None));
            }
        }
    }
}