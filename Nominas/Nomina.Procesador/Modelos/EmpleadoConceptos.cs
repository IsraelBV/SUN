using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Nomina.Procesador.Modelos
{
    public class EmpleadoConceptos
    {
        public int IdConcepto { get; set; }
        public string Descripcion { get; set; }
        public int TipoConcepto { get; set; }
     //   public bool IsComplemento { get; set; }
        public bool IsImpuestoSobreNomina { get; set; }

        public bool IsFormulaFiscal { get; set; }
        public bool IsFormulaComplemento { get; set; }

       public int IdTipoOtroPago { get; set; }
    }
}
