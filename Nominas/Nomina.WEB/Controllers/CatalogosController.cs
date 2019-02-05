using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using RH.BLL;
using Nomina.WEB.Filtros;
using RH.Entidades;
using FiltrosWeb;

namespace Nomina.WEB.Controllers
{
    [Autenticado]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class CatalogosController : Controller
    {
        private Catalogos catalogo = new Catalogos();
        // GET: Catalogos
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ConceptosIndex()
        {
            var listado = catalogo.ListadoConceptos();
            return PartialView(listado);
           
        }
        [HttpPost]
        public ActionResult EditarCuentaAcredora (int id, string ClaveContable)
        {
            var editar = catalogo.CuentaAcredora(id, ClaveContable);
            return null;
        }
        [HttpPost]
        public ActionResult EditarCuentaDeudora(int id, string ClaveContable)
        {
            var editar = catalogo.CuentaDeudora(id, ClaveContable);
            return null;
        }
     
        [SesionSucursal]
        public ActionResult Empleados()
        {            
            var sucursal = Session["sucursal"] as SucursalDatos;
            if (sucursal == null)
            {
                return null;
            }
            else
            {
                Empleados emp = new Empleados();
               var lista= emp.ObtenerEmpleadosPorSucursal(sucursal.IdSucursal);
                return PartialView(lista);
            }
            
        }
        [HttpPost]
        public JsonResult DatosPersonales(int id)
        {

            Empleados emp = new Empleados();

            //var empleadodato = emp.GetEmpleadoById(id);
            //var empleadoContrato = emp.GetUltimoContrato(id);
            //var empleadoEmpresa = emp.EmpleadoEmpresa(id);

            var empleadobanco = emp.GetDatosBancarios(id);
            var empleadoDatos = emp.DatosEmpleadoViewModel(id);


            EmpleadoNomina empnom = new EmpleadoNomina();


            empnom.empleadodatos = empleadoDatos;            
            empnom.datosbanco = empleadobanco;
            //empnom.empleado_contratro = empleadoContrato;
            //empnom.emp_empresa = empleadoEmpresa;
            //return PartialView(lista);
            return Json(empnom, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Bancos()
        {
            Bancos bancos = new Bancos();
            var listabancos = bancos.GetBancos();
            return PartialView(listabancos);
        }
        public ActionResult Incidencias()
        {
            IncidenciasClass incidencia = new IncidenciasClass();
            var listaInci = incidencia.GetIncidencias();
            return PartialView(listaInci);
        }
    }   
    public class EmpleadoNomina
    {
       public EmpleadoDatosNominaViewModel empleadodatos { get; set; }
        public List<DatosBancariosViewModel> datosbanco { get; set; }
        public List<DatosEmpleado> emp_empresa { get; set; }
    }
}