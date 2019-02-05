using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
  public  class EjerciciosFiscales
    {
        public static List<NOM_Ejercicio_Fiscal> GetEjerciciosFiscalesDes()
        {
            using (var context = new RHEntities())
            {
                var lista = (from e in context.NOM_Ejercicio_Fiscal
                    orderby e.IdEjercicio descending
                    select e).ToList();
                return lista;
            }
        }

        public static List<NOM_Ejercicio_Fiscal> GetEjerciciosFiscalesAsc()
        {
            using (var context = new RHEntities())
            {
                var lista = (from e in context.NOM_Ejercicio_Fiscal
                             orderby e.IdEjercicio 
                             select e).ToList();
                return lista;
            }
        }
    }
}
