using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace Nomina.BLL
{
    
   public class Catalogos
    {
        RHEntities ctx = null;

        public Catalogos()
        {
        ctx = new RHEntities();
        }

        public List<C_NOM_Conceptos> ListadoConceptos()
        {
            var listado = ctx.C_NOM_Conceptos.ToList();
            return listado;
        }
        //editar cuenta acredora
        public bool CuentaAcredora(int id, string ClaveContable)
        {
            if (ClaveContable == "")
            {
                ClaveContable = "...";
            }
            var result = false;
            var editar = ctx.C_NOM_Conceptos.FirstOrDefault(x => x.IdConcepto == id);
            editar.Cuenta_Acredora = ClaveContable;
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }
        //editar cuenta deudora
        public bool CuentaDeudora(int id, string ClaveContable)
        {
            if (ClaveContable == "")
            {
                ClaveContable = "...";
            }
            var result = false;
            var editar = ctx.C_NOM_Conceptos.FirstOrDefault(x => x.IdConcepto == id);
            editar.Cuenta_Deudora = ClaveContable;
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

    

    }
}
