// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Confirmation.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Dialogs
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Services.Interfaces;

    /// <summary>
    /// Confirmation dialog.
    /// </summary>
    public partial class Confirmation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Confirmation"/> class.
        /// </summary>
        public Confirmation()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets ContentTex.
        /// </summary>
        [Parameter]
        public string? ContentText { get; set; }

        /// <summary>
        /// Gets or sets ButtonText.
        /// </summary>
        [Parameter]
        public string? ButtonText { get; set; }

        /// <summary>
        /// Gets or sets Color.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Success;

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        [Parameter]
        public string Icon { get; set; } = Icons.Material.Filled.QuestionMark;

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<Confirmation>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the mud dialog instance component service.
        /// </summary>
        public IMudDialogInstanceComponentService? MudDialogInstanceComponentService { get; set; }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(this.ButtonText))
            {
                this.ButtonText = this.StringLocalizer!["Yes"];
            }
        }

        /// <summary>
        /// Called on confirmation.
        /// </summary>
        private void Confirm()
        {
            this.MudDialogInstanceComponentService?.Close(true);
        }

        /// <summary>
        /// Called on cancel.
        /// </summary>
        private void Cancel()
        {
            this.MudDialogInstanceComponentService?.Cancel();
        }
    }
}