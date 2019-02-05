using Common.Enums;
using RH.BLL;
using FiltrosWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RH.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.Rh)]
    public class PeriodoVacacionesController : Controller
    {
        private PeriodosVacaciones periodo = new PeriodosVacaciones();
        //GET: PeriodoVacaciones
        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Consultar)]
        public ActionResult _PeriodoVacaciones(int id)
        {

            return View(periodo.GetPeriodo(id));
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Agregar)]

        [HttpPost]
        public ActionResult CrearPeriodo(int id, DateTime real, int sucursal, int IdEmpleado)
        {


          var crear =  periodo.CrearPeriodo(id, real, IdEmpleado);
            if (crear == true)
            {
                int mensaje = 1;
                //return Redirect("~/Vacaciones/VacacionesMenu/" + IdEmpleado);
                return RedirectToAction("VacacionesMenu", "Vacaciones", new { id = IdEmpleado, mensaje1 = mensaje });
            }
            else
            {
                int mensaje = 0;
                //return Redirect("~/Vacaciones/VacacionesMenu/" + IdEmpleado);
                return RedirectToAction("VacacionesMenu", "Vacaciones", new { id= IdEmpleado,mensaje1 = mensaje });
            }
        
         
        }
    }
}