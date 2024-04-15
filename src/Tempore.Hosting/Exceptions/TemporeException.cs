// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporeException.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Hosting.Exceptions
{
    public class TemporeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemporeException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public TemporeException(string? message)
            : base(message)
        {
        }
    }
}