// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayDayWorkforceMetrics.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay
{
    using System.Collections.Immutable;

    using Tempore.Common.Extensions;

    /// <summary>
    /// The pay day workforce metrics.
    /// </summary>
    public static class PayDayWorkforceMetrics
    {
        public const string JorOrdDiurna = "JorOrd Diurna";

        public const string JorOrdDiuDiaDes = "JorOrd Diu DiaDes";

        public const string JorOrdDiuDiaFer = "JorOrd Diu DiaFer";

        public const string JorOrdDiuDiaCompTrab = "JorOrd Diu DiaComp Trab";

        public const string STDiurnoDiaNor = "ST Diurno DiaNor";

        public const string STMixtoDiaNor = "ST Mixto DiaNor";

        public const string STNoctDiaNor = "ST Noct DiaNor";

        public const string STDiurnoDiaDes = "ST Diurno DiaDes";

        public const string STDiurnoDiaNorRec = "ST Diurno DiaNor Rec";

        public const string STMixtoDiaDes = "ST Mixto DiaDes";

        public const string STMixtoDiaNorRec = "ST Mixto DiaNor Rec";

        public const string STNoctDiaNorRec = "ST Noct DiaNor Rec";

        public const string STDiurnoDiaFer = "ST Diurno DiaFer";

        public const string STDiurnoDiaDesRec = "ST Diurno DiaDes Rec";

        public const string STMixtoDiaFer = "ST Mixto DiaFer";

        public const string STMixtoDiaDesRec = "ST Mixto DiaDes Rec";

        public const string STNoctDiaFer = "ST Noct DiaFer";

        public const string STNoctDiaDesRec = "ST Noct DiaDes Rec";

        public const string STDiurnoDiaFerRec = "ST Diurno DiaFer Rec";

        public const string STMixtoDiaFerRec = "ST Mixto DiaFer Rec";

        public const string STNoctDiaFerRec = "ST Noct DiaFer Rec";

        public const string SoloRecargoDiaCompTrab = "SoloRecargo DiaComp Trab";

        public const string FeriadoNoTrab = "Feriado No Trab";

        public const string CertificadoMedico = "Certificado Medico";

        public const string Vacacion = "Vacacion";

        public const string TardanzaNOpagada = "Tardanza NO pagada";

        public const string AusenciaNOpagada = "Ausencia NO pagada";

        public const string Licencia = "Licencia";

        public const string AusenciaPagada = "Ausencia Pagada";

        public const string TardanzaPagada = "Tardanza Pagada";

        public const string JorOrdMixta = "JorOrd Mixta";

        public const string JorOrdNocturna = "JorOrd Nocturna";

        public const string JorOrdMixDiaFer = "JorOrd Mix DiaFer";

        public const string JorOrdNocDiaFer = "JorOrd Noc DiaFer";

        public const string JorOrdMixDomingo = "JorOrd Mix Domingo";

        public const string JorOrdNocDomingo = "JorOrd Noc Domingo";

        /// <summary>
        /// The all.
        /// </summary>
        public static readonly IImmutableList<string> All = typeof(PayDayWorkforceMetrics).Constants<string>().ToImmutableList();
    }
}