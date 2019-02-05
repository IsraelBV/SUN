using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Nomina.Procesador.Modelos;
using Common.Enums;

namespace Nomina.Procesador.Datos
{
    public class RHDAO
    {
        private readonly RHEntities _ctx = null;

        public RHDAO()
        {
            _ctx = new RHEntities();
        }


        public int GetInasistenciasByIdEmpleado(NOM_PeriodosPago ppPago, int idE)
        {
            //Fechas Periodo
            var FPI = ppPago.Fecha_Inicio;
            var FPF = ppPago.Fecha_Fin;

            //Faltas - tipo 9
            var faltas = _ctx.Inasistencias.Where(x => x.IdTipoInasistencia == 7 && x.IdEmpleado == idE).Select(e => e.IdEmpleado).Sum();

            //Permiso sin Goce
            var ps = _ctx.Permisos.Where(x => x.ConGoce == false && x.IdEmpleado == idE).Select(e => e.IdEmpleado).Sum();

            //Incapacidad - 
            var incapacidad = _ctx.Incapacidades.Where(x => x.IdEmpleado == idE).Select(e => e.IdEmpleado).Sum();

            var totalInasistencias = faltas + ps + incapacidad;

            return totalInasistencias;

        }

        public int GetFaltasByIdEmpleado(NOM_PeriodosPago ppPago, int idE)
        {
            try
            {
                //Faltas - tipo 7
                var faltas = _ctx.Inasistencias.Where(x => x.IdTipoInasistencia == 7 && x.IdEmpleado == idE &&
                                                           x.Fecha >= ppPago.Fecha_Inicio && x.Fecha <= ppPago.Fecha_Fin).Select(e => e.IdEmpleado).Sum();
                return faltas;
            }
            catch (Exception)
            {

                return 0;
            }

        }


    }
}
