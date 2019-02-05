using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;

using Common.Helpers;

namespace RH.WEB.Controllers
{
    public class ReportesController : Controller
    {
        // GET: Reportes
        public ActionResult Index()
        {
            ViewBag.empresas = Empresas.GetEmpresasFiscales();
            ViewBag.sucursales = Sucursales.GetSucursales();

            return View();
        }

        public JsonResult ReporteEmpleados(int idEmpresa = 0, int idSucursal=0, int status= 0)
        {
            if (idEmpresa == null)
            {
                return Json(new { success = false, error = "Debe seleccionar el año del ejercicio fiscal", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            var archivoReporte = "";
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/EmpleadosA");
            var pathDescarga = "/Files/EmpleadosA/";

            archivoReporte = ReportesRh.ReporteEmpleados(idUsuario, ruta, pathDescarga, idEmpresa, idSucursal, status);

            return Json(new { success = true, error = "", resultPath = archivoReporte }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DescargarArchivo(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
    }
}