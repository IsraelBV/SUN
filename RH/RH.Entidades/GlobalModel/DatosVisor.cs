using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.Entidades.GlobalModel
{
   public class DatosVisor
    {
            public int IdTimbrado { get; set; }
            public int IdEmpleado { get; set; }
            public int IdPeriodo { get; set; }
            public int IdNomina { get; set; }
            public int IdFiniquito { get; set; }
            public int IdSucursal { get; set; }
            public string XmlTimbrado { get; set; }

            public string RFCEmisor { get; set; }
            public string RFCReceptor { get; set; }
            public string FolioFiscalUUID { get; set; }
            public decimal TotalRecibo { get; set; }
            public int IdEmisor { get; set; }
            public DateTime? FechaCertificacion { get; set; }
            public DateTime? FechaCancelacion { get; set; }

    }
}
