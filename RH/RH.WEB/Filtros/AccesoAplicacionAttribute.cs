using System.Web.Mvc;
using Common.Enums;
using SYA.BLL;
using RH.Entidades;

namespace FiltrosWeb
{
    public class AccesoAplicacionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public Aplicacion App { get; set; }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!IsValid())
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public bool IsValid()
        {
            //Obtenemos lo datos del usuario que esta en session
            // para acceder a su lista de aplicaciones configurada
            var us = ControlAcceso.GetUsuarioEnSession();

            if (us == null)
                return false;
             
            //Si el usuario es SU se le concede acceso inmediato
            if (us.IdPerfil == 1)
                return true;

            //validamos que tenga acceso a la aplicacion actual y pasamos el tipo de aplicacion
            var acceso = ControlAcceso.AccesoAplicacion(App, us);
            return acceso;
        }


    }
}