using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Helpers;
using RH.BLL;
using Nomina.BLL;
using RH.Entidades;
using Nomina.WEB.Filtros;
using FiltrosWeb;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [SesionSucursal]
    public class PeriodosPagoController : Controller
    {
        public ActionResult GetPeriodosPago()
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            var ctx = new PeriodosPago();
            var periodos = ctx.GetPeriodosPagoBySucursal(sucursal.IdSucursal);

            var modeloLista = periodos.OrderByDescending(x => x.IdPeriodoPago).ToList();

            return PartialView(modeloLista);
        }
        public PartialViewResult NuevoPeriodo()
        {
            var ctx = new PeriodosPago();
            var periodicidades = ctx.GetPeriodicidadPagos();
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago p = new PeriodosPago();
            
            ViewBag.Periodicidades = periodicidades;
            return PartialView();
        }
        public PartialViewResult SeleccionEmpleadosPeriodo(int id)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            ViewBag.IdPeriodo = id;

            //Obtiene los empleados seleccionados en el periodo
            PeriodosPago p = new PeriodosPago();
            var periodo = p.GetPeriodoPagoById(id);

            Empleados emp = new Empleados();
            var empleados = new List<DatosEmpleado>();

            if (periodo.Especial == true) //si el periodo es especial se muestran todos los empleados, independientemente de su tipo de nómina
                empleados = emp.GetEmpleadosBySucursalConTipoNomina(sucursal.IdSucursal);
            else //si no pues no
                empleados = emp.GetEmpleadosByTipoNomina(sucursal.IdSucursal, periodo.IdTipoNomina);

            ViewBag.EmpleadosSeleccionados = p.GetIdEmpleados(id);
            return PartialView(empleados);
        }
        public PartialViewResult PeriodoDetalle(int id)
        {
            var ctx = new PeriodosPago();
            var periodicidades = ctx.GetPeriodicidadPagos();
            ViewBag.Periodicidades = periodicidades;

            PeriodosPago p = new PeriodosPago();
            var model = p.GetPeriodoPagoById(id);
            ViewBag.empleadoPeri = p.empleadosDetalle(id);
            return PartialView(model);
        }
        [HttpPost]
        public void AsignarEmpleadosAPeriodo(int IdPeriodoPago, int[] empleados = null)
        {
            PeriodosPago p = new PeriodosPago();
            p.UpdatePeriodoPagoEmpleados(IdPeriodoPago, empleados);
        }
        public JsonResult CerrarPeriodo(int id)
        {
            PeriodosPago p = new PeriodosPago();
            var result = p.CerrarPeriodo(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void UpdatePeriodoPago(NOM_PeriodosPago updatedModel)
        {
            PeriodosPago p = new PeriodosPago();
            p.UpdatePeriodoPago(updatedModel);
        }
        public ActionResult GetEmpleadoByTipoNomina (int idPeriocidadPago,int rfc)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago p = new PeriodosPago();
            var datos = p.GetEmpleadoByTipoNomina(idPeriocidadPago, sucursal.IdSucursal,rfc);
            return PartialView(datos);
        }
        public ActionResult empleadoFiniquito(int idPeriocidadPago, int rfc)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago p = new PeriodosPago();
            var datos = p.empleadoFiniquito(sucursal.IdSucursal, rfc);
            return PartialView(datos);
        }
        public ActionResult empleadoFiniquitoC(int idPeriocidadPago, int rfc)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago p = new PeriodosPago();
            var datos = p.empleadoFiniquitoC(sucursal.IdSucursal, rfc);
            return PartialView(datos);
        }
        public ActionResult EmpleadoRFCInvalido(int idPeriocidadPago, int rfc)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago p = new PeriodosPago();
            var datos = p.GetEmpleadoByTipoNomina(idPeriocidadPago, sucursal.IdSucursal, rfc);
            return PartialView(datos);
        }
        public ActionResult EmpleadosBaja(int idPeriocidadPago)

        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago p = new PeriodosPago();
            var datos = p.EmpleadosBaja(idPeriocidadPago, sucursal.IdSucursal);
            return PartialView(datos);
        }
        public JsonResult CrearPeriodo (int [] arrayE, string [] periodoDatos)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago crear = new PeriodosPago();
            int idUsuario = SessionHelpers.GetIdUsuario();

            crear.guardarPeriodo(arrayE, periodoDatos, sucursal.IdSucursal, sucursal.IdCliente, idUsuario);

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult empleadosAgregados(int idPeriodo)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;

            PeriodosPago p = new PeriodosPago();
            
            var empleadoPeri = p.empleadosDetalle(idPeriodo);
            ViewBag.idPeriodo = idPeriodo;
            return PartialView(empleadoPeri);

        }
        public ActionResult empleadosDisponibles (int idPeriodo,int idtiponomina)
        {
            PeriodosPago p = new PeriodosPago();
            
            var sucursal = Session["sucursal"] as SucursalDatos;
            if(idtiponomina == 11)
            {
                var emp = p.IdempleadosDetalleFiniquito(idPeriodo, sucursal.IdSucursal);
                ViewBag.idPeriodo = idPeriodo;
                ViewBag.idTipoNomina = idtiponomina;
                return PartialView(emp);
            }
            else
            {
                var emp = p.IdempleadosDetalle(idPeriodo, sucursal.IdSucursal);
                ViewBag.idPeriodo = idPeriodo;
                ViewBag.idTipoNomina = idtiponomina;
                return PartialView(emp);
            }
            
           
        }
        public JsonResult eliminarLista (int [] arrayE,int idPeriodo)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            PeriodosPago p = new PeriodosPago();
           var result= p.eliminarEmpLista(arrayE, idPeriodo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult agregarEmpleadosPeriodo (int [] arrayE, int idPeriodo)
        {
            PeriodosPago p = new PeriodosPago();
            var result = p.agregarEmpleadosAPeriodo(arrayE, idPeriodo);
                return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult cambiarDescripcion (int idPeriodo, string nombreNuevo)
        {
            PeriodosPago p = new PeriodosPago();

            var result = p.cambiarNombre(idPeriodo, nombreNuevo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> eliminarPeriodo(int idPeriodo)
        {
            PeriodosPago p = new PeriodosPago();
            var result = await p.EliminarPeriodoAsync(idPeriodo);

            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}