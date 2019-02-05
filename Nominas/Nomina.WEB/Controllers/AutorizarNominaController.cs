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

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SesionPeriodo]
    [SesionSucursal]
    public class AutorizarNominaController : Controller
    {
        private readonly AutorizarNomina _aut = new AutorizarNomina();

        [PeriodoProcesado]
        public ActionResult Index()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var model = _aut.VisualizarFacturacion(periodo.IdPeriodoPago);
            ViewBag.Facturacion = model;
            ViewBag.Procesar = periodo.Procesado;
            ViewBag.Autorizado = periodo.Autorizado;

            var lista = _aut.autorizaciondetalle(periodo.IdPeriodoPago);
            ViewBag.obligaciones = _aut.obligacionesGenerales(periodo.IdPeriodoPago);
            return PartialView(lista);
        }
        public JsonResult AutorizarNomina()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var resultado = _aut.Autorizacion(periodo.IdPeriodoPago);
            if (resultado)
            {
                periodo.Autorizado = true;
                Session["periodo"] = periodo;
            }
            return Json(new { result = resultado  }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ImprimirAutorizacion()
        {
            Reporte_Autorizar reportes = new Reporte_Autorizar();
            bool complemento = false;
            if ((int)Session["activarComplemento"] == 1)
            {
                complemento = true;
            }


            var sucursal = Session["sucursal"] as SucursalDatos;
            if (sucursal == null)
            {
                return null;
            }
            else
            {
                
                var periodo = Session["periodo"] as NOM_PeriodosPago;
                
                _Incidencias inc = new _Incidencias();
                var incidencias2 = inc.GetIncidenciasByPeriodo2(periodo);
                List<EmpleadoIncidencias2> _inci = new List<EmpleadoIncidencias2>();
                int contadorDescanso = 0;
                int contadorAsistencia = 0;
                int contadorIR = 0;
                int contadorIE = 0;
                int contadorIM = 0;
                int contadorFJ = 0;
                int contadorFI = 0;
                int contadorFA = 0;
                int contadorV = 0;
                foreach (var a in incidencias2)
                {
                    foreach (var b in a.Incidencias)
                    {
                        if (b.TipoIncidencia.Trim() == "D")
                        {
                            contadorDescanso = contadorDescanso + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "X")
                        {
                            contadorAsistencia = contadorAsistencia + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "IR")
                        {
                            contadorIR = contadorIR + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "IE")
                        {
                            contadorIE = contadorIE + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "IM")
                        {
                            contadorIM = contadorIM + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "PS")
                        {
                            contadorFJ = contadorFJ + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "FI")
                        {
                            contadorFI = contadorFI + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "FA")
                        {
                            contadorFA = contadorFA + 1;
                        }
                        if (b.TipoIncidencia.Trim() == "V")
                        {
                            contadorV = contadorV + 1;
                        }
                    }
                    _inci.Add(new EmpleadoIncidencias2 {
                        IdEmpleado = a.IdEmpleado,
                        Descansos = contadorDescanso,
                        Asistencias = contadorAsistencia,
                        IncapacidadesE = contadorIE,
                        IncapacidadesM = contadorIM,
                        IncapacidadesR = contadorIR,
                        FaltasA = contadorFA,
                        FaltasI = contadorFI,
                        _Vacaciones = contadorV,
                    DiasAPagar = a.DiasAPagar

                });


                    contadorAsistencia = 0;
                    contadorDescanso = 0;
                    contadorIR = 0;
                    contadorIE = 0;
                    contadorIM = 0;
                    contadorFJ = 0;
                    contadorFI = 0;
                    contadorFA = 0;
                    contadorV = 0;
                }
                var nombreperiodo = periodo.Descripcion;
                DateTime fechaIni = periodo.Fecha_Inicio;
                DateTime fechaFin = periodo.Fecha_Fin;
                int TipoDeNomina = periodo.IdTipoNomina;
                string NominaTIpo = Cat_Sat.GetPeriodicidadPagoById(TipoDeNomina).Descripcion;

                var ruta = Server.MapPath("~/Files/Autorizacion/");
                var idusuario = SessionHelpers.GetIdUsuario();



                var resultado = "";
                if (periodo.IdTipoNomina == 12)
                {
                    resultado = reportes.ListaDeRayaAguinaldo(idusuario, sucursal.IdSucursal, periodo.IdPeriodoPago,ruta);
                }
                else
                {
                    resultado = reportes.CrearReporteAutorizado(periodo.IdPeriodoPago, sucursal.IdSucursal, fechaIni, fechaFin, NominaTIpo, complemento, ruta, idusuario, _inci);
                    //var file = System.IO.File.ReadAllBytes(resultado); 
                }

                  //return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "Autorizacion.pdf");
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
          
        }
        public FileResult descargarListaRaya(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
        public ActionResult Reporte_Dispersion()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            int idperiodo = periodo.IdPeriodoPago;
            //Total Percepciones
            var lista = _aut.autorizaciondetalle(periodo.IdPeriodoPago);
            //Total Rubros IMSS
            ViewBag.CuotasIMSS = _aut.DispersionCuotasIMSS(idperiodo);
            //Total Impuesto sobre Nomina
            ViewBag.ImpuestoNomina = _aut.totalImpuestoSobreNomina(idperiodo);
            //Infonavit
            ViewBag.Infonavit = _aut.totalInfonavit(idperiodo);
            //Fonacot
            ViewBag.Fonacot = _aut.totalFonacot(idperiodo);

            return PartialView(lista);
        }
        public ActionResult Autorizacion_Factura()
        {
            AutorizarNomina aut = new AutorizarNomina();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var model = aut.VisualizarFacturacion(periodo.IdPeriodoPago);
            var modeloComplemento = aut.VisualizarFacturacionC(periodo.IdPeriodoPago);
            var modeloSindicato = aut.VisualizarFacturacionS(periodo.IdPeriodoPago);

            ViewBag.modeloS = modeloSindicato;           
            ViewBag.modeloC = modeloComplemento;
            
            return PartialView(model);
        }
        public ActionResult Autorizar_Nomina_Fiscal()
        {
            AutorizarNomina aut = new AutorizarNomina();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var model = aut.VisualizarFacturacion(periodo.IdPeriodoPago);
            return PartialView(model);
        }
        /// <summary>
        /// Guarda los nuevos cambios de la facturacion para poder dispersar y autorizar 
        /// </summary>
        /// <param name="collecion"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarFactura(NOM_FacturacionComplemento facturaC, NOM_FacturacionSindicato facturaS)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            AutorizarNomina aut = new AutorizarNomina();
            var sucursal = Session["sucursal"] as SucursalDatos;

            aut.GuardarFacturacion(facturaC, periodo.IdPeriodoPago);
            aut.GuardarFacturacionSindicato(facturaS, periodo.IdPeriodoPago,sucursal);
            return null;
        }
        public JsonResult ImprimirDispersion()
        {
            Reporte_Dispersion repDispersion = new Reporte_Dispersion();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
           
            int IdPeriodo = periodo.IdPeriodoPago;
            var idusuario = SessionHelpers.GetIdUsuario();

            bool complemento = false;
            if ((int)Session["activarComplemento"] == 1)
            {
                complemento = true;
            }


            var sucursal = Session["sucursal"] as SucursalDatos;
            if (sucursal == null)
            {
                return null;
            }
            else
            {
                DateTime fechaIni = periodo.Fecha_Inicio;
                DateTime fechaFin = periodo.Fecha_Fin;

                int IdSucursal = sucursal.IdSucursal;
                var ruta = Server.MapPath("~/Files/Dispersion/");

                var resultado = repDispersion.crearexcel(idusuario, ruta, IdSucursal, periodo, complemento, fechaIni, fechaFin);//ABC - actual
                //var resultado = repDispersion.GenerarReporteDispersion(idusuario, ruta, IdSucursal, IdPeriodo, complemento);//ABC

                //var file = System.IO.File.ReadAllBytes(resultado);
                //Regresa el archivo
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }


        }

        public FileResult descargarDispersion(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
    }
}