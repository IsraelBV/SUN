using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class Fonacot
    {

        RHEntities ctx = null;

        public Fonacot()
        {
            ctx = new RHEntities();
        }

        public List<Empleado_Fonacot> GetFonacotByEmpleado(int id)
        {
            return ctx.Empleado_Fonacot.Where(x => x.IdEmpleadoContrato == id).ToList();
        }

        public bool CreateFonacot(Empleado_Fonacot model)
        {
            model.Status = true;
            model.Saldo = model.MontoInicial;
            ctx.Empleado_Fonacot.Add(model);
            var status = ctx.SaveChanges();
            return status > 0 ? true : false;
        }

    }
}
