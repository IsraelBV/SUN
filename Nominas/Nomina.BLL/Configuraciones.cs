using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Nomina.Procesador;

namespace Nomina.BLL
{
    public class Configuraciones
    {
        private RHEntities ctx = null;

        public Configuraciones()
        {
            ctx = new RHEntities();
        }

        public List<C_NOM_Tabla_Subsidio> ListSubsidio()
        {
            var lista = ctx.C_NOM_Tabla_Subsidio.ToList();
            return lista;
        }

        public List<C_NOM_Tabla_ISR> ListISR()
        {
            var lista = ctx.C_NOM_Tabla_ISR.ToList();
            return lista;
        }

        public List<C_NOM_Tabla_IMSS> ListIMSS()
        {
            var lista = ctx.C_NOM_Tabla_IMSS.ToList();
            return lista;
        }



    }
}
