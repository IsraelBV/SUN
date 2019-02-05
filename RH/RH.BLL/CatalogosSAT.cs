using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
   public class CatalogosSAT
    {

        private readonly RHEntities _ctx = null;

        public CatalogosSAT()
        {
            _ctx = new RHEntities();
        }


       public  List<C_TipoContrato_SAT> GetCatalogoTipoContrato()
       {
           return _ctx.C_TipoContrato_SAT.ToList();
       }

    }
}
