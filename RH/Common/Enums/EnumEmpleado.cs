using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    public enum EstadoCivil {
        [Description("Soltero(a)")]
        soltero = 1,
        [Description("Casado(a)")]
        casado = 2,
        [Description("Divorciado(a)")]
        divorciado = 3,
        [Description("Viudo(a)")]
        viudo = 4,
        [Description("Union Libre")]
        unionlibre = 5
    }
}
