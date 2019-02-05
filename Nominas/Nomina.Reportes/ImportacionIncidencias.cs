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
    public class ImportacionIncidencias
    {
        RHEntities ctx = null;

        public ImportacionIncidencias()
        {
            ctx = new RHEntities();
        }


        public string crearImportacionIncidencias(int idusuario, string ruta,int idPeriodo, List<EmpleadoIncidencias> incidencias)
        {
            
            var periodo = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).FirstOrDefault();

            var empleado = (from peremp in ctx.NOM_Empleado_PeriodoPago
                            join per in ctx.NOM_PeriodosPago  
                            on peremp.IdPeriodoPago equals per.IdPeriodoPago
                            join emp in ctx.Empleado
                            on peremp.IdEmpleado equals emp.IdEmpleado
                            where per.IdPeriodoPago == idPeriodo
                            select new ImportacionEmpleados
                            {
                                IdEmpledo = emp.IdEmpleado,
                                Nombres = emp.Nombres,
                                Paterno = emp.APaterno,
                                Materno = emp.AMaterno
                            }).ToList();
            int i = 3;
            int j = 2;
            
            var newruta = ValidarFolderUsuario(idusuario, ruta);
            newruta = newruta + "Incidencia del "+periodo.Descripcion +".xlsx";
            var wb = new XLWorkbook();
             var ws = wb.Worksheets.Add("Inasistencia");
            var fecha = periodo.Fecha_Inicio;
            var fecha2 = periodo.Fecha_Inicio;


            while (fecha <= periodo.Fecha_Fin)
            {
                
                ws.Cell(1, i).Value = fecha.ToString("dd/MM/yyyy");
                ws.Columns("1," + i).Width = 10;

                fecha = fecha.AddDays(1);
                i++;
            }
            int a = 2;
            foreach (var emp in incidencias){
                
                ws.Cell("A" + j).Value = emp.IdEmpleado;
                    ws.Cell("B" + j).Value = emp.Paterno + " " + emp.Materno + " " + emp.Nombres;
            
                    
                    int k = 3;
                    foreach (var inci in emp.Incidencias.OrderBy(x => x.Fecha))
                    {
                    switch (inci.TipoIncidencia.Trim())
                    {
                        case "X":
                            ws.Cell(a, k).Value = 1;
                            break;
                        case "IR":
                            ws.Cell(a, k).Value = 3;
                            break;
                        case "IE":
                            ws.Cell(a, k).Value = 4;
                            break;
                        case "IM":
                            ws.Cell(a, k).Value = 5;
                            break;
                        case "FI":
                            ws.Cell(a, k).Value = 8;
                            break;
                        case "FA":
                            ws.Cell(a, k).Value = 9;
                            break;
                        case "FJ":
                            ws.Cell(a, k).Value = 16;
                            break;
                        case "V":
                            ws.Cell(a, k).Value = 2;
                            break;
                        case "PS":
                            ws.Cell(a, k).Value = 10;
                            break;
                        case "PC":
                            ws.Cell(a, k).Value =11;
                            break;
                        case "D":
                            ws.Cell(a, k).Value = 15;
                            break;

                    }

                           
                            
                        k++;
                    }


                    
                
                a++;
                j++;
                
            }
            
          
          
            
            ws.Cell("A1").Value = "ID";
            
            ws.Cell("B1").Value = "Colaborador";
            ws.Columns("2,1").Width = 40;
            ws.Columns("1,1").AdjustToContents();


            foreach (var e in empleado)
            {
             
         
                
            }



            wb.SaveAs(newruta,false);

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

        public bool guardarRegistro(DataTable data, int idperiodo)
        {
            try
            {
                int columnas = data.Columns.Count;
                List<DataColumn> drlist = new List<DataColumn>();
                List<Inasistencias> inasistencias = new List<Inasistencias>();
                foreach (DataColumn column in data.Columns)
                {
                    drlist.Add((DataColumn)column);
                }

                foreach (DataRow row in data.Rows)
                {
                    int IdEmpleado = Convert.ToInt32(row[0]);
                    for (int i = 2; i < columnas; i++)
                    {
                        Inasistencias aux = new Inasistencias();
                        DateTime fecha = Convert.ToDateTime(drlist[i].ColumnName);
                        int tipo = Convert.ToInt32(row[i]);

                        aux.IdEmpleado = IdEmpleado;
                        aux.Fecha = fecha;
                        aux.FechaFin = fecha;
                        aux.Dias = 1;
                        aux.xNomina = true;
                        aux.idPeriodo = idperiodo;
                        aux.IdTipoInasistencia = tipo;

                        inasistencias.Add(aux);

                    }
                }

                var periodo = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idperiodo).FirstOrDefault();
                var listacompara = ctx.Inasistencias.Where(x => (x.Fecha >= periodo.Fecha_Inicio && x.Fecha <= periodo.Fecha_Fin) || (x.FechaFin >= periodo.Fecha_Inicio && x.FechaFin <= periodo.Fecha_Fin)).ToList();
                List<Inasistencias> borrarBD = new List<Inasistencias>();
                Inasistencias aux3 = new Inasistencias();
                List<Inasistencias> duplicado = inasistencias.ToList();
                foreach (var ina in duplicado)
                {
                    if (ina.IdTipoInasistencia != 1 && ina.IdTipoInasistencia != 15 && ina.IdTipoInasistencia != 8 && ina.IdTipoInasistencia != 9 && ina.IdTipoInasistencia != 16)
                    {
                        inasistencias.Remove(ina);
                    }


                }
                List<Inasistencias> duplicado2 = inasistencias.ToList();
                if (listacompara.Count > 0)
                {
                    foreach (var ina in duplicado2)
                    {
                        foreach (var comparar in listacompara)
                        {
                            if (ina.Fecha == comparar.Fecha && ina.IdEmpleado == comparar.IdEmpleado && ina.IdTipoInasistencia == comparar.IdTipoInasistencia)
                            {
                                inasistencias.Remove(ina);

                            }
                            else if (ina.Fecha == comparar.Fecha && ina.IdEmpleado == comparar.IdEmpleado && ina.IdTipoInasistencia != comparar.IdTipoInasistencia)
                            {
                                if (ina.IdTipoInasistencia == 1 || ina.IdTipoInasistencia == 15)
                                {
                                    borrarBD.Add(comparar);
                                    inasistencias.Remove(ina);
                                }
                                else
                                {
                                    borrarBD.Add(comparar);
                                }

                            }

                        }
                    }
                }
                else
                {
                    List<Inasistencias> duplicado3 = inasistencias.ToList();
                    foreach (var ina in duplicado3)
                    {
                        if (ina.IdTipoInasistencia == 1 || ina.IdTipoInasistencia == 15)
                        {
                            inasistencias.Remove(ina);
                        }
                    }
                }



                int[] idBorrado = borrarBD.Select(x => x.IdInasistencia).ToArray();
                var borrado = string.Join(",", idBorrado);

                if (borrado != "")
                {
                    using (var context = new RHEntities())
                    {
                        string sqlQuery1 = "DELETE [Inacistencias] WHERE IdInacistencia in (" + borrado + ")";
                        context.Database.ExecuteSqlCommand(sqlQuery1);

                    }
                }


                using (var context = new RHEntities())
                {
                    context.Inasistencias.AddRange(inasistencias);
                    context.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
         

        }

    }
    public class ImportacionEmpleados
    {
        public int IdEmpledo { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
    }
}
