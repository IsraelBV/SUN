using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using FiltrosWeb;
using Common.Helpers;
using Common.Enums;
using System.Globalization;

namespace SS.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.SeguroSocial)]
    public class ReportesController : Controller
    {
        // GET: Reportes
        public ActionResult Index()
        {
            Empresas empresaDao = new Empresas();


            var listaEmpresas = empresaDao.GetAllEmpresas();
            var listaEjercicios = EjerciciosFiscales.GetEjerciciosFiscalesAsc();

            var selectListEmpresas = listaEmpresas.Select(x => new SelectListItem()
            {
                Value = x.IdEmpresa.ToString(),
                Text = x.RazonSocial,
            }).ToList();

            var itemNew = new SelectListItem()
            {
                Value = "0",
                Text = "- Todas -",
                Selected = true
            };
            selectListEmpresas.Insert(0, itemNew);


            ViewBag.ListaEmpresas = selectListEmpresas;
            ViewBag.ListaEjercicios = listaEjercicios;

            return View();
        }

        public ActionResult Confronta_infonavit()
        {
            Empresas empresaDao = new Empresas();


            var listaEmpresas = empresaDao.GetAllEmpresas();
            var listaEjercicios = EjerciciosFiscales.GetEjerciciosFiscalesAsc();
            var listames = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();

            var selectListEmpresas = listaEmpresas.Select(x => new SelectListItem()
            {
                Value = x.IdEmpresa.ToString(),
                Text = x.RazonSocial,
            }).ToList();

            
            var selectListMes = listames.Select(x => new SelectListItem()
            {
                Value = (listames.IndexOf(x) + 1).ToString(),
                Text = x
            }).ToList();




            ViewBag.ListaEmpresas = selectListEmpresas;
            ViewBag.ListaEjercicios = listaEjercicios;
            ViewBag.listameses = selectListMes;

            return View();
        }

        public JsonResult GetClientes(int id)
        {
            if (id > 0)
            {
                Empresas empresaDao = new Empresas();

                var listaClientes = empresaDao.GetClientesByIdEmpresa(id);

                var selectListClientes = listaClientes.Select(x => new SelectListItem()
                {
                    Value = x.IdCliente.ToString(),
                    Text = x.Nombre,
                }).ToList();

                var itemNew = new SelectListItem()
                {
                    Value = "0",
                    Text = "  Todas -",
                    Selected = true
                };
                selectListClientes.Insert(0, itemNew);

                return Json(selectListClientes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var selectListClientes = new List<SelectListItem>();

                var itemNew = new SelectListItem()
                {
                    Value = "0",
                    Text = " Todas - ",
                    Selected = true
                };
                selectListClientes.Insert(0, itemNew);

                return Json(selectListClientes, JsonRequestBehavior.AllowGet);
            }



            return Json(new { status = "--" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ReporteInfonavit(int? idEjercicio, int idEmpresa = 0)
        {
            if (idEjercicio == null)
            {
                return Json(new { success = false, error = "Debe seleccionar el año del ejercicio fiscal", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            var archivoReporte = "";
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/Acumulados");
            var pathDescarga = "/Files/Acumulados/";

            archivoReporte = ReportesImss.GenerarReporteInfonavit(idUsuario, idEjercicio, ruta, pathDescarga, idEmpresa);

            return Json(new { success = true, error = "", resultPath = archivoReporte }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reporte3()
        {
            Empresas empresaDao = new Empresas();
            var listaEmpresas = empresaDao.GetAllEmpresas();
            var selectListEmpresas = listaEmpresas.Where(x => x.RegistroPatronal != null).Select(x => new SelectListItem()
            {
                Value = x.IdEmpresa.ToString(),
                Text = x.RazonSocial,
            }).ToList();

            var itemNew = new SelectListItem()
            {
                Value = "0",
                Text = "- Todas -",
                Selected = true
            };
            selectListEmpresas.Insert(0, itemNew);


            ViewBag.ListaEmpresas = selectListEmpresas;

            return View();
        }
        
        public JsonResult Reporte3P(int idEmpresa ,  DateTime fechaI, DateTime fechaF)
        {
           

            var archivoReporte = "";
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/Reporte3");
            var pathDescarga = "/Files/Reporte3/";

            archivoReporte = ReportesImss.GenerarReporte3Porciento(idUsuario, fechaI, fechaF , idEmpresa, ruta, pathDescarga );

            return Json(new { success = true, error = "", resultPath = archivoReporte }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteIncapacidades()
        {
            //Empresas.GetEmpresasFiscales empresaDao = new Empresas();
            var listaEmpresas = Empresas.GetEmpresasFiscales();
            listaEmpresas.Insert(
                0, 
                new RH.Entidades.Empresa() {
                    IdEmpresa = 0,
                    RazonSocial = "- Todas -"
                }
            );
            /*var selectListEmpresas = listaEmpresas.Select(x => new SelectListItem()
            {
                Value = x.IdEmpresa.ToString(),
                Text = x.RazonSocial,
            }).ToList();

            var itemNew = new SelectListItem()
            {
                Value = "0",
                Text = "- Todas -",
                Selected = true
            };
            selectListEmpresas.Insert(0, itemNew);


            ViewBag.ListaEmpresas = selectListEmpresas;*/
            
            return View(listaEmpresas);
        }


        public JsonResult ReporteIncapacidad(int idEmpresa, DateTime fechaI, DateTime fechaF, int tipo)
        {


            var archivoReporte = "";
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/ReporteIncap");
            var pathDescarga = "/Files/ReporteIncap/";

            archivoReporte = (tipo == 0)? ReportesImss.GenerarReporteIncapacidades(idUsuario, fechaI, fechaF, idEmpresa, ruta, pathDescarga) : ReportesImss.GenerarReporteIncapacidades2(idUsuario, fechaI, fechaF, idEmpresa, ruta, pathDescarga);

            return Json(new { success = true, error = "", resultPath = archivoReporte }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AlertaGlobales()
        {
            var reportes = new ReportesImss();
            var lista = reportes.EmpresasFiscales();
            return View(lista);
        }

        public JsonResult ReporteAlertas(string NombreEmpresa, DateTime fechaI, DateTime fechaF,int tipoAlerta, int idEmpresa = 0)
        {
            
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/AlertasGlobales");
            var pathDescarga = "/Files/AlertasGlobales/";

            var archivoReporte = ReportesImss.GenerarReporteGeneralAlertas(idUsuario, fechaI, fechaF, NombreEmpresa, ruta,tipoAlerta, idEmpresa, pathDescarga);


            return Json(archivoReporte, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IndexReporteAusentismo()
        {
            var reportes = new ReportesImss();
            var lista = reportes.EmpresasFiscales();
            return View(lista);
        }

        public JsonResult ReportesAusentismo(int idempresa ,string NombreEmpresa, DateTime fechaI, DateTime fechaF)
        {
            int idUsuario = SessionHelpers.GetIdUsuario();

            var ruta = Server.MapPath("~/Files/Ausentismo");
            var pathDescarga = "/Files/Ausentismo/";

            var archivoReporte = ReportesImss.GenerarReporteAusentismo2(idUsuario, fechaI, fechaF, NombreEmpresa, ruta,pathDescarga,idempresa);
            return Json(archivoReporte, JsonRequestBehavior.AllowGet);
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