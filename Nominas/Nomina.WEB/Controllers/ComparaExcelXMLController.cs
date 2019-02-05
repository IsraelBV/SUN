using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Utils;
using RH.Entidades;
using System.IO;    // System.IO - OfficeOpenXML -  lo requiere EPPlus 
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Data;


namespace Nomina.WEB.Controllers
{
    public class ComparaExcelXMLController : Controller
    {
        // GET: ComparaExcelXML
        public ActionResult Index()
        {
            return PartialView();
        }


        [HttpPost]
        public JsonResult EnviarAComparar2(HttpPostedFileBase file3)
        {

            HttpPostedFileBase file = Request.Files[0]; //Uploaded file
                                                            
            string cadenaAleatoria = NumerosAleatoriosFactory.GetInstance().GetNumeroEntero().ToString();

            DataTable dt = new DataTable();
            dt = Utils.ExcelToDataTable(file); 

            //Se sube el archivo
            var fileName = Path.GetFileName(file.FileName);
            fileName = cadenaAleatoria + fileName;   //asi se llamara el archivo en el servidor
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            file.SaveAs(path);//supuestamente es la ruta del archivo + nom completo de archivo en el server

            //Se comienza a usar Epplus           
            var package = new ExcelPackage(new FileInfo(@path));
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            int totalFilas;
            int totalColumnas;

            totalFilas = sheet.Dimension.End.Row;
            totalColumnas = sheet.Dimension.End.Column;

            //Se comienza a recorrer el archivo xlsx desde la fila 2, tiene 62 columnas
            //y se guardan lo valores de las celdas que me interesan
            List<FilaExcel> listaFilasExcel = new List<FilaExcel>();
            for (int i = 2; i <= totalFilas; i++)
            {
                //columna 13 tiene uuid
                //columna 62 tiene total neto
                //columna 5 tiene el rfc receptor
                //columna 1 tiene la fecha
                //columna 18 tiene version de CFDI
                string uuid = sheet.Cells[i, 13].Value.ToString();
                decimal totalNeto = Decimal.Parse(sheet.Cells[i, 62].Value.ToString());
                string rfcReceptor = sheet.Cells[i, 5].Value.ToString();
                string fecha = sheet.Cells[i, 1].Value.ToString();
                string versionCFDI = sheet.Cells[i, 18].Value.ToString();

                FilaExcel filaExcel = new FilaExcel(uuid, totalNeto, rfcReceptor, fecha, versionCFDI);
                listaFilasExcel.Add(filaExcel);
            }


            //Recupero de la BD  y hago una lista
            List<ResumenXML> listaResumenXML = new List<ResumenXML>();
            RHEntities dContext = new RHEntities();
            foreach (var item in listaFilasExcel)
            {
                string uuidBuscado = item.Uuid;
                Func<NOM_CFDI_Timbrado, bool> apuBusqueda = itemComparacion =>
                {
                    if (itemComparacion.FolioFiscalUUID == uuidBuscado)
                    { return true; }
                    else { return false; }
                };
                //Busca directo en  tabla de BD
                var registroBuscado = dContext.NOM_CFDI_Timbrado.Single(apuBusqueda);//BUSCA UNO POR UNO? CUANTAS VECES IRA ALA DB?


                //trabajando con datetime
                ResumenXML resumenXML = new ResumenXML(registroBuscado.FechaCertificacion.Value, registroBuscado.FolioFiscalUUID, registroBuscado.TotalRecibo, registroBuscado.RFCReceptor);

                listaResumenXML.Add(resumenXML);
            }


            //Recorro la lista recuperada de excel ó la recuperada de los XML
            // fisicos, preparo para comparar
            List<ItemComparacion> listaComparacion = new List<ItemComparacion>();
            for (int i = 0; i < listaFilasExcel.Count; i++)
            {
                FilaExcel filaExcel = listaFilasExcel.ElementAt(i);
                ResumenXML resumenXML = listaResumenXML.ElementAt(i);
                ItemComparacion item = new ItemComparacion(filaExcel, resumenXML);
                item.Comparar();  //aqui comparo

                listaComparacion.Add(item);
            }


            //Borro archivo excel 
            BorraArchivo borrador = new BorraArchivo();
            borrador.SuprimirArchivo(path);


            return Json(listaComparacion,JsonRequestBehavior.AllowGet);
        }
    }
}