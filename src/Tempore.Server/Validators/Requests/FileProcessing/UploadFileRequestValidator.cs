// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.FileProcessing
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The upload file request validator.
    /// </summary>
    public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
    {
        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer<UploadFileRequestValidator> stringLocalizer;

        /// <summary>
        /// The file processing service factory.
        /// </summary>
        private readonly IFileProcessingServiceFactory fileProcessingServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        /// <param name="fileProcessingServiceFactory">
        /// The file processing service factory.
        /// </param>
        public UploadFileRequestValidator(IStringLocalizer<UploadFileRequestValidator> stringLocalizer, IFileProcessingServiceFactory fileProcessingServiceFactory)
        {
            this.stringLocalizer = stringLocalizer;
            this.fileProcessingServiceFactory = fileProcessingServiceFactory;
            this.RuleFor(request => request.FileType)
                .Must(fileProcessingServiceFactory.IsSupported)
                .WithMessage(stringLocalizer["File type is not supported"])
                .DependentRules(() =>
                {
                    this.RuleFor(request => request.File)
                        .MustAsync(async (request, file, _) =>
                        {
                            using var stream = file.OpenReadStream();
                            return await this.fileProcessingServiceFactory.Create(request.FileType)
                                       .IsValidAsync(stream);
                        })
                        .WithMessage(stringLocalizer["The file is not valid"]);
                });
        }
    }
}