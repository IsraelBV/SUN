using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using ClosedXML.Excel;
using Common.Utils;
using System.IO;
using System.IO.Compression;
using Common.Enums;
namespace Nomina.BLL
{
    public class Reporte_Timbrado
    {
        //Generar Reporte de nominas timbradas
        //Generar Reporte de nominas canceladas
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="fechaI"></param>
        /// <param name="fechaF"></param>
        /// <param name="pathFolder"></param>
        /// <param name="pathDescarga"></param>
        /// <param name="idEmpresa"></param>
        /// <param name="idSucursal"></param>
        /// <param name="cancelados"></param>
        /// <returns></returns>
        public static string GenerarTimbrados(int idUsuario, int? idEjercicio, DateTime? fechaI, DateTime? fechaF,
            string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0, int idPeriodoB = 0, bool cancelados = false)
        {
            //List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();
            List<Sucursal> listaSucursales = new List<Sucursal>();
            List<Empresa> listaEmpresas = new List<Empresa>();
            //List<Empleado> listaEmpleados = new List<Empleado>();
            List<NOM_CFDI_Timbrado> listaTimbrados = new List<NOM_CFDI_Timbrado>();
            List<Cliente> listaClientes = new List<Cliente>();
            NOM_Ejercicio_Fiscal ejercicioFiscal = new NOM_Ejercicio_Fiscal();
            int[] arrayIdEmpresa;
            string nombreEjercicio = "";

            List<NOM_Nomina> listaNominas = new List<NOM_Nomina>();
            List<NOM_Finiquito> listaFiniquitos = new List<NOM_Finiquito>();

            #region GET DATA

            using (var context = new RHEntities())
            {
                listaEmpresas = context.Empresa.ToList();
                listaSucursales = context.Sucursal.ToList();
                listaClientes = context.Cliente.ToList();

                if (idEmpresa == 0) //todas las empresas
                {
                    arrayIdEmpresa = listaEmpresas.Select(x => x.IdEmpresa).ToArray();
                }
                else //Por empresa
                {
                    arrayIdEmpresa = new int[1];
                    arrayIdEmpresa[0] = idEmpresa;
                }

                //ejercicio fiscal
                ejercicioFiscal = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);
                if (ejercicioFiscal != null)
                {
                    nombreEjercicio = ejercicioFiscal.Anio;
                }


                ////Get Lista de timbrados
                //if (cancelados == false)
                //{
                //    listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                //                      where arrayIdEmpresa.Contains(t.IdEmisor)
                //                            && DbFunctions.TruncateTime(t.FechaCertificacion )>= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(t.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                //                            && t.Cancelado == false
                //                            && t.ErrorTimbrado == false
                //                            && t.FolioFiscalUUID != null
                //                            && t.IdEjercicio == idEjercicio
                //                      select t).ToList();
                //}
                //else
                //{
                //    listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                //                      where arrayIdEmpresa.Contains(t.IdEmisor)
                //                            && DbFunctions.TruncateTime(t.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(t.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                //                            && t.Cancelado == true
                //                            && t.FolioFiscalUUID != null
                //                            && t.IdEjercicio == idEjercicio
                //                      select t).ToList();
                //}


                if (idPeriodoB > 0)
                {
                    listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                      where t.IdEmisor == idEmpresa
                                        && t.IdEjercicio == idEjercicio
                                        && t.IdPeriodo == idPeriodoB
                                            && t.Cancelado == cancelados
                                            && t.ErrorTimbrado == false
                                            && t.FolioFiscalUUID != null

                                      select t).ToList();
                }
                else if (idSucursal > 0)
                {
                    listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                      where t.IdEmisor == idEmpresa
                                        && t.IdEjercicio == idEjercicio
                                        && t.IdSucursal == idSucursal
                                            && t.Cancelado == cancelados
                                            && t.ErrorTimbrado == false
                                            && t.FolioFiscalUUID != null

                                      select t).ToList();
                }
                else if (fechaI != null)
                {
                    listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                      where arrayIdEmpresa.Contains(t.IdEmisor)
                                            && DbFunctions.TruncateTime(t.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(t.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                            && t.Cancelado == true
                                            && t.FolioFiscalUUID != null
                                            && t.IdEjercicio == idEjercicio
                                      select t).ToList();
                }
                else
                {

                }



                //Filtra por sucursal
                if (idSucursal > 0)
                {
                    listaTimbrados = (from t in listaTimbrados
                                      where t.IdSucursal == idSucursal
                                      select t).ToList();
                }


                //GET NOMINAS
                var arrayIdNominas = listaTimbrados.Select(x => x.IdNomina).ToArray();

                //GET FINIQUITOS
                var arrayIdFiniquitos = listaTimbrados.Select(x => x.IdFiniquito).ToArray();


                listaNominas = (from n in context.NOM_Nomina
                                where arrayIdNominas.Contains(n.IdNomina)
                                select n).ToList();

                listaFiniquitos = (from f in context.NOM_Finiquito
                                   where arrayIdFiniquitos.Contains(f.IdFiniquito)
                                   select f).ToList();


                //var arrayPeriodos = listaTimbrados.Select(x => x.IdPeriodo).ToArray();
                //arrayPeriodos = arrayPeriodos.Distinct().ToArray();
                // var arrayEmpleados = listaTimbrados.Select(x => x.IdReceptor).ToArray();
                //arrayEmpleados = arrayEmpleados.Distinct().ToArray();

                //listaPeriodos = (from p in context.NOM_PeriodosPago
                //                 where arrayPeriodos.Contains(p.IdPeriodoPago)
                //                 select p).ToList();

                //listaEmpleados = (from e in context.Empleado
                //                  where arrayEmpleados.Contains(e.IdEmpleado)
                //                  select e).ToList();
            }

            #endregion

            //Crear el documento de excel de timbrados
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Recibos Timbrados");

            #region HEADERS

            worksheet.Cell(2, 1).Value = "IdTimbrado";
            worksheet.Cell(2, 2).Value = "IdNomina";
            worksheet.Cell(2, 3).Value = "IdFiniquito";
            worksheet.Cell(2, 4).Value = "IdPeriodo";
            worksheet.Cell(2, 5).Value = "Empresa";
            worksheet.Cell(2, 6).Value = "Cliente-Sucursal";
            worksheet.Cell(2, 7).Value = "Fecha Certificacion";

            if (cancelados)
            {
                worksheet.Cell(2, 8).Value = "Fecha Cancelacion";
            }
            else
            {
                worksheet.Cell(2, 8).Value = "-";
            }

            worksheet.Cell(2, 9).Value = "RFC Emisor";
            worksheet.Cell(2, 10).Value = "RFC Receptor";
            worksheet.Cell(2, 11).Value = "Folio Fiscal";
            worksheet.Cell(2, 12).Value = "Total Recibo Timbrado";
            worksheet.Cell(2, 13).Value = "Complemento";
            worksheet.Cell(2, 14).Value = "Total Entregado";


            #endregion

            #region CONTENIDO

            int rowIndex = 3;
            foreach (var itemTimbre in listaTimbrados)
            {
                var itemNomina = listaNominas.FirstOrDefault(x => x.IdNomina == itemTimbre.IdNomina);

                worksheet.Cell(rowIndex, 1).Value = itemTimbre.IdTimbrado;
                worksheet.Cell(rowIndex, 2).Value = itemTimbre.IdNomina;
                worksheet.Cell(rowIndex, 3).Value = itemTimbre.IdFiniquito;

                if (itemTimbre.IdFiniquito > 0)
                {
                    //                worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range(rowIndex, 1, rowIndex, 14).Style
                        //.Font.SetFontSize(13)
                        //.Font.SetBold(true)
                        //.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                        .Font.SetFontColor(XLColor.Black)
                        .Fill.SetBackgroundColor(XLColor.Amber);
                }


                worksheet.Cell(rowIndex, 4).Value = itemTimbre.IdPeriodo;

                var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == itemTimbre.IdEmisor);
                worksheet.Cell(rowIndex, 5).Value = itemEmpresa?.RazonSocial ?? "empresa";

                var clienteSucursal = "sucursal";

                var itemSucursal = listaSucursales.FirstOrDefault(x => x.IdSucursal == itemTimbre.IdSucursal);
                if (itemSucursal != null)
                {
                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);
                    clienteSucursal = itemCliente.Nombre + "- " + itemSucursal.Ciudad;
                }

                worksheet.Cell(rowIndex, 6).Value = clienteSucursal;
                worksheet.Cell(rowIndex, 7).Value = itemTimbre.FechaCertificacion;

                if (cancelados)
                {
                    worksheet.Cell(rowIndex, 8).Value = itemTimbre.FechaCancelacion;
                }
                else
                {
                    worksheet.Cell(rowIndex, 8).Value = "";
                }

                worksheet.Cell(rowIndex, 9).Value = itemTimbre.RFCEmisor;
                worksheet.Cell(rowIndex, 10).Value = itemTimbre.RFCReceptor;
                worksheet.Cell(rowIndex, 11).Value = itemTimbre.FolioFiscalUUID;
                worksheet.Cell(rowIndex, 12).Value = itemTimbre.TotalRecibo;

                decimal totalEntregado = 0;
                totalEntregado = itemTimbre.TotalRecibo;

                if (itemNomina != null)
                {
                    worksheet.Cell(rowIndex, 13).Value = itemNomina.TotalComplemento;
                    totalEntregado += itemNomina.TotalComplemento;
                }
                worksheet.Cell(rowIndex, 14).Value = totalEntregado;


                rowIndex++;
            }

            #endregion

            #region DISEÑO

            worksheet.SheetView.Freeze(2, 0);

            //Establece un estilo al header
            worksheet.Range("A2:N2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkRed);


            #endregion

            #region AJUSTE DE LAS CELDAS AL CONTENIDO

            //Ajustar el header al contenido
            worksheet.Column(1).AdjustToContents();
            worksheet.Column(2).AdjustToContents();
            worksheet.Column(3).AdjustToContents();
            worksheet.Column(4).AdjustToContents();
            worksheet.Column(5).AdjustToContents();
            worksheet.Column(6).AdjustToContents();
            worksheet.Column(7).AdjustToContents();
            worksheet.Column(8).AdjustToContents();
            worksheet.Column(9).AdjustToContents();
            worksheet.Column(10).AdjustToContents();
            worksheet.Column(11).AdjustToContents();
            worksheet.Column(12).AdjustToContents();
            worksheet.Column(13).AdjustToContents();
            worksheet.Column(14).AdjustToContents();

            #endregion


            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
            var nombreEmpresaArchivo = "Todas_LasEmpresas";

            if (idEmpresa > 0)
            {
                var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == idEmpresa);
                nombreEmpresaArchivo = itemEmpresa.RazonSocial.Substring(0, 14);
            }

            string nombreArchivo = "Timbrados_" + nombreEmpresaArchivo + "_" + nombreEjercicio + ".xlsx";

            if (cancelados)
            {
                nombreArchivo = "Cancelados_" + nombreEmpresaArchivo + "_" + nombreEjercicio + ".xlsx";
            }

            var fileName = pathUsuario + nombreArchivo;
            workbook.SaveAs(fileName);

            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

        }

        //Generar archivo .zip con los xml y pdf

        private static string GetArchivosCfdi(int idUsuario, int? idEjercicio, DateTime? fechaI, DateTime? fechaF,
            string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0)
        {
            List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();
            List<Sucursal> listaSucursales = new List<Sucursal>();
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<NOM_CFDI_Timbrado> listaTimbrados = new List<NOM_CFDI_Timbrado>();
            NOM_Ejercicio_Fiscal ejercicioFiscal = new NOM_Ejercicio_Fiscal();
            List<Cliente> listaClientes = new List<Cliente>();
            int[] arrayIdEmpresa;
            var pathEjercicio = "";
            var pathZip = "";
            var nombreEjercicio = "";
            var folderArchivoZip = "";
            var nombreArchivoZip = "Archivo.zip";

            #region GET DATA

            using (var context = new RHEntities())
            {
                listaEmpresas = context.Empresa.ToList();
                listaSucursales = context.Sucursal.ToList();
                listaClientes = context.Cliente.ToList();

                if (idEmpresa == 0) //todas las empresas
                {
                    arrayIdEmpresa = listaEmpresas.Select(x => x.IdEmpresa).ToArray();
                }
                else //Por empresa
                {
                    arrayIdEmpresa = new int[1];
                    arrayIdEmpresa[0] = idEmpresa;
                }

                //ejercicio fiscal
                ejercicioFiscal = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);
                if (ejercicioFiscal != null)
                {
                    nombreEjercicio = ejercicioFiscal.Anio;
                    folderArchivoZip = nombreEjercicio + "Zip";
                }

                listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                  where arrayIdEmpresa.Contains(t.IdEmisor)
                                        && DbFunctions.TruncateTime(t.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(t.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                        && t.Cancelado == false
                                        && t.ErrorTimbrado == false
                                        && t.FolioFiscalUUID != null
                                        && t.IdEjercicio == idEjercicio.Value
                                  select t).ToList();


                //Filtra por sucursal
                if (idSucursal > 0)
                {
                    listaTimbrados = (from t in listaTimbrados
                                      where t.IdSucursal == idSucursal
                                      select t).ToList();
                }

                var arrayPeriodos = listaTimbrados.Select(x => x.IdPeriodo).ToArray();
                arrayPeriodos = arrayPeriodos.Distinct().ToArray();

                listaPeriodos = (from p in context.NOM_PeriodosPago
                                 where arrayPeriodos.Contains(p.IdPeriodoPago)
                                 select p).ToList();
            }

            #endregion

            #region PATHS

            //Folder donde se guardará el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            //Folder Ejercicio
            pathEjercicio = pathUsuario + "\\Ejercicio\\" + nombreEjercicio;

            Directory.CreateDirectory(pathEjercicio);

            pathZip = pathUsuario + "\\" + folderArchivoZip;

            Directory.CreateDirectory(pathZip);

            //Actualizamos el path para la descarga

            pathDescarga = pathDescarga + "\\" + idUsuario + "\\" + folderArchivoZip + "\\" + nombreArchivoZip;

            #endregion

            #region CREAR FOLDER Y ARCHIVOS

            foreach (var idEmpre in arrayIdEmpresa)
            {
                var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == idEmpre);
                //Folder Empresa
                var pathFolderEmpresa = pathEjercicio + "\\" + itemEmpresa.RazonSocial;
                Directory.CreateDirectory(pathFolderEmpresa);

                //Obtener las sucursales de la empresa
                var arraySucursalId =
                    listaTimbrados.Where(x => x.IdEmisor == idEmpre).Select(x => x.IdSucursal).ToArray();
                arraySucursalId = arraySucursalId.Distinct().ToArray();

                foreach (var idSuc in arraySucursalId)
                {
                    var itemSucursal = listaSucursales.FirstOrDefault(x => x.IdSucursal == idSuc);
                    if (itemSucursal == null)
                    {
                        continue;
                    }

                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);

                    //Folder Sucursal
                    var pathFolderSucursal = pathFolderEmpresa + "\\" + itemCliente.Nombre + " " + itemSucursal.Ciudad;
                    Directory.CreateDirectory(pathFolderSucursal);

                    //Obtener los periodos de la sucursal
                    var arrayPeriodosId =
                        listaTimbrados.Where(x => x.IdSucursal == idSuc).Select(x => x.IdPeriodo).ToArray();
                    arrayPeriodosId = arrayPeriodosId.Distinct().ToArray();


                    foreach (var idPer in arrayPeriodosId)
                    {
                        var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == idPer);

                        //Folder Periodo
                        var pathFolderPeriodo = pathFolderSucursal + "\\" + itemPeriodo.Descripcion;
                        Directory.CreateDirectory(pathFolderPeriodo);

                        //Obtiene la lista de timbrados del periodo
                        var listaDelPeriodo = listaTimbrados.Where(x => x.IdPeriodo == idPer).ToList();

                        foreach (var itemReg in listaDelPeriodo)
                        {
                            //XML

                            string nombreArchivo = itemReg.RFCReceptor + " - " + itemReg.Serie.Trim() + " - " +
                                                   itemReg.Folio;
                            var pathXml = pathFolderPeriodo + "\\" + nombreArchivo + ".xml";
                            using (StreamWriter archivoX = new StreamWriter(pathXml, false))
                            {
                                archivoX.WriteLine(itemReg.XMLTimbrado);
                            }

                            //PDF
                            var pathPdf = pathFolderPeriodo + "\\" + nombreArchivo + ".pdf";
                            File.WriteAllBytes(pathPdf, itemReg.PDF);

                        }
                    }
                }
            }

            #endregion

            ZipFile.CreateFromDirectory(pathUsuario + "\\Ejercicio\\", pathZip + "\\" + nombreArchivoZip);

            return pathDescarga;

        }

        private static string GetArchivosCfdiMes(int idUsuario, int? idEjercicio, DateTime? fechaI, DateTime? fechaF,
            string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0, bool incluirPdf = false)
        {
            List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();
            List<Sucursal> listaSucursales = new List<Sucursal>();
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<NOM_CFDI_Timbrado> listaTimbrados = new List<NOM_CFDI_Timbrado>();
            NOM_Ejercicio_Fiscal ejercicioFiscal = new NOM_Ejercicio_Fiscal();
            List<Cliente> listaClientes = new List<Cliente>();
            int[] arrayIdEmpresa;
            var pathEjercicio = "";
            var pathZip = "";
            var nombreEjercicio = "";
            var folderArchivoZip = "";

            var strXMLPdf = "XML_";

            if (incluirPdf)
            {
                strXMLPdf = "XML_PDF_";
            }

            var nombreArchivoZip = strXMLPdf + "Archivo.zip";

            #region GET DATA

            using (var context = new RHEntities())
            {
                listaEmpresas = context.Empresa.ToList();
                listaSucursales = context.Sucursal.ToList();
                listaClientes = context.Cliente.ToList();

                if (idEmpresa == 0) //todas las empresas
                {
                    arrayIdEmpresa = listaEmpresas.Select(x => x.IdEmpresa).ToArray();
                }
                else //Por empresa
                {
                    arrayIdEmpresa = new int[1];
                    arrayIdEmpresa[0] = idEmpresa;
                }

                //ejercicio fiscal
                ejercicioFiscal = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);
                if (ejercicioFiscal != null)
                {
                    nombreEjercicio = ejercicioFiscal.Anio;
                    folderArchivoZip = nombreEjercicio + "Zip";
                }

                listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                  where arrayIdEmpresa.Contains(t.IdEmisor)
                                        && DbFunctions.TruncateTime(t.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(t.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                        && t.Cancelado == false
                                        && t.ErrorTimbrado == false
                                        && t.FolioFiscalUUID != null
                                        && t.IdEjercicio == idEjercicio.Value
                                  select t).ToList();


                //Filtra por sucursal
                if (idSucursal > 0)
                {
                    listaTimbrados = (from t in listaTimbrados
                                      where t.IdSucursal == idSucursal
                                      select t).ToList();
                }

                var arrayPeriodos = listaTimbrados.Select(x => x.IdPeriodo).ToArray();
                arrayPeriodos = arrayPeriodos.Distinct().ToArray();

                listaPeriodos = (from p in context.NOM_PeriodosPago
                                 where arrayPeriodos.Contains(p.IdPeriodoPago)
                                 select p).ToList();
            }

            #endregion

            #region PATHS

            //Nombre archivo
            if (idEmpresa == 0)
            {
                nombreArchivoZip = strXMLPdf + "_Todas_LasEmpresas_" + nombreEjercicio + ".zip";
            }
            else
            {
                var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == idEmpresa);

                if (itemEmpresa != null)
                    nombreArchivoZip = strXMLPdf + itemEmpresa.RazonSocial.Substring(0, 14) + "_" + nombreEjercicio +
                                       ".zip";
            }


            //Folder donde se guardará el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            //Folder Ejercicio
            pathEjercicio = pathUsuario + "\\Ejercicio\\" + nombreEjercicio;

            Directory.CreateDirectory(pathEjercicio);

            pathZip = pathUsuario + "\\" + folderArchivoZip;

            Directory.CreateDirectory(pathZip);

            //Actualizamos el path para la descarga

            pathDescarga = pathDescarga + "\\" + idUsuario + "\\" + folderArchivoZip + "\\" + nombreArchivoZip;

            #endregion

            #region CREAR FOLDER Y ARCHIVOS

            var nuevalistaOrdenadaPorFecha = listaTimbrados.OrderBy(x => x.FechaCertificacion).ToList();

            var itemFirst = nuevalistaOrdenadaPorFecha.First();
            var itemLast = nuevalistaOrdenadaPorFecha.Last();

            var listaMeses = Utils.GetMesesEntreDosFechas(itemFirst.FechaCertificacion.Value,
                itemLast.FechaCertificacion.Value);

            #region POR EMPRESA

            foreach (var idEmpre in arrayIdEmpresa)
            {
                var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == idEmpre);

                var pathFolderEmpresa = pathEjercicio + "\\" + itemEmpresa.RazonSocial;
                Directory.CreateDirectory(pathFolderEmpresa); //FOLDER EMPRESA

                //Obtener las sucursales de la empresa
                var arraySucursalId =
                    listaTimbrados.Where(x => x.IdEmisor == idEmpre).Select(x => x.IdSucursal).ToArray();
                arraySucursalId = arraySucursalId.Distinct().ToArray();

                #region FOLDER por MES

                foreach (var itemMes in listaMeses)
                {
                    var nombreMes = ((Mes)itemMes).ToString();
                    var pathFolderMes = pathFolderEmpresa + "\\" + nombreMes;
                    Directory.CreateDirectory(pathFolderMes); //Crear Folder Del MES

                    #region POR SUCURSAL

                    foreach (var idSuc in arraySucursalId)
                    {
                        var itemSucursal = listaSucursales.FirstOrDefault(x => x.IdSucursal == idSuc);
                        if (itemSucursal == null)
                        {
                            continue;
                        }

                        var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);

                        var pathFolderSucursal = pathFolderMes + "\\" + itemCliente.Nombre + " " + itemSucursal.Ciudad;
                        Directory.CreateDirectory(pathFolderSucursal); //FOLDER SUCURSAL

                        //Obtener los periodos de la sucursal
                        var arrayPeriodosId =
                            listaTimbrados.Where(
                                    x => x.IdSucursal == idSuc && x.FechaCertificacion.Value.Month == itemMes)
                                .Select(x => x.IdPeriodo)
                                .ToArray();
                        arrayPeriodosId = arrayPeriodosId.Distinct().ToArray();

                        #region PERIODOS

                        foreach (var idPer in arrayPeriodosId)
                        {
                            var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == idPer);


                            var pathFolderPeriodo = pathFolderSucursal + "\\" + itemPeriodo.IdPeriodoPago + " " + itemPeriodo.Descripcion;

                            Directory.CreateDirectory(pathFolderPeriodo); //Folder Periodo

                            //Obtiene la lista de timbrados del periodo
                            var listaDelPeriodo =
                                listaTimbrados.Where(
                                    x => x.IdPeriodo == idPer && x.FechaCertificacion.Value.Month == itemMes).ToList();

                            #region ARCHIVOS XML y PDF

                            foreach (var itemReg in listaDelPeriodo)
                            {
                                //XML
                                string nombreArchivo = itemReg.RFCReceptor + " - " + itemReg.Serie.Trim() + " - " +
                                                       itemReg.Folio;
                                var pathXml = pathFolderPeriodo + "\\" + nombreArchivo + ".xml";

                                using (StreamWriter archivoX = new StreamWriter(pathXml, false))
                                {
                                    archivoX.WriteLine(itemReg.XMLTimbrado);
                                }

                                //PDF
                                if (!incluirPdf) continue;
                                var pathPdf = pathFolderPeriodo + "\\" + nombreArchivo + ".pdf";
                                File.WriteAllBytes(pathPdf, itemReg.PDF);
                            }

                            #endregion
                        }

                        #endregion

                    }

                    #endregion
                }

                #endregion

            }

            #endregion

            #endregion

            ZipFile.CreateFromDirectory(pathUsuario + "\\Ejercicio\\", pathZip + "\\" + nombreArchivoZip);

            return pathDescarga;

        }

        public static Task<string> DownloadRecibosXml(int idUsuario, int? idEjercicio, DateTime? fechaI,
            DateTime? fechaF, string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0,
            Boolean incluirPdf = false)
        {

            Task tt = Task.Factory.StartNew(() =>
            {
                //Crea el folder para el usuario, si ya existe elimina su contenido.
                // -------- var val = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
                //_pathUsuario = val;
                ////Directory.CreateDirectory(fd1);

            });

            return
                tt.ContinueWith(
                    t =>
                        GetArchivosCfdiMes(idUsuario, idEjercicio, fechaI, fechaF, pathFolder, pathDescarga, idEmpresa,
                            idSucursal, incluirPdf));
        }

        //Prototipo Visor xml
        public static string VisorXmlToExcel(int idUsuario, int idEjercicio, DateTime? fechaI, DateTime? fechaF,
            string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0, int idPeriodoB = 0, bool cancelados = false)
        {
            List<DatosVisor> listaTimbrados = new List<DatosVisor>();
            string nombreEjercicio = "";
            string nombreEmpresa = "";
            string nombreCliente = "";
            string nombreSucursal = "";
            string fechaInicial = fechaI?.ToString("dd-MMM") ?? "";
            string fechaFinal = fechaF?.ToString("dd-MMM") ?? "";



            #region GETDATA
            using (var context = new RHEntities())
            {
                //Hacer el filtro por empresa y sucursal

                if (idPeriodoB > 0) //buscar por periodo
                {

                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && tim.IdPeriodo == idPeriodoB
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();

                }
                else if (fechaI != null) //buscar por fecha
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && DbFunctions.TruncateTime(tim.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) &&
                                            DbFunctions.TruncateTime(tim.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();


                }
                else if (idSucursal > 0)
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdSucursal == idSucursal
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();
                }
                else// por ejercicio y empresa
                {

                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();


                }

                var itemEjercicio =
                    context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);

                if (itemEjercicio != null) nombreEjercicio = itemEjercicio.Anio;

                var itemEmpresa = context.Empresa.FirstOrDefault(x => x.IdEmpresa == idEmpresa);
                if (itemEmpresa != null) nombreEmpresa = itemEmpresa.RazonSocial.Substring(0, 14);

                if (idSucursal > 0)
                {
                    var itemSucursal = context.Sucursal.FirstOrDefault(x => x.IdSucursal == idSucursal);
                    if (itemSucursal != null) nombreSucursal = itemSucursal.Ciudad;

                    var itemCliente = context.Cliente.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);
                    if (itemCliente != null) nombreCliente = itemCliente.Nombre;
                }





            }
            #endregion

            //filtra el dato por la sucursal
            if (idSucursal > 0)
            {
                listaTimbrados = listaTimbrados.Where(x => x.IdSucursal == idSucursal).ToList();
            }

            var workbook = new XLWorkbook();

            //Worksheets
            var wsComprobante = workbook.Worksheets.Add("Comprobante");
            var wsNomina = workbook.Worksheets.Add("Nomina");
            var wsPercepciones = workbook.Worksheets.Add("Percepciones");
            var wsDeducciones = workbook.Worksheets.Add("Deducciones");
            var wsOtrosPagos = workbook.Worksheets.Add("OtrosPagos");
            var wsIncapacidad = workbook.Worksheets.Add("Incapacidad");
            var wsTimbre = workbook.Worksheets.Add("TimbreFiscalDigital");

            wsComprobante.TabColor = XLColor.Black;
            wsNomina.TabColor = XLColor.Black;
            wsPercepciones.TabColor = XLColor.Teal;
            wsDeducciones.TabColor = XLColor.Brown;
            wsOtrosPagos.TabColor = XLColor.Teal;
            wsIncapacidad.TabColor = XLColor.Orange;
            wsTimbre.TabColor = XLColor.Olive;
            // wsComprobante.Style.Fill.BackgroundColor = XLColor.Red;

            #region HEADERS

            #region COMPROBANTE 3.3

            int rowc = 2;
            int col = 1;
            wsComprobante.Cell(rowc, col++).Value = "IdEmpleado";
            wsComprobante.Cell(rowc, col++).Value = "IdPeriodo";
            wsComprobante.Cell(rowc, col++).Value = "IdNomina";
            wsComprobante.Cell(rowc, col++).Value = "Version";
            wsComprobante.Cell(rowc, col++).Value = "Serie";
            wsComprobante.Cell(rowc, col++).Value = "Folio";
            wsComprobante.Cell(rowc, col++).Value = "Fecha";
            wsComprobante.Cell(rowc, col++).Value = "FormaPago";
            wsComprobante.Cell(rowc, col++).Value = "NoCertificado";
            wsComprobante.Cell(rowc, col++).Value = "SubTotal";
            wsComprobante.Cell(rowc, col++).Value = "Descuento";
            wsComprobante.Cell(rowc, col++).Value = "Total";
            wsComprobante.Cell(rowc, col++).Value = "Moneda";
            wsComprobante.Cell(rowc, col++).Value = "TipoComprobante";
            wsComprobante.Cell(rowc, col++).Value = "Metodo Pago";
            wsComprobante.Cell(rowc, col++).Value = "Lugar Expedicion";
            //Emisor 
            wsComprobante.Cell(rowc, col++).Value = "RFC";
            wsComprobante.Cell(rowc, col++).Value = "Nombre";
            wsComprobante.Cell(rowc, col++).Value = "Regimen Fiscal";
            //Receptor
            wsComprobante.Cell(rowc, col++).Value = "Rfc";
            wsComprobante.Cell(rowc, col++).Value = "Nombre";
            wsComprobante.Cell(rowc, col++).Value = "Uso CFDI";
            //Concepto
            wsComprobante.Cell(rowc, col++).Value = "ClaveProdServ";
            wsComprobante.Cell(rowc, col++).Value = "Cantidad";
            wsComprobante.Cell(rowc, col++).Value = "Clave Unidad";
            wsComprobante.Cell(rowc, col++).Value = "Descripcion";
            wsComprobante.Cell(rowc, col++).Value = "Valor Unitario";
            wsComprobante.Cell(rowc, col++).Value = "Importe";
            wsComprobante.Cell(rowc, col++).Value = "Descuento";

            #endregion

            #region NOMINA 1.2

            rowc = 2;
            col = 1;
            wsNomina.Cell(rowc, col++).Value = "IdEmpleado";
            wsNomina.Cell(rowc, col++).Value = "IdPeriodo";
            wsNomina.Cell(rowc, col++).Value = "IdNomina";
            wsNomina.Cell(rowc, col++).Value = "Version";
            wsNomina.Cell(rowc, col++).Value = "Tipo Nomina";
            wsNomina.Cell(rowc, col++).Value = "Fecha Pago";
            wsNomina.Cell(rowc, col++).Value = "Fecha Inicio Pago";
            wsNomina.Cell(rowc, col++).Value = "Fecha Final Pago";
            wsNomina.Cell(rowc, col++).Value = "Num Dias Pagados";
            wsNomina.Cell(rowc, col++).Value = "Total Percecepciones";
            wsNomina.Cell(rowc, col++).Value = "Total Deducciones";
            wsNomina.Cell(rowc, col++).Value = "Total OtrosPagos";
            //Emisor
            wsNomina.Cell(rowc, col++).Value = "Curp";
            wsNomina.Cell(rowc, col++).Value = "Registro Patronal";
            wsNomina.Cell(rowc, col++).Value = "Rfc Patron Origen";
            //Sub Entidad SNCF
            wsNomina.Cell(rowc, col++).Value = "OrigenRecurso";
            wsNomina.Cell(rowc, col++).Value = "MontoRecursoPropio";


            //Receptor
            wsNomina.Cell(rowc, col++).Value = "Curp";
            wsNomina.Cell(rowc, col++).Value = "NSS";
            wsNomina.Cell(rowc, col++).Value = "Fecha Inicio Laboral";
            wsNomina.Cell(rowc, col++).Value = "Antiguedad";
            wsNomina.Cell(rowc, col++).Value = "Tipo Contrato";
            wsNomina.Cell(rowc, col++).Value = "Sindicalizado";
            wsNomina.Cell(rowc, col++).Value = "Tipo Jornada";
            wsNomina.Cell(rowc, col++).Value = "Tipo Regimen";
            wsNomina.Cell(rowc, col++).Value = "Num Empleado";
            wsNomina.Cell(rowc, col++).Value = "Departamento";
            wsNomina.Cell(rowc, col++).Value = "Puesto";
            wsNomina.Cell(rowc, col++).Value = "RiesgoPuesto";

            wsNomina.Cell(rowc, col++).Value = "Periodicidad Pago";
            wsNomina.Cell(rowc, col++).Value = "Banco";
            wsNomina.Cell(rowc, col++).Value = "CuentaBancaria";
            wsNomina.Cell(rowc, col++).Value = "SalarioBaseCotApor";

            wsNomina.Cell(rowc, col++).Value = "Salario Diario Int.";
            wsNomina.Cell(rowc, col++).Value = "Clave Ent Fed";

            //Sub - SubContratacion
            wsNomina.Cell(rowc, col++).Value = "RfcLabora";
            wsNomina.Cell(rowc, col++).Value = "Porcentaje Tiempo";

            #endregion

            #region CONCEPTOS - PERCEPCIONES - DEDUCCIONES - OTROS PAGOS - INCAPACIDADES
            rowc = 2;
            col = 1;
            //Percepciones
            wsPercepciones.Cell(rowc, col++).Value = "IdEmpleado";
            wsPercepciones.Cell(rowc, col++).Value = "IdNomina";
            wsPercepciones.Cell(rowc, col++).Value = "Total Sueldos";
            wsPercepciones.Cell(rowc, col++).Value = "Total Separacion Indemnizacion";
            wsPercepciones.Cell(rowc, col++).Value = "Total Jubilacion Pension Retiro";
            wsPercepciones.Cell(rowc, col++).Value = "Total Gravado";
            wsPercepciones.Cell(rowc, col++).Value = "Total Exento";

            //sub - percepcion
            wsPercepciones.Cell(rowc, col++).Value = "Tipo Percepcion";
            wsPercepciones.Cell(rowc, col++).Value = "Clave";
            wsPercepciones.Cell(rowc, col++).Value = "Concepto";
            wsPercepciones.Cell(rowc, col++).Value = "Importe Gravado";
            wsPercepciones.Cell(rowc, col++).Value = "Importe Exento";

            //sub - Acciones o Titulos
            wsPercepciones.Cell(rowc, col++).Value = "ValorMercado";
            wsPercepciones.Cell(rowc, col++).Value = "PrecioAlOtorgarse";

            //sub Horas extras
            wsPercepciones.Cell(rowc, col++).Value = "Dias";
            wsPercepciones.Cell(rowc, col++).Value = "Tipo Horas";
            wsPercepciones.Cell(rowc, col++).Value = "Horas extras";
            wsPercepciones.Cell(rowc, col++).Value = "Importe Pagado";

            //sub Jubilacion Pension Retiro
            wsPercepciones.Cell(rowc, col++).Value = "TotalUnaExhibicion";
            wsPercepciones.Cell(rowc, col++).Value = "Total Parcialidad";
            wsPercepciones.Cell(rowc, col++).Value = "MontoDiario";
            wsPercepciones.Cell(rowc, col++).Value = "Ingreso Acumulable";
            wsPercepciones.Cell(rowc, col++).Value = "Ingreso No Acumulable";

            //Separacion Indemnizacion
            wsPercepciones.Cell(rowc, col++).Value = "Total Pagado";
            wsPercepciones.Cell(rowc, col++).Value = "NumAñoServicio";
            wsPercepciones.Cell(rowc, col++).Value = "Ultimo sueldo Mensual Ordinario";
            wsPercepciones.Cell(rowc, col++).Value = "Ingreso Acumulable";
            wsPercepciones.Cell(rowc, col++).Value = "Ingreso No Acumulable";

            //Deduccionescol = 1;
            col = 1;
            wsDeducciones.Cell(rowc, col++).Value = "IdEmpleado";
            wsDeducciones.Cell(rowc, col++).Value = "IdNomina";
            wsDeducciones.Cell(rowc, col++).Value = "Total Otras Deducciones";
            wsDeducciones.Cell(rowc, col++).Value = "Total Impuestos Tetenidos";

            wsDeducciones.Cell(rowc, col++).Value = "Tipo Deduccion";
            wsDeducciones.Cell(rowc, col++).Value = "Clave";
            wsDeducciones.Cell(rowc, col++).Value = "Concepto";
            wsDeducciones.Cell(rowc, col++).Value = "Importe";

            //Otros Pagos
            col = 1;
            wsOtrosPagos.Cell(rowc, col++).Value = "IdEmpleado";
            wsOtrosPagos.Cell(rowc, col++).Value = "IdNomina";
            wsOtrosPagos.Cell(rowc, col++).Value = "Tipo Otro Pago";
            wsOtrosPagos.Cell(rowc, col++).Value = "Clave";
            wsOtrosPagos.Cell(rowc, col++).Value = "Concepto";
            wsOtrosPagos.Cell(rowc, col++).Value = "Importe";

            //Subsidio
            wsOtrosPagos.Cell(rowc, col++).Value = "Subsidio Causado";

            //Saldo a Favor
            wsOtrosPagos.Cell(rowc, col++).Value = "Saldo a Favor";
            wsOtrosPagos.Cell(rowc, col++).Value = "Año";
            wsOtrosPagos.Cell(rowc, col++).Value = "Remanente Saldo a Favor";
            #endregion

            #region INCAPACIDADES
            //Incapacidades
            rowc = 2;
            col = 1;
            wsIncapacidad.Cell(rowc, col++).Value = "IdEmpleado";
            wsIncapacidad.Cell(rowc, col++).Value = "IdNomina";
            wsIncapacidad.Cell(rowc, col++).Value = "Dias Incapacidad";
            wsIncapacidad.Cell(rowc, col++).Value = "Tipo Incapacidad";
            wsIncapacidad.Cell(rowc, col++).Value = "Importe Monetario";
            #endregion

            #region TIMBRE FISCAL DIGITAL

            rowc = 2;
            col = 1;
            wsTimbre.Cell(rowc, col++).Value = "IdEmpleado";
            wsTimbre.Cell(rowc, col++).Value = "IdPeriodo";
            wsTimbre.Cell(rowc, col++).Value = "IdNomina";

            wsTimbre.Cell(rowc, col++).Value = "Nombre Empleado";
            wsTimbre.Cell(rowc, col++).Value = "Version";
            wsTimbre.Cell(rowc, col++).Value = "UUID";
            wsTimbre.Cell(rowc, col++).Value = "Fecha Timbrado PAC";
            wsTimbre.Cell(rowc, col++).Value = "Rfc Provedor";
            wsTimbre.Cell(rowc, col++).Value = "NoCertificadoSAT";

            #endregion


            #endregion

            #region CONTENIDO
            //For
            int idEmpleado = 0;
            int idNomina = 0;
            int idPeriodo = 0;
            int rowx = 3;
            DataTable tablex = null;
            int rowp = 3; //termino de percepcion
            int rowd = 3;//termino de deduccion
            int rowo = 3;//termina otros pagos
            int rowi = 3;//row inicial para ambos p y d
            int rowinca = 3;
            bool rowColor = false;
            string strNombreEmpleado = "";
            foreach (var itemX in listaTimbrados)
            {
                if (itemX?.XmlTimbrado == null) continue;
                DataSet ds = new DataSet();
                strNombreEmpleado = "";

                //Carga el contenido del XML en el DATASET
                StringReader streamReader = new StringReader(itemX.XmlTimbrado);
                ds.ReadXml(streamReader);

                //leer id empleado
                idEmpleado = itemX.IdEmpleado;
                idNomina = itemX.IdNomina > 0 ? itemX.IdNomina : itemX.IdFiniquito;

                idPeriodo = itemX.IdPeriodo;
                #region Comprobante

                SetLineaComprobante(ref wsComprobante, ds, rowx, idEmpleado, idNomina, idPeriodo);
                //emisor 
                SetLineaEmisor(ref wsComprobante, ds, rowx, idEmpleado);
                //receptor             
                strNombreEmpleado = SetLineaReceptor(ref wsComprobante, ds, rowx, idEmpleado);
                #endregion

                #region Concepto        
                SetLineaProdServ(ref wsComprobante, ds, rowx, idEmpleado);
                #endregion

                #region NOMINA
                SetLineaNomina(ref wsNomina, ds, rowx, idEmpleado, idNomina, idPeriodo);
                //emisor 
                SetLineaEmisorNom(ref wsNomina, ds, rowx, idEmpleado);
                //receptor
                SetLineaReceptorNom(ref wsNomina, ds, rowx, idEmpleado);
                //color - 

                #endregion

                #region PERCEPCION 
                SetLineaPercepciones(ref wsPercepciones, ds, rowp, idEmpleado, idNomina);
                rowp = SetLineaPercepcion(ref wsPercepciones, ds, rowp, idEmpleado, rowColor);
                #endregion

                #region DEDUCCIONES
                SetLineaDeducciones(ref wsDeducciones, ds, rowd, idEmpleado, idNomina);
                rowd = SetLineaDeduccion(ref wsDeducciones, ds, rowd, idEmpleado, rowColor);
                #endregion

                #region OTROS PAGOS
                rowo = SetLineaOtrosPagos(ref wsOtrosPagos, ds, rowo, idEmpleado, idNomina);
                #endregion

                #region INCAPACIDADES
                rowinca = SetLineaIncapacidades(ref wsIncapacidad, ds, rowinca, idEmpleado, idNomina);
                #endregion


                SetLineaTimbre(ref wsTimbre, ds, rowx, idEmpleado, idNomina, idPeriodo, strNombreEmpleado);

                //solo para conceptos
                //if (rowd > rowi || rowp > rowi || rowo > rowi)
                //{
                //    var rowh = rowd > rowp ? rowd : rowp;
                //    rowi = rowh > rowo ? rowh : rowo;
                //}
                //else
                //{
                //    rowi++;
                //}


                //Color line nomina
                if (rowColor)
                {
                    var rangoNomina = "A" + rowx + ":AK" + rowx;
                    wsNomina.Range(rangoNomina).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    var rangoComprobante = "A" + rowx + ":AC" + rowx;
                    wsComprobante.Range(rangoComprobante).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    var rangoOtrosPagos = "A" + rowx + ":J" + rowx;
                    wsOtrosPagos.Range(rangoOtrosPagos).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    var rangoInc = "A" + rowx + ":E" + rowx;
                    wsIncapacidad.Range(rangoInc).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    var rangoTim = "A" + rowx + ":I" + rowx;
                    wsTimbre.Range(rangoTim).Style.Fill.SetBackgroundColor(XLColor.Lavender);


                }

                rowColor = !rowColor;
                //nueva linea
                rowx++;
            }

            //SUMATORIA
            SetSumatoria(rowx, ref wsComprobante, 1);
            SetSumatoria(rowx, ref wsNomina, 2);
            SetSumatoria(rowp, ref wsPercepciones, 3);
            SetSumatoria(rowd, ref wsDeducciones, 4);
            SetSumatoria(rowo, ref wsOtrosPagos, 5);
            #endregion



            //DISEÑO
            SetDiseño(ref wsComprobante, 1, rowx);
            SetDiseño(ref wsNomina, 2, rowx);
            SetDiseño(ref wsPercepciones, 3, rowx);
            SetDiseño(ref wsDeducciones, 4, rowx);
            SetDiseño(ref wsOtrosPagos, 5, rowx);
            SetDiseño(ref wsIncapacidad, 6, rowx);
            SetDiseño(ref wsTimbre, 7, rowx);

            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = $"XmlExcel {nombreEmpresa} {nombreCliente} {nombreSucursal} del {fechaInicial} al {fechaFinal} {nombreEjercicio}.xlsx";
            var fileName = pathUsuario + nombreArchivo;
            workbook.SaveAs(fileName);

            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

        }


        private static void SetLineaComprobante(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo)
        {

            DataTable tabla = null;
            if (ds == null) return;

            if (ds.Tables.Contains("Comprobante"))
            {
                if (ds.Tables["Comprobante"] != null)
                {
                    tabla = ds.Tables["Comprobante"];
                }
            }

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 1;

            //dataSet.Tables["Nomina"].Columns.Contains("TotalPercepciones")
            //if (dataSet.Tables.Contains("Nomina"))

            foreach (DataRow renglon in tabla.Rows)
            {

                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idPeriodo;
                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = renglon["version"] ?? ""; //"Version";
                worksheet.Cell(row, col++).Value = renglon["serie"] ?? ""; //"Serie";
                worksheet.Cell(row, col++).Value = renglon["folio"] ?? ""; //"Folio";
                worksheet.Cell(row, col++).Value = renglon["fecha"] ?? ""; //"Fecha";

                string formadePago = "";
                if (renglon.Table.Columns.Contains("formaDePago"))
                {
                    formadePago = renglon["formaDePago"].ToString();
                }
                else if (renglon.Table.Columns.Contains("formaPago"))
                {
                    formadePago = renglon["formaPago"].ToString();
                }

                worksheet.Cell(row, col++).Value = formadePago; //"FormaPago";

                worksheet.Cell(row, col++).Value = renglon["noCertificado"] ?? ""; //"NoCertificado";
                worksheet.Cell(row, col - 1).Style.NumberFormat.NumberFormatId = 1;
                //range.Style.NumberFormat.NumberFormatId = #;

                worksheet.Cell(row, col++).Value = renglon["subTotal"] ?? ""; //"SubTotal";

                string descuento = "";
                if (renglon.Table.Columns.Contains("descuento"))
                {
                    descuento = renglon["descuento"].ToString();
                }

                worksheet.Cell(row, col++).Value = descuento; //"Descuento";


                worksheet.Cell(row, col++).Value = renglon["total"] ?? "--"; //"Total";
                worksheet.Cell(row, col++).Value = renglon["Moneda"] ?? "--"; //"Moneda";

                worksheet.Cell(row, col++).Value = renglon["tipoDeComprobante"] ?? "--"; //"TipoComprobante";

                string metodoPago = "";

                if (renglon.Table.Columns.Contains("metodoDePago"))
                {
                    metodoPago = renglon["metodoDePago"].ToString();
                }
                else if (renglon.Table.Columns.Contains("MetodoPago"))
                {
                    metodoPago = renglon["MetodoPago"].ToString();
                }

                worksheet.Cell(row, col++).Value = metodoPago; //"Metodo Pago";

                worksheet.Cell(row, col++).Value = renglon["LugarExpedicion"] ?? ""; //"Lugar Expedicion";

                col = 1;
            }

            ////Emisor 
            //worksheet.Cell(row, col++).Value = "RFC";
            //worksheet.Cell(row, col++).Value = "Nombre";
            //worksheet.Cell(row, col++).Value = "Regimen Fiscal";
            ////Receptor
            //worksheet.Cell(row, col++).Value = "Rfc";
            //worksheet.Cell(row, col++).Value = "Nombre";
            ////Concepto
            //worksheet.Cell(row, col++).Value = "ClaveProdServ";
            //worksheet.Cell(row, col++).Value = "Cantidad";
            //worksheet.Cell(row, col++).Value = "Clave Unidad";
            //worksheet.Cell(row, col++).Value = "Descripcion";
            //worksheet.Cell(row, col++).Value = "Valor Unitario";
            //worksheet.Cell(row, col++).Value = "Importe";
            //worksheet.Cell(row, col++).Value = "Descuento";
        }
        private static void SetLineaEmisor(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            DataTable tabla = null;
            if (ds == null) return;

            tabla = ds.Tables[1];

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 17;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = renglon["rfc"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Nombre"] ?? "";

                string regimen = "";

                //3.2
                if (ds.Tables.Contains("RegimenFiscal"))
                {
                    var tb = ds.Tables["RegimenFiscal"];
                    regimen = tb.Rows[0]["Regimen"].ToString();
                }
                else//3.3
                {
                    regimen = renglon["RegimenFiscal"].ToString() ?? "";
                }

                //if (renglon.Table.Columns.Contains("RegimenFiscal"))
                //{
                //    regimen = renglon["RegimenFiscal"].ToString();
                //}

                worksheet.Cell(row, col++).Value = regimen;

            }
        }
        private static string SetLineaReceptor(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            string strNombreEmpleado = "";
            DataTable tabla = null;
            if (ds == null) return strNombreEmpleado;

            tabla = ds.Tables[2];

            if (ds.Tables.Contains("RegimenFiscal"))
            {
                tabla = ds.Tables[3];
            }


            if (tabla == null) return strNombreEmpleado;
            if (worksheet == null) return strNombreEmpleado;

            int col = 20;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = renglon["rfc"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Nombre"] ?? "";

                strNombreEmpleado = renglon["Nombre"]?.ToString() ?? "";

                string usocfdi = "";

                if (renglon.Table.Columns.Contains("UsoCFDI"))
                {
                    usocfdi = renglon["UsoCFDI"].ToString();
                }

                worksheet.Cell(row, col++).Value = usocfdi;
            }

            return strNombreEmpleado;
        }
        private static void SetLineaProdServ(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            DataTable tabla = null;
            if (ds == null) return;

            if (ds.Tables.Contains("Concepto"))
            {
                if (ds.Tables["Concepto"] != null)
                {
                    tabla = ds.Tables["Concepto"];
                }
            }

            if (tabla == null) return;
            if (worksheet == null) return;
            int col = 23;

            foreach (DataRow renglon in tabla.Rows)
            {
                string claveProdServ = "";
                if (renglon.Table.Columns.Contains("ClaveProdServ"))
                {
                    claveProdServ = renglon["ClaveProdServ"].ToString();
                }

                worksheet.Cell(row, col++).Value = claveProdServ;
                worksheet.Cell(row, col++).Value = renglon["Cantidad"] ?? "";

                string unidad = "";
                if (renglon.Table.Columns.Contains("ClaveUnidad"))
                {
                    unidad = renglon["ClaveUnidad"].ToString();
                }
                else
                {
                    unidad = renglon["unidad"].ToString();
                }
                worksheet.Cell(row, col++).Value = unidad;
                worksheet.Cell(row, col++).Value = renglon["Descripcion"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["ValorUnitario"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Importe"] ?? "";

                string descuento = "";
                if (renglon.Table.Columns.Contains("Descuento"))
                {
                    descuento = renglon["Descuento"].ToString();
                }
                worksheet.Cell(row, col++).Value = descuento;


            }

        }
        private static void SetLineaNomina(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo)
        {
            DataTable tabla = null;
            if (ds == null) return;

            if (ds.Tables.Contains("Nomina"))
            {
                if (ds.Tables["Nomina"] != null)
                {
                    tabla = ds.Tables["Nomina"];
                }
            }

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 1;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idPeriodo;
                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = renglon["Version"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["TipoNomina"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["FechaPago"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["FechaInicialPago"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["FechaFinalPago"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["NumDiasPagados"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["TotalPercepciones"] ?? "";

                string totalD = "0.00";

                if (renglon.Table.Columns.Contains("TotalDeducciones"))
                {
                    totalD = renglon["TotalDeducciones"].ToString();
                }
                worksheet.Cell(row, col++).Value = totalD;

                string totalOtros = "0.00";
                if (renglon.Table.Columns.Contains("TotalOtrosPagos"))
                {
                    totalOtros = renglon["TotalOtrosPagos"].ToString() ?? "";
                }
                worksheet.Cell(row, col++).Value = totalOtros;
                //  worksheet.Cell(row, col++).Value = renglon["TotalOtrosPagos"] ?? "";
            }
        }
        private static void SetLineaEmisorNom(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            DataTable tabla = null;
            if (ds == null) return;

            tabla = ds.Tables[7];
            if (ds.Tables.Contains("RegimenFiscal"))
            {
                tabla = ds.Tables[8];
            }

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 13;

            foreach (DataRow renglon in tabla.Rows)
            {
                string curp = "";

                if (renglon.Table.Columns.Contains("Curp"))
                {
                    curp = renglon["Curp"].ToString();
                }

                worksheet.Cell(row, col++).Value = curp;

                string RegPatronal = "";
                if (renglon.Table.Columns.Contains("RegistroPatronal"))
                {
                    RegPatronal = renglon["RegistroPatronal"].ToString() ?? "";
                }
                worksheet.Cell(row, col++).Value = RegPatronal;

                // worksheet.Cell(row, col++).Value = renglon["RegistroPatronal"] ?? "";

                string RfPatronOrig = "";
                if (renglon.Table.Columns.Contains("RfcPatronOrigen"))
                {
                    RfPatronOrig = renglon["RfcPatronOrigen"].ToString() ?? "";
                }
                //worksheet.Cell(row, col++).Value = renglon["RfcPatronOrigen"] ?? "";
                worksheet.Cell(row, col++).Value = RfPatronOrig;
            }

            //--->
            if (ds.Tables.Contains("EntidadSNCF"))
            {
                DataTable tablasncf = null;
                tablasncf = ds.Tables["EntidadSNCF"];

                if (tablasncf == null) return;

                foreach (DataRow renglon in tabla.Rows)
                {
                    worksheet.Cell(row, col++).Value = renglon["OrigenRecurso"] ?? "";
                    worksheet.Cell(row, col++).Value = renglon["MontoRecursoPropio"] ?? "";
                }

            }


        }
        private static void SetLineaReceptorNom(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            DataTable tabla = null;
            if (ds == null) return;

            tabla = ds.Tables[8];
            if (ds.Tables.Contains("RegimenFiscal"))
            {
                tabla = ds.Tables[9];
            }

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 18;
            foreach (DataRow renglon in tabla.Rows)
            {
                string curp = "";
                if (renglon.Table.Columns.Contains("Curp"))
                {
                    curp = renglon["Curp"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = curp;

                string nss = "";
                if (renglon.Table.Columns.Contains("NumSeguridadSocial"))
                {
                    nss = renglon["NumSeguridadSocial"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = nss;


                string finicial = "";
                if (renglon.Table.Columns.Contains("FechaInicioRelLaboral"))
                {
                    finicial = renglon["FechaInicioRelLaboral"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = finicial;


                string antig = "";
                if (renglon.Table.Columns.Contains("Antigüedad"))
                {
                    antig = renglon["Antigüedad"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = antig;


                string tipoCon = "";
                if (renglon.Table.Columns.Contains("TipoContrato"))
                {
                    tipoCon = renglon["TipoContrato"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = tipoCon;

                string sindic = "";

                if (renglon.Table.Columns.Contains("Sindicalizado"))
                {
                    sindic = renglon["Sindicalizado"].ToString();
                }


                worksheet.Cell(row, col++).Value = sindic;


                string tipoJornada = "";

                if (renglon.Table.Columns.Contains("TipoJornada"))
                {
                    tipoJornada = renglon["TipoJornada"].ToString() ?? "";
                }
                worksheet.Cell(row, col++).Value = tipoJornada;



                string TipoRegimen = "";

                if (renglon.Table.Columns.Contains("TipoRegimen"))
                {
                    TipoRegimen = renglon["TipoRegimen"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = TipoRegimen;


                string NumEmpleado = "";

                if (renglon.Table.Columns.Contains("NumEmpleado"))
                {
                    NumEmpleado = renglon["NumEmpleado"].ToString() ?? "";
                }
                worksheet.Cell(row, col++).Value = NumEmpleado;

                string Departamento = "";

                if (renglon.Table.Columns.Contains("Departamento"))
                {
                    Departamento = renglon["Departamento"].ToString() ?? "";
                }


                worksheet.Cell(row, col++).Value = Departamento;


                string Puesto = "";

                if (renglon.Table.Columns.Contains("Puesto"))
                {
                    Puesto = renglon["Puesto"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = Puesto;


                string RiesgoPuesto = "";

                if (renglon.Table.Columns.Contains("RiesgoPuesto"))
                {
                    RiesgoPuesto = renglon["RiesgoPuesto"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = RiesgoPuesto;


                string PeriodicidadPago = "";

                if (renglon.Table.Columns.Contains("PeriodicidadPago"))
                {
                    PeriodicidadPago = renglon["PeriodicidadPago"].ToString() ?? "";
                }

                worksheet.Cell(row, col++).Value = PeriodicidadPago;

                string banco = " ";

                if (renglon.Table.Columns.Contains("Banco"))
                {
                    banco = renglon["Banco"].ToString();
                }
                worksheet.Cell(row, col++).Value = banco;

                string cuenta = " ";

                if (renglon.Table.Columns.Contains("CuentaBancaria"))
                {
                    cuenta = renglon["CuentaBancaria"].ToString();
                }
                worksheet.Cell(row, col++).Value = cuenta;


                string SalarioBaseCotApor = " ";

                if (renglon.Table.Columns.Contains("SalarioBaseCotApor"))
                {
                    SalarioBaseCotApor = renglon["SalarioBaseCotApor"].ToString();
                }

                worksheet.Cell(row, col++).Value = SalarioBaseCotApor;


                string SalarioDiarioIntegrado = " ";

                if (renglon.Table.Columns.Contains("SalarioDiarioIntegrado"))
                {
                    SalarioDiarioIntegrado = renglon["SalarioDiarioIntegrado"].ToString();
                }

                worksheet.Cell(row, col++).Value = SalarioDiarioIntegrado;


                string ClaveEntFed = " ";

                if (renglon.Table.Columns.Contains("ClaveEntFed"))
                {
                    ClaveEntFed = renglon["ClaveEntFed"].ToString();
                }

                worksheet.Cell(row, col++).Value = ClaveEntFed;

                //string usocfdi = "";

                //if (renglon.Table.Columns.Contains("UsoCFDI"))
                //{
                //    usocfdi = renglon["UsoCFDI"].ToString();
                //}

                //worksheet.Cell(row, col++).Value = usocfdi;
            }

            //--->
            if (ds.Tables.Contains("SubContratacion"))
            {
                DataTable tablasncf = null;
                tablasncf = ds.Tables["SubContratacion"];

                if (tablasncf == null) return;

                foreach (DataRow renglon in tabla.Rows)
                {
                    string RfcLabora = "";
                    if (renglon.Table.Columns.Contains("RfcLabora"))
                    {
                        RfcLabora = renglon["RfcLabora"].ToString() ?? "";
                    }

                    worksheet.Cell(row, col++).Value = RfcLabora;

                    string PorcentajeTiempo = "";
                    if (renglon.Table.Columns.Contains("PorcentajeTiempo"))
                    {
                        PorcentajeTiempo = renglon["PorcentajeTiempo"].ToString() ?? "";
                    }
                    worksheet.Cell(row, col++).Value = PorcentajeTiempo;
                }

            }


        }
        private static void SetLineaPercepciones(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina)
        {
            DataTable tabla = null;
            if (ds == null) return;

            if (ds.Tables.Contains("Percepciones"))
            {
                if (ds.Tables["Percepciones"] != null)
                {
                    tabla = ds.Tables["Percepciones"];
                }
            }

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 1;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = renglon["TotalSueldos"] ?? "";

                string tsi = "";
                if (renglon.Table.Columns.Contains("TotalSeparacionIndemnizacion"))
                {
                    tsi = renglon["TotalSeparacionIndemnizacion"].ToString();
                }
                worksheet.Cell(row, col++).Value = tsi;

                string tjpr = "";
                if (renglon.Table.Columns.Contains("TotalJubilacionPensionRetiro"))
                {
                    tjpr = renglon["TotalJubilacionPensionRetiro"].ToString();
                }
                worksheet.Cell(row, col++).Value = tjpr;

                worksheet.Cell(row, col++).Value = renglon["TotalGravado"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["TotalExento"] ?? "";

            }
        }
        private static int SetLineaPercepcion(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, bool rowColor)
        {
            DataTable tabla = null;
            if (ds == null) return row;

            if (ds.Tables.Contains("Percepcion"))
            {
                if (ds.Tables["Percepcion"] != null)
                {
                    tabla = ds.Tables["Percepcion"];
                }
            }

            if (tabla == null) return row;
            if (worksheet == null) return row;

            int col = 8;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = renglon["TipoPercepcion"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Clave"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Concepto"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["ImporteGravado"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["ImporteExento"] ?? "";

                if (rowColor)
                {
                    var rangoPintar = "A" + row + ":AB" + row;
                    worksheet.Range(rangoPintar).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                }

                row++;
                col = 8;
            }



            return row;
        }
        private static void SetLineaDeducciones(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina)
        {
            DataTable tabla = null;
            if (ds == null) return;

            if (ds.Tables.Contains("Deducciones"))
            {
                if (ds.Tables["Deducciones"] != null)
                {
                    tabla = ds.Tables["Deducciones"];
                }
            }

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 1;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = renglon["TotalOtrasDeducciones"] ?? "";

                string tid = "";
                if (renglon.Table.Columns.Contains("TotalImpuestosRetenidos"))
                {
                    tid = renglon["TotalImpuestosRetenidos"].ToString();
                }
                worksheet.Cell(row, col++).Value = tid;
            }
        }
        private static int SetLineaDeduccion(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, bool rowColor)
        {
            DataTable tabla = null;
            if (ds == null) return row;

            if (ds.Tables.Contains("Deduccion"))
            {
                if (ds.Tables["Deduccion"] != null)
                {
                    tabla = ds.Tables["Deduccion"];
                }
            }

            if (tabla == null) return row;
            if (worksheet == null) return row;

            int col = 5;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = renglon["TipoDeduccion"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Clave"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Concepto"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Importe"] ?? "";
                if (rowColor)
                {
                    var rangoPintar = "A" + row + ":H" + row;
                    worksheet.Range(rangoPintar).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                }

                row++;
                col = 5;
            }

            return row;
        }
        private static int SetLineaOtrosPagos(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina)
        {

            if (ds == null) return row;

            DataTable tabla = null;
            DataTable tablaSubsidio = null;
            DataTable tablaSaldoFavor = null;

            if (ds.Tables.Contains("OtrosPagos"))
            {
                if (ds.Tables["OtrosPagos"] != null)
                {
                    tabla = ds.Tables["OtroPago"];


                    if (ds.Tables.Contains("SubsidioAlEmpleo"))
                    {
                        tablaSubsidio = ds.Tables["SubsidioAlEmpleo"];
                    }

                    if (ds.Tables.Contains("CompensacionSaldosAFavor"))
                    {
                        tablaSaldoFavor = ds.Tables["CompensacionSaldosAFavor"];
                    }

                }
            }


            if (tabla == null) return row;
            if (worksheet == null) return row;

            int col = 1;

            foreach (DataRow renglon in tabla.Rows)
            {
                string tipo = renglon["TipoOtroPago"].ToString() ?? "";

                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = tipo;
                worksheet.Cell(row, col++).Value = renglon["Clave"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Concepto"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Importe"] ?? "";


                if (tipo == "002")
                {
                    SetLineaSubsidioAlEmpleo(ref worksheet, tablaSubsidio, row, idEmpleado);
                }
                else if (tipo == "004")
                {
                    SetLineaCompensacionSaldosAFavor(ref worksheet, tablaSaldoFavor, row, idEmpleado);
                }

                row++;
                col = 1;
            }

            return row;
        }

        private static void SetLineaSubsidioAlEmpleo(ref IXLWorksheet worksheet, DataTable tabla, int row, int idEmpleado)
        {
            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 7;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col).Value = renglon["SubsidioCausado"] ?? "";
            }
        }
        private static void SetLineaCompensacionSaldosAFavor(ref IXLWorksheet worksheet, DataTable tabla, int row, int idEmpleado)
        {
            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 8;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = renglon["SaldoAFavor"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Año"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["RemanenteSalFav"] ?? "";
            }
        }
        private static int SetLineaIncapacidades(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina)
        {
            DataTable tabla = null;
            if (ds == null) return row;

            if (ds.Tables.Contains("Incapacidades"))
            {
                if (ds.Tables.Contains("Incapacidad"))
                {
                    if (ds.Tables["Incapacidad"] != null)
                    {
                        tabla = ds.Tables["Incapacidad"];
                    }
                }
            }

            if (tabla == null) return row;
            if (worksheet == null) return row;

            int col = 1;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idNomina;

                worksheet.Cell(row, col++).Value = renglon["DiasIncapacidad"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["TipoIncapacidad"] ?? "";

                string importe = "";
                if (renglon.Table.Columns.Contains("ImporteMonetario"))
                {
                    importe = renglon["ImporteMonetario"].ToString();
                }

                worksheet.Cell(row, col++).Value = importe;
                row++;
                col = 1;
            }

            return row;
        }

        private static void SetLineaTimbre(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo, string receptor)
        {
            DataTable tabla = null;
            if (ds == null) return;

            if (ds.Tables.Contains("TimbreFiscalDigital"))
            {
                if (ds.Tables.Contains("TimbreFiscalDigital"))
                {
                    if (ds.Tables["TimbreFiscalDigital"] != null)
                    {
                        tabla = ds.Tables["TimbreFiscalDigital"];
                    }
                }
            }

            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 1;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idPeriodo;
                worksheet.Cell(row, col++).Value = idNomina;

                worksheet.Cell(row, col++).Value = receptor;

                worksheet.Cell(row, col++).Value = renglon["Version"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["UUID"] ?? "";

                worksheet.Cell(row, col++).Value = renglon["FechaTimbrado"] ?? "";

                string rfcProvCerf = "";

                if (renglon.Table.Columns.Contains("RfcProvCertif"))
                {
                    rfcProvCerf = renglon["RfcProvCertif"].ToString();
                }


                worksheet.Cell(row, col++).Value = rfcProvCerf;

                worksheet.Cell(row, col++).Value = renglon["NoCertificadoSAT"] ?? "";
                worksheet.Cell(row, col - 1).Style.NumberFormat.NumberFormatId = 1;


                row++;
            }

        }

        private static void SetSumatoria(int row, ref IXLWorksheet ws, int tipo)
        {
            int rowSum = row - 1;
            int colummEnd = 0;
            string formula = "";

            switch (tipo)
            {
                case 1:
                    colummEnd = 29;

                    formula = $"=SUM(J2:J{rowSum})";
                    ws.Cell(row, 10).FormulaA1 = formula;
                    ws.Cell(row, 10).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(K2:K{rowSum})";
                    ws.Cell(row, 11).FormulaA1 = formula;
                    ws.Cell(row, 11).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(L2:L{rowSum})";
                    ws.Cell(row, 12).FormulaA1 = formula;
                    ws.Cell(row, 12).Style.NumberFormat.Format = "$ ##,###,##0.00";


                    formula = $"=SUM(AA2:AA{rowSum})";
                    ws.Cell(row, 27).FormulaA1 = formula;
                    ws.Cell(row, 27).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(AB2:AB{rowSum})";
                    ws.Cell(row, 28).FormulaA1 = formula;
                    ws.Cell(row, 28).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(AC2:AC{rowSum})";
                    ws.Cell(row, 29).FormulaA1 = formula;
                    ws.Cell(row, 29).Style.NumberFormat.Format = "$ ##,###,##0.00";




                    break;
                case 2:
                    colummEnd = 37;


                    formula = $"=SUM(J2:J{rowSum})";
                    ws.Cell(row, 10).FormulaA1 = formula;
                    ws.Cell(row, 10).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(K2:K{rowSum})";
                    ws.Cell(row, 11).FormulaA1 = formula;
                    ws.Cell(row, 11).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(L2:L{rowSum})";
                    ws.Cell(row, 12).FormulaA1 = formula;
                    ws.Cell(row, 12).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    break;
                case 3://Perceociones
                    colummEnd = 28;

                    formula = $"=SUM(C2:C{rowSum})";
                    ws.Cell(row, 3).FormulaA1 = formula;
                    ws.Cell(row, 3).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(F2:F{rowSum})";
                    ws.Cell(row, 6).FormulaA1 = formula;
                    ws.Cell(row, 6).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(G2:G{rowSum})";
                    ws.Cell(row, 7).FormulaA1 = formula;
                    ws.Cell(row, 7).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(K2:K{rowSum})";
                    ws.Cell(row, 11).FormulaA1 = formula;
                    ws.Cell(row, 11).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(L2:L{rowSum})";
                    ws.Cell(row, 12).FormulaA1 = formula;
                    ws.Cell(row, 12).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    break;
                case 4://Deducciones
                    colummEnd = 8;

                    formula = $"=SUM(C2:C{rowSum})";
                    ws.Cell(row, 3).FormulaA1 = formula;
                    ws.Cell(row, 3).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(D2:D{rowSum})";
                    ws.Cell(row, 4).FormulaA1 = formula;
                    ws.Cell(row, 4).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(H2:H{rowSum})";
                    ws.Cell(row, 8).FormulaA1 = formula;
                    ws.Cell(row, 8).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    break;
                case 5: //Otros Pagos
                    colummEnd = 10;

                    formula = $"=SUM(F2:F{rowSum})";
                    ws.Cell(row, 6).FormulaA1 = formula;
                    ws.Cell(row, 6).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    break;
            }

            ws.Range(row, 1, row, colummEnd).Style.Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
            ws.Range(row, 1, row, colummEnd).Style.Font.SetBold(true);
        }

        private static void SetDiseño(ref IXLWorksheet ws, int tipo, int row)
        {
            //Fix Columns
            //Ajustar columnas
            //Color de Header
            //Alinear columnas
            //Formato

            int iniFix = 2;
            int finFix = 3;
            int finAjuste = 0;
            switch (tipo)
            {
                case 1://Comprobante
                    //Fix Columns
                    //Ajustar columnas
                    finAjuste = 28;
                    //Color de Header
                    ws.Range("A2:U2").Style.Font.SetFontSize(12)
                   .Font.SetBold(true)
                   .Font.SetFontColor(XLColor.White)
                   .Fill.SetBackgroundColor(XLColor.Black);

                    ws.Range("V2:AC2")//Totales
                   .Style.Font.SetFontSize(12)
                   .Font.SetBold(true)
                   .Font.SetFontColor(XLColor.White)
                   .Fill.SetBackgroundColor(XLColor.Gray);
                    //Alinear columnas

                    //ws.Range(filaDelTotal, 1, filaDelTotal, columna).Style.Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));

                    //ws.Cell(filaDelTotal, columna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    //ws.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"C2:C{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"D2:D{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"G2:G{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"L2:L{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"M2:M{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"N2:N{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"O2:O{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"R2:R{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"U2:U{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"V2:V{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"W2:W{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"X2:X{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"U2:U{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    //Formato


                    break;
                case 2://Nomina
                    finAjuste = 36;
                    ws.Range("A2:AK2")//Totales
                .Style.Font.SetFontSize(12)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Black);

                    ws.Range($"C2:C{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"D2:D{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"H2:H{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"D2:D{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"M2:M{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"N2:N{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"T2:T{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"U2:U{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"W2:W{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"X2:X{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"Y2:Y{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"AB2:AB{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"AC2:AC{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"AD2:AD{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range($"AH2:AH{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    break;
                case 3://Percepciones
                    finAjuste = 28;
                    ws.Range("A2:AB2")//Percepciones
                .Style.Font.SetFontSize(12)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Teal);
                    ws.Range("M2:N2")//Percepciones
               .Style.Font.SetFontSize(12)
               .Font.SetBold(true)
               .Font.SetFontColor(XLColor.White)
               .Fill.SetBackgroundColor(XLColor.Gray);

                    ws.Range("O2:R2")//Percepciones - Horas Extras
             .Style.Font.SetFontSize(12)
             .Font.SetBold(true)
             .Font.SetFontColor(XLColor.White)
             .Fill.SetBackgroundColor(XLColor.BlueGray);

                    ws.Range("S2:W2")//Percepciones - Horas Extras
                     .Style.Font.SetFontSize(12)
                     .Font.SetBold(true)
                     .Font.SetFontColor(XLColor.White)
                     .Fill.SetBackgroundColor(XLColor.SeaGreen);
                    break;
                case 4://Deducciones
                    finAjuste = 8;
                    ws.Range("A2:H2")//Percepciones
                .Style.Font.SetFontSize(12)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Brown);


                    break;
                case 5://OtrosPAgos
                    finAjuste = 10;
                    ws.Range("A2:J2")//Percepciones
                .Style.Font.SetFontSize(12)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Teal);


                    break;
                case 6://Incapacidad
                    finAjuste = 5;
                    ws.Range("A2:E2")//Percepciones
                .Style.Font.SetFontSize(12)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Orange);


                    break;
                case 7://TimbreFiscal
                    finAjuste = 9;
                    ws.Range("A2:I2")//Timbre
                 .Style.Font.SetFontSize(12)
                 .Font.SetBold(true)
                 .Font.SetFontColor(XLColor.White)
                 .Fill.SetBackgroundColor(XLColor.Olive);
                    break;

            }


            ws.SheetView.Freeze(iniFix, finFix);
            ws.Columns(1, finAjuste).AdjustToContents(2, 2);

        }

        private static List<DatosVisor> BuscarTimbrados(DateTime? fechaI, DateTime? fechaF, int idPeriodoB = 0, int idEjercicio = 0, int idEmpresa = 0, int idSucursal = 0)
        {
            List<DatosVisor> listaTimbrados = new List<DatosVisor>();

            using (var context = new RHEntities())
            {


                if (idPeriodoB > 0) //buscar por periodo
                {

                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && tim.IdPeriodo == idPeriodoB
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();

                }
                else if (idSucursal > 0 && (fechaI == null || fechaF == null)) // cliente sin fechas
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdSucursal == idSucursal
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();
                }
                else if (idSucursal > 0 && fechaI != null) //cliente con fecha
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.IdSucursal == idSucursal
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && DbFunctions.TruncateTime(tim.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) &&
                                            DbFunctions.TruncateTime(tim.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();


                }
                else if (idEmpresa > 0 && (fechaI == null || fechaF == null)) // por empresa sin fecha
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();


                }
                else//por empresa y fecha
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == false
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && DbFunctions.TruncateTime(tim.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) 
                                            && DbFunctions.TruncateTime(tim.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                      select new DatosVisor()
                                      {
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado
                                      }).ToList();
                }
            }

            return listaTimbrados;
        }

    }

    public class DatosVisor
    {
        public int IdEmpleado { get; set; }
        public int IdPeriodo { get; set; }
        public int IdNomina { get; set; }
        public int IdFiniquito { get; set; }

        public int IdSucursal { get; set; }
        public string XmlTimbrado { get; set; }
    }
}
