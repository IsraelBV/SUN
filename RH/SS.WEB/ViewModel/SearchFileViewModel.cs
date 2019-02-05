using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SS.WEB.ViewModel
{
    public class SearchFileViewModel
    {
        public int TipoArchivo { get; set; }
        public int TipoMovimiento { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}