extern alias TemporeServer;

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Moq;

public static class MockExtensions
{
    /// <summary>
    /// The wait and verify.
    /// </summary>
    /// <param name="mock">
    /// The mock.
    /// </param>
    /// <param name="timeSpan">
    /// The time span.
    /// </param>
    /// <param name="expression">
    /// The expression.
    /// </param>
    /// <param name="times">
    /// The times.
    /// </param>
    /// <typeparam name="TService">
    /// The service type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public static async Task WaitAndVerifyAsync<TService>(this Mock<TService> mock, TimeSpan timeSpan, Expression<Action<TService>> expression, Times times)
        where TService : class
    {
        Exception? exception;
        var stopwatch = Stopwatch.StartNew();
        do
        {
            await Task.Delay(100);

            try
            {
                mock.Verify(expression, times);
                return;
            }
            catch (MockException ex)
            {
                exception = ex;
            }
        }
        while (stopwatch.Elapsed < timeSpan);

        if (exception is not null)
        {
            throw exception;
        }
    }

    public static async Task WaitAndVerifyAsync<TService>(this Mock<TService> mock, TimeSpan timeSpan, Expression<Action<TService>> expression, Func<Times> times)
        where TService : class
    {
        await mock.WaitAndVerifyAsync(timeSpan, expression, times());
    }
}