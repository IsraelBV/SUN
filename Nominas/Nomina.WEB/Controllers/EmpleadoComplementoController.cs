using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using RH.Entidades;
using Nomina.WEB.Filtros;
using Common.Helpers;
using FiltrosWeb;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SesionPeriodo]
    [SesionSucursal]
    public class EmpleadoComplementoController : Controller
    {
        public PartialViewResult Index()
        {
            //var periodo = Session["periodo"] as NOM_PeriodosPago;
            //var ctx = new Emp_Comp();
            //ctx.UpdateEmpleadoComplementoRegistros(periodo.IdPeriodoPago); //actualiza los registros
            //var empleados = ctx.GetEmpleadosConConceptosComplemento(periodo.IdPeriodoPago);
            
            int i = 0;
            int j = 0;
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            int idUsuario = SessionHelpers.GetIdUsuario();
            var ctx = new Emp_Comp();
            var configuracion = ctx.ObtenerConfiguracion(idUsuario, periodo.IdSucursal, 18);
            var empleados = ctx.empleadoComplemento(periodo.IdPeriodoPago);
            if(configuracion != null)
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
            }else
            {
                if (empleados.Count > 0)
                {
                    var emp = empleados[0];
                    int[] arrayVisible = new int[emp.listaconceptos.Count];
                    int[] arrayOculto = {};
                    arrayVisible = emp.listaconceptos.Select(x => x.idConcepto).ToArray();
                    ViewBag.visible = arrayVisible;
                    ViewBag.oculto = arrayOculto;
                }
            }
       
            return PartialView(empleados);
        }
        [HttpPost]
        public ActionResult guardarComplemento(int IdEmpleado, int IdConcepto, decimal Cantidad)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var ctx = new Emp_Comp ();
            ctx.guardarComplemento(IdEmpleado, periodo.IdPeriodoPago, IdConcepto, Cantidad);
            return null;
        }
        //public PartialViewResult GetDetails(int idEmpleado)
        //{
        //    var periodo = Session["periodo"] as NOM_PeriodosPago;
        //    var ctx = new Emp_Comp();
        //    var model = ctx.GetEmpleadoComplemento(idEmpleado, periodo.IdPeriodoPago);
        //    return PartialView(model.OrderBy(x => x.IdConcepto).ToList());
        //}

        public PartialViewResult GetDetailsMixed(int[] empleados)
        {
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var ctx = new Emp_Comp();
            var model = ctx.GetEmpleadoComplemento(empleados, periodo.IdPeriodoPago);
            ViewBag.Empleados = empleados;
            return PartialView(model);
        }

        public JsonResult GuardarCantidadConcepto(int IdEmpleadoComplemento, decimal Cantidad)
        {
            var ctx = new Emp_Comp();
            var result = ctx.UpdateEmpleadoComplemento(IdEmpleadoComplemento, Cantidad);
            var message = "Registro actualizado correctamente.";
            if (result < 1)
            {
                message = "La cantidad no se modificó";
            }
            return Json(new { code = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GuardarCantidadConceptoMixed(int[] idsEmpleado, int IdConcepto, decimal Cantidad)
        {
            var ctx = new Emp_Comp();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var result = ctx.UpdateEmpleadoComplemento(idsEmpleado, IdConcepto, periodo.IdPeriodoPago, Cantidad);
            var message = result + " registros actualizados correctamente.";
            if (result < 1)
            {
                message = "La cantidad no se modificó";
            }
            return Json(new { code = result, message = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardarConceptosVO(int[] visibles, int[] ocultos)
        {

            var ctx = new Emp_Comp();
            var mensaje = "";
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            int idUsuario = SessionHelpers.GetIdUsuario();
            var result = ctx.GuardarConfiguracion(idUsuario, periodo.IdSucursal, visibles, ocultos, 18);
            if(result == true)
            {
                mensaje = "mensaje guardado";
            }

            return Json(new { message = mensaje }, JsonRequestBehavior.AllowGet);
        }

    }
}