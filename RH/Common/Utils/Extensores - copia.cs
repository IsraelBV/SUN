using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class Extensores
    {
        /// <summary>
        /// Convierte el objeto nullable a String, si el objeto es null retorna un String vacio - 
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToStringOrEmpty<T>(this T? t) where T : struct
        {
            return t.HasValue ? t.Value.ToString() : string.Empty;
        }
        public static string ToStringOrEmptyDate<TDateTime>(this DateTime? t) where TDateTime : struct
        {
            return t.HasValue ? t.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
        public static string ToStringOrEmptyDate<TDateTime>(this DateTime? t, string format) where TDateTime : struct
        {
            return t.HasValue ? t.Value.ToString(format) : string.Empty;
        }

        public static string FormatDDMMAAAA(this DateTime fecha)
        {
            if (fecha == null) return "--------";
            var strFechaFormato = $"{fecha.Day}{fecha.Month}{fecha.Year}";

            return strFechaFormato;
        }

        public static string GeNombreDelMes(this DateTime fecha)
        {
            try
            {
           
            var mes = fecha.Month;

            switch (mes)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";
            }
                return "-";
            }
            catch (Exception e)
            {
                return "?";
            }
        }

        /// <summary>
        /// Convierte el objeto nullable a String, si el objeto es null retorna null - 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static string ToStringOrNull<T>(this T? nullable) where T : struct
        {
            return nullable.HasValue ? nullable.ToString() : null;
        }

        /// <summary>
        /// Retorna el numero de palabras usando el separador
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="separador"></param>
        /// <returns></returns>
        public static int ContarPalabras(this string texto, char separador)
        {
            return texto.Split(separador).Length;
        }

        /// <summary>
        /// Recibe un string en formato mes/dia/año = 12/17/2016
        /// y retorna un string en formato dia/mes/año = 17/12/2016
        /// </summary>
        /// <param name="fechaMesDiaAño"></param>
        /// <returns></returns>
        public static string ToDiaMesAño(this string fechaMesDiaAño)
        {
            var strSplit = fechaMesDiaAño.Split('/');

            string nuevaFechaFormato = strSplit[1] + "/" + strSplit[0] + "/" + strSplit[2];

            return nuevaFechaFormato;
        }

        /// <summary>
        /// Metodo que retorna la antiguedad en el formato del SAT P10Y8M15D
        /// </summary>
        /// <param name="fechaInicioLaboral"></param>
        /// <param name="fechaFinalPago"></param>
        /// <returns></returns>
        public static string FormatoAntiguedadSat(this DateTime fechaInicioLaboral, DateTime fechaFinalPago)
        {
            string result = "";
            // DateTime newDate = DateTime.Now;

            TimeSpan ts = fechaFinalPago - fechaInicioLaboral;

            //var años = newDate.Year - fechaInicioLaboral.Year;
            //var meses = newDate.Month - fechaInicioLaboral.Month;

            var total = ts.TotalDays;
            var dias = ts.Days;

            var semanas = (dias + 1) / 7;
            var anios = dias / 365;
            var meses = dias / 30;

            if (anios > 100)
            {
                result = $"P{anios}Y{meses}M{dias}D";
            }
            //else if (meses > 0)
            //{
            //    result = $"P{meses}M{dias}D";
            //}
            else if (semanas > 0)
            {
                result = $"P{semanas}W";
            }
            else
            {
                result = $"P{dias}D";
            }

            return result;
        }
        public static decimal TruncateDecimal(this decimal cantidad, int longitud)
        {
            return Utils.TruncateDecimales(cantidad);
        }
        public static string ToCurrencyFormat(this decimal cantidad, int decimales = 2, bool signo = false)
        {
            string result = "";
            //var strCantidad = $"{cantidad.TruncateDecimal(decimales):C}";
            //if (signo) return strCantidad;

            //var datos = strCantidad.Split('$');
            //return datos[1];

            cantidad = cantidad.TruncateDecimal(decimales);
            switch (decimales)
            {
                case 1:
                    result = String.Format("{0:###,###,##0.0}", cantidad);
                    break;
                case 2:
                    result = String.Format("{0:###,###,##0.00}",cantidad);
                    break;
                case 3:
                    result = String.Format("{0:###,###,##0.000}", cantidad);
                    break;
                case 4:
                    result = String.Format("{0:###,###,##0.0000}", cantidad);
                    break;
                case 5:
                    result = String.Format("{0:###,###,##0.00000}", cantidad);
                    break;
            }
                       

            if (signo)
            {
                return "$ " + result;
            }
            else
            {
                return result;
            }

        }
        public static string ToCurrencyFormat(this double cantidad, int decimales = 2, bool signo = false)
        {
            string result = "";
            //var strCantidad = $"{cantidad.TruncateDecimal(decimales):C}";
            //if (signo) return strCantidad;

            

            //var datos = strCantidad.Split('$');
            //return datos[1];
            switch (decimales)
            {
                case 1:
                    result = String.Format("{0:###,###,##0.0}", cantidad);
                    break;
                case 2:
                    result = String.Format("{0:###,###,##0.00}", cantidad);
                    break;
                case 3:
                    result = String.Format("{0:###,###,##0.000}", cantidad);
                    break;
                case 4:
                    result = String.Format("{0:###,###,##0.0000}", cantidad);
                    break;
                case 5:
                    result = String.Format("{0:###,###,##0.00000}", cantidad);
                    break;
            }


            if (signo)
            {
                return "$ " + result;
            }
            else
            {
                return result;
            }

        }
        public static string ToCurrencyFormat(this string cantidad, int decimales = 2, bool signo = false)
        {
            string result = "";
            //var strCantidad = $"{cantidad.TruncateDecimal(decimales):C}";
            //if (signo) return strCantidad;

            decimal cant = Convert.ToDecimal(cantidad);

            cant = cant.TruncateDecimal(decimales);

            //var datos = strCantidad.Split('$');
            //return datos[1];
            switch (decimales)
            {
                case 1:
                    result = String.Format("{0:###,###,##0.0}", cant);
                    break;
                case 2:
                    result = String.Format("{0:###,###,##0.00}", cant);
                    break;
                case 3:
                    result = String.Format("{0:###,###,##0.000}", cant);
                    break;
                case 4:
                    result = String.Format("{0:###,###,##0.0000}", cant);
                    break;
                case 5:
                    result = String.Format("{0:###,###,##0.00000}", cant);
                    break;
            }


            if (signo)
            {
                return "$ " + result;
            }
            else
            {
                return result;
            }

        }

        public static string ToIdseFormat(this decimal cantidad)
        {
            var valorTruncado = Utils.TruncateDecimales(cantidad).ToString("0.00");
            var strCantidad = valorTruncado.ToString(CultureInfo.InvariantCulture);
            strCantidad = strCantidad.Replace(".","");
            strCantidad = strCantidad.PadLeft(6, '0');
            return strCantidad;
        }

        public static string ToSUAFormat(this decimal cantidad)
        {
            var valorTruncado = Utils.TruncateDecimales(cantidad);
            var strCantidad = valorTruncado.ToString(CultureInfo.InvariantCulture);
            strCantidad = strCantidad.Replace(".", "");
            strCantidad = strCantidad.PadLeft(7, '0');
            return strCantidad;
        }

    }
}
