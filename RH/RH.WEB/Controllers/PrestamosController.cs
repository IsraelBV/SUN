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
    public class PrestamosController : Controller
    {

        BLL.Prestamos ctx = new BLL.Prestamos();

        [AccesoModuloAttribute(Modulo = Modulos.RHPrestamos, Accion = AccionCrud.Consultar)]
        public ActionResult Index(int? id = null)
        {
            //Se obtiene la sucursal
            var sucursal = Session["Sucursal"] as SucursalDatos;

            //Se obtiene la lista de empleados de la sucursal
            Empleados ctxEmp = new Empleados();
            var empleados = ctxEmp.ObtenerEmpleadosPorSucursal(sucursal.IdSucursal);
            
            //Por cada empleado se busca la cantidad de prestamos que tiene
            foreach (var empleado in empleados)
            {
                var prestamos = ctx.GetNumPrestamosByEmpleado(empleado.IdContrato);
                empleado.Prestamos = prestamos;
            }

            if(id != null)
            {
                return RedirectToAction("Detalle", new { empleado = id });
            }
            return View(empleados);
        }


        [AccesoModuloAttribute(Modulo = Modulos.RHPrestamos, Accion = AccionCrud.Consultar)]
        public ActionResult Detalle(int? empleado)
        {
            if (empleado != null)
            {
                int id = (int)empleado;
                var prestamos = ctx.GetPrestamosByEmpleado(id);
                Empleados ctxEmp = new Empleados();
                ViewBag.Empleado = ctxEmp.GetEmpleadoById(id);

                return View(prestamos);
            }
            else
                return RedirectToAction("Index", "Prestamos");
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHPrestamos, Accion = AccionCrud.Agregar)]
        public ActionResult Nuevo(int id)
        {
            if (id > 0)
            {
                Empleados ctxEmpleado = new Empleados();
                ViewBag.Empleado = ctxEmpleado.GetEmpleadoPrestamoDetalle(id);
                return View();
            }
            else
                return RedirectToAction("Index", "Prestamos");
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHPrestamos, Accion = AccionCrud.Agregar)]
        public ActionResult Create(Entidades.Prestamos prestamo)
        {
            ctx.Create(prestamo);
            return RedirectToAction("Index", "Prestamos");
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHPrestamos, Accion = AccionCrud.Consultar)]
        public PartialViewResult Historial(int id)
        {
            var model = ctx.GetHistorial(id);
            return PartialView(model);
        }

    }
}