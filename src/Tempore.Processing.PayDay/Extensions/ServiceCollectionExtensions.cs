// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    using Tempore.Processing.PayDay.Services;
    using Tempore.Processing.PayDay.Services.WorkforceMetricCalculators;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Processing.Services.WorkforceMetricCalculators.Interfaces;

    /// <summary>
    /// The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds payday service.
        /// </summary>
        /// <param name="serviceCollection">
        /// The service collection/.
        /// </param>
        public static void AddPayDay(this IServiceCollection serviceCollection)
        {
            // Workforce metric seeder
            serviceCollection.AddSingleton<IWorkforceMetricSeeder, PayDayWorkforceMetricSeeder>();

            // Workforce metric calculators
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdDiurnaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdDiuDiaDesWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdDiuDiaFerWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdDiuDiaCompTrabWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STDiurnoDiaNorWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STMixtoDiaNorWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STNoctDiaNorWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STDiurnoDiaDesWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STDiurnoDiaNorRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STMixtoDiaDesWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STMixtoDiaNorRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STNoctDiaNorRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STDiurnoDiaFerWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STDiurnoDiaDesRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STMixtoDiaFerWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STMixtoDiaDesRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STNoctDiaFerWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STNoctDiaDesRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STDiurnoDiaFerRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STMixtoDiaFerRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, STNoctDiaFerRecWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, SoloRecargoDiaCompTrabWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, FeriadoNoTrabWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, CertificadoMedicoWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, VacacionWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, TardanzaNOpagadaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, AusenciaNOpagadaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, LicenciaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, AusenciaPagadaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, TardanzaPagadaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdMixtaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdNocturnaWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdMixDiaFerWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdNocDiaFerWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdMixDomingoWorkforceMetricCalculator>();
            serviceCollection.AddTransient<IWorkforceMetricCalculator, JorOrdNocDomingoWorkforceMetricCalculator>();
        }
    }
}