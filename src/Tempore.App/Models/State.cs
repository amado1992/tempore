// --------------------------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Models
{
    /// <summary>
    /// The State class.
    /// </summary>
    /// <typeparam name="TValue">
    /// The parameter type.
    /// </typeparam>
    public class State<TValue>
        where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="State{TValue}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="text">
        /// The state.
        /// </param>
        public State(TValue value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TValue? Value { get; }

        /// <summary>
        /// Gets state.
        /// </summary>
        public string Text { get; }
    }
}