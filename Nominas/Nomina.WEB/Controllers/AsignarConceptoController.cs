using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using RH.BLL;
using RH.Entidades;
using FiltrosWeb;
namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class AsignarConceptoController : Controller
    {
        // GET: AsignarConcepto
        AsignacionConceptos asig = new AsignacionConceptos();
        public ActionResult Index()
        {

            //Inicializamos la variables
            TempData["emplPeriodo"] = null;
            //Catalogos catalogo = new Catalogos();
            // var lista = catalogo.ListadoConceptos ();
            AsignacionConceptos conceptos = new AsignacionConceptos();
            var lista = conceptos.ListadoConcepto();
            return PartialView(lista);
        }
        public ActionResult BusquedaCE(int id)
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (sucursal == null || periodoPago == null)
            {
                return null;
            }
            else
            {
                Empleados emp = new Empleados();
                
                AsignacionConceptos concep = new AsignacionConceptos();
                List<int> listaIdEmple = new List<int>();

                // var list = emp.ObtenerEmpleadosPorSucursal(sucursal.IdSucursal);
                //var list = emp.empleadosPorSucursalNOM(sucursal.IdSucursal);
                //var list = emp.idEmpleados(sucursal.IdSucursal);
                if (TempData["emplPeriodo"] != null)
                {
                    listaIdEmple = TempData["emplPeriodo"] as List<int>;
                    TempData["emplPeriodo"] = listaIdEmple;
                }
                else
                {
                    listaIdEmple = emp.GetIdEmpleados(sucursal.IdSucursal, periodoPago.IdPeriodoPago);
                    TempData["emplPeriodo"] = listaIdEmple;
                }

                var listaconcepto= asig.busquedaEmplComp(id, listaIdEmple);
                ViewBag.checkConcepto = concep.checkConcepto(id);

                return PartialView(listaconcepto);
            }

        }
        [HttpPost]
        public ActionResult GuardarCon_Empl(int [] arrayC, List<ConceptoEmpleado> arrayE)
        {
            string strMensaje = "Se guardo Correctamente";

            var periodoPago = Session["periodo"] as NOM_PeriodosPago;
            if (periodoPago != null)
            {
                if (periodoPago.Autorizado)
                {
                    strMensaje = "No se puede aplicar cambios a un periodo Autorizado.";
                }
                else
                {
                    var sucursal = Session["sucursal"] as SucursalDatos;
                    asig.GuardarRegistroConcepto_Empleado(arrayC, arrayE, sucursal.IdSucursal);
                }
            } 

            return Json(new {resultado = strMensaje },JsonRequestBehavior.AllowGet);
        }

     }

   
}