using RH.BLL;
using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomina.BLL
{
    public class _CuentasContables
    {
        RHEntities ctx = null;

        public _CuentasContables()
        {
            ctx = new RHEntities();
        }

        public List<Empresa> ListEmpresaFiscales()
        {
            var list = ctx.Empresa.Where(x => x.RegistroPatronal != null).OrderBy(x=>x.RazonSocial).ToList();
            return list;
        }

        public List<Cliente> ListaSucursalByID(int idEmpresa)
        {
            var lista = (from s in ctx.Sucursal
                         join se in ctx.Sucursal_Empresa
                         on s.IdSucursal equals se.IdSucursal
                         join c in ctx.Cliente
                         on s.IdCliente equals c.IdCliente
                         where se.IdEmpresa == idEmpresa
                         select c).OrderBy(x=>x.Nombre).Distinct().ToList();
            return lista;
        }

        public List<PeriodoCuenta> listaPeriodoByIDs(int idEmpresa, int idCliente,DateTime fechaInicio, DateTime fechaFin)
        {
            var nominas = ctx.NOM_Nomina.ToList();
            var finiquitos = ctx.NOM_Finiquito.ToList();
            if(idEmpresa == 0)
            {
                var lista = new List<PeriodoCuenta>();
                var datos = ctx.NOM_PeriodosPago.Where(x=>x.SoloComplemento == false && x.Fecha_Pago >= fechaInicio && x.Fecha_Pago <= fechaFin ).Distinct().ToList();
                foreach(var d in datos)
                {
                    var item = new PeriodoCuenta();
                    item.IdPeriodo = d.IdPeriodoPago;
                    item.Nombre = d.Descripcion;
                    item.TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == d.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault();
                    item.FechaInicio = d.Fecha_Inicio;
                    item.FechaFin = d.Fecha_Fin;
                    item.status = d.Autorizado;
                    lista.Add(item);
                }
                
                
                return lista;
            }else if(idEmpresa != 0 && idCliente == 0)
            {
                List<PeriodoCuenta> periodo = new List<PeriodoCuenta>();
                var lista = ctx.Sucursal_Empresa.Where(x => x.IdEmpresa == idEmpresa ).Select(x=>x.IdSucursal).Distinct().ToList();
                
                foreach(var l in lista)
                {
                    var datos = ctx.NOM_PeriodosPago.Where(x => x.SoloComplemento == false && x.IdSucursal == l && x.Fecha_Pago >= fechaInicio && x.Fecha_Pago <= fechaFin).Distinct().ToList();
                    foreach (var d in datos)
                    {
                        var cliente = (from c in ctx.Cliente
                                       join s in ctx.Sucursal
                                       on c.IdCliente equals s.IdCliente
                                       where s.IdSucursal == d.IdSucursal
                                       select c.Nombre).FirstOrDefault();
                        if (d.IdTipoNomina == 16)
                        {
                            int count2 = nominas.Where(x => (x.IdEmpresaAsimilado == idEmpresa) && x.IdPeriodo == d.IdPeriodoPago).Count();
                            if (count2 > 0)
                            {
                                var item = new PeriodoCuenta();
                                item.IdPeriodo = d.IdPeriodoPago;
                                item.Nombre = d.Descripcion;
                                item.TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == d.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault();
                                item.FechaInicio = d.Fecha_Inicio;
                                item.FechaFin = d.Fecha_Fin;
                                item.status = d.Autorizado;
                                item.Cliente = cliente;
                                periodo.Add(item);
                            }
                        }
                        else
                        {
                            int count = nominas.Where(x => (x.IdEmpresaFiscal == idEmpresa) && x.IdPeriodo == d.IdPeriodoPago).Count();
                            int countf = finiquitos.Where(x => x.IdEmpresaFiscal == idEmpresa && x.IdPeriodo == d.IdPeriodoPago).Count();
                            if (count > 0 || countf >0)
                            {
                                var item = new PeriodoCuenta();
                                item.IdPeriodo = d.IdPeriodoPago;
                                item.Nombre = d.Descripcion;
                                item.TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == d.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault();
                                item.FechaInicio = d.Fecha_Inicio;
                                item.FechaFin = d.Fecha_Fin;
                                item.status = d.Autorizado;
                                item.Cliente = cliente;
                                periodo.Add(item);
                            }
                        }





                    }
                }
                return periodo;
            }
            else
            {

                List<PeriodoCuenta> periodo = new List<PeriodoCuenta>();
                var sucursal = ctx.Sucursal.Where(x => x.IdCliente == idCliente).FirstOrDefault();

             
                    var datos = ctx.NOM_PeriodosPago.Where(x => x.SoloComplemento == false && x.IdSucursal == sucursal.IdSucursal && x.Fecha_Inicio >= fechaInicio && x.Fecha_Fin <= fechaFin).ToList();

                foreach (var d in datos)
                {
                    var cliente = (from c in ctx.Cliente
                                   join s in ctx.Sucursal
                                   on c.IdCliente equals s.IdCliente
                                   where s.IdSucursal == d.IdSucursal
                                   select c.Nombre).FirstOrDefault();

                    if (d.IdTipoNomina == 16)
                    {
                        int count2 = nominas.Where(x => (x.IdEmpresaAsimilado == idEmpresa) && x.IdPeriodo == d.IdPeriodoPago).Count();
                        if (count2 > 0)
                        {
                            var item = new PeriodoCuenta();
                            item.IdPeriodo = d.IdPeriodoPago;
                            item.Nombre = d.Descripcion;
                            item.TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == d.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault();
                            item.FechaInicio = d.Fecha_Inicio;
                            item.FechaFin = d.Fecha_Fin;
                            item.status = d.Autorizado;
                            item.Cliente = cliente;
                            periodo.Add(item);
                        }
                    }
                    else
                    {
                        int count = nominas.Where(x => (x.IdEmpresaFiscal == idEmpresa) && x.IdPeriodo == d.IdPeriodoPago).Count();
                        int countf = finiquitos.Where(x => x.IdEmpresaFiscal == idEmpresa && x.IdPeriodo == d.IdPeriodoPago).Count();
                        if (count > 0 || countf > 0)
                        {
                            var item = new PeriodoCuenta();
                            item.IdPeriodo = d.IdPeriodoPago;
                            item.Nombre = d.Descripcion;
                            item.TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == d.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault();
                            item.FechaInicio = d.Fecha_Inicio;
                            item.FechaFin = d.Fecha_Fin;
                            item.status = d.Autorizado;
                            item.Cliente = cliente;
                            periodo.Add(item);
                        }
                    }





                }
                return periodo;
            }
            
        }

        public List<EmpresasCuentas> empresasCuentas(int idEmpresa)
        {
            var conceptos = ctx.C_NOM_Conceptos.ToList();

            List<EmpresasCuentas> emp_cue = new List<EmpresasCuentas>();
            
            foreach(var c in conceptos)
            {
                EmpresasCuentas item = new EmpresasCuentas();
                _CuentasContables cuentas = new _CuentasContables();
                var datos = ctx.ClavesContables.FirstOrDefault(x => x.IdConcepto == c.IdConcepto && x.IdEmpresa == idEmpresa);
                
                if(datos != null)
                {
                    item.IdConcepto = c.IdConcepto;
                    item.IdEmpresa = idEmpresa;
                    item.ClaveSat = c.Clave;
                    item.Descripcion = c.Descripcion;
                    item.Tipo = c.TipoConcepto == 1 ? "PERCEPCION" : "DEDUCCION";
                    item.Cuenta_Acreedora = datos.Acredora;
                    item.Cuenta_Deudora = datos.Deudora;

                    emp_cue.Add(item);

                }else
                {
                    item.IdConcepto = c.IdConcepto;
                    item.IdEmpresa = idEmpresa;
                    item.ClaveSat = c.Clave;
                    item.Descripcion = c.Descripcion;
                    item.Tipo = c.TipoConcepto == 1 ? "PERCEPCION" : "DEDUCCION";
                    item.Cuenta_Acreedora = "";
                    item.Cuenta_Deudora = "";

                    emp_cue.Add(item);
                }
                
            }
            return emp_cue;
        }

        public bool GuardarDeudora(int idConcepto ,int idEmpresa, string cuenta) 
        {
            try
            {
                var resultado = false;
                ClavesContables item = new ClavesContables();
                var dato = ctx.ClavesContables.Where(x => x.IdConcepto == idConcepto && x.IdEmpresa == idEmpresa).FirstOrDefault();

                if (dato != null)
                {
                    dato.Deudora = cuenta;

                    var r = ctx.SaveChanges();

                    if (r > 0)
                        resultado = true;
                    return resultado;
                }
                else
                {
                    item.IdConcepto = idConcepto;
                    item.IdEmpresa = idEmpresa;
                    item.Deudora = cuenta;
                    item.Acredora = "";

                    var ad = ctx.ClavesContables.Add(item);
                    var r = ctx.SaveChanges();

                    if (r > 0)
                        resultado = true;
                    return resultado;
                }

            }
            catch (Exception e)
            {
                return false;
            }



            
        }
        public bool GuardarAcredora(int idConcepto, int idEmpresa, string cuenta)
        {
            try
            {
                var resultado = false;
                ClavesContables item = new ClavesContables();
                var dato = ctx.ClavesContables.Where(x => x.IdConcepto == idConcepto && x.IdEmpresa == idEmpresa).FirstOrDefault();

                if (dato != null)
                {
                    dato.Acredora = cuenta;

                    var r = ctx.SaveChanges();

                    if (r > 0)
                        resultado = true;
                    return resultado;
                }
                else
                {
                    item.IdConcepto = idConcepto;
                    item.IdEmpresa = idEmpresa;
                    item.Deudora = "";
                    item.Acredora = cuenta;

                    var ad = ctx.ClavesContables.Add(item);
                    var r = ctx.SaveChanges();

                    if (r > 0)
                        resultado = true;
                    return resultado;
                }

            }
            catch (Exception e)
            {
                return false;
            }




        }
        public List<ClaveCliente> ClientesByEmpresa(int idEmpresa)
        {
            var datos = (from c in ctx.Cliente
                         join s in ctx.Sucursal
                         on c.IdCliente equals s.IdCliente
                         join sc in ctx.Sucursal_Empresa
                         on s.IdSucursal equals sc.IdSucursal
                         where sc.IdEmpresa == idEmpresa && sc.IdEsquema == 1
                         select new ClaveCliente
                         {
                             IdSucursalEmpresa = sc.Id,
                             IdCliente = c.IdCliente,
                             Nombre = c.Nombre,
                             Clave = sc.Clave_Contable
                         }).ToList();
            return datos; 
        }
        public bool GuardarClaveCliente(int idCliente, string ClaveCliente,int idEmpresa)
        {
            try
            {
                var resultado = false;
                var listaSucursales = ctx.Sucursal.Where(x => x.IdCliente == idCliente).ToList();

                foreach(var s in listaSucursales)
                {
                    var dato = ctx.Sucursal_Empresa.Where(x => x.IdSucursal == s.IdSucursal && x.IdEmpresa == idEmpresa).FirstOrDefault();
                    dato.Clave_Contable = ClaveCliente.ToUpper();
                   
                    var r = ctx.SaveChanges();
                    if (r > 0)
                        resultado = true;
                    
                }

                return resultado;
            }
            catch (Exception e)
            {
                return false;
            }




        }

        
    }


    public class EmpresasCuentas
    {
        public int IdEmpresa { get; set; }
        public int IdConcepto { get; set; }
        public string ClaveSat { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string Cuenta_Deudora { get; set; }
        public string Cuenta_Acreedora { get; set; }
    }

    public class PeriodoCuenta
    {
        public int IdPeriodo { get; set; }
        public string Nombre { get; set; }
        public string TipoNomina { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool status { get; set; }
        public string Cliente { get; set; }
    }

    public class ClaveCliente
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public int IdSucursalEmpresa { get; set; }
    }
}
