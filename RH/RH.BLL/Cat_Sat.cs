using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public static class Cat_Sat
    {
        public static C_PeriodicidadPago_SAT GetPeriodicidadPagoById(int IdPeriodicidadPago)
        {
            RHEntities rh = new RHEntities();
            return rh.C_PeriodicidadPago_SAT.FirstOrDefault(x => x.IdPeriodicidadPago == IdPeriodicidadPago);
        }

        public static List<C_PeriodicidadPago_SAT> GetPeriodicidadPagos()
        {
            RHEntities rh = new RHEntities();
            return rh.C_PeriodicidadPago_SAT.ToList();
        }

        public static C_TipoJornada_SAT GetTipoJornadaById(int IdTipoJornada)
        {
            RHEntities rh = new RHEntities();
            return rh.C_TipoJornada_SAT.FirstOrDefault(x => x.IdTipoJornada == IdTipoJornada);
        }

        public static List<C_TipoJornada_SAT> GetTiposJornada()
        {
            RHEntities rh = new RHEntities();
            return rh.C_TipoJornada_SAT.ToList();
        }

        public static C_Metodos_Pago GetMetodoPagoById(int IdMetodoPago)
        {
            RHEntities rh = new RHEntities();
            return rh.C_Metodos_Pago.FirstOrDefault(x => x.IdMetodo == IdMetodoPago);
        }

        public static List<C_Metodos_Pago> GetMetodosPago()
        {
            RHEntities rh = new RHEntities();
            return rh.C_Metodos_Pago.ToList();
        }
        public static C_TipoRegimen_SAT GetTipoRegimenById(int IdTipoRegimen)
        {
            RHEntities rh = new RHEntities();
            return rh.C_TipoRegimen_SAT.FirstOrDefault(x => x.IdTipoRegimen == IdTipoRegimen);
        }
        public static List<C_TipoRegimen_SAT> GetTipoRegimen()
        {
            RHEntities rh = new RHEntities();
            return rh.C_TipoRegimen_SAT.ToList();
        }
    }
}
