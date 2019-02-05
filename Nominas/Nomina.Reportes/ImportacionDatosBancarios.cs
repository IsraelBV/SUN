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
    public class ImportacionDatosBancarios
    {
        RHEntities ctx = null;

        public ImportacionDatosBancarios()
        {
            ctx = new RHEntities();
        }


        public string crearLayoutBancario(int idusuario, string ruta, int idSucursal)
        {
            var empleados = ctx.Empleado.Where(x => x.IdSucursal == idSucursal && x.Status == true).ToList();
            var contratos = ctx.Empleado_Contrato.Where(x => x.IdSucursal == idSucursal && x.Status == true).ToList();
            var bancos = ctx.C_Banco_SAT.ToList();
            var metodos = ctx.C_Metodos_Pago.ToList();
            var newruta = ValidarFolderUsuario(idusuario, ruta);
            newruta = newruta + "Datos_Bancarios.xlsx";
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Datos Bancarios");

            ws.Cell("A1").Value = "ID";
            ws.Cell("B1").Value = "Empleado";
            ws.Cell("C1").Value = "No Siga 1";
            ws.Cell("D1").Value = "No Siga 2";
            ws.Cell("E1").Value = "Cuenta Bancaria";
            ws.Cell("F1").Value = "Numero de Tarjeta";
            ws.Cell("G1").Value = "Clabe";
            ws.Cell("H1").Value = "Banco";
            ws.Cell("I1").Value = "Forma de Pago";

            int i = 2;
            foreach (var e in empleados)
            {
                var datosBancarios = ctx.DatosBancarios.Where(x => x.IdEmpleado == e.IdEmpleado).FirstOrDefault();
                var datosContrato = contratos.Where(x => x.IdEmpleado == e.IdEmpleado).FirstOrDefault();
                string nombreBanco;
                string nombreMetodo;
                if(datosBancarios != null)
                {
                     nombreBanco = bancos.Where(x => x.IdBanco == datosBancarios.IdBanco).Select(x => x.Descripcion).FirstOrDefault();
                }else
                {
                    nombreBanco = "Sin banco";
                }
                if(datosContrato != null)
                {
                    nombreMetodo = metodos.Where(x => x.IdMetodo == datosContrato.FormaPago).Select(x=>x.Descripcion).FirstOrDefault();
                }
                else
                {
                    nombreMetodo = "Sin meotodo de pago";
                }
                

                var numTar = datosBancarios != null ? datosBancarios.NumeroTarjeta : "0";
                numTar = numTar == "" || numTar == null ? "0" : numTar;

                var cuenta = datosBancarios != null ? datosBancarios.CuentaBancaria : "0";
                cuenta = cuenta == "" || cuenta == null? "0" : cuenta;
                ws.Cell("A" + i).Value = e.IdEmpleado;
                ws.Cell("B" + i).Value = e.Nombres + " " + e.APaterno + " " + e.AMaterno;
                ws.Cell("C" + i).Value = datosBancarios != null ? datosBancarios.NoSigaF : 0;
                ws.Cell("D" + i).Value = datosBancarios != null ? datosBancarios.NoSigaC : 0;
                ws.Cell("E" + i).DataType = XLCellValues.Text;
                ws.Cell("E" + i).Value = "'"+cuenta+"";
                ws.Cell("F" + i).DataType = XLCellValues.Text;
                ws.Cell("F" + i).Value ="'"+ numTar+"";
                ws.Cell("g" + i).DataType = XLCellValues.Text;
                ws.Cell("G" + i).Value = datosBancarios != null ? "'"+ datosBancarios.Clabe+"" : "0";
                ws.Cell("H" + i).Value = nombreBanco;
                ws.Cell("I" + i).Value = nombreMetodo;
                i++;
            }


            ws.Columns("1,1").AdjustToContents();
            ws.Columns("1,2").AdjustToContents();
            ws.Columns("1,3").AdjustToContents();
            ws.Columns("1,4").AdjustToContents();
            ws.Columns("1,5").AdjustToContents();
            ws.Columns("1,6").AdjustToContents();
            ws.Columns("1,7").AdjustToContents();
            ws.Columns("1,8").AdjustToContents();
            ws.Columns("1,9").AdjustToContents();
            

            wb.SaveAs(newruta, false);

            //Guardar como, y aqui ponemos la ruta de nuestro archivo

            return newruta;
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

        public DataTable ExcelToDataTable(HttpPostedFileBase fileBase)
        {
            Stream stream = fileBase.InputStream;
            IExcelDataReader reader = null;
            if (fileBase.FileName.EndsWith(".xls"))
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            reader.IsFirstRowAsColumnNames = true;
            var result = reader.AsDataSet();
            reader.Close();
            reader.Dispose();
            reader = null;
            return result.Tables[0];

        }

        public bool guardarRegistro(DataTable data)
        {
            var resultado = true;
            try
            {
                var bancos = ctx.C_Banco_SAT.ToList();
                var metodos = ctx.C_Metodos_Pago.ToList();

                int columnas = data.Columns.Count;
                List<DataColumn> drlist = new List<DataColumn>();
                foreach (DataColumn column in data.Columns)
                {
                    drlist.Add((DataColumn)column);
                }
                List<EmpleadoBanco> listEmpleado = new List<EmpleadoBanco>();
                foreach (DataRow row in data.Rows)
                {
                    EmpleadoBanco emp = new EmpleadoBanco();
                    emp.IdEmpleado = Convert.ToInt32(row[0]);
                    emp.NoSiga = row[2] == DBNull.Value  ? 0: Convert.ToInt32(row[2]);
                    emp.NoSIga2 = row[3] == DBNull.Value ? 0 : Convert.ToInt32(row[3]);
                    emp.cuenta = row[4].ToString();
                    emp.Tarjeta = row[5].ToString();
                    emp.Clabe = row[6].ToString();
                    emp.Banco =row[7].ToString().ToUpper();
                    emp.MetodoPago = row[8].ToString();
                    listEmpleado.Add(emp);
                }


                foreach(var list in listEmpleado)
                {
                    var auxmetodos = metodos.Where(x => x.Descripcion.ToUpper() == list.MetodoPago.ToUpper()).FirstOrDefault();
                    var aux = bancos.Where(x => x.Descripcion == list.Banco).FirstOrDefault();
                    const string sqlQuery = "DELETE DatosBancarios WHERE IdEmpleado = @p0";
                    ctx.Database.ExecuteSqlCommand(sqlQuery, list.IdEmpleado);
             
                        var item = new DatosBancarios
                        {
                            IdBanco = aux == null? 2: aux.IdBanco,
                            IdEmpleado = list.IdEmpleado,
                            NoSigaF = list.NoSiga,
                            NoSigaC = list.NoSIga2,
                            CuentaBancaria = list.cuenta,
                            NumeroTarjeta = list.Tarjeta,
                            Clabe = list.Clabe,
                            Status = true
                            
                        };
                        ctx.DatosBancarios.Add(item);
                        var r = ctx.SaveChanges();
                        if (r > 0)
                            resultado = true;
                    
                    var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == list.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();

                    contrato.FormaPago = auxmetodos == null ? 3: auxmetodos.IdMetodo;
                    ctx.SaveChanges();
              
                    
                }
                return resultado;
            }
            catch(Exception e)
            {
                return resultado;
            }

        }
    }
    public class EmpleadoBanco
    {
        public int IdBanco { get; set; }
        public int IdEmpleado { get; set; }
        public string Banco { get; set; }
        public int? NoSiga { get; set; }
        public int? NoSIga2 { get; set; }
        public string cuenta { get; set; }
        public string Tarjeta { get; set; }
        public string Clabe { get; set; }
        public string MetodoPago { get; set; }
        public int IdMetodoPago { get; set; }
    }
}
