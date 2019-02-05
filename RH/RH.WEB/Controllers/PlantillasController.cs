using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;
using FiltrosWeb;
using Common.Enums;

namespace RH.WEB.Controllers
{
    
    public class PlantillasController : Controller
    {
        //variables
        int  _idSucursal = 0;
        readonly Plantillas _plantilla = new Plantillas();

        // GET: Plantillas
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpresas, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var modelo = _plantilla.GetPlantillas();
            var cl = new Clientes();
            ViewBag.Clientes = cl.GetClientesActivos();
            return View(modelo);
        }

        public PartialViewResult Details(int id)
        {
            var cl = new Clientes();
            ViewBag.Clientes = cl.GetClientesActivos();
            var model = _plantilla.GetPlantillaById(id);
            return PartialView(model);
        }

        public JsonResult Edit(string[] Clientes, int NumClientes, int Id)
        {
            Plantilla model = new Plantilla();
            model.Id = Id;

            if (Clientes.Length == NumClientes)
                model.Clientes = "*";
            else
            {
                if(Clientes.Length > 0)
                {
                    model.Clientes = String.Join(",", Clientes);
                }
            }
                
            var status = _plantilla.EditarPlantilla(model);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubirArchivo(HttpPostedFileBase file, string[] Clientes, int NumClientes, int? tipo = 1)
        {
            try
            {

           // if (file == null) return null;

            //string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + file.FileName).ToLower();
            string nombreArchivo = file.FileName;

                string extension = Path.GetExtension(file.FileName);
         
                if (extension.ToUpper() == ".DOCX")
                {
                    //Validar si el archivo ya esta guardado
                    if (!_plantilla.ArchivoRegistrado(file.FileName))
                    {
                        file.SaveAs(Server.MapPath("~/Files/Plantillas/" + nombreArchivo));

                        var client = "";
                        if (Clientes.Length == NumClientes)
                            client = "*";
                        else
                        {
                            if(Clientes.Length > 0)
                            {
                                client = String.Join(",", Clientes);
                            }
                        }

                        var item = new Plantilla
                        {
                            NombreArchivo = nombreArchivo,
                            Tipo = tipo,
                            Status = true,
                            Clientes = client
                        };
                        _plantilla.AgregarPlantilla(item);


                        TempData["accionPlantilla"] = 1;
                        TempData["accionMensaje"] = "¡Se guardo el archivo correctamente!";
                    }
                    else
                    {
                        TempData["accionPlantilla"] = 2;
                        TempData["accionMensaje"] = "¡Este archivo ya esta registrado!";
                    }
                }
                else
                {
                    TempData["accionPlantilla"] = 3;
                    TempData["accionMensaje"] = "¡No se guardó el archivo! No tiene la extension .DOCX!";
                }

                
                return RedirectToAction("Index", "Plantillas");

                //return PartialView("ListaPlantilla",modelo);

            }
            catch (Exception )
            {

                return RedirectToAction("Index", "Plantillas");
            }

        }

   
        public ActionResult Descarga(string nombreArchivo)
        {

            byte[] archiBytes = null;
            try
            {
                var path = Server.MapPath("~/Files/Plantillas/" + nombreArchivo);

                archiBytes = System.IO.File.ReadAllBytes(path);

                TempData["accionPlantilla"] = 2;
                TempData["accionMensaje"] = "Descargando archivo ...";

                return File(archiBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);

            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Plantillas");
            }



        }

        public ActionResult Eliminar(int id)
        {
            _plantilla.EliminarPlantilla(id);

            TempData["accionPlantilla"] = 3;
            TempData["accionMensaje"] = "¡Se eliminó el archivo correctamente!";

            return RedirectToAction("Index", "Plantillas");
        }
       
    }
}