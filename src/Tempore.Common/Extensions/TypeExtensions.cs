// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Common.Extensions
{
    using System.Reflection;

    /// <summary>
    /// The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The constants.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <typeparam name="TValue">
        /// The value type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TValue}"/>.
        /// </returns>
        public static IEnumerable<TValue> Constants<TValue>(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetValue(type) is TValue value)
                {
                    yield return value;
                }
            }

            var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            foreach (var nestedType in nestedTypes)
            {
                foreach (var value in nestedType.Constants<TValue>())
                {
                    yield return value;
                }
            }
        }
    }
}