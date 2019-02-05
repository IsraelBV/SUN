using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class Estados
    {
        readonly RHEntities _ctx = null;

        public Estados()
        {
            _ctx = new RHEntities();
        }

        public List<C_Estado> GetEstados()
        {
            List<C_Estado> edos = null;
            edos = _ctx.C_Estado.ToList();
            return edos;
        }
    }
}
