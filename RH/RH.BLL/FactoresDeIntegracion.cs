using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.BLL
{
    public class FactoresDeIntegracion
    {
        RHEntities ctx = null;

        public FactoresDeIntegracion()
        {
            ctx = new RHEntities();
        }

        public List<C_FactoresIntegracion> GetFactoresIntegracion()
        {
            List<C_FactoresIntegracion> lista = null;

            lista = ctx.C_FactoresIntegracion.ToList();

            return lista;
        }

        public C_FactoresIntegracion GetFactorIntregracioByID(int id)
        {
            C_FactoresIntegracion fac = null;

            fac = ctx.C_FactoresIntegracion
                .Where(x => x.IdFactor == id).FirstOrDefault();

            return fac;
        }
        public bool UpdateFactor(int id, C_FactoresIntegracion bn)
        {
            var result = false;

            var item = ctx.C_FactoresIntegracion.FirstOrDefault(x => x.IdFactor == id);

            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;

            item.Antiguedad = bn.Antiguedad;
            item.DiasAguinaldo = bn.DiasAguinaldo;
            item.DiasVacaciones = bn.DiasVacaciones;
            item.Factor = bn.Factor;
            item.PrimaVacacional = bn.PrimaVacacional/100;

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        public bool CrearFactor(C_FactoresIntegracion bn)
        {
            var result = false;
            decimal prima = bn.PrimaVacacional/100;
           
            var factor = new C_FactoresIntegracion
            {
                Antiguedad = bn.Antiguedad,
                DiasAguinaldo = bn.DiasAguinaldo,
                DiasVacaciones = bn.DiasVacaciones,
                PrimaVacacional = prima,
                Factor = bn.Factor

            };

            ctx.C_FactoresIntegracion.Add(factor);
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        public bool BorrarFactor(int id)
        {
            var result = false;

            var item = ctx.C_FactoresIntegracion.FirstOrDefault(x => x.IdFactor == id);

            //si no se encontro un registro con ese id,
            // retornamos false 
            if (item == null) return false;

            ctx.C_FactoresIntegracion.Attach(item);
            ctx.C_FactoresIntegracion.Remove(item);

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }
        public C_FactoresIntegracion GetUltimoFactor()
        {
            

            var ultifactor = ctx.C_FactoresIntegracion.OrderByDescending(x => x.IdFactor).Take(1).FirstOrDefault();
         
                return ultifactor;
            
            
        }

        public decimal ObtenerFactorUno(int anti=1)
        {
            return ctx.C_FactoresIntegracion.Where(s => s.Antiguedad == anti).Select(s => s.Factor).Take(1).FirstOrDefault();
        }

    }
}
