using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using Common.Enums;
using Common.Helpers;
using FiltrosWeb;
using System.Data;
using Common.Utils;
using RH.Entidades;
using System.Threading.Tasks;

using Nomina.WEB.Filtros;

namespace Nomina.WEB.Controllers
{
    [Autenticado]

    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [SesionPeriodo]
    [SesionSucursal]
    public class ComplementoController : Controller
    {
        // GET: Complemento
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
            DatoComplemento dc = new DatoComplemento();
            var modelo = dc.GetDatosComplementoByIdPeriodo(idPeriodoPago);

            return PartialView(modelo);
        }

        public FileResult DownLayoutComplemento(int id)
        {
            DatoComplemento dc = new DatoComplemento();
            var archivoBytes = dc.CrearLayoutComplemento(id);

            var periodoPago = Session["periodo"] as NOM_PeriodosPago;

            //Se crea un nuevo nombre de archivo para el usuario
            string newFileName = "LC_" + periodoPago.Descripcion + "_.xlsx";
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
                    //idPeriodo = periodoPago.IdPeriodoPago;

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

                    DatoComplemento dc = new DatoComplemento();
                    dc.ImportarDatosComplemento(dt, periodoPago.IdPeriodoPago);
                }
                catch (Exception)
                {
                    return Json(new { error = "ERROR: El archivo no contiene datos correctos o esta dañado." }, JsonRequestBehavior.AllowGet);
                }

            }
            //filebatchuploadsuccess
            return Json(new { fileuploaded = "agregado", filebatchuploadsuccess = "Batch success" }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownLayoutDetalle(int id)
        {
            DatoComplemento dc = new DatoComplemento();
            var archivoBytes = dc.LayoutDetalleComplemento(id);

            var periodoPago = Session["periodo"] as NOM_PeriodosPago;

            //Se crea un nuevo nombre de archivo para el usuario
            string newFileName = "Plantilla" + periodoPago.Descripcion + "_.xlsx";
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
        public JsonResult UploadFileDetalle(HttpPostedFileBase file)
        {
            int idPeriodo = 0;
            //Validar que el file no este vacio o null
            if (file != null)
            {
                try
                {
                    var periodoPago = Session["periodo"] as NOM_PeriodosPago;
                    // idPeriodo = periodoPago.IdPeriodoPago;

                    // PeriodosPago ctx = new PeriodosPago();
                    //  var periodoActualizado = ctx.GetPeriodoPagoById(periodoPago.IdPeriodoPago);

                    if (!file.FileName.Contains("Plantilla"))
                    {
                        return Json(new { error = "ERROR: Debe subir el archivo con el nombre que fue generado." }, JsonRequestBehavior.AllowGet);
                    }


                    //if (periodoActualizado.Autorizado) -- 
                    //{
                    //    return Json(new { error = "ERROR: No se puede subir datos a un periodo Autorizado" }, JsonRequestBehavior.AllowGet);
                    //}

                    DataTable dt = new DataTable();
                    dt = Utils.ExcelToDataTable(file, false);


                    if (dt == null)
                    {
                        return Json(new { error = "ERROR: El archivo no tiene el formato correcto" }, JsonRequestBehavior.AllowGet);
                    }

                    DatoComplemento dc = new DatoComplemento();
                    dc.ImportarDetalleComplemento(dt, periodoPago.IdPeriodoPago);
                }
                catch (Exception)
                {
                    return Json(new { error = "ERROR: El archivo no contiene datos correctos o esta dañado." }, JsonRequestBehavior.AllowGet);
                }

            }
            //filebatchuploadsuccess
            return Json(new { fileuploaded = "agregado", filebatchuploadsuccess = "Batch success" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<FileResult> GetRecibosComplemento(int[] idEmpleados = null)
        {
            try
            {
                if (idEmpleados == null) return null;

                // int[] nominas = new[] {1,2,3,4,5,6};
                Random random = new Random();
                // int randomNumber = random.Next(0, 1000);//A

                var ruta = Server.MapPath("~/Files/DownloadRecibos");
                var idusuario = SessionHelpers.GetIdUsuario();
                var periodoPago = Session["periodo"] as NOM_PeriodosPago;
                if (periodoPago != null)
                {

                    //Crear el pdf con el xml generado
                    var recibo = await ProcesoNomina.GetRecibosComplementoDetalle(idEmpleados, periodoPago, idusuario, ruta);

                    var file = System.IO.File.ReadAllBytes(recibo);

                    var nombreArchivo = "COMP_" + periodoPago.Descripcion + ".pdf";

                    return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {

            }
        }
    }
}