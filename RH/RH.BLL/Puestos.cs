using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class Puestos
    {
        RHEntities ctx = null;

        public Puestos()
        {
            ctx = new RHEntities();
        }

        /// <summary>
        /// Obtener lista de puestos
        /// </summary>
        /// <returns></returns>
        public List<Puesto> GetPuestos()
        {
            var lista = ctx.Puesto.ToList();

            return lista;
        }

        /// <summary>
        /// Buscar puestos por ID de departamento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Puesto> GetPuestosByDepto(int id)
        {
            return ctx.Puesto.Where(x => x.IdDepartamento == id).ToList();

        }

        /// <summary>
        /// Buscar puesto por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Puesto GetPuesto(int? id)
        {
            return ctx.Puesto.FirstOrDefault(x => x.IdPuesto == id);

        }

        /// <summary>
        /// Crear puesto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CrearPuesto(Puesto ps)
        {
            var result = false;

            ps.Status = true;

            ctx.Puesto.Add(ps);
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Actualizar datos Puesto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public bool UpdatePuesto(Puesto ps)
        {
            var result = false;

            var item = ctx.Puesto.FirstOrDefault(x => x.IdPuesto == ps.IdPuesto );

            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;

            item.Descripcion = ps.Descripcion.Trim();
            item.PuestoRecibo = ps.PuestoRecibo.Trim();
            item.Status = ps.Status;

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }



        public List<PuestosDatos> ObtenerPuestosPorEmpresa(int cliente)
        {
            return (from p in ctx.Puesto
                    join d in ctx.Departamento on p.IdDepartamento equals d.IdDepartamento
                    join s in ctx.Cliente_Departamento on d.IdDepartamento equals s.IdDepto
                    where s.IdCliente == cliente
                    orderby d.Descripcion
                    select new PuestosDatos { IdPuesto = p.IdPuesto, Descripcion = p.Descripcion, Departamento = d.Descripcion }).Distinct().ToList();
        }

        public Puesto ObtenerPuestoPorId(int idPuesto)
        {
            return ctx.Puesto.FirstOrDefault(x => x.IdPuesto == idPuesto);
        }

        public List<String> GetPuestosLista(int cliente)
        {
            return (from p in ctx.Puesto
                    join d in ctx.Departamento on p.IdDepartamento equals d.IdDepartamento
                    join s in ctx.Cliente_Departamento on d.IdDepartamento equals s.IdDepto
                    where s.IdCliente == cliente
                    orderby d.Descripcion
                    select p.Descripcion).Distinct().ToList();
        }

        public int ObtenerPuestoPorDescripcion(string descripcion)
        {
            return (from p in ctx.Puesto where p.Descripcion.Equals(descripcion) select p.IdPuesto).FirstOrDefault();
        }

        public string GetDescripcionById(int idPuesto)
        {
            var puesto = ctx.Puesto.FirstOrDefault(x => x.IdPuesto == idPuesto);
            return puesto != null ? puesto.Descripcion : "n/a";
        }
    }

    public class PuestosDatos
    {
        public int IdPuesto { get; set; }
        public String Descripcion { get; set; }
        public String Departamento { get; set; }
    }
}
