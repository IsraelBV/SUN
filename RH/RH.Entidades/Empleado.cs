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
    
    public partial class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Nombres { get; set; }
        public string APaterno { get; set; }
        public string AMaterno { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string RFC { get; set; }
        public int RFCValidadoSAT { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public string FONACOT { get; set; }
        public string Direccion { get; set; }
        public string Nacionalidad { get; set; }
        public string Estado { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string EstadoCivil { get; set; }
        public int IdSucursal { get; set; }
        public bool Status { get; set; }
        public System.DateTime FechaReg { get; set; }
        public int IdUsuarioReg { get; set; }
        public Nullable<System.DateTime> FechaMod { get; set; }
        public Nullable<int> IdUsuarioMod { get; set; }
    }
}