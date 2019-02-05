using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nomina.Procesador.Modelos;
using Nomina.Procesador.Metodos;
using Nomina.Procesador.Datos;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using RH.Entidades;
using SYA.BLL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;


namespace Nomina.Procesador
{
    public class PDFCore
    {
        private int[] _NominasSeleccionadas { get; set; }
        private bool _timbradoDePrueba { get; set; }
        private readonly TimbradoDao _dao = new TimbradoDao();
        private readonly NominasDao _nominasDao = new NominasDao();
        readonly GlobalConfig _gconfig = new GlobalConfig();
        private int _idEjercicio;
        private int _idPeriodo;
        private string _rutaCertificados;
        //private string _pathArchivosXml;
        private string _pathUsuario;

        readonly iTextSharp.text.Font _font6 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font6B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font7 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font7B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font8 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font8B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font9 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font10 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


        public PDFCore()
        {

        }

        public Task<int> GenerarPdfAsync(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo)
        {

            var t = Task.Factory.StartNew(() => GenerarReciboTimbrado(nominasSeleccionadas, idEjercicio, idPeriodo));
            //var t = Task.Factory.StartNew(() => GenerarPDF(nominasSeleccionadas, idEjercicio, idPeriodo));
            //  return GenerarPDF(nominasSeleccionadas, idEjercicio, idPeriodo);

            return t;

        }

        public Task<string> GetRecibosSintimbre(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo, int idUsuario, string pathFolder)
        {
            var t = Task.Factory.StartNew(() => GenerarReciboSinTimbre(nominasSeleccionadas, idEjercicio, idPeriodo, idUsuario, pathFolder));

            return t;
        }

        /// <summary>
        /// Metodo principal para generar los recibos PDF
        /// </summary>
        /// <param name="nominasSeleccionadas"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="timbradoDePrueba"></param>
        private int GenerarPDF(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo)
        {
            _NominasSeleccionadas = nominasSeleccionadas;

            _idEjercicio = idEjercicio;
            _idPeriodo = idPeriodo;
            string _lugarExpedicion = "";
            int erroresPdf = 0;
            //Consulta - obtiene los registros 
            var listaXmlTimbrados = _dao.GetRecibosPdf(_NominasSeleccionadas);


            var config = _gconfig.GetGlobalConfig();
            _rutaCertificados = config.PathCertificados;

            var periodoDatos = _dao.GetPeriodoPagoById(idPeriodo);
            var listaMetodosPagos = _dao.GetMetodosPagos();

            //Validamos la lista que no sea null o este vacia
            if (listaXmlTimbrados == null) return 0;
            if (listaXmlTimbrados.Count > 0)
            {
                Random random = new Random();
                int numeroRandom = random.Next();
                DataSet dataSet = new DataSet();
                int anchoTabla = 520;
                //int contador = 0;

                //  Document documento = new Document(PageSize.A4, 50, 50, 25, 25);
                // PdfWriter writer = PdfWriter.GetInstance(documento, new FileStream(nombreArchivoPdf, FileMode.Create));

                foreach (var itemXml in listaXmlTimbrados)
                {
                    try
                    {

                        //Crear PDF - Configuracion del documento
                        Document documento = new Document(PageSize.A4, 50, 50, 25, 25);

                        System.IO.MemoryStream ms = new System.IO.MemoryStream();

                        PdfWriter writer = PdfWriter.GetInstance(documento, ms);

                        documento.Open();

                        //validar que el registro contenga el UUID
                        if (itemXml.FolioFiscalUUID != null)
                        {

                            StringReader streamReader = new StringReader(itemXml.XMLTimbrado);
                            dataSet.ReadXml(streamReader);

                            #region "VARIABLES"
                            string cnx = string.Empty, fechaini = string.Empty, fechafin = string.Empty, curp = string.Empty, depa = string.Empty, puesto = string.Empty, registrosPatronal = string.Empty, fechaInicioLaboral = string.Empty, TipoJornada = string.Empty, sdi = string.Empty, nss = string.Empty;
                            string nombreReceptor = string.Empty, rfcrecep = string.Empty, rfcEmisor = string.Empty, municipioEmisor = string.Empty, estadoEmisor = string.Empty, regimenEmisior = string.Empty, totalComprobante = string.Empty, formaPago = string.Empty, metodoPago = string.Empty, numCertificado = string.Empty, fechaComprobante = string.Empty, serie = string.Empty, folio = string.Empty, fechaTimbrado = "--/--/--", certificadoSat = string.Empty, uuid = string.Empty, selloCFD = string.Empty, selloSAT = string.Empty;
                            string cadenaOriginal = string.Empty;
                            #endregion

                            #region "DATA"

                            //TAG Nomina
                            foreach (DataRow renglon in dataSet.Tables["Nomina"].Rows)
                            {
                                cnx = renglon["NumEmpleado"].ToString();
                                fechaini = Convert.ToString(renglon["FechaInicialPago"]);
                                fechafin = Convert.ToString(renglon["FechaFinalPago"]);
                                curp = Convert.ToString(renglon["CURP"]);
                                depa = Convert.ToString(renglon["Departamento"]);
                                puesto = Convert.ToString(renglon["Puesto"]);
                                registrosPatronal = Convert.ToString(renglon["RegistroPatronal"]);

                                fechaInicioLaboral = Convert.ToString(renglon["FechaInicioRelLaboral"]);
                                TipoJornada = Convert.ToString(renglon["TipoJornada"]);
                                sdi = Convert.ToString(renglon["SalarioDiarioIntegrado"]);
                                nss = Convert.ToString(renglon["NumSeguridadSocial"]);

                            }

                            //TAG Receptor
                            foreach (DataRow recep in dataSet.Tables["Receptor"].Rows)
                            {
                                nombreReceptor = Convert.ToString(recep["nombre"]);
                                rfcrecep = Convert.ToString(recep["rfc"]);

                            }

                            //TAG Emisor
                            string nombreEmisor = "";

                            foreach (DataRow emisor in dataSet.Tables["Emisor"].Rows)
                            {
                                nombreEmisor = Convert.ToString(emisor["nombre"]);
                                rfcEmisor = emisor["rfc"].ToString();
                            }

                            //TAG Domicilio Fiscal
                            foreach (DataRow domi in dataSet.Tables["DomicilioFiscal"].Rows)
                            {
                                municipioEmisor = Convert.ToString(domi["municipio"]);
                                estadoEmisor = Convert.ToString(domi["estado"]);

                            }

                            //TAG Regimen Fiscal
                            foreach (DataRow regimen in dataSet.Tables["RegimenFiscal"].Rows)
                            {
                                regimenEmisior = Convert.ToString(regimen["Regimen"]);

                            }

                            //TAG Comprobante
                            if (dataSet.Tables["Comprobante"] != null)
                                foreach (DataRow renglon in dataSet.Tables["Comprobante"].Rows)
                                {
                                    totalComprobante = Convert.ToString(renglon["total"]);
                                    formaPago = Convert.ToString(renglon["formaDePago"]);
                                    metodoPago = Convert.ToString(renglon["metodoDePago"]);
                                    numCertificado = Convert.ToString(renglon["noCertificado"]);
                                    fechaComprobante = Convert.ToString(renglon["fecha"]);
                                    serie = Convert.ToString(renglon["serie"]);
                                    folio = Convert.ToString(renglon["folio"]);

                                }

                            //TAG Timbre Fiscal
                            foreach (DataRow timbre in dataSet.Tables["TimbreFiscalDigital"].Rows)
                            {
                                fechaTimbrado = Convert.ToString(timbre["FechaTimbrado"]);
                                certificadoSat = Convert.ToString(timbre["noCertificadoSAT"]);
                                uuid = Convert.ToString(timbre["UUID"]);
                                selloCFD = Convert.ToString(timbre["selloCFD"]);
                                selloSAT = Convert.ToString(timbre["selloSAT"]);
                            }

                            #endregion

                            //Nueva pagina
                            documento.NewPage();

                            #region "DISEÑO y CBB"

                            /***** SEC Emisor ********************************************************************/
                            PdfPTable tablaEmisor = new PdfPTable(4)
                            {
                                TotalWidth = anchoTabla,
                                LockedWidth = true

                            };

                            tablaEmisor.DefaultCell.Border = 0;
                            //NOMBRE DE LA EMPRESA EMISORA
                            PdfPCell cell = new PdfPCell(new Phrase(nombreEmisor, _font10))
                            {
                                Colspan = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                HorizontalAlignment = 0,
                                Border = 0
                            };

                            tablaEmisor.AddCell(cell);
                            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });
                            tablaEmisor.AddCell(
                                new PdfPCell(new Phrase("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd"), _font8))
                                {
                                    BackgroundColor = BaseColor.LIGHT_GRAY,
                                    Border = 0
                                });

                            tablaEmisor.AddCell(new PdfPCell(new Phrase("RFC: " + rfcEmisor, _font7B))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });

                            tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Pat: " + registrosPatronal, _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });
                            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });
                            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });

                            tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Fiscal: " + regimenEmisior, _font7))
                            {
                                Colspan = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });
                            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });
                            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });

                            tablaEmisor.AddCell(new PdfPCell(new Phrase("Lugar Expedicion: " + _lugarExpedicion, _font7))
                            {
                                Colspan = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });
                            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });
                            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                Border = 0
                            });

                            documento.Add(tablaEmisor);

                            iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(37, 818, 557, 760)
                            {
                                BackgroundColor = BaseColor.LIGHT_GRAY,
                                BorderWidth = 1,
                                Border = 3,
                                BorderColor = BaseColor.BLACK
                            };

                            documento.Add(new Paragraph(" "));
                            documento.Add(rectangle);

                            /***** SEC Receptor ********************************************************************/
                            PdfPTable tblContendorReceptor = new PdfPTable(2);
                            PdfPTable tblDatosReceptor = new PdfPTable(2);
                            PdfPTable tblDatosReceptorNomina = new PdfPTable(2);

                            tblDatosReceptor.DefaultCell.Border = 0;
                            tblDatosReceptor.AddCell(
                                new PdfPCell(
                                    new Phrase(
                                        cnx + " - " + nombreReceptor,
                                        _font8B))
                                { Colspan = 2, Border = 0 });
                            tblDatosReceptor.AddCell(new Phrase("RFC :", _font7B));
                            tblDatosReceptor.AddCell(new Phrase(rfcrecep, _font7));
                            tblDatosReceptor.AddCell(new Phrase("CURP :", _font7B));
                            tblDatosReceptor.AddCell(new Phrase(curp, _font7));
                            tblDatosReceptor.AddCell(new Phrase("Fecha Inicio Laboral", _font7));
                            // DateTime FI;

                            tblDatosReceptor.AddCell(new Phrase(fechaInicioLaboral, _font7));
                            tblDatosReceptor.AddCell(new Phrase("NSS", _font7));
                            tblDatosReceptor.AddCell(new Phrase(nss, _font7));
                            tblDatosReceptor.AddCell(new Phrase("Tipo Salario", _font7));
                            tblDatosReceptor.AddCell(new Phrase(TipoJornada, _font7));

                            tblDatosReceptorNomina.DefaultCell.Border = 0;
                            tblDatosReceptorNomina.AddCell(new Phrase("Periodo:", _font8B));

                            DateTime FPI;
                            DateTime FPF;

                            FPI = (DateTime)periodoDatos.Fecha_Inicio;
                            FPF = (DateTime)periodoDatos.Fecha_Fin;

                            tblDatosReceptorNomina.AddCell(
                                new Phrase(FPI.ToString("dd/MM/yyyy") + " - " + FPF.ToString("dd/MM/yyyy"), _font8B));
                            tblDatosReceptorNomina.AddCell(new Phrase("Dias de Pago", _font7));
                            tblDatosReceptorNomina.AddCell(new Phrase(periodoDatos.DiasPeriodo.ToString(), _font7));
                            tblDatosReceptorNomina.AddCell(new Phrase("PUESTO", _font7));
                            tblDatosReceptorNomina.AddCell(new Phrase(puesto, _font7));
                            tblDatosReceptorNomina.AddCell(new Phrase("DEPARTAMENTO", _font7));
                            tblDatosReceptorNomina.AddCell(new Phrase(depa, _font7));
                            tblDatosReceptorNomina.AddCell(new Phrase("SDI $:", _font7));
                            tblDatosReceptorNomina.AddCell(new Phrase(sdi, _font7));


                            tblContendorReceptor.TotalWidth = anchoTabla;
                            tblContendorReceptor.LockedWidth = true;

                            tblContendorReceptor.AddCell(tblDatosReceptor);
                            tblContendorReceptor.AddCell(tblDatosReceptorNomina);

                            documento.Add(tblContendorReceptor);

                            var tblTituloPercepciones = new PdfPTable(5); //2

                            var tblPercepciones = new PdfPTable(6);
                            tblPercepciones.DefaultCell.Border = 0;

                            var tblDeducciones = new PdfPTable(4);
                            tblDeducciones.DefaultCell.Border = 0;

                            //titulos para las percepciones
                            tblPercepciones.AddCell(new PdfPCell(new Phrase("Cve\nSAT", _font8B))
                            {
                                Border = 0,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            //  tblPercepciones.AddCell(new PdfPCell(new Phrase("No", _font8B)) { Border = 0, BackgroundColor = BaseColor.LIGHT_GRAY });
                            tblPercepciones.AddCell(new PdfPCell(new Phrase("Concepto", _font8B))
                            {
                                Border = 0,
                                Colspan = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            tblPercepciones.AddCell(new PdfPCell(new Phrase("Gravado", _font8B))
                            {
                                Border = 0,
                                HorizontalAlignment = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            tblPercepciones.AddCell(new PdfPCell(new Phrase("Exento", _font8B))
                            {
                                Border = 0,
                                HorizontalAlignment = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            tblPercepciones.AddCell(new PdfPCell(new Phrase("Total ", _font8B))
                            {
                                Border = 0,
                                HorizontalAlignment = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });

                            //titulos para las deducciones
                            tblDeducciones.AddCell(new PdfPCell(new Phrase("Cve\nSAT", _font8B))
                            {
                                Border = 0,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });

                            // tblDeducciones.AddCell(new PdfPCell(new Phrase("No", _font8B)){Border = 0,BackgroundColor = BaseColor.LIGHT_GRAY});

                            tblDeducciones.AddCell(new PdfPCell(new Phrase("Concepto", _font8B))
                            {
                                Border = 0,
                                Colspan = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            tblDeducciones.AddCell(new PdfPCell(new Phrase("Total ", _font8B))
                            {
                                Border = 0,
                                HorizontalAlignment = 2,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });


                            documento.Add(new Paragraph(" "));

                            tblTituloPercepciones.AddCell(new PdfPCell(new Phrase("Percepciones", _font8B))
                            {
                                Colspan = 3,
                                HorizontalAlignment = 1,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            tblTituloPercepciones.AddCell(new PdfPCell(new Phrase("Deducciones", _font8B))
                            {
                                Colspan = 2,
                                HorizontalAlignment = 1,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });

                            tblTituloPercepciones.TotalWidth = anchoTabla;
                            tblTituloPercepciones.LockedWidth = true;


                            //PERCEPCIONES
                            // var totalPercepcion = "";
                            var totalPercepcionGravado = 0.00;
                            var totalPercepcionExcento = 0.00;
                            var totalPercepcionDecimal = 0.00;

                            //Totales
                            if (dataSet.Tables["Percepciones"] != null)
                                foreach (DataRow perceps in dataSet.Tables["Percepciones"].Rows)
                                {
                                    totalPercepcionExcento = double.Parse(perceps[1].ToString());
                                    totalPercepcionGravado = double.Parse(perceps[2].ToString());
                                }

                            //Detalle


                            if (dataSet.Tables["Percepcion"] != null)
                                foreach (DataRow percep in dataSet.Tables["Percepcion"].Rows)
                                {
                                    var importeExcento = double.Parse(percep[0].ToString());
                                    var importeGravado = double.Parse(percep[1].ToString());
                                    var concepto = Convert.ToString(percep[2]);
                                    var claveSat = Convert.ToString(percep[4]);
                                    var tipo = Convert.ToString(percep[4]);

                                    var totalImporte = importeExcento + importeGravado;
                                    totalPercepcionDecimal += totalImporte;



                                    tblPercepciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                                    {
                                        Border = 0
                                    });
                                    // tblPercepciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });

                                    tblPercepciones.AddCell(new PdfPCell(new Phrase(concepto, _font7)) { Border = 0, Colspan = 2 });
                                    tblPercepciones.AddCell(new PdfPCell(new Phrase($"{importeGravado:0,0.##}", _font7))
                                    {
                                        Border = 0,
                                        HorizontalAlignment = 2
                                    });
                                    tblPercepciones.AddCell(new PdfPCell(new Phrase($"{importeExcento:0,0.##}", _font7))
                                    {
                                        Border = 0,
                                        HorizontalAlignment = 2
                                    });
                                    tblPercepciones.AddCell(new PdfPCell(new Phrase($"{totalImporte:0,0.##}", _font7))
                                    {
                                        Border = 0,
                                        HorizontalAlignment = 2
                                    });

                                }


                            /***** SEC Deducciones ********************************************************************/
                            var totalDeducciones = 0.0;
                            var totalDeduccionesExcento = 0.0;
                            var totalDeduccionesGravado = 0.0;

                            //Totales
                            if (dataSet.Tables["Deducciones"] != null)
                                foreach (DataRow perceps in dataSet.Tables["Deducciones"].Rows)
                                {
                                    totalDeduccionesExcento = double.Parse(perceps[1].ToString());
                                    totalDeduccionesGravado = double.Parse(perceps[2].ToString());
                                }

                            totalDeducciones = totalDeduccionesExcento + totalDeduccionesGravado;

                            if (dataSet.Tables["Deduccion"] != null)
                                foreach (DataRow deduc in dataSet.Tables["Deduccion"].Rows)
                                {
                                    var importeExcento = double.Parse(deduc[0].ToString());
                                    var importeGravado = double.Parse(deduc[1].ToString());
                                    var concepto = Convert.ToString(deduc[2]);
                                    var claveSat = Convert.ToString(deduc[4]);
                                    var tipo = Convert.ToString(deduc[4]);

                                    var totalImporte = importeExcento + importeGravado;



                                    tblDeducciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                                    {
                                        Border = 0
                                    });
                                    //tblDeducciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });
                                    tblDeducciones.AddCell(new PdfPCell(new Phrase(concepto, _font7)) { Border = 0, Colspan = 2 });
                                    tblDeducciones.AddCell(new PdfPCell(new Phrase($"{totalImporte:0,0.##}", _font7))
                                    {
                                        Border = 0,
                                        HorizontalAlignment = 2
                                    });

                                }


                            //Agrega las tablas a la tabla contenedora
                            tblTituloPercepciones.AddCell(new PdfPCell(tblPercepciones) { Colspan = 3 });
                            tblTituloPercepciones.AddCell(new PdfPCell(tblDeducciones) { Colspan = 2 });
                            documento.Add(tblTituloPercepciones);

                            /***** SEC TOTALES ********************************************************************/

                            var tblTotales = new PdfPTable(5);

                            tblTotales.TotalWidth = anchoTabla;
                            tblTotales.LockedWidth = true;

                            var tblTotalPercepcion = new PdfPTable(6);
                            var tblTotalDeduccion = new PdfPTable(1);

                            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase("Suma de Percepciones $", _font8B))
                            {
                                Colspan = 3,
                                Border = 0
                            });
                            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase($"{totalPercepcionGravado:0,0.##}", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase($"{totalPercepcionExcento:0,0.##}", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase($"{totalPercepcionDecimal:0,0.##}", _font8B))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });



                            tblTotalDeduccion.AddCell(
                                new PdfPCell(new Phrase("Suma de Deducciones $    " + $"{totalDeducciones:0,0.##}", _font8B))
                                {
                                    Border = 0,
                                    HorizontalAlignment = 2
                                });
                            // tblTotalDeduccion.AddCell(new PdfPCell(new Phrase("Neto del recibo $" + (totalPercepcion - totalDeducciones), _font8B)){Border = 0});



                            tblTotales.AddCell(new PdfPCell(tblTotalPercepcion) { Colspan = 3, HorizontalAlignment = 2 });
                            tblTotales.AddCell(new PdfPCell(tblTotalDeduccion) { Colspan = 2, HorizontalAlignment = 2 });
                            documento.Add(tblTotales);

                            /***** SEC Neto del Recibo ********************************************************************/
                            var tblNetoRecibo = new PdfPTable(5);
                            tblNetoRecibo.TotalWidth = anchoTabla;
                            tblNetoRecibo.LockedWidth = true;



                            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0 });
                            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0 });
                            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0 });

                            var totalNetoRecibo = 0.0;
                            totalNetoRecibo = Double.Parse(totalComprobante);

                            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("Neto del recibo $ " + $"{totalNetoRecibo:0,0.##}", _font8B))
                            {
                                Border = 1,
                                Colspan = 2
                            });

                            documento.Add(tblNetoRecibo);
                            documento.Add(new Paragraph(" "));


                            /***** SEC Generar Codigo CBB ********************************************************************/

                            string textCode = @"?re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&id" + uuid;

                            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                            QrCode qrCode = new QrCode();
                            qrEncoder.TryEncode(textCode, out qrCode);
                            GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(150, QuietZoneModules.Four), Brushes.Black, Brushes.White);

                            //new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Two)).Draw(g, qrCode.Matrix);

                            Stream memoryStream = new MemoryStream();






                            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
                            memoryStream.Position = 0;
                            //System.Drawing.Image img = (System.Drawing.Image)renderer;




                            //qCBB.Text = @"?re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&id" +
                            //           uuid;

                            //System.Drawing.Image img = (System.Drawing.Image)qCBB.Image.Clone();

                            //if (img != null)
                            //{
                            //    img.Save(_pathArchivosXml + @"\\ImagenesCBB\\" + rfcrecep + ".jpg");
                            //}
                            //img.Dispose();




                            /***** SEC Firma Empleado ********************************************************************/
                            var tblFirma = new PdfPTable(2);
                            tblFirma.AddCell(
                                new PdfPCell(
                                    new Phrase(
                                        "Recibi de " + registrosPatronal +
                                        " por concepto de pago total de mi salario y demas percepciones del periodo" +
                                        " indicado sin que a la fecha se me adeude cantidad alguna por ningun concepto.",
                                        _font6))
                                {
                                    Border = 0
                                });
                            tblFirma.AddCell(
                                new PdfPCell(new Phrase("_______________________________\n Firma del empleado", _font7))
                                {
                                    Border = 0,
                                    HorizontalAlignment = 1
                                });


                            documento.Add(tblFirma);


                            documento.Add(new Paragraph(" "));

                            /***** SEC LOGO CBB  ********************************************************************/
                            var tblCBB = new PdfPTable(3);
                            var tblCfdidatos = new PdfPTable(2);
                            tblCBB.TotalWidth = anchoTabla;
                            tblCBB.LockedWidth = true;
                            tblCBB.DefaultCell.Border = 0;


                            iTextSharp.text.Image logoCBB;
                            // logoCBB = iTextSharp.text.Image.GetInstance(_pathArchivosXml + @"\\ImagenesCBB\\" + rfcrecep + ".jpg");

                            logoCBB = iTextSharp.text.Image.GetInstance(memoryStream);

                            tblCBB.AddCell(new PdfPCell(logoCBB) { Border = 0 });



                            tblCfdidatos.AddCell(
                                new PdfPCell(new Phrase("Este documento es una representacion impresa de un CFDI", _font7B))
                                {
                                    Colspan = 2,
                                    BackgroundColor = BaseColor.LIGHT_GRAY
                                });
                            var tbldatos = new PdfPTable(1);
                            tbldatos.AddCell(new PdfPCell(new Phrase("Serie del Certificado del Emisor:", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tbldatos.AddCell(new PdfPCell(new Phrase("Folio Fiscal UUID:", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tbldatos.AddCell(new PdfPCell(new Phrase("No certificado del SAT:", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tbldatos.AddCell(new PdfPCell(new Phrase("Fecha y Hora de certificacion:", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tbldatos.AddCell(new PdfPCell(new Phrase("Método de pago:", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });



                            var tblcontenido = new PdfPTable(1);
                            tblcontenido.AddCell(new PdfPCell(new Phrase("Serie Certificado del emisor", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 0
                            });
                            tblcontenido.AddCell(new PdfPCell(new Phrase(uuid, _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 0
                            });
                            tblcontenido.AddCell(new PdfPCell(new Phrase("No Certificado Sat", _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 0
                            });
                            tblcontenido.AddCell(new PdfPCell(new Phrase(fechaTimbrado, _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 0
                            });


                            /***** SEC Metodo Pago ********************************************************************/
                            var mp = listaMetodosPagos.FirstOrDefault(x => x.Clave == metodoPago);
                            var metodoPdf = metodoPago + " - " + mp.Descripcion;

                            tblcontenido.AddCell(new PdfPCell(new Phrase(metodoPdf, _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 0
                            });


                            tblCfdidatos.AddCell(tbldatos);
                            tblCfdidatos.AddCell(tblcontenido);


                            tblCBB.AddCell(new PdfPCell(tblCfdidatos) { Colspan = 2 });

                            documento.Add(tblCBB);

                            documento.Add(new Paragraph(" "));

                            /***** SEC SELLOS Y CADENA DEL SAT ********************************************************************/
                            var tblSELLOS = new PdfPTable(1);
                            tblSELLOS.TotalWidth = anchoTabla;
                            tblSELLOS.LockedWidth = true;

                            tblSELLOS.AddCell(new PdfPCell(new Phrase("Sello Digital del CFDI", _font7B))
                            {
                                Border = 0,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            tblSELLOS.AddCell(new PdfPCell(new Phrase(selloCFD, _font7)) { Border = 0 });
                            tblSELLOS.AddCell(new PdfPCell(new Phrase("    ", _font7B)) { Border = 0 });

                            tblSELLOS.AddCell(new PdfPCell(new Phrase("Sello del SAT", _font7B))
                            {
                                Border = 0,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            });
                            tblSELLOS.AddCell(new PdfPCell(new Phrase(selloSAT, _font7)) { Border = 0 });
                            tblSELLOS.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0 });

                            tblSELLOS.AddCell(
                                new PdfPCell(new Phrase("Cadena Original del complemento del certificado del SAT", _font7B))
                                {
                                    Border = 0,
                                    BackgroundColor = BaseColor.LIGHT_GRAY
                                });
                            tblSELLOS.AddCell(new PdfPCell(new Phrase(itemXml.CadenaOriginal, _font7)) { Border = 0 });


                            documento.Add(tblSELLOS);
                            #endregion

                            dataSet.Dispose();
                            dataSet.Clear();

                        }


                        //Cerrar el documento
                        documento.Close();

                        //Guardar el pdf en bd
                        _dao.GuardarPDF(itemXml.IdTimbrado, ms.ToArray());

                    }
                    catch (Exception)
                    {
                        erroresPdf++;
                    }

                }//final del for

            }


            return erroresPdf;
        }

        private int GenerarReciboTimbrado(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo)
        {
            int erroresPdf = 0;
            _NominasSeleccionadas = nominasSeleccionadas;

            // GET - la lista de XmlTimbrados 
            var listaXmlTimbrados = _dao.GetRecibosPdf(nominasSeleccionadas);

            // GET - path Certificados
            var config = _gconfig.GetGlobalConfig();
            _rutaCertificados = config.PathCertificados;

            // GET - Datos del periodo y Metodos de pago
            var periodoDatos = _dao.GetPeriodoPagoById(idPeriodo);
            var listaMetodosPagos = _dao.GetMetodosPagos();

            //Validamos la lista que no sea null o este vacia
            if (listaXmlTimbrados == null) return 0;
            if (listaXmlTimbrados.Count > 0)
            {
                //Se generar un pdf por cada xml
                foreach (var itemXml in listaXmlTimbrados)
                {
                    try
                    {
                        //Crear PDF - Configuracion del documento
                        Document documento = new Document(PageSize.A4, 50, 50, 25, 25);

                        System.IO.MemoryStream ms = new System.IO.MemoryStream();

                        PdfWriter writer = PdfWriter.GetInstance(documento, ms);

                        documento.Open();

                        //validar que el registro contenga el UUID
                        if (itemXml.FolioFiscalUUID == null) //<- corregir la validacion a que sea != null
                        {
                            AddReciboPdf(ref documento, itemXml.XMLSinTimbre, periodoDatos, listaMetodosPagos, itemXml.CadenaOriginal, false);//<- quitar esta linea que es de prueba
                            //Descomentar esta Linea, es la correcta -- > AddReciboPdf(ref documento, itemXml.XMLTimbrado, periodoDatos, listaMetodosPagos, itemXml.CadenaOriginal, true);
                        }

                        //Cerrar el documento
                        documento.Close();

                        //Guardar el pdf en bd
                        _dao.GuardarPDF(itemXml.IdTimbrado, ms.ToArray());

                    }
                    catch (Exception ex)
                    {
                        erroresPdf++;
                    }
                }


            }

            return erroresPdf;
        }

        /// <summary>
        /// Genera un archivo pdf y retorna el path para acceder al archivo generado
        /// </summary>
        /// <param name="nominasSeleccionadas"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="pathFolder"></param>
        /// <returns></returns>
        private string GenerarReciboSinTimbre(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo, int idUsuario, string pathFolder)
        {
            var pathUsuario = ValidarFolderUsuario(idUsuario, pathFolder);
            var listaNominas = _nominasDao.GetNominasById(nominasSeleccionadas);

            // GET - Datos del periodo y Metodos de pago
            var periodoDatos = _dao.GetPeriodoPagoById(idPeriodo);
            var listaMetodosPagos = _dao.GetMetodosPagos();

            Document doc = new Document(PageSize.A4, 50, 50, 25, 25);
            Random r = new Random();
            int aleatorio = r.Next();
            int cont = 0;

            var archivoPdf = pathUsuario + "Recibos_" + aleatorio + ".pdf";

            PdfWriter w = PdfWriter.GetInstance(doc, new FileStream(archivoPdf, FileMode.Create));

            doc.Open();

            //Se crea un solo archivo pdf con varias hojas
            foreach (var itemNomina in listaNominas)
            {
                if (itemNomina.XMLSinTimbre != null)
                {
                    AddReciboPdf(ref doc, itemNomina.XMLSinTimbre, periodoDatos, listaMetodosPagos);
                    cont++;
                }
            }

            if (cont == 0)
            {
                doc.Add(new Paragraph("No se encontró datos para generar el recibo"));
            }

            doc.Close();

            return archivoPdf;
        }

        /// <summary>
        /// Agrega una página de recibo al documento
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="strXml"></param>
        /// <param name="periodoDatos"></param>
        /// <param name="listaMetodosPagos"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="conTimbrado"></param>
        private void AddReciboPdf(ref Document documento, string strXml, NOM_PeriodosPago periodoDatos, List<C_Metodos_Pago> listaMetodosPagos, string cadenaOriginal = "cadena original", bool conTimbrado = false)
        {
            DataSet dataSet = new DataSet();
            int anchoTabla = 520;
            string _lugarExpedicion = "";

            StringReader streamReader = new StringReader(strXml);
            dataSet.ReadXml(streamReader);

            #region "VARIABLES"

            string cnx = string.Empty, fechaini = string.Empty, fechafin = string.Empty, curp = string.Empty, depa = string.Empty, puesto = string.Empty, registrosPatronal = string.Empty, fechaInicioLaboral = string.Empty, TipoJornada = string.Empty,
                sdi = string.Empty, nss = string.Empty;

            string nombreReceptor = string.Empty, rfcrecep = string.Empty, rfcEmisor = string.Empty, municipioEmisor = string.Empty, estadoEmisor = string.Empty, regimenEmisior = string.Empty, totalComprobante = string.Empty, formaPago = string.Empty,
                metodoPago = string.Empty, numCertificadoEmisor = "--", fechaComprobante = "--", serie = "--", folio = string.Empty, fechaTimbrado = "--/--/----", certificadoSat = "--", uuid = "--", selloCFD = string.Empty, selloSAT = string.Empty;


            //  string cadenaOriginal = string.Empty;
            #endregion

            #region DATASET - DATATABLES

            DataTable dtTagNomina = new DataTable();
            DataTable dtTagReceptor = new DataTable();
            DataTable dtTagEmisor = new DataTable();

            DataTable dtTagRegimenFiscal = new DataTable();
            DataTable dtTagConceptos = new DataTable();
            DataTable dtTagconcepto = new DataTable();
            DataTable dtTagComplemento = new DataTable();
            DataTable dtTagComprobante = new DataTable();

            DataTable dtTagEmisorNomina = new DataTable();
            DataTable dtTagReceptorNomina = new DataTable();
            DataTable dtTagPercepciones = new DataTable();
            DataTable dtTagPercepcion = new DataTable();

            DataTable dtTagDeducciones = new DataTable();
            DataTable dtTagDeduccion = new DataTable();

            DataTable dtTagTimbreDigital = new DataTable();

            

            if (dataSet.Tables.Contains("Nomina"))
            dtTagNomina  = dataSet.Tables["Nomina"];

            if (dataSet.Tables.Contains("TimbreFiscalDigital"))
                dtTagNomina = dataSet.Tables["TimbreFiscalDigital"];

            if (dataSet.Tables.Contains("Comprobante"))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains("Emisor")) //Emisor dentro del comprobante
                dtTag = dataSet.Tables[""];

      

            if (dataSet.Tables.Contains("RegimenFiscal"))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];

            if (dataSet.Tables.Contains(""))
                dtTag = dataSet.Tables[""];


            #endregion


            #region "SET VARIABLES =  DATOS NOMINA - EMISOR - RECEPTOR - COMPROBANTE"

            #region TAG NOMINA

            var tagNomina = conTimbrado == true ? 8 : 6;
            foreach (DataRow renglon in dtTagNomina.Rows)
            {
                fechaini = Convert.ToString(renglon["FechaInicialPago"]);
                fechafin = Convert.ToString(renglon["FechaFinalPago"]);
            }

            #endregion

            #region TAG RECEPTOR

            var tagComprobante_Receptor = conTimbrado == true ? 3 : 2;
            foreach (DataRow recep in dtTagComprobante.Rows)
            {
                nombreReceptor = Convert.ToString(recep["nombre"]);
                rfcrecep = Convert.ToString(recep["rfc"]);
            }




            //TAG Receptor dentro del tag Nomina
            var tagNomina_Receptor = conTimbrado == true ? 3 : 8;
            foreach (DataRow recep in dtTagReceptorNomina.Rows)
            {
                cnx = recep["NumEmpleado"].ToString();
                curp = Convert.ToString(recep["CURP"]);
                depa = Convert.ToString(recep["Departamento"]);
                puesto = Convert.ToString(recep["Puesto"]);
                fechaInicioLaboral = Convert.ToString(recep["FechaInicioRelLaboral"]);
                TipoJornada = Convert.ToString(recep["TipoJornada"]);
                sdi = Convert.ToString(recep["SalarioDiarioIntegrado"]);
                nss = Convert.ToString(recep["NumSeguridadSocial"]);
            }
            #endregion

            #region TAG EMISOR

            string nombreEmisor = "";
            var tagComprobante_Emisor = conTimbrado == true ? 2 : 1;
            foreach (DataRow emisor in dtTagEmisor.Rows)
            {
                nombreEmisor = Convert.ToString(emisor["nombre"]);
                rfcEmisor = emisor["rfc"].ToString();
            }

            //Segundo Tag Emisor
            var tagNominaEmisor = conTimbrado == true ? 5 : 7;
            foreach (DataRow emisor in dtTagEmisorNomina.Rows)
            {
                registrosPatronal = Convert.ToString(emisor["RegistroPatronal"]);
            }

            #endregion


            //TAG Domicilio Fiscal
            //foreach (DataRow domi in dataSet.Tables["DomicilioFiscal"].Rows)
            //{
            //    municipioEmisor = Convert.ToString(domi["municipio"]);
            //    estadoEmisor = Convert.ToString(domi["estado"]);

            //}

            //TAG Regimen Fiscal
            var tagRegimenFiscal = conTimbrado == true ? 5 : 2;
            foreach (DataRow regimen in dtTagRegimenFiscal.Rows)
            {
                //regimenEmisior = Convert.ToString(regimen["Regimen"]);

            }

            #endregion

            #region SET VARIABLES =  DATOS COMPROBANTE - TIMBRE FISCAL DIGITAL

            //TAG Comprobante
            if (dataSet.Tables["Comprobante"] != null)
                foreach (DataRow renglon in dtTagComprobante.Rows)
                {
                    totalComprobante = Convert.ToString(renglon["total"]);
                    formaPago = Convert.ToString(renglon["formaDePago"]);
                    metodoPago = Convert.ToString(renglon["metodoDePago"]);
                    numCertificadoEmisor = "noCertificado?"; //Convert.ToString(renglon["noCertificado"]);
                    fechaComprobante = Convert.ToString(renglon["fecha"]);
                    serie = Convert.ToString(renglon["serie"]);
                    folio = Convert.ToString(renglon["folio"]);
                    _lugarExpedicion = Convert.ToString(renglon["LugarExpedicion"]);

                }

            //TAG Timbre Fiscal
            if (dataSet.Tables["TimbreFiscalDigital"] != null)
                foreach (DataRow timbre in dtTagTimbreDigital.Rows)
                {
                    fechaTimbrado = Convert.ToString(timbre["FechaTimbrado"]);
                    certificadoSat = Convert.ToString(timbre["noCertificadoSAT"]);
                    uuid = Convert.ToString(timbre["UUID"]);
                    selloCFD = Convert.ToString(timbre["selloCFD"]);
                    selloSAT = Convert.ToString(timbre["selloSAT"]);
                }

            #endregion

            #region "DISEÑO y CBB"
            #region DATOS DEL EMISOR

            PdfPTable tablaEmisor = new PdfPTable(4)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true

            };

            tablaEmisor.DefaultCell.Border = 0;
            //NOMBRE DE LA EMPRESA EMISORA
            PdfPCell cell = new PdfPCell(new Phrase(nombreEmisor, _font10))
            {
                Colspan = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                HorizontalAlignment = 0,
                Border = 0
            };

            tablaEmisor.AddCell(cell);
            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });
            tablaEmisor.AddCell(
                new PdfPCell(new Phrase("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd"), _font8))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Border = 0
                });

            tablaEmisor.AddCell(new PdfPCell(new Phrase("RFC: " + rfcEmisor, _font7B))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });

            tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Pat: " + registrosPatronal, _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });
            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });
            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });

            tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Fiscal: " + regimenEmisior, _font7))
            {
                Colspan = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });
            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });
            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });

            tablaEmisor.AddCell(new PdfPCell(new Phrase("Lugar Expedicion: " + _lugarExpedicion, _font7))
            {
                Colspan = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });
            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });
            tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font7))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0
            });

            #endregion

            documento.Add(tablaEmisor);

            iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(37, 818, 557, 760)
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                BorderWidth = 1,
                Border = 3,
                BorderColor = BaseColor.BLACK
            };

            documento.Add(new Paragraph(" "));
            documento.Add(rectangle);

            /***** SEC Receptor ********************************************************************/

            #region DATOS DEL RECEPTOR

            PdfPTable tblContendorReceptor = new PdfPTable(2);
            PdfPTable tblDatosReceptor = new PdfPTable(2);
            PdfPTable tblDatosReceptorNomina = new PdfPTable(2);

            tblDatosReceptor.DefaultCell.Border = 0;
            tblDatosReceptor.AddCell(
                new PdfPCell(
                    new Phrase(
                        cnx + " - " + nombreReceptor,
                        _font8B))
                { Colspan = 2, Border = 0 });
            tblDatosReceptor.AddCell(new Phrase("RFC :", _font7B));
            tblDatosReceptor.AddCell(new Phrase(rfcrecep, _font7));
            tblDatosReceptor.AddCell(new Phrase("CURP :", _font7B));
            tblDatosReceptor.AddCell(new Phrase(curp, _font7));
            tblDatosReceptor.AddCell(new Phrase("Fecha Inicio Laboral", _font7));
            // DateTime FI;

            tblDatosReceptor.AddCell(new Phrase(fechaInicioLaboral, _font7));
            tblDatosReceptor.AddCell(new Phrase("NSS", _font7));
            tblDatosReceptor.AddCell(new Phrase(nss, _font7));
            tblDatosReceptor.AddCell(new Phrase("Tipo Jornada", _font7));
            tblDatosReceptor.AddCell(new Phrase(TipoJornada, _font7));

            tblDatosReceptorNomina.DefaultCell.Border = 0;
            tblDatosReceptorNomina.AddCell(new Phrase("Periodo:", _font8B));

            DateTime FPI;
            DateTime FPF;

            FPI = (DateTime)periodoDatos.Fecha_Inicio;
            FPF = (DateTime)periodoDatos.Fecha_Fin;

            tblDatosReceptorNomina.AddCell(
                new Phrase(FPI.ToString("dd/MM/yyyy") + " - " + FPF.ToString("dd/MM/yyyy"), _font8B));
            tblDatosReceptorNomina.AddCell(new Phrase("Dias de Pago", _font7));
            tblDatosReceptorNomina.AddCell(new Phrase(periodoDatos.DiasPeriodo.ToString(), _font7));
            tblDatosReceptorNomina.AddCell(new Phrase("PUESTO", _font7));
            tblDatosReceptorNomina.AddCell(new Phrase(puesto, _font7));
            tblDatosReceptorNomina.AddCell(new Phrase("DEPARTAMENTO", _font7));
            tblDatosReceptorNomina.AddCell(new Phrase(depa, _font7));
            tblDatosReceptorNomina.AddCell(new Phrase("SDI $:", _font7));
            tblDatosReceptorNomina.AddCell(new Phrase(sdi, _font7));


            tblContendorReceptor.TotalWidth = anchoTabla;
            tblContendorReceptor.LockedWidth = true;

            tblContendorReceptor.AddCell(tblDatosReceptor);
            tblContendorReceptor.AddCell(tblDatosReceptorNomina);

            #endregion

            #region TITULOS DE LA TABLA DE PERCEPCIONES Y DEDUCCIONES

            documento.Add(tblContendorReceptor);

            var tblTituloPercepciones = new PdfPTable(5); //2

            var tblPercepciones = new PdfPTable(6);
            tblPercepciones.DefaultCell.Border = 0;

            var tblDeducciones = new PdfPTable(4);
            tblDeducciones.DefaultCell.Border = 0;

            //titulos para las percepciones
            tblPercepciones.AddCell(new PdfPCell(new Phrase("Cve\nSAT", _font8B))
            {
                Border = 0,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            //  tblPercepciones.AddCell(new PdfPCell(new Phrase("No", _font8B)) { Border = 0, BackgroundColor = BaseColor.LIGHT_GRAY });
            tblPercepciones.AddCell(new PdfPCell(new Phrase("Concepto", _font8B))
            {
                Border = 0,
                Colspan = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            tblPercepciones.AddCell(new PdfPCell(new Phrase("Gravado", _font8B))
            {
                Border = 0,
                HorizontalAlignment = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            tblPercepciones.AddCell(new PdfPCell(new Phrase("Exento", _font8B))
            {
                Border = 0,
                HorizontalAlignment = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            tblPercepciones.AddCell(new PdfPCell(new Phrase("Total ", _font8B))
            {
                Border = 0,
                HorizontalAlignment = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            //titulos para las deducciones
            tblDeducciones.AddCell(new PdfPCell(new Phrase("Cve\nSAT", _font8B))
            {
                Border = 0,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            // tblDeducciones.AddCell(new PdfPCell(new Phrase("No", _font8B)){Border = 0,BackgroundColor = BaseColor.LIGHT_GRAY});

            tblDeducciones.AddCell(new PdfPCell(new Phrase("Concepto", _font8B))
            {
                Border = 0,
                Colspan = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            tblDeducciones.AddCell(new PdfPCell(new Phrase("Total ", _font8B))
            {
                Border = 0,
                HorizontalAlignment = 2,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });


            documento.Add(new Paragraph(" "));

            tblTituloPercepciones.AddCell(new PdfPCell(new Phrase("Percepciones", _font8B))
            {
                Colspan = 3,
                HorizontalAlignment = 1,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            tblTituloPercepciones.AddCell(new PdfPCell(new Phrase("Deducciones", _font8B))
            {
                Colspan = 2,
                HorizontalAlignment = 1,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            tblTituloPercepciones.TotalWidth = anchoTabla;
            tblTituloPercepciones.LockedWidth = true;

            #endregion

            #region  CONCEPTOS DE PERCEPCIONES

            var totalPercepcionGravado = 0.00;
            var totalPercepcionExcento = 0.00;
            var totalPercepcionDecimal = 0.00;

            //Totales
            if (dataSet.Tables["Percepciones"] != null)
                foreach (DataRow perceps in dtTagPercepciones.Rows)
                {
                    totalPercepcionExcento = double.Parse(perceps[1].ToString());
                    totalPercepcionGravado = double.Parse(perceps[2].ToString());
                }

            //Detalle
            if (dataSet.Tables["Percepcion"] != null)
                foreach (DataRow percep in dtTagPercepcion.Rows)
                {
                    var importeExcento = double.Parse(percep[0].ToString());
                    var importeGravado = double.Parse(percep[1].ToString());
                    var concepto = Convert.ToString(percep[2]);
                    var claveSat = Convert.ToString(percep[4]);
                    var tipo = Convert.ToString(percep[4]);

                    var totalImporte = importeExcento + importeGravado;
                    totalPercepcionDecimal += totalImporte;

                    tblPercepciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                    {
                        Border = 0
                    });
                    // tblPercepciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });

                    tblPercepciones.AddCell(new PdfPCell(new Phrase(concepto, _font7)) { Border = 0, Colspan = 2 });
                    tblPercepciones.AddCell(new PdfPCell(new Phrase($"{importeGravado:0,0.##}", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 2
                    });
                    tblPercepciones.AddCell(new PdfPCell(new Phrase($"{importeExcento:0,0.##}", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 2
                    });
                    tblPercepciones.AddCell(new PdfPCell(new Phrase($"{totalImporte:0,0.##}", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 2
                    });

                }


            #endregion

            #region CONCEPTOS DE DEDUCCIONES
            var totalDeducciones = 0.0;
            var totalDeduccionesExcento = 0.0;
            var totalDeduccionesGravado = 0.0;

            //Totales
            if (dataSet.Tables["Deducciones"] != null)
                foreach (DataRow perceps in dtTagDeducciones.Rows)
                {
                    totalDeduccionesExcento = double.Parse(perceps[1].ToString());
                    totalDeduccionesGravado = double.Parse(perceps[2].ToString());
                }

            totalDeducciones = totalDeduccionesExcento + totalDeduccionesGravado;

            if (dataSet.Tables["Deduccion"] != null)
                foreach (DataRow deduc in dtTagDeduccion.Rows)
                {
                    var importeExcento = double.Parse(deduc[0].ToString());
                    var importeGravado = double.Parse(deduc[1].ToString());
                    var concepto = Convert.ToString(deduc[2]);
                    var claveSat = Convert.ToString(deduc[4]);
                    var tipo = Convert.ToString(deduc[4]);

                    var totalImporte = importeExcento + importeGravado;



                    tblDeducciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                    {
                        Border = 0
                    });
                    //tblDeducciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });
                    tblDeducciones.AddCell(new PdfPCell(new Phrase(concepto, _font7)) { Border = 0, Colspan = 2 });
                    tblDeducciones.AddCell(new PdfPCell(new Phrase($"{totalImporte:0,0.##}", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 2
                    });

                }


            //Agrega las tablas a la tabla contenedora
            tblTituloPercepciones.AddCell(new PdfPCell(tblPercepciones) { Colspan = 3 });
            tblTituloPercepciones.AddCell(new PdfPCell(tblDeducciones) { Colspan = 2 });
            documento.Add(tblTituloPercepciones);
            #endregion

            #region TOTALES  PERCEPCION Y DEDUCCION

            var tblTotales = new PdfPTable(5);

            tblTotales.TotalWidth = anchoTabla;
            tblTotales.LockedWidth = true;

            var tblTotalPercepcion = new PdfPTable(6);
            var tblTotalDeduccion = new PdfPTable(1);

            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase("Suma de Percepciones $", _font8B))
            {
                Colspan = 3,
                Border = 0
            });
            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase($"{totalPercepcionGravado:0,0.##}", _font7))
            {
                Border = 0,
                HorizontalAlignment = 2
            });
            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase($"{totalPercepcionExcento:0,0.##}", _font7))
            {
                Border = 0,
                HorizontalAlignment = 2
            });
            tblTotalPercepcion.AddCell(new PdfPCell(new Phrase($"{totalPercepcionDecimal:0,0.##}", _font8B))
            {
                Border = 0,
                HorizontalAlignment = 2
            });



            tblTotalDeduccion.AddCell(
                new PdfPCell(new Phrase("Suma de Deducciones $    " + $"{totalDeducciones:0,0.##}", _font8B))
                {
                    Border = 0,
                    HorizontalAlignment = 2
                });
            // tblTotalDeduccion.AddCell(new PdfPCell(new Phrase("Neto del recibo $" + (totalPercepcion - totalDeducciones), _font8B)){Border = 0});



            tblTotales.AddCell(new PdfPCell(tblTotalPercepcion) { Colspan = 3, HorizontalAlignment = 2 });
            tblTotales.AddCell(new PdfPCell(tblTotalDeduccion) { Colspan = 2, HorizontalAlignment = 2 });
            documento.Add(tblTotales);
            #endregion

            #region NETO DEL RECIBO

            var tblNetoRecibo = new PdfPTable(5);
            tblNetoRecibo.TotalWidth = anchoTabla;
            tblNetoRecibo.LockedWidth = true;



            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0 });
            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0 });
            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0 });

            var totalNetoRecibo = 0.0;
            totalNetoRecibo = Double.Parse(totalComprobante);

            tblNetoRecibo.AddCell(new PdfPCell(new Phrase("Neto del recibo $ " + $"{totalNetoRecibo:0,0.##}", _font8B))
            {
                Border = 1,
                Colspan = 2
            });

            documento.Add(tblNetoRecibo);
            documento.Add(new Paragraph(" "));
            #endregion

            #region GENERA LOGO CBB

            string textCode = @"?re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&id=" + uuid;

            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(textCode, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(150, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            //new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Two)).Draw(g, qrCode.Matrix);

            Stream memoryStream = new MemoryStream();

            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
            memoryStream.Position = 0;
            #endregion

            #region "FIRMA DEL EMPLEADO "
            var tblFirma = new PdfPTable(2);
            tblFirma.AddCell(
                new PdfPCell(
                    new Phrase(
                        "Recibi de " + registrosPatronal +
                        " por concepto de pago total de mi salario y demas percepciones del periodo" +
                        " indicado sin que a la fecha se me adeude cantidad alguna por ningun concepto.",
                        _font6))
                {
                    Border = 0
                });
            tblFirma.AddCell(
                new PdfPCell(new Phrase("_______________________________\n Firma del empleado", _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 1
                });


            documento.Add(tblFirma);


            documento.Add(new Paragraph(" "));

            #endregion

            #region AGREGA LOGO CBB Y DATOS DE CERTIFICACION DEL CFDI 

            var tblCBB = new PdfPTable(3);
            var tblCfdidatos = new PdfPTable(2);
            tblCBB.TotalWidth = anchoTabla;
            tblCBB.LockedWidth = true;
            tblCBB.DefaultCell.Border = 0;


            iTextSharp.text.Image logoCBB;
            // logoCBB = iTextSharp.text.Image.GetInstance(_pathArchivosXml + @"\\ImagenesCBB\\" + rfcrecep + ".jpg");

            logoCBB = iTextSharp.text.Image.GetInstance(memoryStream);

            tblCBB.AddCell(new PdfPCell(logoCBB) { Border = 0 });


            var leyendaTabla = conTimbrado ? "Este documento es una representacion impresa de un CFDI" : "-";

            tblCfdidatos.AddCell(
                new PdfPCell(new Phrase(leyendaTabla, _font7B))
                {
                    Colspan = 2,
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });


            var tbldatos = new PdfPTable(1);
            tbldatos.AddCell(new PdfPCell(new Phrase("Serie del Certificado del Emisor:", _font7))
            {
                Border = 0,
                HorizontalAlignment = 2
            });

            tbldatos.AddCell(new PdfPCell(new Phrase("Folio Fiscal UUID:", _font7))
            {
                Border = 0,
                HorizontalAlignment = 2
            });

            tbldatos.AddCell(new PdfPCell(new Phrase("No certificado del SAT:", _font7))
            {
                Border = 0,
                HorizontalAlignment = 2
            });

            tbldatos.AddCell(new PdfPCell(new Phrase("Fecha y Hora de certificación:", _font7))
            {
                Border = 0,
                HorizontalAlignment = 2
            });

            tbldatos.AddCell(new PdfPCell(new Phrase("Método de pago:", _font7))
            {
                Border = 0,
                HorizontalAlignment = 2
            });



            var tblcontenido = new PdfPTable(1);
            tblcontenido.AddCell(new PdfPCell(new Phrase(numCertificadoEmisor, _font7))
            {
                Border = 0,
                HorizontalAlignment = 0
            });
            tblcontenido.AddCell(new PdfPCell(new Phrase(uuid, _font7))
            {
                Border = 0,
                HorizontalAlignment = 0
            });
            tblcontenido.AddCell(new PdfPCell(new Phrase(certificadoSat, _font7))
            {
                Border = 0,
                HorizontalAlignment = 0
            });
            tblcontenido.AddCell(new PdfPCell(new Phrase(fechaTimbrado, _font7))
            {
                Border = 0,
                HorizontalAlignment = 0
            });


            /***** SEC Metodo Pago ********************************************************************/
            //  var mp = listaMetodosPagos.FirstOrDefault(x => x.Clave == metodoPago);

            var metodoPdf = metodoPago; // + " - " + "antes Transferencia";// mp.Descripcion;

            tblcontenido.AddCell(new PdfPCell(new Phrase(metodoPdf, _font7))
            {
                Border = 0,
                HorizontalAlignment = 0
            });


            tblCfdidatos.AddCell(tbldatos);
            tblCfdidatos.AddCell(tblcontenido);


            tblCBB.AddCell(new PdfPCell(tblCfdidatos) { Colspan = 2 });

            documento.Add(tblCBB);

            documento.Add(new Paragraph(" "));

            #endregion

            #region SELLOS Y CADENA ORIGINAL
            var tblSELLOS = new PdfPTable(1);
            tblSELLOS.TotalWidth = anchoTabla;
            tblSELLOS.LockedWidth = true;

            tblSELLOS.AddCell(new PdfPCell(new Phrase("Sello Digital del CFDI", _font7B))
            {
                Border = 0,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            tblSELLOS.AddCell(new PdfPCell(new Phrase(selloCFD, _font7)) { Border = 0 });
            tblSELLOS.AddCell(new PdfPCell(new Phrase("    ", _font7B)) { Border = 0 });

            tblSELLOS.AddCell(new PdfPCell(new Phrase("Sello del SAT", _font7B))
            {
                Border = 0,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
            tblSELLOS.AddCell(new PdfPCell(new Phrase(selloSAT, _font7)) { Border = 0 });
            tblSELLOS.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0 });

            tblSELLOS.AddCell(
                new PdfPCell(new Phrase("Cadena Original del complemento del certificado del SAT", _font7B))
                {
                    Border = 0,
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });
            tblSELLOS.AddCell(new PdfPCell(new Phrase(cadenaOriginal, _font7)) { Border = 0 });


            documento.Add(tblSELLOS);
            #endregion

            documento.NewPage();

            #endregion

            dataSet.Dispose();
            dataSet.Clear();


        }



        #region DOWNLOAD RECIBOS CFDI

        public Task<byte[]> GetArchivosPdfAsync(int[] idNominas)
        {
            var t = Task.Factory.StartNew(() =>
            {

                var items = _dao.GetItemsPdfBytesByIdNomina(idNominas);

                MemoryStream ms = new MemoryStream();
                Document doc = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                writer.CloseStream = false;

                doc.Open();
                PdfContentByte contentByte = writer.DirectContent;

                foreach (byte[] p in items)
                {
                    var reader = new PdfReader(p);
                    int pages = reader.NumberOfPages;

                    for (int i = 1; i <= pages; i++)
                    {
                        doc.SetPageSize(PageSize.A4);
                        doc.NewPage();
                        var page = writer.GetImportedPage(reader, i);
                        contentByte.AddTemplate(page, 0, 0);
                    }
                }
                doc.Close();

                byte[] byteInfo = ms.ToArray();
                ms.Write(byteInfo, 0, byteInfo.Length);
                ms.Position = 0;
                ms.Flush();
                ms.Dispose();

                return byteInfo;

            });

            return t;
        }

        /// <summary>
        /// Retorna los path de los archivos XML y PDF del CFDI
        /// </summary>
        /// <param name="idNominas"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public Task<string[]> DownloadRecibos(int[] idNominas, int idUsuario, string pathFolder)
        {
            //var folderUsuario = "";
            var items = _dao.GetRecibosPdf(idNominas);

            //Validar ruta del folder
            //var fu = await ValidarFolderUsuario(idUsuario, pathFolder);
            //var fls = await GetFoldersRecibosByIdUsuario(idNominas, idUsuario, fu );

            Task tt = Task.Factory.StartNew(() =>
            {
                //Crea el folder para el usuario, si ya existe elimina su contenido.
                var val = ValidarFolderUsuario(idUsuario, pathFolder);
                //Directory.CreateDirectory(fd1);

            });

            return tt.ContinueWith(t => GetPathsRecibosByIdUsuario(idNominas, idUsuario));

        }

        private string ValidarFolderUsuario(int idUsuario, string pathFolder)
        {
            //var pathArchivos = @"C:\Sites\Nominas\Nomina.WEB\Files\DownloadRecibos";
            //var pathArchivos = HttpContext.Current.Server.MapPath("~/Files/DownloadRecibos/");
            var pathArchivos = pathFolder;

            if (!Directory.Exists(pathArchivos))
            {
                Directory.CreateDirectory(pathArchivos);
            }

            //Crear folder para el usuario con su id
            string folderUsuario = pathArchivos + "\\" + idUsuario + "\\";
            if (Directory.Exists(folderUsuario))
            {
                //Elimina el contenido del folder
                Array.ForEach(Directory.GetFiles(folderUsuario), File.Delete);

            }
            else
            {
                //Crea el folder con el id del usuario
                Directory.CreateDirectory(folderUsuario);
            }


            _pathUsuario = folderUsuario;

            return folderUsuario;

        }

        /// <summary>
        /// Metodo retornsa
        /// </summary>
        /// <param name="idNominas"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        private string[] GetPathsRecibosByIdUsuario(int[] idNominas, int idUsuario)
        {
            var items = _dao.GetRecibosPdf(idNominas);

            Random random = new Random();
            int varRamdom = random.Next();
            //Generar los xml desde los item string
            //Crear un solo archivo PDF
            Document doc = new Document(PageSize.LETTER);

            #region GENERA RECIBOS EN UN SOLO PDF

            var fs = new FileStream(_pathUsuario + varRamdom + "_recibos.pdf", FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            writer.CloseStream = false;
            doc.Open();

            foreach (var item in items)
            {
                //XML
                string nombreArchivo = item.IdNomina + "_" + item.IdReceptor + "_" + item.RFCReceptor;

                string path = _pathUsuario + nombreArchivo;

                //Genera los archivos xml - para ser enviados juntos a los pdf
                using (StreamWriter archivoX = new StreamWriter(path + ".xml", false))
                {
                    archivoX.WriteLine(item.XMLSinTimbre); //<- cambiar item.XMLsinTimbre por  item.XMLTimbrado - 
                }

                //PDF
                PdfContentByte contentByte = writer.DirectContent;

                var reader = new PdfReader(item.PDF);
                int pages = reader.NumberOfPages;

                for (int i = 1; i <= pages; i++)
                {
                    doc.SetPageSize(PageSize.A4);
                    doc.NewPage();
                    var page = writer.GetImportedPage(reader, i);
                    contentByte.AddTemplate(page, 0, 0);
                }
            }
            doc.Close();

            fs.Flush();
            fs.Dispose();

            #endregion


            //Generar pdf individuales
            string nombreArchivoIndividual = "";
            string pathIndividual = "";

            foreach (var item in items)
            {
                nombreArchivoIndividual = item.RFCReceptor;

                pathIndividual = _pathUsuario + nombreArchivoIndividual + ".pdf";

                File.WriteAllBytes(pathIndividual, item.PDF);
            }


            //Retorna una lista de archivos .xml y .pdf contenidos en el folder
            var files =
                Directory.EnumerateFiles(_pathUsuario, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".xml") || f.EndsWith(".pdf"))
                    .ToArray();


            return files;


        }



        #endregion

    }
}
