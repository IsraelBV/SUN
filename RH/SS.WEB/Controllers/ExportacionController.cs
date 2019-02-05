using SS.WEB.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using FiltrosWeb;
using Common.Enums;

namespace SS.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.SeguroSocial)]
    public class ExportacionController : Controller
    {
        //[AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            Empresas _empr = new Empresas();
            ViewBag.Empresas = _empr.GetEmpresasByGuia();
            return View();
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult Search(SearchFileViewModel busqueda)
        {
            List<DatosEmpleado> model = new List<DatosEmpleado>();
            if(busqueda.TipoMovimiento != 4)
            {
                KardexEmpleado krd = new KardexEmpleado();
                model = krd.BuscarMovimientos(busqueda.FechaInicio, busqueda.FechaFin, busqueda.TipoMovimiento, busqueda.IdEmpresa);
            }
            else
            {
                Empleados emp = new Empleados();
                model = emp.GetCumpleIMSS(busqueda.FechaInicio, busqueda.FechaFin, busqueda.IdEmpresa);
            }
            
            return PartialView(model);
        }

        [AccesoModuloAttribute(Modulo = Modulos.SSEmpleados, Accion = AccionCrud.Consultar)]
        [HttpPost]
        public JsonResult CreateFile(int[] empleados, int IdEmpresa, int tipoArchivo, int tipoMovimiento, string nombreArchivo)
        {
            ExportacionIMSS ex = new ExportacionIMSS();
            
            int registros = 0;
            if(tipoArchivo == 2)
            {
                if (tipoMovimiento == 1)
                {
                    registros = ex.IDSEReingreso(empleados, IdEmpresa, nombreArchivo);
                }
                else if (tipoMovimiento == 12)
                    registros = ex.IDSEModificacionSalario(empleados, IdEmpresa, nombreArchivo);
            }

            return Json(registros, JsonRequestBehavior.AllowGet);
        }

        public FileResult Download(string nombreArchivo)
        {
            string file = "C:\\sites/RH/trunk/AlianzaCorp/SS.WEB/Files/" + nombreArchivo;

            byte[] fyleBytes = System.IO.File.ReadAllBytes(file);
            
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);
        }
    }
}