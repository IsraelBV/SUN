using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class _Sucursales
    {
        RHEntities ctx = null;

        public _Sucursales()
        {
            ctx = new RHEntities();
        }

        public List<Sucursal> obtenerSucursales()
        {
            List<Sucursal> sucursales = null;
            sucursales = ctx.Sucursal.ToList();
            return sucursales;
        }

        public List<SucursalDatos> obtenerTopSucursales()
        {
            var lista = (from s in ctx.Sucursal
                         join e in ctx.Empresa on s.IdEmpresa equals e.IdEmpresa
                         orderby s.IdSucursal
                         select new SucursalDatos { IdSucursal = s.IdSucursal, Nombre = e.Nombre, Ciudad = s.Ciudad }).Take(2);
            return lista.ToList();
        }

        public List<SucursalDatos> obtenerSucursalDatos()
        {
            var lista = (from s in ctx.Sucursal
                         join e in ctx.Empresa on s.IdEmpresa equals e.IdEmpresa
                         orderby s.IdSucursal
                         select new SucursalDatos { IdSucursal = s.IdSucursal, Nombre = e.Nombre, Ciudad = s.Ciudad });
            return lista.ToList();
        }
    }


    public class SucursalDatos
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
    }
}
