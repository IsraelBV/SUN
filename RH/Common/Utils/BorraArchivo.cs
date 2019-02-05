using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Common.Utils
{
     public class BorraArchivo
    {
        //-------------constructor
        public void SuprimirArchivo(string path)
        {
            FileInfo fileAux = new FileInfo(path);
            if (fileAux.Exists) fileAux.Delete();
        }

        public void SuprimirArchivo(List<ResumenXML> lista)
        {

            foreach (var item in lista)
            {
                //Armo el nombre del archivo 
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/XML/"), item.Uuid + ".xml");

                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }

        }
    }
}
