using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.Entidades.GlobalModel
{
    public class ParametrosFiniquitos
    {
        public ParametrosFiniquitos(){
            FechaBajaF = null;
            FechaAguinaldoF = null;
            FechaVacacionesF = null;
            FechaAguinaldoC = null;
            FechaVacacionesC = null;
            DiasSueldoPendientesF = 0;
            DiasVacacionesPendientesF = 0;
            DiasSueldoPendientesC = 0;
            DiasVacacionesPendientesC = 0;
            MesesSalarioF = -1;
            MesesSalarioC = -1;
            VeinteDiasPorAF = -1;
            VeinteDiasPorAC = -1;
            DiasVacCorrespondientesF = -1;
            DiasVacCorrespondientesC = -1;
            FechaAltaF = null;
            PorcentajePimaVacPendienteF = -1;
            PorcentajePimaVacPendienteC = -1;
            TipoTarifa = 5;//5 es mensual
            DiasAguinaldoF = -1;
            DiasAguinaldoC = -1;
        }

        public DateTime? FechaAltaF { get; set; }
        public DateTime? FechaBajaF { get; set; }
        public DateTime? FechaAguinaldoF { get; set; }
        public DateTime? FechaVacacionesF { get; set; }
        public decimal DiasSueldoPendientesF { get; set; }
        public decimal DiasVacacionesPendientesF { get; set; }
        public decimal MesesSalarioF { get; set; }
        public DateTime? FechaAguinaldoC { get; set; }
        public DateTime? FechaVacacionesC { get; set; }
        public decimal DiasSueldoPendientesC { get; set; }
        public decimal DiasVacacionesPendientesC { get; set; }
        public decimal MesesSalarioC { get; set; }
        public decimal VeinteDiasPorAF { get; set; }
        public decimal VeinteDiasPorAC { get; set; }
        public int DiasVacCorrespondientesF { get; set; }
        public int DiasVacCorrespondientesC { get; set; }

        public decimal PorcentajePimaVacPendienteF { get; set; }
        public decimal PorcentajePimaVacPendienteC { get; set; }

        public int TipoTarifa { get; set; }

        public int DiasAguinaldoF { get; set; }
        public int DiasAguinaldoC { get; set; }

    }


    public class TotalPersonalizablesFiniquitos
    {
        public TotalPersonalizablesFiniquitos()
        {

            TotalTresMesesFiscalPersonalizado = -1;
            TotalTresMesesCompPersonalizado = -1;
            TotalVeinteDiasFiscalPersonalizado = -1;
            TotalVienteDiasCompPersonalizado = -1;

            TotalPrimaFiscalPersonalizado = -1;
            TotalPrimaCompPersonalizado = -1;

            TotalPrimaVacPersonalizado = -1;
            TotalPrimaVacCompPersonalizado = -1;
        }

  
        public decimal TotalTresMesesFiscalPersonalizado { get; set; }
        public decimal TotalTresMesesCompPersonalizado { get; set; }

        public decimal TotalVeinteDiasFiscalPersonalizado { get; set; }
        public decimal TotalVienteDiasCompPersonalizado { get; set; }

        public decimal TotalPrimaFiscalPersonalizado { get; set; }
        public decimal TotalPrimaCompPersonalizado { get; set; }

        public decimal TotalPrimaVacPersonalizado { get; set; }
        public decimal TotalPrimaVacCompPersonalizado { get; set; }
    }

}
