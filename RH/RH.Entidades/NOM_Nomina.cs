//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RH.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class NOM_Nomina
    {
        public int IdNomina { get; set; }
        public int IdCliente { get; set; }
        public int IdSucursal { get; set; }
        public int IdEjercicio { get; set; }
        public int IdPeriodo { get; set; }
        public int IdEmpleado { get; set; }
        public int IdContrato { get; set; }
        public System.DateTime FechaReg { get; set; }
        public int IdUsuario { get; set; }
        public bool CFDICreado { get; set; }
        public decimal TotalNomina { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalOtrosPagos { get; set; }
        public decimal TotalComplemento { get; set; }
        public decimal TotalImpuestoSobreNomina { get; set; }
        public decimal TotalImpuestoSobreNomina_Complemento { get; set; }
        public decimal TotalObligaciones { get; set; }
        public decimal TotalCuotasIMSS { get; set; }
        public decimal TotalCuotasIMSSComplemento { get; set; }
        public decimal SubsidioCausado { get; set; }
        public decimal SubsidioEntregado { get; set; }
        public decimal ISRantesSubsidio { get; set; }
        public decimal SD { get; set; }
        public decimal SDI { get; set; }
        public decimal SBC { get; set; }
        public decimal SDReal { get; set; }
        public Nullable<int> IdEmpresaFiscal { get; set; }
        public Nullable<int> IdEmpresaComplemento { get; set; }
        public Nullable<int> IdEmpresaAsimilado { get; set; }
        public Nullable<int> IdEmpresaSindicato { get; set; }
        public int IdMetodo_Pago { get; set; }
        public int Dias_Laborados { get; set; }
        public int Faltas { get; set; }
        public decimal Prima_Riesgo { get; set; }
        public int TipoTarifa { get; set; }
        public decimal SBCotizacionDelProcesado { get; set; }
        public string XMLSinTimbre { get; set; }
        public decimal SMGV { get; set; }
        public decimal UMA { get; set; }
        public decimal PorcentajeTiempo { get; set; }
    }
}
