using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using RH.Entidades.GlobalModel;
namespace RH.BLL
{
    public class QueryData
    {
        //devuelve una lista de los cfdi timbrados en base a los parametros del metodo
        public static List<DatosVisor> BuscarTimbrados(DateTime? fechaI, DateTime? fechaF, int idPeriodoB = 0, int idEjercicio = 0, int idEmpresa = 0, int idSucursal = 0, bool Cancelados = false)
        {
            List<DatosVisor> listaTimbrados = new List<DatosVisor>();

            using (var context = new RHEntities())
            {
                var sql =
                "SELECT " +
                    "IdTimbrado," +
                    "IdReceptor as IdEmpleado," +
                    "IdPeriodo," +
                    "IdNomina," +
                    "IdFiniquito," +
                    "IdSucursal," +
                    "XMLTimbrado," +
                    "RFCEmisor," +
                    "RFCReceptor," +
                    "FolioFiscalUUID," +
                    "TotalRecibo," +
                    "IdEmisor," +
                    "FechaCertificacion," +
                    "FechaCancelacion " +
                "FROM " +
                    "NOM_CFDI_Timbrado " +
                "WHERE " +
                    "IdEjercicio = " + idEjercicio +
                    ((idEmpresa > 0) ? " AND IdEmisor = " + idEmpresa : "") + //AGREGA LA CONDICIONANTE SI LA VARIABLE RECIBE VALOR
                    ((idSucursal > 0) ? " AND IdSucursal = " + idSucursal : "") +
                    " AND Cancelado = " + ((Cancelados == false) ? 0 : 1) +
                    " AND IsPrueba = 0" +
                    " AND FolioFiscalUUID IS NOT NULL" +
                    ((idPeriodoB > 0) ? " AND IdPeriodo = " + idPeriodoB : "") +
                    ((fechaI.HasValue) ? " AND FechaCertificacion >= '" + fechaI.Value.ToString("dd/MM/yyyy HH:mm:ss") + "'" : "") +
                    ((fechaI.HasValue && fechaF.HasValue) ? " AND FechaCertificacion <= '" + fechaF.Value.ToString("dd/MM/yyyy HH:mm:ss") + "'" : "");

                listaTimbrados = context.Database.SqlQuery<DatosVisor>(sql).ToList();

            }

            return listaTimbrados;
        }
    }
}
