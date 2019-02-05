using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using SYA.BLL;
namespace SYA.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ControlUsuario control = new ControlUsuario();

            var usuario = control.GetUsuarioByCuenta("test", "test");
            Console.WriteLine(usuario.Nombres);
            Console.ReadKey();
        }
    }
}
