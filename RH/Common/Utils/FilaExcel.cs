using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
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
}
