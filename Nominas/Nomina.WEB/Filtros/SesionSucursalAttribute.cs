using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using RH.BLL;
using RH.Entidades;
using Common.Helpers;


namespace Nomina.WEB.Filtros
{
    public class SesionSucursalAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["Sucursal"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/NoSucursal");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class SesionPeriodoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["Periodo"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/NoPeriodo");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class PeriodoProcesadoAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["Periodo"] != null)
            {
                var periodo = HttpContext.Current.Session["Periodo"] as NOM_PeriodosPago;
                if (periodo == null)
                {
                    filterContext.Result = new RedirectResult("~/Home/NoPeriodo");
                    return;
                }

                if (periodo.Procesado != true)
                {
                    var isFiniquito = periodo.IdTipoNomina == 11;
                    filterContext.Result = new RedirectResult("~/Home/PeriodoNoProcesado?isFiniquito="+ isFiniquito);
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }

    }

    public class PeriodoAutorizadoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["Periodo"] != null)
            {
                var periodo = HttpContext.Current.Session["Periodo"] as NOM_PeriodosPago;

                if (periodo == null)
                {
                    filterContext.Result = new RedirectResult("~/Home/NoPeriodo");
                    return;
                }

                if (periodo.Autorizado != true)
                {
                    var isFiniquito = periodo.IdTipoNomina == 11;
                    filterContext.Result = new RedirectResult("~/Home/PeriodoNoAutorizado?isFiniquito=" + isFiniquito);
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }

    }

    public class SessionUserExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (HttpContext.Current.Session["usuario"] == null)
            {
                SessionHelpers.CerrarSession();
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                 {
             { "action", "LogIn" },
            { "controller", "Acceso" },
            { "returnUrl", filterContext.HttpContext.Request.RawUrl}
                  });

                return;
            }
        }

    }
}