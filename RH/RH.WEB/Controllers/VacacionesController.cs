using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.Entidades;
using RH.BLL;
using Common.Helpers;
using FiltrosWeb;
using Common.Enums;
using Common.Utils;
//using Novacode;

namespace RH.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.Rh)]
    public class VacacionesController : Controller
    {
        private VacacionesClase vaca = new VacacionesClase();
        // GET: Vacaciones
        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {

            var sucursal = Session["Sucursal"] as SucursalDatos;
            if (sucursal != null)
            {
                var empleados = vaca.ObtenerEmpleadosPorSucursal(sucursal.IdSucursal);
                Plantillas pl = new Plantillas();
                ViewBag.Plantillas = pl.GetPlantillasByTipo2((int)TipoPlantilla.Vacaciones, sucursal.IdCliente);
                return View(empleados);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Consultar)]
        public ActionResult VacacionesMenu(int id, int mensaje1 = 2)
        {
            ViewBag.resultado = mensaje1;
            return View(vaca.GetDatosById(id));
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Consultar)]
        public ActionResult ViewDetails(int idContrato = 0)
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;

            var detalle = vaca.GetPeriodoVacacionesV2(idContrato);
                //var detalle = vaca.GetPeriodoVacaciones(id);
                Plantillas pl = new Plantillas();
                ViewBag.Plantillas = pl.GetPlantillasByTipo2((int)TipoPlantilla.Vacaciones, sucursal.IdCliente);
                //ViewBag.vaciones = vaca.GetVacaciones(id=10);

                return PartialView("_PeriodoVacaciones", detalle);
            
        }

        public ActionResult submenuperiodo(int id, int idcontrato)
        {
            var detalle = vaca.GetVacaciones(id);
            ViewBag.dias = vaca.GetPeriodoVacaciones(idcontrato);

            return PartialView("_submenuperiodo", detalle);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Consultar)]
        public ActionResult FormVacas(int id, int idEmpleado, int idemp)
        {
            var detalle = vaca.GetPeriodoVacacionesById(id);
            ViewBag.vacaciones = vaca.GetVacaciones(id);
            ViewBag.descanso = vaca.DiaDescanso(idemp);
            ViewBag.IdEmpleado = idEmpleado;
            ViewBag.idemp = idemp;
            return View("_FormularioVacaciones", detalle);
        }
        [HttpPost]
        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Agregar)]
        public JsonResult CrearVacaciones(Vacaciones collection, int idperiodo, int idemp)

        {
            collection.Id = idemp;
            collection.IdPeridoVacaciones = idperiodo;
            collection.Usuario = SessionHelpers.GetIdUsuario();
            collection.FechaReg = DateTime.Now;
          var result =  vaca.CrearVacaciones(collection);

            //return RedirectToAction("VacacionesMenu/" + idemp);
            return Json(new { resultado = result }, JsonRequestBehavior.AllowGet);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Consultar)]
        public ActionResult HistorialVacaciones(int id)

        {
            var idContrato = vaca.ObtenerIdContrato(id);
            var historial = vaca.Historial(idContrato.IdEmpContrato);
            return PartialView("_HistorialVacaciones", historial);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHVacaciones, Accion = AccionCrud.Consultar)]
        public ActionResult HistorialByPeriodo(int id ,int idcontrato,int Diastomados, int DiasTotales)

        {
            var historial = vaca.HistoriaByPeriodo(id);
            ViewBag.idcontrato = idcontrato;
            ViewBag.DiasTomados = Diastomados;
            ViewBag.DiasTotales = DiasTotales;
            return PartialView("_HistorialPeriodo", historial);
        }

    
        public ActionResult Historial(int id = 0)

        {
            var historial = vaca.HistoriaByPeriodo(id);

            return PartialView("_HistorialPeriodo", historial);
        }

        
        public string GenerarPlantillaVacaciones(int idPlantilla, int[] vacaciones)
        {
            Plantillas p = new Plantillas();
            return p.FormarPlantilla(idPlantilla, (int)TipoPlantilla.Vacaciones, vacaciones);
        }

        public FileResult GetPlantillaVacaciones(string path)
        {
            var file = System.IO.File.ReadAllBytes(path);
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "Vacaciones.zip");
        }

    }
}