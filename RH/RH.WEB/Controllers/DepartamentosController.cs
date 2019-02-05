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
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.Rh)]
    public class DepartamentosController : Controller
    {
        private Departamentos ctx = new Departamentos();

        /// <summary>
        /// GET: Departamentos
        /// </summary>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Consultar)]
        public ActionResult Index(int sucursal)
        {

            var idcliente = ctx.GetIdCliente(sucursal);
            var list = ctx.GetDepartamentosbyCliente(idcliente.IdCliente);

            ////Por cada departamento se busca la cantidad de puestos que tiene
            //foreach (var depto in list)
            //{
            //    var puestos = ctx.GetNumPuestosByDepartamento(depto.IdDepartamento);
            //    depto.Puestos = puestos;
            //}

            return View(list);
        }

        /// <summary>
        /// GET: Departamentos/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Consultar)]
        public ActionResult Details(int id)
        {
            var modelo = ctx.GetDepartamentoById(id);
            return View(modelo);
        }

        /// <summary>
        /// GET: Departamentos/Create
        /// </summary>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Agregar)]
        public ActionResult NuevoRegistro()
        {
            return View();
        }

        /// <summary>
        /// POST: Departamentos/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult NuevoRegistro(Departamento collection,int sucursal)
        {
            try
            {
                collection.Descripcion = collection.Descripcion.Trim();
                ctx.CrearDepartamento(collection);

         
                return RedirectToAction("Index", "Departamentos", new { sucursal = sucursal });
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// GET: Departamentos/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Editar)]
        public ActionResult Edit(int id)
        {
            var modelo = ctx.GetDepartamentoById(id);
            return View(modelo);
        }

        /// <summary>
        /// POST: Departamentos/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Editar)]
        [HttpPost]
        public ActionResult Edit(int id, Departamento collection,int sucursal)
        {
            try
            {
                collection.Descripcion = collection.Descripcion.Trim();
                ctx.UpdateDepa(id, collection);

                return RedirectToAction("Index", "Departamentos", new { sucursal = sucursal });
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// GET: Bancos/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Eliminar)]
        public ActionResult viewDelete(int id)
        {
            var modelo = ctx.GetDepartamentoById(id);
            var depto = ctx.GetDepartamentoById(id);
            ViewBag.nombreDepto = depto.Descripcion;

            return View(modelo);
        }

        /// <summary>
        /// POST: Departamentos/Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Eliminar)]
        public ActionResult Delete(int id)
        {
            try
            {
                ctx.DeleteDepa(id);

                return RedirectToAction("Index", "Departamentos");
            }
            catch
            {
                return View();
            }
        }

        public PartialViewResult _DepartamentosCliente(int sucursal)
        {
            var idcliente = ctx.GetIdCliente(sucursal);
            var list = ctx.GetDepartamentosbyCliente(idcliente.IdCliente);
            return PartialView(list);
        }

        public PartialViewResult _ListaDepartamentos(int sucursal)
        {
            var idcliente = ctx.GetIdCliente(sucursal);
            List<DepartamentoDatos> depas = ctx.GetDepartamentosbyCliente(idcliente.IdCliente);
            ViewBag.Depto = depas;
            var List = ctx.GetDepartamentos();
            return PartialView(List);
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHDepartamentos, Accion = AccionCrud.Agregar)]
        
        public ActionResult ActivarDepartamento (int IdDepa,int sucursal)
        {
           
                var cliente = ctx.GetIdCliente(sucursal);
                var response = ctx.AsignarDepa(IdDepa,cliente.IdCliente);
                return Json(response, JsonRequestBehavior.AllowGet);
           

        }
    }
}