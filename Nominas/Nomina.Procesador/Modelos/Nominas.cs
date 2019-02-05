using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Nomina.Procesador.Modelos
{
    public class Nominas
    {
       public string SalarioDiarioIntegrado { get; set; }
        public string RiesgoPuesto { get; set; }
        public string SalarioBaseCotApor { get; set; }
        public string PeriocidadPago { get; set; }
        public string TipoJornada { get; set; }
        public string TipoContrato { get; set; }
        public string Antiguedad { get; set; }

        public string FechaInicioRealLaboral { get; set; }
        public string Banco { get; set; }
        public string Clabe { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
        public string NumDiasPagados { get; set; }
        public string FechaFinalPago { get; set; }
        public string FechaInicialPago { get; set; }
        public string FechaPago { get; set; }

        public string NumSeguroSocial { get; set; }
        public string TipoRegimen { get; set; }
        public string Curp { get; set; }
        public string NumEmpleado { get; set; }
        public string RegistroPatronal { get; set; }
        public string Version { get; set; }

    }

    public class PercepcionTotales
    {
        public string TotalExcento { get; set; }
        public string TotalGravado { get; set; }
    }

    public class Percepciones
    {
        public string ImporteTotal { get; set; }
        public string ImporteExento { get; set; }
        public string ImporteGravado { get; set; }
        public string Concepto { get; set; }
        public string Clave { get; set; }
        public string TipoPercepcion { get; set; }

    }

    public class DeduccionTotales
    {
        public string TotalExcento { get; set; }
        public string TotalGravado { get; set; }
    }

    public class Deducciones
    {
        public string ImporteExento { get; set; }
        public string ImporteGravado { get; set; }
        public string Concepto { get; set; }
        public string Clave { get; set; }
        public string TipoDeduccion { get; set; }
    }

    public class Incapacidad
    {
        public string DiasIncapacidad { get; set; }
        public string Descuento { get; set; }
        public string TipoIncapacidad { get; set; }

    }

    public class HorasExtras
    {
        public string TipoHoras { get; set; }
        public string HorasExtra { get; set; }
        public string ImportePagado { get; set; }
    }

    /// <summary>
    /// Clase usada para el timbrado
    /// </summary>
    public class NominaData
    {
        public int IdNomina { get; set; }
        public int IdFiniquito { get; set; }
        public int IdEjercicio { get; set; }
        public int IdPeriodo { get; set; }
        public int IdEmisor { get; set; }
        public string RfcEmisor { get; set; }

        public int? IdEmpresa { get; set; }
        public int IdSucursal { get; set; }
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }

        public decimal TotalNomina { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalOtrosPagos { get; set; }
        public decimal TotalComplemento { get; set; }
        public decimal Sd { get; set; }
        public decimal Sdi { get; set; }
        public decimal Sbc { get; set; }
        public int DiasPagados { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Rfc { get; set; }
        public string Nss { get; set; }
        public string Curp { get; set; }
        public string TipoContrato { get; set; }
        public string TipoJornada { get; set; }
        public string Departamento { get; set; }
        public string PuestoOriginal { get; set; }
        public string PuestoRecibo { get; set; }
        public string PeriodicidadPago { get; set; }
        public string RegistroPatronal { get; set; }
        public string ClaveMetodoPago { get; set; }
        public string MetodoPago { get; set; }

        public string LugarExpedicion { get; set; }
        public string NumeroDeCuenta { get; set; }

        public string TipoComprobante { get; set; }
        public string Serie { get; set; }
        
        public string FechaInicialdePago { get; set; }
        public string FechaFinaldePago { get; set; }

        public string FechaDePago { get; set; }
        public string TipoRegimen { get; set; }
        public string CveBanco { get; set; }
        public string ClabeInterbancaria { get; set; }
        public int RiesgoPuesto { get; set; }
        public int TipoDeNomina { get; set; }
        public string TipoNominaSat { get; set; }
        public string ClaveEntidadFederativa { get; set; }
        public decimal SubsidioCausado { get; set; }
        public decimal SubsidioEntregado { get; set; }
        public bool XmlGeneradoPreRecibo { get; set; }
        public string ClaveContableOtroPago { get; set; }
        public bool Sindicalizado { get; set; }
        public decimal Totalliquidacion { get; set; }
        public bool PagoEnEfectivo { get; set; }
        public decimal PorcentajeTiempo { get; set; }
    }

    /// <summary>
    /// Clase usada para el timbrado
    /// </summary>
    public class OtroTipoPagos
    {
        public string  TipoOtroPago { get; set; }
        public string ClaveContable { get; set; }
        public string Concepto { get; set; }
    }
}
