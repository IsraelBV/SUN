using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using RH.Entidades.GlobalModel;


namespace Nomina.BLL
{
  public  class QueryData
    {
        public static List<DatosVisor> BuscarTimbrados(DateTime? fechaI, DateTime? fechaF, int idPeriodoB = 0, int idEjercicio = 0, int idEmpresa = 0, int idSucursal = 0, bool Cancelados = false)
        {
            List<DatosVisor> listaTimbrados = new List<DatosVisor>();

            using (var context = new RHEntities())
            {


                if (idPeriodoB > 0) //buscar por periodo
                {

                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == Cancelados
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && tim.IdPeriodo == idPeriodoB
                                      select new DatosVisor()
                                      {
                                          IdTimbrado = tim.IdTimbrado,
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado,

                                          RFCEmisor = tim.RFCEmisor,
                                          RFCReceptor = tim.RFCReceptor,
                                          FolioFiscalUUID = tim.FolioFiscalUUID,
                                          TotalRecibo = tim.TotalRecibo,
                                          IdEmisor = tim.IdEmisor,
                                          FechaCertificacion = tim.FechaCertificacion,
                                          FechaCancelacion = tim.FechaCancelacion
                                      }).ToList();

                }
                else if (idSucursal > 0 && (fechaI == null || fechaF == null)) // cliente sin fechas
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdSucursal == idSucursal
                                            && tim.Cancelado == Cancelados
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                      select new DatosVisor()
                                      {
                                          IdTimbrado = tim.IdTimbrado,
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado,

                                          RFCEmisor = tim.RFCEmisor,
                                          RFCReceptor = tim.RFCReceptor,
                                          FolioFiscalUUID = tim.FolioFiscalUUID,
                                          TotalRecibo = tim.TotalRecibo,
                                          IdEmisor = tim.IdEmisor,
                                          FechaCertificacion = tim.FechaCertificacion,
                                          FechaCancelacion = tim.FechaCancelacion
                                      }).ToList();
                }
                else if (idSucursal > 0 && fechaI != null) //cliente con fecha
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.IdSucursal == idSucursal
                                            && tim.Cancelado == Cancelados
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && DbFunctions.TruncateTime(tim.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI) &&
                                            DbFunctions.TruncateTime(tim.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                      select new DatosVisor()
                                      {
                                          IdTimbrado = tim.IdTimbrado,
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado,

                                          RFCEmisor = tim.RFCEmisor,
                                          RFCReceptor = tim.RFCReceptor,
                                          FolioFiscalUUID = tim.FolioFiscalUUID,
                                          TotalRecibo = tim.TotalRecibo,
                                          IdEmisor = tim.IdEmisor,
                                          FechaCertificacion = tim.FechaCertificacion,
                                          FechaCancelacion = tim.FechaCancelacion
                                      }).ToList();


                }
                else if (idEmpresa > 0 && (fechaI == null || fechaF == null)) // por empresa sin fecha
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == Cancelados
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                      select new DatosVisor()
                                      {
                                          IdTimbrado = tim.IdTimbrado,
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado,

                                          RFCEmisor = tim.RFCEmisor,
                                          RFCReceptor = tim.RFCReceptor,
                                          FolioFiscalUUID = tim.FolioFiscalUUID,
                                          TotalRecibo = tim.TotalRecibo,
                                          IdEmisor = tim.IdEmisor,
                                          FechaCertificacion = tim.FechaCertificacion,
                                          FechaCancelacion = tim.FechaCancelacion
                                      }).ToList();


                }
                else//por empresa y fecha
                {
                    listaTimbrados = (from tim in context.NOM_CFDI_Timbrado
                                      where tim.IdEjercicio == idEjercicio
                                            && tim.IdEmisor == idEmpresa
                                            && tim.Cancelado == Cancelados
                                            && tim.IsPrueba == false
                                            && tim.FolioFiscalUUID != null
                                            && DbFunctions.TruncateTime(tim.FechaCertificacion) >= DbFunctions.TruncateTime(fechaI)
                                            && DbFunctions.TruncateTime(tim.FechaCertificacion) <= DbFunctions.TruncateTime(fechaF)
                                      select new DatosVisor()
                                      {
                                          IdTimbrado = tim.IdTimbrado,
                                          IdEmpleado = tim.IdReceptor,
                                          IdPeriodo = tim.IdPeriodo,
                                          IdNomina = tim.IdNomina,
                                          IdFiniquito = tim.IdFiniquito,
                                          IdSucursal = tim.IdSucursal,
                                          XmlTimbrado = tim.XMLTimbrado,

                                          RFCEmisor = tim.RFCEmisor,
                                          RFCReceptor = tim.RFCReceptor,
                                          FolioFiscalUUID = tim.FolioFiscalUUID,
                                          TotalRecibo = tim.TotalRecibo,
                                          IdEmisor = tim.IdEmisor,
                                          FechaCertificacion = tim.FechaCertificacion,
                                          FechaCancelacion = tim.FechaCancelacion
                                      }).ToList();
                }
            }

            return listaTimbrados;
        }

    }
}
