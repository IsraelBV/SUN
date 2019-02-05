using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;
using Common.Enums;
using FiltrosWeb;

namespace RH.WEB.Controllers
{
    public class ClientesController : Controller
    {
        readonly Clientes _emp = new Clientes();
        readonly Empresas _rp = new Empresas();
        readonly Departamentos _depto = new Departamentos();
        readonly Estados _edo = new Estados();

        #region "CLIENTES CREAR - EDITAR - CONSULTAR"

        // GET: Empresas
        [AccesoModuloAttribute(Modulo = Modulos.RHClientes, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var modelo = _emp.GetClientes();
            return View(modelo);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHClientes, Accion = AccionCrud.Agregar)]
        public ActionResult Create()
        {
            var registros = _rp.GetRegistrosPatronales();
            ViewBag.SelectListRP = registros;

            var departamentos = _depto.GetDepartamentos();
            ViewBag.SelectListDepto = departamentos;

            return View();
        }

        //[HttpPost]
        //[AccesoModuloAttribute(Modulo = Modulos.Clientes, Accion = AccionCrud.Agregar)]
        //public ActionResult Create(Cliente regsCliente, List<int> itemsRP, List<int> itemsDP)
        //{
        //    try
        //    {
        //       _emp.GuardarCliente(regsCliente, itemsRP, itemsDP);

        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception e)
        //    {

        //        return View();
        //    }
        //}


        [AccesoModuloAttribute(Modulo = Modulos.RHClientes, Accion = AccionCrud.Agregar)]
        public ActionResult CrearVP()
        {
            var registros = _rp.GetRegistrosPatronales();
            var Fis_Asi = registros.Where(x => x.RegistroPatronal != null).ToList();
            var Com_Asim = registros.Where(x => x.RegistroPatronal == null).ToList();

            ViewBag.SelectListFis_Asim = Fis_Asi;
            ViewBag.SelectListComp_Sind = Com_Asim;
            var departamentos = _depto.GetDepartamentos();
            ViewBag.SelectListDepto = departamentos;

            return PartialView();
        }

        [HttpPost]
        [AccesoModuloAttribute(Modulo = Modulos.RHClientes, Accion = AccionCrud.Agregar)]
        public ActionResult CrearVP(Cliente regsCliente, List<int> itemsRP, List<int> itemsCom,List<int> itemsAsim , List<int> itemsSin,  List<int> itemsDP)
        {
            try
            {
                _emp.GuardarCliente(regsCliente, itemsDP);

                return RedirectToAction("Index");
            }
            catch (Exception )
            {

                return View();
            }
        }


        [AccesoModuloAttribute(Modulo = Modulos.RHClientes, Accion = AccionCrud.Editar)]
        public ActionResult EditarVP(int idCliente)
        {

            var modelo = _emp.GetClienteById(idCliente);

       
            var slEmpDep = _emp.GetClienteDepartamentos(idCliente);

   
            ViewBag.SelectListDepto = slEmpDep;     

            return PartialView(modelo);
        }


        [AccesoModuloAttribute(Modulo = Modulos.RHClientes, Accion = AccionCrud.Editar)]
        [HttpPost]
        public ActionResult EditarVP(Cliente regsEmpresa, List<int> itemsRP, List<int> itemsDP,List<int> itemsAsim,List<int> itemsSin,List<int> itemsCom, int idcliente)
        {
            try
            {
                _emp.ActualizarCliente(regsEmpresa,itemsDP,idcliente);

                return RedirectToAction("Index");
            }
            catch (Exception )
            {

                return View();
            }
        }

        public ActionResult DetalleVP(int idCliente)
        {

            var departamentos = _emp.DetallesDepartamentos(idCliente);
            var cliente = _emp.GetClienteById(idCliente);

            ViewBag.SelectListDepto = departamentos;
            ViewBag.Clientes = cliente.Nombre;
            return PartialView();
        }

        #endregion

        #region "SUCURSALES CREAR - EDITAR - CONSULTAR"

        [AccesoModulo(Modulo = Modulos.RHClientes, Accion = AccionCrud.Consultar)]
        public ActionResult IndexSucursales(int idCliente)
        {
            var empresa = _emp.GetClienteById(idCliente);
            var sucursales = _emp.GetSucursalesByIdCliente(idCliente);

            ViewBag.Empresa = empresa;
            return View(sucursales);
        }

        [AccesoModulo(Modulo = Modulos.RHClientes, Accion = AccionCrud.Agregar)]
        public ActionResult CrearSucursalVP(int idCliente)
        {
            

             var listaEstados = _edo.GetEstados();
            var listaZonas = _emp.GetZonaSalario();

            var selectListEstados = listaEstados.Select(x => new SelectListItem()
            {
                Value = x.IdEstado.ToString(),
                Text = x.Descripcion,
                Selected = (x.IdEstado == 1)
            }).ToList();


            var selectListZonas = listaZonas.Select(x => new SelectListItem()
            {
                Value = x.IdZonaSalario.ToString(),
                Text = x.Zona + " - " + x.SMG,
                Selected = (x.IdZonaSalario == 1)
            }).ToList();


            
            ViewBag.SelectListZonas = selectListZonas;
            ViewBag.SelectListEstados = selectListEstados;
            
            ViewBag.EmpresaId = idCliente;

            //Listado de Empresas
            var registros = _rp.GetRegistrosPatronales();
            var Fis_Asi = registros.Where(x => x.RegistroPatronal != null).ToList();
            var Com_Asim = registros.Where(x => x.RegistroPatronal == null).ToList();

            ViewBag.SelectListFis_Asim = Fis_Asi;
            ViewBag.SelectListComp_Sind = Com_Asim;


            return PartialView();
        }


        [AccesoModulo(Modulo = Modulos.RHClientes, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult CrearSucursalVP(Sucursal sucursal, List<int> itemsRP, List<int> itemsCom, List<int> itemsAsim, List<int> itemsSin)
        {
            try
            {
                _emp.GuardarSucursalCliente(sucursal, itemsRP,itemsCom,itemsAsim,itemsSin);

                return RedirectToAction("IndexSucursales", new { idCliente = sucursal.IdCliente});
            }
            catch (Exception )
            {

                return View();
            }
        }

        [AccesoModulo(Modulo = Modulos.RHClientes, Accion = AccionCrud.Editar)]
        public ActionResult EditarSucursalVP(int idSucursal)
        {
            var sucursal = _emp.GetSucursalClienteById(idSucursal);

            var listaEstados = _edo.GetEstados();
            var listaZonas = _emp.GetZonaSalario();
            
            var selectListEstados = listaEstados.Select(e => new SelectListItem()
            {
                Value = e.IdEstado.ToString(), Text = e.Descripcion, Selected = (e.IdEstado == sucursal.IdEstado)
            }).ToList();


            var selectListZonas = listaZonas.Select(z => new SelectListItem()
            {
                Value = z.IdZonaSalario.ToString(),
                Text = z.Zona +" - "+ z.SMG,
                Selected = (z.IdZonaSalario == sucursal.IdSucursal)
            }).ToList();

            var EmpFIs = _emp.GetSucursalEmpresaSeleccionada(idSucursal, 1, 1);
            var EmpAsim = _emp.GetSucursalEmpresaSeleccionada(idSucursal, 1, 4);
            var EmpCom = _emp.GetSucursalEmpresaSeleccionada(idSucursal, 2, 2);
            var EmpSin = _emp.GetSucursalEmpresaSeleccionada(idSucursal, 2, 3);
            

            ViewBag.SelectListFis = EmpFIs;
            ViewBag.SelectListAsim = EmpAsim;
            ViewBag.SelectListRPCom = EmpCom;
            ViewBag.SelectListRPSIn = EmpSin;



            ViewBag.SelectListZonas = selectListZonas;
            ViewBag.SelectListEstados = selectListEstados;

            

            return PartialView(sucursal);
        }

        [AccesoModulo(Modulo = Modulos.RHClientes, Accion = AccionCrud.Editar)]
        [HttpPost]
        public ActionResult EditarSucursalVP(Sucursal sucursal, List<int> itemsRP, List<int> itemsAsim, List<int> itemsSin, List<int> itemsCom)
        {
            _emp.ActualizarSucursal(sucursal, itemsRP,itemsAsim,itemsSin,itemsCom);

            return RedirectToAction("IndexSucursales", new { idCliente = sucursal.IdCliente });
        }



        [AccesoModulo(Modulo = Modulos.RHClientes, Accion = AccionCrud.Consultar)]
        public ActionResult DetalleSucursalVP(int idSucursal)
        {
            var sucursal = _emp.GetSucursalClienteById(idSucursal);
            var empresas = _emp.DetalleSucursal(idSucursal);




            //ViewBag.SelectListRP = listaRP.Where(x => x.Seleccionado == true).ToList();
            ViewBag.Empresas = empresas;

            return PartialView(sucursal);
        }


        #endregion

    }
}