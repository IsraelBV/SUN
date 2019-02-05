using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SS.WEB.ViewModel;
using FiltrosWeb;
using Common.Enums;
using Common.Helpers;
using RH.BLL;
using SYA.BLL;
namespace SS.WEB.Controllers
{
    public class AccesoController : Controller
    {
        [NoLogin]
        public ActionResult LogIn()
        {
            return View();
        }

        [NoLogin]
        public ActionResult LogInAjax(LogInViewModel model)
        {
            int acceso = 3;
            
            ControlUsuario cu = new ControlUsuario();
           
            var usuario = cu.GetUsuarioByCuenta(model.User, model.Password);
            if (usuario != null)
            {
                var accesoApp = ControlAcceso.AccesoAplicacion(Aplicacion.SeguroSocial, usuario);
                if (accesoApp)
                {
                    SessionHelpers.IniciarSession(usuario.Usuario, usuario.IdUsuario.ToString());
                    Session["usuario"] = usuario;
                    acceso = 1;
                }
                else
                    acceso = 2;
            }
            else
                acceso = 3;
            return Json(acceso, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult LogOut()
        {
            SessionHelpers.CerrarSession();
            return RedirectToAction("LogIn", "Acceso");
        }
    }
}