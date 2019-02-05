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

            List<FilaExcel> listaFilasExcel = new List<FilaExcel>();
            foreach (DataRow fila in myDataTableExcel.Rows)
            {
                //columna 13 tiene uuid
                //columna 62 tiene total neto
                //columna 5 tiene el rfc receptor
                //columna 1 tiene la fecha
                //columna 18 tiene version de CFDI
                string uuid = (string)fila[12];
                decimal totalNeto = Decimal.Parse(fila[61].ToString());
                string rfcReceptor = (string)fila[4];
                string fecha = (string)fila[0];
                string versionCFDI = (string)fila[17];


                FilaExcel filaExcel = new FilaExcel(uuid, totalNeto, rfcReceptor, fecha, versionCFDI);
                listaFilasExcel.Add(filaExcel);
            }



            // hacer un array de UID que necesitamos buscar, 
            var arrarUids = listaFilasExcel.Select(x => x.Uuid).ToArray();

            // hacer la consulta usando el array y usando using . listaTimbrados puede ya no contener todos los UUIDs buscados desde Excel
            List<NOM_CFDI_Timbrado> listaDeTimbrados = new List<NOM_CFDI_Timbrado>();
            using (var context = new RHEntities())
            {
                listaDeTimbrados = (from t in context.NOM_CFDI_Timbrado where arrarUids.Contains(t.FolioFiscalUUID) select t).ToList();
            }

            // armar la lista resumenXML
            List<ResumenXML> lResumenXML2 = (from item in listaDeTimbrados
                                             select new ResumenXML() { FechaTimbrado = item.FechaCertificacion.Value, RfcReceptor = item.RFCReceptor, Total = item.TotalRecibo, Uuid = item.FolioFiscalUUID }).ToList();



            // No comparacion por indices, usa foreach, primero obtengo fila excel y luego busco si existe en la listaXML 
            // si existe lo comparo, si no existe sigo con el siguiente, que no se detenga
            List<ItemComparacion> listaComparacion = new List<ItemComparacion>();
            foreach (FilaExcel filaExcel in listaFilasExcel)
            {
                ResumenXML itemResumenXML=null;

                Func<ResumenXML, bool> apu = item =>
                {
                    if (item.Uuid == filaExcel.Uuid) { return true; }
                    else { return false; }
                };

                itemResumenXML = lResumenXML2.SingleOrDefault(apu);

                ItemComparacion itemComparacion = new ItemComparacion(filaExcel, itemResumenXML);
                itemComparacion.Comparar();
                listaComparacion.Add(itemComparacion);
            }

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
            //el elemento existe en lista excel, y existe en lista XML
            if(this.ResumenXML != null)
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

            // el elemento existe en lista excel y no existe en lista XML 
            else {
                this.ResumenXML = new ResumenXML(new DateTime(1, 1, 1), "", 0.0m, "");
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
