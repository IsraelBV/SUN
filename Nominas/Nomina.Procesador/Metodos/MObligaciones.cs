using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nomina.Procesador.Modelos;
using Nomina.Procesador.Datos;
using RH.Entidades;

namespace Nomina.Procesador.Metodos
{
    public static class MObligaciones
    {

        static readonly NominasDao _nominasDao = new NominasDao();

        public static TotalConcepto FondoRetiroSar( decimal total, int idNomina = 0, int idFiniquito = 0)
        {
            GuardarConcepto(idNomina, idFiniquito, 29, total, 0, 0, 0, 0);

                var totalConcepto = new TotalConcepto
                {
                    Total = total,
                    ImpuestoSobreNomina = 0
                };

                return totalConcepto;
        }
        public static TotalConcepto ImpuestoEstatal( decimal total, int idNomina = 0, int idFiniquito = 0)
        {
                GuardarConcepto(idNomina, idFiniquito, 30, total, 0, 0, 0, 0);

                var totalConcepto = new TotalConcepto
                {
                    Total = total,
                    ImpuestoSobreNomina = 0
                };

                return totalConcepto;

          
        }
        public static TotalConcepto GuarderiaImss( decimal total, int idNomina = 0, int idFiniquito = 0)
        {
                GuardarConcepto(idNomina, idFiniquito, 37, total, 0, 0, 0, 0);

                var totalConcepto = new TotalConcepto
                {
                    Total = total,
                    ImpuestoSobreNomina = 0
                };

                return totalConcepto;
        }
        public static TotalConcepto ImssEmpresa( decimal total, int idNomina = 0, int idFiniquito = 0)
        {
                GuardarConcepto(idNomina, idFiniquito, 35, total, 0, 0, 0, 0);

                var totalConcepto = new TotalConcepto
                {
                    Total = total,
                    ImpuestoSobreNomina = 0
                };

                return totalConcepto;
        }
        public static TotalConcepto InfonavitEmpresa( decimal total, int idNomina = 0, int idFiniquito = 0)
        {

       

                GuardarConcepto(idNomina, idFiniquito, 36, total, 0, 0, 0, 0);

                var totalConcepto = new TotalConcepto
                {
                    Total = total,
                    ImpuestoSobreNomina = 0
                };

                return totalConcepto;

           
        }
        public static TotalConcepto RiesgoTrabajo( decimal total, int idNomina = 0, int idFiniquito = 0)
        {          

                GuardarConcepto(idNomina, idFiniquito, 33, total, 0, 0, 0, 0);

                var totalConcepto = new TotalConcepto
                {
                    Total = total,
                    ImpuestoSobreNomina = 0
                };

                return totalConcepto;                      
        }

        private static void GuardarConcepto(int idNomina = 0, int idFiniquito=0, int idConcepto = 0, decimal total = 0, decimal gravaIsr = 0, decimal excentoIsr = 0, decimal integraImss = 0, decimal impuestoNomina = 0)
        {
            if (idNomina > 0)
            {
                var nd = new NOM_Nomina_Detalle()
                {
                    Id = 0,
                    IdNomina = idNomina,
                    IdConcepto = idConcepto,
                    Total = total,
                    GravadoISR = gravaIsr,
                    ExentoISR = excentoIsr,
                    IntegraIMSS = integraImss,
                    ExentoIMSS = 0,
                    ImpuestoSobreNomina = impuestoNomina
                };

                _nominasDao.AddDetalleNomina(nd);
            }
            else if (idFiniquito > 0)
            {
                NOM_Finiquito_Detalle fd = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = idFiniquito,
                    IdConcepto = idConcepto,
                    Total = total,
                    GravadoISR = gravaIsr,
                    ExentoISR = excentoIsr,
                    IntegraIMSS = integraImss,
                    ImpuestoSobreNomina = impuestoNomina
                };
            }
        }


       
    }
}
