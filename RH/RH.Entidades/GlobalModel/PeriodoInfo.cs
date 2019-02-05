using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.Entidades.GlobalModel
{
  public  class PeriodoInfo
    {
        public int IdPeriodo { get; set; }
        public int  IdSucursal { get; set; }
        public int NumEmpleados { get; set; }
        public int NumNominas { get; set; }
        public int NumTimbrados { get; set; }
        public int NumCancelados { get; set; }
        public bool EsComplemento { get; set; }
    }
}
