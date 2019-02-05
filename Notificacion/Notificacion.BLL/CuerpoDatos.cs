using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notificacion.BLL
{
    public class CuerpoDatos
    {
        public int IdEmpleado { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string NSS { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string Departamento { get; set; }
        public string Puesto { get; set; }
        public string Puesto_Anterior { get; set; }
        public string Puesto_Nuevo { get; set; }
        public string Fecha_Inicio { get; set; }
        public string Fecha_Fin { get; set; }
        public string Fecha_Presentacion {get;set;}
        public string Dias { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string Empresa_Anterior { get; set; }
        public Configuracion_Empresa Configuracion_Anterior { get; set; }
        public Configuracion_Empresa Configuracion_Nueva { get; set; }
        public string Fecha_de_Baja { get; set; }
        public string Periodo { get; set; }
        public decimal? SDI_Nuevo { get; set; }
        public decimal? SDI_Anterior { get; set; }
        public decimal? Salario_Real_Nuevo { get; set; }
        public decimal? Salario_Real_Anterior { get; set; }
        public decimal? SD_Nuevo { get; set; }
        public decimal? SD_Viejo { get; set; }
        public string Fecha { get; set; }
        public string Antiguedad { get; set; }
        public decimal? SD { get; set; }
        public decimal? SDI { get; set; }
        public string Factor { get; set; }
        public string Nacionalidad { get; set; }
        public string Fecha_IMSS { get; set; }
        public string Tipo_de_Movimiento { get; set; }
        public string Tipo_Credito { get; set; }
        public string No_Credito { get; set; }
        public string Fecha_Otorgamiento { get; set; }
        public string Importe_de_Descuento { get; set; }
        public decimal Factor_de_Descuento { get; set; }
        public string Fecha_Nacimiento { get; set; }
        public string Fecha_de_Reingreso { get; set; }
        public string Cuenta_Bancaria { get; set; }
        public string No_SIGA { get; set; }
        public string Banco { get; set; }
        public string No_Tarjeta { get; set; }
        public string UMF { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Direccion { get; set; }
        public string Folio { get; set; }
        public string Tipo_Incapacidad { get; set; }
        public string Clase_Incapacidad { get; set; }
        public string Tipo_Salario { get; set; }
        public int IdSucursal { get; set; }
        public string Alerta { get; set; }
        public string Motivo_Baja { get; set; }
        public string Beneficiario_Nombre { get; set; }
        public string Beneficiario_Parentezco { get; set; }
        public string Beneficiario_Domicilio { get; set; }
        public string Beneficiario_Curp { get; set; }
        public string Beneficiario_Rfc { get; set; }
       
       
        
    }
}

public class Configuracion_Empresa
{
    public string Fiscal { get; set; }
    public string Complemento { get; set; }
    public string Asimilado { get; set; }
    public string Sindicato { get; set; }
}