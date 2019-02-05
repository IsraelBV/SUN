using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.BLL;
using RH.Entidades;
using Common.Utils;
using Nomina.BLL;

namespace RH.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //test();
            //  difAntiguedad();

            TuplasSelect();
            Console.ReadKey();
        }

        private static void TuplasSelect()
        {
            PeriodosPago pp = new PeriodosPago();
            pp.GetPeriodosBySucursalTimbrados(5,8,8);
        }

        public static void test()
        {
            Empleados emp = new Empleados();
            Sucursales suc = new Sucursales();
            var datos=suc.ListSucEmp(1);
            

        }


        private static void difAntiguedad()
        {
            DateTime alta = new DateTime(2015, 9, 24);
            DateTime baja = new DateTime(2018, 1, 25);

       var res =      Utils.GetAntiguedadLiquidacion(alta,baja);

            Console.WriteLine("Resultado antiguedad: {0} ", res);

        }
    }
}
