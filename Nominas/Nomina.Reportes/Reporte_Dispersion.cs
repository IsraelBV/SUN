using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML;
using System.Xml;
using RH.Entidades;
using System.IO;
using Nomina.Reportes.Datos;
using Common.Utils;
using SpreadsheetLight;
using ClosedXML.Excel;
using MoreLinq;

namespace Nomina.Reportes
{

    public class Reporte_Dispersion
    {
        RHEntities ctx = null;

        public Reporte_Dispersion()
        {
            ctx = new RHEntities();
        }


      public string crearexcel(int idusuario, string ruta,int IdSucursal,NOM_PeriodosPago Periodo,bool complemento, DateTime FechaIni, DateTime FechaFin)
        {
            var cliente = (from c in ctx.Cliente
                           join s in ctx.Sucursal
                          on c.IdCliente equals s.IdCliente
                           where s.IdSucursal == IdSucursal
                           select c.Nombre).FirstOrDefault();
            List<SucursalesEmpresa> ListaEmpresas = new List<SucursalesEmpresa>();
            ReportesDAO rep = new ReportesDAO();
            List<int> lista = new List<int>();
            List<NOM_Nomina> nominas = new List<NOM_Nomina>();
            ReportesDAO banemp = new ReportesDAO();
            
            bool spei, bnte;
            


            int CountTotal = 0;

            if (complemento == false) // pregunta si la sesion esta en modo complemento o no 
            {
                
                ListaEmpresas = rep.ListSucursalEmpresaFiscales(IdSucursal); // si no esta en modo complemento solo trae el listado de las empresas fiscales
            }
            else
            {
                if (Periodo.Sindicato == false)// si la sesion esta en modo complemento , pregunta si el periodo es sindicato o no
                {
                    ListaEmpresas = rep.ListaSucursalesEmpresasConComplemento(IdSucursal,Periodo); // si es solo complemento /trae las empresas fiscales y complemento
                }
                else
                {
                    ListaEmpresas = rep.ListaSucursalesEmpresasConSindicato(IdSucursal); // si es sindicato trae las empresas fiscales , complemento y sindicato
                }

            }
            var idempelados = rep.GetIdEmpleadosProcesados(Periodo.IdPeriodoPago);

            if (idempelados == null) return "No se encontró datos en este periodo";

            var newruta = ValidarFolderUsuario(idusuario, ruta);
            var nombre = "Dispersion " + cliente + " del Periodo " + Periodo.Descripcion;
            newruta = newruta + nombre+".xlsx";
            var wb = new XLWorkbook();

            

            foreach (var emp in ListaEmpresas)
            {
                List<RegistroEmpleados> pago_efectivo = new List<RegistroEmpleados>();
                spei = false;
                bnte = false;
                decimal total = 0, totale = 0, totalS = 0, totalB = 0, totalisr = 0;
                if (Periodo.IdTipoNomina == 16)
                {
                    nominas = ctx.NOM_Nomina.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && x.IdEmpresaAsimilado == emp.IdTabla).ToList();
                }
                else if(Periodo.SoloComplemento == false && Periodo.Sindicato == false)
                {
                    nominas = ctx.NOM_Nomina.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && (x.IdEmpresaFiscal == emp.IdTabla || x.IdEmpresaComplemento == emp.IdTabla)).ToList();
                }else if(Periodo.SoloComplemento == true)
                {
                    nominas = ctx.NOM_Nomina.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && x.IdEmpresaComplemento == emp.IdTabla).ToList();
                }else if(Periodo.IdTipoNomina == 17 || Periodo.Sindicato == true)
                {
                    nominas = ctx.NOM_Nomina.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && x.IdEmpresaSindicato == emp.IdTabla).ToList();
                }
        
                int count = 0;
                int aux = 1;
                
            
                if (emp.RP != null)
                {

                    CountTotal = nominas.Where(x => x.IdEmpresaFiscal == emp.IdTabla || x.IdEmpresaAsimilado == emp.IdTabla).Count();
                }
                else
                {
                    CountTotal =  nominas.Where(x => x.IdEmpresaComplemento == emp.IdTabla || x.IdEmpresaSindicato == emp.IdTabla).Count();
                }

                string[] nombreEmpresa;
                int i = 7;
                int j = 0;
                               
                if (CountTotal > 0)
                {
                    if (Periodo.IdTipoNomina == 16)
                    {                      
                        var lista_idnomina = nominas.Select(x => x.IdNomina).ToList();
                        totalisr = ctx.NOM_Nomina_Detalle.Where(x => x.IdConcepto == 43 && lista_idnomina.Contains(x.IdNomina)).Select(x => x.Total).Sum();
                    }

                        nombreEmpresa = emp.Nombre.Split(' ');
                    var ws = wb.Worksheets.Add("Dispersion " + nombreEmpresa[0]);
                    ws.Cell("A3").Value = cliente;
                    ws.Cell("A4").Value = emp.Nombre;
                    ws.Cell("A5").Value = "Dispersion";
                    ws.Cell("A6").Value = Periodo.IdPeriodoPago + " | "+FechaIni.ToString("dd-MM-yyyy") + " al " + FechaFin.ToString("dd-MM-yyyy");

                    foreach (var em in nominas)
                    {

                        var listbank = banemp.datosBancariosByEmpresa(em.IdEmpleado);
                        
                        if (!lista.Contains(listbank.IdBanco))
                        {
                            lista.Add(listbank.IdBanco);
                        }


                    }
                    List<ListadoBancoEmpleados> listadoRegistros = new List<ListadoBancoEmpleados>();
                    foreach (var b in lista)
                    {

                        var datos = rep.datosBancoEmpleados(b, nominas, emp.IdTabla);
                        listadoRegistros.Add(datos);
                    }


             
                    if (emp.RP != null)
                    {
                        
                        
                        foreach (var r in listadoRegistros) 
                        {
                            
                            if(r.IdBanco == 2)
                            {
                                
                                if (!bnte)
                                {
                                    ws.Cell("A" + i).Value = r.NombreBanco;
                                    ws.Range("A" + i + ":E" + i).Merge();
                                    ws.Cell("A" + i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                                    i++;
                                    ws.Cell("A" + i).Value = "NoSiga";
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("B" + i).Value = "NOMBRE DEL COLABORADOR";
                                    ws.Cell("B" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("C" + i).Value = "IMPORTE TOTAL";
                                    ws.Cell("C" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("D" + i).Value = "Cuenta";
                                    ws.Cell("D" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("E" + i).Value = "Metodo De Pago";
                                    ws.Cell("E" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("F" + i).Value = "Id Nomina";
                                    ws.Cell("F" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    i++;
                                    bnte = true;
                                }

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaF).ThenBy(im =>im.Importe))
                                {
                                    if (e.tipocuenta != "Efectivo")
                                    {
                                        if (e.NoSigaF > 0)
                                        {
                                            ws.Cell("A" + i).Value = e.NoSigaF;
                                        }
                                        else
                                        {
                                            ws.Cell("A" + i).Value = 0;
                                        }

                                        ws.Cell("B" + i).Value = e.Empleado;
                                        ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe);
                                        ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                        ws.Cell("D" + i).Value = e.cuenta;
                                        ws.Cell("D" + i).DataType = XLCellValues.Text;
                                        ws.Cell("E" + i).Value = e.tipocuenta;
                                        ws.Cell("F" + i).Value = e.idnomina;
                                   

                                    total = total + e.Importe;
                                    totalB = totalB + e.Importe;
                                    i++;
                                    }
                                    else { pago_efectivo.Add(e); }
                                }
                                ws.Cell("B" + i).Value = "Suma";
                                ws.Cell("B" + i).Style.Font.Bold = true;
                                ws.Cell("C" + i).Value = totalB;
                                ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                ws.Cell("C" + i).Style.Font.Bold = true;
                                i++;
                            }else
                            {                               
                                if (!spei)
                                {
                                    ws.Cell("A" + i).Value = "SPEI";
                                    ws.Range("A" + i + ":E" + i).Merge();
                                    ws.Cell("A" + i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                                    i++;
                                    ws.Cell("A" + i).Value = "NoSiga";
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("B" + i).Value = "NOMBRE DEL COLABORADOR";
                                    ws.Cell("B" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("C" + i).Value = "IMPORTE TOTAL";
                                    ws.Cell("C" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("D" + i).Value = "Cuenta";
                                    ws.Cell("D" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("E" + i).Value = "Metodo De Pago";
                                    ws.Cell("E" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("F" + i).Value = "Id Nomina";
                                    ws.Cell("F" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    i++;
                                    spei = true;
                                }
                                  
                                

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaF).ThenBy(im => im.Importe))
                                {
                                    if (e.tipocuenta != "Efectivo")
                                    {
                                        if (e.NoSigaF > 0)
                                    {
                                        ws.Cell("A" + i).Value = e.NoSigaF;
                                    }
                                    else
                                    {
                                        ws.Cell("A" + i).Value = 0;
                                    }

                                    ws.Cell("B" + i).Value = e.Empleado;
                                    ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe);
                                    ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                    ws.Cell("D" + i).Value = e.cuenta;
                                    ws.Cell("D" + i).DataType = XLCellValues.Text;
                                    ws.Cell("E" + i).Value = e.tipocuenta;
                                    ws.Cell("F" + i).Value = e.idnomina;
                                        total = total + e.Importe;
                                        totalS = totalS + e.Importe;
                                        i++;
                                }
                                    else { pago_efectivo.Add(e); }
                            }
                                
                           
                                count++;
                                aux++;
                            }
                           
                           
                        }

                        if (spei)
                        {
                            ws.Cell("B" + i).Value = "Suma";
                            ws.Cell("B" + i).Style.Font.Bold = true;
                            ws.Cell("C" + i).Value = totalS;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            ws.Cell("C" + i).Style.Font.Bold = true;
                        }

                    }
                    else
                    {
                        foreach (var r in listadoRegistros)
                        {
                            if (r.IdBanco == 2)
                            {                               
                                if (!bnte)
                                {
                                    ws.Cell("A" + i).Value = r.NombreBanco;
                                    ws.Range("A" + i + ":E" + i).Merge();
                                    ws.Cell("A" + i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                                    i++;
                                    ws.Cell("A" + i).Value = "NoSiga";
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("B" + i).Value = "NOMBRE DEL COLABORADOR";
                                    ws.Cell("B" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("C" + i).Value = "IMPORTE TOTAL";
                                    ws.Cell("C" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("D" + i).Value = "Cuenta";
                                    ws.Cell("D" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("E" + i).Value = "Metodo De Pago";
                                    ws.Cell("E" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("F" + i).Value = "Id Nomina";
                                    ws.Cell("F" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    i++;
                                    bnte = true;
                                }

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaC).ThenBy(im => im.Importe))
                                {
                                    if (e.tipocuenta != "Efectivo")
                                    {
                                        if (e.NoSigaC > 0)
                                        {
                                            ws.Cell("A" + i).Value = e.NoSigaC;
                                        }
                                        else
                                        {
                                            ws.Cell("A" + i).Value = 0;
                                        }

                                        ws.Cell("B" + i).Value = e.Empleado;
                                        ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe);
                                        ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                        ws.Cell("D" + i).Value = e.cuenta;
                                        ws.Cell("D" + i).DataType = XLCellValues.Text;
                                        ws.Cell("E" + i).Value = e.tipocuenta;
                                        ws.Cell("F" + i).Value = e.idnomina;
                                        total = total + e.Importe;
                                        totalB = totalB + e.Importe;
                                        i++;
                                    }
                                    else { pago_efectivo.Add(e); }
                                }
                                ws.Cell("B" + i).Value = "Suma";
                                ws.Cell("B" + i).Style.Font.Bold = true;
                                ws.Cell("C" + i).Value = totalB;
                                ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                ws.Cell("C" + i).Style.Font.Bold = true;
                                i++;
                            }
                            else{
                                   
                                if (!spei)
                                {
                                    ws.Cell("A" + i).Value = "SPEI";
                                    ws.Range("A" + i + ":E" + i).Merge();
                                    ws.Cell("A" + i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                                    i++;
                                    ws.Cell("A" + i).Value = "NoSiga";
                                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("B" + i).Value = "NOMBRE DEL COLABORADOR";
                                    ws.Cell("B" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("C" + i).Value = "IMPORTE TOTAL";
                                    ws.Cell("C" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("D" + i).Value = "Cuenta";
                                    ws.Cell("D" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("E" + i).Value = "Metodo De Pago";
                                    ws.Cell("E" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    ws.Cell("F" + i).Value = "Id Nomina";
                                    ws.Cell("F" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                                    i++;
                                    spei = true;
                                }
                                   
                                

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaC).ThenBy(im => im.Importe))
                                {
                                    if (e.tipocuenta != "Efectivo")
                                    {
                                        if (e.NoSigaC > 0)
                                        {
                                            ws.Cell("A" + i).Value = e.NoSigaC;
                                        }
                                        else
                                        {
                                            ws.Cell("A" + i).Value = 0;
                                        }

                                        ws.Cell("B" + i).Value = e.Empleado;
                                        ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe);
                                        ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                        ws.Cell("D" + i).Value = e.cuenta;
                                        ws.Cell("D" + i).DataType = XLCellValues.Text;
                                        ws.Cell("E" + i).Value = e.tipocuenta;
                                        ws.Cell("F" + i).Value = e.idnomina;
                                        total = total + e.Importe;
                                        totalS = totalS + e.Importe;
                                        i++;
                                    }
                                    else { pago_efectivo.Add(e); }

                                }
                                
                       

                                aux++;
                            }
                        }

                        if (spei)
                        {
                            ws.Cell("B" + i).Value = "Suma";
                            ws.Cell("B" + i).Style.Font.Bold = true;
                            ws.Cell("C" + i).Value = totalS;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            ws.Cell("C" + i).Style.Font.Bold = true;
                        }

                    }
                    //////////////////////////////////////////////////////////////////////////////
                    i++;
                    ws.Cell("A" + i).Value = "EFECTIVO";
                    ws.Range("A" + i + ":E" + i).Merge();
                    ws.Cell("A" + i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                    i++;
                    ws.Cell("A" + i).Value = "NoSiga";
                    ws.Cell("A" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                    ws.Cell("B" + i).Value = "NOMBRE DEL COLABORADOR";
                    ws.Cell("B" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                    ws.Cell("C" + i).Value = "IMPORTE TOTAL";
                    ws.Cell("C" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                    ws.Cell("D" + i).Value = "Cuenta";
                    ws.Cell("D" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                    ws.Cell("E" + i).Value = "Metodo De Pago";
                    ws.Cell("E" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                    ws.Cell("F" + i).Value = "Id Nomina";
                    ws.Cell("F" + i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                    i++;
                    foreach (var e in pago_efectivo.OrderBy(x => x.NoSigaC).ThenBy(im => im.Importe))
                    {                       
                            if (e.NoSigaC > 0)
                            {
                                ws.Cell("A" + i).Value = e.NoSigaC;
                            }
                            else
                            {
                                ws.Cell("A" + i).Value = 0;
                            }

                            ws.Cell("B" + i).Value = e.Empleado;
                            ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe);
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            ws.Cell("D" + i).Value = e.cuenta;
                            ws.Cell("D" + i).DataType = XLCellValues.Text;
                            ws.Cell("E" + i).Value = e.tipocuenta;
                            ws.Cell("F" + i).Value = e.idnomina;

                        total = total + e.Importe;
                            totale = totale + e.Importe;
                            i++;    
                    }
                    ws.Cell("B" + i).Value = "Suma";
                    ws.Cell("B" + i).Style.Font.Bold = true;
                    ws.Cell("C" + i).Value = totale;
                    ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    ws.Cell("C" + i).Style.Font.Bold = true;
                    i++;
                    /////////////////////////////////////////////////////

                    ws.Cell("B" + i).Value = "Total Dispersion:";
                    ws.Cell("B" + i).Style.Font.Bold = true;
                    ws.Cell("C" + i).Value = total;
                    ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    ws.Cell("C" + i).Style.Font.Bold = true;
                    ws.Cell("C" + i).Style.Font.FontSize = 13;
                    ws.Cell("C" + i).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                    ws.Cell("C" + i).Style.Border.TopBorderColor = XLColor.Black;
                    /////////////////////////////////////////////////////
                    if (emp.RP != null)
                    {

                        var factura = ctx.NOM_Facturacion.Where(x => x.IdEmpresaFi_As == emp.IdTabla && x.IdPeriodo == Periodo.IdPeriodoPago).FirstOrDefault();
                        if (factura != null)
                        {
                            i = i + 2;
                            ws.Cell("B" + i).Value = "Relativos";
                            ws.Range("B" + i + ":F" + i).Merge();
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Font.SetBold();
                            i = i + 1;

                            ws.Cell("B" + i).Value = "CUOTAS IMSS E INFONAVIT";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.F_Cuota_IMSS_Infonavit;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "IMPUESTO SOBRE NOMINAS";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.F_Impuesto_Nomina;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "AMORTIZACION INFONAVIT";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.F_Amortizacion_Infonavit;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            if (Periodo.IdTipoNomina==16) {
                                ws.Cell("B" + i).Value = "ISR";
                                ws.Cell("C" + i).Value = totalisr;
                            }else
                            {
                                ws.Cell("B" + i).Value = "FONACOT";
                                ws.Cell("C" + i).Value = factura.F_Fonacot;
                            }
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            if (factura.F_Pension_Alimenticia > 0)
                            {
                                ws.Cell("B" + i).Value = "PENSION ALIMENTICIA";
                                ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                                ws.Cell("C" + i).Value = factura.F_Pension_Alimenticia;
                                ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                i = i + 1;
                            }

                            ws.Cell("B" + i).Value = "RELATIVO";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.F_Relativo+totalisr;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "TOTAL PERCEPCIONES";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.F_Total_Nomina;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "TOTAL NETO";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.F_Total_Fiscal+totalisr;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                        }
                    }
                    else
                    {
                       /* decimal cuotasImss = 0;
                        decimal impuestoNomina = 0;
                        decimal relativos = 0;
                        decimal percepciones = 0;
                        decimal totalComplemento = 0;*/
                        var factura = ctx.NOM_FacturacionComplemento.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && x.IdEmpresaC == emp.IdTabla).FirstOrDefault();

                        var facturaS = ctx.NOM_FacturacionSindicato.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && x.IdEmpresaS == emp.IdTabla).FirstOrDefault();

                        if (factura != null)
                        {
                            i = i + 2;
                            ws.Cell("B" + i).Value = "FACTURACION";
                            ws.Range("B" + i + ":F" + i).Merge();
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Font.SetBold();
                            i = i + 1;

                            ws.Cell("B" + i).Value = "PERCEPCIONES";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Percepciones;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "SERVICIOS " + factura.C_Porcentaje_Servicio;
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Total_Servicio;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;



                            ws.Cell("B" + i).Value = "RELATIVO";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Relativos;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "DESCUENTOS";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Descuentos;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "OTROS";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Otros;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "SUBTOTAL";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Subtotal;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "IVA";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Total_IVA;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "TOTAL NETO";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.C_Total_Complemento;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                        }else if (facturaS !=null)// facturacion de sindicato
                        {
                            i = i + 2;
                            ws.Cell("B" + i).Value = "FACTURACION";
                            ws.Range("B" + i + ":F" + i).Merge();
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Font.SetBold();
                            i = i + 1;

                            ws.Cell("B" + i).Value = "PERCEPCIONES SINDICATO";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Percepcion_Sindicato;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "SERVICIOS " + facturaS.S_Porcentaje_Comision;
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Total_Comision;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;


                            ws.Cell("B" + i).Value = "PERCEPCIONES FISCAL";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Percepcion_Fiscal;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "IVA PERCEPCION FISCAL";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_IVA_Percepcion_Fiscal;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;


                            ws.Cell("B" + i).Value = "DIFERENCIA EMPRESA";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Dif_Montvde;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "CUOTA LEGADO";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Cuota_Legado;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "COSTO IMSS";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Costo_IMSS;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "IVA COSTO IMSS";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_IVA_Costo_IMSS;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "IMPUESTO SOBRE NOMINA";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Total_Porcentaje_Nomina;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "IVA IMPUESTO SOBRE NOMINA";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_IVA_Porcentaje_Nomina;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                            ws.Cell("B" + i).Value = "TOTAL FACTURA";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = facturaS.S_Total_Sindicato;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                        }
                    }



                    //ESTILOS DE LAS CELDAS 
                    //Titulo 
                    ws.Range("A3:E3").Merge();
                    ws.Cell("A3").Style
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Font.SetBold();
                    ws.Range("A4:E4").Merge();
                    ws.Cell("A4").Style
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Font.SetBold();

                    ws.Range("A5:E5").Merge();
                    ws.Cell("A5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Range("A6:E6").Merge();
                    ws.Cell("A6").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Columns("8,1").AdjustToContents();
                    ws.Columns("8,2").AdjustToContents();
                    ws.Columns("8,3").AdjustToContents();
                    ws.Columns("8,4").AdjustToContents();
                    ws.Columns("8,5").AdjustToContents();
                }
     
            }
            wb.SaveAs(newruta);

            //Guardar como, y aqui ponemos la ruta de nuestro archivo

            return newruta;
        }

        #region ValidarSiExisteFolder
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




            return folderUsuario;

        }
        #endregion



    }
}
