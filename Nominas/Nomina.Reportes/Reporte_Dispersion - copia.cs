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


      public string crearexcel(int idusuario, string ruta,int IdSucursal,int IdPeriodo,bool complemento, DateTime FechaIni, DateTime FechaFin)
        {
            List<SucursalesEmpresa> s = new List<SucursalesEmpresa>();
            ReportesDAO rep = new ReportesDAO();
            List<int> lista = new List<int>();
            
            if (complemento == false)
            {
                s = rep.ListSucursalEmpresaFiscales(IdSucursal);
            }
            else
            {
                s = rep.ListaSucursalesEmpresasConComplemento(IdSucursal);
            }
            var idempelados = rep.GetIdEmpleadosProcesados(IdPeriodo);

            if (idempelados == null) return "No se encontró datos en este periodo";

            var newruta = ValidarFolderUsuario(idusuario,ruta);
            newruta = newruta + "Dispersion.xlsx";
            var wb = new XLWorkbook();
            int count = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;
            int CountTotal = 0;

            foreach (var empresas in s)
            {
                string[] oracion;
                decimal total = 0;
                
                ReportesDAO banemp = new ReportesDAO();
                if(empresas.RP != null)
                {
                     count = ctx.NOM_Nomina.Where(x => x.IdEmpresaFiscal == empresas.IdTabla && x.IdPeriodo == IdPeriodo).Count();
                     count4 = ctx.NOM_Nomina.Where(x => x.IdEmpresaAsimilado == empresas.IdTabla && x.IdPeriodo == IdPeriodo).Count();
                    CountTotal = count +  count4;
                }
                else
                {
                    count2 = ctx.NOM_Nomina.Where(x => x.IdEmpresaComplemento == empresas.IdTabla && x.IdPeriodo == IdPeriodo ).Count();
                    count3 = ctx.NOM_Nomina.Where(x => x.IdEmpresaSindicato == empresas.IdTabla && x.IdPeriodo == IdPeriodo ).Count();
                    
                    CountTotal =  + count2 + count3  ;
                }
   

              

                    
                int i = 6;
                int j = 0;
                if (CountTotal > 0)
                {
                    oracion = empresas.Nombre.Split(' ');
                    var ws = wb.Worksheets.Add("Dispersion " + oracion[0]);
                    ws.Cell("C2").Value = empresas.Nombre;
                    ws.Cell("C3").Value = "Dispersion";
                    ws.Cell("C4").Value = FechaIni.ToString("dd-MM-yyyy") + " al " + FechaFin.ToString("dd-MM-yyyy");

                    
                   
                    foreach(var em in idempelados)
                    {
                       var listbank = banemp.datosBancariosByEmpresa(em.idempleado);

                        if (!lista.Contains(listbank.IdBanco))
                        {
                            lista.Add(listbank.IdBanco);
                        }
                        
                                            
                    }
                  foreach (var b in lista)
                    {
                        var banco = ctx.C_Banco_SAT.Where(x => x.IdBanco == b).FirstOrDefault();
                        ws.Cell("C" + i).Value = banco.Descripcion;
                        ws.Range("C" + i + ":F" + i).Merge();
                        ws.Cell("C" + i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        i++;
                        ws.Cell("C" + i).Value = "NOMBRE DEL COLABORADOR";
                        ws.Cell("D" + i).Value = "IMPORTE TOTAL";
                        ws.Cell("E" + i).Value = "NoSiga";
                        ws.Cell("F" + i).Value = "Cuenta";
                        i++;
                        foreach (var idemp in idempelados)
                        {
                            var datosbanco = banemp.DatosBancarios(idemp.idempleado);
                            if (datosbanco != null)
                            {
                                if (banco.IdBanco == datosbanco.idBanco)
                                {
                                    var DatosEmpleadoNomina = ctx.NOM_Nomina.Where(x => x.IdEmpleado == idemp.idempleado && x.IdPeriodo == IdPeriodo).FirstOrDefault();
                                    if (empresas.IdTabla == DatosEmpleadoNomina.IdEmpresaFiscal || empresas.IdTabla == DatosEmpleadoNomina.IdEmpresaAsimilado)
                                    {
                                        var datosBanco = ctx.DatosBancarios.Where(x => x.IdEmpleado == DatosEmpleadoNomina.IdEmpleado).FirstOrDefault();
                                        var nomina = rep.EmpleadoDispersion(IdPeriodo, idemp.idempleado);
                                        ws.Cell("C" + i).Value = nomina.nombres + " " + nomina.paterno + " " + nomina.materno;
                                        ws.Cell("D" + i).Value = nomina.importePagar;
                                        ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                                        ws.Cell("E" + i).Value = datosBanco.NoSigaF;
                                        ws.Cell("F" + i).Value = datosBanco.CuentaBancaria;
                                        total = total + nomina.importePagar;
                                        i++;
                                        //j = i;
                                    }
                                    else
                                    if (empresas.IdTabla == DatosEmpleadoNomina.IdEmpresaComplemento || empresas.IdTabla == DatosEmpleadoNomina.IdEmpresaSindicato)
                                    {
                                        var datosBanco = ctx.DatosBancarios.Where(x => x.IdEmpleado == DatosEmpleadoNomina.IdEmpleado).FirstOrDefault();
                                        var nomina = rep.EmpleadoDispersion(IdPeriodo, idemp.idempleado);
                                        if(nomina.totalComplemento != 0)
                                        {
                                            ws.Cell("C" + i).Value = nomina.nombres + " " + nomina.paterno + " " + nomina.materno;
                                            ws.Cell("D" + i).Value = nomina.totalComplemento;
                                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                                            ws.Cell("E" + i).Value = datosBanco.NoSigaC;
                                            ws.Cell("F" + i).Value = datosBanco.CuentaBancaria;
                                            total = total + nomina.totalComplemento;
                                            i++;
                                        }
                                        
                                    }


                                }   
                                ws.Cell("C" + i).Value = "Total";
                                ws.Cell("D" + i).Value = total;

                            }
                            
                        }
                        i++;
                        total = 0;
                    }

                    
                    if (empresas.RP != null)
                    {
                        
                        var factura = ctx.NOM_Facturacion.Where(x => x.IdEmpresaFi_As == empresas.IdTabla && x.IdPeriodo == IdPeriodo).FirstOrDefault();
                        if (factura != null)
                        {
                            i = i + 2;
                            ws.Cell("C" + i).Value = "Relativos";
                            ws.Range("C" + i + ":F" + i).Merge();
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Font.SetBold();
                            i = i + 1;

                            ws.Cell("C" + i).Value = "CUOTAS IMSS E INFONAVIT";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = factura.F_Cuota_IMSS_Infonavit;
                            ws.Cell("D"+i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "IMPUESTO SOBRE NOMINAS";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = factura.F_Impuesto_Nomina;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "AMORTIZACION INFONAVIT";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = factura.F_Amortizacion_Infonavit;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "FONACOT";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = factura.F_Fonacot;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            if (factura.F_Pension_Alimenticia > 0)
                            {
                                ws.Cell("C" + i).Value = "PENSION ALIMENTICIA";
                                ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                                ws.Cell("D" + i).Value = factura.F_Pension_Alimenticia;
                                ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                                i = i + 1;
                            }

                            ws.Cell("C" + i).Value = "RELATIVO";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = factura.F_Relativo;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "TOTAL PERCEPCIONES";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = factura.F_Total_Nomina;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "TOTAL NETO";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = factura.F_Total_Fiscal;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
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
                        var factura = ctx.NOM_FacturacionComplemento.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaC == empresas.IdTabla).ToList();
                        foreach (var f in factura)
                        {
                            cuotasImss = cuotasImss + f.C_Cuotas_IMSS_Infonavit;
                            impuestoNomina = impuestoNomina + f.C_Impuesto_Nomina;
                            relativos = relativos + f.C_Relativos;
                            percepciones = percepciones + f.C_Relativos;
                            totalComplemento = totalComplemento + f.C_Total_Complemento;
                        }
                        if (factura != null)
                        {
                            i = i + 2;
                            ws.Cell("C" + i).Value = "Relativos";
                            ws.Range("C" + i + ":F" + i).Merge();
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Font.SetBold();
                            i = i + 1;

                            ws.Cell("C" + i).Value = "CUOTAS IMSS E INFONAVIT";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = cuotasImss;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "IMPUESTO SOBRE NOMINAS";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = impuestoNomina;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;



                            ws.Cell("C" + i).Value = "RELATIVO";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = relativos;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "TOTAL PERCEPCIONES";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = percepciones;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                            ws.Cell("C" + i).Value = "TOTAL NETO";
                            ws.Cell("C" + i).Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                .Font.SetBold();
                            ws.Cell("D" + i).Value = totalComplemento;
                            ws.Cell("D" + i).Style.NumberFormat.Format = "$ #,##0.0000";
                            i = i + 1;
                        }
                    }
                    
                 
            
                    //ESTILOS DE LAS CELDAS 
                    //Titulo 
                    ws.Range("C2:F2").Merge();
                    ws.Cell("C2").Style
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Font.SetBold();

                    ws.Range("C3:F3").Merge();
                    ws.Cell("C3").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Range("C4:F4").Merge();
                    ws.Cell("C4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Columns("7,3").AdjustToContents();
                    ws.Columns("7,4").AdjustToContents();
                    ws.Columns("7,5").AdjustToContents();
                    ws.Columns("7,6").AdjustToContents();
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
