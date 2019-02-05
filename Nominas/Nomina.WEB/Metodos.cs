using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using System.Threading.Tasks;

namespace Nomina.WEB
{
    public class Metodos
    {
        public async static Task<int> EjecutarMetodos()
        {
            var t = await Metodo1();
            var t2 = await Metodo2();

            return 5;
        }

        public static Task<int> Metodo1()
        {

            var t = Task.Factory.StartNew(() =>
            {
                for (int i = 1; i <= 250; i++)
                {
                    Debug.WriteLine("**** : M1 procesando... *" + i);
                    Task.Delay(50).Wait();
                }
                return 2;
            });

            return t;
        }


        public static Task<int> Metodo2()
        {

            var t = Task.Factory.StartNew(() =>
            {
                for (int i = 1; i <= 250; i++)
                {
                    Debug.WriteLine("~~~~ : J2 procesando... #" + i);
                    Task.Delay(50).Wait();
                }
                return 2;
            });

            return t;
        }


    }
}