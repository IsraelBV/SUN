using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using FiltrosWeb;



namespace Nomina.WEB.Controllers
{
    [Autenticado]

    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ComparaExcelvsXMLController : Controller
    {
        // GET: ComparaExcelvsXML
        public PartialViewResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            List<ItemComparacion> resultadoComparacion; //usado para cachar la respuesta del modelo, si es exitosa
            resultadoComparacion = new List<ItemComparacion>();

            //Validar que el file no este vacio o null
            if (file != null)
            {
                try
                {
                  
                    ComparadorExcelXML comparadorExcelXML = new ComparadorExcelXML(file);
                    resultadoComparacion = comparadorExcelXML.ComparaContraBD();

                }
                catch (Exception ex)
                {
                    return Json(new { error = "ERROR: El archivo no contiene datos correctos o esta dañado." }, JsonRequestBehavior.AllowGet);
                }

            }
            //filebatchuploadsuccess
            return Json(new { fileuploaded = "agregado", filebatchuploadsuccess = "Batch success", respuesta = resultadoComparacion}, JsonRequestBehavior.AllowGet);
        }
       
    }
}