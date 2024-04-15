namespace Tempore.Tests.Infraestructure.Helpers;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

public static class TaskHelper
{
    public static async Task RepeatAsync(Func<Task<bool>> task, TimeSpan timeout = default)
    {
        if (timeout == default)
        {
            timeout = TimeSpan.FromSeconds(60);
        }

        var stopwatch = Stopwatch.StartNew();

        bool result;
        while ((result = await task()) && stopwatch.Elapsed <= timeout)
        {
            await Task.Delay(1000);
        }

        if (result)
        {
            throw new TimeoutException();
        }
    }
}