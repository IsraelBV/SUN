using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using RH.Entidades;
using Nomina.WEB.Filtros;
using Common.Helpers;
using System.Threading.Tasks;
using RH.BLL;
using RH.Entidades.GlobalModel;
using FiltrosWeb;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [SesionPeriodo]
    [SesionSucursal]
    public class CfdiController : Controller
    {
        private readonly FacturaElectronica _fe = new FacturaElectronica();

        //[PeriodoAutorizado]
        //[PeriodoProcesado]
        public PartialViewResult Index()
        {
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            HomeController home = new HomeController();

            if (periodoPago == null)
            {
                return home.NoPeriodo();
            }

            //GET Status Actual del periodo
            var ctx = new PeriodosPago();
            var periodoActual = ctx.GetPeriodoPagoById(periodoPago.IdPeriodoPago);

            if (periodoActual == null)
            {
                return home.NoPeriodo();
            }

            //if (periodoActual.Procesado != true) return home.PeriodoNoProcesado(periodoPago.IdTipoNomina == 11);// 11 finiquito

            //Enviaremos a la vista si es finiquito o nomina normal
            ViewBag.isFiniquito = periodoPago.IdTipoNomina == 11;
            ViewBag.idPeriodo = periodoPago.IdPeriodoPago;

            if (periodoActual.Autorizado != true) return home.PeriodoNoAutorizado(periodoPago.IdTipoNomina == 11);

            Session["periodo"] = periodoActual;

            //TipoNomina = 11 es finiquito
            var modelo = periodoPago.IdTipoNomina == 11 ? _fe.GetDatosCfdiFiniquito(periodoPago.IdPeriodoPago) : _fe.GetDatosCfdi(periodoPago.IdPeriodoPago, periodoPago.IdTipoNomina);
            
           
            List<NotificationSummary> listSummary = new List<NotificationSummary>();

            if (TempData["summary"] != null)
            {
                listSummary = TempData["summary"] as List<NotificationSummary>;
            }

            ViewBag.Summary = listSummary;


            return PartialView(modelo);
        }
        public async Task<JsonResult> GenerarCfdi(int[] idNominas = null, int[] idEmisores = null)
        {
            //var t = await Metodos.EjecutarMetodos();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var sucursal = Session["sucursal"] as SucursalDatos;

            bool isFiniquito = false;
            string summaryMsj = "";
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            //Validar que el objeto  periodo no sea null
            if (periodo != null && sucursal != null)
            {
                isFiniquito = periodo.IdTipoNomina == 11;//Finiquito

                //obtenemos la ruta donde se guardará el archivo txt de log <- aun no esta terminado
                var rutaLog = Server.MapPath("~/Files/Log/" + periodo.IdPeriodoPago + "/");

                var idusuario = SessionHelpers.GetIdUsuario();

                //VALIDAR LOS CERTIFICADOS DEL EMISOR
                if (idEmisores != null)
                {
                    var resultValidacion = false;
                    int[] EmisoresDif = idEmisores.Distinct().ToArray();

                    int idEmisor = EmisoresDif[0];

                    resultValidacion = _fe.ValidarCertificados(idEmisor,false, out summaryMsj);

                    if (resultValidacion)
                    {
                        //Se obtiene una lista de Id de las nominas que se generaron su xml y su timbrado
                        var summary = await _fe.GenerarCfdisAsync(sucursal.IdSucursal, periodo.IdEjercicio,
                            periodo.IdPeriodoPago, idNominas, rutaLog, idusuario, isFiniquito: isFiniquito);
                        if (summary != null)
                        {
                            TempData["summary"] = summary;
                        }
                    }
                    else
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = 0,
                            Msg1 = summaryMsj,
                            Msg2 = ""
                        });

                        if (summaryList.Count > 0)
                        {
                            TempData["summary"] = summaryList;
                        }
                    }

                }
            }

            //Se obtiene los datos de la emision del cfdi, ejemplo el Folio fiscal, fecha de Certificacion
            // var cfdinominas = _fe.GetDatosParaEmisionDelCfdi(list, isFiniquito: isFiniquito);

            //
            //return Json(cfdinominas, JsonRequestBehavior.AllowGet);
            return Json(new { status = summaryMsj }, JsonRequestBehavior.AllowGet);


        }

        public async Task<JsonResult> GenerarCfdi33(int[] idNominas = null, int[] idEmisores = null)
        {
            bool isFiniquito = false;
            string summaryMsj = "";
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            
            //var t = await Metodos.EjecutarMetodos();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var sucursal = Session["sucursal"] as SucursalDatos;

            //Aun no esta disponible el timbrado con 3.3
            //NotificationSummary itemNo33 = new NotificationSummary()
            //{
            //    Reg = 0,
            //    Msg1 = "No esta disponible el timbrado con version 3.3"

            //};

            //summaryList.Add(itemNo33);
            //TempData["summary"] = summaryList;

            //return Json(new { status = summaryMsj }, JsonRequestBehavior.AllowGet);



            //Validar que el objeto  periodo no sea null
            if (periodo != null && sucursal != null)
            {
                isFiniquito = periodo.IdTipoNomina == 11;//Finiquito

                //obtenemos la ruta donde se guardará el archivo txt de log <- aun no esta terminado
                var rutaLog = Server.MapPath("~/Files/Log/" + periodo.IdPeriodoPago + "/");

                var idusuario = SessionHelpers.GetIdUsuario();

                //VALIDAR LOS CERTIFICADOS DEL EMISOR
                if (idEmisores != null)
                {
                    var resultValidacion = false;
                    int[] EmisoresDif = idEmisores.Distinct().ToArray();

                    int idEmisor = EmisoresDif[0];

                    resultValidacion = _fe.ValidarCertificados(idEmisor,true, out summaryMsj);

                    if (resultValidacion)
                    {
                        //Se obtiene una lista de Id de las nominas que se generaron su xml y su timbrado
                        var summary = await _fe.GenerarCfdisAsync(sucursal.IdSucursal, periodo.IdEjercicio,
                            periodo.IdPeriodoPago, idNominas, rutaLog, idusuario, isFiniquito: isFiniquito, isCfdi33: true);


                        if (summary != null)
                        {
                            TempData["summary"] = summary;
                        }
                    }
                    else
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = 0,
                            Msg1 = summaryMsj,
                            Msg2 = ""
                        });

                        if (summaryList.Count > 0)
                        {
                            TempData["summary"] = summaryList;
                        }
                    }

                }
            }

            //Se obtiene los datos de la emision del cfdi, ejemplo el Folio fiscal, fecha de Certificacion
            // var cfdinominas = _fe.GetDatosParaEmisionDelCfdi(list, isFiniquito: isFiniquito);

            //
            //return Json(cfdinominas, JsonRequestBehavior.AllowGet);
            return Json(new { status = summaryMsj }, JsonRequestBehavior.AllowGet);
            
        }

        public async Task<ActionResult> DownloadRecibos(int[] idNominas = null)
        {

            if (idNominas == null) return null;

            var ruta = Server.MapPath("~/Files/DownloadRecibos");
            var idusuario = SessionHelpers.GetIdUsuario();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            DateTime dt = DateTime.Now;
            var nombrezip = "Cfdi ";
            bool isFiniquito = false;

            if (periodo != null)
            {
                isFiniquito = periodo.IdTipoNomina == 11;//Finiquito
                nombrezip += periodo.Descripcion;
            }

            nombrezip += " " + dt.Day + dt.Month + dt.Year + ".zip";

            var arr = await _fe.DownloadRecibosCfdiAsync(idNominas, idusuario, ruta, isFiniquito: isFiniquito);

            return new ZipResult(nombrezip, arr);
        }
        public PartialViewResult indexErrores(int idPeriodo)
        {
            FacturaElectronica emp = new FacturaElectronica();
            var empleados = emp.EmpleadosErrorTimbrado(idPeriodo);

            return PartialView(empleados);
        }
        public PartialViewResult listadoErrores(int idNomima = 0)
        {
            //if (idNomima == 0) return null;

            FacturaElectronica emp = new FacturaElectronica();
            var errores = emp.ListadoMensajePac(idNomima);
            return PartialView(errores);
        }

    }
}