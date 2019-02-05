using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.Entidades.GlobalModel
{
    public partial class DataInit
    {
        public List<Empresa> empresa { get; set; }
        public List<C_Banco_SAT> banco { get; set; }
    }
    public class LayoutBanco
    {
        public int IdEmpleado { get; set; }
        public string NombrePeriodo { get; set; }
        public int? NoSiga { get; set; }
        public int? NoSiga1 { get; set; }
        public int? NoSiga2 { get; set; }
        public string CuentaBancaria { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public decimal Importe { get; set; }
        public bool Generado { get; set; }
        public string NombreEmpresa { get; set; }
        public int IdEmpresa { get; set; }
        public int? NoEmisor { get; set; }
        public bool IsComplemento { get; set; }
        public int IdBanco { get; set; }
    }

    public class encabezado
    {
        public string TipoRegistro { get; set; }
        public string ClaveServicio { get; set; }
        public int Fecha { get; set; }
        public int Consecutivo { get; set; }
        public string registroDetalle { get; set; }
        public int Banco { get; set; }


        public decimal ImporteTotal { get; set; }
        public int TotalEmpleados { get; set; }
    }

    public class detallado
    {
        public int NoSiga1 { get; set; }
        public int NoSiga2 { get; set; }
        public decimal Importe { get; set; }
        public int Banco { get; set; }
        public int TipoCuenta { get; set; }
        public string TipoRegistro { get; set; }
        public int CuentaBancaria { get; set; }
        public int fecha { get; set; }
        public int NoEmisor { get; set; }

    }
    public class emisoras
    {
        public int NoEmisor { get; set; }
    }


}
