using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notificacion.BLL;
using Newtonsoft.Json;
using Common.Enums;
using System.Reflection;

namespace Notificacion.TEST
{
    class Program
    {
        static void Main(string[] args)
        {
            var fechaActual = DateTime.Now;
            var fechaItem = new DateTime(2016, 8, 11, 15, 23, 0);
            var time = fechaActual - fechaItem;

            if (time.Days != 0)
                Console.WriteLine("Días de diferencia");
            else
            {
                if (time.Hours != 0)
                    Console.WriteLine("Tiempo: {0} hrs", time.ToString("%h"));
                else
                    Console.WriteLine("Tiempo {0} mins", time.ToString("%m"));
            }
            Console.ReadKey();
        }
    }
}
