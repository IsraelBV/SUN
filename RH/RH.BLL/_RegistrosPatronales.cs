using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
   public class _RegistrosPatronales
    {
        RHEntities ctx = null;

        public _RegistrosPatronales()
        {
            ctx = new RHEntities();
        }


        public List<RegistroPatronal> getRegistrosPatronales()
        {
            List<RegistroPatronal> lista = null;

            lista = ctx.RegistroPatronal.ToList();

            return lista;
        }

        public RegistroPatronal getRegistroPatronalById(int id)
        {
            RegistroPatronal rp = null;

            rp = ctx.RegistroPatronal
                .Where(x => x.IdRegistroPatronal == id).FirstOrDefault();

            return rp;
        }
    }
}
