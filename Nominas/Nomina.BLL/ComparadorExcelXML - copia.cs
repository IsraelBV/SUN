using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Common.Utils;
using System.IO;
using RH.Entidades;

namespace Nomina.BLL
{
    public class ComparadorExcelXML
    {
        private DataTable myDataTableExcel;


        //--------------Constructor
        public ComparadorExcelXML(HttpPostedFileBase file)
        {
            
            this.MyDataTableExcel = Utils.ExcelToDataTable(file);
        }



        //------------------Methods
        public List<ItemComparacion> ComparaContraBD()
        {
            int numeroFilas = MyDataTableExcel.Rows.Count; //son en numero de filas que en realidad tiene el archivo de excel

            List<FilaExcel> listaFilasExcel = 
                (from DataRow fila in myDataTableExcel.Rows
                 let uuid = (string) fila[12]
                 let totalNeto = Decimal.Parse(fila[61].ToString())
                 let rfcReceptor = (string) fila[4]
                 let fecha = (string) fila[0]
                 let versionCFDI = (string) fila[17]
                 select new FilaExcel(uuid, totalNeto, rfcReceptor, fecha, versionCFDI)).ToList();

            //Recupero de la BD  y hago una lista

            #region ALEJANDRO
            //List<ResumenXML> listaResumenXML = new List<ResumenXML>();
            //RHEntities dContext = new RHEntities();
            //foreach (var item in listaFilasExcel)
            //{
            //    string uuidBuscado = item.Uuid;
            //    Func<NOM_CFDI_Timbrado, bool> apuBusqueda = itemComparacion =>
            //    {
            //        if (itemComparacion.FolioFiscalUUID == uuidBuscado)
            //        { return true; }
            //        else { return false; }
            //    };
            //    //Busca directo en  tabla de BD
            //    var registroBuscado = dContext.NOM_CFDI_Timbrado.Single(apuBusqueda);

            //    //trabajando con datetime
            //    ResumenXML resumenXML = new ResumenXML(registroBuscado.FechaCertificacion.Value, registroBuscado.FolioFiscalUUID, registroBuscado.TotalRecibo, registroBuscado.RFCReceptor);
            //    listaResumenXML.Add(resumenXML);
            //}
            #endregion


            #region EDWIN
            //1 hacer un array de uuid a buscar
            var arrayUuiDs = listaFilasExcel.Select(x => x.Uuid).ToArray();

            //2 hacer la busqueda del array usando using
            List<NOM_CFDI_Timbrado> listaTimbrados = new List<NOM_CFDI_Timbrado>();
            using (var context = new RHEntities())
            {
                listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                  where arrayUuiDs.Contains(t.FolioFiscalUUID)
                                  select t).ToList();
            }

            //3 armar la lista de resumen
            var listaResumenXML = (from r in listaTimbrados
                                select new ResumenXML()
                                {
                                    FechaTimbrado = r.FechaCertificacion.Value,
                                    RfcReceptor = r.RFCReceptor,
                                    Total = r.TotalRecibo,
                                    Uuid = r.FolioFiscalUUID
                                }).ToList();

            #endregion


            //Recorro la lista recuperada de excel ó la recuperada de los XML
            // fisicos, preparo para comparar
            List < ItemComparacion > listaComparacion = new List<ItemComparacion>();
            for (int i = 0; i < listaFilasExcel.Count; i++)
            {
                FilaExcel filaExcel = listaFilasExcel.ElementAt(i);
                ResumenXML resumenXML = listaResumenXML.ElementAt(i);
                ItemComparacion item = new ItemComparacion(filaExcel, resumenXML);
                item.Comparar();  //aqui comparo

                listaComparacion.Add(item);
            }

            //return Json(listaComparacion, JsonRequestBehavior.AllowGet);
            return (listaComparacion);
        }




        //---------------------properties
        public DataTable MyDataTableExcel
        {
            set { myDataTableExcel = value; }
            get { return myDataTableExcel; }
        }
    }


    public class FilaExcel
    {
        private string uuid;
        private decimal totalNeto;
        private string rfcReceptor;
        private string fecha;
        private string versionCFDI;


        //-----------------------constructor
        public FilaExcel(string uuid, decimal totalNeto, string rfcReceptor, string fecha, string versionCFDI)
        {
            this.Uuid = uuid;
            this.TotalNeto = totalNeto;
            this.RfcReceptor = rfcReceptor;
            this.Fecha = fecha;
            this.VersionCFDI = versionCFDI;
        }



        //-------------------properties
        public string Uuid
        {
            set { uuid = value; }
            get { return uuid; }
        }

        public decimal TotalNeto
        {
            set { totalNeto = value; }
            get { return totalNeto; }
        }

        public string RfcReceptor
        {
            set { rfcReceptor = value; }
            get { return rfcReceptor; }
        }

        public string Fecha
        {
            set { fecha = value; }
            get { return fecha; }
        }

        public string VersionCFDI
        {
            set { versionCFDI = value; }
            get { return versionCFDI; }
        }
    }

    public class ResumenXML
    {
        private DateTime fechaTimbrado;
        private string uuid;
        private decimal total;
        private string rfcReceptor;

        //---------constructors
        public ResumenXML(DateTime fechaTimbrado, string uuid, decimal total, string rfcReceptor)
        {
            this.FechaTimbrado = fechaTimbrado;
            this.Uuid = uuid;
            this.Total = total;
            this.RfcReceptor = rfcReceptor;
        }

        public ResumenXML() : this(new DateTime(), "", 0.0M, "")
        {
        }//constructor sin parametros, por si se llegara  a necesitar


        //---------------properties
        public DateTime FechaTimbrado
        {
            set { fechaTimbrado = value; }
            get { return fechaTimbrado; }
        }

        public string Uuid
        {
            set { uuid = value; }
            get { return uuid; }
        }

        public decimal Total
        {
            set { total = value; }
            get { return total; }
        }

        public string RfcReceptor
        {
            set { rfcReceptor = value; }
            get { return rfcReceptor; }
        }
    }

    public class ItemComparacion
    {
        private FilaExcel filaExcel;
        private ResumenXML resumenXML;

        private bool esIgualFechaTimbrado;
        private bool esIgualUuid;
        private bool esIgualTotal;
        private bool esIgualRfcReceptor;



        //------------------constructor
        public ItemComparacion(FilaExcel filaExcel, ResumenXML resumenXML)
        {
            this.FilaExcel = filaExcel;
            this.ResumenXML = resumenXML;

            this.EsIgualFechaTimbrado = false;
            this.EsIgualUuid = false;
            this.EsIgualTotal = false;
            this.EsIgualRfcReceptor = false;
        }

        public ItemComparacion() : this(null, null)
        {

        }//constructor sin parametros

        public void Comparar()
        {
            if (this.FilaExcel.Uuid == this.ResumenXML.Uuid)
            { this.EsIgualUuid = true; }

            if (this.FilaExcel.RfcReceptor == this.ResumenXML.RfcReceptor)
            { this.EsIgualRfcReceptor = true; }

            if (this.FilaExcel.TotalNeto == this.ResumenXML.Total)
            { this.EsIgualTotal = true; }


            //Divido la fecha para comparar solo la fecha, no la hora. Despues comparo las fechas
            //Ejemplo de fecha en archivo de  Excel: 2017-11-13T19:00:50
            string[] fechaDivividaDeFilaExcel = this.FilaExcel.Fecha.Split('T');
            string[] fechaDivididaOtraVez = fechaDivividaDeFilaExcel[0].Split('-');
            int anioDeExcel = Int32.Parse(fechaDivididaOtraVez.ElementAt(0));
            int mesDeExcel = Int32.Parse(fechaDivididaOtraVez.ElementAt(1));
            int diaExcel = Int32.Parse(fechaDivididaOtraVez.ElementAt(2));

            if ((anioDeExcel == resumenXML.FechaTimbrado.Year) && (mesDeExcel == resumenXML.FechaTimbrado.Month) && (diaExcel == resumenXML.FechaTimbrado.Day))
            {
                this.EsIgualFechaTimbrado = true;
            }

        }


        //---------------properties
        public FilaExcel FilaExcel
        {
            set { filaExcel = value; }
            get { return filaExcel; }
        }

        public ResumenXML ResumenXML
        {
            set { resumenXML = value; }
            get { return resumenXML; }
        }


        public bool EsIgualFechaTimbrado
        {
            set { esIgualFechaTimbrado = value; }
            get { return esIgualFechaTimbrado; }
        }

        public bool EsIgualUuid
        {
            set { esIgualUuid = value; }
            get { return esIgualUuid; }
        }

        public bool EsIgualTotal
        {
            set { esIgualTotal = value; }
            get { return esIgualTotal; }
        }

        public bool EsIgualRfcReceptor
        {
            set { esIgualRfcReceptor = value; }
            get { return esIgualRfcReceptor; }
        }
    }
}
