using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomina.Procesador.Modelos
{
   public class DetalleNominaComp
    {
        public int IdDetalle { get; set; }
        public int  IdNomina { get; set; }
        public int IdConcepto { get; set; }
        public string DescripcionConcepto { get; set; }
        public decimal Cantidad { get; set; }
        public int  TipoConcepto { get; set; }
    }
}
