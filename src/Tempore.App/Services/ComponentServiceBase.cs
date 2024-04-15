// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentServiceBase.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services
{
    using Blorc.Services;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// The component service base.
    /// </summary>
    /// <typeparam name="TComponent">
    /// </typeparam>
    public class ComponentServiceBase<TComponent> : IComponentService
        where TComponent : ComponentBase
    {
        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        public ComponentBase? Component { get; set; }

        /// <summary>
        /// Gets the typed component.
        /// </summary>
        public TComponent? TypedComponent => this.Component as TComponent;
    }
}