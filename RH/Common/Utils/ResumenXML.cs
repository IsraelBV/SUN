using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
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
}
