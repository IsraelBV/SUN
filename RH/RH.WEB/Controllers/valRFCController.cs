using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using Common.Helpers;
using Common.Enums;
using RH.Entidades;
using FiltrosWeb;

namespace RH.WEB.Controllers
{
    public class valRFCController : Controller
    {
        // GET: valRFC
        [AccesoModuloAttribute(Modulo = Modulos.RHValidadorRFC, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            

            return View();
        }
        //Descarga listado RFC
        public FileResult DescargaListRFC()
            {
                //se obtiene id usuario
                var idU = SessionHelpers.GetIdUsuario();
                //se obtiene los datos de la sucursal
                var sucursal = Session["sucursal"] as SucursalDatos;
                //establecemos la ruta donde se generara el archivo
                var ruta = Server.MapPath("~//Files/valRFC/");
                //se obtiene los datos del empleado
                Empleados emp = new Empleados();
            bool estaVacio = false;
                string archivoGen = emp.GenerarTxtRfcForSat(ruta, sucursal.IdSucursal, idU, ref estaVacio);
            
                //Se lee el archivo
                byte[] fyleBytes = System.IO.File.ReadAllBytes(archivoGen);
            //Regresa el archivo
            string nombreArchivo = "Lista RFC " + sucursal.Nombre.Replace('.', '_') + "_" + sucursal.Ciudad.Replace('.', '_') + ".txt";
            if (estaVacio)
            {
               nombreArchivo = "Sin RFC para validar.txt";
            }
                
                return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);
            
            }

        [HttpPost]
        public ActionResult UploadRFC(HttpPostedFileBase fileInput)
        {
            if (fileInput != null)
            {
                //se obtiene los datos de la sucursal
                var sucursal = Session["sucursal"] as SucursalDatos;
                Empleados emp = new Empleados();
                System.IO.StreamReader Reader = new System.IO.StreamReader(fileInput.InputStream);
                var idemp=emp.ActualizarRfcValidadosFromSat(Reader, sucursal.IdSucursal);
                TempData["idemp"] = idemp;
                return RedirectToAction("RFCNoValidos", "valRFC");
            }
            else
            {
                return Index();
            }
        }

        public ActionResult RFCNoValidos()
        {
            var sucursal = Session["sucursal"] as SucursalDatos;
            Empleados emp = new Empleados();
            var modelid = TempData["idemp"] as List<int>;
            var modelo = emp.RFCnoValidos(sucursal.IdSucursal, modelid);
            ViewBag.sucursal = sucursal.Nombre;
            ViewBag.ciudad = sucursal.Ciudad;
            return View(modelo);
        }
    }
}