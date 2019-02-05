using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using System.Data;
using Common.Utils;

namespace Nomina.BLL
{
    public class DatoComplemento
    {

        public byte[] CrearLayoutComplemento(int idPeriodoPago)
        {
            //Guarda el archivo en la memoria
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            //Crea los Header del layout
            worksheet.Cell(1, 1).Value = "Clave";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "Descripcion";
            worksheet.Cell(1, 4).Value = "Cantidad";

            //Establece un estilo al header
            worksheet.Range("A1:D1").Style
            .Font.SetFontSize(13)
            .Font.SetBold(true)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.Black);


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



            workbook.SaveAs(ms, false);
            return ms.ToArray();


        }

        public void ImportarDatosComplemento(DataTable dataT, int idPeriodo)
        {
            if (dataT == null || idPeriodo <= 0) return;

            //Lista de item de complemento
            List<NOM_Empleado_Complemento> listaComplemento = new List<NOM_Empleado_Complemento>();
            int cont = 0;
            int idEmpleado = 0;
            string descripcion = "";
            decimal cantidad = 0;
            int idConcepto = 0;

            var idEmpArray = GetIdEmpleadosByIdPeriodo(idPeriodo);

            //int[] s = { 1, 2, 3, 3, 4 };
            //int[] q = s.Distinct().ToArray();

            foreach (DataRow row in dataT.Rows)
            {
                cantidad = -1;
                descripcion = "";

                if (row[0].ToString().Trim() == "") continue;

                idEmpleado = int.Parse(row[0].ToString());

                if (row[3].ToString().Trim() == "") continue;

                cantidad = decimal.Parse(row[3].ToString());

                //  if (cantidad < 0) continue;

                descripcion = row[2].ToString().Trim();

                if (descripcion == "")
                {
                    descripcion = "Complemento";
                }

                //Buscamos que el idEmpleado este en el array
                if (!BuscarInArray(idEmpArray, idEmpleado)) continue;

                NOM_Empleado_Complemento itemComplemento = new NOM_Empleado_Complemento()
                {
                    IdEmpleadoComplemento = 0,
                    IdPeriodo = idPeriodo,
                    IdEmpleado = idEmpleado,
                    Cantidad = cantidad,
                    IdConcepto = idConcepto,
                    Descripcion = descripcion
                };

                listaComplemento.Add(itemComplemento);
                cont++;
            }

            //Borrar anterior
            var arrayIDEmpleados = listaComplemento.Select(x => x.IdEmpleado).ToArray();
            BorrarDatoAnterior(arrayIDEmpleados, idPeriodo);


            //Agregar el dato nuevo
            InsertarRegistros(listaComplemento);

        }

        private void BorrarDatoAnterior(int[] idEmpleados, int idPeriodo)
        {

            if (idEmpleados == null) return;

            if (idEmpleados.Length <= 0) return;

            //eliminar de nominas
            var idsE = string.Join(",", idEmpleados);

            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_Empleado_Complemento] WHERE IdEmpleado in  (" + idsE + ") and IdPeriodo = @p0";
                context.Database.ExecuteSqlCommand(sqlQuery1, idPeriodo);
            }
        }

        private void InsertarRegistros(List<NOM_Empleado_Complemento> lista)
        {
            if (lista.Count > 0)
            {
                using (var context = new RHEntities())
                {
                    context.NOM_Empleado_Complemento.AddRange(lista);
                    context.SaveChanges();
                }

            }
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

        public List<DatosDeComplemento> GetDatosComplementoByIdPeriodo(int idPeriodo)
        {
            if (idPeriodo <= 0) return null;

            using (var context = new RHEntities())
            {
                var listaComplemento = (from ec in context.NOM_Empleado_Complemento
                                        join emp in context.Empleado on ec.IdEmpleado equals emp.IdEmpleado
                                        where ec.IdPeriodo == idPeriodo
                                        select new DatosDeComplemento
                                        {
                                            IdEmpleadoComplemento = ec.IdEmpleadoComplemento,
                                            IdEmpleado = ec.IdEmpleado,
                                            Paterno = emp.APaterno,
                                            Materno = emp.AMaterno,
                                            Nombres = emp.Nombres,
                                            Descripcion = ec.Descripcion,
                                            Cantidad = ec.Cantidad,
                                            IdNomina = 0
                                        }).ToList();

                //Revisamos si el periodo ya fue procesado y tiene registros de nomina
                var listaNominas = (from nom in context.NOM_Nomina
                                    where nom.IdPeriodo == idPeriodo
                                    select nom).ToList();

                if (listaNominas.Count > 0 && listaComplemento.Count > 0)
                {
                    foreach (var item in listaComplemento)
                    {
                        //Buscamos el empleado en las nominas
                        var itemNom = listaNominas.FirstOrDefault(x => x.IdEmpleado == item.IdEmpleado);

                        //
                        if (itemNom == null) continue;
                        item.Procesado = itemNom.TotalComplemento;
                        item.IdNomina = itemNom.IdNomina;
                    }
                }


                if (listaComplemento.Count > 0)
                {
                    return listaComplemento.OrderBy(x => x.Paterno).ToList();
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


        public byte[] CrearLayoutDetalleC(int idPeriodoPago)
        {
            //Guarda el archivo en la memoria
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            //Crea los Header del layout
            worksheet.Cell(2, 1).Value = "Nomina";
            worksheet.Cell(2, 2).Value = "ID";
            worksheet.Cell(2, 3).Value = "Nombre";
            worksheet.Cell(2, 4).Value = "Total $";

            //Conceptos complemento
            var listaConceptos = GetConceptos();

            int indx = 5;

            if (listaConceptos != null)
            {
                foreach (var itemConcepto in listaConceptos)
                {
                    worksheet.Cell(1, indx).Value = itemConcepto.IdConceptoC;
                    worksheet.Cell(2, indx).Value = itemConcepto.Descripcion;
                    indx++;
                }
            }
            else
            {
                worksheet.Cell(1, 5).Value = "No se econtraron conceptos de complemento";
                worksheet.Cell(2, 5).Value = "configurado en la bd.";
            }



            //Establece un estilo al header
            worksheet.Range("A2:D2").Style
            .Font.SetFontSize(13)
            .Font.SetBold(true)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.Black);



            //Obtiene los datos del empleado
            var ids = GetIdEmpleadosByIdPeriodo(idPeriodoPago);

            if (ids == null) return null;

            var datosEmpleados = GetEmpleadosDetalle(idPeriodoPago);

            if (datosEmpleados == null) return null;

            //Agrega los datos a la hoja
            int row = 3;
            int col = 1;

            foreach (var item in datosEmpleados)
            {
                StringBuilder strb = new StringBuilder();
                strb.Append(item.Paterno);
                strb.Append(" ");
                strb.Append(item.Materno);
                strb.Append(" ");
                strb.Append(item.Nombres);
                var colaborador = strb.ToString();

                worksheet.Cell(row, col).Value = item.IdNomina;
                worksheet.Cell(row, col + 1).Value = item.IdEmpleado;
                worksheet.Cell(row, col + 2).Value = colaborador;
                worksheet.Cell(row, col + 3).Value = item.TotalC.ToCurrencyFormat();

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



            workbook.SaveAs(ms, false);
            return ms.ToArray();


        }

        public byte[] LayoutDetalleComplemento(int idPeriodoPago)
        {
            //Guarda el archivo en la memoria
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Crea el libro y la hoja para el Layout
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PLANTILLA");
            worksheet.SheetView.Freeze(3, 2);
            var listaConceptos = GetConceptos();

            //Obtiene los datos del empleado
            var ids = GetIdEmpleadosByIdPeriodo(idPeriodoPago);
            if (ids == null) return null;

            var datosEmpleados = GetEmpleadosDetalle(idPeriodoPago);

            if (datosEmpleados == null) return null;


            //Header
            worksheet.Cell(3, 1).Value = "ID";
            worksheet.Cell(3, 2).Value = "Nombre";



            int col = 3;
            foreach (var itemConcepto in listaConceptos)
            {
                worksheet.Cell(1, col).Value = itemConcepto.IdConceptoC;
                worksheet.Cell(2, col).Value = itemConcepto.Descripcion;
                worksheet.Cell(3, col).Value = itemConcepto.DescripcionTipo;
                col++;
            }

            //Empleados
            int fila = 4;
            foreach (var itemEmpleado in datosEmpleados)
            {
                worksheet.Cell(fila, 1).Value = itemEmpleado.IdEmpleado;
                string nombre = itemEmpleado.Paterno + " " + itemEmpleado.Materno + " " + itemEmpleado.Nombres;
                worksheet.Cell(fila, 2).Value = nombre;
                fila++;
            }


            //SET COLOR 
            worksheet.Range("A3:B3").Style
          .Font.SetFontSize(13)
          .Font.SetBold(true)
          .Font.SetFontColor(XLColor.White)
          .Fill.SetBackgroundColor(XLColor.Black);

            //   worksheet.Range("C3:D3").Style
            //.Font.SetFontSize(13)
            //.Font.SetBold(true)
            //.Font.SetFontColor(XLColor.White)
            //.Fill.SetBackgroundColor(XLColor.DarkBlue);

            //Color ROSA
            worksheet.Range("C2:AG2").Style
      .Font.SetFontSize(13)
      .Font.SetBold(true)
      .Font.SetFontColor(XLColor.White)
      .Fill.SetBackgroundColor(XLColor.Raspberry);

            worksheet.Cell("AH2").Style
      .Font.SetFontSize(13)
      .Font.SetBold(true)
      .Font.SetFontColor(XLColor.White)
      .Fill.SetBackgroundColor(XLColor.DarkBlue);

            worksheet.Range("AI2:AU2").Style
.Font.SetFontSize(13)
.Font.SetBold(true)
.Font.SetFontColor(XLColor.Black)
.Fill.SetBackgroundColor(XLColor.Amber);

            worksheet.Range("AV2:AW2").Style
.Font.SetFontSize(13)
.Font.SetBold(true)
.Font.SetFontColor(XLColor.White)
.Fill.SetBackgroundColor(XLColor.DarkBlue);

            //SET Texto en Negrita
            worksheet.Range("C3:AW3").Style
                .Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.Black);

            //Alineacion centradalizada del contenido de las celdas
            worksheet.Range("A2:AW2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:AW1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A3:AW3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            //AJUSTE DE 
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
            worksheet.Column(28).AdjustToContents();
            worksheet.Column(29).AdjustToContents();
            worksheet.Column(30).AdjustToContents();
            worksheet.Column(31).AdjustToContents();
            worksheet.Column(32).AdjustToContents();
            worksheet.Column(33).AdjustToContents();
            worksheet.Column(34).AdjustToContents();
            worksheet.Column(35).AdjustToContents();
            worksheet.Column(36).AdjustToContents();
            worksheet.Column(37).AdjustToContents();
            worksheet.Column(38).AdjustToContents();
            worksheet.Column(39).AdjustToContents();
            worksheet.Column(40).AdjustToContents();
            worksheet.Column(41).AdjustToContents();
            worksheet.Column(42).AdjustToContents();
            worksheet.Column(43).AdjustToContents();
            worksheet.Column(44).AdjustToContents();
            worksheet.Column(45).AdjustToContents();
            worksheet.Column(46).AdjustToContents();
            worksheet.Column(47).AdjustToContents();
            worksheet.Column(48).AdjustToContents();
            worksheet.Column(49).AdjustToContents();



            workbook.SaveAs(ms, false);
            return ms.ToArray();
        }

        public void ImportarDetalleComplemento(DataTable dataT, int idPeriodo)
        {
            if (dataT == null || idPeriodo <= 0) return;

            //GET -  lista de conceptos
            DataRow rowIdConcepto = dataT.Rows[0];

            //lista  detalle
            List<NOM_Nomina_Detalle_C> listaDetalle = new List<NOM_Nomina_Detalle_C>();

            var idEmpArray = GetIdEmpleadosByIdPeriodo(idPeriodo);

            //for a datable
            int rows = 0;
            foreach (DataRow itemRow in dataT.Rows)
            {
                if (rows < 3)
                {
                    rows++;
                    continue;
                }

                int idEmpleado = 0;

                int.TryParse(itemRow[0].ToString(), out idEmpleado);
                //int.TryParse(itemRow[1].ToString(), out idEmpleado);

                if (idEmpleado <= 0) continue;

                for (int j = 2; j < 49; j++)
                {
                    var idConcepto = 0;
                    int.TryParse(rowIdConcepto[j].ToString(), out idConcepto);
                    decimal cantidad = 0;
                    decimal.TryParse(itemRow[j].ToString(), out cantidad);

                    if (idEmpleado <= 0 || cantidad <= 0 || idConcepto <= 0) continue;
                    NOM_Nomina_Detalle_C itemDetalleC = new NOM_Nomina_Detalle_C()
                    {
                        Id = 0,
                        IdPeriodo = idPeriodo,
                        IdEmpleado = idEmpleado,
                        IdNomina = 0,
                        IdConcepto = idConcepto,
                        Cantidad = cantidad
                    };
                    listaDetalle.Add(itemDetalleC);
                }

                rows++;
            }


            //lista de idNominas
            var arrayPeriodos = listaDetalle.Select(x => x.IdPeriodo).ToArray();
            //Borrar datos anterior
            BorrarDetallesAnterior(arrayPeriodos);

            //Guardar nuevos datos
            InsertarRegistrosDetalles(listaDetalle);

        }

        private static IEnumerable<C_NOM_Conceptos_C> GetConceptos()
        {
            try
            {
                using (var context = new RHEntities())
                {
                    return context.C_NOM_Conceptos_C.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        private List<DatosEmpleadoDetalle> GetEmpleadosDetalle(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista = (from n in context.NOM_Nomina
                             join e in context.Empleado on n.IdEmpleado equals e.IdEmpleado
                            // join ec in context.NOM_Empleado_Complemento on n.IdEmpleado equals ec.IdEmpleado
                             where n.IdPeriodo == idPeriodo //&& ec.IdPeriodo == idPeriodo
                             //&& n.TotalComplemento > 0 // Por peticion de maricela debe incluir a todos los del periodo -
                             select new DatosEmpleadoDetalle()
                             {
                                 IdNomina = n.IdNomina,
                                 IdEmpleado = e.IdEmpleado,
                                 Paterno = e.APaterno,
                                 Materno = e.AMaterno,
                                 Nombres = e.Nombres,
                                 TotalC = 0//ec.Cantidad // n.TotalComplemento
                             }).ToList();
                return lista;
            }
        }

        private void BorrarDetallesAnterior(int[] idPeriodos)
        {

            if (idPeriodos == null) return;

            if (idPeriodos.Length <= 0) return;

            //eliminar de nominas
            var idsE = string.Join(",", idPeriodos);

            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_Nomina_Detalle_C] WHERE [IdPeriodo] in  (" + idsE + ") ";
                context.Database.ExecuteSqlCommand(sqlQuery1);
            }
        }


        private void InsertarRegistrosDetalles(List<NOM_Nomina_Detalle_C> lista)
        {
            if (lista.Count > 0)
            {
                using (var context = new RHEntities())
                {
                    context.NOM_Nomina_Detalle_C.AddRange(lista);
                    context.SaveChanges();
                }

            }
        }
    }

    public class DatosDeComplemento
    {
        public int IdEmpleadoComplemento { get; set; }
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Procesado { get; set; }
        public int IdNomina { get; set; }
    }

    public class DatosEmpleadoDetalle
    {
        public int IdNomina { get; set; }
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public decimal TotalC { get; set; }
    }
}
