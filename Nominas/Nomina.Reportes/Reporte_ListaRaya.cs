
using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RH.Entidades;
using System.IO;
using Nomina.Reportes.Datos;
using Common.Utils;


namespace Nomina.Reportes
{
    public class Reporte_ListaRaya
    {
        RHEntities ctx = null;
        public Reporte_ListaRaya()
        {
            ctx = new RHEntities();
        }

        public string ListaDeRayaPorEmpresa(NOM_PeriodosPago pperiodo, bool complemento, string ruta, int idusuario, List<EmpInci> _inci,int idEmpresa)
        {

            ReportesDAO rep = new ReportesDAO();
            var idempleados = new List<int>();
            var listraya = new ListaDeRaya();
            var emp = ctx.Empresa.Where(x => x.IdEmpresa == idEmpresa).FirstOrDefault();
            var Nominas = ctx.NOM_Nomina.ToList();
            var Finiquitos = ctx.NOM_Finiquito.ToList();
            //var DetalleNomina = ctx.NOM_Nomina_Detalle;
            if(pperiodo.IdTipoNomina == 16)
            {
                 idempleados = Nominas.Where(x => x.IdPeriodo == pperiodo.IdPeriodoPago && x.IdEmpresaAsimilado == emp.IdEmpresa).Select(x => x.IdEmpleado).ToList();
            }
            else if(pperiodo.IdTipoNomina == 11)
            {
                idempleados = Finiquitos.Where(x => x.IdPeriodo == pperiodo.IdPeriodoPago && x.IdEmpresaFiscal == emp.IdEmpresa).Select(x => x.IdEmpleado).ToList();
            }
            else 
            {
                idempleados = Nominas.Where(x => x.IdPeriodo == pperiodo.IdPeriodoPago && x.IdEmpresaFiscal == emp.IdEmpresa).Select(x => x.IdEmpleado).ToList();
            }
            
            var listEstados = ctx.C_Estado.ToList();
            var estado = listEstados.Where(x => x.IdEstado == emp.IdEstado).FirstOrDefault();
            
            //create a document object
            var doc = new Document();
            //get the current directory


            var newRuta = Utils.ValidarFolderUsuario(idusuario, ruta);
            newRuta = newRuta + "Lista de Raya del periodo " + pperiodo.Descripcion + ".pdf";
            //get PdfWriter object
            PdfWriter.GetInstance(doc, new FileStream(newRuta, FileMode.Create));
            //open the document for writing
            doc.Open();
              int contador = 0;
                int contadorEmpresas = 0;
            EncabezadoPDF(pperiodo, emp, ref doc,estado);
            foreach (var idemp in idempleados)
            {
                var DatosEmpleado = listraya.ListadoEmpleados(idemp);
                foreach (var inci in _inci)
                {
                    int idiniciemp = inci.IdEmpleado;

                    if (idemp == inci.IdEmpleado)
                    {
                        int descansos = inci.Descansos;
                        int asistencias = inci.Asistencias;
                        int vacaciones = inci._Vacaciones;
                        int incapacidadR = inci.IncapacidadesR;
                        int incapacidadE = inci.IncapacidadesE;
                        int incapacidadM = inci.IncapacidadesM;
                        int faltasI = inci.FaltasI;
                        int faltasA = inci.FaltasA;
                        DetallePDF(idemp, ref doc, emp.RazonSocial, pperiodo, DatosEmpleado, complemento, descansos, asistencias, vacaciones, incapacidadR, incapacidadE, incapacidadM, faltasI, faltasA, Nominas,Finiquitos);
                    }
                 
                }
                contador = contador + 1;
                
                if (complemento == false)
                {
                    if (contador == 4 )
                    {
                        doc.NewPage();
                        EncabezadoPDF(pperiodo, emp, ref doc, estado);
                        

                        contador = 0;


                    }
                }
                
            }
            if(contador == 0)
            {
                TotalesAutorizacion(ref doc, pperiodo, complemento, emp.IdEmpresa);
            }else
            {
                doc.NewPage();

                TotalesAutorizacion(ref doc, pperiodo, complemento, emp.IdEmpresa);
            }
            
            //  System.Diagnostics.Process.Start(newRuta + "/Autorizacion.pdf");
            doc.Close();



            return newRuta;
        }

        public void EncabezadoPDF(NOM_PeriodosPago pperiodo,Empresa emp, ref Document doc ,C_Estado estado)
        {
            Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
            Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


            PdfPTable TablaEncabezado = new PdfPTable(1);
            TablaEncabezado.WidthPercentage = 100;
            PdfPTable TablaEncabezado2 = new PdfPTable(2);
            TablaEncabezado2.WidthPercentage = 100;


            PdfPCell empresa = new PdfPCell(new Paragraph(emp.RazonSocial));

            empresa.VerticalAlignment = Element.ALIGN_CENTER;
            empresa.BorderColor = iTextSharp.text.BaseColor.WHITE;
            empresa.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell PeridoPago = new PdfPCell(new Paragraph("Lista de raya del " + pperiodo.Fecha_Inicio.ToString("dd-MM-yyyy") + " al " + pperiodo.Fecha_Fin.ToString("dd-MM-yyyy"), _standardFont));

            PeridoPago.VerticalAlignment = Element.ALIGN_CENTER;
            PeridoPago.BorderColor = iTextSharp.text.BaseColor.WHITE;
            PeridoPago.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell DireccionEmpresa = new PdfPCell(new Paragraph(emp.Colonia + " " + emp.Calle + " " + emp.NoExt + " " + estado + " " + emp.Municipio + " " + emp.CP, _standardFont));
            DireccionEmpresa.VerticalAlignment = Element.ALIGN_CENTER;
            DireccionEmpresa.BorderColor = iTextSharp.text.BaseColor.WHITE;
            DireccionEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;


            PdfPCell RP = new PdfPCell(new Paragraph("Reg. Patronal : " + emp.RegistroPatronal, _standardFont));

            RP.VerticalAlignment = Element.ALIGN_CENTER;
            RP.BorderColor = iTextSharp.text.BaseColor.WHITE;
            RP.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell RFC = new PdfPCell(new Paragraph("RFC : " + emp.RFC, _standardFont));

            RFC.VerticalAlignment = Element.ALIGN_CENTER;
            RFC.BorderColor = iTextSharp.text.BaseColor.WHITE;
            RFC.HorizontalAlignment = Element.ALIGN_CENTER;


            TablaEncabezado.AddCell(empresa);
            TablaEncabezado.AddCell(PeridoPago);
            TablaEncabezado.AddCell(DireccionEmpresa);
            TablaEncabezado2.AddCell(RP);
            TablaEncabezado2.AddCell(RFC);


            doc.Add(TablaEncabezado);
            doc.Add(TablaEncabezado2);
            doc.Add(new Paragraph(" "));
        }
        public void DetallePDF(int id, ref Document doc, string NombreEmpresa, NOM_PeriodosPago pperiodo, EmpleadoDatosNominaViewModel DatosEmpleado, bool complemento, int descansos, int asistencias, int vacaciones, int incapacidadR, int incapacidadE, int incapacidadM, int faltasI, int faltasA,List<NOM_Nomina> Nominas,List<NOM_Finiquito> Finiquitos )
        {
            ReportesDAO reportes = new ReportesDAO();

            Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
            Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);
            var DetalleNomina = new List<NominaDetalle>();
            var DetalleNominaDeduccion = new List<NominaDetalle>();
            var nominas = new NOM_Nomina();
            var finiquito = new NOM_Finiquito();
            if(pperiodo.IdTipoNomina == 11)
            {
                 finiquito = Finiquitos.Where(x => x.IdEmpleado == DatosEmpleado.IdEmpleado && x.IdPeriodo == pperiodo.IdPeriodoPago).FirstOrDefault();
                 DetalleNomina = reportes.EmpleadoFinDetallePercepcion(finiquito.IdFiniquito,pperiodo.IdPeriodoPago);
                 DetalleNominaDeduccion = reportes.EmpleadoFinDetalleDeduccion(finiquito.IdFiniquito, pperiodo.IdPeriodoPago);
            }
            else
            {
                nominas = Nominas.Where(x => x.IdEmpleado == DatosEmpleado.IdEmpleado && x.IdPeriodo == pperiodo.IdPeriodoPago).FirstOrDefault();
                DetalleNomina = reportes.EmpleadoNomDetallePercepcion(nominas.IdNomina, complemento);
                DetalleNominaDeduccion = reportes.EmpleadoNomDetalleDeduccion(nominas.IdNomina);
            }
        


            PdfPTable TablaEmpleado = new PdfPTable(4);
            TablaEmpleado.WidthPercentage = 100;
            TablaEmpleado.HorizontalAlignment = Element.ALIGN_LEFT;
            PdfPTable TablaEmpleadoFinal = new PdfPTable(1);
            TablaEmpleadoFinal.WidthPercentage = 100;
            TablaEmpleadoFinal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablaPercepciones = new PdfPTable(2);
            TablaPercepciones.WidthPercentage = 100;
            TablaPercepciones.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablaPercepcionesFinal = new PdfPTable(2);
            TablaPercepcionesFinal.WidthPercentage = 100;
            TablaPercepcionesFinal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablaDeduccion = new PdfPTable(2);
            TablaDeduccion.WidthPercentage = 100;
            TablaDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablaAsistencia = new PdfPTable(9);
            TablaAsistencia.WidthPercentage = 100;
            TablaAsistencia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablaAsistenciaFinal = new PdfPTable(1);
            TablaAsistenciaFinal.WidthPercentage = 100;
            TablaAsistenciaFinal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablaTotalesPercepcion = new PdfPTable(2);
            TablaTotalesPercepcion.WidthPercentage = 100;
            TablaTotalesPercepcion.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablasTotalesDeduccion = new PdfPTable(2);
            TablasTotalesDeduccion.WidthPercentage = 100;
            TablasTotalesDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPTable TablaTotalesFinales = new PdfPTable(2);
            TablaTotalesFinales.WidthPercentage = 100;
            TablaTotalesFinales.HorizontalAlignment = Element.ALIGN_LEFT;



            PdfPCell Clave = new PdfPCell(new Paragraph("Clave: " + Convert.ToString(DatosEmpleado.IdEmpleado), _standardFont));

            Clave.VerticalAlignment = Element.ALIGN_CENTER;
            Clave.BorderColor = BaseColor.WHITE;
            //Clave.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;

            Clave.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell NOmbres = new PdfPCell(new Paragraph(DatosEmpleado.Paterno + " " + DatosEmpleado.Materno + " " + DatosEmpleado.Nombres, _standardFont));

            NOmbres.VerticalAlignment = Element.ALIGN_CENTER;
            NOmbres.BorderColor = iTextSharp.text.BaseColor.WHITE;
            NOmbres.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell RFCDato = new PdfPCell(new Paragraph("R.F.C.: " + DatosEmpleado.RFC, _standardFont));

            RFCDato.VerticalAlignment = Element.ALIGN_CENTER;
            RFCDato.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //RFCDato.Border = Rectangle.TOP_BORDER;
            RFCDato.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell IMSS = new PdfPCell(new Paragraph("No. I.M.S.S: " + DatosEmpleado.NSS, _standardFont));

            IMSS.VerticalAlignment = Element.ALIGN_CENTER;
            IMSS.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //IMSS.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
            IMSS.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell FechaIngreso = new PdfPCell(new Paragraph("Fecha Ingreso: " + DatosEmpleado.FechaAlta, _standardFont));

            FechaIngreso.VerticalAlignment = Element.ALIGN_CENTER;
            FechaIngreso.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //FechaIngreso.Border = Rectangle.LEFT_BORDER;
            FechaIngreso.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell CURP = new PdfPCell(new Paragraph("Curp: " + DatosEmpleado.CURP, _standardFont));

            CURP.VerticalAlignment = Element.ALIGN_CENTER;
            CURP.BorderColor = iTextSharp.text.BaseColor.WHITE;
            CURP.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Puesto = new PdfPCell(new Paragraph("Puesto: " + DatosEmpleado.Puesto, _standardFont));

            Puesto.VerticalAlignment = Element.ALIGN_CENTER;
            Puesto.BorderColor = iTextSharp.text.BaseColor.WHITE;
            Puesto.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Departamento = new PdfPCell(new Paragraph("Dpto: " + DatosEmpleado.Departamento, _standardFont));
            Departamento.VerticalAlignment = Element.ALIGN_CENTER;
            Departamento.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //Departamento.Border = Rectangle.RIGHT_BORDER;
            Departamento.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell TipoSalario = new PdfPCell(new Paragraph("Tipo Salario: " + DatosEmpleado.TipoSalario, _standardFont));
            TipoSalario.VerticalAlignment = Element.ALIGN_CENTER;
            TipoSalario.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //TipoSalario.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
            TipoSalario.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell SD = new PdfPCell(new Paragraph("SD: " + DatosEmpleado.SD, _standardFont));
            SD.VerticalAlignment = Element.ALIGN_CENTER;
            SD.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //SD.Border = Rectangle.BOTTOM_BORDER;
            SD.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell SDI = new PdfPCell(new Paragraph("SDI: " + DatosEmpleado.SDI, _standardFont));
            SDI.VerticalAlignment = Element.ALIGN_CENTER;
            SDI.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //SDI.Border = Rectangle.BOTTOM_BORDER;
            SDI.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Blanco = new PdfPCell(new Paragraph("Tipo de Nomina: " + DatosEmpleado.TipoNomina, _standardFont));
            Blanco.VerticalAlignment = Element.ALIGN_CENTER;
            Blanco.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //Blanco.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
            Blanco.HorizontalAlignment = Element.ALIGN_LEFT;


            TablaEmpleado.AddCell(Clave);
            TablaEmpleado.AddCell(NOmbres);
            TablaEmpleado.AddCell(RFCDato);
            TablaEmpleado.AddCell(IMSS);
            TablaEmpleado.AddCell(FechaIngreso);
            TablaEmpleado.AddCell(CURP);
            TablaEmpleado.AddCell(Puesto);
            TablaEmpleado.AddCell(Departamento);
            TablaEmpleado.AddCell(TipoSalario);
            TablaEmpleado.AddCell(SD);
            TablaEmpleado.AddCell(SDI);
            TablaEmpleado.AddCell(Blanco);



            var otracelda3 = new PdfPCell();
            otracelda3.AddElement(TablaEmpleado);
            TablaEmpleadoFinal.AddCell(otracelda3);
            doc.Add(TablaEmpleadoFinal);


            PdfPCell Asistencia = new PdfPCell(new Paragraph("Asistencia", _standardFont));
            Asistencia.VerticalAlignment = Element.ALIGN_LEFT;
            Asistencia.BorderColor = iTextSharp.text.BaseColor.WHITE;
            Asistencia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Descanso = new PdfPCell(new Paragraph("D: " + descansos, _standardFont));
            Descanso.VerticalAlignment = Element.ALIGN_LEFT;
            Descanso.BorderColor = iTextSharp.text.BaseColor.WHITE;
            Descanso.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Asistencias = new PdfPCell(new Paragraph("A: " + asistencias, _standardFont));
            Asistencias.VerticalAlignment = Element.ALIGN_LEFT;
            Asistencias.BorderColor = iTextSharp.text.BaseColor.WHITE;
            Asistencias.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Vacaciones = new PdfPCell(new Paragraph("V: " + vacaciones, _standardFont));
            Vacaciones.VerticalAlignment = Element.ALIGN_LEFT;
            Vacaciones.BorderColor = iTextSharp.text.BaseColor.WHITE;
            Vacaciones.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell IncapacidadR = new PdfPCell(new Paragraph("IR: " + incapacidadR, _standardFont));
            IncapacidadR.VerticalAlignment = Element.ALIGN_LEFT;
            IncapacidadR.BorderColor = iTextSharp.text.BaseColor.WHITE;
            IncapacidadR.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell IncapacidadE = new PdfPCell(new Paragraph("IE: " + incapacidadE, _standardFont));
            IncapacidadE.VerticalAlignment = Element.ALIGN_LEFT;
            IncapacidadE.BorderColor = iTextSharp.text.BaseColor.WHITE;
            IncapacidadE.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell IncapacidadM = new PdfPCell(new Paragraph("IM: " + incapacidadM, _standardFont));
            IncapacidadM.VerticalAlignment = Element.ALIGN_LEFT;
            IncapacidadM.BorderColor = iTextSharp.text.BaseColor.WHITE;
            IncapacidadM.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell FaltasI = new PdfPCell(new Paragraph("FI: " + faltasI, _standardFont));
            FaltasI.VerticalAlignment = Element.ALIGN_LEFT;
            FaltasI.BorderColor = iTextSharp.text.BaseColor.WHITE;
            FaltasI.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell FaltasA = new PdfPCell(new Paragraph("FA: " + faltasA, _standardFont));
            FaltasA.VerticalAlignment = Element.ALIGN_LEFT;
            FaltasA.BorderColor = iTextSharp.text.BaseColor.WHITE;
            FaltasA.HorizontalAlignment = Element.ALIGN_LEFT;

            TablaAsistencia.AddCell(Asistencia);
            TablaAsistencia.AddCell(Descanso);
            TablaAsistencia.AddCell(Asistencias);
            TablaAsistencia.AddCell(Vacaciones);
            TablaAsistencia.AddCell(IncapacidadR);
            TablaAsistencia.AddCell(IncapacidadE);
            TablaAsistencia.AddCell(IncapacidadM);
            TablaAsistencia.AddCell(FaltasI);
            TablaAsistencia.AddCell(FaltasA);

            var CeldaAsis = new PdfPCell();
            CeldaAsis.AddElement(TablaAsistencia);
            TablaAsistenciaFinal.AddCell(CeldaAsis);

            doc.Add(TablaAsistenciaFinal);
            //doc.Add(new Paragraph(" "));

            if (complemento == false)
            {
                foreach (var det in DetalleNomina.Where(x => x.complemento == false && x.Cantidad > 0))
                {
                    PdfPCell Descripcion = new PdfPCell(new Paragraph(det.Detalle, _standardFont));
                    Descripcion.VerticalAlignment = Element.ALIGN_LEFT;
                    Descripcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    Descripcion.HorizontalAlignment = Element.ALIGN_LEFT;

                    PdfPCell Cantidad = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(det.Cantidad).ToString(), _standardFont));
                    Cantidad.VerticalAlignment = Element.ALIGN_LEFT;
                    Cantidad.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    Cantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                    TablaPercepciones.AddCell(Descripcion);
                    TablaPercepciones.AddCell(Cantidad);

                }
            }
            else
            {
                foreach (var det in DetalleNomina.Where(x => x.Cantidad > 0))
                {
                    PdfPCell Descripcion = new PdfPCell(new Paragraph(det.Detalle, _standardFont));
                    Descripcion.VerticalAlignment = Element.ALIGN_LEFT;
                    Descripcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    Descripcion.HorizontalAlignment = Element.ALIGN_LEFT;

                    PdfPCell Cantidad = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(det.Cantidad).ToString(), _standardFont));
                    Cantidad.VerticalAlignment = Element.ALIGN_LEFT;
                    Cantidad.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    Cantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                    TablaPercepciones.AddCell(Descripcion);
                    TablaPercepciones.AddCell(Cantidad);

                }
            }

            decimal totalAuxiliar = 0;
            foreach (var det in DetalleNominaDeduccion)
            {
                PdfPCell Descripcion = new PdfPCell(new Paragraph(det.Detalle, _standardFont));
                Descripcion.VerticalAlignment = Element.ALIGN_LEFT;
                Descripcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Descripcion.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell Cantidad = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(det.Cantidad).ToString(), _standardFont));
                Cantidad.VerticalAlignment = Element.ALIGN_LEFT;
                Cantidad.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Cantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                TablaDeduccion.AddCell(Descripcion);
                TablaDeduccion.AddCell(Cantidad);
                totalAuxiliar = totalAuxiliar + det.Cantidad;
            }


            var otracelda = new PdfPCell();
            otracelda.AddElement(TablaPercepciones);
            TablaPercepcionesFinal.AddCell(otracelda);

            var otracelda2 = new PdfPCell();
            otracelda2.AddElement(TablaDeduccion);
            TablaPercepcionesFinal.AddCell(otracelda2);

            doc.Add(TablaPercepcionesFinal);


          
            if(pperiodo.IdTipoNomina == 11)
            {
                PdfPCell TituloPercepcion = new PdfPCell(new Paragraph("Total Percepcion: ", _standardFont2));
                TituloPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloPercepcion.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell TotalPercepcion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(finiquito.TotalPercepciones).ToString(), _standardFont2));
                TotalPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPercepcion.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell TituloNet = new PdfPCell(new Paragraph("Neto Total: ", _standardFont2));
                TituloNet.VerticalAlignment = Element.ALIGN_LEFT;
                TituloNet.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloNet.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell NetTotal = new PdfPCell(new Paragraph(Utils.TruncateDecimales(finiquito.Total_Finiquito).ToString(), _standardFont2));
                NetTotal.VerticalAlignment = Element.ALIGN_LEFT;
                NetTotal.BorderColor = iTextSharp.text.BaseColor.WHITE;
                NetTotal.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell TituloDeduccion = new PdfPCell(new Paragraph("Total Deduccion: ", _standardFont2));
                TituloDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;


                PdfPCell TotalDeduccion = new PdfPCell(new Paragraph(Utils.TruncateDecimales((finiquito.TotalDeducciones + totalAuxiliar)).ToString(), _standardFont2));
                TotalDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
                TotalDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalDeduccion.HorizontalAlignment = Element.ALIGN_CENTER;


                TablaTotalesPercepcion.AddCell(TituloPercepcion);
                TablaTotalesPercepcion.AddCell(TotalPercepcion);
                TablaTotalesPercepcion.AddCell(TituloNet);
                TablaTotalesPercepcion.AddCell(NetTotal);


                TablasTotalesDeduccion.AddCell(TituloDeduccion);
                TablasTotalesDeduccion.AddCell(TotalDeduccion);

                var CeltaTOtalPercepcion = new PdfPCell();
                CeltaTOtalPercepcion.AddElement(TablaTotalesPercepcion);

                var CeldaTotalDeduccion = new PdfPCell();
                CeldaTotalDeduccion.AddElement(TablasTotalesDeduccion);


                TablaTotalesFinales.AddCell(CeltaTOtalPercepcion);
                TablaTotalesFinales.AddCell(CeldaTotalDeduccion);


                doc.Add(TablaTotalesFinales);
            }
            else
            {
                PdfPCell TituloPercepcion = new PdfPCell(new Paragraph("Total Percepcion: ", _standardFont2));
                TituloPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloPercepcion.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell TotalPercepcion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(nominas.TotalPercepciones).ToString(), _standardFont2));
                TotalPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPercepcion.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell TituloNet = new PdfPCell(new Paragraph("Neto Total: ", _standardFont2));
                TituloNet.VerticalAlignment = Element.ALIGN_LEFT;
                TituloNet.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloNet.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell NetTotal = new PdfPCell(new Paragraph(Utils.TruncateDecimales(nominas.TotalNomina).ToString(), _standardFont2));
                NetTotal.VerticalAlignment = Element.ALIGN_LEFT;
                NetTotal.BorderColor = iTextSharp.text.BaseColor.WHITE;
                NetTotal.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell TituloDeduccion = new PdfPCell(new Paragraph("Total Deduccion: ", _standardFont2));
                TituloDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;


                PdfPCell TotalDeduccion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(nominas.TotalDeducciones).ToString(), _standardFont2));
                TotalDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
                TotalDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalDeduccion.HorizontalAlignment = Element.ALIGN_CENTER;


                TablaTotalesPercepcion.AddCell(TituloPercepcion);
                TablaTotalesPercepcion.AddCell(TotalPercepcion);
            


                TablasTotalesDeduccion.AddCell(TituloDeduccion);
                TablasTotalesDeduccion.AddCell(TotalDeduccion);
                TablasTotalesDeduccion.AddCell(TituloNet);
                TablasTotalesDeduccion.AddCell(NetTotal);

                var CeltaTOtalPercepcion = new PdfPCell();
                CeltaTOtalPercepcion.AddElement(TablaTotalesPercepcion);

                var CeldaTotalDeduccion = new PdfPCell();
                CeldaTotalDeduccion.AddElement(TablasTotalesDeduccion);


                TablaTotalesFinales.AddCell(CeltaTOtalPercepcion);
                TablaTotalesFinales.AddCell(CeldaTotalDeduccion);


                doc.Add(TablaTotalesFinales);
            }
          



          




            //doc.Add(TablaTotalesFinales);
            //doc.Add(TablaDeduccionFinal);

            doc.Add(new Paragraph(" "));






        }

        #region Totales 
        public void TotalesAutorizacion(ref Document doc, NOM_PeriodosPago pperiodo, bool complemento, int IdEmpresa)
        {
            ReportesDAO reportes = new ReportesDAO();
            List<NOM_Cuotas_IMSS> cuotas = new List<NOM_Cuotas_IMSS>();
            var obligacionesTotales = new TotalOblicaciones();
            var CuotasIMSS = new TotalesRubrosIMSS();
            var totales = new List<AutorizacionDetalle>();
            var obligaciones = new Obligaciones();

            if(pperiodo.IdTipoNomina != 11)
            {
                obligacionesTotales = reportes.totalesObligaciones(pperiodo.IdPeriodoPago);
                CuotasIMSS = reportes.HistorialCuotas(pperiodo.IdPeriodoPago, IdEmpresa);
                totales = reportes.autorizaciondetalleByEmpresa(pperiodo.IdPeriodoPago, IdEmpresa);
                obligaciones = reportes.obligacionesGenerales(pperiodo.IdPeriodoPago, IdEmpresa);
            }else
            {
                obligacionesTotales = reportes.totalesObligacionesFiniquito(pperiodo.IdPeriodoPago);
                CuotasIMSS = reportes.HistorialCuotasFiniquito(pperiodo.IdPeriodoPago, IdEmpresa);
                totales = reportes.autorizaciondetalleByEmpresaFiniquito(pperiodo.IdPeriodoPago, IdEmpresa);
                obligaciones = reportes.obligacionesGeneralesFiniquito(pperiodo.IdPeriodoPago, IdEmpresa);
            }
            

            decimal SumaPercecpiones = 0;
            decimal SumaDeducciones = 0;
            decimal SumaObligaciones = 0;
            decimal TotalNeto = 0;
            Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
            Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);
            Font _standardFont3 = new Font(Font.FontFamily.HELVETICA, 11, Font.BOLD, BaseColor.BLACK);
            //Tabla Percepciones
            PdfPTable TablaTotalPercepciones = new PdfPTable(2);
            TablaTotalPercepciones.WidthPercentage = 100;
            TablaTotalPercepciones.HorizontalAlignment = Element.ALIGN_LEFT;

            //Tabla Deducciones
            PdfPTable TablaTotalDeducciones = new PdfPTable(2);
            TablaTotalDeducciones.WidthPercentage = 100;
            TablaTotalDeducciones.HorizontalAlignment = Element.ALIGN_LEFT;

            //Tabla Obligaciones
            PdfPTable TablaTotalObligacion = new PdfPTable(2);
            TablaTotalObligacion.WidthPercentage = 100;
            TablaTotalObligacion.HorizontalAlignment = Element.ALIGN_LEFT;

            //Tabla Rubros IMSS
            PdfPTable TablaTotalRubrosIMSS = new PdfPTable(3);
            TablaTotalRubrosIMSS.WidthPercentage = 100;
            TablaTotalRubrosIMSS.HorizontalAlignment = Element.ALIGN_LEFT;

            //Tabla  Principal que almancena la Tabla de Obligacion y Rubros IMSS
            PdfPTable TablaGuardarTotalesObligacionesRubros = new PdfPTable(2);
            TablaGuardarTotalesObligacionesRubros.WidthPercentage = 100;
            TablaGuardarTotalesObligacionesRubros.HorizontalAlignment = Element.ALIGN_LEFT;

            //Tabla  Principal que almancena la Tabla de Percepciones y Deducciones
            PdfPTable TablaGuardarTotalesPercepcionDeduccion = new PdfPTable(2);
            TablaGuardarTotalesPercepcionDeduccion.WidthPercentage = 100;
            TablaGuardarTotalesPercepcionDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;

            //encabezado de las percepciones totales
            PdfPCell TituloPercepcion1 = new PdfPCell(new Paragraph("Conceptos:", _standardFont2));
            TituloPercepcion1.VerticalAlignment = Element.ALIGN_LEFT;
            TituloPercepcion1.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TituloPercepcion1.HorizontalAlignment = Element.ALIGN_LEFT;
            TituloPercepcion1.PaddingLeft = 10;
            TituloPercepcion1.PaddingTop = 10;
            PdfPCell TotalPercepcion1 = new PdfPCell(new Paragraph("Total", _standardFont2));
            TotalPercepcion1.VerticalAlignment = Element.ALIGN_LEFT;
            TotalPercepcion1.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TotalPercepcion1.HorizontalAlignment = Element.ALIGN_RIGHT;
            TotalPercepcion1.PaddingTop = 10;
            TotalPercepcion1.PaddingRight = 10;

            TablaTotalPercepciones.AddCell(TituloPercepcion1);
            TablaTotalPercepciones.AddCell(TotalPercepcion1);

            //conceptos de las percepciones
            if (complemento == false)
            {
                foreach (var total in totales.Where(x => x.TipoConcepto == 1 && x.Complemento == false))
                {
                    PdfPCell TituloPercepcion = new PdfPCell(new Paragraph(total.NombreConcepto, _standardFont));
                    TituloPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                    TituloPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    TituloPercepcion.HorizontalAlignment = Element.ALIGN_LEFT;
                    TituloPercepcion.PaddingLeft = 10;
                    PdfPCell TotalPercepcion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(total.TotalConcepto).ToString(), _standardFont));
                    TotalPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                    TotalPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    TotalPercepcion.HorizontalAlignment = Element.ALIGN_RIGHT;
                    TotalPercepcion.PaddingRight = 10;
                    TablaTotalPercepciones.AddCell(TituloPercepcion);
                    TablaTotalPercepciones.AddCell(TotalPercepcion);

                    SumaPercecpiones = SumaPercecpiones + total.TotalConcepto;
                }
            }
            else
            {
                foreach (var total in totales.Where(x => x.TipoConcepto == 1))
                {
                    PdfPCell TituloPercepcion = new PdfPCell(new Paragraph(total.NombreConcepto, _standardFont));
                    TituloPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                    TituloPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    TituloPercepcion.HorizontalAlignment = Element.ALIGN_LEFT;
                    TituloPercepcion.PaddingLeft = 10;
                    PdfPCell TotalPercepcion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(total.TotalConcepto).ToString(), _standardFont));
                    TotalPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
                    TotalPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    TotalPercepcion.HorizontalAlignment = Element.ALIGN_RIGHT;
                    TotalPercepcion.PaddingRight = 10;
                    TablaTotalPercepciones.AddCell(TituloPercepcion);
                    TablaTotalPercepciones.AddCell(TotalPercepcion);

                    SumaPercecpiones = SumaPercecpiones + total.TotalConcepto;
                }
            }


            PdfPCell SeparadorPercepciones = new PdfPCell(new Paragraph("-----------------------------"));
            SeparadorPercepciones.VerticalAlignment = Element.ALIGN_LEFT;
            SeparadorPercepciones.BorderColor = iTextSharp.text.BaseColor.WHITE;
            SeparadorPercepciones.HorizontalAlignment = Element.ALIGN_LEFT;
            SeparadorPercepciones.PaddingLeft = 10;
            PdfPCell SeparadorPercepciones1 = new PdfPCell(new Paragraph("-----------------------------"));
            SeparadorPercepciones1.VerticalAlignment = Element.ALIGN_LEFT;
            SeparadorPercepciones1.BorderColor = iTextSharp.text.BaseColor.WHITE;
            SeparadorPercepciones1.HorizontalAlignment = Element.ALIGN_RIGHT;
            SeparadorPercepciones1.PaddingRight = 10;
            TablaTotalPercepciones.AddCell(SeparadorPercepciones);
            TablaTotalPercepciones.AddCell(SeparadorPercepciones1);


            PdfPCell SumaTituloPercepcion = new PdfPCell(new Paragraph("Suma Percepciones", _standardFont2));
            SumaTituloPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
            SumaTituloPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            SumaTituloPercepcion.HorizontalAlignment = Element.ALIGN_LEFT;
            SumaTituloPercepcion.PaddingLeft = 10;
            SumaTituloPercepcion.PaddingBottom = 10;
            PdfPCell TotalSumaPercepcion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(SumaPercecpiones).ToString(), _standardFont2));
            TotalSumaPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
            TotalSumaPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TotalSumaPercepcion.HorizontalAlignment = Element.ALIGN_RIGHT;
            TotalSumaPercepcion.PaddingRight = 10;
            TotalSumaPercepcion.PaddingBottom = 10;

            TablaTotalPercepciones.AddCell(SumaTituloPercepcion);
            TablaTotalPercepciones.AddCell(TotalSumaPercepcion);




            //Titulo lado Deducciones
            PdfPCell TituloDeduccion1 = new PdfPCell(new Paragraph("Concepto:", _standardFont2));
            TituloDeduccion1.VerticalAlignment = Element.ALIGN_LEFT;
            TituloDeduccion1.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TituloDeduccion1.HorizontalAlignment = Element.ALIGN_LEFT;
            TituloDeduccion1.PaddingLeft = 10;
            TituloDeduccion1.PaddingTop = 10;
            PdfPCell TotalDeduccion1 = new PdfPCell(new Paragraph("Total", _standardFont2));
            TotalDeduccion1.VerticalAlignment = Element.ALIGN_LEFT;
            TotalDeduccion1.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TotalDeduccion1.HorizontalAlignment = Element.ALIGN_RIGHT;
            TotalDeduccion1.PaddingRight = 10;
            TotalDeduccion1.PaddingTop = 10;
            TablaTotalDeducciones.AddCell(TituloDeduccion1);
            TablaTotalDeducciones.AddCell(TotalDeduccion1);
            //conceptos deducciones
            foreach (var total in totales.Where(x => x.TipoConcepto == 2 && x.Complemento == false))
            {
                PdfPCell TituloDeduccion = new PdfPCell(new Paragraph(total.NombreConcepto, _standardFont));
                TituloDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion.PaddingLeft = 10;
                PdfPCell TotalDeduccion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(total.TotalConcepto).ToString(), _standardFont));
                TotalDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
                TotalDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalDeduccion.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalDeduccion.PaddingRight = 10;
                TablaTotalDeducciones.AddCell(TituloDeduccion);
                TablaTotalDeducciones.AddCell(TotalDeduccion);

                //if (total.IdConcepto == 51)
                //{
                //    if (total.TotalConcepto > 0)
                //    {
                //        SumaDeducciones = SumaDeducciones + total.TotalConcepto;
                //    }
                //}
                //else
                //if (total.IdConcepto != 52)
                //{
                SumaDeducciones = SumaDeducciones + total.TotalConcepto;
                //}


            }

            PdfPCell SeparadorDeducciones = new PdfPCell(new Paragraph("-----------------------------"));
            SeparadorDeducciones.VerticalAlignment = Element.ALIGN_LEFT;
            SeparadorDeducciones.BorderColor = iTextSharp.text.BaseColor.WHITE;
            SeparadorDeducciones.HorizontalAlignment = Element.ALIGN_LEFT;
            SeparadorDeducciones.PaddingLeft = 10;
            PdfPCell SeparadorDeducciones1 = new PdfPCell(new Paragraph("-----------------------------"));
            SeparadorDeducciones1.VerticalAlignment = Element.ALIGN_LEFT;
            SeparadorDeducciones1.BorderColor = iTextSharp.text.BaseColor.WHITE;
            SeparadorDeducciones1.HorizontalAlignment = Element.ALIGN_RIGHT;
            SeparadorDeducciones1.PaddingRight = 10;
            TablaTotalDeducciones.AddCell(SeparadorDeducciones);
            TablaTotalDeducciones.AddCell(SeparadorDeducciones1);

            PdfPCell SumaDeduccion = new PdfPCell(new Paragraph("Suma Deducciones", _standardFont2));
            SumaDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
            SumaDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            SumaDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;
            SumaDeduccion.PaddingLeft = 10;
            SumaDeduccion.PaddingBottom = 10;
            PdfPCell SumaTotalDeduccion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(SumaDeducciones).ToString(), _standardFont2));
            SumaTotalDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
            SumaTotalDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            SumaTotalDeduccion.HorizontalAlignment = Element.ALIGN_RIGHT;
            SumaTotalDeduccion.PaddingBottom = 10;
            SumaTotalDeduccion.PaddingRight = 10;
            TablaTotalDeducciones.AddCell(SumaDeduccion);
            TablaTotalDeducciones.AddCell(SumaTotalDeduccion);

            var celdafinalP = new PdfPCell();
            celdafinalP.AddElement(TablaTotalPercepciones);

            var celdafinalD = new PdfPCell();
            celdafinalD.AddElement(TablaTotalDeducciones);

            TablaGuardarTotalesPercepcionDeduccion.AddCell(celdafinalP);
            TablaGuardarTotalesPercepcionDeduccion.AddCell(celdafinalD);
            //Pinta la Tabla de Percepciones y Deducciones
            doc.Add(new Paragraph("                     Total Percepciones                                                     Total Deducciones"));
            doc.Add(new Paragraph(" "));
            doc.Add(TablaGuardarTotalesPercepcionDeduccion);

            //Total Final Percepcion - Deduccion
            TotalNeto = SumaPercecpiones - SumaDeducciones;
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("           Total neto = " + Utils.TruncateDecimales(TotalNeto).ToString(), _standardFont3));


            if (CuotasIMSS != null)
            {

                //encabezado de las obligaciones
                PdfPCell TituloObligacion1 = new PdfPCell(new Paragraph("Concepto:", _standardFont2));
                TituloObligacion1.VerticalAlignment = Element.ALIGN_LEFT;
                TituloObligacion1.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloObligacion1.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloObligacion1.PaddingTop = 10;
                TituloObligacion1.PaddingLeft = 10;

                PdfPCell TotalObligacion1 = new PdfPCell(new Paragraph("Total", _standardFont2));
                TotalObligacion1.VerticalAlignment = Element.ALIGN_LEFT;
                TotalObligacion1.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalObligacion1.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalObligacion1.PaddingRight = 10;
                TotalObligacion1.PaddingTop = 10;
                TablaTotalObligacion.AddCell(TituloObligacion1);
                TablaTotalObligacion.AddCell(TotalObligacion1);


                //conceptos obligaciones

                PdfPCell TituloObligacion = new PdfPCell(new Paragraph("Fondo de Retiro", _standardFont));
                TituloObligacion.VerticalAlignment = Element.ALIGN_LEFT;
                TituloObligacion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloObligacion.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloObligacion.PaddingLeft = 10;
                PdfPCell TotalObligacion = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(obligaciones.FondoRetiro).ToString(), _standardFont));
                TotalObligacion.VerticalAlignment = Element.ALIGN_LEFT;
                TotalObligacion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalObligacion.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalObligacion.PaddingRight = 10;
                TablaTotalObligacion.AddCell(TituloObligacion);
                TablaTotalObligacion.AddCell(TotalObligacion);

                PdfPCell TituloObligacion2 = new PdfPCell(new Paragraph("Guarderia", _standardFont));
                TituloObligacion2.VerticalAlignment = Element.ALIGN_LEFT;
                TituloObligacion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloObligacion2.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloObligacion2.PaddingLeft = 10;
                PdfPCell TotalObligacion2 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(obligaciones.Guarderia).ToString(), _standardFont));
                TotalObligacion2.VerticalAlignment = Element.ALIGN_LEFT;
                TotalObligacion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalObligacion2.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalObligacion2.PaddingRight = 10;
                TablaTotalObligacion.AddCell(TituloObligacion2);
                TablaTotalObligacion.AddCell(TotalObligacion2);

                PdfPCell TituloObligacion3 = new PdfPCell(new Paragraph("IMSS Empresa", _standardFont));
                TituloObligacion3.VerticalAlignment = Element.ALIGN_LEFT;
                TituloObligacion3.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloObligacion3.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloObligacion3.PaddingLeft = 10;
                PdfPCell TotalObligacion3 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(obligaciones.IMSSEmpresa).ToString(), _standardFont));
                TotalObligacion3.VerticalAlignment = Element.ALIGN_LEFT;
                TotalObligacion3.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalObligacion3.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalObligacion3.PaddingRight = 10;
                TablaTotalObligacion.AddCell(TituloObligacion3);
                TablaTotalObligacion.AddCell(TotalObligacion3);

                PdfPCell TituloObligacion4 = new PdfPCell(new Paragraph("Riesgo de Trabajo", _standardFont));
                TituloObligacion4.VerticalAlignment = Element.ALIGN_LEFT;
                TituloObligacion4.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloObligacion4.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloObligacion4.PaddingLeft = 10;
                PdfPCell TotalObligacion4 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(obligaciones.RiesgoTrabajo).ToString(), _standardFont));
                TotalObligacion4.VerticalAlignment = Element.ALIGN_LEFT;
                TotalObligacion4.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalObligacion4.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalObligacion4.PaddingRight = 10;
                TablaTotalObligacion.AddCell(TituloObligacion4);
                TablaTotalObligacion.AddCell(TotalObligacion4);

                PdfPCell TituloObligacion5 = new PdfPCell(new Paragraph("Impuesto sobre Nomina", _standardFont));
                TituloObligacion5.VerticalAlignment = Element.ALIGN_LEFT;
                TituloObligacion5.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloObligacion5.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloObligacion5.PaddingLeft = 10;
                PdfPCell TotalObligacion5 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(obligaciones.ImpuestoNomina).ToString(), _standardFont));
                TotalObligacion5.VerticalAlignment = Element.ALIGN_LEFT;
                TotalObligacion5.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalObligacion5.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalObligacion5.PaddingRight = 10;
                TablaTotalObligacion.AddCell(TituloObligacion5);
                TablaTotalObligacion.AddCell(TotalObligacion5);

                SumaObligaciones = SumaObligaciones + 1; /*+ total.TotalConcepto;*/

                PdfPCell SeparadorObligaciones = new PdfPCell(new Paragraph("-----------------------------"));
                SeparadorObligaciones.VerticalAlignment = Element.ALIGN_LEFT;
                SeparadorObligaciones.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SeparadorObligaciones.HorizontalAlignment = Element.ALIGN_LEFT;
                SeparadorObligaciones.PaddingLeft = 10;
                PdfPCell SeparadorObligaciones2 = new PdfPCell(new Paragraph("-----------------------------"));
                SeparadorObligaciones2.VerticalAlignment = Element.ALIGN_LEFT;
                SeparadorObligaciones2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SeparadorObligaciones2.HorizontalAlignment = Element.ALIGN_RIGHT;
                SeparadorObligaciones2.PaddingRight = 10;

                TablaTotalObligacion.AddCell(SeparadorObligaciones);
                TablaTotalObligacion.AddCell(SeparadorObligaciones2);

                SumaObligaciones = obligaciones.FondoRetiro + obligaciones.Guarderia + obligaciones.ImpuestoNomina + obligaciones.IMSSEmpresa + obligaciones.RiesgoTrabajo;
                PdfPCell SumaObligacion = new PdfPCell(new Paragraph("Suma Obligacion", _standardFont2));
                SumaObligacion.VerticalAlignment = Element.ALIGN_LEFT;
                SumaObligacion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SumaObligacion.HorizontalAlignment = Element.ALIGN_LEFT;
                SumaObligacion.PaddingLeft = 10;
                SumaObligacion.PaddingBottom = 10;
                PdfPCell SumaTotalObligacion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(SumaObligaciones).ToString(), _standardFont2));
                SumaTotalObligacion.VerticalAlignment = Element.ALIGN_LEFT;
                SumaTotalObligacion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SumaTotalObligacion.HorizontalAlignment = Element.ALIGN_RIGHT;
                SumaTotalObligacion.PaddingBottom = 10;
                SumaTotalObligacion.PaddingRight = 10;

                TablaTotalObligacion.AddCell(SumaObligacion);
                TablaTotalObligacion.AddCell(SumaTotalObligacion);

                // Inicio de la Tabla de Rubros de IMSS

                //Encabezado de la tabla


                PdfPCell Concepto = new PdfPCell(new Paragraph("Concepto", _standardFont2));
                Concepto.VerticalAlignment = Element.ALIGN_LEFT;
                Concepto.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Concepto.HorizontalAlignment = Element.ALIGN_LEFT;
                Concepto.PaddingLeft = 10;
                Concepto.PaddingTop = 10;
                PdfPCell Empresa = new PdfPCell(new Paragraph("Empresa", _standardFont2));
                Empresa.VerticalAlignment = Element.ALIGN_LEFT;
                Empresa.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Empresa.HorizontalAlignment = Element.ALIGN_RIGHT;
                Empresa.PaddingTop = 10;
                PdfPCell Obrero = new PdfPCell(new Paragraph("Obrero", _standardFont2));
                Obrero.VerticalAlignment = Element.ALIGN_LEFT;
                Obrero.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Obrero.HorizontalAlignment = Element.ALIGN_RIGHT;
                Obrero.PaddingRight = 25;
                Obrero.PaddingTop = 10;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(Concepto);
                TablaTotalRubrosIMSS.AddCell(Empresa);
                TablaTotalRubrosIMSS.AddCell(Obrero);


                //Censatia y Vejez
                PdfPCell CensantiaVejez = new PdfPCell(new Paragraph("Cesantía y Vejez", _standardFont));
                CensantiaVejez.VerticalAlignment = Element.ALIGN_LEFT;
                CensantiaVejez.BorderColor = iTextSharp.text.BaseColor.WHITE;
                CensantiaVejez.HorizontalAlignment = Element.ALIGN_LEFT;
                CensantiaVejez.PaddingLeft = 10;
                PdfPCell TotalCensantiaVejezP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalCenyVejPatron).ToString(), _standardFont));
                TotalCensantiaVejezP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalCensantiaVejezP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalCensantiaVejezP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalCensantiaVejezO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalCenyVejObrero).ToString(), _standardFont));
                TotalCensantiaVejezO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalCensantiaVejezO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalCensantiaVejezO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalCensantiaVejezO.PaddingRight = 25;

                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(CensantiaVejez);
                TablaTotalRubrosIMSS.AddCell(TotalCensantiaVejezP);
                TablaTotalRubrosIMSS.AddCell(TotalCensantiaVejezO);

                //Pension
                PdfPCell Pension = new PdfPCell(new Paragraph("Pension", _standardFont));
                Pension.VerticalAlignment = Element.ALIGN_LEFT;
                Pension.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Pension.HorizontalAlignment = Element.ALIGN_LEFT;
                Pension.PaddingLeft = 10;
                PdfPCell TotalPensionP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalPensionadosPatron).ToString(), _standardFont));
                TotalPensionP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPensionP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPensionP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalPensionO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalPrestacionesObrero).ToString(), _standardFont));
                TotalPensionO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPensionO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPensionO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalPensionO.PaddingRight = 25;

                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(Pension);
                TablaTotalRubrosIMSS.AddCell(TotalPensionP);
                TablaTotalRubrosIMSS.AddCell(TotalPensionO);

                //Prestamo de Dinero
                PdfPCell Prestamo = new PdfPCell(new Paragraph("Prestamo de Dinero", _standardFont));
                Prestamo.VerticalAlignment = Element.ALIGN_LEFT;
                Prestamo.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Prestamo.HorizontalAlignment = Element.ALIGN_LEFT;
                Prestamo.PaddingLeft = 10;
                PdfPCell TotalPretamoP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalPrestacionesPatron).ToString(), _standardFont));
                TotalPretamoP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPretamoP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPretamoP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalPretamoO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalPrestacionesObrero).ToString(), _standardFont));
                TotalPretamoO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPretamoO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPretamoO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalPretamoO.PaddingRight = 25;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(Prestamo);
                TablaTotalRubrosIMSS.AddCell(TotalPretamoP);
                TablaTotalRubrosIMSS.AddCell(TotalPretamoO);

                //Cuota Fija
                PdfPCell CuotaFija = new PdfPCell(new Paragraph("Cuota Fija", _standardFont));
                CuotaFija.VerticalAlignment = Element.ALIGN_LEFT;
                CuotaFija.BorderColor = iTextSharp.text.BaseColor.WHITE;
                CuotaFija.HorizontalAlignment = Element.ALIGN_LEFT;
                CuotaFija.PaddingLeft = 10;
                PdfPCell TotalCuotaFijaP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalCuotoFija).ToString(), _standardFont));
                TotalCuotaFijaP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalCuotaFijaP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalCuotaFijaP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalCuotaFijaO = new PdfPCell(new Paragraph("$0.00", _standardFont));
                TotalCuotaFijaO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalCuotaFijaO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalCuotaFijaO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalCuotaFijaO.PaddingRight = 25;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(CuotaFija);
                TablaTotalRubrosIMSS.AddCell(TotalCuotaFijaP);
                TablaTotalRubrosIMSS.AddCell(TotalCuotaFijaO);

                //Excendentes
                PdfPCell Excedentes = new PdfPCell(new Paragraph("Excedentes", _standardFont));
                Excedentes.VerticalAlignment = Element.ALIGN_LEFT;
                Excedentes.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Excedentes.HorizontalAlignment = Element.ALIGN_LEFT;
                Excedentes.PaddingLeft = 10;
                PdfPCell TotalExcedentesP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalExcedentePatron).ToString(), _standardFont));
                TotalExcedentesP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalExcedentesP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalExcedentesP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalExcedentesO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalExcedenteobrero).ToString(), _standardFont));
                TotalExcedentesO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalExcedentesO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalExcedentesO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalExcedentesO.PaddingRight = 25;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(Excedentes);
                TablaTotalRubrosIMSS.AddCell(TotalExcedentesP);
                TablaTotalRubrosIMSS.AddCell(TotalExcedentesO);

                //Guarderias
                PdfPCell Guarderias = new PdfPCell(new Paragraph("Guarderias", _standardFont));
                Guarderias.VerticalAlignment = Element.ALIGN_LEFT;
                Guarderias.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Guarderias.HorizontalAlignment = Element.ALIGN_LEFT;
                Guarderias.PaddingLeft = 10;
                PdfPCell TotalGuarderiasP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalGuarderiasPatron).ToString(), _standardFont));
                TotalGuarderiasP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalGuarderiasP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalGuarderiasP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalGuarderiasO = new PdfPCell(new Paragraph("$0.00", _standardFont));
                TotalGuarderiasO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalGuarderiasO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalGuarderiasO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalGuarderiasO.PaddingRight = 25;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(Guarderias);
                TablaTotalRubrosIMSS.AddCell(TotalGuarderiasP);
                TablaTotalRubrosIMSS.AddCell(TotalGuarderiasO);

                //Infonavit
                PdfPCell Infonavit = new PdfPCell(new Paragraph("Infonavit", _standardFont));
                Infonavit.VerticalAlignment = Element.ALIGN_LEFT;
                Infonavit.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Infonavit.HorizontalAlignment = Element.ALIGN_LEFT;
                Infonavit.PaddingLeft = 10;
                PdfPCell TotalInfonavitP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalInfonavitPatron).ToString(), _standardFont));
                TotalInfonavitP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalInfonavitP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalInfonavitP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalInfonavitO = new PdfPCell(new Paragraph("$0.00", _standardFont));
                TotalInfonavitO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalInfonavitO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalInfonavitO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalInfonavitO.PaddingRight = 25;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(Infonavit);
                TablaTotalRubrosIMSS.AddCell(TotalInfonavitP);
                TablaTotalRubrosIMSS.AddCell(TotalInfonavitO);

                //Invalidez y Vida
                PdfPCell InvalidezVida = new PdfPCell(new Paragraph("Invalidez y Vida", _standardFont));
                InvalidezVida.VerticalAlignment = Element.ALIGN_LEFT;
                InvalidezVida.BorderColor = iTextSharp.text.BaseColor.WHITE;
                InvalidezVida.HorizontalAlignment = Element.ALIGN_LEFT;
                InvalidezVida.PaddingLeft = 10;
                PdfPCell TotalInvalidezVidaP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalInvalidezVidaPatron).ToString(), _standardFont));
                TotalInvalidezVidaP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalInvalidezVidaP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalInvalidezVidaP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalInvalidezVidaO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalInvalidezVidaObrero).ToString(), _standardFont));
                TotalInvalidezVidaO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalInvalidezVidaO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalInvalidezVidaO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalInvalidezVidaO.PaddingRight = 25;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(InvalidezVida);
                TablaTotalRubrosIMSS.AddCell(TotalInvalidezVidaP);
                TablaTotalRubrosIMSS.AddCell(TotalInvalidezVidaO);

                //Retiro SAR
                PdfPCell RetiroSAR = new PdfPCell(new Paragraph("Retiro SAR", _standardFont));
                RetiroSAR.VerticalAlignment = Element.ALIGN_LEFT;
                RetiroSAR.BorderColor = iTextSharp.text.BaseColor.WHITE;
                RetiroSAR.HorizontalAlignment = Element.ALIGN_LEFT;
                RetiroSAR.PaddingLeft = 10;
                PdfPCell TotalRetiroSARP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalSeguroRetiroPatron).ToString(), _standardFont));
                TotalRetiroSARP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalRetiroSARP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalRetiroSARP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalRetiroSARO = new PdfPCell(new Paragraph("$0.00", _standardFont));
                TotalRetiroSARO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalRetiroSARO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalRetiroSARO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalRetiroSARO.PaddingRight = 25;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(RetiroSAR);
                TablaTotalRubrosIMSS.AddCell(TotalRetiroSARP);
                TablaTotalRubrosIMSS.AddCell(TotalRetiroSARO);

                //Riesgo de Trabajo
                PdfPCell RiesgoTrabajo = new PdfPCell(new Paragraph("Riesgo de Trabajo", _standardFont));
                RiesgoTrabajo.VerticalAlignment = Element.ALIGN_LEFT;
                RiesgoTrabajo.BorderColor = iTextSharp.text.BaseColor.WHITE;
                RiesgoTrabajo.HorizontalAlignment = Element.ALIGN_LEFT;
                RiesgoTrabajo.PaddingLeft = 10;

                PdfPCell TotalRiesgoTrabajoP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(CuotasIMSS.TotalRiesgoTrabajoPatron).ToString(), _standardFont));
                TotalRiesgoTrabajoP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalRiesgoTrabajoP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalRiesgoTrabajoP.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalRiesgoTrabajoP.PaddingBottom = 10;
                PdfPCell TotalRiesgoTrabajoO = new PdfPCell(new Paragraph("$0.00", _standardFont));
                TotalRiesgoTrabajoO.VerticalAlignment = Element.ALIGN_LEFT;
                TotalRiesgoTrabajoO.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalRiesgoTrabajoO.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalRiesgoTrabajoO.PaddingRight = 25;

                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(RiesgoTrabajo);
                TablaTotalRubrosIMSS.AddCell(TotalRiesgoTrabajoP);
                TablaTotalRubrosIMSS.AddCell(TotalRiesgoTrabajoO);

                decimal TotalRubrosPatron = 0;
                decimal TotalRubrosObrero = 0;
                decimal NetoTotalRubros = 0;
                TotalRubrosPatron = CuotasIMSS.TotalCuotoFija + CuotasIMSS.TotalExcedentePatron + CuotasIMSS.TotalGuarderiasPatron + CuotasIMSS.TotalInfonavitPatron + CuotasIMSS.TotalInvalidezVidaPatron + CuotasIMSS.TotalPrestacionesPatron + CuotasIMSS.TotalPensionadosPatron + CuotasIMSS.TotalRiesgoTrabajoPatron + CuotasIMSS.TotalSeguroRetiroPatron + CuotasIMSS.TotalCenyVejPatron;
                TotalRubrosObrero = CuotasIMSS.TotalCenyVejObrero + CuotasIMSS.TotalExcedenteobrero + CuotasIMSS.TotalInvalidezVidaObrero + CuotasIMSS.TotalPrestacionesObrero + CuotasIMSS.TotalPensionadosObrero;
                NetoTotalRubros = TotalRubrosPatron + TotalRubrosObrero;



                //separador
                PdfPCell SeparadorRubros1 = new PdfPCell(new Paragraph("--------------------"));
                SeparadorRubros1.VerticalAlignment = Element.ALIGN_LEFT;
                SeparadorRubros1.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SeparadorRubros1.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell SeparadorRubros2 = new PdfPCell(new Paragraph("--------------------"));
                SeparadorRubros2.VerticalAlignment = Element.ALIGN_LEFT;
                SeparadorRubros2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SeparadorRubros2.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell SeparadorRubros3 = new PdfPCell(new Paragraph("--------------------"));
                SeparadorRubros3.VerticalAlignment = Element.ALIGN_LEFT;
                SeparadorRubros3.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SeparadorRubros3.HorizontalAlignment = Element.ALIGN_RIGHT;

                TablaTotalRubrosIMSS.AddCell(SeparadorRubros1);
                TablaTotalRubrosIMSS.AddCell(SeparadorRubros2);
                TablaTotalRubrosIMSS.AddCell(SeparadorRubros3);


                //Totales de los Rubros IMSS Empresa/Obrero

                PdfPCell SumaRubros = new PdfPCell(new Paragraph("Suma de Rubros", _standardFont2));
                SumaRubros.VerticalAlignment = Element.ALIGN_LEFT;
                SumaRubros.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SumaRubros.HorizontalAlignment = Element.ALIGN_LEFT;
                SumaRubros.PaddingLeft = 10;
                PdfPCell SumaEmpresa = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(TotalRubrosPatron).ToString(), _standardFont2));
                SumaEmpresa.VerticalAlignment = Element.ALIGN_LEFT;
                SumaEmpresa.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SumaEmpresa.HorizontalAlignment = Element.ALIGN_RIGHT;
                SumaEmpresa.PaddingBottom = 10;
                PdfPCell SumaObrero = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(TotalRubrosObrero).ToString(), _standardFont2));
                SumaObrero.VerticalAlignment = Element.ALIGN_LEFT;
                SumaObrero.BorderColor = iTextSharp.text.BaseColor.WHITE;
                SumaObrero.HorizontalAlignment = Element.ALIGN_RIGHT;
                SumaObrero.PaddingRight = 25;
                SumaObrero.PaddingBottom = 10;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(SumaRubros);
                TablaTotalRubrosIMSS.AddCell(SumaEmpresa);
                TablaTotalRubrosIMSS.AddCell(SumaObrero);

                //Totales de los Rubros IMSS Empresa/Obrero
                PdfPCell NetoTotal = new PdfPCell(new Paragraph("Neto Totales", _standardFont2));
                NetoTotal.VerticalAlignment = Element.ALIGN_LEFT;
                NetoTotal.BorderColor = iTextSharp.text.BaseColor.WHITE;
                NetoTotal.HorizontalAlignment = Element.ALIGN_LEFT;
                NetoTotal.PaddingLeft = 10;
                PdfPCell sumaNetoTotal = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(NetoTotalRubros).ToString(), _standardFont2));
                sumaNetoTotal.VerticalAlignment = Element.ALIGN_LEFT;
                sumaNetoTotal.BorderColor = iTextSharp.text.BaseColor.WHITE;
                sumaNetoTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
                sumaNetoTotal.PaddingBottom = 10;
                PdfPCell vacio = new PdfPCell(new Paragraph(" ", _standardFont2));
                vacio.VerticalAlignment = Element.ALIGN_LEFT;
                vacio.BorderColor = iTextSharp.text.BaseColor.WHITE;
                vacio.HorizontalAlignment = Element.ALIGN_RIGHT;
                vacio.PaddingRight = 25;
                vacio.PaddingBottom = 10;
                //Se incerta en las celdas de la Tabla
                TablaTotalRubrosIMSS.AddCell(NetoTotal);
                TablaTotalRubrosIMSS.AddCell(sumaNetoTotal);
                TablaTotalRubrosIMSS.AddCell(vacio);



                var celdafinalO = new PdfPCell();
                celdafinalO.AddElement(TablaTotalObligacion);

                var celdafinalRI = new PdfPCell();
                celdafinalRI.AddElement(TablaTotalRubrosIMSS);



                TablaGuardarTotalesObligacionesRubros.AddCell(celdafinalRI);
                TablaGuardarTotalesObligacionesRubros.AddCell(celdafinalO);


                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("                     Rubros IMSS                                                            Total Obligaciones"));
                doc.Add(new Paragraph(" "));
                doc.Add(TablaGuardarTotalesObligacionesRubros);
            }

        }
        #endregion
    }
}
