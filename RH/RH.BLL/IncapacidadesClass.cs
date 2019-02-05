using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.BLL
{
    public class IncapacidadesClass
    {
        RHEntities ctx = null;
        public IncapacidadesClass()
        {
            ctx = new RHEntities();
        }


        /// <summary>
        /// Obtener lista de Empleados
        /// </summary>
        /// <returns></returns>

        public List<Empleado> GetEmpleados()
        {
            var lista = ctx.Empleado.ToList();

            return lista;
        }

        /// <summary>
        /// Obtener lista de Incapacidades
        /// </summary>
        /// <returns></returns>
        
        public List<Incapacidades> GetIncapacidades()
        {
            var lista = ctx.Incapacidades.ToList();

            return lista;
        }


        /// <summary>
        /// Buscar empleado por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Empleado GetEmpleadosById(int id)
        {
            var emp = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == id);

            return emp;
        }


        /// <summary>
        /// Buscar incapacidades por ID de empleado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Incapacidades> GetIncapacidadById(int id)
        {
            return ctx.Incapacidades.Where(x => x.IdEmpleado == id).ToList();
            
        }

        public Incapacidades GetIncapacidad(int id)
        {
            return ctx.Incapacidades.FirstOrDefault(x => x.Id == id);
        }


        /// <summary>
        /// Crear Incapacidad
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CrearIncapacidad(Incapacidades inc)
        {
            var noti = new Notificaciones();
            var result = false;
            if(inc.Tipo == "Riesgo de trabajo")
            {
                inc.IdIncapacidadesSat = 1;
            }else if (inc.Tipo == "Enfermedad General")
            {
                inc.IdIncapacidadesSat = 2;
            }else if (inc.Tipo == "Prematernal" || inc.Tipo == "Maternal de enlace" || inc.Tipo == "Postmaternal")
            {
                inc.IdIncapacidadesSat = 3;
            }

            using (var context = new RHEntities())
            {
                //buscamos el contrato actual
                var itemContrato =
                    context.Empleado_Contrato.Where(x => x.Status == true)
                        .FirstOrDefault(x => x.IdEmpleado == inc.IdEmpleado);

                if (itemContrato != null)
                {
                    inc.IdContrato = itemContrato.IdContrato;
                    inc.IdSucursal = itemContrato.IdSucursal;
                    inc.IdEmpresaFiscal = itemContrato.IdEmpresaFiscal ?? 0;
                    inc.FechaReg = DateTime.Now;
                    inc.Sdi = itemContrato.SDI;

                    context.Incapacidades.Add(inc);
                    var r = context.SaveChanges();

                    if (r > 0)
                    {
                        result = true;
                        noti.Incapacidad(inc);
                    }
                }
            }
            

           
            return result;
        }

        /// <summary>
        /// Actualizar datos Incapacidad
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bn"></param>
        /// <returns></returns>
        public bool UpdateInc(int id, Incapacidades inc)
            {
            var result = false;

            var item = ctx.Incapacidades.FirstOrDefault(x => x.Id == id);

            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;

            item.Folio = inc.Folio.Trim();
            item.Tipo = inc.Tipo;
            item.Clase = inc.Clase;
            item.FechaInicio = inc.FechaInicio;
            item.FechaFin = inc.FechaFin;
            item.Dias = inc.Dias;
            item.Observaciones = inc.Observaciones.Trim();

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Eliminar una Incapacidad
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool DeleteInc(int id)
        {
            var result = false;

            var item = ctx.Incapacidades.FirstOrDefault(x => x.IdEmpleado == id);

            //si no se encontro un registro con ese id,
            // retornamos false 
            if (item == null) return false;

            ctx.Incapacidades.Attach(item);
            ctx.Incapacidades.Remove(item);

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

    }

}
