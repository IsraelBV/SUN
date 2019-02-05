using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RH.WEB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            //Exception ex = Server.GetLastError();

            ////Borramos el contenido del buffer 
            //Response.Clear();

            //HttpException httpException = ex as HttpException;

            ////404
            ////Si la variable != null se toma el codigo de la excepcion sino se asigna cero
            //var codError = httpException?.GetHttpCode() ?? 0;



            ////--- --- --- Serr
            //Server.ClearError();
            //Response.Redirect("~/Home/GetErrorHttp/?id=" + codError);


        }
    }
}
