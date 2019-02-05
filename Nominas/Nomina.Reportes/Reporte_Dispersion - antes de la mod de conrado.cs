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
using DocumentFormat.OpenXml.Wordprocessing;
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


        public string crearexcel(int idusuario, string ruta, int IdSucursal, NOM_PeriodosPago Periodo, bool complemento, DateTime FechaIni, DateTime FechaFin)
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
            string strSucursal = "";

            var sucursal = ctx.Sucursal.FirstOrDefault(x => x.IdSucursal == IdSucursal);

            if (sucursal != null)
            {
                strSucursal = sucursal.Ciudad;
            }

            int CountTotal = 0;

            if (complemento == false) // pregunta si la sesion esta en modo complemento o no 
            {

                ListaEmpresas = rep.ListSucursalEmpresaFiscales(IdSucursal); // si no esta en modo complemento solo trae el listado de las empresas fiscales
            }
            else
            {
                if (Periodo.Sindicato == false)// si la sesion esta en modo complemento , pregunta si el periodo es sindicato o no
                {
                    ListaEmpresas = rep.ListaSucursalesEmpresasConComplemento(IdSucursal, Periodo); // si es solo complemento /trae las empresas fiscales y complemento
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
            newruta = newruta + nombre + ".xlsx";
            var wb = new XLWorkbook();



            foreach (var emp in ListaEmpresas)
            {
                if (Periodo.IdTipoNomina == 16)
                {
                    nominas = ctx.NOM_Nomina.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && x.IdEmpresaAsimilado == emp.IdTabla).ToList();
                }
                else if (Periodo.SoloComplemento == false && Periodo.Sindicato == false)
                {
                    nominas = ctx.NOM_Nomina.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && (x.IdEmpresaFiscal == emp.IdTabla || x.IdEmpresaComplemento == emp.IdTabla)).ToList();
                }
                else if (Periodo.SoloComplemento == true)
                {
                    nominas = ctx.NOM_Nomina.Where(x => x.IdPeriodo == Periodo.IdPeriodoPago && x.IdEmpresaComplemento == emp.IdTabla).ToList();
                }
                else if (Periodo.IdTipoNomina == 17 || Periodo.Sindicato == true)
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
                    CountTotal = nominas.Where(x => x.IdEmpresaComplemento == emp.IdTabla || x.IdEmpresaSindicato == emp.IdTabla).Count();

                }

                string[] nombreEmpresa;
                int i = 7;
                int j = 0;
                decimal total = 0;
                decimal totalS = 0;
                if (CountTotal > 0)
                {
                    nombreEmpresa = emp.Nombre.Split(' ');
                    var ws = wb.Worksheets.Add("Dispersion " + nombreEmpresa[0]);

                    if (cliente.Trim().ToUpper() != strSucursal.Trim().ToUpper())
                    {
                        ws.Cell("A3").Value = cliente + " - " + strSucursal;
                    }
                    else
                    {
                        ws.Cell("A3").Value = cliente;
                    }

                    ws.Cell("A4").Value = emp.Nombre;
                    ws.Cell("A5").Value = "Dispersion";
                    ws.Cell("A6").Value = FechaIni.ToString("dd-MM-yyyy") + " al " + FechaFin.ToString("dd-MM-yyyy");

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
                            if (r.IdBanco == 2)
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
                                i++;

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaF))
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
                                    ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe, 2);
                                    ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                    ws.Cell("D" + i).Value = e.cuenta;
                                    ws.Cell("E" + i).Value = e.tipocuenta;

                                    total = total + e.Importe;
                                    i++;
                                }
                                ws.Cell("B" + i).Value = "Total";
                                ws.Cell("C" + i).Value = r.Total;
                                ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                i++;
                            }
                            else
                            {
                                int lcount2 = listadoRegistros.Count() - 1;
                                lcount2 = lcount2 == 0 ? 1 : lcount2;
                                if (count == 0)
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
                                    i++;

                                }

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaF))
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
                                    ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe, 2);
                                    ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                    ws.Cell("D" + i).Value = e.cuenta;
                                    ws.Cell("E" + i).Value = e.tipocuenta;

                                    i++;
                                }
                                totalS = totalS + r.Total;
                                if (count != 0)
                                {
                                    if (aux == lcount2)
                                    {
                                        ws.Cell("B" + i).Value = "Total";
                                        ws.Cell("C" + i).Value = totalS;
                                        ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                        i++;

                                    }

                                }
                                count++;
                                aux++;
                            }


                        }



                    }
                    else
                    {
                        foreach (var r in listadoRegistros)
                        {
                            if (r.IdBanco == 2)
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
                                i++;

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaC))
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
                                    ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe, 2);
                                    ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                    ws.Cell("D" + i).Value = e.cuenta;
                                    ws.Cell("E" + i).Value = e.tipocuenta;
                                    total = total + e.Importe;
                                    i++;
                                }
                                ws.Cell("B" + i).Value = "Total";
                                ws.Cell("C" + i).Value = r.Total;
                                ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                i++;
                            }
                            else
                            {
                                int lcount = listadoRegistros.Count() - 1;
                                lcount = lcount == 0 ? 1 : lcount;
                                if (count == 0)
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
                                    i++;
                                    count++;
                                }

                                foreach (var e in r.empleados.OrderBy(x => x.NoSigaC))
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
                                    ws.Cell("C" + i).Value = Utils.TruncateDecimales(e.Importe, 2);
                                    ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                    ws.Cell("D" + i).Value = e.cuenta;
                                    ws.Cell("E" + i).Value = e.tipocuenta;
                                    total = total + e.Importe;
                                    i++;

                                }
                                totalS = totalS + r.Total;
                                if (count != 0)
                                {
                                    if (aux == lcount)
                                    {
                                        ws.Cell("B" + i).Value = "Total";
                                        ws.Cell("C" + i).Value = totalS;
                                        ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                                        i++;

                                    }

                                }

                                aux++;
                            }
                        }

                    }

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
                            ws.Cell("B" + i).Value = "FONACOT";
                            ws.Cell("B" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("C" + i).Value = factura.F_Fonacot;
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
                            ws.Cell("C" + i).Value = factura.F_Relativo;
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
                            ws.Cell("C" + i).Value = factura.F_Total_Fiscal;
                            ws.Cell("C" + i).Style.NumberFormat.Format = "$ #,##0.00";
                            i = i + 1;
                        }
                    }
                    else
                    {
                        decimal cuotasImss = 0;
                        decimal impuestoNomina = 0;
                        decimal relativos = 0;
                        decimal percepciones = 0;
                        decimal totalComplemento = 0;
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
                        }
                        else if (facturaS != null)// facturacion de sindicato
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


        public string GenerarReporteDispersion(int idUsuario, string path, int idSucursal, int idPeriodo, bool complemento)
        {
            string rutaArchivo = "";
            List<Empresa> listaEmpresas;
            List<Empleado> listaEmpleados;
            List<DatosBancarios> listaDatosBancarios;
            List<C_Metodos_Pago> listaMetodosPagos;
            List<NOM_Facturacion> listaFacturasF;
            List<NOM_FacturacionComplemento> listaFacturasC;
            List<NOM_FacturacionSindicato> listaFacturasS;
            List<C_Banco_SAT> listaBancos;
            List<NOM_Nomina> listaNominas;
            NOM_PeriodosPago itemPeriodoPago = new NOM_PeriodosPago();

            int?[] arrayIdEmpresaF;
            int?[] arrayIdEmpresaC;
            int?[] arrayIdEmpresaA;
            int?[] arrayIdEmpresaS;

            string nombreCliente = "Cliente_Sucursal";
            string nombreSucursal = "";
            string nombreClienteSucursal = "";

            #region GET DATA
            //GET DATA
            using (var context = new RHEntities())
            {
                itemPeriodoPago = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                var itemSucursal = context.Sucursal.FirstOrDefault(x => x.IdSucursal == itemPeriodoPago.IdSucursal);

                var itemCliente = context.Cliente.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);

                nombreCliente = $"{itemCliente.Nombre}";
                nombreSucursal = $"{ itemSucursal.Ciudad}";

                nombreClienteSucursal = !string.Equals(nombreCliente, nombreSucursal, StringComparison.CurrentCultureIgnoreCase) ? $"{nombreCliente} - {nombreSucursal}" : nombreSucursal;

                listaNominas = (from n in context.NOM_Nomina
                                where n.IdPeriodo == idPeriodo
                                select n).ToList();

                listaEmpresas = (from e in context.Empresa
                                 select e).ToList();

                listaBancos = (from b in context.C_Banco_SAT
                               select b).ToList();

                listaMetodosPagos = (from m in context.C_Metodos_Pago
                                     select m).ToList();

                var arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();
                var arrayIdcontratos = listaNominas.Select(x => x.IdContrato).ToArray();

                arrayIdEmpresaF = listaNominas.Select(x => x.IdEmpresaFiscal).Distinct().ToArray();
                //arrayIdEmpresaF = arrayIdEmpresaF.Distinct().ToArray();

                arrayIdEmpresaC = listaNominas.Select(x => x.IdEmpresaComplemento).Distinct().ToArray();
                arrayIdEmpresaA = listaNominas.Select(x => x.IdEmpresaAsimilado).Distinct().ToArray();
                arrayIdEmpresaS = listaNominas.Select(x => x.IdEmpresaSindicato).Distinct().ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

                listaDatosBancarios = (from db in context.DatosBancarios
                                       where arrayIdEmpleados.Contains(db.IdEmpleado)
                                       select db).ToList();


                listaFacturasF = (from ff in context.NOM_Facturacion
                                  where ff.IdPeriodo == idPeriodo
                                  select ff).ToList();

                listaFacturasC = (from ff in context.NOM_FacturacionComplemento
                                  where ff.IdPeriodo == idPeriodo
                                  select ff).ToList();

                listaFacturasS = (from ff in context.NOM_FacturacionSindicato
                                  where ff.IdPeriodo == idPeriodo
                                  select ff).ToList();

                C_Banco_SAT itemSinBanco = new C_Banco_SAT()
                {
                    IdBanco = 0,
                    CveSat = 0,
                    Descripcion = "Sin Banco",
                    Status = true
                };

                listaBancos.Add(itemSinBanco);
            }
            #endregion

            //ordenamos los empleados por orden alfabetico
            listaEmpleados = listaEmpleados.OrderBy(x => x.APaterno).ToList();

            var workbook = new XLWorkbook();
            bool isComplemento = false;



            #region FOR - EMPRESAS - DATOS Dispersion

            for (int ie = 1; ie <= 4; ie++)
            {
                int?[] arrayIdEmpresas = null;

                if (ie == 1)//Fiscal
                {
                    arrayIdEmpresas = arrayIdEmpresaF;
                    isComplemento = false;
                }
                else if (ie == 2)//Complemento
                {
                    arrayIdEmpresas = arrayIdEmpresaC;
                    isComplemento = true;
                }
                else if (ie == 3)//Asimilado
                {
                    arrayIdEmpresas = arrayIdEmpresaA;
                    isComplemento = false;
                }
                else if (ie == 4)//Sindicato
                {
                    arrayIdEmpresas = arrayIdEmpresaS;
                    isComplemento = true;
                }

                if (arrayIdEmpresas == null) continue;

                if( arrayIdEmpresas.Length == 0) continue;

                #region FOR- DATOS- HOJA EXCEL

                //Separa las nominas por empresas
                foreach (var itemEmpresaId in arrayIdEmpresas)
                {
                    List<DatosParaDispersion> listaDispersion = new List<DatosParaDispersion>();

                    var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == itemEmpresaId);

                    var nominasEmpresa = listaNominas.Where(x => x.IdEmpresaFiscal == itemEmpresaId).ToList();

                    #region LISTA DE DATOS

                    foreach (var itemNomina in nominasEmpresa)
                    {
                        var nombreEmpleado = "";
                        var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == itemNomina.IdEmpleado);
                        var itemDatoBancario = listaDatosBancarios.FirstOrDefault(x => x.IdEmpleado == itemNomina.IdEmpleado);
                        var itemMetodoPago = listaMetodosPagos.FirstOrDefault(x => x.IdMetodo == itemNomina.IdMetodo_Pago);

                        nombreEmpleado = $"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";

                        int idB = itemDatoBancario?.IdBanco ?? 0;

                        DatosParaDispersion itemDispersion = new DatosParaDispersion()
                        {
                            IdBanco = idB,
                            Siga = itemDatoBancario?.NoSigaF ?? 0,
                            Nombre = nombreEmpleado,
                            Cuenta = itemDatoBancario?.CuentaBancaria ?? "no tiene",
                            MetodoPago = itemMetodoPago?.Descripcion ?? "no tiene",
                            Importe = itemNomina.TotalNomina,
                            Spei = idB != 2
                        };

                        listaDispersion.Add(itemDispersion);
                    }

                    #endregion

                    //Datos de la Factura
                    var itemFacturaF = listaFacturasF.FirstOrDefault(x => x.IdEmpresaFi_As == itemEmpresaId);
                    var itemFacturaC = listaFacturasC.FirstOrDefault(x => x.IdEmpresaC == itemEmpresaId);

                    //Agrego la Hoja de excel con la dispersion
                    AddHojaExcel(ref workbook, itemEmpresa, nombreClienteSucursal, itemPeriodoPago, listaDispersion,
                        listaBancos, itemFacturaF, itemFacturaC, isComplemento);

                }

                #endregion
            }




            #endregion

            //Guarda el archivo
            var pathArchivo = Utils.ValidarFolderUsuario(idUsuario, path);


            pathArchivo += $"Dispersion {nombreSucursal} {itemPeriodoPago.Descripcion}.xlsx";

            workbook.SaveAs(pathArchivo);

            return pathArchivo;

        }

        private void AddHojaExcel(ref XLWorkbook workbook, Empresa itemEmpresa, string clienteSucursal, NOM_PeriodosPago periodo, List<DatosParaDispersion> listaDatos, List<C_Banco_SAT> listaBancos, NOM_Facturacion itemFacturaF, NOM_FacturacionComplemento itemFacturaC, bool isComplemento)
        {
            //Agrega la Hoja
            var nombreArray = itemEmpresa.RazonSocial.Split(' ');
            var nombreEmpresa = nombreArray[0];

            var worksheet = workbook.Worksheets.Add($"Dispersion {nombreEmpresa}");

            #region TITULOS

            worksheet.Cell(1, 1).Value = clienteSucursal;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            //worksheet.Cell(1, 1).Style.Font.FontSize = 18;
            worksheet.Range("A1:E1").Merge();

            worksheet.Cell(2, 1).Value = itemEmpresa.RazonSocial;
            worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(2, 1).Style.Font.Bold = true;
            //worksheet.Cell(2, 1).Style.Font.FontSize = 18;
            worksheet.Range("A2:E2").Merge();

            worksheet.Cell(3, 1).Value = "Dispersion";
            worksheet.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //worksheet.Cell(3, 1).Style.Font.Bold = true;
            //worksheet.Cell(3, 1).Style.Font.FontSize = 18;
            worksheet.Range("A3:E3").Merge();

            worksheet.Cell(4, 1).Value = $"{periodo.Fecha_Inicio:dd-MM-yyyy} al {periodo.Fecha_Fin:dd-MM-yyyy}";
            worksheet.Cell(4, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //worksheet.Cell(4, 1).Style.Font.Bold = true;
            //worksheet.Cell(4, 1).Style.Font.FontSize = 18;
            worksheet.Range("A4:E4").Merge();



            #endregion


            //listaDatos = listaDatos.OrderBy(x => x.IdBanco).ThenBy(x => x.Nombre).ToList();
            listaDatos = listaDatos.OrderBy(x => x.Spei).ThenBy(x => x.Siga).ToList();

            int idBanco = -1;
            int row = 5;
            decimal totalBanco = 0;
            int paramTotal = 0;
            bool activeSpeiHeader = false;
            List<DatosParaDispersion> listaDatosDispersion = new List<DatosParaDispersion>();

            for (int i = 0; i < 2; i++)//1ra iteracion los que nos son spei y 2da iteracion los spei
            {
                listaDatosDispersion = i == 0 ? listaDatos.Where(x => x.Spei == false).ToList() : listaDatos.Where(x => x.Spei == true).ToList();

                foreach (var itemDato in listaDatosDispersion)
                {
                    if (itemDato.IdBanco != idBanco && activeSpeiHeader == false)
                    {
                        idBanco = itemDato.IdBanco;
                        var itemBanco = listaBancos.FirstOrDefault(x => x.IdBanco == idBanco);

                        #region TOTAL

                        if (totalBanco > paramTotal)
                        {
                            worksheet.Cell(row, 2).Value = "Total";
                            worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            worksheet.Cell(row, 3).Value = totalBanco;
                            worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";

                            totalBanco = 0;
                            paramTotal = -1;
                            row++;
                        }
                        #endregion

                        #region HEADER

                        string nombreBanco = "";
                        if (i == 0)
                        {
                            nombreBanco = itemBanco.Descripcion;
                        }
                        else
                        {
                            nombreBanco = "SPEI";
                            activeSpeiHeader = true;
                        }

                        worksheet.Cell(row, 1).Value = nombreBanco;
                        worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(row, 1).Style.Font.Bold = true;

                        worksheet.Range($"A{row}:E{row}").Merge();
                        worksheet.Range($"A{row}:E{row}").Style.Fill.SetBackgroundColor(XLColor.MediumPersianBlue);
                        row++;

                        worksheet.Cell(row, 1).Value = "No.Siga";
                        worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(row, 2).Value = "Nombre Empleado";
                        worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(row, 3).Value = "Importe Total";
                        worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(row, 4).Value = "Cuenta";
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(row, 5).Value = "Metodo de Pago";
                        worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range($"A{row}:E{row}").Style.Fill.SetBackgroundColor(XLColor.CarolinaBlue);

                        row++;

                        #endregion

                    }
                    #region CONTENIDO

                    worksheet.Cell(row, 1).Value = itemDato.Siga;
                    //worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Value = itemDato.Nombre;
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Cell(row, 3).Value = itemDato.Importe;
                    worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                    worksheet.Cell(row, 4).Value = itemDato.Cuenta;
                    worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 5).Value = itemDato.MetodoPago;
                    worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    totalBanco += itemDato.Importe;
                    #endregion
                    row++;
                }
            }



            //
            #region TOTAL

            if (totalBanco > paramTotal)
            {
                worksheet.Cell(row, 2).Value = "Total";
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 3).Value = totalBanco;
                worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";

                totalBanco = 0;
                paramTotal = -1;
                row++;
            }
            #endregion

            //FACTURA
            #region DATOS FACTURA
            if (isComplemento)
            {
                row++;
                worksheet.Cell(row, 2).Value = "Factura";
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                row++;
                worksheet.Cell(row, 2).Value = "PERCEPCIONES";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Percepciones;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = $"SERVICIO {itemFacturaC.C_Porcentaje_Servicio}";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Total_Servicio;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "RELATIVO ";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Total_Servicio;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "DESCUENTOS";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Descuentos;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "OTROS";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Otros;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "SUBTOTAL";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Subtotal;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "IVA";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Total_IVA;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "TOTAL NETO";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaC.C_Total_Complemento;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
            }
            else
            {
                row++;
                worksheet.Cell(row, 2).Value = "Relativos";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;
                worksheet.Cell(row, 2).Value = "CUOTAS IMSS E INFONAVIT";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaF.F_Cuota_IMSS_Infonavit;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "IMPUESTO SOBRE NOMINAS";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaF.F_Impuesto_Nomina;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "AMORTIZACION INFONAVIT";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaF.F_Amortizacion_Infonavit;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "FONACOT";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaF.F_Fonacot;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "RELATIVO";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaF.F_Relativo;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "TOTAL PERCEPCIONES";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaF.F_Total_Nomina;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";
                row++;
                worksheet.Cell(row, 2).Value = "TOTAL NETO";
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = itemFacturaF.F_Total_Fiscal;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ ###,##0.00";

            }
            #endregion

            #region AJUSTE COLUMNAS
            worksheet.Column(1).AdjustToContents();
            worksheet.Column(2).AdjustToContents();
            worksheet.Column(3).AdjustToContents();
            worksheet.Column(4).AdjustToContents();
            worksheet.Column(5).AdjustToContents();
            #endregion

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


        private class DatosParaDispersion
        {
            public int IdBanco { get; set; }
            public int Siga { get; set; }

            public string Nombre { get; set; }
            public decimal Importe { get; set; }
            public string Cuenta { get; set; }
            public string MetodoPago { get; set; }
            public bool Spei { get; set; }

        }
    }
}
