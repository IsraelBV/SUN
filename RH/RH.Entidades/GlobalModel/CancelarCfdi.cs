using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.Entidades.GlobalModel
{
    public class CancelarCfdi
    {
        public int IdTimbrado { get; set; }
        public int IdNomina { get; set; }
        public string FolioFiscalUuid { get; set; }
        public bool Cancelado { get; set; }
    }
}
