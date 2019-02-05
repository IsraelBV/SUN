using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using Nomina.WEB.Models;
using SYA.BLL;
using Common.Enums;
using Common.Helpers;

namespace Nomina.WEB.Controllers
{
    public class AccesoController : Controller
    {
        [NoLogin]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AutenticarNombreUsuario(string username)
        {
            LogIn credentials = new LogIn();
            ControlUsuario cu = new ControlUsuario();

            try
            {
                var user = cu.UserNameExists(username);
                if (user != null)
                {
                    var acceso = ControlAcceso.AccesoAplicacion(Aplicacion.Nominas, user);

                    if (acceso)
                    {
                        credentials.Acceso = true;
                        credentials.foto = cu.GetProfilePicture(user.IdUsuario);

                        credentials.nombre = $"{user.Nombres} {user.ApPaterno}";
                    }
                    else
                    {
                        credentials.Acceso = false;
                        credentials.Error = 1;
                    }

                }
                else
                {
                    credentials.Acceso = false;
                    credentials.Error = 2;
                }
            }
            catch (InvalidOperationException) //usado para capturar la excepcion cuando no se establece la conecion al servidor
            {
                credentials.Acceso = false;
                credentials.Error = 3;
            }


            return Json(credentials, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutenticarUsuario(string username, string password)
        {
            var acceso = false;

            ControlUsuario cu = new ControlUsuario();
            var user = cu.GetUsuarioByCuenta(username, password);
            if (user != null)
            {
                acceso = true;
                SessionHelpers.IniciarSession(username, user.IdUsuario.ToString());
            }
            return Json(acceso, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            SessionHelpers.CerrarSession();

            //Cerrar las valriables de sessiones
            Session["periodo"] = null;
            Session["sucursal"] = null;

            return RedirectToAction("LogIn");
        }
    }
}