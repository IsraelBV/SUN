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
using Nomina.Reportes;
using Common.Utils;
using Common.Enums;
using RH.BLL;
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
           // var periodo = Session["periodo"] as NOM_PeriodosPago;
           //// PeriodosPago pdo = new PeriodosPago();
           // finiquitosClass fini = new finiquitosClass();

           // //var idemp = pdo.GetIdEmpleados(periodo.IdPeriodoPago);
           // var emp = fini.EmpleadoPeriodoById(periodo.IdPeriodoPago);
           // finiquitosClass finiquito = new finiquitosClass();
           // var fin = finiquito.FiniquitoFiscal(periodo.IdPeriodoPago);

           // var listaTarifas = Utils.CreateSelectedFromEnum(typeof(Tarifas), fin.TipoTarifa);

           // ViewBag.ListaTarifas = listaTarifas;
           // ViewBag.Autorizado = periodo.Autorizado;
           // ViewBag.empleado = emp;
           // ViewBag.esLiquidacion = false;
            return PartialView();
        }

        public async Task<JsonResult> finiquito(int idEmpleado, int idFiniquito, ParametrosFiniquitos arrayF, bool calcularLiquidacion,bool paramArt174)
        {
            //finiquitosClass finiq = new finiquitosClass();

            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var sucursal = Session["sucursal"] as SucursalDatos;
            //idFiniquito = await finiquitosClass.ProcesarFiniquitoAsync(periodo, idEmpleado, false);
            int idUsuario = SessionHelpers.GetIdUsuario();

            idFiniquito = await ProcesoNomina.ProcesarFiniquitoIndemnizacionAsync(periodo.IdPeriodoPago, periodo.IdEjercicio, idEmpleado, sucursal.IdCliente, periodo.IdSucursal, arrayF, idUsuario,calcularLiquidacion, null, isArt174: paramArt174);

            //var fin = finiq.FiniquitoFiscal(periodo.IdPeriodoPago);
            ViewBag.esLiquidacion = calcularLiquidacion;
            

            return Json(new { status = "OK - Fin Procesado de nominas", idFiniquito = idFiniquito });
        }

        public ActionResult FiniquitoFiscal()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            //PeriodosPago pdo = new PeriodosPago();
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
            //PeriodosPago pdo = new PeriodosPago();
            finiquitosClass fini = new finiquitosClass();
            //var idemp = pdo.GetIdEmpleados(periodo.IdPeriodoPago);
            var emp = fini.EmpleadoPeriodoById(periodo.IdPeriodoPago);
            finiquitosClass finiquito = new finiquitosClass();
            var listaTarifas = new  List<SelectListItem>();

            var fin = finiquito.FiniquitoFiscal(periodo.IdPeriodoPago);
            var finCom = fin == null ? null : finiquito.FiniquitoComplemento(fin.IdFiniquito);
            var finDet = fin == null ? null : finiquito.FiniquitoDetalle(fin.IdFiniquito);

            //si la variable fin es null se toma el valor 5 como default - tarifa mensual
            listaTarifas = Utils.CreateSelectedFromEnum(typeof(Tarifas), fin?.TipoTarifa ?? 5);


            ViewBag.ListaTarifas = listaTarifas;

            ViewBag.Autorizado = periodo.Autorizado;
            ViewBag.finCom = finCom;
            ViewBag.finDet = finDet;

            ViewBag.empleado = emp;
            return PartialView(fin);
        }
        public JsonResult Autorizar()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var idusuario = SessionHelpers.GetIdUsuario();
            finiquitosClass aut = new finiquitosClass();
            var resultado = aut.Autorizacion(periodo.IdPeriodoPago);
            if (resultado == true)
            {
                periodo.Autorizado = true;
                Session["periodo"] = periodo;
            }
            return Json(new { result = resultado }, JsonRequestBehavior.AllowGet);
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
        public ActionResult PercepcionesExtras(int idFiniquito = 0)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            finiquitosClass finiquito = new finiquitosClass();
            var conceptos = finiquito.ListaConceptosPercepcionesExtras();
            var finDesFis = finiquito.DescuentoFiscal_PercepcionesExtas(periodo.IdPeriodoPago);
            ViewBag.idFiniquito = idFiniquito;
            ViewBag.conceptos = conceptos;
            return PartialView(finDesFis);
        }

        [HttpPost]
        public async Task<JsonResult> SaveCustomData()
        {
            int idFiniquito = 0;
            int idEmpleado = 0;
            bool esLiquidacion = false;
            bool isArt174 = false;
            ParametrosFiniquitos arrayF = null;
            try
            {
                string strTotal3MesesF = Request.Form["totalTresMesesF"];
                string strTotal3MesesC = Request.Form["totalTresMesesC"];

                string strTotal20DiasF = Request.Form["totalVeinteF"];
                string strTotal20DiasC = Request.Form["totalVeinteC"];

                string strTotalPrimaF = Request.Form["totalPrimaF"];
                string strTotalPrimaC = Request.Form["totalPrimaC"];

                idFiniquito = Convert.ToInt32(Request.Form["idFiniquito"]);
                idEmpleado = Convert.ToInt32(Request.Form["idEmpleado"]);
                esLiquidacion = Convert.ToBoolean(Request.Form["esLiquidacion"]);

                isArt174 = Convert.ToBoolean(Request.Form["esArt174"]);

                string strTotalPrimaVacF = Request.Form["totalPrimaVacacionesF"];
                string strTotalPrimaVacC = Request.Form["totalPrimaVacacionesC"];

                if (idFiniquito <= 0)
                {
                    return Json(new { strMensaje = "Es necesario procesar el finiquito para generar sus datos.", status = 1 });
                }

                //if es null o vacio retorna -1
                decimal total3mesesF = Utils.ConvertToDecimal(strTotal3MesesF);
                decimal total3mesesC = Utils.ConvertToDecimal(strTotal3MesesC);

                decimal total20DiasF = Utils.ConvertToDecimal(strTotal20DiasF);
                decimal total20DiasC = Utils.ConvertToDecimal(strTotal20DiasC);

                decimal totalPrimaF = Utils.ConvertToDecimal(strTotalPrimaF);
                decimal totalPrimaC = Utils.ConvertToDecimal(strTotalPrimaC);

                decimal totalPrimaVacF = Utils.ConvertToDecimal(strTotalPrimaVacF);
                decimal totalPrimaVacC = Utils.ConvertToDecimal(strTotalPrimaVacC);

                //string[] keys = Request.Form.AllKeys;
                //for (int i = 0; i < keys.Length; i++)
                //{             
                //}

                //Validar el finiquito guardado si existen cambios en sus totales antes de volver a procesar

                var periodo = Session["periodo"] as NOM_PeriodosPago;
                var sucursal = Session["sucursal"] as SucursalDatos;

                int idUsuario = SessionHelpers.GetIdUsuario();

                TotalPersonalizablesFiniquitos customTotal = new TotalPersonalizablesFiniquitos();
                customTotal.TotalTresMesesFiscalPersonalizado = total3mesesF;
                customTotal.TotalTresMesesCompPersonalizado = total3mesesC;
                customTotal.TotalVeinteDiasFiscalPersonalizado = total20DiasF;
                customTotal.TotalVienteDiasCompPersonalizado = total20DiasC;
                customTotal.TotalPrimaFiscalPersonalizado = totalPrimaF;
                customTotal.TotalPrimaCompPersonalizado = totalPrimaC;
                customTotal.TotalPrimaVacPersonalizado = totalPrimaVacF;
                customTotal.TotalPrimaVacCompPersonalizado = totalPrimaVacC;

                ParametrosFiniquitos arrayFF = new ParametrosFiniquitos();

                arrayFF.FechaBajaF = Convert.ToDateTime(Request.Form["fechaBaja"]);
                arrayFF.FechaAltaF = Convert.ToDateTime(Request.Form["fechaAltaF"]);
                arrayFF.FechaAguinaldoF = Convert.ToDateTime(Request.Form["fechaAguinaldoF"]);
                arrayFF.FechaVacacionesF = Convert.ToDateTime(Request.Form["fechaVacacionesF"]); 
                arrayFF.FechaAguinaldoC = Convert.ToDateTime(Request.Form["fechaAguinaldo"]); 
                arrayFF.FechaVacacionesC = Convert.ToDateTime(Request.Form["fechaVacaciones"]); 
                arrayFF.DiasSueldoPendientesF = Utils.ConvertToDecimal(Request.Form["diasSueldoPendienteF"].ToString());

                arrayFF.DiasVacacionesPendientesF = Utils.ConvertToDecimal(Request.Form["diasVacacionesPendientesF"]);
                arrayFF.DiasSueldoPendientesC = Utils.ConvertToDecimal(Request.Form["diasSueldoPendiente"]); 
                arrayFF.DiasVacacionesPendientesC = Utils.ConvertToDecimal(Request.Form["diasVacacionesPendientes"]);

                arrayFF.MesesSalarioF = Utils.ConvertToDecimal(Request.Form["mesesSalarioF"]); 
                arrayFF.MesesSalarioC = Utils.ConvertToDecimal(Request.Form["mesesSalarioC"]); 
                arrayFF.VeinteDiasPorAF = Utils.ConvertToDecimal(Request.Form["veinteXAF"]); 
                arrayFF.VeinteDiasPorAC = Utils.ConvertToDecimal(Request.Form["veinteXAC"]);

                arrayFF.DiasVacCorrespondientesF = Utils.ConvertToInt(Request.Form["diasVacacionesF"]);
                arrayFF.DiasVacCorrespondientesC = Utils.ConvertToInt(Request.Form["diasVacacionesC"]);

                arrayFF.PorcentajePimaVacPendienteF = Utils.ConvertToDecimal(Request.Form["primaVacacionalPendienteF"]);
                arrayFF.PorcentajePimaVacPendienteC = Utils.ConvertToDecimal(Request.Form["primaVacacionalPendienteC"]);

                arrayFF.TipoTarifa = Utils.ConvertToInt(Request.Form["selectTarifa"]);

                arrayFF.DiasAguinaldoF = Utils.ConvertToInt(Request.Form["diasAguinaldoF"]);
                arrayFF.DiasAguinaldoC = Utils.ConvertToInt(Request.Form["diasAguinaldoC"]);


                idFiniquito = await ProcesoNomina.ProcesarFiniquitoIndemnizacionAsync(periodo.IdPeriodoPago, periodo.IdEjercicio, idEmpleado, sucursal.IdCliente, periodo.IdSucursal, arrayFF, idUsuario, esLiquidacion, customTotal, isArt174);

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
            var strMsg = "Se guardo Correctamente";
            if (resultado == false)
            {
                strMsg = "No se guardo este dato";
            }
            return Json(new { resultado = strMsg }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardarPercepcionesExtas(List<NOM_Finiquito_Descuento_Adicional> arrayDes, List<NOM_Finiquito_Descuento_Adicional> arrayDesC)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            finiquitosClass finiquito = new finiquitosClass();
            var resultado = finiquito.GuardarOtrasPercepciones(arrayDes, arrayDesC, periodo.IdPeriodoPago);
            var strMsg = "Se guardo Correctamente";
            if (resultado == false)
            {
                strMsg = "No se guardo este dato";
            }

            return Json(new { resultado = strMsg }, JsonRequestBehavior.AllowGet);
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
                var xml = await FacturaElectronica.GenerarXMLFiniquitoSintimbre(idFiniquito, periodoPago, periodoPago.IdEjercicio, periodoPago.IdPeriodoPago, idusuario);

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
        public async Task<FileResult> GetRecibos33(int idFiniquito = 0)
        {
            if (idFiniquito == 0) return null;

            // int[] nominas = new[] {1,2,3,4,5,6};

            //Random random = new Random();
            //int randomNumber = random.Next(0, 1000);//A

            var ruta = Server.MapPath("~/Files/DownloadRecibos");
            var idusuario = SessionHelpers.GetIdUsuario();
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (periodoPago != null)
            {
                //Genera el xml
                var xml = await FacturaElectronica.GenerarXMLFiniquitoSintimbre(idFiniquito, periodoPago, periodoPago.IdEjercicio, periodoPago.IdPeriodoPago, idusuario,isCfdi33:true);

                //Crear el pdf con el xml generado
                var recibo = await ProcesoNomina.GetRecibosFiniquitoSinTimbre(idFiniquito, periodoPago, idusuario, ruta, isCfdi33:true);

                var file = System.IO.File.ReadAllBytes(recibo);

                var nombreArchivo = periodoPago.Descripcion + ".pdf";

                return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);
            }
            else
            {
                return null;
            }
        }

        public ActionResult autorizarFiniquito()
        {
            finiquitosClass fini = new finiquitosClass();
            var periodo = Session["periodo"] as NOM_PeriodosPago;

            var factura = fini.facturaF(periodo.IdPeriodoPago);
            var facturaC = fini.facturaC(periodo.IdPeriodoPago);

            if (facturaC == null)
            {
                ViewBag.facturaC = null;
            }
            else
            {
                ViewBag.facturaC = facturaC;
            }


            return PartialView(factura);
        }
        public JsonResult GuardarFactura(NOM_FacturacionC_Finiquito factura)
        {
            finiquitosClass fini = new finiquitosClass();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            fini.GuardarFacturacion(factura, periodo.IdPeriodoPago);
            return Json(new { resultado = "Se guardo Correctamente" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult crearDispersion()
        {
            Dispersion_Finiquito disp = new Dispersion_Finiquito();
            var pperiodo = Session["periodo"] as NOM_PeriodosPago;
            var idusuario = SessionHelpers.GetIdUsuario();
            var ruta = Server.MapPath("~/Files/DispercionFiniquito");
            var archivo = disp.crearExcel(idusuario, ruta, pperiodo.IdSucursal, pperiodo);
            return Json(archivo, JsonRequestBehavior.AllowGet);
        }
        public FileResult descargarArchivo(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
        public JsonResult imprimirReporte()
        {
            Reporte_Finiquito reportes = new Reporte_Finiquito();
            var ruta = Server.MapPath("~/Files/Autorizacion/");
            var pperiodo = Session["periodo"] as NOM_PeriodosPago;
            var idusuario = SessionHelpers.GetIdUsuario();
            var resultado = reportes.CrearReporteAutorizado(pperiodo, ruta, idusuario);
            return Json(resultado, JsonRequestBehavior.AllowGet);
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