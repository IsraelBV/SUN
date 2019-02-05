using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Nomina.WEB.Filtros;
using Nomina.BLL;
using RH.Entidades;
using RH.Entidades.GlobalModel;
using RH.BLL;
using Common.Helpers;
using Common.Utils;
using System.Diagnostics;
using FiltrosWeb;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [SesionPeriodo]
    [SesionSucursal]
    public class ProcesoNominaController : Controller
    {
        // GET: ProcesoNomina
        public PartialViewResult Index()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            ViewBag.Procesando = periodo.Procesando == true;
            ViewBag.Autorizado = periodo.Autorizado;

            //Obtiene las nóminas ya existentes de este periodo
            ProcesoNomina pn = new ProcesoNomina();

            var activarComplemento = Session["activarComplemento"] as int?;
            ViewBag.Complemento = activarComplemento == 0 || activarComplemento == null ? false : true;
            //nominas = nominas.OrderBy(x => x.IdEmpleado).ToList();

            //nuevo modelo 
            var datosNominasModelo = pn.GetNominaDatosProcesado(periodo.IdPeriodoPago);

            if (datosNominasModelo == null) return PartialView(null);

            var totalPercepciones = datosNominasModelo.Sum(x => x.Percepciones);
            var totalDeducciones = datosNominasModelo.Sum(x => x.Deducciones);
            var totalNomina = datosNominasModelo.Sum(x => x.TotalNomina);
            var totalOtrosPagos = datosNominasModelo.Sum(x => x.OtrosPagos);

            var zonaSalario = pn.GetZonaSalario();

            ViewBag.totalP = totalPercepciones.ToCurrencyFormat(2, true);
            ViewBag.totalD = totalDeducciones.ToCurrencyFormat(2, true);
            ViewBag.totalN = totalNomina.ToCurrencyFormat(2, true);
            ViewBag.SMGV = zonaSalario.SMG.ToCurrencyFormat(2, true);
            ViewBag.UMA = zonaSalario.UMA.ToCurrencyFormat(2, true);
            ViewBag.SoloComplemento = periodo.SoloComplemento;

            List<NotificationSummary> listSummary = new List<NotificationSummary>();

            if (TempData["summary"] != null)
            {
                listSummary = TempData["summary"] as List<NotificationSummary>;
            }

            ViewBag.Summary = listSummary;

            int i = 0;
            int j = 0;

            int idUsuario = SessionHelpers.GetIdUsuario();


            if (!periodo.SoloComplemento)
            {
                #region CONFIGURACION DE CONCEPTOS
                var configuracion = pn.ObtenerConfiguracion(idUsuario, periodo.IdSucursal, 18);

                if (configuracion != null)
                {
                    var visible = configuracion.ConceptosVisibles.Split(',');
                    var oculto = configuracion.ConceptosOcultos.Split(',');
                    int[] arrayVisible = new int[visible.Length];
                    int[] arrayOculto = new int[oculto.Length];

                    foreach (var v in visible)
                    {
                        arrayVisible[i] = Convert.ToInt32(v);
                        i++;
                    }

                    foreach (var o in oculto)
                    {
                        arrayOculto[j] = Convert.ToInt32(o);
                        j++;
                    }

                    if (arrayVisible.Contains(8))
                    {

                    }
                    ViewBag.visible = arrayVisible;
                    ViewBag.oculto = arrayOculto;
                }
                else
                {
                    if (datosNominasModelo.Count > 0)
                    {
                        var variable = datosNominasModelo[0];

                        int[] arrayVisible = new int[variable.Conceptos.Count];
                        int[] arrayOculto = { };
                        arrayVisible = variable.Conceptos.Select(x => x.IdConcepto).ToArray();
                        ViewBag.visible = arrayVisible;
                        ViewBag.oculto = arrayOculto;
                    }
                }
                #endregion
            }
            return PartialView(datosNominasModelo);
        }
        public PartialViewResult IndexAguinaldo()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            ViewBag.Procesando = periodo.Procesando == true;
            ViewBag.Autorizado = periodo.Autorizado;

            //Obtiene las nóminas ya existentes de este periodo
            ProcesoNomina pn = new ProcesoNomina();

            var activarComplemento = Session["activarComplemento"] as int?;
            ViewBag.Complemento = activarComplemento == 0 || activarComplemento == null ? false : true;
            //nominas = nominas.OrderBy(x => x.IdEmpleado).ToList();

            //nuevo modelo 
            //var datosNominasModelo = pn.GetNominaDatosProcesado(periodo.IdPeriodoPago);
            var datosNominasModelo = pn.GetNominaDatosProcesadoAguinaldo(periodo.IdPeriodoPago);

            if (datosNominasModelo == null) return PartialView(null);

            //var totalPercepciones = datosNominasModelo.Sum(x => x.Percepciones);
            //var totalDeducciones = datosNominasModelo.Sum(x => x.Deducciones);
            var totalNomina = datosNominasModelo.Sum(x => x.Total);
            //var totalOtrosPagos = datosNominasModelo.Sum(x => x.OtrosPagos);

            var zonaSalario = pn.GetZonaSalario();

            //ViewBag.totalP = 0;// totalPercepciones.ToCurrencyFormat(2, true);
            //ViewBag.totalD = 0;// totalDeducciones.ToCurrencyFormat(2, true);
            ViewBag.totalN = totalNomina.ToCurrencyFormat(2, true);
            ViewBag.SMGV = zonaSalario.SMG.ToCurrencyFormat(2, true);
            ViewBag.UMA = zonaSalario.UMA.ToCurrencyFormat(2, true);
            ViewBag.SoloComplemento = periodo.SoloComplemento;
            List<NotificationSummary> listSummary = new List<NotificationSummary>();

            if (TempData["summary"] != null)
            {
                listSummary = TempData["summary"] as List<NotificationSummary>;
            }

            ViewBag.Summary = listSummary;

            int i = 0;
            int j = 0;

            int idUsuario = SessionHelpers.GetIdUsuario();

            return PartialView(datosNominasModelo);
        }
        public async Task<JsonResult> ProcesarNominas(int[] empleados, bool anual=false)
        {
            //Validar que la Sesion del periodo este activa
            if (Session["periodo"] != null)
            {
                var periodoPago = Session["periodo"] as NOM_PeriodosPago;

                //Validar que el periodo no este Autorizado
                if (periodoPago.Autorizado) return Json(new { status = "El periodo ya esta Autorizado, no se puede volver a procesar" }, JsonRequestBehavior.AllowGet);

                if (periodoPago.Procesando) return Json(new { status = "El periodo esta siendo procesado en este momento ... Favor de Esperar... " }, JsonRequestBehavior.AllowGet);

                var sucursal = Session["sucursal"] as SucursalDatos;

                //INICIA EL PROCESADO DE LA NOMINA
                if (sucursal != null)
                {
                    var summary = await ProcesoNomina.ProcesarNominaAsync(empleados, periodoPago, sucursal.IdCliente, sucursal.IdSucursal, SessionHelpers.GetIdUsuario(), anual, false);

                    //Actualizar los datos del periodo en la variable de session
                    PeriodosPago ctx = new PeriodosPago();
                    var periodoActualizado = ctx.GetPeriodoPagoById(periodoPago.IdPeriodoPago);
                    Session["periodo"] = periodoActualizado;

                    if (summary != null)
                    {
                        TempData["summary"] = summary;
                    }
                }

            }

            return Json(new { status = "OK - nominas" }, JsonRequestBehavior.AllowGet);
        }
       
        public async Task<FileResult> GetRecibos(int[] idNominas = null)
        {
            try
            {
                if (idNominas == null) return null;

                Random random = new Random();
                int randomNumber = random.Next(0, 1000);//A

                var ruta = Server.MapPath("~/Files/DownloadRecibos");
                var idusuario = SessionHelpers.GetIdUsuario();
                var periodoPago = Session["periodo"] as NOM_PeriodosPago;
                if (periodoPago != null)
                {
                    //Genera el xml
                    var xml = await FacturaElectronica.GenerarXMLSintimbre(idNominas, periodoPago, periodoPago.IdEjercicio, periodoPago.IdPeriodoPago, idusuario);

                    //Crear el pdf con el xml generado
                    var recibo = await ProcesoNomina.GetRecibosSinTimbre(idNominas, periodoPago, idusuario, ruta);

                    var file = System.IO.File.ReadAllBytes(recibo);

                    var nombreArchivo = periodoPago.Descripcion + ".pdf";

                    return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {

            }
        }
        public async Task<FileResult> GetRecibos33(int[] idNominas = null)
        {
            try
            {
                if (idNominas == null) return null;

                var ruta = Server.MapPath("~/Files/DownloadRecibos");
                var idusuario = SessionHelpers.GetIdUsuario();
                var periodoPago = Session["periodo"] as NOM_PeriodosPago;
                if (periodoPago != null)
                {
                    //Genera el xml
                    var xml = await FacturaElectronica.GenerarXMLSintimbre(idNominas, periodoPago, periodoPago.IdEjercicio, periodoPago.IdPeriodoPago, idusuario, isCfdi33: true);

                    //Crear el pdf con el xml generado
                    var recibo = await ProcesoNomina.GetRecibosSinTimbre(idNominas, periodoPago, idusuario, ruta, isCfdi33: true);

                    var file = System.IO.File.ReadAllBytes(recibo);

                    var nombreArchivo = periodoPago.Descripcion + ".pdf";

                    return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {

            }
        }
        public async Task<FileResult> GetRecibosComplemento(int[] idNominas = null, bool incluirDetalles = false)
        {
            try
            {
                if (idNominas == null) return null;

                // int[] nominas = new[] {1,2,3,4,5,6};
                Random random = new Random();
                int randomNumber = random.Next(0, 1000);//A

                var ruta = Server.MapPath("~/Files/DownloadRecibos");
                var idusuario = SessionHelpers.GetIdUsuario();
                var periodoPago = Session["periodo"] as NOM_PeriodosPago;
                if (periodoPago != null)
                {

                    //Crear el pdf con el xml generado
                    var recibo = await ProcesoNomina.GetRecibosComplemento(idNominas, periodoPago, idusuario, ruta, incluirDetalles);

                    var file = System.IO.File.ReadAllBytes(recibo);

                    var nombreArchivo = "COMP_" + periodoPago.Descripcion + ".pdf";

                    return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {

            }
        }
        public async Task<JsonResult> ProcesarAguinaldo(int[] empleados, string[] faltas, bool[] generarPensionAlimenticia, bool anual=false)
        {
            //Validar que la Sesion del periodo este activa
            if (Session["periodo"] != null)
            {
                var periodoPago = Session["periodo"] as NOM_PeriodosPago;

                //Actualizar los datos del periodo en la variable de session
                PeriodosPago ctx = new PeriodosPago();
                var periodoActualizado = ctx.GetPeriodoPagoById(periodoPago.IdPeriodoPago);
                Session["periodo"] = periodoActualizado;

                //Validar que el periodo no este Autorizado
                if (periodoActualizado.Autorizado) return Json(new { status = "El periodo ya esta Autorizado, no se puede volver a procesar" }, JsonRequestBehavior.AllowGet);

                if (periodoActualizado.Procesando) return Json(new { status = "El periodo esta siendo procesado en este momento ... Favor de Esperar... " }, JsonRequestBehavior.AllowGet);

                var sucursal = Session["sucursal"] as SucursalDatos;

                //INICIA EL PROCESADO DE LA NOMINA
                if (sucursal != null)
                {
                    var summary = await ProcesoNomina.ProcesarAguinaldoAsync(empleados, faltas, generarPensionAlimenticia, periodoActualizado.IdPeriodoPago, sucursal.IdCliente, sucursal.IdSucursal, SessionHelpers.GetIdUsuario(),anual);

                    if (summary != null)
                    {
                        TempData["summary"] = summary;
                    }
                }

            }

            return Json(new { status = "OK - nominas" }, JsonRequestBehavior.AllowGet);
        }

      

        public PartialViewResult DetalleNomina(int id)
        {
            ProcesoNomina prn = new ProcesoNomina();
            var nomina = prn.GetNomina(id);
            ViewBag.Nomina = nomina;
            var detalle = prn.GetDetalleNomina(id);

            var activarComplemento = Session["activarComplemento"] as int?;
            if (activarComplemento == 0)
            {
                detalle = detalle.Where(x => x.Complemento != true).ToList();
            }
            ViewBag.ModoComplemento = activarComplemento == 1 ? true : false;
            return PartialView(detalle);
        }
        public PartialViewResult GetSeguroSocial(int id)
        {
            ProcesoNomina prn = new ProcesoNomina();
            var model = prn.GetDetalleSS(id);
            return PartialView(model);
        }
        public PartialViewResult GetImpuestosDetalle(int id)
        {
            int diasPeriodo = 0;
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (periodoPago != null)
            {
                diasPeriodo = periodoPago.DiasPeriodo;
            }

            ProcesoNomina prn = new ProcesoNomina();

            //var nomina = prn.GetNomina(id);

            var nomina = ProcesoNomina.GetNominaById(id);

            if (nomina != null)
            {
                var model = prn.GetImpuestosDetalle(nomina, nomina.SBC, nomina.SD, nomina.TipoTarifa, diasPeriodo, periodoPago.IdTipoNomina);

                return PartialView(model);
            }
            else
            {
                return PartialView(null);
            }

        }
        public PartialViewResult GetCalculoAnual(int id)
        {
            int diasPeriodo = 0;
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (periodoPago != null)
            {
                diasPeriodo = periodoPago.DiasPeriodo;
            }

            ProcesoNomina prn = new ProcesoNomina();

            //var nomina = prn.GetNomina(id);

            var nomina = ProcesoNomina.GetNominaById(id);

            if (nomina != null)
            {
                var model = prn.GetCalculoDetalle(id);

                return PartialView(model);
            }
            else
            {
                return PartialView(null);
            }

        }
        public JsonResult GuardarConceptosVO(int[] visibles, int[] ocultos)
        {

            var ctx = new ProcesoNomina();
            var mensaje = "";
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            int idUsuario = SessionHelpers.GetIdUsuario();
            var result = ctx.GuardarConfiguracion(idUsuario, periodo.IdSucursal, visibles, ocultos, 18);
            if (result == true)
            {
                mensaje = "mensaje guardado";
            }

            return Json(new { message = mensaje }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult detallePrincipal(int id)
        {
            ViewBag.idNomina = id;
            return PartialView();
        }

        public ActionResult DetalleAguinaldo(int id)
        {
            ProcesoNomina prn = new ProcesoNomina();
            RH.BLL.Empleados emp = new Empleados();

            var modelo = prn.GetAguinaldoByIdAguinaldo(id);

            var itemEmpleado = emp.GetEmpleadoById(modelo.IdEmpleado);

            ViewBag.Empleado = itemEmpleado;

            return PartialView(modelo);
        }
        public JsonResult reporteNomina()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            Reportes.Reportes_Nomina rep = new Reportes.Reportes_Nomina();

            var idusuario = SessionHelpers.GetIdUsuario();
            var ruta = Server.MapPath("~/Files/ReportesNomina");
            var resultado = rep.ExportarExcelReporteNomina(idusuario, ruta, periodo);

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReporteAguinaldos()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            Reportes.Reportes_Nomina rep = new Reportes.Reportes_Nomina();

            var idusuario = SessionHelpers.GetIdUsuario();
            //int IdSucursal = sucursal.IdSucursal;
            var ruta = Server.MapPath("~/Files/Aguinaldos/");
            var pathDescarga = "/Files/Aguinaldos/";

            var resultado = rep.CrearReporteAguinaldo(idusuario, ruta, pathDescarga, periodo.IdPeriodoPago);

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        public FileResult descargarPlantilla(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
        public PartialViewResult Creditos()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            ProcesoNomina prn = new ProcesoNomina();

            var modeloCreditos = prn.GetCreditosEmpleadosByPeriodo(periodo.IdPeriodoPago);

            return PartialView(modeloCreditos);
        }
        public JsonResult GuardarDiasCredito(int idEmpPeriodo, int diasCredito)
        {
            //Actualizar el nuevo valor
            ProcesoNomina prn = new ProcesoNomina();
            prn.ActualizaDiasInfonavitByPeriodo(idEmpPeriodo, diasCredito);

            return Json(new { result = "OK - nominas" }, JsonRequestBehavior.AllowGet);
        }
    }
}