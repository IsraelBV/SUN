using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
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
