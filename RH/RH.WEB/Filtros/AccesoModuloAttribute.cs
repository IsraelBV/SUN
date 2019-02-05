using System.Web.Mvc;
using Common.Enums;
using System.Web.Routing;
using Common.Helpers;
using SYA.BLL;
using RH.Entidades;

namespace FiltrosWeb
{
    public class AccesoModuloAttribute : FilterAttribute, IAuthorizationFilter
    {
        public Modulos Modulo { get; set; }
        public AccionCrud Accion { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!IsValid())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "AccesoDenegado"
                }));
            }
        }

        private bool IsValid()
        {
            var user = ControlAcceso.GetUsuarioEnSession();
            //Si el perfil del usuario es SU se le concede acceso inmediato
            if (user.IdPerfil == 1)
                return true;

            return ControlAcceso.AccesoAccionModulo(Modulo, Accion, user.IdUsuario);
        }
    }
}