using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Nomina.Procesador;
using RH.Entidades;
using Common.Utils;
using System.IO;
using System.Xml;
using DocumentFormat.OpenXml.Wordprocessing;
using RH.Entidades.GlobalModel;
using SYA.BLL;

namespace Nomina.BLL
{
    public class FacturaElectronica
    {
       // private readonly RHEntities _ctx = null;

        public FacturaElectronica()
        {
          //  _ctx = new RHEntities();
        }

        #region METODOs PARA EL TIMBRADO DEL RECIBOS

        /// <summary>
        /// Ejecuta el timbrado de las nominas que se le proporcionen en el array,
        /// Crea los XML de cada nomina y realiza el timbrado.
        /// Retorna una lista con los id de las nominas timbradas.
        /// </summary>
        /// <param name="idNominas"></param>
        /// <param name="idSucursal"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="timbradoPrueba"></param>
        /// <param name="pathLog"></param>
        /// <returns>Retorna una lista con los id de las nominas timbradas.</returns>
        public async Task<List<NotificationSummary>> GenerarCfdisAsync(int idSucursal, int idEjercicio, int idPeriodo,
            int[] idNominas, string pathLog, int idUsuario, bool timbradoPrueba = true, bool isFiniquito = false, bool isCfdi33 = false)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            //Validar Estatus del Periodo
            var periodoEstatusActual = GetPeriodoPagoById(idPeriodo);

            //No se procede con la generacion del cfdi si el periodo :
            //1) No esta autorizado
            //2) Esta siendo generado por otro usuario
            //3) Se esta procesando las nominas
            if (periodoEstatusActual.Autorizado == false ||
                periodoEstatusActual.CfdiGenerado == (int)GenerarCfdiEstatus.GenerandoCfdi ||
                periodoEstatusActual.Procesando)
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "No se puede continuar por que el periodo no esta Autorizado, o se esta procesando en este momento...",
                    Msg2 = ""
                });
                return summaryList;
            }


            //periodoEstatusActual.CfdiGenerado = (int)GenerarCfdiEstatus.GenerandoCfdi;
            //SaveChange();
            PeriodosPago objPeriodo = new PeriodosPago();
            objPeriodo.UpdateCfdiEstatus(periodoEstatusActual,GenerarCfdiEstatus.GenerandoCfdi);

            //TimbradoCore tc = new TimbradoCore();
            TimbradoCoreV2 tcv2 = new TimbradoCoreV2();
            PDFCore pc = new PDFCore();
            //LogTxtCore.ValidarDirectorio(pathLog);

            //DateTime dt = DateTime.Now;
            //pathLog += dt.Day + dt.Month.ToString() + dt.Year + ".txt";

            //Validar que los IdNominas no hayan sido generadas anteriormente
            idNominas = ValidarRecibosTimbrados(idNominas, idSucursal, idEjercicio, periodoEstatusActual);


            //Si el nuevo array no contiene elementos, entonces retornamos null
            //porque estaban intentado timbrar registros que ya estaban timbrados
            if (idNominas.Length <= 0)
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "Esta intentando timbrar nominas registradas como timbradas",
                    Msg2 = ""
                });
                return summaryList;
            }

            Array.Sort(idNominas);
            //ELIMINAMOS los registros que se generaron anteriormente con estos id de nomina pero pruducieron un error al timbrar
            tcv2.EliminarCfdiByNominaIds(idNominas, idEjercicio, periodoEstatusActual, idSucursal);


            //Generar los XML y el Timbrado.
            //Debug.WriteLine($"Inicio v1 {DateTime.Now}");
            //var nominasTimbradas = await tc.GenerarCfdiAsync(idUsuario, idNominas, idEjercicio, periodoEstatusActual, pathLog, timbradoPrueba, isCfdi33);
            //Debug.WriteLine($"FIN v1 {DateTime.Now}");

            //version 2
            GlobalConfig gconfig = new GlobalConfig();
            var servidorConfig = gconfig.GetGlobalConfig();
            var nominasTimbradas = await tcv2.GenerarCfdiV2Async(idUsuario, idNominas, periodoEstatusActual, servidorConfig, false);
           

            if (nominasTimbradas.Count > 0)
            {
                summaryList.AddRange(nominasTimbradas);
            }

            //Genera los pdf de las nominas timbradas.
            var pErrores = await pc.GenerarPdfAsync(idNominas, idEjercicio, idPeriodo, isFiniquito: isFiniquito, isCfdi33:isCfdi33);


            //Actualizar el Periodo con el estatus de CFDIGenerado a Generado
            //periodoEstatusActual.CfdiGenerado = (int)GenerarCfdiEstatus.Generado;
            //SaveChange();
            objPeriodo.UpdateCfdiEstatus(periodoEstatusActual, GenerarCfdiEstatus.Generado);

            return summaryList;
        }

        public async Task<List<NotificationSummary>> CancelarTimbresAsync(int[] arrayIdTimbrado, int idUsuario)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            if (arrayIdTimbrado.Length <= 0)
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "La lista de Id a cancelar esta vacía",
                    Msg2 = ""
                });
                return summaryList;
            }

            arrayIdTimbrado =  ValidarRecibosCancelados(arrayIdTimbrado);

            if (arrayIdTimbrado.Length <= 0)
            {
                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "Esta intentando cancelar - registros que ya estan cancelados.",
                    Msg2 = ""
                });

                return summaryList;
            }

            TimbradoCoreV2 tc2 = new TimbradoCoreV2();
            var cancelados = await tc2.CancelarTimbresAsync(arrayIdTimbrado, idUsuario);

            summaryList.AddRange(cancelados);

            return summaryList;
        }

        /// <summary>
        /// Metodo que filtra la lista o depura el array de IdNominas a Timbrar
        /// con las nominas que no hayan sido timbrado anteriormente.
        /// </summary>
        /// <param name="arrayIds"></param>
        /// <param name="idSucusal"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="pp"></param>
        /// <returns></returns>
        private int[] ValidarRecibosTimbrados(int[] arrayIds, int idSucusal, int idEjercicio, NOM_PeriodosPago pp)
        {
            //Buscamos en la Tabla las nominas contenidas en el array
            //pero que no esten timbrados correctamente
            int[] listaTimbrados = { };

            if (pp.IdTipoNomina != 11)
            {
                using (var context = new RHEntities())
                {
                    
              
                listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                  where
                                  arrayIds.Contains(t.IdNomina) && t.IdSucursal == idSucusal && t.IdEjercicio == idEjercicio &&
                                  t.IdPeriodo == pp.IdPeriodoPago && t.Cancelado == false && t.IsPrueba == false && t.ErrorTimbrado == false &&
                                  t.IdPeriodo == pp.IdPeriodoPago && t.FolioFiscalUUID != null
                                  select t.IdNomina).ToArray();
                }
            }
            else if (pp.IdTipoNomina == 11) //finiquito
            {
                using (var context = new RHEntities())
                {
                    listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                        where
                        arrayIds.Contains(t.IdFiniquito) && t.IdSucursal == idSucusal && t.IdEjercicio == idEjercicio &&
                        t.IdPeriodo == pp.IdPeriodoPago && t.Cancelado == false && t.IsPrueba == false &&
                        t.ErrorTimbrado == false &&
                        t.IdPeriodo == pp.IdPeriodoPago && t.FolioFiscalUUID != null
                        select t.IdFiniquito).ToArray();
                }

            }


            //Si la lista timbrados no contiene elementos, entonces
            //retornamos el array inicial para que se timbren nuevamente
            if (listaTimbrados.Length <= 0) return arrayIds;

            //Filtra ambas lista para que solo se retorne los id de nominas que no esten timbrados
            var nuevoArray = arrayIds.Where(x => !listaTimbrados.Contains(x)).ToArray();

            return nuevoArray; //retorna un nuevo array de nominas filtradas

        }

        private int[] ValidarRecibosCancelados(int[] arrayIdTimbrados)
        {
            using (var context = new RHEntities())
            {
                //Buscamos en la Tabla los id timbrados contenidas en el array
                //pero que hayan sido timbrados y que ya haya sido cancelados
                int[] listaTimbradosCancelados = { };


                listaTimbradosCancelados = (from t in context.NOM_CFDI_Timbrado
                                            where arrayIdTimbrados.Contains(t.IdNomina)
                                            && t.Cancelado == true
                                            select t.IdNomina).ToArray();



                //Si la lista timbrados no contiene elementos, entonces
                //retornamos el array inicial para que se timbren nuevamente
                if (listaTimbradosCancelados.Length <= 0) return arrayIdTimbrados;

                //Filtra ambas lista para que solo se retorne los id de timbres que no esten cancelados
                var nuevoArray = arrayIdTimbrados.Where(x => !listaTimbradosCancelados.Contains(x)).ToArray();

                return nuevoArray; //retorna un nuevo array de idTimbrados filtradas
            }


        }

        private NOM_PeriodosPago GetPeriodoPagoById(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_PeriodosPago.FirstOrDefault(p => p.IdPeriodoPago == idPeriodo);
            }
            
        }

        //private void SaveChange()
        //{
        //   // _ctx.SaveChanges();
        //}

        #endregion

        /// <summary>
        /// Genera el archivo PDF del xml timbrado - guarda el pdf en la bd en bytes[]
        /// </summary>
        /// <param name="idNominas"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <returns></returns>
        public async Task<int> GenerarRecibosPdfAsync(int[] idNominas, int idEjercicio, int idPeriodo)
        {
            PDFCore pc = new PDFCore();

            var r = await pc.GenerarPdfAsync(idNominas, idEjercicio, idPeriodo);

            return r;
        }

        /// <summary>
        /// Metodo que retorna los archivos pdf en bytes de las nominas solicitadas en el array
        /// </summary>
        /// <param name="idNominas"></param>
        /// <returns></returns>
        public async Task<byte[]> GetArchivosPdfAsync(int[] idNominas)
        {
            PDFCore pc = new PDFCore();

            return await pc.GetArchivosPdfAsync(idNominas);

        }

        /// <summary>
        /// Retorna una lista con las rutas de los archivos generados
        /// </summary>
        /// <param name="idNominas"></param>
        /// <param name="idUsuario"></param>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public async Task<string[]> DownloadRecibosCfdiAsync(int[] idNominas, int idUsuario, string ruta,
            bool isFiniquito = false)
        {
            PDFCore pc = new PDFCore();

            return await pc.DownloadRecibos(idNominas, idUsuario, ruta, isFiniquito: isFiniquito);
        }

        public List<DatosEmision> GetDatosCfdi(int idPeriodo, int idTipoNomina)
        {
            using (var context = new RHEntities())
            {
                List<DatosEmision> lista = new List<DatosEmision>();
                //obtiene las nominas que estan en el periodo y que su total sea mayor a cero
                var listaNominas = context.NOM_Nomina.Where(x => x.IdPeriodo == idPeriodo).Select(x => x.IdNomina).ToList();

                //revisamos cuantas de esas nominas ya han sido timbradas
                var timbrados = context.NOM_CFDI_Timbrado.Where(x => listaNominas.Contains(x.IdNomina)
                                                                  && x.Cancelado == false
                                                                  && x.FechaCertificacion != null
                                                                  && x.FolioFiscalUUID != null
                                                                  && x.ErrorTimbrado == false).ToList();

                switch (idTipoNomina)
                {
                    case 16:
                        lista = (from n in context.NOM_Nomina
                            join e in context.Empleado on n.IdEmpleado equals e.IdEmpleado
                            join em in context.Empresa on n.IdEmpresaAsimilado equals em.IdEmpresa
                            where n.IdPeriodo == idPeriodo
                                  && n.TotalNomina > 0
                            select new DatosEmision()
                            {
                                IdNomina = n.IdNomina,
                                IdEmpleado = n.IdEmpleado,
                                NombreEmpleado = e.APaterno + " " + e.AMaterno + " " + e.Nombres,
                                TotalNomina = n.TotalNomina,
                                MetodoPago = n.IdMetodo_Pago,
                                NombreEmisor = em.RazonSocial,
                                RfcEmisor = em.RFC,
                                RfcReceptor = e.RFC,
                                Generado = n.CFDICreado,
                                IdEmisor = em.IdEmpresa
                            }).ToList();
                        break;
                    case 17:
                        lista = (from n in context.NOM_Nomina
                            join e in context.Empleado on n.IdEmpleado equals e.IdEmpleado
                            join em in context.Empresa on n.IdEmpresaSindicato equals em.IdEmpresa
                            where n.IdPeriodo == idPeriodo
                                  && n.TotalNomina > 0
                            select new DatosEmision()
                            {
                                IdNomina = n.IdNomina,
                                IdEmpleado = n.IdEmpleado,
                                NombreEmpleado = e.APaterno + " " + e.AMaterno + " " + e.Nombres,
                                TotalNomina = n.TotalNomina,
                                MetodoPago = n.IdMetodo_Pago,
                                NombreEmisor = em.RazonSocial,
                                RfcEmisor = em.RFC,
                                RfcReceptor = e.RFC,
                                Generado = n.CFDICreado,
                                IdEmisor = em.IdEmpresa

                            }).ToList();
                        break;
                    default:
                        lista = (from n in context.NOM_Nomina
                            join e in context.Empleado on n.IdEmpleado equals e.IdEmpleado
                            join em in context.Empresa on n.IdEmpresaFiscal equals em.IdEmpresa
                            where n.IdPeriodo == idPeriodo
                                  && n.TotalNomina > 0
                            select new DatosEmision()
                            {
                                IdNomina = n.IdNomina,
                                IdEmpleado = n.IdEmpleado,
                                NombreEmpleado = e.APaterno + " " + e.AMaterno + " " + e.Nombres,
                                TotalNomina = n.TotalNomina,
                                MetodoPago = n.IdMetodo_Pago,
                                NombreEmisor = em.RazonSocial,
                                RfcEmisor = em.RFC,
                                RfcReceptor = e.RFC,
                                Generado = n.CFDICreado,
                                IdEmisor = em.IdEmpresa

                            }).ToList();
                        break;
                }




                foreach (var item in lista)
                {
                    var t = timbrados.FirstOrDefault(x => x.IdNomina == item.IdNomina);
                    var error =
                        context.NOM_CFDI_Timbrado.Where(x => x.IdNomina == item.IdNomina)
                            .Select(x => x.ErrorTimbrado)
                            .FirstOrDefault();
                    //realiza el truncate
                    item.StrTotalNomina = item.TotalNomina.ToCurrencyFormat(signo: false);

                    if (t != null)
                    {
                        item.IdTimbrado = t.IdTimbrado;
                        item.FechaTimbrado = t.FechaCertificacion.ToString();
                        item.Uddi = t.FolioFiscalUUID;
                        item.Version = t.Version;
                        item.Generado = true;
                        item.FechaCancelacion = t.FechaCancelacion != null ? t.FechaCancelacion.ToString() : "--";

                    }
                    item.error = error;
                }

                return lista;

            }
        }

        public List<DatosEmision> GetDatosCfdiFiniquito(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                //obtiene loss finiquito que estan en el periodo
                var finiquito =
                    context.NOM_Finiquito.Where(x => x.IdPeriodo == idPeriodo).Select(x => x.IdFiniquito).ToList();

                //revisamos cuantas de esas nominas ya han sido timbradas
                var timbrados = context.NOM_CFDI_Timbrado.Where(x => finiquito.Contains(x.IdFiniquito)
                                                                  && x.Cancelado == false
                                                                  && x.FechaCertificacion != null
                                                                  && x.FolioFiscalUUID != null
                                                                  && x.ErrorTimbrado == false)
                    .ToList();
                //Datos del colaborador 
                var lista = (from f in context.NOM_Finiquito
                    join e in context.Empleado on f.IdEmpleado equals e.IdEmpleado
                    join em in context.Empresa on f.IdEmpresaFiscal equals em.IdEmpresa
                    where f.IdPeriodo == idPeriodo
                          && f.TOTAL_total > 0
                    select new DatosEmision()
                    {
                        Idfiniquito = f.IdFiniquito,
                        IdEmpleado = f.IdEmpleado,
                        NombreEmpleado = e.APaterno + " " + e.AMaterno + " " + e.Nombres,
                        TotalNomina = f.TOTAL_total,
                        MetodoPago = f.IdMetodo_Pago,
                        NombreEmisor = em.RazonSocial,
                        RfcEmisor = em.RFC,
                        RfcReceptor = e.RFC,
                        Generado = false, //f.cf
                        IdEmisor = em.IdEmpresa
                    }).ToList();


                foreach (var item in lista)
                {
                    var t = timbrados.FirstOrDefault(x => x.IdFiniquito == item.Idfiniquito);

                    //realiza el truncate
                    item.StrTotalNomina = item.TotalNomina.ToCurrencyFormat();

                    if (t != null)
                    {
                        item.IdTimbrado = t.IdTimbrado;
                        item.FechaTimbrado = t.FechaCertificacion.ToString();
                        item.Uddi = t.FolioFiscalUUID;
                        item.Version = t.Version;
                        item.Generado = true;
                        item.FechaCancelacion = t.FechaCancelacion != null ? t.FechaCancelacion.ToString() : "--";
                    }
                }

                return lista;

            }
        }

        public List<DatosEmision> GetDatosParaEmisionDelCfdi(List<int> listaNominas, bool isFiniquito = false)
        {
            using (var context = new RHEntities())
            {
                if (isFiniquito)
                {
                    return (from n in context.NOM_CFDI_Timbrado
                        where listaNominas.Contains(n.IdFiniquito)
                              && n.Cancelado == false
                              && n.FechaCertificacion != null
                              && n.FolioFiscalUUID != null
                        select new DatosEmision
                        {
                            IdTimbrado = n.IdTimbrado,
                            IdNomina = n.IdFiniquito,
                            Idfiniquito = n.IdFiniquito,
                            Uddi = n.FolioFiscalUUID,
                            FechaTimbrado = n.FechaCertificacion.Value.ToString(),
                            Generado = true
                        }).ToList();
                }

                return (from n in context.NOM_CFDI_Timbrado
                    where listaNominas.Contains(n.IdNomina)
                          && n.Cancelado == false
                          && n.FechaCertificacion != null
                          && n.FolioFiscalUUID != null
                    select new DatosEmision
                    {
                        IdTimbrado = n.IdTimbrado,
                        IdNomina = n.IdNomina,
                        Uddi = n.FolioFiscalUUID,
                        FechaTimbrado = n.FechaCertificacion.Value.ToString(),
                        Version = n.Version,
                        Generado = true,
                        FechaCancelacion = n.FechaCancelacion != null ? n.FechaCancelacion.Value.ToString() : "--"
                    }).ToList();
            }
        }

        /// <summary>
        /// Metodo para validar que existan las configuracion de los certificados en la base de datos
        /// y que existan fisicamente antes de realizar el timbrado. 
        /// </summary>
        /// <param name="idEmisor"></param>
        /// <returns></returns>
        public bool ValidarCertificados(int idEmisor,bool isV33, out string summaryMsj) // QUITAR ESTA VALIDACION AL METODO DEL TIMBRADO CrearXmlConSelloYSinTimbre3212 EN TimbradoCore
        {
            bool result = true;

            Empresa itemE = null;
            string _pathCertificados;
            summaryMsj = "";

            using (var context = new RHEntities())
            {

                itemE = context.Empresa.FirstOrDefault(x => x.IdEmpresa == idEmisor);
                var itemConfig = context.SYA_GlobalConfig.FirstOrDefault();

                if (itemE == null) //Si no se encontro la empresa
                {
                    summaryMsj = "- No se encontró datos de la empresa.";
                    return false;
                }
                else if (itemE.ArchivoCER == null || itemE.ArchivoKEY == null || itemE.LlavePrivada == null)
                //Si los campos estan vacios
                {
                    summaryMsj = " Debe configurar los archivos .CER, .KEY y la llave Privada en la BD";
                    return false;
                }

                _pathCertificados = itemConfig.PathCertificados;
            }

            //Validar los archivo fisicos              

            var varArchivoCer = "\\" + itemE.IdEmpresa + "\\" + itemE.ArchivoCER;
            var varArchivoKey = "\\" + itemE.IdEmpresa + "\\" + itemE.ArchivoKEY;
            var pathCadenaOriginalXslt = "";

            if (isV33)
            {
                pathCadenaOriginalXslt = _pathCertificados + "\\" + "cadenaoriginal_3_3_L.xslt";
            }
            else
            {
                pathCadenaOriginalXslt = _pathCertificados + "\\" + "1cadenaoriginal_3_2.xslt";
            }
       


            //<- cambiar la cadena cadenaoriginal_3_2.xslt por una variable global

            //certificado
            string archivoCertificado = _pathCertificados + varArchivoCer;
            string archivoKey = _pathCertificados + varArchivoKey;

            summaryMsj = @"No se encontró el archivo :";
            if (!File.Exists(archivoCertificado))
            {
                summaryMsj += @" .CER ";
                result = false;
            }

            if (!File.Exists(archivoKey))
            {
                summaryMsj += @" .KEY     ";
                result = false;
            }

            if (!File.Exists(pathCadenaOriginalXslt))
            {
                summaryMsj += " para generar la Cadena Original (.xslt)";
                result = false;
            }

            if (result == true)
                summaryMsj = "";
            return result;
        }

        public bool ValidarArchivoPfx(int idEmisor, out string summaryMsj) // QUITAR ESTA VALIDACION AL METODO DEL TIMBRADO CrearXmlConSelloYSinTimbre3212 EN TimbradoCore
        {
            bool result = true;

            Empresa itemE = null;
            string _pathCertificados;
            summaryMsj = "";

            using (var context = new RHEntities())
            {

                itemE = context.Empresa.FirstOrDefault(x => x.IdEmpresa == idEmisor);
                var itemConfig = context.SYA_GlobalConfig.FirstOrDefault();

                if (itemE == null) //Si no se encontro la empresa
                {
                    summaryMsj = "- No se encontró datos de la empresa.";
                    return false;
                }
                else if (itemE.ArchivoPFX == null)
                //Si los campos estan vacios
                {
                    summaryMsj = " Debe configurar el archivo .PFX en la BD para este emisor.";
                    return false;
                }

                _pathCertificados = itemConfig.PathCertificados;
            }

            //Validar los archivo fisicos              

            var varArchivoPfx = "\\" + itemE.IdEmpresa + "\\" + itemE.ArchivoCER;
            //<- cambiar la cadena cadenaoriginal_3_2.xslt por una variable global

            //certificado
            string archivoPfx = _pathCertificados + varArchivoPfx;

            summaryMsj = @"No se encontró el archivo :";
            if (!File.Exists(archivoPfx))
            {
                summaryMsj += @" .PFX ";
                result = false;
            }


            if (result == true)
                summaryMsj = "";
            return result;
        }

        #region METODOS PARA GENERAR RECIBOS DESDE EL PROCESADO DE LA NOMINA - 

        public static async Task<int> GenerarXMLSintimbre(int[] idNominas, NOM_PeriodosPago periodosPago,
            int idEjercicio, int idPeriodo, int idUsuario, bool isCfdi33 = false)
        {
            //TimbradoCore tcCore = new TimbradoCore();
            //var rxml = await tcCore.GenerarXmlAsync(idNominas, periodosPago, idEjercicio, idPeriodo, idUsuario,fromProcesando: true,isCfdi33:isCfdi33);

            TimbradoCoreV2 tcv2 = new TimbradoCoreV2();
            var res = await tcv2.CrearXmlDesdeElProcesado(idUsuario, periodosPago,null, idNominas,true);

            return 1;
        }

        #endregion

        #region METODO PARA GENERAR RECIBO DESDE EL PROCESADO DEL FINIQUITO

        public static async Task<int> GenerarXMLFiniquitoSintimbre(int idFiniquito, NOM_PeriodosPago periodosPago,
            int idEjercicio, int idPeriodo, int idUsuario, bool isCfdi33 = false)
        {
            //TimbradoCore tcCore = new TimbradoCore();
            //var rxml = await tcCore.GenerarXmlFiniquitoAsync(idFiniquito, periodosPago, idEjercicio, idPeriodo,
            //    idUsuario, fromProcesando: true, isCfdi33: isCfdi33);

            TimbradoCoreV2 tcv2 = new TimbradoCoreV2();
            int[] arrayId = new[] { idFiniquito };
            var res = await tcv2.CrearXmlDesdeElProcesado(idUsuario, periodosPago, null, arrayId, true);

            return 1;
        }

        #endregion

        #region metodos detalle errores

        public List<datosPersonales> EmpleadosErrorTimbrado(int idPeriodo) //falta finiquito
        {
            List<NOM_Nomina> listaNominas = new List<NOM_Nomina>();
            List<NOM_Finiquito> listaFiniquitos = new List<NOM_Finiquito>();
            List<Empleado> listaEmpleados;
            var isFiniquito = false;

            using (var context = new RHEntities())
            {
                var itemPeriodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);
                if (itemPeriodo != null && itemPeriodo.IdTipoNomina == 11) //Finiquito
                {
                    isFiniquito = true;
                }

                var listaTimbrado =
                    context.NOM_CFDI_Timbrado.Where(x => x.IdPeriodo == idPeriodo && x.ErrorTimbrado == true).ToList();

                int[] arrayIdEmpleados = null;
                if (isFiniquito)
                {
                    var arrayIdNominas = listaTimbrado.Select(x => x.IdFiniquito).ToArray();

                    listaFiniquitos = (from fin in context.NOM_Finiquito
                                       where arrayIdNominas.Contains(fin.IdFiniquito)
                                       select fin).ToList();

                    arrayIdEmpleados = listaFiniquitos.Select(x => x.IdEmpleado).ToArray();
                }
                else
                {
                    var arrayIdNominas = listaTimbrado.Select(x => x.IdNomina).ToArray();

                    listaNominas = (from nom in context.NOM_Nomina
                                    where arrayIdNominas.Contains(nom.IdNomina)
                                    select nom).ToList();

                    arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();


                }


                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

            }

            if (isFiniquito)
            {
                var listaDatos = (from f in listaFiniquitos
                                  join e in listaEmpleados
                                  on f.IdEmpleado equals e.IdEmpleado
                                  select new datosPersonales
                                  {
                                      IdEmpleado = e.IdEmpleado,
                                      Nombres = e.Nombres,
                                      Paterno = e.APaterno,
                                      Materno = e.AMaterno,
                                      IdNomina = f.IdFiniquito
                                  }).ToList();

                return listaDatos;
            }
            else
            {
                var listaDatos = (from n in listaNominas
                                  join e in listaEmpleados
                                  on n.IdEmpleado equals e.IdEmpleado
                                  select new datosPersonales
                                  {
                                      IdEmpleado = e.IdEmpleado,
                                      Nombres = e.Nombres,
                                      Paterno = e.APaterno,
                                      Materno = e.AMaterno,
                                      IdNomina = n.IdNomina
                                  }).ToList();


                return listaDatos;
            }

        }

        public List<MensajePac> ListadoMensajePac(int idNomina)
        {
            string strMensajePac = "";

            if (idNomina <= 0)
                return null;

            try
            {
                using (var context = new RHEntities())
                {
                    var error = context.NOM_CFDI_Timbrado.FirstOrDefault(x => x.IdNomina == idNomina) ??
                                context.NOM_CFDI_Timbrado.FirstOrDefault(x => x.IdFiniquito == idNomina);

                    if (error != null)
                    {
                        strMensajePac = error.MensajeRespuesta;
                    }
                }

                if (string.IsNullOrEmpty(strMensajePac)) return null;

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(strMensajePac);

                XmlNodeList xnList = xml.SelectNodes("/timbre/errores/Error");

                if (xnList == null) return null;

                return (from XmlNode xn in xnList
                        let co = xn.Attributes["codigo"].Value
                        let ms = xn.Attributes["mensaje"].Value
                        select new MensajePac()
                        {
                            Codigo = co,
                            Mensaje = ms
                        }).ToList();
            }
            catch (Exception e)
            {
                return null;
            }


        }

        #endregion

        public List<NOM_Ejercicio_Fiscal> GetEjercicioFiscales()
        {
            using (var context = new RHEntities())
            {
                return context.NOM_Ejercicio_Fiscal.ToList();
            }
        }

        public List<NOM_PeriodosPago> GetNomPeriodosPagosBySucursal(int idSucursal, int idEjercicio)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_PeriodosPago.Where(x => x.IdSucursal == idSucursal && x.IdEjercicio == idEjercicio).ToList();
            }
        }

        public List<DatosTimbradoCancelacion> GetTimbradosByIdPeriodo(int idPeriodo)
        {
            using (var context = new RHEntities())
            {

                var lista = (from tc in context.NOM_CFDI_Timbrado
                             join e in context.Empleado on tc.IdReceptor equals e.IdEmpleado
                             where tc.IdPeriodo == idPeriodo
                             && tc.ErrorTimbrado == false
                             && tc.Cancelado == false
                             && tc.FolioFiscalUUID != null
                             select new DatosTimbradoCancelacion()
                             {
                                 IdTimbrado = tc.IdTimbrado,
                                 IdEmisor = tc.IdEmisor,
                                 Paterno = e.APaterno,
                                 Materno = e.AMaterno,
                                 Nombres = e.Nombres,
                                 IdNomina = tc.IdNomina,
                                 IdFiniquito = tc.IdFiniquito,
                                 Total = tc.TotalRecibo,
                                 FolioFiscal = tc.FolioFiscalUUID,
                                 FechaTimbrado = tc.FechaCertificacion.Value,
                                 RfcEmisor = tc.RFCEmisor,
                                 RfcReceptor = tc.RFCReceptor,
                                 IdUsuarioTimbro = tc.UsuarioTimbro ?? 0
                             }).ToList();

                if (lista.Count > 0)
                {
                    lista = lista.OrderBy(x => x.Paterno).ToList();
                }

                return lista;
            }
        }

        public List<int> GetNominasTimbradas(int[] nominas, bool esFiniquito)
        {
            using (var context = new RHEntities())
            {
                var lista = (from t in context.NOM_CFDI_Timbrado
                    where nominas.Contains(t.IdNomina)
                          && t.FolioFiscalUUID != null
                    select t.IdNomina).ToList();

                return lista;
            }
        }

    }


    public class DatosTimbradoCancelacion
    {
        public int IdTimbrado { get; set; }
        public int IdEmisor { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int IdNomina { get; set; }
        public int IdFiniquito { get; set; }
        public decimal Total { get; set; }
        public string FolioFiscal { get; set; }
        public DateTime FechaTimbrado { get; set; }
        public string RfcEmisor { get; set; }
        public string RfcReceptor { get; set; }
        public int IdUsuarioTimbro { get; set; }

    }
    public class DatosEmision
    {
        public int IdTimbrado { get; set; }
        public int IdNomina { get; set; }
        public int Idfiniquito { get; set; }
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        //public string Curp { get; set; }
        //public string Nss { get; set; }
        public decimal TotalNomina { get; set; }
        public string StrTotalNomina { get; set; }
        public int MetodoPago { get; set; }
        public string NombreEmisor { get; set; }
        public string RfcEmisor { get; set; }
        public string RfcReceptor { get; set; }
        public string FechaTimbrado { get; set; }
        public string Uddi { get; set; }
        public bool Generado { get; set; }
        public bool error { get; set; }
        public int IdEmisor { get; set; }
        public string Version { get; set; }
        public string FechaCancelacion { get; set; }


    }
    public class datosPersonales
    {
        public int IdEmpleado { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public int IdNomina { get; set; }
    }
    public class MensajePac
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
    }
}
