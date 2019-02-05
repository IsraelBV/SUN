using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using RH.Entidades;
using ClosedXML.Excel;
using Common.Utils;
using MoreLinq;
using System.IO;
using SYA.BLL;

namespace RH.BLL
{

    public class ReportesImss
    {



        public static string GenerarReporteInfonavit(int idUsuario, int? idEjercicio, string pathFolder, string pathDescarga, int idEmpresa = 0)
        {

            var sucursales = Getsucursales();
            if (idUsuario == 0) return "";
            switch (idEjercicio)
            {
                case null:
                    return "";
                case 0:
                    return "";
            }

            List<NOM_Nomina> listaNominas;
            List<NOM_Nomina_Detalle> listaDetalles;
            List<NOM_PeriodosPago> listaPeriodo;
            List<Empleado_Infonavit> listaInfonavit;
            List<Sucursal> listaSucursales;
            List<Cliente> listaClientes;
            List<Empleado_Contrato> listaContratos;
            List<Empleado> listaEmpleados;
            List<Empresa> listaEmpresa;

            int[] arrayIdEmpresa;

            #region GET DATA


            using (var context = new RHEntities())
            {
                listaEmpresa = context.Empresa.ToList();

                if (idEmpresa == 0)//todas las empresas
                {
                    arrayIdEmpresa = listaEmpresa.Select(x => x.IdEmpresa).ToArray();
                }
                else //Por empresa
                {
                    arrayIdEmpresa = new int[1];
                    arrayIdEmpresa[0] = idEmpresa;
                }

                //Hace un metodo que devuelva el id de la primera nimona del ejercicio fiscal *

                //Filtrar por Ejercicio Fiscal
                listaNominas = (from n in context.NOM_Nomina
                                where n.IdEjercicio == idEjercicio
                                select n).ToList();

                //Filtrar por empresa 
                listaNominas = (from n2 in listaNominas
                                where arrayIdEmpresa.Contains(n2.IdEmpresaFiscal ?? 0)
                                select n2).ToList();


                var arrayNominasEje = listaNominas.Select(x => x.IdNomina).ToArray();


                //1.- Obtener los detalles de nominas con el concepto 51 - infonavit
                listaDetalles = (from d in context.NOM_Nomina_Detalle
                                 where arrayNominasEje.Contains(d.IdNomina)
                                 && d.IdConcepto == 51
                                 select d).ToList();

                //2.- Obtener el listado de las nomina filtrado por empresa y por ejercicio
                var arrayIdNominas = listaDetalles.Select(x => x.IdNomina).ToArray();

                arrayIdNominas = arrayIdNominas.Distinct().ToArray();


                //3.- Obtener el listado de contratos
                var arrayIdContratos = listaNominas.Select(x => x.IdContrato).ToArray();
                arrayIdContratos = arrayIdContratos.Distinct().ToArray();

                listaContratos = (from c in context.Empleado_Contrato
                                  where arrayIdContratos.Contains(c.IdContrato)
                                  select c).ToList();

                //4.- Obtner el listado de Empleados
                var arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();

                arrayIdEmpleados = arrayIdEmpleados.Distinct().ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

                //5.- Obtener el listado de Empleado-Infonavit
                listaInfonavit = (from ei in context.Empleado_Infonavit
                                  where arrayIdContratos.Contains(ei.IdEmpleadoContrato)
                                  select ei).ToList();


                //6.- Obtener el listado de periodo
                var arrayIdPeriodos = listaNominas.Select(x => x.IdPeriodo).ToArray();

                arrayIdPeriodos = arrayIdPeriodos.Distinct().ToArray();

                listaPeriodo = (from p in context.NOM_PeriodosPago
                                where arrayIdPeriodos.Contains(p.IdPeriodoPago)
                                select p).ToList();

                //7.- Obtener el listado de Sucursales
                listaSucursales = (from s in context.Sucursal
                                   where sucursales.Contains(s.IdSucursal)
                                   select s).ToList();

                //8.- Obtner el listado de Clientes
                listaClientes = (from c in context.Cliente
                                 select c).ToList();

                listaDetalles = (from d in listaDetalles
                                 where arrayIdNominas.Contains(d.IdNomina)
                                 select d).ToList();

            }

            #endregion

            #region SET LISTA

            //Creamos el objeto -- 
            var listaReporte = (from dn in listaDetalles
                                join n in listaNominas on dn.IdNomina equals n.IdNomina
                                join c in listaContratos on n.IdContrato equals c.IdContrato
                                join e in listaEmpleados on n.IdEmpleado equals e.IdEmpleado
                                join p in listaPeriodo on n.IdPeriodo equals p.IdPeriodoPago
                                join ei in listaInfonavit on n.IdContrato equals ei.IdEmpleadoContrato
                                join emp in listaEmpresa on n.IdEmpresaFiscal equals emp.IdEmpresa
                                join suc in listaSucursales on n.IdSucursal equals suc.IdSucursal
                                join cli in listaClientes on n.IdCliente equals cli.IdCliente
                                select new ReporteInfonavit()
                                {
                                    IdNomina = dn.IdNomina,
                                    IdConcepto = dn.IdConcepto,
                                    TotalDescuento = dn.Total,
                                    IdEmpleado = n.IdEmpleado,
                                    APaterno = e.APaterno,
                                    AMaterno = e.AMaterno,
                                    Nombres = e.Nombres,
                                    NSS = e.NSS,
                                    Estatus = c.Status,
                                    Sd = c.SD,
                                    Sdi = c.SDI,
                                    Sbc = c.SBC,
                                    NombrePeriodo = p.Descripcion,
                                    Autorizado = p.Autorizado,
                                    Bimestre = p.Bimestre,
                                    Mes = p.Fecha_Inicio,
                                    DiasLaborados = n.Dias_Laborados,
                                    NumCredito = ei.NumCredito,
                                    TipoCredito = ei.TipoCredito,
                                    FactorDescuento = ei.FactorDescuento,
                                    FechaInicio = ei.FechaInicio,
                                    FechaSuspension = ei.FechaSuspension == null ? "-" : ei.FechaSuspension.ToString(),
                                    UsaUma = ei.UsarUMA,
                                    Empresa = emp.RazonSocial,
                                    Cliente = cli.Nombre,
                                    Sucursal = suc.Ciudad

                                }).ToList();
            #endregion

            var workbook = new XLWorkbook();

            #region DOCUMENTO EXCEL - NOMINA ********************************************************

            var worksheet = workbook.Worksheets.Add("Infonavit");


            #region HEADER
            worksheet.Cell(1, 1).Value = "IdNomina";
            worksheet.Cell(1, 2).Value = "IdEmpleado";
            worksheet.Cell(1, 3).Value = "Paterno";
            worksheet.Cell(1, 4).Value = "Materno";
            worksheet.Cell(1, 5).Value = "Nombres";
            worksheet.Cell(1, 6).Value = "NSS";
            worksheet.Cell(1, 7).Value = "Estatus";
            worksheet.Cell(1, 8).Value = "Sd";
            worksheet.Cell(1, 9).Value = "Sdi";
            worksheet.Cell(1, 10).Value = "Sbc";
            worksheet.Cell(1, 11).Value = "IdConcepto";
            worksheet.Cell(1, 12).Value = "Nombre Periodo";
            worksheet.Cell(1, 13).Value = "Bimestre";
            worksheet.Cell(1, 14).Value = "Mes";
            worksheet.Cell(1, 15).Value = "Dias Laborados";
            worksheet.Cell(1, 16).Value = "Num Crédito";
            worksheet.Cell(1, 17).Value = "Tipo Crédito";
            worksheet.Cell(1, 18).Value = "Factor Descuento";
            worksheet.Cell(1, 19).Value = "Fecha Inicio de Crédito";
            worksheet.Cell(1, 20).Value = "Fecha Suspensión";
            worksheet.Cell(1, 21).Value = "Usa Uma";
            worksheet.Cell(1, 22).Value = "Total Descuento";
            worksheet.Cell(1, 23).Value = "Empresa";
            worksheet.Cell(1, 24).Value = "Cliente";
            worksheet.Cell(1, 25).Value = "Sucursal";
            #endregion

            #region CONTENIDO
            int row = 2;

            foreach (var item in listaReporte)
            {
                if (item.Autorizado == false) continue;

                worksheet.Cell(row, 1).Value = item.IdNomina;
                worksheet.Cell(row, 2).Value = item.IdEmpleado;
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 3).Value = item.APaterno;
                worksheet.Cell(row, 4).Value = item.AMaterno;
                worksheet.Cell(row, 5).Value = item.Nombres;
                worksheet.Cell(row, 6).Value = item.NSS;
                worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 7).Value = item.Estatus ? "Activo" : "Inactivo";
                worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 8).Value = item.Sd;
                worksheet.Cell(row, 9).Value = item.Sdi;
                worksheet.Cell(row, 10).Value = item.Sbc;
                worksheet.Cell(row, 11).Value = item.IdConcepto;
                worksheet.Cell(row, 12).Value = item.NombrePeriodo;
                worksheet.Cell(row, 13).Value = item.Bimestre;
                worksheet.Cell(row, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 14).Value = item.Mes.GeNombreDelMes();
                worksheet.Cell(row, 15).Value = item.DiasLaborados;
                worksheet.Cell(row, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 16).Value = item.NumCredito;

                var strTipoCredito = "";
                switch (item.TipoCredito)
                {
                    case 1:
                        strTipoCredito = "Cuota Fija";
                        break;
                    case 2:
                        strTipoCredito = "Porcentaje";
                        break;
                    case 3:
                        strTipoCredito = "VSM";
                        break;
                    case 4:
                        strTipoCredito = "VSM UMA";
                        break;
                }

                worksheet.Cell(row, 17).Value = strTipoCredito;

                worksheet.Cell(row, 18).Value = item.FactorDescuento;
                worksheet.Cell(row, 19).Value = item.FechaInicio;
                worksheet.Cell(row, 19).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 20).Value = item.FechaSuspension;
                worksheet.Cell(row, 20).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 21).Value = item.UsaUma ? "Si" : "No";
                worksheet.Cell(row, 22).Value = item.TotalDescuento;
                worksheet.Cell(row, 22).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 23).Value = item.Empresa;
                worksheet.Cell(row, 24).Value = item.Cliente;
                worksheet.Cell(row, 25).Value = item.Sucursal;
                row++;
            }
            #endregion

            #region DISEÑO
            //Fix
            worksheet.SheetView.Freeze(1, 4);

            //Color
            worksheet.Range("A1:Y1").Style
               .Font.SetFontSize(14)
               .Font.SetBold(true)
               .Font.SetFontColor(XLColor.White)
               .Fill.SetBackgroundColor(XLColor.Red);

            //Ajuste de columnas
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
            worksheet.Column(15).AdjustToContents();
            worksheet.Column(16).AdjustToContents();
            worksheet.Column(17).AdjustToContents();
            worksheet.Column(18).AdjustToContents();
            worksheet.Column(19).AdjustToContents();
            worksheet.Column(20).AdjustToContents();
            worksheet.Column(21).AdjustToContents();
            worksheet.Column(22).AdjustToContents();
            worksheet.Column(23).AdjustToContents();
            worksheet.Column(24).AdjustToContents();
            worksheet.Column(25).AdjustToContents();

            #endregion

            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "ReporteInfonavit_.xlsx";

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);
            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

            #endregion


        }

        public static string GenerarReporte3Porciento(int idUsuario, DateTime fecI, DateTime fecF, int idEmpresa = 0, string pathFolder = "", string pathDescarga = "")
        {
            var sucursales = Getsucursales();
            List<Empresa> listaEmpresa;
            List<NOM_PeriodosPago> listaPeriodos;
            List<NOM_PeriodosPago> listaPeriodosFiniquito;
            List<Sucursal> listaSucursal;
            List<NOM_Nomina_Detalle> listaDetalles;
            List<Empleado> listaEmpleados;
            List<C_NOM_Conceptos> listaConceptosValidos;
            List<NOM_Nomina> listaNominas;
            List<Cliente> listaClientes;
            List<NOM_Finiquito> listaFiniquitos;
            List<NOM_Finiquito_Detalle> listaDetallesFiniquito;

            int[] arrayIdEmpresa;
            int[] arrayIdConceptosValidos;
            int[] arrayIdPeriodos_asimilados;

            #region GET DATOS

            using (var context = new RHEntities())
            {

                listaEmpresa = context.Empresa.Where(x => x.RegistroPatronal != null).ToList();

                //Obtiene el array de empresas
                if (idEmpresa == 0) //todas las empresas
                {
                    arrayIdEmpresa = listaEmpresa.Select(x => x.IdEmpresa).ToArray();
                }
                else //Por empresa
                {
                    arrayIdEmpresa = new int[1];
                    arrayIdEmpresa[0] = idEmpresa;
                }


                //DETALLES




                listaSucursal = (from s in context.Sucursal
                                 where sucursales.Contains(s.IdSucursal)
                                 select s).ToList();

                listaClientes = (from c in context.Cliente select c).ToList();

                //LISTA DE CONCEPTOS DE NOMINA VALIDOS
                listaConceptosValidos = (from c in context.C_NOM_Conceptos
                                         where c.ImpuestoSobreNomina == true
                                         select c
                                           ).ToList();

                //array de ids de conceptos validos
                arrayIdConceptosValidos = listaConceptosValidos.OrderBy(x => x.IdConcepto).Select(x => x.IdConcepto).ToArray();

                #region SE OBTIENEN LOS PERIODOS

                //OBTIENE LOS PERIODOS DE NOMINA Y ASIMILADO MIENTRAS LA FECHA DE FIN ESTE ENTRE EL RANGO DE FECHAS
                listaPeriodos = (from p in context.NOM_PeriodosPago
                                 where DbFunctions.TruncateTime(p.Fecha_Fin) >= DbFunctions.TruncateTime(fecI) && DbFunctions.TruncateTime(p.Fecha_Fin) <= DbFunctions.TruncateTime(fecF)
                                       && p.Autorizado == true
                                       && (p.IdTipoNomina < 16 && p.IdTipoNomina != 11) //Nominas - Asimilados
                                                                                        //&& (p.IdTipoNomina <= 16) //Nominas - Asimilados - finiquitos
                                       && p.SoloComplemento == false //Que no sean solo complemento
                                 select p).ToList();

                //ARRAY DE LISTA DE RIODOS DE NOMINA
                var arrayIdPeriodos = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                arrayIdPeriodos_asimilados = listaPeriodos.Where(w => w.IdTipoNomina == 16).Select(x => x.IdPeriodoPago).ToArray();

                //OBTIENE LOS PERIODOS DE FINIQUITO DONDE LA FECHA DE TIMBRADO ESTE ENTRE EL RANGO DE FECHAS
                listaPeriodosFiniquito = (from p in context.NOM_PeriodosPago
                                          join t in context.NOM_CFDI_Timbrado on p.IdPeriodoPago equals t.IdPeriodo
                                          where DbFunctions.TruncateTime(t.FechaCertificacion) >= DbFunctions.TruncateTime(fecI) && DbFunctions.TruncateTime(t.FechaCertificacion) <= DbFunctions.TruncateTime(fecF)
                                          && p.Autorizado == true
                                          && p.IdTipoNomina == 11 //Nominas - Asimilados
                                          && p.SoloComplemento == false //Que no sean solo complemento
                                          select p).ToList();

                //ARRAY DE LISTA DE RIODOS DE NOMINA
                var arrayIdPeriodosFiniquito = listaPeriodosFiniquito.Select(x => x.IdPeriodoPago).ToArray();
                #endregion

                #region SE OBTIENEN LAS NOMINAS LIGADAS A LOS PERIODOS
                //BUSCA TODAS LA NOMINAS QUE CONTENGAN LOS PERIODOS DEL arrayIdPeriodos
                listaNominas = (from n in context.NOM_Nomina
                                where arrayIdPeriodos.Contains(n.IdPeriodo)
                                && sucursales.Contains(n.IdSucursal)
                                && (arrayIdEmpresa.Contains(n.IdEmpresaFiscal.Value) || arrayIdEmpresa.Contains(n.IdEmpresaAsimilado.Value))
                                select n).ToList();

                //BUSCA TODAS LA NOMINAS QUE CONTENGAN LOS PERIODOS DEL arrayIdPeriodosFiniquito
                listaFiniquitos = (from f in context.NOM_Finiquito
                                   where arrayIdPeriodosFiniquito.Contains(f.IdPeriodo)
                                   && sucursales.Contains(f.IdSucursal)
                                   && arrayIdEmpresa.Contains(f.IdEmpresaFiscal.Value)
                                   select f).ToList();
                #endregion

                #region FILTRA NOMINAS CANCELADAS

                //BUSCA TODAS LAS NOMINAS CANCELADAS BASANDOSE EN LOS PERIODOS (arrayIdPeriodos)
                var TimbradosCancelados = (from nc in context.NOM_CFDI_Timbrado
                                           where nc.Cancelado == true && arrayIdPeriodos.Contains(nc.IdPeriodo)
                                           select nc).ToList();
                //ARRAY DE NOMINAS CANCELADAS
                var arrayNominasTimbradosCancelados = TimbradosCancelados.Where(x => x.IdNomina != 0).Select(x => x.IdNomina).ToArray();
                //ARRAY DE FINIQUITOS CANCELADOS
                var arrayFiniquitosTimbradosCancelados = TimbradosCancelados.Where(x => x.IdFiniquito != 0).Select(x => x.IdFiniquito).ToArray();

                if (arrayNominasTimbradosCancelados.Length > 0)
                {
                    //SI EXITEN CANCELADAS SE FILTRAN  EN BASE AL ARRAY 
                    listaNominas = (from nn in listaNominas
                                    where !arrayNominasTimbradosCancelados.Contains(nn.IdNomina)
                                    select nn).ToList();
                }

                if (arrayFiniquitosTimbradosCancelados.Length > 0)
                {
                    //SI EXITEN FINIQUITOS CANCELADOS SE FILTRAN  EN BASE AL ARRAY 
                    listaFiniquitos = (from nn in listaFiniquitos
                                       where !arrayFiniquitosTimbradosCancelados.Contains(nn.IdFiniquito)
                                       select nn).ToList();
                }

                #endregion

                #region CONCEPTOS DE LAS NOMINAS
                //Busca los detalles de nominas 
                var arrayIdNominas = (from n in listaNominas
                                      select n.IdNomina).ToArray();

                listaDetalles = (from nd in context.NOM_Nomina_Detalle
                                 where arrayIdNominas.Contains(nd.IdNomina) && arrayIdConceptosValidos.Contains(nd.IdConcepto) //&& nd.ImpuestoSobreNomina > 0
                                 select nd).ToList();

                //Buscamos los detalles de finiquito

                var arrayIdFiniquitos = (from f in listaFiniquitos
                                         select f.IdFiniquito).ToArray();

                listaDetallesFiniquito = (from df in context.NOM_Finiquito_Detalle
                                          where arrayIdFiniquitos.Contains(df.IdFiniquito) && arrayIdConceptosValidos.Contains(df.IdConcepto) //df.ImpuestoSobreNomina > 0
                                          select df).ToList();


                //Se filtran los conceptos validos que tienen algun valor en las tablas de detalles 
                var listIdConceptosValidos = listaDetalles.Select(x => x.IdConcepto).ToList();
                listIdConceptosValidos.AddRange(listaDetallesFiniquito.Select(x => x.IdConcepto).ToList());

                #endregion

                arrayIdConceptosValidos = listIdConceptosValidos.Distinct().OrderBy(x => x).ToArray();

                #region SE CONSIGUE LOS EMPLEADOS
                //EMPLEADOS
                var empleadosnominas = (from n in listaNominas
                                        select n.IdEmpleado).ToList();

                var empleadosfiniquitos = (from n in listaFiniquitos
                                           select n.IdEmpleado).ToList();

                empleadosnominas.AddRange(empleadosfiniquitos);
                //se busacan los empleados que pertenecen a esa empresa, que estan activos para tener el reporte de imss completo-----
                var empleadosSinPeriodo = (from ec in context.Empleado_Contrato 
                                           where arrayIdEmpresa.Contains(ec.IdEmpresaFiscal.Value)
                                           && !empleadosnominas.Contains(ec.IdEmpleado)
                                           && ec.Status == true
                                           select ec.IdEmpleado).ToList();
                    
                //-------------------------------------------------------------------------------------------------------------------

                empleadosnominas.AddRange(empleadosSinPeriodo);

                //hacer distinct//quitar ids empleados repetidos
                var arrayIdEmpleados = empleadosnominas.Distinct().OrderBy(x => x).ToArray();

                //arrayIdEmpleados = arrayIdEmpleados.Distinct().ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();
                #endregion
            }
            #endregion

            //#region DOCUMENTO EXCEL - NOMINA ********************************************************
            var workbook = new XLWorkbook();

            //PRIMER FOR POR EMPRESA
            #region Resumen Worksheet // encabezados primera hoja
            var worksheetR = workbook.Worksheets.Add("Resumen");
            worksheetR.Cell(1, 1).Value = "Empresa";
            worksheetR.Cell(1, 2).Value = "Cliente";
            worksheetR.Cell(1, 3).Value = "Num Emp";
            worksheetR.Cell(1, 4).Value = "Empleado";

            int colheadR = 5;
            foreach (var varId in arrayIdConceptosValidos)
            {
                var itemConcepto = listaConceptosValidos.FirstOrDefault(x => x.IdConcepto == varId);

                worksheetR.Cell(1, colheadR).Value = itemConcepto.DescripcionCorta;
                colheadR++;
            }
            worksheetR.Cell(1, colheadR).Value = "Total";
            colheadR++;
            worksheetR.Cell(1, colheadR).Value = "Total ISN";

            #endregion

            foreach (var idEmpresaArray in arrayIdEmpresa)
            {               
                var itemEmpresa = listaEmpresa.FirstOrDefault(x => x.IdEmpresa == idEmpresaArray);

                //Crea la hoja del archivo
                var worksheet = workbook.Worksheets.Add(itemEmpresa.RazonSocial.Substring(0, 5) + itemEmpresa.IdEmpresa);

                worksheet.SheetView.Freeze(1, 3);

                #region HEADERS segunda hoja

                worksheet.Cell(1, 1).Value = "Cliente";
                worksheet.Cell(1, 2).Value = "Num Emp";
                worksheet.Cell(1, 3).Value = "Empleado";

                int colhead = 4;
                foreach (var varId in arrayIdConceptosValidos)
                {
                    var itemConcepto = listaConceptosValidos.FirstOrDefault(x => x.IdConcepto == varId);

                    worksheet.Cell(1, colhead).Value = itemConcepto.DescripcionCorta;
                    colhead++;
                }
                worksheet.Cell(1, colhead).Value = "Total";
                colhead++;
                worksheet.Cell(1, colhead).Value = "Total ISN";

                #endregion


                decimal[] totalesConceptos = new decimal[arrayIdConceptosValidos.Length + 2];

                int rowExcel = 2;
                int rowExcelr = worksheetR.RowsUsed().Count() + 1;
                worksheetR.Cell(rowExcelr, 1).Value = itemEmpresa.RazonSocial;
                worksheetR.Row(rowExcelr).Style.Font.SetFontSize(13).Font.SetBold(true);
                rowExcelr++;


                //COMBINACION NOMINAS - FINIQUITOS
                //Clasifica las nominas por IdEmpresaFiscal
                var listaNominaByEmpresa = (from n in listaNominas
                                            where n.IdEmpresaFiscal == idEmpresaArray || (n.IdEmpresaAsimilado == idEmpresaArray && arrayIdPeriodos_asimilados.Contains(n.IdPeriodo))
                                            //IdEmpresa
                                            select n).ToList();

                var listaFiniquitosByEmpresa = (from f in listaFiniquitos
                                                where f.IdEmpresaFiscal == idEmpresaArray
                                                select f).ToList();




                //Obtenemos el array de Sucursales
                var ns = listaNominaByEmpresa.Select(x => x.IdSucursal).ToList();

                var fs = listaFiniquitosByEmpresa.Select(x => x.IdSucursal).ToList();

                ns.AddRange(fs);

                var arrayIdSucursales = ns.ToArray();

                arrayIdSucursales = arrayIdSucursales.Distinct().ToArray();

                
                //SEGUNDO FOR POR SUCURSAL

                foreach (var itemSucursal in arrayIdSucursales)
                {
                    //Limpia el array reiniciando a ceros
                    Array.Clear(totalesConceptos, 0, totalesConceptos.Length);

                    var listaNominaBySucursal = (from n in listaNominaByEmpresa
                                                 where n.IdSucursal == itemSucursal //Sucursal
                                                 select n).ToList();

                    var listaFiniquitoBySucursal = (from f in listaFiniquitosByEmpresa
                                                    where f.IdSucursal == itemSucursal //Sucursal
                                                    select f).ToList();


                    //TERCER FOR POR EMPLEADO

                    ////-->forma anterior se sacan las empleados de la lista de nominas por sucursal: listaNominaBySucursal// cambiado el 01/02/2019

                    //var ne = listaNominaBySucursal.Select(x => x.IdEmpleado).ToList();
                    //var fe = listaFiniquitoBySucursal.Select(x => x.IdEmpleado).ToList();
                    //ne.AddRange(fe);
                    //var arrayIdEmpleadosFromSuc = ne.ToArray();
                    //arrayIdEmpleadosFromSuc = arrayIdEmpleadosFromSuc.Distinct().ToArray();

                    ////-->nueva forma a partir del 01/02/2019 para considerar a los empleados que no tienen nominas se toma de la lista de empleados de nominas y finiquitos "listaEmpleados"
                    var arrayIdEmpleadosFromSuc = listaEmpleados.Where(x => x.IdSucursal == itemSucursal).Select(x => x.IdEmpleado).ToArray();

                    #region FOR EMPLEADOS

                    var strNombreCliente = "";

                    foreach (var idEmpleadoArray in arrayIdEmpleadosFromSuc)
                    {
                        int columnExcel = 1;

                        //Hace la sumatoria por concepto
                        var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == idEmpleadoArray);

                        var nombreEmpleado = $"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";

                        //Obtiene la nominas del empleado
                        var listaNominaByEmpleado = (from n in listaNominaBySucursal
                                                     where n.IdEmpleado == idEmpleadoArray
                                                     select n).ToList();

                        //obtiene el array Id nominas
                        var arrayIdNominaEmpleado = listaNominaByEmpleado.Select(x => x.IdNomina).ToArray();

                        //Obtengo los detalles del array de nominas
                        var listaDetalleByEmpleado = (from d in listaDetalles
                                                      where arrayIdNominaEmpleado.Contains(d.IdNomina)
                                                      select d).ToList();

                        //Obtiene los finiquitos del empleado
                        var listaFiniquitosByEmpleado = (from f in listaFiniquitoBySucursal
                                                         where f.IdEmpleado == idEmpleadoArray
                                                         select f).ToList();

                        //obtiene el array Id Finiquitos
                        var arrayIdFiniquitoEmpleado = listaFiniquitosByEmpleado.Select(x => x.IdFiniquito).ToArray();

                        //Obtengo los detalles del array de nominas
                        var listaDetalleFByEmpleado = (from d in listaDetallesFiniquito
                                                       where arrayIdFiniquitoEmpleado.Contains(d.IdFiniquito)
                                                       select d).ToList();



                        var sucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemSucursal);
                        var cliente = listaClientes.FirstOrDefault(x => x.IdCliente == sucursal.IdCliente);

                        //Establece el nombre del cliente
                        strNombreCliente = cliente.Nombre + " - " + sucursal.Ciudad;
                        worksheet.Cell(rowExcel, columnExcel).Value = strNombreCliente;
                        columnExcel++;
                        columnExcel++;
                        //Establece el nombre del empleado
                        worksheet.Cell(rowExcel, columnExcel).Value = nombreEmpleado;
                        columnExcel++;

                        if (idEmpleadoArray ==1192 || idEmpleadoArray == 1819) {
                            var qw = 0; 
                        }

                        //CUARTO FOR POR CONCEPTOS
                        decimal sumaIsn = 0;
                        decimal totalPercepciones = 0;
                        int indexConceptos = 0;
                        foreach (var itemConcepto in arrayIdConceptosValidos)
                        {
                            //DATOS DE LA NOMINA
                            var listaConceptoN = (from c in listaDetalleByEmpleado
                                                  where c.IdConcepto == itemConcepto
                                                  select c).ToList();

                            var sumatoriaConcepto = listaConceptoN.Sum(x => x.Total);

                            sumaIsn += listaConceptoN.Sum(x => x.ImpuestoSobreNomina);


                            //DATOS DEL FINIQUITO
                            var listaConceptoF = (from c in listaDetalleFByEmpleado
                                                  where c.IdConcepto == itemConcepto
                                                  select c).ToList();


                            if (listaConceptoF.Count > 0)
                            {
                                var sumatoriaConceptoF = listaConceptoF.Sum(x => x.Total);
                                var sumaisnF = listaConceptoF.Sum(x => x.ImpuestoSobreNomina);

                                sumatoriaConcepto += sumatoriaConceptoF;
                                sumaIsn += sumaisnF;
                            }

                            totalPercepciones += sumatoriaConcepto;


                            //se agrega al total
                            totalesConceptos[indexConceptos] += sumatoriaConcepto;
                            indexConceptos++;

                            //lo guardamos al excel
                            worksheet.Cell(rowExcel, columnExcel).Value = sumatoriaConcepto;
                            columnExcel++;
                        }

                        worksheet.Cell(rowExcel, columnExcel).Value = totalPercepciones;
                        columnExcel++;
                        worksheet.Cell(rowExcel, columnExcel).Value = sumaIsn;

                        totalesConceptos[indexConceptos] += totalPercepciones;
                        indexConceptos++;
                        totalesConceptos[indexConceptos] += sumaIsn;
                        //*****************************************************************************

                        rowExcel++;

                    } //Fin del for empleado
                    #endregion



                    //Agregamos linea para los totales

                    worksheet.Cell(rowExcel, 1).Value = strNombreCliente;
                    worksheet.Cell(rowExcel, 2).Value = arrayIdEmpleadosFromSuc.Length;
                    worksheet.Cell(rowExcel, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheetR.Cell(rowExcelr, 2).Value = strNombreCliente;
                    worksheetR.Cell(rowExcelr, 3).Value = arrayIdEmpleadosFromSuc.Length;
                    worksheetR.Cell(rowExcelr, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    int indexTotal = 4;
                    foreach (var itemtotal in totalesConceptos)
                    {
                        worksheet.Cell(rowExcel, indexTotal).Value = itemtotal;
                        worksheetR.Cell(rowExcelr, indexTotal + 1).Value = itemtotal;
                        indexTotal++;
                    }

                    worksheet.Range(rowExcel, 1, rowExcel, 26).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);

                    worksheetR.Range(rowExcelr, 1, rowExcelr, 14).Style.Font.SetFontColor(XLColor.Black);

                    if (rowExcelr % 2 == 0)
                    {
                        worksheetR.Range(rowExcelr, 1, rowExcelr, 14).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    }


                    rowExcel++;
                    rowExcelr++;

                } //Fin for sucursal

                #region AJUSTES DE COLUMNA
                //worksheet.Column(1).AdjustToContents();
                //worksheet.Column(2).AdjustToContents();
                //worksheet.Column(3).AdjustToContents();
                //worksheet.Column(4).AdjustToContents();
                //worksheet.Column(5).AdjustToContents();
                //worksheet.Column(6).AdjustToContents();
                //worksheet.Column(7).AdjustToContents();
                //worksheet.Column(8).AdjustToContents();
                //worksheet.Column(9).AdjustToContents();
                //worksheet.Column(10).AdjustToContents();
                //worksheet.Column(11).AdjustToContents();
                //worksheet.Column(12).AdjustToContents();
                //worksheet.Column(13).AdjustToContents();
                //worksheet.Column(14).AdjustToContents();
                //worksheet.Column(15).AdjustToContents();
                //worksheet.Column(16).AdjustToContents();
                //worksheet.Column(17).AdjustToContents();
                //worksheet.Column(18).AdjustToContents();
                //worksheet.Column(19).AdjustToContents();
                //worksheet.Column(20).AdjustToContents();
                //worksheet.Column(21).AdjustToContents();
                //worksheet.Column(22).AdjustToContents();
                //worksheet.Column(23).AdjustToContents();
                //worksheet.Column(24).AdjustToContents();
                //worksheet.Column(25).AdjustToContents();
                //worksheet.Column(26).AdjustToContents();
                worksheet.Columns(1, 23).AdjustToContents();

                #endregion

                worksheetR.Columns().AdjustToContents();


                #region DISEÑO

                worksheet.Range("A1:Z1").Style
              .Font.SetFontSize(13)
              .Font.SetBold(true)
              .Font.SetFontColor(XLColor.Black)
              .Fill.SetBackgroundColor(XLColor.YellowGreen)
               .Border.BottomBorder = XLBorderStyleValues.Thick;


                #endregion
                #region DISEÑO

                worksheetR.Range("A1:Z1").Style
              .Font.SetFontSize(13)
              .Font.SetBold(true)
              .Font.SetFontColor(XLColor.Black)
              .Fill.SetBackgroundColor(XLColor.LightSteelBlue)
               .Border.BottomBorder = XLBorderStyleValues.Thick;


                #endregion

            }//--> fin for empresa

            workbook.Worksheet(1).Position = workbook.Worksheets.Count() + 1;
            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "Reporte3Porciento.xlsx";
            if (arrayIdEmpresa.Length == 1)
            {
                var itemEmpresa = listaEmpresa.FirstOrDefault(x => x.IdEmpresa == arrayIdEmpresa[0]);
                nombreArchivo = "Reporte3Porciento_"+itemEmpresa.RazonSocial+".xlsx";
            }

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);


            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

        }

        //public static string GenerarReporte3Porciento(int idUsuario, DateTime fecI, DateTime fecF, int idEmpresa = 0, string pathFolder = "", string pathDescarga = "")
        //{
        //    var sucursales = Getsucursales();
        //    List<Empresa> listaEmpresa;
        //    List<NOM_PeriodosPago> listaPeriodos;
        //    List<Sucursal> listaSucursal;
        //    List<NOM_Nomina_Detalle> listaDetalles;
        //    List<Empleado> listaEmpleados;
        //    List<C_NOM_Conceptos> listaConceptos;
        //    List<NOM_Nomina> listaNominas;
        //    List<Cliente> listaClientes;
        //    List<NOM_Finiquito> listaFiniquitos;
        //    List<NOM_Finiquito_Detalle> listaDetallesFiniquito;

        //    int[] arrayIdEmpresa;
        //    int[] arrayIdConceptos;
        //    int[] arrayIdPeriodos_asimilados;


        //    var fechaI = new DateTime(fecI.Year, fecI.Month, fecI.Day, 05, 00, 0);
        //    var fechaF = new DateTime(fecF.Year, fecF.Month, fecF.Day, 23, 50, 0);

        //    #region GET DATOS
        //    using (var context = new RHEntities())
        //    {
        //        listaEmpresa = context.Empresa.Where(x => x.RegistroPatronal != null).ToList();

        //        //Obtiene el array de empresas
        //        if (idEmpresa == 0) //todas las empresas
        //        {
        //            arrayIdEmpresa = listaEmpresa.Select(x => x.IdEmpresa).ToArray();
        //        }
        //        else //Por empresa
        //        {
        //            arrayIdEmpresa = new int[1];
        //            arrayIdEmpresa[0] = idEmpresa;
        //        }

        //        //Obtiene todos los periodos en el rango de fecha
        //        listaPeriodos = (from p in context.NOM_PeriodosPago
        //                         where p.Fecha_Fin >= fechaI && p.Fecha_Fin <= fechaF
        //                               // && p.IdEjercicio == idEjercicio
        //                               && p.Autorizado == true
        //                               && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
        //                               && p.SoloComplemento == false
        //                         //Que no sean solo complemento
        //                         select p).ToList();

        //        var arrayIdPeriodos = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();
        //        arrayIdPeriodos_asimilados = listaPeriodos.Where(w => w.IdTipoNomina == 16).Select(x => x.IdPeriodoPago).ToArray();

        //        //Busca las nominas en el rango de periodos
        //        //Busca los finiquitos

        //        listaNominas = (from n in context.NOM_Nomina
        //                        where arrayIdPeriodos.Contains(n.IdPeriodo) && sucursales.Contains(n.IdSucursal)
        //                        select n).ToList();

        //        listaFiniquitos = (from f in context.NOM_Finiquito
        //                           where arrayIdPeriodos.Contains(f.IdPeriodo) && sucursales.Contains(f.IdSucursal)
        //                           select f).ToList();

        //        //*** Filtra la nominas canceladas ***
        //        var arrayNominasCanceladas = (from nc in context.NOM_CFDI_Timbrado
        //                                      where nc.Cancelado == true
        //                                            && nc.IdNomina > 0
        //                                      select nc.IdNomina).ToArray();

        //        var arrayFiniquitosCancelados = (from nc in context.NOM_CFDI_Timbrado
        //                                         where nc.Cancelado == true
        //                                               && nc.IdFiniquito > 0
        //                                         select nc.IdFiniquito).ToArray();


        //        if (arrayNominasCanceladas.Length > 0)
        //        {
        //            listaNominas = (from nn in listaNominas
        //                            where !arrayNominasCanceladas.Contains(nn.IdNomina)
        //                            select nn).ToList();
        //        }

        //        if (arrayFiniquitosCancelados.Length > 0)
        //        {
        //            listaFiniquitos = (from nn in listaFiniquitos
        //                               where !arrayFiniquitosCancelados.Contains(nn.IdFiniquito)
        //                               select nn).ToList();
        //        }


        //        //DETALLES

        //        listaSucursal = (from s in context.Sucursal
        //                         where sucursales.Contains(s.IdSucursal)
        //                         select s).ToList();

        //        listaClientes = (from c in context.Cliente select c).ToList();

        //        var arrayIdNominas = (from n in listaNominas
        //                              select n.IdNomina).ToArray();

        //        //Busca los detalles de nominas 

        //        listaDetalles = (from nd in context.NOM_Nomina_Detalle
        //                         where arrayIdNominas.Contains(nd.IdNomina) && nd.ImpuestoSobreNomina > 0
        //                         select nd).ToList();

        //        //Buscamos los detalles de finiquito

        //        var arrayIdFiniquitos = (from f in listaFiniquitos
        //                                 select f.IdFiniquito).ToArray();

        //        listaDetallesFiniquito = (from df in context.NOM_Finiquito_Detalle
        //                                  where arrayIdFiniquitos.Contains(df.IdFiniquito) && df.ImpuestoSobreNomina > 0
        //                                  select df).ToList();


        //        //EMPLEADOS

        //        var empleadosnominas = (from n in listaNominas
        //                                select n.IdEmpleado).ToList();

        //        var empleadosfiniquitos = (from n in listaFiniquitos
        //                                   select n.IdEmpleado).ToList();

        //        empleadosnominas.AddRange(empleadosfiniquitos);
        //        //hacer distinct
        //        var arrayIdEmpleados = empleadosnominas.ToArray();

        //        arrayIdEmpleados = arrayIdEmpleados.Distinct().ToArray();

        //        listaEmpleados = (from e in context.Empleado
        //                          where arrayIdEmpleados.Contains(e.IdEmpleado)
        //                          select e).ToList();

        //        //CONCEPTOS

        //        //Agrupa por IdConcepto
        //        var listaGroupConceptos = listaDetalles.GroupBy(x => x.IdConcepto).ToList();

        //        // finiquito ---
        //        var listaGroupConceptosF = listaDetallesFiniquito.GroupBy(x => x.IdConcepto).ToList();



        //        ////Genera Array de Id de los conceptos
        //        //var arrayIdConceptosF = listaGroupConceptosF.Select(x => x.Key).ToArray();

        //        ////juntamos los dos array de conceptos
        //        //var z = new int[arrayIdConceptos.Length + arrayIdConceptosF.Length];
        //        //arrayIdConceptos.CopyTo(z, 0);
        //        //arrayIdConceptosF.CopyTo(z, arrayIdConceptos.Length);

        //        //arrayIdConceptos = z;

        //        //Genera Array de Id de los conceptos
        //        var listacn = listaGroupConceptos.Select(x => x.Key).ToList();
        //        var listacf = listaGroupConceptosF.Select(x => x.Key).ToList();

        //        listacn.AddRange(listacf);

        //        arrayIdConceptos = listacn.ToArray();
        //        arrayIdConceptos = arrayIdConceptos.Distinct().ToArray();

        //        //GET - Lista de Conceptos
        //        listaConceptos = (from conce in context.C_NOM_Conceptos
        //                          where arrayIdConceptos.Contains(conce.IdConcepto)
        //                          select conce).ToList();




        //    }

        //    #endregion

        //    //#region DOCUMENTO EXCEL - NOMINA ********************************************************
        //    var workbook = new XLWorkbook();



        //    //PRIMER FOR POR EMPRESA
        //    #region Rsumen Worksheet
        //    var worksheetR = workbook.Worksheets.Add("Resumen");
        //    worksheetR.Cell(1, 1).Value = "Empresa";
        //    worksheetR.Cell(1, 2).Value = "Cliente";
        //    worksheetR.Cell(1, 3).Value = "Num Emp";
        //    worksheetR.Cell(1, 4).Value = "Empleado";

        //    int colheadR = 5;
        //    foreach (var varId in arrayIdConceptos)
        //    {
        //        var itemConcepto = listaConceptos.FirstOrDefault(x => x.IdConcepto == varId);

        //        worksheetR.Cell(1, colheadR).Value = itemConcepto.DescripcionCorta;
        //        colheadR++;
        //    }
        //    worksheetR.Cell(1, colheadR).Value = "Total";
        //    colheadR++;
        //    worksheetR.Cell(1, colheadR).Value = "Total ISN";

        //    #endregion

        //    foreach (var idEmpresaArray in arrayIdEmpresa)
        //    {
        //        Random random = new Random();
        //        var itemEmpresa = listaEmpresa.FirstOrDefault(x => x.IdEmpresa == idEmpresaArray);

        //        //Crea la hoja del archivo
        //        var worksheet = workbook.Worksheets.Add(itemEmpresa.RazonSocial.Substring(0, 5) + itemEmpresa.IdEmpresa);

        //        worksheet.SheetView.Freeze(1, 3);

        //        #region HEADERS

        //        worksheet.Cell(1, 1).Value = "Cliente";
        //        worksheet.Cell(1, 2).Value = "Num Emp";
        //        worksheet.Cell(1, 3).Value = "Empleado";

        //        int colhead = 4;
        //        foreach (var varId in arrayIdConceptos)
        //        {
        //            var itemConcepto = listaConceptos.FirstOrDefault(x => x.IdConcepto == varId);

        //            worksheet.Cell(1, colhead).Value = itemConcepto.DescripcionCorta;
        //            colhead++;
        //        }
        //        worksheet.Cell(1, colhead).Value = "Total";
        //        colhead++;
        //        worksheet.Cell(1, colhead).Value = "Total ISN";

        //        #endregion


        //        decimal[] totalesConceptos = new decimal[arrayIdConceptos.Length + 2];

        //        int rowExcel = 2;
        //        int rowExcelr = worksheetR.RowsUsed().Count() + 1;
        //        worksheetR.Cell(rowExcelr, 1).Value = itemEmpresa.RazonSocial;
        //        worksheetR.Row(rowExcelr).Style.Font.SetFontSize(13).Font.SetBold(true);
        //        rowExcelr++;


        //        //COMBINACION NOMINAS - FINIQUITOS
        //        //Clasifica las nominas por IdEmpresaFiscal
        //        var listaNominaByEmpresa = (from n in listaNominas
        //                                    where n.IdEmpresaFiscal == idEmpresaArray || (n.IdEmpresaAsimilado == idEmpresaArray && arrayIdPeriodos_asimilados.Contains(n.IdPeriodo))
        //                                    //IdEmpresa
        //                                    select n).ToList();

        //        var listaFiniquitosByEmpresa = (from f in listaFiniquitos
        //                                        where f.IdEmpresaFiscal == idEmpresaArray
        //                                        select f).ToList();



        //        //Obtenemos el array de Sucursales
        //        var ns = listaNominaByEmpresa.Select(x => x.IdSucursal).ToList();

        //        var fs = listaFiniquitosByEmpresa.Select(x => x.IdSucursal).ToList();

        //        ns.AddRange(fs);

        //        var arrayIdSucursales = ns.ToArray();

        //        arrayIdSucursales = arrayIdSucursales.Distinct().ToArray();



        //        //SEGUNDO FOR POR SUCURSAL

        //        foreach (var itemSucursal in arrayIdSucursales)
        //        {
        //            //Limpia el array reiniciando a ceros
        //            Array.Clear(totalesConceptos, 0, totalesConceptos.Length);

        //            var listaNominaBySucursal = (from n in listaNominaByEmpresa
        //                                         where n.IdSucursal == itemSucursal //Sucursal
        //                                         select n).ToList();

        //            var listaFiniquitoBySucursal = (from f in listaFiniquitosByEmpresa
        //                                            where f.IdSucursal == itemSucursal //Sucursal
        //                                            select f).ToList();


        //            //TERCER FOR POR EMPLEADO
        //            var ne = listaNominaBySucursal.Select(x => x.IdEmpleado).ToList();
        //            var fe = listaFiniquitoBySucursal.Select(x => x.IdEmpleado).ToList();

        //            ne.AddRange(fe);

        //            var arrayIdEmpleadosFromSuc = ne.ToArray();

        //            arrayIdEmpleadosFromSuc = arrayIdEmpleadosFromSuc.Distinct().ToArray();

        //            #region FOR EMPLEADOS

        //            var strNombreCliente = "";

        //            foreach (var idEmpleadoArray in arrayIdEmpleadosFromSuc)
        //            {
        //                int columnExcel = 1;


        //                //Hace la sumatoria por concepto
        //                var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == idEmpleadoArray);

        //                var nombreEmpleado = $"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";

        //                //Obtiene la nominas del empleado
        //                var listaNominaByEmpleado = (from n in listaNominaBySucursal
        //                                             where n.IdEmpleado == idEmpleadoArray
        //                                             select n).ToList();

        //                //obtiene el array Id nominas
        //                var arrayIdNominaEmpleado = listaNominaByEmpleado.Select(x => x.IdNomina).ToArray();

        //                //Obtengo los detalles del array de nominas
        //                var listaDetalleByEmpleado = (from d in listaDetalles
        //                                              where arrayIdNominaEmpleado.Contains(d.IdNomina)
        //                                              select d).ToList();

        //                //Obtiene los finiquitos del empleado
        //                var listaFiniquitosByEmpleado = (from f in listaFiniquitoBySucursal
        //                                                 where f.IdEmpleado == idEmpleadoArray
        //                                                 select f).ToList();

        //                //obtiene el array Id Finiquitos
        //                var arrayIdFiniquitoEmpleado = listaFiniquitosByEmpleado.Select(x => x.IdFiniquito).ToArray();

        //                //Obtengo los detalles del array de nominas
        //                var listaDetalleFByEmpleado = (from d in listaDetallesFiniquito
        //                                               where arrayIdFiniquitoEmpleado.Contains(d.IdFiniquito)
        //                                               select d).ToList();



        //                var sucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemSucursal);
        //                var cliente = listaClientes.FirstOrDefault(x => x.IdCliente == sucursal.IdCliente);

        //                //Establece el nombre del cliente
        //                strNombreCliente = cliente.Nombre + " - " + sucursal.Ciudad;
        //                worksheet.Cell(rowExcel, columnExcel).Value = strNombreCliente;
        //                columnExcel++;
        //                columnExcel++;
        //                //Establece el nombre del empleado
        //                worksheet.Cell(rowExcel, columnExcel).Value = nombreEmpleado;
        //                columnExcel++;

        //                //CUARTO FOR POR CONCEPTOS
        //                decimal sumaIsn = 0;
        //                decimal totalPercepciones = 0;
        //                int indexConceptos = 0;
        //                foreach (var itemConcepto in arrayIdConceptos)
        //                {
        //                    //DATOS DE LA NOMINA
        //                    var listaConceptoN = (from c in listaDetalleByEmpleado
        //                                          where c.IdConcepto == itemConcepto
        //                                          select c).ToList();

        //                    var sumatoriaConcepto = listaConceptoN.Sum(x => x.Total);

        //                    sumaIsn += listaConceptoN.Sum(x => x.ImpuestoSobreNomina);


        //                    //DATOS DEL FINIQUITO
        //                    var listaConceptoF = (from c in listaDetalleFByEmpleado
        //                                          where c.IdConcepto == itemConcepto
        //                                          select c).ToList();


        //                    if (listaConceptoF.Count > 0)
        //                    {
        //                        var sumatoriaConceptoF = listaConceptoF.Sum(x => x.Total);
        //                        var sumaisnF = listaConceptoF.Sum(x => x.ImpuestoSobreNomina);

        //                        sumatoriaConcepto += sumatoriaConceptoF;
        //                        sumaIsn += sumaisnF;
        //                    }

        //                    totalPercepciones += sumatoriaConcepto;


        //                    //se agrega al total
        //                    totalesConceptos[indexConceptos] += sumatoriaConcepto;
        //                    indexConceptos++;

        //                    //lo guardamos al excel
        //                    worksheet.Cell(rowExcel, columnExcel).Value = sumatoriaConcepto;
        //                    columnExcel++;
        //                }

        //                worksheet.Cell(rowExcel, columnExcel).Value = totalPercepciones;
        //                columnExcel++;
        //                worksheet.Cell(rowExcel, columnExcel).Value = sumaIsn;

        //                totalesConceptos[indexConceptos] += totalPercepciones;
        //                indexConceptos++;
        //                totalesConceptos[indexConceptos] += sumaIsn;
        //                //*****************************************************************************

        //                rowExcel++;

        //            } //Fin del for empleado
        //            #endregion



        //            //Agregamos linea para los totales

        //            worksheet.Cell(rowExcel, 1).Value = strNombreCliente;
        //            worksheet.Cell(rowExcel, 2).Value = arrayIdEmpleadosFromSuc.Length;
        //            worksheet.Cell(rowExcel, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //            worksheetR.Cell(rowExcelr, 2).Value = strNombreCliente;
        //            worksheetR.Cell(rowExcelr, 3).Value = arrayIdEmpleadosFromSuc.Length;
        //            worksheetR.Cell(rowExcelr, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //            int indexTotal = 4;
        //            foreach (var itemtotal in totalesConceptos)
        //            {
        //                worksheet.Cell(rowExcel, indexTotal).Value = itemtotal;
        //                worksheetR.Cell(rowExcelr, indexTotal + 1).Value = itemtotal;
        //                indexTotal++;
        //            }

        //            worksheet.Range(rowExcel, 1, rowExcel, 26).Style
        //        .Font.SetFontSize(13)
        //        .Font.SetBold(true)
        //        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        //        .Font.SetFontColor(XLColor.Black)
        //        .Fill.SetBackgroundColor(XLColor.Amber);

        //            worksheetR.Range(rowExcelr, 1, rowExcelr, 14).Style.Font.SetFontColor(XLColor.Black);

        //            if (rowExcelr % 2 == 0)
        //            {
        //                worksheetR.Range(rowExcelr, 1, rowExcelr, 14).Style.Fill.SetBackgroundColor(XLColor.LightGray);
        //            }


        //            rowExcel++;
        //            rowExcelr++;

        //        } //Fin for sucursal

        //        #region AJUSTES DE COLUMNA
        //        worksheet.Column(1).AdjustToContents();
        //        worksheet.Column(2).AdjustToContents();
        //        worksheet.Column(3).AdjustToContents();
        //        worksheet.Column(4).AdjustToContents();
        //        worksheet.Column(5).AdjustToContents();
        //        worksheet.Column(6).AdjustToContents();
        //        worksheet.Column(7).AdjustToContents();
        //        worksheet.Column(8).AdjustToContents();
        //        worksheet.Column(9).AdjustToContents();
        //        worksheet.Column(10).AdjustToContents();
        //        worksheet.Column(11).AdjustToContents();
        //        worksheet.Column(12).AdjustToContents();
        //        worksheet.Column(13).AdjustToContents();
        //        worksheet.Column(14).AdjustToContents();
        //        worksheet.Column(15).AdjustToContents();
        //        worksheet.Column(16).AdjustToContents();
        //        worksheet.Column(17).AdjustToContents();
        //        worksheet.Column(18).AdjustToContents();
        //        worksheet.Column(19).AdjustToContents();
        //        worksheet.Column(20).AdjustToContents();
        //        worksheet.Column(21).AdjustToContents();
        //        worksheet.Column(22).AdjustToContents();
        //        worksheet.Column(23).AdjustToContents();
        //        worksheet.Column(24).AdjustToContents();
        //        worksheet.Column(25).AdjustToContents();
        //        worksheet.Column(26).AdjustToContents();

        //        #endregion

        //        worksheetR.Columns().AdjustToContents();


        //        #region DISEÑO

        //        worksheet.Range("A1:Z1").Style
        //      .Font.SetFontSize(13)
        //      .Font.SetBold(true)
        //      .Font.SetFontColor(XLColor.Black)
        //      .Fill.SetBackgroundColor(XLColor.YellowGreen)
        //       .Border.BottomBorder = XLBorderStyleValues.Thick;


        //        #endregion
        //        #region DISEÑO

        //        worksheetR.Range("A1:Z1").Style
        //      .Font.SetFontSize(13)
        //      .Font.SetBold(true)
        //      .Font.SetFontColor(XLColor.Black)
        //      .Fill.SetBackgroundColor(XLColor.LightSteelBlue)
        //       .Border.BottomBorder = XLBorderStyleValues.Thick;


        //        #endregion

        //    }//--> fin for empresa

        //    workbook.Worksheet(1).Position = workbook.Worksheets.Count() + 1;
        //    //Creamos el folder para guardar el archivo
        //    var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

        //    string nombreArchivo = "Reporte3Porciento_.xlsx";

        //    var fileName = pathUsuario + nombreArchivo;
        //    //Guarda el archivo
        //    workbook.SaveAs(fileName);


        //    return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

        //}

        public static string GenerarReporte3PorcientoSeparados(int idUsuario, DateTime fecI, DateTime fecF, int idEmpresa = 0, string pathFolder = "", string pathDescarga = "")
        {

            List<Empresa> listaEmpresa;
            List<NOM_PeriodosPago> listaPeriodos;
            List<Sucursal> listaSucursal;
            List<NOM_Nomina_Detalle> listaDetalles;
            List<Empleado> listaEmpleados;
            List<Empleado> listaEmpleadosF;
            List<C_NOM_Conceptos> listaConceptos;
            List<C_NOM_Conceptos> listaConceptosF;
            List<NOM_Nomina> listaNominas;
            List<Cliente> listaClientes;
            List<NOM_Finiquito> listaFiniquitos;
            List<NOM_Finiquito_Detalle> listaDetallesFiniquito;

            int[] arrayIdEmpresa;
            int[] arrayIdConceptos;
            int[] arrayIdConceptosF;

            var fechaI = new DateTime(fecI.Year, fecI.Month, fecI.Day, 05, 00, 0);
            var fechaF = new DateTime(fecF.Year, fecF.Month, fecF.Day, 23, 50, 0);

            #region GET DATOS
            using (var context = new RHEntities())
            {
                listaEmpresa = context.Empresa.ToList();

                //Obtiene el array de empresas
                if (idEmpresa == 0) //todas las empresas
                {
                    arrayIdEmpresa = listaEmpresa.Select(x => x.IdEmpresa).ToArray();
                }
                else //Por empresa
                {
                    arrayIdEmpresa = new int[1];
                    arrayIdEmpresa[0] = idEmpresa;
                }

                //Obtiene todos los periodos en el rango de fecha
                listaPeriodos = (from p in context.NOM_PeriodosPago
                                 where p.Fecha_Fin >= fechaI && p.Fecha_Fin <= fechaF
                                       // && p.IdEjercicio == idEjercicio
                                       && p.Autorizado == true
                                       && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
                                       && p.SoloComplemento == false
                                 //Que no sean solo complemento
                                 select p).ToList();

                var arrayIdPeriodos = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                //Busca las nominas en el rango de periodos
                //Busca los finiquitos

                listaNominas = (from n in context.NOM_Nomina
                                where arrayIdPeriodos.Contains(n.IdPeriodo)
                                && n.CFDICreado == true
                                select n).ToList();

                listaFiniquitos = (from f in context.NOM_Finiquito
                                   where arrayIdPeriodos.Contains(f.IdPeriodo)
                                   && f.CFDICreado == true
                                   select f).ToList();

                //*** Filtra la nominas canceladas ***
                //var arrayNominasCanceladas = (from nc in context.NOM_CFDI_Timbrado
                //                              where nc.Cancelado == true
                //                                    && nc.IdNomina > 0
                //                              select nc.IdNomina).ToArray();

                //var arrayFiniquitosCancelados = (from nc in context.NOM_CFDI_Timbrado
                //                                 where nc.Cancelado == true
                //                                       && nc.IdFiniquito > 0
                //                                 select nc.IdFiniquito).ToArray();


                //if (arrayNominasCanceladas.Length > 0)
                //{
                //    listaNominas = (from nn in listaNominas
                //                    where !arrayNominasCanceladas.Contains(nn.IdNomina)
                //                    select nn).ToList();
                //}

                //if (arrayFiniquitosCancelados.Length > 0)
                //{
                //    listaFiniquitos = (from nn in listaFiniquitos
                //                       where !arrayFiniquitosCancelados.Contains(nn.IdFiniquito)
                //                       select nn).ToList();
                //}


                //DETALLES

                listaSucursal = (from s in context.Sucursal
                                 select s).ToList();

                listaClientes = (from c in context.Cliente select c).ToList();

                var arrayIdNominas = (from n in listaNominas
                                      select n.IdNomina).ToArray();

                //Busca los detalles de nominas 

                listaDetalles = (from nd in context.NOM_Nomina_Detalle
                                 where arrayIdNominas.Contains(nd.IdNomina) && nd.ImpuestoSobreNomina > 0
                                 select nd).ToList();

                //Buscamos los detalles de finiquito

                var arrayIdFiniquitos = (from f in listaFiniquitos
                                         select f.IdFiniquito).ToArray();

                listaDetallesFiniquito = (from df in context.NOM_Finiquito_Detalle
                                          where arrayIdFiniquitos.Contains(df.IdFiniquito) && df.ImpuestoSobreNomina > 0
                                          select df).ToList();


                //EMPLEADOS

                var empleadosnominas = (from n in listaNominas
                                        select n.IdEmpleado).ToList();

                var empleadosfiniquitos = (from n in listaFiniquitos
                                           select n.IdEmpleado).ToList();

                //empleadosnominas.AddRange(empleadosfiniquitos);
                //hacer distinct
                var arrayIdEmpleados = empleadosnominas.ToArray();

                arrayIdEmpleados = arrayIdEmpleados.Distinct().ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();


                var arrayIdEmpleadosF = empleadosfiniquitos.ToArray();

                arrayIdEmpleadosF = arrayIdEmpleadosF.Distinct().ToArray();


                listaEmpleadosF = (from e in context.Empleado
                                   where arrayIdEmpleadosF.Contains(e.IdEmpleado)
                                   select e).ToList();

                //CONCEPTOS

                //Agrupa por IdConcepto
                var listaGroupConceptos = listaDetalles.GroupBy(x => x.IdConcepto).ToList();

                // finiquito ---
                var listaGroupConceptosF = listaDetallesFiniquito.GroupBy(x => x.IdConcepto).ToList();



                ////Genera Array de Id de los conceptos
                //var arrayIdConceptosF = listaGroupConceptosF.Select(x => x.Key).ToArray();

                ////juntamos los dos array de conceptos
                //var z = new int[arrayIdConceptos.Length + arrayIdConceptosF.Length];
                //arrayIdConceptos.CopyTo(z, 0);
                //arrayIdConceptosF.CopyTo(z, arrayIdConceptos.Length);

                //arrayIdConceptos = z;

                //Genera Array de Id de los conceptos
                var listacn = listaGroupConceptos.Select(x => x.Key).ToList();
                var listacf = listaGroupConceptosF.Select(x => x.Key).ToList();

                //  listacn.AddRange(listacf);

                arrayIdConceptos = listacn.ToArray();
                arrayIdConceptos = arrayIdConceptos.Distinct().ToArray();

                Array.Sort(arrayIdConceptos);

                //GET - Lista de Conceptos Nom
                listaConceptos = (from conce in context.C_NOM_Conceptos
                                  where arrayIdConceptos.Contains(conce.IdConcepto)
                                  select conce).ToList();



                arrayIdConceptosF = listacf.ToArray();
                arrayIdConceptosF = arrayIdConceptosF.Distinct().ToArray();

                Array.Sort(arrayIdConceptosF);
                //GET - Lista de Conceptos Fin
                listaConceptosF = (from conce in context.C_NOM_Conceptos
                                   where arrayIdConceptosF.Contains(conce.IdConcepto)
                                   select conce).ToList();

            }

            #endregion

            //#region DOCUMENTO EXCEL - NOMINA ********************************************************
            var workbook = new XLWorkbook();

            #region FOR EMPRESA
            //PRIMER FOR POR EMPRESA
            foreach (var idEmpresaArray in arrayIdEmpresa)
            {
                var itemEmpresa = listaEmpresa.FirstOrDefault(x => x.IdEmpresa == idEmpresaArray);

                //Crea la hoja del archivo
                var worksheet = workbook.Worksheets.Add(itemEmpresa.RazonSocial.Substring(0, 5));
                worksheet.SheetView.Freeze(1, 3);

                #region NOMINA

                #region HEADERS

                worksheet.Cell(1, 1).Value = "Cliente";
                worksheet.Cell(1, 2).Value = "Num Emp";
                worksheet.Cell(1, 3).Value = "Empleado";

                int colhead = 4;
                foreach (var varId in arrayIdConceptos)
                {
                    var itemConcepto = listaConceptos.FirstOrDefault(x => x.IdConcepto == varId);

                    worksheet.Cell(1, colhead).Value = itemConcepto.DescripcionCorta;
                    colhead++;
                }
                worksheet.Cell(1, colhead).Value = "Total";
                colhead++;
                worksheet.Cell(1, colhead).Value = "Total ISN";

                #endregion

                decimal[] totalesConceptosN = new decimal[arrayIdConceptos.Length + 2];

                int rowExcelN = 2;

                //COMBINACION NOMINAS - FINIQUITOS
                //Clasifica las nominas por IdEmpresaFiscal
                var listaNominaByEmpresa = (from n in listaNominas
                                            where n.IdEmpresaFiscal == idEmpresaArray
                                            //IdEmpresa
                                            select n).ToList();

                var listaFiniquitosByEmpresaF = (from f in listaFiniquitos
                                                 where f.IdEmpresaFiscal == idEmpresaArray
                                                 select f).ToList();

                //Obtenemos el array de Sucursales
                var ns = listaNominaByEmpresa.Select(x => x.IdSucursal).ToList();

                var arrayIdSucursales = ns.ToArray();
                arrayIdSucursales = arrayIdSucursales.Distinct().ToArray();

                #region FOR SUCURSAL
                foreach (var itemSucursal in arrayIdSucursales)
                {
                    //Limpia el array reiniciando a ceros
                    Array.Clear(totalesConceptosN, 0, totalesConceptosN.Length);

                    var listaNominaBySucursal = (from n in listaNominaByEmpresa
                                                 where n.IdSucursal == itemSucursal //Sucursal
                                                 select n).ToList();



                    //TERCER FOR POR EMPLEADO
                    var ne = listaNominaBySucursal.Select(x => x.IdEmpleado).ToList();
                    var arrayIdEmpleadosFromSuc = ne.ToArray();

                    arrayIdEmpleadosFromSuc = arrayIdEmpleadosFromSuc.Distinct().ToArray();

                    #region FOR EMPLEADOS

                    var strNombreCliente = "";

                    foreach (var idEmpleadoArray in arrayIdEmpleadosFromSuc)
                    {
                        int columnExcel = 1;


                        //Hace la sumatoria por concepto
                        var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == idEmpleadoArray);

                        var nombreEmpleado = $"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";

                        //Obtiene la nominas del empleado
                        var listaNominaByEmpleado = (from n in listaNominaBySucursal
                                                     where n.IdEmpleado == idEmpleadoArray
                                                     select n).ToList();

                        //obtiene el array Id nominas
                        var arrayIdNominaEmpleado = listaNominaByEmpleado.Select(x => x.IdNomina).ToArray();

                        //Obtengo los detalles del array de nominas
                        var listaDetalleByEmpleado = (from d in listaDetalles
                                                      where arrayIdNominaEmpleado.Contains(d.IdNomina)
                                                      select d).ToList();


                        var sucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemSucursal);
                        var cliente = listaClientes.FirstOrDefault(x => x.IdCliente == sucursal.IdCliente);

                        //Establece el nombre del cliente
                        strNombreCliente = cliente.Nombre + " - " + sucursal.Ciudad;
                        worksheet.Cell(rowExcelN, columnExcel).Value = strNombreCliente;
                        columnExcel++;
                        columnExcel++;
                        //Establece el nombre del empleado
                        worksheet.Cell(rowExcelN, columnExcel).Value = nombreEmpleado;
                        columnExcel++;

                        //CUARTO FOR POR CONCEPTOS
                        decimal sumaIsn = 0;
                        decimal totalPercepciones = 0;
                        int indexConceptos = 0;
                        foreach (var itemConcepto in listaConceptos)
                        {
                            //DATOS DE LA NOMINA
                            var listaConceptoN = (from c in listaDetalleByEmpleado
                                                  where c.IdConcepto == itemConcepto.IdConcepto
                                                  select c).ToList();

                            var sumatoriaConcepto = listaConceptoN.Sum(x => x.Total);

                            sumaIsn += listaConceptoN.Sum(x => x.ImpuestoSobreNomina);


                            totalPercepciones += sumatoriaConcepto;


                            //se agrega al total
                            totalesConceptosN[indexConceptos] += sumatoriaConcepto;
                            indexConceptos++;

                            //lo guardamos al excel
                            worksheet.Cell(rowExcelN, columnExcel).Value = sumatoriaConcepto;
                            columnExcel++;
                        }

                        worksheet.Cell(rowExcelN, columnExcel).Value = totalPercepciones;
                        columnExcel++;
                        worksheet.Cell(rowExcelN, columnExcel).Value = sumaIsn;

                        totalesConceptosN[indexConceptos] += totalPercepciones;
                        indexConceptos++;
                        totalesConceptosN[indexConceptos] += sumaIsn;
                        //*****************************************************************************

                        rowExcelN++;

                    } //Fin del for empleado
                    #endregion

                    //Agregamos linea para los totales
                    worksheet.Cell(rowExcelN, 1).Value = strNombreCliente;
                    worksheet.Cell(rowExcelN, 2).Value = arrayIdEmpleadosFromSuc.Length;
                    worksheet.Cell(rowExcelN, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    int indexTotal = 4;
                    foreach (var itemtotal in totalesConceptosN)
                    {
                        worksheet.Cell(rowExcelN, indexTotal).Value = itemtotal;
                        indexTotal++;
                    }

                    worksheet.Range(rowExcelN, 1, rowExcelN, 26).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);


                    rowExcelN++; // worksheet.Cell(rowExcel, columnExcel).Value = sumaIsn;

                } //Fin for sucursal
                #endregion

                #endregion

                #region AJUSTES DE COLUMNA
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
                worksheet.Column(15).AdjustToContents();
                worksheet.Column(16).AdjustToContents();
                worksheet.Column(17).AdjustToContents();
                worksheet.Column(18).AdjustToContents();
                worksheet.Column(19).AdjustToContents();
                worksheet.Column(20).AdjustToContents();
                worksheet.Column(21).AdjustToContents();
                worksheet.Column(22).AdjustToContents();
                worksheet.Column(23).AdjustToContents();
                worksheet.Column(24).AdjustToContents();
                worksheet.Column(25).AdjustToContents();
                worksheet.Column(26).AdjustToContents();

                #endregion

                #region DISEÑO

                worksheet.Range("A1:Z1").Style
              .Font.SetFontSize(13)
              .Font.SetBold(true)
              .Font.SetFontColor(XLColor.Black)
              .Fill.SetBackgroundColor(XLColor.YellowGreen)
               .Border.BottomBorder = XLBorderStyleValues.Thick;


                #endregion



                //Crea la hoja del archivo
                var worksheetF = workbook.Worksheets.Add(itemEmpresa.RazonSocial.Substring(0, 5) + " Finiquito");
                worksheetF.SheetView.Freeze(1, 3);

                #region FINIQUITOS


                #region HEADERS

                worksheetF.Cell(1, 1).Value = "Cliente";
                worksheetF.Cell(1, 2).Value = "Num Emp";
                worksheetF.Cell(1, 3).Value = "Empleado";

                int colheadF = 4;
                foreach (var varId in arrayIdConceptosF)
                {
                    var itemConceptoF = listaConceptosF.FirstOrDefault(x => x.IdConcepto == varId);
                    worksheetF.Cell(1, colheadF).Value = itemConceptoF.DescripcionCorta;
                    colheadF++;
                }
                worksheetF.Cell(1, colheadF).Value = "Total";
                colheadF++;
                worksheetF.Cell(1, colheadF).Value = "Total ISN";

                #endregion

                decimal[] totalesConceptos = new decimal[arrayIdConceptosF.Length + 2];

                int rowExcel = 2;

                //COMBINACION NOMINAS - FINIQUITOS
                //Clasifica las nominas por IdEmpresaFiscal
                var listaFiniquitosByEmpresa = (from f in listaFiniquitos
                                                where f.IdEmpresaFiscal == idEmpresaArray
                                                select f).ToList();

                //Obtenemos el array de Sucursales

                var fs = listaFiniquitosByEmpresa.Select(x => x.IdSucursal).ToList();
                // ns.AddRange(fs);

                var arrayIdSucursalesF = fs.ToArray();
                arrayIdSucursalesF = arrayIdSucursalesF.Distinct().ToArray();

                #region FOR SUCURSAL
                foreach (var itemSucursal in arrayIdSucursalesF)
                {
                    //Limpia el array reiniciando a ceros
                    Array.Clear(totalesConceptos, 0, totalesConceptos.Length);



                    var listaFiniquitoBySucursal = (from f in listaFiniquitosByEmpresa
                                                    where f.IdSucursal == itemSucursal //Sucursal
                                                    select f).ToList();


                    //TERCER FOR POR EMPLEADO
                    var fe = listaFiniquitoBySucursal.Select(x => x.IdEmpleado).ToList();

                    var arrayIdEmpleadosFromSuc = fe.ToArray();

                    arrayIdEmpleadosFromSuc = arrayIdEmpleadosFromSuc.Distinct().ToArray();

                    #region FOR EMPLEADOS

                    var strNombreCliente = "";

                    foreach (var idEmpleadoArray in arrayIdEmpleadosFromSuc)
                    {
                        int columnExcel = 1;


                        //Hace la sumatoria por concepto
                        var itemEmpleado = listaEmpleadosF.FirstOrDefault(x => x.IdEmpleado == idEmpleadoArray);

                        var nombreEmpleado = $"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";

                        //Obtiene los finiquitos del empleado
                        var listaFiniquitosByEmpleado = (from f in listaFiniquitoBySucursal
                                                         where f.IdEmpleado == idEmpleadoArray
                                                         select f).ToList();

                        //obtiene el array Id Finiquitos
                        var arrayIdFiniquitoEmpleado = listaFiniquitosByEmpleado.Select(x => x.IdFiniquito).ToArray();

                        //Obtengo los detalles del array de nominas
                        var listaDetalleFByEmpleado = (from d in listaDetallesFiniquito
                                                       where arrayIdFiniquitoEmpleado.Contains(d.IdFiniquito)
                                                       select d).ToList();



                        var sucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemSucursal);
                        var cliente = listaClientes.FirstOrDefault(x => x.IdCliente == sucursal.IdCliente);

                        //Establece el nombre del cliente
                        strNombreCliente = cliente.Nombre + " - " + sucursal.Ciudad;
                        worksheetF.Cell(rowExcel, columnExcel).Value = strNombreCliente;
                        columnExcel++;
                        columnExcel++;
                        //Establece el nombre del empleado
                        worksheetF.Cell(rowExcel, columnExcel).Value = nombreEmpleado;
                        columnExcel++;

                        //CUARTO FOR POR CONCEPTOS
                        decimal sumaIsn = 0;
                        decimal totalPercepciones = 0;
                        int indexConceptos = 0;
                        foreach (var itemConcepto in listaConceptosF)
                        {

                            decimal sumatoriaConcepto = 0;

                            //DATOS DEL FINIQUITO
                            var listaConceptoF = (from c in listaDetalleFByEmpleado
                                                  where c.IdConcepto == itemConcepto.IdConcepto
                                                  select c).ToList();


                            if (listaConceptoF.Count > 0)
                            {
                                var sumatoriaConceptoF = listaConceptoF.Sum(x => x.Total);
                                var sumaisnF = listaConceptoF.Sum(x => x.ImpuestoSobreNomina);

                                sumatoriaConcepto += sumatoriaConceptoF;
                                sumaIsn += sumaisnF;
                            }

                            totalPercepciones += sumatoriaConcepto;


                            //se agrega al total
                            totalesConceptos[indexConceptos] += sumatoriaConcepto;
                            indexConceptos++;

                            //lo guardamos al excel
                            worksheetF.Cell(rowExcel, columnExcel).Value = sumatoriaConcepto;
                            columnExcel++;
                        }

                        worksheetF.Cell(rowExcel, columnExcel).Value = totalPercepciones;
                        columnExcel++;
                        worksheetF.Cell(rowExcel, columnExcel).Value = sumaIsn;

                        totalesConceptos[indexConceptos] += totalPercepciones;
                        indexConceptos++;
                        totalesConceptos[indexConceptos] += sumaIsn;
                        //*****************************************************************************

                        rowExcel++;

                    } //Fin del for empleado
                    #endregion



                    //Agregamos linea para los totales

                    worksheetF.Cell(rowExcel, 1).Value = strNombreCliente;
                    worksheetF.Cell(rowExcel, 2).Value = arrayIdEmpleadosFromSuc.Length;
                    worksheetF.Cell(rowExcel, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    int indexTotal = 4;
                    foreach (var itemtotal in totalesConceptos)
                    {
                        worksheetF.Cell(rowExcel, indexTotal).Value = itemtotal;
                        indexTotal++;
                    }

                    worksheetF.Range(rowExcel, 1, rowExcel, 26).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);


                    rowExcel++; // worksheet.Cell(rowExcel, columnExcel).Value = sumaIsn;

                } //Fin for sucursal
                #endregion

                #endregion



                #region AJUSTES DE COLUMNA
                worksheetF.Column(1).AdjustToContents();
                worksheetF.Column(2).AdjustToContents();
                worksheetF.Column(3).AdjustToContents();
                worksheetF.Column(4).AdjustToContents();
                worksheetF.Column(5).AdjustToContents();
                worksheetF.Column(6).AdjustToContents();
                worksheetF.Column(7).AdjustToContents();
                worksheetF.Column(8).AdjustToContents();
                worksheetF.Column(9).AdjustToContents();
                worksheetF.Column(10).AdjustToContents();
                worksheetF.Column(11).AdjustToContents();
                worksheetF.Column(12).AdjustToContents();
                worksheetF.Column(13).AdjustToContents();
                worksheetF.Column(14).AdjustToContents();
                worksheetF.Column(15).AdjustToContents();
                worksheetF.Column(16).AdjustToContents();
                worksheetF.Column(17).AdjustToContents();
                worksheetF.Column(18).AdjustToContents();
                worksheetF.Column(19).AdjustToContents();
                worksheetF.Column(20).AdjustToContents();
                worksheetF.Column(21).AdjustToContents();
                worksheetF.Column(22).AdjustToContents();
                worksheetF.Column(23).AdjustToContents();
                worksheetF.Column(24).AdjustToContents();
                worksheetF.Column(25).AdjustToContents();
                worksheetF.Column(26).AdjustToContents();

                #endregion

                #region DISEÑO

                worksheetF.Range("A1:Z1").Style
              .Font.SetFontSize(13)
              .Font.SetBold(true)
              .Font.SetFontColor(XLColor.Black)
              .Fill.SetBackgroundColor(XLColor.YellowGreen)
               .Border.BottomBorder = XLBorderStyleValues.Thick;


                #endregion

            }//--> fin for empresa
            #endregion

            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "Reporte3Porciento_.xlsx";

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);


            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;



        }

        //corregido de fechas 
        public static string GenerarReporteGeneralAlertas(int idUsuario, DateTime fechaI, DateTime fechaF, string NombreEmpresa, string ruta, int tipoAlerta, int idEmpresa, string pathDescarga)
        {
            var sucursales = Getsucursales();
            int i = 4;
            bool genero_info = false;
            var newruta = Utils.ValidarFolderUsuario(idUsuario, ruta);
            var wb = new XLWorkbook();
            var nombreHoja = "REPORTE";
            var nombreArchivo = "";
            NombreEmpresa = NombreEmpresa.Length > 15 ? NombreEmpresa.Substring(0, 15) : NombreEmpresa; 

            //SE CAMBIA EL FORMATO DE LA FECHA EN LAS BUSQUEDAS 13/06/2018
            //var fechaInicial = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 05, 00, 0);
            //var fechaFinal = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 50, 0);

            switch (tipoAlerta)
            {
                case 1:
                    nombreHoja = "Alertas ALTAS";
                    nombreArchivo = "Alertas_Altas_" + NombreEmpresa + ".xlsx";
                    break;
                case 2:
                    nombreHoja = "Alertas BAJAS";
                    nombreArchivo = "Alertas_Bajas_" + NombreEmpresa + ".xlsx";
                    break;
                case 3:
                    nombreHoja = "Alertas CAMBIO SALARIO";
                    nombreArchivo = "Alertas_CambioSalario_" + NombreEmpresa + ".xlsx";
                    break;
                case 4:
                    nombreHoja = "Alertas CAMBIO DE EMPRESA";
                    nombreArchivo = "Alertas_CambioEmpresa_" + NombreEmpresa + ".xlsx";
                    break;
                case 5:
                    nombreHoja = "Alertas REINGRESO";
                    nombreArchivo = "Alertas_Reingreso_" + NombreEmpresa + ".xlsx";
                    break;
            }

            newruta = newruta + nombreArchivo;

            var ws = wb.Worksheets.Add(nombreHoja);

            using (var context = new RHEntities())
            {

                var listaEmpleados = (from e in context.Empleado
                                      join ec in context.Empleado_Contrato on e.IdEmpleado equals ec.IdEmpleado
                                      join s in context.Sucursal on e.IdSucursal equals s.IdSucursal
                                      join c in context.Cliente on s.IdCliente equals c.IdCliente
                                      where ec.IdEmpresaFiscal == idEmpresa && sucursales.Contains(s.IdSucursal)
                                      orderby ec.FechaIMSS descending
                                      select new { e.IdEmpleado, e.IdSucursal, e.NSS, Nombre = (e.APaterno + " " + e.AMaterno + " " + e.Nombres), ec.FechaIMSS, ec.BajaIMSS, ec.SDI, e.RFC, e.CURP, Cliente = c.Nombre }).ToList();


                var notificacion = new List<RH.Entidades.Notificaciones>();

                switch (tipoAlerta)
                {
                    case 0:
                        //notificacion = context.Notificaciones.Where(x => x.Fecha >= fechaInicial && x.Fecha <= fechaFinal && x.IdTipo == 2 && x.IdTipo == 3 || x.IdTipo == 8).ToList();
                        notificacion = context.Notificaciones.Where(x => DbFunctions.TruncateTime(x.Fecha) >= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(x.Fecha) <= DbFunctions.TruncateTime(fechaF) && x.IdTipo == 2 && x.IdTipo == 3 || x.IdTipo == 8).ToList();

                        break;
                    case 1:
                        #region Reporte alta
                        // Se genera encabezado
                        ws = encabezadoExcel(ws, fechaI, fechaF);
                        //Se elimina columnas que no se ocuparan
                        ws.Column(4).Delete();
                        ws.Column(4).Delete();
                        ws.Column(5).Delete();
                        ws.Column(6).Delete();

                        ws.Cell("H3").Value = "Fecha Limite";
                        ws.Cell("H3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("H3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                        var empleadoAlta = listaEmpleados.Where(x => x.FechaIMSS >= fechaI && x.FechaIMSS <= fechaF);
                        if (empleadoAlta.Count() > 0)
                        {
                            genero_info = true;
                            foreach (var dato in empleadoAlta)
                            {
                                ws.Cell("A" + i).Value = dato.NSS;
                                ws.Cell("B" + i).Value = dato.Nombre;
                                ws.Cell("C" + i).Value = dato.FechaIMSS;
                                ws.Cell("D" + i).Value = dato.SDI;
                                ws.Cell("E" + i).Value = dato.Cliente;
                                ws.Cell("F" + i).Value = dato.RFC;
                                ws.Cell("G" + i).Value = dato.CURP;
                                ws.Cell("H" + i).Value = Convert.ToDateTime(dato.FechaIMSS).AddDays(5);
                                i++;

                            }
                        }


                        #endregion
                        break;
                    case 2:
                        #region Reporte Baja
                        // Se genera encabezado
                        ws = encabezadoExcel(ws, fechaI, fechaF);
                        //Se elimina columnas que no se ocuparan
                        ws.Column(3).Delete();
                        ws.Range("D:F").Delete(XLShiftDeletedCells.ShiftCellsLeft);
                        ws.Range("E:H").Delete(XLShiftDeletedCells.ShiftCellsLeft);

                        ws.Cell("E3").Value = "Fecha Limite";
                        ws.Cell("E3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("E3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                        var empleadoBaja = listaEmpleados.Where(x => x.BajaIMSS >= fechaI && x.BajaIMSS <= fechaF);
                        if (empleadoBaja.Count() > 0)
                        {
                            genero_info = true;
                            foreach (var dato in empleadoBaja)
                            {
                                ws.Cell("A" + i).Value = dato.NSS;
                                ws.Cell("B" + i).Value = dato.Nombre;
                                ws.Cell("C" + i).Value = dato.BajaIMSS;
                                ws.Cell("D" + i).Value = dato.Cliente;
                                ws.Cell("E" + i).Value = Convert.ToDateTime(dato.BajaIMSS).AddDays(5);

                                i++;
                            }
                        }
                        break;
                    #endregion
                    case 3: //case de los cambios de salario por cumple IMSS y cambio normal
                        var listaNSS = new List<string>();
                        var listaNSS4 = new List<string>();
                        #region Reporte Cambio Salarios
                        // Se genera encabezado
                        ws = encabezadoExcel(ws, fechaI, fechaF);
                        ws.Column(4).Delete();
                        ws.Column(4).Delete();
                        var idempkardex_cumple = context.Kardex.Where(k => k.Tipo == 11 && k.Fecha.Year == DateTime.Now.Year).Select(k => k.IdEmpleado).ToList();

                        var empleadoCumpleimss = listaEmpleados.Where(x => idempkardex_cumple.Contains(x.IdEmpleado)
                                                && (Convert.ToDateTime(x.FechaIMSS).AddYears(fechaI.Year - Convert.ToDateTime(x.FechaIMSS).Year) >= fechaI
                                                && Convert.ToDateTime(x.FechaIMSS).AddYears(fechaI.Year - Convert.ToDateTime(x.FechaIMSS).Year) <= fechaF
                                                && Convert.ToDateTime(x.FechaIMSS).Year < fechaI.Year)
                                                && x.BajaIMSS == null);





                        var empleadosSDI = (from e in context.Empleado
                                            join ec in context.Empleado_Contrato on e.IdEmpleado equals ec.IdEmpleado
                                            join s in context.Sucursal on e.IdSucursal equals s.IdSucursal
                                            join c in context.Cliente on s.IdCliente equals c.IdCliente
                                            join k in context.Kardex on e.IdEmpleado equals k.IdEmpleado
                                            where ec.IdEmpresaFiscal == idEmpresa && k.Fecha >= fechaI && k.Fecha <= fechaF && k.Tipo == 12 && sucursales.Contains(s.IdSucursal)
                                            orderby ec.FechaIMSS descending
                                            select new { e.IdEmpleado, e.NSS, Nombre = (e.APaterno + " " + e.AMaterno + " " + e.Nombres), k.Fecha, ec.SDI, e.RFC, e.CURP, Cliente = c.Nombre }).ToList();


                        if (empleadoCumpleimss.Count() > 0)
                        {
                            genero_info = true;
                            foreach (var dato in empleadoCumpleimss)
                            {
                                ws.Cell("A" + i).Value = dato.NSS;
                                ws.Cell("B" + i).Value = dato.Nombre;
                                ws.Cell("C" + i).Value = Convert.ToDateTime(dato.FechaIMSS).AddYears(fechaI.Year - Convert.ToDateTime(dato.FechaIMSS).Year);
                                ws.Cell("D" + i).Value = dato.SDI;
                                ws.Cell("F" + i).Value = NombreEmpresa;
                                ws.Cell("G" + i).Value = dato.Cliente;
                                ws.Cell("H" + i).Value = "Cumple Imss";
                                ws.Cell("I" + i).Value = dato.RFC;
                                ws.Cell("J" + i).Value = dato.CURP;
                                i++;
                            }
                        }


                        if (empleadosSDI.Count() > 0)
                        {
                            genero_info = true;
                            foreach (var dato in empleadosSDI)
                            {
                                ws.Cell("A" + i).Value = dato.NSS;
                                ws.Cell("B" + i).Value = dato.Nombre;
                                ws.Cell("C" + i).Value = dato.Fecha.ToString("dd/MM/yyyy");
                                ws.Cell("D" + i).Value = dato.SDI;
                                ws.Cell("F" + i).Value = NombreEmpresa;
                                ws.Cell("G" + i).Value = dato.Cliente;
                                ws.Cell("H" + i).Value = "SDI";
                                ws.Cell("I" + i).Value = dato.RFC;
                                ws.Cell("J" + i).Value = dato.CURP;
                                i++;
                            }
                        }

                        break;


                    #endregion

                    case 4:
                        #region Reporte Cambio Empresa
                        //Obtener la lista de notificaciones de tipo 6
                        var listaEmpresaCam = new List<modelCambioEmp>();
                        var listaidEmpleados = new List<int>();
                        //var lnotificacion = context.Notificaciones.Where(x => x.Fecha > fechaInicial && x.Fecha < fechaFinal && x.IdTipo == 6 && x.Cuerpo.Contains(NombreEmpresa)).ToList();
                        var lnotificacion = context.Notificaciones.Where(x => DbFunctions.TruncateTime(x.Fecha) > DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(x.Fecha) < DbFunctions.TruncateTime(fechaF) && x.IdTipo == 6 && x.Cuerpo.Contains(NombreEmpresa)).ToList();
                        // Se genera encabezado
                        ws = encabezadoExcel(ws, fechaI, fechaF);
                        ws.Cell("C3").Value = "Fecha Cambio";
                        ws.Cell("C3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("C3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                        ws.Range("D:E").Delete(XLShiftDeletedCells.ShiftCellsLeft);
                        ws.Column(5).Delete();

                        #region Columnas Extras Encabezado
                        ws.Cell("E3").Value = "RFC";
                        ws.Cell("E3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("E3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                        ws.Cell("F3").Value = "CURP";
                        ws.Cell("F3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("F3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                        ws.Cell("G3").Value = "CLIENTE";
                        ws.Cell("G3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("G3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                        ws.Cell("H3").Value = "Empresa Fiscal Anterior";
                        ws.Cell("H3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("H3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                        ws.Cell("I3").Value = "Empresa Fiscal Nueva";
                        ws.Cell("I3").Style
                              .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                              .Font.SetBold();
                        ws.Cell("I3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);


                        #endregion

                        //Crear el contenido
                        foreach (var n in lnotificacion)
                        {

                            string cadena = n.Cuerpo;

                            cadena = cadena.Replace("<ul class='collection'>", "");
                            cadena = cadena.Replace("<li class='collection-item'><b>", "");
                            cadena = cadena.Replace("</b> <span class='secondary-content'>", "");
                            cadena = cadena.Replace("</b><span class='secondary-content'>", "");
                            cadena = cadena.Replace("</span></li>", ":");
                            cadena = cadena.Replace("</ul>", "");
                            var array = cadena.Split(':');
                            listaidEmpleados.Add(Convert.ToInt32(array[1]));
                            listaEmpresaCam.Add(new modelCambioEmp { idEmpleado = Convert.ToInt32(array[1]), empresaO = array[23], empresaN = array[31], fechaCambio = n.Fecha.ToString("dd/MM/yyyy") });

                        }


                        var empleadosCE = listaEmpleados.Where(x => listaidEmpleados.Contains(x.IdEmpleado));
                        if (empleadosCE.Count() > 0)
                        {
                            genero_info = true;
                            foreach (var dato in empleadosCE)
                            {
                                ws.Cell("A" + i).Value = dato.NSS;
                                ws.Cell("B" + i).Value = dato.Nombre;
                                ws.Cell("C" + i).Value = listaEmpresaCam.Where(x => x.idEmpleado == dato.IdEmpleado).Select(x => x.fechaCambio);
                                ws.Cell("D" + i).Value = dato.SDI;
                                ws.Cell("E" + i).Value = dato.RFC;
                                ws.Cell("F" + i).Value = dato.CURP;
                                ws.Cell("G" + i).Value = dato.Cliente;
                                ws.Cell("H" + i).Value = listaEmpresaCam.Where(x => x.idEmpleado == dato.IdEmpleado).Select(x => x.empresaO);
                                ws.Cell("I" + i).Value = listaEmpresaCam.Where(x => x.idEmpleado == dato.IdEmpleado).Select(x => x.empresaN);


                                i++;
                            }
                        }
                        break;
                    #endregion

                    case 5:
                        #region Reporte Reingreso
                        notificacion = context.Notificaciones.Where(x => x.Fecha.Year >= fechaI.Year && x.Fecha.Year <= fechaF.Year && x.IdTipo == 7 && sucursales.Contains(x.IdSucursal)).ToList();
                        var listaidEmp = new List<int>();
                        // Se genera encabezado
                        ws = encabezadoExcel(ws, fechaI, fechaF);
                        //Se elimina columnas que no se ocuparan
                        ws.Column(4).Delete();
                        ws.Column(4).Delete();
                        ws.Column(5).Delete();
                        foreach (var n in notificacion)
                        {
                            if (n.Cuerpo.Contains(NombreEmpresa))
                            {
                                var arrayLista = new List<string>();
                                string cadena = n.Cuerpo;

                                cadena = cadena.Replace("<ul class='collection'>", "");
                                cadena = cadena.Replace("<li class='collection-item'><b>", "");
                                cadena = cadena.Replace("</b> <span class='secondary-content'>", "");
                                cadena = cadena.Replace("</span></li>", ":");
                                cadena = cadena.Replace("</ul>", "");
                                var array = cadena.Split(':');
                                listaidEmp.Add(Convert.ToInt32(array[1]));
                            }

                        }
                        //var empleadoReingreso = listaEmpleados.Where(x => listaidEmp.Contains(x.IdEmpleado) && x.FechaIMSS >= fechaInicial && x.FechaIMSS <= fechaFinal && x.BajaIMSS == null);
                        var empleadoReingreso = listaEmpleados.Where(x => listaidEmp.Contains(x.IdEmpleado) && DbFunctions.TruncateTime(x.FechaIMSS) >= DbFunctions.TruncateTime(fechaI) && DbFunctions.TruncateTime(x.FechaIMSS) <= DbFunctions.TruncateTime(fechaF) && x.BajaIMSS == null);
                        if (empleadoReingreso.Count() > 0)
                        {
                            genero_info = true;
                            foreach (var dato in empleadoReingreso)
                            {
                                ws.Cell("A" + i).Value = dato.NSS;
                                ws.Cell("B" + i).Value = dato.Nombre;
                                ws.Cell("C" + i).Value = dato.FechaIMSS;
                                ws.Cell("D" + i).Value = dato.SDI;
                                ws.Cell("E" + i).Value = dato.Cliente;
                                ws.Cell("F" + i).Value = "Reingreso";
                                ws.Cell("G" + i).Value = dato.RFC;
                                ws.Cell("H" + i).Value = dato.CURP;
                                i++;
                            }
                        }
                        break;
                        #endregion
                }



            }

            ws.Rows().AdjustToContents();
            ws.Columns().AdjustToContents();

            wb.SaveAs(newruta);

            return genero_info ? pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo : "Nada";
        }

        public static string GenerarReporteAusentismo(int idUsuario, DateTime fecI, DateTime fecF, string NombreEmpresa, string ruta, string pathDescarga, int idempresa)
        {
            var sucursales = Getsucursales();
            bool genero_info = false;
            var incidencias = new List<Inasistencias>();
            var itemList = new List<contadorAusentismo>();
            var itemPermiso = new List<contadorAusentismo>();
            var newruta = Utils.ValidarFolderUsuario(idUsuario, ruta);
            newruta = newruta + NombreEmpresa + ".xlsx";
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Faltas");
            var wp = wb.Worksheets.Add("Permisos");

            //var fechaI = new DateTime(fecI.Year, fecI.Month, fecI.Day, 05, 00, 0);
            //var fechaF = new DateTime(fecF.Year, fecF.Month, fecF.Day, 23, 50, 0);

            using (var context = new RHEntities())
            {
                int count = 0;
                var empresa = context.Empresa.Where(x => x.IdEmpresa == idempresa).FirstOrDefault();

                var listaIdcontratos = context.Empleado_Contrato.Where(x => x.IdEmpresaFiscal == empresa.IdEmpresa && x.Status == true && sucursales.Contains(x.IdSucursal)).Select(x => x.IdContrato).ToList();

                var listaEmpleados = context.Empleado.ToList();

                var listaIncidencias = context.Inasistencias.Where(x => x.IdEmpresaFiscal == empresa.IdEmpresa && (x.IdTipoInasistencia == 8 || x.IdTipoInasistencia == 9 || x.IdTipoInasistencia == 16) && x.xNomina == false).ToList();

                foreach (var id in listaIdcontratos)
                {
                    var item = new contadorAusentismo();
                    var inci = listaIncidencias.Where(x => x.IdContrato == id).Select(x => x.Fecha).ToList();
                    var cantidad = listaIncidencias.Where(x => x.IdContrato == id && DbFunctions.TruncateTime(x.Fecha) >= DbFunctions.TruncateTime(fecI) && DbFunctions.TruncateTime(x.Fecha) <= DbFunctions.TruncateTime(fecF)).Count();
                    var idempleado = context.Empleado_Contrato.Where(x => x.IdContrato == id).Select(x => x.IdEmpleado).FirstOrDefault();
                    var empleado = context.Empleado.Where(x => x.IdEmpleado == idempleado).FirstOrDefault();
                    var nombre = empleado.APaterno + " " + empleado.AMaterno + " " + empleado.Nombres;

                    item.IdContrato = id;
                    item.NSS = empleado.NSS;
                    item.Nombres = nombre;
                    item.Cantidad = cantidad;
                    item.fechas = inci;
                    item.Tipo = "Falta";
                    if (cantidad > 0)
                    {
                        itemList.Add(item);
                    }



                    var permisos = context.Permisos.Where(x => x.IdEmpleado == idempleado && DbFunctions.TruncateTime(x.FechaInicio) >= DbFunctions.TruncateTime(fecI)
                    && DbFunctions.TruncateTime(x.FechaFin) <= DbFunctions.TruncateTime(fecF)
                    && x.ConGoce == false && x.PorHoras == false).ToList();

                    foreach (var p in permisos)
                    {
                        List<DateTime> fechas = new List<DateTime>();
                        var Lpermiso = new contadorAusentismo();
                        Lpermiso.IdContrato = id;
                        Lpermiso.NSS = empleado.NSS;
                        Lpermiso.Nombres = nombre;
                        Lpermiso.Cantidad = (int)p.Dias;
                        for (DateTime date = p.FechaInicio; date <= p.FechaFin; date = date.AddDays(1))
                        { fechas.Add(date); }
                        Lpermiso.fechas = fechas;
                        Lpermiso.Tipo = "Permiso";

                        itemPermiso.Add(Lpermiso);
                    }
                }

            }
            if (itemList.Count > 0)
            {
                genero_info = true;
                ws = Llenaausentismo(ws, fecI, fecF, itemList);
                ws.Columns().AdjustToContents();
            }

            if (itemPermiso.Count > 0)
            {
                genero_info = true;
                wp = Llenaausentismo(wp, fecI, fecF, itemPermiso);
                wp.Columns().AdjustToContents();
            }




            wb.SaveAs(newruta);
            return genero_info ? pathDescarga + "\\" + idUsuario + "\\" + NombreEmpresa + ".xlsx" : "Nada";
        }

        public static string GenerarReporteAusentismo2(int idUsuario, DateTime fecI, DateTime fecF, string NombreEmpresa, string ruta, string pathDescarga, int idempresa)
        {
            var sucursales = Getsucursales();
            bool genero_info = false;
            var incidencias = new List<Inasistencias>();
            var itemList = new List<contadorAusentismo>();
            var itemPermiso = new List<contadorAusentismo>();
            var newruta = Utils.ValidarFolderUsuario(idUsuario, ruta);
            newruta = newruta + NombreEmpresa + ".xlsx";
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Faltas");
            var wp = wb.Worksheets.Add("Permisos");

            //var fechaI = new DateTime(fecI.Year, fecI.Month, fecI.Day, 05, 00, 0);
            //var fechaF = new DateTime(fecF.Year, fecF.Month, fecF.Day, 23, 50, 0);

            DateTime fifaltas = fecI;
            DateTime fffaltas = fecF;
            DateTime fipermisos = fecI;
            DateTime ffpermisos = fecF;

            using (var context = new RHEntities())
            {
                int count = 0;
                var strA = fecI.Year.ToString();

                var ejercicio = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.Anio == strA);
                //var empresa = context.Empresa.FirstOrDefault(x => x.IdEmpresa == idempresa);

                var listaTimbrados = QueryData.BuscarTimbrados(fecI, fecF, 0, ejercicio.IdEjercicio, idempresa, 0, false);

                var arrayPeriodos = listaTimbrados.Select(x => x.IdPeriodo).Distinct().ToArray();
                var arrayNominas = listaTimbrados.Select(x => x.IdNomina).ToArray();
                //var arrayFiniquitos = listaTimbrados.Select(x => x.IdFiniquito).ToArray();
                //var arrayEmpleados = listaTimbrados.Select(x => x.IdEmpleado).ToArray();


                var arrayContratos = (from n in context.NOM_Nomina
                                      where arrayNominas.Contains(n.IdNomina)
                                      select n.IdContrato).ToArray();

                var listaIdcontratos = (from c in context.Empleado_Contrato
                                        where arrayContratos.Contains(c.IdContrato)
                                        select c.IdContrato).ToList();

                var listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where arrayPeriodos.Contains(p.IdPeriodoPago)
                                     select p).ToList();

                //var listaIdcontratos = context.Empleado_Contrato.Where(x => x.IdEmpresaFiscal == empresa.IdEmpresa && x.Status == true && sucursales.Contains(x.IdSucursal)).Select(x => x.IdContrato).ToList();
                // var listaIncidencias = context.Inasistencias.Where(x => x.IdEmpresaFiscal == empresa.IdEmpresa && (x.IdTipoInasistencia == 8 || x.IdTipoInasistencia == 9 || x.IdTipoInasistencia == 16) && x.xNomina == false).ToList();


                //var primerPeriodo = listaPeriodos.OrderByDescending(x => x.Fecha_Inicio).LastOrDefault();
                //var ultimoPeriodo = listaPeriodos.OrderByDescending(x => x.Fecha_Inicio).FirstOrDefault();

                //var fechaInicioIncidencias = primerPeriodo.Fecha_Inicio;
                //var fechaFinInicidencias = ultimoPeriodo.Fecha_Fin;

                //var listaIncidencias = (from i in context.Inasistencias
                //    where arrayEmpleados.Contains(i.IdEmpleado)
                //          && DbFunctions.TruncateTime(i.Fecha) >= DbFunctions.TruncateTime(fechaInicioIncidencias)
                //          && DbFunctions.TruncateTime(i.Fecha) <= DbFunctions.TruncateTime(fechaFinInicidencias)
                //          && (i.IdTipoInasistencia == 8 || i.IdTipoInasistencia == 9 || i.IdTipoInasistencia == 16)
                //                        select i).ToList();


                List<EmpleadoIncidencias> listaIncidencias = new List<EmpleadoIncidencias>();
                _Incidencias inc = new _Incidencias();
                foreach (var itemP in listaPeriodos)
                {

                    var arrayEmp = listaTimbrados.Where(x => x.IdPeriodo == itemP.IdPeriodoPago).Select(x => x.IdEmpleado).Distinct().ToArray();

                    var listaInc = inc.GetIncidenciasByPeriodo2(itemP, arrayEmp);

                    listaIncidencias.AddRange(listaInc);
                }

                //aplicar las incidencias pero el metodo no regresa las fechas sino solo los totales


                foreach (var id in listaIdcontratos)
                {
                    var item = new contadorAusentismo();

                    //var inci = listaIncidencias.Where(x => x.IdContrato == id).Select(x => x.Fecha).ToList();

                    //var cantidad = listaIncidencias.Where(x => x.IdContrato == id && DbFunctions.TruncateTime(x.Fecha) >= DbFunctions.TruncateTime(fecI) && DbFunctions.TruncateTime(x.Fecha) <= DbFunctions.TruncateTime(fecF)).Count();

                    var idempleado = context.Empleado_Contrato.Where(x => x.IdContrato == id).Select(x => x.IdEmpleado).FirstOrDefault();

                    var empleado = context.Empleado.Where(x => x.IdEmpleado == idempleado).FirstOrDefault();

                    var nombre = empleado.APaterno + " " + empleado.AMaterno + " " + empleado.Nombres;

                    //obtnemos las incidencias del empleado
                    var itmemIncEmpleado = listaIncidencias.Where(x => x.IdEmpleado == idempleado).ToList();

                    List<DateTime> listaFechasFaltas = new List<DateTime>();
                    List<DateTime> listaFechasPermisos = new List<DateTime>();
                    var cantidadF = 0;
                    var cantidadP = 0;
                    foreach (var itemFlista in itmemIncEmpleado)
                    {

                        var faltas = itemFlista.Incidencias.Where(x => x.TipoIncidencia.Trim() == "F"
                         || x.TipoIncidencia.Trim() == "FI"
                         || x.TipoIncidencia.Trim() == "FJ"
                         || x.TipoIncidencia.Trim() == "FA").Select(i => i.Fecha).ToList();

                        listaFechasFaltas.AddRange(faltas);

                        var permisoSinG = itemFlista.Incidencias.Where(x => x.TipoIncidencia.Trim() == "PS")
                            .Select(i => i.Fecha).ToList();

                        listaFechasPermisos.AddRange(permisoSinG);

                    }

                    cantidadF = listaFechasFaltas.Count();
                    cantidadP = listaFechasPermisos.Count();
                    //-->

                    if (cantidadF > 0)
                    {
                        //ordenamos las fecha para obtener el meno y menor de las fechas
                        var mayorf = listaFechasFaltas.OrderByDescending(x => x.Date).FirstOrDefault(); //mayor
                        var menorf = listaFechasFaltas.OrderByDescending(x => x.Date).LastOrDefault(); //mayor

                        if (mayorf > fffaltas)
                            fffaltas = mayorf;

                        if (menorf < fifaltas)
                            fifaltas = menorf;



                        item.IdContrato = id;
                        item.NSS = empleado.NSS;
                        item.Nombres = nombre;
                        item.Cantidad = cantidadF;
                        item.fechas = listaFechasFaltas;// DateTime.Now; //inci;
                        item.Tipo = "Falta";

                        if (cantidadF > 0)
                        {
                            itemList.Add(item);
                        }


                    }


                    if (cantidadP > 0)
                    {
                        var mayorp = listaFechasPermisos.OrderByDescending(x => x.Date).FirstOrDefault(); //mayor
                        var menorp = listaFechasPermisos.OrderByDescending(x => x.Date).LastOrDefault(); //mayor


                        if (mayorp > ffpermisos)
                            ffpermisos = mayorp;

                        if (menorp < fipermisos)
                            fipermisos = menorp;

                        var Lpermiso = new contadorAusentismo();

                        Lpermiso.IdContrato = id;
                        Lpermiso.NSS = empleado.NSS;
                        Lpermiso.Nombres = nombre;
                        Lpermiso.Cantidad = cantidadP;
                        Lpermiso.fechas = listaFechasPermisos;
                        Lpermiso.Tipo = "Permiso";
                        if (cantidadP > 0)
                            itemPermiso.Add(Lpermiso);

                    }










                    //var permisos = context.Permisos.Where(x => x.IdEmpleado == idempleado && DbFunctions.TruncateTime(x.FechaInicio) >= DbFunctions.TruncateTime(fecI)
                    //&& DbFunctions.TruncateTime(x.FechaFin) <= DbFunctions.TruncateTime(fecF)
                    //&& x.ConGoce == false && x.PorHoras == false).ToList();

                    //var permisos = (from p in context.Permisos
                    //    where arrayEmpleados.Contains(p.IdEmpleado)
                    //          &&
                    //          DbFunctions.TruncateTime(p.FechaInicio) >= DbFunctions.TruncateTime(fechaInicioIncidencias)
                    //          &&
                    //          DbFunctions.TruncateTime(p.FechaInicio) <= DbFunctions.TruncateTime(fechaFinInicidencias)
                    //    select p).ToList();


                    //foreach (var p in permisos)
                    //{
                    //    List<DateTime> fechas = new List<DateTime>();
                    //    var Lpermiso = new contadorAusentismo();
                    //    Lpermiso.IdContrato = id;
                    //    Lpermiso.NSS = empleado.NSS;
                    //    Lpermiso.Nombres = nombre;
                    //    Lpermiso.Cantidad = (int)p.Dias;
                    //    for (DateTime date = p.FechaInicio; date <= p.FechaFin; date = date.AddDays(1))
                    //    { fechas.Add(date); }
                    //    Lpermiso.fechas = fechas;
                    //    Lpermiso.Tipo = "Permiso";

                    //    itemPermiso.Add(Lpermiso);
                    //}



                }

            }

            if (itemList.Count > 0)
            {
                genero_info = true;
                ws = Llenaausentismo(ws, fifaltas, fffaltas, itemList);
                ws.Columns().AdjustToContents();
            }

            if (itemPermiso.Count > 0)
            {
                genero_info = true;
                wp = Llenaausentismo(wp, fipermisos, ffpermisos, itemPermiso);
                wp.Columns().AdjustToContents();
            }




            wb.SaveAs(newruta);
            return genero_info ? pathDescarga + "\\" + idUsuario + "\\" + NombreEmpresa + ".xlsx" : "Nada";


        }


        private static Tuple<int, int, int> GetIncidenciasEmpleado(List<EmpleadoIncidencias> listaIncidencias, int ide)
        {
            int faltas = 0;
            int permisos = 0;
            int incapacidades = 0;

            //var listaIncicendiasByEmpleado =listaIncidencias.FirstOrDefault(x => x.idPeriodo == itemNomina.IdPeriodo && x.IdEmpleado == itemEmpleado.IdEmpleado);

            var listaIncicendiasByEmpleado = listaIncidencias.FirstOrDefault(x => x.IdEmpleado == ide);

            if (listaIncicendiasByEmpleado != null)
            {
                //Faltas
                var faltase =
                    listaIncicendiasByEmpleado.Incidencias.Where(
                            x =>
                                x.TipoIncidencia.Trim() == "F" || x.TipoIncidencia.Trim() == "FI" ||
                                x.TipoIncidencia.Trim() == "FJ" || x.TipoIncidencia.Trim() == "FA")
                        .Select(i => i.TipoIncidencia)
                        .ToList()
                        .Count;
                //Permisos sin goce
                var permisoSinG =
                    listaIncicendiasByEmpleado.Incidencias.Where(x => x.TipoIncidencia.Trim() == "PS")
                        .Select(i => i.TipoIncidencia)
                        .ToList()
                        .Count;

                //Incapacidad


                var incapacidadE =
                    listaIncicendiasByEmpleado.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IE")
                        .Select(i => i.TipoIncidencia)
                        .ToList()
                        .Count;

                var incapacidadM =
                    listaIncicendiasByEmpleado.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IM")
                        .Select(i => i.TipoIncidencia)
                        .ToList()
                        .Count;

                var incapacidadR =
                    listaIncicendiasByEmpleado.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IR")
                        .Select(i => i.TipoIncidencia)
                        .ToList()
                        .Count;

                var incapacidadTotalDias = incapacidadE + incapacidadM + incapacidadR;


                faltas = faltas;
                permisos = permisoSinG;
                incapacidades = incapacidadTotalDias;
            }

            return new Tuple<int, int, int>(faltas, permisos, incapacidades);
        }

        private static IXLWorksheet Llenaausentismo(IXLWorksheet ws, DateTime fechaI, DateTime fechaF, List<contadorAusentismo> itemList)
        {
            TimeSpan ts = fechaF - fechaI;
            int cantidadDias = ts.Days;

            ws.Cell("A1").Value = "Reporte Alerta de Ausentismo del " + fechaI.ToString("dd/MM/yyyy") + " AL " + fechaF.ToString("dd/MM/yyyy");
            ws.Range("A1:I1").Merge();
            ws.Cell("A1").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Font.SetBold();
            ws.Cell("A3").Value = "NSS";
            ws.Cell("A3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("A3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("B3").Value = "Nombre";
            ws.Cell("B3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("B3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("C3").Value = "Cantidad";
            ws.Cell("C3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("C3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            int i = 4;
            int dias = 0;

            while (dias <= cantidadDias)
            {
                ws.Cell(3, i).Value = fechaI.AddDays(dias).ToString("dd/MM/yyyy");
                ws.Cell(3, i).Style
                      .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                      .Font.SetBold();
                ws.Cell(3, i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                ws.Columns("3," + i).AdjustToContents();
                ws.Columns("3," + i).AdjustToContents();
                i++;
                dias++;
            }
            int c = 4;
            int k = 4;

            foreach (var item in itemList)
            {

                int dias2 = 0;
                int j = 4;
                ws.Cell("A" + c).Value = item.NSS;
                ws.Cell("B" + c).Value = item.Nombres;
                ws.Cell("C" + c).Value = item.Cantidad;


                while (dias2 <= cantidadDias)
                {


                    var fechaComparar = fechaI.AddDays(dias2);
                    var fechastring = fechaComparar.ToString("dd/MM/yyyy");

                    foreach (var f in item.fechas)
                    {
                        var fechastring2 = f.ToString("dd/MM/yyyy");
                        if (fechastring2 == fechastring)
                        {

                            ws.Cell(k, j).Value = "X";
                            ws.Cell(k, j).Style
                                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                  .Font.SetBold();

                        }

                    }


                    j++;
                    dias2++;

                }
                k++;
                c++;
            }
            return ws;
        }

        public static string GenerarReporteIncapacidades(int idUsuario, DateTime fecI, DateTime fecF, int idEmpresa = 0, string pathFolder = "", string pathDescarga = "")
        {
            var sucursales = Getsucursales();
            List<Sucursal> listaSucursal;
            List<Empleado> listaEmpleados;
            List<Cliente> listaClientes;
            //List<Empleado_Contrato> listaContratos;
            List<Incapacidades> listaIncapacidades;
            string nombreEmpresa = "-";
            int[] arrayIdEmpresa;

            #region GET DATOS
            using (var context = new RHEntities())
            {

                //Obtiene el array de empresas
                if (idEmpresa == 0) //todas las empresas
                {
                    List<Empresa> listaEmpresa = Empresas.GetEmpresasFiscales();
                    nombreEmpresa = "TODAS";
                    
                    arrayIdEmpresa = listaEmpresa.Select(x => x.IdEmpresa).ToArray();
                }
                else //Por empresa
                {
                    Empresa itemEmpresa = new Empresas().GetEmpresaById(idEmpresa);
                    nombreEmpresa = itemEmpresa.RazonSocial.Substring(0, 15);

                    arrayIdEmpresa = new int[1];
                    arrayIdEmpresa[0] = idEmpresa;
                }


                //Obtenemos las incapacidades
                listaIncapacidades = (from i in context.Incapacidades
                                      where arrayIdEmpresa.Contains(i.IdEmpresaFiscal)
                                            && DbFunctions.TruncateTime(i.FechaInicio) >= DbFunctions.TruncateTime(fecI) && DbFunctions.TruncateTime(i.FechaInicio) <= DbFunctions.TruncateTime(fecF) && sucursales.Contains(i.IdSucursal)
                                      select i).ToList();

                //Obtener los contratos
                    //var arrayContratos = listaIncapacidades.Select(x => x.IdContrato).ToArray();

                    //listaContratos = (from c in context.Empleado_Contrato
                    //                  where arrayContratos.Contains(c.IdContrato)
                    //                  select c).ToList();

                //Obtener los empleados

                var arrayIdEmpleado = listaIncapacidades.Select(x => x.IdEmpleado).ToArray();
                var arraySucursal = listaIncapacidades.Select(x => x.IdSucursal).ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleado.Contains(e.IdEmpleado)
                                  select e).ToList();

                listaSucursal = (from s in context.Sucursal 
                                 where arraySucursal.Contains(s.IdSucursal)
                                 select s).ToList();

                var arrayClientes = listaSucursal.Select(x => x.IdCliente).ToArray();

                listaClientes = (from c in context.Cliente
                                where arrayClientes.Contains(c.IdCliente)
                                select c).ToList();
            }

            #endregion

            //#region DOCUMENTO EXCEL - NOMINA ********************************************************
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Incapacidades");


            #region HEADER
            string[] arrayHeaders = { "NSS", "Nombre", "Fecha Inicio", "Folio Incapacidad", "Dias", "SDI", "Tipo", "Clase", "Fecha Fin", "Cliente" };

            for (int idx = 0; idx < arrayHeaders.Count(); idx++)
            {
                worksheet.Cell(1, idx+1).Value = arrayHeaders[idx];
            }

            //worksheet.Cell(1, 1).Value = "NSS";
            //worksheet.Cell(1, 2).Value = "Nombre";
            //worksheet.Cell(1, 3).Value = "Fecha Inicio";
            //worksheet.Cell(1, 4).Value = "Folio Incapacidad";
            //worksheet.Cell(1, 5).Value = "Dias";
            //worksheet.Cell(1, 6).Value = "SDI";
            //worksheet.Cell(1, 7).Value = "Tipo";
            //worksheet.Cell(1, 8).Value = "Clase";
            //worksheet.Cell(1, 9).Value = "Fecha Fin";
            //worksheet.Cell(1, 10).Value = "Cliente";

            #endregion

            #region CONTENIDO
            int row = 2;

            foreach (var itemIncapacidad in listaIncapacidades)
            {
                var strCliente = "";
                var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == itemIncapacidad.IdEmpleado);

                var itemSucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemIncapacidad.IdSucursal);

                //if (itemSucursal != null)
                //{
                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);

                    //if (itemCliente != null)
                        strCliente = itemCliente.Nombre + " " + itemSucursal.Ciudad;
                //}

                worksheet.Cell(row, 1).Value = itemEmpleado.NSS;
                worksheet.Cell(row, 2).Value = itemEmpleado.APaterno + " " + itemEmpleado.AMaterno + "" + itemEmpleado.Nombres;
                worksheet.Cell(row, 3).Value = itemIncapacidad.FechaInicio;
                worksheet.Cell(row, 4).Value = itemIncapacidad.Folio;
                worksheet.Cell(row, 5).Value = itemIncapacidad.Dias;
                worksheet.Cell(row, 6).Value = itemIncapacidad.Sdi;
                worksheet.Cell(row, 7).Value = itemIncapacidad.Tipo;
                worksheet.Cell(row, 8).Value = itemIncapacidad.Clase;
                worksheet.Cell(row, 9).Value = itemIncapacidad.FechaFin;
                worksheet.Cell(row, 10).Value = strCliente;
                row++;
            }
            #endregion


            #region AJUSTES DE COLUMNA
            //worksheet.Column(1).AdjustToContents();
            //worksheet.Column(2).AdjustToContents();
            //worksheet.Column(3).AdjustToContents();
            //worksheet.Column(4).AdjustToContents();
            //worksheet.Column(5).AdjustToContents();
            //worksheet.Column(6).AdjustToContents();
            //worksheet.Column(7).AdjustToContents();
            //worksheet.Column(8).AdjustToContents();
            //worksheet.Column(9).AdjustToContents();
            //worksheet.Column(10).AdjustToContents();
            worksheet.Columns(1, 10).AdjustToContents();


            #endregion

            #region DISEÑO

            worksheet.Range("A1:J1").Style
            .Font.SetFontSize(13)
            .Font.SetBold(true)
            .Font.SetFontColor(XLColor.Black)
            .Fill.SetBackgroundColor(XLColor.YellowGreen)
           .Border.BottomBorder = XLBorderStyleValues.Thick;


            #endregion




            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = nombreEmpresa + "_Incapacidades_.xlsx";

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);

            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;
        }

        public static string GenerarReporteIncapacidades2(int idUsuario, DateTime fecI, DateTime fecF, int idEmpresa = 0, string pathFolder = "", string pathDescarga = "")
        {
            var sucursales = Getsucursales();
            List<Sucursal> listaSucursal;
            List<Empleado> listaEmpleados;
            List<Cliente> listaClientes;
            List<Empleado_Contrato> listaContratos;
            List<Incapacidades> listaIncapacidades;
            List<NOM_PeriodosPago> listaPeriodos;
            Empresa itemEmpresa = new Empresa();
            string nombreEmpresa = "-";
            List<EmpleadoIncidencias> listaIncidencias = new List<EmpleadoIncidencias>();
            

            #region GET DATOS
            using (var context = new RHEntities())
            {

                var strA = fecI.Year.ToString();//año de fecha inicio

                var ejercicio = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.Anio == strA);// datos del ejercicio fical de acuerdo al año

                var listaTimbrados = QueryData.BuscarTimbrados(fecI, fecF, 0, ejercicio.IdEjercicio, idEmpresa, 0, false);//devuelve nominas timbradas de acuerdo a los parametros dados
                //arrays se obtienen a partir de la lista de Timbrados
                var arrayPeriodos = listaTimbrados.Select(x => x.IdPeriodo).Distinct().ToArray();//todos los periodos de la lista de timbrados
                var arrayNominas = listaTimbrados.Select(x => x.IdNomina).ToArray();//todos los id de nominas de la lista de timbrados
                var arrayEmpleados = listaTimbrados.Select(x => x.IdEmpleado).Distinct().ToArray();//los id de empleados de la lista de timbrados

                var arrayContratos = (from n in context.NOM_Nomina // todos los id de contrado que coincidan con las nominas en el array
                                      where arrayNominas.Contains(n.IdNomina)
                                      select n.IdContrato).ToArray();

                listaContratos = (from c in context.Empleado_Contrato   //devuelve una lista de empledo_contrato de acuerdo a el array con los contratos
                                  where arrayContratos.Contains(c.IdContrato)
                                  select c).ToList();


                listaEmpleados = (from e in context.Empleado//lista de empleados base al array de empledos
                                  where arrayEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList(); 

                listaPeriodos = (from p in context.NOM_PeriodosPago//lista de periodos en base al array de id de periodos
                                 where arrayPeriodos.Contains(p.IdPeriodoPago)
                                 select p).ToList();

                listaIncapacidades = (from i in context.Incapacidades
                                      where arrayEmpleados.Contains(i.IdEmpleado) &&
                                     (i.FechaInicio >= fecI.Date && i.FechaInicio <= fecF)
                                      select i).ToList();


                _Incidencias inc = new _Incidencias();
                foreach (var itemP in listaPeriodos)
                {
                    var arrayEmp = listaTimbrados.Where(x => x.IdPeriodo == itemP.IdPeriodoPago).Select(x => x.IdEmpleado).Distinct().ToArray();//id de los empleados de este periodo que coincidan en la lista de timbrados

                    var listaInc = inc.GetIncidenciasByPeriodo2(itemP, arrayEmp);//lista de incidencias de los empleados en este periodo

                    listaIncidencias.AddRange(listaInc);
                }
                
                listaSucursal = context.Sucursal.ToList();
                listaClientes = context.Cliente.ToList();

            }

            #endregion

            //#region DOCUMENTO EXCEL - NOMINA ********************************************************
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Incapacidades");


            #region HEADER
            string[] arrayHeaders = { "NSS", "Nombre", "Fecha Inicio", "Folio Incapacidad", "Dia", "SDI", "Tipo", "Clase", "Fecha Fin", "Cliente", "Contrato", "Periodo" };

            for (int idx = 0; idx < arrayHeaders.Count(); idx++)
            {
                worksheet.Cell(1, idx + 1).Value = arrayHeaders[idx];
            }
            //worksheet.Cell(1, 1).Value = "NSS";
            //worksheet.Cell(1, 2).Value = "Nombre";
            //worksheet.Cell(1, 3).Value = "Fecha Inicio";
            //worksheet.Cell(1, 4).Value = "Folio Incapacidad";
            //worksheet.Cell(1, 5).Value = "Dia";
            //worksheet.Cell(1, 6).Value = "SDI";
            //worksheet.Cell(1, 7).Value = "Tipo";
            //worksheet.Cell(1, 8).Value = "Clase";
            //worksheet.Cell(1, 9).Value = "Fecha Fin";
            //worksheet.Cell(1, 10).Value = "Cliente";
            //worksheet.Cell(1, 11).Value = "Contrato";
            //worksheet.Cell(1, 12).Value = "Periodo";

            #endregion

             #region CONTENIDO
            int row = 2;


            foreach (var itemContrato in listaContratos)
            {

                var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == itemContrato.IdEmpleado);

                if (itemEmpleado == null) continue;

                var itmemIncEmpleado = listaIncidencias.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();

                if (!itmemIncEmpleado.Any()) continue;

                var strNombreCliente = "Cliente";

                var itemSucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemContrato.IdSucursal);

                if (itemSucursal != null)
                {
                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);
                    if (itemCliente != null)
                        strNombreCliente = $"{itemCliente.Nombre} {itemSucursal.Ciudad} ";
                }

                foreach (var itemFlista in itmemIncEmpleado)
                {
                    var incapacidades = itemFlista.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IE"
                                                                   || x.TipoIncidencia.Trim() == "IM"
                                                                   || x.TipoIncidencia.Trim() == "IR")
                        .Select(i => i.IdIncidencia)
                        .ToList();

                    if (!incapacidades.Any()) continue;

                    int cont = 0;
                    int contIncDate = 0;
                    String folio = "";
                    foreach (var inca in incapacidades)
                    {

                        var itemIncapacidad = listaIncapacidades.FirstOrDefault(x => x.Id == inca);

                        if (itemIncapacidad == null) continue;
                        var periodo = listaPeriodos.Where(x => x.IdPeriodoPago == itemFlista.idPeriodo).FirstOrDefault();
                        contIncDate = (folio == "" || folio != itemIncapacidad.Folio) ? 0 : contIncDate;
                        var dia = (itemIncapacidad.FechaInicio >= periodo.Fecha_Inicio) ? itemIncapacidad.FechaInicio.AddDays(contIncDate) : periodo.Fecha_Inicio.AddDays(contIncDate);

                        worksheet.Cell(row, 1).Value = itemEmpleado.NSS;
                        worksheet.Cell(row, 2).Value = $"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";
                        worksheet.Cell(row, 3).Value = itemIncapacidad.FechaInicio;
                        worksheet.Cell(row, 4).Value = itemIncapacidad.Folio;
                        worksheet.Cell(row, 5).Value = dia;//1;//itemIncapacidad.Dias;
                        worksheet.Cell(row, 6).Value = itemIncapacidad.Sdi;
                        worksheet.Cell(row, 7).Value = itemIncapacidad.Tipo;
                        worksheet.Cell(row, 8).Value = itemIncapacidad.Clase;
                        worksheet.Cell(row, 9).Value = itemIncapacidad.FechaFin;
                        worksheet.Cell(row, 10).Value = strNombreCliente;
                        worksheet.Cell(row, 11).Value = itemContrato.IdContrato;
                        worksheet.Cell(row, 12).Value = itemFlista.idPeriodo;
                        row++;
                        cont++;
                        contIncDate++;
                        folio = itemIncapacidad.Folio;
                    }
                    if (cont > 0)
                    {
                        worksheet.Cell(row, 1).Value = itemEmpleado.NSS;
                        worksheet.Cell(row, 2).Value =$"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";
                        worksheet.Cell(row, 5).Value = cont;
                        worksheet.Cell(row, 11).Value = itemContrato.IdContrato;
                        worksheet.Cell(row, 12).Value = itemFlista.idPeriodo;



                        worksheet.Range(row, 1, row, 12).Style.Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                        worksheet.Range(row, 1, row, 12).Style.Font.SetBold(true);


                        row++;
                    }



                }

            }

            #endregion


            #region AJUSTES DE COLUMNA
            //worksheet.Column(1).AdjustToContents();
            //worksheet.Column(2).AdjustToContents();
            //worksheet.Column(3).AdjustToContents();
            //worksheet.Column(4).AdjustToContents();
            //worksheet.Column(5).AdjustToContents();
            //worksheet.Column(6).AdjustToContents();
            //worksheet.Column(7).AdjustToContents();
            //worksheet.Column(8).AdjustToContents();
            //worksheet.Column(9).AdjustToContents();
            //worksheet.Column(10).AdjustToContents();
            //worksheet.Column(11).AdjustToContents();
            //worksheet.Column(12).AdjustToContents();
            worksheet.Columns(1,12).AdjustToContents();


            #endregion

            #region DISEÑO

            worksheet.Range("A1:L1").Style
          .Font.SetFontSize(13)
          .Font.SetBold(true)
          .Font.SetFontColor(XLColor.Black)
          .Fill.SetBackgroundColor(XLColor.YellowGreen)
           .Border.BottomBorder = XLBorderStyleValues.Thick;


            #endregion

            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = nombreEmpresa + "_Incapacidades_.xlsx";

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);

            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;
        }

        public List<Empresa> EmpresasFiscales()
        {

            using (var context = new RHEntities())
            {
                var lista = context.Empresa.Where(x => x.RegistroPatronal != null).ToList();
                return lista;
            }
        }

        private static IXLWorksheet encabezadoExcel(IXLWorksheet ws, DateTime fechaI, DateTime fechaF)
        {
            ws.Cell("A1").Value = "Reporte Alerta de generales del " + fechaI.ToString("dd/MM/yyyy") + " AL " + fechaF.ToString("dd/MM/yyyy");
            ws.Range("A1:I1").Merge();
            ws.Cell("A1").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Font.SetBold();

            ws.Cell("A3").Value = "NSS";
            ws.Cell("A3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("A3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("B3").Value = "EMPLEADO";
            ws.Cell("B3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("B3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("C3").Value = "ALTA IMSS";
            ws.Cell("C3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("C3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("D3").Value = "BAJA";
            ws.Cell("D3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("D3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("E3").Value = "MS";
            ws.Cell("E3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("E3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("F3").Value = "SDI";
            ws.Cell("F3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("F3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("G3").Value = "EMPRESA";
            ws.Cell("G3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("G3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("H3").Value = "CLIENTE";
            ws.Cell("H3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("H3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("I3").Value = "TIPO";
            ws.Cell("I3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("I3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("J3").Value = "RFC";
            ws.Cell("J3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("J3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("K3").Value = "CURP";
            ws.Cell("K3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("K3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            return ws;
        }

        private static int[] Getsucursales()
        {
            ControlUsuario cu = new ControlUsuario();
            return cu.GetSucursalesUsuario(ControlAcceso.GetUsuarioEnSession());
        }
    }

    public class ReporteInfonavit
    {
        public int IdNomina { get; set; }
        public int IdConcepto { get; set; }
        public decimal TotalDescuento { get; set; }

        public int IdEmpleado { get; set; }
        public string APaterno { get; set; }
        public string AMaterno { get; set; }
        public string Nombres { get; set; }
        public string NSS { get; set; }
        public bool Estatus { get; set; }
        public decimal Sd { get; set; }
        public decimal Sdi { get; set; }
        public decimal Sbc { get; set; }

        public string NombrePeriodo { get; set; }
        public bool Autorizado { get; set; }
        public int Bimestre { get; set; }
        public DateTime Mes { get; set; }

        public int DiasLaborados { get; set; }


        public string NumCredito { get; set; }
        public int TipoCredito { get; set; }
        public decimal FactorDescuento { get; set; }
        public DateTime FechaInicio { get; set; }
        public string FechaSuspension { get; set; }
        public bool UsaUma { get; set; }
        public string Empresa { get; set; }
        public string Cliente { get; set; }
        public string Sucursal { get; set; }

    }

    public class modelCambioEmp
    {
        public int idEmpleado { get; set; }
        public string empresaO { get; set; }
        public string empresaN { get; set; }
        public string fechaCambio { get; set; }

    }
    public class contadorAusentismo
    {
        public int IdContrato { get; set; }
        public string Nombres { get; set; }
        public List<DateTime> fechas { get; set; }
        public int Cantidad { get; set; }
        public string NSS { get; set; }
        public string Tipo { get; set; }
    }
}
