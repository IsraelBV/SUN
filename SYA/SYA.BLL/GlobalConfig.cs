using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace SYA.BLL
{
   public class GlobalConfig
    {
        readonly RHEntities _ctx;

        //Metodo Constructor
        public GlobalConfig()
        {
            _ctx = new RHEntities();
        }

       public SYA_GlobalConfig GetGlobalConfig()
       {
           var item = _ctx.SYA_GlobalConfig.FirstOrDefault();
           return item;
       }

        /// <summary>
        /// Actualiza los folios disponible menos los folios usados.
        /// </summary>
        /// <param name="foliosUsados"></param>
       public void SetFoliosUsados(int foliosUsados)
       {
           var item = GetGlobalConfig();
           var actualFolios = item.Folios;

           item.Folios = (actualFolios - foliosUsados);
           _ctx.SaveChanges();

       }
    }
}
