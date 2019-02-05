using ClosedXML.Excel;
using RH.Entidades;
using Nomina.Procesador.Metodos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Utils;
using System.Data.Entity;

namespace Nomina.Reportes
{
    public class Reporte_Acumulado
    {
        RHEntities ctx = null;

        public Reporte_Acumulado()
        {
            ctx = new RHEntities();
        }

        public string crearAcumulado(int idusuario, string ruta, int[] arrayIdPeriodo)
        {



            //select n.IdNomina, n.IdCliente, n.IdSucursal, n.IdPeriodo, n.IdEmpleado, n.TotalNomina, n.TotalPercepciones, n.TotalDeducciones, 
            //n.TotalComplemento, n.TotalImpuestoSobreNomina, n.SubsidioCausado, n.SubsidioEntregado, n.ISRantesSubsidio,
            //n.SD, n.SBC, n.SMGV, n.UMA,

            //con.DescripcionCorta,
            //nd.IdConcepto,nd.Total ,nd.GravadoISR,nd.ExentoISR,nd.IntegraIMSS,nd.ImpuestoSobreNomina,nd.IdPrestamo

            //,ci.TotalPatron,ci.TotalObrero,ci.Cuota_Fija_Patron,ci.Excedente_Patron,ci.Excedente_Obrero,ci.PrestacionesDinero_Patron,ci.PrestacionesDinero_Obrero,ci.Pensionados_Patron
            //,ci.Pensionados_Obrero,ci.InvalidezVida_Patron,ci.InvalidezVida_Obrero ,ci.GuarderiasPrestaciones_Patron
            //,ci.SeguroRetiro_Patron,ci.CesantiaVejez_Patron
            //,ci.CesantiaVejez_Obrero,ci.Infonavit_Patron,ci.RiesgoTrabajo_Patron




            return "";

            //var newruta = ValidarFolderUsuario(idusuario, ruta);
            //newruta = newruta + "Acumulado.xlsx";
            //var wb = new XLWorkbook();
            //var ws = wb.Worksheets.Add("Acumulado");
            //#region Encabezado excel

            //ws.Cell("A3").Value = "# REGISTRO";
            //ws.Cell("B3").Value = "ID EMPELADO";
            //ws.Cell("C3").Value = "EMPLEADO";
            //ws.Cell("D3").Value = "RFC";
            //ws.Cell("E3").Value = "CURP";
            //ws.Cell("F3").Value = "DIAS LABORADOS";

            //ws.Cell("G2").Value = "SUELDO";
            //ws.Cell("G3").Value = "TOTAL";
            //ws.Cell("H3").Value = "GRAVADO";
            //ws.Cell("I3").Value = "EXENTO";

            //ws.Cell("J2").Value = "PREMIO POR ASISTENCIA";
            //ws.Cell("J3").Value = "TOTAL";
            //ws.Cell("K3").Value = "GRAVADO";
            //ws.Cell("L3").Value = "EXENTO";

            //ws.Cell("M2").Value = "PREMIO POR PUNTUALIDAD";
            //ws.Cell("M3").Value = "TOTAL";
            //ws.Cell("N3").Value = "GRAVADO";
            //ws.Cell("O3").Value = "EXENTO";

            //ws.Cell("P2").Value = "AGUINALDO";
            //ws.Cell("P3").Value = "TOTAL";
            //ws.Cell("Q3").Value = "GRAVADO";
            //ws.Cell("R3").Value = "EXENTO";

            //ws.Cell("S2").Value = "VACACIONES";
            //ws.Cell("S3").Value = "TOTAL";
            //ws.Cell("T3").Value = "GRAVADO";
            //ws.Cell("U3").Value = "EXENTO";

            //ws.Cell("V2").Value = "PRIMA VACACIONAL";
            //ws.Cell("V3").Value = "TOTAL";
            //ws.Cell("W3").Value = "GRAVADO";
            //ws.Cell("X3").Value = "EXENTO";

            //ws.Cell("Y2").Value = "OTROS INGRESOS";
            //ws.Cell("Y3").Value = "TOTAL";
            //ws.Cell("Z3").Value = "GRAVADO";
            //ws.Cell("AA3").Value = "EXENTO";

            //ws.Cell("AB2").Value = "SUBSIDIO";
            //ws.Cell("AB3").Value = "TOTAL";
            //ws.Cell("AC3").Value = "GRAVADO";
            //ws.Cell("AD3").Value = "EXENTO";

            //ws.Cell("AE3").Value = "TOTAL DE PERCEPCIONES";
            ////DEDUCCIONES
            //ws.Cell("AF2").Value = "FONACOT";
            //ws.Cell("AF3").Value = "TOTAL";
            //ws.Cell("AG3").Value = "GRAVADO";
            //ws.Cell("AH3").Value = "EXENTO";
            //ws.Cell("AI2").Value = "INFONAVIT";
            //ws.Cell("AI3").Value = "TOTAL";
            //ws.Cell("AJ3").Value = "GRAVADO";
            //ws.Cell("AK3").Value = "EXENTO";
            //ws.Cell("AL2").Value = "ISR";
            //ws.Cell("AL3").Value = "TOTAL";
            //ws.Cell("AM3").Value = "GRAVADO";
            //ws.Cell("AN3").Value = "EXENTO";
            //ws.Cell("AO2").Value = "I.M.S.S";
            //ws.Cell("AO3").Value = "TOTAL";
            //ws.Cell("AP3").Value = "GRAVADO";
            //ws.Cell("AQ3").Value = "EXENTO";
            //ws.Cell("AR3").Value = "TOTAL DE DEDUCCIONES";
            //ws.Cell("AS2").Value = "CUOTA FIJA";
            //ws.Cell("AS3").Value = "PATRON";
            //ws.Cell("AT3").Value = "OBRERO";
            //ws.Cell("AT2").Value = "EXCEDENTE";
            //ws.Cell("AU3").Value = "PATRON";
            //ws.Cell("AV3").Value = "OBRERO";
            //ws.Cell("AW2").Value = "PRESTACIONES";
            //ws.Cell("AW3").Value = "PATRON";
            //ws.Cell("AX3").Value = "OBRERO";
            //ws.Cell("AY2").Value = "PENSIONADOS";
            //ws.Cell("AY3").Value = "PATRON";
            //ws.Cell("AZ3").Value = "OBRERO";
            //ws.Cell("BA2").Value = "INVALIDEZ VIDA";
            //ws.Cell("BA3").Value = "PATRON";
            //ws.Cell("BB3").Value = "OBRERO";            
            //ws.Cell("BC2").Value = "PRESTACION GUARDERIA";
            //ws.Cell("BC3").Value = "PATRON";
            //ws.Cell("BD3").Value = "OBRERO";
            //ws.Cell("BE2").Value = "SEGURO RETIRO";
            //ws.Cell("BE3").Value = "PATRON";
            //ws.Cell("BF3").Value = "OBRERO";
            //ws.Cell("BG2").Value = "CESANTIA VEJEZ";
            //ws.Cell("BG3").Value = "PATRON";
            //ws.Cell("BH3").Value = "OBRERO";
            //ws.Cell("BI2").Value = "INFONAVIT";
            //ws.Cell("BI3").Value = "PATRON";
            //ws.Cell("BJ3").Value = "OBRERO";
            //ws.Cell("BK2").Value = "RIESGO TRABAJO";
            //ws.Cell("BK3").Value = "PATRON";
            //ws.Cell("BL3").Value = "OBRERO";
            //ws.Cell("BM3").Value = "TOTAL NOMINA";
            //ws.Cell("BN3").Value = "PERIODO";
            //ws.Cell("BO3").Value = "SUCURSAL";
            //#endregion
            //int i = 4;
            //int conteoEmp = 1;
            //List<acumulado> acumu = new List<acumulado>();
            //foreach (var idperiod in idPeriodo)
            //{
            //    var idemp = ctx.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == idperiod).ToList();

            //    foreach (var id in idemp)
            //    {
            //        var dato = (from e in ctx.Empleado
            //                    join ec in ctx.Empleado_Contrato
            //                    on e.IdEmpleado equals ec.IdEmpleado
            //                    join n in ctx.NOM_Nomina
            //                    on e.IdEmpleado equals n.IdEmpleado
            //                    join ep in ctx.NOM_Empleado_PeriodoPago
            //                    on e.IdEmpleado equals ep.IdEmpleado

            //                    where e.IdEmpleado == id.IdEmpleado && n.IdPeriodo == idperiod
            //                    select new acumulado
            //                    {
            //                        ID = e.IdEmpleado,
            //                        Id_Nomina = n.IdNomina,
            //                        EMPLEADO = e.APaterno + " " + e.AMaterno + " " + e.Nombres,
            //                        RFC = e.RFC,
            //                        CURP = e.CURP,
            //                        SD = n.SD,
            //                        SDI = n.SDI,
            //                        Dias_Laborados = n.Dias_Laborados,
            //                        TOTAL_PERCEPCION = n.TotalPercepciones,
            //                        TOTAL_DEDUCCION = n.TotalDeducciones,
            //                        TOTAL_TOTAL = n.TotalNomina,
            //                        IMSS = ctx.NOM_Nomina_Detalle.Where(x => x.IdConcepto == 42 && x.IdNomina == n.IdNomina).Select(x => x.Total).FirstOrDefault(),
            //                        ISR = ctx.NOM_Nomina_Detalle.Where(x => x.IdConcepto == 43 && x.IdNomina == n.IdNomina).Select(x => x.Total).FirstOrDefault(),
            //                        TOTAL_NETO = n.TotalNomina,
            //                        IdSucursal = e.IdSucursal

            //                    }).FirstOrDefault();

            //        if(dato != null)
            //        {

            //            var percepciones = (from nomina in ctx.NOM_Nomina
            //                                join nom_det in ctx.NOM_Nomina_Detalle
            //                                on nomina.IdNomina equals nom_det.IdNomina
            //                                join con in ctx.C_NOM_Conceptos
            //                                on nom_det.IdConcepto equals con.IdConcepto
            //                                where nomina.IdPeriodo == idperiod && nom_det.IdNomina == dato.Id_Nomina
            //                                select new acumulado
            //                                {
            //                                    IDCONCEPTO = con.IdConcepto,
            //                                    CONCEPTO = con.DescripcionCorta,
            //                                    TIPO_CONCEPTO = con.TipoConcepto,
            //                                    CONCEPTO_TOTAL = nom_det.Total,
            //                                    CONCEPTO_GRAVADO = nom_det.GravadoISR,
            //                                    CONCEPTO_EXCENTO = nom_det.ExentoISR

            //                                }).ToList();





            //            var dato2 = (from e in ctx.Empleado
            //                         join n in ctx.NOM_Nomina
            //                         on e.IdEmpleado equals n.IdEmpleado
            //                         join imss in ctx.NOM_Cuotas_IMSS
            //                         on n.IdNomina equals imss.IdNomina
            //                         where e.IdEmpleado == id.IdEmpleado
            //                         select new acumulado
            //                         {
            //                             CUOTA_FIJA = imss.Cuota_Fija_Patron,
            //                             EXCEDENTE_PATRON = imss.Excedente_Patron,
            //                             EXCENDENTE_OBRERO = imss.Excedente_Obrero,
            //                             PRESTACIONES_PATRON = imss.PrestacionesDinero_Patron,
            //                             PRESTACION_OBRERO = imss.PrestacionesDinero_Obrero,
            //                             PENSIONADOS_PATRON = imss.Pensionados_Patron,
            //                             PENSIONADOS_OBRERA = imss.Pensionados_Obrero,
            //                             INALIDEZ_VIDA_PATRON = imss.InvalidezVida_Patron,
            //                             INVALIDEZ_VIDA_OBRERA = imss.InvalidezVida_Obrero,
            //                             PRESTACION_GUARDERIA_PATRON = imss.PrestacionesDinero_Patron,
            //                             SEGURO_RETIRO_PATRON = imss.SeguroRetiro_Patron,
            //                             CESANTIA_VEJEZ_PATRON = imss.CesantiaVejez_Patron,
            //                             CESANTIA_VEJEZ_OBRERO = imss.CesantiaVejez_Obrero,
            //                             INFONAVIT_PATRON = imss.Infonavit_Patron,
            //                             RIESGO_TRABAJO = imss.RiesgoTrabajo_Patron,
            //                             PERIODO = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == n.IdPeriodo).Select(x => x.Descripcion).FirstOrDefault(),

            //                         }).FirstOrDefault();

            //            var periodo = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idperiod).Select(x => x.Descripcion).FirstOrDefault();
            //            var sucursal = ctx.Sucursal.Where(x => x.IdSucursal == dato.IdSucursal).FirstOrDefault();
            //            var cliente = ctx.Cliente.Where(x => x.IdCliente == sucursal.IdCliente).FirstOrDefault();


            //            var sueldoTotal = percepciones.Where(x => x.IDCONCEPTO == 1).Select(x => x.CONCEPTO_TOTAL).FirstOrDefault();
            //            var sueldoGravado = percepciones.Where(x => x.IDCONCEPTO == 1).Select(x => x.CONCEPTO_GRAVADO).FirstOrDefault();
            //            var sueldoExento = percepciones.Where(x => x.IDCONCEPTO == 1).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var premioAsiTotal = percepciones.Where(x => x.IDCONCEPTO == 40).Select(x => x.CONCEPTO_TOTAL).FirstOrDefault();
            //            var premioAsiGravado = percepciones.Where(x => x.IDCONCEPTO == 40).Select(x => x.CONCEPTO_GRAVADO).FirstOrDefault();
            //            var premioAsiExento = percepciones.Where(x => x.IDCONCEPTO == 40).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var premioPunTotal = percepciones.Where(x => x.IDCONCEPTO == 8).Select(x => x.CONCEPTO_TOTAL).FirstOrDefault();
            //            var premioPunGravado = percepciones.Where(x => x.IDCONCEPTO == 8).Select(x => x.CONCEPTO_GRAVADO).FirstOrDefault();
            //            var premioPunExcento = percepciones.Where(x => x.IDCONCEPTO == 8).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var premioAguTotal = percepciones.Where(x => x.IDCONCEPTO == 2).Select(x => x.CONCEPTO_TOTAL).FirstOrDefault();
            //            var premioAguGravado = percepciones.Where(x => x.IDCONCEPTO == 2).Select(x => x.CONCEPTO_GRAVADO).FirstOrDefault();
            //            var premioAguExcento = percepciones.Where(x => x.IDCONCEPTO == 2).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var vacacionesTotal = percepciones.Where(x => x.IDCONCEPTO == 148).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var vacacionesGravado = percepciones.Where(x => x.IDCONCEPTO == 148).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var vacacionesExcento = percepciones.Where(x => x.IDCONCEPTO == 148).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var primaTotal = percepciones.Where(x => x.IDCONCEPTO == 16).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var primaGravado = percepciones.Where(x => x.IDCONCEPTO == 16).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var primaExento = percepciones.Where(x => x.IDCONCEPTO == 16).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var otrosTotal = percepciones.Where(x => x.IDCONCEPTO == 33).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var otrosGravado = percepciones.Where(x => x.IDCONCEPTO == 33).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var otrosExento = percepciones.Where(x => x.IDCONCEPTO == 33).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var subsidioTotal = percepciones.Where(x => x.IDCONCEPTO == 144).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var subsidioGravado = percepciones.Where(x => x.IDCONCEPTO == 144).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var subsidioExento = percepciones.Where(x => x.IDCONCEPTO == 144).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var subsidio = ctx.NOM_Nomina.Where(x => x.IdNomina == dato.Id_Nomina).Select(x => x.SubsidioEntregado).FirstOrDefault();
            //            // deducciones
            //            var fonacotTotal = percepciones.Where(x => x.IDCONCEPTO == 52).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var fonacotGravado = percepciones.Where(x => x.IDCONCEPTO == 52).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var fonacotExento = percepciones.Where(x => x.IDCONCEPTO == 52).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var infonavitTotal = percepciones.Where(x => x.IDCONCEPTO == 51).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var infonavitGravado = percepciones.Where(x => x.IDCONCEPTO == 51).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var infonavitExento = percepciones.Where(x => x.IDCONCEPTO == 51).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var isrTotal = percepciones.Where(x => x.IDCONCEPTO == 43).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var isrGravado = percepciones.Where(x => x.IDCONCEPTO == 43).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var isrExento = percepciones.Where(x => x.IDCONCEPTO == 43).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var imssTotal = percepciones.Where(x => x.IDCONCEPTO == 42).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var imssGravado = percepciones.Where(x => x.IDCONCEPTO == 42).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();
            //            var imssExento = percepciones.Where(x => x.IDCONCEPTO == 42).Select(x => x.CONCEPTO_EXCENTO).FirstOrDefault();

            //            ws.Cell("A" + i).Value = conteoEmp;
            //            ws.Cell("B" + i).Value = dato.ID;
            //            ws.Cell("C" + i).Value = dato.EMPLEADO;
            //            ws.Cell("D" + i).Value = dato.RFC;
            //            ws.Cell("E"+ i).Value = dato.CURP;
            //            ws.Cell("F"+ i).Value = dato.Dias_Laborados;

            //            ws.Cell("G"+ i).Value = sueldoTotal == 0 ? 0 : sueldoTotal;
            //            ws.Cell("H"+ i).Value = sueldoGravado == 0 ? 0 : sueldoGravado;
            //            ws.Cell("I"+ i).Value = sueldoExento == 0 ? 0 : sueldoExento;

            //            ws.Cell("J"+ i).Value = premioAsiTotal == 0 ? 0 : premioAsiTotal;
            //            ws.Cell("K"+ i).Value = premioAsiGravado == 0 ? 0 : premioAsiGravado;
            //            ws.Cell("L"+ i).Value = premioAsiExento == 0 ? 0 : premioAsiExento;

            //            ws.Cell("M"+ i).Value = premioPunTotal == 0 ? 0 : premioPunTotal;
            //            ws.Cell("N"+ i).Value = premioPunGravado == 0 ? 0 : premioPunGravado;
            //            ws.Cell("O"+ i).Value = premioPunExcento == 0 ? 0 : premioPunExcento;

            //            ws.Cell("P"+ i).Value = premioAguTotal == 0 ? 0 : premioAguTotal;
            //            ws.Cell("Q"+ i).Value = premioAguGravado == 0 ? 0 : premioAguGravado;
            //            ws.Cell("R"+ i).Value = premioAguExcento == 0 ? 0 : premioAguExcento;

            //            ws.Cell("S"+ i).Value = vacacionesTotal == 0 ? 0 : vacacionesTotal;
            //            ws.Cell("T"+ i).Value = vacacionesGravado == 0 ? 0 : vacacionesGravado;
            //            ws.Cell("U"+ i).Value = vacacionesExcento == 0 ? 0 : vacacionesExcento;

            //            ws.Cell("V"+ i).Value = primaTotal == 0 ? 0 : primaTotal;
            //            ws.Cell("W"+ i).Value = primaGravado == 0 ? 0 : primaGravado;
            //            ws.Cell("X"+ i).Value = primaExento == 0 ? 0 : primaExento;

            //            ws.Cell("Y"+ i).Value = otrosTotal == 0 ? 0 : otrosTotal;
            //            ws.Cell("Z"+ i).Value = otrosGravado == 0 ? 0 : otrosGravado;
            //            ws.Cell("AA"+ i).Value = otrosExento == 0 ? 0 : otrosExento;

            //            ws.Cell("AB" + i).Value = subsidioTotal == 0 ? subsidio : subsidioTotal;
            //            ws.Cell("AC" + i).Value = subsidioGravado == 0 ? 0 : subsidioGravado;
            //            ws.Cell("AD" + i).Value = subsidioExento == 0 ? 0 : subsidioExento;

            //            ws.Cell("AE" + i).Value = subsidioGravado == 0 ? dato.TOTAL_PERCEPCION + subsidio :dato.TOTAL_PERCEPCION + subsidioTotal;

            //            ws.Cell("AF" + i).Value = fonacotTotal == 0 ? 0 : fonacotTotal;
            //            ws.Cell("AG" + i).Value = fonacotGravado == 0 ? 0 : fonacotGravado;
            //            ws.Cell("AH" + i).Value = fonacotExento == 0 ? 0 : fonacotExento;
            //            ws.Cell("AI" + i).Value = infonavitTotal == 0 ? 0 : infonavitTotal;
            //            ws.Cell("AJ" + i).Value = infonavitGravado == 0 ? 0 : infonavitGravado;
            //            ws.Cell("AK" + i).Value = infonavitExento == 0 ? 0 : infonavitExento;
            //            ws.Cell("AL" + i).Value = isrTotal == 0 ? 0 : isrTotal;
            //            ws.Cell("AM" + i).Value = isrGravado == 0 ? 0 : isrGravado;
            //            ws.Cell("AN" + i).Value = isrExento == 0 ? 0 : isrExento;
            //            ws.Cell("AO" + i).Value = imssTotal == 0 ? 0 : imssTotal;
            //            ws.Cell("AP" + i).Value = imssGravado == 0 ? 0 : imssGravado;
            //            ws.Cell("AQ" + i).Value = imssExento == 0 ? 0 : imssExento;
            //            ws.Cell("AR" + i).Value = dato.TOTAL_DEDUCCION;
            //            ws.Cell("AS" + i).Value = dato2 == null ? 0 : dato2.CUOTA_FIJA;
            //            ws.Cell("AT" + i).Value = 0;
            //            ws.Cell("AU" + i).Value = dato2 == null ? 0 : dato2.EXCEDENTE_PATRON;
            //            ws.Cell("AV" + i).Value = dato2 == null ? 0 : dato2.EXCENDENTE_OBRERO;
            //            ws.Cell("AW" + i).Value = dato2 == null ? 0 : dato2.PRESTACIONES_PATRON;
            //            ws.Cell("AX" + i).Value = dato2 == null ? 0 : dato2.PRESTACION_OBRERO;
            //            ws.Cell("AY" + i).Value = dato2 == null ? 0 : dato2.PENSIONADOS_PATRON;
            //            ws.Cell("AZ" + i).Value = dato2 == null ? 0 : dato2.PENSIONADOS_OBRERA;
            //            ws.Cell("BA" + i).Value = dato2 == null ? 0 : dato2.INALIDEZ_VIDA_PATRON;
            //            ws.Cell("BB" + i).Value = dato2 == null ? 0 : dato2.INVALIDEZ_VIDA_OBRERA;
            //            ws.Cell("BC" + i).Value = dato2 == null ? 0 : dato2.PRESTACION_GUARDERIA_PATRON;
            //            ws.Cell("BD" + i).Value = 0;
            //            ws.Cell("BE" + i).Value = dato2 == null ? 0 : dato2.SEGURO_RETIRO_PATRON;
            //            ws.Cell("BF" + i).Value = 0;
            //            ws.Cell("BG" + i).Value = dato2 == null ? 0 : dato2.CESANTIA_VEJEZ_PATRON;
            //            ws.Cell("BH" + i).Value = dato2 == null ? 0 : dato2.CESANTIA_VEJEZ_OBRERO;
            //            ws.Cell("BI" + i).Value = dato2 == null ? 0 : dato2.INFONAVIT_PATRON;
            //            ws.Cell("BJ" + i).Value = 0;
            //            ws.Cell("BK" + i).Value = dato2 == null ? 0 : dato2.RIESGO_TRABAJO;
            //            ws.Cell("BL" + i).Value = 0;
            //            ws.Cell("BM" + i).Value = dato.TOTAL_TOTAL;
            //            ws.Cell("BN" + i).Value = dato2== null? "": dato2.PERIODO;
            //            ws.Cell("BO" + i).Value = cliente.Nombre + " " + sucursal.Ciudad;

            //            i++;
            //            conteoEmp++;
            //        }
            //        ws.Columns("3,1").AdjustToContents();
            //        ws.Columns("3,2").AdjustToContents();
            //        ws.Columns("3,3").AdjustToContents();
            //        ws.Columns("3,4").AdjustToContents();
            //        ws.Columns("3,6").AdjustToContents();
            //        ws.Columns("3,7").AdjustToContents();
            //        ws.Columns("3,8").AdjustToContents();
            //        ws.Columns("3,9").AdjustToContents();
            //        ws.Columns("3,10").AdjustToContents();
            //        ws.Columns("3,11").AdjustToContents();
            //        ws.Columns("3,12").AdjustToContents();
            //        ws.Columns("3,13").AdjustToContents();
            //        ws.Columns("3,14").AdjustToContents();
            //        ws.Columns("3,15").AdjustToContents();
            //        ws.Columns("3,16").AdjustToContents();
            //        ws.Columns("3,17").AdjustToContents();
            //        ws.Columns("3,18").AdjustToContents();
            //        ws.Columns("3,19").AdjustToContents();
            //        ws.Columns("3,20").AdjustToContents();
            //        ws.Columns("3,21").AdjustToContents();
            //        ws.Columns("3,22").AdjustToContents();
            //        ws.Columns("3,23").AdjustToContents();
            //        ws.Columns("3,24").AdjustToContents();
            //        ws.Columns("3,25").AdjustToContents();
            //        ws.Columns("3,26").AdjustToContents();
            //        ws.Columns("3,27").AdjustToContents();
            //        ws.Columns("3,28").AdjustToContents();
            //        ws.Columns("3,29").AdjustToContents();
            //        ws.Columns("3,30").AdjustToContents();
            //        ws.Columns("3,31").AdjustToContents();
            //        ws.Columns("3,32").AdjustToContents();
            //        ws.Columns("3,33").AdjustToContents();
            //        ws.Columns("3,34").AdjustToContents();
            //        ws.Columns("3,35").AdjustToContents();
            //        ws.Columns("3,36").AdjustToContents();
            //        ws.Columns("3,37").AdjustToContents();
            //        ws.Columns("3,38").AdjustToContents();
            //        ws.Columns("3,39").AdjustToContents();
            //        ws.Columns("3,40").AdjustToContents();
            //        ws.Columns("3,41").AdjustToContents();
            //        ws.Columns("3,42").AdjustToContents();
            //        ws.Columns("3,43").AdjustToContents();
            //        ws.Columns("3,44").AdjustToContents();
            //        ws.Columns("3,45").AdjustToContents();
            //        ws.Columns("3,46").AdjustToContents();
            //        ws.Columns("3,47").AdjustToContents();
            //        ws.Columns("3,48").AdjustToContents();
            //        ws.Columns("3,49").AdjustToContents();
            //        ws.Columns("3,50").AdjustToContents();
            //        ws.Columns("3,51").AdjustToContents();
            //        ws.Columns("3,52").AdjustToContents();
            //        ws.Columns("3,53").AdjustToContents();
            //        ws.Columns("3,54").AdjustToContents();
            //        ws.Columns("3,55").AdjustToContents();
            //        ws.Columns("3,56").AdjustToContents();
            //        ws.Columns("3,57").AdjustToContents();
            //        ws.Columns("3,58").AdjustToContents();
            //        ws.Columns("3,59").AdjustToContents();
            //        ws.Columns("3,60").AdjustToContents();
            //        ws.Columns("3,61").AdjustToContents();
            //        ws.Columns("3,62").AdjustToContents();
            //        ws.Columns("3,63").AdjustToContents();
            //        ws.Columns("3,64").AdjustToContents();
            //        ws.Columns("3,65").AdjustToContents();
            //        ws.Columns("3,66").AdjustToContents();

            //    }


            //}

            //wb.SaveAs(newruta, false);
            //return newruta;
        }

        /// <summary>
        /// Metodo Retorna la url del archivo
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="arrayPeriodos"></param>
        /// <returns></returns>
        public string GenerarArchivoAcumulado(int idUsuario, int[] arrayPeriodos, string pathFolder, string pathDescarga,
            int idEmpresa = 0, int idCliente = 0)
        {
            if (arrayPeriodos.Length <= 0) return null;


            #region Variables - List<>

            List<C_NOM_Conceptos> listaConceptos;
            List<Empleado> listaEmpleados;
            List<NOM_Nomina> listaNominas;
            List<NOM_Nomina_Detalle> listaDetalles;
            List<NOM_Cuotas_IMSS> listaCuotas;
            List<Empresa> listaEmpresasF;
            List<NOM_PeriodosPago> listaPeriodos;
            List<NOM_Finiquito> listaFiniquitos;
            List<NOM_Finiquito_Detalle> listaFiniquitosDetalles;
            List<C_NOM_Conceptos> listaConceptosFiniquito;
            List<Empleado> listaEmpleadosFiniquitos;
            List<Cliente> listaClientes;

            #endregion

            using (var context = new RHEntities())
            {
                #region GET DATA ACUMULADO

                //GET - Lista Periodos - filtrar periodos autorizados
                listaPeriodos = (from p in context.NOM_PeriodosPago
                                 where arrayPeriodos.Contains(p.IdPeriodoPago)
                                       // && p.Autorizado == true
                                       && (p.IdTipoNomina <= 15 || p.IdTipoNomina == 16 || p.IdTipoNomina == 11)
                                 //Solo toma nomina normales y asimilados
                                 select p).ToList();

                var arrayPeriodosAutorizados = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                //GET - lista de nominas que estan en el array de periodos
                listaNominas = (from n in context.NOM_Nomina
                                where arrayPeriodosAutorizados.Contains(n.IdPeriodo)
                                select n).ToList();

                listaFiniquitos = (from f in context.NOM_Finiquito
                                   where arrayPeriodosAutorizados.Contains(f.IdPeriodo)
                                   select f).ToList();

                if (idEmpresa > 0)
                {
                    #region NOMINAS FILTRO

                    //Buscamos las nominas que se generaron con la empresa en Fiscal
                    var arrayPeriodosFiscales =
                        listaPeriodos.Where(x => x.IdTipoNomina <= 15).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Fiscales
                    var listaF = (from n in listaNominas
                                  where arrayPeriodosFiscales.Contains(n.IdPeriodo)
                                        && n.IdEmpresaFiscal == idEmpresa
                                  select n).ToList();

                    //Buscamos las nominas que se generaron con la empresa en Asimilado
                    var arrayPeriodosAsimilados =
                        listaPeriodos.Where(x => x.IdTipoNomina == 16).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Asimilados
                    var listaA = (from n in listaNominas
                                  where arrayPeriodosAsimilados.Contains(n.IdPeriodo)
                                        && n.IdEmpresaAsimilado == idEmpresa
                                  select n).ToList();

                    //Unimos las dos listas
                    listaNominas = listaF;
                    listaNominas.AddRange(listaA);

                    #endregion

                    #region FINIQUITOS FILTRO

                    listaFiniquitos = listaFiniquitos.Where(x => x.IdEmpresaFiscal == idEmpresa).ToList();

                    #endregion


                }


                if (idCliente > 0)
                {
                    listaNominas = listaNominas.Where(x => x.IdCliente == idCliente).ToList();

                    listaFiniquitos = listaFiniquitos.Where(x => x.IdCliente == idCliente).ToList();
                }


                if (listaNominas.Count <= 0) return null;

                var arrayIdNominas = listaNominas.Select(x => x.IdNomina).ToArray();
                var arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();

                var arrayIdFiniquitos = listaFiniquitos.Select(x => x.IdFiniquito).ToArray();

                //GET Detalle de finiquitos
                listaFiniquitosDetalles = (from fd in context.NOM_Finiquito_Detalle
                                           where arrayIdFiniquitos.Contains(fd.IdFiniquito)
                                           select fd).ToList();


                //GET - Lista de detalles de nominas del arrray de nominas
                listaDetalles = (from nd in context.NOM_Nomina_Detalle
                                 where arrayIdNominas.Contains(nd.IdNomina)
                                 select nd).ToList();

                //GET - lista de cuotas imss del array de nominas
                listaCuotas = (from cuotas in context.NOM_Cuotas_IMSS
                               where arrayIdNominas.Contains(cuotas.IdNomina)
                               select cuotas).ToList();


                //GET -  Datos del empleado
                listaEmpleados = (from emp in context.Empleado
                                  where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                  orderby emp.APaterno
                                  select emp).ToList();

                //Agrupa por IdConcepto
                var listaGroupConceptos = listaDetalles.GroupBy(x => x.IdConcepto).ToList();

                //Genera Array de Id de los conceptos
                var arrayIdConceptos = listaGroupConceptos.Select(x => x.Key).ToArray();

                //GET - Lista de Conceptos
                listaConceptos = (from conce in context.C_NOM_Conceptos
                                  where arrayIdConceptos.Contains(conce.IdConcepto)
                                  select conce).ToList();

                var listaGroupConceptosF = listaFiniquitosDetalles.GroupBy(x => x.IdConcepto).ToList();

                arrayIdConceptos = listaGroupConceptosF.Select(x => x.Key).ToArray();

                listaConceptosFiniquito = (from conce in context.C_NOM_Conceptos
                                           where arrayIdConceptos.Contains(conce.IdConcepto)
                                           select conce).ToList();

                arrayIdEmpleados = listaFiniquitos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleadosFiniquitos = (from emp in context.Empleado
                                            where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                            orderby emp.APaterno
                                            select emp).ToList();

                //GET - Lista empresas fiscales
                listaEmpresasF = (from em in context.Empresa
                                  select em).ToList();

                listaClientes = (from clie in context.Cliente
                                 select clie).ToList();

                #endregion

            }
            //Validamos las listas
            if (listaConceptos.Count <= 0) return null;

            //Guarda el archivo en la memoria
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();

            #region DOCUMENTO EXCEL - NOMINA

            var worksheet = workbook.Worksheets.Add("Nominas");

            #region HEADERS

            #region HEADER EMPLEADOS

            worksheet.Cell(2, 1).Value = "IdNomina";
            worksheet.Cell(2, 2).Value = "Clave";
            worksheet.Cell(2, 3).Value = "Paterno";
            worksheet.Cell(2, 4).Value = "Materno";
            worksheet.Cell(2, 5).Value = "Nombre";
            worksheet.Cell(2, 6).Value = "RFC";
            worksheet.Cell(2, 7).Value = "CURP";

            //Establece un estilo al header
            worksheet.Range("A2:F2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue);

            #endregion

            #region HEADER NOMINA

            worksheet.Cell(2, 8).Value = "SD";
            worksheet.Cell(2, 9).Value = "SDI";
            worksheet.Cell(2, 10).Value = "SBC";
            worksheet.Cell(2, 11).Value = "Empresa";
            worksheet.Cell(2, 12).Value = "Cliente";
            worksheet.Cell(2, 13).Value = "Periodo";
            worksheet.Cell(2, 14).Value = "Bimestre";
            worksheet.Cell(2, 15).Value = "Autorizado";
            worksheet.Cell(2, 16).Value = "Total";
            worksheet.Cell(2, 17).Value = "Total Percepciones";
            worksheet.Cell(2, 18).Value = "Total Deducciones";
            worksheet.Cell(2, 19).Value = "Subsidio Causado";
            worksheet.Cell(2, 20).Value = "Subsidio Entregado";
            worksheet.Cell(2, 21).Value = "Isr antes Subsidio";
            worksheet.Cell(2, 22).Value = "Total ISN";

            //Establece un estilo al header
            worksheet.Range("G2:U2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Raspberry);


            #endregion

            #region HEADER IMSS

            worksheet.Cell(1, 23).Value = "Cuotas IMSS";
            worksheet.Cell(2, 23).Value = "Patron";
            worksheet.Cell(2, 24).Value = "Obrero";
            //MERGE CELL
            worksheet.Range(1, 23, 1, 24).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //Establece un estilo al header
            worksheet.Range(1, 23, 2, 24).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGreen);


            #endregion

            #region HEADER CONCEPTOS

            int columnaH = 25; //inicia en el 18
            //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
            bool changeColor = false;

            foreach (var itemConcepto in listaConceptos)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheet.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;

                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    worksheet.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Exento";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "ISN";
                    colFinal = columnaH;
                    columnaH++;
                }



                //MERGE CELL
                worksheet.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Range(1, colInicial, 2, colFinal).Style
                    .Font.SetFontSize(13)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Font.SetFontColor(XLColor.Black)
                    .Fill.SetBackgroundColor(changeColor == true ? XLColor.Amber : XLColor.LightBlue);

                changeColor = !changeColor;
            }


            #endregion

            #endregion

            //Variables
            int fila = 3;

            #region CONTENIDO

            //Por Empleado
            //Buscamos sus nominas y sus detalles de cada periodo
            foreach (var itemEmpleado in listaEmpleados)
            {
                //Cuantas nominas tiene el empleado
                var nominasDelEmpleado = listaNominas.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();

                //Por cada Nomina
                foreach (var itemNomina in nominasDelEmpleado)
                {

                    #region DATOS DEL EMPLEADO

                    worksheet.Cell(fila, 1).Value = itemNomina.IdNomina;
                    worksheet.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                    worksheet.Cell(fila, 3).Value = itemEmpleado.APaterno;
                    worksheet.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                    worksheet.Cell(fila, 5).Value = itemEmpleado.Nombres;
                    worksheet.Cell(fila, 6).Value = itemEmpleado.RFC;
                    worksheet.Cell(fila, 7).Value = itemEmpleado.CURP;

                    #endregion

                    #region DATOS NOMINA

                    worksheet.Cell(fila, 8).Value = itemNomina.SD;
                    worksheet.Cell(fila, 9).Value = itemNomina.SDI;
                    worksheet.Cell(fila, 10).Value = itemNomina.SBC;

                    //Empresa - Periodo - Cliente
                    var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemNomina.IdPeriodo);

                    Empresa itemEmpresa = null;

                    if (itemPeriodo.IdTipoNomina <= 15) //normal
                    {
                        itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaFiscal);
                    }
                    else if (itemPeriodo.IdTipoNomina == 16)
                    {
                        itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaAsimilado);
                    }


                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemNomina.IdCliente);

                    worksheet.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                    worksheet.Cell(fila, 12).Value = itemCliente != null ? itemCliente.Nombre : "-";
                    worksheet.Cell(fila, 13).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                    worksheet.Cell(fila, 14).Value = itemPeriodo?.Bimestre ?? 0;
                    worksheet.Cell(fila, 15).Value = itemPeriodo.Autorizado ? "Si" : "No";

                    //   .Alignment.Horizontal = XLAlignmentHorizontalValues.Center)

                    worksheet.Cell(fila, 15).Style
                         .Font.SetFontSize(13)
                         .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                         .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(itemPeriodo.Autorizado ? XLColor.LightGray : XLColor.AirForceBlue);



                    worksheet.Cell(fila, 16).Value = itemNomina.TotalNomina;
                    worksheet.Cell(fila, 17).Value = itemNomina.TotalPercepciones;
                    worksheet.Cell(fila, 18).Value = itemNomina.TotalDeducciones;
                    worksheet.Cell(fila, 19).Value = itemNomina.SubsidioCausado;
                    worksheet.Cell(fila, 20).Value = itemNomina.SubsidioEntregado;
                    worksheet.Cell(fila, 21).Value = itemNomina.ISRantesSubsidio;
                    worksheet.Cell(fila, 22).Value = itemNomina.TotalImpuestoSobreNomina;

                    #endregion

                    #region CUOTAS IMSS

                    var itemCuotaImss = listaCuotas.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina);
                    decimal totalPatron = 0;
                    decimal totalObrero = 0;

                    if (itemCuotaImss != null)
                    {
                        totalPatron = itemCuotaImss.TotalPatron;
                        totalObrero = itemCuotaImss.TotalObrero;
                    }

                    worksheet.Cell(fila, 23).Value = totalPatron;
                    worksheet.Cell(fila, 24).Value = totalObrero;

                    #endregion

                    #region CONCEPTOS

                    int columna = 25; //inicia en el 18
                    //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
                    foreach (var itemConcepto in listaConceptos)
                    {
                        decimal total = 0;
                        decimal gravado = 0;
                        decimal exento = 0;
                        decimal isn = 0;
                        //Busca en los detalles de la nomina por concepto y por id de nomina
                        var itemDetalle =
                            listaDetalles.FirstOrDefault(
                                x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);

                        if (itemDetalle != null)
                        {
                            total = itemDetalle.Total;
                            gravado = itemDetalle.GravadoISR;
                            exento = itemDetalle.ExentoISR;
                            isn = itemDetalle.ImpuestoSobreNomina;
                        }
                        //  43 - ISR
                        //  144 - Subsidio
                        //  42 - SS
                        if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 ||
                            itemConcepto.IdConcepto == 42)
                        {
                            worksheet.Cell(fila, columna).Value = total;
                            columna++;
                        }
                        else
                        {
                            worksheet.Cell(fila, columna).Value = total;
                            columna++;
                            worksheet.Cell(fila, columna).Value = gravado;
                            columna++;
                            worksheet.Cell(fila, columna).Value = exento;
                            columna++;
                            worksheet.Cell(fila, columna).Value = isn;
                            columna++;
                        }

                    }

                    #endregion

                    //Sumar los totales de Gravados Exentos - ISN

                    fila++;
                }
                //
            }

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
            worksheet.Column(15).AdjustToContents();
            worksheet.Column(16).AdjustToContents();
            worksheet.Column(17).AdjustToContents();
            worksheet.Column(18).AdjustToContents();
            worksheet.Column(19).AdjustToContents();
            worksheet.Column(20).AdjustToContents();
            worksheet.Column(21).AdjustToContents();

            #endregion

            #endregion

            #region DOCUMENTO EXCEL - FINIQUITO

            var worksheetF = workbook.Worksheets.Add("Finiquitos");

            //Header

            #region HEADERS

            #region EMPLEADO 

            worksheetF.Cell(2, 1).Value = "IdFiniquito";
            worksheetF.Cell(2, 2).Value = "Clave";
            worksheetF.Cell(2, 3).Value = "Paterno";
            worksheetF.Cell(2, 4).Value = "Materno";
            worksheetF.Cell(2, 5).Value = "Nombre";
            worksheetF.Cell(2, 6).Value = "RFC";
            worksheetF.Cell(2, 7).Value = "CURP";

            #endregion

            #region DATOS

            worksheetF.Cell(2, 8).Value = "SD";
            worksheetF.Cell(2, 9).Value = "SDI";
            worksheetF.Cell(2, 10).Value = "SBC";

            worksheetF.Cell(2, 11).Value = "Empresa";
            worksheetF.Cell(2, 12).Value = "Cliente";
            worksheetF.Cell(2, 13).Value = "Periodo";
            worksheetF.Cell(2, 14).Value = "Bimestre";
            worksheetF.Cell(2, 15).Value = "Autorizado";

            //Establece un estilo al header
            worksheetF.Range("A2:O2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue); //AirForceBlue

            #endregion

            #region CONCEPTOS

            columnaH = 16; //inicia en el 18
            //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
            changeColor = false;

            foreach (var itemConcepto in listaConceptosFiniquito)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;

                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheetF.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    // worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                    worksheetF.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Exento";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "ISN";
                    colFinal = columnaH;
                    columnaH++;
                }





                //MERGE CELL
                worksheetF.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheetF.Range(1, colInicial, 2, colFinal).Style
                    .Font.SetFontSize(13)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Font.SetFontColor(XLColor.Black)
                    .Fill.SetBackgroundColor(changeColor == true ? XLColor.Amber : XLColor.LightBlue);

                changeColor = !changeColor;
            }

            #endregion

            #endregion

            //Contenido

            #region CONTENIDO

            fila = 3;
            foreach (var itemFiniquito in listaFiniquitos)
            {
                //GET dato empleado
                var itemEmpleado = listaEmpleadosFiniquitos.FirstOrDefault(x => x.IdEmpleado == itemFiniquito.IdEmpleado);

                if (itemEmpleado == null) continue;

                worksheetF.Cell(fila, 1).Value = itemFiniquito.IdFiniquito;
                worksheetF.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                worksheetF.Cell(fila, 3).Value = itemEmpleado.APaterno;
                worksheetF.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                worksheetF.Cell(fila, 5).Value = itemEmpleado.Nombres;
                worksheetF.Cell(fila, 6).Value = itemEmpleado.RFC;
                worksheetF.Cell(fila, 7).Value = itemEmpleado.CURP;

                worksheetF.Cell(fila, 8).Value = itemFiniquito.SD;
                worksheetF.Cell(fila, 9).Value = itemFiniquito.SDI;
                worksheetF.Cell(fila, 10).Value = itemFiniquito.SBC;

                //Empresa - Periodo
                var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemFiniquito.IdPeriodo);
                var itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaFiscal) ??
                                  listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaAsimilado);

                var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemFiniquito.IdCliente);

                worksheetF.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                worksheetF.Cell(fila, 12).Value = itemCliente != null ? itemCliente.Nombre : "-";
                worksheetF.Cell(fila, 13).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                worksheetF.Cell(fila, 14).Value = itemPeriodo.Bimestre;
                worksheetF.Cell(fila, 15).Value = itemPeriodo.Autorizado ? "Si" : "No";

                worksheetF.Cell(fila, 15).Style
                  .Font.SetFontSize(13)
                  .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                  .Font.SetFontColor(XLColor.Black)
                  .Fill.SetBackgroundColor(itemPeriodo.Autorizado ? XLColor.LightGray : XLColor.AirForceBlue);


                int columna = 16; //
                foreach (var itemConcepto in listaConceptosFiniquito)
                {
                    decimal total = 0;
                    decimal gravado = 0;
                    decimal exento = 0;
                    decimal isn = 0;

                    var itemDetalleF =
                        listaFiniquitosDetalles.FirstOrDefault(
                            x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto);

                    if (itemDetalleF != null)
                    {
                        total = itemDetalleF.Total;
                        gravado = itemDetalleF.GravadoISR;
                        exento = itemDetalleF.ExentoISR;
                        isn = itemDetalleF.ImpuestoSobreNomina;
                    }


                    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    {
                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                    }
                    else
                    {

                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = gravado;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = exento;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = isn;
                        columna++;
                    }
                }

                fila++;
            }

            #endregion

            //Ajuste de celda

            #region AJUSTE DE CELDAS

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

            #endregion

            #endregion

            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);
            string nombreArchivo = "Acumulado_.xlsx";
            var fileName = pathUsuario + nombreArchivo;
            workbook.SaveAs(fileName);
            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

            //Guarda el documento en la memori
            //workbook.SaveAs(ms, false);
            //var archivoByte =  ms.ToArray();
            //return archivoByte;
        }

        public int[] GetPeriodosAutorizados(int? idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
            int idEmpresa = 0, int idCliente = 0)
        {
            //si empresa es 0 es todas las empresas
            //si cliente es 0 es todos los clientes
            List<NOM_Nomina> listaConsulta = new List<NOM_Nomina>();
            List<NOM_Finiquito> listaConsultaFiniquitos = new List<NOM_Finiquito>();
            List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();

            using (var context = new RHEntities())
            {
                //Filtramos por periodos que estan dentro del Rango de Fecha
                //Sean del periodo seleccinado
                //Sean periodos Autorizados y CFDI Generados
                listaPeriodos = (from p in context.NOM_PeriodosPago
                                 where p.Fecha_Fin >= dateInicial && p.Fecha_Fin <= dateFinal
                                       && p.IdEjercicio == idEjercicio
                                       //&& p.Autorizado == true &&
                                       //p.CfdiGenerado == (int) GenerarCfdiEstatus.Generado
                                       && p.SoloComplemento == false
                                 select p).ToList();

                //1er Filtro - por empresa
                if (idEmpresa > 0 || idCliente > 0)
                {

                    var arrayPeriodos = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                    listaConsulta = (from n in context.NOM_Nomina
                                     where arrayPeriodos.Contains(n.IdPeriodo)
                                           && n.IdEjercicio == idEjercicio
                                     // && n.IdEmpresaFiscal == idEmpresa
                                     select n).ToList();

                    listaConsultaFiniquitos = (from f in context.NOM_Finiquito
                                               where arrayPeriodos.Contains(f.IdPeriodo)
                                               // && f.IdEmpresaFiscal == idEmpresa
                                               select f).ToList();

                    //Buscamos la empresas que se generaron con la empresa en Asimilado
                }
                else
                {
                    //retornamos todos los periodos
                    return listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();
                }

            }

            //1er filtro
            if (idEmpresa > 0)
            {
                //Buscamos las nominas que se generaron con la empresa en Fiscal
                var arrayPeriodosFiscales =
                    listaPeriodos.Where(x => x.IdTipoNomina <= 15).Select(x => x.IdPeriodoPago).ToArray();

                var listaF = (from n in listaConsulta
                              where arrayPeriodosFiscales.Contains(n.IdPeriodo)
                                    && n.IdEmpresaFiscal == idEmpresa
                              select n).ToList();

                var arrayPeriodosAsimilados =
                    listaPeriodos.Where(x => x.IdTipoNomina == 16).Select(x => x.IdPeriodoPago).ToArray();


                var listaA = (from n in listaConsulta
                              where arrayPeriodosAsimilados.Contains(n.IdPeriodo)
                                    && n.IdEmpresaAsimilado == idEmpresa
                              select n).ToList();

                listaConsulta = listaF;
                listaConsulta.AddRange(listaA);

                //FILTRO PARA FINIQUITOS
                listaConsultaFiniquitos = listaConsultaFiniquitos.Where(x => x.IdEmpresaFiscal == idEmpresa).ToList();

            }


            //2do Filtro - por  Cliente
            if (idCliente > 0 && listaConsulta.Count > 0)
            {
                listaConsulta = listaConsulta.Where(x => x.IdCliente == idCliente).ToList();

                //FILTRO PARA FINIQUITOS
                listaConsultaFiniquitos = listaConsultaFiniquitos.Where(x => x.IdCliente == idCliente).ToList();
            }


            //
            var listaNominas = listaConsulta.Select(x => x.IdPeriodo).ToList();
            var listaFinis = listaConsultaFiniquitos.Select(x => x.IdPeriodo).ToList();

            listaNominas.AddRange(listaFinis);

            var arrayIdPeriodos = listaNominas.ToArray();

            arrayIdPeriodos = arrayIdPeriodos.Distinct().ToArray();

            return arrayIdPeriodos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="pathFolder"></param>
        /// <param name="pathDescarga"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="dateInicial"></param>
        /// <param name="dateFinal"></param>
        /// <param name="idEmpresa"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GenerarArchivoAcumuladoComplete(int idUsuario, string pathFolder, string pathDescarga,
            int? idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
            int idEmpresa = 0, int idCliente = 0, bool incluirNoAutorizados = false)
        {
            //Validaciones
            if (pathFolder.Trim() == "" || pathDescarga.Trim() == "" || dateInicial == null || dateFinal == null || idEjercicio == null)
                return null;


            #region Variables - List<>

            List<C_NOM_Conceptos> listaConceptos;
            List<Empleado> listaEmpleados;
            List<NOM_Nomina> listaNominas;
            List<NOM_Nomina_Detalle> listaDetalles;
            List<NOM_Cuotas_IMSS> listaCuotas;
            List<NOM_Cuotas_IMSS> listaCuotasFiniquito;
            List<Empresa> listaEmpresasF;
            List<NOM_PeriodosPago> listaPeriodos;
            List<NOM_Finiquito> listaFiniquitos;
            List<NOM_Finiquito_Detalle> listaFiniquitosDetalles;
            List<C_NOM_Conceptos> listaConceptosFiniquito;
            List<Empleado> listaEmpleadosFiniquitos;
            List<Cliente> listaClientes;
            List<NOM_Finiquito_Descuento_Adicional> listaDescuentoComisionFiniquito;

            #endregion

            using (var context = new RHEntities())
            {
                #region GET DATA ACUMULADO

                if (incluirNoAutorizados)

                {
                    listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where p.Fecha_Fin >= dateInicial && p.Fecha_Fin <= dateFinal
                                           && p.IdEjercicio == idEjercicio
                                           //&& p.Autorizado == true
                                           //p.CfdiGenerado == (int) GenerarCfdiEstatus.Generado
                                           && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
                                           && p.SoloComplemento == false
                                     //Que no sean solo complemento
                                     select p).ToList();
                }
                else
                {
                    listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where p.Fecha_Fin >= dateInicial && p.Fecha_Fin <= dateFinal
                                           && p.IdEjercicio == idEjercicio
                                           && p.Autorizado == true
                                           //p.CfdiGenerado == (int) GenerarCfdiEstatus.Generado
                                           && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
                                           && p.SoloComplemento == false
                                     //Que no sean solo complemento
                                     select p).ToList();
                }


                var arrayPeriodosAutorizados = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                //GET - lista de nominas que estan en el array de periodos
                listaNominas = (from n in context.NOM_Nomina
                                where arrayPeriodosAutorizados.Contains(n.IdPeriodo)
                                select n).ToList();

                //GET - lista de finiquitos que estan en el array de periodos
                listaFiniquitos = (from f in context.NOM_Finiquito
                                   where arrayPeriodosAutorizados.Contains(f.IdPeriodo)
                                   select f).ToList();

                //1er Filtro- por Empresa
                if (idEmpresa > 0)
                {
                    #region NOMINAS FILTRO

                    //Buscamos las nominas que se generaron con la empresa en Fiscal
                    var arrayPeriodosFiscales =
                        listaPeriodos.Where(x => x.IdTipoNomina <= 15).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Fiscales
                    var listaF = (from n in listaNominas
                                  where arrayPeriodosFiscales.Contains(n.IdPeriodo)
                                        && n.IdEmpresaFiscal == idEmpresa
                                  select n).ToList();

                    //Buscamos las nominas que se generaron con la empresa en Asimilado
                    var arrayPeriodosAsimilados =
                        listaPeriodos.Where(x => x.IdTipoNomina == 16).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Asimilados
                    var listaA = (from n in listaNominas
                                  where arrayPeriodosAsimilados.Contains(n.IdPeriodo)
                                        && n.IdEmpresaAsimilado == idEmpresa
                                  select n).ToList();

                    //Unimos las dos listas
                    listaNominas = listaF;
                    listaNominas.AddRange(listaA);

                    #endregion

                    #region FINIQUITOS FILTRO
                    //Busca los finiquitos que se generaron con la empresa fiscal
                    listaFiniquitos = listaFiniquitos.Where(x => x.IdEmpresaFiscal == idEmpresa).ToList();

                    #endregion


                }

                //2do Filtro
                if (idCliente > 0)
                {
                    //De la lista anterior filtramos las nominas dond el cliente sea idCliente
                    listaNominas = listaNominas.Where(x => x.IdCliente == idCliente).ToList();

                    //Filtramos los finiquitos por IdCliente
                    listaFiniquitos = listaFiniquitos.Where(x => x.IdCliente == idCliente).ToList();
                }


                if (listaNominas.Count <= 0) return null;

                //Generamos los array de id's para
                //Buscar sus detalles de nomina
                //Datos del empleado
                //Detalles del finiquito
                var arrayIdNominas = listaNominas.Select(x => x.IdNomina).ToArray();
                var arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();
                var arrayIdFiniquitos = listaFiniquitos.Select(x => x.IdFiniquito).ToArray();

                //GET Detalle de finiquitos
                listaFiniquitosDetalles = (from fd in context.NOM_Finiquito_Detalle
                                           where arrayIdFiniquitos.Contains(fd.IdFiniquito)
                                           select fd).ToList();

                //GET 


                //GET - Lista de detalles de nominas del arrray de nominas
                listaDetalles = (from nd in context.NOM_Nomina_Detalle
                                 where arrayIdNominas.Contains(nd.IdNomina)
                                 select nd).ToList();

                //GET - lista de cuotas imss del array de nominas
                listaCuotas = (from cuotas in context.NOM_Cuotas_IMSS
                               where arrayIdNominas.Contains(cuotas.IdNomina)
                               select cuotas).ToList();


                //GET - lista de cuotas imss de los finiquitos
                listaCuotasFiniquito = (from cuotas in context.NOM_Cuotas_IMSS
                                        where arrayIdFiniquitos.Contains(cuotas.IdFiniquito)
                                        select cuotas).ToList();

                listaDescuentoComisionFiniquito = (from dc in context.NOM_Finiquito_Descuento_Adicional
                                                   where arrayPeriodosAutorizados.Contains(dc.IdPeriodo)
                                                   select dc).ToList();

                //GET -  Datos del empleado
                listaEmpleados = (from emp in context.Empleado
                                  where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                  orderby emp.APaterno
                                  select emp).ToList();

                //Agrupa por IdConcepto
                var listaGroupConceptos = listaDetalles.GroupBy(x => x.IdConcepto).ToList();

                //Genera Array de Id de los conceptos
                var arrayIdConceptos = listaGroupConceptos.Select(x => x.Key).ToArray();

                //GET - Lista de Conceptos de las Nominas
                listaConceptos = (from conce in context.C_NOM_Conceptos
                                  where arrayIdConceptos.Contains(conce.IdConcepto)
                                  select conce).ToList();

                var listaGroupConceptosF = listaFiniquitosDetalles.GroupBy(x => x.IdConcepto).ToList();

                arrayIdConceptos = listaGroupConceptosF.Select(x => x.Key).ToArray();

                //Lista de Conceptos de los finiquitos
                listaConceptosFiniquito = (from conce in context.C_NOM_Conceptos
                                           where arrayIdConceptos.Contains(conce.IdConcepto)
                                           select conce).ToList();

                arrayIdEmpleados = listaFiniquitos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleadosFiniquitos = (from emp in context.Empleado
                                            where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                            orderby emp.APaterno
                                            select emp).ToList();

                //GET - Lista empresas fiscales
                listaEmpresasF = (from em in context.Empresa
                                  select em).ToList();

                listaClientes = (from clie in context.Cliente
                                 select clie).ToList();

                #endregion
            }
            //Validamos las listas
            if (listaConceptos.Count <= 0) return null;

            Empresa itemEmpresa = null;

            //Guarda el archivo en la memoria
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();

            #region DOCUMENTO EXCEL - NOMINA ********************************************************

            var worksheet = workbook.Worksheets.Add("Nominas");
            worksheet.SheetView.Freeze(2, 5);

            #region HEADERS

            #region HEADER EMPLEADOS

            worksheet.Cell(2, 1).Value = "IdNomina";
            worksheet.Cell(2, 2).Value = "Clave";
            worksheet.Cell(2, 3).Value = "Paterno";
            worksheet.Cell(2, 4).Value = "Materno";
            worksheet.Cell(2, 5).Value = "Nombre";
            worksheet.Cell(2, 6).Value = "RFC";
            worksheet.Cell(2, 7).Value = "CURP";

            #endregion

            #region HEADER NOMINA
            worksheet.Cell(2, 8).Value = "SD";
            worksheet.Cell(2, 9).Value = "SDI";
            worksheet.Cell(2, 10).Value = "Cliente";
            worksheet.Cell(2, 11).Value = "Periodo";
            worksheet.Cell(2, 12).Value = "Bimestre";
            worksheet.Cell(2, 13).Value = "Dias Trabajados";
            worksheet.Cell(2, 14).Value = "Total Nomina";
            #endregion

            #region HEADER CONCEPTOS

            int columnaH = 15; //inicia en el 18
            //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO

            var listaConceptosPercecpciones = listaConceptos.Where(x => x.TipoConcepto == 1).ToList();
            var listaConceptosDeducciones = listaConceptos.Where(x => x.TipoConcepto == 2).ToList();

            int rangeInicial = columnaH;
            #region PERCEPCIONES
            foreach (var itemConcepto in listaConceptosPercecpciones)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheet.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    worksheet.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                }

                //MERGE CELL
                worksheet.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            #endregion

            //TOTAL DE PERCEPCIONES
            worksheet.Cell(2, columnaH).Value = "Total Percepciones";
            columnaH++;

            //Establece un estilo al header
            worksheet.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGray);


            rangeInicial = columnaH;
            #region DEDUCCIONES
            foreach (var itemConcepto in listaConceptosDeducciones)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheet.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    worksheet.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                }

                //MERGE CELL
                worksheet.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }


            worksheet.Cell(1, columnaH).Value = "Subsidio Causado";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Subsidio Entregado";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Isr antes Subsidio";
            columnaH++;


            //TOTAL DE PERCEPCIONES
            worksheet.Cell(1, columnaH).Value = "Total Deducciones";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Total ISN";
            columnaH++;
            #endregion

            //Establece un estilo al header
            worksheet.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);


            #endregion

            #region HEADER IMSS

            int mergeI = columnaH;
            int colorI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cuotas IMSS";
            worksheet.Cell(2, columnaH).Value = "Total Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Total Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cuota Fija";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Excedente";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Prestaciones";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Pensionados";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Invalidez y Vida";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Guarderia";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Seguro Retiro";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cesantía y Vejez";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Infonavit";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Riesgo Trabajo";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            //Establece un estilo al header
            worksheet.Range(1, colorI, 2, columnaH).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGreen);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Complemento";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);

            //MERGE CELL
            //  worksheet.Range(1, columnaH, 1, columnaH + 1).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



            #endregion



            #endregion

            //Variables
            int fila = 3;
            bool rowColor = false;
            int idEmp = 0;
            #region CONTENIDO

            //Por Empleado
            //Buscamos sus nominas y sus detalles de cada periodo
            foreach (var itemEmpleado in listaEmpleados)
            {
                //Cuantas nominas tiene el empleado
                var nominasDelEmpleado = listaNominas.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();

                if (idEmp != itemEmpleado.IdEmpleado)
                {
                    idEmp = itemEmpleado.IdEmpleado;
                    rowColor = !rowColor;
                    rowColor = true;
                }

                //Por cada Nomina
                foreach (var itemNomina in nominasDelEmpleado)
                {

                    #region DATOS DEL EMPLEADO
                    worksheet.Cell(fila, 1).Value = itemNomina.IdNomina;
                    worksheet.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                    worksheet.Cell(fila, 3).Value = itemEmpleado.APaterno;
                    worksheet.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                    worksheet.Cell(fila, 5).Value = itemEmpleado.Nombres;
                    worksheet.Cell(fila, 6).Value = itemEmpleado.RFC;
                    worksheet.Cell(fila, 7).Value = itemEmpleado.CURP;
                    #endregion

                    #region DATOS NOMINA

                    worksheet.Cell(fila, 8).Value = itemNomina.SD;
                    worksheet.Cell(fila, 9).Value = itemNomina.SDI;
                    //worksheet.Cell(fila, 10).Value = itemNomina.SBC;

                    //Empresa - Periodo - Cliente
                    var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemNomina.IdPeriodo);

                    itemEmpresa = null;

                    if (itemPeriodo.IdTipoNomina <= 15) //normal
                    {
                        itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaFiscal);
                    }
                    else if (itemPeriodo.IdTipoNomina == 16)
                    {
                        itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaAsimilado);
                    }


                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemNomina.IdCliente);

                    // worksheet.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                    worksheet.Cell(fila, 10).Value = itemCliente != null ? itemCliente.Nombre : "-";
                    worksheet.Cell(fila, 11).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                    worksheet.Cell(fila, 12).Value = itemPeriodo?.Bimestre ?? 0;

                    if (incluirNoAutorizados)
                    {
                        if (itemPeriodo.Autorizado == false)
                        {
                            worksheet.Cell(fila, 12).Style
                                .Font.SetFontSize(13)
                                .Font.SetBold(true)
                                //.Font.SetFontColor(XLColor.White)
                                .Fill.SetBackgroundColor(XLColor.Gray);
                        }
                    }

                    worksheet.Cell(fila, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(fila, 13).Value = itemNomina.Dias_Laborados;
                    worksheet.Cell(fila, 14).Value = itemNomina.TotalNomina;

                    #region CONCEPTOS

                    int columna = 15;

                    #region PERCEPCIONES
                    //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
                    foreach (var itemConcepto in listaConceptosPercecpciones)
                    {
                        decimal total = 0;
                        decimal gravado = 0;
                        decimal exento = 0;
                        //Busca en los detalles de la nomina por concepto y por id de nomina
                        var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);

                        if (itemDetalle != null)
                        {
                            total = itemDetalle.Total;
                            gravado = itemDetalle.GravadoISR;
                            exento = itemDetalle.ExentoISR;
                        }
                        //  43 - ISR
                        //  144 - Subsidio
                        //  42 - SS
                        if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                        {
                            worksheet.Cell(fila, columna).Value = total;
                            columna++;
                        }
                        else
                        {
                            worksheet.Cell(fila, columna).Value = total;
                            columna++;
                            worksheet.Cell(fila, columna).Value = gravado;
                            columna++;
                            worksheet.Cell(fila, columna).Value = exento;
                            columna++;
                            //worksheet.Cell(fila, columna).Value = isn;
                            //columna++;
                        }
                    }


                    #endregion

                    worksheet.Cell(fila, columna).Value = itemNomina.TotalPercepciones;
                    columna++;

                    #region DEDUCCIONES
                    //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
                    foreach (var itemConcepto in listaConceptosDeducciones)
                    {
                        decimal total = 0;
                        decimal gravado = 0;
                        decimal exento = 0;
                        //Busca en los detalles de la nomina por concepto y por id de nomina
                        var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);

                        if (itemDetalle != null)
                        {
                            total = itemDetalle.Total;
                            gravado = itemDetalle.GravadoISR;
                            exento = itemDetalle.ExentoISR;
                        }
                        //  43 - ISR
                        //  144 - Subsidio
                        //  42 - SS
                        if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                        {
                            worksheet.Cell(fila, columna).Value = total;
                            columna++;
                        }
                        else
                        {
                            worksheet.Cell(fila, columna).Value = total;
                            columna++;
                            worksheet.Cell(fila, columna).Value = gravado;
                            columna++;
                            worksheet.Cell(fila, columna).Value = exento;
                            columna++;
                            //worksheet.Cell(fila, columna).Value = isn;
                            //columna++;
                        }
                    }
                    #endregion


                    #endregion

                    worksheet.Cell(fila, columna).Value = itemNomina.SubsidioCausado;
                    columna++;
                    worksheet.Cell(fila, columna).Value = itemNomina.SubsidioEntregado;
                    columna++;
                    worksheet.Cell(fila, columna).Value = itemNomina.ISRantesSubsidio;
                    columna++;
                    worksheet.Cell(fila, columna).Value = itemNomina.TotalDeducciones;
                    columna++;
                    worksheet.Cell(fila, columna).Value = itemNomina.TotalImpuestoSobreNomina;
                    columna++;
                    #endregion

                    #region CUOTAS IMSS

                    var itemCuotaImss = listaCuotas.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina);
                    decimal totalPatron = 0;
                    decimal totalObrero = 0;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalPatron ?? 0;
                    columna++;
                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalObrero ?? 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.Cuota_Fija_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Obrero ?? 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Obrero ?? 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Obrero ?? 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Obrero ?? 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.GuarderiasPrestaciones_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.SeguroRetiro_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Obrero ?? 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.Infonavit_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = 0; columna++;

                    worksheet.Cell(fila, columna).Value = itemCuotaImss?.RiesgoTrabajo_Patron ?? 0; columna++;
                    worksheet.Cell(fila, columna).Value = 0; columna++;
                    #endregion

                    worksheet.Cell(fila, columna).Value = itemNomina.TotalComplemento;
                    worksheet.Cell(fila, columna).Style.NumberFormat.Format = "$ ###,##0.00";

                    //Sumar los totales de Gravados Exentos - ISN

                    if (rowColor)
                    {
                        rowColor = false;
                        worksheet.Range(fila, 1, fila, columna).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(fila, 1, fila, columna).Style.Border.TopBorderColor = XLColor.SlateGray;

                        // worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Lavender;
                        //worksheet.Range(fila, 1, fila, columna).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                    }

                    fila++;
                    //rowColor = !rowColor;
                }
                //
            }

            #endregion

            #region  COLORES DISEÑO DE CELDAS

            //Establece un estilo al header
            worksheet.Range("G2:N2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Raspberry);

            //Establece un estilo al header
            worksheet.Range("A2:F2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue);


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
            worksheet.Column(15).AdjustToContents();
            worksheet.Column(16).AdjustToContents();
            worksheet.Column(17).AdjustToContents();
            worksheet.Column(18).AdjustToContents();
            worksheet.Column(19).AdjustToContents();
            worksheet.Column(20).AdjustToContents();
            worksheet.Column(21).AdjustToContents();

            #endregion

            #endregion

            #region DOCUMENTO EXCEL - FINIQUITO *****************************************************

            var worksheetF = workbook.Worksheets.Add("Finiquitos");
            worksheetF.SheetView.Freeze(2, 4);

            #region HEADERS

            #region EMPLEADO 

            worksheetF.Cell(2, 1).Value = "IdFiniquito";
            worksheetF.Cell(2, 2).Value = "Clave";
            worksheetF.Cell(2, 3).Value = "Paterno";
            worksheetF.Cell(2, 4).Value = "Materno";
            worksheetF.Cell(2, 5).Value = "Nombre";
            worksheetF.Cell(2, 6).Value = "RFC";
            worksheetF.Cell(2, 7).Value = "CURP";

            #endregion

            #region DATOS

            worksheetF.Cell(2, 8).Value = "SD";
            worksheetF.Cell(2, 9).Value = "SDI";

            worksheetF.Cell(2, 10).Value = "Cliente";
            worksheetF.Cell(2, 11).Value = "Periodo";
            worksheetF.Cell(2, 12).Value = "Bimestre";
            worksheetF.Cell(2, 13).Value = "Total Finiquito";
            worksheetF.Cell(2, 14).Value = "Total Liq";
            worksheetF.Cell(2, 15).Value = "Total Neto";

            //Establece un estilo al header
            worksheetF.Range("A2:O2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue); //AirForceBlue

            #endregion

            #region CONCEPTOS

            columnaH = 16; //inicia en el 18
                           //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO

            rangeInicial = columnaH;
            foreach (var itemConcepto in listaConceptosFiniquito)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;

                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheetF.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    // worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                    worksheetF.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                    //worksheetF.Cell(2, columnaH).Value = "ISN";
                    //colFinal = columnaH;
                    //columnaH++;
                }


                //MERGE CELL
                worksheetF.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            //Establece un estilo al header
            worksheetF.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGray);

            #endregion

            #region HEADER DESCUENTOS Y COMISIONES
            //TOTAL ISN
            rangeInicial = columnaH;
            worksheetF.Cell(2, columnaH).Value = "TOTAL ISN";
            columnaH++;
            //DESCUENTOS
            worksheetF.Cell(2, columnaH).Value = "DESCUENTO ADICIONAL";
            columnaH++;
            //COMISIONES
            worksheetF.Cell(2, columnaH).Value = "COMISIONES";
            columnaH++;


            //Establece un estilo al header
            worksheetF.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);
            #endregion

            #region HEADER IMSS

            mergeI = columnaH;
            colorI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cuotas IMSS";
            worksheetF.Cell(2, columnaH).Value = "Total Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Total Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cuota Fija";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Excedente";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Prestaciones";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Pensionados";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Invalidez y Vida";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Guarderia";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Seguro Retiro";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cesantía y Vejez";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Infonavit";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Riesgo Trabajo";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            //Establece un estilo al header
            worksheetF.Range(1, colorI, 2, columnaH).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGreen);
            columnaH++;


            //MERGE CELL
            //  worksheet.Range(1, columnaH, 1, columnaH + 1).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheetF.Cell(2, columnaH).Value = "Complemento";
            worksheetF.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);

            #endregion

            #endregion

            #region CONTENIDO

            #region CONCEPTOS
            fila = 3;
            foreach (var itemFiniquito in listaFiniquitos)
            {
                //GET dato empleado
                var itemEmpleado = listaEmpleadosFiniquitos.FirstOrDefault(x => x.IdEmpleado == itemFiniquito.IdEmpleado);

                if (itemEmpleado == null) continue;

                worksheetF.Cell(fila, 1).Value = itemFiniquito.IdFiniquito;
                worksheetF.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                worksheetF.Cell(fila, 3).Value = itemEmpleado.APaterno;
                worksheetF.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                worksheetF.Cell(fila, 5).Value = itemEmpleado.Nombres;
                worksheetF.Cell(fila, 6).Value = itemEmpleado.RFC;
                worksheetF.Cell(fila, 7).Value = itemEmpleado.CURP;

                worksheetF.Cell(fila, 8).Value = itemFiniquito.SD;
                worksheetF.Cell(fila, 9).Value = itemFiniquito.SDI;

                //Empresa - Periodo
                var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemFiniquito.IdPeriodo);
                itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaFiscal) ??
                                 listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaAsimilado);

                var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemFiniquito.IdCliente);

                // worksheetF.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                worksheetF.Cell(fila, 10).Value = itemCliente != null ? itemCliente.Nombre : "-";
                worksheetF.Cell(fila, 11).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                worksheetF.Cell(fila, 12).Value = itemPeriodo.Bimestre;
                worksheetF.Cell(fila, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheetF.Cell(fila, 13).Value = itemFiniquito.Total_Finiquito;
                worksheetF.Cell(fila, 14).Value = itemFiniquito.Total_Liquidacion;
                worksheetF.Cell(fila, 15).Value = itemFiniquito.TOTAL_total;

                int columna = 16; //
                decimal totalIsn = 0;
                foreach (var itemConcepto in listaConceptosFiniquito)
                {
                    decimal total = 0;
                    decimal gravado = 0;
                    decimal exento = 0;
                    decimal isn = 0;

                    var itemDetalleF =
                        listaFiniquitosDetalles.FirstOrDefault(
                            x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto);

                    if (itemDetalleF != null)
                    {
                        total = itemDetalleF.Total;
                        gravado = itemDetalleF.GravadoISR;
                        exento = itemDetalleF.ExentoISR;
                        isn = itemDetalleF.ImpuestoSobreNomina;
                        totalIsn += isn;
                    }


                    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    {
                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                    }
                    else
                    {
                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = gravado;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = exento;
                        columna++;
                        //worksheetF.Cell(fila, columna).Value = isn;
                        //columna++;
                    }
                }



                #region  DESCUENTOS Y COMISIONES

                var descuentosAdicionales = listaDescuentoComisionFiniquito.Where(x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 2)
                       .Select(x => x.TotalDescuento)
                       .Sum();

                var comisiones = listaDescuentoComisionFiniquito.Where(x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 1)
                        .Select(x => x.TotalDescuento)
                        .Sum();
                //TOTAL ISN
                worksheetF.Cell(fila, columna).Value = totalIsn;
                columna++;
                //DESCUENTOS
                worksheetF.Cell(fila, columna).Value = descuentosAdicionales;
                columna++;
                //COMISIONES
                worksheetF.Cell(fila, columna).Value = comisiones;
                columna++;
                #endregion


                #region CUOTAS IMSS

                var itemCuotaImss = listaCuotasFiniquito.FirstOrDefault(x => x.IdFiniquito == itemFiniquito.IdFiniquito);
                decimal totalPatron = 0;
                decimal totalObrero = 0;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.TotalPatron ?? 0;
                columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.TotalObrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Cuota_Fija_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.GuarderiasPrestaciones_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.SeguroRetiro_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Infonavit_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.RiesgoTrabajo_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;
                #endregion

                worksheetF.Cell(fila, columna).Value = itemFiniquito.TotalComplemento;

                // if (rowColor)
                //    {
                //        // worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Lavender;
                //        worksheetF.Range(fila, 1, fila, columna).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                //    }

                //rowColor = !rowColor;
                fila++;
            }

            #endregion

            #endregion

            #region AJUSTE DE CELDAS

            worksheetF.Column(1).AdjustToContents(1);
            worksheetF.Column(2).AdjustToContents(2);
            worksheetF.Column(3).AdjustToContents(3);
            worksheetF.Column(4).AdjustToContents(4);
            worksheetF.Column(5).AdjustToContents(5);
            worksheetF.Column(6).AdjustToContents(6);
            worksheetF.Column(7).AdjustToContents(7);
            worksheetF.Column(8).AdjustToContents(8);
            worksheetF.Column(9).AdjustToContents(9);
            worksheetF.Column(10).AdjustToContents(10);
            worksheetF.Column(11).AdjustToContents(11);
            worksheetF.Column(12).AdjustToContents(12);
            worksheetF.Column(13).AdjustToContents(13);
            worksheetF.Column(14).AdjustToContents(14);
            worksheetF.Column(15).AdjustToContents(15);
            worksheetF.Column(16).AdjustToContents(16);
            worksheetF.Column(17).AdjustToContents(17);
            worksheetF.Column(18).AdjustToContents(18);
            worksheetF.Column(19).AdjustToContents(19);
            worksheetF.Column(20).AdjustToContents(20);
            worksheetF.Column(21).AdjustToContents(21);


            #endregion

            #endregion


            #region CREA EL ARCHIVO Y RETORNA EL PATH RELATIVO PARA LA DESCARGA
            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "Acumulado_.xlsx";

            if (itemEmpresa != null)
            {
                nombreArchivo = "ACUM_" + itemEmpresa.RazonSocial + "_.xlsx";
            }

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);
            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

            //Guarda el documento en la memori
            //workbook.SaveAs(ms, false);
            //var archivoByte =  ms.ToArray();
            //return archivoByte;
            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="pathFolder"></param>
        /// <param name="pathDescarga"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="dateInicial"></param>
        /// <param name="dateFinal"></param>
        /// <param name="idEmpresa"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GenerarArchivoAcumuladoComplete2(int idUsuario, string pathFolder, string pathDescarga,
            int? idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
            int idEmpresa = 0, int idCliente = 0, bool incluirNoAutorizados = false, bool calculoanual = false)
        {
            decimal UMA = 0;

            //Validaciones
            if (pathFolder.Trim() == "" || pathDescarga.Trim() == "" || dateInicial == null || dateFinal == null || idEjercicio == null)
                return null;

            #region Variables - List<>

            List<C_NOM_Conceptos> listaConceptos;
            List<Empleado> listaEmpleados;
            List<NOM_Nomina> listaNominas;
            List<NOM_Nomina_Detalle> listaDetalles;
            List<NOM_Cuotas_IMSS> listaCuotas;
            List<NOM_Cuotas_IMSS> listaCuotasFiniquito;
            List<Empresa> listaEmpresasF;
            List<NOM_PeriodosPago> listaPeriodos;
            List<NOM_Finiquito> listaFiniquitos;
            List<NOM_Finiquito_Detalle> listaFiniquitosDetalles;
            List<C_NOM_Conceptos> listaConceptosFiniquito;
            List<Empleado> listaEmpleadosFiniquitos;
            List<Cliente> listaClientes;
            List<NOM_Finiquito_Descuento_Adicional> listaDescuentoComisionFiniquito;
            List<Empleado_Contrato> listaContratos;
            NOM_Ejercicio_Fiscal ejercicioFiscal;

            List<Permisos> listaPermisos;
            List<Inasistencias> listafaltas;
            List<Incapacidades> listaIncapacidadeses;

            List<NOM_CFDI_Timbrado> registrosQueSonNomina;
            List<NOM_CFDI_Timbrado> registrosQueSonFiniquito;
            #endregion

            using (var context = new RHEntities())
            {



                #region GET DATA ACUMULADO

                UMA = context.ZonaSalario.FirstOrDefault().UMA;

                ejercicioFiscal = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);

                if (incluirNoAutorizados)

                {
                    listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where DbFunctions.TruncateTime(p.Fecha_Fin) >= DbFunctions.TruncateTime(dateInicial) && DbFunctions.TruncateTime(p.Fecha_Fin) <= DbFunctions.TruncateTime(dateFinal)
                                           && p.IdEjercicio == idEjercicio
                                           //&& p.Autorizado == true
                                           //p.CfdiGenerado == (int) GenerarCfdiEstatus.Generado
                                           && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
                                           && p.SoloComplemento == false
                                     //Que no sean solo complemento
                                     select p).ToList();
                }
                else
                {
                    listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where DbFunctions.TruncateTime(p.Fecha_Fin) >= DbFunctions.TruncateTime(dateInicial) && DbFunctions.TruncateTime(p.Fecha_Fin) <= DbFunctions.TruncateTime(dateFinal)
                                           && p.IdEjercicio == idEjercicio
                                           && p.Autorizado == true
                                           //p.CfdiGenerado == (int) GenerarCfdiEstatus.Generado
                                           && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
                                           && p.SoloComplemento == false
                                     //Que no sean solo complemento
                                     select p).ToList();
                }


                var arrayPeriodosAutorizados = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                //GET - lista de nominas que estan en el array de periodos
                listaNominas = (from n in context.NOM_Nomina
                                where arrayPeriodosAutorizados.Contains(n.IdPeriodo)
                                select n).ToList();

                //GET - lista de finiquitos que estan en el array de periodos
                listaFiniquitos = (from f in context.NOM_Finiquito
                                   where arrayPeriodosAutorizados.Contains(f.IdPeriodo)
                                   select f).ToList();

                //1er Filtro- por Empresa
                if (idEmpresa > 0)
                {
                    #region NOMINAS FILTRO

                    //Buscamos las nominas que se generaron con la empresa en Fiscal
                    var arrayPeriodosFiscales =
                        listaPeriodos.Where(x => x.IdTipoNomina <= 15).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Fiscales
                    var listaF = (from n in listaNominas
                                  where arrayPeriodosFiscales.Contains(n.IdPeriodo)
                                        && n.IdEmpresaFiscal == idEmpresa
                                  select n).ToList();

                    //Buscamos las nominas que se generaron con la empresa en Asimilado
                    var arrayPeriodosAsimilados =
                        listaPeriodos.Where(x => x.IdTipoNomina == 16).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Asimilados
                    var listaA = (from n in listaNominas
                                  where arrayPeriodosAsimilados.Contains(n.IdPeriodo)
                                        && n.IdEmpresaAsimilado == idEmpresa
                                  select n).ToList();

                    //Unimos las dos listas
                    listaNominas = listaF;
                    listaNominas.AddRange(listaA);

                    #endregion

                    #region FINIQUITOS FILTRO
                    //Busca los finiquitos que se generaron con la empresa fiscal
                    listaFiniquitos = listaFiniquitos.Where(x => x.IdEmpresaFiscal == idEmpresa).ToList();

                    #endregion


                }

                //2do Filtro
                if (idCliente > 0)
                {
                    //De la lista anterior filtramos las nominas dond el cliente sea idCliente
                    listaNominas = listaNominas.Where(x => x.IdCliente == idCliente).ToList();

                    //Filtramos los finiquitos por IdCliente
                    listaFiniquitos = listaFiniquitos.Where(x => x.IdCliente == idCliente).ToList();
                }


                if (listaNominas.Count <= 0) return null;

                //Generamos los array de id's para
                //Buscar sus detalles de nomina
                //Datos del empleado
                //Detalles del finiquito
                var arrayIdNominas = listaNominas.Select(x => x.IdNomina).ToArray();
                var arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();
                var arrayIdFiniquitos = listaFiniquitos.Select(x => x.IdFiniquito).ToArray();

                //GET Detalle de finiquitos
                listaFiniquitosDetalles = (from fd in context.NOM_Finiquito_Detalle
                                           where arrayIdFiniquitos.Contains(fd.IdFiniquito)
                                           select fd).ToList();

                //GET PERMISOS E INCAPACIDADES
                var fecha2mesesMenos = dateInicial.Value.AddMonths(-2);

                listaPermisos = (from p in context.Permisos
                                 where arrayIdEmpleados.Contains(p.IdEmpleado)
                                       && p.FechaInicio >= fecha2mesesMenos && p.FechaInicio <= dateFinal.Value
                                 select p).ToList();


                listaIncapacidadeses = (from i in context.Incapacidades
                                        where arrayIdEmpleados.Contains(i.IdEmpleado)
                                              && i.FechaInicio >= fecha2mesesMenos && i.FechaInicio <= dateFinal.Value
                                        select i).ToList();


                listafaltas = (from i in context.Inasistencias
                               where arrayIdEmpleados.Contains(i.IdEmpleado)
                                     && i.Fecha >= fecha2mesesMenos && i.Fecha <= dateFinal.Value
                               select i).ToList();



                //GET - Lista de detalles de nominas del arrray de nominas
                listaDetalles = (from nd in context.NOM_Nomina_Detalle
                                 where arrayIdNominas.Contains(nd.IdNomina)
                                 select nd).ToList();

                //GET - lista de cuotas imss del array de nominas
                listaCuotas = (from cuotas in context.NOM_Cuotas_IMSS
                               where arrayIdNominas.Contains(cuotas.IdNomina)
                               select cuotas).ToList();


                //GET - lista de cuotas imss de los finiquitos
                //listaCuotasFiniquito = (from cuotas in context.NOM_Cuotas_IMSS
                //                        where arrayIdFiniquitos.Contains(cuotas.IdFiniquito)
                //                        select cuotas).ToList();

                listaDescuentoComisionFiniquito = (from dc in context.NOM_Finiquito_Descuento_Adicional
                                                   where arrayPeriodosAutorizados.Contains(dc.IdPeriodo)
                                                   select dc).ToList();

                //GET -  Datos del empleado
                listaEmpleados = (from emp in context.Empleado
                                  where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                  orderby emp.APaterno
                                  select emp).ToList();

                //Agrupa por IdConcepto
                var listaGroupConceptos = listaDetalles.GroupBy(x => x.IdConcepto).ToList();

                //Genera Array de Id de los conceptos
                var arrayIdConceptos = listaGroupConceptos.Select(x => x.Key).ToArray();

                //GET - Lista de Conceptos de las Nominas
                listaConceptos = (from conce in context.C_NOM_Conceptos
                                  where arrayIdConceptos.Contains(conce.IdConcepto)
                                  select conce).ToList();

                var listaGroupConceptosF = listaFiniquitosDetalles.GroupBy(x => x.IdConcepto).ToList();

                arrayIdConceptos = listaGroupConceptosF.Select(x => x.Key).ToArray();

                //Lista de Conceptos de los finiquitos
                listaConceptosFiniquito = (from conce in context.C_NOM_Conceptos
                                           where arrayIdConceptos.Contains(conce.IdConcepto)
                                           select conce).ToList();

                arrayIdEmpleados = listaFiniquitos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleadosFiniquitos = (from emp in context.Empleado
                                            where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                            orderby emp.APaterno
                                            select emp).ToList();

                //GET - Lista empresas fiscales
                listaEmpresasF = (from em in context.Empresa
                                  select em).ToList();

                listaClientes = (from clie in context.Cliente
                                 select clie).ToList();



                //Buscamos los contratos que se usaron en las nominas del ejercicio fiscal
                var contratosNominas = listaNominas.Select(x => x.IdContrato).ToList();
                var contratosFiniquitos = listaFiniquitos.Select(x => x.IdFiniquito).ToList();

                contratosNominas.AddRange(contratosFiniquitos);

                var arrayIdContratos = contratosNominas.ToArray();

                listaContratos = (from con in context.Empleado_Contrato
                                  where arrayIdContratos.Contains(con.IdContrato)
                                  select con).ToList();


                //-------------------------------------intentar  recuperar uids de NOM_CFDI_Timbrado
                var listaUidsNomina = from s in listaNominas
                                      select s.IdNomina;

                var listaUidsFiniquito = from s in listaFiniquitos
                                         select s.IdFiniquito;

                registrosQueSonNomina = (from s in context.NOM_CFDI_Timbrado
                                         where listaUidsNomina.Contains(s.IdNomina) && s.Cancelado == false && s.IdFiniquito == 0 && s.ErrorTimbrado == false && String.IsNullOrEmpty(s.FolioFiscalUUID) == false
                                         select s).ToList();

                registrosQueSonFiniquito = (from s in context.NOM_CFDI_Timbrado
                                            where listaUidsFiniquito.Contains(s.IdFiniquito) && s.Cancelado == false && s.IdNomina == 0 && s.ErrorTimbrado == false && String.IsNullOrEmpty(s.FolioFiscalUUID) == false
                                            select s).ToList();

                #endregion
            }


            #region UNIR LISTAS NOMINAS Y FINIQUITOS
            //Unir la lista de conceptos de nomina con la lista de conceptos del finiquito
            listaConceptos.AddRange(listaConceptosFiniquito);
            listaConceptos = listaConceptos.Distinct().ToList();

            //Unir la lista de empleados de la nomina con la lista de empleado del finiquito
            listaEmpleados.AddRange(listaEmpleadosFiniquitos);
            listaEmpleados = listaEmpleados.Distinct().ToList();
            #endregion



            //Validamos las listas
            if (listaConceptos.Count <= 0) return null;

            Empresa itemEmpresa = null;

            //Guarda el archivo en la memoria
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();

            #region DOCUMENTO EXCEL - NOMINA ********************************************************

            var worksheet = workbook.Worksheets.Add("Acumulado");
            worksheet.SheetView.Freeze(2, 4);



            #region HEADERS

            #region HEADER EMPLEADOS

            worksheet.Cell(2, 1).Value = "Id";
            worksheet.Cell(2, 2).Value = "Clave";
            worksheet.Cell(2, 3).Value = "Paterno";
            worksheet.Cell(2, 4).Value = "Materno";
            worksheet.Cell(2, 5).Value = "Nombre";
            worksheet.Cell(2, 6).Value = "RFC";
            worksheet.Cell(2, 7).Value = "UUID";   // se agrego esta columna  
            worksheet.Cell(2, 8).Value = "CURP"; //originalmente este era columna 7

            #endregion

            #region HEADER NOMINA
            worksheet.Cell(2, 9).Value = "SD";
            worksheet.Cell(2, 10).Value = "SDI";
            worksheet.Cell(2, 11).Value = "Cliente";
            worksheet.Cell(2, 12).Value = "Empresa";
            worksheet.Cell(2, 13).Value = "Periodo";
            worksheet.Cell(2, 14).Value = "Fecha timbrado";  //se agrego esta columna
            worksheet.Cell(2, 15).Value = "Bimestre";
            worksheet.Cell(2, 16).Value = "Dias Trabajados";
            worksheet.Cell(2, 17).Value = "Total Nomina";
            worksheet.Cell(2, 18).Value = "Total Finiquito";
            worksheet.Cell(2, 19).Value = "Total Liq";
            worksheet.Cell(2, 20).Value = "Total Neto";
            worksheet.Cell(2, 21).Value = "Descuento Adicional Fin";
            worksheet.Cell(2, 22).Value = "Comisiones Fin";
            worksheet.Cell(2, 23).Value = "Permisos";
            worksheet.Cell(2, 24).Value = "Incapacidades";
            worksheet.Cell(2, 25).Value = "Faltas";
            #endregion

            #region HEADER CONCEPTOS

            int columnaH = 26; //inicia en el 18    //---aqui decia 24
            //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO

            var listaConceptosPercecpciones = listaConceptos.Where(x => x.TipoConcepto == 1).ToList();
            var listaConceptosDeducciones = listaConceptos.Where(x => x.TipoConcepto == 2).ToList();

            int rangeInicial = columnaH;
            #region PERCEPCIONES
            foreach (var itemConcepto in listaConceptosPercecpciones)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheet.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    worksheet.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                }

                //MERGE CELL
                worksheet.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            #endregion

            //TOTAL DE PERCEPCIONES
            worksheet.Cell(2, columnaH).Value = "Total Percepciones";
            columnaH++;

            //Establece un estilo al header
            worksheet.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGray);


            rangeInicial = columnaH;
            #region DEDUCCIONES
            foreach (var itemConcepto in listaConceptosDeducciones)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheet.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    worksheet.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                }

                //MERGE CELL
                worksheet.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }


            worksheet.Cell(1, columnaH).Value = "Subsidio Causado";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Subsidio Entregado";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Isr antes Subsidio";
            columnaH++;


            //TOTAL DE PERCEPCIONES
            worksheet.Cell(1, columnaH).Value = "Total Deducciones";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Total ISN";
            columnaH++;
            #endregion

            //Establece un estilo al header
            worksheet.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);


            #endregion

            #region HEADER IMSS

            int mergeI = columnaH;
            int colorI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cuotas IMSS";
            worksheet.Cell(2, columnaH).Value = "Total Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Total Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cuota Fija";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Excedente";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Prestaciones";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Pensionados";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Invalidez y Vida";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Guarderia";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Seguro Retiro";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cesantía y Vejez";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Infonavit";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Riesgo Trabajo";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //Establece un estilo al header
            worksheet.Range(1, colorI, 2, columnaH).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGreen);
            columnaH++;

            #endregion


            worksheet.Cell(2, columnaH).Value = "Complemento";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Ejercicio Completo";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Base Gravable Anual";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Limite Inferior";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Excedente";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Tasa";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Impuesto Marginal";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Cuota Fija";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "ISR Retener Anual";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Saldo Favor Anual";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);



            worksheet.Cell(2, columnaH).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //MERGE CELL
            //  worksheet.Range(1, columnaH, 1, columnaH + 1).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            #endregion

            //Variables
            int fila = 3;
            int filaInicial = 0;
            int filaFinal = 0;
            bool rowColor = false;
            int idEmp = 0;
            int arraySize = 7 + 22 + 1;

            var periodosIdx = listaPeriodos.Count * 3;
            arraySize += periodosIdx;

            double[] totales = new double[arraySize];
            #region CONTENIDO

            //Por Empleado
            //Buscamos sus nominas y sus detalles de cada periodo
            foreach (var itemEmpleado in listaEmpleados)
            {
                //   var columna = 0;
                //Cuantas nominas tiene el empleado
                var nominasDelEmpleado = listaNominas.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();
                bool ejercicioCompleto = false;

                if (idEmp != itemEmpleado.IdEmpleado)
                {
                    idEmp = itemEmpleado.IdEmpleado;
                    rowColor = !rowColor;
                    rowColor = true;
                    filaInicial = fila;
                }

                //Por cada Nomina
                #region CONTENIDO NOMINA
                foreach (var itemNomina in nominasDelEmpleado)
                {
                    #region FORMA anterior
                    //#region DATOS DEL EMPLEADO
                    //worksheet.Cell(fila, 1).Value = itemNomina.IdNomina;
                    //worksheet.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                    //worksheet.Cell(fila, 3).Value = itemEmpleado.APaterno;
                    //worksheet.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                    //worksheet.Cell(fila, 5).Value = itemEmpleado.Nombres;
                    //worksheet.Cell(fila, 6).Value = itemEmpleado.RFC;
                    //worksheet.Cell(fila, 7).Value = itemEmpleado.CURP;
                    //#endregion

                    //#region DATOS NOMINA

                    //worksheet.Cell(fila, 8).Value = itemNomina.SD;
                    //worksheet.Cell(fila, 9).Value = itemNomina.SDI;
                    ////worksheet.Cell(fila, 10).Value = itemNomina.SBC;

                    ////Empresa - Periodo - Cliente
                    //var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemNomina.IdPeriodo);

                    //itemEmpresa = null;

                    //if (itemPeriodo.IdTipoNomina <= 15) //normal
                    //{
                    //    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaFiscal);
                    //}
                    //else if (itemPeriodo.IdTipoNomina == 16)
                    //{
                    //    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaAsimilado);
                    //}


                    //var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemNomina.IdCliente);

                    //// worksheet.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                    //worksheet.Cell(fila, 10).Value = itemCliente != null ? itemCliente.Nombre : "-";
                    //worksheet.Cell(fila, 11).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                    //worksheet.Cell(fila, 12).Value = itemPeriodo?.Bimestre ?? 0;

                    //if (incluirNoAutorizados)
                    //{
                    //    if (itemPeriodo.Autorizado == false)
                    //    {
                    //        worksheet.Cell(fila, 12).Style
                    //            .Font.SetFontSize(13)
                    //            .Font.SetBold(true)
                    //            //.Font.SetFontColor(XLColor.White)
                    //            .Fill.SetBackgroundColor(XLColor.Gray);
                    //    }
                    //}

                    //worksheet.Cell(fila, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    //worksheet.Cell(fila, 13).Value = itemNomina.Dias_Laborados;
                    //worksheet.Cell(fila, 14).Value = itemNomina.TotalNomina;

                    //#region CONCEPTOS

                    //int columna = 15;

                    //#region PERCEPCIONES
                    ////Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
                    //foreach (var itemConcepto in listaConceptosPercecpciones)
                    //{
                    //    decimal total = 0;
                    //    decimal gravado = 0;
                    //    decimal exento = 0;
                    //    //Busca en los detalles de la nomina por concepto y por id de nomina
                    //    var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);

                    //    if (itemDetalle != null)
                    //    {
                    //        total = itemDetalle.Total;
                    //        gravado = itemDetalle.GravadoISR;
                    //        exento = itemDetalle.ExentoISR;
                    //    }
                    //    //  43 - ISR
                    //    //  144 - Subsidio
                    //    //  42 - SS
                    //    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = gravado;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = exento;
                    //        columna++;
                    //        //worksheet.Cell(fila, columna).Value = isn;
                    //        //columna++;
                    //    }
                    //}


                    //#endregion

                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalPercepciones;
                    //columna++;

                    //#region DEDUCCIONES
                    ////Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
                    //foreach (var itemConcepto in listaConceptosDeducciones)
                    //{
                    //    decimal total = 0;
                    //    decimal gravado = 0;
                    //    decimal exento = 0;
                    //    //Busca en los detalles de la nomina por concepto y por id de nomina
                    //    var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);

                    //    if (itemDetalle != null)
                    //    {
                    //        total = itemDetalle.Total;
                    //        gravado = itemDetalle.GravadoISR;
                    //        exento = itemDetalle.ExentoISR;
                    //    }
                    //    //  43 - ISR
                    //    //  144 - Subsidio
                    //    //  42 - SS
                    //    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = gravado;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = exento;
                    //        columna++;
                    //        //worksheet.Cell(fila, columna).Value = isn;
                    //        //columna++;
                    //    }
                    //}
                    //#endregion


                    //#endregion

                    //worksheet.Cell(fila, columna).Value = itemNomina.SubsidioCausado;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.SubsidioEntregado;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.ISRantesSubsidio;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalDeducciones;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalImpuestoSobreNomina;
                    //columna++;
                    //#endregion

                    //#region CUOTAS IMSS

                    //var itemCuotaImss = listaCuotas.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina);
                    //decimal totalPatron = 0;
                    //decimal totalObrero = 0;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalPatron ?? 0;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalObrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Cuota_Fija_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.GuarderiasPrestaciones_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.SeguroRetiro_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Infonavit_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.RiesgoTrabajo_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;
                    //#endregion


                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalComplemento;
                    //worksheet.Cell(fila, columna).Style.NumberFormat.Format = "$ ###,##0.00";

                    #endregion

                    ejercicioCompleto = ContenidoAcumulado(ref worksheet, itemNomina, null, itemEmpleado, listaPeriodos, listaEmpresasF, listaClientes, listaCuotas, listaDetalles, null, listaConceptosPercecpciones, listaConceptosDeducciones, incluirNoAutorizados, false, fila, null, listaContratos, ejercicioFiscal, listaPermisos, listaIncapacidadeses, listafaltas, registrosQueSonNomina, registrosQueSonFiniquito);

                    //Sumar los totales de Gravados Exentos - ISN

                    fila++;
                    //rowColor = !rowColor;
                }//Fin del For de la nominas del empleado
                #endregion


                #region CONTENIDO FINIQUITO

                //Obtener los finiquitos del empleado  
                var finiquitosEmpleado = listaFiniquitos.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();

                foreach (var itemFiniquito in finiquitosEmpleado)
                {
                    ejercicioCompleto = ContenidoAcumulado(ref worksheet, null, itemFiniquito, itemEmpleado, listaPeriodos, listaEmpresasF, listaClientes, listaCuotas, null, listaFiniquitosDetalles, listaConceptosPercecpciones, listaConceptosDeducciones, incluirNoAutorizados, true, fila, listaDescuentoComisionFiniquito, listaContratos, ejercicioFiscal, listaPermisos, listaIncapacidadeses, listafaltas, registrosQueSonNomina, registrosQueSonFiniquito);

                    //set color al row de finiquito
                    worksheet.Range(fila, 1, fila, columnaH).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                    fila++;
                }


                #endregion


                if (rowColor)
                {
                    rowColor = false;
                    worksheet.Range(fila, 1, fila, columnaH).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(fila, 1, fila, columnaH).Style.Border.TopBorderColor = XLColor.SlateGray;
                    //worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Lavender;
                    //worksheet.Range(fila, 1, fila, columna).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    //Agregamos la sumatoria
                    if (nominasDelEmpleado.Count > 0)
                    {
                        SumarColumnas(ref worksheet, filaInicial, fila - 1, columnaH - 8, ejercicioCompleto, listaPeriodos[0].IdEjercicio, nominasDelEmpleado[0].IdEmpleado, nominasDelEmpleado[nominasDelEmpleado.Count - 1].SD, nominasDelEmpleado[nominasDelEmpleado.Count - 1].SDI, nominasDelEmpleado[0].IdEmpresaFiscal, calculoanual, UMA);
                    }
                    fila++;
                    fila++;
                }

            }//Fin For por empleado

            #endregion

            #region  COLORES DISEÑO DE CELDAS

            //Establece un estilo al header
            worksheet.Range("G2:Y2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Raspberry);

            //Establece un estilo al header
            worksheet.Range("A2:F2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue);


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
            worksheet.Column(15).AdjustToContents();
            worksheet.Column(16).AdjustToContents();
            worksheet.Column(17).AdjustToContents();
            worksheet.Column(18).AdjustToContents();
            worksheet.Column(19).AdjustToContents();
            worksheet.Column(20).AdjustToContents();
            worksheet.Column(21).AdjustToContents();

            #endregion

            #endregion

            #region DOCUMENTO EXCEL - FINIQUITO *****************************************************
            /*
            var worksheetF = workbook.Worksheets.Add("Finiquitos");
            worksheetF.SheetView.Freeze(2, 4);

            #region HEADERS

            #region EMPLEADO 

            worksheetF.Cell(2, 1).Value = "IdFiniquito";
            worksheetF.Cell(2, 2).Value = "Clave";
            worksheetF.Cell(2, 3).Value = "Paterno";
            worksheetF.Cell(2, 4).Value = "Materno";
            worksheetF.Cell(2, 5).Value = "Nombre";
            worksheetF.Cell(2, 6).Value = "RFC";
            worksheetF.Cell(2, 7).Value = "CURP";

            #endregion

            #region DATOS

            worksheetF.Cell(2, 8).Value = "SD";
            worksheetF.Cell(2, 9).Value = "SDI";

            worksheetF.Cell(2, 10).Value = "Cliente";
            worksheetF.Cell(2, 11).Value = "Periodo";
            worksheetF.Cell(2, 12).Value = "Bimestre";
            worksheetF.Cell(2, 13).Value = "Total Finiquito";
            worksheetF.Cell(2, 14).Value = "Total Liq";
            worksheetF.Cell(2, 15).Value = "Total Neto";

            //Establece un estilo al header
            worksheetF.Range("A2:O2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue); //AirForceBlue

            #endregion

            #region CONCEPTOS

            columnaH = 16; //inicia en el 18
                           //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO

            rangeInicial = columnaH;
            foreach (var itemConcepto in listaConceptosFiniquito)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;

                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheetF.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    // worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                    worksheetF.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                    //worksheetF.Cell(2, columnaH).Value = "ISN";
                    //colFinal = columnaH;
                    //columnaH++;
                }


                //MERGE CELL
                worksheetF.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            //Establece un estilo al header
            worksheetF.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGray);

            #endregion

            #region HEADER DESCUENTOS Y COMISIONES
            //TOTAL ISN
            rangeInicial = columnaH;
            worksheetF.Cell(2, columnaH).Value = "TOTAL ISN";
            columnaH++;
            //DESCUENTOS
            worksheetF.Cell(2, columnaH).Value = "DESCUENTO ADICIONAL";
            columnaH++;
            //COMISIONES
            worksheetF.Cell(2, columnaH).Value = "COMISIONES";
            columnaH++;


            //Establece un estilo al header
            worksheetF.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);
            #endregion

            #region HEADER IMSS

            mergeI = columnaH;
            colorI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cuotas IMSS";
            worksheetF.Cell(2, columnaH).Value = "Total Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Total Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cuota Fija";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Excedente";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Prestaciones";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Pensionados";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Invalidez y Vida";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Guarderia";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Seguro Retiro";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cesantía y Vejez";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Infonavit";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Riesgo Trabajo";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            //Establece un estilo al header
            worksheetF.Range(1, colorI, 2, columnaH).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGreen);
            columnaH++;


            //MERGE CELL
            //  worksheet.Range(1, columnaH, 1, columnaH + 1).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheetF.Cell(2, columnaH).Value = "Complemento";
            worksheetF.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);

            #endregion

            #endregion

            #region CONTENIDO

            #region CONCEPTOS
            fila = 3;
            foreach (var itemFiniquito in listaFiniquitos)
            {
                //GET dato empleado
                var itemEmpleado = listaEmpleadosFiniquitos.FirstOrDefault(x => x.IdEmpleado == itemFiniquito.IdEmpleado);

                if (itemEmpleado == null) continue;

                worksheetF.Cell(fila, 1).Value = itemFiniquito.IdFiniquito;
                worksheetF.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                worksheetF.Cell(fila, 3).Value = itemEmpleado.APaterno;
                worksheetF.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                worksheetF.Cell(fila, 5).Value = itemEmpleado.Nombres;
                worksheetF.Cell(fila, 6).Value = itemEmpleado.RFC;
                worksheetF.Cell(fila, 7).Value = itemEmpleado.CURP;

                worksheetF.Cell(fila, 8).Value = itemFiniquito.SD;
                worksheetF.Cell(fila, 9).Value = itemFiniquito.SDI;

                //Empresa - Periodo
                var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemFiniquito.IdPeriodo);
                itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaFiscal) ??
                                 listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaAsimilado);

                var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemFiniquito.IdCliente);

                // worksheetF.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                worksheetF.Cell(fila, 10).Value = itemCliente != null ? itemCliente.Nombre : "-";
                worksheetF.Cell(fila, 11).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                worksheetF.Cell(fila, 12).Value = itemPeriodo.Bimestre;
                worksheetF.Cell(fila, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheetF.Cell(fila, 13).Value = itemFiniquito.Total_Finiquito;
                worksheetF.Cell(fila, 14).Value = itemFiniquito.Total_Liquidacion;
                worksheetF.Cell(fila, 15).Value = itemFiniquito.TOTAL_total;

                int columna = 16; //
                decimal totalIsn = 0;
                foreach (var itemConcepto in listaConceptosFiniquito)
                {
                    decimal total = 0;
                    decimal gravado = 0;
                    decimal exento = 0;
                    decimal isn = 0;

                    var itemDetalleF =
                        listaFiniquitosDetalles.FirstOrDefault(
                            x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto);

                    if (itemDetalleF != null)
                    {
                        total = itemDetalleF.Total;
                        gravado = itemDetalleF.GravadoISR;
                        exento = itemDetalleF.ExentoISR;
                        isn = itemDetalleF.ImpuestoSobreNomina;
                        totalIsn += isn;
                    }


                    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    {
                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                    }
                    else
                    {
                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = gravado;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = exento;
                        columna++;
                        //worksheetF.Cell(fila, columna).Value = isn;
                        //columna++;
                    }
                }



                #region  DESCUENTOS Y COMISIONES

                var descuentosAdicionales = listaDescuentoComisionFiniquito.Where(x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 2)
                       .Select(x => x.TotalDescuento)
                       .Sum();

                var comisiones = listaDescuentoComisionFiniquito.Where(x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 1)
                        .Select(x => x.TotalDescuento)
                        .Sum();
                //TOTAL ISN
                worksheetF.Cell(fila, columna).Value = totalIsn;
                columna++;
                //DESCUENTOS
                worksheetF.Cell(fila, columna).Value = descuentosAdicionales;
                columna++;
                //COMISIONES
                worksheetF.Cell(fila, columna).Value = comisiones;
                columna++;
                #endregion


                #region CUOTAS IMSS

                var itemCuotaImss = listaCuotasFiniquito.FirstOrDefault(x => x.IdFiniquito == itemFiniquito.IdFiniquito);
                decimal totalPatron = 0;
                decimal totalObrero = 0;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.TotalPatron ?? 0;
                columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.TotalObrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Cuota_Fija_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.GuarderiasPrestaciones_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.SeguroRetiro_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Infonavit_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.RiesgoTrabajo_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;
                #endregion

                worksheetF.Cell(fila, columna).Value = itemFiniquito.TotalComplemento;

                // if (rowColor)
                //    {
                //        // worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Lavender;
                //        worksheetF.Range(fila, 1, fila, columna).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                //    }

                //rowColor = !rowColor;
                fila++;
            }

            #endregion

            #endregion

            #region AJUSTE DE CELDAS

            worksheetF.Column(1).AdjustToContents(1);
            worksheetF.Column(2).AdjustToContents(2);
            worksheetF.Column(3).AdjustToContents(3);
            worksheetF.Column(4).AdjustToContents(4);
            worksheetF.Column(5).AdjustToContents(5);
            worksheetF.Column(6).AdjustToContents(6);
            worksheetF.Column(7).AdjustToContents(7);
            worksheetF.Column(8).AdjustToContents(8);
            worksheetF.Column(9).AdjustToContents(9);
            worksheetF.Column(10).AdjustToContents(10);
            worksheetF.Column(11).AdjustToContents(11);
            worksheetF.Column(12).AdjustToContents(12);
            worksheetF.Column(13).AdjustToContents(13);
            worksheetF.Column(14).AdjustToContents(14);
            worksheetF.Column(15).AdjustToContents(15);
            worksheetF.Column(16).AdjustToContents(16);
            worksheetF.Column(17).AdjustToContents(17);
            worksheetF.Column(18).AdjustToContents(18);
            worksheetF.Column(19).AdjustToContents(19);
            worksheetF.Column(20).AdjustToContents(20);
            worksheetF.Column(21).AdjustToContents(21);


            #endregion
            */
            //fin hoja de finiquito
            #endregion

            // AddColsUuidFechaCertificacion( worksheet, ctx.NOM_CFDI_Timbrado.ToList<NOM_CFDI_Timbrado>() );    //mandar a agregar columna  UUID y fecha de certificacion


            #region CREA EL ARCHIVO Y RETORNA EL PATH RELATIVO PARA LA DESCARGA



            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "Acumulado_.xlsx";

            if (itemEmpresa != null)
            {
                nombreArchivo = "ACUM_" + itemEmpresa.RazonSocial + "_.xlsx";
            }

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);
            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

            //Guarda el documento en la memori
            //workbook.SaveAs(ms, false);
            //var archivoByte =  ms.ToArray();
            //return archivoByte;
            #endregion
        }

        private bool ContenidoAcumulado(ref IXLWorksheet worksheet, NOM_Nomina itemNomina, NOM_Finiquito itemFiniquito, Empleado itemEmpleado,
            List<NOM_PeriodosPago> listaPeriodos,
            List<Empresa> listaEmpresasF,
            List<Cliente> listaClientes,
            List<NOM_Cuotas_IMSS> listaCuotas,
            List<NOM_Nomina_Detalle> listaDetalles,
            List<NOM_Finiquito_Detalle> listaDetallesF,
            List<C_NOM_Conceptos> listaConceptosPercecpciones,
            List<C_NOM_Conceptos> listaConceptosDeducciones,
            bool incluirNoAutorizados,
            bool isFiniquito,
            int fila,
            List<NOM_Finiquito_Descuento_Adicional> listaDescuentoComisionFiniquito,
            List<Empleado_Contrato> listaContratos,
            NOM_Ejercicio_Fiscal itemEjercicioFiscal,
            List<Permisos> listaPermisos,
            List<Incapacidades> listaIncapacidades,
            List<Inasistencias> listafaltas,
            List<NOM_CFDI_Timbrado> registrosQueSonNomina,
            List<NOM_CFDI_Timbrado> registrosQueSonFiniquito
            )
        {
            #region DATOS DEL EMPLEADO
            worksheet.Cell(fila, 1).Value = isFiniquito ? itemFiniquito.IdFiniquito : itemNomina.IdNomina;
            worksheet.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
            worksheet.Cell(fila, 3).Value = itemEmpleado.APaterno;
            worksheet.Cell(fila, 4).Value = itemEmpleado.AMaterno;
            worksheet.Cell(fila, 5).Value = itemEmpleado.Nombres;
            worksheet.Cell(fila, 6).Value = itemEmpleado.RFC;


            NOM_CFDI_Timbrado auxTimbrado = isFiniquito ? registrosQueSonFiniquito.SingleOrDefault(s => s.IdFiniquito == itemFiniquito.IdFiniquito) : registrosQueSonNomina.SingleOrDefault(s => s.IdNomina == itemNomina.IdNomina);
            worksheet.Cell(fila, 7).Value = auxTimbrado != null ? auxTimbrado.FolioFiscalUUID : "";  //asignando UUID 
            worksheet.Cell(fila, 8).Value = itemEmpleado.CURP;

            #endregion

            #region DATOS NOMINA

            worksheet.Cell(fila, 9).Value = isFiniquito ? itemFiniquito.SD : itemNomina.SD;
            worksheet.Cell(fila, 10).Value = isFiniquito ? itemFiniquito.SDI : itemNomina.SDI;
            //worksheet.Cell(fila, 10).Value = itemNomina.SBC;

            //Empresa - Periodo - Cliente
            NOM_PeriodosPago itemPeriodo = null;

            if (isFiniquito)
            {
                itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemFiniquito.IdPeriodo);
            }
            else
            {
                itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemNomina.IdPeriodo);
            }


            Empresa itemEmpresa = null;

            if (itemPeriodo.IdTipoNomina <= 15) //normal
            {
                if (isFiniquito)
                {
                    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaFiscal);
                }
                else
                {
                    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaFiscal);
                }
            }
            else if (itemPeriodo.IdTipoNomina == 16)//Asimilado
            {
                if (isFiniquito)
                {
                    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaAsimilado);
                }
                else
                {
                    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaAsimilado);
                }

            }


            Cliente itemCliente = null;

            if (isFiniquito)
            {
                itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemFiniquito.IdCliente);
            }
            else
            {
                itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemNomina.IdCliente);
            }



            // worksheet.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
            worksheet.Cell(fila, 11).Value = itemCliente != null ? itemCliente.Nombre : "-";
            worksheet.Cell(fila, 12).Value = itemEmpresa != null ? $"{itemEmpresa.IdEmpresa} {itemEmpresa.RazonSocial}" : "-";
            worksheet.Cell(fila, 13).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
            worksheet.Cell(fila, 14).Value = auxTimbrado != null ? auxTimbrado.FechaCertificacion.Value.Date.ToString() : ""; //asignando  la fecha de certificacion (nombre en bd)

            worksheet.Cell(fila, 15).Value = itemPeriodo?.Bimestre ?? 0;

            if (incluirNoAutorizados)
            {
                if (itemPeriodo.Autorizado == false)
                {
                    worksheet.Cell(fila, 15).Style.Font.SetFontSize(13).Font.SetBold(true)//.Font.SetFontColor(XLColor.White)
                        .Fill.SetBackgroundColor(XLColor.Gray);
                }
            }

            worksheet.Cell(fila, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(fila, 16).Value = isFiniquito ? 0 : itemNomina.Dias_Laborados;

            //Dias Permisos - Dias Incapacidad

            #region PERMISOS - INCAPACIDADES
            int totalPermisos = 0;
            int totalIncapacidades = 0;
            int totalfaltas = 0;

            if (!isFiniquito)
            {
                var listaPermisosXEmpleado = listaPermisos.Where(x => x.IdEmpleado == itemNomina.IdEmpleado);

                foreach (var itemPermiso in listaPermisosXEmpleado)
                {
                    var numDias = Utils.GetDiasByPeriodoRange(itemPermiso.FechaInicio, itemPermiso.FechaFin,
                        itemPeriodo.Fecha_Inicio, itemPeriodo.Fecha_Fin);
                    totalPermisos += numDias;
                }

                var listaIncapacidadxEmpleado = listaIncapacidades.Where(x => x.IdEmpleado == itemNomina.IdEmpleado);


                foreach (var itemIncapacidad in listaIncapacidadxEmpleado)
                {
                    var numDias = Utils.GetDiasByPeriodoRange(itemIncapacidad.FechaInicio, itemIncapacidad.FechaFin,
                        itemPeriodo.Fecha_Inicio, itemPeriodo.Fecha_Fin);

                    totalIncapacidades += numDias;
                }

                var listafaltasxEmpleado = listafaltas.Where(x => x.IdEmpleado == itemNomina.IdEmpleado);



                totalfaltas = listafaltasxEmpleado.Where(x => x.Fecha >= itemPeriodo.Fecha_Inicio && x.Fecha <= itemPeriodo.Fecha_Fin).Select(x => x.Dias).Sum();
                //itemPeriodo.Fecha_Inicio, itemPeriodo.Fecha_Fin);



            }

            #endregion
            
            worksheet.Cell(fila, 17).Value = isFiniquito ? 0 : itemNomina.TotalNomina;
            worksheet.Cell(fila, 18).Value = isFiniquito ? itemFiniquito.Total_Finiquito : 0;
            worksheet.Cell(fila, 19).Value = isFiniquito ? itemFiniquito.Total_Liquidacion : 0;
            worksheet.Cell(fila, 20).Value = isFiniquito ? itemFiniquito.TOTAL_total : itemNomina.TotalNomina;

            #region  DESCUENTOS Y COMISIONES

            decimal descuentosAdicionales = 0;
            decimal comisiones = 0;

            if (isFiniquito)
            {
                descuentosAdicionales =
                    listaDescuentoComisionFiniquito.Where(
                            x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 2)
                        .Select(x => x.TotalDescuento)
                        .Sum();


                comisiones =
                    listaDescuentoComisionFiniquito.Where(
                            x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 1)
                        .Select(x => x.TotalDescuento)
                        .Sum();
            }

            #endregion

            worksheet.Cell(fila, 21).Value = isFiniquito ? descuentosAdicionales : 0;
            worksheet.Cell(fila, 22).Value = isFiniquito ? comisiones : 0;

            worksheet.Cell(fila, 23).Value = totalPermisos;
            worksheet.Cell(fila, 24).Value = totalIncapacidades;
            worksheet.Cell(fila, 25).Value = totalfaltas;


            #region CONCEPTOS

            int columna = 26;
            decimal totalIsn = 0;
            #region PERCEPCIONES
            //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
            foreach (var itemConcepto in listaConceptosPercecpciones)
            {
                decimal total = 0;
                decimal gravado = 0;
                decimal exento = 0;
                //Busca en los detalles de la nomina por concepto y por id de nomina
                if (isFiniquito)
                {
                    //var itemDetalleF = listaDetallesF.FirstOrDefault(x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto);
                    var itemDetalleF = listaDetallesF.Where(x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto).ToList();

                    if (itemDetalleF != null)
                    {
                        if (itemDetalleF.Any())
                        {
                            total = itemDetalleF.Sum(x => x.Total);
                            gravado = itemDetalleF.Sum(x => x.GravadoISR);
                            exento = itemDetalleF.Sum(x => x.ExentoISR);
                            totalIsn += itemDetalleF.Sum(x => x.ImpuestoSobreNomina);
                        }
                    }
                }
                else
                {
                    //var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);
                    var itemDetalle = listaDetalles.Where(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto).ToList();

                    if (itemDetalle != null)
                    {
                        if (itemDetalle.Any())
                        {
                            total = itemDetalle.Sum(x => x.Total);
                            gravado = itemDetalle.Sum(x => x.GravadoISR);
                            exento = itemDetalle.Sum(x => x.ExentoISR);
                        }
                    }
                }

                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(fila, columna).Value = total;
                    columna++;
                }
                else
                {
                    worksheet.Cell(fila, columna).Value = total;
                    columna++;
                    worksheet.Cell(fila, columna).Value = gravado;
                    columna++;
                    worksheet.Cell(fila, columna).Value = exento;
                    columna++;
                    //worksheet.Cell(fila, columna).Value = isn;
                    //columna++;
                }
            }


            #endregion

            worksheet.Cell(fila, columna).Value = isFiniquito ? itemFiniquito.TotalPercepciones : itemNomina.TotalPercepciones;
            columna++;

            #region DEDUCCIONES
            //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
            foreach (var itemConcepto in listaConceptosDeducciones)
            {
                decimal total = 0;
                decimal gravado = 0;
                decimal exento = 0;

                if (isFiniquito)
                {
                    //var itemDetalleF = listaDetallesF.FirstOrDefault(x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto);
                    var itemDetalleF = listaDetallesF.Where(x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto).ToList();

                    if (itemDetalleF != null)
                    {
                        if (itemDetalleF.Any())
                        {
                            total = itemDetalleF.Sum(x => x.Total);
                            gravado = itemDetalleF.Sum(x => x.GravadoISR);
                            exento = itemDetalleF.Sum(x => x.ExentoISR);
                        }
                    }
                }
                else
                {
                    //Busca en los detalles de la nomina por concepto y por id de nomina
                    //var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);
                    var itemDetalle = listaDetalles.Where(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto).ToList();

                    if (itemDetalle != null)
                    {
                        if (itemDetalle.Any())
                        {
                            total = itemDetalle.Sum(x => x.Total);
                            gravado = itemDetalle.Sum(x => x.GravadoISR);
                            exento = itemDetalle.Sum(x => x.ExentoISR);
                        }
                    }
                }

                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(fila, columna).Value = total;
                    columna++;
                }
                else
                {
                    worksheet.Cell(fila, columna).Value = total;
                    columna++;
                    worksheet.Cell(fila, columna).Value = gravado;
                    columna++;
                    worksheet.Cell(fila, columna).Value = exento;
                    columna++;
                    //worksheet.Cell(fila, columna).Value = isn;
                    //columna++;
                }
            }
            #endregion


            #endregion

            worksheet.Cell(fila, columna).Value = isFiniquito ? itemFiniquito.SubsidioCausado : itemNomina.SubsidioCausado;
            columna++;
            worksheet.Cell(fila, columna).Value = isFiniquito ? itemFiniquito.SubsidioEntregado : itemNomina.SubsidioEntregado;
            columna++;
            worksheet.Cell(fila, columna).Value = isFiniquito ? 0 : itemNomina.ISRantesSubsidio;
            columna++;
            worksheet.Cell(fila, columna).Value = isFiniquito ? itemFiniquito.TotalDeducciones : itemNomina.TotalDeducciones;
            columna++;
            worksheet.Cell(fila, columna).Value = isFiniquito ? totalIsn : itemNomina.TotalImpuestoSobreNomina;
            columna++;
            #endregion

            #region CUOTAS IMSS

            NOM_Cuotas_IMSS itemCuotaImss = null;
            if (isFiniquito)
            {
                itemCuotaImss = listaCuotas.FirstOrDefault(x => x.IdNomina == itemFiniquito.IdFiniquito);
            }
            else
            {
                itemCuotaImss = listaCuotas.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina);
            }

            decimal totalPatron = 0;
            decimal totalObrero = 0;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalPatron ?? 0;
            columna++;
            worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalObrero ?? 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.Cuota_Fija_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Obrero ?? 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Obrero ?? 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Obrero ?? 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Obrero ?? 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.GuarderiasPrestaciones_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.SeguroRetiro_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Obrero ?? 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.Infonavit_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = 0; columna++;

            worksheet.Cell(fila, columna).Value = itemCuotaImss?.RiesgoTrabajo_Patron ?? 0; columna++;
            worksheet.Cell(fila, columna).Value = 0; columna++;
            #endregion

            worksheet.Cell(fila, columna).Value = isFiniquito ? itemFiniquito.TotalComplemento : itemNomina.TotalComplemento;
            worksheet.Cell(fila, columna).Style.NumberFormat.Format = "$ ###,##0.00";

            //Buscamos su ultimo contrato en el ejercicio
            bool añoCompleto = false;
            var itemContrato =
                listaContratos.OrderByDescending(x => x.FechaAlta)
                    .FirstOrDefault(x => x.IdEmpleado == itemEmpleado.IdEmpleado);

            if (itemContrato != null)
            {
                bool alta = false;
                bool baja = false;
                var fechaAlta = itemContrato.FechaIMSS;
                var fechaBaja = itemContrato.FechaBaja;
                var fechaEjercicio = itemEjercicioFiscal.Anio;

                int añoFiscal = Int32.Parse(fechaEjercicio);

                DateTime inicioEjercicio = new DateTime(añoFiscal, 01, 01);
                DateTime finEjercicio = new DateTime(añoFiscal, 12, 15);


                if (fechaAlta <= inicioEjercicio)
                {
                    alta = true;
                }


                if (fechaBaja == null)
                {
                    baja = true;
                }
                else
                {
                    if (fechaBaja >= finEjercicio)
                    {
                        baja = true;
                    }
                }


                if (alta && baja)
                {
                    añoCompleto = true;
                }

            }

            // worksheet.Cell(fila, columna + 1).Value = añoCompleto ? "x" : "";

            worksheet.Cell(fila, columna + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            return añoCompleto;
        }


        private void SumarColumnas(ref IXLWorksheet worksheet, int filaInicial, int filaFinal, int columna, bool ejercicioCompleto, int idejercicio, int idempleado, decimal sd, decimal sdi, int? idempresa, bool calculoanual, decimal UMA)
        {
            int filaDelTotal = filaFinal + 1;

            string[] letra = new[]
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM",
                "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD",
                "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU",
                "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL",
                "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ"
            };

            for (int c = 2; c < columna - 1; c++)
            {

                if (c < 15)
                {
                    if (c == 12 || c == 13 || c == 7 || c == 14) continue;

                    var contenido = worksheet.Cell(filaDelTotal - 1, c).Value;
                    worksheet.Cell(filaDelTotal, c).Value = contenido;
                }
                else
                {
                    string formula = $"=SUM({letra[c]}{filaInicial}:{letra[c]}{filaFinal})";
                    worksheet.Cell(filaDelTotal, c + 1).FormulaA1 = formula;
                }


                worksheet.Range(filaDelTotal, 1, filaDelTotal, columna).Style.Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                worksheet.Range(filaDelTotal, 1, filaDelTotal, columna).Style.Font.SetBold(true);
                worksheet.Range(filaDelTotal, columna, filaDelTotal, columna + 8).Style.Fill.SetBackgroundColor(XLColor.LightBlue);
                worksheet.Range(filaDelTotal, columna, filaDelTotal, columna + 8).Style.Font.SetBold(true);

                //worksheet.Cell("B9").SetFormulaA1("=SUM(B3:B8)");
            }

            if (ejercicioCompleto)
            {
                worksheet.Cell(filaDelTotal, columna).Value = "x";
                worksheet.Cell(filaDelTotal, columna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (calculoanual)
                {
                    var calculoAnual = MNominas.GetCalculoAnualconPrevision(idempleado, idejercicio, sd, sdi, idempresa, 0, UMA);
                    worksheet.Cell(filaDelTotal, columna + 1).Value = calculoAnual.BaseGravable;
                    worksheet.Cell(filaDelTotal, columna + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(filaDelTotal, columna + 2).Value = calculoAnual.LimiteInferior;
                    worksheet.Cell(filaDelTotal, columna + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(filaDelTotal, columna + 3).Value = calculoAnual.Base;
                    worksheet.Cell(filaDelTotal, columna + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(filaDelTotal, columna + 4).Value = calculoAnual.Tasa;
                    worksheet.Cell(filaDelTotal, columna + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(filaDelTotal, columna + 5).Value = calculoAnual.marginal;// Se usa esta propiedad para tomar el impuesto marginal
                    worksheet.Cell(filaDelTotal, columna + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(filaDelTotal, columna + 6).Value = calculoAnual.Cuotafija;// Se usa esta propiedad para tomar la cuota fija
                    worksheet.Cell(filaDelTotal, columna + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(filaDelTotal, columna + 7).Value = calculoAnual.ResultadoIsrOSubsidio > 0 ? calculoAnual.ResultadoIsrOSubsidio : 0;
                    worksheet.Cell(filaDelTotal, columna + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(filaDelTotal, columna + 8).Value = calculoAnual.ResultadoIsrOSubsidio < 0 ? calculoAnual.ResultadoIsrOSubsidio : 0;
                    worksheet.Cell(filaDelTotal, columna + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

            }

        }





        //NUEVO ACUMULADO -- 
        //Ya no será por todas las empresas sino solo una
        

        public string GenerarArchivoAcumuladoComplete2_NUEVO(int idUsuario, string pathFolder, string pathDescarga,
    int? idEjercicio, DateTime? dateInicial, DateTime? dateFinal,
    int idEmpresa = 0, int idCliente = 0, bool incluirNoAutorizados = false, bool calculoanual = false)
        {
            decimal UMA = 0;
            //Validaciones
            if (pathFolder.Trim() == "" || pathDescarga.Trim() == "" || dateInicial == null || dateFinal == null || idEjercicio == null)
                return null;


            #region Variables - List<>

            List<C_NOM_Conceptos> listaConceptos;
            List<Empleado> listaEmpleados;
            List<NOM_Nomina> listaNominas;
            List<NOM_Nomina_Detalle> listaDetalles;
            List<NOM_Cuotas_IMSS> listaCuotas;
            List<NOM_Cuotas_IMSS> listaCuotasFiniquito;
            List<Empresa> listaEmpresasF;
            List<NOM_PeriodosPago> listaPeriodos;
            List<NOM_Finiquito> listaFiniquitos;
            List<NOM_Finiquito_Detalle> listaFiniquitosDetalles;
            List<C_NOM_Conceptos> listaConceptosFiniquito;
            List<Empleado> listaEmpleadosFiniquitos;
            List<Cliente> listaClientes;
            List<NOM_Finiquito_Descuento_Adicional> listaDescuentoComisionFiniquito;
            List<Empleado_Contrato> listaContratos;
            NOM_Ejercicio_Fiscal ejercicioFiscal;

            List<Permisos> listaPermisos;
            List<Inasistencias> listafaltas;
            List<Incapacidades> listaIncapacidadeses;

            List<NOM_CFDI_Timbrado> registrosQueSonNomina;
            List<NOM_CFDI_Timbrado> registrosQueSonFiniquito;
            #endregion

            using (var context = new RHEntities())
            {
                #region GET DATA ACUMULADO

                UMA = context.ZonaSalario.FirstOrDefault().UMA;

                //obtener los periodos que se timbraron
                //obtener las sucursales de esos periodos
                //obtener id empleados 
                //obtener los id de nominas
                //pendiente agregar autorizados de inicio no se agregará




                ejercicioFiscal = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idEjercicio);

                if (incluirNoAutorizados)

                {
                    listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where DbFunctions.TruncateTime(p.Fecha_Fin) >= DbFunctions.TruncateTime(dateInicial) && DbFunctions.TruncateTime(p.Fecha_Fin) <= DbFunctions.TruncateTime(dateFinal)
                                           && p.IdEjercicio == idEjercicio
                                           //&& p.Autorizado == true
                                           //p.CfdiGenerado == (int) GenerarCfdiEstatus.Generado
                                           && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
                                           && p.SoloComplemento == false
                                     //Que no sean solo complemento
                                     select p).ToList();
                }
                else
                {
                    listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where DbFunctions.TruncateTime(p.Fecha_Fin) >= DbFunctions.TruncateTime(dateInicial) && DbFunctions.TruncateTime(p.Fecha_Fin) <= DbFunctions.TruncateTime(dateFinal)
                                           && p.IdEjercicio == idEjercicio
                                           && p.Autorizado == true
                                           //p.CfdiGenerado == (int) GenerarCfdiEstatus.Generado
                                           && (p.IdTipoNomina <= 16) //Nominas - Asimilados - y finiquitos
                                           && p.SoloComplemento == false
                                     //Que no sean solo complemento
                                     select p).ToList();
                }


                var arrayPeriodosAutorizados = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                //GET - lista de nominas que estan en el array de periodos
                listaNominas = (from n in context.NOM_Nomina
                                where arrayPeriodosAutorizados.Contains(n.IdPeriodo)
                                select n).ToList();

                //GET - lista de finiquitos que estan en el array de periodos
                listaFiniquitos = (from f in context.NOM_Finiquito
                                   where arrayPeriodosAutorizados.Contains(f.IdPeriodo)
                                   select f).ToList();

                //1er Filtro- por Empresa
                if (idEmpresa > 0)
                {
                    #region NOMINAS FILTRO

                    //Buscamos las nominas que se generaron con la empresa en Fiscal
                    var arrayPeriodosFiscales =
                        listaPeriodos.Where(x => x.IdTipoNomina <= 15).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Fiscales
                    var listaF = (from n in listaNominas
                                  where arrayPeriodosFiscales.Contains(n.IdPeriodo)
                                        && n.IdEmpresaFiscal == idEmpresa
                                  select n).ToList();

                    //Buscamos las nominas que se generaron con la empresa en Asimilado
                    var arrayPeriodosAsimilados =
                        listaPeriodos.Where(x => x.IdTipoNomina == 16).Select(x => x.IdPeriodoPago).ToArray();

                    //Consultamos la nosminas Asimilados
                    var listaA = (from n in listaNominas
                                  where arrayPeriodosAsimilados.Contains(n.IdPeriodo)
                                        && n.IdEmpresaAsimilado == idEmpresa
                                  select n).ToList();

                    //Unimos las dos listas
                    listaNominas = listaF;
                    listaNominas.AddRange(listaA);

                    #endregion

                    #region FINIQUITOS FILTRO
                    //Busca los finiquitos que se generaron con la empresa fiscal
                    listaFiniquitos = listaFiniquitos.Where(x => x.IdEmpresaFiscal == idEmpresa).ToList();

                    #endregion


                }

                //2do Filtro
                if (idCliente > 0)
                {
                    //De la lista anterior filtramos las nominas dond el cliente sea idCliente
                    listaNominas = listaNominas.Where(x => x.IdCliente == idCliente).ToList();

                    //Filtramos los finiquitos por IdCliente
                    listaFiniquitos = listaFiniquitos.Where(x => x.IdCliente == idCliente).ToList();
                }


                if (listaNominas.Count <= 0) return null;

                //Generamos los array de id's para
                //Buscar sus detalles de nomina
                //Datos del empleado
                //Detalles del finiquito
                var arrayIdNominas = listaNominas.Select(x => x.IdNomina).ToArray();
                var arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();
                var arrayIdFiniquitos = listaFiniquitos.Select(x => x.IdFiniquito).ToArray();

                //GET Detalle de finiquitos
                listaFiniquitosDetalles = (from fd in context.NOM_Finiquito_Detalle
                                           where arrayIdFiniquitos.Contains(fd.IdFiniquito)
                                           select fd).ToList();

                //GET PERMISOS E INCAPACIDADES
                var fecha2mesesMenos = dateInicial.Value.AddMonths(-2);

                listaPermisos = (from p in context.Permisos
                                 where arrayIdEmpleados.Contains(p.IdEmpleado)
                                       && p.FechaInicio >= fecha2mesesMenos && p.FechaInicio <= dateFinal.Value
                                 select p).ToList();


                listaIncapacidadeses = (from i in context.Incapacidades
                                        where arrayIdEmpleados.Contains(i.IdEmpleado)
                                              && i.FechaInicio >= fecha2mesesMenos && i.FechaInicio <= dateFinal.Value
                                        select i).ToList();


                listafaltas = (from i in context.Inasistencias
                               where arrayIdEmpleados.Contains(i.IdEmpleado)
                                     && i.Fecha >= fecha2mesesMenos && i.Fecha <= dateFinal.Value
                               select i).ToList();



                //GET - Lista de detalles de nominas del arrray de nominas
                listaDetalles = (from nd in context.NOM_Nomina_Detalle
                                 where arrayIdNominas.Contains(nd.IdNomina)
                                 select nd).ToList();

                //GET - lista de cuotas imss del array de nominas
                listaCuotas = (from cuotas in context.NOM_Cuotas_IMSS
                               where arrayIdNominas.Contains(cuotas.IdNomina)
                               select cuotas).ToList();


                //GET - lista de cuotas imss de los finiquitos
                //listaCuotasFiniquito = (from cuotas in context.NOM_Cuotas_IMSS
                //                        where arrayIdFiniquitos.Contains(cuotas.IdFiniquito)
                //                        select cuotas).ToList();

                listaDescuentoComisionFiniquito = (from dc in context.NOM_Finiquito_Descuento_Adicional
                                                   where arrayPeriodosAutorizados.Contains(dc.IdPeriodo)
                                                   select dc).ToList();

                //GET -  Datos del empleado
                listaEmpleados = (from emp in context.Empleado
                                  where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                  orderby emp.APaterno
                                  select emp).ToList();

                //Agrupa por IdConcepto
                var listaGroupConceptos = listaDetalles.GroupBy(x => x.IdConcepto).ToList();

                //Genera Array de Id de los conceptos
                var arrayIdConceptos = listaGroupConceptos.Select(x => x.Key).ToArray();

                //GET - Lista de Conceptos de las Nominas
                listaConceptos = (from conce in context.C_NOM_Conceptos
                                  where arrayIdConceptos.Contains(conce.IdConcepto)
                                  select conce).ToList();

                var listaGroupConceptosF = listaFiniquitosDetalles.GroupBy(x => x.IdConcepto).ToList();

                arrayIdConceptos = listaGroupConceptosF.Select(x => x.Key).ToArray();

                //Lista de Conceptos de los finiquitos
                listaConceptosFiniquito = (from conce in context.C_NOM_Conceptos
                                           where arrayIdConceptos.Contains(conce.IdConcepto)
                                           select conce).ToList();

                arrayIdEmpleados = listaFiniquitos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleadosFiniquitos = (from emp in context.Empleado
                                            where arrayIdEmpleados.Contains(emp.IdEmpleado)
                                            orderby emp.APaterno
                                            select emp).ToList();

                //GET - Lista empresas fiscales
                listaEmpresasF = (from em in context.Empresa
                                  select em).ToList();

                listaClientes = (from clie in context.Cliente
                                 select clie).ToList();



                //Buscamos los contratos que se usaron en las nominas del ejercicio fiscal
                var contratosNominas = listaNominas.Select(x => x.IdContrato).ToList();
                var contratosFiniquitos = listaFiniquitos.Select(x => x.IdFiniquito).ToList();

                contratosNominas.AddRange(contratosFiniquitos);

                var arrayIdContratos = contratosNominas.ToArray();

                listaContratos = (from con in context.Empleado_Contrato
                                  where arrayIdContratos.Contains(con.IdContrato)
                                  select con).ToList();


                //-------------------------------------intentar  recuperar uids de NOM_CFDI_Timbrado
                var listaUidsNomina = from s in listaNominas
                                      select s.IdNomina;

                var listaUidsFiniquito = from s in listaFiniquitos
                                         select s.IdFiniquito;

                registrosQueSonNomina = (from s in context.NOM_CFDI_Timbrado
                                         where listaUidsNomina.Contains(s.IdNomina) && s.Cancelado == false && s.IdFiniquito == 0 && s.ErrorTimbrado == false && String.IsNullOrEmpty(s.FolioFiscalUUID) == false
                                         select s).ToList();

                registrosQueSonFiniquito = (from s in context.NOM_CFDI_Timbrado
                                            where listaUidsFiniquito.Contains(s.IdFiniquito) && s.Cancelado == false && s.IdNomina == 0 && s.ErrorTimbrado == false && String.IsNullOrEmpty(s.FolioFiscalUUID) == false
                                            select s).ToList();

                #endregion
            }


            #region UNIR LISTAS NOMINAS Y FINIQUITOS
            //Unir la lista de conceptos de nomina con la lista de conceptos del finiquito
            listaConceptos.AddRange(listaConceptosFiniquito);
            listaConceptos = listaConceptos.Distinct().ToList();

            //Unir la lista de empleados de la nomina con la lista de empleado del finiquito
            listaEmpleados.AddRange(listaEmpleadosFiniquitos);
            listaEmpleados = listaEmpleados.Distinct().ToList();
            #endregion



            //Validamos las listas
            if (listaConceptos.Count <= 0) return null;

            Empresa itemEmpresa = null;

            //Guarda el archivo en la memoria
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();

            #region DOCUMENTO EXCEL - NOMINA ********************************************************

            var worksheet = workbook.Worksheets.Add("Acumulado");
            worksheet.SheetView.Freeze(2, 4);



            #region HEADERS

            #region HEADER EMPLEADOS

            worksheet.Cell(2, 1).Value = "Id";
            worksheet.Cell(2, 2).Value = "Clave";
            worksheet.Cell(2, 3).Value = "Paterno";
            worksheet.Cell(2, 4).Value = "Materno";
            worksheet.Cell(2, 5).Value = "Nombre";
            worksheet.Cell(2, 6).Value = "RFC";
            worksheet.Cell(2, 7).Value = "UUID";   // se agrego esta columna  
            worksheet.Cell(2, 8).Value = "CURP"; //originalmente este era columna 7

            #endregion

            #region HEADER NOMINA
            worksheet.Cell(2, 9).Value = "SD";
            worksheet.Cell(2, 10).Value = "SDI";
            worksheet.Cell(2, 11).Value = "Cliente";
            worksheet.Cell(2, 12).Value = "Empresa";
            worksheet.Cell(2, 13).Value = "Periodo";
            worksheet.Cell(2, 14).Value = "Fecha timbrado";  //se agrego esta columna
            worksheet.Cell(2, 15).Value = "Bimestre";
            worksheet.Cell(2, 16).Value = "Dias Trabajados";
            worksheet.Cell(2, 17).Value = "Total Nomina";
            worksheet.Cell(2, 18).Value = "Total Finiquito";
            worksheet.Cell(2, 19).Value = "Total Liq";
            worksheet.Cell(2, 20).Value = "Total Neto";
            worksheet.Cell(2, 21).Value = "Descuento Adicional Fin";
            worksheet.Cell(2, 22).Value = "Comisiones Fin";
            worksheet.Cell(2, 23).Value = "Permisos";
            worksheet.Cell(2, 24).Value = "Incapacidades";
            worksheet.Cell(2, 25).Value = "Faltas";
            #endregion

            #region HEADER CONCEPTOS

            int columnaH = 26; //inicia en el 18    //---aqui decia 24
            //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO

            var listaConceptosPercecpciones = listaConceptos.Where(x => x.TipoConcepto == 1).ToList();
            var listaConceptosDeducciones = listaConceptos.Where(x => x.TipoConcepto == 2).ToList();

            int rangeInicial = columnaH;
            #region PERCEPCIONES
            foreach (var itemConcepto in listaConceptosPercecpciones)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheet.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    worksheet.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                }

                //MERGE CELL
                worksheet.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            #endregion

            //TOTAL DE PERCEPCIONES
            worksheet.Cell(2, columnaH).Value = "Total Percepciones";
            columnaH++;

            //Establece un estilo al header
            worksheet.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGray);


            rangeInicial = columnaH;
            #region DEDUCCIONES
            foreach (var itemConcepto in listaConceptosDeducciones)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheet.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheet.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    worksheet.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheet.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                }

                //MERGE CELL
                worksheet.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }


            worksheet.Cell(1, columnaH).Value = "Subsidio Causado";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Subsidio Entregado";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Isr antes Subsidio";
            columnaH++;


            //TOTAL DE PERCEPCIONES
            worksheet.Cell(1, columnaH).Value = "Total Deducciones";
            columnaH++;
            worksheet.Cell(1, columnaH).Value = "Total ISN";
            columnaH++;
            #endregion

            //Establece un estilo al header
            worksheet.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);


            #endregion

            #region HEADER IMSS

            int mergeI = columnaH;
            int colorI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cuotas IMSS";
            worksheet.Cell(2, columnaH).Value = "Total Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Total Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cuota Fija";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Excedente";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Prestaciones";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Pensionados";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Invalidez y Vida";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Guarderia";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Seguro Retiro";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Cesantía y Vejez";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Infonavit";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheet.Cell(1, columnaH).Value = "Riesgo Trabajo";
            worksheet.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Obrero";
            worksheet.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //Establece un estilo al header
            worksheet.Range(1, colorI, 2, columnaH).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGreen);
            columnaH++;

            #endregion


            worksheet.Cell(2, columnaH).Value = "Complemento";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Ejercicio Completo";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Base Gravable Anual";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Limite Inferior";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Excedente";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Tasa";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Impuesto Marginal";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;
            worksheet.Cell(2, columnaH).Value = "Cuota Fija";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "ISR Retener Anual";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);
            columnaH++;

            worksheet.Cell(2, columnaH).Value = "Saldo Favor Anual";
            worksheet.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);



            worksheet.Cell(2, columnaH).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //MERGE CELL
            //  worksheet.Range(1, columnaH, 1, columnaH + 1).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            #endregion

            //Variables
            int fila = 3;
            int filaInicial = 0;
            int filaFinal = 0;
            bool rowColor = false;
            int idEmp = 0;
            int arraySize = 7 + 22 + 1;

            var periodosIdx = listaPeriodos.Count * 3;
            arraySize += periodosIdx;

            double[] totales = new double[arraySize];
            #region CONTENIDO

            //Por Empleado
            //Buscamos sus nominas y sus detalles de cada periodo
            foreach (var itemEmpleado in listaEmpleados)
            {
                //   var columna = 0;
                //Cuantas nominas tiene el empleado
                var nominasDelEmpleado = listaNominas.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();
                bool ejercicioCompleto = false;

                if (idEmp != itemEmpleado.IdEmpleado)
                {
                    idEmp = itemEmpleado.IdEmpleado;
                    rowColor = !rowColor;
                    rowColor = true;
                    filaInicial = fila;
                }

                //Por cada Nomina
                #region CONTENIDO NOMINA
                foreach (var itemNomina in nominasDelEmpleado)
                {
                    #region FORMA anterior
                    //#region DATOS DEL EMPLEADO
                    //worksheet.Cell(fila, 1).Value = itemNomina.IdNomina;
                    //worksheet.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                    //worksheet.Cell(fila, 3).Value = itemEmpleado.APaterno;
                    //worksheet.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                    //worksheet.Cell(fila, 5).Value = itemEmpleado.Nombres;
                    //worksheet.Cell(fila, 6).Value = itemEmpleado.RFC;
                    //worksheet.Cell(fila, 7).Value = itemEmpleado.CURP;
                    //#endregion

                    //#region DATOS NOMINA

                    //worksheet.Cell(fila, 8).Value = itemNomina.SD;
                    //worksheet.Cell(fila, 9).Value = itemNomina.SDI;
                    ////worksheet.Cell(fila, 10).Value = itemNomina.SBC;

                    ////Empresa - Periodo - Cliente
                    //var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemNomina.IdPeriodo);

                    //itemEmpresa = null;

                    //if (itemPeriodo.IdTipoNomina <= 15) //normal
                    //{
                    //    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaFiscal);
                    //}
                    //else if (itemPeriodo.IdTipoNomina == 16)
                    //{
                    //    itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemNomina.IdEmpresaAsimilado);
                    //}


                    //var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemNomina.IdCliente);

                    //// worksheet.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                    //worksheet.Cell(fila, 10).Value = itemCliente != null ? itemCliente.Nombre : "-";
                    //worksheet.Cell(fila, 11).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                    //worksheet.Cell(fila, 12).Value = itemPeriodo?.Bimestre ?? 0;

                    //if (incluirNoAutorizados)
                    //{
                    //    if (itemPeriodo.Autorizado == false)
                    //    {
                    //        worksheet.Cell(fila, 12).Style
                    //            .Font.SetFontSize(13)
                    //            .Font.SetBold(true)
                    //            //.Font.SetFontColor(XLColor.White)
                    //            .Fill.SetBackgroundColor(XLColor.Gray);
                    //    }
                    //}

                    //worksheet.Cell(fila, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    //worksheet.Cell(fila, 13).Value = itemNomina.Dias_Laborados;
                    //worksheet.Cell(fila, 14).Value = itemNomina.TotalNomina;

                    //#region CONCEPTOS

                    //int columna = 15;

                    //#region PERCEPCIONES
                    ////Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
                    //foreach (var itemConcepto in listaConceptosPercecpciones)
                    //{
                    //    decimal total = 0;
                    //    decimal gravado = 0;
                    //    decimal exento = 0;
                    //    //Busca en los detalles de la nomina por concepto y por id de nomina
                    //    var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);

                    //    if (itemDetalle != null)
                    //    {
                    //        total = itemDetalle.Total;
                    //        gravado = itemDetalle.GravadoISR;
                    //        exento = itemDetalle.ExentoISR;
                    //    }
                    //    //  43 - ISR
                    //    //  144 - Subsidio
                    //    //  42 - SS
                    //    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = gravado;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = exento;
                    //        columna++;
                    //        //worksheet.Cell(fila, columna).Value = isn;
                    //        //columna++;
                    //    }
                    //}


                    //#endregion

                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalPercepciones;
                    //columna++;

                    //#region DEDUCCIONES
                    ////Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO
                    //foreach (var itemConcepto in listaConceptosDeducciones)
                    //{
                    //    decimal total = 0;
                    //    decimal gravado = 0;
                    //    decimal exento = 0;
                    //    //Busca en los detalles de la nomina por concepto y por id de nomina
                    //    var itemDetalle = listaDetalles.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == itemConcepto.IdConcepto);

                    //    if (itemDetalle != null)
                    //    {
                    //        total = itemDetalle.Total;
                    //        gravado = itemDetalle.GravadoISR;
                    //        exento = itemDetalle.ExentoISR;
                    //    }
                    //    //  43 - ISR
                    //    //  144 - Subsidio
                    //    //  42 - SS
                    //    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cell(fila, columna).Value = total;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = gravado;
                    //        columna++;
                    //        worksheet.Cell(fila, columna).Value = exento;
                    //        columna++;
                    //        //worksheet.Cell(fila, columna).Value = isn;
                    //        //columna++;
                    //    }
                    //}
                    //#endregion


                    //#endregion

                    //worksheet.Cell(fila, columna).Value = itemNomina.SubsidioCausado;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.SubsidioEntregado;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.ISRantesSubsidio;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalDeducciones;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalImpuestoSobreNomina;
                    //columna++;
                    //#endregion

                    //#region CUOTAS IMSS

                    //var itemCuotaImss = listaCuotas.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina);
                    //decimal totalPatron = 0;
                    //decimal totalObrero = 0;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalPatron ?? 0;
                    //columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.TotalObrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Cuota_Fija_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.GuarderiasPrestaciones_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.SeguroRetiro_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Obrero ?? 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.Infonavit_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;

                    //worksheet.Cell(fila, columna).Value = itemCuotaImss?.RiesgoTrabajo_Patron ?? 0; columna++;
                    //worksheet.Cell(fila, columna).Value = 0; columna++;
                    //#endregion


                    //worksheet.Cell(fila, columna).Value = itemNomina.TotalComplemento;
                    //worksheet.Cell(fila, columna).Style.NumberFormat.Format = "$ ###,##0.00";

                    #endregion

                    ejercicioCompleto = ContenidoAcumulado(ref worksheet, itemNomina, null, itemEmpleado, listaPeriodos, listaEmpresasF, listaClientes, listaCuotas, listaDetalles, null, listaConceptosPercecpciones, listaConceptosDeducciones, incluirNoAutorizados, false, fila, null, listaContratos, ejercicioFiscal, listaPermisos, listaIncapacidadeses, listafaltas, registrosQueSonNomina, registrosQueSonFiniquito);

                    //Sumar los totales de Gravados Exentos - ISN

                    fila++;
                    //rowColor = !rowColor;
                }//Fin del For de la nominas del empleado
                #endregion


                #region CONTENIDO FINIQUITO

                //Obtener los finiquitos del empleado  
                var finiquitosEmpleado = listaFiniquitos.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();

                foreach (var itemFiniquito in finiquitosEmpleado)
                {
                    ejercicioCompleto = ContenidoAcumulado(ref worksheet, null, itemFiniquito, itemEmpleado, listaPeriodos, listaEmpresasF, listaClientes, listaCuotas, null, listaFiniquitosDetalles, listaConceptosPercecpciones, listaConceptosDeducciones, incluirNoAutorizados, true, fila, listaDescuentoComisionFiniquito, listaContratos, ejercicioFiscal, listaPermisos, listaIncapacidadeses, listafaltas, registrosQueSonNomina, registrosQueSonFiniquito);

                    //set color al row de finiquito
                    worksheet.Range(fila, 1, fila, columnaH).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                    fila++;
                }


                #endregion


                if (rowColor)
                {
                    rowColor = false;
                    worksheet.Range(fila, 1, fila, columnaH).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(fila, 1, fila, columnaH).Style.Border.TopBorderColor = XLColor.SlateGray;
                    //worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Lavender;
                    //worksheet.Range(fila, 1, fila, columna).Style.Fill.SetBackgroundColor(XLColor.Lavender);

                    //Agregamos la sumatoria
                    if (nominasDelEmpleado.Count > 0)
                    {
                        SumarColumnas(ref worksheet, filaInicial, fila - 1, columnaH - 8, ejercicioCompleto, listaPeriodos[0].IdEjercicio, nominasDelEmpleado[0].IdEmpleado, nominasDelEmpleado[nominasDelEmpleado.Count - 1].SD, nominasDelEmpleado[nominasDelEmpleado.Count - 1].SDI, nominasDelEmpleado[0].IdEmpresaFiscal, calculoanual, UMA);
                    }
                    fila++;
                    fila++;
                }

            }//Fin For por empleado

            #endregion

            #region  COLORES DISEÑO DE CELDAS

            //Establece un estilo al header
            worksheet.Range("G2:Y2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.Raspberry);

            //Establece un estilo al header
            worksheet.Range("A2:F2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue);


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
            worksheet.Column(15).AdjustToContents();
            worksheet.Column(16).AdjustToContents();
            worksheet.Column(17).AdjustToContents();
            worksheet.Column(18).AdjustToContents();
            worksheet.Column(19).AdjustToContents();
            worksheet.Column(20).AdjustToContents();
            worksheet.Column(21).AdjustToContents();

            #endregion

            #endregion

            #region DOCUMENTO EXCEL - FINIQUITO *****************************************************
            /*
            var worksheetF = workbook.Worksheets.Add("Finiquitos");
            worksheetF.SheetView.Freeze(2, 4);

            #region HEADERS

            #region EMPLEADO 

            worksheetF.Cell(2, 1).Value = "IdFiniquito";
            worksheetF.Cell(2, 2).Value = "Clave";
            worksheetF.Cell(2, 3).Value = "Paterno";
            worksheetF.Cell(2, 4).Value = "Materno";
            worksheetF.Cell(2, 5).Value = "Nombre";
            worksheetF.Cell(2, 6).Value = "RFC";
            worksheetF.Cell(2, 7).Value = "CURP";

            #endregion

            #region DATOS

            worksheetF.Cell(2, 8).Value = "SD";
            worksheetF.Cell(2, 9).Value = "SDI";

            worksheetF.Cell(2, 10).Value = "Cliente";
            worksheetF.Cell(2, 11).Value = "Periodo";
            worksheetF.Cell(2, 12).Value = "Bimestre";
            worksheetF.Cell(2, 13).Value = "Total Finiquito";
            worksheetF.Cell(2, 14).Value = "Total Liq";
            worksheetF.Cell(2, 15).Value = "Total Neto";

            //Establece un estilo al header
            worksheetF.Range("A2:O2").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue); //AirForceBlue

            #endregion

            #region CONCEPTOS

            columnaH = 16; //inicia en el 18
                           //Por cada concepto busca su detalle-- GUARDAR EL TOTAL DE GRAVADO Y EXENTO

            rangeInicial = columnaH;
            foreach (var itemConcepto in listaConceptosFiniquito)
            {
                int colInicial = columnaH;
                int colFinal = 0;
                worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;

                //  43 - ISR
                //  144 - Subsidio
                //  42 - SS
                if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                {
                    worksheetF.Cell(2, columnaH).Value = " ";
                    colFinal = columnaH;
                    columnaH++;
                }
                else
                {
                    // worksheetF.Cell(1, columnaH).Value = itemConcepto.DescripcionCorta;
                    worksheetF.Cell(2, columnaH).Value = "Total ";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Gravado";
                    columnaH++;
                    worksheetF.Cell(2, columnaH).Value = "Exento";
                    colFinal = columnaH;
                    columnaH++;
                    //worksheetF.Cell(2, columnaH).Value = "ISN";
                    //colFinal = columnaH;
                    //columnaH++;
                }


                //MERGE CELL
                worksheetF.Range(1, colInicial, 1, colFinal).Row(1).Merge().Style
                    .Font.SetBold(true)
                    .Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            //Establece un estilo al header
            worksheetF.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGray);

            #endregion

            #region HEADER DESCUENTOS Y COMISIONES
            //TOTAL ISN
            rangeInicial = columnaH;
            worksheetF.Cell(2, columnaH).Value = "TOTAL ISN";
            columnaH++;
            //DESCUENTOS
            worksheetF.Cell(2, columnaH).Value = "DESCUENTO ADICIONAL";
            columnaH++;
            //COMISIONES
            worksheetF.Cell(2, columnaH).Value = "COMISIONES";
            columnaH++;


            //Establece un estilo al header
            worksheetF.Range(1, rangeInicial, 2, columnaH - 1).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.Amber);
            #endregion

            #region HEADER IMSS

            mergeI = columnaH;
            colorI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cuotas IMSS";
            worksheetF.Cell(2, columnaH).Value = "Total Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Total Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cuota Fija";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Excedente";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Prestaciones";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Pensionados";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Invalidez y Vida";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Guarderia";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Seguro Retiro";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Cesantía y Vejez";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Infonavit";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            columnaH++;

            mergeI = columnaH;
            worksheetF.Cell(1, columnaH).Value = "Riesgo Trabajo";
            worksheetF.Cell(2, columnaH).Value = "Patron";
            columnaH++;
            worksheetF.Cell(2, columnaH).Value = "Obrero";
            worksheetF.Range(1, mergeI, 1, columnaH).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            //Establece un estilo al header
            worksheetF.Range(1, colorI, 2, columnaH).Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Font.SetFontColor(XLColor.Black)
                .Fill.SetBackgroundColor(XLColor.BlueGreen);
            columnaH++;


            //MERGE CELL
            //  worksheet.Range(1, columnaH, 1, columnaH + 1).Row(1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheetF.Cell(2, columnaH).Value = "Complemento";
            worksheetF.Cell(2, columnaH).Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetFontColor(XLColor.Black)
                 .Fill.SetBackgroundColor(XLColor.DarkSalmon);

            #endregion

            #endregion

            #region CONTENIDO

            #region CONCEPTOS
            fila = 3;
            foreach (var itemFiniquito in listaFiniquitos)
            {
                //GET dato empleado
                var itemEmpleado = listaEmpleadosFiniquitos.FirstOrDefault(x => x.IdEmpleado == itemFiniquito.IdEmpleado);

                if (itemEmpleado == null) continue;

                worksheetF.Cell(fila, 1).Value = itemFiniquito.IdFiniquito;
                worksheetF.Cell(fila, 2).Value = itemEmpleado.IdEmpleado;
                worksheetF.Cell(fila, 3).Value = itemEmpleado.APaterno;
                worksheetF.Cell(fila, 4).Value = itemEmpleado.AMaterno;
                worksheetF.Cell(fila, 5).Value = itemEmpleado.Nombres;
                worksheetF.Cell(fila, 6).Value = itemEmpleado.RFC;
                worksheetF.Cell(fila, 7).Value = itemEmpleado.CURP;

                worksheetF.Cell(fila, 8).Value = itemFiniquito.SD;
                worksheetF.Cell(fila, 9).Value = itemFiniquito.SDI;

                //Empresa - Periodo
                var itemPeriodo = listaPeriodos.FirstOrDefault(x => x.IdPeriodoPago == itemFiniquito.IdPeriodo);
                itemEmpresa = listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaFiscal) ??
                                 listaEmpresasF.FirstOrDefault(x => x.IdEmpresa == itemFiniquito.IdEmpresaAsimilado);

                var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemFiniquito.IdCliente);

                // worksheetF.Cell(fila, 11).Value = itemEmpresa != null ? itemEmpresa.RazonSocial : "-";
                worksheetF.Cell(fila, 10).Value = itemCliente != null ? itemCliente.Nombre : "-";
                worksheetF.Cell(fila, 11).Value = itemPeriodo?.Fecha_Fin.ToShortDateString() ?? "null";
                worksheetF.Cell(fila, 12).Value = itemPeriodo.Bimestre;
                worksheetF.Cell(fila, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheetF.Cell(fila, 13).Value = itemFiniquito.Total_Finiquito;
                worksheetF.Cell(fila, 14).Value = itemFiniquito.Total_Liquidacion;
                worksheetF.Cell(fila, 15).Value = itemFiniquito.TOTAL_total;

                int columna = 16; //
                decimal totalIsn = 0;
                foreach (var itemConcepto in listaConceptosFiniquito)
                {
                    decimal total = 0;
                    decimal gravado = 0;
                    decimal exento = 0;
                    decimal isn = 0;

                    var itemDetalleF =
                        listaFiniquitosDetalles.FirstOrDefault(
                            x => x.IdFiniquito == itemFiniquito.IdFiniquito && x.IdConcepto == itemConcepto.IdConcepto);

                    if (itemDetalleF != null)
                    {
                        total = itemDetalleF.Total;
                        gravado = itemDetalleF.GravadoISR;
                        exento = itemDetalleF.ExentoISR;
                        isn = itemDetalleF.ImpuestoSobreNomina;
                        totalIsn += isn;
                    }


                    if (itemConcepto.IdConcepto == 43 || itemConcepto.IdConcepto == 144 || itemConcepto.IdConcepto == 42)
                    {
                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                    }
                    else
                    {
                        worksheetF.Cell(fila, columna).Value = total;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = gravado;
                        columna++;
                        worksheetF.Cell(fila, columna).Value = exento;
                        columna++;
                        //worksheetF.Cell(fila, columna).Value = isn;
                        //columna++;
                    }
                }



                #region  DESCUENTOS Y COMISIONES

                var descuentosAdicionales = listaDescuentoComisionFiniquito.Where(x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 2)
                       .Select(x => x.TotalDescuento)
                       .Sum();

                var comisiones = listaDescuentoComisionFiniquito.Where(x => x.IdPeriodo == itemFiniquito.IdPeriodo && x.IdConcepto == 1)
                        .Select(x => x.TotalDescuento)
                        .Sum();
                //TOTAL ISN
                worksheetF.Cell(fila, columna).Value = totalIsn;
                columna++;
                //DESCUENTOS
                worksheetF.Cell(fila, columna).Value = descuentosAdicionales;
                columna++;
                //COMISIONES
                worksheetF.Cell(fila, columna).Value = comisiones;
                columna++;
                #endregion


                #region CUOTAS IMSS

                var itemCuotaImss = listaCuotasFiniquito.FirstOrDefault(x => x.IdFiniquito == itemFiniquito.IdFiniquito);
                decimal totalPatron = 0;
                decimal totalObrero = 0;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.TotalPatron ?? 0;
                columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.TotalObrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Cuota_Fija_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Excedente_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.PrestacionesDinero_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Pensionados_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.InvalidezVida_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.GuarderiasPrestaciones_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.SeguroRetiro_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.CesantiaVejez_Obrero ?? 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.Infonavit_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;

                worksheetF.Cell(fila, columna).Value = itemCuotaImss?.RiesgoTrabajo_Patron ?? 0; columna++;
                worksheetF.Cell(fila, columna).Value = 0; columna++;
                #endregion

                worksheetF.Cell(fila, columna).Value = itemFiniquito.TotalComplemento;

                // if (rowColor)
                //    {
                //        // worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Lavender;
                //        worksheetF.Range(fila, 1, fila, columna).Style.Fill.SetBackgroundColor(XLColor.Lavender);
                //    }

                //rowColor = !rowColor;
                fila++;
            }

            #endregion

            #endregion

            #region AJUSTE DE CELDAS

            worksheetF.Column(1).AdjustToContents(1);
            worksheetF.Column(2).AdjustToContents(2);
            worksheetF.Column(3).AdjustToContents(3);
            worksheetF.Column(4).AdjustToContents(4);
            worksheetF.Column(5).AdjustToContents(5);
            worksheetF.Column(6).AdjustToContents(6);
            worksheetF.Column(7).AdjustToContents(7);
            worksheetF.Column(8).AdjustToContents(8);
            worksheetF.Column(9).AdjustToContents(9);
            worksheetF.Column(10).AdjustToContents(10);
            worksheetF.Column(11).AdjustToContents(11);
            worksheetF.Column(12).AdjustToContents(12);
            worksheetF.Column(13).AdjustToContents(13);
            worksheetF.Column(14).AdjustToContents(14);
            worksheetF.Column(15).AdjustToContents(15);
            worksheetF.Column(16).AdjustToContents(16);
            worksheetF.Column(17).AdjustToContents(17);
            worksheetF.Column(18).AdjustToContents(18);
            worksheetF.Column(19).AdjustToContents(19);
            worksheetF.Column(20).AdjustToContents(20);
            worksheetF.Column(21).AdjustToContents(21);


            #endregion
            */
            //fin hoja de finiquito
            #endregion

            // AddColsUuidFechaCertificacion( worksheet, ctx.NOM_CFDI_Timbrado.ToList<NOM_CFDI_Timbrado>() );    //mandar a agregar columna  UUID y fecha de certificacion


            #region CREA EL ARCHIVO Y RETORNA EL PATH RELATIVO PARA LA DESCARGA



            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "Acumulado_.xlsx";

            if (itemEmpresa != null)
            {
                nombreArchivo = "ACUM_" + itemEmpresa.RazonSocial + "_.xlsx";
            }

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);
            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

            //Guarda el documento en la memori
            //workbook.SaveAs(ms, false);
            //var archivoByte =  ms.ToArray();
            //return archivoByte;
            #endregion
        }





    }

}
