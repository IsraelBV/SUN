using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using Nomina.Reportes;
using RH.BLL;
using RH.Entidades;
using Common.Helpers;

namespace Nomina.WEB.Controllers
{
    public class ContabilidadController : Controller
    {
        // GET: CuentasContables
        public ActionResult Index()
        {
            _CuentasContables cuentas = new _CuentasContables();

            var listaEmpresas = cuentas.ListEmpresaFiscales();


            return PartialView(listaEmpresas);
        }

        public ActionResult tablasCuentas(int idEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();
            var listaconceptos = cuentas.empresasCuentas(idEmpresa);
            return PartialView(listaconceptos);
        }

        public JsonResult EditarCuentaDeudora(int Id, string ClaveContable, int IdEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();

            var resultado = cuentas.GuardarDeudora(Id, IdEmpresa, ClaveContable);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditarCuentaAcredora(int Id, string ClaveContable, int IdEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();

            var resultado = cuentas.GuardarAcredora(Id, IdEmpresa, ClaveContable);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteCuentaContable()
        {
            _CuentasContables cuentas = new _CuentasContables();

            var listaEmpresas = cuentas.ListEmpresaFiscales();


            return PartialView(listaEmpresas);

        }
        public ActionResult tablaCliente(int idEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();
            var listaCliente = cuentas.ListaSucursalByID(idEmpresa);
            return PartialView(listaCliente);
        }

        public ActionResult tablaPeriodos(int idEmpresa, int idCliente, DateTime fechaInicio, DateTime fechaFin)
        {
            _CuentasContables cuentas = new _CuentasContables();
            var lista = cuentas.listaPeriodoByIDs(idEmpresa, idCliente, fechaInicio, fechaFin);
            return PartialView(lista);
        }

        public JsonResult ListaDeRayaByEmpresa(int idEmpresa, int idPeriodo)
        {
            var inci = new Reportes.Datos.ListaDeRaya();
            var reportes = new Reporte_ListaRaya();


                var ctx = new RHEntities();
                var periodo = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).FirstOrDefault();

                _Incidencias inc = new _Incidencias();
                var incidencias2 = inc.GetIncidenciasByPeriodo2(periodo);
                var listaInicencias = inci.ContadoresIncidencias(incidencias2);

                var nombreperiodo = periodo.Descripcion;
                DateTime fechaIni = periodo.Fecha_Inicio;
                DateTime fechaFin = periodo.Fecha_Fin;
                int TipoDeNomina = periodo.IdTipoNomina;
                string NominaTIpo = Cat_Sat.GetPeriodicidadPagoById(TipoDeNomina).Descripcion;

                var ruta = Server.MapPath("~/Files/Autorizacion/");
                var idusuario = SessionHelpers.GetIdUsuario();



                var resultado = reportes.ListaDeRayaPorEmpresa(periodo, false, ruta, idusuario, listaInicencias,idEmpresa);
                var file = System.IO.File.ReadAllBytes(resultado);

                //return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "Autorizacion.pdf");
                return Json(resultado, JsonRequestBehavior.AllowGet);
                    }

        public FileResult descargarListaRaya(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, "application/pdf");
        }

        public JsonResult reporteGeneral(int[] idperiodos, int idempresa,DateTime fechaInicio, DateTime fechaFin)
        {
            var idU = SessionHelpers.GetIdUsuario();
            var ruta = Server.MapPath("~//Files/ReporteContabilidad/");

            Reporte_ContableGeneral conta = new Reporte_ContableGeneral();

            var resultado = conta.crearReporteGeneral(idU, ruta, idperiodos,idempresa,fechaInicio,fechaFin);

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

     
        public JsonResult reporteByEmpleado(int[] idperiodos, int idempresa, DateTime fechaInicio, DateTime fechaFin)
        {
            var idU = SessionHelpers.GetIdUsuario();
            var ruta = Server.MapPath("~//Files/ReporteContabilidad/");

            Reporte_ContableByEmpleado conta = new Reporte_ContableByEmpleado();

            var resultado = conta.crearReportByEmpleado(idU, ruta, idperiodos, idempresa, fechaInicio, fechaFin);

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public FileResult descargarReporteContable(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }

        public ActionResult indexClientes()
        {
            _CuentasContables cuentas = new _CuentasContables();

            var listaEmpresas = cuentas.ListEmpresaFiscales();
            return PartialView(listaEmpresas);
        }

        public ActionResult configuracionClientes(int idEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();
            var lista = cuentas.ClientesByEmpresa(idEmpresa);
            return PartialView(lista);
        }

        public JsonResult EditarClaveCliente(int IdCliente, string ClaveCliente,int IdEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();

            var resultado = cuentas.GuardarClaveCliente(IdCliente, ClaveCliente, IdEmpresa);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}