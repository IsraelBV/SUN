using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;
using FiltrosWeb;
using Common.Enums;

namespace RH.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.Rh)]
    public class FactoresDeIntegracionController : Controller
    {
        private FactoresDeIntegracion factor = new BLL.FactoresDeIntegracion();
        // GET: FactoresDeIntegracion
        [AccesoModuloAttribute(Modulo = Modulos.RHFDI, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            return View(factor.GetFactoresIntegracion());
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHFDI, Accion = AccionCrud.Editar)]
        public ActionResult Editar(int id)
        {
            return View(factor.GetFactorIntregracioByID(id));
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHFDI, Accion = AccionCrud.Editar)]
        [HttpPost]
        public ActionResult Editar(int id, C_FactoresIntegracion collection,decimal factor2 =0, int anti=0,decimal prima=0 )
        {
            collection.Antiguedad = anti;
            collection.Factor = factor2;
            collection.PrimaVacacional = prima;
            factor.UpdateFactor(id, collection);
            return RedirectToAction("Index");


        }
        [AccesoModuloAttribute(Modulo = Modulos.RHFDI, Accion = AccionCrud.Agregar)]
        public ActionResult NuevoFactor()
        {
           var ultimofac= factor.GetUltimoFactor();
            if (ultimofac == null)
            {
                ViewBag.Factor = null;
            }
            else
            {
                ViewBag.Factor = ultimofac.Antiguedad;
            }
            return View();
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHFDI, Accion = AccionCrud.Agregar)]
        [HttpPost]
      
        public ActionResult NuevoFactor(C_FactoresIntegracion collection, decimal factor2, int Anti)
        {
            collection.Antiguedad = Anti;
            collection.Factor = factor2;
                factor.CrearFactor(collection);
                return RedirectToAction("Index");
            
            
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHFDI, Accion = AccionCrud.Eliminar)]
        public ActionResult Borrar(int id)
        {
            try
            {
                factor.BorrarFactor(id);
                return Redirect("~/FactoresDeIntegracion/index");
            }
            catch
            {
                return View();
            }

            
        }
    }
}