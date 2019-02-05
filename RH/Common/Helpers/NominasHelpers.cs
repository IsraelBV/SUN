using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace Common.Helpers
{
    public static class NominasHelpers
    {
        public static string TipoNomina(int IdTipoNomina)
        {
            return "--";
            // return ((TipoNomina)IdTipoNomina).ToString();
        }

        public static string TipoConcepto(int IdTipoConcepto)
        {
            return ((TipoConcepto)IdTipoConcepto).ToString();
        }
    }
}
