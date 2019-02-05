using RH.Entidades;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

using Nomina.Reportes.Datos;

namespace Nomina.Reportes
{
    public class Reporte_ContableGeneral
    {
        RHEntities ctx = null;
        public Reporte_ContableGeneral()
        {
            ctx = new RHEntities();
        }


        public string crearReporteGeneral(int idusuario,string ruta, int[] idperiodos, int idempresa,DateTime fechaInicio,DateTime fechaFin)
        {
            var ListaNominas = new List<NOM_Nomina>();
            var nombreEmpresa = ctx.Empresa.Where(x => x.IdEmpresa == idempresa).Select(x => x.RazonSocial).FirstOrDefault();
            var reportes = new ListaDeRaya();
            var clavecliente = "";
            int i = 4;
            
            var newruta = Utils.ValidarFolderUsuario(idusuario, ruta);
            newruta = newruta + nombreEmpresa+".xlsx";

            var wb = new XLWorkbook();


            var ws = wb.Worksheets.Add("REPORTE");

            ws.Cell("A1").Value = "DATOS PARA LA POLIZA DEL " + fechaInicio.ToString("dd/MM/yyyy") + " AL " + fechaFin.ToString("dd/MM/yyyy");
            ws.Range("A1:I1").Merge();
            ws.Cell("A1").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Font.SetBold();
         
            ws.Cell("A3").Value = "CUENTA";
            ws.Cell("A3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("A3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("B3").Value = "NOMBRE";
            ws.Cell("B3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("B3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("C3").Value = "CARGO M.E";
            ws.Cell("C3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("C3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("D3").Value = "ABONO M.E.";
            ws.Cell("D3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("D3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("E3").Value = "CARGO";
            ws.Cell("E3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("E3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("F3").Value = "ABONO";
            ws.Cell("F3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("F3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("G3").Value = "REFERENCIA";
            ws.Cell("G3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("G3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("H3").Value = "CONCEPTO";
            ws.Cell("H3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("H3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            ws.Cell("I3").Value = "DIARIO";
            ws.Cell("I3").Style
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                  .Font.SetBold();
            ws.Cell("I3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            foreach (var p in idperiodos)
            {
                var periodo = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == p).FirstOrDefault();
                if(periodo.IdTipoNomina != 11)
                {
                    decimal netop = 0;
                    decimal netod = 0;
                    var totales = reportes.conceptosDetallados(p, idempresa);
                    var pperiodos = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == p).FirstOrDefault();
                    var suc = ctx.Sucursal_Empresa.Where(x => x.IdSucursal == pperiodos.IdSucursal && x.IdEmpresa == idempresa).FirstOrDefault();
                    
                    var claveNominas = ctx.ClavesContables.Where(x => x.IdEmpresa == idempresa && x.IdConcepto == 150).FirstOrDefault();

                    if (clavecliente == "")
                    {

                        clavecliente = suc.Clave_Contable;

                    }

                    if (clavecliente != suc.Clave_Contable)
                    {
                        ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("B" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("C" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("D" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("E" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("F" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("G" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("H" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("I" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        i++;
                        clavecliente = suc.Clave_Contable;
                    }
                    foreach (var t in totales.Where(x=>x.TipoConcepto == 1 && x.TotalConcepto > 0 ))
                    {
                        ws.Cell("A" + i).Value = t.Deudora;
                        ws.Cell("B" + i).Value = "               ";
                        ws.Cell("C" + i).Value = "               ";
                        ws.Cell("D" + i).Value = "               ";
                        ws.Cell("E" + i).Value = t.TotalConcepto;
                        ws.Cell("E" + i).Style.NumberFormat.Format = "$ #,##0.00";
                        ws.Cell("F" + i).Value = "               ";
                        ws.Cell("G" + i).Value = t.ClaveCliente;
                        ws.Cell("H" + i).Value = periodo.Fecha_Inicio.ToString("dd-MM-yyyy")+ " - " +periodo.Fecha_Fin.ToString("dd-MM-yyyy");
                        ws.Cell("I" + i).Value = "               ";
                        i++;
                        netop = netop + t.TotalConcepto;
                    }
                    foreach (var t in totales.Where(x => x.TipoConcepto == 2 && x.TotalConcepto > 0))
                    {
                        ws.Cell("A" + i).Value = t.Acredora;
                        ws.Cell("B" + i).Value = "               ";
                        ws.Cell("C" + i).Value = "               ";
                        ws.Cell("D" + i).Value = "               ";
                        ws.Cell("E" + i).Value = "               ";
                        ws.Cell("F" + i).Value = t.TotalConcepto;
                        ws.Cell("F" + i).Style.NumberFormat.Format = "$ #,##0.00";
                        ws.Cell("G" + i).Value = t.ClaveCliente;
                        ws.Cell("H" + i).Value = periodo.Fecha_Inicio.ToString("dd-MM-yyyy") + " - " + periodo.Fecha_Fin.ToString("dd-MM-yyyy");
                        ws.Cell("I" + i).Value = "               ";
                        i++;
                        netod = netod + t.TotalConcepto;
                    }

                    var totalesfinales = netop - netod;
                    ws.Cell("A" + i).Value = claveNominas == null ? "sin clave":claveNominas.Acredora;
                    ws.Cell("B" + i).Value = "               ";
                    ws.Cell("C" + i).Value = "               ";
                    ws.Cell("D" + i).Value = "               ";
                    ws.Cell("E" + i).Value = "               ";
                    ws.Cell("F" + i).Value = totalesfinales;
                    ws.Cell("F" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    ws.Cell("G" + i).Value = suc.Clave_Contable;
                    ws.Cell("H" + i).Value = periodo.Fecha_Inicio.ToString("dd-MM-yyyy") + " - " + periodo.Fecha_Fin.ToString("dd-MM-yyyy");
                    ws.Cell("I" + i).Value = "               ";
                    i++;
                }
                else
                {
                    decimal netop = 0;
                    decimal netod = 0;
                    var totales = reportes.conceptosDetalladosFiniquito(p, idempresa);

                    var pperiodos = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == p).FirstOrDefault();
                    var suc = ctx.Sucursal_Empresa.Where(x => x.IdSucursal == pperiodos.IdSucursal && x.IdEmpresa == idempresa).FirstOrDefault();

                    var claveNominas = ctx.ClavesContables.Where(x => x.IdEmpresa == idempresa && x.IdConcepto == 150).FirstOrDefault();

                    if (clavecliente == "")
                    {

                        clavecliente = suc.Clave_Contable;

                    }

                    if (clavecliente != suc.Clave_Contable)
                    {
                        ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("B" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("C" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("D" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("E" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("F" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("G" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("H" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        ws.Cell("I" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        i++;
                        clavecliente = suc.Clave_Contable;
                    }

                    foreach (var t in totales.Where(x => x.TipoConcepto == 1 && x.TotalConcepto > 0))
                    {
                        ws.Cell("A" + i).Value = t.Deudora;
                        ws.Cell("B" + i).Value = "               ";
                        ws.Cell("C" + i).Value = "               ";
                        ws.Cell("D" + i).Value = "               ";
                        ws.Cell("E" + i).Value = t.TotalConcepto;
                        ws.Cell("E" + i).Style.NumberFormat.Format = "$ #,##0.00";
                        ws.Cell("F" + i).Value = "               ";
                        ws.Cell("G" + i).Value = t.ClaveCliente;
                        ws.Cell("H" + i).Value = periodo.Fecha_Inicio.ToString("dd-MM-yyyy") + " - " + periodo.Fecha_Fin.ToString("dd-MM-yyyy");
                        ws.Cell("I" + i).Value = "               ";
                        i++;
                        netop = netop + t.TotalConcepto;
                    }
                    foreach (var t in totales.Where(x => x.TipoConcepto == 2 && x.TotalConcepto > 0))
                    {
                        ws.Cell("A" + i).Value = t.Acredora;
                        ws.Cell("B" + i).Value = "               ";
                        ws.Cell("C" + i).Value = "               ";
                        ws.Cell("D" + i).Value = "               ";
                        ws.Cell("E" + i).Value = "               ";
                        ws.Cell("F" + i).Value = t.TotalConcepto;
                        ws.Cell("F" + i).Style.NumberFormat.Format = "$ #,##0.00";
                        ws.Cell("G" + i).Value = t.ClaveCliente;
                        ws.Cell("H" + i).Value = periodo.Fecha_Inicio.ToString("dd-MM-yyyy") + " - " + periodo.Fecha_Fin.ToString("dd-MM-yyyy");
                        ws.Cell("I" + i).Value = "               ";
                        i++;
                        netod = netod + t.TotalConcepto;
                    }

                    
                    var totalesfinales = netop - netod;
                    
                    ws.Cell("A" + i).Value = claveNominas == null ? "sin clave" : claveNominas.Acredora;
                    ws.Cell("B" + i).Value = "               ";
                    ws.Cell("C" + i).Value = "               ";
                    ws.Cell("D" + i).Value = "               ";
                    ws.Cell("E" + i).Value = "               ";
                    ws.Cell("F" + i).Value = totalesfinales; 
                    ws.Cell("F" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    ws.Cell("G" + i).Value = suc.Clave_Contable;
                    ws.Cell("H" + i).Value = periodo.Fecha_Inicio.ToString("dd-MM-yyyy") + " - " + periodo.Fecha_Fin.ToString("dd-MM-yyyy");
                    ws.Cell("I" + i).Value = "               ";
                    i++;
                }
         
            }
            ws.Columns("3,1").AdjustToContents();
            ws.Columns("3,2").AdjustToContents();
            ws.Columns("3,3").AdjustToContents();
            ws.Columns("3,4").AdjustToContents();
            ws.Columns("3,5").AdjustToContents();
            ws.Columns("3,6").AdjustToContents();
            ws.Columns("3,7").AdjustToContents();
            ws.Columns("3,8").AdjustToContents();
            ws.Columns("3,9").AdjustToContents();
            

            wb.SaveAs(newruta);

            return newruta;
        }
    }
}
