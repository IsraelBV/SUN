using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using System.Threading.Tasks;
using FiltrosWeb;

using RH.BLL;
namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ConfiguracionController : Controller
    {
        private Configuraciones configuracion = new Configuraciones();

        // GET: Configuracion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexSubsidio()
        {
            return PartialView();
        }

        public ActionResult TablaSubsidio(int id)
        {
            ViewBag.id = id;
            var listasub = configuracion.ListSubsidio();
            return PartialView(listasub);
        }
        public ActionResult IndexISR()
        {
            return PartialView();
        }
        public ActionResult TablaISR(int id)
        {
            ViewBag.id = id;
            var listaisr = configuracion.ListISR();
            return PartialView(listaisr);
        }
        public ActionResult TablaIMSS()
        {
            var listasub = configuracion.ListIMSS();
            return PartialView(listasub);
        }

        public ActionResult ClavesContables()
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            if (sucursal == null)
            {
                return null;
            }
            else
            {
                
                Sucursales sucu = new Sucursales();

                var ss = sucu.GetSucursalById(sucursal.IdSucursal);

                var s = sucu.ListSucursalEmpresa(sucursal.IdSucursal);

                return PartialView(s);
            }

        }
        public ActionResult EmpresasSucursal()
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            if (sucursal == null)
            {
                return PartialView(null);
            }
            else
            {
                Sucursales sucu = new Sucursales();

              //  var ss = sucu.GetSucursalById(sucursal.IdSucursal);

                var s = sucu.ListSucEmp(sucursal.IdSucursal);

                return PartialView(s);
            }
        }
        [HttpPost]
        public ActionResult EditarClaveContable(int id, string ClaveContable)
        {
            Sucursales sucursal = new Sucursales();
            var editar = sucursal.UpdateClave(id, ClaveContable);
            return null;
        }



    }
}