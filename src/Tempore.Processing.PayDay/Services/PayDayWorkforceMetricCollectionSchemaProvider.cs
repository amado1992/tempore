// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayDayWorkforceMetricCollectionSchemaProvider.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Services
{
    using Tempore.Processing.Services.Interfaces;

    using SchemaType = Tempore.Processing.SchemaType;

    /// <summary>
    /// The PayDayWorkforceMetricCollectionSchemaProvider class.
    /// </summary>
    public class PayDayWorkforceMetricCollectionSchemaProvider : IWorkforceMetricCollectionSchemaProvider
    {
        /// <inheritdoc />
        public IEnumerable<ColumnInfo> GetSchema(SchemaType schemaType)
        {
            var idx = 0;
            yield return new ColumnInfo("Id", idx++);
            if (schemaType == SchemaType.Display)
            {
                yield return new ColumnInfo("Name", idx++);
            }
            else
            {
                yield return new ColumnInfo("Name", idx++, false);
            }

            foreach (var columnName in GetColumnNames())
            {
                yield return new ColumnInfo(columnName, idx++);
            }
        }

        private static IEnumerable<string> GetColumnNames()
        {
            yield return PayDayWorkforceMetrics.JorOrdDiurna;
            yield return PayDayWorkforceMetrics.JorOrdDiuDiaDes;
            yield return PayDayWorkforceMetrics.JorOrdDiuDiaFer;
            yield return PayDayWorkforceMetrics.JorOrdDiuDiaCompTrab;
            yield return PayDayWorkforceMetrics.STDiurnoDiaNor;
            yield return PayDayWorkforceMetrics.STMixtoDiaNor;
            yield return PayDayWorkforceMetrics.STNoctDiaNor;
            yield return PayDayWorkforceMetrics.STDiurnoDiaDes;
            yield return PayDayWorkforceMetrics.STDiurnoDiaNorRec;
            yield return PayDayWorkforceMetrics.STMixtoDiaDes;
            yield return PayDayWorkforceMetrics.STMixtoDiaNorRec;
            yield return PayDayWorkforceMetrics.STNoctDiaNorRec;
            yield return PayDayWorkforceMetrics.STDiurnoDiaFer;
            yield return PayDayWorkforceMetrics.STDiurnoDiaDesRec;
            yield return PayDayWorkforceMetrics.STMixtoDiaFer;
            yield return PayDayWorkforceMetrics.STMixtoDiaDesRec;
            yield return PayDayWorkforceMetrics.STNoctDiaFer;
            yield return PayDayWorkforceMetrics.STNoctDiaDesRec;
            yield return PayDayWorkforceMetrics.STDiurnoDiaFerRec;
            yield return PayDayWorkforceMetrics.STMixtoDiaFerRec;
            yield return PayDayWorkforceMetrics.STNoctDiaFerRec;
            yield return PayDayWorkforceMetrics.SoloRecargoDiaCompTrab;
            yield return PayDayWorkforceMetrics.FeriadoNoTrab;
            yield return PayDayWorkforceMetrics.CertificadoMedico;
            yield return PayDayWorkforceMetrics.Vacacion;
            yield return PayDayWorkforceMetrics.TardanzaNOpagada;
            yield return PayDayWorkforceMetrics.AusenciaNOpagada;
            yield return PayDayWorkforceMetrics.Licencia;
            yield return PayDayWorkforceMetrics.AusenciaPagada;
            yield return PayDayWorkforceMetrics.TardanzaPagada;
            yield return PayDayWorkforceMetrics.JorOrdMixta;
            yield return PayDayWorkforceMetrics.JorOrdNocturna;
            yield return PayDayWorkforceMetrics.JorOrdMixDiaFer;
            yield return PayDayWorkforceMetrics.JorOrdNocDiaFer;
            yield return PayDayWorkforceMetrics.JorOrdMixDomingo;
            yield return PayDayWorkforceMetrics.JorOrdNocDomingo;
        }
    }
}