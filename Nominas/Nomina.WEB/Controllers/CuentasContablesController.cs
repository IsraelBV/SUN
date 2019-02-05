using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
namespace Nomina.WEB.Controllers
{
    public class CuentasContablesController : Controller
    {
        // GET: CuentasContables
        public ActionResult Index()
        {
            _CuentasContables cuentas = new _CuentasContables();

            var listaEmpresas = cuentas.ListEmpresaFiscales();
            
        
            return PartialView(listaEmpresas);
        }

        public ActionResult tablasCuentas(int idEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();
            var listaconceptos = cuentas.empresasCuentas(idEmpresa);
            return PartialView(listaconceptos);
        }

        public JsonResult EditarCuentaDeudora(int Id, string ClaveContable, int IdEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();

            var resultado = cuentas.GuardarDeudora(Id, IdEmpresa, ClaveContable);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditarCuentaAcredora(int Id, string ClaveContable, int IdEmpresa)
        {
            _CuentasContables cuentas = new _CuentasContables();

            var resultado = cuentas.GuardarAcredora(Id, IdEmpresa, ClaveContable);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}