using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using RH.Entidades;
using System.Data;
using Common.Utils;

namespace Nomina.BLL
{
    public class DatoAjustesNomina
    {
        public byte[] CrearLayoutAjuste(int idPeriodoPago)
        {
            //Guarda el archivo en la memoria
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Ajustes");
            //Crea los Header del layout
            worksheet.Cell(1, 1).Value = "Clave";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "IdConcepto";
            worksheet.Cell(1, 4).Value = "Total";
            worksheet.Cell(1, 5).Value = "GravadoISR";
            worksheet.Cell(1, 6).Value = "ExentoISR";
            worksheet.Cell(1, 7).Value = "IntegraImss";
            worksheet.Cell(1, 8).Value = "Impuesto Sobre Nomina";

            //Establece un estilo al header
            worksheet.Range("A1:H1").Style
            .Font.SetFontSize(13)
            .Font.SetBold(true)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.Awesome);



            //Obtiene los datos del empleado
            var ids = GetIdEmpleadosByIdPeriodo(idPeriodoPago);

            if (ids == null) return null;

            var datosEmpleados = GetEmpleadosById(ids);

            if (datosEmpleados == null) return null;

            //Agrega los datos a la hoja
            int row = 2;
            int col = 1;

            foreach (var item in datosEmpleados)
            {
                StringBuilder strb = new StringBuilder();
                strb.Append(item.APaterno);
                strb.Append(" ");
                strb.Append(item.AMaterno);
                strb.Append(" ");
                strb.Append(item.Nombres);
                var colaborador = strb.ToString();

                worksheet.Cell(row, col).Value = item.IdEmpleado;
                worksheet.Cell(row, col + 1).Value = colaborador;
                row++;
                col = 1;
            }

            //Agregar validacion a la columna de la cantidad

            //Dar formato adicional al layout
            //Ajustar el header al contenido
            worksheet.Column(1).AdjustToContents();
            worksheet.Column(2).AdjustToContents();
            worksheet.Column(3).AdjustToContents();
            worksheet.Column(4).AdjustToContents();
            worksheet.Column(5).AdjustToContents();
            worksheet.Column(6).AdjustToContents();
            worksheet.Column(7).AdjustToContents();
            worksheet.Column(8).AdjustToContents();




            workbook.SaveAs(ms, false);
            return ms.ToArray();


        }

        public void ImportarDatosAjustes(DataTable dataT, int idPeriodo)
        {
            if (dataT == null || idPeriodo <= 0) return;


            List<NOM_Nomina_Ajuste> listaDeAjustes = new List<NOM_Nomina_Ajuste>();
            int cont = 0;
            int idEmpleado = 0;
            decimal total = 0;
            int idConcepto = 0;
            decimal gravado = 0;
            decimal excento = 0;
            decimal integraimss = 0;
            decimal impuestosn = 0;

            var idEmpArray = GetIdEmpleadosByIdPeriodo(idPeriodo);

            //int[] s = { 1, 2, 3, 3, 4 };
            //int[] q = s.Distinct().ToArray();

            foreach (DataRow row in dataT.Rows)
            {
                idEmpleado = 0;
                total = 0;
                idConcepto = 0;
                gravado = 0;
                excento = 0;
                integraimss = 0;
                impuestosn = 0;

                if (row[0].ToString().Trim() == "") continue;

                idEmpleado = int.Parse(row[0].ToString());

                if (idEmpleado <= 0) continue;

                if (row[2].ToString().Trim() == "") continue;

                idConcepto = int.Parse(row[2].ToString());

                if (idConcepto <= 0) continue;

                total = decimal.Parse(row[3].ToString());
                gravado = decimal.Parse(row[4].ToString());
                excento = decimal.Parse(row[5].ToString());
                integraimss = decimal.Parse(row[6].ToString());
                impuestosn = decimal.Parse(row[7].ToString());
                
                //Buscamos que el idEmpleado este en el array
                if (!BuscarInArray(idEmpArray, idEmpleado)) continue;

                NOM_Nomina_Ajuste itemAsimilados = new NOM_Nomina_Ajuste()
                {
                    IdAjuste = 0,
                    IdPeriodo = idPeriodo,
                    IdEmpleado = idEmpleado,
                    IdConcepto = idConcepto,
                    Total = total,
                    GravadoIsr = gravado,
                    ExentoIsr = excento,
                    IntegraImss = integraimss,
                    ImpuestoSobreNomina = impuestosn
                };

                listaDeAjustes.Add(itemAsimilados);
                cont++;
            }

            //Borrar anterior
            int[] arrayIdEmpleados = listaDeAjustes.Select(x => x.IdEmpleado).ToArray();

            BorrarDatoAnteriorDeAjuste(arrayIdEmpleados, idPeriodo);


            //Agregar el dato nuevo
            InsertarRegistrosDeAjuste(listaDeAjustes);

        }

        private int[] GetIdEmpleadosByIdPeriodo(int idPeriodoPago)
        {
            using (var context = new RHEntities())
            {
                var lista = (from t in context.NOM_Empleado_PeriodoPago
                             where t.IdPeriodoPago == idPeriodoPago
                             select t.IdEmpleado).ToArray();
                return lista;
            }
        }

        private List<Empleado> GetEmpleadosById(int[] IdEmpleadosArray)
        {
            if (IdEmpleadosArray == null) return null;

            using (var context = new RHEntities())
            {
                var lista = (from t in context.Empleado
                             where IdEmpleadosArray.Contains(t.IdEmpleado)
                             select t).ToList();

                var nuevaLista = lista.OrderBy(x => x.APaterno).ToList();
                return nuevaLista;
            }
        }

        public List<ModeloAjuste> GetDatosAjuste(int idPeriodo)
        {
            if (idPeriodo <= 0) return null;

            using (var context = new RHEntities())
            {
                var lista = (from na in context.NOM_Nomina_Ajuste
                             join emp in context.Empleado on na.IdEmpleado equals emp.IdEmpleado
                             join con in context.C_NOM_Conceptos on na.IdConcepto equals con.IdConcepto
                             where na.IdPeriodo == idPeriodo
                             select new ModeloAjuste
                             {
                                 IdAjuste = na.IdAjuste,
                                 IdPeriodo = na.IdPeriodo,
                                 IdEmpleado = na.IdEmpleado,
                                 Paterno = emp.APaterno,
                                 Materno = emp.AMaterno,
                                 Nombres = emp.Nombres,
                                 IdConcepto = na.IdConcepto,
                                 Concepto = con.DescripcionCorta,
                                 Total = na.Total,
                                 Gravado = na.GravadoIsr,
                                 Exento = na.ExentoIsr,
                                 IntegraImss = na.IntegraImss,
                                 Isn = na.ImpuestoSobreNomina
                             }).ToList();

                if (lista.Count > 0)
                {
                    return lista.OrderBy(x => x.Paterno).ToList();
                }
            }

            return null;
        }

        public bool BuscarInArray(int[] array, int elemento)
        {
            bool result = false;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == elemento)
                {
                    result = true;
                }
            }

            return result;
        }

        private void BorrarDatoAnteriorDeAjuste(int[] idEmpleados, int idPeriodo)
        {

            if (idEmpleados == null) return;
            if (idEmpleados.Length <= 0) return;
            //eliminar de nominas
            var idsE = string.Join(",", idEmpleados);
            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_Nomina_Ajuste] WHERE IdEmpleado in  (" + idsE + ") and IdPeriodo = @p0";
                context.Database.ExecuteSqlCommand(sqlQuery1, idPeriodo);
            }
        }
        private void InsertarRegistrosDeAjuste(List<NOM_Nomina_Ajuste> lista)
        {
            if (lista.Count > 0)
            {
                using (var context = new RHEntities())
                {
                    context.NOM_Nomina_Ajuste.AddRange(lista);
                    context.SaveChanges();
                }

            }
        }

        public void EliminarAjustes(int[] arrayAjuste)
        {
            if( arrayAjuste == null ) return;

            if(arrayAjuste.Length <= 0) return;

            var ids = string.Join(",", arrayAjuste);

            using (var context = new RHEntities())
            {
                 var sqlQuery = "DELETE [NOM_Nomina_Ajuste] WHERE [IdAjuste] in  (" + ids + ")";
                context.Database.ExecuteSqlCommand(sqlQuery);
            }

        }

    }

    public class ModeloAjuste
    {
        public int IdAjuste { get; set; }
        public int IdPeriodo { get; set; }
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int IdConcepto { get; set; }
        public string Concepto { get; set; }
        public decimal Total { get; set; }
        public decimal Gravado { get; set; }
        public decimal Exento { get; set; }
        public decimal IntegraImss { get; set; }
        public decimal Isn { get; set; }
    }
}
