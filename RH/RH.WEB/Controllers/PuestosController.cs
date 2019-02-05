using FiltrosWeb;
using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Enums;
using RH.BLL;

namespace RH.WEB.Controllers
{
    public class PuestosController : Controller
    {
        private Puestos ctx = new Puestos();
        private Departamentos ctxDep = new Departamentos();

        // GET: Puestos
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Consultar)]
        public ActionResult Index(int id)
        {
            
            var sucursal = Session["sucursal"] as SucursalDatos;
            var modelo = ctx.GetPuestosByDepto(id);
            var depto = ctxDep.GetDepartamentoById(id);
            ViewBag.IdDepartamento = depto.IdDepartamento;
            ViewBag.nombreDepto = depto.Descripcion;
            return View(modelo);
        }

        
        // GET: Puestos/Details/5
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Consultar)]
        public ActionResult Details(int id)
        {
            var modelo = ctx.GetPuesto(id);
            
            return View(modelo);
        }

        // GET: Puestos/Create
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Agregar)]
        public ActionResult viewCreate(int id)
        {
            ViewBag.idDepartamento = id;
            return PartialView();
        }

        // POST: Puestos/Create
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult Create(Puesto collection)
        {
            try
            {
                
                ctx.CrearPuesto(collection);

                return Redirect("~/Puestos/Index/"+collection.IdDepartamento);
            }
            catch
            {
                return View();
            }
        }

        // GET: Puestos/Edit/5
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Editar)]
        public ActionResult viewEdit(int id)
        {
            var modelo = ctx.GetPuesto(id);
            return View(modelo);
        }

        // POST: Puestos/Edit/5
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Editar)]
        [HttpPost]
        public JsonResult Edit(Puesto collection)
        {
            try
            {
                collection.IdDepartamento = collection.IdDepartamento;
                ctx.UpdatePuesto(collection);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Puestos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Puestos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
