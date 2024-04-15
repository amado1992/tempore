// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFileInvokable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.FileProcessing
{
    using Coravel.Invocable;

    using Mapster;

    using MediatR;

    using MethodTimer;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Notifications.FileProcessing;
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The process file invokable class.
    /// </summary>
    public class ProcessFileInvokable : IInvocable, IInvocableWithPayload<IInvocationContext<ProcessFileRequest>>, ICancellableInvocable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ProcessFileInvokable> logger;

        /// <summary>
        /// The file processing service factory.
        /// </summary>
        private readonly IFileProcessingServiceFactory fileProcessingServiceFactory;

        /// <summary>
        /// The data file repository.
        /// </summary>
        private readonly IRepository<DataFile, ApplicationDbContext> dataFileRepository;

        /// <summary>
        /// The employee repository.
        /// </summary>
        private readonly IRepository<Employee, ApplicationDbContext> employeeRepository;

        /// <summary>
        /// The publisher.
        /// </summary>
        private readonly IPublisher publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessFileInvokable"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="fileProcessingServiceFactory">
        /// The file processing service factory.
        /// </param>
        /// <param name="dataFileRepository">
        /// The data file repository.
        /// </param>
        /// <param name="employeeRepository">
        /// The employee repository.
        /// </param>
        /// <param name="publisher">
        /// The publisher.
        /// </param>
        public ProcessFileInvokable(
            ILogger<ProcessFileInvokable> logger,
            IFileProcessingServiceFactory fileProcessingServiceFactory,
            IRepository<DataFile, ApplicationDbContext> dataFileRepository,
            IRepository<Employee, ApplicationDbContext> employeeRepository,
            IPublisher publisher)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(fileProcessingServiceFactory);
            ArgumentNullException.ThrowIfNull(dataFileRepository);
            ArgumentNullException.ThrowIfNull(employeeRepository);
            ArgumentNullException.ThrowIfNull(publisher);

            this.logger = logger;
            this.fileProcessingServiceFactory = fileProcessingServiceFactory;
            this.publisher = publisher;
            this.dataFileRepository = dataFileRepository;
            this.employeeRepository = employeeRepository;
        }

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get; set; }

        /// <inheritdoc/>
        public IInvocationContext<ProcessFileRequest> Payload { get; set; } = default!;

        /// <inheritdoc/>
        [Time]
        public async Task Invoke()
        {
            var request = this.Payload.Request;

            this.logger.LogInformation("Processing file {ID}", request.FileId);

            var file = await this.dataFileRepository.SingleOrDefaultAsync(dataFile => dataFile.Id == request.FileId);
            if (file is null)
            {
                // throw this.logger.LogErrorAndCreateException<NotFoundException>($"Data file with id {request.FileId} not found");
                await this.publisher.Publish(new ProcessFileProcessCompletedNotification(this.Payload, Severity.Error));
                return;
            }

            // TODO: Review what we can do if something goes wrong with this (rollback?)
            var fileProcessingService = this.fileProcessingServiceFactory.Create(file.FileType);
            try
            {
                await foreach (var employee in fileProcessingService.GetEmployeesAsync(file))
                {
                    if (string.IsNullOrWhiteSpace(employee.ExternalId))
                    {
                        this.logger.LogWarning("Employee with empty external id empty will be skipped.");
                        file.ProcessingState = FileProcessingState.Incomplete;

                        continue;
                    }

                    var storedEmployee = await this.employeeRepository.SingleOrDefaultAsync(e => e.ExternalId == employee.ExternalId);
                    if (storedEmployee is null)
                    {
                        this.employeeRepository.Add(employee);
                    }
                    else
                    {
                        var typeAdapterSetter = TypeAdapterConfig<Employee, Employee>.NewConfig()
                            .IgnoreNullValues(true)
                            .Ignore(e => e.Id);

                        employee.Adapt(storedEmployee, typeAdapterSetter.Config);
                        this.employeeRepository.Update(storedEmployee);
                    }

                    await this.employeeRepository.SaveChangesAsync();
                }

                file.ProcessingState = FileProcessingState.Completed;
                await this.publisher.Publish(new ProcessFileProcessCompletedNotification(this.Payload, Severity.Success));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error processing file.");
                file.ProcessingState = FileProcessingState.Failed;

                await this.publisher.Publish(new ProcessFileProcessCompletedNotification(this.Payload, Severity.Error));
            }
            finally
            {
                file.ProcessingDate = DateTime.Now;

                this.dataFileRepository.Update(file);
                await this.dataFileRepository.SaveChangesAsync();
            }
        }
    }
}