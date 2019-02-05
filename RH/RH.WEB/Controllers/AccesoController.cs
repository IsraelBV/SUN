using System.Web.Mvc;
using Common.Enums;
using Common.Helpers;
using RH.BLL;
using RH.WEB.ViewModel;
using FiltrosWeb;
using SYA.BLL;
namespace RH.WEB.Controllers
{
 
    public class AccesoController : Controller
    {
        // GET: Acceso
        [NoLogin]
        public ActionResult LogIn()
        {
            return View();
        }

        [NoLogin]
        public ActionResult LogInAJAX(string user, string password)
        {
            ControlUsuario cu = new ControlUsuario();
            int acceso = 0;
            var usuario = cu.GetUsuarioByCuenta(user, password);
            if(usuario != null)
            {
                var accesoApp = ControlAcceso.AccesoAplicacion(Aplicacion.Rh, usuario);
                if(accesoApp)
                {
                    SessionHelpers.IniciarSession(usuario.Usuario, usuario.IdUsuario.ToString());
                    Session["usuario"] = usuario;
                    acceso = 1;
                }
                else
                {
                    acceso = 2;
                }

            }
            else
            {
                acceso = 3;
            }
            return Json(acceso, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LogOut()
        {
           SessionHelpers.CerrarSession();
            //return Redirect("~/");
            return RedirectToAction("LogIn", "Acceso");
        }

    }
}