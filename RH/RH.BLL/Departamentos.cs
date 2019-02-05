using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class Departamentos
    {
        RHEntities ctx = null;

        public Departamentos()
        {
            ctx = new RHEntities();
        }

        /// <summary>
        /// Buscar departamento por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Departamento GetDepartamentoById(int id)
        {
            var depa = ctx.Departamento.FirstOrDefault(x => x.IdDepartamento == id);

            return depa;
        }

        /// <summary>
        /// Crear departamento
        /// </summary>
        /// <param name="bn"></param>
        /// <returns></returns>
        public bool CrearDepartamento(Departamento dp)
        {
            var result = false;

            ctx.Departamento.Add(dp);
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Actualizar datos Departamento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bn"></param>
        /// <returns></returns>
        public bool UpdateDepa(int id, Departamento dp)
        {
            var result = false;

            var item = ctx.Departamento.FirstOrDefault(x => x.IdDepartamento == id);

            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;

            item.Descripcion = dp.Descripcion.Trim();
            item.Status = dp.Status;

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Eliminar un departamento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool DeleteDepa(int id)
        {
            var result = false;

            var item = ctx.Departamento.FirstOrDefault(x => x.IdDepartamento == id);

            //si no se encontro un registro con ese id,
            // retornamos false 
            if (item == null) return false;

            ctx.Departamento.Attach(item);
            ctx.Departamento.Remove(item);

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        public List<DepartamentoDatos> ObtenerDepartamentosPorEmpresa(int empresa)
        {
            List<DepartamentoDatos> lista =
                (from dep in ctx.Departamento
                 join rel in ctx.Cliente_Departamento on dep.IdDepartamento equals rel.IdDepto
                 where rel.IdCliente == empresa
                 orderby dep.Descripcion
                 select new DepartamentoDatos { IdDepto = dep.IdDepartamento, Descripcion = dep.Descripcion }).Distinct().ToList();
            return lista;
        }

        /// <summary>
        /// Obtener lista de departamentos
        /// </summary>
        /// <returns></returns>
        public List<Departamento> GetDepartamentos()
        {
            var departamentos = ctx.Departamento.ToList();

            return departamentos;
        }

        /// <summary>
        /// Obtener número de puestos por departamento
        /// </summary>
        /// <param name="departamento"></param>
        /// <returns></returns>
        public int GetNumPuestosByDepartamento(int departamento)
        {
            return (from p in ctx.Puesto
                    where p.IdDepartamento == departamento
                    select p.IdPuesto)
                    .Count();
        }
        //obtiene el id del cliente  
        //donde esta ingresado el usuario
        public Sucursal GetIdCliente(int id)
        {
            var idcliente = ctx.Sucursal.FirstOrDefault(x => x.IdSucursal == id);
            return idcliente;
        }

        // obtiene el listado de departamentos 
        //del cliente 
        public List<DepartamentoDatos> GetDepartamentosbyCliente(int id)
        {

            var datos = from cli_depa in ctx.Cliente_Departamento
                        join depa in ctx.Departamento on cli_depa.IdDepto equals depa.IdDepartamento
                        where cli_depa.IdCliente == id
                        select new DepartamentoDatos
                        {
                            IdDepto = depa.IdDepartamento,
                            IdCliente = cli_depa.IdCliente,
                            Descripcion = depa.Descripcion,
                            status = depa.Status
                        };

            //var departamentos = ctx.Departamento.ToList();

            return datos.ToList();
        }
        public bool AsignarDepa(int IdDepartamento , int IdCliente)
        {
            var result = false;
            Cliente_Departamento item = new Cliente_Departamento();
            item.IdDepto = IdDepartamento;
            item.IdCliente = IdCliente;
            ctx.Cliente_Departamento.Add(item);
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }
    }

    public class DepartamentoDatos
    {
        public int IdDepto { get; set; }
        public int? IdCliente { get; set; }
        public string Descripcion { get; set; }
        public bool status { get; set; }
        public int Puestos { get; set; }
    }
}
