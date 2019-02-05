using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.Entidades;
using RH.BLL;
using Common.Helpers;
using FiltrosWeb;
using Common.Enums;

namespace RH.WEB.Controllers
{
    public class PermisosController : Controller
    {
        private _Permisos _permisos = new _Permisos();
        // GET: Permisos
        [AccesoModuloAttribute(Modulo = Modulos.RHPermisos, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;
            if (sucursal != null)
            {
                Plantillas pl = new Plantillas();
                ViewBag.Plantillas = pl.GetPlantillasByTipo2((int)TipoPlantilla.Permisos, sucursal.IdCliente);
                var empleados = _permisos.ObtenerEmpleadosPorSucursal(sucursal.IdSucursal);
                return View(empleados);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        [AccesoModuloAttribute(Modulo = Modulos.RHPermisos, Accion = AccionCrud.Agregar)]
        public JsonResult CrearPermiso(Permisos collection, int XHoras = 0, int XGoce = 0)
        {
            if (XHoras == 1)
            { collection.PorHoras = true; }
            else
            { collection.PorHoras = false; }

            if (XGoce == 1)
            { collection.ConGoce = true; }
            else
            { collection.ConGoce = false; }

            if (collection.Dias == null)
            { collection.Dias = 0; }

            if (collection.Horas == null)
            { collection.Horas = 0; }
            collection.Status = true;
            var result = _permisos.CrearPermiso(collection);

            return Json(new { resultado = result }, JsonRequestBehavior.AllowGet);
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHPermisos, Accion = AccionCrud.Consultar)]
        public ActionResult FormPermiso (int id)
        {
            var datospermiso = _permisos.datosempleado(id);
            return PartialView("_Permiso", datospermiso);
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHPermisos, Accion = AccionCrud.Consultar)]
        public ActionResult HistorialPermisos (int id)
        {
            var vistapermiso = _permisos.HistorialPermiso(id);
            return PartialView("_HistorialPermisos", vistapermiso);
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHPermisos, Accion = AccionCrud.Agregar)]
        public string GenerarPlantillaContrato(int idPlantilla, int[] empleados )
        {
            Plantillas p = new Plantillas();
            return p.FormarPlantilla(idPlantilla, (int)TipoPlantilla.Permisos, empleados);
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHPermisos, Accion = AccionCrud.Consultar)]
        public FileResult GetPlantillaPermiso(string path)
        {
            var file = System.IO.File.ReadAllBytes(path);
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "Permisos.zip");
        }

    }
}