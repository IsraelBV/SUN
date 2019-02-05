using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml;
using Nomina.Procesador.Modelos;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Nomina.Procesador.Metodos
{
    public class MSatXml
    {
       // public PathsCertificado ObjCfdi { get; set; }

        #region CFDI 3.2 Nomina 1.2

        private static string GenerarXml12(Modelos.Nomina12.Nomina objetoNomina)
        {
            string result = "";
            Encoding LocalEncoding = Encoding.UTF8;
            MemoryStream stream = null;
            stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            string xmlResult = "";

            XmlSerializerNamespaces nsNom12 = new XmlSerializerNamespaces();
            nsNom12.Add("nomina12", "http://www.sat.gob.mx/nomina12");
            nsNom12.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            XmlSerializer serializer = new XmlSerializer(typeof(Modelos.Nomina12.Nomina));

            //writer = new StreamWriter(stream, LocalEncoding);
            //serializer.Serialize(writer, nom);
            //var buffer = new byte[stream.Length];
            //stream.Read(buffer, 0, (int)stream.Length);
            //xmlResult = LocalEncoding.GetString(buffer);

            using (StreamWriter sw = new StreamWriter(stream))
            {
                serializer.Serialize(writer, objetoNomina, nsNom12);
                xmlResult = LocalEncoding.GetString(stream.ToArray(), 0, (int)stream.Length);
            }

            return xmlResult;
        }
        public static string GenerarXmlComprobante323(Modelos.Cfdi32.Comprobante objetoComprobante)
        {
            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            nameSpaces.Add("la", "myDomain.com");

            //XmlSerializer xmlser = new XmlSerializer(typeof(Report));

            //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            //{
            //    xmlser.Serialize(ms, this, nameSpaces);
            //   // return new UTF8Encoding().GetString(ms.ToArray());
            //}

            ////******************************************************************************************

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add("xxa", "http://www.w3.org/2001/XMLSchema-instance");
            ns.Add("la", "http://www.sat.gob.mx/cfd/3/nomi");


            string result = "";
            Encoding localEncoding = Encoding.UTF8;
            MemoryStream stream = null;
            stream = new MemoryStream();
            // StreamWriter writer = new StreamWriter(stream);

            XmlSerializer serializer = new XmlSerializer(typeof(Modelos.Cfdi32.Comprobante));

            using (StreamWriter w = new StreamWriter(stream))
            {
                serializer.Serialize(w, objetoComprobante, ns);
                result = localEncoding.GetString(stream.ToArray(), 0, (int)stream.Length);
            }

            return result;
        }

        /// <summary>
        /// Retorna el xml del comprobante en formato string
        /// </summary>
        /// <param name="objetoComprobante"></param>
        /// <returns></returns>
        public static string GenerarXmlComprobante32(Modelos.Cfdi32.Comprobante objetoComprobante)
        {
            //Prefix NameSpaces - cfdi
            XmlSerializerNamespaces nsCfdi = new XmlSerializerNamespaces();
            nsCfdi.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            nsCfdi.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
           

            //XmlAttribute attr = doc.CreateAttribute("xsi", "schemaLocation", " ");
            //attr.Value = "http://www.irs.goc/efile ReturnData941.xsd";
            //returnData.Attributes.Append(attr);


            string result = "";
            Encoding localEncoding = Encoding.UTF8;
            MemoryStream stream = null;
            stream = new MemoryStream();
            // StreamWriter writer = new StreamWriter(stream);
          

            XmlSerializer serializer = new XmlSerializer(typeof(Modelos.Cfdi32.Comprobante));

            using (StreamWriter w = new StreamWriter(stream))
            {
               
                serializer.Serialize(w, objetoComprobante, nsCfdi);
                result = localEncoding.GetString(stream.ToArray(), 0, (int) stream.Length);
            }

            return result;
        }

        /// <summary>
        /// Retorna Elemento xml de la nomina para integrarse al xml de cfdi 3.2
        /// </summary>
        /// <param name="objetoNomina"></param>
        /// <returns></returns>
        public static XmlElement GetXmlElement(Modelos.Nomina12.Nomina objetoNomina)
        {
            var result = GenerarXml12(objetoNomina);

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(result);

            return xdoc.DocumentElement;
        }

        /// <summary>
        /// Retorna el Sello para el complemento, y la Cadena Original con la que se genero el sello.
        /// </summary>
        /// <param name="objCfdi"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="strXmlSinSello32"></param>
        /// <param name="pathCadenaOriginalXslt32"></param>
        /// <returns></returns>
        public string GenerarSello(string strXmlSinSello32, string pathCadenaOriginalXslt32, PathsCertificado objCfdi,  ref string cadenaOriginal)
        {
            try
            {
                var noCertificado = "";
                var certificadoB64 = "";

                strXmlSinSello32 = strXmlSinSello32.Replace("&", "&amp;");

                if (!File.Exists(pathCadenaOriginalXslt32)) throw new InvalidOperationException($"No se encontro el archivo: {strXmlSinSello32}");

                cadenaOriginal = MCfdi.GetCadenaOriginal(strXmlSinSello32, pathCadenaOriginalXslt32);
                // proceso para generar el sello  
                var sello = MCfdi.GetSelloDigitalEmisor(ref noCertificado, ref certificadoB64, cadenaOriginal, objCfdi);
           
                return sello;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string GenerarSello33(string strXmlSinSello33, string pathCadenaOriginalXslt33, PathsCertificado objCfdi, ref string cadenaOriginal)
        {
            try
            {
                var noCertificado = "";
                var certificadoB64 = "";

                strXmlSinSello33 = strXmlSinSello33.Replace("&", "&amp;");

                if (!File.Exists(pathCadenaOriginalXslt33)) throw new InvalidOperationException($"No se encontro el archivo: {strXmlSinSello33}");

                cadenaOriginal = MCfdi.GetCadenaOriginal(strXmlSinSello33, pathCadenaOriginalXslt33);
                // proceso para generar el sello  
                var sello = MCfdi.GetSelloDigitalEmisor33(ref noCertificado, ref certificadoB64, cadenaOriginal, objCfdi);

                return sello;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion

        //VERSION 2 3.3
        public string GenerarSello33_v2(string strXmlSinSello33, XslCompiledTransform transformador, byte[] archivoCer, byte[] archivoKey, string password, ref string cadenaOriginal)
        {
            try
            {
                var noCertificado = "";
                var certificadoB64 = "";

                strXmlSinSello33 = strXmlSinSello33.Replace("&", "&amp;");

               // if (!File.Exists(pathCadenaOriginalXslt33)) throw new InvalidOperationException($"No se encontro el archivo: {strXmlSinSello33}");

                cadenaOriginal = MCfdi.GetCadenaOriginal_v2(strXmlSinSello33, transformador);
                // proceso para generar el sello  
                var sello = MCfdi.GetSelloDigitalEmisor33_v2(ref noCertificado, ref certificadoB64, cadenaOriginal, archivoCer, archivoKey, password);

                return sello;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #region CFDI 3.3 -  Nomina 1.2
        public static string GenerarXmlComprobante33(Modelos.Cfdi33.Comprobante objetoComprobante)
        {
            //Prefix NameSpaces - cfdi
            XmlSerializerNamespaces nsCfdi = new XmlSerializerNamespaces();
            nsCfdi.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            nsCfdi.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");


            //XmlAttribute attr = doc.CreateAttribute("xsi", "schemaLocation", " ");
            //attr.Value = "http://www.irs.goc/efile ReturnData941.xsd";
            //returnData.Attributes.Append(attr);


            string result = "";
            Encoding localEncoding = Encoding.UTF8;
            MemoryStream stream = null;
            stream = new MemoryStream();
            // StreamWriter writer = new StreamWriter(stream);


            XmlSerializer serializer = new XmlSerializer(typeof(Modelos.Cfdi33.Comprobante));

            using (StreamWriter w = new StreamWriter(stream))
            {

                serializer.Serialize(w, objetoComprobante, nsCfdi);
                result = localEncoding.GetString(stream.ToArray(), 0, (int)stream.Length);
            }

            return result;
        }
        #endregion

    }
}
