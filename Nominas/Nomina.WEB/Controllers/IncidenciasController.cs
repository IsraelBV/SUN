using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.Entidades;
using RH.BLL;
using Nomina.BLL;
using Nomina.WEB.Filtros;
using Common.Enums;
using Common.Helpers;
using Nomina.Reportes;
using FiltrosWeb;
using System.Data;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [SesionPeriodo]
    [SesionSucursal]
    public class IncidenciasController : Controller
    {
        public PartialViewResult Index()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            _Incidencias inc = new _Incidencias();
            var incidencias = inc.GetIncidenciasByPeriodo2(periodo);

            IncidenciasClass ic = new IncidenciasClass();
            ViewBag.catalogo = inc.catalagoIna();
            ViewBag.catInc = ic.GetIncidencias();
            ViewBag.autorizado = periodo.Autorizado;
            return PartialView(incidencias);
        }

        [HttpPost]
        public JsonResult CambiarTIpo(string [] Array, string Tipo)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            _Incidencias inc = new _Incidencias();
            var IdUser = SessionHelpers.GetIdUsuario();
            inc.cambiarIncidencias(Array, Tipo,periodo.IdPeriodoPago,IdUser);

            return Json("succes", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ImprimirPlantillaIncidencia()
        {
            _Incidencias inc = new _Incidencias();            
            ImportacionIncidencias inci = new ImportacionIncidencias();            
            var periodo = Session["periodo"] as NOM_PeriodosPago;            
            var incidencias = inc.GetIncidenciasByPeriodo2(periodo);            
            var idusuario = SessionHelpers.GetIdUsuario();
            //int IdSucursal = sucursal.IdSucursal;
            var ruta = Server.MapPath("~/Files/Incidencias");
            var resultado = inci.crearImportacionIncidencias(idusuario,ruta,periodo.IdPeriodoPago,incidencias);
            //var file = System.IO.File.ReadAllBytes(resultado);
            //Regresa el archivo
            //return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "Incidencia.xlsx");
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public FileResult descargarPlantilla(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var data = new System.Data.DataTable();
            ImportacionIncidencias import = new ImportacionIncidencias();

            data = import.ExcelToDataTable(file);
           var dat = import.guardarRegistro(data,periodo.IdPeriodoPago);
            return Json("succes", JsonRequestBehavior.AllowGet); ;
        }

       

    }

}