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
using RH.BLL;
using System.Data;
using System.Web;
using Excel;

namespace Nomina.Reportes
{
    public class Reportes_Nomina
    {
        // RHEntities ctx = null;

        public Reportes_Nomina()
        {
            // ctx = new RHEntities();
        }
        public string ExportarExcelReporteNomina(int idusuario, string ruta, NOM_PeriodosPago periodoPago)
        {
            List<DatosReporteNominas> datos;

            List<NOM_Nomina_Detalle> listaDetalles;
            List<NOM_Nomina> listaNominas;
            List<NOM_Cuotas_IMSS> listaCuotasImsss;

            using (var context = new RHEntities())
            {
                datos = (from emp in context.Empleado
                         join empPer in context.NOM_Empleado_PeriodoPago
                         on emp.IdEmpleado equals empPer.IdEmpleado
                         //join nom in ctx.NOM_Nomina
                         //on emp_per.IdPeriodoPago equals nom.IdPeriodo
                         where empPer.IdPeriodoPago == periodoPago.IdPeriodoPago
                         select new DatosReporteNominas
                         {
                             IdEmpleado = emp.IdEmpleado,
                             //IdNomina = nom.IdNomina,
                             Nombres = emp.Nombres,
                             Paterno = emp.APaterno,
                             Materno = emp.AMaterno
                         }).ToList();


                listaNominas = context.NOM_Nomina.Where(x => x.IdPeriodo == periodoPago.IdPeriodoPago).ToList();

                var arrayIdNominas = listaNominas.Select(x => x.IdNomina).ToArray();

                listaDetalles = (from d in context.NOM_Nomina_Detalle
                                 where arrayIdNominas.Contains(d.IdNomina)
                                 select d).ToList();


                listaCuotasImsss = (from c in context.NOM_Cuotas_IMSS
                                    where arrayIdNominas.Contains(c.IdNomina)
                                    select c).ToList();

            }


            var newruta = ValidarFolderUsuario(idusuario, ruta);
            newruta = newruta + "ReporteNomina_" + periodoPago.Descripcion + "_.xlsx";

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte Nominas ");
            ws.Cell("A1").Value = "ID EMPLEADO";
            ws.Cell("B1").Value = "EMPLEADO";
            ws.Cell("C1").Value = "PERCEPCIONES";
            ws.Cell("D1").Value = "DEDUCCIONES";
            ws.Cell("E1").Value = "COMPLEMENTO";
            ws.Cell("F1").Value = "SUELDOS";
            ws.Cell("G1").Value = "SALARIO DIARIO";
            ws.Cell("H1").Value = "SALARIO DIARIO I";
            ws.Cell("I1").Value = "SALARIO BASE C";
            ws.Cell("J1").Value = "SALARIO REAL";
            ws.Cell("K1").Value = "PENSION ALIMENTICIA";
            ws.Cell("L1").Value = "SUBSIDIO AL EMPLEO";
            ws.Cell("M1").Value = "IMPUESTO NOMINAS";
            ws.Cell("N1").Value = "CUOTA PATRONAL";
            ws.Cell("O1").Value = "CUOTA OBRERA";
            ws.Cell("P1").Value = "ISR";
            ws.Cell("Q1").Value = "INFONAVIT";
            ws.Cell("R1").Value = "FONACOT";
            ws.Cell("S1").Value = "TOTAL";


            int i = 2;
            foreach (var emp in datos)
            {
                if (emp == null) continue;

                // var nomina = ctx.NOM_Nomina.Where(x => x.IdPeriodo == idPeriodo && x.IdEmpleado == emp.IdEmpleado).FirstOrDefault();
                var nomina = listaNominas.FirstOrDefault(x => x.IdEmpleado == emp.IdEmpleado);

                if (nomina == null) continue;

                decimal totalPatron = 0;
                decimal totalObrero = 0;

                //var pension = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 48).FirstOrDefault();
                var pension = listaDetalles.FirstOrDefault(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 48);

                //var imss = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 42).FirstOrDefault();
                var imss = listaDetalles.FirstOrDefault(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 42);

                //var isr = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 43).FirstOrDefault();
                var isr = listaDetalles.FirstOrDefault(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 43);

                //var infonavit = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 51).FirstOrDefault();
                var infonavit = listaDetalles.FirstOrDefault(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 51);

                //var fonacot = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 52).FirstOrDefault();
                var fonacot = listaDetalles.FirstOrDefault(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 52);

                //var sueldo = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 1).FirstOrDefault();
                var sueldo = listaDetalles.FirstOrDefault(x => x.IdNomina == nomina.IdNomina && x.IdConcepto == 1);

                var itemCuota = listaCuotasImsss.FirstOrDefault(x => x.IdNomina == nomina.IdNomina);

                if (itemCuota != null)
                {
                    totalPatron = itemCuota.TotalPatron;
                    totalObrero = itemCuota.TotalObrero;
                }

                ws.Cell($"A{i}").Value = emp.IdEmpleado;
                ws.Cell($"B{i}").Value = emp.Paterno + emp.Materno + emp.Nombres;

                if (nomina != null)
                {
                    ws.Cell($"C{i}").Value = nomina.TotalPercepciones;
                    ws.Cell($"D{i}").Value = nomina.TotalDeducciones;
                    ws.Cell($"E{i}").Value = nomina.TotalComplemento;
                    ws.Cell($"F{i}").Value = sueldo == null ? 0 : sueldo.Total;
                    ws.Cell($"G{i}").Value = nomina.SD;
                    ws.Cell($"H{i}").Value = nomina.SDI;
                    ws.Cell($"I{i}").Value = nomina.SBC;
                    ws.Cell($"J{i}").Value = nomina.SDReal;
                    ws.Cell($"K{i}").Value = pension == null ? 0 : pension.Total;
                    ws.Cell($"L{i}").Value = nomina.SubsidioEntregado;
                    ws.Cell($"M{i}").Value = nomina.TotalImpuestoSobreNomina;
                    ws.Cell($"N{i}").Value = totalPatron;
                    ws.Cell($"O{i}").Value = totalObrero;
                    ws.Cell($"P{i}").Value = isr == null ? 0 : isr.Total;
                    ws.Cell($"Q{i}").Value = infonavit == null ? 0 : infonavit.Total;
                    ws.Cell($"R{i}").Value = fonacot == null ? 0 : fonacot.Total;
                    ws.Cell($"S{i}").Value = nomina.TotalNomina;
                    
                    i++;
                }
                else
                {
                    ws.Range($"C{i}:R{i}").Value = 0;
                    
                    i++;
                }
                
            }

            ws.Cell($"C{i}").FormulaA1 = $"=SUM(C2:C{i - 1})";
            ws.Cell($"D{i}").FormulaA1 = $"=SUM(D2:D{i - 1})";
            ws.Cell($"E{i}").FormulaA1 = $"=SUM(E2:E{i - 1})";
            ws.Cell($"F{i}").FormulaA1 = $"=SUM(F2:F{i - 1})";
            ws.Cell($"K{i}").FormulaA1 = $"=SUM(K2:K{i - 1})";
            ws.Cell($"L{i}").FormulaA1 = $"=SUM(L2:L{i - 1})";
            ws.Cell($"M{i}").FormulaA1 = $"=SUM(M2:M{i - 1})";
            ws.Cell($"N{i}").FormulaA1 = $"=SUM(N2:N{i - 1})";
            ws.Cell($"O{i}").FormulaA1 = $"=SUM(O2:O{i - 1})";
            ws.Cell($"P{i}").FormulaA1 = $"=SUM(P2:P{i - 1})";
            ws.Cell($"Q{i}").FormulaA1 = $"=SUM(Q2:Q{i - 1})";
            ws.Cell($"R{i}").FormulaA1 = $"=SUM(R2:R{i - 1})";
            ws.Cell($"S{i}").FormulaA1 = $"=SUM(S2:S{i - 1})";
            
            ws.Range($"C2:S{i}").Style.NumberFormat.Format = "$ #,##0.00";
            ws.Range("A1:S1").Style.Font.SetBold();
            ws.Range($"A{i}:S{i}").Style.Font.SetBold();
            
            ws.Columns("1:19").AdjustToContents();

            wb.SaveAs(newruta);


            return newruta;
        }

        public string CrearReporteAguinaldo(int idUsuario, string pathFolder, string pathDescarga, int idPeriodo)
        {
            NOM_PeriodosPago periodo;
            List<NOM_Aguinaldo> listaAguinaldos;
            List<Empleado> listaEmpleados;
            List<NOM_Nomina> listaNominas;
            using (var context = new RHEntities())
            {

                periodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                listaAguinaldos = context.NOM_Aguinaldo.Where(x => x.IdPeriodo == idPeriodo).ToList();

                var arrayIdEmpleados = listaAguinaldos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

                var arrayIdNominas = listaAguinaldos.Select(x => x.IdNomina).ToArray();

                listaNominas = (from n in context.NOM_Nomina
                                where arrayIdNominas.Contains(n.IdNomina)
                                select n).ToList();

            }


            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Aguinaldos");



            #region HEADER

            worksheet.Cell(1, 1).Value = $"AGUINALDOS {periodo.Fecha_Fin.Year}";
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 18;
            worksheet.Range("A1:E1").Merge();

            worksheet.Cell(1, 6).Value = periodo.Descripcion;
            worksheet.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 6).Style.Font.Bold = true;
            worksheet.Cell(1, 6).Style.Font.FontSize = 16;
            worksheet.Cell(1, 6).Style.Font.FontColor = XLColor.Olive;
            worksheet.Range("F1:K1").Merge();

            worksheet.Cell(3, 1).Value = "IDA";
            worksheet.Cell(3, 2).Value = "IDE";
            worksheet.Cell(3, 3).Value = "Nombre Empleado";
            //worksheet.Cell(3, 4).Value = "Materno";
            //worksheet.Cell(3, 5).Value = "Nombres";
            worksheet.Cell(3, 4).Value = "SD";
            worksheet.Cell(3, 5).Value = "SDI";
            worksheet.Cell(3, 6).Value = "SM";
            worksheet.Cell(3, 7).Value = "SR";
            worksheet.Cell(3,8).Value = "Dias Aguinaldo";
            worksheet.Cell(3,9).Value = "Dias Ejercicio";

            worksheet.Cell(3,10).Value = "Primer Dia";
            worksheet.Cell(3,11).Value = "Ultimo Dia";
            worksheet.Cell(3,12).Value = "Faltas";
            worksheet.Cell(3,13).Value = "Faltas Personalizadas";
            worksheet.Cell(3,14).Value = "Dias Trabajados";
            worksheet.Cell(3,15).Value = "Sueldo Mensual";
            worksheet.Cell(3,16).Value = "Proporcion";
            worksheet.Cell(3,17).Value = "Tope Exento";
            worksheet.Cell(3,18).Value = "Exento";
            worksheet.Cell(3,19).Value = "Gravado";

            worksheet.Cell(3,20).Value = "Aguinaldo";
            worksheet.Cell(3,21 ).Value = "ISR";
            worksheet.Cell(3,21).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3,22).Value = "Pension Alimenticia";
            worksheet.Cell(3,23).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3,23).Value = "Neto";
            worksheet.Cell(3,23).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3,24).Value = "Complemento";
            worksheet.Cell(3,25).Value = "Total";
            worksheet.Cell(3,25).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3,26).Value = "ISN";
            worksheet.Cell(3,26).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3,27).Value = "Factor";
            worksheet.Cell(3,27 ).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3,28).Value = "Fecha Reg";
            #endregion

            #region CONTENIDO
            int row = 4;
            var rowColor = false;

            listaEmpleados = listaEmpleados.OrderBy(x => x.APaterno).ToList();

            foreach (var itemEmpleado in listaEmpleados)
            {
                //var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == item.IdEmpleado);
                var item = listaAguinaldos.FirstOrDefault(x => x.IdEmpleado == itemEmpleado.IdEmpleado);

                if (itemEmpleado == null || item == null) continue;

                var itemN = listaNominas.FirstOrDefault(x => x.IdNomina == item.IdNomina);

                worksheet.Cell(row, 1).Value = item.IdAguinaldo;
                worksheet.Cell(row, 2).Value = item.IdEmpleado;
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 3).Value = $"{itemEmpleado.APaterno} {itemEmpleado.AMaterno} {itemEmpleado.Nombres}";
                worksheet.Cell(row, 3).Style.Font.Bold = true;
        

                worksheet.Cell(row, 4).Value = item.SD;
                worksheet.Cell(row, 5).Value = item.SDI;
                worksheet.Cell(row, 6).Value = item.SalarioMinimo;

                worksheet.Cell(row, 7).Value = itemN?.SDReal ?? 0;

                worksheet.Cell(row, 8).Value = item.DiasAguinaldo;
                worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 9).Value = item.DiasEjercicioFiscal;
                worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 10).Value = item.FechaPrimerDia;
                worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 11).Value = item.FechaUltimoDia;
                worksheet.Cell(row, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 12).Value = item.TotalFaltas;
                worksheet.Cell(row, 12).Style.Font.FontColor = XLColor.Red;
                worksheet.Cell(row, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 13).Value = item.FaltasPersonalizadas;
                worksheet.Cell(row, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 14).Value = item.DiasTrabajados;
                worksheet.Cell(row, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 15).Value = item.SueldoMensual;
                worksheet.Cell(row, 16).Value = item.Proporcion;
                worksheet.Cell(row, 17).Value = item.TopeExcento;

                worksheet.Cell(row, 18).Value = item.Exento;
                worksheet.Cell(row, 19).Value = item.Gravado;
                worksheet.Cell(row, 20).Value = item.Aguinaldo;
                worksheet.Cell(row, 20).Style.Font.Bold = true;
                worksheet.Cell(row, 21).Value = item.ISR;
                worksheet.Cell(row, 22).Value = item.PensionAlimenticia;
                worksheet.Cell(row, 23).Value = item.Neto;
                worksheet.Cell(row, 23).Style.Font.Bold = true;
                worksheet.Cell(row, 24).Value = item.Complemento;
                worksheet.Cell(row, 25).Value = item.Total;
                worksheet.Cell(row, 25).Style.Font.Bold = true;


                worksheet.Cell(row, 26).Value = item.ISN3;
                worksheet.Cell(row, 27).Value = item.Factor;
                worksheet.Cell(row, 27).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 28).Value = item.FechaReg;

                if (rowColor)
                {
                    // worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Lavender;

                    var rangoPintar = "A" + row + ":AA" + row;
                    worksheet.Range(rangoPintar).Style
                   .Fill.SetBackgroundColor(XLColor.Lavender);

                }

                row++;
                rowColor = !rowColor;
            }

            #endregion

            #region DISEÑO
            //Fix
            worksheet.SheetView.Freeze(3, 5);

            //Color
            worksheet.Range("A3:AA3").Style
               .Font.SetFontSize(14)
               .Font.SetBold(true)
               .Font.SetFontColor(XLColor.White)
               .Fill.SetBackgroundColor(XLColor.Teal);

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
            worksheet.Column(26).AdjustToContents();
            worksheet.Column(27).AdjustToContents();
            
            #endregion


            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = $"Aguinaldos_{periodo.Fecha_Fin.Year}.xlsx";

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);
            //  return fileName;
            return fileName; //pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;
        }



        private string ValidarFolderUsuario(int idUsuario, string pathFolder)
        {
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
    public class DatosReporteNominas
    {
        public int IdEmpleado { get; set; }
        public int IdNomina { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public decimal Percepciones { get; set; }
        public decimal Deducciones { get; set; }
        public decimal Complemento { get; set; }
        public decimal Sueldo { get; set; }
        public decimal SD { get; set; }
        public decimal SDI { get; set; }
        public decimal SBC { get; set; }
        public decimal PensionAlimenticia { get; set; }
        public decimal Subsidio { get; set; }
        public decimal ImpuestoNomina { get; set; }
        public decimal IMSS { get; set; }
        public decimal ISR { get; set; }
        public decimal Infonavit { get; set; }
        public decimal TotalNomina { get; set; }

    }
}
