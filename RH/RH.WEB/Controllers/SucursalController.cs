using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using Common.Helpers;
using SYA.BLL;
namespace RH.WEB.Controllers
{
    public class SucursalController : Controller
    {

        Sucursales ctx = new Sucursales();

        public ActionResult Index(int id = 0)
        {
            //obtiene los datos principales de la sucursal y los asigna a una variable de sesión


            if(id > 0)
            {
                var suc = new SucursalDatos();

                if (!ControlAcceso.AccesoSucursal(id))
                {
                    return RedirectToAction("AccesoDenegadoSuc", "Home");
                }

                //Si ya existe datos guardado en sesion
                //validamos si el idSucursal es distinto al de la session actual.
                if (Session["Sucursal"] != null)
                {
                  suc = Session["Sucursal"] as SucursalDatos;

                    //Si la variable idSucursal es distinto al IdSucursal de la session
                    //entonces se selecciono una nueva sucursal
                    if (suc != null && suc.IdSucursal != id)
                    {
                        suc = ctx.ObtenerSucursalDatosPorId(id);
                    }
                }
                //Si la session actual es null
                else if (Session["Sucursal"] == null)
                {
                     suc = ctx.ObtenerSucursalDatosPorId(id);
                }


                //Si no se encontró la sucursal
                if(suc == null ) return RedirectToAction("Index", "Home");

                //Agregamos los nuevos datos a la session
                //y guardamos en la lista de mas recientes.
                if (suc.GuardadoEnReciente == false)
                {
                    suc.GuardadoEnReciente = ctx.InsertarSucursalReciente(id);
                    Session["Sucursal"] = suc;
                }

                return View(suc);

            }

            return RedirectToAction("Index", "Home");
        }
    }
}