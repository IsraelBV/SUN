using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using Common.Helpers;
using Nomina.Reportes;
using FiltrosWeb;

namespace Nomina.WEB.Controllers
{
    public class CambiosDatosBancariosController : Controller
    {
        // GET: CambiosDatosBancarios
        public ActionResult Index()
        {
            _DatosBancarios datosB = new _DatosBancarios();
            var datos = datosB.Clientes();
            return PartialView(datos);
        }


        public ActionResult Empleados(int idSucursal)
        {
            _DatosBancarios datosB = new _DatosBancarios();
            var empleados = datosB.empleados(idSucursal);
            return PartialView(empleados);
        }


        public ActionResult datosBancarios (int IdEmpleado)
        {
            _DatosBancarios datosB = new _DatosBancarios();
            var datos = datosB.datosBank(IdEmpleado);
            var bancos = datosB.listBancos();
            var metodos = datosB.metodos();

            ViewBag.bancos = bancos;
            ViewBag.metodos = metodos;
            return PartialView(datos);
        }

        public JsonResult guardarDatos(EmpleadoBank item)
        {
            _DatosBancarios datosB = new _DatosBancarios();

            var resultado = datosB.guardarDatosBancarios(item);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        public JsonResult crearLayout(int idSucursal)
        {
            ImportacionDatosBancarios datos = new ImportacionDatosBancarios();
            var idusuario = SessionHelpers.GetIdUsuario();
            var ruta = Server.MapPath("~/Files/PlantillaDatosBancarios");
            var resultado = datos.crearLayoutBancario(idusuario,ruta, idSucursal);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public FileResult descargarPlantilla(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }

        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            
            var data = new System.Data.DataTable();
            ImportacionDatosBancarios datos = new ImportacionDatosBancarios();

            data = datos.ExcelToDataTable(file);
            var dat = datos.guardarRegistro(data);
            return Json("succes", JsonRequestBehavior.AllowGet); ;
        }
    }
}