using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using RH.Entidades;
using Common.Enums;
using RH.BLL;
using SYA.BLL;

namespace SS.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.SeguroSocial)]
    public class EmpleadosController : Controller
    {
        Empleados emp = new Empleados();

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var user = ControlAcceso.GetUsuarioEnSession();            
            var model = emp.GetEmpleadosFiscales(user);
            return View(model);
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Consultar)]
        public ActionResult Details(int id, int idContrato)
        {
            //Obtener datos del empleado
            var empleado = emp.ObtenerEmpleadoPorId(id);
            ViewBag.Empleado = empleado;
            // var contrato = emp.GetUltimoContrato(id);
            var contrato = emp.GetContratoByIdEmpleadoContrato(idContrato);
            ViewBag.Contrato = contrato;

          

            //Obtener los datos de la empresa y la sucursal
            Empresas es = new Empresas();
            ViewBag.Empresa = es.GetEmpresaById((int)contrato.IdEmpresaFiscal);

            Sucursales suc = new Sucursales();
            ViewBag.Sucursal = suc.ObtenerSucursalDatosPorId(ViewBag.Empleado.IdSucursal);

            //Obtener el salario mínimo para cálculos
            Infonavit inf = new Infonavit();
            var zona = inf.GetZonaSalario();
            ViewBag.SM = zona.SMG;
            ViewBag.UMA = inf.GetValorUMA();
            return View();
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Editar)]
        public ActionResult UpdateSS()
        {
            var success = emp.UpdateSS(Request.Form);
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckInfonavit(int id)
        {
            Infonavit inf = new Infonavit();
            var model = inf.GetListaInfonavit(id);
            var status = true;
            var url = "/Empleados/ListaInfonavit";
            if (model.Count == 0)
            {
                status = false;
                url = "/Empleados/NewInfonavitForm";
            }

            var active = model.Where(x => x.Status == true).ToList().Count; //hay alguno activo? 
                
            return Json(new { model = status, IdContrato = id, url = url, activos = active }, JsonRequestBehavior.AllowGet);
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult GetInfonavit(int IdInfonavit)
        {
            Infonavit inf = new Infonavit();
            var model = inf.GetInfonavitById(IdInfonavit);
            return PartialView(model);
        }

        public PartialViewResult NewInfonavitForm(int IdEmpleadoContrato)
        {
            ViewBag.IdEmpleadoContrato = IdEmpleadoContrato;
            return PartialView();
        }

        public PartialViewResult ListaInfonavit(int IdEmpleadoContrato)
        {
            Infonavit inf = new Infonavit();
            var list = inf.GetListaInfonavit(IdEmpleadoContrato);
            return PartialView(list);
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Agregar)]
        public ActionResult CreateInfonavit(Empleado_Infonavit model)
        {
            Infonavit inf = new Infonavit();
            var id = inf.Create(model);
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateInfonavit(CalculoInfonavit model)
        {
            Empleado_Infonavit infonavit = new Empleado_Infonavit();
            infonavit.Id = model.IdInfonavit;
            infonavit.IdEmpleadoContrato = model.IdEmpleadoContrato;
            infonavit.NumCredito = model.NumCredito;
            infonavit.TipoCredito = model.TipoCredito;
            infonavit.FactorDescuento = model.FactorDescuento;
            infonavit.Salario = model.Salario;
            infonavit.FechaInicio = model.FechaInicio;
            infonavit.FechaSuspension = model.FechaSuspension;
            infonavit.Status = model.Status;

            Infonavit inf = new Infonavit();
            var response = inf.Update(infonavit);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

  

        public PartialViewResult GetHistorialPagosInfonavit(int IdInfonavit)
        {
            Infonavit inf = new Infonavit();
            var model = inf.GetHistorialPagos(IdInfonavit);
            return PartialView(model);
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult GetFonacot(int IdEmpleadoContrato)
        {
            ViewBag.IdEmpleadoContrato = IdEmpleadoContrato;
            Fonacot fon = new Fonacot();
            var model = fon.GetFonacotByEmpleado(IdEmpleadoContrato);
            return PartialView(model);
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Agregar)]
        public ActionResult CreateFonacot(Empleado_Fonacot model)
        {
            Fonacot fon = new Fonacot();
            var response = fon.CreateFonacot(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}