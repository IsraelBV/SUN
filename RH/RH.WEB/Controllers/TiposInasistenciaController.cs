using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.Entidades;
using FiltrosWeb;
using Common.Enums;
using RH.BLL;

namespace RH.WEB.Controllers
{
    public class TiposInasistenciaController : Controller
    {
        private IncidenciasClass ctx = new IncidenciasClass();

        // GET: Incidencias
        [AccesoModuloAttribute(Modulo = Modulos.RHTiposDeIncidencias, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var modelo = ctx.GetIncidencias();
            return View(modelo);
        }


        // GET: Incidencias/Details/5
        [AccesoModuloAttribute(Modulo = Modulos.RHTiposDeIncidencias, Accion = AccionCrud.Consultar)]
        public ActionResult Details(int id)
        {
            var modelo = ctx.GetIncidenciasById(id);
            return View(modelo);
        }


        // GET: Incidencias/Create
        [AccesoModuloAttribute(Modulo = Modulos.RHTiposDeIncidencias, Accion = AccionCrud.Agregar)]
        public ActionResult viewCreate()
        {
            return PartialView();
        }

        // POST: Incidencias/Create
        [AccesoModuloAttribute(Modulo = Modulos.RHTiposDeIncidencias, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult Create(C_TiposInasistencia collection)
        {
            try
            {
                collection.Clave = collection.Clave.ToUpper().Trim();
                ctx.CrearIncidencia(collection);

                return Redirect("~/Incidencias/Index");
            }
            catch
            {
                return View("Index");
            }
        }

        // GET: Incidencias/Edit/5
        [AccesoModuloAttribute(Modulo = Modulos.RHTiposDeIncidencias, Accion = AccionCrud.Editar)]
        public ActionResult viewEdit(int id)
        {
            ViewBag.Id = id;
            string[] arrayTipoPago = { "Días", "Horas" };
            int[] arrayTP = { 1, 2 };
            var modelo = ctx.GetIncidenciasById(id);
            List<SelectListItem> tipoPago = new List<SelectListItem>();
            for(int i = 0; i < arrayTipoPago.Length; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = arrayTipoPago[i];
                item.Value = (i+1).ToString();
                if (arrayTP[i] == modelo.TipoPago)
                {
                    item.Selected = true;
                }
                tipoPago.Add(item);
            }
            ViewBag.listaTipoPago = tipoPago;
            return View(modelo);
        }

        // POST: Incidencias/Edit/5
        [AccesoModuloAttribute(Modulo = Modulos.RHTiposDeIncidencias, Accion = AccionCrud.Editar)]
        [HttpPost]
        public ActionResult Edit(int id, C_TiposInasistencia collection)
        {
            try
            {
                collection.Clave = collection.Clave.ToUpper().Trim();
                ctx.UpdateInc(id, collection);

                return RedirectToAction("Index", "Incidencias");
            }
            catch
            {
                return View();
            }
        }

        // GET: Incidencias/Delete/5
        [AccesoModuloAttribute(Modulo = Modulos.RHTiposDeIncidencias, Accion = AccionCrud.Eliminar)]
        public ActionResult viewDelete(int id)
        {
            var lista = ctx.GetIncidenciasById(id);
            return View(lista);
        }

        // POST: Incidencias/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                ctx.DeleteInc(id);

                return RedirectToAction("Index", "Incidencias");
            }
            catch
            {
                return View();
            }
        }
    }
}
