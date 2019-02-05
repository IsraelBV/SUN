using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RH.Entidades;
using System.Web.Mvc;
using Nomina.WEB.Filtros;
using RH.Entidades.GlobalModel;
using FiltrosWeb;
using Nomina.BLL;
using RH.BLL;
using Common.Helpers;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SesionSucursal]
    [SesionPeriodo]
    public class LayoutBancarioController : Controller
    {
        public ActionResult Index()
        {
            LayoutBancario layoutBancario = new LayoutBancario();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            ViewBag.periodo = periodo;
            var sucursal = Session["sucursal"] as SucursalDatos;
            var empresas = layoutBancario.ListaEmpresas(sucursal.IdSucursal);
            var bancos = layoutBancario.ListaBancos(periodo.IdPeriodoPago, periodo.IdTipoNomina);           
            //DataInit datainit = new DataInit();
            var datainit = new DataInit
            {
                banco = bancos,
                empresa = empresas
            };
            //datainit.banco = bancos;
            //datainit.empresa = empresas;
            return PartialView(datainit);
        }

        //devuelve el listado de empleados con informacion para nomina o finiquito segun sea el caso
        public ActionResult tablaLayout(int idEmpresa, int idBanco)
        {
            LayoutBancario layoutBancario = new LayoutBancario();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var empresa = layoutBancario.ListaEmpresas(periodo.IdSucursal);
            var datos = layoutBancario.ListaEmpleados(periodo.IdTipoNomina, periodo.IdPeriodoPago, idEmpresa, idBanco);
            ViewBag.empresa = empresa;
            return PartialView(datos);
        }

        public JsonResult crearLayout(encabezado Encabezado, List<int> Detalle, int idEmpresa)
        {
            var idU = SessionHelpers.GetIdUsuario();
            LayoutBancario layoutBancario = new LayoutBancario();
            var periodo = Session["periodo"] as NOM_PeriodosPago;
            var ruta = Server.MapPath("~//Files/Layout/");
            var listaEmpleadosSeleccionados = layoutBancario.ListaEmpleadosSeleccionados(periodo.IdTipoNomina, periodo.IdPeriodoPago, idEmpresa, Detalle);
            string[] archivoGen = layoutBancario.GenerarLayout(ruta, Encabezado, idU, listaEmpleadosSeleccionados);
            return Json(archivoGen, JsonRequestBehavior.AllowGet); 
        }

        public FileResult descargarLayout(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }


    }
}
