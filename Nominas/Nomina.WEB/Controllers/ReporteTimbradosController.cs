using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Helpers;
using RH.BLL;
using Nomina.BLL;

namespace Nomina.WEB.Controllers
{
    public class ReporteTimbradosController : Controller
    {
        // GET: ReporteTimbrados
        public ActionResult Index()
        {

            ViewBag.ejercicio = EjerciciosFiscales.GetEjerciciosFiscalesAsc();
            var listaEmpresas =  Empresas.GetEmpresasFiscales();
            listaEmpresas = listaEmpresas.OrderBy(x => x.RazonSocial).ToList();

            ViewBag.empresas = listaEmpresas;
            ViewBag.sucursales = null;// Sucursales.GetSucursales();

            return PartialView();
        }

        public JsonResult GenReporteTimbrados(int idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
            int idEmpresa = 0, int idCliente = 0, int idPeriodoB = 0)
        {
           

            var ruta = Server.MapPath("~/Files/ReporteTimbrados");
            var pathDescarga = "/Files/ReporteTimbrados/";

            int idUsuario = SessionHelpers.GetIdUsuario();

           var archivoTimbrados = Reporte_Timbrado.GenerarTimbrados(idUsuario, idEjercicio, dateInicial, dateFinal, ruta, pathDescarga, idEmpresa, idCliente,idPeriodoB, false);

            if (archivoTimbrados == null)
            {
                return Json(new { success = false, error = "No se encontrarón registros con esos criterios de busqueda", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, error = "", resultPath = archivoTimbrados }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GenReporteCanceladoss(int idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
            int idEmpresa = 0, int idCliente = 0, int idPeriodoB = 0)
        {
            

            var ruta = Server.MapPath("~/Files/ReporteCancelados");
            var pathDescarga = "/Files/ReporteCancelados/";

            int idUsuario = SessionHelpers.GetIdUsuario();

            var archivoTimbrados = Reporte_Timbrado.GenerarTimbrados(idUsuario, idEjercicio, dateInicial, dateFinal, ruta, pathDescarga, idEmpresa, idCliente,idPeriodoB, true);


            if (archivoTimbrados == null)
            {
                return Json(new { success = false, error = "No se encontrarón registros con esos criterios de busqueda", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, error = "", resultPath = archivoTimbrados }, JsonRequestBehavior.AllowGet);


        }

        public async Task<JsonResult> DownloadRecibos(int idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
            int idEmpresa = 0, int idCliente = 0, int idPeriodoB = 0, bool incluirPdf = false)
        {
         

            var ruta = Server.MapPath("~/Files/Reportexml");
            var pathDescarga = "/Files/Reportexml/";

            int idUsuario = SessionHelpers.GetIdUsuario();



            //DateTime dt = DateTime.Now;
            //var nombrezip = "Cfdi ";

            //nombrezip += " " + dt.Day + dt.Month + dt.Year + ".zip";

            //var arr = await _fe.DownloadRecibosCfdiAsync(idNominas, idusuario, ruta, isFiniquito: isFiniquito);

           var result= await Reporte_Timbrado.DownloadRecibosXml(idUsuario, idEjercicio, dateInicial, dateFinal, ruta, pathDescarga, idEmpresa, idCliente, idPeriodoB, incluirPdf);

            return Json(new { success = true, error = "", resultPath = result }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetVisorXmlExcel(int idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
            int idEmpresa = 0, int idCliente = 0, int idPeriodoB = 0)
        {
            

            var ruta = Server.MapPath("~/Files/ReporteTimbrados");
            var pathDescarga = "/Files/ReporteTimbrados/";

            int idUsuario = SessionHelpers.GetIdUsuario();

           
           var archivoTimbrados = Reporte_Timbrado.VisorXmlToExcel(idUsuario, idEjercicio, dateInicial, dateFinal, ruta, pathDescarga, idEmpresa, idCliente, idPeriodoB, false);

            if (archivoTimbrados == null)
            {
                return Json(new { success = false, error = "No se encontrarón registros con esos criterios de busqueda", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, error = "", resultPath = archivoTimbrados }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetClienteSucursal(int idEmpresa)
        {
            Sucursales obj = new Sucursales();

            var lista = obj.GetListaClienteSucursalByIdEmpresa(idEmpresa);

            SucursalDatos seleccionar = new SucursalDatos()
            {
                IdSucursal =0,
                Ciudad = "",
                Nombre = "Todos"
            };

            lista.Insert(0,seleccionar);

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriodosBySucursal(int id, int idEjercicio, int idEmisor)
        {
            PeriodosPago pp = new PeriodosPago();
            var listaPeriodos = pp.GetPeriodosBySucursalTimbrados(id, idEjercicio, idEmisor);

            Tuple<int, string> item0 = new Tuple<int, string>(0,"Todos -");

            listaPeriodos.Insert(0,item0);

            return Json(listaPeriodos,JsonRequestBehavior.AllowGet);

        }

    }
}