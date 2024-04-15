// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Common.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> task)
        {
            return (await task).ToList();
        }
    }
}