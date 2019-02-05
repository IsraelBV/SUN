using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nomina.Procesador.Modelos;
using JavaScience;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Common.Utils;

namespace Nomina.Procesador.Metodos
{
    public static class MCfdi
    {
        /// <summary>
        /// Metodo para obtener los datos de Certificado, CertificadoBase64 del archivo .CER del cliente
        /// </summary>
        /// <param name="pathDelArchivoCer"></param>
        /// <returns></returns>
        public static CertificadoDigital GetCertificadoDigital(string pathDelArchivoCer)
        {
            var srtBase64 = "";
            var certificado = new CertificadoDigital();

            System.Security.Cryptography.X509Certificates.X509Certificate2 certEmisor = new System.Security.Cryptography.X509Certificates.X509Certificate2();
            byte[] data = Utils.GetBytesArchivo(pathDelArchivoCer);

            certEmisor.Import(data);

            srtBase64 = Convert.ToBase64String(certEmisor.GetRawCertData());

            certificado.Certificado = certEmisor.GetRawCertDataString();
            certificado.CertificadoBase64 = srtBase64;

            byte[] byteArray = certEmisor.GetSerialNumber();
            //string test = byteArray.ToString();

            string strSerialHex = certEmisor.GetSerialNumberString();

            string serialTest2 = certEmisor.SerialNumber;

            var strSerial = ConvertHexToString(strSerialHex);

            //var str = System.Text.Encoding.Default.GetString(byteArray);

            //string result = System.Text.Encoding.UTF8.GetString(byteArray);

            //System.Text.Encoding enc = System.Text.Encoding.ASCII;
            //string myString = enc.GetString(byteArray);
            //string s = System.Text.UTF8Encoding.UTF8.GetString(byteArray);


            //char[] array = str.ToCharArray();
            //Array.Reverse(array);
            //var nuevoStr = new string(array);

            certificado.NoCertificado = strSerial;//str;

            return certificado;

        }

        public static string ConvertHexToString(string hexValue)
        {
            string strValue = "";
            while (hexValue.Length > 0)
            {
                strValue += System.Convert.ToChar(System.Convert.ToUInt32(hexValue.Substring(0, 2), 16)).ToString();
                hexValue = hexValue.Substring(2, hexValue.Length - 2);
            }
            return strValue;
        }

        /// <summary>
        /// GET - Genera la cadena original del cfdi, usando el path del archivo xml
        /// </summary>
        /// <param name="xmlSintimbre"></param>
        /// <param name="pathCadenaOriginalXslt32"></param>
        /// <returns></returns>
        public static string GetCadenaOriginal(string xmlSintimbre, string pathCadenaOriginalXslt32)
        {
            string cadenaOriginal = "";

            //Carga el xml Generado
            StringReader sr = new StringReader(xmlSintimbre);
            XPathDocument xmlGenerado = new XPathDocument(sr);

            //Carga el xslt
            //StreamReader archivoXslt = new StreamReader(pathXslt);
            //XPathDocument xsltGenerado = new XPathDocument(archivoXslt);

            //configgura el transformador 
            XslCompiledTransform transformador = new XslCompiledTransform();
            XsltSettings sett = new XsltSettings(true, true);
            transformador.Load(pathCadenaOriginalXslt32);//transformador.Load(xsltGenerado);

            StringWriter strWriter = new StringWriter();
            XmlTextWriter xmlText = new XmlTextWriter(strWriter);

            //Aplicamos la transformacion
            transformador.Transform(xmlGenerado, null, strWriter);

            //Resultado
            cadenaOriginal = strWriter.ToString();
            return cadenaOriginal;

            ////Cargar el XML
            //StreamReader reader = new StreamReader(xmlSintimbre);
            //XPathDocument myXPathDoc = new XPathDocument(reader);

            ////Cargando el XSLT
            //XslCompiledTransform myXslTrans = new XslCompiledTransform();
            //myXslTrans.Load(pathXslt);

            //StringWriter str = new StringWriter();
            //XmlTextWriter myWriter = new XmlTextWriter(str);

            ////Aplicando transformacion
            //myXslTrans.Transform(myXPathDoc, null, myWriter);

            ////Resultado
            //string result = str.ToString();

            //return result;
        }

        /// <summary>
        /// Genera el Sello digital para el cfdi -  pasando el archivo xslt -
        /// internamente se genera la cadena original
        /// </summary>
        /// <param name="noCertificado"></param>
        /// <param name="certificadoBase64"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="pathXslt"></param>
        /// <param name="datos"></param>
        /// <returns></returns>
        public static string GetSelloDigitalEmisor(ref string noCertificado, ref string certificadoBase64, ref string cadenaOriginal, string pathXslt, PathsCertificado datos)
        {

            string selloDigital = null;

            try
            {

                CertificadoDigital cert = GetCertificadoDigital(datos.PathArchivoCer);
                noCertificado = cert.NoCertificado;
                certificadoBase64 = cert.CertificadoBase64;
                cadenaOriginal = GetCadenaOriginal(datos.PathArchivoXmlGenerado, pathXslt);

                SecureString identidad = new SecureString();

                identidad.Clear();


                foreach (var c in datos.Password.ToCharArray())
                {
                    identidad.AppendChar(c);
                }

                byte[] llavePrivadaBytes = File.ReadAllBytes(datos.PathArchivoKey);

                RSACryptoServiceProvider lrsa = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
                SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                byte[] bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher);

                selloDigital = Convert.ToBase64String(bytesFirmados);

                return selloDigital;
            }
            catch (Exception)
            {
                return selloDigital;
            }


        }

        /// <summary>
        /// Genera la cadena original del cfdi, pasando el xml como string
        /// </summary>
        /// <param name="strXmlSinTimbre"></param>
        /// <param name="pathXslt"></param>
        /// <returns></returns>
        public static string GetCadenaOriginalByString(string strXmlSinTimbre, string pathXslt)
        {
            string cadenaOriginal = "";


            StringReader sr = new StringReader(strXmlSinTimbre);
            XPathDocument xmlGenerado = new XPathDocument(sr);

            //Cargar el xml Generado
            //    StreamReader archivoXml = new StreamReader(XmlSinTimbre);
            //  XPathDocument xmlGenerado = new XPathDocument(archivoXml);

            //Cargar el xslt
            StreamReader archivoXslt = new StreamReader(pathXslt);
            XPathDocument xsltGenerado = new XPathDocument(archivoXslt);

            XslCompiledTransform transformador = new XslCompiledTransform();
            transformador.Load(xsltGenerado);

            StringWriter strWriter = new StringWriter();
            XmlTextWriter xmlText = new XmlTextWriter(strWriter);

            //Aplicamos la transformacion
            transformador.Transform(xmlGenerado, null, strWriter);

            //Resultado
            cadenaOriginal = strWriter.ToString();
            return cadenaOriginal;

        }

        /// <summary>
        /// Genera el sello digital para el cfdi - pasando como string la cadena original generada previamente
        /// </summary>
        /// <param name="noCertificado"></param>
        /// <param name="certificadoB64"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="datos"></param>
        /// <returns></returns>
        public static string GetSelloDigitalEmisor(ref string noCertificado, ref string certificadoB64, string cadenaOriginal, PathsCertificado datos)
        {
            string selloDigital = null;

            try
            {

                CertificadoDigital cert = GetCertificadoDigital(datos.PathArchivoCer);
                noCertificado = cert.NoCertificado;
                certificadoB64 = cert.CertificadoBase64;

                SecureString identidad = new SecureString();
                identidad.Clear();

                foreach (var c in datos.Password.ToCharArray())
                {
                    identidad.AppendChar(c);
                }

                byte[] llavePrivadaBytes = File.ReadAllBytes(datos.PathArchivoKey);

                RSACryptoServiceProvider lrsa = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
                SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                byte[] bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher);

                selloDigital = Convert.ToBase64String(bytesFirmados);
                return selloDigital;
            }
            catch (Exception)
            {
                return selloDigital;
            }
        }

        /// <summary>
        /// Sello digital SHA256 para la version 3.3
        /// </summary>
        /// <param name="noCertificado"></param>
        /// <param name="certificadoB64"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="datos"></param>
        /// <returns></returns>
        public static string GetSelloDigitalEmisor33(ref string noCertificado, ref string certificadoB64, string cadenaOriginal, PathsCertificado datos)
        {
            string selloDigital = null;

            try
            {
                CertificadoDigital cert = GetCertificadoDigital(datos.PathArchivoCer);
                noCertificado = cert.NoCertificado;
                certificadoB64 = cert.CertificadoBase64;

                SecureString identidad = new SecureString();
                identidad.Clear();

                foreach (var c in datos.Password.ToCharArray())
                {
                    identidad.AppendChar(c);
                }

                byte[] llavePrivadaBytes = File.ReadAllBytes(datos.PathArchivoKey);

                RSACryptoServiceProvider lrsa = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
                SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
                byte[] bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher);
                //bool isValid = lrsa.VerifyData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher, bytesFirmados);

                selloDigital = Convert.ToBase64String(bytesFirmados);

                return selloDigital;
            }
            catch (Exception ex)
            {

                return selloDigital;
            }


        }

        /// <summary>
        /// Metodo donde ser realizaron varias formas para obtener el Sello a SHA256
        /// </summary>
        /// <param name="noCertificado"></param>
        /// <param name="certificadoB64"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="datos"></param>
        /// <returns></returns>
        public static string GetSelloDigitalEmisor33123(ref string noCertificado, ref string certificadoB64,
            string cadenaOriginal, PathsCertificado datos)
        {
            string selloDigital = "";

            CertificadoDigital cert = GetCertificadoDigital(datos.PathArchivoCer);
            noCertificado = cert.NoCertificado;
            certificadoB64 = cert.CertificadoBase64;


            //Metodo 1
            SecureString identidad = new SecureString();
            identidad.Clear();

            foreach (var c in datos.Password.ToCharArray())
            {
                identidad.AppendChar(c);
            }

            byte[] llavePrivadaBytes = File.ReadAllBytes(datos.PathArchivoKey);
            var data = Encoding.UTF8.GetBytes(cadenaOriginal);

            //METODO SHA1
            RSACryptoServiceProvider lrsa0 = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
            SHA1CryptoServiceProvider hasher0 = new SHA1CryptoServiceProvider();
            byte[] bytesFirmados0 = lrsa0.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher0);

            var selloDigital0 = Convert.ToBase64String(bytesFirmados0);


            #region METODO 1
            RSACryptoServiceProvider lrsa1 = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
            SHA256CryptoServiceProvider hasher1 = new SHA256CryptoServiceProvider();

            byte[] bytesFirmados1 = lrsa1.SignData(data, hasher1);

            //byte[] bytesFirmados11 = lrsa1.SignData(data, "sha256");

            bool isValid = lrsa1.VerifyData(data, hasher1, bytesFirmados1);

            //byte[] bytesFirmados111 = lrsa1.SignData(data, HashAlgorithmName.SHA256);

            //var selloDigital11 = Convert.ToBase64String(bytesFirmados11);
            //var selloDigital111 = Convert.ToBase64String(bytesFirmados111);

            selloDigital = Convert.ToBase64String(bytesFirmados1);

            #endregion

            #region METODO 2

            RSACryptoServiceProvider lrsa2 = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
            SHA256 sha256 = SHA256Managed.Create();

            byte[] bytes2 = Encoding.UTF8.GetBytes(cadenaOriginal);
            byte[] hash = sha256.ComputeHash(bytes2);
            //return GetStringFromHash(hash);
            //CryptoConfig.CreateFromName("SHA256");
            byte[] bytesFirmados2 = lrsa2.SignData(data, sha256);

            var selloDigital2 = Convert.ToBase64String(bytesFirmados2);

            #endregion

            #region METODO 3

            RSACryptoServiceProvider lrsa3 = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
            Byte[] inputBytes = Encoding.UTF8.GetBytes(cadenaOriginal);
            SHA256CryptoServiceProvider sha3 = new SHA256CryptoServiceProvider();

            Byte[] hashedBytes = sha3.ComputeHash(inputBytes);

            byte[] bytesFirmados3 = lrsa3.SignData(data, sha3);

            var selloDigital3 = Convert.ToBase64String(bytesFirmados3);

            #endregion

            #region METODO 4

            string strSello = string.Empty;

            RSACryptoServiceProvider rsa4 = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);

            SHA256Managed sha = new SHA256Managed();
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(cadenaOriginal);
            byte[] digest = sha.ComputeHash(bytes);

            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa4);
            rsaFormatter.SetHashAlgorithm("SHA256");
            byte[] signedHashValue4 = rsaFormatter.CreateSignature(digest);

            SHA256CryptoServiceProvider hasher4 = new SHA256CryptoServiceProvider();

            byte[] bytesFirmados4 = rsa4.SignData(System.Text.Encoding.UTF8.GetBytes(cadenaOriginal), hasher4);

            strSello = Convert.ToBase64String(bytesFirmados4);  // Y aquí está el sello
            string strSello256 = Convert.ToBase64String(signedHashValue4);// Y aquí está el sello 2 

            #endregion

            #region METODO 5
            //X509Certificate2 miCertificado5 = new X509Certificate2(datos.PathArchivoCer, datos.Password);

            //RSACryptoServiceProvider rsa5 = (RSACryptoServiceProvider)miCertificado5.PrivateKey;
            //SHA256CryptoServiceProvider hasher5 = new SHA256CryptoServiceProvider();
            //UTF8Encoding e = new UTF8Encoding(true);

            ////byte[] signature = RSA1.SignHash(hasher5, CryptoConfig.MapNameToOID("SHA256"));

            //byte[] bytesFirmados5 = rsa5.SignData(e.GetBytes(cadenaOriginal), hasher5);
            //var selloDigital5 = Convert.ToBase64String(bytesFirmados5);

            #endregion

            #region METODO 6
            //System.Security.Cryptography.X509Certificates.X509Certificate2 miCertificado6 = new System.Security.Cryptography.X509Certificates.X509Certificate2(datos.PathArchivoCer, datos.Password);

            // GetRSAPrivateKey returns an object with an independent lifetime, so it should be
            // handled via a using statement.
            //using (RSA rsa6 = miCertificado6.GetRSAPrivateKey())
            //{
                // RSA now exposes SignData, and the hash algorithm parameter takes a strong type,
                // which allows for IntelliSense hints.
                //var bytesFirmados6 = rsa6.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                //var selloDigital6 = Convert.ToBase64String(bytesFirmados6);
            //}



            #endregion




            return selloDigital;
        }

        public static string GetCadenaOriginal_v2(string xmlSintimbre, XslCompiledTransform transformador)
        {
            string cadenaOriginal = "";

            //var reader = XmlReader.Create(File.OpenRead(pathCadenaOriginalXslt32));

            //Carga el xml Generado
            StringReader sr = new StringReader(xmlSintimbre);
            XPathDocument xmlGenerado = new XPathDocument(sr);

            //Carga el xslt
            //StreamReader archivoXslt = new StreamReader(pathXslt);
            //XPathDocument xsltGenerado = new XPathDocument(archivoXslt);

            //configgura el transformador 
           //aqui XslCompiledTransform transformador = new XslCompiledTransform();
            XsltSettings sett = new XsltSettings(true, true);
           //aqui  transformador.Load(reader);//transformador.Load(pathCadenaOriginalXslt32);//transformador.Load(xsltGenerado);

            StringWriter strWriter = new StringWriter();
            XmlTextWriter xmlText = new XmlTextWriter(strWriter);

            //Aplicamos la transformacion
            transformador.Transform(xmlGenerado, null, strWriter);

            //Resultado
            cadenaOriginal = strWriter.ToString();
            return cadenaOriginal;

            ////Cargar el XML
            //StreamReader reader = new StreamReader(xmlSintimbre);
            //XPathDocument myXPathDoc = new XPathDocument(reader);

            ////Cargando el XSLT
            //XslCompiledTransform myXslTrans = new XslCompiledTransform();
            //myXslTrans.Load(pathXslt);

            //StringWriter str = new StringWriter();
            //XmlTextWriter myWriter = new XmlTextWriter(str);

            ////Aplicando transformacion
            //myXslTrans.Transform(myXPathDoc, null, myWriter);

            ////Resultado
            //string result = str.ToString();

            //return result;
        }

        public static string GetSelloDigitalEmisor33_v2(ref string noCertificado, ref string certificadoB64, string cadenaOriginal, byte[] archivoCer, byte[] archivoKey, string password)
        {
            string selloDigital = null;

            try
            {
                CertificadoDigital cert = GetCertificadoDigital_v2(archivoCer);
                noCertificado = cert.NoCertificado;
                certificadoB64 = cert.CertificadoBase64;

                SecureString identidad = new SecureString();
                identidad.Clear();

                foreach (var c in password.ToCharArray())
                {
                    identidad.AppendChar(c);
                }

                byte[] llavePrivadaBytes = archivoKey;

                RSACryptoServiceProvider lrsa = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, identidad);
                SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
                byte[] bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher);
                //bool isValid = lrsa.VerifyData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher, bytesFirmados);

                selloDigital = Convert.ToBase64String(bytesFirmados);

                return selloDigital;
            }
            catch (Exception ex)
            {

                return selloDigital;
            }


        }

        public static CertificadoDigital GetCertificadoDigital_v2(byte[] archivoCer)
        {
            var srtBase64 = "";
            var certificado = new CertificadoDigital();

            System.Security.Cryptography.X509Certificates.X509Certificate2 certEmisor = new System.Security.Cryptography.X509Certificates.X509Certificate2();
            byte[] data = archivoCer;

            certEmisor.Import(data);

            srtBase64 = Convert.ToBase64String(certEmisor.GetRawCertData());

            certificado.Certificado = certEmisor.GetRawCertDataString();
            certificado.CertificadoBase64 = srtBase64;

            byte[] byteArray = certEmisor.GetSerialNumber();
            //string test = byteArray.ToString();

            string strSerialHex = certEmisor.GetSerialNumberString();

            string serialTest2 = certEmisor.SerialNumber;

            var strSerial = ConvertHexToString(strSerialHex);

            //var str = System.Text.Encoding.Default.GetString(byteArray);

            //string result = System.Text.Encoding.UTF8.GetString(byteArray);

            //System.Text.Encoding enc = System.Text.Encoding.ASCII;
            //string myString = enc.GetString(byteArray);
            //string s = System.Text.UTF8Encoding.UTF8.GetString(byteArray);


            //char[] array = str.ToCharArray();
            //Array.Reverse(array);
            //var nuevoStr = new string(array);

            certificado.NoCertificado = strSerial;//str;

            return certificado;

        }
    }
}
