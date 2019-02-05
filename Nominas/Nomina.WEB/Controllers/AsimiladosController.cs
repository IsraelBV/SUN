using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using RH.Entidades;
using FiltrosWeb;
using System.Data;
using Common.Utils;



namespace Nomina.WEB.Controllers
{
    public class AsimiladosController : Controller
    {
        // GET: Asimilados
        public PartialViewResult Index()
        {
            int idPeriodoPago = 0;
            if (Session["periodo"] != null)
            {
                var periodoPago = Session["periodo"] as NOM_PeriodosPago;
                idPeriodoPago = periodoPago.IdPeriodoPago;
                ViewBag.IdPeriodo = periodoPago.IdPeriodoPago;
            }


            ViewBag.IdPeriodo = idPeriodoPago;

            DatoAsimilado da = new DatoAsimilado();
            var modelo = da.GetDatosAsimilados(idPeriodoPago);

            return PartialView(modelo);
            
        }
        
        public FileResult DownLayoutAsimilado(int id)
        {
            DatoAsimilado da = new DatoAsimilado();
            var archivoBytes = da.CrearLayoutAsimilado(id);

            var periodoPago = Session["periodo"] as NOM_PeriodosPago;

            //Se crea un nuevo nombre de archivo para el usuario
            string newFileName = "LA_" + periodoPago.Descripcion + "_.xlsx";
            //application/vnd.ms-excel
            //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet

            //application / vnd.ms - excel(official)
            //application / msexcel
            //application / x - msexcel
            //application / x - ms - excel
            //application / x - excel
            //application / x - dos_ms_excel
            //application / xls
            //application / x - xls
            //application / vnd.openxmlformats - officedocument.spreadsheetml.sheet(xlsx)

            //header('Content-Disposition: attachment; filename="name_of_excel_file.xls"');
            //Regresa el archivo
            // System.Net.Mime.MediaTypeNames.Application.
            return File(archivoBytes, "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", newFileName);
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            int idPeriodo = 0;
            //Validar que el file no este vacio o null
            if (file != null)
            {
                try
                {
                    var periodoPago = Session["periodo"] as NOM_PeriodosPago;
                    idPeriodo = periodoPago.IdPeriodoPago;

                    PeriodosPago ctx = new PeriodosPago();
                    var periodoActualizado = ctx.GetPeriodoPagoById(periodoPago.IdPeriodoPago);


                    if (periodoActualizado.Autorizado)
                    {
                        return Json(new { error = "ERROR: No se puede subir datos a un periodo Autorizado" }, JsonRequestBehavior.AllowGet);
                    }

                    DataTable dt = new DataTable();
                    dt = Utils.ExcelToDataTable(file);
                    //Validar que sea el formato correcto

                    if (dt == null)
                    {
                        return Json(new { error = "ERROR: El archivo no tiene el formato correcto" }, JsonRequestBehavior.AllowGet);
                    }

                    DatoAsimilado da = new DatoAsimilado();
                    da.ImportarDatosAsimilado(dt, periodoPago.IdPeriodoPago);
                }
                catch (Exception)
                {
                    return Json(new { error = "ERROR: El archivo no contiene datos correctos o esta dañado." }, JsonRequestBehavior.AllowGet);
                }

            }
            //filebatchuploadsuccess
            return Json(new { fileuploaded = "agregado", filebatchuploadsuccess = "Batch success" }, JsonRequestBehavior.AllowGet);
        }

    }
}