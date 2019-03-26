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
using Common.Utils;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Nomina.Procesador
{
    public class PDFCore
    {
        private int[] _NominasSeleccionadas { get; set; }
        //private bool _timbradoDePrueba { get; set; }
        private readonly TimbradoDao _dao = new TimbradoDao();
        private readonly NominasDao _nominasDao = new NominasDao();
        readonly GlobalConfig _gconfig = new GlobalConfig();
        //private int _idEjercicio;
        //private int _idPeriodo;
        //private string _rutaCertificados;
        //private string _pathArchivosXml;
        private string _pathUsuario;

        #region  PDF - FONT - 

        readonly iTextSharp.text.Font _font6 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font6B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font7 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font7B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font8 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font8B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font9 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font10 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font10B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font11 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font11B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font12B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font13B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font14B = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        readonly iTextSharp.text.Font _font14 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);


        readonly iTextSharp.text.Font _font6W = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font6BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font7W = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font7BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font8W = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font8BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font9W = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font10W = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font10BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font11W = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font11BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font12BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font13BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font14BW = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        readonly iTextSharp.text.Font _font14W = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);



        #endregion


        #region METODOS ASYNC
        public Task<int> GenerarPdfAsync(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo, bool isFiniquito = false, bool isCfdi33 = false)
        {
            var t = Task.Factory.StartNew(() => GenerarReciboTimbrado(nominasSeleccionadas, idEjercicio, idPeriodo, isFiniquito, isCfdi33));
            //var t = Task.Factory.StartNew(() => GenerarPDF(nominasSeleccionadas, idEjercicio, idPeriodo));
            //  return GenerarPDF(nominasSeleccionadas, idEjercicio, idPeriodo);

            return t;
        }

        public Task<string> GetRecibosSintimbre(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo,
            int idUsuario, string pathFolder, bool isCfdi33 = false)
        {
            var t =
                Task.Factory.StartNew(
                    () => GenerarReciboSinTimbre(nominasSeleccionadas, idEjercicio, idPeriodo, idUsuario, pathFolder, isCfdi33));

            return t;
        }

        public Task<string> GetRecibosComplemento(int[] nominasSeleccionadas, int idEjercicio, NOM_PeriodosPago ppago,
             int idUsuario, string pathFolder, bool incluirDetalles)
        {
            var t =
                Task.Factory.StartNew(
                    () => GenerarReciboComplemento(nominasSeleccionadas, idEjercicio, ppago, idUsuario, pathFolder, incluirDetalles));

            return t;
        }



        public Task<string> GetRecibosComplementoDetalle(int[] idEmpleados, int idEjercicio, NOM_PeriodosPago ppago,
         int idUsuario, string pathFolder)
        {
            var t =
                Task.Factory.StartNew(
                    () => GenerarComplementoFiscal(idEmpleados, idEjercicio, ppago, idUsuario, pathFolder));

            return t;
        }

        public Task<string> GetRecibosFiniquitoSintimbre(int idFiniquito, int idEjercicio, int idPeriodo, int idUsuario, string pathFolder, bool isCfdi33 = false)
        {
            var t =
                Task.Factory.StartNew(
                    () => GenerarReciboFiniquitoSinTimbre(idFiniquito, idEjercicio, idPeriodo, idUsuario, pathFolder, isCfdi33));

            return t;
        }


        #endregion


        private int GenerarReciboTimbrado(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo,
            bool isFiniquito = false, bool isCfdi33 = false)
        {
            int erroresPdf = 0;
            NOM_PeriodosPago periodoDatos;
            List<C_Metodos_Pago> listaMetodosPagos;
            List<C_TipoJornada_SAT> listaTipoJornada;
            List<C_RegimenFiscal_SAT> listaRegimenFiscal;
            List<C_TipoIncapacidad_SAT> listaIncapacidadSats;

                _NominasSeleccionadas = nominasSeleccionadas;

            // GET - la lista de XmlTimbrados 
            var listaXmlTimbrados = _dao.GetRecibosPdf(nominasSeleccionadas, isFiniquito: isFiniquito);

            // GET - path Certificados
            //var config = _gconfig.GetGlobalConfig();
            //_rutaCertificados = config.PathCertificados;

            // GET - Datos del periodo y Metodos de pago
            using (var context = new RHEntities())
            {
                periodoDatos = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                listaMetodosPagos = context.C_Metodos_Pago.ToList();

                listaTipoJornada = context.C_TipoJornada_SAT.ToList();

                listaRegimenFiscal = context.C_RegimenFiscal_SAT.ToList();

                listaIncapacidadSats = context.C_TipoIncapacidad_SAT.ToList();
            }

            //Validamos la lista que no sea null o este vacia
            if

            (listaXmlTimbrados == null)
                return 0;
            if (listaXmlTimbrados.Count > 0)
            {
                bool esAsimilado = periodoDatos.IdTipoNomina == 16;//Asimilado
                //Se generar un pdf por cada xml
                foreach (var itemXml in listaXmlTimbrados)
                {
                    try
                    {
                        // Debug.WriteLine($"Creando PDF - Periodo: {itemXml.IdPeriodo} Nomina {itemXml.IdNomina} Empleado {itemXml.IdReceptor}");

                        //Crear PDF - Configuracion del documento
                        Document documento = new Document(PageSize.LETTER, 50, 50, 10, 10);
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        PdfWriter writer = PdfWriter.GetInstance(documento, ms);

                        documento.Open();

                        //validar que el registro contenga el UUID - 
                        if (itemXml.FolioFiscalUUID != null && itemXml.ErrorTimbrado == false)
                        {
                            if (esAsimilado)
                            {
                                if (isCfdi33)
                                {
                                    AddReciboPdfAsimilados33(ref documento, itemXml.XMLTimbrado, periodoDatos, listaTipoJornada, listaRegimenFiscal, itemXml.IdNomina, itemXml.CadenaOriginal, true);
                                }
                                else
                                {
                                    AddReciboPdfAsimilados(ref documento, itemXml.XMLTimbrado, periodoDatos, listaMetodosPagos, itemXml.IdNomina, itemXml.CadenaOriginal, true);
                                }

                            }
                            else
                            {
                                if (isCfdi33)
                                {
                                    AddReciboPdf33(ref documento, itemXml.XMLTimbrado, periodoDatos, listaTipoJornada, listaRegimenFiscal, listaIncapacidadSats, itemXml.IdNomina, itemXml.CadenaOriginal, true);
                                }
                                else
                                {
                                    AddReciboPdf(ref documento, itemXml.XMLTimbrado, periodoDatos, listaMetodosPagos, itemXml.IdNomina, itemXml.CadenaOriginal, true);
                                }

                            }

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
        private string GenerarReciboSinTimbre(int[] nominasSeleccionadas, int idEjercicio, int idPeriodo, int idUsuario, string pathFolder, bool isCfdi33 = false)
        {
            Document doc = new Document(PageSize.LETTER, 50, 50, 10, 10);
            var archivoPdf = "";
            List<string> listaItemSinXML = new List<string>();
            NOM_PeriodosPago periodoDatos;
            List<C_Metodos_Pago> listaMetodosPagos;
            List<C_TipoJornada_SAT> listaTipoJornada;
            List<C_RegimenFiscal_SAT> listaRegimenFiscal;
            List<C_TipoIncapacidad_SAT> listaIncapacidadSats;
            try
            {
                //Crea directorio, o si ya existe elimina el contenido
                var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
                _pathUsuario = pathUsuario;
                //
                var listaNominas = _nominasDao.GetNominasById(nominasSeleccionadas);

                using (var context = new RHEntities())
                {
                    periodoDatos = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                    listaMetodosPagos = context.C_Metodos_Pago.ToList();

                    listaTipoJornada = context.C_TipoJornada_SAT.ToList();

                    listaRegimenFiscal = context.C_RegimenFiscal_SAT.ToList();

                    listaIncapacidadSats = context.C_TipoIncapacidad_SAT.ToList();
                }

                Random r = new Random();
                int aleatorio = r.Next();
                int cont = 0;

                archivoPdf = pathUsuario + "Recibos_" + aleatorio + ".pdf";
                PdfWriter w = PdfWriter.GetInstance(doc, new FileStream(archivoPdf, FileMode.Create));

                doc.Open();

                bool esAsimilado = periodoDatos.IdTipoNomina == 16;
                //Se crea un solo archivo pdf con varias hojas
                foreach (var itemNomina in listaNominas)
                {
                    if (itemNomina.XMLSinTimbre != null)
                    {
                        if (esAsimilado)
                        {
                            if (isCfdi33)
                            {
                                AddReciboPdfAsimilados33(ref doc, itemNomina.XMLSinTimbre, periodoDatos, listaTipoJornada, listaRegimenFiscal, itemNomina.IdNomina);
                            }
                            else
                            {
                                AddReciboPdfAsimilados(ref doc, itemNomina.XMLSinTimbre, periodoDatos, listaMetodosPagos, itemNomina.IdNomina);
                            }

                        }
                        else
                        {
                            if (isCfdi33)
                            {
                                AddReciboPdf33(ref doc, itemNomina.XMLSinTimbre, periodoDatos, listaTipoJornada, listaRegimenFiscal, listaIncapacidadSats, itemNomina.IdNomina);
                            }
                            else
                            {
                                AddReciboPdf(ref doc, itemNomina.XMLSinTimbre, periodoDatos, listaMetodosPagos, itemNomina.IdNomina);
                            }

                        }

                        cont++;
                    }
                    else
                    {
                        var itemX = $"No se encontró xml para generar el recibo Nomina ID: {itemNomina.IdNomina} Total Nomina: {itemNomina.TotalNomina} Periodo: {itemNomina.IdPeriodo}";

                        listaItemSinXML.Add(itemX);
                    }
                }

                if (cont == 0)
                {
                    doc.Add(new Paragraph("Ningún recibo generado"));
                }


                if (listaItemSinXML.Count > 0)
                {
                    doc.NewPage();
                    foreach (var itemLista in listaItemSinXML)
                    {
                        doc.Add(new Paragraph(itemLista, _font7));
                    }
                }

                doc.Close();

            }
            catch (Exception ex)
            {
                doc.Add(new Paragraph("? - Exception "));
                return string.Empty;
            }
            finally
            {
                doc.Close();
            }

            return archivoPdf; //retorna la ruta del archivo pdf generado
        }

        private string GenerarReciboFiniquitoSinTimbre(int idFiniquito, int idEjercicio, int idPeriodo, int idUsuario, string pathFolder, bool isCfdi33 = false)
        {
            Document doc = new Document(PageSize.LETTER, 50, 50, 10, 10);
            var archivoPdf = "";
            List<string> listaItemSinXML = new List<string>();
            NOM_PeriodosPago periodoDatos;
            List<C_Metodos_Pago> listaMetodosPagos;
            List<C_TipoJornada_SAT> listaTipoJornada;
            List<C_RegimenFiscal_SAT> listaRegimenFiscal;
            List<C_TipoIncapacidad_SAT> listaIncapacidadSats;

            try
            {
                //Crea directorio, o si ya existe elimina el contenido
                var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
                _pathUsuario = pathUsuario;
                //
                var itemFiniquito = _nominasDao.GetFiniquitoById(idFiniquito);

                // GET - Datos del periodo y Metodos de pago
                using (var context = new RHEntities())
                {
                    periodoDatos = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                    listaMetodosPagos = context.C_Metodos_Pago.ToList();

                    listaTipoJornada = context.C_TipoJornada_SAT.ToList();

                    listaRegimenFiscal = context.C_RegimenFiscal_SAT.ToList();

                    listaIncapacidadSats = context.C_TipoIncapacidad_SAT.ToList();
                }

                Random r = new Random();
                int aleatorio = r.Next();
                int cont = 0;

                archivoPdf = pathUsuario + "Recibos_" + aleatorio + ".pdf";
                PdfWriter w = PdfWriter.GetInstance(doc, new FileStream(archivoPdf, FileMode.Create));

                doc.Open();

                //Se crea un solo archivo pdf con varias hojas

                if (itemFiniquito.XMLSinTimbre != null)
                {
                    if (isCfdi33)
                    {
                        AddReciboPdf33(ref doc, itemFiniquito.XMLSinTimbre, periodoDatos, listaTipoJornada, listaRegimenFiscal, listaIncapacidadSats, itemFiniquito.IdFiniquito);
                    }
                    else
                    {
                        AddReciboPdf(ref doc, itemFiniquito.XMLSinTimbre, periodoDatos, listaMetodosPagos, itemFiniquito.IdFiniquito);
                    }

                    cont++;
                }
                else
                {
                    var itemX = $"No se encontró xml para generar el recibo Finiquito ID: {itemFiniquito.IdFiniquito} Total Nomina: {itemFiniquito.TOTAL_total} Periodo: {itemFiniquito.IdPeriodo}";

                    listaItemSinXML.Add(itemX);
                }


                if (cont == 0)
                {
                    doc.Add(new Paragraph("Ningún recibo generado"));
                }


                if (listaItemSinXML.Count > 0)
                {
                    doc.NewPage();
                    foreach (var itemLista in listaItemSinXML)
                    {
                        doc.Add(new Paragraph(itemLista, _font7));
                    }
                }

                doc.Close();

            }
            catch (Exception ex)
            {
                doc.Add(new Paragraph("? - Exception "));
                return string.Empty;
            }
            finally
            {
                doc.Close();
            }

            return archivoPdf; //retorna la ruta del archivo pdf generado
        }

        private string GenerarReciboComplemento(int[] nominasSeleccionadas, int idEjercicio, NOM_PeriodosPago ppago, int idUsuario, string pathFolder, bool incluirDetalles)
        {
            Document doc = new Document(PageSize.LETTER, 50, 50, 25, 25);
            var archivoPdf = "";
            int idEmpresa = 0;
            int idColaborador = 0;
            string nombreEmpleado = "";
            string lugarExpedicion = "";
            List<string> sinEmpresas = new List<string>();
            try
            {
                //Crea directorio, o si ya existe elimina el contenido
                var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
                _pathUsuario = pathUsuario;
                //
                var listaNominas = _nominasDao.GetNominasById(nominasSeleccionadas);

                // GET - Datos del periodo y Metodos de pago
                var periodoDatos = _dao.GetPeriodoPagoById(ppago.IdPeriodoPago);

                //GET DATOS SUBIDOS DE COMPLEMENTO

                var listaComplementosSubidos = _nominasDao.GetDatosComplementoDelPeriodo(ppago.IdPeriodoPago);

                Random r = new Random();
                int aleatorio = r.Next();
                int cont = 0;

                archivoPdf = pathUsuario + "Recibos Complemento_" + aleatorio + ".pdf";
                PdfWriter w = PdfWriter.GetInstance(doc, new FileStream(archivoPdf, FileMode.Create));

                doc.Open();

                //Se crea un solo archivo pdf con varias hojas
                Empresa itemEmpresa = new Empresa();
                C_Estado itemEstado = new C_Estado();
                int idEmpresaC = 0;
                int seccionPagina = 0;
                foreach (var itemNomina in listaNominas)
                {
                    // if(seccionPagina == 0)
                    doc.NewPage();

                    //idEmpresaC = itemNomina.IdEmpresaComplemento ?? 0;

                    if (ppago.Sindicato == true)
                    {
                        idEmpresaC = itemNomina.IdEmpresaSindicato ?? 0;
                    }
                    else
                    {
                        idEmpresaC = itemNomina.IdEmpresaComplemento ?? 0;
                    }


                    if (idEmpresaC == 0)
                    {
                        string msg = "El empleado id : " + itemNomina.IdEmpleado + " No tiene empresa Complemento asignado.";
                        sinEmpresas.Add(msg);
                        continue;
                    }

                    //Get datos de la empresa
                    if (idEmpresa != idEmpresaC)
                    {
                        idEmpresa = idEmpresaC;
                        itemEmpresa = _nominasDao.GetEmpresa(idEmpresa);
                        //buscar el nombre del estado de la empresa
                        itemEstado = _nominasDao.GetEstadoById(itemEmpresa.IdEstado);
                    }


                    if (itemEmpresa != null && itemEstado != null)
                    {
                        //Get datos del colaborador
                        idColaborador = itemNomina.IdEmpleado;
                        var itemEmpleado = _nominasDao.GetDatosEmpleado(idColaborador);

                        if (itemEmpleado != null)
                        {
                            lugarExpedicion = itemEmpresa.Municipio + ", " + itemEstado.Descripcion;
                            nombreEmpleado = itemEmpleado.APaterno + " " + itemEmpleado.AMaterno + " " + itemEmpleado.Nombres;

                            //GET - Detalles de nomina complemento

                            var listaDetalles = _nominasDao.GetDetallesNominasCompByIdNomina(itemNomina.IdNomina);

                            var itemComplementoSubido =
                                listaComplementosSubidos.FirstOrDefault(x => x.IdEmpleado == itemEmpleado.IdEmpleado);

                            if (itemComplementoSubido == null) continue;

                            //AGREGA UNA HOJA DE RECIBO DE COMPLEMENTO
                            if (itemEmpresa.IdEmpresa == 12)
                            {
                                AddReciboPdfComplementoSindicato(ref doc, itemEmpresa.RazonSocial, nombreEmpleado, itemEmpleado.IdEmpleado.ToString(), itemComplementoSubido.Cantidad, itemNomina.TotalComplemento, periodoDatos.Fecha_Pago, lugarExpedicion);
                            }
                            else
                            {
                                AddReciboPdfComplemento(ref doc, itemEmpresa.RazonSocial, nombreEmpleado, itemEmpleado.IdEmpleado.ToString(), itemComplementoSubido.Cantidad, itemNomina.TotalComplemento, periodoDatos.Fecha_Pago, lugarExpedicion, listaDetalles, incluirDetalles);
                            }
                            
                            cont++;

                         

                        }

                    }

                    //
                    seccionPagina++;
                    if (seccionPagina == 2)
                    {
                        seccionPagina = 0;
                    }

                }

                if (cont == 0)
                {
                    doc.NewPage();
                    doc.Add(new Paragraph("Ningún recibo generado"));
                }

                if (sinEmpresas.Count > 0)
                {
                    doc.NewPage();
                    foreach (var item in sinEmpresas)
                    {
                        doc.Add(new Paragraph(item));
                    }


                }


                doc.Close();

            }
            catch (Exception ex)
            {
                doc.Add(new Paragraph("? - Exception "));
                //   return string.Empty;
            }
            finally
            {
                doc.Close();
            }

            return archivoPdf; //retorna la ruta del archivo pdf generado
        }


        //private string GenerarReciboComplementoSindicato( int idEjercicio, NOM_PeriodosPago ppago, int idUsuario, string pathFolder, bool incluirDetalles)
        //{
        //    Document doc = new Document(PageSize.LETTER, 50, 50, 25, 25);
        //    var archivoPdf = "";
        //    int idEmpresa = 0;
        //    int idColaborador = 0;
        //    string nombreEmpleado = "";
        //    string lugarExpedicion = "";
        //    List<string> sinEmpresas = new List<string>();
        //    try
        //    {
        //        //Crea directorio, o si ya existe elimina el contenido
        //        var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
        //        _pathUsuario = pathUsuario;
        //        //
        //        var listaNominas = _nominasDao.GetNominasById();

        //        // GET - Datos del periodo y Metodos de pago
        //        var periodoDatos = _dao.GetPeriodoPagoById(ppago.IdPeriodoPago);

        //        //GET DATOS SUBIDOS DE COMPLEMENTO

        //        var listaComplementosSubidos = _nominasDao.GetDatosComplementoDelPeriodo(ppago.IdPeriodoPago);

        //        Random r = new Random();
        //        int aleatorio = r.Next();
        //        int cont = 0;

        //        archivoPdf = pathUsuario + "Recibos Complemento_" + aleatorio + ".pdf";
        //        PdfWriter w = PdfWriter.GetInstance(doc, new FileStream(archivoPdf, FileMode.Create));

        //        doc.Open();

        //        //Se crea un solo archivo pdf con varias hojas
        //        Empresa itemEmpresa = new Empresa();
        //        C_Estado itemEstado = new C_Estado();
        //        int idEmpresaC = 0;
        //        int seccionPagina = 0;
        //        foreach (var itemNomina in listaNominas)
        //        {
        //            // if(seccionPagina == 0)
        //            doc.NewPage();

        //            //idEmpresaC = itemNomina.IdEmpresaComplemento ?? 0;

        //            if (ppago.Sindicato == true)
        //            {
        //                idEmpresaC = itemNomina.IdEmpresaSindicato ?? 0;
        //            }
        //            else
        //            {
        //                idEmpresaC = itemNomina.IdEmpresaComplemento ?? 0;
        //            }


        //            if (idEmpresaC == 0)
        //            {
        //                string msg = "El empleado id : " + itemNomina.IdEmpleado + " No tiene empresa Complemento asignado.";
        //                sinEmpresas.Add(msg);
        //                continue;
        //            }

        //            //Get datos de la empresa
        //            if (idEmpresa != idEmpresaC)
        //            {
        //                idEmpresa = idEmpresaC;
        //                itemEmpresa = _nominasDao.GetEmpresa(idEmpresa);
        //                //buscar el nombre del estado de la empresa
        //                itemEstado = _nominasDao.GetEstadoById(itemEmpresa.IdEstado);
        //            }


        //            if (itemEmpresa != null && itemEstado != null)
        //            {
        //                //Get datos del colaborador
        //                idColaborador = itemNomina.IdEmpleado;
        //                var itemEmpleado = _nominasDao.GetDatosEmpleado(idColaborador);

        //                if (itemEmpleado != null)
        //                {
        //                    lugarExpedicion = itemEmpresa.Municipio + ", " + itemEstado.Descripcion;
        //                    nombreEmpleado = itemEmpleado.APaterno + " " + itemEmpleado.AMaterno + " " + itemEmpleado.Nombres;

        //                    //GET - Detalles de nomina complemento

        //                    var listaDetalles = _nominasDao.GetDetallesNominasCompByIdNomina(itemNomina.IdNomina);

        //                    var itemComplementoSubido =
        //                        listaComplementosSubidos.FirstOrDefault(x => x.IdEmpleado == itemEmpleado.IdEmpleado);

        //                    if (itemComplementoSubido == null) continue;

        //                    //AGREGA UNA HOJA DE RECIBO DE COMPLEMENTO
        //                    AddReciboPdfComplemento(ref doc, itemEmpresa.RazonSocial, nombreEmpleado, itemEmpleado.IdEmpleado.ToString(), itemComplementoSubido.Cantidad, itemNomina.TotalComplemento, periodoDatos.Fecha_Pago, lugarExpedicion, listaDetalles, incluirDetalles);
        //                    cont++;

        //                    //Se agrega un espacio para dividir el recibo
        //                    PdfPTable tablaEspacio = new PdfPTable(1)
        //                    {
        //                        TotalWidth = 520,
        //                        LockedWidth = true
        //                    };

        //                    // Espacio entre row superior
        //                    tablaEspacio.AddCell(new PdfPCell(new Phrase("", _font8))
        //                    {
        //                        Border = 0,
        //                        FixedHeight = 90,

        //                    });

        //                    doc.Add(tablaEspacio);

        //                }

        //            }

        //            //
        //            seccionPagina++;
        //            if (seccionPagina == 2)
        //            {
        //                seccionPagina = 0;
        //            }

        //        }

        //        if (cont == 0)
        //        {
        //            doc.NewPage();
        //            doc.Add(new Paragraph("Ningún recibo generado"));
        //        }

        //        if (sinEmpresas.Count > 0)
        //        {
        //            doc.NewPage();
        //            foreach (var item in sinEmpresas)
        //            {
        //                doc.Add(new Paragraph(item));
        //            }


        //        }


        //        doc.Close();

        //    }
        //    catch (Exception ex)
        //    {
        //        doc.Add(new Paragraph("? - Exception "));
        //        //   return string.Empty;
        //    }
        //    finally
        //    {
        //        doc.Close();
        //    }

        //    return archivoPdf; //retorna la ruta del archivo pdf generado
        //}

        #region RECIBOS DE FINIQUITOS FISCAL - REAL 
        public byte[] ReciboFiniquitoFiscal(int idFiniquito, int idPeriodo, int idUsuario, string pathFolder, bool isLiquidacion = false)
        {
            if (idFiniquito == 0)
            {
                return GenerarReciboVacio();
            }

            //GET LOS DATOS DEL FINIQUITO FISCAL
            var itemFiniquitoF = _nominasDao.GetFiniquitoById(idFiniquito);

            if (itemFiniquitoF == null)
            {
                return GenerarReciboVacio();
            }

            int idEmpresa = 0;

            decimal TotalRecibo = 0;
            idEmpresa = itemFiniquitoF.IdEmpresaFiscal ?? 0;

            //GET DATOS DE LA EMPRESA
            var itemEmpresa = _nominasDao.GetEmpresa(idEmpresa);

            //GET DATOS DEL EMPLEADO
            var itemEmpleado = _nominasDao.GetDatosEmpleado(itemFiniquitoF.IdEmpleado);

            // GET DATO CONTRATO
            var itemContrato = NominasDao.GetContratoByIdContrato(itemFiniquitoF.IdContrato);   //NominasDao.GetContratoEmpleado(itemFiniquitoF.IdEmpleado, itemFiniquitoF.IdSucursal);

            //GET DATO PUESTO
            var itemPuesto = _nominasDao.GetPuestoById(itemContrato.IdPuesto);

            //GET DATO DEPARTAMENTO
            var itemDepartamento = _nominasDao.GetDepartamentoById(itemPuesto.IdDepartamento);

            //GET DESCUENTOS ADICIONALES - que ya estan aplicados en el calculo del finiquito
            //var descuentosAplicados = _nominasDao.GetDescuentosFiniquito(idPeriodo);
            var listaDescuentos = _nominasDao.GetDescuentosAdicionalesFiniquito(idPeriodo);
            var listaDescuentosComplemento = listaDescuentos.Where(x => x.IsComplemento).ToList();
            var listaDescuentosFiscales = listaDescuentos.Where(x => x.IsComplemento == false).ToList();
            var listaConceptos = _nominasDao.GetConceptosFiscales();
            var listaDescuentoPorSueldoPendiente = _nominasDao.GetDescuentoPorSueldoPendiente(idFiniquito);


            //CREAR EL DOCUMENTO pdf
            Random r = new Random();
            int aleatorio = r.Next();
            var archivoPdf = "";
            int cont = 0;
            BaseColor colorBackAzulOscuro = BaseColor.LIGHT_GRAY;

            //Se Creará el pdf en memoria 
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.LETTER, 50, 50, 25, 25);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;
            doc.Open();
            // PdfContentByte contentByte = writer.DirectContent;

            int anchoTabla = 520;


            //NOMBRE DE LA EMPRESA
            //Tabla del recibo
            #region "DATOS EMPRESA"
            PdfPTable tablaRecibo = new PdfPTable(3)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            //Celda vacia
            tablaRecibo.AddCell(new PdfPCell(new Phrase(" ", _font11))
            {
                Colspan = 1,
                BackgroundColor = colorBackAzulOscuro,
                Border = 0,
                HorizontalAlignment = 1//Centrado
            });
            //Nombre de la empresa
            tablaRecibo.AddCell(new PdfPCell(new Phrase(itemEmpresa.RazonSocial, _font11))
            {
                Colspan = 1,
                BackgroundColor = colorBackAzulOscuro,
                Border = 0,
                HorizontalAlignment = 1//Centrado
            });

            //Fecha 
            var fechaEMisionPago = DateTime.Now;
            tablaRecibo.AddCell(new PdfPCell(new Phrase(fechaEMisionPago.ToShortDateString(), _font9))
            {
                Colspan = 1,
                BackgroundColor = colorBackAzulOscuro,
                Border = 0,
                HorizontalAlignment = 2//
            });

            //Leyenda Finiquito

            tablaRecibo.AddCell(new PdfPCell(new Phrase(isLiquidacion ? "LIQUIDACIÓN" : "FINIQUITO F", _font11))
            {
                Colspan = 3,
                BackgroundColor = colorBackAzulOscuro,
                Border = 0,
                HorizontalAlignment = 1//Centrado
            });

            doc.Add(tablaRecibo);
            #endregion


            //DATOS DEL EMPLEADO
            PdfPTable tablaDatosEmpleado = new PdfPTable(1)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true,

            };
            string nombreEmpleado = itemEmpleado.APaterno + " " + itemEmpleado.AMaterno + " " + itemEmpleado.Nombres;
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Empleado         :  " + nombreEmpleado, _font10)) { Border = PdfPCell.TOP_BORDER });

            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Puesto              :  " + itemPuesto.Descripcion, _font10)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Departamento  :  " + itemDepartamento.Descripcion, _font11)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Fecha Ingreso  :  " + itemFiniquitoF.FechaAlta.ToShortDateString(), _font10)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Fecha Baja       :  " + itemFiniquitoF.FechaBaja.ToShortDateString(), _font10)) { Border = 0 });

            var añosAntiguedad = itemFiniquitoF.FechaBaja.Year - itemFiniquitoF.FechaAlta.Year;

            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Antiguedad       :  " + añosAntiguedad, _font10)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("SD                    : $ " + itemFiniquitoF.SD.ToCurrencyFormat(), _font10)) { Border = 0 });

            // Espacio entre row superior
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 5, Colspan = 1 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 1, HorizontalAlignment = 2, FixedHeight = 5, Colspan = 1 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 1 });

            doc.Add(tablaDatosEmpleado);

            //CONTENIDO DEL RECIBO
            PdfPTable tablaContenidoRecibo = new PdfPTable(3)
            {

            };

            if (itemFiniquitoF.AGUINDALDO > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Aguinaldo:", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.AGUINDALDO.ToCurrencyFormat(), _font10B)) { Border = 0, HorizontalAlignment = 2 });
            }

            if (itemFiniquitoF.VACACIONES > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Vacaciones:", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.VACACIONES.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }
            if (itemFiniquitoF.VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Vacaciones Pendientes:", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.VACACIONES_PENDIENTE.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }

            if (itemFiniquitoF.VACACIONES + itemFiniquitoF.VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Vacaciones", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + (itemFiniquitoF.VACACIONES + itemFiniquitoF.VACACIONES_PENDIENTE).ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });
            }
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

            if (itemFiniquitoF.PRIMA_VACACIONAL > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Prima Vacacional", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.PRIMA_VACACIONAL.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }
            if (itemFiniquitoF.PRIMA_VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Prima Vacacional Pendiente", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.PRIMA_VACACIONES_PENDIENTE.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }

            if (itemFiniquitoF.PRIMA_VACACIONAL + itemFiniquitoF.PRIMA_VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Prima Vacacional", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + (itemFiniquitoF.PRIMA_VACACIONAL + itemFiniquitoF.PRIMA_VACACIONES_PENDIENTE).ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });
            }
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

            if (itemFiniquitoF.SUELDO > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Sueldo Pendiente", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.SUELDO.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
            }
            if (itemFiniquitoF.GRATIFICACION > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Gratificacion", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.GRATIFICACION.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
            }

            if (itemFiniquitoF.SubTotal_Finiquito > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("SubTotal Finiquito:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + itemFiniquitoF.SubTotal_Finiquito.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });
            }
            if (itemFiniquitoF.ISR_Finiquito > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("ISR", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemFiniquitoF.ISR_Finiquito.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
            }

            if (itemFiniquitoF.Subsidio_Finiquito > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Subsidio", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("+ " + itemFiniquitoF.Subsidio_Finiquito.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
            }


            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Finiquito:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + itemFiniquitoF.Total_Finiquito.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });

            TotalRecibo = itemFiniquitoF.TOTAL_total;

            //LIQUIDACION
            if (isLiquidacion)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

                if (itemFiniquitoF.L_3MESES_SUELDO > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("3 meses salario", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.L_3MESES_SUELDO.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
                }

                if (itemFiniquitoF.L_20DIAS_POR_AÑO > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("20 dias x año", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.L_20DIAS_POR_AÑO.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
                }
                if (itemFiniquitoF.PRIMA_ANTIGUEDAD > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Prima Antiguedad", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.PRIMA_ANTIGUEDAD.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
                }

                if (itemFiniquitoF.COMPENSACION_X_INDEMNIZACION > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Compensación x Indemnizacion", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.COMPENSACION_X_INDEMNIZACION.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
                }


                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Subtotal Liquidación:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + itemFiniquitoF.SubTotal_Liquidacion.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });

                if (itemFiniquitoF.ISR_Liquidacion > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("ISR Liq", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemFiniquitoF.ISR_Liquidacion.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
                }
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Liquidacion:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + itemFiniquitoF.Total_Liquidacion.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });


                TotalRecibo = itemFiniquitoF.TOTAL_total;
            }

            if (itemFiniquitoF.TotalDeducciones > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

                foreach (var itemDeduccion in listaDescuentosFiscales)
                {
                    //para evitar que se repita cuando hay sueldo pendiente
                    if (itemDeduccion.IdConcepto != 48 && itemDeduccion.IdConcepto != 52 && itemFiniquitoF.SUELDO >0)
                    {
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase(itemDeduccion.Descripcion, _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemDeduccion.TotalDescuento.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
                    }
                }

                if (itemFiniquitoF.SUELDO > 0)
                {
                    foreach (var itemDeduccion in listaDescuentoPorSueldoPendiente)
                    {
                        var itemConcepto = listaConceptos.FirstOrDefault(x => x.IdConcepto == itemDeduccion.IdConcepto);
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase(itemConcepto.DescripcionCorta, _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemDeduccion.Total.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
                    }
                }
            }

            if (itemFiniquitoF.PensionAlimenticiaDelTotal > 0)
            {
                //espacio en blanco
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });
                //
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Pension Alimenticia", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemFiniquitoF.PensionAlimenticiaDelTotal.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
            }


            //TOTAL DEL RECIBO
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 20, Colspan = 3 });

            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("_____________", _font10)) { HorizontalAlignment = 2, Border = 0 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total:", _font11B)) { HorizontalAlignment = 2, Border = 0, Colspan = 2 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + TotalRecibo.ToCurrencyFormat(), _font11B)) { HorizontalAlignment = 2, Border = 0 });

            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 20, Colspan = 3 });



            PdfPTable tablaDescuentosAplicados = new PdfPTable(2)
            {

            };

            tablaDescuentosAplicados.AddCell(new PdfPCell(new Phrase("Descuentos adicionales", _font8)) { Border = PdfPCell.BOTTOM_BORDER, Colspan = 2 });
            foreach (var item in listaDescuentosComplemento)
            {
                tablaDescuentosAplicados.AddCell(new PdfPCell(new Phrase(item.Descripcion, _font8)) { Border = 0 });
                tablaDescuentosAplicados.AddCell(new PdfPCell(new Phrase(item.TotalDescuento.ToCurrencyFormat(), _font8)) { Border = 0 });
            }


            PdfPTable tablaFormaContenido = new PdfPTable(9)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            if (listaDescuentosComplemento.Count > 0)
            {
                tablaFormaContenido.AddCell(new PdfPCell(tablaDescuentosAplicados) { Border = 0, HorizontalAlignment = 0, Colspan = 4 });
                tablaFormaContenido.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2 });
                tablaFormaContenido.AddCell(new PdfPCell(tablaContenidoRecibo) { Border = 0, HorizontalAlignment = 0, Colspan = 4 });

            }
            else
            {
                tablaFormaContenido.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 0, Colspan = 4 });
                tablaFormaContenido.AddCell(new PdfPCell(tablaContenidoRecibo) { Border = 0, HorizontalAlignment = 0, Colspan = 4 });
                tablaFormaContenido.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2 });
            }



            doc.Add(tablaFormaContenido);

            //LEYENDA
            PdfPTable tablaLeyenda = new PdfPTable(1)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            tablaLeyenda.AddCell(new PdfPCell(new Paragraph("HAGO CONSTAR QUE DURANTE EL TIEMPO QUE PRESTE MIS SERVICIOS A ESTA EMPRESA, ME FUERON CUBIERTOS COMPLETA Y OPORTUNAMENTE TODAS LAS PRESTACIONES ORDINARIAS Y EXTRAORDINARIAS A QUE TUVE DERECHO, TALES COMO: SALARIOS, SÉPTIMOS DIAS(días de descanso, tanto semanales como obligatorios, también conocidos como FESTIVOS, VACACIONES, GRATIFICACIONES O AGUINALDO, Y EN GENERAL TODAS LAS PRESTACIONES LEGALES Y CONTRACTUALES, POR LO QUE NO ME RESERVO ACCION O DERECHO QUE EJERCITAR EN CONTRA DE LA EMPRESA Y OTORGO A SU FAVOR EL FINIQUITO MAS AMPLIO QUE EN DERECHO PROCEDE, reconociendo que únicamente le preste servicios en jornada ordinaria.", _font9)) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, Border = 0 });

            tablaLeyenda.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 20, Colspan = 1 });

            doc.Add(tablaLeyenda);

            //FIRMA DEL EMPLEADO

            PdfPTable tablaFirmaEmpleado = new PdfPTable(1)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            tablaFirmaEmpleado.AddCell(new PdfPCell(new Phrase("_______________________________", _font10B)) { HorizontalAlignment = 1, Border = 0 });
            tablaFirmaEmpleado.AddCell(new PdfPCell(new Phrase("Firma del empleado", _font10B)) { HorizontalAlignment = 1, Border = 0 });
            doc.Add(tablaFirmaEmpleado);
            //Cierre del documento
            doc.Close();

            //
            byte[] byteInfo = ms.ToArray();
            ms.Write(byteInfo, 0, byteInfo.Length);
            ms.Position = 0;
            ms.Flush();
            ms.Dispose();

            return byteInfo;
        }
        public byte[] ReciboFiniquitoReal(int idFiniquito, int idPeriodo, int idUsuario, string pathFolder, bool isLiquidacion = false)
        {

            if (idFiniquito == 0)
            {
                return GenerarReciboVacio();
            }


            //GET LOS DATOS DEL FINIQUITO FISCAL
            var itemFiniquitoF = _nominasDao.GetFiniquitoById(idFiniquito);
            var itemFiniquitoReal = _nominasDao.GetFiniquitoRealById(idFiniquito);

            if (itemFiniquitoF == null || itemFiniquitoReal == null)
            {
                return GenerarReciboVacio();
            }

            decimal subTotalFC = 0;
            decimal totalFC = 0;

            decimal subTotalLC = 0;
            decimal TotalLC = 0;

            decimal TotalRecibo = 0;
            int idEmpresa = 0;

            idEmpresa = itemFiniquitoF.IdEmpresaFiscal ?? 0;

            //GET DATOS DE LA EMPRESA
            var itemEmpresa = _nominasDao.GetEmpresa(idEmpresa);

            //GET DATOS DEL EMPLEADO
            var itemEmpleado = _nominasDao.GetDatosEmpleado(itemFiniquitoF.IdEmpleado);

            // GET DATO CONTRATO
            var itemContrato = NominasDao.GetContratoByIdContrato(itemFiniquitoF.IdContrato); //NominasDao.GetContratoEmpleado(itemFiniquitoF.IdEmpleado, itemFiniquitoF.IdSucursal);

            //GET DATO PUESTO
            var itemPuesto = _nominasDao.GetPuestoById(itemContrato.IdPuesto);

            //GET DATO DEPARTAMENTO
            var itemDepartamento = _nominasDao.GetDepartamentoById(itemPuesto.IdDepartamento);

            //GET DATA COMISIONES 
            var listaComisionesComplemento = _nominasDao.GetComisionesComplementoFiniquito(idPeriodo);

            //GET DATA DESCUENTOS
            var listaDescuentos = _nominasDao.GetDescuentosAdicionalesFiniquito(idPeriodo);
            var listaDescuentosComplemento = listaDescuentos.Where(x => x.IsComplemento).ToList();
            var listaDescuentosFiscales = listaDescuentos.Where(x => x.IsComplemento == false).ToList();

            var listaConceptos = _nominasDao.GetConceptosFiscales();
            var listaDescuentoPorSueldoPendiente = _nominasDao.GetDescuentoPorSueldoPendiente(idFiniquito);


            //CREAR EL DOCUMENTO pdf

            Random r = new Random();
            int aleatorio = r.Next();
            var archivoPdf = "";
            int cont = 0;


            //Se Creará el pdf en memoria 
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.LETTER, 50, 50, 25, 25);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;
            doc.Open();
            // PdfContentByte contentByte = writer.DirectContent;

            int anchoTabla = 520;


            //NOMBRE DE LA EMPRESA
            //Tabla del recibo
            #region "DATOS EMPRESA"
            PdfPTable tablaRecibo = new PdfPTable(1)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };



            //Fecha 
            var fechaEMisionPago = DateTime.Now;
            tablaRecibo.AddCell(new PdfPCell(new Phrase(fechaEMisionPago.ToShortDateString(), _font9))
            {
                Colspan = 1,
                Border = 0,
                HorizontalAlignment = 2//
            });

            //Leyenda Finiquito

            tablaRecibo.AddCell(new PdfPCell(new Phrase(isLiquidacion ? "LIQUIDACION" : "FINIQUITO R", _font11))
            {
                Colspan = 1,
                Border = 0,
                HorizontalAlignment = 1//Centrado
            });

            doc.Add(tablaRecibo);
            #endregion


            //DATOS DEL EMPLEADO
            PdfPTable tablaDatosEmpleado = new PdfPTable(1)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true,

            };
            string nombreEmpleado = itemEmpleado.APaterno + " " + itemEmpleado.AMaterno + " " + itemEmpleado.Nombres;
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Empleado         :  " + nombreEmpleado, _font10)) { Border = PdfPCell.TOP_BORDER });

            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Puesto              :  " + itemPuesto.Descripcion, _font10)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Departamento  :  " + itemDepartamento.Descripcion, _font11)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Fecha Ingreso  :  " + itemFiniquitoReal.FechaAlta.ToShortDateString(), _font10)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Fecha Baja       :  " + itemFiniquitoReal.FechaBaja.ToShortDateString(), _font10)) { Border = 0 });

            var añosAntiguedad = itemFiniquitoReal.FechaBaja.Year - itemFiniquitoReal.FechaAlta.Year;

            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("Antigüedad       :  " + añosAntiguedad, _font10)) { Border = 0 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("SDReal              : $ " + itemContrato.SalarioReal.ToCurrencyFormat(), _font10)) { Border = 0 });

            // Espacio entre row superior
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 5, Colspan = 1 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 1, HorizontalAlignment = 2, FixedHeight = 5, Colspan = 1 });
            tablaDatosEmpleado.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 1 });

            doc.Add(tablaDatosEmpleado);

            //CONTENIDO DEL RECIBO
            PdfPTable tablaContenidoRecibo = new PdfPTable(3)
            {

            };

            if (itemFiniquitoReal.AGUINALDO > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Aguinaldo:", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.AGUINALDO.ToCurrencyFormat(), _font10B)) { Border = 0, HorizontalAlignment = 2 });
            }
            if (itemFiniquitoReal.VACACIONES > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Vacaciones:", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.VACACIONES.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }
            if (itemFiniquitoReal.VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Vacaciones Pendientes:", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.VACACIONES_PENDIENTE.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }
            if (itemFiniquitoReal.VACACIONES + itemFiniquitoReal.VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Vacaciones", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + (itemFiniquitoReal.VACACIONES + itemFiniquitoReal.VACACIONES_PENDIENTE).ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });
            }
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

            if (itemFiniquitoReal.PRIMA_VACACIONAL > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Prima Vacacional", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.PRIMA_VACACIONAL.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }
            if (itemFiniquitoReal.PRIMA_VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Prima Vacacional Pendiente", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.PRIMA_VACACIONES_PENDIENTE.ToCurrencyFormat(), _font10)) { Border = 0, HorizontalAlignment = 2 });
            }

            if (itemFiniquitoReal.PRIMA_VACACIONAL + itemFiniquitoReal.PRIMA_VACACIONES_PENDIENTE > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Prima Vacacional", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + (itemFiniquitoReal.PRIMA_VACACIONAL + itemFiniquitoReal.PRIMA_VACACIONES_PENDIENTE).ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });
            }


            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

            if (itemFiniquitoReal.SUELDO > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Sueldo", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.SUELDO.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
            }


            //tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Gratificacion", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
            //tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.GRATIFICACION.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });

            //if (isLiquidacion)
            //{
            //    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Comp x Indemnizacion", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
            //    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoF.COMPENSACION_X_INDEMNIZACION.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
            //}

            subTotalFC = itemFiniquitoReal.AGUINALDO + (itemFiniquitoReal.VACACIONES + itemFiniquitoReal.VACACIONES_PENDIENTE) + (itemFiniquitoReal.PRIMA_VACACIONAL + itemFiniquitoReal.PRIMA_VACACIONES_PENDIENTE) + itemFiniquitoReal.SUELDO;


            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("SubTotal Finiquito:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + subTotalFC.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });

            if (itemFiniquitoF.Subsidio_Finiquito > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Subsidio", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("+ " + itemFiniquitoF.Subsidio_Finiquito.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
            }
            totalFC = subTotalFC + itemFiniquitoF.Subsidio_Finiquito;

            if (itemFiniquitoF.ISR_Finiquito > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("ISR", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemFiniquitoF.ISR_Finiquito.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });

                totalFC -= itemFiniquitoF.ISR_Finiquito;
            }



            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Finiquito:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + totalFC.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });


            //LIQUIDACION
            if (isLiquidacion)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

                if (itemFiniquitoReal.L_3MESES_SUELDO > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("3 meses salario", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.L_3MESES_SUELDO.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
                }
                if (itemFiniquitoReal.L_20DIAS_POR_AÑO > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("20 dias x año", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.L_20DIAS_POR_AÑO.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
                }
                if (itemFiniquitoReal.PRIMA_ANTIGUEDAD > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Prima Antiguedad", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("" + itemFiniquitoReal.PRIMA_ANTIGUEDAD.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = 0 });
                }
                subTotalLC = itemFiniquitoReal.L_3MESES_SUELDO + itemFiniquitoReal.L_20DIAS_POR_AÑO + itemFiniquitoReal.PRIMA_ANTIGUEDAD;

                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Subtotal Liquidación:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + subTotalLC.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });

                if (itemFiniquitoF.ISR_Liquidacion > 0)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("ISR Liq", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemFiniquitoF.ISR_Liquidacion.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
                }
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

                TotalLC = subTotalLC - itemFiniquitoF.ISR_Liquidacion;

                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total Liquidacion:", _font10B)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + TotalLC.ToCurrencyFormat(), _font10B)) { HorizontalAlignment = 2, Border = PdfPCell.TOP_BORDER });

            }


            if (listaComisionesComplemento.Count > 0)
            {
                //agregamos un espacio
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

                foreach (var itemComision in listaComisionesComplemento)
                {
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase(itemComision.Descripcion)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                    tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase(itemComision.TotalDescuento.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });

                    totalFC += itemComision.TotalDescuento;
                }
            }

            //DEDUCCIONES
            if (itemFiniquitoF.TotalDeducciones > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });

                foreach (var itemDeduccion in listaDescuentosFiscales)
                {
                    //para evitar que se repita cuando hay sueldo pendiente
                    if (itemDeduccion.IdConcepto != 48 && itemDeduccion.IdConcepto != 52 && itemFiniquitoF.SUELDO > 0)
                    {
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase(itemDeduccion.Descripcion, _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemDeduccion.TotalDescuento.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
                    }
                }

                if (itemFiniquitoF.SUELDO > 0)
                {
                    foreach (var itemDeduccion in listaDescuentoPorSueldoPendiente)
                    {
                        var itemConcepto = listaConceptos.FirstOrDefault(x => x.IdConcepto == itemDeduccion.IdConcepto);
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase(itemConcepto.DescripcionCorta, _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                        tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemDeduccion.Total.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });
                    }
                }
            }

            //DESCUENTOS ADICIONALES - se aplica aqui para cuadrar el recibo fiscal
            if (itemFiniquitoF.DescuentosAplicados > 0)
            {
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 10, Colspan = 3 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Descuentos Adicionales", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
                tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("- " + itemFiniquitoF.DescuentosAplicados.ToCurrencyFormat(), _font10)) { HorizontalAlignment = 2, Border = 0 });

                totalFC -= itemFiniquitoF.DescuentosAplicados;
            }



            //TotalRecibo = (totalFC + TotalLC);
            TotalRecibo = itemFiniquitoF.TOTAL_total;


            //TOTAL DEL RECIBO
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 20, Colspan = 3 });

            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font10)) { HorizontalAlignment = 0, Border = 0, Colspan = 2 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("_____________", _font10)) { HorizontalAlignment = 2, Border = 0 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("Total:", _font11B)) { HorizontalAlignment = 2, Border = 0, Colspan = 2 });
            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("$ " + TotalRecibo.ToCurrencyFormat(), _font11B)) { HorizontalAlignment = 2, Border = 0 });

            tablaContenidoRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2, FixedHeight = 20, Colspan = 3 });




            PdfPTable tablaFormaContenido = new PdfPTable(9)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            tablaFormaContenido.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 0, Colspan = 4 });
            tablaFormaContenido.AddCell(new PdfPCell(tablaContenidoRecibo) { Border = 0, HorizontalAlignment = 0, Colspan = 4 });
            tablaFormaContenido.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, HorizontalAlignment = 2 });

            doc.Add(tablaFormaContenido);



            //FIRMA DEL EMPLEADO

            PdfPTable tablaFirmaEmpleado = new PdfPTable(1)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            tablaFirmaEmpleado.AddCell(new PdfPCell(new Phrase("_______________________________", _font10B)) { HorizontalAlignment = 1, Border = 0 });
            tablaFirmaEmpleado.AddCell(new PdfPCell(new Phrase("Firma del empleado", _font10B)) { HorizontalAlignment = 1, Border = 0 });
            doc.Add(tablaFirmaEmpleado);
            //Cierre del documento
            doc.Close();

            //
            byte[] byteInfo = ms.ToArray();
            ms.Write(byteInfo, 0, byteInfo.Length);
            ms.Position = 0;
            ms.Flush();
            ms.Dispose();

            return byteInfo;
        }
        public byte[] GenerarReciboVacio()
        {
            //Se Creará el pdf en memoria 
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.LETTER, 50, 50, 25, 25);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;
            doc.Open();

            PdfPTable tablainfo = new PdfPTable(1)
            {
                TotalWidth = 520,
                LockedWidth = true
            };

            tablainfo.AddCell(new PdfPCell(new Phrase("¡ No se encontró datos para generar el recibo!", _font10B)) { HorizontalAlignment = 1, Border = 0 });
            tablainfo.AddCell(new PdfPCell(new Phrase("Debe procesar el Finiquito...", _font10B)) { HorizontalAlignment = 1, Border = 0 });
            doc.Add(tablainfo);


            doc.Close();

            //
            byte[] byteInfo = ms.ToArray();
            ms.Write(byteInfo, 0, byteInfo.Length);
            ms.Position = 0;
            ms.Flush();
            ms.Dispose();

            return byteInfo;
        }

        #endregion


        /// <summary>
        /// Agrega una página PDF de recibo al documento.
        /// Metodo principal que generar el contenido del PDF
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="strXml"></param>
        /// <param name="periodoDatos"></param>
        /// <param name="listaMetodosPagos"></param>
        /// <param name="idNomina"></param>
        /// <param name="cadenaOriginal"></param>
        /// <param name="conTimbrado"></param>
        private void AddReciboPdf(ref Document documento, string strXml, NOM_PeriodosPago periodoDatos,
            List<C_Metodos_Pago> listaMetodosPagos, int idNomina = 0, string cadenaOriginal = "cadena original", bool conTimbrado = false)
        {
            DataSet dataSet = new DataSet();
            try
            {
                int anchoTabla = 520;
                string lugarExpedicion = "";
                BaseColor colorBackAzulOscuro = BaseColor.LIGHT_GRAY;

                //if (periodoDatos.IdTipoNomina == 17)
                //{
                //    colorBackAzulOscuro = new BaseColor(219,186,191);
                //}

                //Carga el contenido del XML en el DATASET
                StringReader streamReader = new StringReader(strXml);
                dataSet.ReadXml(streamReader);

                #region "VARIABLES"

                string cnx = string.Empty,
                    //fechaIniPago = string.Empty,
                    //fechaFinPago = string.Empty,
                    //fechaPago = string.Empty,
                    curp = string.Empty,
                    depa = string.Empty,
                    puesto = string.Empty,
                    registrosPatronal = string.Empty,
                    fechaInicioLaboral = string.Empty,
                    tipoJornada = string.Empty,
                    sdi = string.Empty,
                    sbc = string.Empty,
                    nss = string.Empty;

                string nombreReceptor = string.Empty,
                    rfcrecep = string.Empty,
                    rfcEmisor = string.Empty,
                    //municipioEmisor = string.Empty,
                    //estadoEmisor = string.Empty,
                    regimenEmisior = string.Empty,

                    //formaPago = string.Empty,
                    metodoPago = string.Empty,
                    numCertificadoEmisor = "--",
                    //fechaComprobante = "--",
                    //serie = "--",
                    //folio = string.Empty,
                    fechaTimbrado = "--/--/----",
                    certificadoSat = "--",
                    uuid = "--",
                    selloCfd = string.Empty,
                    selloSat = string.Empty;

                string totalComprobante = string.Empty, totalPercepciones = string.Empty, totalDeducciones = string.Empty, totalOtrosPagos = string.Empty;

                string totalGravado = string.Empty, totalExento = string.Empty;

                //  string cadenaOriginal = string.Empty;

                #endregion

                #region SET DATOS NOMINA - EMISOR - RECEPTOR 

                //TAG Nomina --
                if (dataSet.Tables.Contains("Nomina"))
                {
                    foreach (DataRow renglon in dataSet.Tables["Nomina"].Rows)
                    {
                        //fechaIniPago = Convert.ToString(renglon["FechaInicialPago"]);
                        //fechaFinPago = Convert.ToString(renglon["FechaFinalPago"]);
                        //fechaPago = Convert.ToString(renglon["FechaPago"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalPercepciones"))
                            totalPercepciones = Convert.ToString(renglon["TotalPercepciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalDeducciones"))
                            totalDeducciones = Convert.ToString(renglon["TotalDeducciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalOtrosPagos"))
                            totalOtrosPagos = Convert.ToString(renglon["TotalOtrosPagos"]);
                    }
                }

                #region Tag Receptor

                //Tag RECEPTOR dentro del Tag COMPROBANTE
                foreach (DataRow recep in dataSet.Tables[3].Rows)
                {
                    nombreReceptor = Convert.ToString(recep["nombre"]);
                    rfcrecep = Convert.ToString(recep["rfc"]);
                }

                //TAG RECEPTOR dentro del tag NOMINA
                foreach (DataRow recep in dataSet.Tables[9].Rows)//
                {
                    cnx = recep["NumEmpleado"].ToString();
                    curp = Convert.ToString(recep["CURP"]);
                    depa = Convert.ToString(recep["Departamento"]);
                    puesto = Convert.ToString(recep["Puesto"]);
                    fechaInicioLaboral = Convert.ToString(recep["FechaInicioRelLaboral"]);
                    tipoJornada = Convert.ToString(recep["TipoJornada"]);
                    sdi = Convert.ToString(recep["SalarioDiarioIntegrado"]);

                    if (dataSet.Tables[9].Columns.Contains("NumSeguridadSocial"))
                    {
                        nss = Convert.ToString(recep["NumSeguridadSocial"]);
                    }
                    sbc = Convert.ToString(recep["SalarioBaseCotApor"]);
                }

                #endregion

                #region Tag  Emisor

                //TAG EMISOR dentro del tag de comprobante
                string nombreEmisor = "";

                foreach (DataRow emisor in dataSet.Tables[1].Rows)
                {
                    nombreEmisor = Convert.ToString(emisor["nombre"]);
                    rfcEmisor = emisor["rfc"].ToString();
                }

                //Segundo Tag EMISOR del tag de Nomina
                foreach (DataRow emisor in dataSet.Tables[8].Rows)
                {
                    try
                    {
                        registrosPatronal = Convert.ToString(emisor["RegistroPatronal"]);
                    }
                    catch (Exception ex)
                    {
                        registrosPatronal = "-";
                    }

                }

                //TAG Domicilio Fiscal
                //foreach (DataRow domi in dataSet.Tables["DomicilioFiscal"].Rows)
                //{
                //    municipioEmisor = Convert.ToString(domi["municipio"]);
                //    estadoEmisor = Convert.ToString(domi["estado"]);
                //}

                //TAG Regimen Fiscal
                if (dataSet.Tables.Contains("RegimenFiscal"))
                {
                    foreach (DataRow regimen in dataSet.Tables["RegimenFiscal"].Rows)
                    {
                        regimenEmisior = Convert.ToString(regimen["Regimen"]);
                    }
                }

                #endregion

                #endregion

                #region SET DATOS COMPROBANTE - TIMBRE FISCAL DIGITAL

                //TAG Comprobante
                if (dataSet.Tables.Contains("Comprobante"))
                {
                    if (dataSet.Tables["Comprobante"] != null)
                        foreach (DataRow renglon in dataSet.Tables["Comprobante"].Rows)
                        {
                            totalComprobante = Convert.ToString(renglon["total"]);
                            //formaPago = Convert.ToString(renglon["formaDePago"]);
                            metodoPago = Convert.ToString(renglon["metodoDePago"]);
                            numCertificadoEmisor = conTimbrado ? Convert.ToString(renglon["noCertificado"]) : "noCertificado?";
                            //fechaComprobante = Convert.ToString(renglon["fecha"]);
                            //serie = Convert.ToString(renglon["serie"]);
                            //folio = Convert.ToString(renglon["folio"]);
                            lugarExpedicion = Convert.ToString(renglon["LugarExpedicion"]);
                        }
                }

                //TAG Timbre Fiscal
                if (dataSet.Tables.Contains("TimbreFiscalDigital"))
                {
                    if (dataSet.Tables["TimbreFiscalDigital"] != null)
                        foreach (DataRow timbre in dataSet.Tables["TimbreFiscalDigital"].Rows)
                        {
                            fechaTimbrado = Convert.ToString(timbre["FechaTimbrado"]);
                            certificadoSat = Convert.ToString(timbre["noCertificadoSAT"]);
                            uuid = Convert.ToString(timbre["UUID"]);
                            selloCfd = Convert.ToString(timbre["selloCFD"]);
                            selloSat = Convert.ToString(timbre["selloSAT"]);
                        }
                }

                #endregion

                #region DISEÑO DEL CONTENIDO DE LA PÁGINA DEL DOCUMENTO PDF

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
                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 0,
                    Border = 0
                };

                tablaEmisor.AddCell(cell);
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd"), _font8))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("RFC: " + rfcEmisor, _font8B))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Pat: " + registrosPatronal, _font8B))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Fiscal: " + regimenEmisior, _font8))
                {
                    Colspan = 1,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("Lugar Expedicion: " + lugarExpedicion, _font8))
                {
                    Colspan = 1,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = 0,
                    Colspan = 4,
                    HorizontalAlignment = 2
                });

                documento.Add(tablaEmisor);

                //Rectangle rectangle = new Rectangle(37, 818, 557, 770) //37, 818, 557, 760
                //{
                //    BackgroundColor = colorBackAzulOscuro,
                //    BorderWidth = 1,
                //    Border = 3,
                //    BorderColor = BaseColor.BLACK
                //};

                // documento.Add(new Paragraph(" "));
                //documento.Add(rectangle);

                #endregion

                #region DATOS DEL RECEPTOR

                PdfPTable tblContendorReceptor = new PdfPTable(2);
                PdfPTable tblDatosReceptor = new PdfPTable(2);
                PdfPTable tblDatosReceptorNomina = new PdfPTable(2);

                tblDatosReceptor.DefaultCell.Border = 0;
                tblDatosReceptor.AddCell(
                    new PdfPCell(new Phrase("Num: " + cnx + " - " + nombreReceptor, _font8B)) { Colspan = 2, Border = 0 });
                tblDatosReceptor.AddCell(new Phrase("RFC :", _font7B));
                tblDatosReceptor.AddCell(new Phrase(rfcrecep, _font7));
                tblDatosReceptor.AddCell(new Phrase("CURP :", _font7B));
                tblDatosReceptor.AddCell(new Phrase(curp, _font7));
                tblDatosReceptor.AddCell(new Phrase("Fecha Inicio Laboral", _font7));
                // DateTime FI;

                tblDatosReceptor.AddCell(new Phrase(fechaInicioLaboral, _font7));
                tblDatosReceptor.AddCell(new Phrase("NSS :", _font7));
                tblDatosReceptor.AddCell(new Phrase(nss, _font7));
                tblDatosReceptor.AddCell(new Phrase("Tipo Jornada :", _font7));
                tblDatosReceptor.AddCell(new Phrase(tipoJornada, _font7));

                tblDatosReceptorNomina.DefaultCell.Border = 0;
                tblDatosReceptorNomina.AddCell(new Phrase("Periodo :", _font8B));

                var FPI = (DateTime)periodoDatos.Fecha_Inicio;
                var FPF = (DateTime)periodoDatos.Fecha_Fin;
                var FP = (DateTime)periodoDatos.Fecha_Pago;

                tblDatosReceptorNomina.AddCell(new Phrase(FPI.ToString("dd/MM/yyyy") + " - " + FPF.ToString("dd/MM/yyyy"), _font8B));
                tblDatosReceptorNomina.AddCell(new Phrase("Días de Pago : " + periodoDatos.DiasPeriodo.ToString(), _font7));
                tblDatosReceptorNomina.AddCell(new Phrase("Fecha Pago : " + FP.ToString("dd/MM/yyyy"), _font7));
                tblDatosReceptorNomina.AddCell(new Phrase("Puesto :", _font7));
                tblDatosReceptorNomina.AddCell(new Phrase(puesto, _font7));
                tblDatosReceptorNomina.AddCell(new Phrase("Departamento :", _font7));
                tblDatosReceptorNomina.AddCell(new Phrase(depa, _font7));
                tblDatosReceptorNomina.AddCell(new Phrase("Salario base cotización $:", _font7));
                tblDatosReceptorNomina.AddCell(new Phrase(sbc, _font7));
                tblDatosReceptorNomina.AddCell(new Phrase("Salario diario integrado $:", _font7));
                tblDatosReceptorNomina.AddCell(new Phrase(sdi, _font7));

                tblContendorReceptor.TotalWidth = anchoTabla;
                tblContendorReceptor.LockedWidth = true;

                tblContendorReceptor.AddCell(tblDatosReceptor);
                tblContendorReceptor.AddCell(tblDatosReceptorNomina);

                #endregion

                tblContendorReceptor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = 0,
                    Colspan = 2,
                    HorizontalAlignment = 2
                });
                documento.Add(tblContendorReceptor);
                //documento.Add(new Paragraph(" "));

                #region  TABLAS DE CONTENIDO - PERCEPCIONES - DEDUCCIONES - OTRO PAGO - TOTAL RECIBO

                var columnas = 8; //Numero de columnas de las tablas

                #region TABLA PERCEPCION
                //variables
                // var totalPercepcionGravado = 0.00;
                // var totalPercepcionExento = 0.00;
                // var totalPercepcionDecimal = 0.00;

                if (dataSet.Tables.Contains("Percepcion"))
                {

                    //1) definicion de la tabla
                    var tablaPercepciones = new PdfPTable(columnas);

                    //3) Subtitulos  - Cve SAT  - Concepto - Gravado - Exento - Total
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Cve", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Percepciones", _font8B))
                    {
                        Colspan = (columnas - 4),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Gravado", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Exento", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Importe", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - longitud de la tabla
                    tablaPercepciones.TotalWidth = anchoTabla;
                    tablaPercepciones.LockedWidth = true;



                    // Get Totales
                    if (dataSet.Tables["Percepciones"] != null)
                        foreach (DataRow perceps in dataSet.Tables["Percepciones"].Rows)
                        {
                            totalGravado = Convert.ToString(perceps["TotalGravado"]);//3
                            totalExento = Convert.ToString(perceps["TotalExento"]);//4
                        }

                    //5) Detalle conceptos
                    if (dataSet.Tables["Percepcion"] != null)
                        foreach (DataRow percep in dataSet.Tables["Percepcion"].Rows)
                        {
                            var importeExcento = double.Parse(percep[4].ToString());
                            var importeGravado = double.Parse(percep[3].ToString());
                            var concepto = Convert.ToString(percep[2]);
                            var claveSat = Convert.ToString(percep[1]);
                            var tipo = Convert.ToString(percep[0]);

                            var totalImporte = importeExcento + importeGravado;
                            // totalPercepcionDecimal += totalImporte;

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            // tblPercepciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 4)
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeGravado.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeExcento.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    //6) TOTAL PERCEPCIONES - GRAVA - EXENTA - TOTAL

                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 3) });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(str: totalGravado.ToCurrencyFormat(signo: true), font: _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalExento.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalPercepciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    //documento.Add(new Paragraph(" "));
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });

                    //8) Agregar la tabla al documento
                    documento.Add(tablaPercepciones);
                }

                #endregion

                #region TABLA DEDUCCION
                // Variables
                // var totalDeducciones = 0.0;

                if (dataSet.Tables.Contains("Deduccion"))
                {

                    //1) Definir la tabla
                    var tablaDeucciones = new PdfPTable(columnas);

                    //3) Subtitulos
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Cve", _font8B))
                    {
                        HorizontalAlignment = 1, //Centrado
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Deducciones", _font8B))
                    {
                        Colspan = (columnas - 2),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1 //Centrado
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Importe", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaDeucciones.TotalWidth = anchoTabla;
                    tablaDeucciones.LockedWidth = true;



                    //5) Detalle conceptos
                    if (dataSet.Tables["Deduccion"] != null)
                        foreach (DataRow deduc in dataSet.Tables["Deduccion"].Rows)
                        {
                            //var importeExcento = double.Parse(deduc[0].ToString());
                            //var importeGravado = double.Parse(deduc[1].ToString());
                            var totalImporte = double.Parse(deduc[3].ToString());
                            var concepto = Convert.ToString(deduc[2]);
                            var claveSat = Convert.ToString(deduc[1]);
                            var tipo = Convert.ToString(deduc[0]);

                            //  totalDeducciones += totalImporte;

                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7)) { Border = 0 });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }


                    //6) Totales Deducciones
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalDeducciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });


                    //7) Agregar espacio entre tablas
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });
                    //documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaDeucciones);

                }
                #endregion

                #region TABLA OTROS PAGOS

                // Variables
                // var totalOtroPago = 0.0;

                if (dataSet.Tables.Contains("OtroPago"))
                {

                    //1) definicion de la tabla
                    var tablaOtrosPagos = new PdfPTable(columnas);


                    //3) Subtitulos
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Cve", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Otros Pagos", _font8B))
                    {
                        Colspan = (columnas - 2),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1 //Centrado
                    });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Importe", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaOtrosPagos.TotalWidth = anchoTabla;
                    tablaOtrosPagos.LockedWidth = true;

                    //5) Detalle conceptos
                    if (dataSet.Tables["OtroPago"] != null)
                        foreach (DataRow otroPago in dataSet.Tables["OtroPago"].Rows)
                        {
                            var totalImporte = double.Parse(otroPago[4].ToString());
                            var concepto = Convert.ToString(otroPago[3]);
                            var claveSat = Convert.ToString(otroPago[2]);
                            //totalOtroPago += totalImporte;

                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalOtrosPagos.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });
                    // documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaOtrosPagos);
                }

                #endregion

                #region TOTAL

                // Variables
                //var totalRecibo = 0.0;

                //1) definicion de la tabla
                // var tablaTotalRecibo = new PdfPTable(columnas);

                //2) Set - Longitud de la Tabla
                //tablaTotalRecibo.TotalWidth = anchoTabla;
                //tablaTotalRecibo.LockedWidth = true;

                // totalRecibo = (totalPercepcionExento + totalPercepcionGravado + totalOtroPago) - totalDeducciones;
                // 3) Detalle total
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase("TOTAL $", _font10B)) { Border = 0, Colspan = (columnas-1), HorizontalAlignment = 2 });
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase($"{totalRecibo:0,0.##}", _font10B)) { Border = 0, Colspan = 1, HorizontalAlignment = 2 });

                //4 Agregar espacio entre tablas


                //5) Agregar la tabla al documento
                // documento.Add(tablaTotalRecibo);

                #endregion
                documento.Add(new Paragraph(" "));

                #endregion

                #region GENERA LOGO CBB

                string textCode = @"?re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&id=" + uuid;

                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(textCode, out qrCode);
                GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Four), Brushes.Black, Brushes.White);

                //new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Two)).Draw(g, qrCode.Matrix);

                Stream memoryStream = new MemoryStream();

                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
                memoryStream.Position = 0;

                #endregion

                #region "FIRMA DEL EMPLEADO -  TOTAL RECIBO"

                var tblFirma = new PdfPTable(3);

                tblFirma.TotalWidth = anchoTabla;
                tblFirma.LockedWidth = true;

                tblFirma.AddCell(
                    new PdfPCell(
                        new Phrase(
                            "Recibí de " + registrosPatronal +
                            " por concepto de pago total de mi salario y demás percepciones del periodo" +
                            " indicado sin que a la fecha se me adeude cantidad alguna por ningún concepto.",
                            _font6))
                    {
                        Border = 0,
                        HorizontalAlignment = 0
                    });
                tblFirma.AddCell(
                    new PdfPCell(new Phrase("_______________________________________\n Firma del colaborador", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 1
                    });



                tblFirma.AddCell(new PdfPCell(new Phrase("TOTAL $ " + totalComprobante.ToCurrencyFormat(), _font13B))
                {
                    Border = 0,
                    Colspan = 1,
                    HorizontalAlignment = 2
                });

                documento.Add(tblFirma);

                documento.Add(new Paragraph(" "));

                #endregion

                #region AGREGA LOGO CBB Y DATOS DE CERTIFICACION DEL CFDI 

                var tblCbb = new PdfPTable(3);
                var tblCfdidatos = new PdfPTable(2);
                tblCbb.TotalWidth = anchoTabla;
                tblCbb.LockedWidth = true;
                tblCbb.DefaultCell.Border = 0;

                Image logoCBB;
                // logoCBB = iTextSharp.text.Image.GetInstance(_pathArchivosXml + @"\\ImagenesCBB\\" + rfcrecep + ".jpg");

                logoCBB = Image.GetInstance(memoryStream);
                logoCBB.ScaleToFit(150f, 150f);

                tblCbb.AddCell(new PdfPCell(logoCBB) { Border = 0 });

                var leyendaTabla = conTimbrado ? "Este documento es una representacion impresa de un CFDI" : "-";

                tblCfdidatos.AddCell(new PdfPCell(new Phrase(leyendaTabla, _font7B)) { Colspan = 2, BackgroundColor = colorBackAzulOscuro });

                var tbldatos = new PdfPTable(1);
                tbldatos.AddCell(new PdfPCell(new Phrase("Serie del Certificado del Emisor:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Folio Fiscal UUID:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("No certificado del SAT:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Fecha y Hora de certificación:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Método de pago:", _font7)) { Border = 0, HorizontalAlignment = 2 });

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

                tblCbb.AddCell(new PdfPCell(tblCfdidatos) { Colspan = 2 });

                documento.Add(tblCbb);

                documento.Add(new Paragraph(" "));

                #endregion

                #region SELLOS Y CADENA ORIGINAL

                var tblSellos = new PdfPTable(1);
                tblSellos.TotalWidth = anchoTabla;
                tblSellos.LockedWidth = true;

                tblSellos.AddCell(new PdfPCell(new Phrase("Sello Digital del CFDI", _font7B))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloCfd, _font7)) { Border = 0 });
                tblSellos.AddCell(new PdfPCell(new Phrase("    ", _font7B)) { Border = 0 });

                tblSellos.AddCell(new PdfPCell(new Phrase("Sello del SAT", _font7B))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloSat, _font7)) { Border = 0 });
                tblSellos.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0 });

                tblSellos.AddCell(
                    new PdfPCell(new Phrase("Cadena Original del complemento del certificado del SAT", _font7B))
                    {
                        Border = 0,
                        BackgroundColor = colorBackAzulOscuro
                    });
                tblSellos.AddCell(new PdfPCell(new Phrase(cadenaOriginal, _font7)) { Border = 0 });

                documento.Add(tblSellos);

                #endregion

                #endregion


            }
            catch (Exception ex)
            {
                documento.Add(new Paragraph($" ? - Nomina: {idNomina}  Periodo:{periodoDatos.IdSucursal}"));
            }
            finally
            {
                documento.NewPage();

                dataSet.Dispose();
                dataSet.Clear();
            }

        }

        private void AddReciboPdfAsimilados(ref Document documento, string strXml, NOM_PeriodosPago periodoDatos,
    List<C_Metodos_Pago> listaMetodosPagos, int idNomina = 0, string cadenaOriginal = "cadena original", bool conTimbrado = false)
        {
            DataSet dataSet = new DataSet();
            try
            {
                int anchoTabla = 520;
                string lugarExpedicion = "";
                BaseColor colorBackAzulOscuro = BaseColor.LIGHT_GRAY;

                //Carga el contenido del XML en el DATASET
                StringReader streamReader = new StringReader(strXml);
                dataSet.ReadXml(streamReader);

                #region "VARIABLES"

                string cnx = string.Empty,
                    //fechaIniPago = string.Empty,
                    //fechaFinPago = string.Empty,
                    //fechaPago = string.Empty,
                    curp = string.Empty,
                    //depa = string.Empty,
                    //puesto = string.Empty,
                    //registrosPatronal = string.Empty,
                    fechaInicioLaboral = string.Empty;
                //tipoJornada = string.Empty;
                //sdi = string.Empty,
                //sbc = string.Empty,
                //nss = string.Empty;

                string nombreReceptor = string.Empty,
                    rfcrecep = string.Empty,
                    rfcEmisor = string.Empty,
                    //municipioEmisor = string.Empty,
                    //estadoEmisor = string.Empty,
                    regimenEmisior = string.Empty,

                    //formaPago = string.Empty,
                    metodoPago = string.Empty,
                    numCertificadoEmisor = "--",
                    //fechaComprobante = "--",
                    //serie = "--",
                    //folio = string.Empty,
                    fechaTimbrado = "--/--/----",
                    certificadoSat = "--",
                    uuid = "--",
                    selloCfd = string.Empty,
                    selloSat = string.Empty;

                string totalComprobante = string.Empty, totalPercepciones = string.Empty, totalDeducciones = string.Empty, totalOtrosPagos = string.Empty;

                string totalGravado = string.Empty, totalExento = string.Empty;

                //  string cadenaOriginal = string.Empty;

                #endregion

                #region SET DATOS NOMINA - EMISOR - RECEPTOR 

                //TAG Nomina --
                if (dataSet.Tables.Contains("Nomina"))
                {
                    foreach (DataRow renglon in dataSet.Tables["Nomina"].Rows)
                    {
                        //fechaIniPago = Convert.ToString(renglon["FechaInicialPago"]);
                        //fechaFinPago = Convert.ToString(renglon["FechaFinalPago"]);
                        //fechaPago = Convert.ToString(renglon["FechaPago"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalPercepciones"))
                            totalPercepciones = Convert.ToString(renglon["TotalPercepciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalDeducciones"))
                            totalDeducciones = Convert.ToString(renglon["TotalDeducciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalOtrosPagos"))
                            totalOtrosPagos = Convert.ToString(renglon["TotalOtrosPagos"]);
                    }
                }

                #region Tag Receptor

                //Tag RECEPTOR dentro del Tag COMPROBANTE
                foreach (DataRow recep in dataSet.Tables[3].Rows)
                {
                    nombreReceptor = Convert.ToString(recep["nombre"]);
                    rfcrecep = Convert.ToString(recep["rfc"]);
                }

                //TAG RECEPTOR dentro del tag NOMINA
                foreach (DataRow recep in dataSet.Tables[8].Rows)//
                {
                    cnx = recep["NumEmpleado"].ToString();
                    curp = Convert.ToString(recep["CURP"]);
                    //fechaInicioLaboral = Convert.ToString(recep["FechaInicioRelLaboral"]);
                }

                #endregion

                #region Tag  Emisor

                //TAG EMISOR dentro del tag de comprobante
                string nombreEmisor = "";

                foreach (DataRow emisor in dataSet.Tables[1].Rows)
                {
                    nombreEmisor = Convert.ToString(emisor["nombre"]);
                    rfcEmisor = emisor["rfc"].ToString();
                }



                //TAG Domicilio Fiscal
                //foreach (DataRow domi in dataSet.Tables["DomicilioFiscal"].Rows)
                //{
                //    municipioEmisor = Convert.ToString(domi["municipio"]);
                //    estadoEmisor = Convert.ToString(domi["estado"]);
                //}

                //TAG Regimen Fiscal
                if (dataSet.Tables.Contains("RegimenFiscal"))
                {
                    foreach (DataRow regimen in dataSet.Tables["RegimenFiscal"].Rows)
                    {
                        regimenEmisior = Convert.ToString(regimen["Regimen"]);
                    }
                }

                #endregion

                #endregion

                #region SET DATOS COMPROBANTE - TIMBRE FISCAL DIGITAL

                //TAG Comprobante
                if (dataSet.Tables.Contains("Comprobante"))
                {
                    if (dataSet.Tables["Comprobante"] != null)
                        foreach (DataRow renglon in dataSet.Tables["Comprobante"].Rows)
                        {
                            totalComprobante = Convert.ToString(renglon["total"]);
                            //formaPago = Convert.ToString(renglon["formaDePago"]);
                            metodoPago = Convert.ToString(renglon["metodoDePago"]);
                            numCertificadoEmisor = conTimbrado ? Convert.ToString(renglon["noCertificado"]) : "noCertificado?";
                            //fechaComprobante = Convert.ToString(renglon["fecha"]);
                            //serie = Convert.ToString(renglon["serie"]);
                            //folio = Convert.ToString(renglon["folio"]);
                            lugarExpedicion = Convert.ToString(renglon["LugarExpedicion"]);
                        }
                }

                //TAG Timbre Fiscal
                if (dataSet.Tables.Contains("TimbreFiscalDigital"))
                {
                    if (dataSet.Tables["TimbreFiscalDigital"] != null)
                        foreach (DataRow timbre in dataSet.Tables["TimbreFiscalDigital"].Rows)
                        {
                            fechaTimbrado = Convert.ToString(timbre["FechaTimbrado"]);
                            certificadoSat = Convert.ToString(timbre["noCertificadoSAT"]);
                            uuid = Convert.ToString(timbre["UUID"]);
                            selloCfd = Convert.ToString(timbre["selloCFD"]);
                            selloSat = Convert.ToString(timbre["selloSAT"]);
                        }
                }

                #endregion

                #region DISEÑO DEL CONTENIDO DE LA PÁGINA DEL DOCUMENTO PDF

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
                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 0,
                    Border = 0
                };

                tablaEmisor.AddCell(cell);
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd"), _font8))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("RFC: " + rfcEmisor, _font8B))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font8B))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Fiscal: " + regimenEmisior, _font8))
                {
                    Colspan = 1,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("Lugar Expedicion: " + lugarExpedicion, _font8))
                {
                    Colspan = 1,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font7))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = 0,
                    Colspan = 4,
                    HorizontalAlignment = 2
                });

                documento.Add(tablaEmisor);

                //Rectangle rectangle = new Rectangle(37, 818, 557, 770) //37, 818, 557, 760
                //{
                //    BackgroundColor = colorBackAzulOscuro,
                //    BorderWidth = 1,
                //    Border = 3,
                //    BorderColor = BaseColor.BLACK
                //};

                // documento.Add(new Paragraph(" "));
                //documento.Add(rectangle);

                #endregion

                #region DATOS DEL RECEPTOR

                PdfPTable tblContendorReceptor = new PdfPTable(2);
                PdfPTable tblDatosReceptor = new PdfPTable(2);
                PdfPTable tblDatosReceptorNomina = new PdfPTable(2);

                tblDatosReceptor.DefaultCell.Border = 0;
                tblDatosReceptor.AddCell(
                    new PdfPCell(new Phrase("Num: " + cnx + " - " + nombreReceptor, _font8B)) { Colspan = 2, Border = 0 });
                tblDatosReceptor.AddCell(new Phrase("RFC :", _font7B));
                tblDatosReceptor.AddCell(new Phrase(rfcrecep, _font7));
                tblDatosReceptor.AddCell(new Phrase("CURP :", _font7B));
                tblDatosReceptor.AddCell(new Phrase(curp, _font7));
                //tblDatosReceptor.AddCell(new Phrase("", _font7));//Fecha Inicio Laboral
                //tblDatosReceptor.AddCell(new Phrase(fechaInicioLaboral, _font7));

                tblDatosReceptorNomina.DefaultCell.Border = 0;
                tblDatosReceptorNomina.AddCell(new Phrase("Periodo :", _font8B));

                var FPI = (DateTime)periodoDatos.Fecha_Inicio;
                var FPF = (DateTime)periodoDatos.Fecha_Fin;
                var FP = (DateTime)periodoDatos.Fecha_Pago;

                tblDatosReceptorNomina.AddCell(new Phrase("MENSUAL", _font8B));
                tblDatosReceptorNomina.AddCell(new Phrase("Días de Pago : ", _font7));
                tblDatosReceptorNomina.AddCell(new Phrase("Fecha Pago : " + FP.ToString("dd/MM/yyyy"), _font7));

                tblContendorReceptor.TotalWidth = anchoTabla;
                tblContendorReceptor.LockedWidth = true;

                tblContendorReceptor.AddCell(tblDatosReceptor);
                tblContendorReceptor.AddCell(tblDatosReceptorNomina);

                #endregion

                tblContendorReceptor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = 0,
                    Colspan = 2,
                    HorizontalAlignment = 2
                });
                documento.Add(tblContendorReceptor);
                //documento.Add(new Paragraph(" "));

                #region  TABLAS DE CONTENIDO - PERCEPCIONES - DEDUCCIONES - OTRO PAGO - TOTAL RECIBO

                var columnas = 8; //Numero de columnas de las tablas

                #region TABLA PERCEPCION
                //variables
                // var totalPercepcionGravado = 0.00;
                // var totalPercepcionExento = 0.00;
                // var totalPercepcionDecimal = 0.00;

                if (dataSet.Tables.Contains("Percepcion"))
                {

                    //1) definicion de la tabla
                    var tablaPercepciones = new PdfPTable(columnas);

                    //3) Subtitulos  - Cve SAT  - Concepto - Gravado - Exento - Total
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Cve", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Percepciones", _font8B))
                    {
                        Colspan = (columnas - 4),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Gravado", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Exento", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Importe", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - longitud de la tabla
                    tablaPercepciones.TotalWidth = anchoTabla;
                    tablaPercepciones.LockedWidth = true;



                    // Get Totales
                    if (dataSet.Tables["Percepciones"] != null)
                        foreach (DataRow perceps in dataSet.Tables["Percepciones"].Rows)
                        {
                            totalGravado = Convert.ToString(perceps["TotalGravado"]);//3
                            totalExento = Convert.ToString(perceps["TotalExento"]);//4
                        }

                    //5) Detalle conceptos
                    if (dataSet.Tables["Percepcion"] != null)
                        foreach (DataRow percep in dataSet.Tables["Percepcion"].Rows)
                        {
                            var importeExcento = double.Parse(percep[4].ToString());
                            var importeGravado = double.Parse(percep[3].ToString());
                            var concepto = Convert.ToString(percep[2]);
                            var claveSat = Convert.ToString(percep[1]);
                            var tipo = Convert.ToString(percep[0]);

                            var totalImporte = importeExcento + importeGravado;
                            // totalPercepcionDecimal += totalImporte;

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            // tblPercepciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 4)
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeGravado.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeExcento.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    //6) TOTAL PERCEPCIONES - GRAVA - EXENTA - TOTAL

                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 3) });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(str: totalGravado.ToCurrencyFormat(signo: true), font: _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalExento.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalPercepciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    //documento.Add(new Paragraph(" "));
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });

                    //8) Agregar la tabla al documento
                    documento.Add(tablaPercepciones);
                }

                #endregion

                #region TABLA DEDUCCION
                // Variables
                // var totalDeducciones = 0.0;

                if (dataSet.Tables.Contains("Deduccion"))
                {

                    //1) Definir la tabla
                    var tablaDeucciones = new PdfPTable(columnas);

                    //3) Subtitulos
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Cve", _font8B))
                    {
                        HorizontalAlignment = 1, //Centrado
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Deducciones", _font8B))
                    {
                        Colspan = (columnas - 2),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1 //Centrado
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Importe", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaDeucciones.TotalWidth = anchoTabla;
                    tablaDeucciones.LockedWidth = true;



                    //5) Detalle conceptos
                    if (dataSet.Tables["Deduccion"] != null)
                        foreach (DataRow deduc in dataSet.Tables["Deduccion"].Rows)
                        {
                            //var importeExcento = double.Parse(deduc[0].ToString());
                            //var importeGravado = double.Parse(deduc[1].ToString());
                            var totalImporte = double.Parse(deduc[3].ToString());
                            var concepto = Convert.ToString(deduc[2]);
                            var claveSat = Convert.ToString(deduc[1]);
                            var tipo = Convert.ToString(deduc[0]);

                            //  totalDeducciones += totalImporte;

                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7)) { Border = 0 });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }


                    //6) Totales Deducciones
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalDeducciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });


                    //7) Agregar espacio entre tablas
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });
                    //documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaDeucciones);

                }
                #endregion

                #region TABLA OTROS PAGOS

                // Variables
                // var totalOtroPago = 0.0;

                if (dataSet.Tables.Contains("OtroPago"))
                {

                    //1) definicion de la tabla
                    var tablaOtrosPagos = new PdfPTable(columnas);


                    //3) Subtitulos
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Cve", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Otros Pagos", _font8B))
                    {
                        Colspan = (columnas - 2),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1 //Centrado
                    });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Importe", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaOtrosPagos.TotalWidth = anchoTabla;
                    tablaOtrosPagos.LockedWidth = true;

                    //5) Detalle conceptos
                    if (dataSet.Tables["OtroPago"] != null)
                        foreach (DataRow otroPago in dataSet.Tables["OtroPago"].Rows)
                        {
                            var totalImporte = double.Parse(otroPago[4].ToString());
                            var concepto = Convert.ToString(otroPago[3]);
                            var claveSat = Convert.ToString(otroPago[2]);
                            //totalOtroPago += totalImporte;

                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalOtrosPagos.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });
                    // documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaOtrosPagos);
                }

                #endregion

                #region TOTAL

                // Variables
                //var totalRecibo = 0.0;

                //1) definicion de la tabla
                // var tablaTotalRecibo = new PdfPTable(columnas);

                //2) Set - Longitud de la Tabla
                //tablaTotalRecibo.TotalWidth = anchoTabla;
                //tablaTotalRecibo.LockedWidth = true;

                // totalRecibo = (totalPercepcionExento + totalPercepcionGravado + totalOtroPago) - totalDeducciones;
                // 3) Detalle total
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase("TOTAL $", _font10B)) { Border = 0, Colspan = (columnas-1), HorizontalAlignment = 2 });
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase($"{totalRecibo:0,0.##}", _font10B)) { Border = 0, Colspan = 1, HorizontalAlignment = 2 });

                //4 Agregar espacio entre tablas


                //5) Agregar la tabla al documento
                // documento.Add(tablaTotalRecibo);

                #endregion
                documento.Add(new Paragraph(" "));

                #endregion

                #region GENERA LOGO CBB

                string textCode = @"?re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&id=" + uuid;

                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(textCode, out qrCode);
                GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Four), Brushes.Black, Brushes.White);

                //new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Two)).Draw(g, qrCode.Matrix);

                Stream memoryStream = new MemoryStream();

                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
                memoryStream.Position = 0;

                #endregion

                #region "FIRMA DEL EMPLEADO -  TOTAL RECIBO"

                var tblFirma = new PdfPTable(3);

                tblFirma.TotalWidth = anchoTabla;
                tblFirma.LockedWidth = true;

                tblFirma.AddCell(
                    new PdfPCell(
                        new Phrase(
                            "Recibi de la empresa " + nombreEmisor + " por concepto de Honorarios asimilados del periodo indicado sin que a la fecha se me adeude la cantidad alguna por ningún concepto",
                            _font6))
                    {
                        Border = 0,
                        HorizontalAlignment = 0
                    });
                tblFirma.AddCell(
                    new PdfPCell(new Phrase("_______________________________________\n Firma del colaborador", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 1
                    });



                tblFirma.AddCell(new PdfPCell(new Phrase("TOTAL $ " + totalComprobante.ToCurrencyFormat(), _font13B))
                {
                    Border = 0,
                    Colspan = 1,
                    HorizontalAlignment = 2
                });

                documento.Add(tblFirma);

                documento.Add(new Paragraph(" "));

                #endregion

                #region AGREGA LOGO CBB Y DATOS DE CERTIFICACION DEL CFDI 

                var tblCbb = new PdfPTable(3);
                var tblCfdidatos = new PdfPTable(2);
                tblCbb.TotalWidth = anchoTabla;
                tblCbb.LockedWidth = true;
                tblCbb.DefaultCell.Border = 0;

                Image logoCBB;
                // logoCBB = iTextSharp.text.Image.GetInstance(_pathArchivosXml + @"\\ImagenesCBB\\" + rfcrecep + ".jpg");

                logoCBB = Image.GetInstance(memoryStream);
                logoCBB.ScaleToFit(150f, 150f);

                tblCbb.AddCell(new PdfPCell(logoCBB) { Border = 0 });

                var leyendaTabla = conTimbrado ? "Este documento es una representacion impresa de un CFDI" : "-";

                tblCfdidatos.AddCell(new PdfPCell(new Phrase(leyendaTabla, _font7B)) { Colspan = 2, BackgroundColor = colorBackAzulOscuro });

                var tbldatos = new PdfPTable(1);
                tbldatos.AddCell(new PdfPCell(new Phrase("Serie del Certificado del Emisor:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Folio Fiscal UUID:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("No certificado del SAT:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Fecha y Hora de certificación:", _font7)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Método de pago:", _font7)) { Border = 0, HorizontalAlignment = 2 });

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

                tblCbb.AddCell(new PdfPCell(tblCfdidatos) { Colspan = 2 });

                documento.Add(tblCbb);

                documento.Add(new Paragraph(" "));

                #endregion

                #region SELLOS Y CADENA ORIGINAL

                var tblSellos = new PdfPTable(1);
                tblSellos.TotalWidth = anchoTabla;
                tblSellos.LockedWidth = true;

                tblSellos.AddCell(new PdfPCell(new Phrase("Sello Digital del CFDI", _font7B))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloCfd, _font7)) { Border = 0 });
                tblSellos.AddCell(new PdfPCell(new Phrase("    ", _font7B)) { Border = 0 });

                tblSellos.AddCell(new PdfPCell(new Phrase("Sello del SAT", _font7B))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloSat, _font7)) { Border = 0 });
                tblSellos.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0 });

                tblSellos.AddCell(
                    new PdfPCell(new Phrase("Cadena Original del complemento del certificado del SAT", _font7B))
                    {
                        Border = 0,
                        BackgroundColor = colorBackAzulOscuro
                    });
                tblSellos.AddCell(new PdfPCell(new Phrase(cadenaOriginal, _font7)) { Border = 0 });

                documento.Add(tblSellos);

                #endregion

                #endregion


            }
            catch (Exception ex)
            {
                documento.Add(new Paragraph($" ? - Nomina: {idNomina}  Periodo:{periodoDatos.IdSucursal}"));
            }
            finally
            {
                documento.NewPage();

                dataSet.Dispose();
                dataSet.Clear();
            }

        }

        private void AddReciboPdfComplemento(ref Document documento, string nombreEmpresa, string nombreEmpleado, string Clave, decimal cantidadSubida, decimal cantidadProcesada, DateTime fecha, string lugarExp, List<DetalleNominaComp> listaDetalles, bool incluirDetalles)
        {

            //GET - datos detalle de la nomina
            if (incluirDetalles == false)
                cantidadSubida = cantidadProcesada; // Para MTY

            int anchoTabla = 520;
            BaseColor colorBackAzulOscuro = BaseColor.LIGHT_GRAY;

            //Tabla del recibo
            PdfPTable tablaRecibo = new PdfPTable(2)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };
            //Tabla del recibo
            PdfPTable tablaReciboc = new PdfPTable(2)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            //Celda Nombre de la empresa
            tablaRecibo.AddCell(new PdfPCell(new Phrase(nombreEmpresa, _font10B))
            {
                Colspan = 2,
                BackgroundColor = colorBackAzulOscuro,
                Border = 0,
                HorizontalAlignment = 1//Centrado
            });

            //Nombre del colaborador
            tablaRecibo.AddCell(new PdfPCell(new Phrase("NOMBRE COLABORADOR: " + nombreEmpleado, _font8))
            {
                Border = 1,
                HorizontalAlignment = 0,
                FixedHeight = 18,
            });
            tablaRecibo.AddCell(new PdfPCell(new Phrase("ID: " + Clave, _font8))
            {
                Border = 1,
                HorizontalAlignment = 0,
                FixedHeight = 18
            });

            // titulo Recibo de Pago
            tablaRecibo.AddCell(new PdfPCell(new Phrase("RECIBO DE PAGO  ", _font14))
            {
                Colspan = 2,
                Border = 1,
                BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 25,
            });


            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                HorizontalAlignment = 2,
                FixedHeight = 20,
                Colspan = 2
            });

            //Contenido - Dividir en dos Celdas la tabla principal
            //Leyendas Recibi la cantidad, Expedido en
            tablaRecibo.AddCell(new PdfPCell(new Phrase("RECIBÍ LA CANTIDAD DE :", _font8))
            {
                Border = 0,
                HorizontalAlignment = 1,
            });

            tablaRecibo.AddCell(new PdfPCell(new Phrase("EXPEDIDO EN LA CIUDAD DE : \n" + lugarExp, _font8))
            {
                Border = 0,
                HorizontalAlignment = 1
            });


            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                HorizontalAlignment = 2,
                FixedHeight = 20,
                Colspan = 2
            });


            // Celda Vacia a la izquierda
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                HorizontalAlignment = 2
            });

            // Palabra FECHA
            tablaRecibo.AddCell(new PdfPCell(new Phrase("FECHA", _font8))
            {
                Border = 0,
                HorizontalAlignment = 1
            });

            //CANTIDAD
            tablaRecibo.AddCell(new PdfPCell(new Phrase(cantidadSubida.ToCurrencyFormat(signo: true), _font10B))
            {
                Border = 0,
                HorizontalAlignment = 1

            });

            //FECHA
            string fechaPagoRecibo = fecha.ToString("dd   MMMM  yyyy", new CultureInfo("es-ES", false));

            tablaRecibo.AddCell(new PdfPCell(new Phrase(fechaPagoRecibo, _font10B))
            { Border = 0, HorizontalAlignment = 1 });


            // espacio verticales
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, FixedHeight = 30, Colspan = 2 });


            #region DETALLE DE LA NOMINA POR CONCEPTOS

            if (incluirDetalles == true)
            {
                if (listaDetalles.Count > 0)
                {
                    //AQUI LA LISTA DE DETALLE
                    PdfPTable tablaDetalle = new PdfPTable(3)
                    {
                        TotalWidth = anchoTabla,
                        LockedWidth = true
                    };

                    //FixedHeight = 50
                    tablaDetalle.AddCell(new PdfPCell(new Phrase("Conceptos", _font8))
                    {
                        Border = 0,
                        BackgroundColor = colorBackAzulOscuro,
                        Colspan = 3
                    });


                    foreach (var itemDetalle in listaDetalles)
                    {
                        string strCantidad = itemDetalle.Cantidad.ToCurrencyFormat();
                        if (itemDetalle.TipoConcepto == 2)
                        {
                            strCantidad = "- " + strCantidad;
                        }

                        tablaDetalle.AddCell(new PdfPCell(new Phrase(itemDetalle.DescripcionConcepto, _font8))
                        {
                            Border = 0
                        });
                        tablaDetalle.AddCell(new PdfPCell(new Phrase(strCantidad, _font8))
                        {
                            Border = 0,
                            HorizontalAlignment = 2
                        });
                        tablaDetalle.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0 });
                    }


                    //Agrega la tabla detalle al 
                    tablaRecibo.AddCell(new PdfPCell(tablaDetalle) { Colspan = 2, Border = 0 });

                    // espacio verticales
                    tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, FixedHeight = 30, Colspan = 2 });

                }
            }

            #endregion


            //CANTIDAD EN LETRAS
            var cantidadLetra = Utils.ConvertCantidadALetras(cantidadSubida.ToCurrencyFormat());
            tablaRecibo.AddCell(new PdfPCell(new Phrase("IMPORTE CON LETRAS: \n" + cantidadLetra, _font8))
            {
                Colspan = 2,
                Border = 0,
                HorizontalAlignment = 0
            });


            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = PdfPCell.BOTTOM_BORDER,
                FixedHeight = 15,
                Colspan = 2
            });

            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                FixedHeight = 15,
                Colspan = 2
            });

            //Leyenda Recibi por concepto de
            tablaRecibo.AddCell(new PdfPCell(new Phrase("Recibí por concepto de pago total sin que a la fecha se me adeude cantidad alguna.  ", _font8))
            {
                Border = 0,
                HorizontalAlignment = 0
            });

            //Firma del empleado
            tablaRecibo.AddCell(new PdfPCell(new Phrase("______________________________\n Firma del Colaborador  ", _font8))
            {
                Border = 0,
                HorizontalAlignment = 1
            });

            tablaReciboc = tablaRecibo;
            tablaRecibo.SpacingAfter = 140f;
            

            //IMAGEN DE SINDICATO
            if (nombreEmpresa.ToUpper().Contains("SINDICATO"))
            {
                try
                {
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance("C:\\SUNFILES\\Certificados\\Logo.jpg");
                    jpg.ScaleToFit(100, 100);
                    //jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                    jpg.SetAbsolutePosition(250, 590);
                    //pdf.GetOverContent(1).AddImage(img);
                    documento.Add(jpg);
                }
                catch (Exception e)
                {

                }
            }

            documento.Add(tablaRecibo);
            documento.Add(tablaReciboc);
            
            

        }

        private void AddReciboPdfComplementoSindicato(ref Document documento, string nombreEmpresa, string nombreEmpleado, string Clave, decimal cantidadSubida, decimal cantidadProcesada, DateTime fecha, string lugarExp)
        {
            // Calculo para sindicato
            string[] strConceptos = {"Apoyo de alimentos","Apoyo para Herramientas de trabajo","Beca" };
            decimal[] importes = new decimal[3];
            int divisor = 3;
            decimal ht = cantidadProcesada * .90M;
            decimal restante = (cantidadProcesada - ht) / divisor;
            while (restante > 7160)
            {
                divisor++;
                restante = (cantidadProcesada-ht) / divisor;
            }
            importes[0]=restante * (divisor-1);
            importes[1]=ht;
            importes[2]=restante;
            /////////////////////////////////////////////////////////////////////////////////
            int anchoTabla = 520;
            BaseColor colorBackAzulOscuro = BaseColor.LIGHT_GRAY;

            //Tabla del recibo
            PdfPTable tablaRecibo = new PdfPTable(2)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };
            //Tabla del recibo
            PdfPTable tablaReciboc = new PdfPTable(2)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            //Celda Nombre de la empresa
            tablaRecibo.AddCell(new PdfPCell(new Phrase(nombreEmpresa, _font10B))
            {
                Colspan = 2,
                BackgroundColor = colorBackAzulOscuro,
                Border = 0,
                HorizontalAlignment = 1//Centrado
            });

            //Nombre del colaborador
            tablaRecibo.AddCell(new PdfPCell(new Phrase("NOMBRE COLABORADOR: " + nombreEmpleado, _font8))
            {
                Border = 1,
                HorizontalAlignment = 0,
                FixedHeight = 18,
            });
            tablaRecibo.AddCell(new PdfPCell(new Phrase("ID: " + Clave, _font8))
            {
                Border = 1,
                HorizontalAlignment = 0,
                FixedHeight = 18
            });

            // titulo Recibo de Pago
            tablaRecibo.AddCell(new PdfPCell(new Phrase("RECIBO DE PAGO  ", _font14))
            {
                Colspan = 2,
                Border = 1,
                BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 25,
            });


            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                HorizontalAlignment = 2,
                FixedHeight = 20,
                Colspan = 2
            });

            //Contenido - Dividir en dos Celdas la tabla principal
            //Leyendas Recibi la cantidad, Expedido en     
            #region DETALLE DE LA NOMINA POR CONCEPTOS


            //AQUI LA LISTA DE DETALLE
            PdfPTable tablaDetalle = new PdfPTable(3)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };

            //FixedHeight = 50
            tablaDetalle.AddCell(new PdfPCell(new Phrase("Conceptos", _font8))
            {
                Border = 0,
                BackgroundColor = colorBackAzulOscuro,
                Colspan = 3
            });


            for (int i = 0; i < 3; i++)
            {
                string strCantidad = importes[i].ToCurrencyFormat(decimales: 4);


                tablaDetalle.AddCell(new PdfPCell(new Phrase(strConceptos[i], _font8))
                {
                    Border = 0
                });
                tablaDetalle.AddCell(new PdfPCell(new Phrase(strCantidad, _font8))
                {
                    Border = 0,
                    HorizontalAlignment = 2
                });
                tablaDetalle.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0 });
            }


            //Agrega la tabla detalle al 
            tablaRecibo.AddCell(new PdfPCell(tablaDetalle) { Colspan = 2, Border = 0 });

            // espacio verticales
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, FixedHeight = 30, Colspan = 2 });



            #endregion
            tablaRecibo.AddCell(new PdfPCell(new Phrase("RECIBÍ LA CANTIDAD DE :", _font8))
            {
                Border = 0,
                HorizontalAlignment = 1,
            });

            tablaRecibo.AddCell(new PdfPCell(new Phrase("EXPEDIDO EN LA CIUDAD DE : \n" + lugarExp, _font8))
            {
                Border = 0,
                HorizontalAlignment = 1
            });


            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                HorizontalAlignment = 2,
                FixedHeight = 20,
                Colspan = 2
            });


            // Celda Vacia a la izquierda
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                HorizontalAlignment = 2
            });

            // Palabra FECHA
            tablaRecibo.AddCell(new PdfPCell(new Phrase("FECHA", _font8))
            {
                Border = 0,
                HorizontalAlignment = 1
            });

            //CANTIDAD
            tablaRecibo.AddCell(new PdfPCell(new Phrase(cantidadProcesada.ToCurrencyFormat(signo: true), _font10B))
            {
                Border = 0,
                HorizontalAlignment = 1

            });

            //FECHA
            string fechaPagoRecibo = fecha.ToString("dd   MMMM  yyyy", new CultureInfo("es-ES", false));

            tablaRecibo.AddCell(new PdfPCell(new Phrase(fechaPagoRecibo, _font10B))
            { Border = 0, HorizontalAlignment = 1 });


            // espacio verticales
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8)) { Border = 0, FixedHeight = 30, Colspan = 2 });


         


            //CANTIDAD EN LETRAS
            var cantidadLetra = Utils.ConvertCantidadALetras(cantidadProcesada.ToCurrencyFormat());
            tablaRecibo.AddCell(new PdfPCell(new Phrase("IMPORTE CON LETRAS: \n" + cantidadLetra, _font8))
            {
                Colspan = 2,
                Border = 0,
                HorizontalAlignment = 0
            });


            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = PdfPCell.BOTTOM_BORDER,
                FixedHeight = 15,
                Colspan = 2
            });

            // Espacio entre row superior
            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font8))
            {
                Border = 0,
                FixedHeight = 15,
                Colspan = 2
            });

            //Leyenda Recibi por concepto de
            tablaRecibo.AddCell(new PdfPCell(new Phrase("Recibí por concepto de pago total sin que a la fecha se me adeude cantidad alguna.  ", _font8))
            {
                Border = 0,
                HorizontalAlignment = 0
            });

            //Firma del empleado
            tablaRecibo.AddCell(new PdfPCell(new Phrase("______________________________\n Firma del Colaborador  ", _font8))
            {
                Border = 0,
                HorizontalAlignment = 1
            });

           


            //IMAGEN DE SINDICATO
            if (nombreEmpresa.ToUpper().Contains("SINDICATO"))
            {
                try
                {
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance("C:\\SUNFILES\\Certificados\\Logo.jpg");
                    iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance("C:\\SUNFILES\\Certificados\\Logo.jpg");
                    jpg.ScaleToFit(100, 100);
                    jpg2.ScaleToFit(100, 100);
                    //jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                    jpg.SetAbsolutePosition(250, 540);
                    jpg2.SetAbsolutePosition(250, 100);
                    //pdf.GetOverContent(1).AddImage(img);
                    documento.Add(jpg);
                    documento.Add(jpg2);
                }
                catch (Exception e)
                {

                }
            }
            tablaReciboc = tablaRecibo;
            tablaRecibo.SpacingAfter = 100f;

            documento.Add(tablaRecibo);
            documento.Add(tablaReciboc);



        }

        #region DOWNLOAD RECIBOS CFDI

        public Task<byte[]> GetArchivosPdfAsync(int[] idNominas)
        {
            var t = Task.Factory.StartNew(() =>
            {
                var items = _dao.GetItemsPdfBytesByIdNomina(idNominas);

                MemoryStream ms = new MemoryStream();
                Document doc = new Document(PageSize.LETTER, 50, 50, 25, 25);
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
                        doc.SetPageSize(PageSize.LETTER);
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
        public Task<string[]> DownloadRecibos(int[] idNominas, int idUsuario, string pathFolder, bool isFiniquito = false)
        {
            //var folderUsuario = "";
            var items = _dao.GetRecibosPdf(idNominas, isFiniquito: isFiniquito);

            //Validar ruta del folder
            //var fu = await ValidarFolderUsuario(idUsuario, pathFolder);
            //var fls = await GetFoldersRecibosByIdUsuario(idNominas, idUsuario, fu );

            Task tt = Task.Factory.StartNew(() =>
            {
                //Crea el folder para el usuario, si ya existe elimina su contenido.
                var val = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
                _pathUsuario = val;
                //Directory.CreateDirectory(fd1);

            });

            return tt.ContinueWith(t => GetPathsRecibosByIdUsuario(idNominas, idUsuario, isFiniquito: isFiniquito));
        }

        //private string ValidarFolderUsuario(int idUsuario, string pathFolder)
        //{
        //    //var pathArchivos = @"C:\Sites\Nominas\Nomina.WEB\Files\DownloadRecibos";
        //    //var pathArchivos = HttpContext.Current.Server.MapPath("~/Files/DownloadRecibos/");
        //    var pathArchivos = pathFolder;

        //    if (!Directory.Exists(pathArchivos))
        //    {
        //        Directory.CreateDirectory(pathArchivos);
        //    }

        //    //Crear folder para el usuario con su id
        //    string folderUsuario = pathArchivos + "\\" + idUsuario + "\\";

        //    if (Directory.Exists(folderUsuario))
        //    {
        //        try
        //        {
        //            //Elimina el contenido del folder
        //            Array.ForEach(Directory.GetFiles(folderUsuario), File.Delete);
        //            //Eliminar los folder
        //            Array.ForEach(Directory.GetDirectories(folderUsuario), Directory.Delete);
        //        }
        //        catch (Exception)
        //        {
        //            folderUsuario += "\\" + idUsuario + "\\";
        //        }
        //    }
        //    else
        //    {
        //        //Crea el folder con el id del usuario
        //        Directory.CreateDirectory(folderUsuario);
        //    }

        //    _pathUsuario = folderUsuario;

        //    return folderUsuario;
        //}

        /// <summary>
        /// Metodo retornsa
        /// </summary>
        /// <param name="idNominas"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        private string[] GetPathsRecibosByIdUsuario(int[] idNominas, int idUsuario, bool isFiniquito = false)
        {
            var items = _dao.GetRecibosPdf(idNominas, isFiniquito: isFiniquito);

            Random random = new Random();
            int varRamdom = random.Next();
            //Generar los xml desde los item string
            //Crear un solo archivo PDF
            Document doc = new Document(PageSize.LETTER);
            FileStream fs = null;
            PdfWriter writer = null;
            #region GENERA RECIBOS EN UN SOLO PDF

            if (items.Count > 1)
            {
                fs = new FileStream(_pathUsuario + varRamdom + "_todos_recibos.pdf", FileMode.Create);
                writer = PdfWriter.GetInstance(doc, fs);
                writer.CloseStream = false;
                doc.Open();
            }

            foreach (var item in items)
            {
                //Si el PDF no esta generado
                // llamar al metodo : GenerarReciboTimbrado(nominasSeleccionadas, idEjercicio, idPeriodo, isFiniquito)
                // actualizar la variable item.PDF

                //XML
                string nombreArchivo = item.RFCReceptor + " - " + item.Serie.Trim() + " - " + item.Folio;

                string path = _pathUsuario + nombreArchivo;

                //Genera los archivos xml - para ser enviados juntos a los pdf
                using (StreamWriter archivoX = new StreamWriter(path + ".xml", false))
                {
                    archivoX.WriteLine(item.XMLTimbrado); //<- cambiar item.XMLsinTimbre por  item.XMLTimbrado - 
                }

                //PDF
                if (items.Count <= 1) continue;
                if (writer == null) continue;

                PdfContentByte contentByte = writer.DirectContent;

                var reader = new PdfReader(item.PDF);
                int pages = reader.NumberOfPages;

                for (int i = 1; i <= pages; i++)
                {
                    doc.SetPageSize(PageSize.LETTER);
                    doc.NewPage();

                    var page = writer.GetImportedPage(reader, i);
                    contentByte.AddTemplate(page, 0, 0);
                }
            }

            if (items.Count > 1)
            {

                doc.Close();
                if (fs != null)
                {
                    fs.Flush();
                    fs.Dispose();
                }
            }

            #endregion

            //Generar pdf individuales

            foreach (var item in items)
            {
                var nombreArchivoIndividual = item.RFCReceptor + " - " + item.Serie.Trim() + " - " + item.Folio;

                var pathIndividual = _pathUsuario + nombreArchivoIndividual + ".pdf";

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

        #region NUEVO DETALLE DE COMPLEMENTO

        private string GenerarComplementoFiscal(int[] idEmpleados, int idEjercicio, NOM_PeriodosPago ppago, int idUsuario, string pathFolder)
        {
            Document doc = new Document(PageSize.LETTER, 50, 50, 25, 25);
            var archivoPdf = "";
            //int idEmpresa = 0;
            //int idColaborador = 0;
            //string nombreEmpleado = "";
            //string lugarExpedicion = "";
            //List<string> sinEmpresas = new List<string>();

            try
            {
                //Crea directorio, o si ya existe elimina el contenido
                var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
                _pathUsuario = pathUsuario;

                //GET DATOS SUBIDOS DE COMPLEMENTO
                var listaComplementosSubidos = _nominasDao.GetDatosComplementoFiscal(ppago.IdPeriodoPago);

                //GET EMPLEADOS
                var listaEmpleados = _nominasDao.GetEmpleadosByArray(idEmpleados);

                //GET Dato SUCURSAL
                var sucursal = _nominasDao.GetSucursal(ppago.IdSucursal);

                //GET Catalogo se conceptos
                var listaConceptos = _nominasDao.GetConceptosComplemento();

                Random r = new Random();
                int aleatorio = r.Next();
                int cont = 0;

                archivoPdf = pathUsuario + "Recibos Complemento_" + aleatorio + ".pdf";
                PdfWriter w = PdfWriter.GetInstance(doc, new FileStream(archivoPdf, FileMode.Create));

                doc.Open();

                //Se crea un solo archivo pdf con varias hojas
                int seccionPagina = 0;
                foreach (var itemEmpleado in listaEmpleados)
                {
                    // if(seccionPagina == 0)
                    doc.NewPage();

                    if (itemEmpleado != null)
                    {
                        //GET - Detalles de nomina complemento
                        var listaComplementoSubido =
                            listaComplementosSubidos.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();

                        if (listaComplementoSubido.Count <= 0) continue;

                        //AGREGA UNA HOJA DE RECIBO DE COMPLEMENTO
                        AddReciboPdfDetalle(ref doc, itemEmpleado, ppago, sucursal, listaComplementoSubido, listaConceptos);

                        cont++;

                        //Se agrega un espacio para dividir el recibo
                        PdfPTable tablaEspacio = new PdfPTable(1)
                        {
                            TotalWidth = 520,
                            LockedWidth = true
                        };

                        // Espacio entre row superior
                        tablaEspacio.AddCell(new PdfPCell(new Phrase("", _font8))
                        {
                            Border = 0,
                            FixedHeight = 90,

                        });

                        doc.Add(tablaEspacio);

                    }

                    //
                    seccionPagina++;
                    if (seccionPagina == 2)
                    {
                        seccionPagina = 0;
                    }

                }

                if (cont == 0)
                {
                    doc.NewPage();
                    doc.Add(new Paragraph("Ningún recibo generado"));
                }

                doc.Close();
            }
            catch (Exception ex)
            {
                doc.Add(new Paragraph("? - Exception "));
                //   return string.Empty;
            }
            finally
            {
                doc.Close();
            }

            return archivoPdf; //retorna la ruta del archivo pdf generado
        }

        private void AddReciboPdfDetalle(ref Document documento, Empleado itemEmpleado, NOM_PeriodosPago itemPeriodo, Sucursal itemSucursal, List<NOM_Nomina_Detalle_C> listaDetalle, List<C_NOM_Conceptos_C> listaConceptos)
        {
            int anchoTabla = 520;
            BaseColor colorBackAzulOscuro = BaseColor.LIGHT_GRAY;

            //Tabla del recibo
            PdfPTable tablaRecibo = new PdfPTable(2)
            {
                TotalWidth = anchoTabla,
                LockedWidth = true
            };


            //Titulo - Detalle de pago
            tablaRecibo.AddCell(new PdfPCell(new Phrase("DETALLE DE PAGO ", _font11B))
            {
                Colspan = 2,
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
            });


            //Nombre del periodo
            var periodo = "Periodo del " + itemPeriodo.Fecha_Inicio.ToLongDateString() + " al " +
                          itemPeriodo.Fecha_Fin.ToLongDateString();
            tablaRecibo.AddCell(new PdfPCell(new Phrase(periodo, _font10))
            {
                Colspan = 2,
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
            });

            //Nombre del trabajador
            var nombreEmpleado = itemEmpleado.APaterno + " " + itemEmpleado.AMaterno + " " + itemEmpleado.Nombres;
            tablaRecibo.AddCell(new PdfPCell(new Phrase("Nombre del Trabajador:   " + nombreEmpleado, _font10B))
            {
                Colspan = 2,
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,

            });


            //Unidad donde labora  - Salario dirio
            tablaRecibo.AddCell(new PdfPCell(new Phrase("Unidad donde labora:  " + itemSucursal.Ciudad, _font10))
            {
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,

            });
            var itemSueldo = listaDetalle.FirstOrDefault(x => x.IdConcepto == 1);
            var cantidadSueldo = itemSueldo == null ? "0.00" : itemSueldo.Cantidad.ToCurrencyFormat(2, true);

            tablaRecibo.AddCell(new PdfPCell(new Phrase("Sueldo Diario: " + cantidadSueldo, _font10))
            {
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 2,

            });


            //Percepciones  
            var arrayPercepciones = listaConceptos.Where(x => x.TipoConcepto == 1).Select(x => x.IdConceptoC).ToList();

            var itemsPercepciones = (from c in listaDetalle
                                     where arrayPercepciones.Contains(c.IdConcepto)
                                     select c).ToList();

            PdfPTable tablaPercepciones = new PdfPTable(2)
            {
                TotalWidth = 250,
                LockedWidth = true
            };

            tablaPercepciones.AddCell(new PdfPCell(new Phrase("PERCEPCIONES", _font11B))
            {
                Colspan = 2,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
                VerticalAlignment = Element.ALIGN_MIDDLE

            });


            tablaPercepciones.AddCell(new PdfPCell(new Phrase("Conceptos", _font10))
            {
                // Border = Rectangle.BOTTOM_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                BorderColorRight = BaseColor.WHITE,
                HorizontalAlignment = 0,


            });
            tablaPercepciones.AddCell(new PdfPCell(new Phrase("Importes", _font10))
            {
                // Border = Rectangle.BOTTOM_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 2,

            });

            foreach (var item in itemsPercepciones)
            {
                var itemConcepto =
                    listaConceptos.FirstOrDefault(x => x.IdConceptoC == item.IdConcepto);

                var descripcion = itemConcepto == null ? "--" : itemConcepto.Descripcion;
                tablaPercepciones.AddCell(new PdfPCell(new Phrase(descripcion, _font10))
                {
                    Border = Rectangle.LEFT_BORDER,
                    //BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 0,
                });


                tablaPercepciones.AddCell(new PdfPCell(new Phrase(item.Cantidad.ToString(), _font10))
                {
                    Border = Rectangle.RIGHT_BORDER,
                    //BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 2,
                });
            }

            tablaPercepciones.AddCell(new PdfPCell(new Phrase(" ", _font10))
            {
                Border = Rectangle.LEFT_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 10
            });

            tablaPercepciones.AddCell(new PdfPCell(new Phrase(" ", _font10))
            {
                Border = Rectangle.RIGHT_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 10
            });


            //agregamos a la tabla principal
            tablaRecibo.AddCell(new PdfPCell(tablaPercepciones)
            {
                Border = 0,
                HorizontalAlignment = 0
            });


            // Deducciones
            var arrayDeducciones = listaConceptos.Where(x => x.TipoConcepto == 2).Select(x => x.IdConceptoC).ToList();

            var itemsDeducciones = (from c in listaDetalle
                                    where arrayDeducciones.Contains(c.IdConcepto)
                                    select c).ToList();

            PdfPTable tablaDeducciones = new PdfPTable(2)
            {
                TotalWidth = 250,
                LockedWidth = true

            };

            tablaDeducciones.AddCell(new PdfPCell(new Phrase("DEDUCCIONES", _font11B))
            {
                Colspan = 2,
                // BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });


            tablaDeducciones.AddCell(new PdfPCell(new Phrase("Conceptos", _font10))
            {
                // Border = Rectangle.BOTTOM_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0
            });

            tablaDeducciones.AddCell(new PdfPCell(new Phrase("Importes", _font10))
            {
                //  Border = Rectangle.BOTTOM_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 2,

            });

            foreach (var item in itemsDeducciones)
            {
                var itemConcepto =
                    listaConceptos.FirstOrDefault(x => x.IdConceptoC == item.IdConcepto);

                var descripcion = itemConcepto == null ? "--" : itemConcepto.Descripcion;

                tablaDeducciones.AddCell(new PdfPCell(new Phrase(descripcion, _font10))
                {
                    Border = Rectangle.LEFT_BORDER,
                    //BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 0,
                });

                tablaDeducciones.AddCell(new PdfPCell(new Phrase(item.Cantidad.ToString(), _font10))
                {
                    Border = Rectangle.RIGHT_BORDER,
                    //BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 2,
                });
            }

            //
            tablaDeducciones.AddCell(new PdfPCell(new Phrase(" ", _font10))
            {
                Border = Rectangle.LEFT_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 10
            });

            tablaDeducciones.AddCell(new PdfPCell(new Phrase(" ", _font10))
            {
                Border = Rectangle.RIGHT_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 10
            });

            //agregamos a la tabla principal
            tablaRecibo.AddCell(new PdfPCell(tablaDeducciones)
            {
                Border = 0,
                HorizontalAlignment = 2
            });

            //   TOTALES  ********************************************************
            //Totales - Percepciones - Deducciones
            PdfPTable tablaTotalPercepciones = new PdfPTable(2)
            {
                TotalWidth = 250,
                LockedWidth = true
            };

            tablaTotalPercepciones.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaTotalPercepciones.AddCell(new PdfPCell(new Phrase("Total de Percepciones", _font11))
            {
                Border = Rectangle.TOP_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 25,
            });

            var itemTotalPerpcepciones = listaDetalle.FirstOrDefault(x => x.IdConcepto == 32);
            var totalPerpcepcion = itemTotalPerpcepciones == null ? "0.00" : itemTotalPerpcepciones.Cantidad.ToCurrencyFormat(2, true);

            tablaTotalPercepciones.AddCell(new PdfPCell(new Phrase(totalPerpcepcion, _font11))
            {
                Border = Rectangle.TOP_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 2,
                FixedHeight = 25,
            });


            PdfPTable tablaTotalDeducciones = new PdfPTable(2)
            {
                TotalWidth = 250,
                LockedWidth = true
            };

            tablaTotalDeducciones.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaTotalDeducciones.AddCell(new PdfPCell(new Phrase("Total de Deducciones", _font11))
            {
                Border = Rectangle.TOP_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 25,
            });

            var itemTotalDeduccion = listaDetalle.FirstOrDefault(x => x.IdConcepto == 46);

            var totalDeduccion = itemTotalDeduccion == null ? "0.00" : itemTotalDeduccion.Cantidad.ToCurrencyFormat(2, true);

            tablaTotalDeducciones.AddCell(new PdfPCell(new Phrase(totalDeduccion, _font11))
            {
                Border = Rectangle.TOP_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 2,
                FixedHeight = 25,
            });


            //agregamos a la tabla principal
            tablaRecibo.AddCell(new PdfPCell(tablaTotalPercepciones) { Border = 0 });
            tablaRecibo.AddCell(new PdfPCell(tablaTotalDeducciones) { Border = 0 });


            //Neto a Pagar
            var itemTotalaApagar = listaDetalle.FirstOrDefault(x => x.IdConcepto == 47);


            PdfPTable tablaTotalAPagar = new PdfPTable(2)
            {
                TotalWidth = 250,
                LockedWidth = true
            };

            tablaTotalAPagar.AddCell(new PdfPCell(new Phrase("NETO A PAGAR", _font11B))
            {
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 0,
                FixedHeight = 25,
            });

            var totalAPagar = itemTotalaApagar == null ? "0.00" : itemTotalaApagar.Cantidad.ToCurrencyFormat(2, true);
            tablaTotalAPagar.AddCell(new PdfPCell(new Phrase(totalAPagar, _font11B))
            {
                Border = Rectangle.TOP_BORDER,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 2,
                FixedHeight = 25,
            });

            tablaRecibo.AddCell(new PdfPCell(new Phrase("  ")) { Border = 0 });
            tablaRecibo.AddCell(new PdfPCell(tablaTotalAPagar) { Border = 0 });


            //Firma del empleado


            tablaRecibo.AddCell(new PdfPCell(new Phrase("__________________________________________", _font11B))
            {

                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
            });

            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font11B))
            {
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
            });


            tablaRecibo.AddCell(new PdfPCell(new Phrase("FIRMA DEL TRABAJADOR", _font11B))
            {

                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
            });

            tablaRecibo.AddCell(new PdfPCell(new Phrase("", _font11B))
            {
                Border = 0,
                //BackgroundColor = colorBackAzulOscuro,
                HorizontalAlignment = 1,
                FixedHeight = 25,
            });



            documento.Add(tablaRecibo);
        }

        #endregion

        #region Recibo cfdi 3.3

        private void AddReciboPdf33(ref Document documento, string strXml, NOM_PeriodosPago periodoDatos,
        List<C_TipoJornada_SAT> listaTipoJornada,
        List<C_RegimenFiscal_SAT> listaRegimenFiscal,
        List<C_TipoIncapacidad_SAT> listaIncapacidades, 
        int idNomina = 0, string cadenaOriginal = "cadena original", bool conTimbrado = false)
        {
            DataSet dataSet = new DataSet();
            try
            {
                int anchoTabla = 550;
                string lugarExpedicion = "";
                //BaseColor colorBackAzulOscuro = new BaseColor(36, 142, 167); //new BaseColor(8, 107, 129); //BaseColor.LIGHT_GRAY;
                BaseColor colorBackAzulOscuro = new BaseColor(36, 142, 167);//new BaseColor(5, 95, 130);
                BaseColor colorBackAzulClaro = new BaseColor(40, 162, 189);//0, 204, 204//90, 151, 206
                BaseColor colorBackGray = new BaseColor(102, 102, 102);

                //if (periodoDatos.IdTipoNomina == 17)
                //{
                //    colorBackAzulOscuro = new BaseColor(219,186,191);
                //}

                //Carga el contenido del XML en el DATASET
                StringReader streamReader = new StringReader(strXml);
                dataSet.ReadXml(streamReader);

                #region "VARIABLES"

                string cnx = string.Empty,

                    curp = string.Empty,
                    depa = string.Empty,
                    puesto = string.Empty,
                    registrosPatronal = string.Empty,
                    fechaInicioLaboral = string.Empty,
                    tipoJornada = string.Empty,
                    sdi = string.Empty,
                    sbc = string.Empty,
                    nss = string.Empty;

                string nombreReceptor = string.Empty,
                    rfcrecep = string.Empty,
                    rfcEmisor = string.Empty,

                    regimenEmisior = string.Empty,


                    metodoPago = string.Empty,
                    numCertificadoEmisor = "--",

                     fechaTimbrado = "----/--/--T00:00:00",
                    certificadoSat = "--",
                    uuid = "00000000-0000-0000-0000-000000000000",
                    selloCfd = "--",
                    selloSat = "--";

                string totalComprobante = string.Empty, totalPercepciones = string.Empty, totalDeducciones = string.Empty, totalOtrosPagos = string.Empty;

                string totalGravado = string.Empty, totalExento = string.Empty;

                string tipoComprobante = string.Empty;
                string usoCfdi = string.Empty;
                string formaDePago = string.Empty;
                string moneda = string.Empty;
                //  string cadenaOriginal = string.Empty;

                string fechaIniPago = string.Empty;
                string fechaFinPago = string.Empty;
                string fechaPago = string.Empty;
                string numDiasPagados = string.Empty;
                string version = string.Empty;

                string ClaveProdServ = "84111505";
                string Cantidad = "1";
                string ClaveUnidad = "ACT";
                string Descripcion = "Pago de nómina";
                string ValorUnitario = "0.00";
                string Importe = "0.00";
                string Descuento = "0.00";

                #endregion

                #region SET DATOS NOMINA - EMISOR - RECEPTOR 

                //TAG Nomina --
                if (dataSet.Tables.Contains("Nomina"))
                {
                    foreach (DataRow renglon in dataSet.Tables["Nomina"].Rows)
                    {
                        fechaIniPago = Convert.ToString(renglon["FechaInicialPago"]);
                        fechaFinPago = Convert.ToString(renglon["FechaFinalPago"]);
                        fechaPago = Convert.ToString(renglon["FechaPago"]);
                        numDiasPagados = Convert.ToString(renglon["NumDiasPagados"]);

                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalPercepciones"))
                            totalPercepciones = Convert.ToString(renglon["TotalPercepciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalDeducciones"))
                            totalDeducciones = Convert.ToString(renglon["TotalDeducciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalOtrosPagos"))
                            totalOtrosPagos = Convert.ToString(renglon["TotalOtrosPagos"]);
                    }
                }

                #region Tag Receptor

                //Tag RECEPTOR dentro del Tag COMPROBANTE
                foreach (DataRow recep in dataSet.Tables[2].Rows)
                {
                    nombreReceptor = Convert.ToString(recep["nombre"]);
                    rfcrecep = Convert.ToString(recep["rfc"]);
                    usoCfdi = Convert.ToString(recep["UsoCFDI"]);
                }

                //TAG RECEPTOR dentro del tag NOMINA
                foreach (DataRow recep in dataSet.Tables[8].Rows)//
                {
                    cnx = recep["NumEmpleado"].ToString();
                    curp = Convert.ToString(recep["CURP"]);
                    depa = Convert.ToString(recep["Departamento"]);
                    puesto = Convert.ToString(recep["Puesto"]);
                    fechaInicioLaboral = Convert.ToString(recep["FechaInicioRelLaboral"]);
                    tipoJornada = Convert.ToString(recep["TipoJornada"]);
                    sdi = Convert.ToString(recep["SalarioDiarioIntegrado"]);

                    if (dataSet.Tables[8].Columns.Contains("NumSeguridadSocial"))
                    {
                        nss = Convert.ToString(recep["NumSeguridadSocial"]);
                    }
                    sbc = Convert.ToString(recep["SalarioBaseCotApor"]);
                }

                #endregion

                #region Tag  Emisor

                //TAG EMISOR dentro del tag de comprobante
                string nombreEmisor = "";

                foreach (DataRow emisor in dataSet.Tables[1].Rows)
                {
                    nombreEmisor = Convert.ToString(emisor["nombre"]);
                    rfcEmisor = emisor["rfc"].ToString();
                    regimenEmisior = Convert.ToString(emisor["RegimenFiscal"]);
                }

                //Segundo Tag EMISOR del tag de Nomina
                foreach (DataRow emisor in dataSet.Tables[7].Rows)
                {
                    try
                    {
                        registrosPatronal = Convert.ToString(emisor["RegistroPatronal"]);
                    }
                    catch (Exception ex)
                    {
                        registrosPatronal = "-";
                    }

                }

                //TAG Domicilio Fiscal
                //foreach (DataRow domi in dataSet.Tables["DomicilioFiscal"].Rows)
                //{
                //    municipioEmisor = Convert.ToString(domi["municipio"]);
                //    estadoEmisor = Convert.ToString(domi["estado"]);
                //}

                //TAG Regimen Fiscal
                if (dataSet.Tables.Contains("RegimenFiscal"))
                {
                    foreach (DataRow regimen in dataSet.Tables["RegimenFiscal"].Rows)
                    {
                        regimenEmisior = Convert.ToString(regimen["Regimen"]);
                    }
                }

                #endregion

                #endregion

                #region SET DATOS COMPROBANTE - CONCEPTO - TIMBRE FISCAL DIGITAL 

                //TAG Comprobante
                if (dataSet.Tables.Contains("Comprobante"))
                {
                    if (dataSet.Tables["Comprobante"] != null)
                        foreach (DataRow renglon in dataSet.Tables["Comprobante"].Rows)
                        {
                            totalComprobante = Convert.ToString(renglon["total"]);
                            formaDePago = Convert.ToString(renglon["formaPago"]);
                            metodoPago = Convert.ToString(renglon["metodoPago"]);//metodoDePago
                            numCertificadoEmisor = conTimbrado ? Convert.ToString(renglon["noCertificado"]) : "noCertificado?";
                            moneda = Convert.ToString(renglon["moneda"]);
                            //fechaComprobante = Convert.ToString(renglon["fecha"]);
                            //serie = Convert.ToString(renglon["serie"]);
                            //folio = Convert.ToString(renglon["folio"]);
                            lugarExpedicion = Convert.ToString(renglon["LugarExpedicion"]);
                            tipoComprobante = Convert.ToString(renglon["TipoDeComprobante"]);
                            version = Convert.ToString(renglon["Version"]);
                        }
                }

                //TAG - Concepto
                if (dataSet.Tables.Contains("Concepto"))
                {
                    if (dataSet.Tables["Concepto"] != null)
                        foreach (DataRow renglon in dataSet.Tables["Concepto"].Rows)
                        {
                            ClaveProdServ = Convert.ToString(renglon["ClaveProdServ"]);
                            Cantidad = Convert.ToString(renglon["Cantidad"]);
                            ClaveUnidad = Convert.ToString(renglon["ClaveUnidad"]);
                            Descripcion = Convert.ToString(renglon["Descripcion"]);
                            ValorUnitario = Convert.ToString(renglon["ValorUnitario"]);
                            Importe = Convert.ToString(renglon["Importe"]);
                            try
                            {
                                Descuento = Convert.ToString(renglon["Descuento"]);
                            }
                            catch (Exception)
                            {
                                Descuento = "0.00";
                            }
                        }
                }


                //TAG Timbre Fiscal
                if (dataSet.Tables.Contains("TimbreFiscalDigital"))
                {
                    if (dataSet.Tables["TimbreFiscalDigital"] != null)
                        foreach (DataRow timbre in dataSet.Tables["TimbreFiscalDigital"].Rows)
                        {
                            fechaTimbrado = Convert.ToString(timbre["FechaTimbrado"]);
                            certificadoSat = Convert.ToString(timbre["noCertificadoSAT"]);
                            uuid = Convert.ToString(timbre["UUID"]);
                            selloCfd = Convert.ToString(timbre["selloCFD"]);
                            selloSat = Convert.ToString(timbre["selloSAT"]);
                        }
                }

                #endregion

                #region DISEÑO DEL CONTENIDO DE LA PÁGINA DEL DOCUMENTO PDF

                #region DATOS DEL EMISOR

                PdfPTable tablaEmisor = new PdfPTable(6)
                {
                    TotalWidth = anchoTabla,
                    LockedWidth = true
                };

                tablaEmisor.DefaultCell.Border = 0;
                //NOMBRE DE LA EMPRESA EMISORA
                //ROW 1
                PdfPCell cell = new PdfPCell(new Phrase(nombreEmisor, _font14W))
                {
                    Colspan = 5,
                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 1,
                    Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER,

                };
                tablaEmisor.AddCell(cell);

                tablaEmisor.AddCell(new PdfPCell(new Phrase("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd"), _font8W))
                {

                    BackgroundColor = colorBackAzulClaro,
                    HorizontalAlignment = 2,
                    Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER
                });



                //ROW 2
                var strRegimenEmisor = "";

                var itemRegimen = listaRegimenFiscal.FirstOrDefault(x => x.ClaveRegimen == regimenEmisior);

                if (itemRegimen != null)
                {
                    strRegimenEmisor = $"{regimenEmisior} {itemRegimen.Descripcion}";
                }
                else
                {
                    strRegimenEmisor = regimenEmisior;
                }

                tablaEmisor.AddCell(new PdfPCell(new Phrase($"Regimen Fiscal: {strRegimenEmisor}", _font10W))
                {
                    Colspan = 5,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = Rectangle.LEFT_BORDER,
                    HorizontalAlignment = 1
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font10))
                {
                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                    HorizontalAlignment = 1
                });

                //ROW EN BLANCO
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Colspan = 5,
                    Border = Rectangle.LEFT_BORDER,
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                });


                //ROW 3
                tablaEmisor.AddCell(new PdfPCell(new Phrase("RFC ", _font8BW))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = Rectangle.LEFT_BORDER,
                    HorizontalAlignment = 1
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("Reg Patronal", _font8BW))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("Tipo de Comprobante", _font8BW))
                {
                    Colspan = 2,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });




                tablaEmisor.AddCell(new PdfPCell(new Phrase("Lugar Expedición", _font8BW))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 1,
                    Border = 0
                });



                tablaEmisor.AddCell(new PdfPCell(new Phrase("Versión", _font8BW))
                {

                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                    HorizontalAlignment = 1
                });

                //ROW 4
                tablaEmisor.AddCell(new PdfPCell(new Phrase(rfcEmisor, _font8W))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = Rectangle.LEFT_BORDER,
                    HorizontalAlignment = 1
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase(registrosPatronal, _font8W))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("N - Nomina", _font8W))
                {
                    Colspan = 2,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });


                tablaEmisor.AddCell(new PdfPCell(new Phrase(lugarExpedicion, _font8W))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 1,
                    Border = 0
                });



                tablaEmisor.AddCell(new PdfPCell(new Phrase("3.3", _font8W))
                {

                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                    HorizontalAlignment = 1
                });


                //ROW 6
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = Rectangle.TOP_BORDER,
                    Colspan = 6,
                    BorderWidthTop = 1f,
                    HorizontalAlignment = 2
                });


                documento.Add(tablaEmisor);

                //Rectangle rectangle = new Rectangle(37, 818, 557, 770) //37, 818, 557, 760
                //{
                //    BackgroundColor = colorBackAzulOscuro,
                //    BorderWidth = 1,
                //    Border = 3,
                //    BorderColor = BaseColor.BLACK
                //};

                // documento.Add(new Paragraph(" "));
                //documento.Add(rectangle);

                #endregion

                #region DATOS DEL RECEPTOR

                PdfPTable tblContendorReceptor = new PdfPTable(2);
                PdfPTable tblDatosReceptor = new PdfPTable(3);
                PdfPTable tblDatosReceptorNomina = new PdfPTable(3);

                tblDatosReceptor.DefaultCell.Border = 0;
                tblDatosReceptor.AddCell(new PdfPCell(new Phrase(cnx + " - " + nombreReceptor, _font8B))
                {
                    Colspan = 3,
                    Border = 0
                });

                tblDatosReceptor.AddCell(new PdfPCell(new Phrase("RFC ", _font7B)) { Border = 0 });
                tblDatosReceptor.AddCell(new PdfPCell(new Phrase(rfcrecep, _font7)) { Colspan = 2, Border = 0 });
                tblDatosReceptor.AddCell(new PdfPCell(new Phrase("CURP ", _font7B)) { Border = 0 });
                tblDatosReceptor.AddCell(new PdfPCell(new Phrase(curp, _font7)) { Colspan = 2, Border = 0 });
                tblDatosReceptor.AddCell(new PdfPCell(new Phrase("NSS ", _font7B)) { Border = 0 });
                tblDatosReceptor.AddCell(new PdfPCell(new Phrase(nss, _font7)) { Colspan = 2, Border = 0 });

                tblDatosReceptor.AddCell(new PdfPCell(new Phrase("Fecha Inicio Laboral", _font7B)) { Border = 0 });
                tblDatosReceptor.AddCell(new PdfPCell(new Phrase(fechaInicioLaboral, _font7)) { Colspan = 2, Border = 0 });

                tblDatosReceptor.AddCell(new PdfPCell(new Phrase("Tipo Jornada ", _font7B)) { Border = 0 });

                var strTipoJornada = tipoJornada;

                var itemTipoJornada = listaTipoJornada.FirstOrDefault(x => x.Clave.Trim() == tipoJornada.Trim());

                if (itemTipoJornada != null)
                {
                    strTipoJornada = $"{tipoJornada} {itemTipoJornada.Descripcion}";
                }

                tblDatosReceptor.AddCell(new PdfPCell(new Phrase(strTipoJornada, _font7)) { Colspan = 2, Border = 0 });

                //tblDatosReceptor.AddCell(new PdfPCell(new Phrase("Uso CFDI ", _font7B)) { Border = 0 });
                //tblDatosReceptor.AddCell(new PdfPCell(new Phrase($"{usoCfdi} Por definir", _font7)) { Colspan = 2, Border = 0 });

                tblDatosReceptorNomina.DefaultCell.Border = 0;
                tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase("Periodo ", _font8B)) { Border = 0 });

                //var FPI = (DateTime)periodoDatos.Fecha_Inicio;
                //var FPF = (DateTime)periodoDatos.Fecha_Fin;
                //var FP = (DateTime)periodoDatos.Fecha_Pago;

                tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase(fechaIniPago + " / " + fechaFinPago, _font8B)) { Colspan = 2, Border = 0, });
                tblDatosReceptorNomina.AddCell(new Phrase("Días Pagados :  " + numDiasPagados, _font7B));
                tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase("Fecha Pago : " + fechaPago, _font7B)) { Colspan = 2, Border = 0 });

                //INCAPACIDADES
                if (dataSet.Tables["Incapacidad"] != null)
                {
                    foreach (DataRow perceps in dataSet.Tables["Incapacidad"].Rows)
                    {
                        var diasIncapacidad = Convert.ToString(perceps["DiasIncapacidad"]);
                        var tipoIncapacidad = Convert.ToString(perceps["TipoIncapacidad"]);
                        var nombresIncapacidad = "--";

                        if (listaIncapacidades != null)
                        {
                            var itemI = listaIncapacidades.FirstOrDefault(x => x.Clave.Trim() == tipoIncapacidad);
                            if (itemI != null)
                            {
                                nombresIncapacidad = itemI.Descripcion;
                            }
                        }

                        tblDatosReceptorNomina.AddCell(new Phrase($"Días de Incapacidad: {diasIncapacidad}", _font7B));
                        tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase($"Tipo: {tipoIncapacidad} - {nombresIncapacidad}", _font7)) { Colspan = 2, Border = 0 });


                    }
                }


                tblDatosReceptorNomina.AddCell(new Phrase("Puesto ", _font7B));
                tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase(puesto, _font7)) { Colspan = 2, Border = 0 });
                tblDatosReceptorNomina.AddCell(new Phrase("Departamento ", _font7B));
                tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase(depa, _font7)) { Colspan = 2, Border = 0 });
                tblDatosReceptorNomina.AddCell(new Phrase($"SBC: {sbc} ", _font7B));
                tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase($"SDI:{sdi}", _font7B)) { Colspan = 2, Border = 0 });
                //tblDatosReceptorNomina.AddCell(new Phrase("SDI", _font7B));
                //tblDatosReceptorNomina.AddCell(new PdfPCell(new Phrase(sdi, _font7)) { Colspan = 2, Border = 0 });

                



                tblContendorReceptor.TotalWidth = anchoTabla;
                tblContendorReceptor.LockedWidth = true;

                tblContendorReceptor.AddCell(tblDatosReceptor);
                tblContendorReceptor.AddCell(tblDatosReceptorNomina);

                #endregion

                tblContendorReceptor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = Rectangle.TOP_BORDER,
                    BorderWidthTop = 1f,
                    Colspan = 2,
                    HorizontalAlignment = 2
                });
                documento.Add(tblContendorReceptor);
                //documento.Add(new Paragraph(" "));

                #region CONCEPTO - Unidad - Cantidad - 

                PdfPTable tablaConcepto = new PdfPTable(7)
                {
                    TotalWidth = anchoTabla,
                    LockedWidth = true
                };

                tablaConcepto.DefaultCell.Border = 0;


                tablaConcepto.AddCell(new PdfPCell(new Phrase("Clave ProdServ", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });

                tablaConcepto.AddCell(new PdfPCell(new Phrase("Cantidad", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Clave Unidad", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Descripción", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Valor Unitario", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Importe", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Descuento", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });



                tablaConcepto.AddCell(new PdfPCell(new Phrase(ClaveProdServ, _font8B))
                {

                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });

                tablaConcepto.AddCell(new PdfPCell(new Phrase(Cantidad, _font8B))
                {
                    BorderColor = colorBackGray,
                    HorizontalAlignment = 1,

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(ClaveUnidad, _font8B))
                {
                    BorderColor = colorBackGray,
                    HorizontalAlignment = 1,

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(Descripcion, _font8B))
                {

                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(ValorUnitario, _font8B))
                {
                    BorderColor = colorBackGray,
                    HorizontalAlignment = 1,

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(Importe, _font8B))
                {

                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(Descuento, _font8B))
                {
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });


                #endregion

                tablaConcepto.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = Rectangle.BOTTOM_BORDER,
                    BorderWidthBottom = 1f,
                    Colspan = 7,
                    HorizontalAlignment = 2
                });

                documento.Add(tablaConcepto);

                #region  TABLAS DE CONTENIDO - PERCEPCIONES - DEDUCCIONES - OTRO PAGO - TOTAL RECIBO

                var columnas = 8; //Numero de columnas de las tablas

                #region TABLA PERCEPCION
                //variables
                // var totalPercepcionGravado = 0.00;
                // var totalPercepcionExento = 0.00;
                // var totalPercepcionDecimal = 0.00;

                if (dataSet.Tables.Contains("Percepcion"))
                {

                    //1) definicion de la tabla
                    var tablaPercepciones = new PdfPTable(columnas);

                    //3) Subtitulos  - Cve SAT  - Concepto - Gravado - Exento - Total
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Clave", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Percepciones", _font8BW))
                    {
                        Colspan = (columnas - 4),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Gravado", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Exento", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Importe", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - longitud de la tabla
                    tablaPercepciones.TotalWidth = anchoTabla;
                    tablaPercepciones.LockedWidth = true;



                    // Get Totales
                    if (dataSet.Tables["Percepciones"] != null)
                        foreach (DataRow perceps in dataSet.Tables["Percepciones"].Rows)
                        {
                            totalGravado = Convert.ToString(perceps["TotalGravado"]);//3
                            totalExento = Convert.ToString(perceps["TotalExento"]);//4
                        }

                    //5) Detalle conceptos
                    if (dataSet.Tables["Percepcion"] != null)
                        foreach (DataRow percep in dataSet.Tables["Percepcion"].Rows)
                        {
                            var importeExcento = double.Parse(percep[4].ToString());
                            var importeGravado = double.Parse(percep[3].ToString());
                            var concepto = Convert.ToString(percep[2]);
                            var claveSat = Convert.ToString(percep[1]);
                            var tipo = Convert.ToString(percep[0]);

                            var totalImporte = importeExcento + importeGravado;
                            // totalPercepcionDecimal += totalImporte;

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            // tblPercepciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 4)
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeGravado.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeExcento.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    //6) TOTAL PERCEPCIONES - GRAVA - EXENTA - TOTAL

                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 3) });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(str: totalGravado.ToCurrencyFormat(signo: true), font: _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalExento.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalPercepciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    //documento.Add(new Paragraph(" "));
                    
                    if (dataSet.Tables.Contains("Deduccion") || (dataSet.Tables.Contains("OtroPago")))
                    {
                     
                        tablaPercepciones.AddCell(new PdfPCell(new Phrase(" ", _font7))
                        {
                            Border = Rectangle.BOTTOM_BORDER,
                            BorderWidthBottom = 1f,
                            Colspan = (columnas)
                        });

                    }
                    else
                    {
                        tablaPercepciones.AddCell(new PdfPCell(new Phrase(" ", _font7))
                        {
                            Border = 0,
                            Colspan = (columnas)
                        });
                    }

                   



                    //8) Agregar la tabla al documento
                    documento.Add(tablaPercepciones);
                }

                #endregion

                #region TABLA DEDUCCION
                // Variables
                // var totalDeducciones = 0.0;

                if (dataSet.Tables.Contains("Deduccion"))
                {

                    //1) Definir la tabla
                    var tablaDeucciones = new PdfPTable(columnas);

                    //3) Subtitulos
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Clave", _font8BW))
                    {
                        HorizontalAlignment = 1, //Centrado
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Deducciones", _font8BW))
                    {
                        Colspan = (columnas - 4),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1, //Centrado,
                        Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER
                    });

                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro,
                        Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro,
                        Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER
                    });


                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Importe", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaDeucciones.TotalWidth = anchoTabla;
                    tablaDeucciones.LockedWidth = true;



                    //5) Detalle conceptos
                    if (dataSet.Tables["Deduccion"] != null)
                        foreach (DataRow deduc in dataSet.Tables["Deduccion"].Rows)
                        {
                            //var importeExcento = double.Parse(deduc[0].ToString());
                            //var importeGravado = double.Parse(deduc[1].ToString());
                            var totalImporte = double.Parse(deduc[3].ToString());
                            var concepto = Convert.ToString(deduc[2]);
                            var claveSat = Convert.ToString(deduc[1]);
                            var tipo = Convert.ToString(deduc[0]);

                            //  totalDeducciones += totalImporte;

                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7)) { Border = 0 });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }


                    //6) Totales Deducciones
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalDeducciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });


                    //7) Agregar espacio entre tablas
                    if (dataSet.Tables.Contains("OtroPago"))
                    {
                        tablaDeucciones.AddCell(new PdfPCell(new Phrase(" ", _font7))
                        {
                            Border = Rectangle.BOTTOM_BORDER,
                            BorderWidthBottom = 1f,
                            Colspan = (columnas)
                        });
                    }
                    else
                    {
                        tablaDeucciones.AddCell(new PdfPCell(new Phrase(" ", _font7))
                        {
                            Border = 0,
                            Colspan = (columnas)
                        });
                    }
                    //documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaDeucciones);

                }
                #endregion

                #region TABLA OTROS PAGOS

                // Variables
                // var totalOtroPago = 0.0;

                if (dataSet.Tables.Contains("OtroPago"))
                {

                    //1) definicion de la tabla
                    var tablaOtrosPagos = new PdfPTable(columnas);


                    //3) Subtitulos
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Clave", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Otros Pagos", _font8BW))
                    {
                        Colspan = (columnas - 4),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1, //Centrado
                        Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER
                    });

                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro,
                        Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER
                    });

                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro,
                        Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER
                    });

                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Importe", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaOtrosPagos.TotalWidth = anchoTabla;
                    tablaOtrosPagos.LockedWidth = true;

                    //5) Detalle conceptos
                    if (dataSet.Tables["OtroPago"] != null)
                        foreach (DataRow otroPago in dataSet.Tables["OtroPago"].Rows)
                        {
                            decimal totalImporte = 0;
                            var concepto = "";
                            var claveSat = "";

                            if (otroPago.ItemArray.Length > 5)//
                            {
                                totalImporte = decimal.Parse(otroPago[4].ToString());
                                concepto = Convert.ToString(otroPago[3]);
                                claveSat = Convert.ToString(otroPago[2]);
                            }
                            else//por el caso de Otros Pagos de Devolucion de ISR
                            {
                                totalImporte = decimal.Parse(otroPago[3].ToString());
                                concepto = Convert.ToString(otroPago[2]);
                                claveSat = Convert.ToString(otroPago[1]);

                            }


                            //totalOtroPago += totalImporte;

                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalOtrosPagos.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });
                    // documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaOtrosPagos);
                }

                #endregion

                #region TOTAL

                // Variables
                //var totalRecibo = 0.0;

                //1) definicion de la tabla
                // var tablaTotalRecibo = new PdfPTable(columnas);

                //2) Set - Longitud de la Tabla
                //tablaTotalRecibo.TotalWidth = anchoTabla;
                //tablaTotalRecibo.LockedWidth = true;

                // totalRecibo = (totalPercepcionExento + totalPercepcionGravado + totalOtroPago) - totalDeducciones;
                // 3) Detalle total
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase("TOTAL $", _font10B)) { Border = 0, Colspan = (columnas-1), HorizontalAlignment = 2 });
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase($"{totalRecibo:0,0.##}", _font10B)) { Border = 0, Colspan = 1, HorizontalAlignment = 2 });

                //4 Agregar espacio entre tablas


                //5) Agregar la tabla al documento
                // documento.Add(tablaTotalRecibo);

                #endregion
                // documento.Add(new Paragraph(" "));//ABC

                #endregion

                #region GENERA LOGO CBB

                //string textCode = @"?re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&id=" + uuid;
                // version 3.3
                string selloDigitalEmisor = selloCfd;

                if (selloDigitalEmisor != "")
                {
                    if (selloDigitalEmisor.Length > 8)
                    {
                        selloDigitalEmisor = selloDigitalEmisor.Substring(selloDigitalEmisor.Length - 8, 8);
                        selloDigitalEmisor = selloDigitalEmisor.Replace("=", "%3D");
                    }
                }

                string textCode = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx" + @"?id=" + uuid + @"&re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&fe=" + selloDigitalEmisor;

                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(textCode, out qrCode);
                //GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Four), Brushes.Black, Brushes.White);
                GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Four), Brushes.Black, Brushes.White);
                //new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Two)).Draw(g, qrCode.Matrix);

                Stream memoryStream = new MemoryStream();

                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
                memoryStream.Position = 0;

                #endregion

                #region "FIRMA DEL EMPLEADO -  TOTAL RECIBO"

                var tblFirma = new PdfPTable(3);

                tblFirma.TotalWidth = anchoTabla;
                tblFirma.LockedWidth = true;

                tblFirma.AddCell(
                    new PdfPCell(
                        new Phrase(
                            "Recibí de " + registrosPatronal +
                            " por concepto de pago total de mi salario y demás percepciones del periodo" +
                            " indicado sin que a la fecha se me adeude cantidad alguna por ningún concepto.",
                            _font6))
                    {
                        Border = 0,
                        HorizontalAlignment = 0
                    });
                tblFirma.AddCell(
                    new PdfPCell(new Phrase("_______________________________________\n Firma del colaborador", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 1
                    });



                tblFirma.AddCell(new PdfPCell(new Phrase("TOTAL $ " + totalComprobante.ToCurrencyFormat(), _font13B))
                {
                    Border = 0,
                    Colspan = 1,
                    HorizontalAlignment = 2
                });

                documento.Add(tblFirma);

                //documento.Add(new Paragraph(" "));

                #endregion

                #region AGREGA LOGO CBB Y DATOS DE CERTIFICACION DEL CFDI 

                var tblCbb = new PdfPTable(3);
                var tblCfdidatos = new PdfPTable(2);
                tblCbb.TotalWidth = anchoTabla;
                tblCbb.LockedWidth = true;
                tblCbb.DefaultCell.Border = 0;

                Image logoCBB;
                // logoCBB = iTextSharp.text.Image.GetInstance(_pathArchivosXml + @"\\ImagenesCBB\\" + rfcrecep + ".jpg");

                logoCBB = Image.GetInstance(memoryStream);
                //logoCBB.ScaleToFit(150f, 150f);
                //logoCBB.ScaleAbsolute(150f, 150f);



                //tblCbb.AddCell(new PdfPCell(logoCBB, true) { Border = 0 });
                tblCbb.AddCell(new PdfPCell(new Phrase(" ", _font7B)) { Border = 0 });

                var leyendaTabla = conTimbrado ? "Este documento es una representación impresa de un CFDI" : "-";

                tblCfdidatos.AddCell(new PdfPCell(new Phrase(leyendaTabla, _font7BW)) { Colspan = 2, BackgroundColor = colorBackAzulOscuro });
                var tbldatos = new PdfPTable(1);
                tbldatos.AddCell(new PdfPCell(new Phrase("Serie del Certificado del Emisor:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Folio Fiscal UUID:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("No. certificado del SAT:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Fecha y Hora de certificación:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Método de pago:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Forma de pago:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Moneda:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Uso CFDI:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
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

                tblcontenido.AddCell(new PdfPCell(new Phrase($"{metodoPdf} Pago en una sola exhibición", _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });

                tblcontenido.AddCell(new PdfPCell(new Phrase($"{formaDePago} Por definir", _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });

                tblcontenido.AddCell(new PdfPCell(new Phrase(moneda, _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });

                tblcontenido.AddCell(new PdfPCell(new Phrase($"{usoCfdi} Por definir", _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });

                tblCfdidatos.AddCell(new PdfPCell(tbldatos) { });
                tblCfdidatos.AddCell(new PdfPCell(tblcontenido) { });

                tblCbb.AddCell(new PdfPCell(tblCfdidatos) { Colspan = 2 });

                documento.Add(tblCbb);

                documento.Add(new Paragraph(" ")); // -- este

                #endregion

                #region SELLOS Y CADENA ORIGINAL

                var tblSellos = new PdfPTable(8);
                tblSellos.TotalWidth = anchoTabla;
                tblSellos.LockedWidth = true;
                int colspanSet = 6;

                tblSellos.AddCell(new PdfPCell(logoCBB, true)
                {
                    HorizontalAlignment = 0,
                    Colspan = 2,
                    Rowspan = 8,
                    BorderWidth = 1f
                });

                tblSellos.AddCell(new PdfPCell(new Phrase("Sello Digital del CFDI", _font7BW))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro,
                    Colspan = colspanSet
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloCfd, _font7)) { Border = 0, Colspan = colspanSet });
                tblSellos.AddCell(new PdfPCell(new Phrase("    ", _font7B)) { Border = 0, Colspan = colspanSet });

                tblSellos.AddCell(new PdfPCell(new Phrase("Sello del SAT", _font7BW))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro,
                    Colspan = colspanSet
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloSat, _font7)) { Border = 0, Colspan = colspanSet });
                tblSellos.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0, Colspan = colspanSet });


                tblSellos.AddCell(
                    new PdfPCell(new Phrase("Cadena Original del complemento del certificado del SAT", _font7BW))
                    {
                        Border = 0,
                        BackgroundColor = colorBackAzulOscuro,
                        Colspan = colspanSet
                    });
                tblSellos.AddCell(new PdfPCell(new Phrase(cadenaOriginal, _font7)) { Border = 1, Colspan = colspanSet });
                                
                tblSellos.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0, Colspan = 10 });
                tblSellos.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0, Colspan = 10 });
                tblSellos.AddCell(new PdfPCell(new Phrase("Se puso a mi disposición el archivo XML correspondiente y recibí de la empresa arriba mencionada la cantidad neta a que este documento se refiere, estando conforme con las percepciones y deducciones que en él aparecen especificados", _font7B)) { Border = 0, Colspan = 10 });

                documento.Add(tblSellos);

                #endregion

                #endregion


            }
            catch (Exception ex)
            {
                documento.Add(new Paragraph($" ? - Nomina: {idNomina}  Periodo:{periodoDatos.IdSucursal}"));
            }
            finally
            {
                documento.NewPage();

                dataSet.Dispose();
                dataSet.Clear();
            }

        }


        private void AddReciboPdfAsimilados33(ref Document documento, string strXml, NOM_PeriodosPago periodoDatos,
 List<C_TipoJornada_SAT> listaTipoJornada,
        List<C_RegimenFiscal_SAT> listaRegimenFiscal, int idNomina = 0, string cadenaOriginal = "cadena original", bool conTimbrado = false)
        {
            DataSet dataSet = new DataSet();
            try
            {
                int anchoTabla = 550;
                string lugarExpedicion = "";
                //BaseColor colorBackAzulOscuro = new BaseColor(36, 142, 167);    //BaseColor.LIGHT_GRAY;;
                BaseColor colorBackAzulOscuro = new BaseColor(36, 142, 167);//new BaseColor(5, 95, 130);
                BaseColor colorBackAzulClaro = new BaseColor(40, 162, 189);//0, 204, 204//90, 151, 206
                BaseColor colorBackGray = new BaseColor(102, 102, 102);

                //Carga el contenido del XML en el DATASET
                StringReader streamReader = new StringReader(strXml);
                dataSet.ReadXml(streamReader);

                #region "VARIABLES"

                string cnx = string.Empty,
                    //fechaIniPago = string.Empty,
                    //fechaFinPago = string.Empty,
                    //fechaPago = string.Empty,
                    curp = string.Empty,
                    //depa = string.Empty,
                    //puesto = string.Empty,
                    //registrosPatronal = string.Empty,
                    fechaInicioLaboral = string.Empty;
                //tipoJornada = string.Empty;
                //sdi = string.Empty,
                //sbc = string.Empty,
                //nss = string.Empty;

                string nombreReceptor = string.Empty,
                    rfcrecep = string.Empty,
                    rfcEmisor = string.Empty,
                    //municipioEmisor = string.Empty,
                    //estadoEmisor = string.Empty,
                    regimenEmisior = string.Empty,

                    //formaPago = string.Empty,
                    metodoPago = string.Empty,
                    numCertificadoEmisor = "--",
                    //fechaComprobante = "--",
                    //serie = "--",
                    //folio = string.Empty,
                    fechaTimbrado = "----/--/--T00:00:00",
                    certificadoSat = "--",
                    uuid = "00000000-0000-0000-0000-000000000000",
                    selloCfd = "--",
                    selloSat = "--";

                string totalComprobante = string.Empty, totalPercepciones = string.Empty, totalDeducciones = string.Empty, totalOtrosPagos = string.Empty;

                string totalGravado = string.Empty, totalExento = string.Empty;

                string usoCfdi = string.Empty;
                string formaDePago = string.Empty;
                string moneda = string.Empty;
                //  string cadenaOriginal = string.Empty;

                string fechaIniPago = string.Empty;
                string fechaFinPago = string.Empty;
                string fechaPago = string.Empty;
                string numDiasPagados = string.Empty;
                string version = string.Empty;

                string ClaveProdServ = "84111505";
                string Cantidad = "1";
                string ClaveUnidad = "ACT";
                string Descripcion = "Pago de nómina";
                string ValorUnitario = "0.00";
                string Importe = "0.00";
                string Descuento = "0.00";
                #endregion

                #region SET DATOS NOMINA - EMISOR - RECEPTOR 

                //TAG Nomina --
                if (dataSet.Tables.Contains("Nomina"))
                {
                    foreach (DataRow renglon in dataSet.Tables["Nomina"].Rows)
                    {
                        fechaIniPago = Convert.ToString(renglon["FechaInicialPago"]);
                        fechaFinPago = Convert.ToString(renglon["FechaFinalPago"]);
                        fechaPago = Convert.ToString(renglon["FechaPago"]);
                        numDiasPagados = Convert.ToString(renglon["NumDiasPagados"]);

                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalPercepciones"))
                            totalPercepciones = Convert.ToString(renglon["TotalPercepciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalDeducciones"))
                            totalDeducciones = Convert.ToString(renglon["TotalDeducciones"]);
                        if (dataSet.Tables["Nomina"].Columns.Contains("TotalOtrosPagos"))
                            totalOtrosPagos = Convert.ToString(renglon["TotalOtrosPagos"]);
                    }
                }

                #region Tag Receptor

                //Tag RECEPTOR dentro del Tag COMPROBANTE
                foreach (DataRow recep in dataSet.Tables[2].Rows)
                {
                    nombreReceptor = Convert.ToString(recep["nombre"]);
                    rfcrecep = Convert.ToString(recep["rfc"]);
                    usoCfdi = Convert.ToString(recep["UsoCFDI"]);
                }

                //TAG RECEPTOR dentro del tag NOMINA
                foreach (DataRow recep in dataSet.Tables[7].Rows)//
                {
                    cnx = recep["NumEmpleado"].ToString();
                    curp = Convert.ToString(recep["CURP"]);
                    //fechaInicioLaboral = Convert.ToString(recep["FechaInicioRelLaboral"]);
                }

                #endregion

                #region Tag  Emisor                                                                     

                //TAG EMISOR dentro del tag de comprobante
                string nombreEmisor = "";

                foreach (DataRow emisor in dataSet.Tables[1].Rows)
                {
                    nombreEmisor = Convert.ToString(emisor["nombre"]);
                    rfcEmisor = emisor["rfc"].ToString();
                    regimenEmisior = Convert.ToString(emisor["RegimenFiscal"]);
                }



                //TAG Domicilio Fiscal
                //foreach (DataRow domi in dataSet.Tables["DomicilioFiscal"].Rows)
                //{
                //    municipioEmisor = Convert.ToString(domi["municipio"]);
                //    estadoEmisor = Convert.ToString(domi["estado"]);
                //}

                //TAG Regimen Fiscal
                if (dataSet.Tables.Contains("RegimenFiscal"))
                {
                    foreach (DataRow regimen in dataSet.Tables["RegimenFiscal"].Rows)
                    {
                        regimenEmisior = Convert.ToString(regimen["Regimen"]);
                    }
                }

                #endregion

                #endregion

                #region SET DATOS COMPROBANTE - TIMBRE FISCAL DIGITAL

                //TAG Comprobante
                if (dataSet.Tables.Contains("Comprobante"))
                {
                    if (dataSet.Tables["Comprobante"] != null)
                        foreach (DataRow renglon in dataSet.Tables["Comprobante"].Rows)
                        {
                            totalComprobante = Convert.ToString(renglon["total"]);
                            formaDePago = Convert.ToString(renglon["formaPago"]);
                            metodoPago = Convert.ToString(renglon["metodoPago"]);
                            numCertificadoEmisor = conTimbrado ? Convert.ToString(renglon["noCertificado"]) : "noCertificado?";
                            moneda = Convert.ToString(renglon["moneda"]);
                            //fechaComprobante = Convert.ToString(renglon["fecha"]);
                            //serie = Convert.ToString(renglon["serie"]);
                            //folio = Convert.ToString(renglon["folio"]);
                            lugarExpedicion = Convert.ToString(renglon["LugarExpedicion"]);
                            version = Convert.ToString(renglon["Version"]);
                        }
                }


                //TAG - Concepto
                if (dataSet.Tables.Contains("Concepto"))
                {
                    if (dataSet.Tables["Concepto"] != null)
                        foreach (DataRow renglon in dataSet.Tables["Concepto"].Rows)
                        {
                            ClaveProdServ = Convert.ToString(renglon["ClaveProdServ"]);
                            Cantidad = Convert.ToString(renglon["Cantidad"]);
                            ClaveUnidad = Convert.ToString(renglon["ClaveUnidad"]);
                            Descripcion = Convert.ToString(renglon["Descripcion"]);
                            ValorUnitario = Convert.ToString(renglon["ValorUnitario"]);
                            Importe = Convert.ToString(renglon["Importe"]);
                            try
                            {
                                Descuento = Convert.ToString(renglon["Descuento"]);
                            }
                            catch (Exception)
                            {
                                Descuento = "0.00";
                            }
                        }
                }

                //TAG Timbre Fiscal
                if (dataSet.Tables.Contains("TimbreFiscalDigital"))
                {
                    if (dataSet.Tables["TimbreFiscalDigital"] != null)
                        foreach (DataRow timbre in dataSet.Tables["TimbreFiscalDigital"].Rows)
                        {
                            fechaTimbrado = Convert.ToString(timbre["FechaTimbrado"]);
                            certificadoSat = Convert.ToString(timbre["noCertificadoSAT"]);
                            uuid = Convert.ToString(timbre["UUID"]);
                            selloCfd = Convert.ToString(timbre["selloCFD"]);
                            selloSat = Convert.ToString(timbre["selloSAT"]);
                        }
                }

                #endregion

                #region DISEÑO DEL CONTENIDO DE LA PÁGINA DEL DOCUMENTO PDF

                #region DATOS DEL EMISOR

                PdfPTable tablaEmisor = new PdfPTable(6)
                {
                    TotalWidth = anchoTabla,
                    LockedWidth = true

                };

                tablaEmisor.DefaultCell.Border = 0;
                //NOMBRE DE LA EMPRESA EMISORA
                //ROW 1
                PdfPCell cell = new PdfPCell(new Phrase(nombreEmisor, _font14W))
                {
                    Colspan = 5,
                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 1,
                    Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER
                };

                tablaEmisor.AddCell(cell);

                tablaEmisor.AddCell(new PdfPCell(new Phrase("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd"), _font8W))
                {
                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER,
                    HorizontalAlignment = 2
                });

                //ROW 2
                var strRegimenEmisor = "";

                var itemRegimen = listaRegimenFiscal.FirstOrDefault(x => x.ClaveRegimen == regimenEmisior);

                if (itemRegimen != null)
                {
                    strRegimenEmisor = $"{regimenEmisior} {itemRegimen.Descripcion}";
                }
                else
                {
                    strRegimenEmisor = regimenEmisior;
                }

                tablaEmisor.AddCell(new PdfPCell(new Phrase($"Regimen Fiscal: {strRegimenEmisor}", _font10W))
                {
                    Colspan = 5,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = Rectangle.LEFT_BORDER,
                    HorizontalAlignment = 1
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font10))
                {
                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                    HorizontalAlignment = 1
                });

                //ROW EN BLANCO
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    Colspan = 5,
                    Border = Rectangle.LEFT_BORDER,
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                });

                //ROW 3
                //ROW 3
                tablaEmisor.AddCell(new PdfPCell(new Phrase("RFC ", _font8BW))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = Rectangle.LEFT_BORDER,
                    HorizontalAlignment = 1
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font8BW))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("Tipo de Comprobante", _font8BW))
                {
                    Colspan = 2,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });




                tablaEmisor.AddCell(new PdfPCell(new Phrase("Lugar Expedición", _font8BW))
                {
                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 1,
                    Border = 0
                });



                tablaEmisor.AddCell(new PdfPCell(new Phrase("Versión", _font8BW))
                {
                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                    HorizontalAlignment = 1
                });

                //ROW 4
                tablaEmisor.AddCell(new PdfPCell(new Phrase(rfcEmisor, _font8W))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = Rectangle.LEFT_BORDER,
                    HorizontalAlignment = 1
                });
                tablaEmisor.AddCell(new PdfPCell(new Phrase("", _font8W))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });

                tablaEmisor.AddCell(new PdfPCell(new Phrase("N - Nomina", _font8W))
                {
                    Colspan = 2,
                    BackgroundColor = colorBackAzulOscuro,
                    Border = 0,
                    HorizontalAlignment = 1
                });


                tablaEmisor.AddCell(new PdfPCell(new Phrase(lugarExpedicion, _font8W))
                {

                    BackgroundColor = colorBackAzulOscuro,
                    HorizontalAlignment = 1,
                    Border = 0
                });



                tablaEmisor.AddCell(new PdfPCell(new Phrase("3.3", _font8W))
                {

                    BackgroundColor = colorBackAzulClaro,
                    Border = Rectangle.RIGHT_BORDER,
                    HorizontalAlignment = 1
                });


                //ROW 6
                tablaEmisor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = Rectangle.TOP_BORDER,
                    Colspan = 6,
                    BorderWidthTop = 1f,
                    HorizontalAlignment = 2
                });


                documento.Add(tablaEmisor);

                //Rectangle rectangle = new Rectangle(37, 818, 557, 770) //37, 818, 557, 760
                //{
                //    BackgroundColor = colorBackAzulOscuro,
                //    BorderWidth = 1,
                //    Border = 3,
                //    BorderColor = BaseColor.BLACK
                //};

                // documento.Add(new Paragraph(" "));
                //documento.Add(rectangle);

                #endregion

                #region DATOS DEL RECEPTOR

                PdfPTable tblContendorReceptor = new PdfPTable(2);
                PdfPTable tblDatosReceptor = new PdfPTable(2);
                PdfPTable tblDatosReceptorNomina = new PdfPTable(2);

                tblDatosReceptor.DefaultCell.Border = 0;
                tblDatosReceptor.AddCell(
                    new PdfPCell(new Phrase("Num: " + cnx + " - " + nombreReceptor, _font8B)) { Colspan = 2, Border = 0 });
                tblDatosReceptor.AddCell(new Phrase("RFC :", _font7B));
                tblDatosReceptor.AddCell(new Phrase(rfcrecep, _font7));
                tblDatosReceptor.AddCell(new Phrase("CURP :", _font7B));
                tblDatosReceptor.AddCell(new Phrase(curp, _font7));
                //tblDatosReceptor.AddCell(new Phrase("Uso CFDI :", _font7B));
                //tblDatosReceptor.AddCell(new Phrase($"{usoCfdi} Por definir", _font7));
                //tblDatosReceptor.AddCell(new Phrase("", _font7));//Fecha Inicio Laboral
                //tblDatosReceptor.AddCell(new Phrase(fechaInicioLaboral, _font7));

                tblDatosReceptorNomina.DefaultCell.Border = 0;
                tblDatosReceptorNomina.AddCell(new Phrase("Periodo :", _font8B));

                //var FPI = (DateTime)periodoDatos.Fecha_Inicio;
                //var FPF = (DateTime)periodoDatos.Fecha_Fin;
                //var FP = (DateTime)periodoDatos.Fecha_Pago;

                tblDatosReceptorNomina.AddCell(new Phrase("MENSUAL", _font8B));
                tblDatosReceptorNomina.AddCell(new Phrase("Días de Pago : " + numDiasPagados, _font7B));
                tblDatosReceptorNomina.AddCell(new Phrase("Fecha Pago : " + fechaPago, _font7));

                tblContendorReceptor.TotalWidth = anchoTabla;
                tblContendorReceptor.LockedWidth = true;

                tblContendorReceptor.AddCell(tblDatosReceptor);
                tblContendorReceptor.AddCell(tblDatosReceptorNomina);

                #endregion

                tblContendorReceptor.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Colspan = 2,
                    Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER,
                    BorderWidthBottom = 1f,
                    BorderWidthTop = 1f


                });
                documento.Add(tblContendorReceptor);
                //documento.Add(new Paragraph(" "));

                #region CONCEPTO - Unidad - Cantidad - 

                PdfPTable tablaConcepto = new PdfPTable(7)
                {
                    TotalWidth = anchoTabla,
                    LockedWidth = true
                };

                tablaConcepto.DefaultCell.Border = 0;


                tablaConcepto.AddCell(new PdfPCell(new Phrase("Clave ProdServ", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });

                tablaConcepto.AddCell(new PdfPCell(new Phrase("Cantidad", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Clave Unidad", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Descripción", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Valor Unitario", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Importe", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase("Descuento", _font8BW))
                {
                    BackgroundColor = colorBackGray,
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });



                tablaConcepto.AddCell(new PdfPCell(new Phrase(ClaveProdServ, _font8B))
                {

                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });

                tablaConcepto.AddCell(new PdfPCell(new Phrase(Cantidad, _font8B))
                {
                    BorderColor = colorBackGray,
                    HorizontalAlignment = 1,

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(ClaveUnidad, _font8B))
                {
                    BorderColor = colorBackGray,
                    HorizontalAlignment = 1,

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(Descripcion, _font8B))
                {

                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(ValorUnitario, _font8B))
                {
                    BorderColor = colorBackGray,
                    HorizontalAlignment = 1,

                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(Importe, _font8B))
                {

                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });
                tablaConcepto.AddCell(new PdfPCell(new Phrase(Descuento, _font8B))
                {
                    HorizontalAlignment = 1,
                    BorderColor = colorBackGray
                });


                #endregion

                tablaConcepto.AddCell(new PdfPCell(new Phrase(" ", _font10B))
                {
                    Border = Rectangle.BOTTOM_BORDER,
                    BorderWidthBottom = 1f,
                    Colspan = 7,
                    HorizontalAlignment = 2
                });

                documento.Add(tablaConcepto);

                #region  TABLAS DE CONTENIDO - PERCEPCIONES - DEDUCCIONES - OTRO PAGO - TOTAL RECIBO

                var columnas = 8; //Numero de columnas de las tablas

                #region TABLA PERCEPCION
                //variables
                // var totalPercepcionGravado = 0.00;
                // var totalPercepcionExento = 0.00;
                // var totalPercepcionDecimal = 0.00;

                if (dataSet.Tables.Contains("Percepcion"))
                {

                    //1) definicion de la tabla
                    var tablaPercepciones = new PdfPTable(columnas);

                    //3) Subtitulos  - Cve SAT  - Concepto - Gravado - Exento - Total
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Clave", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Percepciones", _font8BW))
                    {
                        Colspan = (columnas - 4),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Gravado", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Exento", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("Importe", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - longitud de la tabla
                    tablaPercepciones.TotalWidth = anchoTabla;
                    tablaPercepciones.LockedWidth = true;



                    // Get Totales
                    if (dataSet.Tables["Percepciones"] != null)
                        foreach (DataRow perceps in dataSet.Tables["Percepciones"].Rows)
                        {
                            totalGravado = Convert.ToString(perceps["TotalGravado"]);//3
                            totalExento = Convert.ToString(perceps["TotalExento"]);//4
                        }

                    //5) Detalle conceptos
                    if (dataSet.Tables["Percepcion"] != null)
                        foreach (DataRow percep in dataSet.Tables["Percepcion"].Rows)
                        {
                            var importeExcento = double.Parse(percep[4].ToString());
                            var importeGravado = double.Parse(percep[3].ToString());
                            var concepto = Convert.ToString(percep[2]);
                            var claveSat = Convert.ToString(percep[1]);
                            var tipo = Convert.ToString(percep[0]);

                            var totalImporte = importeExcento + importeGravado;
                            // totalPercepcionDecimal += totalImporte;

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            // tblPercepciones.AddCell(new PdfPCell(new Phrase(item.IdConcepto.ToString().PadLeft(3, '0'), _font7)) { Border = 0 });

                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 4)
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeGravado.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(importeExcento.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                            tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    //6) TOTAL PERCEPCIONES - GRAVA - EXENTA - TOTAL

                    tablaPercepciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 3) });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(str: totalGravado.ToCurrencyFormat(signo: true), font: _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalExento.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(totalPercepciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    //documento.Add(new Paragraph(" "));
                    tablaPercepciones.AddCell(new PdfPCell(new Phrase(" ", _font7))
                    {
                        Border = Rectangle.BOTTOM_BORDER,
                        BorderWidthBottom = 1f,
                        Colspan = (columnas)
                    });

                    //8) Agregar la tabla al documento
                    documento.Add(tablaPercepciones);
                }

                #endregion

                #region TABLA DEDUCCION
                // Variables
                // var totalDeducciones = 0.0;

                if (dataSet.Tables.Contains("Deduccion"))
                {

                    //1) Definir la tabla
                    var tablaDeucciones = new PdfPTable(columnas);

                    //3) Subtitulos
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Clave", _font8BW))
                    {
                        HorizontalAlignment = 1, //Centrado
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Deducciones", _font8BW))
                    {
                        Colspan = (columnas - 4),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1 //Centrado
                    });

                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("Importe", _font8BW))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaDeucciones.TotalWidth = anchoTabla;
                    tablaDeucciones.LockedWidth = true;



                    //5) Detalle conceptos
                    if (dataSet.Tables["Deduccion"] != null)
                        foreach (DataRow deduc in dataSet.Tables["Deduccion"].Rows)
                        {
                            //var importeExcento = double.Parse(deduc[0].ToString());
                            //var importeGravado = double.Parse(deduc[1].ToString());
                            var totalImporte = double.Parse(deduc[3].ToString());
                            var concepto = Convert.ToString(deduc[2]);
                            var claveSat = Convert.ToString(deduc[1]);
                            var tipo = Convert.ToString(deduc[0]);

                            //  totalDeducciones += totalImporte;

                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7)) { Border = 0 });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }


                    //6) Totales Deducciones
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase(totalDeducciones.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });


                    //7) Agregar espacio entre tablas
                    tablaDeucciones.AddCell(new PdfPCell(new Phrase(" ", _font7))
                    {
                        Border = 0,
                        Colspan = (columnas)
                    });
                    //documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaDeucciones);

                }
                #endregion

                #region TABLA OTROS PAGOS

                // Variables
                // var totalOtroPago = 0.0;

                if (dataSet.Tables.Contains("OtroPago"))
                {

                    //1) definicion de la tabla
                    var tablaOtrosPagos = new PdfPTable(columnas);


                    //3) Subtitulos
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Cve", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Otros Pagos", _font8B))
                    {
                        Colspan = (columnas - 2),
                        BackgroundColor = colorBackAzulOscuro,
                        HorizontalAlignment = 1 //Centrado
                    });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("Importe", _font8B))
                    {
                        HorizontalAlignment = 1,
                        BackgroundColor = colorBackAzulOscuro
                    });

                    //4) Set - Longitud de la Tabla
                    tablaOtrosPagos.TotalWidth = anchoTabla;
                    tablaOtrosPagos.LockedWidth = true;

                    //5) Detalle conceptos
                    if (dataSet.Tables["OtroPago"] != null)
                        foreach (DataRow otroPago in dataSet.Tables["OtroPago"].Rows)
                        {
                            var totalImporte = double.Parse(otroPago[4].ToString());
                            var concepto = Convert.ToString(otroPago[3]);
                            var claveSat = Convert.ToString(otroPago[2]);
                            //totalOtroPago += totalImporte;

                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(claveSat.PadLeft(3, '0'), _font7))
                            {
                                Border = 0
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(concepto, _font7))
                            {
                                Border = 0,
                                Colspan = (columnas - 2)
                            });
                            tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalImporte.ToCurrencyFormat(), _font7))
                            {
                                Border = 0,
                                HorizontalAlignment = 2
                            });
                        }

                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase("", _font7)) { Border = 0, Colspan = (columnas - 1) });
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(totalOtrosPagos.ToCurrencyFormat(signo: true), _font8B))
                    {
                        Border = 0,
                        Colspan = 1,
                        HorizontalAlignment = 2
                    });

                    //7) Agregar espacio entre tablas
                    tablaOtrosPagos.AddCell(new PdfPCell(new Phrase(" ", _font7)) { Border = 0, Colspan = (columnas) });
                    // documento.Add(new Paragraph(" "));

                    //8) Agregar la tabla al documento
                    documento.Add(tablaOtrosPagos);
                }

                #endregion

                #region TOTAL

                // Variables
                //var totalRecibo = 0.0;

                //1) definicion de la tabla
                // var tablaTotalRecibo = new PdfPTable(columnas);

                //2) Set - Longitud de la Tabla
                //tablaTotalRecibo.TotalWidth = anchoTabla;
                //tablaTotalRecibo.LockedWidth = true;

                // totalRecibo = (totalPercepcionExento + totalPercepcionGravado + totalOtroPago) - totalDeducciones;
                // 3) Detalle total
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase("TOTAL $", _font10B)) { Border = 0, Colspan = (columnas-1), HorizontalAlignment = 2 });
                // tablaTotalRecibo.AddCell(new PdfPCell(new Phrase($"{totalRecibo:0,0.##}", _font10B)) { Border = 0, Colspan = 1, HorizontalAlignment = 2 });

                //4 Agregar espacio entre tablas


                //5) Agregar la tabla al documento
                // documento.Add(tablaTotalRecibo);

                #endregion
                documento.Add(new Paragraph(" "));

                #endregion

                #region GENERA LOGO CBB

                string selloDigitalEmisor = selloCfd;

                if (selloDigitalEmisor != "")
                {
                    if (selloDigitalEmisor.Length > 8)
                    {
                        selloDigitalEmisor = selloDigitalEmisor.Substring(selloDigitalEmisor.Length - 8, 8);
                        selloDigitalEmisor = selloDigitalEmisor.Replace("=", "%3D");
                    }
                }

                //string textCode = @"?re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&id=" + uuid;

                string textCode = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx" + @"?id=" + uuid + @"&re=" + rfcEmisor + @"&rr=" + rfcrecep + @"&tt=" + totalComprobante + @"&fe=" + selloDigitalEmisor;

                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(textCode, out qrCode);
                //GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Four), Brushes.Black, Brushes.White);
                GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Four), Brushes.Black, Brushes.White);
                //new GraphicsRenderer(new FixedCodeSize(200, QuietZoneModules.Two)).Draw(g, qrCode.Matrix);

                Stream memoryStream = new MemoryStream();

                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
                memoryStream.Position = 0;

                #endregion

                #region "FIRMA DEL EMPLEADO -  TOTAL RECIBO"

                var tblFirma = new PdfPTable(3);

                tblFirma.TotalWidth = anchoTabla;
                tblFirma.LockedWidth = true;

                tblFirma.AddCell(
                    new PdfPCell(
                        new Phrase(
                            "Recibi de la empresa " + nombreEmisor + " por concepto de Honorarios asimilados del periodo indicado sin que a la fecha se me adeude la cantidad alguna por ningún concepto",
                            _font6))
                    {
                        Border = 0,
                        HorizontalAlignment = 0
                    });
                tblFirma.AddCell(
                    new PdfPCell(new Phrase("_______________________________________\n Firma del colaborador", _font7))
                    {
                        Border = 0,
                        HorizontalAlignment = 1
                    });



                tblFirma.AddCell(new PdfPCell(new Phrase("TOTAL $ " + totalComprobante.ToCurrencyFormat(), _font13B))
                {
                    Border = 0,
                    Colspan = 1,
                    HorizontalAlignment = 2
                });

                documento.Add(tblFirma);

                documento.Add(new Paragraph(" "));

                #endregion

                #region AGREGA LOGO CBB Y DATOS DE CERTIFICACION DEL CFDI 

                var tblCbb = new PdfPTable(3);
                var tblCfdidatos = new PdfPTable(2);
                tblCbb.TotalWidth = anchoTabla;
                tblCbb.LockedWidth = true;
                tblCbb.DefaultCell.Border = 0;

                Image logoCBB;
                // logoCBB = iTextSharp.text.Image.GetInstance(_pathArchivosXml + @"\\ImagenesCBB\\" + rfcrecep + ".jpg");

                logoCBB = Image.GetInstance(memoryStream);
                //logoCBB.ScaleToFit(150f, 150f);

                //tblCbb.AddCell(new PdfPCell(logoCBB) { Border = 0 });
                tblCbb.AddCell(new PdfPCell(new Phrase("", _font7B)) { Border = 0 });

                var leyendaTabla = conTimbrado ? "Este documento es una representación impresa de un CFDI" : "-";

                tblCfdidatos.AddCell(new PdfPCell(new Phrase(leyendaTabla, _font7BW)) { Colspan = 2, BackgroundColor = colorBackAzulOscuro });

                var tbldatos = new PdfPTable(1);
                tbldatos.AddCell(new PdfPCell(new Phrase("Serie del Certificado del Emisor:", _font7B)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Folio Fiscal UUID:", _font7B)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("No. certificado del SAT:", _font7B)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Fecha y Hora de certificación:", _font7B)) { Border = 0, HorizontalAlignment = 2 });

                tbldatos.AddCell(new PdfPCell(new Phrase("Método de pago:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Forma de pago:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Moneda:", _font7B)) { Border = 0, HorizontalAlignment = 2 });
                tbldatos.AddCell(new PdfPCell(new Phrase("Uso CFDI:", _font7B)) { Border = 0, HorizontalAlignment = 2 });

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

                tblcontenido.AddCell(new PdfPCell(new Phrase($"{metodoPdf} Pago en una sola exhibición", _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });

                tblcontenido.AddCell(new PdfPCell(new Phrase($"{formaDePago} Por definir", _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });


                tblcontenido.AddCell(new PdfPCell(new Phrase(moneda, _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });

                tblcontenido.AddCell(new PdfPCell(new Phrase($"{usoCfdi} Por definir", _font7))
                {
                    Border = 0,
                    HorizontalAlignment = 0
                });

                tblCfdidatos.AddCell(tbldatos);
                tblCfdidatos.AddCell(tblcontenido);

                tblCbb.AddCell(new PdfPCell(tblCfdidatos) { Colspan = 2 });

                documento.Add(tblCbb);

                documento.Add(new Paragraph(" "));

                #endregion

                #region SELLOS Y CADENA ORIGINAL

                var tblSellos = new PdfPTable(8);
                tblSellos.TotalWidth = anchoTabla;
                tblSellos.LockedWidth = true;
                int colspanSet = 6;
                //Nuevo Logo

                tblSellos.AddCell(new PdfPCell(logoCBB, true)
                {
                    HorizontalAlignment = 0,
                    Colspan = 2,
                    Rowspan = 8,
                    BorderWidth = 1f
                });
                //-->


                tblSellos.AddCell(new PdfPCell(new Phrase("Sello Digital del CFDI", _font7BW))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro,
                    Colspan = colspanSet
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloCfd, _font7)) { Border = 0, Colspan = colspanSet });

                tblSellos.AddCell(new PdfPCell(new Phrase("    ", _font7B)) { Border = 0, Colspan = colspanSet });

                tblSellos.AddCell(new PdfPCell(new Phrase("Sello del SAT", _font7BW))
                {
                    Border = 0,
                    BackgroundColor = colorBackAzulOscuro,
                    Colspan = colspanSet
                });
                tblSellos.AddCell(new PdfPCell(new Phrase(selloSat, _font7)) { Border = 0, Colspan = colspanSet });
                tblSellos.AddCell(new PdfPCell(new Phrase(" ", _font7B)) { Border = 0, Colspan = colspanSet });

                tblSellos.AddCell(
                    new PdfPCell(new Phrase("Cadena Original del complemento del certificado del SAT", _font7BW))
                    {
                        Border = 0,
                        BackgroundColor = colorBackAzulOscuro,
                        Colspan = colspanSet
                    });
                tblSellos.AddCell(new PdfPCell(new Phrase(cadenaOriginal, _font7)) { Border = 0, Colspan = colspanSet });

                tblSellos.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0, Colspan = 10 });
                tblSellos.AddCell(new PdfPCell(new Phrase("      ", _font7B)) { Border = 0, Colspan = 10 });
                tblSellos.AddCell(new PdfPCell(new Phrase("Se puso a mi disposición el archivo XML correspondiente y recibí de la empresa arriba mencionada la cantidad neta a que este documento se refiere, estando conforme con las percepciones y deducciones que en él aparecen especificados", _font7B)) { Border = 0, Colspan = 10 });

                documento.Add(tblSellos);

                #endregion

                #endregion


            }
            catch (Exception ex)
            {
                documento.Add(new Paragraph($" ? - Nomina: {idNomina}  Periodo:{periodoDatos.IdSucursal}"));
            }
            finally
            {
                documento.NewPage();

                dataSet.Dispose();
                dataSet.Clear();
            }

        }



        #endregion
    }
}