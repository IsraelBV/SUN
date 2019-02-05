using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.Entidades;
using Nomina.BLL;
using System.Threading.Tasks;
using Nomina.WEB.Models;
using RH.Entidades.GlobalModel;
using Common.Helpers;
using FiltrosWeb;
using Nomina.WEB.Filtros;
using Common.Utils;


namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [SesionPeriodo]
    [SesionSucursal]
    public class FiniquitoController : Controller
    {
        // GET: Finiquito
        public ActionResult Index()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            PeriodosPago pdo = new PeriodosPago();
            finiquitosClass fini = new finiquitosClass();
            //var idemp = pdo.GetIdEmpleados(periodo.IdPeriodoPago);
            var emp = fini.EmpleadoPeriodoById(periodo.IdPeriodoPago);
            finiquitosClass finiquito = new finiquitosClass();


            var fin = finiquito.FiniquitoFiscal(periodo.IdPeriodoPago);
            ViewBag.empleado = emp;
            ViewBag.esLiquidacion = false;
            return PartialView(fin);
        }

        public async Task<JsonResult> finiquito(int idEmpleado, int idFiniquito, ParametrosFiniquitos arrayF, bool calcularLiquidacion)
        {
            finiquitosClass finiq = new finiquitosClass();

            var periodo = Session["periodo"] as NOM_PeriodosPago;

            //idFiniquito = await finiquitosClass.ProcesarFiniquitoAsync(periodo, idEmpleado, false);

            idFiniquito = await ProcesoNomina.ProcesarFiniquitoIndemnizacionAsync(periodo.IdPeriodoPago, periodo.IdEjercicio, idEmpleado, periodo.IdSucursal, arrayF, calcularLiquidacion);

            //var fin = finiq.FiniquitoFiscal(periodo.IdPeriodoPago);
            ViewBag.esLiquidacion = calcularLiquidacion;

            return Json(new { status = "OK - Fin Procesado de nominas", idFiniquito = idFiniquito });
        }

        public ActionResult FiniquitoFiscal()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            PeriodosPago pdo = new PeriodosPago();
            finiquitosClass fini = new finiquitosClass();
            //var idemp = pdo.GetIdEmpleados(periodo.IdPeriodoPago);
            var emp = fini.EmpleadoPeriodoById(periodo.IdPeriodoPago);
            finiquitosClass finiquito = new finiquitosClass();


            var fin = finiquito.FiniquitoFiscal(periodo.IdPeriodoPago);
            if (fin != null)
            {
                var finDet = finiquito.FiniquitoDetalle(fin.IdFiniquito);
                ViewBag.finDet = finDet;
            }
            else
            {
                var finDet = finiquito.FiniquitoDetalle(0);
                ViewBag.finDet = finDet;
            }


            ViewBag.empleado = emp;

            return PartialView(fin);
        }
        public ActionResult FiniquitoComplemento()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            PeriodosPago pdo = new PeriodosPago();
            finiquitosClass fini = new finiquitosClass();
            //var idemp = pdo.GetIdEmpleados(periodo.IdPeriodoPago);
            var emp = fini.EmpleadoPeriodoById(periodo.IdPeriodoPago);
            finiquitosClass finiquito = new finiquitosClass();


            var fin = finiquito.FiniquitoFiscal(periodo.IdPeriodoPago);
            var finCom = fin == null ? null : finiquito.FiniquitoComplemento(fin.IdFiniquito);
            var finDet = fin == null ? null : finiquito.FiniquitoDetalle(fin.IdFiniquito);

            ViewBag.finCom = finCom;
            ViewBag.finDet = finDet;

            ViewBag.empleado = emp;
            return PartialView(fin);
        }
        public ActionResult DescuentoFiscal(int idFiniquito = 0)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            finiquitosClass finiquito = new finiquitosClass();
            var conceptos = finiquito.ListaConceptosDescuentos();
            var finDesFis = finiquito.DescuentoFiscal_Complemento(periodo.IdPeriodoPago);
            ViewBag.idFiniquito = idFiniquito;
            ViewBag.conceptos = conceptos;
            return PartialView(finDesFis);
        }

        [HttpPost]
        public async Task<JsonResult> SaveCustomData()
        {
            int idFiniquito = 0;
            try
            {
                string strTotal3MesesF = Request.Form["totalTresMesesF"];
                string strTotal3MesesC = Request.Form["totalTresMesesC"];

                 idFiniquito = Convert.ToInt32(Request.Form["idFiniquito"]);

               double total3mesesF = Utils.ConvertToDouble(strTotal3MesesF);
               double total3mesesC = Utils.ConvertToDouble(strTotal3MesesC);

                //string[] keys = Request.Form.AllKeys;
                //for (int i = 0; i < keys.Length; i++)
                //{             
                //}

                var periodo = Session["periodo"] as NOM_PeriodosPago;

                idFiniquito = await ProcesoNomina.ProcesarFiniquitoTotalesPersonalizadoAsync(0, 0);
                //idFiniquito = await ProcesoNomina.ProcesarFiniquitoTotalesPersonalizadoAsync(periodo.IdPeriodoPago, idFiniquito);

                return Json(new { strMensaje = "OK - Procesado de Finiquito", status = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { strMensaje = ex.Message, status = 0 });
            }


        }

        [HttpPost]
        public JsonResult GuardarDescuentos(List<NOM_Finiquito_Descuento_Adicional> arrayDes, List<NOM_Finiquito_Descuento_Adicional> arrayDesC)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            finiquitosClass finiquito = new finiquitosClass();
            var resultado = finiquito.GuardarDescuentos(arrayDes, arrayDesC, periodo.IdPeriodoPago);
            return Json(new { resultado = "Se guardo Correctamente" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<FileResult> GetRecibos(int idFiniquito = 0)
        {
            if (idFiniquito == 0) return null;

            // int[] nominas = new[] {1,2,3,4,5,6};

            Random random = new Random();
            int randomNumber = random.Next(0, 1000);//A

            var ruta = Server.MapPath("~/Files/DownloadRecibos");
            var idusuario = SessionHelpers.GetIdUsuario();
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (periodoPago != null)
            {
                //Genera el xml
                var xml = await FacturaElectronica.GenerarXMLFiniquitoSintimbre(idFiniquito, periodoPago.IdEjercicio, periodoPago.IdPeriodoPago, idusuario);

                //Crear el pdf con el xml generado
                var recibo = await ProcesoNomina.GetRecibosFiniquitoSinTimbre(idFiniquito, periodoPago, idusuario, ruta);

                var file = System.IO.File.ReadAllBytes(recibo);

                var nombreArchivo = periodoPago.Descripcion + ".pdf";

                return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);
            }
            else
            {
                return null;
            }
        }

        //RECIBOS DE PDF - FISCAL - REAL

        public FileResult GetReciboFiscal(int idFiniquito = 0, bool liquidacion = false)
        {
            int idPeriodo = 0;
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (periodoPago != null)
            {
                idPeriodo = periodoPago.IdPeriodoPago;
            }

            finiquitosClass fc = new finiquitosClass();


            var pdfBytes = fc.GetReciboFiscal(idFiniquito, idPeriodo, 0, "", liquidacion);

            //header('Content-Disposition: attachment; filename="name_of_excel_file.xls"');
            //Regresa el archivo
            // System.Net.Mime.MediaTypeNames.Application.
            var nombreArchivo = periodoPago.Descripcion + "_SA.pdf";

            return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);
        }

        public FileResult GetReciboComplemento(int idFiniquito = 0, bool liquidacion = false)
        {
            int idPeriodo = 0;
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (periodoPago != null)
            {
                idPeriodo = periodoPago.IdPeriodoPago;
            }

            finiquitosClass fc = new finiquitosClass();


            var pdfBytes = fc.GetReciboReal(idFiniquito, idPeriodo, 0, "", liquidacion);


            var nombreArchivo = periodoPago.Descripcion + "_CO.pdf";

            return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);
        }
    }


}