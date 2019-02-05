using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;
using Common.Enums;
using Common.Helpers;
using FiltrosWeb;
using System.Data;

namespace RH.WEB.Controllers
{
    public class CumpleIMSSController : Controller
    {
        private CumpleIMSS ctx = new CumpleIMSS();
        // GET: CumpleIMSS
        public ActionResult Index()
        {
            cumpleIMSS cumple = new cumpleIMSS();
            var empresas = new Empresas().getEmpresasByUser(Session["Usuario"] as SYA_Usuarios);
            var clientes = new Clientes().getClientesByUser(Session["Usuario"] as SYA_Usuarios);
            var sucursales = new Sucursales().getSucursalesByUser(Session["Usuario"] as SYA_Usuarios);
            var clienteSucursal = cumple.obtenerSucursal(sucursales);
            ViewBag.empresas = empresas;
            ViewBag.clientes = clientes;
            ViewBag.sucursales = clienteSucursal;
            var sucursal = Session["Sucursal"] as SucursalDatos;
            cumpleIMSS cump = new cumpleIMSS();
            var empleadosimss = cump.cumpleActuaal(sucursal.IdSucursal);
            return View(empleadosimss);
        }
        public ActionResult cumpleIMSSEmpresa(int Idempresa, DateTime? FechaInicio, DateTime? FechaFin)
        {
            DateTime FI = new DateTime();
            DateTime FF = new DateTime();
            
            FI = FechaInicio == null ? DateTime.Now : FechaInicio.Value;
            FF = FechaFin == null ? DateTime.Now : FechaFin.Value;

            cumpleIMSS cump = new cumpleIMSS();

            var empleados =cump.cumpleByEmpresa(Idempresa, FI, FF);
            return PartialView(empleados);
        }

        public ActionResult cumpleIMSSCliente(int IdCliente, DateTime? FechaInicio, DateTime? FechaFin)
        {
            DateTime FI = new DateTime();
            DateTime FF = new DateTime();

            FI = FechaInicio == null ? DateTime.Now : FechaInicio.Value;
            FF = FechaFin == null ? DateTime.Now : FechaFin.Value;

            cumpleIMSS cump = new cumpleIMSS();
            var empleados = cump.cumpleByCliente(IdCliente, FI, FF);
            return PartialView(empleados);
        }

        public ActionResult cumpleIMSSSucursal(int IdSucursal, DateTime? FechaInicio, DateTime? FechaFin)
        {
            DateTime FI = new DateTime();
            DateTime FF = new DateTime();

            FI = FechaInicio == null ? DateTime.Now : FechaInicio.Value;
            FF = FechaFin == null ? DateTime.Now : FechaFin.Value;

            cumpleIMSS cump = new cumpleIMSS();
            var empleados = cump.cumpleBySucursal(IdSucursal, FI, FF);
            return PartialView(empleados);
        }

        public JsonResult crearRegistro (List<CumpleIMSS> cumple)
        {
            cumpleIMSS cump = new cumpleIMSS();
            var IdUser = SessionHelpers.GetIdUsuario();
     

               var cumpleimss =  cump.registrarCumpleIMSS(cumple, IdUser);
                
            
            
            return Json(cumpleimss, JsonRequestBehavior.AllowGet);
        }
    }
}