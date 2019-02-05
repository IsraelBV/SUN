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
    public class RegistrosPatronalesController : Controller
    {
        private Empresas registro = new Empresas();
        // GET: RegistrosPatronales

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            return View(registro.GetEstadosByIQueryable());
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Editar)]
        public ActionResult editar(int id)
        {
            //1 obtengo el registro de la empresa que deseamos editar
            var modelo = registro.GetRegistroPatronalById(id);

            //2 obtengo la lista de todos los estados
            var edos = new Estados();
            var lista = edos.GetEstados();

            //2 obtengo la lista de todos los regimenes  fiscales
            Empresas regimen = new Empresas();
            var reg = regimen.Regimen();


            //3 hacemos una lista de tipo select list para usarla en el combobox de estados
            var listaEstados = lista.Select(x => new SelectListItem()
            {
                Value = x.IdEstado.ToString(),
                Text = x.Descripcion,
                Selected = (x.IdEstado == modelo.IdEstado) //buscamos en la lista de estados que coincida con el id de mi modelo y lo ponemos como seleccionado
                }).ToList();

            //3 hacemos una lista tipo select list para usarla en el combobox de regimen fiscal
            var listaRegimen = reg.Select(x => new SelectListItem()
            {
                Value = x.IdRegimenFiscal.ToString(),
                Text = x.Descripcion,
                Selected = (x.IdRegimenFiscal == modelo.RegimenFiscal)
            }).ToList();

            ViewBag.RegimenFiscalSelectList = listaRegimen;
            ViewBag.EstadoLista = listaEstados;

            return View(modelo);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Editar)]

        [HttpPost]
        public ActionResult editar(int id, RegistroDatos collection)
        {

            registro.UpdateRegistroPatronal(id, collection);
            return RedirectToAction("Index");


        }
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Agregar)]
        public ActionResult NuevoRegistro()
        {
            var edos = new Estados();
            var lista = edos.GetEstados();

            var listaEstados = lista.Select(x => new SelectListItem()
            {
                Value = x.IdEstado.ToString(),
                Text = x.Descripcion,
                Selected = (x.IdEstado == 1)
            }).ToList();
            Empresas regimen = new Empresas();
            var reg = regimen.Regimen();
            ViewBag.Regimen = reg;
            ViewBag.EstadoLista = listaEstados;


            return View();
        }



        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult NuevoRegistro(Empresa collection)
        {
            try
            {
                if (collection.RegistroPatronal == null)
                {
                    collection.Clase = null;
                    registro.CreateRegistroPatronal(collection);
                }
                else
                {
                    registro.CreateRegistroPatronal(collection);
                }


                return RedirectToAction("Index", "RegistrosPatronales");
            }
            catch
            {
                return View();
            }
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Eliminar)]
        public ActionResult Delete(int id, Empresas collection)
        {
            registro.DeleteRegistroPatronal(id, collection);
            return Redirect("~/RegistrosPatronales/index");
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Eliminar)]
        public ActionResult DropEstados(int id, int idEstado)
        {
            var edos = new Estados();
            var lista = edos.GetEstados();

            ViewBag.IdEstadoSelect = idEstado;

            return View(lista);
        }

        public ActionResult Detalles (int id)
        {
            var detalle = registro.GetRegistroPatronalById(id);
     
            var edos = new Estados();
            var lista = edos.GetEstados();
            var listaEstados = lista.Select(x => new SelectListItem()
            {
                Value = x.IdEstado.ToString(),
                Text = x.Descripcion,
                Selected = (x.IdEstado == detalle.IdEstado)
            }).ToList();


            ViewBag.EstadoLista = listaEstados;
            
            return PartialView("_DetallesEmpresa", detalle);
        }

        public ActionResult DetallesEliminar(int id)
        {
            var eliminar = registro.GetRegistroPatronalById(id);
            var edos = new Estados();
            var lista = edos.GetEstados();
            var listaEstados = lista.Select(x => new SelectListItem()
            {
                Value = x.IdEstado.ToString(),
                Text = x.Descripcion,
                Selected = (x.IdEstado == eliminar.IdEstado)
            }).ToList();


            ViewBag.EstadoLista = listaEstados;

            return PartialView("_DetallesEliminar", eliminar);
        }

        public ActionResult EmpresasInactivas()
        {
            var empreina = registro.GetEstadosByIQueryable();
            return PartialView("_EmpresasInactivas", empreina);
        }
        public ActionResult EmpresasActivas()
        {
            var empreacti = registro.GetEstadosByIQueryable();
            return PartialView("_EmpresasActivas", empreacti);
        }
        public ActionResult ActivarEmpresa(int idempresa)
        {

            registro.ActivarEmpresa(idempresa);
            return RedirectToAction("Index","RegistrosPatronales");

        }
        public ActionResult DesactivarEmpresa(int idempresa3)
        {

            registro.DesactivarEmpresa(idempresa3);
            return RedirectToAction("Index", "RegistrosPatronales");

        }
    }




}