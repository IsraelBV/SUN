using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class Prestamos
    {
        RHEntities ctx;
        public Prestamos()
        {
            ctx = new RHEntities();
        }

        /// <summary>
        /// Obtener el resumen de cuantos prestamos tiene un empleado
        /// </summary>
        /// <param name="empleado"></param>
        /// <returns></returns>
        public int GetNumPrestamosByEmpleado(int empleado)
        {
            return (from p in ctx.Prestamos
                    where p.IdEmpleadoContrato == empleado
                    select p.IdPrestamo)
                    .Count();
        }

        public List<RH.Entidades.Prestamos> GetPrestamosByEmpleado(int empleado)
        {
            Empleados emp = new Empleados();
            var contrato = emp.GetUltimoContrato(empleado);
            return ctx.Prestamos.Where(s => s.IdEmpleadoContrato == contrato.IdContrato).ToList();
        }

        public bool Create(Entidades.Prestamos prestamo)
        {
            prestamo.FechaRegistro = DateTime.Today;
            prestamo.Saldo = prestamo.MontoConInteres;
            prestamo.Status = true;

            ctx.Prestamos.Add(prestamo);
            var status = ctx.SaveChanges();

            return status > 0 ? true : false;
        }

        public List<HistorialPagos> GetHistorial(int id)
        {
            return ctx.HistorialPagos.Where(x => x.IdPrestamo == id).ToList();
        }
    }
}
