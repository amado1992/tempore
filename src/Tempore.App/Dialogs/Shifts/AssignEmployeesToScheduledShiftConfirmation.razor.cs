// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignEmployeesToScheduledShiftConfirmation.razor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Dialogs.Shifts
{
    using FluentValidation;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    using MudBlazor;

    using Tempore.App.Services.Interfaces;
    using Tempore.App.ViewModels.Dialogs.Shifts;
    using Tempore.Client;

    /// <summary>
    /// The AssignEmployeesToScheduledShiftConfirmation dialog.
    /// </summary>
    public partial class AssignEmployeesToScheduledShiftConfirmation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignEmployeesToScheduledShiftConfirmation"/> class.
        /// </summary>
        public AssignEmployeesToScheduledShiftConfirmation()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the mud form component service.
        /// </summary>
        public IMudFormComponentService? MudFormComponentService { get; set; }

        /// <summary>
        /// Gets or sets the mud dialog instance component service.
        /// </summary>
        public IMudDialogInstanceComponentService? MudDialogInstanceComponentService { get; set; }

        /// <summary>
        /// Gets or sets the validator.
        /// </summary>
        [Inject]
        public IValidator<AssignEmployeesToScheduledShiftConfirmationViewModel>? Validator { get; set; }

        /// <summary>
        /// Gets or sets Color.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Success;

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        [Parameter]
        public string Icon { get; set; } = Icons.Material.Filled.Add;

        /// <summary>
        /// Gets or sets the string localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<AssignEmployeesToScheduledShiftConfirmation>? StringLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the AssignShiftToEmployeesConfirmationViewModel.
        /// </summary>
        [Parameter]
        public AssignEmployeesToScheduledShiftConfirmationViewModel? ViewModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the field to read only.
        /// </summary>
        [Parameter]
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Called on cancel.
        /// </summary>
        private void Cancel()
        {
            this.MudDialogInstanceComponentService?.Cancel();
        }

        private async Task ConfirmAsync()
        {
            if (await this.MudFormComponentService!.ValidateAsync())
            {
                var request = new AssignEmployeesToScheduledShiftRequest
                {
                    ShiftId = this.ViewModel!.ShiftId,
                    EmployeeIds = this.ViewModel.EmployeeIds,
                    StartDate = this.ViewModel.StartDate!.Value,
                    ExpireDate = this.ViewModel.ExpireDate!.Value,
                    EffectiveWorkingTime = TimeSpan.FromHours(this.ViewModel.EffectiveWorkingHours!.Value),
                };

                this.MudDialogInstanceComponentService!.Close(request);
            }
        }
    }
}