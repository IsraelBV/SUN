using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using RH.Entidades;
using RH.BLL;
using Common.Enums;
using Common.Utils;
using Novacode;
using Notificacion.BLL;
using Common.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using SYA.BLL;

namespace RH.BLL
{
    public class Empleados
    {
        RHEntities ctx = null;


        public Empleados()
        {
            ctx = new RHEntities();
        }

        public Empresa GetEmpresaFiscalByEmpleado(int IdEmpleado)
        {
            var result = (from ec in ctx.Empleado_Contrato
                          join e in ctx.Empresa on ec.IdEmpresaFiscal equals e.IdEmpresa
                          where ec.IdEmpleado == IdEmpleado && ec.Status == true
                          select e
             ).FirstOrDefault();

            return result;
        }

        public bool ConfigurarEmpleadoEmpresas(int IdEmpleado, int? IdEmpresaFiscal, int? IdEmpresaComplemento, int? IdEmpresaAsimilado, int? IdEmpresaSindicato, string fechaIMSS)
        {
            //var contrato = GetUltimoContrato(IdEmpleado);
            var contrato = ctx.Empleado_Contrato.Where(s => s.IdEmpleado == IdEmpleado).OrderByDescending(s => s.IdContrato).FirstOrDefault();

            CuerpoDatos cd = new CuerpoDatos();
            Empresas emp = new Empresas();
            cd.Configuracion_Anterior = new Configuracion_Empresa();
            cd.Configuracion_Nueva = new Configuracion_Empresa();
            cd.Configuracion_Anterior.Fiscal = emp.GetRazonSocialById(contrato.IdEmpresaFiscal);
            contrato.IdEmpresaFiscal = IdEmpresaFiscal;
            cd.Configuracion_Nueva.Fiscal = emp.GetRazonSocialById(contrato.IdEmpresaFiscal);

            cd.Configuracion_Anterior.Complemento = emp.GetRazonSocialById(contrato.IdEmpresaComplemento);
            contrato.IdEmpresaComplemento = IdEmpresaComplemento;
            cd.Configuracion_Nueva.Complemento = emp.GetRazonSocialById(contrato.IdEmpresaComplemento);

            cd.Configuracion_Anterior.Asimilado = emp.GetRazonSocialById(contrato.IdEmpresaAsimilado);
            contrato.IdEmpresaAsimilado = IdEmpresaAsimilado;
            cd.Configuracion_Nueva.Asimilado = emp.GetRazonSocialById(contrato.IdEmpresaAsimilado);

            cd.Configuracion_Anterior.Sindicato = emp.GetRazonSocialById(contrato.IdEmpresaSindicato);
            contrato.IdEmpresaSindicato = IdEmpresaSindicato;
            cd.Configuracion_Nueva.Sindicato = emp.GetRazonSocialById(contrato.IdEmpresaSindicato);

            cd.IdSucursal = contrato.IdSucursal;
            if (IdEmpresaFiscal != null && IdEmpresaFiscal != 0)
            {
                contrato.FechaIMSS = Convert.ToDateTime(fechaIMSS);
                cd.Fecha_IMSS = fechaIMSS;
            }
            else
            {
                contrato.FechaIMSS = null;
            }

            var response = ctx.SaveChanges();
            if (response > 0)
            {
                var noti = new Notificaciones();
                noti.Empresas(IdEmpleado, cd);
                return true;
            }
            return false;
        }

        public string GetNombreCompleto(int IdEmpleado)
        {
            var empleado = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == IdEmpleado);
            if (empleado == null) return null;
            return GetNombreCompleto(empleado);
        }

        public string GetNombreCompleto(Empleado empleado)
        {
            return empleado.APaterno + " " + empleado.AMaterno + " " + empleado.Nombres;
        }

        public List<DatosEmpleado> GetEmpleadosBySucursalConTipoNomina(int IdSucursal)
        {
            return (from e in ctx.Empleado
                    let c =
                        ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado)
                            .OrderByDescending(x => x.IdContrato)
                            .FirstOrDefault()
                    where e.IdSucursal == IdSucursal
                          && c.IdEmpresaFiscal != null || c.IdEmpresaAsimilado != null || c.IdEmpresaComplemento != null || c.IdEmpresaSindicato != null
                    select new DatosEmpleado
                    {
                        IdEmpleado = e.IdEmpleado,
                        Nombres = e.Nombres,
                        Paterno = e.APaterno,
                        Materno = e.AMaterno,
                        Status = e.Status,
                        TipoNomina = c.IdPeriodicidadPago
                    }
                    ).Distinct().ToList();
        }

        public List<DatosEmpleado> GetEmpleadosByTipoNomina(int IdSucursal, int TipoNomina)
        {
            //return GetEmpleadosBySucursalConTipoNomina(IdSucursal).Where(x => x.TipoNomina == TipoNomina).ToList();

            return GetEmpleadosBySucursalConTipoNomina(IdSucursal).Where(x => x.TipoNomina == TipoNomina).ToList();
        }


        public List<DatosEmpleado> GetCumpleIMSS(DateTime fechaini, DateTime fechafin, int Empresa)
        {
            return (from emp in ctx.Empleado
                    join con in ctx.Empleado_Contrato on emp.IdEmpleado equals con.IdEmpleado
                    join s in ctx.Sucursal on emp.IdSucursal equals s.IdSucursal
                    join c in ctx.Cliente on s.IdCliente equals c.IdCliente
                    where con.IdEmpresaFiscal == Empresa && con.Status == true && con.FechaIMSS != null &&
                    (con.FechaIMSS.Value.Day >= fechaini.Day && con.FechaIMSS.Value.Day <= fechafin.Day && con.FechaIMSS.Value.Month >= fechaini.Month && con.FechaIMSS.Value.Month <= fechafin.Month)
                    select new DatosEmpleado
                    {
                        IdEmpleado = emp.IdEmpleado,
                        Nombres = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        FechaMovimiento = con.FechaIMSS.Value,
                        Cliente = c.Nombre
                    }
                    ).ToList();
        }

        public List<MovimientosViewModel> GetCumpleIMSSRecientes(DateTime date)
        {
            ControlUsuario cu = new ControlUsuario();
            int[] sucursales = cu.GetSucursalesUsuario(ControlAcceso.GetUsuarioEnSession());
            return (from emp in ctx.Empleado
                    join con in ctx.Empleado_Contrato on emp.IdEmpleado equals con.IdEmpleado
                    join s in ctx.Sucursal on emp.IdSucursal equals s.IdSucursal
                    join c in ctx.Cliente on s.IdCliente equals c.IdCliente
                    join e in ctx.Empresa on con.IdEmpresaFiscal equals e.IdEmpresa
                    where con.Status == true && con.FechaIMSS != null &&
                    (con.FechaIMSS.Value.Day >= date.Day && con.FechaIMSS.Value.Day <= DateTime.Today.Day && con.FechaIMSS.Value.Month >= date.Month && con.FechaIMSS.Value.Month <= DateTime.Today.Month)
                     && sucursales.Contains(s.IdSucursal)
                    select new MovimientosViewModel
                    {
                        IdEmpleado = emp.IdEmpleado,
                        Nombres = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        Fecha = con.FechaIMSS.Value,
                        Cliente = c.Nombre,
                        Empresa = e.RazonSocial,
                        TipoMovimiento = "Cumple IMSS",
                        IdTipoMovimiento = 7,
                    }).ToList();
        }

        public RegistroDatos GetEmpresaFiscal(int id)
        {
            return (from ee in ctx.Empleado_Contrato
                    join emp in ctx.Empresa on ee.IdEmpresaFiscal equals emp.IdEmpresa
                    where ee.IdEmpleado == id
                    select new RegistroDatos
                    {
                        IdRegistro = emp.IdEmpresa,
                        RazonSocial = emp.RazonSocial
                    }).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene solamente los empleados activos de cada sucursal
        /// </summary>
        /// <param name="sucursal"></param>
        /// <returns></returns>
        public List<DatosEmpleado> ObtenerEmpleadosPorSucursal(int sucursal)
        {
            List<DatosEmpleado> lista = new List<DatosEmpleado>();
            List<Incapacidades> listaIncapacidades = new List<Incapacidades>();

            using (var context = new RHEntities())
            {
                lista = (from emp in context.Empleado
                         join c in context.Empleado_Contrato on emp.IdEmpleado equals c.IdEmpleado
                         join p in context.Puesto on c.IdPuesto equals p.IdPuesto
                         where emp.IdSucursal == sucursal && c.Status == true && emp.Status == true
                         select new DatosEmpleado
                         {
                             IdEmpleado = emp.IdEmpleado,
                             IdContrato = c.IdContrato,
                             Nombres = emp.Nombres,
                             Materno = emp.AMaterno,
                             Paterno = emp.APaterno,
                             Puesto = p.Descripcion,
                             CURP = emp.CURP,
                             NSS = emp.NSS,
                             RFC = emp.RFC,
                             RFCValidadoSAT = emp.RFCValidadoSAT,
                             FechaAlta = c.FechaAlta,
                             Status = emp.Status,
                             Incapacidad = false
                         })
                       .ToList();

                var arrayIdEmp = lista.Select(x => x.IdEmpleado).ToArray();

                DateTime dt = DateTime.Now;
                var fechaHoy = new DateTime(dt.Year, dt.Month, dt.Day);

                listaIncapacidades = (from inca in context.Incapacidades
                                      where arrayIdEmp.Contains(inca.IdEmpleado)
                                      && (inca.FechaInicio <= fechaHoy && inca.FechaFin >= fechaHoy)
                                      select inca).ToList();

            }


            if (listaIncapacidades != null)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    lista[i].Incapacidad = incapacidadActiva2(listaIncapacidades, lista[i].IdEmpleado);
                }
            }

            return lista;


        }
        private bool incapacidadActiva(int Idemple)
        {
            DateTime dt = DateTime.Now;
            var fechaHoy = new DateTime(dt.Year, dt.Month, dt.Day);

            var Incapacidad = ctx.Incapacidades.Where(x => x.IdEmpleado == Idemple && x.FechaInicio <= fechaHoy && x.FechaFin >= fechaHoy).FirstOrDefault();
            if (Incapacidad != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool incapacidadActiva2(List<Incapacidades> lista, int Idemple)
        {

            var Incapacidad = lista.Where(x => x.IdEmpleado == Idemple).FirstOrDefault();
            if (Incapacidad != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene los empleados activos e inactivos de la sucursal
        /// </summary>
        /// <param name="IdSucursal"></param>
        /// <returns></returns>
        public List<DatosEmpleado> GetEmpleadosBySucursal(int IdSucursal)
        {
            return (from e in ctx.Empleado
                    let c = ctx.Empleado_Contrato
                        .Where(x => x.IdEmpleado == e.IdEmpleado)
                        .OrderByDescending(x => x.IdContrato)
                        .FirstOrDefault()
                    where e.IdSucursal == IdSucursal
                    select new DatosEmpleado
                    {
                        IdEmpleado = e.IdEmpleado,
                        Nombres = e.Nombres,
                        Paterno = e.APaterno,
                        Materno = e.AMaterno,
                        FechaAlta = c.FechaAlta,
                        FechaBaja = c.FechaBaja,
                        Status = e.Status
                    }
                    ).ToList();
        }

        /// <summary>
        /// Obtiene solamente los empleados inactivos de cada sucursal
        /// </summary>
        /// <param name="sucursal"></param>
        /// <returns></returns>
        public List<DatosEmpleado> GetEmpleadosInactivos(int sucursal)
        {
            return (from emp in ctx.Empleado
                    let c = ctx.Empleado_Contrato
                        .Where(x => x.IdEmpleado == emp.IdEmpleado)
                        .OrderByDescending(x => x.FechaBaja)
                        .FirstOrDefault()
                    where emp.Status == false && emp.IdSucursal == sucursal
                    select new DatosEmpleado
                    {
                        IdEmpleado = emp.IdEmpleado,
                        Nombres = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        RFC = emp.RFC,
                        RFCValidadoSAT = emp.RFCValidadoSAT,
                        CURP = emp.CURP,
                        NSS = emp.NSS,
                        FechaBaja = c.FechaBaja
                    })
                    .ToList();
        }

        public List<DatosEmpleado> ObtenerEmpleadosConPuesto(int sucursal)
        {
            return (from emp in ctx.Empleado
                    join c in ctx.Empleado_Contrato on emp.IdEmpleado equals c.IdEmpleado
                    join p in ctx.Puesto on c.IdPuesto equals p.IdPuesto
                    where emp.IdSucursal == sucursal && c.Status == true
                    select new DatosEmpleado
                    {
                        IdEmpleado = emp.IdEmpleado,
                        Nombres = emp.Nombres,
                        Materno = emp.AMaterno,
                        Paterno = emp.APaterno,
                        Puesto = p.Descripcion
                    })
                        .ToList();
        }

        public List<DatosEmpleado> GetEmpleadosFiscales(SYA_Usuarios user)
        {
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<Empleado_Contrato> listaEmpleadoContratos = new List<Empleado_Contrato>();
            List<Sucursal> listaSucursals = new List<Sucursal>();
            List<Cliente> listaClientes = new List<Cliente>();
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<Empleado_Infonavit> listaEmpleadoInfonavits = new List<Empleado_Infonavit>();
            List<DatosEmpleado> listaE = new List<DatosEmpleado>();
            using (var context = new RHEntities())
            {
                ControlUsuario cu = new ControlUsuario();
                int[] sucursales = cu.GetSucursalesUsuario(user);

                listaEmpleadoContratos = (from c in context.Empleado_Contrato
                                          where sucursales.Contains(c.IdSucursal)
                                          select c).ToList();

                var arrayIdEmpleados = listaEmpleadoContratos.Select(x => x.IdEmpleado).Distinct().ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

                listaSucursals = context.Sucursal.ToList();
                listaEmpresas = context.Empresa.ToList();
                listaClientes = context.Cliente.ToList();

                var arrayIdContratos = listaEmpleadoContratos.Select(x => x.IdContrato).ToArray();

                listaEmpleadoInfonavits = (from inf in context.Empleado_Infonavit
                                           where arrayIdContratos.Contains(inf.IdEmpleadoContrato)
                                           select inf).ToList();
            }

            //ordenamos los empleados por nombre
            listaEmpleados = listaEmpleados.OrderBy(x => x.Nombres).ToList();

            //Por cada empleado
            foreach (var itemEmpleado in listaEmpleados)
            {
                //obtenemos los contratos
                var contratos = listaEmpleadoContratos.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).OrderByDescending(x => x.IdContrato).ToList();

                //Por cada contrato
                foreach (var itemContrato in contratos)
                {
                    //Buscamos la sucursal
                    var itemSuc = listaSucursals.FirstOrDefault(x => x.IdSucursal == itemContrato.IdSucursal);
                    //Buscamos su empresa
                    var itemEmpre = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == itemContrato.IdEmpresaFiscal);
                    //Buscamos su cliente

                    Cliente itemClien = null;
                    if (itemSuc != null)
                        itemClien = listaClientes.FirstOrDefault(x => x.IdCliente == itemSuc.IdCliente);
                    //Buscamos su creditoinfornavit
                    var itemCreditInf = listaEmpleadoInfonavits.FirstOrDefault(x => x.IdEmpleadoContrato == itemContrato.IdContrato);

                    var strEmprsa = "";

                    if (itemEmpre?.RazonSocial != null)
                    {
                        // strEmprsa = itemEmpre.RazonSocial.Length > 14 ? itemEmpre.RazonSocial.Substring(0, 14) : itemEmpre.RazonSocial;

                        var splitName = itemEmpre.RazonSocial.Split(' ');
                        strEmprsa = splitName[0];
                    }

                    //creamos el item y lo agregamos a la lista
                    DatosEmpleado itemDatos = new DatosEmpleado()
                    {
                        IdEmpleado = itemEmpleado.IdEmpleado,
                        IdContrato = itemContrato.IdContrato,
                        Nombres = itemEmpleado.Nombres,
                        Paterno = itemEmpleado.APaterno,
                        Materno = itemEmpleado.AMaterno,
                        Empresa = strEmprsa,
                        EmpresaFiscal = itemEmpre?.RazonSocial ?? "--",
                        RegistroPatronal = itemEmpre?.RegistroPatronal ?? "--",
                        EmpresaRFC = itemEmpre?.RFC ?? "--",
                        Cliente = itemClien?.Nombre ?? "no se encontró",
                        FechaBaja = itemContrato.FechaBaja,
                        Infonavit = itemCreditInf != null,
                        Status = itemContrato.Status
                    };

                    listaE.Add(itemDatos);

                }

            }

            //var listaE = (from emp in context.Empleado
            //    join contra in context.Empleado_Contrato on emp.IdEmpleado equals contra.IdEmpleado
            //    join sucu in context.Sucursal   on contra.IdSucursal equals sucu.IdSucursal
            //    join cli in context.Cliente on sucu.IdCliente equals cli.IdCliente
            //    join empre in context.Empresa on contra.IdEmpresaFiscal equals empre.IdEmpresa
            //    where sucursales.Contains(sucu.IdSucursal)
            //    //&& contra.Status == true
            //select new DatosEmpleado
            //{
            //    IdEmpleado = emp.IdEmpleado,
            //    IdContrato = contra.IdContrato,
            //    Nombres = emp.Nombres,
            //    Paterno = emp.APaterno,
            //    Materno = emp.AMaterno,
            //    Empresa = empre.RazonSocial,
            //    RegistroPatronal = empre.RegistroPatronal,
            //    EmpresaRFC = empre.RFC,
            //    Cliente = cli.Nombre,
            //    FechaBaja = contra.FechaBaja
            //}).ToList();


            //Get los credito infonavit activos
            //var listaCreditosInfonavit = (from info in context.Empleado_Infonavit
            //    where info.Status == true 
            //    select  info).ToList();

            //foreach (var item in listaE)
            //{
            //    var findInfo = listaCreditosInfonavit.FirstOrDefault(x => x.IdEmpleadoContrato == item.IdContrato);
            //    if (findInfo != null)
            //    {
            //        item.Infonavit = true;
            //    }
            //}

            return listaE;


            //    var lista = (from emp in ctx.Empleado
            //                 let con = ctx.Empleado_Contrato.OrderByDescending(x => x.IdContrato).FirstOrDefault(x => x.IdEmpleado == emp.IdEmpleado)
            //                 join s in ctx.Sucursal on emp.IdSucursal equals s.IdSucursal
            //                 join cli in ctx.Cliente on s.IdCliente equals cli.IdCliente
            //                 join empr in ctx.Empresa on con.IdEmpresaFiscal equals empr.IdEmpresa
            //                 where con.BajaIMSS == null && cli.Status == true
            //                 select new DatosEmpleado
            //                 {
            //                     IdEmpleado = emp.IdEmpleado,
            //                     Nombres = emp.Nombres,
            //                     Paterno = emp.APaterno,
            //                     Materno = emp.AMaterno,
            //                     Empresa = empr.RazonSocial,
            //                     RegistroPatronal = empr.RegistroPatronal,
            //                     EmpresaRFC = empr.RFC,
            //                     Cliente = cli.Nombre,
            //                     FechaBaja = con.FechaBaja
            //                 }).Distinct().ToList();

            //return lista;
        }

        public List<DatosEmpleado> GetInfoEmpleadosReporte(int Id, bool activos)
        {

            var ls = (from e in ctx.Empleado.AsEnumerable()
                      let c =
                          ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado)
                              .OrderByDescending(x => x.IdContrato)
                              .FirstOrDefault()
                      let ef =
                          ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaFiscal).Select(x => x.RazonSocial).FirstOrDefault()
                      let ec =
                          ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaComplemento)
                              .Select(x => x.RazonSocial)
                              .FirstOrDefault()
                      let ea =
                          ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaAsimilado)
                              .Select(x => x.RazonSocial)
                              .FirstOrDefault()
                      let es =
                          ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaSindicato)
                              .Select(x => x.RazonSocial)
                              .FirstOrDefault()
                      let ps = ctx.Puesto.FirstOrDefault(x => x.IdPuesto == c.IdPuesto)
                      where e.IdSucursal == Id && e.Status == activos
                      select new DatosEmpleado
                      {
                          IdEmpleado = e.IdEmpleado,
                          Nombres = e.Nombres,
                          Paterno = e.APaterno,
                          Materno = e.AMaterno,
                          FechaAlta = c.FechaAlta,
                          EmpresaFiscal = ef,
                          EmpresaComplemento = ec,
                          EmpresaAsimilado = ea,
                          EmpresaSindicato = es,
                          Puesto = ps.Descripcion
                      }).ToList();





            //var lista = (from emp in ctx.Empleado
            //             join emc in ctx.Empleado_Contrato on emp.IdEmpleado equals emc.IdEmpleado
            //             join ef in ctx.Empresa on emc.IdEmpresaFiscal equals ef.IdEmpresa
            //             join ec in ctx.Empresa on emc.IdEmpresaComplemento equals ec.IdEmpresa
            //             join ea in ctx.Empresa on emc.IdEmpresaAsimilado equals ea.IdEmpresa
            //             join es in ctx.Empresa on emc.IdEmpresaSindicato equals es.IdEmpresa
            //             join p in ctx.Puesto on emc.IdPuesto equals p.IdPuesto
            //             where emp.IdSucursal == Id && emc.Status == true
            //             select new DatosEmpleado
            //             {
            //                 IdEmpleado = emp.IdEmpleado,
            //                 Nombres = emp.Nombres,
            //                 Paterno = emp.APaterno,
            //                 Materno = emp.AMaterno,
            //                 FechaAlta = emc.FechaAlta,
            //                 EmpresaFiscal = ef.RazonSocial,
            //                 EmpresaComplemento = ec.RazonSocial,
            //                 EmpresaAsimilado = ea.RazonSocial,
            //                 EmpresaSindicato = es.RazonSocial,
            //                 Puesto = p.Descripcion,
            //             }).Distinct().ToList();

            return ls;
        }

        private string GetRazonSocialById(List<Empresa> listaDeEmpresas, int? id)
        {
            var result = "";
            var item = listaDeEmpresas.FirstOrDefault(x => x.IdEmpresa == id);
            if (item != null) result = item.RazonSocial;
            return result;
        }

        /// <summary>
        /// registra los empleados de una lista dada 
        /// </summary>
        /// <param name="nuevoEmpleado"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<Empleado> CrearEmpleados(List<Empleado> listaEmpleados)
        {
             using (var context = new RHEntities())
                {
                    context.Empleado.AddRange(listaEmpleados);
                    var state = context.SaveChanges();
                    return listaEmpleados;
                }
        }

        /// <summary>
        /// registra los empleados de una lista dada 
        /// </summary>
        /// <param name="nuevoEmpleado"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public int CrearContratosDatosB(List<Empleado_Contrato> lC, List<DatosBancarios> lDB)
        {
            try
            {
                using (var context = new RHEntities())
                {
                    context.Empleado_Contrato.AddRange(lC);
                    context.DatosBancarios.AddRange(lDB);
                    var state = context.SaveChanges();
                    
                    return state;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        /// <summary>
        /// version 2.0
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public int CrearEmpleado(Empleado nuevoEmpleado, int idUsuario)
        {
            try
            {
                //forzamos que el campo RFC validado sea 2(Pendiente de validar)
                nuevoEmpleado.RFCValidadoSAT = 2;

                //Crear el nuevo empleado
                nuevoEmpleado = ctx.Empleado.Add(nuevoEmpleado);
                var state = ctx.SaveChanges();
                if (state > 0)
                {
                    KardexEmpleado kardex = new KardexEmpleado();
                    kardex.Alta(nuevoEmpleado.IdEmpleado, idUsuario);
                    return nuevoEmpleado.IdEmpleado;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        ///version 1.0
        //public int CrearEmpleado(Empleado nuevoEmpleado, int idUsuario)
        //{
        //    try
        //    {
        //        //validar si el RFC y CURP ya esten registrados en la base de datos
        //        var itemVal1 = ctx.Empleado.FirstOrDefault(e => Equals(e.CURP, nuevoEmpleado.CURP) && e.IdSucursal == nuevoEmpleado.IdSucursal);
        //        if (itemVal1 != null) return 0;
        //        var itemVal2 = ctx.Empleado.FirstOrDefault(e => e.RFC == nuevoEmpleado.RFC && e.IdSucursal == nuevoEmpleado.IdSucursal);
        //        if (itemVal2 != null) return 0;
        //        if (nuevoEmpleado.RFC.Length > 13) return 0;

        //        //forzamos que el campo RFC validado sea 2 (Pendiente de validar)
        //        nuevoEmpleado.RFCValidadoSAT = 2;
        //        //Crear el nuevo empleado
        //        nuevoEmpleado = ctx.Empleado.Add(nuevoEmpleado);
        //        var state = ctx.SaveChanges();
        //        if (state > 0)
        //        {
        //            KardexEmpleado kardex = new KardexEmpleado();
        //            kardex.Alta(nuevoEmpleado.IdEmpleado, idUsuario);
        //            return nuevoEmpleado.IdEmpleado;
        //        }
        //        else
        //            return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}


        /// <summary>
        /// version anterior para actualizacion de informacion
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>

        public bool EX_UpdateEmpleado(Empleado model, int idSucursal)
        {

            var old = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado);
            


            if (model.RFC != old.RFC)
            {
                model.RFCValidadoSAT = 2;
            }
            else
            {
                model.RFCValidadoSAT = old.RFCValidadoSAT;
            }


            model.IdSucursal = idSucursal;
            ctx.Entry(old).CurrentValues.SetValues(model);

            var result = ctx.SaveChanges();
            return result > 0 ? true : false;
        }

        /// <summary>
        /// Version vigente para actualizacion de informacion del empleado
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public bool UpdateEmpleado(Empleado model, int idUsuario)
        {
            using (var context = new RHEntities()) {

                var old = context.Empleado.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado);

                if (model.RFC != old.RFC)
                {
                    model.RFCValidadoSAT = 2;
                }
                else
                {
                    model.RFCValidadoSAT = old.RFCValidadoSAT;
                }

                model.IdSucursal = old.IdSucursal;
                model.FechaReg = old.FechaReg;
                model.IdUsuarioReg = old.IdUsuarioReg;
                model.FechaMod = DateTime.Today;
                model.IdUsuarioMod = idUsuario;
                
                context.Entry(old).CurrentValues.SetValues(model);

                var result = context.SaveChanges();
                return result > 0 ? true : false;
            }
        }

        public bool CrearContrato(Empleado_Contrato contrato)
        {
            contrato.Status = true;
            //si el id de forma de pago es 3:transferencia-4:tc-5:monederoElec-6:DinElec-7:TarjDig se considerará pago electrónico
            if (contrato.FormaPago > 2 && contrato.FormaPago < 8)
            {
                contrato.PagoElectronico = true;
            }
            //de otra forma no será pago electrónico 
            else
            {
                contrato.PagoElectronico = false;
            }
            ctx.Empleado_Contrato.Add(contrato);
            var state = ctx.SaveChanges();
            return state > 0 ? true : false;
        }
        
        public bool UpdateContrato(Empleado_Contrato model, int idUsuario)
        {
            var noti = new Notificaciones();
            model.Status = true;
            KardexEmpleado kardex = new KardexEmpleado();
            var result = false;
            var contrato = ctx.Empleado_Contrato.FirstOrDefault(x => x.IdContrato == model.IdContrato);

            if (contrato != null)
            {

                //PUESTO
                if (contrato.IdPuesto != model.IdPuesto && contrato.IdPuesto > 0)
                {
                    if (kardex.Puesto(model.IdEmpleado, contrato.IdPuesto, model.IdPuesto, idUsuario))
                    {
                        noti.CambioPuesto(contrato.IdEmpleado, contrato.IdPuesto, model.IdPuesto);
                    }
                }
                //FECHA IMSS
                if (contrato.FechaIMSS == null && model.FechaIMSS != null)
                    noti.IMSS(model);

                //SD
                if (contrato.SD != model.SD && contrato.SD > 0)
                {
                    kardex.Salarios(model.IdEmpleado, contrato.SD, model.SD, (int)TipoKardex.SD, idUsuario);
                    noti.SalarioDiario(model.IdEmpleado, model.SD, contrato.SD);
                }
                //SDI
                if (contrato.SDI != model.SDI && contrato.SDI > 0)
                {
                    kardex.Salarios(model.IdEmpleado, contrato.SDI, model.SDI, (int)TipoKardex.SDI, idUsuario);
                    noti.SalarioDiarioIntegrado(model.IdEmpleado, model.SDI, contrato.SDI);
                }
                //SD REAL
                if (contrato.SalarioReal != model.SalarioReal && contrato.SalarioReal > 0)
                {
                    kardex.Salarios(model.IdEmpleado, contrato.SalarioReal, model.SalarioReal, (int)TipoKardex.SR, idUsuario);
                    noti.SalarioReal(model.IdEmpleado, model.SalarioReal, contrato.SalarioReal);
                }
                //FORMA DE PAGO
                if (contrato.FormaPago != model.FormaPago)
                {
                    //si el id de forma de pago es 3:transferencia-4:tc-5:monederoElec-6:DinElec-7:TarjDig se considerará pago electrónico
                    if (model.FormaPago > 2 && model.FormaPago < 8)
                    {
                        model.PagoElectronico = true;
                    }
                    //de otra forma no será pago electrónico
                    else
                    {
                        model.PagoElectronico = false;
                    }
                }

                ctx.Entry(contrato).CurrentValues.SetValues(model);
                var saved = ctx.SaveChanges();
                if (saved > 0) result = true;

                //Agregar el concepto Pension Alimenticia al empleado
                if (contrato.PensionAlimenticiaPorcentaje > 0)
                {
                    var item = ctx.NOM_Empleado_Conceptos.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado && x.IdConcepto == 87);
                    if (item == null)
                    {
                        NOM_Empleado_Conceptos empleadoConceptos = new NOM_Empleado_Conceptos()
                        {
                            IdEmpleado = model.IdEmpleado,
                            IdConcepto = 87,
                            IdSucursal = model.IdSucursal,
                            Fiscal = true,
                            Complemento = true
                        };
                        ctx.NOM_Empleado_Conceptos.Add(empleadoConceptos);
                        ctx.SaveChanges();
                    }
                }
                else
                {
                    var item = ctx.NOM_Empleado_Conceptos.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado && x.IdConcepto == 87);

                    if (item != null)
                    {
                        ctx.NOM_Empleado_Conceptos.Attach(item);
                        ctx.NOM_Empleado_Conceptos.Remove(item);
                        ctx.SaveChanges();
                    }
                }

            }
            return result;
        }

        public bool UpdateContrato2(Empleado_Contrato model, int idUsuario)
        {
            using (var context = new RHEntities())
            {
                var noti = new Notificaciones();
                KardexEmpleado kardex = new KardexEmpleado();
                var result = false;
                //Buscamos el contrato actual
                var contrato = context.Empleado_Contrato.FirstOrDefault(x => x.IdContrato == model.IdContrato);

                if (contrato != null)
                {
                    //PUESTO
                    if (contrato.IdPuesto != model.IdPuesto && contrato.IdPuesto > 0)
                    {
                        if (kardex.Puesto(model.IdEmpleado, contrato.IdPuesto, model.IdPuesto, idUsuario))
                        {
                            noti.CambioPuesto(contrato.IdEmpleado, contrato.IdPuesto, model.IdPuesto);
                        }
                    }
                    //FECHA IMSS
                    if (contrato.FechaIMSS != model.FechaIMSS && model.FechaIMSS != null)
                        noti.IMSS(model);

                    //SD
                    if (contrato.SD != model.SD && contrato.SD > 0)
                    {
                        kardex.Salarios(model.IdEmpleado, contrato.SD, model.SD, (int)TipoKardex.SD, idUsuario);
                        noti.SalarioDiario(model.IdEmpleado, model.SD, contrato.SD);
                    }
                    //SDI
                    if (contrato.SDI != model.SDI && contrato.SDI > 0)
                    {
                        kardex.Salarios(model.IdEmpleado, contrato.SDI, model.SDI, (int)TipoKardex.SDI, idUsuario);
                        noti.SalarioDiarioIntegrado(model.IdEmpleado, model.SDI, contrato.SDI);
                    }
                    //SD REAL
                    if (contrato.SalarioReal != model.SalarioReal && contrato.SalarioReal > 0)
                    {
                        kardex.Salarios(model.IdEmpleado, contrato.SalarioReal, model.SalarioReal, (int)TipoKardex.SR, idUsuario);
                        noti.SalarioReal(model.IdEmpleado, model.SalarioReal, contrato.SalarioReal);
                    }
                    //FORMA DE PAGO
                    if (contrato.FormaPago != model.FormaPago)
                    {
                        //si el id de forma de pago es 3:transferencia-4:tc-5:monederoElec-6:DinElec-7:TarjDig se considerará pago electrónico
                        if (model.FormaPago > 2 && model.FormaPago < 8)
                        {
                            model.PagoElectronico = true;
                        }
                        //de otra forma no será pago electrónico
                        else
                        {
                            model.PagoElectronico = false;
                        }
                    }

                    //ctx.Entry(contrato).CurrentValues.SetValues(model);
                    //var saved = ctx.SaveChanges();
                    //if (saved > 0) result = true;

                    //contrato.IdEmpleado = model.IdEmpleado;
                    contrato.FechaAlta = model.FechaAlta;
                    contrato.FechaBaja = model.FechaBaja;
                    contrato.FechaReal = model.FechaReal;
                    contrato.FechaIMSS = model.FechaIMSS;
                    contrato.BajaIMSS = model.BajaIMSS;
                    contrato.MotivoBaja = model.MotivoBaja;
                    contrato.FechaCalculoAguinaldo = model.FechaCalculoAguinaldo;
                    contrato.UMF = model.UMF;
                    contrato.DiasContrato = model.DiasContrato;
                    contrato.Vigencia = model.Vigencia;
                    if (model.IdPuesto > 0)
                    {
                        contrato.IdPuesto = model.IdPuesto;
                    }
                    contrato.Turno = model.Turno;
                    contrato.DiaDescanso = model.DiaDescanso;
                    contrato.SD = model.SD;
                    contrato.SDI = model.SDI;
                    contrato.SBC = model.SBC;
                    contrato.SalarioReal = model.SalarioReal;
                    contrato.TipoContrato = model.TipoContrato;
                    contrato.IdPeriodicidadPago = model.IdPeriodicidadPago;
                    contrato.FormaPago = model.FormaPago;
                    contrato.TipoSalario = model.TipoSalario;
                    contrato.IdEmpresaFiscal = model.IdEmpresaFiscal;
                    contrato.IdEmpresaComplemento = model.IdEmpresaComplemento;
                    contrato.IdEmpresaAsimilado = model.IdEmpresaAsimilado;
                    contrato.IdEmpresaSindicato = model.IdEmpresaSindicato;
                    //contrato.Status = model.Status;
                    contrato.IdTipoJornada = model.IdTipoJornada;
                    contrato.IdTipoRegimen = model.IdTipoRegimen;
                    contrato.IdSucursal = model.IdSucursal;
                    contrato.PensionAlimenticiaPorcentaje = model.PensionAlimenticiaPorcentaje;
                    contrato.PensionAlimenticiaSueldo = model.PensionAlimenticiaSueldo;
                    contrato.EntidadDeServicio = model.EntidadDeServicio;
                    contrato.Sindicalizado = model.Sindicalizado;
                    contrato.PagoElectronico = model.PagoElectronico;

                    contrato.FechaMod = DateTime.Today;
                    contrato.IdUsuarioMod = idUsuario;

                    var saved = context.SaveChanges();
                    if (saved > 0) result = true;

                    //Agregar el concepto Pension Alimenticia al empleado
                    if (contrato.PensionAlimenticiaPorcentaje > 0)
                    {
                        var item = context.NOM_Empleado_Conceptos.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado && x.IdConcepto == 87);
                        if (item == null)
                        {
                            NOM_Empleado_Conceptos empleadoConceptos = new NOM_Empleado_Conceptos()
                            {
                                IdEmpleado = model.IdEmpleado,
                                IdConcepto = 87,
                                IdSucursal = model.IdSucursal,
                                Fiscal = true,
                                Complemento = true
                            };
                            ctx.NOM_Empleado_Conceptos.Add(empleadoConceptos);
                            ctx.SaveChanges();
                        }
                    }
                    else
                    {
                        var item = context.NOM_Empleado_Conceptos.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado && x.IdConcepto == 87);

                        if (item != null)
                        {
                            ctx.NOM_Empleado_Conceptos.Attach(item);
                            ctx.NOM_Empleado_Conceptos.Remove(item);
                            ctx.SaveChanges();
                        }
                    }

                }
                return result;
            }

        }

        public List<DatosBancariosViewModel> GetDatosBancarios(int id)
        {
            using (var contexto = new RHEntities())
            {
                return (from db in contexto.DatosBancarios
                        join bank in contexto.C_Banco_SAT on db.IdBanco equals bank.IdBanco
                        where db.IdEmpleado == id
                        select new DatosBancariosViewModel
                        {
                            Id = db.Id,
                            IdContrato = db.IdEmpleado,
                            Banco = bank.Descripcion,
                            CuentaBancaria = db.CuentaBancaria,
                            NoSigaF = db.NoSigaF,
                            NoSigaC = db.NoSigaC,
                            NumTarjeta = db.NumeroTarjeta,
                            Clabe = db.Clabe,
                            Status = db.Status
                        }
                        ).ToList();
            }
        }

        public DatosBancarios GetDatosBancariosById(int id)
        {
            return ctx.DatosBancarios.Where(x => x.Id == id).FirstOrDefault();
        }
        public DatosBancariosViewModel GetDatosBancariosByIdEmpleado(int id)
        {

            return (from db in ctx.DatosBancarios
                    join bank in ctx.C_Banco_SAT on db.IdBanco equals bank.IdBanco
                    where db.IdEmpleado == id
                    select new DatosBancariosViewModel
                    {
                        CuentaBancaria = db.CuentaBancaria,
                        Descripcion = bank.Descripcion
                    }
                    ).FirstOrDefault();


        }
        public bool UpdateDatosBancarios(DatosBancarios updated)
        {
            var original = ctx.DatosBancarios.Where(x => x.Id == updated.Id).FirstOrDefault();
            if (original != null)
            {
                ctx.Entry(original).CurrentValues.SetValues(updated);
                var status = ctx.SaveChanges();
                return status > 0 ? true : false;
            }
            return false;
        }

        public int NewDatosBancarios(DatosBancarios model, bool validation = true)
        {
            model.Status = true;
            if (validation)
                if (model.ParentezcoBeneficiario.Equals('0'))
                    model.ParentezcoBeneficiario = null;

            ctx.DatosBancarios.Add(model);
            var status = ctx.SaveChanges();
            return status > 0 ? model.IdEmpleado : 0;
        }

        public bool BajaEmpleado(int idEmpleado, DateTime fecha, int idUsuario, string MotivoBaja, string ComentarioBaja)
        {
            var noti = new Notificaciones();
            Empleado empleado = ctx.Empleado.FirstOrDefault(s => s.IdEmpleado == idEmpleado);
            if (empleado == null) return false;
            empleado.Status = false;
            //primero buscamos el contrato que deseamos dar de baja
            Empleado_Contrato contrato = ctx.Empleado_Contrato.Where(s => s.IdEmpleado == idEmpleado).OrderByDescending(s => s.IdContrato).FirstOrDefault();
            //si el contrato fue encontrado entonces registramos las fechas de baja
            if (contrato != null)
            {
                //validar si tiene asignada una empresa fiscal
                if (contrato.IdEmpresaFiscal != null)
                {
                    //guardamos las dos fechas de baja (imss y sistema)
                    contrato.BajaIMSS = fecha;
                }

                //si solo tiene complemento guardamos la fecha del sistema
                contrato.Status = false;
                contrato.FechaBaja = fecha;
                contrato.MotivoBaja = MotivoBaja;
                contrato.ComentarioBaja = ComentarioBaja;
            }

            var status = ctx.SaveChanges();


            if (status > 0)
            {
                KardexEmpleado kardex = new KardexEmpleado();
                kardex.Baja(empleado.IdEmpleado, idUsuario);
                noti.Baja(empleado.IdEmpleado, contrato.FechaBaja, MotivoBaja);
                return true;
            }
            else
                return false;
        }

        public bool UpdateSS(System.Collections.Specialized.NameValueCollection form)
        {
            var noti = new Notificaciones();
            int id = Convert.ToInt32(form["IdEmpleado"]);
            var empleado = GetEmpleadoById(id);
            var contrato = GetUltimoContrato(id);

            if (string.IsNullOrEmpty(form["nss"]))
                empleado.NSS = null;
            else
                empleado.NSS = form["nss"];

            if (string.IsNullOrEmpty(form["fecha-imss"]))
                contrato.FechaIMSS = null;
            else
                contrato.FechaIMSS = Convert.ToDateTime(form["fecha-imss"]);

            if (string.IsNullOrEmpty(form["fonacot"]))
                empleado.FONACOT = null;
            else
                empleado.FONACOT = form["fonacot"];

            if (string.IsNullOrEmpty(form["BajaIMSS"]))
                contrato.BajaIMSS = null;
            else
            {
                contrato.BajaIMSS = Convert.ToDateTime(form["BajaIMSS"]);
                noti.BajaIMSS(id, contrato.BajaIMSS);
            }

            var status = ctx.SaveChanges();

            return status > 0 ? true : false;
        }

        public Empleado ObtenerEmpleadoPorId(int idEmpleado)
        {

            Empleado empleado = ctx.Empleado.FirstOrDefault(s => s.IdEmpleado == idEmpleado);
            return empleado;
        }

        public Empleado GetEmpleadoById(int idEmpleado)
        {
            return ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == idEmpleado);
        }

        public DatosEmpleado GetDatosEmpleado(int IdEmpleado)
        {
            return (from e in ctx.Empleado
                    let c = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault()
                    let contratos = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado).Count()
                    let ef = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaFiscal).Select(x => x.RazonSocial).FirstOrDefault()
                    let ec = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault()
                    let ea = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaAsimilado).Select(x => x.RazonSocial).FirstOrDefault()
                    let es = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaSindicato).Select(x => x.RazonSocial).FirstOrDefault()
                    where e.IdEmpleado == IdEmpleado
                    select new DatosEmpleado
                    {
                        IdEmpleado = e.IdEmpleado,
                        Nombres = e.Nombres,
                        Paterno = e.APaterno,
                        Materno = e.AMaterno,
                        FechaAlta = c.FechaAlta,
                        FechaReal = c.FechaReal,
                        FechaBaja = c.FechaBaja,
                        FechaIMSS = c.FechaIMSS,
                        BajaIMSS = c.BajaIMSS,
                        Status = e.Status,
                        NumContratos = contratos,
                        EmpresaFiscal = ef,
                        EmpresaComplemento = ec,
                        EmpresaAsimilado = ea,
                        EmpresaSindicato = es
                    }
                    ).FirstOrDefault();
        }

        public Empleado_Contrato ObtenerContratoEmpleadoPorId(int idEmpleado)
        {
            return ctx.Empleado_Contrato.Where(s => s.IdEmpleado == idEmpleado && s.Status == true).OrderByDescending(s => s.IdContrato).FirstOrDefault();
        }

        public Empleado_Contrato GetContratoByIdEmpleadoContrato(int IdEmpleadoContrato)
        {
            return ctx.Empleado_Contrato.FirstOrDefault(x => x.IdContrato == IdEmpleadoContrato);
        }

        public Empleado_Contrato GetUltimoContrato(int IdEmpleado)
        {
            using (var context = new RHEntities())
            {
                return context.Empleado_Contrato.Where(s => s.IdEmpleado == IdEmpleado).OrderByDescending(s => s.IdContrato).FirstOrDefault();
            }
        }


        public DatosBancarios GetDatosBancoEmpleado(int idEmpleado)
        {
            return ctx.DatosBancarios.Where(x => x.IdEmpleado == idEmpleado).FirstOrDefault();
        }
        public DatosEmpleado GetEmpleadoPrestamoDetalle(int id)
        {
            return (from emp in ctx.Empleado
                    join c in ctx.Empleado_Contrato on emp.IdEmpleado equals c.IdEmpleado
                    where emp.IdEmpleado == id && c.Status == true
                    select new DatosEmpleado
                    {
                        IdEmpleado = emp.IdEmpleado,
                        IdContrato = c.IdContrato,
                        Nombres = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        FechaAlta = c.FechaAlta,
                        TipoNomina = c.IdPeriodicidadPago
                    }).FirstOrDefault();
        }

        public bool Recontratacion(Empleado_Contrato model, int idUsuario)
        {
            var noti = new Notificaciones();
            var empleado = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado);
            var contratoAnterior = ctx.Empleado_Contrato.FirstOrDefault(x => x.IdEmpleado == model.IdEmpleado);
            if (empleado != null)
            {
                empleado.Status = true;
                empleado.FechaMod = DateTime.Today;
                empleado.IdUsuarioMod = idUsuario;

                model.Status = true;
                //esta solucion es temporal, permite obtener los datos faltantes del contrato anterior
                model.SBC = model.SDI;
                model.EntidadDeServicio = contratoAnterior.EntidadDeServicio;
                model.IdPeriodicidadPago = contratoAnterior.IdPeriodicidadPago;
                model.IdTipoJornada = contratoAnterior.IdTipoJornada;
                model.IdTipoRegimen = contratoAnterior.IdTipoRegimen;
                model.Sindicalizado = contratoAnterior.Sindicalizado;
                model.PagoElectronico = contratoAnterior.PagoElectronico;
                model.IdSucursal = contratoAnterior.IdSucursal;
                model.Turno = contratoAnterior.Turno;
                model.FormaPago = contratoAnterior.FormaPago;
                model.TipoSalario = contratoAnterior.TipoSalario;
                model.IsReingreso = true;

                model.FechaReg = DateTime.Today;
                model.IdUsuarioReg = idUsuario;
                
                ctx.Empleado_Contrato.Add(model);
                int status = ctx.SaveChanges();
                if (status > 0)
                {
                    KardexEmpleado kardex = new KardexEmpleado();
                    kardex.Recontratacion(model.IdEmpleado, idUsuario);
                    noti.Recontratacion(model.IdEmpleado, model.FechaAlta);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool Cambiosalario(int idUsuario, int idContrato, DateTime Fechainicio, decimal SalarioReal = 0, decimal SD = 0, decimal SDI = 0)
        {
            var noti = new Notificaciones();
            KardexEmpleado kardex = new KardexEmpleado();
            using (var context = new RHEntities())
            {
                //Buscamos el contrato actual
                var contrato = context.Empleado_Contrato.FirstOrDefault(x => x.IdContrato == idContrato);

                if (SalarioReal > 0)
                {
                    kardex.Salarios(contrato.IdEmpleado, contrato.SalarioReal, SalarioReal, (int)TipoKardex.SR, idUsuario);
                    noti.SalarioReal(contrato.IdEmpleado, SalarioReal, contrato.SalarioReal);
                    contrato.SalarioReal = SalarioReal;
                }
                if (SD > 0)
                {
                    kardex.Salarios(contrato.IdEmpleado, contrato.SD, SD, (int)TipoKardex.SD, idUsuario, Fechainicio.ToString());
                    noti.SalarioDiario(contrato.IdEmpleado, SD, contrato.SD);
                    contrato.SD = SD;
                }
                if (SDI > 0)
                {
                    kardex.Salarios(contrato.IdEmpleado, contrato.SDI, SDI, (int)TipoKardex.SDI, idUsuario, Fechainicio.ToString());
                    noti.SalarioDiarioIntegrado(contrato.IdEmpleado, SDI, contrato.SDI);
                    contrato.SDI = SDI;
                    contrato.SBC = SDI;
                }
                int status = context.SaveChanges();
                if (status > 0)
                {
                    return true;
                }
                else
                    return false;
            }

        }

        //Empleado Nomina 
        public EmpleadoDatosNominaViewModel DatosEmpleadoViewModel(int id)
        {
            //var contra =
            //   ctx.Empleado_Contrato.Where(x => x.IdEmpleado == id)
            //       .OrderByDescending(d => d.IdContrato)
            //       .FirstOrDefault();
            using (var context = new RHEntities())
            {
                var listaEmpresas = context.Empresa.ToList();

                var itemContrato = context.Empleado_Contrato.Where(x => x.IdEmpleado == id && x.Status == true)
                        .OrderByDescending(x => x.IdContrato)
                        .FirstOrDefault();
                var itemEmpleado = context.Empleado.FirstOrDefault(x => x.IdEmpleado == id);

                var itemPuesto = context.Puesto.FirstOrDefault(x => x.IdPuesto == itemContrato.IdPuesto);

                var itemDepartamento =
                    context.Departamento.FirstOrDefault(x => x.IdDepartamento == itemPuesto.IdDepartamento);

                var itemInfonavit =
                    context.Empleado_Infonavit.FirstOrDefault(x => x.IdEmpleadoContrato == itemContrato.IdContrato && x.Status == true);

                var itemFonacot =
                    context.Empleado_Fonacot.FirstOrDefault(x => x.IdEmpleadoContrato == itemContrato.IdContrato && x.Status == true);



                EmpleadoDatosNominaViewModel itemDatosEmpleado = new EmpleadoDatosNominaViewModel();

                if (itemInfonavit != null)
                {
                    Infonavit cInfonavit = new Infonavit();
                    var datosInfonavit = cInfonavit.calcularInfonavit(itemInfonavit);

                    itemDatosEmpleado.FactorDescuento = datosInfonavit.FactorDescuento;
                    itemDatosEmpleado.DescuentoBimestral = Math.Round(datosInfonavit.DescuentoBimestral, 2);
                    itemDatosEmpleado.TipoCredito = ((TipoCreditoInfonavit)datosInfonavit.TipoCredito).ToString().Replace('_', ' ');
                }

                if (itemFonacot != null) itemDatosEmpleado.RetencionFonacot = itemFonacot.Retencion;


                itemDatosEmpleado.IdEmpleado = id;
                itemDatosEmpleado.IdContrato = itemContrato.IdContrato;
                itemDatosEmpleado.Nombres = itemEmpleado.Nombres;
                itemDatosEmpleado.Paterno = itemEmpleado.APaterno;
                itemDatosEmpleado.Materno = itemEmpleado.AMaterno;
                itemDatosEmpleado.Sexo = itemEmpleado.Sexo;
                itemDatosEmpleado.RFC = itemEmpleado.RFC;
                itemDatosEmpleado.CURP = itemEmpleado.CURP;
                itemDatosEmpleado.NSS = itemEmpleado.NSS;
                itemDatosEmpleado.UMF = itemContrato.UMF;
                itemDatosEmpleado.FechaAlta = itemContrato.FechaAlta.ToString("dd-MM-yyyy");
                itemDatosEmpleado.FechaReal = itemContrato.FechaReal.ToString("dd-MM-yyyy");
                if (itemContrato.FechaIMSS != null)
                    itemDatosEmpleado.FechaIMSS = itemContrato.FechaIMSS.Value.ToString("dd-MM-yyyy");
                if (itemContrato.Vigencia != null)
                    itemDatosEmpleado.FechaVigencia = itemContrato.Vigencia.Value.ToString("dd-MM-yyyy");
                itemDatosEmpleado.TipoContrato = itemContrato.TipoContrato == 1 ? "Permanente" : "Temporal";
                itemDatosEmpleado.DiasContrato = itemContrato.DiasContrato;
                itemDatosEmpleado.DiaDescanso = UtilsEmpleados.seleccionarDia(itemContrato.DiaDescanso);

                itemDatosEmpleado.TipoNomina = UtilsEmpleados.SeleccionaTipoNomina(itemContrato.IdPeriodicidadPago);
                itemDatosEmpleado.TipoPago = UtilsEmpleados.TipoDePagoInfo(itemContrato.FormaPago);
                itemDatosEmpleado.TipoSalario = UtilsEmpleados.TipoSalario(itemContrato.TipoSalario);
                itemDatosEmpleado.SD = itemContrato.SD.ToString(CultureInfo.InvariantCulture);
                itemDatosEmpleado.SDI = itemContrato.SDI.ToString();
                itemDatosEmpleado.SalarioReal = itemContrato.SalarioReal.ToString();
                itemDatosEmpleado.EmpresaFiscal = itemContrato.IdEmpresaFiscal.ToStringOrNull();
                itemDatosEmpleado.EmpresaComplemento = itemContrato.IdEmpresaComplemento.ToStringOrNull();
                itemDatosEmpleado.EmpresaSindicato = itemContrato.IdEmpresaSindicato.ToStringOrNull();
                itemDatosEmpleado.EmpresaAsimilado = itemContrato.IdEmpresaAsimilado.ToStringOrNull();
                if (itemPuesto != null) itemDatosEmpleado.Puesto = itemPuesto.Descripcion;
                if (itemDepartamento != null) itemDatosEmpleado.Departamento = itemDepartamento.Descripcion;


                string empresaFiscal = "";
                string empresaComplemento = "";
                string empresaAsimilado = "";
                string empresaSindicato = "";


                if (!string.IsNullOrEmpty(itemDatosEmpleado.EmpresaFiscal))
                {
                    var item = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == Convert.ToInt16(itemDatosEmpleado.EmpresaFiscal));
                    if (item != null) empresaFiscal = item.RazonSocial;
                }

                if (!string.IsNullOrEmpty(itemDatosEmpleado.EmpresaComplemento))
                {
                    var item = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == Convert.ToInt16(itemDatosEmpleado.EmpresaComplemento));
                    if (item != null) empresaComplemento = item.RazonSocial;
                }

                if (!string.IsNullOrEmpty(itemDatosEmpleado.EmpresaAsimilado))
                {
                    var item = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == Convert.ToInt16(itemDatosEmpleado.EmpresaAsimilado));
                    if (item != null) empresaAsimilado = item.RazonSocial;
                }

                if (!string.IsNullOrEmpty(itemDatosEmpleado.EmpresaSindicato))
                {
                    var item = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == Convert.ToInt16(itemDatosEmpleado.EmpresaSindicato));
                    if (item != null) empresaSindicato = item.RazonSocial;
                }


                itemDatosEmpleado.EmpresaFiscal = empresaFiscal;
                itemDatosEmpleado.EmpresaComplemento = empresaComplemento;
                itemDatosEmpleado.EmpresaAsimilado = empresaAsimilado;
                itemDatosEmpleado.EmpresaSindicato = empresaSindicato;

                return itemDatosEmpleado;
            }

        }
        //utilizado para el modulo de asignacio de concepto(Nominas)
        public List<DatosEmpleado> empleadosPorSucursalNOM(int idSucursal)
        {

            //      var list = ctx.Empleado.Where(x => x.IdSucursal == idSucursal).ToList();
            var datos = (from e in ctx.Empleado
                         join ce in ctx.Empleado_Contrato
                         on e.IdEmpleado equals ce.IdEmpleado
                         where e.IdSucursal == idSucursal
                         select new DatosEmpleado
                         {
                             IdEmpleado = e.IdEmpleado,
                             Nombres = e.Nombres,
                             Paterno = e.APaterno,
                             Materno = e.AMaterno,
                             Status = e.Status,
                             idEmpresaFiscal = ce.IdEmpresaFiscal,
                             idEmpresaAsimilado = ce.IdEmpresaAsimilado,
                             idEmpresaComplemento = ce.IdEmpresaComplemento,
                             idEmpresaSindicato = ce.IdEmpresaSindicato
                         }).ToList();

            return datos;
        }

        /// <summary>
        /// Metodo para generar la lista de RFC para validar en el sitio del SAT.
        /// Se Genera la lista por sucursal, para empleados activos e inactivos.
        /// Retorna la ruta del archivo generado.
        /// </summary>
        /// <param name="pathTxt"></param>
        /// <param name="idSucursal"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public string GenerarTxtRfcForSat(string pathTxt, int idSucursal, int idUsuario, ref bool estaVacio)
        {
            //Validar variables
            if (string.IsNullOrEmpty(pathTxt) || idSucursal <= 0 || idUsuario <= 0) return null;

            pathTxt = pathTxt + "\\" + idSucursal + "\\" + idUsuario + "\\";
            //
            var sucursal = ctx.Sucursal.FirstOrDefault(x => x.IdSucursal == idSucursal);
            //if (sucursal == null) return null;

            var cliente = ctx.Cliente.FirstOrDefault(x => x.IdCliente == sucursal.IdCliente);
            //if (cliente == null) return null;

            //validar que el directorio para el txt este creado, sino se crea
            if (!ValidarDirectorio(pathTxt)) return null;

            //agregamos el nombre al archivo txt
            var archivoTxt = pathTxt + "Lista RFC " + cliente.Nombre + "_" + sucursal.Ciudad.Trim() + ".txt";
            //si existe un txt precio se elimina
            if (File.Exists(archivoTxt))
            {
                File.Delete(archivoTxt);
            }
            // obtenemos la lista de rfc pendientes de validación
            var lista = ctx.Empleado.Where(x => x.IdSucursal == idSucursal && x.RFCValidadoSAT == 2).Select(x => x.RFC).ToList();

            //validar que la lista contenga datos
            if (lista.Count <= 0)
            {
                estaVacio = true;
                var archivoVacioTxt = pathTxt + "Sin RFC para validar.txt";
                string[] lineas = new string[2];
                lineas[0] = "No se encontraron RFC's para validar en esta Sucursal";
                lineas[1] = "Al día " + DateTime.Today.ToString("dd/MM/yyyy");
                File.WriteAllLines(archivoVacioTxt, lineas);
                return archivoVacioTxt;
            }
            else
            {

                estaVacio = false;
                var cont = lista.Count;
                var index = 1;

                string[] lines = new string[cont + 1];
                lines[0] = "";

                foreach (var item in lista)
                {
                    lines[index] = index + "|" + item.Trim();
                    index++;
                }

                File.WriteAllLines(archivoTxt, lines);

                return archivoTxt;
            }
        }

        /// <summary>
        /// Actualiza el campo de validacion del sat, en la tabla Empleado.
        /// Es necesario indicar la sucusal a la que se actualizarán los datos.
        /// </summary>
        /// <param name="fileTxt"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<int> ActualizarRfcValidadosFromSat(StreamReader fileTxt, int idSucursal)
        {
            //Validacion
            if (fileTxt == StreamReader.Null || idSucursal <= 0) return null;

            //   StreamReader sr = new StreamReader(@"C:\SUNFILES\4\5\RESPUESTA_SAT_RFC (1).txt");
            var cont = 0;
            string linea;
            var result = true;
            List<int> ListadeInvalidos = new List<int>();

            while ((linea = fileTxt.ReadLine()) != null)
            {
                //ignorar la primera linea
                if (cont <= 0) { cont++; continue; }
                try
                {

                    var array = linea.Split('|');
                    if (array.Length == 3)
                    {
                        var rfcSat = array[1];

                        //actualizar rfc vs sat
                        var item = ctx.Empleado.FirstOrDefault(x => x.RFC == rfcSat && x.IdSucursal == idSucursal);

                        if (item != null)
                        {
                            switch (array[2])
                            {
                                case "V":
                                    item.RFCValidadoSAT = 1;
                                    break;
                                case "I":
                                    item.RFCValidadoSAT = 0;
                                    ListadeInvalidos.Add(item.IdEmpleado);
                                    break;
                                case "EI":
                                    item.RFCValidadoSAT = 0;
                                    break;
                            }
                            ctx.SaveChanges();
                        }
                    }
                    else
                    {
                        var item = ctx.Empleado.FirstOrDefault(x => x.IdSucursal == idSucursal);
                        if (item != null)
                        {
                            item.RFCValidadoSAT = 2;
                            ListadeInvalidos.Add(item.IdEmpleado);
                            ctx.SaveChanges();
                        }
                    }
                    cont++;

                }
                catch (Exception)
                {
                    cont++;
                    result = false;
                }
            }
            return ListadeInvalidos;
        }


        private static bool ValidarDirectorio(string pathtxt)
        {
            var result = false;
            try
            {
                var dirName = new DirectoryInfo(pathtxt).FullName;

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                    result = true;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public List<int> idEmpleados(int idSucursal)
        {
            var idLista = ctx.Empleado.Where(x => x.IdSucursal == idSucursal).Select(x => x.IdEmpleado).ToList();
            return idLista;
        }

        public List<int> GetIdEmpleados(int idSucursal, int idPeriodoPago)
        {
            using (var context = new RHEntities())
            {
                var idLista = (from e in context.Empleado
                               join ep in context.NOM_Empleado_PeriodoPago on e.IdEmpleado equals ep.IdEmpleado
                               where e.IdSucursal == idSucursal && ep.IdPeriodoPago == idPeriodoPago
                               select e.IdEmpleado).ToList();

                return idLista;
            }

        }

        public List<Empleado> RFCnoValidos(int idSucursal, List<int> idemp)
        {
            List<Empleado> emp = new List<Empleado>();
            if (idemp != null)
            {

                foreach (var i in idemp)
                {
                    var emp2 = ctx.Empleado.FirstOrDefault(x => x.IdSucursal == idSucursal && x.IdEmpleado == i && (x.RFCValidadoSAT == 0 || x.RFCValidadoSAT == 2));
                    emp.Add(emp2);
                }
            }
            //var idNoValidos = ctx.Empleado.Where(x => x.IdSucursal == idSucursal && x.RFCValidadoSAT == 0).ToList();
            return emp;
        }

        public List<DatosEmpleadoValidacion> GetDatosValidacion()
        {

            using (var context = new RHEntities())
            {
                var idLista = (from e in context.Empleado
                               join db in context.DatosBancarios on e.IdEmpleado equals db.IdEmpleado
                               //join ec in context.Empleado_Contrato on e.IdEmpleado equals ec.IdEmpleado
                               //where 
                               select new DatosEmpleadoValidacion
                               {
                                   RFC = e.RFC,
                                   CURP = e.CURP,
                                   NSS = e.NSS,
                                   CuentaBancaria = db.CuentaBancaria,
                                   NumTarjeta = db.NumeroTarjeta,
                                   Clabe = db.Clabe
                               }).ToList();

                return idLista;
            }
        }

    }



    public static class EmpleadosHelpers
    {
        public static string GetNombreEmpleado(int IdEmpleado)
        {
            RHEntities ctx = new RHEntities();
            var empleado = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == IdEmpleado);
            return empleado != null ? empleado.APaterno + " " + empleado.AMaterno + " " + empleado.Nombres : "n/a";
        }
    }




    public class DatosEmpleado
    {
        public int IdEmpleado { get; set; }
        public int IdContrato { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
        public string RFC { get; set; }
        public int RFCValidadoSAT { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaReal { get; set; }
        public DateTime? FechaIMSS { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime? BajaIMSS { get; set; }
        public bool Status { get; set; }
        public int Prestamos { get; set; }
        public int? TipoNomina { get; set; }
        public int? IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public string EmpresaFiscal { get; set; }
        public string EmpresaComplemento { get; set; }
        public string EmpresaSindicato { get; set; }
        public string EmpresaAsimilado { get; set; }
        public string RegistroPatronal { get; set; }
        public string EmpresaRFC { get; set; }
        public string Cliente { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal? SDI { get; set; }
        public decimal SD { get; set; }
        public decimal SalarioReal { get; set; }
        public string Nacionalidad { get; set; }
        public int Esquema { get; set; }
        public string RazonSocial { get; set; }
        public int NumContratos { get; set; }
        public int? idEmpresaFiscal { get; set; }
        public int? idEmpresaAsimilado { get; set; }
        public int? idEmpresaSindicato { get; set; }
        public int? idEmpresaComplemento { get; set; }
        public bool Incapacidad { get; set; }
        public bool Infonavit { get; set; }

    }

    public class DatosBancariosViewModel
    {
        public int Id { get; set; }
        public int IdContrato { get; set; }
        public string Banco { get; set; }
        public string CuentaBancaria { get; set; }
        public string NumTarjeta { get; set; }
        public int? NoSigaF { get; set; }
        public int? NoSigaC { get; set; }
        public string Clabe { get; set; }
        public bool Status { get; set; }
        public string Descripcion { get; set; }
    }

    public class EmpleadoDatosNominaViewModel
    {
        public int IdEmpleado { get; set; }
        public int IdContrato { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Sexo { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public string UMF { get; set; }
        public string FechaAlta { get; set; }
        public string FechaReal { get; set; }
        public string FechaIMSS { get; set; }
        public string FechaVigencia { get; set; }
        public string TipoContrato { get; set; }
        public int? DiasContrato { get; set; }
        public string DiaDescanso { get; set; }

        public string TipoNomina { get; set; }
        public string TipoPago { get; set; }
        public string TipoSalario { get; set; }
        public string SD { get; set; }
        public string SDI { get; set; }
        public string SalarioReal { get; set; }
        public string EmpresaFiscal { get; set; }
        public string EmpresaComplemento { get; set; }
        public string EmpresaSindicato { get; set; }
        public string EmpresaAsimilado { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
        public decimal? FactorDescuento { get; set; }
        public string TipoCredito { get; set; }
        public decimal? DescuentoBimestral { get; set; }
        public decimal? RetencionFonacot { get; set; }

    }
    //public class EmpleadosRFCnoValidoViewModel
    //{
    //    public int IdEmpleado { get; set; }
    //    public string Nombres { get; set; }
    //    public string Paterno { get; set; }
    //    public string Materno { get; set; }
    //    public string RFC { get; set; }
    //}

    public class DatosEmpleadoValidacion
    {
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public string CuentaBancaria { get; set; }
        public string NumTarjeta { get; set; }
        public string Clabe { get; set; }
    }

    public class ValidationData
    {
        public string Title { get; set; }
        public string Data { get; set; }
        public bool Type { get; set; }
        public string Msg { get; set; }
    }

    public class ValidationEmpleado
    {
        public string Nombre { get; set; }
        public int NumWar { get; set; }
        public int NumError { get; set; }
        public List<ValidationData> data { get; set; }
    }
}
