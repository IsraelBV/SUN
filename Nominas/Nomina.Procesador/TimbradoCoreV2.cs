using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Nomina.Procesador.Datos;
using Nomina.Procesador.Modelos;
using RH.Entidades;
using RH.Entidades.GlobalModel;
using SYA.BLL;
using Common.Utils;
using Nomina.Procesador.Metodos;
using Nomina.Procesador.Modelos.Cfdi33;
using Nomina.Procesador.Modelos.Nomina12;
using Nomina.Procesador.webServicePAC;
using RH.BLL;
using ConceptosNomina = Nomina.Procesador.Datos.ConceptosNomina;


namespace Nomina.Procesador
{


    public class TimbradoCoreV2
    {
        //Variables globales
        private readonly TimbradoDao _dao = new TimbradoDao();
        private readonly GlobalConfig _gconfig = new GlobalConfig();

        private bool TimbradoDePrueba { get; set; }

        //Constructor

        //Metodos Asyncronos

        //Crear XML


        public Task<List<NotificationSummary>> GenerarCfdiV2Async(int idUsuario, int[] nominasSeleccionadas = null, NOM_PeriodosPago periodo = null, SYA_GlobalConfig globalConfig = null, bool fromModuloProcesar = false)
        {
            var t = Task.Factory.StartNew(() =>
            {
                if (nominasSeleccionadas == null) return null;

                var r = CrearXml(idUsuario, periodo, globalConfig, nominasSeleccionadas, fromModuloProcesar);

                var rt = Timbrado33_v2(periodo, nominasSeleccionadas, globalConfig);

                r.AddRange(rt);

                return r;
            });

            return t;
        }

        public Task<int> CrearXmlDesdeElProcesado(int idUsuario, NOM_PeriodosPago ppago, SYA_GlobalConfig configuracionGlobal, int[] nominasSeleccionadas, bool fromModuloProcesar)
        {
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    CrearXml(idUsuario, ppago, configuracionGlobal, nominasSeleccionadas, fromModuloProcesar);
                }
                catch (Exception)
                {
                }
                return 1;
            });

            return t;
        }

        public List<NotificationSummary> CrearXml(int idUsuario, NOM_PeriodosPago ppago, SYA_GlobalConfig configuracionGlobal, int[] nominasSeleccionadas, bool fromModuloProcesar)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            bool esFiniquito = false;

            //Lista de Datos de la nomina
            List<NominaData> listaDatosNomina = new List<NominaData>();

            //obtiene los datos de las nominas y empleados
            if (ppago.IdTipoNomina == 11)//finiquito
            {
                esFiniquito = true;
                var itemData = _dao.GetDatosFiniquito(ppago.IdEjercicio, ppago.IdPeriodoPago, nominasSeleccionadas[0], ref summaryList);

                listaDatosNomina.Add(itemData);

            }
            else
            {
                listaDatosNomina = _dao.GetDatosNomina(ppago.IdEjercicio, ppago.IdPeriodoPago, nominasSeleccionadas, ppago.IdTipoNomina, ref summaryList);
            }

            //Get dato del cliente
            var itemCliente = _dao.GetClienteByIdCliente(ppago.IdCliente);
            var pathCertificados = "";
            //Obtenemos de la tabla [SYA_GlobalConfig] - el path de los Certificados archivs .CER .KEY
            if (configuracionGlobal == null)
            {
                var configGlobal = _gconfig.GetGlobalConfig();

                if (configGlobal == null)
                {
                    summaryList.Add(new NotificationSummary()
                    {
                        Reg = 0,
                        Msg1 = $"No existen los parametros de configuracion",
                        Msg2 = ""
                    });

                    return summaryList;
                }

                pathCertificados = configGlobal.PathCertificados;
            }
            else
            {
                pathCertificados = configuracionGlobal.PathCertificados;
            }



            List<NOM_CFDI_Timbrado> listaXmlGenerados = new List<NOM_CFDI_Timbrado>();
            //1)Por cada nomina se crea un xml y se anexa a la lista

            //GET DATA LIST
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<ConceptosNomina> listaPercepciones = new List<ConceptosNomina>();
            List<ConceptosNomina> listaDeducciones = new List<ConceptosNomina>();
            List<ConceptosNomina> listaListaOtrosPagos = new List<ConceptosNomina>();
            List<NOM_Incapacidad> listaIncapacidadesGeneral = new List<NOM_Incapacidad>();

            var arrayEmpleados = listaDatosNomina.Select(x => x.IdEmpleado).Distinct().ToArray();
            var arrayNominas = listaDatosNomina.Select(x => x.IdNomina).Distinct().ToArray();

            if (esFiniquito)
                arrayNominas = listaDatosNomina.Select(x => x.IdFiniquito).Distinct().ToArray();

            listaEmpresas = _dao.GetListaEmpresas();
            listaEmpleados = _dao.GetListaEmpleados(arrayEmpleados);
            listaPercepciones = _dao.GetListaPercepciones(arrayNominas, esFiniquito);
            listaDeducciones = _dao.GetListaDeducciones(arrayNominas, esFiniquito);
            listaListaOtrosPagos = _dao.GetListaListaOtrosPagos(arrayNominas, esFiniquito);
            listaIncapacidadesGeneral = _dao.GetListaIncapacidadesGeneral(arrayNominas, esFiniquito);

            //Validar los archivos fisicos, aunque puede ser antes de este metodo

            //Generar los bytes de certificados y reader del xslt
            var pathCadenaOriginalXslt = pathCertificados + "\\" + "cadenaoriginal_3_3_L.xslt";
            //XmlReader readerXsltCadenaOriginal;//XmlReader.Create(File.OpenRead(pathCadenaOriginalXslt));
            //eaderXsltCadenaOriginal = XmlReader.Create(File.OpenRead(pathCadenaOriginalXslt));

            //using (StringReader sr = new StringReader(pathCadenaOriginalXslt))
            //{
            //    readerXsltCadenaOriginal = XmlReader.Create(sr);
            //}

            if (!Utils.ExisteArchivo(pathCadenaOriginalXslt))
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = $"No se encontró el archivo: cadenaoriginal_3_3_L.xslt",
                    Msg2 = ""
                });

                return summaryList;
            }

            XslCompiledTransform transformador = new XslCompiledTransform();
            transformador.Load(pathCadenaOriginalXslt);


            int idEmpresaArchivos = 0;
            byte[] archivoCer = null;
            byte[] archivoKey = null;
            string strPasswordEmisor = "";
            foreach (var itemData in listaDatosNomina)
            {
                try//usamos el try para evitar que se detenga por algun error de un registro
                {
                    List<NotificationSummary> resultMesg = new List<NotificationSummary>();

                    if (idEmpresaArchivos != itemData.IdEmisor)
                    {
                        idEmpresaArchivos = itemData.IdEmisor;
                        var emisorDatos = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == itemData.IdEmpresa);

                        strPasswordEmisor = emisorDatos.LlavePrivada;
                        var varArchivoCer = pathCertificados + "\\" + emisorDatos.IdEmpresa + "\\" + emisorDatos.ArchivoCER;
                        var varArchivoKey = pathCertificados + "\\" + emisorDatos.IdEmpresa + "\\" + emisorDatos.ArchivoKEY;

                        if (!Utils.ExisteArchivo(varArchivoCer))
                        {
                            summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = $"No se encontró el archivo {emisorDatos.ArchivoCER}", Msg2 = "" });
                            continue;
                        }

                        if (!Utils.ExisteArchivo(varArchivoKey))
                        {
                            summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = $"No se encontró el archivo {emisorDatos.ArchivoKEY}", Msg2 = "" });
                            continue;
                        }

                        archivoCer = File.ReadAllBytes(varArchivoCer);
                        archivoKey = File.ReadAllBytes(varArchivoKey);
                    }


                    //Crea el xml 3.3
                    //Debug.WriteLine($"{cont} {strIni} data: {itemData.IdNomina}  {DateTime.Now}");
                    var registro = CrearXmlConSelloYSinTimbre3312_v2(ref resultMesg, itemData, ppago, idUsuario,
                        esFiniquito, itemCliente,
                        listaEmpresas,
                        listaEmpleados,
                        listaPercepciones,
                        listaDeducciones,
                        listaListaOtrosPagos,
                        listaIncapacidadesGeneral,
                        transformador,
                        archivoCer,
                        archivoKey,
                        strPasswordEmisor, fromModuloProcesar
                    );


                    if (registro == null) continue;

                    //si registro es correcto , se agrega a la lista
                    if (registro.XMLSinTimbre != null)
                    {
                        //Debug.WriteLine($"{cont} {strIni} data: {itemData.IdNomina}  {DateTime.Now}");
                        listaXmlGenerados.Add(registro);

                    }
                }
                catch (Exception e)
                {

                }


            }



            //2) Guardar los registros a la BD
            // si el registro viene del modulo de procesar se guarda en la tabla de nominas o finiquito
            GuardarXMlenDb(listaXmlGenerados, esFiniquito, fromModuloProcesar);



            return summaryList;
        }

        private void GuardarXMlenDb(List<NOM_CFDI_Timbrado> listaXmlGenerados, bool esFiniquito, bool fromModuloProcesar)
        {
            if (fromModuloProcesar)
            {
                //Actualizar las tablas
                _dao.ActualizarXmlToTable(listaXmlGenerados, esFiniquito);
            }
            else
            {
                _dao.InsertXmlToDb(listaXmlGenerados);
            }
        }

        //Realizar el Timbrado
        public List<NotificationSummary> Timbrado33_v2(NOM_PeriodosPago periodo, int[] nominasSeleccionadas, SYA_GlobalConfig servidorConfig)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            string _remoteAddress = "";
            bool isfiniquito = false;
            isfiniquito = periodo.IdTipoNomina == 11;
            List<int> nominasTimbradasCorrectamente = new List<int>();

            //GET DATA
            // consultar los registros a timbrar de la tabla de nom_cfdi_timbrado
            var listaRegistros = _dao.GetRegistrosATimbrar(nominasSeleccionadas, isFiniquito: isfiniquito);

            //validar que la lista no sea null 
            if (listaRegistros == null)
            {
                summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontraron registros para timbrar", Msg2 = "" });
                return summaryList;
            }

            //DATOS de [SYA_GlobalConfig]
            //GlobalConfig gconfig = new GlobalConfig();
            //var servidorConfig = gconfig.GetGlobalConfig();

            // obetener las claves de usuario para el timbrado
            var usuarioPac = "";
            var passwordPac = "";

            usuarioPac = servidorConfig.UsuarioPac;
            passwordPac = servidorConfig.PasswordPac;

            //Si es timbrado de prueba, se asigna el web service de Stage
            //sino se toma la url real
            // tomar la url desde el objeto servidor
            // https://stagetimbrado.facturador.com/timbrado.asmx
            _remoteAddress = servidorConfig.UrlPac;

            //Actualizamos el Objeto SOAP con la url para el timbrado
            System.Net.ServicePointManager.Expect100Continue = false;
            var clienteSoap = new wsTimbradoSoapClient("wsTimbradoSoap", _remoteAddress);
            clienteSoap.Open();

            #region "SE REALIZA EL TIMBRADO POR CADA REGISTRO DE LA LISTA Y ACTUALIZA LA BD"

            int registrosTimbrado = 0;
            string strError = "";


            try
            {


                #region EJECUCION DEL FOR

                foreach (var item in listaRegistros)
                {
                    try
                    {
                        //Se envia la Solicitud de Timbrado al web service del PAC 
                        // Solicitud --> PAC --> Valida el xml -->PAC -> Solicita el Timbre al SAT
                        // Respuesta <-- PAC <-- SAT retornar el timbre al PAC


                        // System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AddressOf ValidarCertificado);
                        System.Net.ServicePointManager.ServerCertificateValidationCallback =
                            new System.Net.Security.RemoteCertificateValidationCallback(ValidarCertificado);
                        var respuestaPac = clienteSoap.obtenerTimbrado(item.XMLSinTimbre, usuarioPac, passwordPac);


                        //VALIDAR RESPUESTA ANTES DE INTEGRAR EL TIMBRE AL XML DEL CFDI
                        if (EsRespuestaValida(respuestaPac))
                        {
                            #region ACTUALIZAR EL REGISTRO EN BD Y XML CON LOS DATOS DEL TIMBRADO

                            nominasTimbradasCorrectamente.Add(isfiniquito ? item.IdFiniquito : item.IdNomina);

                            //Convertir la respuesta del PAC en string
                            string respuesta = respuestaPac.ToString();

                            //Separa los tag de la respuesta
                            string[] arrayTag = respuesta.Split('>');

                            //se genera el tag donde se incluye el timbre de la respuesta del pac
                            string tagTimbre = "</nomina12:Nomina>" + arrayTag[1] + ">";

                            //se anexa el tag al XML 
                            string xmlNuevo = item.XMLSinTimbre.Replace("</nomina12:Nomina>", tagTimbre);

                            //Si el xmlNuevo contiene un timbre y esto no contiene el valor del atributo error
                            //guardamos el timbrado en la BD
                            registrosTimbrado++;

                            //actualizamos el item con su valor 
                            item.XMLTimbrado = xmlNuevo;

                            //Actualizar el registro con los valores del timbrado, uiid, certificado, fecha de timbrado, etc.
                            XmlDocument xdoc = new XmlDocument();
                            xdoc.LoadXml(xmlNuevo);

                            XmlNodeList node = xdoc.GetElementsByTagName("tfd:TimbreFiscalDigital");
                            foreach (XmlElement elemento in node)
                            {
                                item.FechaCertificacion = DateTime.Parse((string)elemento.GetAttribute("FechaTimbrado"));
                                item.FolioFiscalUUID = (string)elemento.GetAttribute("UUID");
                                item.Folio = item.IdTimbrado;
                            }
                            item.ErrorTimbrado = false;


                            #endregion
                        }
                        else
                        {

                            //Convertir la respuesta del PAC en string
                            string respuesta = respuestaPac.ToString();
                            item.ErrorTimbrado = true;
                            item.MensajeRespuesta = respuesta;

                        }
                    }
                    catch (Exception ex)
                    {

                        item.ErrorTimbrado = true;
                        item.MensajeRespuesta = "ex ? - " + ex.Message;
                    }
                    finally
                    {
                        //ACTUALIZA CADA REGISTRO DE LA TABLA NOM_CFDI_TIMBRADO
                        _dao.ActualizarCambiosTimbrado(item);
                    }



                } //final for

                #endregion

            }
            catch (Exception e)
            {

            }
            finally
            {
                clienteSoap.Close();
                ActualizarPeriodoYNomina(periodo, nominasTimbradasCorrectamente, isfiniquito);

            }

            #endregion





            return summaryList;
        }

        //
        private Boolean ValidarCertificado(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static bool EsRespuestaValida(XElement mensajeRespuesta)
        {
            if (mensajeRespuesta == null) return false;

            bool result = false;
            string strRespuesta = mensajeRespuesta.ToString().ToLower();

            //1.- Buscar el texto esValido="true"
            if (strRespuesta.Contains("esvalido=\"true\"") && strRespuesta.Contains("timbrefiscaldigital"))
            {
                result = true;
            }

            return result;
        }

        //Realizar la cancelacion

        //Actualizar Periodo y nominas
        private void ActualizarPeriodoYNomina(NOM_PeriodosPago ppPago, List<int> nominas, bool esFiniquito)
        {
            var t = Task.Factory.StartNew(() =>
            {
                _dao.ActualizarNominaCfdiCreado(nominas, ppPago, esFiniquito);
            });


        }

        #region CFDI 3.3 Version 2


        /// <summary>
        /// Crea el archivo xml con la version 3.3 y complemento de nomina 1.2
        /// </summary>
        /// <param name="datosNomina"></param>
        /// <param name="periodoPago"></param>
        /// <param name="idUsuario"></param>
        /// <param name="isFiniquito"></param>
        /// <param name="fromModuloProcesar"></param>
        /// <returns></returns>
        private NOM_CFDI_Timbrado CrearXmlConSelloYSinTimbre3312_v2(ref List<NotificationSummary> summaryList, NominaData datosNomina, NOM_PeriodosPago periodoPago, int idUsuario, bool isFiniquito,
            Cliente itemCliente,
            List<Empresa> listaEmpresas,
            List<Empleado> listaEmpleados,
            List<Nomina.Procesador.Datos.ConceptosNomina> listaPercepciones,
            List<Nomina.Procesador.Datos.ConceptosNomina> listaDedudcciones,
            List<Nomina.Procesador.Datos.ConceptosNomina> listaOtrosPagos,
            List<NOM_Incapacidad> listaIncapacidadesGeneral,
            XslCompiledTransform transformador,
           byte[] archivoCer,
           byte[] archivoKey,
           string password,

            bool fromModuloProcesar = true)
           {


            if (datosNomina == null) return null; //se suspende la creacion del cfdi de este registro

            try
            {
                #region VARIABLES

                MSatXml xmlsat = new MSatXml();
                var objPath = new PathsCertificado();

                #endregion

                #region GET DATOS EMISOR Y RECEPTOR 

                if (datosNomina.IdEmpresa == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = " Id de la empresa es null", Msg2 = "" });
                    return null; //Si en datos de la nomina no tiene Id de Empresa Emisora - se suspende la creacion del cfdi
                }

                var emisorDatos = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == datosNomina.IdEmpresa); //empresaObj.GetEmpresaById((int)datosNomina.IdEmpresa);//v22
                if (emisorDatos == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = (int)datosNomina.IdEmpresa, Msg1 = "No se encontro datos del emisor.", Msg2 = "" });
                    return null; //si no se encontró datos de la empresa emisora del cfdi - se suspende la creacion del cfdi
                }

                //Datos del colaborador
                var datosReceptor = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == datosNomina.IdEmpleado); //_dao.GetDatosReceptor(datosNomina.IdEmpleado); v22

                if (datosReceptor == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se encontro datos del receptor.", Msg2 = "" });
                    return null; // si no se encontró datos del empleado - - se suspende la creacion del cfdi
                }

                if (datosReceptor.RFCValidadoSAT != 1)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "El RFC del colaborador no esta validado.", Msg2 = "" });
                    //  return summaryList; //si el rfc no esta validado por el sat
                }

                #endregion

                #region GET  -  PERCEPCIONES  -   DEDUCCIONES  -   OTROS PAGOS  - DATOS INDEMNIZACION - INCAPACIDADES

                // _dao.GetPercepcionesByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetPercepcionesByIdNomina(datosNomina.IdNomina);//v22
                var listaPercepcionesDeLaNomina = isFiniquito
                    ? listaPercepciones.Where(x => x.IdFiniquito == datosNomina.IdFiniquito).ToList()
                    : listaPercepciones.Where(x => x.IdNomina == datosNomina.IdNomina).ToList();


                //var listaDeduccionesDeLaNomina = isFiniquito ? _dao.GetDeduccionesByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetDeduccionesByIdNomina(datosNomina.IdNomina);//v22
                var listaDeduccionesDeLaNomina = isFiniquito
                    ? listaDedudcciones.Where(x => x.IdFiniquito == datosNomina.IdFiniquito).ToList()
                    : listaDedudcciones.Where(x => x.IdNomina == datosNomina.IdNomina).ToList();

                //var listaOtrosPagosDeLaNomina = isFiniquito ? _dao.GetOtrosPagosByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetOtrosPagosByIdNomina(datosNomina.IdNomina);//v22
                var listaOtrosPagosDeLaNomina = isFiniquito
                    ? listaOtrosPagos.Where(x => x.IdFiniquito == datosNomina.IdFiniquito).ToList()
                    : listaOtrosPagos.Where(x => x.IdNomina == datosNomina.IdNomina).ToList();

                //   List<NOM_Incapacidad> listaIncapacidades = isFiniquito ? _dao.GetDatosIncapacidadFiniquitos(datosNomina.IdFiniquito) : _dao.GetDatosIncapacidad(datosNomina.IdNomina);

                List<NOM_Incapacidad> listaIncapacidades = isFiniquito
                    ? listaIncapacidadesGeneral.Where(x => x.IdFiniquito == datosNomina.IdFiniquito).ToList()
                    : listaIncapacidadesGeneral.Where(x => x.IdNomina == datosNomina.IdNomina).ToList();


                var itemIndemnizacion = isFiniquito ? _dao.GetDatosIndemnizacionByIdFiniquito(datosNomina.IdFiniquito) : null;//pendiente

                if (listaPercepcionesDeLaNomina.Count <= 0 && listaDeduccionesDeLaNomina.Count <= 0 && listaOtrosPagosDeLaNomina.Count <= 0)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se encontrarón datos para generar el cfdi", Msg2 = "" });
                    return null; // se suspende la creacion del cfdi de este registro de nomina
                }
                #endregion

                #region SET TOTALES COMPLEMENTO Y NOMINA


                #endregion

                #region COMPROBANTE 3.3

                var subtotal = listaPercepcionesDeLaNomina.Sum(x => x.Importe);
                decimal descuento = listaDeduccionesDeLaNomina.Sum(x => x.Importe);
                var otrosPagos = listaOtrosPagosDeLaNomina.Sum(x => x.Importe);
                var impuesto = listaDeduccionesDeLaNomina.Where(x => x.IdConcepto == 43).Sum(x => x.Importe);

                subtotal = (subtotal + otrosPagos);

                var total = (subtotal - descuento);




                Modelos.Cfdi33.Comprobante comprobante33 = new Modelos.Cfdi33.Comprobante();
                DateTime now = Convert.ToDateTime(DateTime.Now.ToString("s"));

                comprobante33.Serie = "A";//<-- no quitar este texto ni cambiarlo por otro
                //comprobante33.Folio = "00";//<-- no quitar este texto ni cambiarlo por otro
                comprobante33.Folio = isFiniquito ? datosNomina.IdFiniquito.ToString() : datosNomina.IdNomina.ToString();
                comprobante33.Fecha = now;
                comprobante33.Sello = "sello"; //<-- no quitar este texto ni cambiarlo por otro
                comprobante33.FormaPago = Modelos.Cfdi33.c_FormaPago.Item99;// 99 por definir  
                comprobante33.FormaPagoSpecified = true;

                //comprobante33.CondicionesDePago = "3 Meses";//Segun la guía este campo no debe de existir
                comprobante33.SubTotal = (decimal)0.00;
                //comprobante33.Descuento = (decimal)0.00;//debe ser <= que el campo subtotal
                comprobante33.Moneda = Modelos.Cfdi33.c_Moneda.MXN;
                //comprobante33.TipoCambio = //Se debe registrar si Modena es diferente de MXN
                comprobante33.Total = (decimal)0.00;
                comprobante33.TipoDeComprobante = Modelos.Cfdi33.c_TipoDeComprobante.N;//Tipo Nomina
                comprobante33.MetodoPago = Modelos.Cfdi33.c_MetodoPago.PUE; //Pago en una sola excibicion
                comprobante33.MetodoPagoSpecified = true;


                comprobante33.LugarExpedicion = emisorDatos.CP; //codigo postal del cat del sat  [0-9]{5}
                comprobante33.schemaLocation = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd ";
                //comprobante33.Confirmacion = "";

                comprobante33.SubTotal = subtotal;
                comprobante33.Total = total;
                if (descuento > 0)
                {
                    comprobante33.Descuento = descuento;
                    comprobante33.DescuentoSpecified = true;
                }



                //Si el metodo es ejecutado desde el modulo de procesar nomina o procesar finiquito, 
                //no se requiere certificados ni el archivo de cadena original
                if (!fromModuloProcesar)
                {

                    CertificadoDigital certificado = MCfdi.GetCertificadoDigital_v2(archivoCer);

                    comprobante33.Certificado = certificado.CertificadoBase64;
                    comprobante33.NoCertificado = certificado.NoCertificado;

                }

                #endregion

                #region RELACIONADOS 3.3

                //Modelos.Cfdi33.ComprobanteCfdiRelacionadosCfdiRelacionado[] arrayRelacionados = new Modelos.Cfdi33.ComprobanteCfdiRelacionadosCfdiRelacionado[1];

                //Modelos.Cfdi33.ComprobanteCfdiRelacionadosCfdiRelacionado relacionado =
                //    new Modelos.Cfdi33.ComprobanteCfdiRelacionadosCfdiRelacionado()
                //    {
                //        UUID = "5FB2822E-396D-4725-8521-CDC4BDD20CCF"
                //    };

                //arrayRelacionados[0] = relacionado;


                //Modelos.Cfdi33.ComprobanteCfdiRelacionados relacionados = new ComprobanteCfdiRelacionados();
                //relacionados.TipoRelacion = c_TipoRelacion.Item01;//Nota de Crédito de los documentos relacionados
                //relacionados.CfdiRelacionado = arrayRelacionados;

                #endregion

                #region EMISOR 3.3

                //Obtener el regimen que tiene asignado la empresa basado en su ID de REgimen
                var regimen = Modelos.Cfdi33.c_RegimenFiscal.Item605;

                switch (emisorDatos.RegimenFiscal)
                {
                    case 1:
                        regimen = Modelos.Cfdi33.c_RegimenFiscal.Item601;
                        break;
                    case 2:
                        regimen = Modelos.Cfdi33.c_RegimenFiscal.Item603;
                        break;
                    case 3:
                        regimen = Modelos.Cfdi33.c_RegimenFiscal.Item605;
                        break;
                    case 4:
                        regimen = Modelos.Cfdi33.c_RegimenFiscal.Item606;
                        break;
                    case 5:
                        regimen = Modelos.Cfdi33.c_RegimenFiscal.Item608;
                        break;
                }


                Modelos.Cfdi33.ComprobanteEmisor comprobanteEmisor33 = new Modelos.Cfdi33.ComprobanteEmisor()
                {
                    Rfc = emisorDatos.RFC,//Persona moral 12 digitos
                    Nombre = emisorDatos.RazonSocial,
                    RegimenFiscal = regimen

                };

                comprobante33.Emisor = comprobanteEmisor33;
                #endregion

                #region RECEPTOR 3.3

                Modelos.Cfdi33.ComprobanteReceptor comprobanteReceptor33 = new Modelos.Cfdi33.ComprobanteReceptor()
                {
                    //ABC 4 - 3.3
                    Rfc = datosReceptor.RFC,
                    //Rfc = "AAQM610917QJA",
                    Nombre = ($"{datosReceptor.APaterno} {datosReceptor.AMaterno} {datosReceptor.Nombres}"),
                    UsoCFDI = c_UsoCFDI.P01 //Por definir
                };

                comprobante33.Receptor = comprobanteReceptor33;
                #endregion

                #region CONCEPTO DE PAGO 3.3 -  Pago de nomina - Unidad ATC -

                Modelos.Cfdi33.ComprobanteConcepto[] arrayComprobanteConcepto33 = new Modelos.Cfdi33.ComprobanteConcepto[1];
                Modelos.Cfdi33.ComprobanteConcepto comprobanteConcepto33 = new Modelos.Cfdi33.ComprobanteConcepto()
                {
                    ClaveProdServ = "84111505",//c_ClaveProdServ - Servicios de contabilidad de sueldos y salarios
                    //  NoIdentificacion = "UT421510",
                    ClaveUnidad = "ACT", //ACT -     Actividad  -  Unidad de recuento para definir el número de actividades(actividad: una unidad de trabajo o acción).
                    //  Unidad = "Kilo",
                    Descripcion = "Pago de nómina",
                    Cantidad = 1,//HASTA seis decimales
                    ValorUnitario = subtotal,
                    Importe = subtotal,
                    Descuento = descuento,//Puede ser <= al Importe
                    DescuentoSpecified = descuento > 0 ? true : false // este lo comente porque en la validacion del pac pedia que venga el campo aunque sea cero

                };

                arrayComprobanteConcepto33[0] = comprobanteConcepto33;

                comprobante33.Conceptos = arrayComprobanteConcepto33;


                #endregion

                #region IMPUESTOS 3.3 - no aplica para nominas

                //Modelos.Cfdi33.ComprobanteImpuestosRetencion comprobanteImpuestoRetencion33 = new Modelos.Cfdi33.ComprobanteImpuestosRetencion()
                //{
                //    Importe = 0,
                //    Impuesto = Modelos.Cfdi33.c_Impuesto.Item001 //Item001 - ISR
                //};

                //Modelos.Cfdi33.ComprobanteImpuestosRetencion[] arrayComprobanteImpuestosRetencion = new Modelos.Cfdi33.ComprobanteImpuestosRetencion[1];
                //arrayComprobanteImpuestosRetencion[0] = comprobanteImpuestoRetencion33;

                //Modelos.Cfdi33.ComprobanteImpuestos comprobanteImpuesto33 = new ComprobanteImpuestos()
                //{
                //    TotalImpuestosRetenidos = 0,
                //    Retenciones = arrayComprobanteImpuestosRetencion,
                //    TotalImpuestosRetenidosSpecified = true,
                //    TotalImpuestosTrasladados = 0,
                //    TotalImpuestosTrasladadosSpecified = false
                //};
                //comprobante33.Impuestos = comprobanteImpuesto33;


                comprobante33.Impuestos = null;
                #endregion

                #region RETENCIONES



                #endregion


                #region COMPLEMENTO DE NOMINA 1.2 en 3.3

                var nomina12 = ComplementoDeNomina12_v2(datosNomina, periodoPago, emisorDatos, datosReceptor, listaPercepcionesDeLaNomina, listaDeduccionesDeLaNomina, listaOtrosPagosDeLaNomina, itemIndemnizacion, itemCliente, listaIncapacidades);



                #endregion

                #region GENERAR EL TAG NOMINA Y SE ANEXA AL COMPLEMENTO DEL COMPROBANTE 3.3

                //Genera los tag xml de la nomina 1.2 -  que se anexará al tag de complemento
                var elementResult = MSatXml.GetXmlElement(nomina12);
                XmlElement[] arrayXmlElements = new XmlElement[1];

                arrayXmlElements[0] = elementResult;

                //Genera Tag complemento de nomina donde agregamos los tag xml de la nomina 1.2
                Modelos.Cfdi33.ComprobanteComplemento comprobanteComplemento = new Modelos.Cfdi33.ComprobanteComplemento()
                {
                    Any = arrayXmlElements
                };

                Modelos.Cfdi33.ComprobanteComplemento[] arrayComplementos = new Modelos.Cfdi33.ComprobanteComplemento[1];
                arrayComplementos[0] = comprobanteComplemento;

                //Agrega el tag complemento al xml 3.3
                comprobante33.Complemento = arrayComplementos;

                #endregion

                #region SELLO Y CADENA ORIGINAL

                //Generar el xml en string
                var xml33SinSello = MSatXml.GenerarXmlComprobante33(comprobante33);

                string cadenaOriginal33 = null;// en esta variable guardamos la cadena original que se generará juntamente con el sello

                //Si el metodo es ejecutado desde el modulo de procesar nomina o procesar finiquito no se requiere generar el sello
                if (!fromModuloProcesar)
                {
                    //Se crea el sello 33 y se anexa al comprobante en el tag de sello
                    //string sello33 = xmlsat.GenerarSello33(xml33SinSello, pathCadenaOriginalXslt, objPath, ref cadenaOriginal33);

                    string sello33 = xmlsat.GenerarSello33_v2(xml33SinSello, transformador, archivoCer, archivoKey, password, ref cadenaOriginal33);

                    comprobante33.Sello = sello33;
                }

                #endregion

                //Finalmente generamos la estrucutura del xml.
                var xml33Sintimbre = MSatXml.GenerarXmlComprobante33(comprobante33); // ¡Este xml aun no esta timbrado! 

                //Guardamos el xml en la base de datos - para ser timbrado posteriormente
                //GuardarXmLenBd(datosNomina, xml33Sintimbre, cadenaOriginal33, idUsuario, isFiniquito, fromModuloProcesar, versionCfdi: 33);


                NOM_CFDI_Timbrado registro = new NOM_CFDI_Timbrado
                {
                    IdTimbrado = 0,
                    Version = "3.3",
                    Serie = "A",
                    Folio = 0,
                    IdNomina = datosNomina.IdNomina,
                    IdFiniquito = datosNomina.IdFiniquito,
                    IdSucursal = datosNomina.IdSucursal,
                    IdEmisor = datosNomina.IdEmisor,
                    IdReceptor = datosNomina.IdEmpleado,
                    RFCReceptor = datosNomina.Rfc,
                    Cancelado = false,
                    XMLSinTimbre = xml33Sintimbre,
                    TotalRecibo = datosNomina.TotalNomina,
                    CadenaOriginal = cadenaOriginal33,
                    UsuarioTimbro = idUsuario,
                    IdEjercicio = datosNomina.IdEjercicio,
                    IdPeriodo = datosNomina.IdPeriodo,
                    RFCEmisor = datosNomina.RfcEmisor,
                    FechaReg = DateTime.Now

                };

                return registro;
            }
            catch (Exception ex)
            {
                summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = ex.Message, Msg2 = "" });

                return null;


            }

            return null;
        }

        private Modelos.Nomina12.Nomina ComplementoDeNomina12_v2(NominaData datosNomina, NOM_PeriodosPago periodoPago, Empresa emisorDatos, Empleado datosReceptor, List<Datos.ConceptosNomina> listaPercepcionesDeLaNomina, List<Datos.ConceptosNomina> listaDeduccionesDeLaNomina, List<Datos.ConceptosNomina> listaOtrosPagosDeLaNomina, IndemnizacionData itemIndemnizacion, Cliente itemCliente, List<NOM_Incapacidad> listaIncapacidades)
        {

            #region COMPLEMENTO DE NOMINA 1.2

            var fechaFinalPago = DateTime.Parse(datosNomina.FechaFinaldePago);
            Modelos.Nomina12.Nomina nomina12 = new Modelos.Nomina12.Nomina();
            nomina12.Version = "1.2";
            nomina12.TipoNomina = datosNomina.TipoNominaSat; // pagos ordinarios diario a mensual, Extraordinario : aguinaldo, finiquito, etc.
            if (periodoPago.IdTipoNomina == 16)
            {
                nomina12.TipoNomina = "E";
            }

            nomina12.FechaPago = DateTime.Parse(datosNomina.FechaFinaldePago);
            nomina12.FechaInicialPago = DateTime.Parse(datosNomina.FechaInicialdePago);
            nomina12.FechaFinalPago = fechaFinalPago; //DateTime.Parse(datosNomina.FechaFinaldePago);
            nomina12.NumDiasPagados = datosNomina.DiasPagados;
            nomina12.schemaLocation = "http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/informacion_fiscal/factura_electronica/Documents/Complementoscfdi/nomina12.xsd";

            #region EMISOR - RECEPTOR 1.2 3.3

            // EMISOR - Metodo por separado
            var nominaEmisor = new Modelos.Nomina12.NominaEmisor();

            if (periodoPago.IdTipoNomina != 16) //sino es Asimilados
            {
                nominaEmisor.RegistroPatronal = emisorDatos.RegistroPatronal;// "Registro patronal" NO APLICA CUANDO ES SIMILADO A SALARIOS
                nominaEmisor.RfcPatronOrigen = emisorDatos.RFC;
                //ABC 5    -     3.3 1.2 - solo
                //nominaEmisor.Curp = "CACF880922HJCMSR03"; // Curp = Personas morales no tienen este dato - Comentar esta linea - solo se usa para pruebas con GOYA780416GM0
            }


            //Fecha inicio laboral = fecha Alta
            var fechaInicialLaboral = DateTime.Parse(datosNomina.FechaAlta.ToString("yyyy-MM-dd"));

            // RECEPTOR
            var nominaReceptor = new Modelos.Nomina12.NominaReceptor();

            nominaReceptor.Curp = datosReceptor.CURP;

            if (periodoPago.IdTipoNomina != 16) //no aplica para Asimilados
            {
                nominaReceptor.NumSeguridadSocial = datosReceptor.NSS;//1 a 15 caracteres, no aplica para asimilados a salarios
                nominaReceptor.FechaInicioRelLaboral = fechaInicialLaboral; // no aplica para asimilados a salarios
                nominaReceptor.FechaInicioRelLaboralSpecified = true;
            }

            if (periodoPago.IdTipoNomina != 16) //no aplica para Asimilados
            {
                nominaReceptor.Antigüedad = fechaInicialLaboral.FormatoAntiguedadSat(fechaFinalPago);
                //GeneraFormatoAntiguedad(fechaInicialLaboral),//FORMATO ESPECIAL P10Y8M15D //no aplica para asimilados a salarios
            }

            nominaReceptor.TipoContrato = datosNomina.TipoContrato.Trim(); // cat sat c_TipoContrato

            if (periodoPago.IdTipoNomina == 16)//ASIMILADOS
            {
                nominaReceptor.TipoContrato = "09";//Contratación donde no existe relación de trabajo
            }

            nominaReceptor.Sindicalizado = Modelos.Nomina12.NominaReceptorSindicalizado.No;//no aplica para asimilados a salarios

            if (periodoPago.IdTipoNomina != 16) //no aplica para Asimilados
            {
                nominaReceptor.TipoJornada = datosNomina.TipoJornada.Trim();//cat sat c_TipoJornada //No aplica para asimilados a salarios
            }

            nominaReceptor.TipoRegimen = datosNomina.TipoRegimen.Trim();// cat sat c_TipoRegimen // revisar regla de la guía de llenado de comprobante

            if (periodoPago.IdTipoNomina == 16)//ASIMILADOS
            {
                nominaReceptor.TipoRegimen = "09";// Asimilados Honorarios
            }

            nominaReceptor.NumEmpleado = datosNomina.IdEmpleado.ToString();


            if (periodoPago.IdTipoNomina != 16) //no aplica para Asimilados
            {
                nominaReceptor.Departamento = datosNomina.Departamento;
                nominaReceptor.Puesto = datosNomina.PuestoRecibo;
                nominaReceptor.RiesgoPuesto = datosNomina.RiesgoPuesto.ToString();
                // conforme al cat sat c_RiesgoPuesto
            }

            nominaReceptor.PeriodicidadPago = datosNomina.PeriodicidadPago.Trim(); //conforme al cat sat c_PeriodicidadPago

            if (periodoPago.IdTipoNomina != 16)//no aplica para Asimilados
            {
                if (datosNomina.PagoEnEfectivo == false)//33
                {
                    //si la longitud es de 18digitos no debe ir el campo Banco, pero si es de 16 num de tarjeta o 10 telefono celular el campo banco es obligatorio
                    if (datosNomina.NumeroDeCuenta.Length == 18)
                    {
                        nominaReceptor.CuentaBancaria = datosNomina.NumeroDeCuenta;
                        nominaReceptor.CuentaBancariaSpecified = true;
                    }
                    else
                    {
                        nominaReceptor.CuentaBancaria = datosNomina.NumeroDeCuenta;
                        nominaReceptor.CuentaBancariaSpecified = true;
                        nominaReceptor.Banco = datosNomina.CveBanco; //confome al cat sat c_Banco   
                    }
                }
                else
                {
                    nominaReceptor.CuentaBancariaSpecified = false;
                }
            }


            if (periodoPago.IdTipoNomina != 16) //no aplica para Asimilados
            {
                nominaReceptor.SalarioBaseCotApor = datosNomina.Sbc.TruncateDecimal(2);
                //No aplica para asimilados a salarios
                nominaReceptor.SalarioBaseCotAporSpecified = true;
                nominaReceptor.SalarioDiarioIntegrado = datosNomina.Sdi.TruncateDecimal(2);
                //no aplica para asimilados a salarios
                nominaReceptor.SalarioDiarioIntegradoSpecified = true;
            }

            nominaReceptor.ClaveEntFed = datosNomina.ClaveEntidadFederativa; //cat sat c_estado

            #endregion

            #region SUBCONTRATACION

            if (itemCliente.NodoSubcontratacion)
            {
                Modelos.Nomina12.NominaReceptorSubContratacion itemSubContratacion = new NominaReceptorSubContratacion();
                itemSubContratacion.RfcLabora = itemCliente.Rfc;


                itemSubContratacion.PorcentajeTiempo = datosNomina.PorcentajeTiempo;

                NominaReceptorSubContratacion[] arraySubcontratacion = new NominaReceptorSubContratacion[1];

                arraySubcontratacion[0] = itemSubContratacion;

                nominaReceptor.SubContratacion = arraySubcontratacion;

            }

            #endregion

            #region PERCEPCIONES 1.2

            Modelos.Nomina12.NominaPercepcionesPercepcion[] arrayPercepciones = new Modelos.Nomina12.NominaPercepcionesPercepcion[listaPercepcionesDeLaNomina.Count()];

            decimal totalGravadoPercepcion = 0;
            decimal totalExcentoPercepcion = 0;
            decimal totalSueldo = 0; // 
            int idxPercepcion = 0;
            string strConcepto = "";
            foreach (var p in listaPercepcionesDeLaNomina)
            {
                strConcepto = p.NombreConcepto;

                #region ARRAY - ITEM PERCEPCIONES

                //HORAS EXTRAS
                NominaPercepcionesPercepcionHorasExtra[] agregarHorasExtras = new NominaPercepcionesPercepcionHorasExtra[1];
                NominaPercepcionesPercepcionHorasExtra itemHoraExtras = new NominaPercepcionesPercepcionHorasExtra();
                if (p.IdConcepto == 14 && p.ClaveSat == "019")
                {
                    itemHoraExtras.Dias = p.DiasHe.Value;
                    itemHoraExtras.HorasExtra = p.HorasExtrasAplicadas.Value;
                    itemHoraExtras.ImportePagado = p.Importe;
                    itemHoraExtras.TipoHoras = p.TipoHe.ToString().PadLeft(2, '0');
                    agregarHorasExtras[0] = itemHoraExtras;

                    if (p.TipoHe == 1)
                    {
                        strConcepto = "Horas extra d";
                    }
                    else if (p.TipoHe == 2)
                    {
                        strConcepto = "Horas extra t";
                    }
                    else if (p.TipoHe == 3)
                    {

                    }


                }



                Modelos.Nomina12.NominaPercepcionesPercepcion itemP = new Modelos.Nomina12.NominaPercepcionesPercepcion
                {
                    TipoPercepcion = p.ClaveSat.PadLeft(3, '0'),
                    Clave = p.IdConcepto.ToString().PadLeft(3, '0'),
                    Concepto = strConcepto, //p.NombreConcepto,
                    ImporteGravado = p.Gravado.TruncateDecimal(2),
                    ImporteExento = p.Excento.TruncateDecimal(2),
                    HorasExtra = p.IdConcepto == 14 ? agregarHorasExtras : null
                    //Horas extras
                    //Acciones O Titulos
                };

                // Total de percepciones brutas (gravadas y exentas) exepto conceptos con las claves: 
                //022 prima antiguedad //023 pagos por separacion //025 Indemnizaciones //039 Jubilaciones, pensiones o haberes de retiro 
                //044 Jubilaciones, pensiones o haberes de retiro en parcialidades
                if (p.ClaveSat == "022" || p.ClaveSat == "023" || p.ClaveSat == "025" || p.ClaveSat == "039" ||
                    p.ClaveSat == "044")
                {
                    totalGravadoPercepcion += p.Gravado.TruncateDecimal(2);
                    totalExcentoPercepcion += p.Excento.TruncateDecimal(2);
                }
                else
                {
                    //Nomina.Percepciones.TotalSueldos = ImporteGravado + ImporteExento 
                    //donde la clave expresada en el atributo TipoPercepcion es distinta de 022 Prima por Antigüedad, 
                    //023 Pagos por separación, 025 Indemnizaciones, 039 Jubilaciones, pensiones o haberes de retiro en una exhibición y
                    //044 Jubilaciones, pensiones o haberes de retiro en parcialidades
                    totalSueldo += p.Importe.TruncateDecimal(2);
                    totalGravadoPercepcion += p.Gravado.TruncateDecimal(2);
                    totalExcentoPercepcion += p.Excento.TruncateDecimal(2);
                }

                arrayPercepciones[idxPercepcion] = itemP;
                idxPercepcion++;

                #endregion

            }

            #region TAG JUBILACION E INDEMNIZACION

            Modelos.Nomina12.NominaPercepcionesJubilacionPensionRetiro jubilacionPensionRetiro = new Modelos.Nomina12.NominaPercepcionesJubilacionPensionRetiro
                ()
            {
                TotalUnaExhibicion = 0
            };

            Modelos.Nomina12.NominaPercepcionesSeparacionIndemnizacion separacionIndemnizacion = new Modelos.Nomina12.NominaPercepcionesSeparacionIndemnizacion
                ()
            {
                TotalPagado = itemIndemnizacion?.Total.TruncateDecimal(2) ?? 0, //total indemnizacion
                NumAñosServicio = itemIndemnizacion?.AniosServicios ?? 0,
                //años de servicio 10años + 6meses = 11 años, 6 meses se considera completo
                UltimoSueldoMensOrd = itemIndemnizacion?.UltimoSueldoMensOrd.TruncateDecimal(2) ?? 0,
                IngresoAcumulable = itemIndemnizacion?.IngresoAcumulable.TruncateDecimal(2) ?? 0,
                IngresoNoAcumulable = itemIndemnizacion?.IngresoNoAcumulable.TruncateDecimal(2) ?? 0
            };

            #endregion

            //NODO Principal de Percepciones
            var nominaPercepcion = new Modelos.Nomina12.NominaPercepciones();

            nominaPercepcion.Percepcion = arrayPercepciones;
            nominaPercepcion.TotalExento = totalExcentoPercepcion.TruncateDecimal(2);
            nominaPercepcion.TotalGravado = totalGravadoPercepcion.TruncateDecimal(2);
            nominaPercepcion.TotalSueldos = totalSueldo.TruncateDecimal(2);// Total de percepciones brutas (gravadas y exentas) exepto conceptos con las  claves:  022, 023, 025, 039, 044
            nominaPercepcion.TotalSueldosSpecified = true;
            nominaPercepcion.TotalSeparacionIndemnizacion = 0; //Suma del excento y gravado de los conceptos 022, 023, 025, solo debe aparecer si se registro alguno de los conceptos anteriores
            nominaPercepcion.TotalJubilacionPensionRetiro = 0; //Suma del importe gravado y excento de los conceptos 039, 044, solo debe aparecer si se registro alguno de los conceptos anteriores

            if (itemIndemnizacion != null)
            {
                nominaPercepcion.SeparacionIndemnizacion = separacionIndemnizacion;
                nominaPercepcion.TotalSeparacionIndemnizacion = separacionIndemnizacion.TotalPagado;
                nominaPercepcion.TotalSeparacionIndemnizacionSpecified = true;
            }

            //JubilacionPensionRetiro = jubilacionPensionRetiro

            #endregion

            #region OTROS PAGOS 1.2
            //SE APLICARA OTROS PAGOS SI EN LA NOMINA SE GENERO SUBSIDIO AL EMPLADO O SE TIENE SALDO A FAVOR EN SU DECLARACION ANUAL
            //Para el caso de Alianza solo se esta generando Subsidio en las nominas

            //Array de otros pagos
            List<Modelos.Nomina12.NominaOtroPago> arrayOtrosPagos = new List<Modelos.Nomina12.NominaOtroPago>();

            foreach (var itemOtroPago in listaOtrosPagosDeLaNomina)
            {
                switch (itemOtroPago.IdTipoOtroPago)
                {
                    case 1://Devolucion ISR
                        Modelos.Nomina12.NominaOtroPago otroPagoDevolucionIsr = new Modelos.Nomina12.NominaOtroPago()
                        {
                            TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //Clave = itemOtroPago.ClaveContable,
                            Clave = itemOtroPago.IdConcepto.ToString().PadLeft(3, '0'),
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe,
                        };
                        arrayOtrosPagos.Add(otroPagoDevolucionIsr);
                        break;
                    case 2://Subsidio Efectivamente Entregado al colaborador
                        Modelos.Nomina12.NominaOtroPagoSubsidioAlEmpleo subsidioAlEmpleo = new Modelos.Nomina12.NominaOtroPagoSubsidioAlEmpleo()
                        {
                            SubsidioCausado = datosNomina.SubsidioCausado //Subsidio Causado
                        };

                        Modelos.Nomina12.NominaOtroPago otroPago = new Modelos.Nomina12.NominaOtroPago()
                        {
                            TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //Clave = itemOtroPago.ClaveContable,
                            Clave = itemOtroPago.IdConcepto.ToString().PadLeft(3, '0'),
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe,
                            SubsidioAlEmpleo = subsidioAlEmpleo
                        };
                        arrayOtrosPagos.Add(otroPago);
                        break;
                    case 3: //Viaticos

                        Modelos.Nomina12.NominaOtroPago otrosViaticos = new Modelos.Nomina12.NominaOtroPago()
                        {
                            TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //Clave = itemOtroPago.ClaveContable,
                            Clave = itemOtroPago.IdConcepto.ToString().PadLeft(3, '0'),
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe
                        };

                        arrayOtrosPagos.Add(otrosViaticos);

                        break;
                    case 4://Saldo a Favor
                        var año = (short)DateTime.Now.Year;
                        año = (short)periodoPago.Fecha_Fin.Year;//Fecha final del periodo, ya que esta fecha se concidera la fecha de pago
                        Modelos.Nomina12.NominaOtroPagoCompensacionSaldosAFavor otroPagoSaldoFavor = new Modelos.Nomina12.NominaOtroPagoCompensacionSaldosAFavor()
                        {
                            SaldoAFavor = itemOtroPago.Importe,
                            Año = año,
                            RemanenteSalFav = itemOtroPago.Importe
                        };

                        Modelos.Nomina12.NominaOtroPago otroPagoSv = new Modelos.Nomina12.NominaOtroPago()
                        {
                            TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //Clave = itemOtroPago.ClaveContable,
                            Clave = itemOtroPago.IdConcepto.ToString().PadLeft(3, '0'),
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe,
                            CompensacionSaldosAFavor = otroPagoSaldoFavor
                        };
                        arrayOtrosPagos.Add(otroPagoSv);

                        break;
                    case 5: //999 otro pagos que no deben considerarse como ingreso por sueldo, salarios o ingresos asimilados
                        Modelos.Nomina12.NominaOtroPago otrosPago = new Modelos.Nomina12.NominaOtroPago()
                        {
                            TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //Clave = itemOtroPago.ClaveContable,
                            Clave = itemOtroPago.IdConcepto.ToString().PadLeft(3, '0'),
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe
                        };

                        arrayOtrosPagos.Add(otrosPago);

                        break;
                    case 6:// Reintegro de ISR retenido en exceso de ejercicio anterior (siempre que no haya sido enterado al SAT)
                        Modelos.Nomina12.NominaOtroPago otroPagoReintegroIsr = new Modelos.Nomina12.NominaOtroPago()
                        {
                            TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //Clave = itemOtroPago.ClaveContable,
                            Clave = itemOtroPago.IdConcepto.ToString().PadLeft(3, '0'),
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe,
                        };
                        arrayOtrosPagos.Add(otroPagoReintegroIsr);
                        break;

                }
            }

            //Si existen registro como otros pagos 
            if (arrayOtrosPagos.Count > 0)
            {
                var totalOtrosPagos = arrayOtrosPagos.Sum(x => x.Importe);
                //Se agrega el nodo de Otros pagos
                nomina12.OtrosPagos = arrayOtrosPagos.ToArray();
                //Se actualizan los valores de los atributos del tag de nomina
                nomina12.TotalOtrosPagosSpecified = true;
                nomina12.TotalOtrosPagos = totalOtrosPagos;
                //Se suma el total de otros pagos a el total de Percepciones
                //Pero actualmente en la tabla nomina se esta guardando el total de ambas cosas
            }

            #endregion

            #region DEDUCCIONES 1.2

            Modelos.Nomina12.NominaDeduccionesDeduccion[] arrayD = new Modelos.Nomina12.NominaDeduccionesDeduccion[listaDeduccionesDeLaNomina.Count()];
            int idxDeduccion = 0;
            decimal totalOtrasDeducciones = 0;
            decimal totalImpuestosRetenidos = 0;

            foreach (var d in listaDeduccionesDeLaNomina)
            {
                Modelos.Nomina12.NominaDeduccionesDeduccion itemD = new Modelos.Nomina12.NominaDeduccionesDeduccion()
                {
                    TipoDeduccion = d.ClaveSat.PadLeft(3, '0'),
                    Clave = d.IdConcepto.ToString().PadLeft(3, '0'),
                    Concepto = d.NombreConcepto,
                    Importe = d.Importe.TruncateDecimal(2)
                };


                if (itemD.TipoDeduccion != "002") //
                {
                    totalOtrasDeducciones += d.Importe;
                }
                else //Suma a todos los que tienen Tipo deduccion  002 // ISR
                {
                    if (d.IsLiquidacion)
                    {
                        itemD.Concepto = "ISR Liq";
                    }

                    totalImpuestosRetenidos += d.Importe;
                }

                arrayD[idxDeduccion] = itemD;
                idxDeduccion++;
            }

            var nominaDeduccion = new Modelos.Nomina12.NominaDeducciones();

            nominaDeduccion.Deduccion = arrayD;
            nominaDeduccion.TotalOtrasDeducciones = totalOtrasDeducciones.TruncateDecimal(2);
            //total = Seguridad Social, aportaciones a retiro, cesantía en edad avanzada y vejez, aportaciones al fondo de vivienda, descuento por incapacidad, pension alimenticia, no incluye el concepto 002 (ISR) -
            nominaDeduccion.TotalOtrasDeduccionesSpecified = true;

            if (totalImpuestosRetenidos > 0)
            {
                nominaDeduccion.TotalImpuestosRetenidos = totalImpuestosRetenidos.TruncateDecimal(2);
                //suma del ISR , donde la clave sea de tipo concepto 002, si no existe no se deberá registrar
                nominaDeduccion.TotalImpuestosRetenidosSpecified = true;
            }

            //Actualiza el campo TotalDeduccioes de la nomina
            //Sumando el totalOtrasDeducciones + totalImpuestoRetenidos
            nomina12.TotalDeducciones = totalOtrasDeducciones.TruncateDecimal(2) + totalImpuestosRetenidos.TruncateDecimal(2);

            nomina12.TotalDeduccionesSpecified = nomina12.TotalDeducciones > 0; // este campo lo comente por que el pac pedia que se registre este tag aunque no haya deducciones

            #endregion

            #region INCAPACIDADES 1.2 en 3.3

            if (listaIncapacidades != null)
            {
                if (listaIncapacidades.Count > 0)
                {
                    Modelos.Nomina12.NominaIncapacidad[] arrayIncapacidades =
                        new Modelos.Nomina12.NominaIncapacidad[listaIncapacidades.Count];

                    int indx = 0;
                    foreach (var itemIncapacidad in listaIncapacidades)
                    {
                        Modelos.Nomina12.NominaIncapacidad incapacidad = new Modelos.Nomina12.NominaIncapacidad()
                        {
                            DiasIncapacidad = itemIncapacidad.DiasIncapacidad,
                            TipoIncapacidad = itemIncapacidad.IdTipoIncapacidad.ToString().PadLeft(2, '0'),
                            ImporteMonetario = itemIncapacidad.Importe
                        };

                        arrayIncapacidades[indx] = incapacidad;
                        indx++;
                    }

                    nomina12.Incapacidades = arrayIncapacidades;
                }

            }

            #endregion

            //--
            nomina12.Emisor = nominaEmisor;
            nomina12.Receptor = nominaReceptor;
            nomina12.Percepciones = nominaPercepcion;
            if (nomina12.TotalDeducciones > 0) nomina12.Deducciones = nominaDeduccion; //Agregar TAG Deducciones

            //nomina12.Incapacidades = arrayIncapacidades;
            #endregion

            //ACTUALIZAR LOS TOTALES DEL TAG COMPROBANTE - PARA QUE COICIDAN CON LOS TOTALES DE LA NOMINA12
            #region ACTUALIZA TOTAL DEL COMPROBANTE

            //comprobante32.descuento = nomina12.TotalDeducciones;

            var totalActualizadoRecibo = (nomina12.TotalPercepciones + nomina12.TotalOtrosPagos) - nomina12.TotalDeducciones;



            #endregion

            //SET TOTALES COMPROBANTE Y NOMINAS

            #region SET TOTALES COMPROBANTE Y TOTALES NOMINA

            //Variables TAG Comprobante
            decimal ctotal = 0;
            decimal csubtotal = 0;
            decimal cdescuento = 0;

            decimal ccimporte = 0;
            decimal ccvalorunitario = 0;

            //Variables TAG Nomina
            decimal ntotalpercepciones = 0;
            decimal ntotaldeducciones = 0;
            decimal ntotalotrospagos = 0;


            // Nomina.TotalPercepciones  = TotalSueldos + TotalSeparacionIndemnizacion + TotalJubilacionPensionRetiro.

            ntotalpercepciones = totalSueldo.TruncateDecimal(2) + (itemIndemnizacion?.Total.TruncateDecimal(2) ?? 0);// Total sueldo + total indemnizacion + total JubilacionPensionRetiro

            //nomina Total deducciones
            ntotaldeducciones = nomina12.TotalDeducciones; //datosNomina.TotalDeducciones.TruncateDecimal(2);// si no hay deducciones este campo no debera incluirse


            // total = totalPercepciones + totalOtros Pagos - Total deducciones
            //Se actualiza con las deduccuibes e impuesto retenidos
            ctotal = (ntotalpercepciones + datosNomina.TotalOtrosPagos.TruncateDecimal(2)) - ntotaldeducciones;//datosNomina.TotalDeducciones.TruncateDecimal(2);


            //nomina total otros pagos
            ntotalotrospagos = datosNomina.TotalOtrosPagos.TruncateDecimal(2);//<-- condicionado al total de listaOtrosPagosDeLaNomina. Suma de otros pagos - reintegro del ISR, subsidio para el empleo, viaticos, compensacion anual, otros

            // Nomina.Percepciones.TotalSueldos = (ImporteGravado + ImporteExento) 

            // SubTotal = Nomina12:TotalPercepciones+ Nomina12:TotalOtrosPagos
            csubtotal = ntotalpercepciones + ntotalotrospagos;

            // conceto importe = total Percepciones + total otros pagos
            ccimporte = ntotalpercepciones + ntotalotrospagos;

            // concepto valor Unitario = total Percepciones + total otros pagos
            ccvalorunitario = ntotalpercepciones + ntotalotrospagos;

            cdescuento = ntotaldeducciones;

            if (ntotaldeducciones > 0)
            {
                nomina12.TotalDeduccionesSpecified = true;
            }

            if (periodoPago.IdTipoNomina != 16) //Asimilados
            {
                nomina12.TotalOtrosPagos = ntotalotrospagos;
                nomina12.TotalOtrosPagosSpecified = true;//<-- condicionado si hay otros pagos en listaOtrosPagosDeLaNomina
            }




            // TOTALES NOMINA 1.2
            nomina12.TotalPercepciones = ntotalpercepciones;
            nomina12.TotalPercepcionesSpecified = true;



            #endregion



            return nomina12;

        }


        #endregion

        #region CANCELAR TIMBRES
        
        public Task<List<NotificationSummary>> CancelarTimbresAsync(int[] arrayIdTimbrado, int idUsuario)
        {
            var t = Task.Factory.StartNew(() =>
            {
                var r = CancelarTimbres(arrayIdTimbrado, idUsuario);
                return r;
            });

            return t;
        }


        private List<NotificationSummary> CancelarTimbres(int[] arrayIdTimbrado, int idUsuario)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            string remoteAddressWs = "";
            //GET Registos timbrados != Cancelados
            var listaTimbrados = _dao.GetCfdiTimbradosByArrayIdTimbre(arrayIdTimbrado);
            List<int> listaCancelados = new List<int>();

            //Valida que la lista no sea vacia o null
            if (listaTimbrados == null)
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "No se encontró datos para cancelar...",
                    Msg2 = ""
                });

                return summaryList;
            }
            if (listaTimbrados.Count <= 0)
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "No se encontró datos para cancelar...",
                    Msg2 = ""
                });
                return summaryList;
            }

            //Buscamos los datos de configuracion  //DATOS de [SYA_GlobalConfig]
            GlobalConfig gconfig = new GlobalConfig();
            var servidorConfig = gconfig.GetGlobalConfig();

            if (servidorConfig == null)
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "No se encontró datos de configuracion en GlobalConfig",
                    Msg2 = ""
                });
                return summaryList;
            }

            // obetener las claves de usuario para el timbrado
            var usuarioPac = "";
            var passwordPac = "";

            usuarioPac = servidorConfig.UsuarioPac;
            passwordPac = servidorConfig.PasswordPac;

            remoteAddressWs = TimbradoDePrueba ? servidorConfig.UrlPacTest : servidorConfig.UrlPac;

            if (remoteAddressWs.Trim() == "")
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = $"La url del web service esta vacía",
                    Msg2 = ""
                });
            }

            summaryList.Add(new NotificationSummary()
            {
                Reg = 0,
                Msg1 = $"Se usa el Usuario: {usuarioPac}",
                Msg2 = ""
            });
             

            //Establece conexion con el web Service del PAC
            //Actualizamos el Objeto SOAP con la url para el timbrado
            System.Net.ServicePointManager.Expect100Continue = false;
            var clienteSoap = new wsTimbradoSoapClient("wsTimbradoSoap", remoteAddressWs);
            //WEB SERVICE OPEN
            clienteSoap.Open();

            int idEmisor = 0;
            var pathArchivoPfx = "";
            var llavePrivadaEmisor = "";
            var rfcEmisor = "";
            byte[] pfxBytes = null;
            //FOR - Por cada registro se ejecutar el cancelado
            foreach (var itemTimbrado in listaTimbrados)
            {
                #region PROCESO DE CANCELACION CON EL PAC

                try
                {
                    //Obtener el dato de la empresa Emisora
                    //Obtener los datos del Emisor
                    //Si el emisor es el mismo que el anterior se conserva sus datos
                    //sino se validan los datos del nuevo emisor
                    if (idEmisor != itemTimbrado.IdEmisor)
                    {
                        var emisor = _dao.GetEmpresaById(itemTimbrado.IdEmisor);

                        #region VALIDACION DE LOS DATOS DEL EMISOR

                        if (emisor == null)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = itemTimbrado.IdEmisor,
                                Msg1 = $"No se encontró datos del emisor Id: {itemTimbrado.IdEmisor}",
                                Msg2 = ""
                            });

                            continue;
                        }

                        idEmisor = emisor.IdEmpresa;

                        if (emisor.LlavePrivada == null)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = idEmisor,
                                Msg1 = $"LlavePrivada es null de la empresa id: {emisor.IdEmpresa} rfc:{emisor.RFC}",
                                Msg2 = ""
                            });
                            continue;
                        }


                        llavePrivadaEmisor = emisor.LlavePrivada;
                        rfcEmisor = emisor.RFC;

                        pathArchivoPfx = servidorConfig.PathCertificados + "/" + emisor.IdEmpresa + "/" + emisor.ArchivoPFX;

                        //Validar que exista el archivo pfx
                        if (Utils.ExisteArchivo(pathArchivoPfx))
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = idEmisor,
                                Msg1 = $"Se usó el archivo {emisor.ArchivoPFX} para la empresa con Id:{emisor.IdEmpresa}. RFC Emisor:{emisor.RFC}",
                                Msg2 = ""
                            });

                            pfxBytes = Utils.GetBytesArchivo(pathArchivoPfx);
                        }
                        else
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = idEmisor,
                                Msg1 = $"No se encontró el archivo certificado <strong>.pfx</strong> del emisor. IdEmpresa:{emisor.IdEmpresa}.",
                                Msg2 = ""
                            });

                            continue;
                        }

                        #endregion
                    }

                    #region VALID MSG - NULL Or Empty

                    var strValidNull = "";

                    if (pfxBytes == null)
                    {
                        strValidNull = $"El pfxBytes es null. <br/>";
                    }

                    if (itemTimbrado.FolioFiscalUUID == null)
                    {
                        strValidNull += $"El folio fiscal es null. <br/>";
                    }

                    if (llavePrivadaEmisor == "")
                    {
                        strValidNull += $"La llave privada del emisor esta vacio. <br/>";
                    }

                    if (rfcEmisor == "")
                    {
                        strValidNull += $"El rfc del emisor esta vacio. <br/>";
                    }

                    if (strValidNull != "")
                    {
                        strValidNull += $" Rfc emisor: { itemTimbrado.RFCEmisor} Rfc Receptor : { itemTimbrado.RFCReceptor}. <br/>";

                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = idEmisor,
                            Msg1 = strValidNull,
                            Msg2 = string.Empty
                        });
                    }

                    if (pfxBytes == null || itemTimbrado.FolioFiscalUUID == null || llavePrivadaEmisor == "" || rfcEmisor == "")
                    {
                        continue;
                    }

                    #endregion

                    string[] uudi = new string[1];
                    uudi[0] = itemTimbrado.FolioFiscalUUID;

                    ArrayOfString uudiList = new ArrayOfString();
                    uudiList.AddRange(uudi);

                    //Se realiza la cancelacion
                    // Solicitud --> PAC --> Validacion --> Envia al SAT la Cancelacion
                    //Respuesta <-- PAC <-- SAT Response
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidarCertificado);
                    XElement respuestaXe = clienteSoap.EnviarCancelacionPFX(usuarioPac, passwordPac, uudiList, pfxBytes, llavePrivadaEmisor, rfcEmisor);


                    //Convertimos la respuesta en string 
                    string strRespuesta = respuestaXe.ToString();

                    if (strRespuesta.Contains("esValido=\"True\""))
                    {
                        //Si se realizo el cancelado se actualiza el objeto como cancelado
                        listaCancelados.Add(itemTimbrado.IdTimbrado);
                    }
                    else
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = itemTimbrado.IdTimbrado,
                            Msg1 = $"<strong>Rfc Emisor:</strong> {rfcEmisor}   <strong>Rfc Receptor:</strong> {itemTimbrado.RFCReceptor} <br/> <strong>Respuesta del PAC : </strong> ",
                            Msg2 = $"{strRespuesta}"
                        });
                    }

                }
                catch (Exception e)
                {
                    summaryList.Add(new NotificationSummary()
                    {
                        Reg = itemTimbrado.IdTimbrado,
                        Msg1 = $"? {e.Message}",
                        Msg2 = string.Empty
                    });
                }

                #endregion
            }

            //WEB SERIVE CLOSE
            clienteSoap.Close();


            //UPD - Actualizar su campo CFDI Creado = 0
            if (listaCancelados.Count <= 0) 
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "<strong>No se canceló ningun registro timbrado</strong>",
                    Msg2 = ""
                });

                return summaryList;
            }

            var arrayIdTimbrados = listaCancelados.ToArray();
            _dao.ActualizarCancelados(arrayIdTimbrados, idUsuario);

            return summaryList;
        }

        #endregion


        /// <summary>
        /// Elimina los cfdi que se registraron pero contienen error o el timbrado no se realizo correctamente
        /// </summary>
        /// <param name="idNominas"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public bool EliminarCfdiByNominaIds(int[] idNominas, int idEjercicio, NOM_PeriodosPago pp, int idSucursal)
        {
            try
            {
                if (pp.IdTipoNomina == 11)
                {
                    _dao.EliminarCfdiByIdFiniquito(idNominas[0], idEjercicio, pp.IdPeriodoPago, idSucursal);
                }
                else
                {
                    _dao.EliminarCfdiByIdNomina(idNominas, idEjercicio, pp.IdPeriodoPago, idSucursal);
                }
            }
            catch (Exception ex)
            {


            }


            return true;
        }

    }
}
