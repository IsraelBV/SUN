using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class NotificationHelpers
    {
        public static string SelectColor(int Tipo)
        {
            var cssClass = "";
            switch (Tipo)
            {
                case 1:
                    cssClass = "teal darken-3";
                    break;
                case 2:
                    cssClass = "red darken-3";
                    break;
                case 3:
                    cssClass = "orange darken-3";
                    break;
                default:
                    break;
            }
            return cssClass;
        }

        public static string getFechaDeNotificacion(DateTime date)
        {
            var hoy = DateTime.Now;
            if (hoy.Year != date.Year)
            {
                return date.ToString("dd/MM/yyyy");
            }
            else
            {
                var time = hoy - date;
                if (time.Days != 0)
                {
                    if (time.Days == 1)
                    {
                        return "Ayer " + date.ToString("H:mm");
                    }
                    else
                    {
                        return date.ToString("dd MMM H:mm");
                    }
                }
                else
                {
                    if (time.Hours != 0)
                    {
                        return "Hace " + time.Hours + " hrs.";
                    }
                    else if (time.Minutes != 0)
                    {
                        return "Hace " + @time.Minutes + " mins.";
                    }
                    else
                    {
                        return "Justo Ahora";
                    }
                }
            }
        }
    }
}
