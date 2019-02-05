using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.BLL
{
   public class PeriodosVacaciones
    {
        RHEntities ctx = null;
        public PeriodosVacaciones()
        {
            ctx = new RHEntities();
        }

     
        public List<PeriodoVacaciones> GetPeriodo(int id)
        {
            List<PeriodoVacaciones> periodo = null;

            periodo = ctx.PeriodoVacaciones
                .Where(x => x.IdEmpleado_Contrato == id).ToList();

            return periodo;
        }

        public bool CrearPeriodo(int id, DateTime real,int IdEmpleado)
        {
            DateTime hoy = DateTime.Now;

            // Difference in days, hours, and minutes.
            TimeSpan ts = hoy - real;
            // Difference in days.
            int differenceInDays = ts.Days;
            int anti = GetAntiguedad(differenceInDays);
            IEnumerable<int> values = Enumerable.Range(1, anti);
          
           foreach(int n in values)
            { 
                var factInt = ctx.C_FactoresIntegracion.FirstOrDefault(x => x.Antiguedad == n);
                if (factInt != null)
                { 
                    string periodo = Convert.ToString(n);                

                    var comprobar = ctx.PeriodoVacaciones.FirstOrDefault(x => x.IdEmpleado_Contrato == id && x.PeridoVacaciones == periodo);

                    if (comprobar == null)
                    {      
                        var datos = new PeriodoVacaciones
                        {
                            PeridoVacaciones = periodo,
                            DiasCorresponde = factInt.DiasVacaciones,
                            IdEmpleado_Contrato = id
                        };

                        ctx.PeriodoVacaciones.Add(datos);
                        var r = ctx.SaveChanges();                       
                    }                   
                }               
            }

            return true;
        }

        private int GetAntiguedad(int dias)
        {
            dias = (dias / 365)+1;            
            return dias;
        }

    
    }
    public class FactorPerido
    {
        public int DiasCorres { get; set; }
       public  string Periodo { get; set; }
       public  int IdEmpleado { get; set; }
    }
}
