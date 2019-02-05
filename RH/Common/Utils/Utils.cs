using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Data;
using System.Reflection;
using System.Xml.Linq;
using System.Web;
using Excel;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Common.Utils
{
    public static class Utils
    {
        /// <summary>
        /// Regresa un string con la fecha en el formato apuntado
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="DateFormat"></param>
        /// <returns></returns>
        public static string GetDateNullableToString(DateTime? Date, string DateFormat)
        {
            return Date != null ? Date.Value.ToString(DateFormat) : "";
        }

        /// <summary>
        /// Regresa el nombre del enumerador
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetNameOfEnum(Type enumType, int id)
        {
            return Enum.GetName(enumType, id);
        }

        /// <summary>
        /// Regresa el valor de un enumador pasando el nombre de la propiedad como string 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetValueEnumByStrName(Type enumType, string name)
        {
            return (int)Enum.ToObject(enumType, name);
        }

        /// <summary>
        /// Retorna el array de bytes de un archivo, pasando la ruta del mismo.
        /// </summary>
        /// <param name="pathArchivo"></param>
        /// <returns></returns>
        public static byte[] GetBytesArchivo(string pathArchivo)
        {
            FileStream fs = new FileStream(pathArchivo, FileMode.Open, FileAccess.Read);
            int size = (int)fs.Length;
            byte[] data = new byte[size];
            size = fs.Read(data, 0, size);
            fs.Close();
            return data;
        }
        public static decimal TruncateDecimalesAbc(decimal numero, int length)
        {

            if (!numero.ToString().Contains('.')) return numero;

            string[] param = numero.ToString().Split('.');

            if (param[1].Length >= length)
                return Convert.ToDecimal(param[0] + "." + param[1].Substring(0, length));
            else
                return Convert.ToDecimal(param[0] + "." + param[1].Substring(0, param[1].Length));
        }

        public static decimal TruncateDecimales(decimal cantidad)
        {
            if (cantidad == 0) return 0.00M;

            return Math.Truncate(cantidad * 100) / 100;
        }

        /// <summary>
        /// Retorna los días que contiene el bimestre. (2016,1)
        /// </summary>
        /// <param name="añoFiscal"></param>
        /// <param name="numBimestre"></param>
        /// <returns></returns>
        public static int GetDiasDelBimestre(int añoFiscal = 0, int numBimestre = 0)
        {
            var dt = DateTime.Now;
            var diasDelBimestre = 0;


            switch (numBimestre)
            {
                case 1:
                    diasDelBimestre = DateTime.DaysInMonth(añoFiscal, 1) + DateTime.DaysInMonth(añoFiscal, 2);
                    break;
                case 2:
                    diasDelBimestre = DateTime.DaysInMonth(añoFiscal, 3) + DateTime.DaysInMonth(añoFiscal, 4);
                    break;
                case 3:
                    diasDelBimestre = DateTime.DaysInMonth(añoFiscal, 5) + DateTime.DaysInMonth(añoFiscal, 6);
                    break;
                case 4:
                    diasDelBimestre = DateTime.DaysInMonth(añoFiscal, 7) + DateTime.DaysInMonth(añoFiscal, 8);
                    break;
                case 5:
                    diasDelBimestre = DateTime.DaysInMonth(añoFiscal, 9) + DateTime.DaysInMonth(añoFiscal, 10);
                    break;
                case 6:
                    diasDelBimestre = DateTime.DaysInMonth(añoFiscal, 11) + DateTime.DaysInMonth(añoFiscal, 12);
                    break;
            }

            return diasDelBimestre;
        }

        /// <summary>
        /// Retorna el numero de Bimestre al que pertenece el numero del mes indicado
        /// </summary>
        /// <param name="numMes"></param>
        /// <returns></returns>
        public static int GetBimestre(int numMes)
        {
            switch (numMes)
            {
                case 1:
                case 2:
                    return 1;
                case 3:
                case 4:
                    return 2;
                case 5:
                case 6:
                    return 3;
                case 7:
                case 8:
                    return 4;
                case 9:
                case 10:
                    return 5;
                case 11:
                case 12:
                    return 6;

            }

            return 0;
        }

        /// <summary>
        /// Retorna el numero de Bimestre al que pertenece la fecha indicada
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public static int GetBimestre(DateTime fecha)
        {
            return GetBimestre(fecha.Month);
        }
        public static int GetDiasEntreDosFechas(DateTime fi, DateTime ff)
        {
            TimeSpan ts = ff - fi;

            //diferencia de años
            return ts.Days + 1;
        }

        public static int GetDiasDelAño(DateTime f)
        {
            var dias = 365;
            if (DateTime.IsLeapYear(f.Year))
            {
                dias = 366;
            }
            return dias;
        }

        public static string GetAñoActual()
        {
            var dt = DateTime.Now;

            var año = dt.Year;

            return año.ToString();
        }

        public static bool ExisteArchivo(string pathArchivo)
        {
            var result = File.Exists(pathArchivo);

            return result;
        }
        public static DataTable XElementToDataTable(XElement x)
        {
            DataTable dtable = new DataTable();
            XElement setup = (from p in x.Descendants() select p).First();
            // build your DataTable
            foreach (XElement xe in setup.Descendants())
                dtable.Columns.Add(new DataColumn(xe.Name.ToString(), typeof(string)));
            var all = from p in x.Descendants(setup.Name.ToString()) select p;
            foreach (XElement xe in all)
            {
                DataRow dr = dtable.NewRow();
                foreach (XElement xe2 in xe.Descendants())
                    dr[xe2.Name.ToString()] = xe2.Value;
                dtable.Rows.Add(dr);
            }
            return dtable;
        }
        public static DataTable ExcelToDataTable(HttpPostedFileBase fileBase, bool firstRowAsColumnNames = true)
        {
            //Valid
            if (!fileBase.FileName.EndsWith(".xls") && !fileBase.FileName.EndsWith(".xlsx"))
            {
                return null;
            }

            Stream stream = fileBase.InputStream;
            IExcelDataReader reader = null;
            if (fileBase.FileName.EndsWith(".xls"))
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            reader.IsFirstRowAsColumnNames = firstRowAsColumnNames;
            var result = reader.AsDataSet();
            reader.Close();
            reader.Dispose();
            reader = null;
            return result.Tables[0];

        }

        /// <summary>
        /// Metodo que retorna los días que esten dentro de las fechas del periodo de pago.
        /// </summary>
        /// <param name="fechaRangoInicial"></param>
        /// <param name="fechaRangoFinal"></param>
        /// <param name="fechaInicialPeriodo"></param>
        /// <param name="fechaFinalPeriodo"></param>
        /// <returns></returns>
        public static int GetDiasByPeriodoRange(DateTime fechaRangoInicial, DateTime fechaRangoFinal, DateTime fechaInicialPeriodo, DateTime fechaFinalPeriodo)
        {
            int contadorDias = 0;

            for (var date = fechaInicialPeriodo; date.Date <= fechaFinalPeriodo.Date; date = date.AddDays(1))
            {
                if (date >= fechaRangoInicial && date <= fechaRangoFinal)
                {
                    contadorDias++;
                }
            }
            return contadorDias;
        }

        public static decimal ConvertToDecimal(string data)
        {
            try
            {
                if (data.Trim() == "") return -1;
                if (data == null) return -1;

                decimal result = 0;
                // Double.TryParse(data, out result);
                result = Convert.ToDecimal(data);
                return result;
            }
            catch (Exception)
            {
                throw new FormatException(data + " no es un valor válido. \n Debe escribir solo números.");
            }
        }

        public static int ConvertToInt(string data)
        {
            try
            {
                if (data.Trim() == "") return 0;
                if (data == null) return 0;

                int result = 0;
                // Double.TryParse(data, out result);
                result = Convert.ToInt32(Convert.ToDouble(data));
                return result;
            }
            catch (Exception)
            {
                throw new FormatException(data + " no es un valor válido. \n Debe escribir solo números.");
            }
        }

        public static decimal ImpuestoSobreNomina(decimal cantidad, decimal porcentaje)
        {
            decimal result = 0;

            result = Utils.TruncateDecimales(cantidad * porcentaje);

            return result;
        }

        public static decimal ImpuestoSobreNomina(decimal cantidad, decimal porcentaje, int diasDelPeriodo)
        {
            decimal result = 0;

            result = Utils.TruncateDecimales((cantidad * porcentaje) * diasDelPeriodo);

            return result;
        }

        /// <summary>
        /// Metodo que crea el folder con el id del usuario 
        /// y guarda el archivo en el folder.
        /// Si el foldeR existe elimina el contenido.
        /// Retorna el path del nuevo folder.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="pathFolder"></param>
        /// <returns></returns>
        public static string ValidarFolderUsuario(int idUsuario, string pathFolder)
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

            folderUsuario = folderUsuario.Replace("\\\\", "\\");

            if (Directory.Exists(folderUsuario))
            {
                try
                {
                    //Elimina el contenido del folder
                    Array.ForEach(Directory.GetFiles(folderUsuario), File.Delete);
                    //Eliminar los folder
                    // Array.ForEach(Directory.GetDirectories(folderUsuario), Directory.Delete);


                    var arrayDirectorios = Directory.GetDirectories(folderUsuario);

                    foreach (var itemDir in arrayDirectorios)
                    {
                        Directory.Delete(itemDir, true);
                    }




                }
                catch (Exception ex)
                {
                    // folderUsuario += "\\" + idUsuario + "\\";
                }
            }
            else
            {
                //Crea el folder con el id del usuario
                Directory.CreateDirectory(folderUsuario);
            }

            // _pathUsuario = folderUsuario;

            return folderUsuario;
        }

        public static int[] ElimarEnArrarDeOtroArray(int[] arrayPrincipal, int[] arraySecundario)
        {
            var arrayReducido = (from a in arrayPrincipal
                                 where !arraySecundario.Contains(a)
                                 select a).ToArray();

            return arrayReducido;
        }

        /// <summary>
        /// Crea una lista de tipo SelectListItem tomando un enumerador como referencia
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="identificador"></param>
        /// <param name="valorSeleccionado"></param>
        /// <returns></returns>
        public static List<SelectListItem> CreateSelectedFromEnum(Type enumType, int valorSeleccionado)
        {
            var valores = Enum.GetValues(enumType);
            var nombres = Enum.GetNames(enumType);
            var selected = false;

            Type tipo = Enum.GetUnderlyingType(enumType);

            var lista = new List<SelectListItem>();

            for (var i = 0; i < valores.Length; i++)
            {
                var valor = (int)Convert.ChangeType(valores.GetValue(i), tipo);

                if (valor == valorSeleccionado)
                    selected = true;

                lista.Add(new SelectListItem() { Value = valor.ToString(), Text = nombres[i], Selected = selected });

                selected = false;
            }

            return lista;
        }

        public static List<Tuple<int, int>> GetMesesEntreDosFechas(DateTime inicio, DateTime fin)
        {
            var mesInicio = inicio.Month;
            var mesfinal = fin.Month;
            var año = inicio.Year;

            List<Tuple<int, int>> lista = new List<Tuple<int, int>>();

            for (int i = mesInicio; i <= mesfinal; i++)
            {
                lista.Add(Tuple.Create(i,año));
            }

            return lista;
        }

        public static int GetDaysleft(DateTime fecha)
        {

            int daysInYear = DateTime.IsLeapYear(fecha.Year) ? 366 : 365;

            return daysInYear - fecha.DayOfYear;
        }

        public static int GetAntiguedadLiquidacion(DateTime fechaAlta, DateTime fechaBaja)
        {
            int result = 0;

            if (fechaAlta == null || fechaBaja == null)
                return result;

            int difAño = fechaBaja.Year - fechaAlta.Year; //obtenemos las antiguedad por año

            int difMes = fechaBaja.Month - fechaAlta.Month;

            //Primera revision
            if (difMes >= 0) // tiene los años cumplidos con meses de mas
            {
                return difAño;//por lo tanto le corresponde la diferencia de años difAño
            }

            //Segunda revision
            if (difMes >= -5)//-5,-4,-3,-2,-1 significa que le faltan menos de 6 meses para completar el año, o que al menos ya paso mas de 6 meses corrientes
            {
                return difAño;//aun le corresponde la diferencia de años difAño
            }

            //Tercera revision
            if (difMes == -6)// todavía le falta 6 meses para completar el año, pero vericamos los dias
            {
                //revisamos que los dias de dif sean 0

                int difDias = fechaBaja.Day - fechaAlta.Day;

                if (difDias >= 0)// 0 completo los 6 meses, si es mas, entonces ya tiene seis meses mas dias adicionales
                {
                    return difAño;
                }
                else
                {
                    difAño -= 1;
                    return difAño;
                }
            }

            if (difMes <= -7)
            {
                difAño -= 1;
                return difAño;
            }


            return result;
        }

        #region CONVERT CANTIDAD A LETRAS

        static double _decimales = 0.0;

        static readonly string[] _numeroBase ={
                                 "",
                                 "uno",
                                 "dos",
                                 "tres",
                                 "cuatro",
                                 "cinco",
                                 "seis",
                                 "siete",
                                 "ocho",
                                 "nueve",
                                 "diez",
                                 "once",
                                 "doce",
                                 "trece",
                                 "catorce",
                                 "quince",
                                 "dieciseis",
                                 "diecisiete",
                                 "dieciocho",
                                 "diecinueve",
                            };

        static readonly string[] _numeroBase2 = {
                                   "",
                                   "",
                                   "veinte",
                                   "treinta",
                                   "cuarenta",
                                   "ciencuenta",
                                   "sesenta",
                                   "setenta",
                                   "ochenta",
                                   "noventa"
                               };



        /// <summary>
        /// Metodo para convertir Cantidad a Letras.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ConvertCantidadALetras(string num)
        {
            try
            {


                num = Sanitize(num);

                var Resultado = "";
                var longitudCadena = num.Length;

                if (longitudCadena < 4) //3
                    Resultado = Medida(longitudCadena, num);
                else if (longitudCadena < 7) //6
                {
                    var millares = longitudCadena - 3;
                    if (num.Substring(0, 1) == "1" && longitudCadena == 4)
                        Resultado = "mil " + Medida(3, num.Substring(millares, 3));
                    else
                        Resultado = Medida(millares, num.Substring(0, millares)) + " mil " +
                                    Medida(3, num.Substring(millares, 3));
                }
                else if (longitudCadena < 10) //9
                {
                    int millares = longitudCadena - 3;
                    int millon = longitudCadena - 6;
                    if (num.Substring(0, 1) == "1" && longitudCadena == 7)
                    {
                        if (Medida(3, num.Substring(millon, 3)) == "")
                            Resultado = "un millon " + Medida(3, num.Substring(millares, 3));
                        else
                        {
                            var miles = Medida(3, num.Substring(millon, 3));

                            if (miles == "uno")
                                miles = "";

                            Resultado = "un millon " + miles + " mil " +
                                        Medida(3, num.Substring(millares, 3));
                        }
                    }
                    else
                    {
                        var mil = " mil ";
                        if (Medida(3, num.Substring(millon, 3)) == "")
                        {
                            mil = " ";
                        }

                        var cntMil = Medida(3, num.Substring(millon, 3));

                        if (cntMil == "uno")
                        {
                            cntMil = "";
                        }

                        Resultado = Medida(millon, num.Substring(0, millon)) + " milllones "
                                    + cntMil + mil + Medida(3, num.Substring(millares, 3));
                    }

                }
                else if (longitudCadena < 13)
                    Resultado = Medida(3, num.Substring(0, 3)) + " mil " + Medida(3, num.Substring(4, 3)) +
                                " milllones " + Medida(3, num.Substring(7, 3)) + " mil " +
                                Medida(3, num.Substring(10, 3));
                else if (longitudCadena < 16)
                    Resultado = Medida(3, num.Substring(0, 3)) + " billones " + Medida(3, num.Substring(4, 3)) + " mil " +
                                Medida(3, num.Substring(7, 3)) + " milllones " + Medida(3, num.Substring(10, 3)) +
                                " mil " + Medida(3, num.Substring(13, 3));
                else if (longitudCadena < 19)
                    Resultado = Medida(3, num.Substring(0, 3)) + " mil " + Medida(3, num.Substring(4, 3)) + " billones " +
                                Medida(3, num.Substring(7, 3)) + " mil " + Medida(3, num.Substring(10, 3)) +
                                " milllones " + Medida(3, num.Substring(13, 3)) + " mil " +
                                Medida(3, num.Substring(13, 3));
                else if (longitudCadena < 21)
                    Resultado = "";
                else if (longitudCadena < 24)
                    Resultado = "";


                //
                if (_decimales > 0)
                {
                    Resultado += " PESOS (" + _decimales + "/100) M.N.";

                }
                else
                {
                    Resultado += " PESOS (00/100) M.N.";
                }

                return Resultado.ToUpper();
            }
            catch (Exception)
            {

                return "CANTIDAD INCORRECTA";
            }
        }

        private static string Unidades(string numx)
        {
            return _numeroBase[Convert.ToInt32(numx)];
        }

        private static string Decenas(string numx)
        {
            var pre = "";
            var num = Convert.ToInt32(numx);
            if (num < 20)
            {
                pre = _numeroBase[num];
            }
            else
            {
                if (numx.Substring(0, 1) == "2")
                {
                    if (num > 20 && num < 30)
                        pre = "veinti" + Unidades(numx.Substring(1, 1));
                    else
                        pre = _numeroBase2[2] + Unidades(numx.Substring(1, 1));
                }
                else
                {
                    if (numx.Substring(1, 1) == "0")
                        pre = _numeroBase2[Convert.ToInt32(numx.Substring(0, 1))];
                    else
                        pre = _numeroBase2[Convert.ToInt32(numx.Substring(0, 1))] + " y " +
                              Unidades(numx.Substring(1, 1));
                }
            }
            return pre;
        }

        private static string Centenas(string numx)
        {
            string pre = "";
            if (numx.Substring(0, 1) == "1")
            {
                if (numx.Substring(1, 1) == "0" && numx.Substring(2, 1) == "0")
                    pre = "cien ";
                else
                    pre = "ciento " + Decenas(numx.Substring(1, 2));
            }
            else if (numx.Substring(0, 1) == "0")
            {
                pre = "" + Decenas(numx.Substring(1, 2));
            }
            else if (numx.Substring(0, 1) == "5")
            {
                pre = "quinientos " + Decenas(numx.Substring(1, 2));
            }
            else if (numx.Substring(0, 1) == "7")
            {
                pre = "setecientos " + Decenas(numx.Substring(1, 2));
            }
            else if (numx.Substring(0, 1) == "9")
            {
                pre = "novecientos " + Decenas(numx.Substring(1, 2));
            }
            else
            {
                pre = _numeroBase[Convert.ToInt32(numx.Substring(0, 1))] + "cientos " + Decenas(numx.Substring(1, 2));
            }
            return pre;
        }

        private static string Medida(int cant, string Val)
        {
            var cadenaFinal = "";
            switch (cant)
            {
                case 1:
                    cadenaFinal = Unidades(Val);
                    break;
                case 2:
                    cadenaFinal = Decenas(Val);
                    break;
                case 3:
                    cadenaFinal = Centenas(Val);
                    break;
            }
            return cadenaFinal;
        }

        private static string Sanitize(string num)
        {

            num = num.Replace(" ", "");

            var dblCantidad = Convert.ToDouble(num);

            var entero = Convert.ToInt64(Math.Truncate(dblCantidad));
            _decimales = Convert.ToInt32(Math.Round((dblCantidad - entero) * 100, 2));

            num = entero.ToString();

            return num;
        }

        #endregion

        public static bool ValidateData(string ER, string val) {
            return Regex.IsMatch(val, ER);
        }
    }
}

