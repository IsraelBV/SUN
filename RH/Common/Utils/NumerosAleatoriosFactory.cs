using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public class NumerosAleatoriosFactory
    {
        private static NumerosAleatoriosFactory instance;
        private System.Random autoRand;


        //-------Methods------------------------
        public static NumerosAleatoriosFactory GetInstance()
        {
            if (instance == null)
                instance = new NumerosAleatoriosFactory();

            return instance;
        }//GetInstance

        public int GetNumeroEntero()
        {
            if (autoRand == null)
                autoRand = new System.Random();

            return (autoRand.Next());
        }//GetNumeroEntero
    }
}
