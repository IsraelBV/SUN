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
using RH.Entidades.GlobalModel;

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
        public static string GenerarTimbrados(int idUsuario, int idEjercicio, DateTime? fechaI, DateTime? fechaF,
            string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0, int idPeriodoB = 0, bool cancelados = false)
        {
            //List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();
            List<Sucursal> listaSucursales = new List<Sucursal>();
            List<Empresa> listaEmpresas = new List<Empresa>();
            //List<Empleado> listaEmpleados = new List<Empleado>();
            //  List<NOM_CFDI_Timbrado> listaTimbrados = new List<NOM_CFDI_Timbrado>();
            List<Cliente> listaClientes = new List<Cliente>();
            NOM_Ejercicio_Fiscal ejercicioFiscal = new NOM_Ejercicio_Fiscal();
            int[] arrayIdEmpresa;
            string nombreEjercicio = "";

            List<NOM_Nomina> listaNominas = new List<NOM_Nomina>();
            List<NOM_Finiquito> listaFiniquitos = new List<NOM_Finiquito>();

            List<DatosVisor> listaTimbrados = QueryData.BuscarTimbrados(fechaI, fechaF, idPeriodoB, idEjercicio, idEmpresa, idSucursal, cancelados);

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

        //private static string GetArchivosCfdi(int idUsuario, int? idEjercicio, DateTime? fechaI, DateTime? fechaF,
        //    string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0)
        //{
        //    List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();
        //    List<Sucursal> listaSucursales = new List<Sucursal>();
        //    List<Empresa> listaEmpresas = new List<Empresa>();
        //    List<Empleado> listaEmpleados = new List<Empleado>();
        //   List<NOM_CFDI_Timbrado> listaTimbrados = new List<NOM_CFDI_Timbrado>();
        //    NOM_Ejercicio_Fiscal ejercicioFiscal = new NOM_Ejercicio_Fiscal();
        //    List<Cliente> listaClientes = new List<Cliente>();
        //    int[] arrayIdEmpresa;
        //    var pathEjercicio = "";
        //    var pathZip = "";
        //    var nombreEjercicio = "";
        //    var folderArchivoZip = "";
        //    var nombreArchivoZip = "Archivo.zip";

        //    #region GET DATA



        //    using (var context = new RHEntities())
        //    {
        //        listaEmpresas = context.Empresa.ToList();
        //        listaSucursales = context.Sucursal.ToList();
        //        listaClientes = context.Cliente.ToList();

        //        if (idEmpresa == 0) //todas las empresas
        //        {
        //            arrayIdEmpresa = listaEmpresas.Select(x => x.IdEmpresa).ToArray();
        //        }
        //        else //Por empresa
        //        {
        //            arrayIdEmpresa = new int[1];
        //            arrayIdEmpresa[0] = idEmpresa;
        //        }

        //        //ejercicio fiscal
        //        ejercicioFiscal = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);
        //        if (ejercicioFiscal != null)
        //        {
        //            nombreEjercicio = ejercicioFiscal.Anio;
        //            folderArchivoZip = nombreEjercicio + "Zip";
        //        }

        //        listaTimbrados = (from t in context.NOM_CFDI_Timbrado
        //                          where arrayIdEmpresa.Contains(t.IdEmisor)
        //                                && DbFunctions.TruncateTime(t.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(t.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
        //                                && t.Cancelado == false
        //                                && t.ErrorTimbrado == false
        //                                && t.FolioFiscalUUID != null
        //                                && t.IdEjercicio == idEjercicio.Value
        //                          select t).ToList();


        //        //Filtra por sucursal
        //        if (idSucursal > 0)
        //        {
        //            listaTimbrados = (from t in listaTimbrados
        //                              where t.IdSucursal == idSucursal
        //                              select t).ToList();
        //        }

        //        var arrayPeriodos = listaTimbrados.Select(x => x.IdPeriodo).ToArray();
        //        arrayPeriodos = arrayPeriodos.Distinct().ToArray();

        //        listaPeriodos = (from p in context.NOM_PeriodosPago
        //                         where arrayPeriodos.Contains(p.IdPeriodoPago)
        //                         select p).ToList();
        //    }

        //    #endregion

        //    #region PATHS

        //    //Folder donde se guardará el archivo
        //    var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

        //    //Folder Ejercicio
        //    pathEjercicio = pathUsuario + "\\Ejercicio\\" + nombreEjercicio;

        //    Directory.CreateDirectory(pathEjercicio);

        //    pathZip = pathUsuario + "\\" + folderArchivoZip;

        //    Directory.CreateDirectory(pathZip);

        //    //Actualizamos el path para la descarga

        //    pathDescarga = pathDescarga + "\\" + idUsuario + "\\" + folderArchivoZip + "\\" + nombreArchivoZip;

        //    #endregion

        //    #region CREAR FOLDER Y ARCHIVOS

        //    foreach (var idEmpre in arrayIdEmpresa)
        //    {
        //        var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == idEmpre);
        //        //Folder Empresa
        //        var pathFolderEmpresa = pathEjercicio + "\\" + itemEmpresa.RazonSocial;
        //        Directory.CreateDirectory(pathFolderEmpresa);

        //        //Obtener las sucursales de la empresa
        //        var arraySucursalId =
        //            listaTimbrados.Where(x => x.IdEmisor == idEmpre).Select(x => x.IdSucursal).ToArray();
        //        arraySucursalId = arraySucursalId.Distinct().ToArray();

        //        foreach (var idSuc in arraySucursalId)
        //        {
        //            var itemSucursal = listaSucursales.FirstOrDefault(x => x.IdSucursal == idSuc);
        //            if (itemSucursal == null)
        //            {
        //                continue;
        //            }

        //            var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);

        //            //Folder Sucursal
        //            var pathFolderSucursal = pathFolderEmpresa + "\\" + itemCliente.Nombre + " " + itemSucursal.Ciudad;
        //            Directory.CreateDirectory(pathFolderSucursal);

        //            //Obtener los periodos de la sucursal
        //            var arrayPeriodosId =
        //                listaTimbrados.Where(x => x.IdSucursal == idSuc).Select(x => x.IdPeriodo).ToArray();
        //            arrayPeriodosId = arrayPeriodosId.Distinct().ToArray();


        //            foreach (var idPer in arrayPeriodosId)
        //            {
        //                var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == idPer);

        //                //Folder Periodo
        //                var pathFolderPeriodo = pathFolderSucursal + "\\" + itemPeriodo.Descripcion;
        //                Directory.CreateDirectory(pathFolderPeriodo);

        //                //Obtiene la lista de timbrados del periodo
        //                var listaDelPeriodo = listaTimbrados.Where(x => x.IdPeriodo == idPer).ToList();

        //                foreach (var itemReg in listaDelPeriodo)
        //                {
        //                    //XML

        //                    string nombreArchivo = itemReg.RFCReceptor + " - " + itemReg.Serie.Trim() + " - " +
        //                                           itemReg.Folio;
        //                    var pathXml = pathFolderPeriodo + "\\" + nombreArchivo + ".xml";
        //                    using (StreamWriter archivoX = new StreamWriter(pathXml, false))
        //                    {
        //                        archivoX.WriteLine(itemReg.XMLTimbrado);
        //                    }

        //                    //PDF
        //                    var pathPdf = pathFolderPeriodo + "\\" + nombreArchivo + ".pdf";
        //                    File.WriteAllBytes(pathPdf, itemReg.PDF);

        //                }
        //            }
        //        }
        //    }

        //    #endregion

        //    ZipFile.CreateFromDirectory(pathUsuario + "\\Ejercicio\\", pathZip + "\\" + nombreArchivoZip);

        //    return pathDescarga;

        //}

        private static string GetArchivosCfdiMes(int idUsuario, int idEjercicio, DateTime? fechaI, DateTime? fechaF,
            string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0, int idPeriodoB = 0, bool incluirPdf = false)
        {
            List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();
            List<Sucursal> listaSucursales = new List<Sucursal>();
            Empresa itemEmpresa;
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<NOM_CFDI_Timbrado> listaTimbrados = new List<NOM_CFDI_Timbrado>();
            NOM_Ejercicio_Fiscal ejercicioFiscal = new NOM_Ejercicio_Fiscal();
            List<Cliente> listaClientes = new List<Cliente>();
            // int[] arrayIdEmpresa;
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

            var listaFiltrados = QueryData.BuscarTimbrados(fechaI, fechaF, idPeriodoB, idEjercicio, idEmpresa, idSucursal, false);

            var arrayTimbres = listaFiltrados.Select(x => x.IdTimbrado).ToArray();
            var arrayIdSucursales = listaFiltrados.Select(x => x.IdSucursal).Distinct().ToArray();

            using (var context = new RHEntities())
            {
                listaSucursales = (from s in context.Sucursal
                                   where arrayIdSucursales.Contains(s.IdSucursal)
                                   select s).ToList();


                listaClientes = context.Cliente.ToList();

                itemEmpresa = context.Empresa.FirstOrDefault(x => x.IdEmpresa == idEmpresa);

                //ejercicio fiscal
                ejercicioFiscal = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);
                if (ejercicioFiscal != null)
                {
                    nombreEjercicio = ejercicioFiscal.Anio;
                    folderArchivoZip = nombreEjercicio + "Zip";
                }

                listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                  where arrayTimbres.Contains(t.IdTimbrado)
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
            if (itemEmpresa != null)
            {
                string nombreEmp = "";
                if (itemEmpresa.RazonSocial.Length > 20)
                {
                    nombreEmp = itemEmpresa.RazonSocial.Substring(0, 20);
                }
                else
                {
                    nombreEmp = itemEmpresa.RazonSocial;
                }

                nombreArchivoZip = $"{strXMLPdf}{nombreEmp}-{nombreEjercicio}.zip";
            }


            //Folder donde se guardará el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            //Folder Ejercicio
            pathEjercicio = $"{pathUsuario}Ejercicio\\{nombreEjercicio}_{itemEmpresa.RazonSocial}";

            Directory.CreateDirectory(pathEjercicio);

            pathZip = pathUsuario + "\\" + folderArchivoZip;

            Directory.CreateDirectory(pathZip);

            //Actualizamos el path para la descarga

            pathDescarga = pathDescarga + "\\" + idUsuario + "\\" + folderArchivoZip + "\\" + nombreArchivoZip;

            #endregion

            #region CREAR FOLDER Y ARCHIVOS

            #region OBTIENE LOS MESES DE LOS TIMBRADOS
            var nuevalistaOrdenadaPorFecha = listaTimbrados.OrderBy(x => x.FechaCertificacion).ToList();

            var itemFirst = nuevalistaOrdenadaPorFecha.First();
            var itemLast = nuevalistaOrdenadaPorFecha.Last();
            var listaMeses = new List<Tuple<int, int>>();


            if (itemFirst.FechaCertificacion.Value.Year != itemLast.FechaCertificacion.Value.Year)
            {
                //validamos que las fechas del ultimo timbrado corresponde a otro año
                //esto puede suceder que el periodo correspanda a un ejercicio fiscal pero el timbrado lo realizaron al siguiente año
                var primerAño = itemFirst.FechaCertificacion.Value.Year;
                var segundoAño = itemLast.FechaCertificacion.Value.Year;

                var mesIni = nuevalistaOrdenadaPorFecha.First(x => x.FechaCertificacion.Value.Year == primerAño);
                var mesFin = nuevalistaOrdenadaPorFecha.Last(x => x.FechaCertificacion.Value.Year == primerAño);

                var mesIniAdi = nuevalistaOrdenadaPorFecha.First(x => x.FechaCertificacion.Value.Year == segundoAño);
                var mesFinAdi = nuevalistaOrdenadaPorFecha.Last(x => x.FechaCertificacion.Value.Year == segundoAño);

                listaMeses = Utils.GetMesesEntreDosFechas(mesIni.FechaCertificacion.Value, mesFin.FechaCertificacion.Value);

                var listaMesesProx = Utils.GetMesesEntreDosFechas(mesIniAdi.FechaCertificacion.Value, mesFinAdi.FechaCertificacion.Value);

                listaMeses.AddRange(listaMesesProx);
            }
            else
            {
                listaMeses = Utils.GetMesesEntreDosFechas(itemFirst.FechaCertificacion.Value, itemLast.FechaCertificacion.Value);
            }

            #endregion

            #region POR EMPRESA

            var pathFolderEmpresa = pathEjercicio;

            Directory.CreateDirectory(pathFolderEmpresa); //FOLDER EMPRESA

            #region FOLDER por MES

            foreach (var itemMes in listaMeses)
            {
                #region CREAR EL FOLDER DEL MES




                var año = itemMes.Item2 - 2000;
                var nombreMes = $"{((Mes)itemMes.Item1).ToString()}{año}";


                var pathFolderMes = pathFolderEmpresa + "\\" + nombreMes;
                Directory.CreateDirectory(pathFolderMes); //Crear Folder Del MES
                #endregion

                #region POR SUCURSAL
                var arraySucursalId = listaTimbrados.Where(x => x.IdEmisor == idEmpresa && x.FechaCertificacion.Value.Month == itemMes.Item1 && x.FechaCertificacion.Value.Year == itemMes.Item2).Select(x => x.IdSucursal).ToArray();

                arraySucursalId = arraySucursalId.Distinct().ToArray();

                foreach (var idSuc in arraySucursalId)
                {
                    #region EL FOLDER CLIENTE Y SUCURSAL
                    var itemSucursal = listaSucursales.FirstOrDefault(x => x.IdSucursal == idSuc);
                    var nombreSucursal = "SNotFound";
                    var nombreCliente = "CNotFound";

                    if (itemSucursal != null) //poner id de la sucursal no encontrada
                    {
                        var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);

                        if (itemCliente != null) nombreCliente = itemCliente.Nombre;
                        nombreSucursal = itemSucursal.Ciudad;
                    }

                    var pathFolderSucursal = $"{pathFolderMes}\\{idSuc} {nombreCliente}-{nombreSucursal}";

                    Directory.CreateDirectory(pathFolderSucursal); //FOLDER SUCURSAL

                    #endregion

                    //Obtener los periodos de la sucursal
                    var arrayPeriodosId =
                        listaTimbrados.Where(
                                x => x.IdSucursal == idSuc && x.FechaCertificacion.Value.Month == itemMes.Item1 && x.FechaCertificacion.Value.Year == itemMes.Item2)
                            .Select(x => x.IdPeriodo)
                            .ToArray();
                    arrayPeriodosId = arrayPeriodosId.Distinct().ToArray();

                    #region PERIODOS

                    foreach (var idPer in arrayPeriodosId)
                    {
                        #region CREAR EL FOLDER DEL PERIODO
                        var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == idPer);
                        string periodoDescripcion = "PNotFound";

                        if (itemPeriodo != null)
                        {
                            periodoDescripcion = itemPeriodo.Descripcion;
                        }

                        var pathFolderPeriodo = $"{pathFolderSucursal}\\{idPer} {periodoDescripcion}";

                        Directory.CreateDirectory(pathFolderPeriodo); //Folder Periodo
                        #endregion

                        //Obtiene la lista de timbrados del periodo
                        var listaDelPeriodo =
                            listaTimbrados.Where(
                                x => x.IdPeriodo == idPer && x.FechaCertificacion.Value.Month == itemMes.Item1 && x.FechaCertificacion.Value.Year == itemMes.Item2).ToList();

                        #region ARCHIVOS XML y PDF

                        foreach (var itemReg in listaDelPeriodo)
                        {
                            //XML
                            string nombreArchivo = itemReg.RFCReceptor + " - " + itemReg.Serie.Trim() + " - " + itemReg.Folio;
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



            #endregion

            #endregion

            ZipFile.CreateFromDirectory(pathUsuario + "\\Ejercicio\\", pathZip + "\\" + nombreArchivoZip);

            return pathDescarga;

        }

        public static Task<string> DownloadRecibosXml(int idUsuario, int idEjercicio, DateTime? fechaI,
            DateTime? fechaF, string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0, int idPeriodoB = 0,
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
                            idSucursal, idPeriodoB, incluirPdf));
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
            List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();

                #region GETDATA

            listaTimbrados = QueryData.BuscarTimbrados(fechaI, fechaF, idPeriodoB, idEjercicio, idEmpresa, idSucursal, false);
            using (var context = new RHEntities())
            {
                //Hacer el filtro por empresa y sucursal

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

                //buscamos los periodos de los registros timbrados
                var arrayPeriodos = listaTimbrados.Select(x => x.IdPeriodo).Distinct().ToArray();

                listaPeriodos = (from p in context.NOM_PeriodosPago
                    where arrayPeriodos.Contains(p.IdPeriodoPago)
                    select p).ToList();

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
            var wsResumen = workbook.Worksheets.Add("Resumen");

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
            wsComprobante.Cell(rowc, col++).Value = "IdEmp";
            wsComprobante.Cell(rowc, col++).Value = "IdPer";
            wsComprobante.Cell(rowc, col++).Value = "IdNom";
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
            wsNomina.Cell(rowc, col++).Value = "IdEmp";
            wsNomina.Cell(rowc, col++).Value = "IdPer";
            wsNomina.Cell(rowc, col++).Value = "IdNom";
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
            wsPercepciones.Cell(rowc, col++).Value = "IdEmp";
            wsPercepciones.Cell(rowc, col++).Value = "IdPer";
            wsPercepciones.Cell(rowc, col++).Value = "IdNom";
            wsPercepciones.Cell(rowc, col++).Value = "Nombre";
            wsPercepciones.Cell(rowc, col++).Value = "Fecha Timbrado PAC";

            wsPercepciones.Cell(rowc, col++).Value = "RFC Empleado";
            wsPercepciones.Cell(rowc, col++).Value = "RFC Empresa";
            wsPercepciones.Cell(rowc, col++).Value = "UUID";


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
            wsDeducciones.Cell(rowc, col++).Value = "IdEmp";
            wsDeducciones.Cell(rowc, col++).Value = "IdPer";
            wsDeducciones.Cell(rowc, col++).Value = "IdNom";
            wsDeducciones.Cell(rowc, col++).Value = "Nombre";
            wsDeducciones.Cell(rowc, col++).Value = "Fecha Timbrado";

            wsDeducciones.Cell(rowc, col++).Value = "RFC Empleado";
            wsDeducciones.Cell(rowc, col++).Value = "RFC Empresa";
            wsDeducciones.Cell(rowc, col++).Value = "UUID";



            wsDeducciones.Cell(rowc, col++).Value = "Total Otras Deducciones";
            wsDeducciones.Cell(rowc, col++).Value = "Total Impuestos Retenidos";

            wsDeducciones.Cell(rowc, col++).Value = "Tipo Deduccion";
            wsDeducciones.Cell(rowc, col++).Value = "Clave";
            wsDeducciones.Cell(rowc, col++).Value = "Concepto";
            wsDeducciones.Cell(rowc, col++).Value = "Importe";

            //Otros Pagos
            col = 1;
            wsOtrosPagos.Cell(rowc, col++).Value = "IdEmp";
            wsOtrosPagos.Cell(rowc, col++).Value = "IdPer";
            wsOtrosPagos.Cell(rowc, col++).Value = "IdNom";
            wsOtrosPagos.Cell(rowc, col++).Value = "Nombre";
            wsOtrosPagos.Cell(rowc, col++).Value = "Fecha Timbrado";

            wsOtrosPagos.Cell(rowc, col++).Value = "RFC Empleado";
            wsOtrosPagos.Cell(rowc, col++).Value = "RFC Empresa";
            wsOtrosPagos.Cell(rowc, col++).Value = "UUID";



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
            wsTimbre.Cell(rowc, col++).Value = "IdEmp";
            wsTimbre.Cell(rowc, col++).Value = "IdPer";
            wsTimbre.Cell(rowc, col++).Value = "IdNom";

            wsTimbre.Cell(rowc, col++).Value = "Nombre Empleado";
            wsTimbre.Cell(rowc, col++).Value = "Version";
            wsTimbre.Cell(rowc, col++).Value = "UUID";
            wsTimbre.Cell(rowc, col++).Value = "Fecha Timbrado PAC";
            wsTimbre.Cell(rowc, col++).Value = "Rfc Provedor";
            wsTimbre.Cell(rowc, col++).Value = "NoCertificadoSAT";

            #endregion

            #region RESUMEN 

            rowc = 2;
            col = 1;
            wsResumen.Cell(rowc, col++).Value = "IdEmp";
            wsResumen.Cell(rowc, col++).Value = "IdPer";
            wsResumen.Cell(rowc, col++).Value = "IdNom";
            wsResumen.Cell(rowc, col++).Value = "Nombre Empleado";
            wsResumen.Cell(rowc, col++).Value = "RFC Empleado";
            wsResumen.Cell(rowc, col++).Value = "RFC Empresa";
            wsResumen.Cell(rowc, col++).Value = "UUID";

            //wsResumen.Cell(rowc, col++).Value = "Fecha Ini Periodo";
            wsResumen.Cell(rowc, col++).Value = "Fecha Fin Periodo";
            wsResumen.Cell(rowc, col++).Value = "Fecha Timbrado PAC";

            wsResumen.Cell(rowc, col++).Value = "Percepciones";
            wsResumen.Cell(rowc, col++).Value = "Gravado";
            wsResumen.Cell(rowc, col++).Value = "Excento";
            wsResumen.Cell(rowc, col++).Value = "Otros Pagos";
            wsResumen.Cell(rowc, col++).Value = "ISR";

            wsResumen.Cell(rowc, col++).Value = "SUB-TOTAL";
            wsResumen.Cell(rowc, col++).Value = "DESCUENTO";
            wsResumen.Cell(rowc, col++).Value = "TOTAL RECIBO";


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
            Tuple<string, string> tpTimbre = new Tuple<string, string>("", "");
            Tuple<string, string> tpReceptor = new Tuple<string, string>("", "");
            string rfcEmisor = "";





            foreach (var itemX in listaTimbrados)
            {
                if (itemX?.XmlTimbrado == null) continue;
                DataSet ds = new DataSet();
                strNombreEmpleado = "";
                rfcEmisor = "";
                tpTimbre = new Tuple<string, string>("", "");
                tpReceptor = new Tuple<string, string>("", "");

                //Carga el contenido del XML en el DATASET
                StringReader streamReader = new StringReader(itemX.XmlTimbrado);
                ds.ReadXml(streamReader);

                //leer id empleado
                idEmpleado = itemX.IdEmpleado;
                idNomina = itemX.IdNomina > 0 ? itemX.IdNomina : itemX.IdFiniquito;

                idPeriodo = itemX.IdPeriodo;

                #region Comprobante

                var tpComprobante = SetLineaComprobante(ref wsComprobante, ds, rowx, idEmpleado, idNomina, idPeriodo);
                //emisor 
                rfcEmisor = SetLineaEmisor(ref wsComprobante, ds, rowx, idEmpleado);
                //receptor             
                tpReceptor = SetLineaReceptor(ref wsComprobante, ds, rowx, idEmpleado);
                #endregion

                #region Concepto        
                SetLineaProdServ(ref wsComprobante, ds, rowx, idEmpleado);
                #endregion

                #region NOMINA
                var tpNom = SetLineaNomina(ref wsNomina, ds, rowx, idEmpleado, idNomina, idPeriodo);
                //emisor 
                SetLineaEmisorNom(ref wsNomina, ds, rowx, idEmpleado);
                //receptor
                SetLineaReceptorNom(ref wsNomina, ds, rowx, idEmpleado);
                //color - 

                #endregion


                tpTimbre = SetLineaTimbre(ref wsTimbre, ds, rowx, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1);


                #region PERCEPCION 
                var tpPercepciones = SetLineaPercepciones(ref wsPercepciones, ds, rowp, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");

                rowp = SetLineaPercepcion(ref wsPercepciones, ds, rowp, idEmpleado, rowColor, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
                #endregion

                #region DEDUCCIONES
                SetLineaDeducciones(ref wsDeducciones, ds, rowd, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
                var tpD = SetLineaDeduccion(ref wsDeducciones, ds, rowd, idEmpleado, rowColor, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
                rowd = tpD.Item1;
                #endregion

                #region OTROS PAGOS
                rowo = SetLineaOtrosPagos(ref wsOtrosPagos, ds, rowo, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
                #endregion

                #region INCAPACIDADES
                rowinca = SetLineaIncapacidades(ref wsIncapacidad, ds, rowinca, idEmpleado, idNomina);
                #endregion


                //Tuple<object, object, object> tpComprobante, Tuple< object, object, object> tpNomina, Tuple<string, string> tpReceptor, Tuple< object, object> tpPercepciones, Tuple<int, decimal> tpIsr, Tuple< string, string> tpTimbre)

                var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemX.IdPeriodo);

                SetResumen(ref wsResumen, rowx, idEmpleado, idNomina, idPeriodo, rfcEmisor, tpComprobante, tpNom, tpReceptor, tpPercepciones, tpD, tpTimbre, itemPeriodo);

                //antes aqui timbre

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

                    var rangoOtrosPagos = "A" + rowx + ":Q" + rowx;
                    wsOtrosPagos.Range(rangoOtrosPagos).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    var rangoInc = "A" + rowx + ":E" + rowx;
                    wsIncapacidad.Range(rangoInc).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    var rangoTim = "A" + rowx + ":I" + rowx;
                    wsTimbre.Range(rangoTim).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    var rangoResumen = "A" + rowx + ":Q" + rowx;
                    wsResumen.Range(rangoResumen).Style.Fill.SetBackgroundColor(XLColor.Lavender);


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
            SetSumatoria(rowx, ref wsResumen, 6);
            #endregion




            //DISEÑO
            SetDiseño(ref wsComprobante, 1, rowx);
            SetDiseño(ref wsNomina, 2, rowx);
            SetDiseño(ref wsPercepciones, 3, rowx);
            SetDiseño(ref wsDeducciones, 4, rowx);
            SetDiseño(ref wsOtrosPagos, 5, rowx);
            SetDiseño(ref wsIncapacidad, 6, rowx);
            SetDiseño(ref wsTimbre, 7, rowx);
            SetDiseño(ref wsResumen, 8, rowx);

            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = $"XmlExcel {nombreEmpresa} {nombreCliente} {nombreSucursal} del {fechaInicial} al {fechaFinal} {nombreEjercicio}.xlsx";
            var fileName = pathUsuario + nombreArchivo;
            workbook.SaveAs(fileName);

            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

        }


    //    public static string VisorXmlToExcelJuanita(int idUsuario, int idEjercicio, DateTime? fechaI, DateTime? fechaF,
    //string pathFolder, string pathDescarga, int idEmpresa = 0, int idSucursal = 0, int idPeriodoB = 0, bool cancelados = false)
    //    {
    //        List<DatosVisor> listaTimbrados = new List<DatosVisor>();
            

      

    //        var workbook = new XLWorkbook();

    //        //Worksheets
    //        var wsComprobante = workbook.Worksheets.Add("Comprobante");
    //        var wsNomina = workbook.Worksheets.Add("Nomina");
    //        var wsPercepciones = workbook.Worksheets.Add("Percepciones");
    //        var wsDeducciones = workbook.Worksheets.Add("Deducciones");
    //        var wsOtrosPagos = workbook.Worksheets.Add("OtrosPagos");
    //        var wsIncapacidad = workbook.Worksheets.Add("Incapacidad");
    //        var wsTimbre = workbook.Worksheets.Add("TimbreFiscalDigital");
    //        var wsResumen = workbook.Worksheets.Add("Resumen");

    //        wsComprobante.TabColor = XLColor.Black;
    //        wsNomina.TabColor = XLColor.Black;
    //        wsPercepciones.TabColor = XLColor.Teal;
    //        wsDeducciones.TabColor = XLColor.Brown;
    //        wsOtrosPagos.TabColor = XLColor.Teal;
    //        wsIncapacidad.TabColor = XLColor.Orange;
    //        wsTimbre.TabColor = XLColor.Olive;
    //        // wsComprobante.Style.Fill.BackgroundColor = XLColor.Red;

    //        #region HEADERS

    //        #region COMPROBANTE 3.3

    //        int rowc = 2;
    //        int col = 1;
    //        wsComprobante.Cell(rowc, col++).Value = "IdEmp";
    //        wsComprobante.Cell(rowc, col++).Value = "IdPer";
    //        wsComprobante.Cell(rowc, col++).Value = "IdNom";
    //        wsComprobante.Cell(rowc, col++).Value = "Version";
    //        wsComprobante.Cell(rowc, col++).Value = "Serie";
    //        wsComprobante.Cell(rowc, col++).Value = "Folio";
    //        wsComprobante.Cell(rowc, col++).Value = "Fecha";
    //        wsComprobante.Cell(rowc, col++).Value = "FormaPago";
    //        wsComprobante.Cell(rowc, col++).Value = "NoCertificado";
    //        wsComprobante.Cell(rowc, col++).Value = "SubTotal";
    //        wsComprobante.Cell(rowc, col++).Value = "Descuento";
    //        wsComprobante.Cell(rowc, col++).Value = "Total";
    //        wsComprobante.Cell(rowc, col++).Value = "Moneda";
    //        wsComprobante.Cell(rowc, col++).Value = "TipoComprobante";
    //        wsComprobante.Cell(rowc, col++).Value = "Metodo Pago";
    //        wsComprobante.Cell(rowc, col++).Value = "Lugar Expedicion";
    //        //Emisor 
    //        wsComprobante.Cell(rowc, col++).Value = "RFC";
    //        wsComprobante.Cell(rowc, col++).Value = "Nombre";
    //        wsComprobante.Cell(rowc, col++).Value = "Regimen Fiscal";
    //        //Receptor
    //        wsComprobante.Cell(rowc, col++).Value = "Rfc";
    //        wsComprobante.Cell(rowc, col++).Value = "Nombre";
    //        wsComprobante.Cell(rowc, col++).Value = "Uso CFDI";
    //        //Concepto
    //        wsComprobante.Cell(rowc, col++).Value = "ClaveProdServ";
    //        wsComprobante.Cell(rowc, col++).Value = "Cantidad";
    //        wsComprobante.Cell(rowc, col++).Value = "Clave Unidad";
    //        wsComprobante.Cell(rowc, col++).Value = "Descripcion";
    //        wsComprobante.Cell(rowc, col++).Value = "Valor Unitario";
    //        wsComprobante.Cell(rowc, col++).Value = "Importe";
    //        wsComprobante.Cell(rowc, col++).Value = "Descuento";

    //        #endregion

    //        #region NOMINA 1.2

    //        rowc = 2;
    //        col = 1;
    //        wsNomina.Cell(rowc, col++).Value = "IdEmp";
    //        wsNomina.Cell(rowc, col++).Value = "IdPer";
    //        wsNomina.Cell(rowc, col++).Value = "IdNom";
    //        wsNomina.Cell(rowc, col++).Value = "Version";
    //        wsNomina.Cell(rowc, col++).Value = "Tipo Nomina";
    //        wsNomina.Cell(rowc, col++).Value = "Fecha Pago";
    //        wsNomina.Cell(rowc, col++).Value = "Fecha Inicio Pago";
    //        wsNomina.Cell(rowc, col++).Value = "Fecha Final Pago";
    //        wsNomina.Cell(rowc, col++).Value = "Num Dias Pagados";
    //        wsNomina.Cell(rowc, col++).Value = "Total Percecepciones";
    //        wsNomina.Cell(rowc, col++).Value = "Total Deducciones";
    //        wsNomina.Cell(rowc, col++).Value = "Total OtrosPagos";
    //        //Emisor
    //        wsNomina.Cell(rowc, col++).Value = "Curp";
    //        wsNomina.Cell(rowc, col++).Value = "Registro Patronal";
    //        wsNomina.Cell(rowc, col++).Value = "Rfc Patron Origen";
    //        //Sub Entidad SNCF
    //        wsNomina.Cell(rowc, col++).Value = "OrigenRecurso";
    //        wsNomina.Cell(rowc, col++).Value = "MontoRecursoPropio";


    //        //Receptor
    //        wsNomina.Cell(rowc, col++).Value = "Curp";
    //        wsNomina.Cell(rowc, col++).Value = "NSS";
    //        wsNomina.Cell(rowc, col++).Value = "Fecha Inicio Laboral";
    //        wsNomina.Cell(rowc, col++).Value = "Antiguedad";
    //        wsNomina.Cell(rowc, col++).Value = "Tipo Contrato";
    //        wsNomina.Cell(rowc, col++).Value = "Sindicalizado";
    //        wsNomina.Cell(rowc, col++).Value = "Tipo Jornada";
    //        wsNomina.Cell(rowc, col++).Value = "Tipo Regimen";
    //        wsNomina.Cell(rowc, col++).Value = "Num Empleado";
    //        wsNomina.Cell(rowc, col++).Value = "Departamento";
    //        wsNomina.Cell(rowc, col++).Value = "Puesto";
    //        wsNomina.Cell(rowc, col++).Value = "RiesgoPuesto";

    //        wsNomina.Cell(rowc, col++).Value = "Periodicidad Pago";
    //        wsNomina.Cell(rowc, col++).Value = "Banco";
    //        wsNomina.Cell(rowc, col++).Value = "CuentaBancaria";
    //        wsNomina.Cell(rowc, col++).Value = "SalarioBaseCotApor";

    //        wsNomina.Cell(rowc, col++).Value = "Salario Diario Int.";
    //        wsNomina.Cell(rowc, col++).Value = "Clave Ent Fed";

    //        //Sub - SubContratacion
    //        wsNomina.Cell(rowc, col++).Value = "RfcLabora";
    //        wsNomina.Cell(rowc, col++).Value = "Porcentaje Tiempo";

    //        #endregion

    //        #region CONCEPTOS - PERCEPCIONES - DEDUCCIONES - OTROS PAGOS - INCAPACIDADES
    //        rowc = 2;
    //        col = 1;
    //        //Percepciones
    //        wsPercepciones.Cell(rowc, col++).Value = "IdEmp";
    //        wsPercepciones.Cell(rowc, col++).Value = "IdPer";
    //        wsPercepciones.Cell(rowc, col++).Value = "IdNom";
    //        wsPercepciones.Cell(rowc, col++).Value = "Nombre";
    //        wsPercepciones.Cell(rowc, col++).Value = "Fecha Timbrado PAC";

    //        wsPercepciones.Cell(rowc, col++).Value = "RFC Empleado";
    //        wsPercepciones.Cell(rowc, col++).Value = "RFC Empresa";
    //        wsPercepciones.Cell(rowc, col++).Value = "UUID";


    //        wsPercepciones.Cell(rowc, col++).Value = "Total Sueldos";
    //        wsPercepciones.Cell(rowc, col++).Value = "Total Separacion Indemnizacion";
    //        wsPercepciones.Cell(rowc, col++).Value = "Total Jubilacion Pension Retiro";
    //        wsPercepciones.Cell(rowc, col++).Value = "Total Gravado";
    //        wsPercepciones.Cell(rowc, col++).Value = "Total Exento";

    //        //sub - percepcion
    //        wsPercepciones.Cell(rowc, col++).Value = "Tipo Percepcion";
    //        wsPercepciones.Cell(rowc, col++).Value = "Clave";
    //        wsPercepciones.Cell(rowc, col++).Value = "Concepto";
    //        wsPercepciones.Cell(rowc, col++).Value = "Importe Gravado";
    //        wsPercepciones.Cell(rowc, col++).Value = "Importe Exento";

    //        //sub - Acciones o Titulos
    //        wsPercepciones.Cell(rowc, col++).Value = "ValorMercado";
    //        wsPercepciones.Cell(rowc, col++).Value = "PrecioAlOtorgarse";

    //        //sub Horas extras
    //        wsPercepciones.Cell(rowc, col++).Value = "Dias";
    //        wsPercepciones.Cell(rowc, col++).Value = "Tipo Horas";
    //        wsPercepciones.Cell(rowc, col++).Value = "Horas extras";
    //        wsPercepciones.Cell(rowc, col++).Value = "Importe Pagado";

    //        //sub Jubilacion Pension Retiro
    //        wsPercepciones.Cell(rowc, col++).Value = "TotalUnaExhibicion";
    //        wsPercepciones.Cell(rowc, col++).Value = "Total Parcialidad";
    //        wsPercepciones.Cell(rowc, col++).Value = "MontoDiario";
    //        wsPercepciones.Cell(rowc, col++).Value = "Ingreso Acumulable";
    //        wsPercepciones.Cell(rowc, col++).Value = "Ingreso No Acumulable";

    //        //Separacion Indemnizacion
    //        wsPercepciones.Cell(rowc, col++).Value = "Total Pagado";
    //        wsPercepciones.Cell(rowc, col++).Value = "NumAñoServicio";
    //        wsPercepciones.Cell(rowc, col++).Value = "Ultimo sueldo Mensual Ordinario";
    //        wsPercepciones.Cell(rowc, col++).Value = "Ingreso Acumulable";
    //        wsPercepciones.Cell(rowc, col++).Value = "Ingreso No Acumulable";



    //        //Deduccionescol = 1;
    //        col = 1;
    //        wsDeducciones.Cell(rowc, col++).Value = "IdEmp";
    //        wsDeducciones.Cell(rowc, col++).Value = "IdPer";
    //        wsDeducciones.Cell(rowc, col++).Value = "IdNom";
    //        wsDeducciones.Cell(rowc, col++).Value = "Nombre";
    //        wsDeducciones.Cell(rowc, col++).Value = "Fecha Timbrado";

    //        wsDeducciones.Cell(rowc, col++).Value = "RFC Empleado";
    //        wsDeducciones.Cell(rowc, col++).Value = "RFC Empresa";
    //        wsDeducciones.Cell(rowc, col++).Value = "UUID";



    //        wsDeducciones.Cell(rowc, col++).Value = "Total Otras Deducciones";
    //        wsDeducciones.Cell(rowc, col++).Value = "Total Impuestos Retenidos";

    //        wsDeducciones.Cell(rowc, col++).Value = "Tipo Deduccion";
    //        wsDeducciones.Cell(rowc, col++).Value = "Clave";
    //        wsDeducciones.Cell(rowc, col++).Value = "Concepto";
    //        wsDeducciones.Cell(rowc, col++).Value = "Importe";

    //        //Otros Pagos
    //        col = 1;
    //        wsOtrosPagos.Cell(rowc, col++).Value = "IdEmp";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "IdPer";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "IdNom";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Nombre";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Fecha Timbrado";

    //        wsOtrosPagos.Cell(rowc, col++).Value = "RFC Empleado";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "RFC Empresa";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "UUID";



    //        wsOtrosPagos.Cell(rowc, col++).Value = "Tipo Otro Pago";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Clave";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Concepto";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Importe";

    //        //Subsidio
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Subsidio Causado";

    //        //Saldo a Favor
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Saldo a Favor";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Año";
    //        wsOtrosPagos.Cell(rowc, col++).Value = "Remanente Saldo a Favor";
    //        #endregion

    //        #region INCAPACIDADES
    //        //Incapacidades
    //        rowc = 2;
    //        col = 1;
    //        wsIncapacidad.Cell(rowc, col++).Value = "IdEmpleado";
    //        wsIncapacidad.Cell(rowc, col++).Value = "IdNomina";
    //        wsIncapacidad.Cell(rowc, col++).Value = "Dias Incapacidad";
    //        wsIncapacidad.Cell(rowc, col++).Value = "Tipo Incapacidad";
    //        wsIncapacidad.Cell(rowc, col++).Value = "Importe Monetario";
    //        #endregion

    //        #region TIMBRE FISCAL DIGITAL

    //        rowc = 2;
    //        col = 1;
    //        wsTimbre.Cell(rowc, col++).Value = "IdEmp";
    //        wsTimbre.Cell(rowc, col++).Value = "IdPer";
    //        wsTimbre.Cell(rowc, col++).Value = "IdNom";

    //        wsTimbre.Cell(rowc, col++).Value = "Nombre Empleado";
    //        wsTimbre.Cell(rowc, col++).Value = "Version";
    //        wsTimbre.Cell(rowc, col++).Value = "UUID";
    //        wsTimbre.Cell(rowc, col++).Value = "Fecha Timbrado PAC";
    //        wsTimbre.Cell(rowc, col++).Value = "Rfc Provedor";
    //        wsTimbre.Cell(rowc, col++).Value = "NoCertificadoSAT";

    //        #endregion

    //        #region RESUMEN 

    //        rowc = 2;
    //        col = 1;
    //        wsResumen.Cell(rowc, col++).Value = "IdEmp";
    //        wsResumen.Cell(rowc, col++).Value = "IdPer";
    //        wsResumen.Cell(rowc, col++).Value = "IdNom";
    //        wsResumen.Cell(rowc, col++).Value = "Nombre Empleado";
    //        wsResumen.Cell(rowc, col++).Value = "RFC Empleado";
    //        wsResumen.Cell(rowc, col++).Value = "RFC Empresa";
    //        wsResumen.Cell(rowc, col++).Value = "UUID";

    //        //wsResumen.Cell(rowc, col++).Value = "Fecha Ini Periodo";
    //        wsResumen.Cell(rowc, col++).Value = "Fecha Fin Periodo";
    //        wsResumen.Cell(rowc, col++).Value = "Fecha Timbrado PAC";

    //        wsResumen.Cell(rowc, col++).Value = "Percepciones";
    //        wsResumen.Cell(rowc, col++).Value = "Gravado";
    //        wsResumen.Cell(rowc, col++).Value = "Excento";
    //        wsResumen.Cell(rowc, col++).Value = "Otros Pagos";
    //        wsResumen.Cell(rowc, col++).Value = "ISR";

    //        wsResumen.Cell(rowc, col++).Value = "SUB-TOTAL";
    //        wsResumen.Cell(rowc, col++).Value = "DESCUENTO";
    //        wsResumen.Cell(rowc, col++).Value = "TOTAL RECIBO";


    //        #endregion




    //        #endregion

    //        #region CONTENIDO
    //        //For
    //        int idEmpleado = 0;
    //        int idNomina = 0;
    //        int idPeriodo = 0;
    //        int rowx = 3;
    //        DataTable tablex = null;
    //        int rowp = 3; //termino de percepcion
    //        int rowd = 3;//termino de deduccion
    //        int rowo = 3;//termina otros pagos
    //        int rowi = 3;//row inicial para ambos p y d
    //        int rowinca = 3;
    //        bool rowColor = false;
    //        string strNombreEmpleado = "";
    //        Tuple<string, string> tpTimbre = new Tuple<string, string>("", "");
    //        Tuple<string, string> tpReceptor = new Tuple<string, string>("", "");
    //        string rfcEmisor = "";


    //        string folderPath = @"C:\xmlJuanita\Juanita XML\";
    //        //string folderPath = @"C:\xmlJuanita\Zada XML\";


    //        foreach (string file in Directory.EnumerateFiles(folderPath, "*.xml"))
    //        {
    //            string contents = File.ReadAllText(file);
            
           
    //            DataSet ds = new DataSet();
    //            strNombreEmpleado = "";
    //            rfcEmisor = "";
    //            tpTimbre = new Tuple<string, string>("", "");
    //            tpReceptor = new Tuple<string, string>("", "");

    //            //Carga el contenido del XML en el DATASET
    //            StringReader streamReader = new StringReader(contents);
    //            ds.ReadXml(streamReader);

    //            //leer id empleado
    //            idEmpleado = 99;
    //            idNomina = 99;

    //            idPeriodo = 99;

    //            #region Comprobante

    //            var tpComprobante = SetLineaComprobante(ref wsComprobante, ds, rowx, idEmpleado, idNomina, idPeriodo);
    //            //emisor 
    //            rfcEmisor = SetLineaEmisor(ref wsComprobante, ds, rowx, idEmpleado);
    //            //receptor             
    //            tpReceptor = SetLineaReceptor(ref wsComprobante, ds, rowx, idEmpleado);
    //            #endregion

    //            #region Concepto        
    //            SetLineaProdServ(ref wsComprobante, ds, rowx, idEmpleado);
    //            #endregion

    //            #region NOMINA
    //            var tpNom = SetLineaNomina(ref wsNomina, ds, rowx, idEmpleado, idNomina, idPeriodo);
    //            //emisor 
    //            SetLineaEmisorNom(ref wsNomina, ds, rowx, idEmpleado);
    //            //receptor
    //            SetLineaReceptorNom(ref wsNomina, ds, rowx, idEmpleado);
    //            //color - 

    //            #endregion


    //            tpTimbre = SetLineaTimbre(ref wsTimbre, ds, rowx, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1);


    //            #region PERCEPCION 
    //            var tpPercepciones = SetLineaPercepciones(ref wsPercepciones, ds, rowp, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");

    //            rowp = SetLineaPercepcion(ref wsPercepciones, ds, rowp, idEmpleado, rowColor, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
    //            #endregion

    //            #region DEDUCCIONES
    //            SetLineaDeducciones(ref wsDeducciones, ds, rowd, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
    //            var tpD = SetLineaDeduccion(ref wsDeducciones, ds, rowd, idEmpleado, rowColor, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
    //            rowd = tpD.Item1;
    //            #endregion

    //            #region OTROS PAGOS
    //            rowo = SetLineaOtrosPagos(ref wsOtrosPagos, ds, rowo, idEmpleado, idNomina, idPeriodo, tpReceptor.Item1, tpReceptor.Item2, rfcEmisor, tpTimbre.Item1, tpTimbre.Item2, "--");
    //            #endregion

    //            #region INCAPACIDADES
    //            rowinca = SetLineaIncapacidades(ref wsIncapacidad, ds, rowinca, idEmpleado, idNomina);
    //            #endregion


    //            //Tuple<object, object, object> tpComprobante, Tuple< object, object, object> tpNomina, Tuple<string, string> tpReceptor, Tuple< object, object> tpPercepciones, Tuple<int, decimal> tpIsr, Tuple< string, string> tpTimbre)

       

    //            SetResumen(ref wsResumen, rowx, idEmpleado, idNomina, idPeriodo, rfcEmisor, tpComprobante, tpNom, tpReceptor, tpPercepciones, tpD, tpTimbre, null);

    //            //antes aqui timbre

    //            //solo para conceptos
    //            //if (rowd > rowi || rowp > rowi || rowo > rowi)
    //            //{
    //            //    var rowh = rowd > rowp ? rowd : rowp;
    //            //    rowi = rowh > rowo ? rowh : rowo;
    //            //}
    //            //else
    //            //{
    //            //    rowi++;
    //            //}


    //            //Color line nomina
    //            if (rowColor)
    //            {
    //                var rangoNomina = "A" + rowx + ":AK" + rowx;
    //                wsNomina.Range(rangoNomina).Style.Fill.SetBackgroundColor(XLColor.Lavender);

    //                var rangoComprobante = "A" + rowx + ":AC" + rowx;
    //                wsComprobante.Range(rangoComprobante).Style.Fill.SetBackgroundColor(XLColor.Lavender);

    //                var rangoOtrosPagos = "A" + rowx + ":Q" + rowx;
    //                wsOtrosPagos.Range(rangoOtrosPagos).Style.Fill.SetBackgroundColor(XLColor.Lavender);

    //                var rangoInc = "A" + rowx + ":E" + rowx;
    //                wsIncapacidad.Range(rangoInc).Style.Fill.SetBackgroundColor(XLColor.Lavender);

    //                var rangoTim = "A" + rowx + ":I" + rowx;
    //                wsTimbre.Range(rangoTim).Style.Fill.SetBackgroundColor(XLColor.Lavender);

    //                var rangoResumen = "A" + rowx + ":Q" + rowx;
    //                wsResumen.Range(rangoResumen).Style.Fill.SetBackgroundColor(XLColor.Lavender);


    //            }

    //            rowColor = !rowColor;
    //            //nueva linea
    //            rowx++;
    //        }

    //        //SUMATORIA
    //        SetSumatoria(rowx, ref wsComprobante, 1);
    //        SetSumatoria(rowx, ref wsNomina, 2);
    //        SetSumatoria(rowp, ref wsPercepciones, 3);
    //        SetSumatoria(rowd, ref wsDeducciones, 4);
    //        SetSumatoria(rowo, ref wsOtrosPagos, 5);
    //        SetSumatoria(rowx, ref wsResumen, 6);
    //        #endregion




    //        //DISEÑO
    //        SetDiseño(ref wsComprobante, 1, rowx);
    //        SetDiseño(ref wsNomina, 2, rowx);
    //        SetDiseño(ref wsPercepciones, 3, rowx);
    //        SetDiseño(ref wsDeducciones, 4, rowx);
    //        SetDiseño(ref wsOtrosPagos, 5, rowx);
    //        SetDiseño(ref wsIncapacidad, 6, rowx);
    //        SetDiseño(ref wsTimbre, 7, rowx);
    //        SetDiseño(ref wsResumen, 8, rowx);

    //        var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

    //        string nombreArchivo = $"XmlExcel juanita.xlsx";
    //        var fileName = pathUsuario + nombreArchivo;
    //        workbook.SaveAs(fileName);

    //        //  return fileName;
    //        return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

    //    }




        private static Tuple<object, object, object> SetLineaComprobante(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo)
        {
            Tuple<object, object, object> tpDatos = new Tuple<object, object, object>("", "", "");
            DataTable tabla = null;
            if (ds == null) return tpDatos;

            if (ds.Tables.Contains("Comprobante"))
            {
                if (ds.Tables["Comprobante"] != null)
                {
                    tabla = ds.Tables["Comprobante"];
                }
            }

            if (tabla == null) return tpDatos;
            if (worksheet == null) return tpDatos;

            int col = 1;

            //dataSet.Tables["Nomina"].Columns.Contains("TotalPercepciones")
            //if (dataSet.Tables.Contains("Nomina"))
            object subtotalT = "";
            object descuentoT = "";
            object totalT = "";
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

                subtotalT = renglon["subTotal"] ?? "";
                worksheet.Cell(row, col++).Value = subtotalT; //"SubTotal";

                string descuento = "";
                if (renglon.Table.Columns.Contains("descuento"))
                {
                    descuentoT = renglon["descuento"].ToString();
                    // descuento = renglon["descuento"].ToString();
                }

                worksheet.Cell(row, col++).Value = descuentoT; //"Descuento";

                totalT = renglon["total"] ?? "--";
                worksheet.Cell(row, col++).Value = totalT;//renglon["total"] ?? "--"; //"Total";
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

            tpDatos = new Tuple<object, object, object>(subtotalT, descuentoT, totalT);

            return tpDatos;
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
        private static string SetLineaEmisor(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            string strRfcEmisor = "";
            DataTable tabla = null;
            if (ds == null) return strRfcEmisor;

           // tabla = ds.Tables[1];

            int cont = 0;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (cont == 1)
                    break;

                if (ds.Tables[i].TableName == "Emisor")
                {
                    tabla = ds.Tables[i];
                    cont++;
                }
            }



            if (tabla == null) return strRfcEmisor;
            if (worksheet == null) return strRfcEmisor;

            int col = 17;

            foreach (DataRow renglon in tabla.Rows)
            {
                strRfcEmisor = renglon["rfc"].ToString() ?? "";
                worksheet.Cell(row, col++).Value = strRfcEmisor;
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

            return strRfcEmisor;
        }
        private static Tuple<string, string> SetLineaReceptor(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            string strNombreEmpleado = "";
            string strRfcEmpleado = "";
            Tuple<string, string> datosEmpleado = new Tuple<string, string>("", "");

            DataTable tabla = null;
            if (ds == null) return datosEmpleado;

            //tabla = ds.Tables[4];

            //if (ds.Tables.Contains("RegimenFiscal"))
            //{
            //    tabla = ds.Tables[3];
            //}

            int cont = 0;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (cont == 1)
                break;

                if (ds.Tables[i].TableName == "Receptor")
                {
                    tabla = ds.Tables[i];
                    cont++;
                }
            }


            if (tabla == null) return datosEmpleado;
            if (worksheet == null) return datosEmpleado;

            int col = 20;

            foreach (DataRow renglon in tabla.Rows)
            {
                strNombreEmpleado = renglon["Nombre"]?.ToString() ?? "";
                strRfcEmpleado = renglon["rfc"].ToString() ?? "";

                worksheet.Cell(row, col++).Value = strRfcEmpleado;
                worksheet.Cell(row, col++).Value = strNombreEmpleado;

                string usocfdi = "";

                if (renglon.Table.Columns.Contains("UsoCFDI"))
                {
                    usocfdi = renglon["UsoCFDI"].ToString();
                }

                worksheet.Cell(row, col++).Value = usocfdi;
            }

            datosEmpleado = new Tuple<string, string>(strNombreEmpleado, strRfcEmpleado);


            return datosEmpleado;
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
        private static Tuple<object, object, object> SetLineaNomina(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo)
        {
            DataTable tabla = null;
            Tuple<object, object, object> tpDatos = new Tuple<object, object, object>("", "", "");

            if (ds == null) return tpDatos;

            if (ds.Tables.Contains("Nomina"))
            {
                if (ds.Tables["Nomina"] != null)
                {
                    tabla = ds.Tables["Nomina"];
                }
            }

            if (tabla == null) return tpDatos;
            if (worksheet == null) return tpDatos;

            int col = 1;

            object totalPT = "";
            object totalDT = "";
            object totalOT = "";
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
                totalPT = renglon["TotalPercepciones"] ?? "";
                worksheet.Cell(row, col++).Value = totalPT;//renglon["TotalPercepciones"] ?? "";

                //string totalD = "0.00";
                totalDT = "";

                if (renglon.Table.Columns.Contains("TotalDeducciones"))
                {
                    totalDT = renglon["TotalDeducciones"].ToString();
                }
                worksheet.Cell(row, col++).Value = totalDT;

                //string totalOtros = "0.00";
                totalOT = "";
                if (renglon.Table.Columns.Contains("TotalOtrosPagos"))
                {
                    totalOT = renglon["TotalOtrosPagos"].ToString() ?? "";
                }
                worksheet.Cell(row, col++).Value = totalOT;
                //  worksheet.Cell(row, col++).Value = renglon["TotalOtrosPagos"] ?? "";
            }


            tpDatos = new Tuple<object, object, object>(totalPT, totalDT, totalOT);

            return tpDatos;
        }
        private static void SetLineaEmisorNom(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado)
        {
            DataTable tabla = null;
            if (ds == null) return;

            //tabla = ds.Tables[7];
            //if (ds.Tables.Contains("RegimenFiscal"))
            //{
            //    tabla = ds.Tables[8];
            //}

            int cont = 0;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (cont == 2)
                    break;

                if (ds.Tables[i].TableName == "Emisor")
                {
                    tabla = ds.Tables[i];
                    cont++;
                }
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

            //tabla = ds.Tables[8];
            //if (ds.Tables.Contains("RegimenFiscal"))
            //{
            //    tabla = ds.Tables[9];
            //}


            int cont = 0;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (cont == 2)
                    break;

                if (ds.Tables[i].TableName == "Receptor")
                {
                    tabla = ds.Tables[i];
                    cont++;
                }
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
        private static Tuple<object, object> SetLineaPercepciones(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo, string nombreEmpleado, string rfcEmpleado, string rfcEmpresa, string uuid, string fechaTimbrado, string fechaProcesado)
        {
            DataTable tabla = null;
            Tuple<object, object> tpDatos = new Tuple<object, object>("", "");
            if (ds == null) return tpDatos;

            if (ds.Tables.Contains("Percepciones"))
            {
                if (ds.Tables["Percepciones"] != null)
                {
                    tabla = ds.Tables["Percepciones"];
                }
            }

            if (tabla == null) return tpDatos;
            if (worksheet == null) return tpDatos;

            int col = 1;
            object totalGT = "";
            object totalET = "";
            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idPeriodo;

                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = nombreEmpleado;
                worksheet.Cell(row, col++).Value = fechaTimbrado;

                worksheet.Cell(row, col++).Value = rfcEmpleado;
                worksheet.Cell(row, col++).Value = rfcEmpresa;
                worksheet.Cell(row, col++).Value = uuid;


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

                totalGT = renglon["TotalGravado"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["TotalGravado"] ?? "";
                totalET = renglon["TotalExento"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["TotalExento"] ?? "";

            }

            tpDatos = new Tuple<object, object>(totalGT, totalET);
            return tpDatos;
        }
        private static int SetLineaPercepcion(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, bool rowColor, int idNomina, int idPeriodo, string nombreEmpleado, string rfcEmpleado, string rfcEmpresa, string uuid, string fechaTimbrado, string fechaProcesado)
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

            int col = 14;//H

            foreach (DataRow renglon in tabla.Rows)
            {

                worksheet.Cell(row, 1).Value = idEmpleado;
                worksheet.Cell(row, 2).Value = idPeriodo;
                worksheet.Cell(row, 3).Value = idNomina;
                worksheet.Cell(row, 4).Value = nombreEmpleado;
                worksheet.Cell(row, 5).Value = fechaTimbrado;

                worksheet.Cell(row, 6).Value = rfcEmpleado;
                worksheet.Cell(row, 7).Value = rfcEmpresa;
                worksheet.Cell(row, 8).Value = uuid;


                worksheet.Cell(row, col++).Value = renglon["TipoPercepcion"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Clave"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Concepto"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["ImporteGravado"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["ImporteExento"] ?? "";

                if (rowColor)
                {
                    var rangoPintar = "A" + row + ":AJ" + row;
                    worksheet.Range(rangoPintar).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                }

                row++;
                col = 14;
            }



            return row;
        }
        private static void SetLineaDeducciones(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo, string nombreEmpleado, string rfcEmpleado, string rfcEmpresa, string uuid, string fechaTimbrado, string fechaProcesado)
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
                worksheet.Cell(row, col++).Value = idPeriodo;

                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = nombreEmpleado;
                worksheet.Cell(row, col++).Value = fechaTimbrado;
                // worksheet.Cell(row, col++).Value = fechaProcesado;
                worksheet.Cell(row, col++).Value = rfcEmpleado;
                worksheet.Cell(row, col++).Value = rfcEmpresa;
                worksheet.Cell(row, col++).Value = uuid;



                worksheet.Cell(row, col++).Value = renglon["TotalOtrasDeducciones"] ?? "";

                string tid = "";
                if (renglon.Table.Columns.Contains("TotalImpuestosRetenidos"))
                {
                    tid = renglon["TotalImpuestosRetenidos"].ToString();
                }
                worksheet.Cell(row, col++).Value = tid;
            }
        }
        private static Tuple<int, decimal> SetLineaDeduccion(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, bool rowColor, int idNomina, int idPeriodo, string nombreEmpleado, string rfcEmpleado, string rfcEmpresa, string uuid, string fechaTimbrado, string fechaProcesado)
        {
            DataTable tabla = null;
            Tuple<int, decimal> tpDatos = new Tuple<int, decimal>(row, 0);

            if (ds == null) return tpDatos;

            if (ds.Tables.Contains("Deduccion"))
            {
                if (ds.Tables["Deduccion"] != null)
                {
                    tabla = ds.Tables["Deduccion"];
                }
            }

            if (tabla == null) return tpDatos;
            if (worksheet == null) return tpDatos;

            int col = 11;
            decimal sumaIsr = 0;

            foreach (DataRow renglon in tabla.Rows)
            {

                worksheet.Cell(row, 1).Value = idEmpleado;
                worksheet.Cell(row, 2).Value = idPeriodo;
                worksheet.Cell(row, 3).Value = idNomina;
                worksheet.Cell(row, 4).Value = nombreEmpleado;
                worksheet.Cell(row, 5).Value = fechaTimbrado;
                //worksheet.Cell(row, 6).Value = fechaProcesado;
                worksheet.Cell(row, 6).Value = rfcEmpleado;
                worksheet.Cell(row, 7).Value = rfcEmpresa;
                worksheet.Cell(row, 8).Value = uuid;

                object claved = renglon["Clave"] ?? "";
               // object importeT = renglon["ImporteGravado"] ?? "";


                object importeT = "";
                if (renglon.Table.Columns.Contains("Importe"))
                {
                    importeT = renglon["Importe"] ?? "";
                }


                worksheet.Cell(row, col++).Value = renglon["TipoDeduccion"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Clave"] ?? "";
                worksheet.Cell(row, col++).Value = renglon["Concepto"] ?? "";
                worksheet.Cell(row, col++).Value = importeT;//renglon["Importe"] ?? "";

                if ( claved.ToString() == "043" || claved.ToString() == "154")
                {
                    decimal isrI = 0;
                    Decimal.TryParse(importeT.ToString(), out isrI);

                    sumaIsr += isrI;
                }


                if (rowColor)
                {
                    var rangoPintar = "A" + row + ":O" + row;
                    worksheet.Range(rangoPintar).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                }

                row++;
                col = 11;
            }


            tpDatos = new Tuple<int, decimal>(row, sumaIsr);

            return tpDatos;
        }
        private static int SetLineaOtrosPagos(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo, string nombreEmpleado, string rfcEmpleado, string rfcEmpresa, string uuid, string fechaTimbrado, string fechaProcesado)
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
                worksheet.Cell(row, col++).Value = idPeriodo;

                worksheet.Cell(row, col++).Value = idNomina;
                worksheet.Cell(row, col++).Value = nombreEmpleado;
                worksheet.Cell(row, col++).Value = fechaTimbrado;
                //worksheet.Cell(row, col++).Value = fechaProcesado;
                worksheet.Cell(row, col++).Value = rfcEmpleado;
                worksheet.Cell(row, col++).Value = rfcEmpresa;
                worksheet.Cell(row, col++).Value = uuid;




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

            int col = 13;

            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col).Value = renglon["SubsidioCausado"] ?? "";
            }
        }
        private static void SetLineaCompensacionSaldosAFavor(ref IXLWorksheet worksheet, DataTable tabla, int row, int idEmpleado)
        {
            if (tabla == null) return;
            if (worksheet == null) return;

            int col = 14;

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

        private static Tuple<string, string> SetLineaTimbre(ref IXLWorksheet worksheet, DataSet ds, int row, int idEmpleado, int idNomina, int idPeriodo, string receptor)
        {
            DataTable tabla = null;
            Tuple<string, string> datosTimbre = new Tuple<string, string>("", "");

            if (ds == null) return datosTimbre;
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

            if (tabla == null) return datosTimbre;
            if (worksheet == null) return datosTimbre;

            int col = 1;
            string strFechaTim = "";
            string strUUID = "";
            foreach (DataRow renglon in tabla.Rows)
            {
                worksheet.Cell(row, col++).Value = idEmpleado;
                worksheet.Cell(row, col++).Value = idPeriodo;
                worksheet.Cell(row, col++).Value = idNomina;

                worksheet.Cell(row, col++).Value = receptor;

                strFechaTim = renglon["FechaTimbrado"].ToString() ?? "";
                strUUID = renglon["UUID"].ToString() ?? "";

                worksheet.Cell(row, col++).Value = renglon["Version"] ?? "";
                worksheet.Cell(row, col++).Value = strUUID;
                worksheet.Cell(row, col++).Value = renglon["FechaTimbrado"].ToString() ?? "";

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

            datosTimbre = new Tuple<string, string>(strUUID, strFechaTim);
            return datosTimbre;
        }

        private static void SetResumen(ref IXLWorksheet worksheet, int row, int idEmpleado, int idNomina, int idPeriodo, string rfcEmisor, Tuple<object, object, object> tpComprobante, Tuple<object, object, object> tpNomina, Tuple<string, string> tpReceptor, Tuple<object, object> tpPercepciones, Tuple<int, decimal> tpIsr, Tuple<string, string> tpTimbre,
            NOM_PeriodosPago periodo)
        {
            int col = 1;
            worksheet.Cell(row, col++).Value = idEmpleado;
            worksheet.Cell(row, col++).Value = idPeriodo;
            worksheet.Cell(row, col++).Value = idNomina;
            worksheet.Cell(row, col++).Value = tpReceptor.Item1;

            worksheet.Cell(row, col++).Value = tpReceptor.Item2;
            worksheet.Cell(row, col++).Value = rfcEmisor;
            worksheet.Cell(row, col++).Value = tpTimbre.Item1;
            worksheet.Cell(row, col++).Value = periodo != null ? periodo.Fecha_Fin.ToString() : "--";
            worksheet.Cell(row, col++).Value = tpTimbre.Item2;


            worksheet.Cell(row, col++).Value = tpNomina.Item1;
            worksheet.Cell(row, col++).Value = tpPercepciones.Item1;
            worksheet.Cell(row, col++).Value = tpPercepciones.Item2;
            worksheet.Cell(row, col++).Value = tpNomina.Item3;

            worksheet.Cell(row, col++).Value = tpIsr.Item2;

            worksheet.Cell(row, col++).Value = tpComprobante.Item1;
            worksheet.Cell(row, col++).Value = tpComprobante.Item2;
            worksheet.Cell(row, col++).Value = tpComprobante.Item3;

        }

        private static void SetSumatoria(int row, ref IXLWorksheet ws, int tipo)
        {
            int rowSum = row - 1;
            int colummEnd = 0;
            string formula = "";

            switch (tipo)
            {
                case 1://comprobante
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
                case 3://Percepciones
                    colummEnd = 36;

                    formula = $"=SUM(I2:I{rowSum})";
                    ws.Cell(row, 9).FormulaA1 = formula;
                    ws.Cell(row, 9).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(L2:L{rowSum})";
                    ws.Cell(row, 12).FormulaA1 = formula;
                    ws.Cell(row, 12).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(M2:M{rowSum})";
                    ws.Cell(row, 13).FormulaA1 = formula;
                    ws.Cell(row, 13).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(Q2:Q{rowSum})";
                    ws.Cell(row, 17).FormulaA1 = formula;
                    ws.Cell(row, 17).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(R2:R{rowSum})";
                    ws.Cell(row, 18).FormulaA1 = formula;
                    ws.Cell(row, 18).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    break;
                case 4://Deducciones
                    colummEnd = 15;

                    formula = $"=SUM(I2:I{rowSum})";
                    ws.Cell(row, 9).FormulaA1 = formula;
                    ws.Cell(row, 9).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(J2:J{rowSum})";
                    ws.Cell(row, 10).FormulaA1 = formula;
                    ws.Cell(row, 10).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(N2:N{rowSum})";
                    ws.Cell(row, 14).FormulaA1 = formula;
                    ws.Cell(row, 14).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    break;
                case 5: //Otros Pagos
                    colummEnd = 17;

                    formula = $"=SUM(L2:L{rowSum})";
                    ws.Cell(row, 12).FormulaA1 = formula;
                    ws.Cell(row, 12).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    break;

                case 6://Resumen
                    colummEnd = 17;

                    formula = $"=SUM(J2:J{rowSum})";
                    ws.Cell(row, 10).FormulaA1 = formula;
                    ws.Cell(row, 10).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(K2:K{rowSum})";
                    ws.Cell(row, 11).FormulaA1 = formula;
                    ws.Cell(row, 11).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(L2:L{rowSum})";
                    ws.Cell(row, 12).FormulaA1 = formula;
                    ws.Cell(row, 12).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(M2:M{rowSum})";
                    ws.Cell(row, 13).FormulaA1 = formula;
                    ws.Cell(row, 13).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(N2:N{rowSum})";
                    ws.Cell(row, 14).FormulaA1 = formula;
                    ws.Cell(row, 14).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(O2:O{rowSum})";
                    ws.Cell(row, 15).FormulaA1 = formula;
                    ws.Cell(row, 15).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(P2:P{rowSum})";
                    ws.Cell(row, 16).FormulaA1 = formula;
                    ws.Cell(row, 16).Style.NumberFormat.Format = "$ ##,###,##0.00";

                    formula = $"=SUM(Q2:Q{rowSum})";
                    ws.Cell(row, 17).FormulaA1 = formula;
                    ws.Cell(row, 17).Style.NumberFormat.Format = "$ ##,###,##0.00";

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
            int rowAjuste = 2;
            switch (tipo)
            {
                case 1://Comprobante
                    #region COMPROBANTE
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
                    ws.RangeUsed().SetAutoFilter();
                    #endregion
                    break;
                case 2://Nomina
                    #region NOMINA
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

                    ws.RangeUsed().SetAutoFilter();
                    #endregion
                    break;
                case 3://Percepciones
                    #region PERCEPCIONES
                    finAjuste = 35;
                    rowAjuste = 3;
                    finFix = 5;

                 ws.Range("A2:AJ2")//Percepciones
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

                    ws.RangeUsed().SetAutoFilter();
                    #endregion
                    break;
                case 4://Deducciones
                    finAjuste = 16;
                    finFix = 5;
                    rowAjuste = 3;

                    ws.Range("A2:O2")//Percepciones
                .Style.Font.SetFontSize(12)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Brown);

                    ws.RangeUsed().SetAutoFilter();
                    break;
                case 5://OtrosPAgos
                    finAjuste = 16;
                    finFix = 5;
                    rowAjuste = 3;

                    ws.Range("A2:Q2")//Percepciones
                .Style.Font.SetFontSize(12)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Teal);

                    ws.RangeUsed().SetAutoFilter();
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
                    ws.RangeUsed().SetAutoFilter();

                    break;
                case 8://Resumen

                    finAjuste = 16;
                    finFix = 4;
                    ws.Range("A2:Q2")//Timbre
                 .Style.Font.SetFontSize(12)
                 .Font.SetBold(true)
                 .Font.SetFontColor(XLColor.White)
                 .Fill.SetBackgroundColor(XLColor.AirForceBlue);

                    
                    ws.Range($"N3:N{row}").Style.Font.SetFontColor(XLColor.Red);
                    ws.Range($"Q3:Q{row}").Style.Font.SetBold(true);

                    ws.Cell(2,14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range($"M3:M{row}").Style.NumberFormat.Format = " ##,###,##0.00";
                    ws.Range($"P3:P{row}").Style.NumberFormat.Format = " ##,###,##0.00";
                    ws.Range($"Q3:Q{row}").Style.NumberFormat.Format = " ##,###,##0.00";

                    ws.RangeUsed().SetAutoFilter();


                    break;

            }


            ws.SheetView.Freeze(iniFix, finFix);
            ws.Columns(1, finAjuste).AdjustToContents(2, 10);

        }

        //private static List<DatosVisor> BuscarTimbrados(DateTime? fechaI, DateTime? fechaF, int idPeriodoB = 0, int idEjercicio = 0, int idEmpresa = 0, int idSucursal = 0, bool Cancelados = false)
        //{
        //    List<DatosVisor> listaTimbrados = new List<DatosVisor>();

        //    using (var context = new RHEntities())
        //    {


        //        if (idPeriodoB > 0) //buscar por periodo
        //        {

        //            listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
        //                              where tim.IdEjercicio == idEjercicio
        //                                    && tim.IdEmisor == idEmpresa
        //                                    && tim.Cancelado == Cancelados
        //                                    && tim.IsPrueba == false
        //                                    && tim.FolioFiscalUUID != null
        //                                    && tim.IdPeriodo == idPeriodoB
        //                              select new DatosVisor()
        //                              {
        //                                  IdTimbrado = tim.IdTimbrado,
        //                                  IdEmpleado = tim.IdReceptor,
        //                                  IdPeriodo = tim.IdPeriodo,
        //                                  IdNomina = tim.IdNomina,
        //                                  IdFiniquito = tim.IdFiniquito,
        //                                  IdSucursal = tim.IdSucursal,
        //                                  XmlTimbrado = tim.XMLTimbrado,

        //                                  RFCEmisor = tim.RFCEmisor,
        //                                  RFCReceptor = tim.RFCReceptor,
        //                                  FolioFiscalUUID = tim.FolioFiscalUUID,
        //                                  TotalRecibo = tim.TotalRecibo,
        //                                  IdEmisor = tim.IdEmisor,
        //                                  FechaCertificacion = tim.FechaCertificacion,
        //                                  FechaCancelacion = tim.FechaCancelacion
        //                              }).ToList();

        //        }
        //        else if (idSucursal > 0 && (fechaI == null || fechaF == null)) // cliente sin fechas
        //        {
        //            listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
        //                              where tim.IdEjercicio == idEjercicio
        //                                    && tim.IdSucursal == idSucursal
        //                                    && tim.Cancelado == Cancelados
        //                                    && tim.IsPrueba == false
        //                                    && tim.FolioFiscalUUID != null
        //                              select new DatosVisor()
        //                              {
        //                                  IdTimbrado = tim.IdTimbrado,
        //                                  IdEmpleado = tim.IdReceptor,
        //                                  IdPeriodo = tim.IdPeriodo,
        //                                  IdNomina = tim.IdNomina,
        //                                  IdFiniquito = tim.IdFiniquito,
        //                                  IdSucursal = tim.IdSucursal,
        //                                  XmlTimbrado = tim.XMLTimbrado,

        //                                  RFCEmisor = tim.RFCEmisor,
        //                                  RFCReceptor = tim.RFCReceptor,
        //                                  FolioFiscalUUID = tim.FolioFiscalUUID,
        //                                  TotalRecibo = tim.TotalRecibo,
        //                                  IdEmisor = tim.IdEmisor,
        //                                  FechaCertificacion = tim.FechaCertificacion,
        //                                  FechaCancelacion = tim.FechaCancelacion
        //                              }).ToList();
        //        }
        //        else if (idSucursal > 0 && fechaI != null) //cliente con fecha
        //        {
        //            listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
        //                              where tim.IdEjercicio == idEjercicio
        //                                    && tim.IdEmisor == idEmpresa
        //                                    && tim.IdSucursal == idSucursal
        //                                    && tim.Cancelado == Cancelados
        //                                    && tim.IsPrueba == false
        //                                    && tim.FolioFiscalUUID != null
        //                                    && DbFunctions.TruncateTime(tim.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) &&
        //                                    DbFunctions.TruncateTime(tim.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
        //                              select new DatosVisor()
        //                              {
        //                                  IdTimbrado = tim.IdTimbrado,
        //                                  IdEmpleado = tim.IdReceptor,
        //                                  IdPeriodo = tim.IdPeriodo,
        //                                  IdNomina = tim.IdNomina,
        //                                  IdFiniquito = tim.IdFiniquito,
        //                                  IdSucursal = tim.IdSucursal,
        //                                  XmlTimbrado = tim.XMLTimbrado,

        //                                  RFCEmisor = tim.RFCEmisor,
        //                                  RFCReceptor = tim.RFCReceptor,
        //                                  FolioFiscalUUID = tim.FolioFiscalUUID,
        //                                  TotalRecibo = tim.TotalRecibo,
        //                                  IdEmisor = tim.IdEmisor,
        //                                  FechaCertificacion = tim.FechaCertificacion,
        //                                  FechaCancelacion = tim.FechaCancelacion
        //                              }).ToList();


        //        }
        //        else if (idEmpresa > 0 && (fechaI == null || fechaF == null)) // por empresa sin fecha
        //        {
        //            listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
        //                              where tim.IdEjercicio == idEjercicio
        //                                    && tim.IdEmisor == idEmpresa
        //                                    && tim.Cancelado == Cancelados
        //                                    && tim.IsPrueba == false
        //                                    && tim.FolioFiscalUUID != null
        //                              select new DatosVisor()
        //                              {
        //                                  IdTimbrado = tim.IdTimbrado,
        //                                  IdEmpleado = tim.IdReceptor,
        //                                  IdPeriodo = tim.IdPeriodo,
        //                                  IdNomina = tim.IdNomina,
        //                                  IdFiniquito = tim.IdFiniquito,
        //                                  IdSucursal = tim.IdSucursal,
        //                                  XmlTimbrado = tim.XMLTimbrado,

        //                                  RFCEmisor = tim.RFCEmisor,
        //                                  RFCReceptor = tim.RFCReceptor,
        //                                  FolioFiscalUUID = tim.FolioFiscalUUID,
        //                                  TotalRecibo = tim.TotalRecibo,
        //                                  IdEmisor = tim.IdEmisor,
        //                                  FechaCertificacion = tim.FechaCertificacion,
        //                                  FechaCancelacion = tim.FechaCancelacion
        //                              }).ToList();


        //        }
        //        else//por empresa y fecha
        //        {
        //            listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
        //                              where tim.IdEjercicio == idEjercicio
        //                                    && tim.IdEmisor == idEmpresa
        //                                    && tim.Cancelado == Cancelados
        //                                    && tim.IsPrueba == false
        //                                    && tim.FolioFiscalUUID != null
        //                                    && DbFunctions.TruncateTime(tim.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI)
        //                                    && DbFunctions.TruncateTime(tim.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
        //                              select new DatosVisor()
        //                              {
        //                                  IdTimbrado = tim.IdTimbrado,
        //                                  IdEmpleado = tim.IdReceptor,
        //                                  IdPeriodo = tim.IdPeriodo,
        //                                  IdNomina = tim.IdNomina,
        //                                  IdFiniquito = tim.IdFiniquito,
        //                                  IdSucursal = tim.IdSucursal,
        //                                  XmlTimbrado = tim.XMLTimbrado,

        //                                  RFCEmisor = tim.RFCEmisor,
        //                                  RFCReceptor = tim.RFCReceptor,
        //                                  FolioFiscalUUID = tim.FolioFiscalUUID,
        //                                  TotalRecibo = tim.TotalRecibo,
        //                                  IdEmisor = tim.IdEmisor,
        //                                  FechaCertificacion = tim.FechaCertificacion,
        //                                  FechaCancelacion = tim.FechaCancelacion
        //                              }).ToList();
        //        }
        //    }

        //    return listaTimbrados;
        //}

    }

    //public class DatosVisor
    //{
    //    public int IdTimbrado { get; set; }
    //    public int IdEmpleado { get; set; }
    //    public int IdPeriodo { get; set; }
    //    public int IdNomina { get; set; }
    //    public int IdFiniquito { get; set; }
    //    public int IdSucursal { get; set; }
    //    public string XmlTimbrado { get; set; }

    //    public string RFCEmisor { get; set; }
    //    public string RFCReceptor { get; set; }
    //    public string FolioFiscalUUID { get; set; }
    //    public decimal TotalRecibo { get; set; }
    //    public int IdEmisor { get; set; }
    //    public DateTime? FechaCertificacion { get; set; }
    //    public DateTime? FechaCancelacion { get; set; }
    //}
}
