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
   public class Reporte_Finiquito
    {
        RHEntities ctx = null;

        public Reporte_Finiquito()
        {
            ctx = new RHEntities();
        }
        public string CrearReporteAutorizado(NOM_PeriodosPago pperiodo,string ruta, int idusuario )
        {
            var finiquito = ctx.NOM_Finiquito.Where(x => x.IdPeriodo == pperiodo.IdPeriodoPago).FirstOrDefault();
            var cuotas = ctx.NOM_Cuotas_IMSS.Where(x => x.IdFiniquito == finiquito.IdFiniquito).FirstOrDefault();
            var empleado = ctx.Empleado.Where(x => x.IdEmpleado == finiquito.IdEmpleado).FirstOrDefault();
            //var empleadoC = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == empleado.IdEmpleado && x.Status == true).FirstOrDefault();
            var empleadoC = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == empleado.IdEmpleado ).FirstOrDefault();

            var empresaF = ctx.Empresa.Where(x => x.IdEmpresa == empleadoC.IdEmpresaFiscal).FirstOrDefault();
            var puesto = ctx.Puesto.Where(x => x.IdPuesto == empleadoC.IdPuesto).FirstOrDefault();
            var depa = ctx.Departamento.Where(x => x.IdDepartamento == puesto.IdDepartamento).FirstOrDefault();
            var tiponomina = ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == empleadoC.IdPeriodicidadPago).FirstOrDefault();
            var estado = ctx.C_Estado.Where(x => x.IdEstado == empresaF.IdEstado).Select(x => x.Descripcion).FirstOrDefault();
            var empresaC = ctx.Empresa.Where(x => x.IdEmpresa == empleadoC.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault();
            var factura = ctx.NOM_FacturacionF_Finiquito.Where(x => x.IdFiniquito == finiquito.IdFiniquito && x.IdPeriodo == pperiodo.IdPeriodoPago).FirstOrDefault();
            var facturaC = ctx.NOM_FacturacionC_Finiquito.Where(x => x.IdFiniquito == finiquito.IdFiniquito && x.IdPeriodo == pperiodo.IdPeriodoPago).FirstOrDefault();
         

            var impuesto = ctx.NOM_Finiquito_Detalle.Where(x => x.IdFiniquito == finiquito.IdFiniquito).ToList();
            decimal impuestoNomina = 0;

            foreach(var imp in impuesto)
            {
                impuestoNomina = impuestoNomina + imp.ImpuestoSobreNomina; 
            }


            var datos = (from fin in ctx.NOM_Finiquito
                         join cuot in ctx.NOM_Cuotas_IMSS
                         on fin.IdFiniquito equals cuot.IdFiniquito
                         where fin.IdPeriodo == pperiodo.IdPeriodoPago
                         select new Obligaciones
                         {
                             idNomina = fin.IdFiniquito,
                             FondoRetiro = cuot.SeguroRetiro_Patron,
                             ImpuestoNomina = impuestoNomina,
                             Guarderia = cuot.GuarderiasPrestaciones_Patron,
                             IMSSEmpresa = cuot.CesantiaVejez_Patron + cuot.Pensionados_Patron + cuot.Cuota_Fija_Patron + cuot.Excedente_Patron + cuot.InvalidezVida_Patron,
                             RiesgoTrabajo = cuot.RiesgoTrabajo_Patron

                         }).FirstOrDefault();

            var doc = new Document();
            //get the current directory


            var newRuta = ValidarFolderUsuario(idusuario, ruta);
            newRuta = newRuta + "Lista De Raya del Periodo "+pperiodo.Descripcion+".pdf";
            //get PdfWriter object
            PdfWriter.GetInstance(doc, new FileStream(newRuta, FileMode.Create));


            //open the document for writing
            doc.Open();
            // Creamos el tipo de Font que vamos utilizar                
            Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
            Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);
            Font _standardFont3 = new Font(Font.FontFamily.HELVETICA, 11, Font.BOLD, BaseColor.BLACK);
            PdfPTable TablaEncabezado = new PdfPTable(1);
            TablaEncabezado.WidthPercentage = 100;
            PdfPTable TablaEncabezado2 = new PdfPTable(2);
            TablaEncabezado2.WidthPercentage = 100;


            PdfPCell empresa = new PdfPCell(new Paragraph(empresaF.RazonSocial));

            empresa.VerticalAlignment = Element.ALIGN_CENTER;
            empresa.BorderColor = iTextSharp.text.BaseColor.WHITE;
            empresa.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell PeridoPago = new PdfPCell(new Paragraph("Lista de raya del " + pperiodo.Fecha_Inicio.ToString("dd-MM-yyyy") + " al " + pperiodo.Fecha_Fin.ToString("dd-MM-yyyy"), _standardFont));

            PeridoPago.VerticalAlignment = Element.ALIGN_CENTER;
            PeridoPago.BorderColor = iTextSharp.text.BaseColor.WHITE;
            PeridoPago.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell DireccionEmpresa = new PdfPCell(new Paragraph(empresaF.Colonia + " " + empresaF.Calle + " " + empresaF.NoExt + " " + estado + " " + empresaF.Municipio + " " + empresaF.CP, _standardFont));
            DireccionEmpresa.VerticalAlignment = Element.ALIGN_CENTER;
            DireccionEmpresa.BorderColor = iTextSharp.text.BaseColor.WHITE;
            DireccionEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;


            PdfPCell RP = new PdfPCell(new Paragraph("Reg. Patronal : " + empresaF.RegistroPatronal, _standardFont));

            RP.VerticalAlignment = Element.ALIGN_CENTER;
            RP.BorderColor = iTextSharp.text.BaseColor.WHITE;
            RP.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell RFC = new PdfPCell(new Paragraph("RFC : " + empresaF.RFC, _standardFont));

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

            PdfPTable TablaAsistencia = new PdfPTable(1);
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



            PdfPCell Clave = new PdfPCell(new Paragraph("Clave: " + Convert.ToString(empleado.IdEmpleado), _standardFont));

            Clave.VerticalAlignment = Element.ALIGN_CENTER;
            Clave.BorderColor = BaseColor.WHITE;
            //Clave.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;

            Clave.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell NOmbres = new PdfPCell(new Paragraph(empleado.APaterno + " " + empleado.AMaterno + " " + empleado.Nombres, _standardFont));

            NOmbres.VerticalAlignment = Element.ALIGN_CENTER;
            NOmbres.BorderColor = iTextSharp.text.BaseColor.WHITE;
            NOmbres.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell RFCDato = new PdfPCell(new Paragraph("R.F.C.: " + empleado.RFC, _standardFont));

            RFCDato.VerticalAlignment = Element.ALIGN_CENTER;
            RFCDato.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //RFCDato.Border = Rectangle.TOP_BORDER;
            RFCDato.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell IMSS = new PdfPCell(new Paragraph("No. I.M.S.S: " + empleado.NSS, _standardFont));

            IMSS.VerticalAlignment = Element.ALIGN_CENTER;
            IMSS.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //IMSS.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
            IMSS.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell FechaIngreso = new PdfPCell(new Paragraph("Fecha Ingreso: " + empleadoC.FechaAlta.ToString("dd/MM/yyyy"), _standardFont));

            FechaIngreso.VerticalAlignment = Element.ALIGN_CENTER;
            FechaIngreso.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //FechaIngreso.Border = Rectangle.LEFT_BORDER;
            FechaIngreso.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell CURP = new PdfPCell(new Paragraph("Curp: " + empleado.CURP, _standardFont));

            CURP.VerticalAlignment = Element.ALIGN_CENTER;
            CURP.BorderColor = iTextSharp.text.BaseColor.WHITE;
            CURP.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Puesto = new PdfPCell(new Paragraph("Puesto: " + puesto.Descripcion, _standardFont));

            Puesto.VerticalAlignment = Element.ALIGN_CENTER;
            Puesto.BorderColor = iTextSharp.text.BaseColor.WHITE;
            Puesto.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Departamento = new PdfPCell(new Paragraph("Dpto: " + depa.Descripcion, _standardFont));
            Departamento.VerticalAlignment = Element.ALIGN_CENTER;
            Departamento.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //Departamento.Border = Rectangle.RIGHT_BORDER;
            Departamento.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell TipoSalario = new PdfPCell(new Paragraph("Tipo Salario: Fijo" , _standardFont));
            TipoSalario.VerticalAlignment = Element.ALIGN_CENTER;
            TipoSalario.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //TipoSalario.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
            TipoSalario.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell SD = new PdfPCell(new Paragraph("SD: " + empleadoC.SD, _standardFont));
            SD.VerticalAlignment = Element.ALIGN_CENTER;
            SD.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //SD.Border = Rectangle.BOTTOM_BORDER;
            SD.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell SDI = new PdfPCell(new Paragraph("SDI: " + empleadoC.SDI, _standardFont));
            SDI.VerticalAlignment = Element.ALIGN_CENTER;
            SDI.BorderColor = iTextSharp.text.BaseColor.WHITE;
            //SDI.Border = Rectangle.BOTTOM_BORDER;
            SDI.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell Blanco = new PdfPCell(new Paragraph("Tipo de Nomina: " + tiponomina.Descripcion, _standardFont));
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

            PdfPCell FaltasA = new PdfPCell(new Paragraph(".", _standardFont));
            FaltasA.VerticalAlignment = Element.ALIGN_LEFT;
            FaltasA.BorderColor = iTextSharp.text.BaseColor.WHITE;
            FaltasA.HorizontalAlignment = Element.ALIGN_LEFT;

            TablaAsistencia.AddCell(FaltasA);
            var CeldaAsis = new PdfPCell();
            CeldaAsis.AddElement(TablaAsistencia);
            TablaAsistenciaFinal.AddCell(CeldaAsis);

            doc.Add(TablaAsistenciaFinal);

            ///
            ///Detalle Percepcion Finiquito
            ///
            ReportesDAO reportes = new ReportesDAO();
            var DetalleFiniquito = reportes.detalleFiniquito(finiquito.IdFiniquito);

            if(finiquito.Subsidio_Finiquito != 0)
            {
                PdfPCell DescripcionS = new PdfPCell(new Paragraph("Subsidio Finiquito", _standardFont));
                DescripcionS.VerticalAlignment = Element.ALIGN_LEFT;
                DescripcionS.BorderColor = iTextSharp.text.BaseColor.WHITE;
                DescripcionS.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell CantidadS = new PdfPCell(new Paragraph("$" + finiquito.Subsidio_Finiquito, _standardFont));
                CantidadS.VerticalAlignment = Element.ALIGN_LEFT;
                CantidadS.BorderColor = iTextSharp.text.BaseColor.WHITE;
                CantidadS.HorizontalAlignment = Element.ALIGN_CENTER;

                TablaPercepciones.AddCell(DescripcionS);
                TablaPercepciones.AddCell(CantidadS);
            }
           

            foreach (var det in DetalleFiniquito.Where(x=>x.tipoConcpeto == 1))
            {
                PdfPCell Descripcion = new PdfPCell(new Paragraph(det.Detalle, _standardFont));
                Descripcion.VerticalAlignment = Element.ALIGN_LEFT;
                Descripcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Descripcion.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell Cantidad = new PdfPCell(new Paragraph("$" + det.Cantidad, _standardFont));
                Cantidad.VerticalAlignment = Element.ALIGN_LEFT;
                Cantidad.BorderColor = iTextSharp.text.BaseColor.WHITE;
                Cantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                TablaPercepciones.AddCell(Descripcion);
                TablaPercepciones.AddCell(Cantidad);

            }

            //Detalle Deduccion Finiquito
            if(finiquito.ISR_Finiquito != 0)
            {
                PdfPCell DescripcionD = new PdfPCell(new Paragraph("ISR FINIQUITO", _standardFont));
                DescripcionD.VerticalAlignment = Element.ALIGN_LEFT;
                DescripcionD.BorderColor = iTextSharp.text.BaseColor.WHITE;
                DescripcionD.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell CantidadD = new PdfPCell(new Paragraph("$" + finiquito.ISR_Finiquito, _standardFont));
                CantidadD.VerticalAlignment = Element.ALIGN_LEFT;
                CantidadD.BorderColor = iTextSharp.text.BaseColor.WHITE;
                CantidadD.HorizontalAlignment = Element.ALIGN_CENTER;

                TablaDeduccion.AddCell(DescripcionD);
                TablaDeduccion.AddCell(CantidadD);
            }

            if (finiquito.ISR_Liquidacion != 0)
            {
                PdfPCell DescripcionD = new PdfPCell(new Paragraph("ISR FINIQUITO", _standardFont));
                DescripcionD.VerticalAlignment = Element.ALIGN_LEFT;
                DescripcionD.BorderColor = iTextSharp.text.BaseColor.WHITE;
                DescripcionD.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell CantidadD = new PdfPCell(new Paragraph("$" + finiquito.ISR_Finiquito, _standardFont));
                CantidadD.VerticalAlignment = Element.ALIGN_LEFT;
                CantidadD.BorderColor = iTextSharp.text.BaseColor.WHITE;
                CantidadD.HorizontalAlignment = Element.ALIGN_CENTER;

                TablaDeduccion.AddCell(DescripcionD);
                TablaDeduccion.AddCell(CantidadD);
            }


            var otracelda = new PdfPCell();
            otracelda.AddElement(TablaPercepciones);
            TablaPercepcionesFinal.AddCell(otracelda);

            var otracelda2 = new PdfPCell();
            otracelda2.AddElement(TablaDeduccion);
            TablaPercepcionesFinal.AddCell(otracelda2);

            doc.Add(TablaPercepcionesFinal);

            ///
            ///Totales Finiquito Percepcion/Deduccion
            ///
            decimal total = finiquito.TotalPercepciones + finiquito.Subsidio_Finiquito +finiquito.Total_Liquidacion;

            PdfPCell TituloPercepcion = new PdfPCell(new Paragraph("Total Percepcion: ", _standardFont2));
            TituloPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
            TituloPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TituloPercepcion.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell TotalPercepcion = new PdfPCell(new Paragraph(Convert.ToString(total), _standardFont2));
            TotalPercepcion.VerticalAlignment = Element.ALIGN_LEFT;
            TotalPercepcion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TotalPercepcion.HorizontalAlignment = Element.ALIGN_CENTER;



            decimal deduccion = finiquito.TotalDeducciones + finiquito.ISR_Finiquito + finiquito.ISR_Liquidacion;
            PdfPCell TituloNet = new PdfPCell(new Paragraph("Neto Total: ", _standardFont2));
            TituloNet.VerticalAlignment = Element.ALIGN_LEFT;
            TituloNet.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TituloNet.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell NetTotal = new PdfPCell(new Paragraph(Convert.ToString(finiquito.Total_Finiquito), _standardFont2));
            NetTotal.VerticalAlignment = Element.ALIGN_LEFT;
            NetTotal.BorderColor = iTextSharp.text.BaseColor.WHITE;
            NetTotal.HorizontalAlignment = Element.ALIGN_CENTER;



            PdfPCell TituloDeduccion = new PdfPCell(new Paragraph("Total Deduccion: ", _standardFont2));
            TituloDeduccion.VerticalAlignment = Element.ALIGN_LEFT;
            TituloDeduccion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TituloDeduccion.HorizontalAlignment = Element.ALIGN_LEFT;


            PdfPCell TotalDeduccion = new PdfPCell(new Paragraph(Convert.ToString(deduccion), _standardFont2));
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

            doc.NewPage();


            /// Totales Y Obligaciones de la Lista de raya
            /// 
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



            if(finiquito.Subsidio_Finiquito != 0)
            {
                PdfPCell TituloPercepcion3 = new PdfPCell(new Paragraph("Subsidio Finiquito", _standardFont));
                TituloPercepcion3.VerticalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion3.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloPercepcion3.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion3.PaddingLeft = 10;
                PdfPCell TotalPercepcion3 = new PdfPCell(new Paragraph(Utils.TruncateDecimales(finiquito.Subsidio_Finiquito).ToString(), _standardFont));
                TotalPercepcion3.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPercepcion3.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPercepcion3.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalPercepcion3.PaddingRight = 10;
                TablaTotalPercepciones.AddCell(TituloPercepcion3);
                TablaTotalPercepciones.AddCell(TotalPercepcion3);
            }

            if (finiquito.Subsidio_Liquidacion != 0)
            {
                PdfPCell TituloPercepcion3 = new PdfPCell(new Paragraph("Subsidio Liquidacion", _standardFont));
                TituloPercepcion3.VerticalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion3.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloPercepcion3.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion3.PaddingLeft = 10;
                PdfPCell TotalPercepcion3 = new PdfPCell(new Paragraph(Utils.TruncateDecimales(finiquito.Subsidio_Liquidacion).ToString(), _standardFont));
                TotalPercepcion3.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPercepcion3.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPercepcion3.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalPercepcion3.PaddingRight = 10;
                TablaTotalPercepciones.AddCell(TituloPercepcion3);
                TablaTotalPercepciones.AddCell(TotalPercepcion3);
            }

            foreach (var totalTotales in DetalleFiniquito.Where(x => x.tipoConcpeto == 1))
            {
                PdfPCell TituloPercepcion2 = new PdfPCell(new Paragraph(totalTotales.Detalle, _standardFont));
                TituloPercepcion2.VerticalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloPercepcion2.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloPercepcion2.PaddingLeft = 10;
                PdfPCell TotalPercepcion2 = new PdfPCell(new Paragraph(Utils.TruncateDecimales(totalTotales.Cantidad).ToString(), _standardFont));
                TotalPercepcion2.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPercepcion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPercepcion2.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalPercepcion2.PaddingRight = 10;
                TablaTotalPercepciones.AddCell(TituloPercepcion2);
                TablaTotalPercepciones.AddCell(TotalPercepcion2);
                          
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
            PdfPCell TotalSumaPercepcion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(total).ToString(), _standardFont2));
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
            //  System.Diagnostics.Process.Start(newRuta + "/Autorizacion.pdf");

            if(finiquito.ISR_Finiquito != 0)
            {
                PdfPCell TituloDeduccion2 = new PdfPCell(new Paragraph("ISR Finiquito", _standardFont));
                TituloDeduccion2.VerticalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloDeduccion2.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion2.PaddingLeft = 10;
                PdfPCell TotalDeduccion2 = new PdfPCell(new Paragraph(Utils.TruncateDecimales(finiquito.ISR_Finiquito).ToString(), _standardFont));
                TotalDeduccion2.VerticalAlignment = Element.ALIGN_LEFT;
                TotalDeduccion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalDeduccion2.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalDeduccion2.PaddingRight = 10;
                TablaTotalDeducciones.AddCell(TituloDeduccion2);
                TablaTotalDeducciones.AddCell(TotalDeduccion2);
            }
            if (finiquito.ISR_Liquidacion != 0)
            {
                PdfPCell TituloDeduccion2 = new PdfPCell(new Paragraph("ISR Liquidacion", _standardFont));
                TituloDeduccion2.VerticalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TituloDeduccion2.HorizontalAlignment = Element.ALIGN_LEFT;
                TituloDeduccion2.PaddingLeft = 10;
                PdfPCell TotalDeduccion2 = new PdfPCell(new Paragraph(Utils.TruncateDecimales(finiquito.ISR_Liquidacion).ToString(), _standardFont));
                TotalDeduccion2.VerticalAlignment = Element.ALIGN_LEFT;
                TotalDeduccion2.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalDeduccion2.HorizontalAlignment = Element.ALIGN_RIGHT;
                TotalDeduccion2.PaddingRight = 10;
                TablaTotalDeducciones.AddCell(TituloDeduccion2);
                TablaTotalDeducciones.AddCell(TotalDeduccion2);
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
            PdfPCell SumaTotalDeduccion = new PdfPCell(new Paragraph(Utils.TruncateDecimales(deduccion).ToString(), _standardFont2));
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
            
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("           Total neto = " + Utils.TruncateDecimales(finiquito.TOTAL_total).ToString(), _standardFont3));

            /// Total Obligacion 
            /// 
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
            decimal SumaObligaciones = 0;

            PdfPCell TituloObligacion = new PdfPCell(new Paragraph("Fondo de Retiro", _standardFont));
            TituloObligacion.VerticalAlignment = Element.ALIGN_LEFT;
            TituloObligacion.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TituloObligacion.HorizontalAlignment = Element.ALIGN_LEFT;
            TituloObligacion.PaddingLeft = 10;
            PdfPCell TotalObligacion = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(datos?.FondoRetiro??0 ).ToString(), _standardFont));
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
            PdfPCell TotalObligacion2 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(datos?.Guarderia??0).ToString(), _standardFont));
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
            PdfPCell TotalObligacion3 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(datos?.IMSSEmpresa??0).ToString(), _standardFont));
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
            PdfPCell TotalObligacion4 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(datos?.RiesgoTrabajo??0).ToString(), _standardFont));
            TotalObligacion4.VerticalAlignment = Element.ALIGN_LEFT;
            TotalObligacion4.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TotalObligacion4.HorizontalAlignment = Element.ALIGN_RIGHT;
            TotalObligacion4.PaddingRight = 10;
            TablaTotalObligacion.AddCell(TituloObligacion4);
            TablaTotalObligacion.AddCell(TotalObligacion4);

            PdfPCell TituloObligacion5 = new PdfPCell(new Paragraph("Impuresto sobre Nomina", _standardFont));
            TituloObligacion5.VerticalAlignment = Element.ALIGN_LEFT;
            TituloObligacion5.BorderColor = iTextSharp.text.BaseColor.WHITE;
            TituloObligacion5.HorizontalAlignment = Element.ALIGN_LEFT;
            TituloObligacion5.PaddingLeft = 10;
            PdfPCell TotalObligacion5 = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(datos?.ImpuestoNomina??0).ToString(), _standardFont));
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

            if (datos != null)
            {
                SumaObligaciones = datos.FondoRetiro + datos.Guarderia + datos.ImpuestoNomina + datos.IMSSEmpresa +
                                   datos.RiesgoTrabajo;
            }
            else
            {
                SumaObligaciones = 0;
            }
            


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

           if(cuotas != null)
            {

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
                PdfPCell TotalCensantiaVejezP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.CesantiaVejez_Patron).ToString(), _standardFont));
                TotalCensantiaVejezP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalCensantiaVejezP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalCensantiaVejezP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalCensantiaVejezO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.CesantiaVejez_Obrero).ToString(), _standardFont));
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
                PdfPCell TotalPensionP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.Pensionados_Patron).ToString(), _standardFont));
                TotalPensionP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPensionP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPensionP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalPensionO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.Pensionados_Obrero).ToString(), _standardFont));
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
                PdfPCell TotalPretamoP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.PrestacionesDinero_Patron).ToString(), _standardFont));
                TotalPretamoP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalPretamoP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalPretamoP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalPretamoO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.PrestacionesDinero_Obrero).ToString(), _standardFont));
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
                PdfPCell TotalCuotaFijaP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.Cuota_Fija_Patron).ToString(), _standardFont));
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
                PdfPCell TotalExcedentesP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.Excedente_Patron).ToString(), _standardFont));
                TotalExcedentesP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalExcedentesP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalExcedentesP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalExcedentesO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.Excedente_Obrero).ToString(), _standardFont));
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
                PdfPCell TotalGuarderiasP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.GuarderiasPrestaciones_Patron).ToString(), _standardFont));
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
                PdfPCell TotalInfonavitP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.Infonavit_Patron).ToString(), _standardFont));
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
                PdfPCell TotalInvalidezVidaP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.InvalidezVida_Patron).ToString(), _standardFont));
                TotalInvalidezVidaP.VerticalAlignment = Element.ALIGN_LEFT;
                TotalInvalidezVidaP.BorderColor = iTextSharp.text.BaseColor.WHITE;
                TotalInvalidezVidaP.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell TotalInvalidezVidaO = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.InvalidezVida_Obrero).ToString(), _standardFont));
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
                PdfPCell TotalRetiroSARP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.SeguroRetiro_Patron).ToString(), _standardFont));
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

                PdfPCell TotalRiesgoTrabajoP = new PdfPCell(new Paragraph("$" + Utils.TruncateDecimales(cuotas.RiesgoTrabajo_Patron).ToString(), _standardFont));
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
                TotalRubrosPatron = cuotas.Cuota_Fija_Patron + cuotas.Excedente_Patron + cuotas.GuarderiasPrestaciones_Patron + cuotas.Infonavit_Patron + cuotas.InvalidezVida_Patron + cuotas.PrestacionesDinero_Patron + cuotas.Pensionados_Patron + cuotas.RiesgoTrabajo_Patron + cuotas.SeguroRetiro_Patron + cuotas.CesantiaVejez_Patron;
                TotalRubrosObrero = cuotas.CesantiaVejez_Obrero + cuotas.Excedente_Obrero + cuotas.InvalidezVida_Obrero + cuotas.PrestacionesDinero_Obrero + cuotas.Pensionados_Obrero;
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


            doc.Close();



            return newRuta;
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
}
