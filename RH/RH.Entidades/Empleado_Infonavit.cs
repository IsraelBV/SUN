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
    
    public partial class Empleado_Infonavit
    {
        public int Id { get; set; }
        public int IdEmpleadoContrato { get; set; }
        public string NumCredito { get; set; }
        public int TipoCredito { get; set; }
        public decimal FactorDescuento { get; set; }
        public Nullable<decimal> Salario { get; set; }
        public System.DateTime FechaInicio { get; set; }
        public Nullable<System.DateTime> FechaSuspension { get; set; }
        public bool UsarUMA { get; set; }
        public bool Status { get; set; }
    }
}