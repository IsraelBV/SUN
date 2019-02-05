using System;
using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
//using System.Runtime.Remoting.Channels;
using System.Xml;
using Nomina.Procesador.Modelos;
using Nomina.Procesador.Metodos;
using Nomina.Procesador.Datos;

using System.IO;
//using System.Web;
//using System.Xml.Schema;
using RH.Entidades;
using RH.BLL;
using SYA.BLL;
using Common.Utils;
using Nomina.Procesador.Modelos.Cfdi32;
using Nomina.Procesador.Modelos.Cfdi33;
using Nomina.Procesador.Modelos.Nomina12;
using RH.Entidades.GlobalModel;
using Nomina.Procesador.webServicePAC;
//using Org.BouncyCastle.Crypto.Generators;
//using static Nomina.Procesador.Modelos.Cfdi32.Nomina;
//using Common.Enums;



namespace Nomina.Procesador
{
    public class TimbradoCore
    {
        #region VARIABLES GLOBALES

        private int[] _NominasSeleccionadas { get; set; }
        private bool _timbradoDePrueba { get; set; }
        private readonly TimbradoDao _dao = new TimbradoDao();

        private readonly GlobalConfig _gconfig = new GlobalConfig();
        private SYA_GlobalConfig _configuracionGlobal = null;

        private readonly List<int> _nominasTimbradas = new List<int>();
        //guarda las nominas que se timbraron correctamente

        private int _idEjercicio;
        private NOM_PeriodosPago _Periodo;
        private string _pathCertificados;
        private string _pathLog;
        private int _noErrores = 0;
        // private const string FormatoMoneda = "########0.00";
        private PathsCertificado _objPath = null;

        #endregion

        //Constructor

        #region METODO ASYNCRONOS

        /// <summary>
        /// Metodo que realiza el proceso Generar el XML y Timbrado de manera Asyncrona
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="nominasSeleccionadas"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="Periodo"></param>
        /// <param name="pathLog"></param>
        /// <param name="timbradoDePrueba"></param>
        /// <returns></returns>
        public Task<List<NotificationSummary>> GenerarCfdiAsync(int idUsuario, int[] nominasSeleccionadas = null, int idEjercicio = 0,
            NOM_PeriodosPago Periodo = null, string pathLog = null, bool timbradoDePrueba = true, bool isCfdi33 = false)
        {
            var t = Task.Factory.StartNew(() =>
            {

                //valida que los datos esten correcto
                if (idEjercicio <= 0) return null;
                if (nominasSeleccionadas == null) return null;

                // LogTxtCore.EscribirLog(pathLog, "inicio de la tarea GenerarCfdiAsync ...");

                //Establece los valores a las variables globales de la clase
                _NominasSeleccionadas = nominasSeleccionadas;
                _timbradoDePrueba = timbradoDePrueba;
                //necesario para usar el WEB Service de Prueba y los usuarios TEST - TEST
                _idEjercicio = idEjercicio;
                _Periodo = Periodo;
                _pathLog = pathLog;

                //1ro se Genera el XML de cada Empleado
                //2do Guarda el xml sin timbre en la base de datos
                //3ro Consulta la BD con los xml generados y se realiza el proceso de timbrado
                //4to Por cada registro timbrado se actualiza en la bd con el xml y su Timbre
                var r = CrearXmlyTimbrado(idUsuario, Periodo, isCfdi33);

                return r;
            });

            return t;
        }

        public Task<List<NotificationSummary>> CancelarTimbresAsync(int[] arrayIdTimbrado, int idUsuario)
        {
            var t = Task.Factory.StartNew(() =>
            {
                var r = CancelarTimbres(arrayIdTimbrado, idUsuario);
                return r;
            });

            return t;
        }

        /// <summary>
        /// Metodo que ejecuta la creacion de los xml y realiza el timbrado de los xml
        /// por cada registro de nomina
        /// </summary>
        /// <param name="rutaDeArchivos"></param>
        /// <param name="idUsuario"></param>
        /// <param name="isCfdi33"></param>
        /// <returns></returns>
        private List<NotificationSummary> CrearXmlyTimbrado(int idUsuario, NOM_PeriodosPago periodo, bool isCfdi33 = false)
        {
            int foliosUsuados = 0;
            int erroresTimbrados = 0;
            int erroresCrearXml = 0;
            bool isFiniquito = false;
            int consecutivoFolio = 0;
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            //Consulta para obtener los datos de las nominas - Crear xml y timbrado
            List<NominaData> listaDatosNomina = new List<NominaData>();
            if (_Periodo.IdTipoNomina == 11) //finiquito
            {
                isFiniquito = true;
                var itemData = _dao.GetDatosFiniquito(_idEjercicio, _Periodo.IdPeriodoPago, _NominasSeleccionadas[0], ref summaryList);
                listaDatosNomina.Add(itemData);
            }
            else
            {
                listaDatosNomina = _dao.GetDatosNomina(_idEjercicio, _Periodo.IdPeriodoPago, _NominasSeleccionadas, _Periodo.IdTipoNomina, ref summaryList);
            }

            var itemCliente = _dao.GetClienteByIdCliente(periodo.IdCliente);

            //int ultimoRegistro = _dao.GetLastIdTimbrado();

            //Obtenemos de la tabla [SYA_GlobalConfig] - el path de los Certificados archivs .CER .KEY
            _configuracionGlobal = _gconfig.GetGlobalConfig();
            _pathCertificados = _configuracionGlobal.PathCertificados;

            //1) Para cada nomina se crea un xml y se guarda el xml en bd
            foreach (var item in listaDatosNomina)
            {
                //LogTxtCore.EscribirLog(_pathLog, "log...");
                try
                {
                    //Crea el xml en el formato nomina 1.2 - Agrega el Sello al xml generado
                    //***********************************************************************************************************************************
                    List<NotificationSummary> resultMesg;
                    if (isCfdi33) //cfdi 3.3
                    {
                        //Timbrado
                        resultMesg = CrearXmlConSelloYSinTimbre3312(item, periodo, idUsuario, isFiniquito: isFiniquito, itemCliente: itemCliente, fromModuloProcesar: false);
                    }
                    else//cfdi 3.2
                    {
                        resultMesg = CrearXmlConSelloYSinTimbre3212(item, periodo, idUsuario, isFiniquito: isFiniquito, fromModuloProcesar: false);
                    }

                    if (resultMesg.Count > 0)
                    {
                        summaryList.AddRange(resultMesg);
                    }

                    //***********************************************************************************************************************************

                }
                catch (Exception ex)
                {
                    erroresCrearXml++;
                }
            }

            //2) Proceso que realiza el timbrado de las nominas y retorn el numero de folios utilizados
            //************************************************************
            var summaryTimbrado = MetodoPrincipalDeTimbrado3212();

            if (summaryTimbrado.Count > 0)
            {
                summaryList.AddRange(summaryTimbrado);
            }

            //************************************************************


            //3) Actualiar las nominas su campo CFDI creado en la Tabla NOM_Nomina
            if (_nominasTimbradas == null) return summaryList;
            if (_nominasTimbradas.Count > 0)
                _dao.ActualizarNominaCfdiCreado(_nominasTimbradas, isFiniquito: isFiniquito);

            return summaryList;
        }

        #endregion

        #region ***** ***** GENERA EL XML DEL CFDI CON SELLO - COMPROBANTE 3.2 Y COMPLEMENTO NOMINA 1.2 ***** *****

        /// <summary>
        /// Crea el Xml con Sello y Sin Timbre con los datos de la nomina procesada, usando el formato cfdi 3.2  y complemento de nomina 1.2
        /// </summary>
        /// <param name="datosNomina"></param>
        /// <param name="isProcesando"></param>
        private List<NotificationSummary> CrearXmlConSelloYSinTimbre3212(NominaData datosNomina, NOM_PeriodosPago periodoPago, int idUsuario, bool isFiniquito, bool fromModuloProcesar = false)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            if (datosNomina == null) return null; //se suspende la creacion del cfdi de este registro

            try
            {
                #region VARIABLES

                MSatXml xmlsat = new MSatXml();

                // var nombreReceptor = datosNomina.Nombres + " " + datosNomina.Paterno + " " + datosNomina.Materno;
                //Datos para sindicato //string[] strConceptos = { "Apoyo de alimentos", "Herramientas de trabajo", "Beca" };

                #endregion

                #region GET DATOS EMISOR Y RECEPTOR 

                if (datosNomina.IdEmpresa == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = " Id de la empresa es null", Msg2 = "" });
                    return summaryList; //Si en datos de la nomina no tiene Id de Empresa Emisora - se suspende la creacion del cfdi
                }

                //get datos del emisor
                var empresaObj = new Empresas();

                var emisorDatos = empresaObj.GetEmpresaById((int)datosNomina.IdEmpresa);
                if (emisorDatos == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = (int)datosNomina.IdEmpresa, Msg1 = "No se encontro datos del emisor.", Msg2 = "" });
                    return summaryList; //si no se encontró datos de la empresa emisora del cfdi - se suspende la creacion del cfdi
                }

                //Datos del colaborador
                var datosReceptor = _dao.GetDatosReceptor(datosNomina.IdEmpleado);

                if (datosReceptor == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se encontro datos del receptor.", Msg2 = "" });
                    return summaryList; // si no se encontró datos del empleado - - se suspende la creacion del cfdi
                }

                if (datosReceptor.RFCValidadoSAT != 1)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "El RFC del colaborador no esta validado.", Msg2 = "" });
                    //return summaryList; //si el rfc no esta validado por el sat
                }

                #endregion

                #region ARCHIVOS CERTIFICADOS PARA GENERAR EL Certificado Digital del cfdi

                var varArchivoCer = "\\" + emisorDatos.IdEmpresa + "\\" + emisorDatos.ArchivoCER;
                var varArchivoKey = "\\" + emisorDatos.IdEmpresa + "\\" + emisorDatos.ArchivoKEY;
                var pathCadenaOriginalXslt = _pathCertificados + "\\" + "1cadenaoriginal_3_2.xslt"; //<- cambiar la cadena cadenaoriginal_3_2.xslt por una variable global

                #endregion

                #region GET  -  PERCEPCIONES  -   DEDUCCIONES  -   OTROS PAGOS  - DATOS INDEMNIZACION

                var listaPercepcionesDeLaNomina = isFiniquito ? _dao.GetPercepcionesByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetPercepcionesByIdNomina(datosNomina.IdNomina);
                var listaDeduccionesDeLaNomina = isFiniquito ? _dao.GetDeduccionesByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetDeduccionesByIdNomina(datosNomina.IdNomina);
                var listaOtrosPagosDeLaNomina = isFiniquito ? _dao.GetOtrosPagosByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetOtrosPagosByIdNomina(datosNomina.IdNomina);

                var itemIndemnizacion = isFiniquito ? _dao.GetDatosIndemnizacionByIdFiniquito(datosNomina.IdFiniquito) : null;

                if (listaPercepcionesDeLaNomina.Count <= 0 && listaDeduccionesDeLaNomina.Count <= 0 && listaOtrosPagosDeLaNomina.Count <= 0)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se encontrarón datos para generar el cfdi", Msg2 = "" });
                    return summaryList; // se suspende la creacion del cfdi de este registro de nomina
                }
                #endregion

                #region SET TOTALES COMPLEMENTO Y NOMINA

                //// total = totalPercepciones + totalOtros Pagos - Total deducciones
                ////Se actualiza con las deduccuibes e impuesto retenidos
                //ctotal = ((datosNomina.TotalPercepciones.TruncateDecimal(2) + datosNomina.TotalOtrosPagos.TruncateDecimal(2)) - datosNomina.TotalDeducciones.TruncateDecimal(2));

                //// descuento 
                //cdescuento = datosNomina.TotalDeducciones.TruncateDecimal(2); // suma total de deducciones

                //// Nomina.TotalPercepciones  = TotalSueldos + TotalSeparacionIndemnizacion + TotalJubilacionPensionRetiro.
                //ntotalpercepciones = datosNomina.TotalPercepciones.TruncateDecimal(2) + (itemIndemnizacion?.Total.TruncateDecimal(2) ?? 0);// Total sueldo + total indemnizacion + total JubilacionPensionRetiro

                ////nomina Total deducciones
                //ntotaldeducciones = datosNomina.TotalDeducciones.TruncateDecimal(2);// si no hay deducciones este campo no debera incluirse

                ////nomina total otros pagos
                //ntotalotrospagos = datosNomina.TotalOtrosPagos.TruncateDecimal(2);//<-- condicionado al total de listaOtrosPagosDeLaNomina. Suma de otros pagos - reintegro del ISR, subsidio para el empleo, viaticos, compensacion anual, otros

                //// Nomina.Percepciones.TotalSueldos = (ImporteGravado + ImporteExento) 

                //// SubTotal = Nomina12:TotalPercepciones+ Nomina12:TotalOtrosPagos
                //csubtotal = ntotalpercepciones + ntotalotrospagos;

                //// conceto importe = total Percepciones + total otros pagos
                //ccimporte = ntotalpercepciones + ntotalotrospagos;

                //// concepto valor Unitario = total Percepciones + total otros pagos
                //ccvalorunitario = ntotalpercepciones + ntotalotrospagos;




                #endregion

                #region COMPROBANTE 32
                Modelos.Cfdi32.Comprobante comprobante32 = new Modelos.Cfdi32.Comprobante();
                DateTime now = Convert.ToDateTime(DateTime.Now.ToString("s"));

                comprobante32.serie = "A";//<-- no quitar este texto ni cambiarlo por otro
                //comprobante32.folio = "00";//<-- no quitar este texto ni cambiarlo por otro
                //Asignamos como el folio el id de la nomina o del finiquito
                comprobante32.folio = isFiniquito ? datosNomina.IdFiniquito.ToString() : datosNomina.IdNomina.ToString();

                comprobante32.fecha = now;
                comprobante32.sello = "sello"; //<-- no quitar este texto ni cambiarlo por otro
                comprobante32.formaDePago = "Pago en una sola exhibición"; //<-- no quitar este texto ni cambiarlo por otro

                //comprobante32.total = ctotal;
                //comprobante32.subTotal = csubtotal;
                //comprobante32.descuento = cdescuento;
                //comprobante32.descuentoSpecified = true;

                comprobante32.Moneda = "MXN";
                comprobante32.TipoCambio = "1";
                comprobante32.tipoDeComprobante = Modelos.Cfdi32.ComprobanteTipoDeComprobante.egreso;
                comprobante32.metodoDePago = "NA";
                comprobante32.LugarExpedicion = emisorDatos.CP; //codigo postal del cat del sat  [0-9]{5}
                comprobante32.schemaLocation = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv32.xsd http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd";


                //Si el metodo es ejecutado desde el modulo de procesar nomina o procesar finiquito, 
                //no se requiere certificados ni el archivo de cadena original
                if (!fromModuloProcesar)
                {
                    //certificado
                    string archivoCertificado = _pathCertificados + varArchivoCer;
                    string archivoKey = _pathCertificados + varArchivoKey;
                    if (File.Exists(archivoCertificado) && File.Exists(archivoKey) && File.Exists(pathCadenaOriginalXslt))
                    {
                        CertificadoDigital certificado = MCfdi.GetCertificadoDigital(archivoCertificado);

                        comprobante32.certificado = certificado.CertificadoBase64;
                        comprobante32.noCertificado = certificado.NoCertificado;


                        //Rutas de los cerficiados
                        _objPath = new PathsCertificado();
                        _objPath.PathArchivoCer = archivoCertificado;
                        _objPath.PathArchivoKey = archivoKey;
                        _objPath.Password = emisorDatos.LlavePrivada;

                    }
                    else
                    {
                        //Enviar mensaje de los certificados
                        summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se contró los certificados .cer .key", Msg2 = "" });
                        return summaryList;
                        //sino se encontró el archivo .cer se suspende la creacion del cfdi <-- QUITAR EL COMENTARIO AL RETURN
                    }
                }

                #endregion

                #region EMISOR 32

                Modelos.Cfdi32.ComprobanteEmisorRegimenFiscal[] arrayRegimenFiscal = new Modelos.Cfdi32.ComprobanteEmisorRegimenFiscal[1];

                var _regimenFiscalSat = _dao.GetRegimenFiscalById(emisorDatos.RegimenFiscal);
                Modelos.Cfdi32.ComprobanteEmisorRegimenFiscal comprobanteEmisorRegimenFiscal = new Modelos.Cfdi32.ComprobanteEmisorRegimenFiscal()
                {
                    Regimen = _regimenFiscalSat.ClaveRegimen // DEBE SER SEGUN EL CATALOGO DEL SAT  c_RegimenFiscal
                };

                arrayRegimenFiscal[0] = comprobanteEmisorRegimenFiscal;

                Modelos.Cfdi32.ComprobanteEmisor comprobanteEmisor = new Modelos.Cfdi32.ComprobanteEmisor()
                {
                    nombre = emisorDatos.RazonSocial,
                    RegimenFiscal = arrayRegimenFiscal,
                    rfc = emisorDatos.RFC
                };

                #endregion

                #region RECEPTOR 3.2

                Modelos.Cfdi32.ComprobanteReceptor comprobanteReceptor = new Modelos.Cfdi32.ComprobanteReceptor()
                {
                    //USADO ACTUALMENTE - ABC 1 - 32.12
                    rfc = datosReceptor.RFC,
                    //rfc = "AAQM610917QJA",
                    nombre = ($"{datosReceptor.APaterno} {datosReceptor.AMaterno} {datosReceptor.Nombres}")
                };

                #endregion

                #region CONCEPTO DE PAGO 32 -  Pago de nomina - Unidad ATC -

                Modelos.Cfdi32.ComprobanteConcepto[] arrayComprobanteConceptos = new Modelos.Cfdi32.ComprobanteConcepto[1];
                Modelos.Cfdi32.ComprobanteConcepto comprobanteConcepto = new Modelos.Cfdi32.ComprobanteConcepto()
                {
                    cantidad = 1,
                    descripcion = "Pago de nómina",
                    importe = 0,//ccimporte,
                    unidad = "ACT",
                    valorUnitario = 0//ccvalorunitario
                };
                arrayComprobanteConceptos[0] = comprobanteConcepto;

                #endregion

                #region IMPUESTOS 32 - no aplica para nominas

                Modelos.Cfdi32.ComprobanteImpuestosRetencion comprobanteImpuestosRetencion = new Modelos.Cfdi32.ComprobanteImpuestosRetencion()
                {
                    impuesto = Modelos.Cfdi32.ComprobanteImpuestosRetencionImpuesto.ISR,
                    importe = 0 //total de la nomina
                };

                Modelos.Cfdi32.ComprobanteImpuestosRetencion[] arrayComprobanteImpuestosRetenciones = new Modelos.Cfdi32.ComprobanteImpuestosRetencion[1];
                arrayComprobanteImpuestosRetenciones[0] = comprobanteImpuestosRetencion;

                Modelos.Cfdi32.ComprobanteImpuestos comprobanteImpuestos = new Modelos.Cfdi32.ComprobanteImpuestos()
                {
                    Retenciones = null,//arrayComprobanteImpuestosRetenciones,
                    totalImpuestosRetenidos = 0,
                    totalImpuestosRetenidosSpecified = false,

                };

                #endregion

                // Anexar al Tag Comprobante los objetos creados
                comprobante32.Emisor = comprobanteEmisor;
                comprobante32.Receptor = comprobanteReceptor;
                comprobante32.Conceptos = arrayComprobanteConceptos;
                comprobante32.Impuestos = comprobanteImpuestos; //no aplica para nomina porque el dato de impuesto esta incluido en el TAG de <complemento>
                                                                //pero la validación del pac requiere que el tag impuesto este en el xml


                #region COMPLEMENTO DE NOMINA 1.2

                var fechaFinalPago = DateTime.Parse(datosNomina.FechaFinaldePago);
                Modelos.Nomina12.Nomina nomina12 = new Modelos.Nomina12.Nomina();
                nomina12.Version = "1.2";
                nomina12.TipoNomina = datosNomina.TipoNominaSat; // pagos ordinarios diario a mensual, Extraordinario : aguinaldo, finiquito, etc.
                if (periodoPago.IdTipoNomina == 16)
                {
                    nomina12.TipoNomina = "E";
                }

                nomina12.FechaPago = fechaFinalPago;//DateTime.Parse(datosNomina.FechaInicialdePago);
                nomina12.FechaInicialPago = DateTime.Parse(datosNomina.FechaInicialdePago);
                nomina12.FechaFinalPago = fechaFinalPago; //DateTime.Parse(datosNomina.FechaFinaldePago);
                nomina12.NumDiasPagados = datosNomina.DiasPagados;
                //nomina12.TotalPercepciones = ntotalpercepciones;
                //nomina12.TotalPercepcionesSpecified = true;
                //nomina12.TotalDeducciones = ntotaldeducciones;
                //nomina12.TotalDeduccionesSpecified = true;
                //nomina12.TotalOtrosPagos = ntotalotrospagos;
                //nomina12.TotalOtrosPagosSpecified = true; //<-- condicionado si hay otros pagos en listaOtrosPagosDeLaNomina

                #region EMISOR - RECEPTOR 1.2  - 3.2

                // EMISOR
                var nominaEmisor = new Modelos.Nomina12.NominaEmisor();

                if (periodoPago.IdTipoNomina != 16) //Asimilados
                {
                    //USADO ACTUALMENTE - ABC 2 - 32.12
                    nominaEmisor.RegistroPatronal = emisorDatos.RegistroPatronal;// "Registro patronal" NO APLICA CUANDO ES SIMILADO A SALARIOS
                    nominaEmisor.RfcPatronOrigen = emisorDatos.RFC;
                    //nominaEmisor.Curp = "CACF880922HJCMSR03"; // solo comentar - Curp = Personas morales no tienen este dato - Comentar esta linea - solo se usa para pruebas con GOYA780416GM0
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

                if (periodoPago.IdTipoNomina != 16) //no aplica para Asimilados
                {
                    nominaReceptor.Sindicalizado = Modelos.Nomina12.NominaReceptorSindicalizado.No;//no aplica para asimilados a salarios
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
                    if (datosNomina.PagoEnEfectivo == false)
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
                    nominaReceptor.SalarioBaseCotApor = datosNomina.Sbc;
                    //No aplica para asimilados a salarios
                    nominaReceptor.SalarioBaseCotAporSpecified = true;
                    nominaReceptor.SalarioDiarioIntegrado = datosNomina.Sdi;
                    //no aplica para asimilados a salarios
                    nominaReceptor.SalarioDiarioIntegradoSpecified = true;
                }

                nominaReceptor.ClaveEntFed = datosNomina.ClaveEntidadFederativa; //cat sat c_estado

                #endregion

                #region PERCEPCIONES 1.2

                Modelos.Nomina12.NominaPercepcionesPercepcion[] arrayPercepciones = new Modelos.Nomina12.NominaPercepcionesPercepcion[listaPercepcionesDeLaNomina.Count()];

                decimal totalGravadoPercepcion = 0;
                decimal totalExcentoPercepcion = 0;
                decimal totalSueldo = 0; // 
                int idxPercepcion = 0;

                foreach (var p in listaPercepcionesDeLaNomina)
                {
                    #region ARRAY - ITEM PERCEPCIONES

                    Modelos.Nomina12.NominaPercepcionesPercepcion itemP = new Modelos.Nomina12.NominaPercepcionesPercepcion
                    {
                        TipoPercepcion = p.ClaveSat.PadLeft(3, '0'),
                        Clave = p.IdConcepto.ToString().PadLeft(3, '0'),
                        Concepto = p.NombreConcepto,
                        ImporteGravado = p.Gravado,
                        ImporteExento = p.Excento
                        //Horas extras
                        //Acciones O Titulos
                    };

                    // Total de percepciones brutas (gravadas y exentas) exepto conceptos con las claves: 
                    //022 prima antiguedad //023 pagos por separacion //025 Indemnizaciones //039 Jubilaciones, pensiones o haberes de retiro //044 
                    if (p.ClaveSat == "022" || p.ClaveSat == "023" || p.ClaveSat == "025" || p.ClaveSat == "039" ||
                        p.ClaveSat == "044")
                    {
                        totalGravadoPercepcion += p.Gravado;
                        totalExcentoPercepcion += p.Excento;
                    }
                    else
                    {
                        //Nomina.Percepciones.TotalSueldos = ImporteGravado + ImporteExento 
                        //donde la clave expresada en el atributo TipoPercepcion es distinta de 022 Prima por Antigüedad, 023 Pagos por separación, 025 Indemnizaciones, 039 Jubilaciones, pensiones o haberes de retiro en una exhibición y 044 Jubilaciones, pensiones o haberes de retiro en parcialidades
                        totalSueldo += p.Importe;
                        totalGravadoPercepcion += p.Gravado;
                        totalExcentoPercepcion += p.Excento;
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
                    TotalPagado = itemIndemnizacion?.Total ?? 0, //total indemnizacion
                    NumAñosServicio = itemIndemnizacion?.AniosServicios ?? 0,
                    //años de servicio 10años + 6meses = 11 años, 6 meses se considera completo
                    UltimoSueldoMensOrd = itemIndemnizacion?.UltimoSueldoMensOrd ?? 0,
                    IngresoAcumulable = itemIndemnizacion?.IngresoAcumulable ?? 0,
                    IngresoNoAcumulable = itemIndemnizacion?.IngresoNoAcumulable ?? 0
                };

                #endregion

                //NODO Principal de Percepciones
                var nominaPercepcion = new Modelos.Nomina12.NominaPercepciones();

                nominaPercepcion.Percepcion = arrayPercepciones;
                nominaPercepcion.TotalExento = totalExcentoPercepcion;
                nominaPercepcion.TotalGravado = totalGravadoPercepcion;
                nominaPercepcion.TotalSueldos = totalSueldo;// Total de percepciones brutas (gravadas y exentas) exepto conceptos con las  claves:  022, 023, 025, 039, 044
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
                            //Modelos.Cfdi32.NominaOtroPago otroDevolucionIsr = new NominaOtroPago()
                            //{
                            //    TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //    Clave = itemOtroPago.ClaveContable,
                            //    Concepto = itemOtroPago.NombreConcepto,
                            //    Importe = itemOtroPago.Importe
                            //};
                            //arrayOtrosPagos.Add(otroDevolucionIsr);
                            break;
                        case 2://Subsidio Efectivamente Entregado al colaborador
                            Modelos.Nomina12.NominaOtroPagoSubsidioAlEmpleo subsidioAlEmpleo = new Modelos.Nomina12.NominaOtroPagoSubsidioAlEmpleo()
                            {
                                SubsidioCausado = datosNomina.SubsidioCausado //Subsidio Causado
                            };

                            Modelos.Nomina12.NominaOtroPago otroPago = new Modelos.Nomina12.NominaOtroPago()
                            {
                                TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                                Clave = itemOtroPago.ClaveContable,
                                Concepto = itemOtroPago.NombreConcepto,
                                Importe = itemOtroPago.Importe,
                                SubsidioAlEmpleo = subsidioAlEmpleo
                            };
                            arrayOtrosPagos.Add(otroPago);
                            break;
                        case 3: //Viaticos
                            //Modelos.Cfdi32.NominaOtroPago otroViaticos = new NominaOtroPago()
                            //{
                            //    TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //    Clave = itemOtroPago.ClaveContable,
                            //    Concepto = itemOtroPago.NombreConcepto,
                            //    Importe = itemOtroPago.Importe
                            //};
                            //arrayOtrosPagos.Add(otroViaticos);
                            break;
                        case 4://Saldo a Favor

                            //Modelos.Cfdi32.NominaOtroPagoCompensacionSaldosAFavor otroSaldoFavor = new NominaOtroPagoCompensacionSaldosAFavor()
                            //{
                            // SaldoAFavor = itemOtroPago.Importe,
                            // Año = 5,
                            // RemanenteSalFav = 10
                            //};


                            //Modelos.Cfdi32.NominaOtroPago otroPago2 = new Modelos.Cfdi32.NominaOtroPago()
                            //{
                            //    TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            //    Clave = itemOtroPago.ClaveContable,
                            //    Concepto = itemOtroPago.NombreConcepto,
                            //    Importe = itemOtroPago.Importe,
                            //    CompensacionSaldosAFavor = otroSaldoFavor
                            //};

                            //arrayOtrosPagos.Add(otroPago2);

                            break;
                        case 5: //999 otro pagos que no deben considerarse como ingreso por sueldo, salarios o ingresos asimilados
                            Modelos.Nomina12.NominaOtroPago otrosPago = new Modelos.Nomina12.NominaOtroPago()
                            {
                                TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                                Clave = itemOtroPago.ClaveContable,
                                Concepto = itemOtroPago.NombreConcepto,
                                Importe = itemOtroPago.Importe
                            };

                            arrayOtrosPagos.Add(otrosPago);

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
                        Importe = d.Importe
                    };


                    if (itemD.TipoDeduccion != "002") //
                    {
                        totalOtrasDeducciones += d.Importe;
                    }
                    else //Suma a todos los que tienen Tipo deduccion  002 // ISR
                    {
                        totalImpuestosRetenidos += d.Importe;
                        if (d.IsLiquidacion)
                        {
                            itemD.Concepto = "ISR Liq";
                        }
                    }

                    arrayD[idxDeduccion] = itemD;
                    idxDeduccion++;
                }

                var nominaDeduccion = new Modelos.Nomina12.NominaDeducciones();

                nominaDeduccion.Deduccion = arrayD;
                nominaDeduccion.TotalOtrasDeducciones = totalOtrasDeducciones;
                //total = Seguridad Social, aportaciones a retiro, cesantía en edad avanzada y vejez, aportaciones al fondo de vivienda, descuento por incapacidad, pension alimenticia, no incluye el concepto 002 (ISR) -
                nominaDeduccion.TotalOtrasDeduccionesSpecified = true;

                if (totalImpuestosRetenidos > 0)
                {
                    nominaDeduccion.TotalImpuestosRetenidos = totalImpuestosRetenidos;
                    //suma del ISR , donde la clave sea de tipo concepto 002, si no existe no se deberá registrar
                    nominaDeduccion.TotalImpuestosRetenidosSpecified = true;
                }

                //Actualiza el campo TotalDeduccioes de la nomina
                //Sumando el totalOtrasDeducciones + totalImpuestoRetenidos
                nomina12.TotalDeducciones = totalOtrasDeducciones + totalImpuestosRetenidos;

                nomina12.TotalDeduccionesSpecified = nomina12.TotalDeducciones > 0;

                #endregion

                #region INCAPACIDADES 1.2 en 3.2

                //Modelos.Nomina12.NominaIncapacidad incapacidad = new Modelos.Nomina12.NominaIncapacidad()
                //{
                //    DiasIncapacidad = 7,
                //    TipoIncapacidad = "Maternidad",
                //    ImporteMonetario = 0
                //};



                //Modelos.Nomina12.NominaIncapacidad[] arrayIncapacidades = new Modelos.Nomina12.NominaIncapacidad[1];

                //arrayIncapacidades[0] = incapacidad;

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

                comprobante32.descuento = nomina12.TotalDeducciones;

                var totalActualizadoRecibo = (nomina12.TotalPercepciones + nomina12.TotalOtrosPagos) - nomina12.TotalDeducciones;

                //if (itemIndemnizacion != null)
                //{
                //    totalActualizadoRecibo += nominaPercepcion.SeparacionIndemnizacion.TotalPagado;
                //}

                comprobante32.total = totalActualizadoRecibo;

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

                // descuento 
                //cdescuento = datosNomina.TotalDeducciones.TruncateDecimal(2); // suma total de deducciones
                //cdescuento = totalOtrasDeducciones;

                // Nomina.TotalPercepciones  = TotalSueldos + TotalSeparacionIndemnizacion + TotalJubilacionPensionRetiro.
                //ntotalpercepciones = datosNomina.TotalPercepciones.TruncateDecimal(2) + (itemIndemnizacion?.Total.TruncateDecimal(2) ?? 0);// Total sueldo + total indemnizacion + total JubilacionPensionRetiro
                ntotalpercepciones = totalSueldo + (itemIndemnizacion?.Total ?? 0);// Total sueldo + total indemnizacion + total JubilacionPensionRetiro

                //nomina Total deducciones
                ntotaldeducciones = nomina12.TotalDeducciones; //datosNomina.TotalDeducciones.TruncateDecimal(2);// si no hay deducciones este campo no debera incluirse


                // total = totalPercepciones + totalOtros Pagos - Total deducciones
                //Se actualiza con las deduccuibes e impuesto retenidos
                ctotal = (ntotalpercepciones + datosNomina.TotalOtrosPagos) - ntotaldeducciones;//datosNomina.TotalDeducciones.TruncateDecimal(2);


                //nomina total otros pagos
                ntotalotrospagos = datosNomina.TotalOtrosPagos;//<-- condicionado al total de listaOtrosPagosDeLaNomina. Suma de otros pagos - reintegro del ISR, subsidio para el empleo, viaticos, compensacion anual, otros

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
                    //cdescuento += ntotaldeducciones;
                    //nomina12.TotalDeducciones = ntotaldeducciones;
                    nomina12.TotalDeduccionesSpecified = true;
                }

                if (periodoPago.IdTipoNomina != 16) //Asimilados
                {
                    nomina12.TotalOtrosPagos = ntotalotrospagos;
                    nomina12.TotalOtrosPagosSpecified = true;//<-- condicionado si hay otros pagos en listaOtrosPagosDeLaNomina
                }


                // TOTALES COMPROBANTE
                comprobante32.total = ctotal;
                comprobante32.subTotal = csubtotal;
                comprobante32.descuento = cdescuento;
                comprobante32.descuentoSpecified = true;

                comprobanteConcepto.importe = ccimporte;
                comprobanteConcepto.valorUnitario = ccvalorunitario;

                // TOTALES NOMINA 1.2
                nomina12.TotalPercepciones = ntotalpercepciones;
                nomina12.TotalPercepcionesSpecified = true;



                #endregion

                #region GENERAR EL TAG NOMINA Y SE ANEXA AL COMPLEMENTO DEL COMPROBANTE

                //Genera los tag xml de la nomina 1.2 -  que se anexará al tag de complemento
                var elementResult = MSatXml.GetXmlElement(nomina12);

                XmlElement[] arrayXmlElements = new XmlElement[1];

                arrayXmlElements[0] = elementResult;

                //Genera Tag complemento de nomina donde agregamos los tag xml de la nomina 1.2
                Modelos.Cfdi32.ComprobanteComplemento comprobanteComplemento = new Modelos.Cfdi32.ComprobanteComplemento()
                {
                    Any = arrayXmlElements
                };

                //Agrega el tag complemento al xml 3.2
                comprobante32.Complemento = comprobanteComplemento;

                #endregion

                #region SELLO Y CADENA ORIGINAL

                //Generar el xml en string
                var xml32SinSello = MSatXml.GenerarXmlComprobante32(comprobante32);

                string cadenaOriginal32 = null;// en esta variable guardamos la cadena original que se generará juntamente con el sello

                //Si el metodo es ejecutado desde el modulo de procesar nomina o procesar finiquito no se requiere generar el sello
                if (!fromModuloProcesar)
                {
                    //Se crea el sello 32 y se anexa al comprobante en el tag de sello
                    string sello32 = xmlsat.GenerarSello(xml32SinSello, pathCadenaOriginalXslt, _objPath, ref cadenaOriginal32);
                    comprobante32.sello = sello32;
                }

                #endregion

                //Finalmente generamos la estrucutura del xml.
                var xml32Sintimbre = MSatXml.GenerarXmlComprobante32(comprobante32); // ¡Este xml aun no esta timbrado! 

                //Guardamos el xml en la base de datos - para ser timbrado posteriormente
                GuardarXmLenBd(datosNomina, xml32Sintimbre, cadenaOriginal32, idUsuario, isFiniquito, fromModuloProcesar, versionCfdi: 32);

            }
            catch (Exception ex)
            {
                summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = ex.Message, Msg2 = "" });

            }

            return summaryList;
        }

        #endregion

        #region ***** ***** TIMBRADO CFDI 3.2 NOMINA 1.2 - CONEXION CON EL WEB SERVICE DEL PAC ***** ***** ****** ***** ****** *

        /// <summary>
        /// Metodo que realiza el timbrado de los recibos que ya fueron procesados, que tienen su xml creado en la bd
        /// CFDI 3.2  y complemento de Nomina 1.2
        /// </summary>
        /// <param name="rutaDeArchivos"></param> 
        private List<NotificationSummary> MetodoPrincipalDeTimbrado3212()
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            string _remoteAddress = "";
            #region GET DATOS

            string xmlSinTimbrePrueba = "";
            bool isfiniquito = false;
            isfiniquito = _Periodo.IdTipoNomina == 11;

            List<int> nominasTimbradas = new List<int>();
            //string xmlSinTimbre = "";
            //int IdFolio = 0;
            int erroresTimbrado = 0;

            //validar que el array de id nominas no sea null
            if (this._NominasSeleccionadas == null)
            {
                return summaryList;
            }

            // consultar los registros a timbrar de la tabla de nom_cfdi_timbrado
            var listaRegistros = _dao.GetRegistrosATimbrar(_NominasSeleccionadas, isFiniquito: isfiniquito);

            //validar que la lista no sea null 
            if (listaRegistros == null)
            {
                summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontraron registros para timbrar", Msg2 = "" });
                return summaryList;
            }

            //DATOS de [SYA_GlobalConfig]
            GlobalConfig gconfig = new GlobalConfig();
            var servidorConfig = gconfig.GetGlobalConfig();

            // obetener las claves de usuario para el timbrado
            var usuarioPac = "";
            var passwordPac = "";

            //CLAVE DE USUARIO PARA EL TIMBRADO
            if (_timbradoDePrueba)
            {
                summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = $"1:Se uso el usuario: {servidorConfig.UsuarioPacTest}", Msg2 = "" });
                usuarioPac = servidorConfig.UsuarioPacTest;
                passwordPac = servidorConfig.PasswordPacTest;
            }
            else
            {
                summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = $"2:Se uso el usuario: {servidorConfig.UsuarioPac}", Msg2 = "" });
                usuarioPac = servidorConfig.UsuarioPac;
                passwordPac = servidorConfig.PasswordPac;
            }

            #endregion

            //Si es timbrado de prueba, se asigna el web service de Stage
            //sino se toma la url real
            // tomar la url desde el objeto servidor
            // https://stagetimbrado.facturador.com/timbrado.asmx
            _remoteAddress = _timbradoDePrueba ? servidorConfig.UrlPacTest : servidorConfig.UrlPac;

            //Actualizamos el Objeto SOAP con la url para el timbrado
            System.Net.ServicePointManager.Expect100Continue = false;
            var clienteSoap = new wsTimbradoSoapClient("wsTimbradoSoap", _remoteAddress);
            clienteSoap.Open();

            #region "SE REALIZA EL TIMBRADO POR CADA REGISTRO DE LA LISTA Y ACTUALIZA LA BD"

            int registrosTimbrado = 0;
            Empresa empresaEmisorDatos = new Empresa(); //para obtener los datos del emisor del recibo
            var empObj = new Empresas(); //Objeto para ejecutar la consulta y obtener los datos de la empresa

            foreach (var item in listaRegistros)
            {
                try
                {
                    #region VALIDA LOS DATOS DE LA EMPRESA EMISORA Y OBTIENE LOS CERTIFICADOS

                    //Datos del Emisor de Recibo - La Empresa Emisora con registro patronal
                    if (empresaEmisorDatos == null)
                    {
                        empresaEmisorDatos = empObj.GetEmpresaById(item.IdEmisor);
                    }
                    else
                    {
                        //Si los datos del emisor no son los mismo que en la anterior iteracion
                        //se realiza la consulta, sino se mantiene los datos de la empresa
                        if (empresaEmisorDatos.IdEmpresa != item.IdEmisor)
                            empresaEmisorDatos = empObj.GetEmpresaById(item.IdEmisor);
                    }


                    //Obtiene la ruta del archivo .cer  del emisor del recibo
                    var archivoCer = _pathCertificados + "/" + empresaEmisorDatos.IdEmpresa + "/" +
                                     empresaEmisorDatos.ArchivoCER;

                    // CertificadoDigital certificado = MCfdi.GetCertificadoDigital(archivoCer);

                    #endregion

                    //Se envia la Solicitud de Timbrado al web service del PAC 
                    // Solicitud --> PAC --> Valida el xml -->PAC -> Solicita el Timbre al SAT
                    // Respuesta <-- PAC <-- SAT retornar el timbre al PAC

                    // System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AddressOf ValidarCertificado);
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidarCertificado);
                    var respuestaPac = clienteSoap.obtenerTimbrado(item.XMLSinTimbre, usuarioPac, passwordPac);


                    //VALIDAR RESPUESTA ANTES DE INTEGRAR EL TIMBRE AL XML DEL CFDI
                    if (EsRespuestaValida(respuestaPac))
                    {
                        #region ACTUALIZAR EL REGISTRO EN BD Y XML CON LOS DATOS DEL TIMBRADO

                        //Convertir la respuesta del PAC en string
                        string respuesta = respuestaPac.ToString();

                        //Separa los tag de la respuesta
                        string[] arrayTag = respuesta.Split('>');

                        //se genera el tag donde se incluye el timbre de la respuesta del pac
                        string tagTimbre = "</nomina12:Nomina>" + arrayTag[1] + ">";

                        //se anexa el tag al XML 
                        string xmlNuevo = item.XMLSinTimbre.Replace("</nomina12:Nomina>", tagTimbre);


                        //se actualiza el folio en el xml
                        // xmlNuevo = xmlNuevo.Replace("folio=\"00\"", "folio=\"" + item.IdTimbrado + "\""); //versio anterior

                        //se actualiza el folio en el xml
                        //if (xmlNuevo.Contains("folio=\"00\"")) // version 3.2
                        //{
                        //    xmlNuevo = xmlNuevo.Replace("folio=\"00\"", "folio=\"" + item.IdTimbrado + "\"");
                        //}
                        //else if (xmlNuevo.Contains("Folio=\"00\"")) // version 3.3
                        //{
                        //    xmlNuevo = xmlNuevo.Replace("Folio=\"00\"", "Folio=\"" + item.IdTimbrado + "\"");
                        //}


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
                        //  _dao.GuardarCambios();

                        //agregar a la lista de nominas timbradas correctamente
                        _nominasTimbradas.Add(isfiniquito ? item.IdFiniquito : item.IdNomina);

                        #endregion
                    }
                    else
                    {
                        //Convertir la respuesta del PAC en string
                        string respuesta = respuestaPac.ToString();
                        item.ErrorTimbrado = true;
                        item.MensajeRespuesta = respuesta;
                        //   _dao.GuardarCambios();
                    }
                }
                catch (Exception ex)
                {
                    erroresTimbrado++;
                    item.ErrorTimbrado = true;
                    item.MensajeRespuesta = "ex ? - " + ex.Message;
                }
                finally
                {

                    _dao.ActualizarCambiosTimbrado(item);
                }

            } //final for

            #endregion

            clienteSoap.Close();
            //guardar folios usados - registrosTimbrado
            if (registrosTimbrado > 0)
            {
                gconfig.SetFoliosUsados(registrosTimbrado);
            }

            return summaryList;
        }

        /// <summary>
        /// Metodo para revisar la respuesta del PAC
        /// </summary>
        /// <param name="mensajeRespuesta"></param>
        /// <returns></returns>
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
        #endregion

        #region METODOS USADOS EN EL PROCESADO DE LA NOMINA

        /// <summary>
        /// Metodo utilizado para mostrar un recibo sin timbre en el modulo del procesado de la nomina
        /// </summary>
        /// <param name="nominasId"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="isProcesando"></param>
        /// <returns></returns>
        public Task<int> GenerarXmlAsync(int[] nominasId, NOM_PeriodosPago periodoPago, int idEjercicio, int idPeriodo, int idUsuario, bool fromProcesando = false, bool isCfdi33 = false)
        {
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    CrearXMlSinTimbre(nominasId, periodoPago, idEjercicio, idPeriodo, idUsuario, fromProcesando, isCfdi33);
                }
                catch (Exception)
                {
                }
                finally
                {
                    // _nominasDao.SaveChange();

                }

                return 1;
            });

            return t;
        }

        /// <summary>
        /// Metodo utilizado en el modulo de procesar nomina, genera el xml para generar el pdf del recibo sin timbre.
        /// Se utiliza para pre-visualizar como quedará el recibo con el timbrado
        /// </summary>
        /// <param name="nominasId"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="isProcesando"></param>
        private void CrearXMlSinTimbre(int[] nominasId, NOM_PeriodosPago periodoPago, int idEjercicio, int idPeriodo, int idUsuario, bool fromModuloProcesar = false, bool isCfdi33 = false)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            //recibo sin timbre
            var listaDatosNominas = _dao.GetDatosNomina(idEjercicio, idPeriodo, nominasId, periodoPago.IdTipoNomina, ref summaryList);

            if (listaDatosNominas == null) return;

            var itemCliente = _dao.GetClienteByIdCliente(periodoPago.IdCliente);

            var config = _gconfig.GetGlobalConfig();
            _pathCertificados = config.PathCertificados;

            listaDatosNominas = listaDatosNominas.OrderBy(x => x.IdNomina).ToList();

            foreach (var item in listaDatosNominas)
            {
                //LogTxtCore.EscribirLog(_pathLog, "log...");
                try
                {
                    if (isCfdi33)
                    {
                        //Usado en el metodo del procesado - generar xml sin timbre
                        CrearXmlConSelloYSinTimbre3312(item, periodoPago, idUsuario, isFiniquito: false, itemCliente: itemCliente, fromModuloProcesar: fromModuloProcesar);//1)
                    }
                    else
                    {
                        //Usado en el metodo del procesado - generar xml sin timbre
                        CrearXmlConSelloYSinTimbre3212(item, periodoPago, idUsuario, isFiniquito: false, fromModuloProcesar: fromModuloProcesar);//1)
                    }



                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }

        #endregion

        #region METODOS USADOS  EN EL PROCESADO DEL FINIQUITO

        public Task<int> GenerarXmlFiniquitoAsync(int idFiniquito, NOM_PeriodosPago periodoPago, int idEjercicio, int idPeriodo, int idUsuario, bool fromProcesando, bool isCfdi33 = false)
        {
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    CrearXMlSinTimbreFiniquito(idFiniquito, periodoPago, idEjercicio, idPeriodo, idUsuario, fromProcesando, isCfdi33: isCfdi33);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    // _nominasDao.SaveChange();

                }

                return 1;
            });

            return t;
        }

        /// <summary>
        /// Metodo utilizado en el modulo de procesar nomina, genera el xml para generar el pdf del recibo sin timbre.
        /// Se utiliza para pre-visualizar como quedará el recibo con el timbrado
        /// </summary>
        /// <param name="nominasId"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="isProcesando"></param>
        private void CrearXMlSinTimbreFiniquito(int idFiniquito, NOM_PeriodosPago periodoPago, int idEjercicio, int idPeriodo, int idUsuario, bool fromModuloProcesar, bool isCfdi33 = false)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            //recibo sin timbre
            var itemFiniquito = _dao.GetDatosFiniquito(idEjercicio, idPeriodo, idFiniquito, ref summaryList);

            if (itemFiniquito == null) return;

            var config = _gconfig.GetGlobalConfig();
            _pathCertificados = config.PathCertificados;

            var itemCliente = _dao.GetClienteByIdCliente(periodoPago.IdCliente);
            //LogTxtCore.EscribirLog(_pathLog, "log...");
            try
            {
                if (isCfdi33)
                {
                    //xml para el finiquito
                    CrearXmlConSelloYSinTimbre3312(itemFiniquito, periodoPago, idUsuario, isFiniquito: true, itemCliente: itemCliente, fromModuloProcesar: fromModuloProcesar);
                }
                else
                {
                    //xml para el finiquito
                    CrearXmlConSelloYSinTimbre3212(itemFiniquito, periodoPago, idUsuario, isFiniquito: true, fromModuloProcesar: fromModuloProcesar);
                }


            }
            catch (Exception ex)
            {
                // ignored
            }

        }

        #endregion

        #region CANCELACION DE TIMBRADO

        public List<NotificationSummary> CancelarTimbres(int[] arrayIdTimbrado, int idUsuario)
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

            remoteAddressWs = _timbradoDePrueba ? servidorConfig.UrlPacTest : servidorConfig.UrlPac;

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

        public void CancelarTimbre(int idEmpresaEmisora, int idEjercicio, int idPeriodo, int idSucursal,
            List<CancelarCfdi> uuidsACancelar, int idUsuario)
        {
            //Obtener los datos del Emisor
            var emisor = _dao.GetEmpresaById(idEmpresaEmisora);

            if (emisor == null) return;


            //Obtener el Path de los Certificados
            var config = _gconfig.GetGlobalConfig();

            if (config == null) return;

            var pathArchivoPfx = config.PathCertificados + "/" + emisor.IdEmpresa + "/" + emisor.ArchivoPFX;

            //Validar que exista el archivo pfx del emisor
            if (Utils.ExisteArchivo(pathArchivoPfx) == false) return;

            //Establece conexion con el web Service del PAC
            wsTimbradoSoapClient pacServiceClient = new wsTimbradoSoapClient();
            pacServiceClient.Open();

            foreach (var itemUudi in uuidsACancelar)
            {
                string[] uudi = new string[1];
                uudi[0] = itemUudi.FolioFiscalUuid;

                ArrayOfString uudiList = new ArrayOfString();
                uudiList.AddRange(uudi);

                //Se realiza la cancelacion
                // Solicitud --> PAC --> Validacion --> Envia al SAT la Cancelacion
                //Respuesta <-- PAC <-- SAT Response
                byte[] pfxBytes = Utils.GetBytesArchivo(pathArchivoPfx);

                //0 referencias
                XElement respuestaXE = pacServiceClient.EnviarCancelacionPFX(config.UsuarioPac, config.PasswordPac, uudiList, pfxBytes, emisor.LlavePrivada, emisor.RFC);
                ////Cancelado Prueba ABC 3 XElement respuestaXE = pacServiceClient.EnviarCancelacionPFX(config.UsuarioPacTest, config.PasswordPacTest, uudiList, pfxBytes, "12345678a", "GOYA780416GM0"); 


                //Convertimos la respuesta en string 
                string strRespuesta = respuestaXE.ToString();

                if (strRespuesta.Contains("esValido=\"True\""))
                {
                    //Si se realizo el cancelado se actualiza el objeto como cancelado
                    itemUudi.Cancelado = true;
                }

            }
            //Cerrar conexion con el Pac
            pacServiceClient.Close();

            //Actualizar 
            ActualizarBdCancelados(uuidsACancelar, idUsuario);
        }

        /// <summary>
        /// Actualiza la 
        /// </summary>
        /// <param name="uuidsACancelar"></param>
        /// <param name="idUsuario"></param>
        private void ActualizarBdCancelados(List<CancelarCfdi> uuidsACancelar, int idUsuario)
        {
            if (uuidsACancelar == null) return;

            //se filtra los registros con la propiedad cancelado es true
            var listaCancelados = uuidsACancelar.Where(x => x.Cancelado == true).ToList();
            _dao.ActualizarCancelados(listaCancelados, idUsuario);

        }



        #endregion

        #region METODOS DE UTILIDAD - EliminaCfdi - Validar Certificados - Guarda en BD

        /// <summary>
        /// Guarda el xml genereado en la BD -
        /// </summary>
        /// <param name="datosNomina"></param>
        /// <param name="xmlGenerado"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="idUsuario"></param>
        /// <param name="isFiniquito"></param>
        /// <param name="fromModuloProcesar"></param>
        /// <param name="versionCfdi"></param>
        /// <returns></returns>
        private bool GuardarXmLenBd(NominaData datosNomina, string xmlGenerado, string cadenaOriginal, int idUsuario, bool isFiniquito, bool fromModuloProcesar = false, int versionCfdi = 32)
        {
            if (xmlGenerado == null) return false;

            var result = false;

            if (fromModuloProcesar) //se esta ejecutando desde el Procesado de la nomina
            {
                if (isFiniquito)
                {
                    var finiquito = _dao.GetFiniquitoById(datosNomina.IdFiniquito);
                    finiquito.XMLSinTimbre = xmlGenerado;
                    _dao.ActualizarCambiosFiniquito(finiquito);
                }
                else
                {
                    //guarda el xml sin timbre en la nomina
                    var nomina = _dao.GetNominaById(datosNomina.IdNomina);
                    nomina.XMLSinTimbre = xmlGenerado;
                    _dao.ActualizarCambiosNomina(nomina);
                }
            }
            else //se esta ejecutando desde el Generador de CFDI
            {
                var version = "3.2";

                if (versionCfdi == 33)
                {
                    version = "3.3";
                }

                NOM_CFDI_Timbrado registro = new NOM_CFDI_Timbrado
                {
                    IdTimbrado = 0,
                    Version = version,
                    Serie = "A",
                    Folio = 0,
                    IdNomina = datosNomina.IdNomina,
                    IdFiniquito = datosNomina.IdFiniquito,
                    IdSucursal = datosNomina.IdSucursal,
                    IdEmisor = datosNomina.IdEmisor,
                    IdReceptor = datosNomina.IdEmpleado,
                    RFCReceptor = datosNomina.Rfc,
                    Cancelado = false,
                    XMLSinTimbre = xmlGenerado,
                    TotalRecibo = datosNomina.TotalNomina,
                    CadenaOriginal = cadenaOriginal,
                    UsuarioTimbro = idUsuario,
                    IdEjercicio = datosNomina.IdEjercicio,
                    IdPeriodo = datosNomina.IdPeriodo,
                    RFCEmisor = datosNomina.RfcEmisor,
                    FechaReg = DateTime.Now

                };

                result = _dao.GuardarDatosCfdi(registro);
            }


            return result;
        }

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

        private Boolean ValidarCertificado(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion

        #region CFDI 3.3


        /// <summary>
        /// Crea el archivo xml con la version 3.3 y complemento de nomina 1.2
        /// </summary>
        /// <param name="datosNomina"></param>
        /// <param name="periodoPago"></param>
        /// <param name="idUsuario"></param>
        /// <param name="isFiniquito"></param>
        /// <param name="fromModuloProcesar"></param>
        /// <returns></returns>
        private List<NotificationSummary> CrearXmlConSelloYSinTimbre3312(NominaData datosNomina, NOM_PeriodosPago periodoPago, int idUsuario, bool isFiniquito, Cliente itemCliente, bool fromModuloProcesar = true)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            if (datosNomina == null) return null; //se suspende la creacion del cfdi de este registro

            try
            {
                #region VARIABLES

                MSatXml xmlsat = new MSatXml();

                #endregion

                #region GET DATOS EMISOR Y RECEPTOR 

                if (datosNomina.IdEmpresa == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = " Id de la empresa es null", Msg2 = "" });
                    return summaryList; //Si en datos de la nomina no tiene Id de Empresa Emisora - se suspende la creacion del cfdi
                }

                //get datos del emisor
                var empresaObj = new Empresas();

                var emisorDatos = empresaObj.GetEmpresaById((int)datosNomina.IdEmpresa);
                if (emisorDatos == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = (int)datosNomina.IdEmpresa, Msg1 = "No se encontro datos del emisor.", Msg2 = "" });
                    return summaryList; //si no se encontró datos de la empresa emisora del cfdi - se suspende la creacion del cfdi
                }

                //Datos del colaborador
                var datosReceptor = _dao.GetDatosReceptor(datosNomina.IdEmpleado);

                if (datosReceptor == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se encontro datos del receptor.", Msg2 = "" });
                    return summaryList; // si no se encontró datos del empleado - - se suspende la creacion del cfdi
                }

                if (datosReceptor.RFCValidadoSAT != 1)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "El RFC del colaborador no esta validado.", Msg2 = "" });
                  //  return summaryList; //si el rfc no esta validado por el sat
                }

                #endregion

                #region ARCHIVOS CERTIFICADOS PARA GENERAR EL Certificado Digital del cfdi

                var varArchivoCer = "\\" + emisorDatos.IdEmpresa + "\\" + emisorDatos.ArchivoCER;
                var varArchivoKey = "\\" + emisorDatos.IdEmpresa + "\\" + emisorDatos.ArchivoKEY;
                var pathCadenaOriginalXslt = _pathCertificados + "\\" + "cadenaoriginal_3_3.xslt"; //<- cambiar la cadena cadenaoriginal_3_2.xslt por una variable global

                #endregion

                #region GET  -  PERCEPCIONES  -   DEDUCCIONES  -   OTROS PAGOS  - DATOS INDEMNIZACION - INCAPACIDADES

                var listaPercepcionesDeLaNomina = isFiniquito ? _dao.GetPercepcionesByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetPercepcionesByIdNomina(datosNomina.IdNomina);
                var listaDeduccionesDeLaNomina = isFiniquito ? _dao.GetDeduccionesByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetDeduccionesByIdNomina(datosNomina.IdNomina);
                var listaOtrosPagosDeLaNomina = isFiniquito ? _dao.GetOtrosPagosByIdFiniquito(datosNomina.IdFiniquito) : _dao.GetOtrosPagosByIdNomina(datosNomina.IdNomina);
                List<NOM_Incapacidad> listaIncapacidades = isFiniquito ? _dao.GetDatosIncapacidadFiniquitos(datosNomina.IdFiniquito) : _dao.GetDatosIncapacidad(datosNomina.IdNomina);


                var itemIndemnizacion = isFiniquito ? _dao.GetDatosIndemnizacionByIdFiniquito(datosNomina.IdFiniquito) : null;

                if (listaPercepcionesDeLaNomina.Count <= 0 && listaDeduccionesDeLaNomina.Count <= 0 && listaOtrosPagosDeLaNomina.Count <= 0)
                {
                    summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se encontrarón datos para generar el cfdi", Msg2 = "" });
                    return summaryList; // se suspende la creacion del cfdi de este registro de nomina
                }
                #endregion

                #region SET TOTALES COMPLEMENTO Y NOMINA


                #endregion

                #region COMPROBANTE 3.3


                var totalDeduccionN = datosNomina.TotalDeducciones;
                var totalPercepcionN = datosNomina.TotalPercepciones;
                var totalOtrosPagos = datosNomina.TotalOtrosPagos;

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
                    //certificado
                    string archivoCertificado = _pathCertificados + varArchivoCer;
                    string archivoKey = _pathCertificados + varArchivoKey;
                    if (File.Exists(archivoCertificado) && File.Exists(archivoKey) && File.Exists(pathCadenaOriginalXslt))
                    {
                        CertificadoDigital certificado = MCfdi.GetCertificadoDigital(archivoCertificado);

                        comprobante33.Certificado = certificado.CertificadoBase64;
                        comprobante33.NoCertificado = certificado.NoCertificado;


                        //Rutas de los cerficiados
                        _objPath = new PathsCertificado();
                        _objPath.PathArchivoCer = archivoCertificado;
                        _objPath.PathArchivoKey = archivoKey;
                        _objPath.Password = emisorDatos.LlavePrivada;

                    }
                    else
                    {
                        //Enviar mensaje de los certificados
                        summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = "No se contró los certificados .cer .key", Msg2 = "" });
                        return summaryList;
                        //sino se encontró el archivo .cer se suspende la creacion del cfdi <-- QUITAR EL COMENTARIO AL RETURN
                    }
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
                    //ABC 4 - 33
                   //Rfc = datosReceptor.RFC,
                    Rfc = "AAQM610917QJA",
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

                var nomina12 = ComplementoDeNomina12(datosNomina, periodoPago, emisorDatos, datosReceptor, listaPercepcionesDeLaNomina, listaDeduccionesDeLaNomina, listaOtrosPagosDeLaNomina, itemIndemnizacion, itemCliente, listaIncapacidades);



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
                    string sello33 = xmlsat.GenerarSello33(xml33SinSello, pathCadenaOriginalXslt, _objPath, ref cadenaOriginal33);
                    comprobante33.Sello = sello33;
                }

                #endregion

                //Finalmente generamos la estrucutura del xml.
                var xml33Sintimbre = MSatXml.GenerarXmlComprobante33(comprobante33); // ¡Este xml aun no esta timbrado! 

                //Guardamos el xml en la base de datos - para ser timbrado posteriormente
                GuardarXmLenBd(datosNomina, xml33Sintimbre, cadenaOriginal33, idUsuario, isFiniquito, fromModuloProcesar, versionCfdi: 33);

            }
            catch (Exception ex)
            {
                summaryList.Add(new NotificationSummary() { Reg = datosNomina.IdEmpleado, Msg1 = ex.Message, Msg2 = "" });

            }

            return summaryList;
        }

        private Modelos.Nomina12.Nomina ComplementoDeNomina12(NominaData datosNomina, NOM_PeriodosPago periodoPago, Empresa emisorDatos, Empleado datosReceptor, List<Datos.ConceptosNomina> listaPercepcionesDeLaNomina, List<Datos.ConceptosNomina> listaDeduccionesDeLaNomina, List<Datos.ConceptosNomina> listaOtrosPagosDeLaNomina, IndemnizacionData itemIndemnizacion, Cliente itemCliente, List<NOM_Incapacidad> listaIncapacidades)
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
                //ABC 5    -     3.3 1.2
                nominaEmisor.Curp = "CACF880922HJCMSR03"; // Curp = Personas morales no tienen este dato - Comentar esta linea - solo se usa para pruebas con GOYA780416GM0
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

            foreach (var p in listaPercepcionesDeLaNomina)
            {
                #region ARRAY - ITEM PERCEPCIONES

                Modelos.Nomina12.NominaPercepcionesPercepcion itemP = new Modelos.Nomina12.NominaPercepcionesPercepcion
                {
                    TipoPercepcion = p.ClaveSat.PadLeft(3, '0'),
                    Clave = p.IdConcepto.ToString().PadLeft(3, '0'),
                    Concepto = p.NombreConcepto,
                    ImporteGravado = p.Gravado.TruncateDecimal(2),
                    ImporteExento = p.Excento.TruncateDecimal(2)
                    //Horas extras
                    //Acciones O Titulos
                };

                // Total de percepciones brutas (gravadas y exentas) exepto conceptos con las claves: 
                //022 prima antiguedad //023 pagos por separacion //025 Indemnizaciones //039 Jubilaciones, pensiones o haberes de retiro //044 Jubilaciones, pensiones o haberes de retiro en parcialidades
                if (p.ClaveSat == "022" || p.ClaveSat == "023" || p.ClaveSat == "025" || p.ClaveSat == "039" ||
                    p.ClaveSat == "044")
                {
                    totalGravadoPercepcion += p.Gravado.TruncateDecimal(2);
                    totalExcentoPercepcion += p.Excento.TruncateDecimal(2);
                }
                else
                {
                    //Nomina.Percepciones.TotalSueldos = ImporteGravado + ImporteExento 
                    //donde la clave expresada en el atributo TipoPercepcion es distinta de 022 Prima por Antigüedad, 023 Pagos por separación, 025 Indemnizaciones, 039 Jubilaciones, pensiones o haberes de retiro en una exhibición y 044 Jubilaciones, pensiones o haberes de retiro en parcialidades
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
                            Clave = itemOtroPago.ClaveContable,
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
                            Clave = itemOtroPago.ClaveContable,
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
                            Clave = itemOtroPago.ClaveContable,
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe
                        };

                        arrayOtrosPagos.Add(otrosViaticos);

                        break;
                    case 4://Saldo a Favor
                        Modelos.Nomina12.NominaOtroPagoCompensacionSaldosAFavor otroPagoSaldoFavor = new Modelos.Nomina12.NominaOtroPagoCompensacionSaldosAFavor()
                        {
                            SaldoAFavor = itemOtroPago.saldo,
                            Año = (short)(DateTime.Now.Year-1),
                            RemanenteSalFav = itemOtroPago.remanente
                        };

                        Modelos.Nomina12.NominaOtroPago otroPagoSv = new Modelos.Nomina12.NominaOtroPago()
                        {
                            TipoOtroPago = itemOtroPago.ClaveOtroPago.Trim(),
                            Clave = itemOtroPago.ClaveContable,
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
                            Clave = itemOtroPago.ClaveContable,
                            Concepto = itemOtroPago.NombreConcepto,
                            Importe = itemOtroPago.Importe
                        };

                        arrayOtrosPagos.Add(otrosPago);

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

            //if (itemIndemnizacion != null)
            //{
            //    totalActualizadoRecibo += nominaPercepcion.SeparacionIndemnizacion.TotalPagado;
            //}

            //comprobante32.total = totalActualizadoRecibo;

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

            // descuento 
            //cdescuento = datosNomina.TotalDeducciones.TruncateDecimal(2); // suma total de deducciones
            //cdescuento = totalOtrasDeducciones;

            // Nomina.TotalPercepciones  = TotalSueldos + TotalSeparacionIndemnizacion + TotalJubilacionPensionRetiro.
            //ntotalpercepciones = datosNomina.TotalPercepciones.TruncateDecimal(2) + (itemIndemnizacion?.Total.TruncateDecimal(2) ?? 0);// Total sueldo + total indemnizacion + total JubilacionPensionRetiro
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
                //cdescuento += ntotaldeducciones;
                //nomina12.TotalDeducciones = ntotaldeducciones;
                nomina12.TotalDeduccionesSpecified = true;
            }

            if (periodoPago.IdTipoNomina != 16) //Asimilados
            {
                nomina12.TotalOtrosPagos = ntotalotrospagos;
                nomina12.TotalOtrosPagosSpecified = true;//<-- condicionado si hay otros pagos en listaOtrosPagosDeLaNomina
            }


            // TOTALES COMPROBANTE
            //comprobante32.total = ctotal;
            //comprobante32.subTotal = csubtotal;
            //comprobante32.descuento = cdescuento;
            //comprobante32.descuentoSpecified = true;

            //comprobanteConcepto.importe = ccimporte.TruncateDecimal(2);
            //comprobanteConcepto.valorUnitario = ccvalorunitario.TruncateDecimal(2);

            // TOTALES NOMINA 1.2
            nomina12.TotalPercepciones = ntotalpercepciones;
            nomina12.TotalPercepcionesSpecified = true;



            #endregion

            #region GENERAR EL TAG NOMINA Y SE ANEXA AL COMPLEMENTO DEL COMPROBANTE

            ////Genera los tag xml de la nomina 1.2 -  que se anexará al tag de complemento
            //var elementResult = MSatXml.GetXmlElement(nomina12);

            //XmlElement[] arrayXmlElements = new XmlElement[1];

            //arrayXmlElements[0] = elementResult;

            ////Genera Tag complemento de nomina donde agregamos los tag xml de la nomina 1.2
            //Modelos.Cfdi32.ComprobanteComplemento comprobanteComplemento = new Modelos.Cfdi32.ComprobanteComplemento()
            //{
            //    Any = arrayXmlElements
            //};

            //Agrega el tag complemento al xml 3.2
            //comprobante32.Complemento = comprobanteComplemento;

            #endregion

            return nomina12;

        }


        #endregion
    }
}
