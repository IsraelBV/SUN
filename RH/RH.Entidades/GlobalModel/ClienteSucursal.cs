using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.Entidades.GlobalModel
{
    public class ClienteSucursal
    {
        public int IdCliente { get; set; }
        public int IdSucursal { get; set; }
        public string Cliente { get; set; }
        public string Sucursal { get; set; }

    }
}
