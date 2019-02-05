using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using RH.BLL;
using Nomina.BLL;
using RH.Entidades;
using Common.Helpers;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    public class HomeController : Controller
    {
             
        public ActionResult Index()
        {
            if(Session["activarComplemento"] == null)
            {
                Session["activarComplemento"] = 0;
            }
         
            return View();
        }

        public PartialViewResult GetClientes()
        {
            if (Session["activarComplemento"] == null)
            {
                ViewBag.SesionComplemento = 0;
            }else
            {
                ViewBag.SesionComplemento = Session["activarComplemento"];
            }
                
            Sucursales s = new Sucursales();

            var sucursales = s.GetSucursalesByUser();

            return PartialView(sucursales);
        }

        /// <summary>
        /// Guarda la sucursal en la sesión
        /// </summary>
        /// <param name="id"></param>
        [HttpPost]
        public JsonResult SelectSucursal(int id)
        {
            Sucursales ctx = new Sucursales();
            var sucursal = ctx.ObtenerSucursalDatosPorId(id);
            Session["sucursal"] = sucursal;
            if(Session["periodo"] != null)
            {
                Session["periodo"] = null;
            }
            return Json(sucursal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ActivarComplemento(int activar )
        {
           Session["activarComplemento"] = activar;
            
        }

        public PartialViewResult GetPeriodosPago(int id = 0)
        {
            var idusuario = SessionHelpers.GetIdUsuario();
            int departamento = 0;
            
            var sucursal = Session["sucursal"] as SucursalDatos;
            var periodos = new List<NOM_PeriodosPago>();
            if(sucursal != null)
            {
                PeriodosPago ctx = new PeriodosPago();
                
                var periodosYDepartamento = ctx.GetPeriodosPagoBySucursal(sucursal.IdSucursal, id, idusuario);
                departamento = periodosYDepartamento.Item2;
                periodos = periodosYDepartamento.Item1;
                periodos = periodos.OrderByDescending(x => x.IdPeriodoPago).ToList();

                ViewBag.SelectedEf = id;
                ViewBag.Ejercicios = sucursal.Ejercicios;
            }                   

            ViewBag.usuarios = departamento;
            return PartialView(periodos);
        }

        public JsonResult SelectPeriodo(int id)
        {
            PeriodosPago ctx = new PeriodosPago();
            var periodo = ctx.GetPeriodoPagoById(id);
            Session["periodo"] = periodo;
            return Json(periodo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PeriodoProcesando()
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            if (periodo == null) return Json(false, JsonRequestBehavior.AllowGet);

            PeriodosPago pp = new PeriodosPago();
            var procesando = pp.PeriodoEnProceso(periodo.IdPeriodoPago);
            periodo.Procesando = procesando;
            Session["periodo"] = periodo;
            return Json(procesando, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult NoSucursal()
        {
            return PartialView();
        }

        public PartialViewResult NoPeriodo()
        {
            return PartialView();
        }

        public PartialViewResult PeriodoNoProcesado(bool isFiniquito = false)
        {
            ViewBag.Finiquito = isFiniquito;
            return PartialView();
        }

        public PartialViewResult PeriodoNoAutorizado(bool isFiniquito = false)
        {
            ViewBag.Finiquito = isFiniquito;
            return PartialView();
        }

        [HttpPost]
        public JsonResult GetPeriodoInfo(int id = 0)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;

            if (sucursal != null)
            {

                PeriodosPago pp = new PeriodosPago();
                var result = pp.GetPeriodoInfo(sucursal.IdSucursal, id);


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("{}", JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult SetPeriodoPagado(int IdPeriodo) {
            PeriodosPago pp = new PeriodosPago();
            var result = pp.SetPeriodoPago(IdPeriodo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}