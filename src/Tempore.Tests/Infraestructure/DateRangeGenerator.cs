namespace Tempore.Tests.Infraestructure;

using System;
using System.Collections.Generic;
using System.Linq;

public static class DateRangeGenerator
{
    private static readonly List<(DateTime StartDate, DateTime ExpireDate)> GeneratedDateRanges = new List<(DateTime StartDate, DateTime ExpireDate)>();

    private static readonly Random Random = new Random();

    public static (DateTime StartDate, DateTime ExpireDate) Next(
        int days = 15, List<(DateTime StartDate, DateTime ExpireDate)>? usedDateRanges = null)
    {
        var startDate = new DateTime(Random.Next(2020, 2030), Random.Next(1, 12), Random.Next(1, 28));
        var expireDate = startDate.AddDays(days);

        var allUsingDateRanges = GeneratedDateRanges.ToList();
        if (usedDateRanges is not null && usedDateRanges.Count > 0)
        {
            allUsingDateRanges = allUsingDateRanges.Union(allUsingDateRanges).ToList();
        }

        while (allUsingDateRanges.Exists(
                   ((DateTime StartDate, DateTime ExpireDate) tuple) =>
                       startDate <= tuple.ExpireDate && expireDate >= tuple.StartDate))
        {
            startDate = new DateTime(Random.Next(2020, 2030), Random.Next(1, 12), Random.Next(1, 28));
            expireDate = startDate.AddDays(days);
        }

        GeneratedDateRanges.Add((startDate, expireDate));

        return (startDate, expireDate);
    }
}