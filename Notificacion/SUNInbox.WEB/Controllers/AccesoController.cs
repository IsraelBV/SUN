using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using SUNInbox.WEB.ViewModels;
using SYA.BLL;
using Common.Enums;
using Common.Helpers;

namespace SUNInbox.WEB.Controllers
{
    public class AccesoController : Controller
    {
        [NoLogin]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LogIn(LogInViewModel model)
        {
            ControlUsuario cu = new ControlUsuario();
            var user = cu.GetUsuarioByCuenta(model.User, model.Password);
            string[] array = new string[3];
            if(user != null)
            {
                var accesoApp = ControlAcceso.AccesoAplicacion(Aplicacion.Notificaciones, user);
                if (accesoApp)
                {
                    SessionHelpers.IniciarSession(user.Usuario, user.IdUsuario.ToString());
                    Session["usuario"] = user;
                    array[0] = "1";
                    array[1] = user.Nombres;
                    array[2] = user.ApPaterno;
                }
                else
                    array[0] = "2"; //no tiene acceso a la app
            }
            else
                array[0] = "3"; //las credenciales no están bien
            return Json(array, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            SessionHelpers.CerrarSession();
            return RedirectToAction("LogIn", "Acceso");
        }
    }
}