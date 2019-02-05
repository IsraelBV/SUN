using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RH.Entidades;
using System.Diagnostics;
using System.Web;
using Excel;
using System.Data;
using Common.Utils;
using ClosedXML;
using ClosedXML.Excel;

namespace RH.BLL
{
    public class ExportarReporte
    {
        RHEntities ctx = null;

        public ExportarReporte()
        {
            ctx = new RHEntities();
        }
        public string CrearExcel(int idSucursal, string ruta, int idusuario, string nombreSucursal, string nombreCliente)
        {

            var newruta = ValidarFolderUsuario(idusuario, ruta);

            List<Empleado> listaEmpleados;
            List<Empleado_Contrato> listaContratos = new List<Empleado_Contrato>();
            List<C_TipoContrato_SAT> listaTipoContrato = new List<C_TipoContrato_SAT>();
            List<Puesto> listaPuestos = new List<Puesto>();
            List<C_PeriodicidadPago_SAT> listaPeriodicidadPagoSat = new List<C_PeriodicidadPago_SAT>();
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<DatosBancarios> listaDatosBancarios = new List<DatosBancarios>();
            List<C_Banco_SAT> listaBancos = new List<C_Banco_SAT>();

            using (var contexto = new RHEntities())
            {
                listaEmpleados = contexto.Empleado.Where(x => x.IdSucursal == idSucursal).ToList();

                if (listaEmpleados != null)
                {
                    var arrayIdEmpleados = listaEmpleados.Select(x => x.IdEmpleado).ToArray();

                    listaContratos = (from c in contexto.Empleado_Contrato
                                      where arrayIdEmpleados.Contains(c.IdEmpleado)
                                                && c.Status == true
                                      orderby c.IdContrato descending
                                      select c).ToList();

                    listaTipoContrato = contexto.C_TipoContrato_SAT.ToList();


                    var arrayIdPuesto = listaContratos.Select(x => x.IdPuesto).ToArray();

                    listaPuestos = (from p in contexto.Puesto
                                    where arrayIdPuesto.Contains(p.IdPuesto)
                                    select p).ToList();

                    listaPeriodicidadPagoSat = contexto.C_PeriodicidadPago_SAT.ToList();

                    listaEmpresas = contexto.Empresa.ToList();

                    listaDatosBancarios = (from db in contexto.DatosBancarios
                                           where arrayIdEmpleados.Contains(db.IdEmpleado)
                                           select db).ToList();


                    listaBancos = contexto.C_Banco_SAT.ToList();
                }
                else
                {
                    return "";
                }
            }


            newruta = newruta + "ReporteEmpleados " + nombreSucursal+"-"+nombreCliente + "_.xlsx";
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Exportacion");

            ws.Cell("A1").Value = "PATERNO";
            ws.Cell("B1").Value = "MATERNO";
            ws.Cell("C1").Value = "NOMBRES";
            ws.Cell("D1").Value = "FECHA DE NACIMIENTO";
            ws.Cell("E1").Value = "SEXO";
            ws.Cell("F1").Value = "RFC";
            ws.Cell("G1").Value = "CURP";
            ws.Cell("H1").Value = "NSS";
            ws.Cell("I1").Value = "UMF";
            ws.Cell("J1").Value = "NACIONALIDAD";
            ws.Cell("K1").Value = "ESTADO DE ORIGEN";
            ws.Cell("L1").Value = "DIRECCION";
            ws.Cell("M1").Value = "TELEFONO";
            ws.Cell("N1").Value = "CELULAR";
            ws.Cell("O1").Value = "EMAIL";
            ws.Cell("P1").Value = "EDO CIVIL";
            ws.Cell("Q1").Value = "FECHA ALTA";
            ws.Cell("R1").Value = "FECHA REAL";
            ws.Cell("S1").Value = "FECHA IMSS";
            ws.Cell("T1").Value = "FECHA BAJA";
            ws.Cell("U1").Value = "TIPO CONTRATO";
            ws.Cell("V1").Value = "DIAS CONTRATO";
            ws.Cell("W1").Value = "VIGENCIA";
            ws.Cell("X1").Value = "PUESTO";
            ws.Cell("Y1").Value = "TURNO";
            ws.Cell("Z1").Value = "DESCANSO";
            ws.Cell("AA1").Value = "PERIOCIDAD DE PAGO";
            ws.Cell("AB1").Value = "METODO DE PAGO";
            ws.Cell("AC1").Value = "SD";
            ws.Cell("AD1").Value = "SDI";
            ws.Cell("AE1").Value = "SBC";
            ws.Cell("AF1").Value = "SALARIO REAL";
            ws.Cell("AG1").Value = "EMPRESA FISCAL";
            ws.Cell("AH1").Value = "EMPRESA COMPLEMENTO";
            ws.Cell("AI1").Value = "EMPRESA SINDICATO";
            ws.Cell("AJ1").Value = "ASIMILADO";
            ws.Cell("AK1").Value = "NO SIGA FISCAL";
            ws.Cell("AL1").Value = "NO SIGA COMPLEMENTO";
            ws.Cell("AM1").Value = "CUENTA BANCARIA";
            ws.Cell("AN1").Value = "# TARJETA";
            ws.Cell("AO1").Value = "CLABE";
            ws.Cell("AP1").Value = "BANCO";
            ws.Cell("AQ1").Value = "TIPO DE JORNADA";
            ws.Cell("AR1").Value = "TIPO DE SALARIO";
            ws.Cell("AS1").Value = "EDO SERVICIO";
            ws.Cell("AT1").Value = "SINDICALIZADO";
            int i = 2;

            foreach (var emp in listaEmpleados)
            {
                var puesto = "";
                var tipocontrato = "";
                var periocidad = "";
                var empresaF = "";
                var empresaC = "";
                var empresaA = "";
                var empresaS = "";
                var banco = "--";

                var contrato = listaContratos.FirstOrDefault(x => x.IdEmpleado == emp.IdEmpleado);

                if (contrato != null)

                {
                    tipocontrato =listaTipoContrato.Where(x => x.IdTipoContrato == contrato.TipoContrato)
                            .Select(x => x.Descripcion)
                            .FirstOrDefault();

                    puesto = listaPuestos.Where(x => x.IdPuesto == contrato.IdPuesto).Select(x => x.Descripcion).FirstOrDefault();

                    periocidad = listaPeriodicidadPagoSat.Where(x => x.IdPeriodicidadPago == contrato.IdPeriodicidadPago)
                            .Select(x => x.Descripcion)
                            .FirstOrDefault();
                    empresaF = listaEmpresas.Where(x => x.IdEmpresa == contrato.IdEmpresaFiscal)
                            .Select(x => x.RazonSocial)
                            .FirstOrDefault();
                    empresaC = listaEmpresas.Where(x => x.IdEmpresa == contrato.IdEmpresaComplemento)
                            .Select(x => x.RazonSocial)
                            .FirstOrDefault();
                    empresaA = listaEmpresas.Where(x => x.IdEmpresa == contrato.IdEmpresaAsimilado)
                            .Select(x => x.RazonSocial)
                            .FirstOrDefault();
                    empresaS = listaEmpresas.Where(x => x.IdEmpresa == contrato.IdEmpresaSindicato)
                            .Select(x => x.RazonSocial)
                            .FirstOrDefault();
                }

                var datosBanco = listaDatosBancarios.FirstOrDefault(x => x.IdEmpleado == emp.IdEmpleado);

                //var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == emp.IdEmpleado).FirstOrDefault();
                //var tipocontrato = ctx.C_TipoContrato_SAT.Where(x => x.IdTipoContrato == contrato.TipoContrato).Select(x => x.Descripcion).FirstOrDefault();
                //var puesto = ctx.Puesto.Where(x => x.IdPuesto == contrato.IdPuesto).Select(x => x.Descripcion).FirstOrDefault();
                //var periocidad = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == contrato.IdPeriodicidadPago).Select(x => x.Descripcion).FirstOrDefault();
                //var empresaF = ctx.Empresa.Where(x => x.IdEmpresa == contrato.IdEmpresaFiscal).Select(x => x.RazonSocial).FirstOrDefault();
                //var empresaC = ctx.Empresa.Where(x => x.IdEmpresa == contrato.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault();
                //var empresaA = ctx.Empresa.Where(x => x.IdEmpresa == contrato.IdEmpresaAsimilado).Select(x => x.RazonSocial).FirstOrDefault();
                //var empresaS = ctx.Empresa.Where(x => x.IdEmpresa == contrato.IdEmpresaSindicato).Select(x => x.RazonSocial).FirstOrDefault();
                //var datosBanco = ctx.DatosBancarios.Where(x => x.IdEmpleado == emp.IdEmpleado).FirstOrDefault();

                if (datosBanco != null)
                {
                    banco = listaBancos.Where(x => x.IdBanco == datosBanco.IdBanco).Select(x => x.Descripcion).FirstOrDefault();
                }


                ws.Cell("A" + i).Value = emp.APaterno;
                ws.Cell("B" + i).Value = emp.AMaterno;
                ws.Cell("C" + i).Value = emp.Nombres;
                ws.Cell("D" + i).Value = emp.FechaNacimiento;
                ws.Cell("E" + i).Value = emp.Sexo == "H" ? "Hombre" : "Mujer";
                ws.Cell("F" + i).Value = emp.RFC;
                ws.Cell("G" + i).Value = emp.CURP;
                ws.Cell("H" + i).Value = emp.NSS;
                ws.Cell("I" + i).Value = contrato != null ? contrato.UMF : "";
                ws.Cell("J" + i).Value = emp.Nacionalidad;
                ws.Cell("K" + i).Value = emp.Estado;
                ws.Cell("L" + i).Value = emp.Direccion;
                ws.Cell("M" + i).Value = emp.Telefono;
                ws.Cell("N" + i).Value = emp.Celular;
                ws.Cell("O" + i).Value = emp.Email;
                ws.Cell("P" + i).Value = emp.EstadoCivil;
                ws.Cell("Q" + i).Value = contrato?.FechaAlta.ToString("d") ?? "";
                ws.Cell("R" + i).Value = contrato?.FechaReal.ToString("d")??"";

                if (contrato != null)
                {
                    ws.Cell("S" + i).Value = contrato.FechaIMSS?.ToString("d") ?? "";
                    ws.Cell("T" + i).Value = contrato.FechaBaja?.ToString("d") ?? "";
                }
                else
                {
                    ws.Cell("S" + i).Value =  "";
                    ws.Cell("T" + i).Value =  "";
                }


                ws.Cell("U" + i).Value = tipocontrato;
                ws.Cell("V" + i).Value = contrato?.DiasContrato??0;

                if (contrato != null)
                {
                    ws.Cell("W" + i).Value = contrato.Vigencia?.ToString("d") ?? "";
                }
                else
                {
                    ws.Cell("W" + i).Value = "";
                }
                ws.Cell("X" + i).Value = puesto;
                ws.Cell("Y" + i).Value = contrato != null ? UtilsEmpleados.SeleccionarTurno(contrato.Turno) : "-";
                ws.Cell("Z" + i).Value = contrato != null ? UtilsEmpleados.selectDay(contrato.DiaDescanso) : "-";
                ws.Cell("AA" + i).Value = periocidad;
                ws.Cell("AB" + i).Value = contrato != null ? UtilsEmpleados.SeleccionarFormaPagoById(contrato.FormaPago) : "-";
                ws.Cell("AC" + i).Value = contrato?.SD??0;
                ws.Cell("AD" + i).Value = contrato?.SDI??0;
                ws.Cell("AE" + i).Value = contrato?.SBC??0;
                ws.Cell("AF" + i).Value = contrato?.SalarioReal??0;
                ws.Cell("AG" + i).Value = empresaF;
                ws.Cell("AH" + i).Value = empresaC;
                ws.Cell("AI" + i).Value = empresaS;
                ws.Cell("AJ" + i).Value = empresaA;
                ws.Cell("AK" + i).Value = datosBanco == null ? 0 : datosBanco.NoSigaF;
                ws.Cell("AL" + i).Value = datosBanco == null ? 0 : datosBanco.NoSigaC;

                if (datosBanco != null)
                {
                    ws.Cell("AM" + i).Value = !string.IsNullOrEmpty(datosBanco.CuentaBancaria) ? datosBanco.CuentaBancaria : "0";
                    ws.Cell("AN" + i).Value = !string.IsNullOrEmpty(datosBanco.NumeroTarjeta) ? datosBanco.NumeroTarjeta : "0";
                    ws.Cell("AO" + i).Value = datosBanco == null ? "" : datosBanco.Clabe;
                }
                else
                {
                    ws.Cell("AM" + i).Value = 0;
                    ws.Cell("AN" + i).Value = 0;
                    ws.Cell("AO" + i).Value = "";
                }

                ws.Cell("AP" + i).Value = banco;
                ws.Cell("AQ" + i).Value = contrato != null ? UtilsEmpleados.SeleccionarTipoSemanaById(contrato.IdTipoJornada) :"-";
                ws.Cell("AR" + i).Value = contrato != null ? UtilsEmpleados.TipoSalario(contrato.TipoSalario) : "-";
                ws.Cell("AS" + i).Value = contrato?.EntidadDeServicio??"";

                if (contrato != null)
                {
                    ws.Cell("AT" + i).Value = contrato.Sindicalizado == false ? "NO" : "Si";
                }
                else
                {
                    ws.Cell("AT" + i).Value = "-";
                }
               
                i++;
            }

            ws.Columns("2,1").AdjustToContents();
            ws.Columns("2,2").AdjustToContents();
            ws.Columns("2,3").AdjustToContents();
            ws.Columns("2,4").AdjustToContents();
            ws.Columns("2,5").AdjustToContents();
            ws.Columns("2,6").AdjustToContents();
            ws.Columns("2,7").AdjustToContents();
            ws.Columns("2,8").AdjustToContents();
            ws.Columns("2,9").AdjustToContents();
            ws.Columns("2,10").AdjustToContents();
            ws.Columns("2,11").AdjustToContents();
            ws.Columns("2,12").AdjustToContents();
            ws.Columns("2,13").AdjustToContents();
            ws.Columns("2,14").AdjustToContents();
            ws.Columns("2,15").AdjustToContents();
            ws.Columns("2,16").AdjustToContents();
            ws.Columns("2,17").AdjustToContents();
            ws.Columns("2,18").AdjustToContents();
            ws.Columns("2,19").AdjustToContents();
            ws.Columns("2,20").AdjustToContents();
            ws.Columns("2,21").AdjustToContents();
            ws.Columns("2,22").AdjustToContents();
            ws.Columns("2,23").AdjustToContents();
            ws.Columns("2,24").AdjustToContents();
            ws.Columns("2,25").AdjustToContents();
            ws.Columns("2,26").AdjustToContents();
            ws.Columns("2,27").AdjustToContents();
            ws.Columns("2,28").AdjustToContents();
            ws.Columns("2,29").AdjustToContents();
            ws.Columns("2,30").AdjustToContents();
            ws.Columns("2,31").AdjustToContents();
            ws.Columns("2,32").AdjustToContents();
            ws.Columns("2,33").AdjustToContents();
            ws.Columns("2,34").AdjustToContents();
            ws.Columns("2,35").AdjustToContents();
            ws.Columns("2,36").AdjustToContents();
            ws.Columns("2,37").AdjustToContents();
            ws.Columns("2,38").AdjustToContents();
            ws.Columns("2,39").AdjustToContents();
            ws.Columns("2,40").AdjustToContents();
            ws.Columns("2,41").AdjustToContents();
            ws.Columns("2,42").AdjustToContents();
            ws.Columns("2,43").AdjustToContents();
            ws.Columns("2,44").AdjustToContents();
            ws.Columns("2,45").AdjustToContents();
            ws.Columns("2,46").AdjustToContents();
            ws.Columns("2,47").AdjustToContents();
            ws.Columns("2,48").AdjustToContents();

            ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("C1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("D1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("B1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("E1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("F1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("G1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("H1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("I1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("J1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("K1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("L1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("M1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("N1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("O1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("P1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("Q1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("R1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("S1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("T1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("U1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("V1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("W1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("X1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("Y1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("Z1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AA1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AB1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AC1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AD1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AE1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AF1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AG1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AH1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AI1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AJ1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AK1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AL1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AM1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AN1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AO1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AP1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AQ1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AR1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AS1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("AT1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
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
        //public int FormarPlantilla(int idSucursal, string fileName, bool activos)
        //{
        //    using (var wb = new XLWorkbook(fileName))
        //    {
        //        var wsMain = wb.Worksheet(1);

        //        Empleados e = new Empleados();
        //        var datos = e.GetInfoEmpleadosReporte(idSucursal, activos);
        //        int row = 2;
        //        foreach (var item in datos)
        //        {
        //            wsMain.Cell(row, 1).Value = item.Nombres;
        //            wsMain.Cell(row, 2).Value = item.Paterno;
        //            wsMain.Cell(row, 3).Value = item.Materno;
        //            wsMain.Cell(row, 4).Value = item.FechaAlta;
        //            wsMain.Cell(row, 5).Value = item.Puesto;
        //            wsMain.Cell(row, 6).Value = item.EmpresaFiscal;
        //            wsMain.Cell(row, 7).Value = item.EmpresaComplemento;
        //            wsMain.Cell(row, 8).Value = item.EmpresaAsimilado;
        //            wsMain.Cell(row, 9).Value = item.EmpresaSindicato;
        //            row++;
        //        }


        //        wb.SaveAs(fileName);
        //    }
        //    return 1;
        //}
    }

}
