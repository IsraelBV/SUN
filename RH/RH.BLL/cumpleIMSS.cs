using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RH.Entidades;
using Common.Enums;
using Common.Utils;
using Novacode;
using Notificacion.BLL;
using System.Data.Entity.Core.Objects;

namespace RH.BLL
{
    public class cumpleIMSS
    {
        RHEntities ctx = null;

        public cumpleIMSS()
        {
            ctx = new RHEntities();            
        }

        //public List<Empresa> obtenerEmpresas()
        //{
        //    var empresas = ctx.Empresa.Where(x=> x.RegistroPatronal != null).ToList();
        //    return empresas;
        //}

        //public List<Cliente> obtenerClientes()
        //{
        //    var clientes = ctx.Cliente.ToList();
        //    return clientes;
        //}

        //public List<sucursalCliente> obtenerSucursal()
        //{
        //    var sucursales = (from s in ctx.Sucursal
        //                      join c in ctx.Cliente
        //                      on s.IdCliente equals c.IdCliente
        //                      select new sucursalCliente
        //                      {
        //                          IdSucursal = s.IdSucursal,
        //                          NombreCliente = c.Nombre,
        //                          NombreSucursal = s.Ciudad
        //                      }).ToList();
        //    return sucursales;
        //}
        public List<sucursalCliente> obtenerSucursal(List<Sucursal> listasucursales)
        {
            using (var context = new RHEntities())
            {
                var sucursales = (from s in listasucursales
                                  join c in context.Cliente
                                  on s.IdCliente equals c.IdCliente
                                  select new sucursalCliente
                                  {
                                      IdSucursal = s.IdSucursal,
                                      NombreCliente = c.Nombre,
                                      NombreSucursal = s.Ciudad
                                  }).ToList();
                return sucursales;
            }
        }

        public List<cumpleañosIMSS> cumpleActuaal(int idSucursal)
        {
            List<cumpleañosIMSS> cumple = new List<cumpleañosIMSS>();

            return cumple;
             
            var empleados = (from emp in ctx.Empleado
                             join empc in ctx.Empleado_Contrato
                             on emp.IdEmpleado equals empc.IdEmpleado
                             where empc.IdSucursal == idSucursal && empc.Status == true && (empc.FechaIMSS.Value.Day >= DateTime.Now.Day && empc.FechaIMSS.Value.Day <= DateTime.Now.Day) && (empc.FechaIMSS.Value.Month >= DateTime.Now.Month && empc.FechaIMSS.Value.Month <= DateTime.Now.Month)
                             select new cumpleañosIMSS {
                                 IdContrato = empc.IdContrato,
                                 IdSucursal = empc.IdSucursal,
                                 Nombre = emp.Nombres,
                                 Paterno = emp.APaterno,
                                 Materno = emp.AMaterno,
                                 IdEmpresaFiscal = empc.IdEmpresaFiscal,
                                 fechaImss = empc.FechaIMSS,
                                 SdiViejo= empc.SDI,
                                 SD = empc.SD
                                 
                                 }).ToList();

            foreach (var e in empleados)
            {


                if (e.fechaImss != null)
                {
                    
                    
                        DateTime oldDate = (DateTime)e.fechaImss;
                        DateTime newDate = DateTime.Now;
                        int anio = newDate.Year - oldDate.Year;
                    int valorFactor = anio == 0 ? 0 : anio + 1;

                    var factor = ctx.C_FactoresIntegracion.FirstOrDefault(x => x.Antiguedad == valorFactor);
                    if (factor == null) continue;

                    var empresa = ctx.Empresa.FirstOrDefault(x => x.IdEmpresa == e.IdEmpresaFiscal);
                    decimal sdnew = e.SD * (factor == null ? 0 : factor.Factor);
                   

                    cumpleañosIMSS item = new cumpleañosIMSS();
                    item.Nombre = e.Nombre;
                        item.Paterno = e.Paterno;
                        item.Materno = e.Materno;
                        item.IdContrato = e.IdContrato;
                        item.IdEmpresaFiscal = e.IdEmpresaFiscal;
                        item.NombreEmpresa = empresa.RazonSocial;
                        item.fechaImss = e.fechaImss;
                        item.AniosCumplidos = anio;
                        item.FactorInt = factor.Factor;
                        item.SdiViejo = e.SdiViejo;
                    item.SdiNuevo = Utils.TruncateDecimales(sdnew);
                    cumple.Add(item);

                }


            }
            return cumple;
        }

        //public List<cumpleañosIMSS> excumpleByEmpresa(int Idempresa, DateTime FechaInicio, DateTime FechaFin)
        //{
        //    List<cumpleañosIMSS> cumpleVerificado = new List<cumpleañosIMSS>();
        //    List<cumpleañosIMSS> cumple = new List<cumpleañosIMSS>();
        //    var empleados = (from emp in ctx.Empleado
        //                     join empc in ctx.Empleado_Contrato
        //                     on emp.IdEmpleado equals empc.IdEmpleado
        //                     where empc.IdEmpresaFiscal == Idempresa && (empc.FechaIMSS.Value.Day >= FechaInicio.Day && empc.FechaIMSS.Value.Day <= FechaFin.Day)
        //                     && (empc.FechaIMSS.Value.Month >= FechaInicio.Month && empc.FechaIMSS.Value.Month <= FechaFin.Month)
        //                     && empc.FechaIMSS.Value != null
        //                     && empc.Status == true
        //                     select new cumpleañosIMSS
        //                     {
        //                         IdContrato = empc.IdContrato,
        //                         IdSucursal = empc.IdSucursal,
        //                         Nombre = emp.Nombres,
        //                         Paterno = emp.APaterno,
        //                         Materno = emp.AMaterno,
        //                         IdEmpresaFiscal = empc.IdEmpresaFiscal,
        //                         fechaImss = empc.FechaIMSS,
        //                         SdiViejo = empc.SDI,
        //                         SD = empc.SD
        //                     }).ToList();


        //    foreach (var e in empleados)
        //    {


        //        if (e.fechaImss != null && e.IdEmpresaFiscal != null)
        //        {

        //            cumpleañosIMSS item = new cumpleañosIMSS();
        //            DateTime oldDate = (DateTime)e.fechaImss;
        //            DateTime newDate = DateTime.Now;
        //            int anio = newDate.Year - oldDate.Year;
        //            int valorFactor = anio == 0 ? 0 : anio + 1;
        //            var factor = ctx.C_FactoresIntegracion.Where(x => x.Antiguedad == valorFactor).FirstOrDefault();
        //            var empresa = ctx.Empresa.Where(x => x.IdEmpresa == e.IdEmpresaFiscal).FirstOrDefault();
        //            string nombreEmpresa = empresa == null ? "No se encontró Empresa" : empresa.RazonSocial;

        //            decimal sdnew = e.SD * (factor == null ? 0 : factor.Factor);
        //            item.Nombre = e.Nombre;
        //            item.Paterno = e.Paterno;
        //            item.Materno = e.Materno;
        //            item.IdContrato = e.IdContrato;
        //            item.IdEmpresaFiscal = e.IdEmpresaFiscal;
        //            item.NombreEmpresa = nombreEmpresa;
        //            item.fechaImss = e.fechaImss;
        //            item.AniosCumplidos = anio;
        //            item.FactorInt = factor == null ? 0 : factor.Factor;
        //            item.SdiViejo = e.SdiViejo;
        //            item.SdiNuevo = Utils.TruncateDecimales(sdnew);
        //            item.Status = false;
        //            cumple.Add(item);

        //        }


        //    }
        //    foreach (var c in cumple)
        //    {
        //        cumpleañosIMSS item = new cumpleañosIMSS();
        //        var busqueda = ctx.CumpleIMSS.Where(x => x.IdContrato == c.IdContrato && x.FechaIMSS == c.fechaImss && x.Anio == c.AniosCumplidos).FirstOrDefault();

        //        if (busqueda != null)
        //        {
        //            item.Nombre = c.Nombre;
        //            item.Paterno = c.Paterno;
        //            item.Materno = c.Materno;
        //            item.IdContrato = c.IdContrato;
        //            item.IdEmpresaFiscal = c.IdEmpresaFiscal;
        //            item.NombreEmpresa = c.NombreEmpresa;
        //            item.fechaImss = c.fechaImss;
        //            item.AniosCumplidos = c.AniosCumplidos;
        //            item.FactorInt = c.FactorInt;
        //            item.SdiViejo = c.SdiViejo;
        //            item.SdiNuevo = c.SdiNuevo;
        //            item.Status = true;
        //            cumpleVerificado.Add(item);

        //        }
        //        else
        //        {
        //            cumpleVerificado.Add(c);
        //        }


        //    }
        //    return cumpleVerificado;
        //}





        //public List<cumpleañosIMSS> cumpleByCliente(int IdCliente, DateTime FechaInicio, DateTime FechaFin)
        //{
        //    List<cumpleañosIMSS> cumple = new List<cumpleañosIMSS>();
        //    List<cumpleañosIMSS> cumpleVerificado = new List<cumpleañosIMSS>();
        //    List<cumpleañosIMSS> empleados = new List<cumpleañosIMSS>();
        //    List<int> sucursalesId = new List<int>();
        //    List<C_FactoresIntegracion> factores = new List<C_FactoresIntegracion>();
        //    List<Empresa> empresas = new List<Empresa>();
        //    List<CumpleIMSS> cumplesImss = new List<CumpleIMSS>();

        //    using (var context = new RHEntities())
        //    {
        //        sucursalesId = context.Sucursal.Where(x => x.IdCliente == IdCliente).Select(x => x.IdSucursal).ToList();

        //        empleados = (from emp in context.Empleado
        //                     join empc in context.Empleado_Contrato
        //                     on emp.IdEmpleado equals empc.IdEmpleado
        //                     where sucursalesId.Contains(empc.IdSucursal) &&
        //                        (
        //                            empc.FechaIMSS.Value.Day >= FechaInicio.Day &&
        //                            empc.FechaIMSS.Value.Day <= FechaFin.Day
        //                        ) &&
        //                        (
        //                            empc.FechaIMSS.Value.Month >= FechaInicio.Month &&
        //                            empc.FechaIMSS.Value.Month <= FechaFin.Month
        //                        ) &&
        //                        empc.FechaIMSS.Value != null &&
        //                        empc.Status == true
        //                     select new cumpleañosIMSS
        //                     {
        //                         IdContrato = empc.IdContrato,
        //                         IdSucursal = empc.IdSucursal,
        //                         Nombre = emp.Nombres,
        //                         Paterno = emp.APaterno,
        //                         Materno = emp.AMaterno,
        //                         IdEmpresaFiscal = empc.IdEmpresaFiscal,
        //                         fechaImss = empc.FechaIMSS,
        //                         SdiViejo = empc.SDI,
        //                         SD = empc.SD
        //                     }).ToList();

        //        factores = context.C_FactoresIntegracion.ToList();
        //        empresas = context.Empresa.ToList();
        //        cumplesImss = context.CumpleIMSS.ToList();
        //    }

        //    foreach (var e in empleados)
        //    {
        //        if (e.fechaImss != null && e.IdEmpresaFiscal != null)
        //        {
        //            cumpleañosIMSS item = new cumpleañosIMSS();
        //            DateTime oldDate = (DateTime)e.fechaImss;
        //            DateTime newDate = DateTime.Now;
        //            int anio = newDate.Year - oldDate.Year;
        //            int valorFactor = anio == 0 ? 0 : anio + 1;
        //            var factor = factores.Where(x => x.Antiguedad == valorFactor).FirstOrDefault();
        //            var empresa = empresas.Where(x => x.IdEmpresa == e.IdEmpresaFiscal).FirstOrDefault();
        //            string nombreEmpresa = empresa == null ? "No se encontró Empresa" : empresa.RazonSocial;
        //            decimal sdnew = e.SD * (factor == null ? 0 : factor.Factor);
        //            item.Nombre = e.Nombre;
        //            item.Paterno = e.Paterno;
        //            item.Materno = e.Materno;
        //            item.IdContrato = e.IdContrato;
        //            item.IdEmpresaFiscal = e.IdEmpresaFiscal;
        //            item.NombreEmpresa = nombreEmpresa;
        //            item.fechaImss = e.fechaImss;
        //            item.AniosCumplidos = anio;
        //            item.FactorInt = factor == null ? 0 : factor.Factor;
        //            item.SdiViejo = e.SdiViejo;
        //            item.SdiNuevo = Utils.TruncateDecimales(sdnew);
        //            item.Status = false;
        //            cumple.Add(item);
        //        }
        //    }

        //    foreach (var c in cumple)
        //    {
        //        cumpleañosIMSS item = new cumpleañosIMSS();
        //        var busqueda = cumplesImss.Where(x => x.IdContrato == c.IdContrato && x.FechaIMSS == c.fechaImss && x.Anio == c.AniosCumplidos).FirstOrDefault();

        //        if (busqueda != null)
        //        {
        //            item.Nombre = c.Nombre;
        //            item.Paterno = c.Paterno;
        //            item.Materno = c.Materno;
        //            item.IdContrato = c.IdContrato;
        //            item.IdEmpresaFiscal = c.IdEmpresaFiscal;
        //            item.NombreEmpresa = c.NombreEmpresa;
        //            item.fechaImss = c.fechaImss;
        //            item.AniosCumplidos = c.AniosCumplidos;
        //            item.FactorInt = c.FactorInt;
        //            item.SdiViejo = c.SdiViejo;
        //            item.SdiNuevo = c.SdiNuevo;
        //            item.Status = true;
        //            cumpleVerificado.Add(item);
        //        }
        //        else
        //        {
        //            cumpleVerificado.Add(c);
        //        }
        //    }

        //    return cumpleVerificado;
        //}



        //public List<cumpleañosIMSS> cumpleBySucursal(int IdSucursal, DateTime FechaInicio, DateTime FechaFin)
        //{
        //    List<cumpleañosIMSS> cumple = new List<cumpleañosIMSS>();
        //    List<cumpleañosIMSS> cumpleVerificado = new List<cumpleañosIMSS>();
        //    var empleados = (from emp in ctx.Empleado
        //                     join empc in ctx.Empleado_Contrato
        //                     on emp.IdEmpleado equals empc.IdEmpleado
        //                     where empc.IdSucursal == IdSucursal
        //                     && (empc.FechaIMSS.Value.Day >= FechaInicio.Day && empc.FechaIMSS.Value.Day <= FechaFin.Day)
        //                     && (empc.FechaIMSS.Value.Month >= FechaInicio.Month && empc.FechaIMSS.Value.Month <= FechaFin.Month)
        //                     && empc.FechaIMSS.Value != null
        //                     && empc.Status == true
        //                     select new cumpleañosIMSS
        //                     {
        //                         IdContrato = empc.IdContrato,
        //                         IdSucursal = empc.IdSucursal,
        //                         Nombre = emp.Nombres,
        //                         Paterno = emp.APaterno,
        //                         Materno = emp.AMaterno,
        //                         IdEmpresaFiscal = empc.IdEmpresaFiscal,
        //                         fechaImss = empc.FechaIMSS,
        //                         SdiViejo = empc.SDI,
        //                         SD = empc.SD
        //                     }).ToList();


        //    foreach (var e in empleados)
        //    {


        //        if (e.fechaImss != null && e.IdEmpresaFiscal != null)
        //        {

        //            cumpleañosIMSS item = new cumpleañosIMSS();

        //            DateTime oldDate = (DateTime)e.fechaImss;
        //            DateTime newDate = DateTime.Now;
        //            int anio = newDate.Year - oldDate.Year;
        //            int valorFactor = anio == 0 ? 0 : anio + 1;
        //            var factor = ctx.C_FactoresIntegracion.Where(x => x.Antiguedad == valorFactor).FirstOrDefault();
        //            var empresa = ctx.Empresa.Where(x => x.IdEmpresa == e.IdEmpresaFiscal).FirstOrDefault();

        //            string nombreEmpresa = empresa == null ? "No se encontró Empresa" : empresa.RazonSocial;
        //            decimal sdnew = e.SD * (factor == null ? 0 : factor.Factor);

        //            item.Nombre = e.Nombre;
        //            item.Paterno = e.Paterno;
        //            item.Materno = e.Materno;
        //            item.IdContrato = e.IdContrato;
        //            item.IdEmpresaFiscal = e.IdEmpresaFiscal;
        //            item.NombreEmpresa = nombreEmpresa;
        //            item.fechaImss = e.fechaImss;
        //            item.AniosCumplidos = anio;
        //            item.FactorInt = factor == null ? 0 : factor.Factor;
        //            item.SdiViejo = e.SdiViejo;
        //            item.SdiNuevo = Utils.TruncateDecimales(sdnew);
        //            cumple.Add(item);

        //        }


        //    }
        //    foreach (var c in cumple)
        //    {
        //        cumpleañosIMSS item = new cumpleañosIMSS();
        //        var busqueda = ctx.CumpleIMSS.Where(x => x.IdContrato == c.IdContrato && x.FechaIMSS == c.fechaImss && x.Anio == c.AniosCumplidos).FirstOrDefault();

        //        if (busqueda != null)
        //        {
        //            item.Nombre = c.Nombre;
        //            item.Paterno = c.Paterno;
        //            item.Materno = c.Materno;
        //            item.IdContrato = c.IdContrato;
        //            item.IdEmpresaFiscal = c.IdEmpresaFiscal;
        //            item.NombreEmpresa = c.NombreEmpresa;
        //            item.fechaImss = c.fechaImss;
        //            item.AniosCumplidos = c.AniosCumplidos;
        //            item.FactorInt = c.FactorInt;
        //            item.SdiViejo = c.SdiViejo;
        //            item.SdiNuevo = c.SdiNuevo;
        //            item.Status = true;
        //            cumpleVerificado.Add(item);

        //        }
        //        else
        //        {
        //            cumpleVerificado.Add(c);
        //        }


        //    }
        //    return cumpleVerificado;
        //}

        public List<cumpleañosIMSS> cumpleByEmpresa(int Idempresa, DateTime FechaInicio,DateTime FechaFin)
        {
            List<cumpleañosIMSS> cumpleVerificado = new List<cumpleañosIMSS>();
            int anioPresente = DateTime.Now.Year;

            using (var context = new RHEntities()) {

                cumpleVerificado = (from emp in context.Empleado
                    join empc in context.Empleado_Contrato
                        on emp.IdEmpleado equals empc.IdEmpleado
                    join empr in context.Empresa
                        on empc.IdEmpresaFiscal equals empr.IdEmpresa
                    join fct in context.C_FactoresIntegracion
                        on ((anioPresente - empc.FechaIMSS.Value.Year) == 0 ? 0 : anioPresente - empc.FechaIMSS.Value.Year + 1) equals fct.Antiguedad
                    join cmpl in context.CumpleIMSS
                        on new {empc.IdContrato, fechaimss = empc.FechaIMSS.Value, anio = anioPresente - empc.FechaIMSS.Value.Year} equals new {cmpl.IdContrato, fechaimss = cmpl.FechaIMSS, anio = cmpl.Anio} into cumplimss
                        from cmpl in cumplimss.DefaultIfEmpty()
                    where empc.IdEmpresaFiscal == Idempresa && 
                    (
                        empc.FechaIMSS.Value.Day >= FechaInicio.Day && 
                        empc.FechaIMSS.Value.Day <= FechaFin.Day
                    ) && 
                    (
                        empc.FechaIMSS.Value.Month >= FechaInicio.Month && 
                        empc.FechaIMSS.Value.Month <= FechaFin.Month
                    )
                    && empc.FechaIMSS.Value != null
                    && empc.Status == true
                    && empc.IdEmpresaFiscal != null
                    select new cumpleañosIMSS
                    {
                        IdContrato = empc.IdContrato,
                        //IdSucursal = empc.IdSucursal,
                        Nombre = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        IdEmpresaFiscal = empc.IdEmpresaFiscal,
                        NombreEmpresa = empr.RazonSocial ?? "No se encontró Empresa", //<-----
                        fechaImss = empc.FechaIMSS,
                        AniosCumplidos = anioPresente - empc.FechaIMSS.Value.Year, //<-----
                        FactorInt = fct != null? fct.Factor:0 ,//<-----
                        SdiViejo = empc.SDI,
                        SdiNuevo = Math.Truncate(empc.SD*(fct == null ? 0 : fct.Factor) * 100) / 100,//<-----
                        Status = cmpl != null ? true : false
                        //SD = empc.SD
                    }).ToList();
            }

            return cumpleVerificado;
        }

        public List<cumpleañosIMSS> cumpleByCliente(int IdCliente, DateTime FechaInicio, DateTime FechaFin)
        {
            List<cumpleañosIMSS> cumpleVerificado = new List<cumpleañosIMSS>();
            int anioPresente = DateTime.Now.Year;

            using (var context = new RHEntities()) {

                cumpleVerificado = (from emp in context.Empleado
                    join empc in context.Empleado_Contrato
                        on emp.IdEmpleado equals empc.IdEmpleado
                    join empr in context.Empresa
                        on empc.IdEmpresaFiscal equals empr.IdEmpresa
                    join fct in context.C_FactoresIntegracion
                        on ((anioPresente - empc.FechaIMSS.Value.Year) == 0 ? 0 : anioPresente - empc.FechaIMSS.Value.Year + 1) equals fct.Antiguedad
                    join suc in context.Sucursal
                        on IdCliente equals suc.IdCliente
                    join cmpl in context.CumpleIMSS
                        on new {empc.IdContrato, fechaimss = empc.FechaIMSS.Value, anio = anioPresente - empc.FechaIMSS.Value.Year} equals new {cmpl.IdContrato, fechaimss = cmpl.FechaIMSS, anio = cmpl.Anio} into cumplimss
                        from cmpl in cumplimss.DefaultIfEmpty()
                    where empc.IdSucursal == suc.IdSucursal &&
                    (
                        empc.FechaIMSS.Value.Day >= FechaInicio.Day && 
                        empc.FechaIMSS.Value.Day <= FechaFin.Day
                    ) && 
                    (
                        empc.FechaIMSS.Value.Month >= FechaInicio.Month && 
                        empc.FechaIMSS.Value.Month <= FechaFin.Month
                    )
                    && empc.FechaIMSS.Value != null
                    && empc.Status == true
                    && empc.IdEmpresaFiscal != null
                    select new cumpleañosIMSS
                    {
                        IdContrato = empc.IdContrato,
                        //IdSucursal = empc.IdSucursal,
                        Nombre = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        IdEmpresaFiscal = empc.IdEmpresaFiscal,
                        NombreEmpresa = empr.RazonSocial ?? "No se encontró Empresa", //<-----
                        fechaImss = empc.FechaIMSS,
                        AniosCumplidos = anioPresente - empc.FechaIMSS.Value.Year, //<-----
                        FactorInt = fct != null? fct.Factor:0 ,//<-----
                        SdiViejo = empc.SDI,
                        SdiNuevo = Math.Truncate(empc.SD*(fct == null ? 0 : fct.Factor) * 100) / 100,//<-----
                        Status = cmpl != null ? true : false
                        //SD = empc.SD
                    }).ToList();
            }

            return cumpleVerificado;
        }
            
        public List<cumpleañosIMSS> cumpleBySucursal(int IdSucursal, DateTime FechaInicio, DateTime FechaFin)
        {
            List<cumpleañosIMSS> cumpleVerificado = new List<cumpleañosIMSS>();
            int anioPresente = DateTime.Now.Year;

            using (var context = new RHEntities())
            {
                cumpleVerificado = (from emp in context.Empleado
                                    join empc in context.Empleado_Contrato
                                        on emp.IdEmpleado equals empc.IdEmpleado
                                    join empr in context.Empresa
                                        on empc.IdEmpresaFiscal equals empr.IdEmpresa
                                    join fct in context.C_FactoresIntegracion
                                        on ((anioPresente - empc.FechaIMSS.Value.Year) == 0 ? 0 : anioPresente - empc.FechaIMSS.Value.Year + 1) equals fct.Antiguedad
                                    join cmpl in context.CumpleIMSS
                                        on new { empc.IdContrato, fechaimss = empc.FechaIMSS.Value, anio = anioPresente - empc.FechaIMSS.Value.Year } equals new { cmpl.IdContrato, fechaimss = cmpl.FechaIMSS, anio = cmpl.Anio } into cumplimss
                                    from cmpl in cumplimss.DefaultIfEmpty()
                                    where empc.IdSucursal == IdSucursal &&
                                    (
                                        empc.FechaIMSS.Value.Day >= FechaInicio.Day &&
                                        empc.FechaIMSS.Value.Day <= FechaFin.Day
                                    ) &&
                                    (
                                        empc.FechaIMSS.Value.Month >= FechaInicio.Month &&
                                        empc.FechaIMSS.Value.Month <= FechaFin.Month
                                    )
                                    && empc.FechaIMSS.Value != null
                                    && empc.Status == true
                                    && empc.IdEmpresaFiscal != null
                                    select new cumpleañosIMSS
                                    {
                                        IdContrato = empc.IdContrato,
                                        //IdSucursal = empc.IdSucursal,
                                        Nombre = emp.Nombres,
                                        Paterno = emp.APaterno,
                                        Materno = emp.AMaterno,
                                        IdEmpresaFiscal = empc.IdEmpresaFiscal,
                                        NombreEmpresa = empr.RazonSocial ?? "No se encontró Empresa", //<-----
                                        fechaImss = empc.FechaIMSS,
                                        AniosCumplidos = anioPresente - empc.FechaIMSS.Value.Year, //<-----
                                        FactorInt = fct != null ? fct.Factor : 0,//<-----
                                        SdiViejo = empc.SDI,
                                        SdiNuevo = Math.Truncate(empc.SD * (fct == null ? 0 : fct.Factor) * 100) / 100,//<-----
                                        Status = cmpl != null ? true : false
                                        //SD = empc.SD
                                    }).ToList();
            }

            return cumpleVerificado;
        }

        public List<respuesta> registrarCumpleIMSS(List<CumpleIMSS> cumple,int iduser)
        {
            var result = false;
            List<respuesta> res = new List<respuesta>();
            
            foreach(var c in cumple)
            {
                respuesta itemr = new respuesta();
                Empleado emp = new Empleado();
                 emp = (from ec in ctx.Empleado_Contrato
                           join e in ctx.Empleado
                           on ec.IdEmpleado equals e.IdEmpleado
                           where ec.IdContrato == c.IdContrato
                           select e).FirstOrDefault();

                var busqueda = ctx.CumpleIMSS.Where(x => x.IdContrato == c.IdContrato && x.FechaIMSS == c.FechaIMSS && x.Anio == c.Anio).FirstOrDefault();

                if (busqueda == null)
                {

                    c.Usuario = iduser;
                    c.FechaRegistro = DateTime.Now;
                    ctx.CumpleIMSS.Add(c);

                    var r = ctx.SaveChanges();

                    if (r > 0)
                        result = true;

                    if (result == true)
                    {
                        //Guarda los cambios al contrato
                        var item = ctx.Empleado_Contrato.FirstOrDefault(x => x.IdContrato == c.IdContrato);

                        if (item != null)
                        {
                            var sdiAnterior = item.SDI;

                            item.SDI = c.SDINuevo;
                            item.SBC = c.SDINuevo;
                            ctx.SaveChanges();

                            //Agregar un registro al Kardex
                            KardexEmpleado kardex = new KardexEmpleado();
                            kardex.Salarios(item.IdEmpleado, sdiAnterior, c.SDINuevo, (int) TipoKardex.Cumple_Imss, iduser);
                        }

                    }
                    var noti = new Notificaciones();
                    Notificaciones.CumpleIMSS(c.IdContrato, c);
                    noti.SalarioDiarioIntegrado(emp.IdEmpleado, c.SDINuevo, c.SDIViejo);
                    itemr.Nombre = emp.Nombres;
                    itemr.Paterno = emp.APaterno;
                    itemr.Materno = emp.AMaterno;
                    itemr.Estatus = true;
                    res.Add(itemr);
                }
                else
                {
                    itemr.Nombre = emp.Nombres;
                    itemr.Paterno = emp.APaterno;
                    itemr.Materno = emp.AMaterno;
                    itemr.Estatus = false;
                    res.Add(itemr);
                }
            }

            return res; 
   
        }
    }


    public class cumpleañosIMSS
    {
        public int IdContrato { get; set; }
        public int IdSucursal { get; set; }
        public int? IdEmpresaFiscal { get; set; }
        public string NombreEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public DateTime?  fechaImss { get; set; }
        public int AniosCumplidos { get; set; }
        public decimal FactorInt { get; set; }
        public decimal SdiViejo { get; set; }
        public decimal SdiNuevo { get; set; }
        public decimal SD { get; set; }
        public bool Status { get; set; }

    }
    public class sucursalCliente
    {
        public int IdSucursal { get; set; }
        public string NombreCliente { get; set; }
        public string NombreSucursal { get; set; }
    }

    public class respuesta
    {
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public bool Estatus { get; set; }
    }
}
