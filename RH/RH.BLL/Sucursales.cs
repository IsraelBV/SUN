using System;
using System.Collections.Generic;
using System.Linq;
using Common.Helpers;
using MoreLinq;
using RH.Entidades;
using SYA.BLL;

namespace RH.BLL
{
    public class Sucursales
    {
      //  readonly RHEntities _ctx;

        public Sucursales()
        {
          //  _ctx = new RHEntities();
        }

        public List<SucursalDatos> SearchSucursales(string keyword)
        {
            var user = ControlAcceso.GetUsuarioEnSession();
            if (user != null)
            {
                using (var context = new RHEntities())
                {

                    if (user.IdPerfil == 1 || user.Sucursales == "*")
                    {
                        return (from s in context.Sucursal
                            join e in context.Cliente on s.IdCliente equals e.IdCliente
                            orderby s.IdSucursal
                            where
                            s.Ciudad.Contains(keyword) || e.Nombre.Contains(keyword) ||
                            s.IdSucursal.ToString().Equals(keyword)
                            select new SucursalDatos
                            {
                                IdSucursal = s.IdSucursal,
                                Nombre = e.Nombre,
                                Ciudad = s.Ciudad
                            }
                        ).ToList();
                    }
                    else
                    {
                        var sucursales = user.Sucursales.Trim(',');
                        return (from s in context.Sucursal
                            join e in context.Cliente on s.IdCliente equals e.IdCliente
                            orderby s.IdSucursal
                            where
                            sucursales.Contains(s.IdSucursal.ToString()) &&
                            (s.Ciudad.Contains(keyword) || e.Nombre.Contains(keyword) ||
                             e.IdCliente.ToString().Equals(keyword))
                            select new SucursalDatos
                            {
                                IdSucursal = s.IdSucursal,
                                Nombre = e.Nombre,
                                Ciudad = s.Ciudad
                            }
                        ).ToList();
                    }

                }
            }
            else return null;
        }

        public List<Sucursal> ObtenerSucursales()
        {
            using (var context = new RHEntities())
            {
                List<Sucursal> sucursales;
                sucursales = context.Sucursal.ToList();
                return sucursales;
            }
        }

        public static List<SucursalDatos> GetSucursales()
        {
            using (var context = new RHEntities())
            {
                return (from s in context.Sucursal
                    join c in context.Cliente on s.IdCliente equals c.IdCliente
                    orderby s.IdSucursal
                    select new SucursalDatos
                    {
                        IdSucursal = s.IdSucursal,
                        Cliente = c.Nombre,
                        Sucursal = s.Ciudad
                    }).ToList();
            }
        }

        /// <summary>
        /// Obtiene las sucursales a las que tiene acceso el usuario
        /// </summary>
        /// <returns></returns>
        public List<SucursalDatos> GetSucursalesByUser()
        {
            var user = ControlAcceso.GetUsuarioEnSession();

            if (user == null) return null;

            //Validamos el campo sucursal no este vacio
            if(user.Sucursales.Trim() == "") return null;

            var list = new List<SucursalDatos>();
           
                list = Sucursales.GetSucursales();

            if (user.IdPerfil == 1 || user.Sucursales.Trim() == "*") return list;

            var sucursales = user.Sucursales.Split(',');
            list = list.Where(x => sucursales.Contains(x.IdSucursal.ToString())).ToList();

            return list;
        }

        public List<SucursalDatos> ObtenerTopSucursales()
        {
            using (var context = new RHEntities())
            {
                var lista = (from s in context.Sucursal
                    join e in context.Cliente on s.IdCliente equals e.IdCliente
                    orderby s.IdSucursal
                    select new SucursalDatos {IdSucursal = s.IdSucursal, Nombre = e.Nombre, Ciudad = s.Ciudad}).Take(10);
                return lista.ToList();
            }
        }

        public List<SucursalDatos> ObtenerSucursalDatos()
        {
            using (var context = new RHEntities())
            {
                var lista = (from s in context.Sucursal
                    join e in context.Cliente on s.IdCliente equals e.IdCliente
                    orderby s.IdSucursal
                    select new SucursalDatos {IdSucursal = s.IdSucursal, Nombre = e.Nombre, Ciudad = s.Ciudad});
                return lista.ToList();
            }
        }

        /// <summary>
        /// Metodo para obtener las sucursales con paginacion, y retorna el total de paginas 
        /// </summary>
        /// <param name="regsPorPagina"></param>
        /// <param name="numDePagina"></param>
        /// <param name="totalPaginas"></param>
        /// <param name="sucursales"></param>
        /// <returns></returns>
        public List<SucursalDatos> ObtenerSucursalPaginado(int regsPorPagina, int numDePagina, out int totalPaginas, string sucursales)
        {
            var indice = 1;
            totalPaginas = 0;
            numDePagina = numDePagina - 1;

            if (numDePagina < 0 || regsPorPagina <= 0) return null;

            var inicial = (regsPorPagina * numDePagina + 1);
            var final = (regsPorPagina * (numDePagina + 1));


            var datos = ObtenerSucursalByIds();

            if (datos == null) return null;

            var paginas = (datos.Count() / (double)regsPorPagina);

            totalPaginas = (int)Math.Ceiling(paginas);

            if (numDePagina + 1 > totalPaginas) return null;

            var lista = new List<SucursalDatos>();

            foreach (var item in datos)
            {
                lista.Add(new SucursalDatos
                {
                    IdSucursal = item.IdSucursal,
                    Nombre = item.Nombre,
                    Ciudad = item.Ciudad,
                    NumRegistro = indice
                });

                indice++;
            }

            var paginado = lista.Where(x => x.NumRegistro >= inicial && x.NumRegistro <= final).ToList();

            return paginado;

        }

        public List<SucursalDatos> ObtenerSucursalPaginadoV2()
        {
            var indice = 1;

            var datos = ObtenerSucursalByIds();

            if (datos == null) return null;

            var lista = new List<SucursalDatos>();

            foreach (var item in datos)
            {
                lista.Add(new SucursalDatos
                {
                    IdSucursal = item.IdSucursal,
                    Nombre = item.Nombre,
                    Ciudad = item.Ciudad,
                    NumRegistro = indice
                });

                indice++;
            }

            lista = lista.OrderBy(x => x.IdSucursal).ToList();

            return lista;

        }

        /// <summary>
        /// Retorna las sucursales que estan en la cadena ids = "10,11,12,13". 
        /// Puede ser usado para obtener las sucursales que tiene asignado los usuarios.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SucursalDatos> ObtenerSucursalByIds()
        {
            List<SucursalDatos> lista;

            var user = ControlAcceso.GetUsuarioEnSession();

            if (user.IdPerfil == 1)
                lista = ObtenerSucursalDatos();
            else if (user.Sucursales.Trim() == "*")
                lista = ObtenerSucursalDatos();
            else
            {
                var array = user.Sucursales.Split(',');

                using (var context = new RHEntities())
                {
                    var datos = (from s in context.Sucursal
                        join e in context.Cliente on s.IdCliente equals e.IdCliente
                        where array.Contains(s.IdSucursal.ToString())
                        orderby s.IdSucursal
                        select new SucursalDatos {IdSucursal = s.IdSucursal, Nombre = e.Nombre, Ciudad = s.Ciudad});

                    lista = !datos.Any() ? null : datos.ToList();
                }
            }

            lista = lista?.OrderBy(x => x.Nombre).ToList();

            return lista;
            
        }


        //Obtiene el número de sucursales a las que tiene acceso el usuario para el paginador
        //Según el númeroq que tenga lo divide entre 10 para el paginador
        public int ObtenerNumeroSucursalesPaginador()
        {
            var usuario = ControlAcceso.GetUsuarioEnSession();
            double totalSucursales = 0;
            if(usuario.Sucursales.Trim() =="*")
            {
                totalSucursales = ObtenerSucursalDatos().Count();
            }
            else
            {
                totalSucursales = (usuario.Sucursales).Split(',').Count();
            }
            double paginas = totalSucursales / 10;
            paginas = paginas > 0 ? Math.Ceiling(paginas) : 0;
            return Convert.ToInt32(paginas);
        }


        public Sucursal GetSucursalById(int id)
        {
            using (var context = new RHEntities())
            {
                var sucursal = context.Sucursal.FirstOrDefault(x => x.IdSucursal == id);
                return sucursal;
            }
        }

        public SucursalDatos ObtenerSucursalDatosPorId(int id)
        {
            using (var context = new RHEntities())
            {
                var sucursal = (
                    from s in context.Sucursal
                    join e in context.Cliente on s.IdCliente equals e.IdCliente
                    where s.IdSucursal == id
                    select
                    new SucursalDatos
                    {
                        IdSucursal = s.IdSucursal,
                        Nombre = e.Nombre,
                        Ciudad = s.Ciudad,
                        IdCliente = s.IdCliente
                    }).FirstOrDefault();

                if (sucursal == null) return null;

                //Ejercicios Fiscales en esta sucursal
                var arrayIdE = context.NOM_PeriodosPago.Select(x => x.IdEjercicio).Distinct().ToArray();

                if (arrayIdE.Length <= 0) return sucursal;
                {
                    var lista = (from e in context.NOM_Ejercicio_Fiscal
                        where arrayIdE.Contains(e.IdEjercicio)
                        select e).ToList();

                    if (!lista.Any()) return sucursal;

                    lista = lista.OrderByDescending(x => x.IdEjercicio).ToList();
                    sucursal.Ejercicios = lista;
                }


                //fin eje

                return sucursal;
            }
        }

        public bool InsertarSucursalReciente(int IdSucursal)
        {
            bool result = false;

            var usuario = SessionHelpers.GetIdUsuario();

            SYA_Recientes sucursalReciente = new SYA_Recientes();

            sucursalReciente.IdSucursal = IdSucursal;

            sucursalReciente.IdUsuario = usuario;

            var date = DateTime.Now;

            sucursalReciente.FechaIngreso = date;

            using (var context = new RHEntities())
            {
                context.SYA_Recientes.Add(sucursalReciente);

                int response = context.SaveChanges();
                
                if (response > 0)
                {
                    result = true;
                }
            }

            return result;
        }


        /// <summary>
        /// Retorna las 5 sucursales mas recientes a las que ha accedido el usuario
        /// que esta actualemente en session.
        /// </summary>
        /// <returns></returns>
        public List<SucursalDatos> GetSucursalesRecientes()
        {

            int idUser = SessionHelpers.GetIdUsuario();

            if (idUser <= 0) return null;


            using (var context = new RHEntities())
            {
                var listaReciente =
                    context.SYA_Recientes.Where(x => x.IdUsuario == idUser)
                        .Distinct()
                        .OrderByDescending(x => x.Id)
                        .ToList();

                var primeros5 = listaReciente.DistinctBy(x => x.IdSucursal).Take(5).ToList();

                var datos = (from a in primeros5
                    join s in context.Sucursal on a.IdSucursal equals s.IdSucursal
                    join e in context.Cliente on s.IdCliente equals e.IdCliente
                    select
                    new SucursalDatos
                    {
                        IdSucursal = s.IdSucursal,
                        Nombre = e.Nombre,
                        Ciudad = s.Ciudad,
                        FechaReciente = a.FechaIngreso.ToString("dd/MM/yyyy")
                    }).ToList();


                return datos;
            }
        }


        public List<SucursalesEmpresa> ListSucursalEmpresa(int idsucursal)
        {
            using (var context = new RHEntities())
            {

                var datos = (from empresa in context.Empresa.AsEnumerable()
                    join sucursal in context.Sucursal_Empresa.AsEnumerable()
                    on empresa.IdEmpresa equals sucursal.IdEmpresa
                    where sucursal.IdSucursal == idsucursal && sucursal.IdEsquema == 1

                    select new SucursalesEmpresa
                    {
                        IdTabla = empresa.IdEmpresa,
                        Nombre = empresa.RazonSocial,
                        ClaveContable = sucursal.Clave_Contable,
                        CP = empresa.CP,
                        Estado =
                            Convert.ToString(
                                context.C_Estado.Where(x => x.IdEstado == empresa.IdEstado)
                                    .Select(x => x.Descripcion)
                                    .FirstOrDefault()),
                        Municipio = empresa.Municipio,
                        Colonia = empresa.Colonia,
                        Calle = empresa.Calle,
                        NoExt = empresa.NoExt,
                        RFC = empresa.RFC,
                        RP = empresa.RegistroPatronal
                    }).ToList();
                return datos.DistinctBy(x => x.Nombre).ToList();

            }

        }
        public List<SucursalesEmpresa> ListSucEmp(int idsucursal)
        {
            using (var context = new RHEntities())
            {

                var datos = (from empresa in context.Empresa.AsEnumerable()
                    join sucursal in context.Sucursal_Empresa.AsEnumerable()
                    on empresa.IdEmpresa equals sucursal.IdEmpresa
                    where sucursal.IdSucursal == idsucursal

                    select new SucursalesEmpresa
                    {
                        IdTabla = empresa.IdEmpresa,
                        Nombre = empresa.RazonSocial,
                        ClaveContable = sucursal.Clave_Contable,
                        CP = empresa.CP,
                        Estado =
                            Convert.ToString(
                                context.C_Estado.Where(x => x.IdEstado == empresa.IdEstado)
                                    .Select(x => x.Descripcion)
                                    .FirstOrDefault()),
                        Municipio = empresa.Municipio,
                        Colonia = empresa.Colonia,
                        Calle = empresa.Calle,
                        NoExt = empresa.NoExt,
                        RFC = empresa.RFC,
                        RP = empresa.RegistroPatronal,
                        PrimaRiesgo = empresa.PrimaRiesgo == null ? 0 : empresa.PrimaRiesgo.Value
                    }).ToList();
                return datos.DistinctBy(x => x.Nombre).ToList();

            }
        }
        public bool UpdateClave(int id, string ClaveContable)
        {
            using (var context = new RHEntities())
            {

                if (ClaveContable == "")
                {
                    ClaveContable = "...";
                }
                
                var result = false;
                var editar = context.Sucursal_Empresa.FirstOrDefault(x => x.Id == id);
                editar.Clave_Contable = ClaveContable;
                var r = context.SaveChanges();



                if (r > 0)
                    result = true;

                return result;
            }
        }

        public List<SucursalDatos> GetListaClienteSucursalByIdEmpresa(int idEmpresa)
        {
            using (var context = new RHEntities())
            {
                //Buscar las sucusales que tiene asignada la empresa
                //var listaSucursales =
                //    context.Sucursal_Empresa.Where(x => x.IdEmpresa == idEmpresa)
                //        .Select(x => x.IdSucursal)
                //        .Distinct()
                //        .ToArray();

                var listaSucursales = (from t in context.NOM_CFDI_Timbrado
                    where t.IdEmisor == idEmpresa
                    select t.IdSucursal).ToList();

                var listaClientes= (from s in context.Sucursal
                                   join c in context.Cliente on s.IdCliente equals c.IdCliente
                                   where listaSucursales.Contains(s.IdSucursal)
                                   select new SucursalDatos { IdSucursal = s.IdSucursal, Nombre = c.Nombre, Ciudad = s.Ciudad, IdCliente = c.IdCliente }).ToList();


                listaClientes = listaClientes.OrderBy(x => x.Nombre).ToList();

                return listaClientes;


            }
          
        }

        //obtener sucursales por usuario en la sesion
        public List<Sucursal> getSucursalesByUser(SYA_Usuarios usuario)
        {
            var listaSucursales = new List<Sucursal>();

            using (var context = new RHEntities())
            {
                if (usuario.Sucursales != "*")
                {
                    var sucursalesUsuario = usuario.Sucursales.Split(',').Select(Int32.Parse).ToArray();

                    listaSucursales = (from suc in context.Sucursal
                                       where sucursalesUsuario.Contains(suc.IdSucursal)
                                       select suc).ToList();
                }
                else
                {
                    listaSucursales = context.Sucursal.ToList();
                }
            }
            return listaSucursales;
        }
    }


    public class SucursalDatos
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
        public int NumRegistro { get; set; }
        public bool GuardadoEnReciente { get; set; }
        public string FechaReciente { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Sucursal { get; set; }
        public List<NOM_Ejercicio_Fiscal> Ejercicios { get; set; }
    }

    public class SucursalesEmpresa
    {
        public int IdTabla { get; set; }
        public int IdSucursal { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string ClaveContable { get; set; }
        public string CP { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public string NoExt { get; set; }
        public string Calle { get; set; }
        public string RP { get; set; }
        public string RFC { get; set; }
        public decimal PrimaRiesgo { get; set; }


    }
}
