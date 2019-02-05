using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Notificacion.BLL;
using Common.Helpers;
using Common.Enums;
using Common.Utils;

namespace RH.BLL
{
    public class Notificaciones
    {
        RHEntities ctx = null;

        public Notificaciones()
        {
            ctx = new RHEntities();
        }
        public void Alta(int[] arrayEmpleados, int idUsuario)
        {
            foreach (var item in arrayEmpleados)
            {
                Alta(item, idUsuario);
            }
        }
        public void Alta(int IdEmpleado, int idUsuario)
        {
            var confi = new Configuracion_Empresa();
            using (var context = new RHEntities())
            { 
                var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
                var datos = GetDatosPersonales(IdEmpleado);

                var empresas = context.Empresa.ToList();
                var fiscal = empresas.Where(x => x.IdEmpresa == contrato.IdEmpresaFiscal).Select(x => x.RazonSocial).FirstOrDefault();
                var complemento = empresas.Where(x => x.IdEmpresa == contrato.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault();
                var asimilado = empresas.Where(x => x.IdEmpresa == contrato.IdEmpresaAsimilado).Select(x => x.RazonSocial).FirstOrDefault();
                var sindicato = empresas.Where(x => x.IdEmpresa == contrato.IdEmpresaSindicato).Select(x => x.RazonSocial).FirstOrDefault();
                confi.Fiscal = fiscal;
                confi.Complemento = complemento;
                confi.Asimilado = asimilado;
                confi.Sindicato = sindicato;
                datos.Configuracion_Nueva = confi;
                var titulo = GetTitulo(datos);
                if (contrato.FechaIMSS == null) {
                    NotificacionesCommon.CreateNotification(titulo, DateTime.Now, idUsuario, TiposNotificacion.Alta_General, datos.IdCliente, datos, datos.IdSucursal, contrato.IdContrato, contrato.FechaAlta.ToString("dd-MM-yyyy"));
                }else
                {
                    NotificacionesCommon.CreateNotification(titulo, DateTime.Now, idUsuario, TiposNotificacion.Alta_Fiscal, datos.IdCliente, datos, datos.IdSucursal, contrato.IdContrato, contrato.FechaIMSS.Value.ToString("dd-MM-yyyy"));
                }
            }   
            
        }
        //Metodo para reenviar Altas y Altas IMSS
        public  void ReenvioAlta(int IdEmpleado)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var datos = GetDatosPersonalesReenvio(IdEmpleado);
            var titulo = GetTitulo(datos);
            //NotificacionesCommon.CreateNotification(titulo, Convert.ToDateTime(datos.Fecha_IMSS), SessionHelpers.GetIdUsuario(), TiposNotificacion.Alta, datos.IdCliente, datos, datos.IdSucursal,contrato.IdContrato,contrato.FechaAlta.ToString("dd-MM-yyyy"));
            //NotificacionesCommon.CreateNotification(titulo, Convert.ToDateTime(datos.Fecha_IMSS), SessionHelpers.GetIdUsuario(), TiposNotificacion.IMSS, datos.IdCliente, datos, datos.IdSucursal,contrato.IdContrato, contrato.FechaAlta.ToString("dd-MM-yyyy"));
            if (contrato.FechaIMSS == null)
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Alta_General, datos.IdCliente, datos, datos.IdSucursal, contrato.IdContrato, contrato.FechaAlta.ToString("dd-MM-yyyy"));
            }
            else
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Alta_Fiscal, datos.IdCliente, datos, datos.IdSucursal, contrato.IdContrato, contrato.FechaIMSS.Value.ToString("dd-MM-yyyy"));
            }
        }

        public void Baja(int IdEmpleado, DateTime? FechaBaja,string MotivoBaja)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var cd = GetDatosPersonales(IdEmpleado);
            cd.Fecha_de_Baja = FechaBaja.Value.ToString("dd-MM-yyyy");
            cd.Motivo_Baja = MotivoBaja;
            var titulo = GetTitulo(cd);
            if(contrato.FechaIMSS == null)
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Baja_General, cd.IdCliente, cd, cd.IdSucursal, contrato.IdContrato, FechaBaja.Value.ToString("dd-MM-yyyy"));
            }else
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Baja_Fiscal, cd.IdCliente, cd, cd.IdSucursal, contrato.IdContrato, FechaBaja.Value.ToString("dd-MM-yyyy"));
            }
            
        }

        public void Recontratacion(int IdEmpleado, DateTime FechaAlta)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var cd = GetDatosPersonales(IdEmpleado);
            cd.Fecha_de_Reingreso = FechaAlta.ToString("dd-MM-yyyy");
            var titulo = GetTitulo(cd);
            if(contrato.IdEmpresaFiscal == null)
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Reingreso_General, cd.IdCliente, cd, cd.IdSucursal, contrato.IdContrato, FechaAlta.ToString("dd-MM-yyyy"));
            }else
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Reingresos, cd.IdCliente, cd, cd.IdSucursal, contrato.IdContrato, FechaAlta.ToString("dd-MM-yyyy"));
            }
            
        }


        public void SalarioDiario(int IdEmpleado, decimal? SD, decimal? SDOld)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var cd = GetDatosPersonales(IdEmpleado);
            cd.SD_Nuevo = SD;
            cd.SD_Viejo = SDOld;
            cd.Tipo_Salario = "SDI";
            var titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.SD, cd.IdCliente, cd,cd.IdSucursal,contrato.IdContrato,SD.ToString());
        }

        public void SalarioDiarioIntegrado(int IdEmpleado, decimal? SDI, decimal? SDIOld)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var cd = GetDatosPersonales(IdEmpleado);
            cd.SDI_Nuevo = SDI;
            cd.SDI_Anterior = SDIOld;
            cd.Tipo_Salario = "SDI";
            var titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.SDI, cd.IdCliente, cd, cd.IdSucursal, contrato.IdContrato, SDI.ToString());
        }

        public void SalarioReal(int IdEmpleado, decimal? salarioReal, decimal? srOld)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var cd = GetDatosPersonales(IdEmpleado);
            cd.Salario_Real_Nuevo = salarioReal;
            cd.Salario_Real_Anterior = srOld;
            cd.Tipo_Salario = "Salario Real";
            var titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Salario_Real, cd.IdCliente, cd,cd.IdSucursal,contrato.IdContrato,salarioReal.ToString());
        }

        public void IMSS(Empleado_Contrato contrato)
        {
            
            var cd = GetDatosPersonales(contrato.IdEmpleado);
            cd.Fecha_IMSS = contrato.FechaIMSS.Value.ToString("dd-MM-yyyy");
            var titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Cambio_Fecha_IMSS, cd.IdCliente, cd, cd.IdSucursal,contrato.IdContrato,contrato.FechaAlta.ToString("dd-MM-yyyy"));
        }

        public void Empresas(int IdEmpleado, CuerpoDatos cd)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var empresa = ctx.Empresa.ToList();
            var idFiscal = empresa.Where(x => x.RazonSocial == cd.Configuracion_Nueva.Fiscal).Select(x=>x.IdEmpresa).FirstOrDefault();
            var idComplemento = empresa.Where(x => x.RazonSocial == cd.Configuracion_Nueva.Complemento).Select(x => x.IdEmpresa).FirstOrDefault();
            var idAsimilado = empresa.Where(x => x.RazonSocial == cd.Configuracion_Nueva.Asimilado).Select(x => x.IdEmpresa).FirstOrDefault();
            var idSindicato = empresa.Where(x => x.RazonSocial == cd.Configuracion_Nueva.Sindicato).Select(x => x.IdEmpresa).FirstOrDefault();
            string idsEmpresas =  idFiscal.ToString()+ "," + idComplemento.ToString() + "," +idAsimilado.ToString() + "," +idSindicato.ToString();
            var other = GetDatosPersonales(IdEmpleado);
            other.Configuracion_Anterior = cd.Configuracion_Anterior;
            other.Configuracion_Nueva = cd.Configuracion_Nueva;
            string titulo = GetTitulo(other);
            if(cd.Configuracion_Nueva.Fiscal == "n/a")
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Tranferencia_General, other.IdCliente, other, contrato.IdSucursal, contrato.IdContrato, idsEmpresas);
            }else
            {
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Transferencia, other.IdCliente, other, contrato.IdSucursal, contrato.IdContrato, idsEmpresas);
            }
            
        }

        public void CambioPuesto(int IdEmpleado, int puestoAnterior, int puestoNuevo)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            var cd = GetDatosPersonales(IdEmpleado);
            cd.Puesto = null;
            Puestos pCtx = new Puestos();
            cd.Puesto_Anterior = pCtx.GetDescripcionById(puestoAnterior);
            cd.Puesto_Nuevo = pCtx.GetDescripcionById(puestoNuevo);
            var titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Puesto, cd.IdCliente, cd, cd.IdSucursal,contrato.IdContrato,puestoNuevo.ToString());
        }

        public void Vacaciones(Vacaciones collection, DatosEmpleadoVacaciones datos, PeriodoVacaciones periodo)
        {
            string fechas = collection.FechaInicio + "," + collection.FechaFin;
            var cd = GetDatosPersonales(datos.IdEmpleado);
            int contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == datos.IdEmpleado).Select(x => x.IdContrato).FirstOrDefault();
            cd.Fecha_Inicio = collection.FechaInicio.ToString("dd-MM-yyyy");
            cd.Fecha_Fin = collection.FechaFin.ToString("dd-MM-yyyy");
            cd.Fecha_Presentacion = collection.Presentarse.ToString("dd-MM-yyyy");
            cd.Dias = collection.DiasTomados.ToString();
            cd.Periodo = periodo.PeridoVacaciones;

            string titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Vacaciones, cd.IdCliente, cd, cd.IdSucursal,contrato,collection.FechaInicio.ToString("dd-MM-yyyy"));
        }

        public void Permisos(Permisos bn, Empleado DatosEmpleado, Puesto DatosPuesto, Departamento DatosDepartamento)
        {
            string fechas = bn.FechaInicio.ToString("dd-MM-yyyy") + "," + bn.FechaFin.ToString("dd-MM-yyyy");
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == DatosEmpleado.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            int? diastomados = 0;
            var cd = GetDatosPersonales(DatosEmpleado.IdEmpleado);
            string titulo = GetTitulo(cd);

            if (bn.Dias == 0)
            {
                diastomados = 1;
            }
            else
            {
                diastomados = bn.Dias;
            }

            cd.Fecha_Inicio = bn.FechaInicio.ToString("dd-MM-yyyy");
            cd.Fecha_Fin = bn.FechaFin.ToString("dd-MM-yyyy");
            cd.Dias = diastomados.ToString();
            
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Permisos, cd.IdCliente, cd, cd.IdSucursal,contrato.IdContrato, bn.FechaInicio.ToString("dd-MM-yyyy"));
        }

        public void BajaIMSS(int IdEmpleado, DateTime? bajaIMSS)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            Empleados emp = new Empleados();
            var empleado = emp.GetEmpleadoById(IdEmpleado);
            if(empleado != null)
            {
                var cd = GetDatosPersonales(IdEmpleado);
                string titulo = GetTitulo(cd);
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Baja_Fiscal, cd.IdCliente, cd, cd.IdSucursal,contrato.IdContrato,bajaIMSS.Value.ToString("dd-MM-yyyy"));
            }
        }
        public static void CumpleIMSS(int idContrato, CumpleIMSS cumple)
        {
            RHEntities ctx = new RHEntities();
            var idcliente = (from c in ctx.Cliente
                             join s in ctx.Sucursal 
                             on c.IdCliente equals s.IdCliente
                             join ec in ctx.Empleado_Contrato
                             on s.IdSucursal equals ec.IdSucursal
                             where ec.IdContrato == idContrato
                             select c.IdCliente).FirstOrDefault();

            var empresa = (from ec in ctx.Empleado_Contrato
                           join em in ctx.Empresa
                           on ec.IdEmpresaFiscal equals em.IdEmpresa
                           where ec.IdContrato == idContrato
                           select em.RazonSocial).FirstOrDefault();
            var datos = (from e in ctx.Empleado
                         join ec in ctx.Empleado_Contrato
                         on e.IdEmpleado equals ec.IdEmpleado
                         where ec.IdContrato == idContrato
                         select new CuerpoDatos
                         {
                             IdCliente =idcliente,
                             IdSucursal = e.IdSucursal,
                             Nombres = e.Nombres,
                             Paterno = e.APaterno,
                             Materno = e.AMaterno,
                             NSS = e.NSS,
                             SDI_Anterior = cumple.SDIViejo,
                             SDI_Nuevo = cumple.SDINuevo,
                             Antiguedad = cumple.Anio.ToString(),
                             Factor = cumple.FactorIntegracion.ToString()
                         }).FirstOrDefault();

            datos.Fecha_IMSS = cumple.FechaIMSS.ToString("dd-MM-yyyy");
            datos.Empresa = empresa;
                if (datos != null)
            {
                
                string titulo = GetTitulo(datos);
                NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Cumple_IMSS, datos.IdCliente, datos, datos.IdSucursal,idContrato,cumple.SDINuevo.ToString());
            }
        }

        private static string GetTitulo(CuerpoDatos cd)
        {
            return cd.Paterno + " " + cd.Materno + " " + cd.Nombres;
        }

        private static CuerpoDatos GetDatosPersonales(int IdEmpleado)
        {
            Empleados e = new Empleados();
            var empleado = e.GetEmpleadoById(IdEmpleado);
            var contrato = e.GetUltimoContrato(IdEmpleado);
            var datosbancarios = e.GetDatosBancoEmpleado(IdEmpleado);
            Clientes c = new Clientes();
            var cliente = c.GetClienteBySucursal(empleado.IdSucursal);
            var idCliente = c.GetIdClienteBySucursal(empleado.IdSucursal);
            CuerpoDatos cd = new CuerpoDatos()
            {
                IdEmpleado = empleado.IdEmpleado,
                Nombres = empleado.Nombres,
                Paterno = empleado.APaterno,
                Materno = empleado.AMaterno,
                NSS = empleado.NSS,
                RFC = empleado.RFC,
                CURP = empleado.CURP,
                Nacionalidad = empleado.Nacionalidad,
                Direccion = empleado.Direccion,
                SDI = contrato.SDI,
                Cliente = cliente,
                IdCliente = idCliente,
                Cuenta_Bancaria = datosbancarios != null ? datosbancarios.CuentaBancaria:"0",
                No_Tarjeta = datosbancarios != null ?  datosbancarios.NumeroTarjeta : "0",
                Beneficiario_Nombre = datosbancarios != null ? datosbancarios.NombreBeneficiario:"-",
                Beneficiario_Parentezco =  datosbancarios != null ? datosbancarios.ParentezcoBeneficiario : "-",
                Beneficiario_Domicilio =  datosbancarios != null ? datosbancarios.DomicilioBeneficiario : "-",
                Beneficiario_Curp = datosbancarios != null ? datosbancarios.CURPBeneficiario : "-",
                Beneficiario_Rfc = datosbancarios != null ? datosbancarios.RFCBeneficiario : "-",
                IdSucursal = contrato.IdSucursal
            };
            if (contrato.FechaIMSS != null)
                cd.Fecha_IMSS = contrato.FechaIMSS.Value.ToString("dd/MM/yyyy");

            Puestos p = new Puestos();
            var puesto = p.GetPuesto(contrato.IdPuesto);
            cd.Puesto = puesto != null ? puesto.Descripcion : "n/a";

            Empresas empresas = new Empresas();

            cd.Empresa = empresas.GetRazonSocialById(contrato.IdEmpresaFiscal);

            return cd;
        }

        private static CuerpoDatos GetDatosPersonalesReenvio(int IdEmpleado)
        {
            Empleados e = new Empleados();
            var empleado = e.GetEmpleadoById(IdEmpleado);
            var contrato = e.GetUltimoContrato(IdEmpleado);
            var datosbancarios = e.GetDatosBancoEmpleado(IdEmpleado);
            Clientes c = new Clientes();
            var cliente = c.GetClienteBySucursal(empleado.IdSucursal);
            var idCliente = c.GetIdClienteBySucursal(empleado.IdSucursal);
            CuerpoDatos cd = new CuerpoDatos()
            {
                Alerta = "Reenvio",
                IdEmpleado = empleado.IdEmpleado,
                Nombres = empleado.Nombres,
                Paterno = empleado.APaterno,
                Materno = empleado.AMaterno,
                NSS = empleado.NSS,
                RFC = empleado.RFC,
                CURP = empleado.CURP,
                Nacionalidad = empleado.Nacionalidad,
                Direccion = empleado.Direccion,
                SDI = contrato.SDI,
                Cliente = cliente,
                IdCliente = idCliente,
                Cuenta_Bancaria = datosbancarios.CuentaBancaria,
                No_Tarjeta = datosbancarios.NumeroTarjeta,
                IdSucursal = contrato.IdSucursal
            };
            if (contrato.FechaIMSS != null)
                cd.Fecha_IMSS = contrato.FechaIMSS.Value.ToString("dd/MM/yyyy");

            Puestos p = new Puestos();
            var puesto = p.GetPuesto(contrato.IdPuesto);
            cd.Puesto = puesto != null ? puesto.Descripcion : "n/a";

            Empresas empresas = new Empresas();

            cd.Empresa = empresas.GetRazonSocialById(contrato.IdEmpresaFiscal);

            return cd;
        }

        public static void INFONAVIT(Empleado_Infonavit model, string movimiento)
        {

            Empleados emp = new Empleados();
            var contrato = emp.GetContratoByIdEmpleadoContrato(model.IdEmpleadoContrato);
            var cd = GetDatosPersonales(contrato.IdEmpleado);
            cd.No_Credito = model.NumCredito;
            cd.Fecha_Inicio = model.FechaInicio.ToString("dd/MM/yyyy");
            cd.Factor_de_Descuento = model.FactorDescuento;
            cd.Tipo_Credito = Utils.GetNameOfEnum(typeof(TipoCreditoInfonavit), model.TipoCredito);
            cd.Tipo_de_Movimiento = movimiento;
            string titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.INFONAVIT, cd.IdCliente, cd, cd.IdSucursal,contrato.IdContrato,model.FechaInicio.ToString("dd-MM-yyyy"));
        }

        public void Incapacidad(Incapacidades model)
        {
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == model.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
            string fechas = model.FechaInicio.ToString("dd-MM-yyyy")+","+model.FechaFin.ToString("dd-MM-yyyy");
            var cd = GetDatosPersonales(model.IdEmpleado);
            cd.Tipo_Incapacidad = model.Tipo;
            cd.Folio = model.Folio;
            cd.Clase_Incapacidad = model.Clase;
            cd.Fecha_Inicio = model.FechaInicio.ToString("dd/MM/yyyy");
            cd.Fecha_Fin = model.FechaFin.ToString("dd/MM/yyyy");
            cd.Dias = model.Dias.ToString();
            string titulo = GetTitulo(cd);
            NotificacionesCommon.CreateNotification(titulo, DateTime.Now, SessionHelpers.GetIdUsuario(), TiposNotificacion.Incapacidades, cd.IdCliente, cd,cd.IdSucursal,contrato.IdContrato,model.FechaInicio.ToString("dd-MM-yyyy"));
        }
    }
}
