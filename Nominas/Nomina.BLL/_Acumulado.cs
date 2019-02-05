using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.BLL;
using RH.Entidades;
using RH.Entidades.GlobalModel;
using Common.Utils;
namespace Nomina.BLL
{
   
    public class _Acumulado
    {
        RHEntities ctx = null;

        public _Acumulado()
        {
            ctx = new RHEntities();
        }

        public List<NOM_Ejercicio_Fiscal> ejercicios()
        {
            using (var context = new RHEntities())
            {
                var lista = (from e in context.NOM_Ejercicio_Fiscal
                    orderby e.IdEjercicio descending
                    select e).ToList();

                return lista;
            }
        }
        public List<Empresa> empresas()
        {
            using (var context = new RHEntities())
            {
                var lista = (from e in context.Empresa
                    orderby e.RazonSocial
                    select e).ToList();
                return lista;
            }

      
        }
        public List<ClienteSucursal> clienteSucursal()
        {
            var datos = (from s in ctx.Sucursal
                         join c in ctx.Cliente
                         on s.IdCliente equals c.IdCliente
                         select new ClienteSucursal
                         {
                             IdSucursal = s.IdSucursal,
                             Cliente =  c.Nombre,
                             Sucursal = s.Ciudad
                         }).OrderBy(x=>x.IdSucursal).ToList();
            return datos;
        }

        public List<EstructuraPeriodo> periodoByEjercicio(int idEjercicio)
        {
            try
            {
                var datos = (from p in ctx.NOM_PeriodosPago
                             join s in ctx.Sucursal
                             on p.IdSucursal equals s.IdSucursal
                             join c in ctx.Cliente
                             on s.IdCliente equals c.IdCliente
                             where p.IdEjercicio == idEjercicio && p.Autorizado == true
                             select new EstructuraPeriodo
                             {
                                 IdPeriodo = p.IdPeriodoPago,
                                 Descripcion = p.Descripcion,
                                 TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == p.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault(),
                                 DiasPeriodo = p.DiasPeriodo,
                                 FechaInicio = p.Fecha_Inicio,
                                 FechaFin = p.Fecha_Fin,
                                 Sucursal = s.Ciudad,
                                 Autorizado = p.Autorizado,
                                 Cliente = c.Nombre

                             }).ToList();
                return datos;
            }
            catch(Exception e)
            {
                return null;
            }
            //var datos = ctx.NOM_PeriodosPago.Where(x => x.IdEjercicio == idEjercicio).ToList();
      
        }

        public List<EstructuraPeriodo> periodoByEjercicioByBimestre(int idEjercicio, int idBimestres)
        {
            try
            {
                var datos = (from p in ctx.NOM_PeriodosPago
                             join s in ctx.Sucursal
                             on p.IdSucursal equals s.IdSucursal
                             join c in ctx.Cliente
                             on s.IdCliente equals c.IdCliente
                             where p.IdEjercicio == idEjercicio && p.Autorizado == true && p.Bimestre == idBimestres
                             select new EstructuraPeriodo
                             {
                                 IdPeriodo = p.IdPeriodoPago,
                                 Descripcion = p.Descripcion,
                                 TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == p.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault(),
                                 DiasPeriodo = p.DiasPeriodo,
                                 FechaInicio = p.Fecha_Inicio,
                                 FechaFin = p.Fecha_Fin,
                                 Sucursal = s.Ciudad,
                                 Autorizado = p.Autorizado,
                                 Cliente = c.Nombre

                             }).ToList();
                return datos;
            }
            catch (Exception e)
            {
                return null;
            }
            //var datos = ctx.NOM_PeriodosPago.Where(x => x.IdEjercicio == idEjercicio).ToList();

        }

        public List<EstructuraPeriodo> periodoByBimestre(int idBimestres)
        {
            try
            {
                var datos = (from p in ctx.NOM_PeriodosPago
                             join s in ctx.Sucursal
                             on p.IdSucursal equals s.IdSucursal
                             join c in ctx.Cliente
                             on s.IdCliente equals c.IdCliente
                             where p.Autorizado == true && p.Bimestre == idBimestres
                             select new EstructuraPeriodo
                             {
                                 IdPeriodo = p.IdPeriodoPago,
                                 Descripcion = p.Descripcion,
                                 TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == p.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault(),
                                 DiasPeriodo = p.DiasPeriodo,
                                 FechaInicio = p.Fecha_Inicio,
                                 FechaFin = p.Fecha_Fin,
                                 Sucursal = s.Ciudad,
                                 Autorizado = p.Autorizado,
                                 Cliente = c.Nombre

                             }).ToList();
                return datos;
            }
            catch (Exception e)
            {
                return null;
            }
            //var datos = ctx.NOM_PeriodosPago.Where(x => x.IdEjercicio == idEjercicio).ToList();

        }
        public List<EstructuraPeriodo> periodos()
        {
            try
            {
                var datos = (from p in ctx.NOM_PeriodosPago
                             join s in ctx.Sucursal
                             on p.IdSucursal equals s.IdSucursal
                             join c in ctx.Cliente
                             on s.IdCliente equals c.IdCliente
                             where p.Autorizado == true 
                             select new EstructuraPeriodo
                             {
                                 IdPeriodo = p.IdPeriodoPago,
                                 Descripcion = p.Descripcion,
                                 TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == p.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault(),
                                 DiasPeriodo = p.DiasPeriodo,
                                 FechaInicio = p.Fecha_Inicio,
                                 FechaFin = p.Fecha_Fin,
                                 Sucursal = s.Ciudad,
                                 Autorizado = p.Autorizado,
                                 Cliente = c.Nombre

                             }).ToList();
                return datos;
            }
            catch (Exception e)
            {
                return null;
            }
            //var datos = ctx.NOM_PeriodosPago.Where(x => x.IdEjercicio == idEjercicio).ToList();

        }
        public List<EstructuraPeriodo> periodoBySucursal(int IdSucursal)
        {
            try
            {
                var datos = (from p in ctx.NOM_PeriodosPago
                             join s in ctx.Sucursal
                             on p.IdSucursal equals s.IdSucursal
                             join c in ctx.Cliente
                             on s.IdCliente equals c.IdCliente
                             where p.Autorizado == true && p.IdSucursal == IdSucursal
                             select new EstructuraPeriodo
                             {
                                 IdPeriodo = p.IdPeriodoPago,
                                 Descripcion = p.Descripcion,
                                 TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == p.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault(),
                                 DiasPeriodo = p.DiasPeriodo,
                                 FechaInicio = p.Fecha_Inicio,
                                 FechaFin = p.Fecha_Fin,
                                 Sucursal = s.Ciudad,
                                 Autorizado = p.Autorizado,
                                 Cliente = c.Nombre

                             }).ToList();
                return datos;
            }
            catch (Exception e)
            {
                return null;
            }
            //var datos = ctx.NOM_PeriodosPago.Where(x => x.IdEjercicio == idEjercicio).ToList();

        }
        public List<EstructuraPeriodo> periodoByEmpresa(int IdEmpresa)
        {
         
            try
            {
                List<EstructuraPeriodo> listado = new List<EstructuraPeriodo>();
                var sucursales = ctx.Sucursal_Empresa.Where(x => x.IdEmpresa == IdEmpresa).ToList();
                foreach (var suc in sucursales)
                {
                    var datos = (from p in ctx.NOM_PeriodosPago
                                 join s in ctx.Sucursal
                                 on p.IdSucursal equals s.IdSucursal
                                 join c in ctx.Cliente
                                 on s.IdCliente equals c.IdCliente
                                 where p.Autorizado == true && s.IdSucursal == suc.IdSucursal
                                 select new EstructuraPeriodo
                                 {
                                     IdPeriodo = p.IdPeriodoPago,
                                     Descripcion = p.Descripcion,
                                     TipoNomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == p.IdTipoNomina).Select(x => x.Descripcion).FirstOrDefault(),
                                     DiasPeriodo = p.DiasPeriodo,
                                     FechaInicio = p.Fecha_Inicio,
                                     FechaFin = p.Fecha_Fin,
                                     Sucursal = s.Ciudad,
                                     Autorizado = p.Autorizado,
                                     Cliente = c.Nombre

                                 }).FirstOrDefault();
                    if(datos != null)
                    {
                        listado.Add(datos);
                    }
                    
                }
           
                return listado;
            }
            catch (Exception e)
            {
                return null;
            }
            //var datos = ctx.NOM_PeriodosPago.Where(x => x.IdEjercicio == idEjercicio).ToList();

        }

        public List<Cliente> GetClientes()
        {
            using (var context = new RHEntities())

            {
                var lista = (from c in context.Cliente
                             orderby c.Nombre
                    select c).ToList();

                return lista;
            }
        }

    }

 
    //public class ClienteSucursal{
    //    public int IdSucursal { get; set; }
    //    public string Cliente { get; set; }
    //    public string Sucursal { get; set; }
    //}

    public class EstructuraPeriodo
    {
        public int IdPeriodo { get; set; }
        public string Descripcion { get; set; }
        public string TipoNomina { get; set; }
        public int DiasPeriodo { get; set; } 
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Sucursal { get; set; }
        public bool Autorizado { get; set; }
        public string Cliente { get; set; }
    }
}
