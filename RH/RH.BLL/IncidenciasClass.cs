using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class IncidenciasClass
    {
        RHEntities ctx = null;

        public IncidenciasClass()
        {
            ctx = new RHEntities();
        }

        /// <summary>
        /// Obtener lista de Incidencias
        /// </summary>
        /// <returns></returns>
        public List<C_TiposInasistencia> GetIncidencias()
        {
            var lista = ctx.C_TiposInasistencia.ToList();
            return lista;
        }

        /// <summary>
        /// Buscar Incidencias por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public C_TiposInasistencia GetIncidenciasById(int id)
        {
            var inc = ctx.C_TiposInasistencia.FirstOrDefault(x => x.IdTipoInasistencia == id);
            return inc;
        }

        /// <summary>
        /// Crear Incidencia
        /// </summary>
        /// <param name="inc"></param>
        /// <returns></returns>
        public bool CrearIncidencia(C_TiposInasistencia inc)
        {
            var result = false;

            ctx.C_TiposInasistencia.Add(inc);
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Actualizar incidencias
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inc"></param>
        /// <returns></returns>
        public bool UpdateInc(int id, C_TiposInasistencia inc)
        {
            var result = false;

            var item = ctx.C_TiposInasistencia.FirstOrDefault(x => x.IdTipoInasistencia == id);

            //Si no se encuentra registro con id se detiene la actualización
            if (item == null) return false;

            item.Clave = inc.Clave.Trim();
            item.Descripcion = inc.Descripcion.Trim();
            item.PorcentajePago = inc.PorcentajePago;
            item.TipoPago = inc.TipoPago;
            item.DerechoPago = inc.DerechoPago;
            item.Status = inc.Status;

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Eliminar una incidencia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteInc(int id)
        {
            var result = false;

            var item = ctx.C_TiposInasistencia.FirstOrDefault(x => x.IdTipoInasistencia == id);
            //si no se encontró registro con el id retornamos false
            if (item == null) return false;

            ctx.C_TiposInasistencia.Attach(item);
            ctx.C_TiposInasistencia.Remove(item);

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }
    }
}
