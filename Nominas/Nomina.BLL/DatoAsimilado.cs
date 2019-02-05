using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

using RH.Entidades;
using System.Data;

namespace Nomina.BLL
{
  public  class DatoAsimilado
    {

        public byte[] CrearLayoutAsimilado(int idPeriodoPago)
        {
            //Guarda el archivo en la memoria
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            //Crea los Header del layout
            worksheet.Cell(1, 1).Value = "Clave";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "Cantidad Asimilado";
            //Establece un estilo al header
            worksheet.Range("A1:C1").Style
            .Font.SetFontSize(13)
            .Font.SetBold(true)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.Orange);



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
  



            workbook.SaveAs(ms, false);
            return ms.ToArray();


        }
        public void ImportarDatosAsimilado(DataTable dataT, int idPeriodo)
        {
            if (dataT == null || idPeriodo <= 0) return;

          
            List<NOM_Empleado_Asimilado> listaAsimilados = new List<NOM_Empleado_Asimilado>();
            int cont = 0;
            int idEmpleado = 0;
      
            decimal cantidad = 0;
            int idConcepto = 0;

            var idEmpArray = GetIdEmpleadosByIdPeriodo(idPeriodo);

            //int[] s = { 1, 2, 3, 3, 4 };
            //int[] q = s.Distinct().ToArray();

            foreach (DataRow row in dataT.Rows)
            {
                cantidad = 0;

                if (row[0].ToString().Trim() == "") continue;

                idEmpleado = int.Parse(row[0].ToString());

                if (row[2].ToString().Trim() == "") continue;

                cantidad = decimal.Parse(row[2].ToString());

                if (cantidad <= 0) continue;

         

                //Buscamos que el idEmpleado este en el array
                if (!BuscarInArray(idEmpArray, idEmpleado)) continue;

                NOM_Empleado_Asimilado itemAsimilados = new NOM_Empleado_Asimilado()
                {
                    IdEmpleadoAsimilado = 0,
                    IdPeriodo = idPeriodo,
                    IdEmpleado = idEmpleado,
                    Cantidad = cantidad
                };

                listaAsimilados.Add(itemAsimilados);
                cont++;
            }

            //Borrar anterior
            int[] arrayIdEmpleados = listaAsimilados.Select(x => x.IdEmpleado).ToArray();

            BorrarDatoAnterior(arrayIdEmpleados, idPeriodo);


            //Agregar el dato nuevo
            InsertarRegistros(listaAsimilados);

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
        private void BorrarDatoAnterior(int[] idEmpleados, int idPeriodo)
        {

            if (idEmpleados == null) return;

            if (idEmpleados.Length <= 0) return;

            //eliminar de nominas
            var idsE = string.Join(",", idEmpleados);

            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_Empleado_Asimilado] WHERE IdEmpleado in  (" + idsE + ") and IdPeriodo = @p0";
                context.Database.ExecuteSqlCommand(sqlQuery1, idPeriodo);
            }
        }
        private void InsertarRegistros(List<NOM_Empleado_Asimilado> lista)
        {
            if (lista.Count > 0)
            {
                using (var context = new RHEntities())
                {
                    context.NOM_Empleado_Asimilado.AddRange(lista);
                    context.SaveChanges();
                }

            }
        }
        public List<ModeloAsimilado> GetDatosAsimilados(int idPeriodo)
        {
            if (idPeriodo <= 0) return null;

            using (var context = new RHEntities())
            {
                var lista = (from ec in context.NOM_Empleado_Asimilado
                             join emp in context.Empleado on ec.IdEmpleado equals emp.IdEmpleado
                             where ec.IdPeriodo == idPeriodo
                             select new ModeloAsimilado
                             {
                                 IdEmpleadoASimilado = ec.IdEmpleadoAsimilado,
                                 IdEmpleado = ec.IdEmpleado,
                                 Paterno = emp.APaterno,
                                 Materno = emp.AMaterno,
                                 Nombres = emp.Nombres,
              
                                 Cantidad = ec.Cantidad
                             }).ToList();

                if (lista.Count > 0)
                {
                    return lista.OrderBy(x => x.Paterno).ToList();
                }
            }

            return null;
        }
    }

    public class ModeloAsimilado
    {
        public int IdEmpleadoASimilado { get; set; }
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public decimal Cantidad { get; set; }
    }
}
