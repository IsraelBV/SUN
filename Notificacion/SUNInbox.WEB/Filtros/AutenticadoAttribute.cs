using System.Web.Mvc;
using System.Web.Routing;
using Common.Helpers;


namespace FiltrosWeb
{
    public class AutenticadoAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!SessionHelpers.IsAutenticado())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                { controller = "Acceso", action = "LogIn" }));
            }
        }
    }


    public class NoLoginAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filtro)
        {

            if (SessionHelpers.IsAutenticado())
            {
                filtro.Result = new RedirectToRouteResult(
                   new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
        }
    }
}