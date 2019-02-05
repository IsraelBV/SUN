using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Common.Helpers;
using SYA.BLL;
using RH.Entidades;


namespace FiltrosWeb
{
    public class AccesoSucursalAttribute : FilterAttribute, IAuthorizationFilter
    {
        public string Sucursal { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!IsValid())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "AccesoDenegadoSuc"
                }));
            }
        }

        public bool IsValid()
        {
            var user = ControlAcceso.GetUsuarioEnSession();
            //Si el perfil del usuario es SU se le concede acceso inmediato
            if (user.IdPerfil == 0)
                return true;

            var reg = HttpContext.Current.Session["Sucursal"] as Sucursal;

            if (reg == null) return false;

            return ControlAcceso.AccesoSucursal(reg.IdSucursal);
        }


    }
}