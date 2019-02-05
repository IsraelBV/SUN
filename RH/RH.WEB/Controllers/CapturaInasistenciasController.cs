using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;
using Common.Enums;
using FiltrosWeb;
using Common.Helpers;

namespace RH.WEB.Controllers
{
    public class CapturaInasistenciasController : Controller
    {
        [AccesoModuloAttribute(Modulo = Modulos.RHInasistencias, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            if (sucursal == null)
                return RedirectToAction("Index", "Home");
            Empleados e = new Empleados();
            var model = e.GetEmpleadosBySucursal(sucursal.IdSucursal);

            IncidenciasClass i = new IncidenciasClass();
            ViewBag.TiposInasistencias = i.GetIncidencias();

            return View(model);
        }

        [HttpPost]
        public JsonResult Nueva(int[] Empleados, DateTime Fecha, int IdTipoInasistencia, int Dias, DateTime? FechaFin = null)
        {
            _Inasistencias ctx = new _Inasistencias();
            List<Inasistencias> lista = new List<Inasistencias>();
            if (FechaFin < Fecha || FechaFin == null) FechaFin = Fecha;
            foreach(var item in Empleados)
            {
                Inasistencias inasistencia = new Inasistencias();
                inasistencia.Fecha = Fecha;
                inasistencia.FechaFin = FechaFin;
                inasistencia.IdTipoInasistencia = IdTipoInasistencia;
                inasistencia.IdEmpleado = item;
                inasistencia.Dias = Dias;
                lista.Add(inasistencia);
            }
            var result = ctx.CapturaMasiva(lista);
            TempData["duplicados"] = result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegistrarInasistencia(string[] fechas, int[] idEmpleados, int IdTipoInasistencia)
        {
            _Inasistencias ctx = new _Inasistencias();
            var IdUser = SessionHelpers.GetIdUsuario();
            var result = ctx.GuardarInasistencias(idEmpleados, fechas, IdTipoInasistencia,IdUser);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GuardarDuplicados()
        {
            var duplicados = TempData["duplicados"] as List<IncidenciasDuplicadas>;
            if (duplicados == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            var inasistencias = new List<Inasistencias>();
            foreach(var item in duplicados)
            {
                var inasistencia = new Inasistencias();
                inasistencia.IdEmpleado = item.IdEmpleado;
                inasistencia.Fecha = item.FechaIncidencia;
                inasistencia.FechaFin = item.FechaFinIncidencia;
                inasistencia.Dias = item.Dias;
                inasistencia.IdTipoInasistencia = item.IdTipoInasistencia;
                inasistencias.Add(inasistencia);
            }
            _Inasistencias ins = new _Inasistencias();
            var result = ins.CapturaMasivaSinComprobasion(inasistencias);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detalle(int id)
        {
            _Inasistencias ins = new _Inasistencias();
            var model = ins.GetDetail(id);

            var e = new Empleados();
            ViewBag.Empleado = e.GetEmpleadoById(id);
            return View(model);
        }

        public JsonResult Delete(int id)
        {
            _Inasistencias ins = new _Inasistencias();
            var result = ins.DeleteInasistencia(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}