using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomina.Procesador.Modelos
{
   public class TotalConcepto
    {
       public decimal Total { get; set; }
       public decimal ImpuestoSobreNomina { get; set; }
       public decimal TotalObligaciones { get; set; }
    }

    public class TotalIsrSubsidio
    {
        public decimal SubsidioCausado { get; set; }
        public decimal SubsidioEntregado { get; set; }
        public decimal IsrAntesSubsidio { get; set; }
        public decimal IsrCobrado { get; set; }
    }
}
