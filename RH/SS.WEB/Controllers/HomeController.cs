using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using Common.Helpers;
using RH.BLL;
using RH.Entidades;
using System.Web.Script.Serialization;
using Common.Enums;

namespace SS.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.SeguroSocial)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult GetErrorHttp(int id)
        {
            ViewBag.errorCode = id;
            return View();
        }

        public ActionResult AccesoDenegado()
        {
            return View();
        }

        public PartialViewResult MovimientosRecientes(int numDays)
        {
            var date = new DateTime();
            if (numDays <= 29)
                date = DateTime.Today.AddDays(-numDays);
            else
                date = DateTime.Today.AddMonths(-1);

            KardexEmpleado k = new KardexEmpleado();
            var model = k.MovimientosRecientes(date);

            Empleados emp = new Empleados();
            var lista2 = emp.GetCumpleIMSSRecientes(date);

            model.AddRange(lista2);

            //Ordenamos el modelo por fecha
            if (model.Count > 0)
            {
                //model = model.OrderByDescending(x => x.Fecha.Date).ThenByDescending(c => c.Fecha.TimeOfDay).ToList();//
                model = model.OrderByDescending(x => x.Fecha.Date).ToList();

            }

            return PartialView(model);
        }


        static List<DateTime> SortAscending(List<DateTime> list)
        {
            list.Sort((a, b) => a.CompareTo(b));
            return list;
        }

        static List<DateTime> SortDescending(List<DateTime> list)
        {
            list.Sort((a, b) => b.CompareTo(a));
            return list;
        }


        public JsonResult RecargarMovimientos(int numDays)
        {
            var date = new DateTime();
            if (numDays <= 29)
                date = DateTime.Today.AddDays(-numDays);
            else
                date = DateTime.Today.AddMonths(-1);

            KardexEmpleado k = new KardexEmpleado();
            var model = k.MovimientosRecientes(date);

            Empleados emp = new Empleados();
            var lista2 = emp.GetCumpleIMSSRecientes(date);

            model.AddRange(lista2);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}