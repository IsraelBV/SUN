using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using Common.Helpers;
using FiltrosWeb;
using Common.Enums;
using RH.Entidades;

namespace SS.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.SeguroSocial)]
    public class LayoutController : Controller
    {
        Empresas empresas = new Empresas();
        // GET: Layout
        public ActionResult Index()
        {
            
            var listaEmpresas = empresas.getEmpresasByUser(Session["Usuario"] as SYA_Usuarios);

            // listaEmpresas = listaEmpresas.Where(x => x.Guia != null).ToList();

            var selectListEmpresas = listaEmpresas.Where(x => x.RegistroPatronal != null).Select(x => new SelectListItem()
            {
                Value = x.IdEmpresa.ToString(),
                Text = x.RazonSocial + " - G " + x.Guia
            }).ToList();

            var itemNew = new SelectListItem()
            {
                Value = "0",
                Text = "- Seleccione una empresa -",
                Selected = true
            };
            selectListEmpresas.Insert(0, itemNew);

            ViewBag.ListaEmpresas = selectListEmpresas;

            return View();
        }

        public JsonResult ReporteIdse(int idEmpresa ,int tipoMovimiento, DateTime fechaI, DateTime fechaF)
        {
            if (idEmpresa == null)
            {
                return Json(new { success = false, error = "Debe seleccionar el año del ejercicio fiscal", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }


            
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/Idse/");
            var pathDescarga = "/Files/Idse/";
            var archivoReporte = LayoutImss.IdseGenerarLayout(idUsuario, tipoMovimiento, idEmpresa, fechaI, fechaF, ruta,pathDescarga);
            string[] arrayRuta = archivoReporte.Split('\\');
            string nombre = arrayRuta[arrayRuta.Length - 1];
            return Json(new { success = true, error = "",name=nombre, resultPath = archivoReporte }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ReporteExcel(int idEmpresa, int tipoMovimiento, DateTime fechaI, DateTime fechaF)
        {
            if (idEmpresa == null)
            {
                return Json(new { success = false, error = "Debe seleccionar el año del ejercicio fiscal", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }


            var archivoReporte = "";
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/ReporteEx/");
            var pathDescarga = "/Files/ReporteEx/";

            archivoReporte = LayoutImss.ExcelLayout(idUsuario, tipoMovimiento, idEmpresa, fechaI, fechaF, ruta, pathDescarga);

            return Json(new { success = true, error = "", resultPath = archivoReporte }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IndexSua()
        {
            var listaEmpresas = empresas.getEmpresasByUser(Session["Usuario"] as SYA_Usuarios);

            listaEmpresas = listaEmpresas.Where(x => x.Guia != null).ToList();

            var selectListEmpresas = listaEmpresas.Select(x => new SelectListItem()
            {
                Value = x.IdEmpresa.ToString(),
                Text = x.RazonSocial + " - G " + x.Guia
            }).ToList();

            var itemNew = new SelectListItem()
            {
                Value = "0",
                Text = "- Seleccione una empresa -",
                Selected = true
            };
            selectListEmpresas.Insert(0, itemNew);

            ViewBag.ListaEmpresas = selectListEmpresas;

            return View();
        }

        public JsonResult ReporteSua(int idEmpresa, int tipoMovimiento, DateTime fechaI, DateTime fechaF)
        {
            if (idEmpresa == null)
            {
                return Json(new { success = false, error = "Debe seleccionar el año del ejercicio fiscal", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }


            var archivoReporte = "";
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/Sua/");
            var pathDescarga = "/Files/Sua/";

            archivoReporte = LayoutImss.SuaGenerarLayout(idUsuario, tipoMovimiento, idEmpresa, fechaI, fechaF, ruta, pathDescarga);
            string[] arrayRuta = archivoReporte.Split('\\');
            string nombre = arrayRuta[arrayRuta.Length - 1];
            return Json(new { success = true, error = "", name = nombre, resultPath = archivoReporte }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DescargarLayout(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;

            var rutaR = Server.MapPath("~"+ ruta);

           // var test = Server.MapPath("~");

            //var test2 = Server.MapPath("~/");

            byte[] fyleBytes = System.IO.File.ReadAllBytes(rutaR);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
    }
}