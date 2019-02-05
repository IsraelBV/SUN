using System.Xml;
using RH.Entidades;
using System.IO;
using Nomina.Reportes.Datos;
using Common.Utils;
using SpreadsheetLight;
using ClosedXML.Excel;
using MoreLinq;
using RH.BLL;
using System.Data;
using System.Web;
using Excel;
using System;
using System.Linq;

namespace Nomina.Reportes
{
    public class Dispersion_Finiquito
    {
        RHEntities ctx = null;

        public Dispersion_Finiquito()
        {
            ctx = new RHEntities();
        }

        public string crearExcel(int idusuario, string ruta, int IdSucursal, NOM_PeriodosPago pperiodo)
        {
            //Variables
            NOM_Finiquito finiquito;
            Empleado empleado;
            Empleado_Contrato empleadoC;
            string empresaF;
            string empresaC;
            NOM_FacturacionF_Finiquito factura;
            NOM_FacturacionC_Finiquito facturaC;
            DatosBancarios datosbancarios;
            C_Banco_SAT banco = new C_Banco_SAT();

            using (var context = new RHEntities())
            {
                finiquito = context.NOM_Finiquito.FirstOrDefault(x => x.IdPeriodo == pperiodo.IdPeriodoPago);
                empleado = context.Empleado.FirstOrDefault(x => x.IdEmpleado == finiquito.IdEmpleado);
                //var empleadoC = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == empleado.IdEmpleado && x.Status == true).FirstOrDefault();
                empleadoC = context.Empleado_Contrato.FirstOrDefault(x => x.IdEmpleado == empleado.IdEmpleado);


                empresaF =
                   context.Empresa.Where(x => x.IdEmpresa == empleadoC.IdEmpresaFiscal)
                       .Select(x => x.RazonSocial)
                       .FirstOrDefault();

                empresaC =
                   context.Empresa.Where(x => x.IdEmpresa == empleadoC.IdEmpresaComplemento)
                       .Select(x => x.RazonSocial)
                       .FirstOrDefault();

                if (empresaC == null)
                {
                    empresaC =
                        context.Empresa.Where(x => x.IdEmpresa == empleadoC.IdEmpresaSindicato)
                            .Select(x => x.RazonSocial)
                            .FirstOrDefault();
                }

                factura =
                   context.NOM_FacturacionF_Finiquito
                       .FirstOrDefault(x => x.IdFiniquito == finiquito.IdFiniquito && x.IdPeriodo == pperiodo.IdPeriodoPago);
                facturaC =
                   context.NOM_FacturacionC_Finiquito
                       .FirstOrDefault(x => x.IdFiniquito == finiquito.IdFiniquito && x.IdPeriodo == pperiodo.IdPeriodoPago);
                datosbancarios = context.DatosBancarios.FirstOrDefault(x => x.IdEmpleado == empleado.IdEmpleado);

                if (datosbancarios == null)
                {
                    banco = null;
                }
                else
                {
                    banco = context.C_Banco_SAT.FirstOrDefault(x => x.IdBanco == datosbancarios.IdBanco);
                }
            }



            var newruta = ValidarFolderUsuario(idusuario, ruta);

            //string[] arrayEmpresaF = empresaF.Split(' ');
            //string[] arrayEmpresaC = empresaC.Split(' ');
            int contadorHojas = 0;
            string Nombre;
            string NombreF;
            int nosiga = 0;

            newruta = newruta + "Dispersion.xlsx";
            var wb = new XLWorkbook();

            while (contadorHojas < 2)
            {
                int i = 6;
                if (contadorHojas == 0)
                {
                    //Nombre = arrayEmpresaF[0]+" " +arrayEmpresaF[1] + " " +arrayEmpresaF[2];
                    NombreF = empresaF;
                    nosiga = datosbancarios == null ? 0 : datosbancarios.NoSigaF.Value;
                }
                else
                {
                    //Nombre = arrayEmpresaC[0] + " " + arrayEmpresaC[1] + " " + arrayEmpresaC[2];
                    NombreF = empresaC;
                    nosiga = datosbancarios == null ? 0 : datosbancarios.NoSigaC.Value;
                }


                if (NombreF.Length > 30)
                {
                    NombreF = NombreF.Substring(0, 29);
                }

                var ws = wb.Worksheets.Add(NombreF);


                ws.Cell("C2").Value = NombreF;
                ws.Cell("C3").Value = "Dispersion";
                ws.Cell("C4").Value = pperiodo.Fecha_Inicio.ToString("dd/MM/yyyy") + " - " + pperiodo.Fecha_Fin.ToString("dd/MM/yyyy");



                ws.Range("C2:F2").Merge();
                ws.Cell("C2").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Font.SetBold();

                ws.Range("C3:F3").Merge();
                ws.Cell("C3").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Range("C4:F4").Merge();
                ws.Cell("C4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell("C" + i).Value = banco == null ? "no tiene asignado banco" : banco.Descripcion;
                ws.Range("C" + i + ":F" + i).Merge();
                ws.Cell("C" + i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                i++;
                ws.Cell("C" + i).Value = "NOMBRE DEL COLABORADOR";
                ws.Cell("D" + i).Value = "IMPORTE TOTAL";
                ws.Cell("E" + i).Value = "NoSiga";
                ws.Cell("F" + i).Value = "Cuenta";
                i++;

                ws.Cell("C" + i).Value = empleado.APaterno + " " + empleado.AMaterno + " " + empleado.Nombres;
                if (contadorHojas == 0)
                {
                    ws.Cell("D" + i).Value = finiquito.TOTAL_total;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                }
                else
                {
                    ws.Cell("D" + i).Value = 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                }

                ws.Cell("E" + i).Value = nosiga;
                ws.Cell("F" + i).Value = datosbancarios == null ? "Sin datos bancarios" : datosbancarios.CuentaBancaria;
                i = i + 4;

                ws.Cell("C" + i).Value = contadorHojas == 0 ? "RELATIVOS" : "FACTURA";


                ws.Range("C" + i + ":F" + i).Merge();
                ws.Cell("C" + i).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Font.SetBold();
                i++;
                if (contadorHojas == 0)
                {
                    ws.Cell("C" + i).Value = "CUOTAS IMSS - INFONAVIT";
                    ws.Cell("D" + i).Value = factura?.F_Cuota_IMSS_Infonavit ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                    ws.Cell("C" + i).Value = "IMPUESTO SOBRE NOMINAS";
                    ws.Cell("D" + i).Value = factura?.F_ImpuestoNomina ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                    ws.Cell("C" + i).Value = "AMORTIZACION INFONAVIT";
                    ws.Cell("D" + i).Value = factura?.F_Amortizacion_Infonavit ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                    ws.Cell("C" + i).Value = "FONACOT";
                    ws.Cell("D" + i).Value = factura?.F_Fonacot ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                    ws.Cell("C" + i).Value = "PENSION ALIMENTICIA";
                    ws.Cell("D" + i).Value = factura?.F_Pension_Alimenticia ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                    ws.Cell("C" + i).Value = "RELATIVOS";
                    ws.Cell("D" + i).Value = factura?.F_Relativo ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                    ws.Cell("C" + i).Value = "TOTAL DISPERSION";
                    ws.Cell("D" + i).Value = factura?.F_Total_Nomina ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                    ws.Cell("C" + i).Value = "TOTAL GENERAL";
                    ws.Cell("D" + i).Value = factura?.F_Total_Fiscal ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.00";
                    i++;
                }
                else
                {
                    ws.Cell("C" + i).Value = "PERCEPCIONES";
                    ws.Cell("D" + i).Value = facturaC?.C_Percepciones ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                    ws.Cell("C" + i).Value = "SERVICIOS" + facturaC?.C_Porcentaje_Servicio ?? "0";
                    ws.Cell("D" + i).Value = facturaC?.C_Total_Servicio ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                    ws.Cell("C" + i).Value = "RELATIVOS";
                    ws.Cell("D" + i).Value = facturaC?.C_Relativos ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                    ws.Cell("C" + i).Value = "DESCUENTO";
                    ws.Cell("D" + i).Value = facturaC?.C_Descuentos ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                    ws.Cell("C" + i).Value = "OTROS";
                    ws.Cell("D" + i).Value = facturaC?.C_Otros ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                    ws.Cell("C" + i).Value = "SUB TOTAL";
                    ws.Cell("D" + i).Value = facturaC?.C_Subtotal ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                    ws.Cell("C" + i).Value = "IVA";
                    ws.Cell("D" + i).Value = facturaC?.C_Total_IVA ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                    ws.Cell("C" + i).Value = "TOTAL DISPERSION COMPLEMENTO";
                    ws.Cell("D" + i).Value = facturaC?.C_Total_Complemento ?? 0;
                    ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                    i++;
                }



                ws.Columns("7,3").AdjustToContents();
                ws.Columns("7,4").AdjustToContents();
                ws.Columns("7,5").AdjustToContents();
                ws.Columns("7,6").AdjustToContents();
                contadorHojas++;
            }


            wb.SaveAs(newruta);

            return newruta;
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




            return folderUsuario;

        }

    }
}
