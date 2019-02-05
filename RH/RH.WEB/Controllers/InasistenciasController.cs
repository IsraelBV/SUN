using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;
using Common.Enums;
using FiltrosWeb;


namespace RH.WEB.Controllers
{
    public class InasistenciasController : Controller
    {
        // GET: Inasistencias
        public ActionResult Index()
        {
            if (Session["sucursal"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var sucursal = Session["sucursal"] as SucursalDatos;

            Empleados emp = new Empleados();
            var modelo = emp.GetEmpleadosBySucursal(sucursal.IdSucursal);

            IncidenciasClass incidencias = new IncidenciasClass();
            ViewBag.TiposInasistencias = incidencias.GetIncidencias();

            return View(modelo);
        }
    }
}