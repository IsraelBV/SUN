using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using System.Data.Entity;
using RH.Entidades;
using FiltrosWeb;
using Common.Enums;

namespace RH.WEB.Controllers
{
    public class IncapacidadesController : Controller
    {
        private IncapacidadesClass ctx = new IncapacidadesClass();

        /// <summary>
        /// GET: Empleados/Incapacidades
        /// </summary>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Consultar)]
        public ActionResult Index(string id)
        {
            Empleados ctxEmpleados = new Empleados();
            var sucursal = Session["sucursal"] as SucursalDatos;
            var modelo = ctxEmpleados.ObtenerEmpleadosConPuesto(sucursal.IdSucursal);

            return View(modelo);
        }

        /// <summary>
        /// GET: Incapacidades/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Consultar)]
        public ActionResult Details(int id)
        {
            var modelo = ctx.GetIncapacidadById(id);
            ViewBag.idEmpleado = id;
            var emp = ctx.GetEmpleadosById(id);
            ViewBag.nombEmp = emp.Nombres;
            ViewBag.apEmp = emp.APaterno;
            ViewBag.amEmp = emp.AMaterno;

            return View(modelo);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Consultar)]
        public ActionResult viewDetails(int id)
        {
            var modelo = ctx.GetIncapacidad(id);
            ViewBag.Id = id;
            return View(modelo);
        }

        /// <summary>
        /// GET: Incapacidades/Create
        /// </summary>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Agregar)]
        public ActionResult viewCreate(int id)
        {
            ViewBag.idEmpleado = id;
            return PartialView();
        }

        /// <summary>
        /// POST: Incapacidades/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult Create(Incapacidades collection)
        {
            try
            {
                collection.Folio = collection.Folio.ToUpper().Trim();
                collection.IdEmpleado = collection.IdEmpleado;
                ctx.CrearIncapacidad(collection);

                return Redirect("~/Incapacidades/Details/"+collection.IdEmpleado);
            }
            catch(Exception e)
            {
                return View();
            }
        }

        /// <summary>
        /// GET: Incapacidades/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Editar)]
        public ActionResult viewEdit(int id)
        {
            string[] arrayTipo = { "Riesgo de trabajo", "Enfermedad General", "Prematernal", "Maternal de enlace", "Postmaternal" };

            string[] arrayClase = { "Inicial", "Subsecuente", "Recaída"};

            var modelo = ctx.GetIncapacidad(id);
            List<SelectListItem> tipo = new List<SelectListItem>();
            for(int i = 0; i < arrayTipo.Length; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = arrayTipo[i];
                item.Value = arrayTipo[i];
                if (arrayTipo[i] == modelo.Tipo)
                {
                    item.Selected = true;
                }
                tipo.Add(item);
            }
            List<SelectListItem> clase = new List<SelectListItem>();
            for (int i = 0; i < arrayClase.Length; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = arrayClase[i];
                item.Value = arrayClase[i];
                if (arrayClase[i] == modelo.Clase)
                {
                    item.Selected = true;
                }
                clase.Add(item);
            }
            ViewBag.listaTipos = tipo;
            ViewBag.listaClases = clase;
            return View(modelo);
        }


        /// <summary>
        /// POST: Incapacidades/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Editar)]
        [HttpPost]
        public JsonResult Edit(int Id, Incapacidades collection)
        {
            try
            {
                collection.Folio = collection.Folio.ToUpper().Trim();
                collection.IdEmpleado = collection.IdEmpleado;
                ctx.UpdateInc(Id, collection);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GET: Incapacidades/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Consultar)]
        public ActionResult viewDelete(int id)
        {
            var lista = ctx.GetIncapacidad(id);
            return View(lista);
        }

        /// <summary>
        /// POST: Incapacidades/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHIncapacidades, Accion = AccionCrud.Consultar)]
        public ActionResult Delete(int id, Incapacidades collection)
        {
            try
            {
                ctx.DeleteInc(id);
                return Redirect("~/Incapacidades/Details/" + collection.IdEmpleado);
            }
            catch
            {
                return View();
            }
        }
    }
}
