using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Nomina.Procesador.Modelos;
using Common.Enums;
using Common.Utils;
using Org.BouncyCastle.Asn1.Crmf;
using RH.Entidades.GlobalModel;

namespace Nomina.Procesador.Datos
{
    public class TimbradoDao
    {
        // private readonly RHEntities _ctx = null;



        /// <summary>
        /// Retorna una lista de Registros para timbrar 
        /// </summary>
        /// <param name="idNominas"></param>
        /// <returns></returns>
        public List<NOM_CFDI_Timbrado> GetRegistrosATimbrar(int[] idNominas, bool isFiniquito = false)
        {
            using (var context = new RHEntities())
            {
                if (isFiniquito)
                {
                    var finiquito = (from t in context.NOM_CFDI_Timbrado
                                     where idNominas.Contains(t.IdFiniquito) && t.FolioFiscalUUID == null
                                     select t).ToList();

                    return finiquito;
                }

                //Se filtra la consulta para que no retorne los registros que ya estan timbrados los que ya tengan su Folio UUID.
                var lista = (from t in context.NOM_CFDI_Timbrado
                             where idNominas.Contains(t.IdNomina) && t.FolioFiscalUUID == null
                             select t).ToList();

                return lista;
            }
        }
        public List<NOM_CFDI_Timbrado> GetRecibosPdf(int[] nominas, bool isFiniquito = false)
        {
            using (var context = new RHEntities())
            {
                List<NOM_CFDI_Timbrado> lista;

                if (isFiniquito)
                {
                    lista = (from t in context.NOM_CFDI_Timbrado
                             where nominas.Contains(t.IdFiniquito)
                                   && t.ErrorTimbrado == false
                                   && t.Cancelado == false
                             orderby t.RFCReceptor
                             select t).ToList();
                }
                else
                {

                    lista = (from t in context.NOM_CFDI_Timbrado
                             where nominas.Contains(t.IdNomina)
                                   && t.ErrorTimbrado == false
                                     && t.Cancelado == false
                             orderby t.RFCReceptor
                             select t).ToList();

                }

                return lista;
            }
        }
        public NOM_PeriodosPago GetPeriodoPagoById(int id)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == id);
            }
        }
        public List<C_Metodos_Pago> GetMetodosPagos()
        {
            using (var context = new RHEntities())
            {
                return context.C_Metodos_Pago.ToList();
            }
        }
        public NOM_Nomina GetNominaById(int idNomina)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_Nomina.FirstOrDefault(x => x.IdNomina == idNomina);
            }
        }
        public NOM_Finiquito GetFiniquitoById(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_Finiquito.FirstOrDefault(x => x.IdFiniquito == idFiniquito);
            }
        }
        public Empleado GetDatosReceptor(int idEmpleado)
        {
            using (var context = new RHEntities())
            {
                return context.Empleado.FirstOrDefault(x => x.IdEmpleado == idEmpleado);
            }
        }
        public void EliminarCfdiByIdNomina(int[] idNominas, int idEjercicio, int idPeriodo, int idSucursal)
        {
            if (idNominas == null) return;

            if (idNominas.Length <= 0) return;

            //eliminar de nominas
            var nominasIds = string.Join(",", idNominas);

            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_CFDI_Timbrado] WHERE IdNomina in ( " + nominasIds +
                                   " ) and IdEjercicio = @p0 and IdPeriodo = @p1 and IdSucursal = @p2 and ErrorTimbrado = 1 and FolioFiscalUUID is NULL ";
                context.Database.ExecuteSqlCommand(sqlQuery1, idEjercicio, idPeriodo, idSucursal);
            }

        }
        public void EliminarCfdiByIdFiniquito(int idFiniquito, int idEjercicio, int idPeriodo, int idSucursal)
        {
            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_CFDI_Timbrado] WHERE IdFiniquito in ( " + idFiniquito +
                                   " ) and IdEjercicio = @p0 and IdPeriodo = @p1 and IdSucursal = @p2 and ErrorTimbrado = 1 and FolioFiscalUUID is NULL ";
                context.Database.ExecuteSqlCommand(sqlQuery1, idEjercicio, idPeriodo, idSucursal);
            }
        }
        public byte[] getPDFBytesById(int id)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_CFDI_Timbrado.FirstOrDefault(x => x.IdTimbrado == id);

                return item.PDF;
            }
        }
        public List<byte[]> GetItemsPdfBytesByIdNomina(int[] idNominas)
        {
            using (var context = new RHEntities())
            {

                var listaBytes = (from t in context.NOM_CFDI_Timbrado
                                  where idNominas.Contains(t.IdNomina)
                                  select t.PDF).ToList();

                //var lista = (from t in _ctx.NOM_CFDI_Timbrado
                //             where idNominas.Contains(t.IdNomina)
                //             select t).ToList();

                return listaBytes;
            }
        }
        public bool GuardarPDF(int id, byte[] pdfBytes)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_CFDI_Timbrado.FirstOrDefault(x => x.IdTimbrado == id);
                if (item != null)
                {
                    item.PDF = pdfBytes;
                    context.SaveChanges();
                }
                return true;
            }
        }
        public void ActualizarCambiosTimbrado(NOM_CFDI_Timbrado item)
        {
            using (var context = new RHEntities())
            {
                try
                {
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }


        public void ActualizarCambiosTimbrado(List<NOM_CFDI_Timbrado> lista)
        {
            using (var context = new RHEntities())
            {
                try
                {
                    context.Entry(lista).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }



        public void ActualizarCambiosNomina(NOM_Nomina item)
        {
            using (var context = new RHEntities())
            {
                try
                {
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }
        public void ActualizarCambiosFiniquito(NOM_Finiquito item)
        {
            using (var context = new RHEntities())
            {
                try
                {
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }
        public Empresa GetEmpresaById(int idEmpresa)
        {
            using (var context = new RHEntities())
            {
                return context.Empresa.FirstOrDefault(x => x.IdEmpresa == idEmpresa);
            }
        }
        public C_RegimenFiscal_SAT GetRegimenFiscalById(int idRegimen)
        {
            using (var context = new RHEntities())
            {
                return context.C_RegimenFiscal_SAT.FirstOrDefault(x => x.IdRegimenFiscal == idRegimen);
            }
        }

        /// <summary>
        /// Retorna las percepciones que estan ligados al IdNomina
        /// </summary>
        /// <param name="idNomina"></param>
        /// <returns></returns>
        public List<ConceptosNomina> GetPercepcionesByIdNomina(int idNomina)
        {
            using (var context = new RHEntities())
            {
                var r = (from nd in context.NOM_Nomina_Detalle
                         join c in context.C_NOM_Conceptos on nd.IdConcepto equals c.IdConcepto
                         //join sat in _ctx.C_NOM_Conceptos_SAT on c.IdConceptoSAT equals sat.IdConceptoSAT
                         where
                         c.TipoConcepto == 1 && nd.IdNomina == idNomina && c.IdTipoOtroPago == 0 && nd.Complemento == false
                         && nd.Total > 0
                         select new ConceptosNomina()
                         {
                             IdConcepto = nd.IdConcepto,
                             NombreConcepto = c.DescripcionCorta,
                             Importe = nd.Total,
                             Gravado = nd.GravadoISR,
                             Excento = nd.ExentoISR,
                             ClaveSat = c.Clave
                         }).ToList();

                return r;
            }
        }
        public List<ConceptosNomina> GetPercepcionesByIdFiniquito(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var r = (from fd in context.NOM_Finiquito_Detalle
                         join c in context.C_NOM_Conceptos on fd.IdConcepto equals c.IdConcepto
                         //join sat in _ctx.C_NOM_Conceptos_SAT on c.IdConceptoSAT equals sat.IdConceptoSAT
                         where c.TipoConcepto == 1 && fd.IdFiniquito == idFiniquito && c.IdTipoOtroPago == 0
                         && fd.Total > 0
                         select new ConceptosNomina()
                         {
                             IdConcepto = fd.IdConcepto,
                             NombreConcepto = c.DescripcionCorta,
                             Importe = fd.Total,
                             Gravado = fd.GravadoISR,
                             Excento = fd.ExentoISR,
                             ClaveSat = c.Clave
                         }).ToList();

                return r;
            }
        }
        public List<ConceptosNomina> GetOtrosPagosByIdNomina(int idNomina)
        {
            using (var context = new RHEntities())
            {
                var r = (from nd in context.NOM_Nomina_Detalle
                         join c in context.C_NOM_Conceptos on nd.IdConcepto equals c.IdConcepto
                         //join sat in _ctx.C_NOM_Conceptos_SAT on c.IdConceptoSAT equals sat.IdConceptoSAT
                         join otroPago in context.C_TipoOtroPago_SAT on c.IdTipoOtroPago equals otroPago.IdTipoOtroPago
                         where c.TipoConcepto == 1 && nd.IdNomina == idNomina && c.IdTipoOtroPago > 0 && nd.Total > 0
                         select new ConceptosNomina()
                         {
                             IdConcepto = nd.IdConcepto,
                             NombreConcepto = c.DescripcionCorta,
                             Importe = nd.Total,
                             Gravado = nd.GravadoISR,
                             Excento = nd.ExentoISR,
                             ClaveSat = c.Clave,
                             IdTipoOtroPago = c.IdTipoOtroPago,
                             ClaveContable = c.Cuenta_Acredora,
                             //<- confirmar si se debe usar la cuenta acredora o deudora de contabilidad
                             ClaveOtroPago = otroPago.Clave,
                             saldo = nd.SaldoFavor,
                             remanente = nd.Remanente
                             
                         }).ToList();

                return r;
            }
        }
        public List<ConceptosNomina> GetOtrosPagosByIdFiniquito(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var r = (from fd in context.NOM_Finiquito_Detalle
                         join c in context.C_NOM_Conceptos on fd.IdConcepto equals c.IdConcepto
                         join otroPago in context.C_TipoOtroPago_SAT on c.IdTipoOtroPago equals otroPago.IdTipoOtroPago
                         where c.TipoConcepto == 1 && fd.IdFiniquito == idFiniquito && c.IdTipoOtroPago > 0 && fd.Total > 0
                         select new ConceptosNomina()
                         {
                             IdConcepto = fd.IdConcepto,
                             NombreConcepto = c.DescripcionCorta,
                             Importe = fd.Total,
                             Gravado = fd.GravadoISR,
                             Excento = fd.ExentoISR,
                             ClaveSat = c.Clave,
                             IdTipoOtroPago = c.IdTipoOtroPago,
                             ClaveContable = c.Cuenta_Acredora,
                             //<- confirmar si se debe usar la cuenta acredora o deudora de contabilidad
                             ClaveOtroPago = otroPago.Clave
                         }).ToList();

                return r;
            }
        }
        public IndemnizacionData GetDatosIndemnizacionByIdFiniquito(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                IndemnizacionData item = (from indem in context.NOM_Finiquito
                                          where indem.IdFiniquito == idFiniquito
                                          select new IndemnizacionData()
                                          {
                                              Total = indem.SubTotal_Liquidacion, //indem.Total_Liquidacion + indem.Subsidio_Liquidacion,
                                              AniosServicios = indem.L_AniosServicio,
                                              UltimoSueldoMensOrd = indem.L_UltimoSueldoMensualOrd,
                                              IngresoAcumulable = indem.L_IngresoAcumulable,
                                              IngresoNoAcumulable = indem.L_IngresoNoAcumulable
                                          }).FirstOrDefault();

                return item;
            }
        }

        public List<NOM_Incapacidad> GetDatosIncapacidad(int idNomina)
        {
            using (var context = new RHEntities())
            {
                var lista = context.NOM_Incapacidad.Where(x => x.IdNomina == idNomina).ToList();
                return lista;
            }
        }

        public List<NOM_Incapacidad> GetDatosIncapacidadFiniquitos(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var lista = context.NOM_Incapacidad.Where(x => x.IdFiniquito == idFiniquito).ToList();
                return lista;
            }
        }


        /// <summary>
        /// Retorna las deducciones que estan ligados al IdNomina
        /// </summary>
        /// <param name="idNomina"></param>
        /// <returns></returns>
        public List<ConceptosNomina> GetDeduccionesByIdNomina(int idNomina)
        {
            using (var context = new RHEntities())
            {
                var r = (from nd in context.NOM_Nomina_Detalle
                         join c in context.C_NOM_Conceptos on nd.IdConcepto equals c.IdConcepto
                         //  join sat in _ctx.C_NOM_Conceptos_SAT on c.IdConceptoSAT equals sat.IdConceptoSAT
                         where c.TipoConcepto == 2 && nd.IdNomina == idNomina
                               && nd.Total > 0
                         select new ConceptosNomina()
                         {
                             IdConcepto = nd.IdConcepto,
                             NombreConcepto = c.DescripcionCorta,
                             Importe = nd.Total,
                             Gravado = nd.GravadoISR,
                             Excento = nd.ExentoISR,
                             ClaveSat = c.Clave
                         }).ToList();


                return r;
            }
        }
        public List<ConceptosNomina> GetDeduccionesByIdFiniquito(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var r = (from fd in context.NOM_Finiquito_Detalle
                         join c in context.C_NOM_Conceptos on fd.IdConcepto equals c.IdConcepto
                         //  join sat in _ctx.C_NOM_Conceptos_SAT on c.IdConceptoSAT equals sat.IdConceptoSAT
                         where c.TipoConcepto == 2 && fd.IdFiniquito == idFiniquito
                               && fd.Total > 0
                         select new ConceptosNomina()
                         {
                             IdConcepto = fd.IdConcepto,
                             NombreConcepto = c.DescripcionCorta,
                             Importe = fd.Total,
                             Gravado = fd.GravadoISR,
                             Excento = fd.ExentoISR,
                             ClaveSat = c.Clave,
                             IsLiquidacion = fd.Liq
                         }).ToList();


                return r;
            }
        }
        public bool GuardarDatosCfdi(NOM_CFDI_Timbrado registroNuevo)
        {
            using (var context = new RHEntities())
            {
                var result = false;

                context.NOM_CFDI_Timbrado.Add(registroNuevo);
                var r = context.SaveChanges();

                if (r > 0)
                    result = true;

                return result;
            }
        }
        public bool ActualizarFolios(int foliosUsados)
        {
            using (var context = new RHEntities())
            {
                var item = context.SYA_GlobalConfig.FirstOrDefault(x => x.Id == 1);

                if (item == null) return false;

                var actual = item.Folios - foliosUsados;

                item.Folios = actual;

                var r = context.SaveChanges();

                return r > 0;
            }

        }

        /// <summary>
        /// Actualiza el campo CFDICreado de la tabla de nominas,
        /// de cada nomina timbrada
        /// </summary>
        /// <param name="idNominas"></param>
        /// <returns></returns>
        public bool ActualizarNominaCfdiCreado(List<int> idNominas, NOM_PeriodosPago ppPago, bool isFiniquito = false)
        {
            if (idNominas == null) return false;

            using (var context = new RHEntities())
            {
                var itemPeriodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == ppPago.IdPeriodoPago);
                if (itemPeriodo != null) itemPeriodo.CfdiGenerado = (int) GenerarCfdiEstatus.Generado;

                foreach (var item in idNominas)
                {
                    if (isFiniquito)
                    {
                        var f = context.NOM_Finiquito.FirstOrDefault(x => x.IdFiniquito == item);
                        if (f != null)
                        {
                            f.CFDICreado = true;
                        }
                    }
                    else
                    {
                        var n = context.NOM_Nomina.FirstOrDefault(x => x.IdNomina == item);

                        if (n != null)
                        {
                            n.CFDICreado = true;
                        }
                    }

                }

                var r = context.SaveChanges();

                return r > 0;
            }
        }
        public bool ActualizarFoliosByEmpresa(int foliosUsados, int idEmpresa)
        {
            using (var context = new RHEntities())
            {
                var item = context.Empresa.FirstOrDefault(x => x.IdEmpresa == idEmpresa);
                if (item == null) return false;
                var actual = item.FolioUsados - foliosUsados;
                item.FolioUsados = actual;
                var r = context.SaveChanges();

                return r > 0;
            }
        }


        /// <summary>
        /// Retorna los datos de las nominas a crear xml
        /// </summary>
        /// <param name="idEjercicio"></param>
        /// <param name="idPeriodo"></param>
        /// <param name="nominasSeleccionadas"></param>
        /// <param name="tipoNomina"></param>
        /// <returns></returns>
        public List<NominaData> GetDatosNomina(int idEjercicio, int idPeriodo, int[] nominasSeleccionadas, int tipoNomina, ref List<NotificationSummary> listaSummaries)
        {
            List<C_Banco_SAT> listaBancos = new List<C_Banco_SAT>();
            List<DatosBancarios> listaDatosBancario = new List<DatosBancarios>();
            List<NominaData> listaNominas = new List<NominaData>();
            List<NOM_Nomina> filtro = new List<NOM_Nomina>();
            using (var context = new RHEntities())
            {
                //Se filtra las nominas del periodo que sean mayor a Cero, 
                // y que la nomina no este Generada
                //var filtroNominas = _ctx.NOM_Nomina.Where(x => x.IdEjercicio == idEjercicio && x.IdPeriodo == idPeriodo && x.TotalNomina > 0 && x.CFDICreado == false).ToList();
                var filtroNominas =
                    context.NOM_Nomina.Where(
                        x => x.IdEjercicio == idEjercicio && x.IdPeriodo == idPeriodo && x.TotalNomina > 0).ToList();

                if (filtroNominas.Count <= 0) return null;

                //aplicar el filtro a las nominas seleccionadas
                 filtro = (from f in filtroNominas where nominasSeleccionadas.Contains(f.IdNomina) select f).ToList();

                if (filtro.Count <= 0) return null;


                switch (tipoNomina)
                {
                    case 16://ASIMILADOS
                        #region ASIMILADO
                        listaNominas = (from nom in filtro
                                        join empr in context.Empresa on nom.IdEmpresaAsimilado equals empr.IdEmpresa
                                        join p in context.NOM_PeriodosPago on nom.IdPeriodo equals p.IdPeriodoPago

                                        join contra in context.Empleado_Contrato on nom.IdContrato equals contra.IdContrato
                                        join empl in context.Empleado on nom.IdEmpleado equals empl.IdEmpleado
                                        join puest in context.Puesto on contra.IdPuesto equals puest.IdPuesto
                                        join depa in context.Departamento on puest.IdDepartamento equals depa.IdDepartamento
                                        join mpago in context.C_Metodos_Pago on nom.IdMetodo_Pago equals mpago.IdMetodo

                                        join tipoContrato in context.C_TipoContrato_SAT on contra.TipoContrato equals
                                        tipoContrato.IdTipoContrato
                                        join tipoJornada in context.C_TipoJornada_SAT on contra.IdTipoJornada equals
                                        tipoJornada.IdTipoJornada
                                        join tipoRegimen in context.C_TipoRegimen_SAT on contra.IdTipoRegimen equals
                                        tipoRegimen.IdTipoRegimen
                                        join periodicidadPago in context.C_PeriodicidadPago_SAT on contra.IdPeriodicidadPago equals
                                        periodicidadPago.IdPeriodicidadPago
                                        join tipoNominasat in context.C_TipoNomina_SAT on p.IdTipoNominaSat equals
                                        tipoNominasat.IdTipoNomina
                                        // where contra.Status == true
                                        orderby empl.APaterno, empl.AMaterno
                                        select new NominaData()
                                        {
                                            IdNomina = nom.IdNomina,
                                            IdEjercicio = nom.IdEjercicio,
                                            IdPeriodo = nom.IdPeriodo,
                                            IdEmisor = empr.IdEmpresa,
                                            IdEmpresa = nom.IdEmpresaAsimilado,
                                            RfcEmisor = empr.RFC,
                                            IdSucursal = nom.IdSucursal,
                                            IdEmpleado = nom.IdEmpleado,
                                            Paterno = empl.APaterno,
                                            Materno = empl.AMaterno,
                                            Nombres = empl.Nombres,
                                            TotalNomina = nom.TotalNomina,
                                            TotalPercepciones = nom.TotalPercepciones,
                                            TotalDeducciones = nom.TotalDeducciones,
                                            TotalComplemento = nom.TotalComplemento,
                                            TotalOtrosPagos = nom.TotalOtrosPagos,
                                            Sd = nom.SD,
                                            Sdi = nom.SDI,
                                            Sbc = nom.SBC,
                                            DiasPagados =p.DiasPeriodo, //DiasPagados = nom.Dias_Laborados,  //Asimilado
                                            FechaAlta = contra.FechaAlta,
                                            Rfc = empl.RFC,
                                            Nss = empl.NSS,
                                            Curp = empl.CURP,
                                            TipoContrato = tipoContrato.Clave,
                                            TipoJornada = tipoJornada.Clave,
                                            Departamento = depa.Descripcion,
                                            PuestoOriginal = puest.Descripcion,
                                            PuestoRecibo = puest.PuestoRecibo,
                                            PeriodicidadPago = "99", //periodicidadPago.Clave,
                                            RegistroPatronal = empr.RegistroPatronal,
                                            ClaveMetodoPago = mpago.Clave,
                                            MetodoPago = mpago.Descripcion,
                                            NumeroDeCuenta = "0",
                                            TipoComprobante = "egreso",
                                            Serie = "A",
                                            FechaInicialdePago = p.Fecha_Inicio.ToString("yyyy-MM-dd"),
                                            FechaFinaldePago = p.Fecha_Fin.ToString("yyyy-MM-dd"),
                                            FechaDePago = p.Fecha_Pago.ToString("yyyy-MM-dd"),
                                            TipoRegimen = tipoRegimen.Clave, //Nomina = sueldo ,  asimilado = Asimilados Honorarios
                                            CveBanco = "0", //000 usar para pruebas --> 001 --no se usa banco.CveSat.ToString(),
                                            ClabeInterbancaria = "0",
                                            LugarExpedicion = "", //CP del  emisor 
                                            RiesgoPuesto = UtilsEmpleados.ValorRiesgoPuesto(empr.Clase),
                                            TipoDeNomina = p.IdTipoNomina,
                                            ClaveEntidadFederativa = contra.EntidadDeServicio.Trim(),
                                            TipoNominaSat = tipoNominasat.Clave.Trim(),
                                            SubsidioCausado = nom.SubsidioCausado,
                                            SubsidioEntregado = nom.SubsidioEntregado,
                                            XmlGeneradoPreRecibo = nom.XMLSinTimbre != null,
                                            PagoEnEfectivo = nom.IdMetodo_Pago == 1, //Pago en efectivo
                                            PorcentajeTiempo = nom.PorcentajeTiempo
                                        }).ToList();
#endregion
                        break;
                    case 17:
                        #region SINDICATO
                          
                        listaNominas = (from nom in filtro
                                        join empr in context.Empresa on nom.IdEmpresaSindicato equals empr.IdEmpresa
                                        join p in context.NOM_PeriodosPago on nom.IdPeriodo equals p.IdPeriodoPago

                                        join contra in context.Empleado_Contrato on nom.IdContrato equals contra.IdContrato
                                        join empl in context.Empleado on nom.IdEmpleado equals empl.IdEmpleado

                                        join puest in context.Puesto on contra.IdPuesto equals puest.IdPuesto
                                        join depa in context.Departamento on puest.IdDepartamento equals depa.IdDepartamento

                                        join mpago in context.C_Metodos_Pago on nom.IdMetodo_Pago equals mpago.IdMetodo

                                        join tipoContrato in context.C_TipoContrato_SAT on contra.TipoContrato equals
                                        tipoContrato.IdTipoContrato
                                        join tipoJornada in context.C_TipoJornada_SAT on contra.IdTipoJornada equals
                                        tipoJornada.IdTipoJornada
                                        join tipoRegimen in context.C_TipoRegimen_SAT on contra.IdTipoRegimen equals
                                        tipoRegimen.IdTipoRegimen
                                        join periodicidadPago in context.C_PeriodicidadPago_SAT on contra.IdPeriodicidadPago equals
                                        periodicidadPago.IdPeriodicidadPago
                                        join tipoNominasat in context.C_TipoNomina_SAT on p.IdTipoNominaSat equals
                                        tipoNominasat.IdTipoNomina
                                        //  where contra.Status == true
                                        orderby empl.APaterno, empl.AMaterno
                                        select new NominaData()
                                        {
                                            IdNomina = nom.IdNomina,
                                            IdEjercicio = nom.IdEjercicio,
                                            IdPeriodo = nom.IdPeriodo,
                                            IdEmisor = empr.IdEmpresa,
                                            IdEmpresa = nom.IdEmpresaSindicato,
                                            RfcEmisor = empr.RFC,
                                            IdSucursal = nom.IdSucursal,
                                            IdEmpleado = nom.IdEmpleado,
                                            Paterno = empl.APaterno,
                                            Materno = empl.AMaterno,
                                            Nombres = empl.Nombres,
                                            TotalNomina = nom.TotalNomina,
                                            TotalPercepciones = nom.TotalPercepciones,
                                            TotalDeducciones = nom.TotalDeducciones,
                                            TotalComplemento = nom.TotalComplemento,
                                            TotalOtrosPagos = nom.TotalOtrosPagos,
                                            Sd = nom.SD,
                                            Sdi = nom.SDI,
                                            Sbc = nom.SBC,
                                            DiasPagados= p.DiasPeriodo, //Sindicato DiasPagados = nom.Dias_Laborados, //
                                            FechaAlta = contra.FechaAlta,
                                            Rfc = empl.RFC,
                                            Nss = empl.NSS,
                                            Curp = empl.CURP,
                                            TipoContrato = tipoContrato.Clave,
                                            TipoJornada = tipoJornada.Clave,
                                            Departamento = depa.Descripcion,
                                            PuestoOriginal = puest.Descripcion,
                                            PuestoRecibo = puest.PuestoRecibo,
                                            PeriodicidadPago = periodicidadPago.Clave,
                                            RegistroPatronal = empr.RegistroPatronal,
                                            ClaveMetodoPago = mpago.Clave,
                                            MetodoPago = mpago.Descripcion,
                                            NumeroDeCuenta = "0",
                                            TipoComprobante = "egreso",
                                            Serie = "A",
                                            FechaInicialdePago = p.Fecha_Inicio.ToString("yyyy-MM-dd"),
                                            FechaFinaldePago = p.Fecha_Fin.ToString("yyyy-MM-dd"),
                                            FechaDePago = p.Fecha_Pago.ToString("yyyy-MM-dd"),
                                            TipoRegimen = tipoRegimen.Clave, //Nomina = sueldo ,  asimilado = Asimilados Honorarios
                                            CveBanco = "0", //000 usar para pruebas --> 001 --no se usa banco.CveSat.ToString(),
                                            ClabeInterbancaria = "0",
                                            LugarExpedicion = "", //CP del  emisor 
                                            RiesgoPuesto = UtilsEmpleados.ValorRiesgoPuesto(empr.Clase),
                                            TipoDeNomina = p.IdTipoNomina,
                                            ClaveEntidadFederativa = contra.EntidadDeServicio.Trim(),
                                            TipoNominaSat = tipoNominasat.Clave.Trim(),
                                            SubsidioCausado = nom.SubsidioCausado,
                                            SubsidioEntregado = nom.SubsidioEntregado,
                                            XmlGeneradoPreRecibo = nom.XMLSinTimbre != null,
                                            PagoEnEfectivo = nom.IdMetodo_Pago == 1, //Pago en efectivo
                                            PorcentajeTiempo = nom.PorcentajeTiempo
                                        }).ToList();
                        #endregion
                        break;
                    default:
                        #region NOMINAS

                        listaNominas = (from nom in filtro
                                        join empr in context.Empresa on nom.IdEmpresaFiscal equals empr.IdEmpresa
                                        join p in context.NOM_PeriodosPago on nom.IdPeriodo equals p.IdPeriodoPago
                                        join empl in context.Empleado on nom.IdEmpleado equals empl.IdEmpleado
                                        join mpago in context.C_Metodos_Pago on nom.IdMetodo_Pago equals mpago.IdMetodo

                                        join contra in context.Empleado_Contrato on nom.IdContrato equals contra.IdContrato

                                        join puest in context.Puesto on contra.IdPuesto equals puest.IdPuesto
                                        join depa in context.Departamento on puest.IdDepartamento equals depa.IdDepartamento
                                        join tipoContrato in context.C_TipoContrato_SAT on contra.TipoContrato equals
                                        tipoContrato.IdTipoContrato
                                        join tipoJornada in context.C_TipoJornada_SAT on contra.IdTipoJornada equals
                                        tipoJornada.IdTipoJornada
                                        join tipoRegimen in context.C_TipoRegimen_SAT on contra.IdTipoRegimen equals
                                        tipoRegimen.IdTipoRegimen
                                        join periodicidadPago in context.C_PeriodicidadPago_SAT on contra.IdPeriodicidadPago equals
                                        periodicidadPago.IdPeriodicidadPago

                                        join tipoNominasat in context.C_TipoNomina_SAT on p.IdTipoNominaSat equals
                                        tipoNominasat.IdTipoNomina
                                        // where contra.Status == true
                                        orderby empl.APaterno, empl.AMaterno
                                        select new NominaData()
                                        {
                                            IdNomina = nom.IdNomina,
                                            IdEjercicio = nom.IdEjercicio,
                                            IdPeriodo = nom.IdPeriodo,
                                            IdEmisor = empr.IdEmpresa,
                                            IdEmpresa = nom.IdEmpresaFiscal,
                                            RfcEmisor = empr.RFC,
                                            IdSucursal = nom.IdSucursal,
                                            IdEmpleado = nom.IdEmpleado,
                                            Paterno = empl.APaterno,
                                            Materno = empl.AMaterno,
                                            Nombres = empl.Nombres,
                                            TotalNomina = nom.TotalNomina,
                                            TotalPercepciones = nom.TotalPercepciones,
                                            TotalDeducciones = nom.TotalDeducciones,
                                            TotalComplemento = nom.TotalComplemento,
                                            TotalOtrosPagos = nom.TotalOtrosPagos,
                                            Sd = nom.SD,
                                            Sdi = nom.SDI,
                                            Sbc = nom.SBC,
                                            DiasPagados = nom.Dias_Laborados, //p.DiasPeriodo, //ABC
                                            FechaAlta = contra.FechaAlta,
                                            Rfc = empl.RFC,
                                            Nss = empl.NSS,
                                            Curp = empl.CURP,
                                            TipoContrato = tipoContrato.Clave,
                                            TipoJornada = tipoJornada.Clave,
                                            Departamento = depa.Descripcion,
                                            PuestoOriginal = puest.Descripcion,
                                            PuestoRecibo = puest.PuestoRecibo,
                                            PeriodicidadPago = periodicidadPago.Clave,
                                            RegistroPatronal = empr.RegistroPatronal,
                                            ClaveMetodoPago = mpago.Clave,
                                            MetodoPago = mpago.Descripcion,
                                            NumeroDeCuenta = "0",
                                            TipoComprobante = "egreso",
                                            Serie = "A",
                                            FechaInicialdePago = p.Fecha_Inicio.ToString("yyyy-MM-dd"),
                                            FechaFinaldePago = p.Fecha_Fin.ToString("yyyy-MM-dd"),
                                            FechaDePago = p.Fecha_Pago.ToString("yyyy-MM-dd"),
                                            TipoRegimen = tipoRegimen.Clave, //Nomina = sueldo ,  asimilado = Asimilados Honorarios
                                            CveBanco = "0", //000 usar para pruebas --> 001 --no se usa banco.CveSat.ToString(),
                                            ClabeInterbancaria = "0",
                                            LugarExpedicion = "", //CP del  emisor 
                                            RiesgoPuesto = UtilsEmpleados.ValorRiesgoPuesto(empr.Clase),
                                            TipoDeNomina = p.IdTipoNomina,
                                            ClaveEntidadFederativa = contra.EntidadDeServicio.Trim(),
                                            TipoNominaSat = tipoNominasat.Clave.Trim(),
                                            SubsidioCausado = nom.SubsidioCausado,
                                            SubsidioEntregado = nom.SubsidioEntregado,
                                            XmlGeneradoPreRecibo = nom.XMLSinTimbre != null,
                                            PagoEnEfectivo = nom.IdMetodo_Pago == 1, //Pago en efectivo
                                            PorcentajeTiempo = nom.PorcentajeTiempo
                                        }).ToList();
#endregion
                        break;
                } //final del switch


                //Buscar los bancos y los datos bancarios
                if (listaNominas.Count > 0)
                {
                    listaBancos = context.C_Banco_SAT.ToList();
                    var arrayIdEmpleados = listaNominas.Select(x => x.IdEmpleado).ToArray();

                    listaDatosBancario = (from d in context.DatosBancarios
                                          where arrayIdEmpleados.Contains(d.IdEmpleado)
                                          select d).ToList();
                }
            }


            //Asgina los datos Bancarios
            #region DATOS BANCARIOS

            foreach (var itemNomina in listaNominas)
            {
                string claveBanco = "00";
                string strCuenta = "";

                //Si el pago es en efectivo no se asigna cuentas bancarias
                if (itemNomina.PagoEnEfectivo == true)
                {
                    continue;
                }
                SetDatosBancarios(itemNomina.IdEmpleado, listaBancos, listaDatosBancario, out claveBanco, out strCuenta);

                itemNomina.CveBanco = claveBanco;
                itemNomina.NumeroDeCuenta = strCuenta;

                //46 Subsidio ya viene en el registro de la nomina, ya no es necesario buscarlo en el detalle
                //var itemDetalle =
                //    _ctx.NOM_Nomina_Detalle.FirstOrDefault(x => x.IdNomina == itemNomina.IdNomina && x.IdConcepto == 46);

                //if(itemDetalle == null) continue;
                //itemNomina.Subsidio = itemDetalle.Total;
            }

            #endregion

            //Si la lista de noominas encontradas no es la misma cantidad de registros del filtro
            if (listaNominas.Count != filtro.Count)
            {
                var listaNominasSinMetodoDePago =
                    filtro.Where(x => x.IdMetodo_Pago <= 0).ToList();

                foreach (var itemN in listaNominasSinMetodoDePago)
                {
                    listaSummaries.Add(new NotificationSummary() { Reg = 0, Msg1 = $"La nomina {itemN.IdNomina} no tiene asignado un Metodo de Pago. {itemN.IdMetodo_Pago}", Msg2 = "" });
                }
            }

            if (listaNominas.Count > 0)
            {
                listaNominas = listaNominas.OrderBy(x => x.IdNomina).ToList();
            }

            return listaNominas;



        }

        public int GetLastIdTimbrado()
        {
            int result = 0;
            using (var context = new RHEntities())
            {
                var item = context.NOM_CFDI_Timbrado.OrderByDescending(x => x.IdTimbrado).FirstOrDefault();

                if (item != null)
                {
                    result = item.IdTimbrado;
                }
            }
            return result;
        }

        public NominaData GetDatosFiniquito(int idEjercicio, int idPeriodo, int idFiniquito, ref List<NotificationSummary> listaSummaries)
        {
            List<C_Banco_SAT> listaBancos = new List<C_Banco_SAT>();
            List<DatosBancarios> listaDatosBancario = new List<DatosBancarios>();
            NominaData itemFiniquito = new NominaData();
            using (var context = new RHEntities())
            {
                var itemFin = context.NOM_Finiquito.Where(x => x.IdFiniquito == idFiniquito).ToList();
                var itemPeriodo = context.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).ToList();

                #region FINIQUITO
                //Se filtra las nominas del periodo que sean mayor a Cero, 
                // y que la nomina no este Generada       
                var itemFiniquitoL = (from fin in itemFin
                    join empr in context.Empresa on fin.IdEmpresaFiscal equals empr.IdEmpresa
                    join p in itemPeriodo on fin.IdPeriodo equals p.IdPeriodoPago
                    //join p in _ctx.NOM_PeriodosPago on fin.IdPeriodo equals p.IdPeriodoPago

                    join contra in context.Empleado_Contrato on fin.IdContrato equals contra.IdContrato
                    join empl in context.Empleado on fin.IdEmpleado equals empl.IdEmpleado
                    join puest in context.Puesto on contra.IdPuesto equals puest.IdPuesto
                    join depa in context.Departamento on puest.IdDepartamento equals depa.IdDepartamento
                    join mpago in context.C_Metodos_Pago on fin.IdMetodo_Pago equals mpago.IdMetodo

                    join tipoContrato in context.C_TipoContrato_SAT on contra.TipoContrato equals
                    tipoContrato.IdTipoContrato
                    join tipoJornada in context.C_TipoJornada_SAT on contra.IdTipoJornada equals
                    tipoJornada.IdTipoJornada
                    join tipoRegimen in context.C_TipoRegimen_SAT on contra.IdTipoRegimen equals
                    tipoRegimen.IdTipoRegimen

                    join periodicidadPago in context.C_PeriodicidadPago_SAT on p.IdTipoNomina equals
                    periodicidadPago.IdPeriodicidadPago
                    join tipoNominasat in context.C_TipoNomina_SAT on p.IdTipoNominaSat equals
                    tipoNominasat.IdTipoNomina
                    //where fin.IdFiniquito == idFiniquito && p.IdPeriodoPago == idPeriodo
                    // where contra.Status == true
                    orderby empl.APaterno, empl.AMaterno
                    select new NominaData()
                    {
                        IdFiniquito = fin.IdFiniquito,
                        IdEjercicio = p.IdEjercicio,
                        IdPeriodo = fin.IdPeriodo,
                        IdEmisor = empr.IdEmpresa,
                        IdEmpresa = fin.IdEmpresaFiscal,
                        RfcEmisor = empr.RFC,
                        IdSucursal = fin.IdSucursal,
                        IdEmpleado = fin.IdEmpleado,
                        Paterno = empl.APaterno,
                        Materno = empl.AMaterno,
                        Nombres = empl.Nombres,
                        TotalNomina = fin.TOTAL_total,
                        TotalPercepciones = fin.TotalPercepciones,
                        TotalDeducciones = fin.TotalDeducciones,
                        TotalComplemento = fin.TotalComplemento,
                        TotalOtrosPagos = fin.TotalOtrosPagos,
                        Sd = fin.SD,
                        Sdi = fin.SDI,
                        Sbc = fin.SBC,
                        DiasPagados = p.DiasPeriodo,//finiquito
                        FechaAlta = contra.FechaAlta,
                        Rfc = empl.RFC,
                        Nss = empl.NSS,
                        Curp = empl.CURP,
                        TipoContrato = tipoContrato.Clave,
                        TipoJornada = tipoJornada.Clave,
                        Departamento = depa.Descripcion,
                        PuestoOriginal = puest.Descripcion,
                        PuestoRecibo = puest.PuestoRecibo,
                        PeriodicidadPago = periodicidadPago.Clave,
                        RegistroPatronal = empr.RegistroPatronal,
                        ClaveMetodoPago = mpago.Clave,
                        MetodoPago = mpago.Descripcion,
                        NumeroDeCuenta = "0",
                        TipoComprobante = "egreso",
                        Serie = "A",
                        FechaInicialdePago = p.Fecha_Inicio.ToString("yyyy-MM-dd"),
                        FechaFinaldePago = p.Fecha_Fin.ToString("yyyy-MM-dd"),
                        FechaDePago = p.Fecha_Fin.ToString("yyyy-MM-dd"),
                        //p.Fecha_Fin.ToString("yyyy-MM-dd"),//  p.Fecha_Pago.ToString("yyyy-MM-dd"), //0001-01-01
                        TipoRegimen = tipoRegimen.Clave, //Nomina = sueldo ,  asimilado = Asimilados Honorarios
                        CveBanco = "0", //000 usar para pruebas --> "072" ////no se usa --> banco.CveSat.ToString(),
                        ClabeInterbancaria = "0",
                        LugarExpedicion = "", //CP del  emisor 
                        RiesgoPuesto = UtilsEmpleados.ValorRiesgoPuesto(empr.Clase),
                        TipoDeNomina = p.IdTipoNomina,
                        ClaveEntidadFederativa = contra.EntidadDeServicio.Trim(),
                        TipoNominaSat = tipoNominasat.Clave.Trim(),
                        SubsidioCausado = fin.SubsidioCausado,
                        SubsidioEntregado = fin.SubsidioEntregado,
                        XmlGeneradoPreRecibo = fin.XMLSinTimbre != null,
                        PagoEnEfectivo = fin.IdMetodo_Pago == 1, //Pago en efectivo
                        PorcentajeTiempo = 100.00M //agregar campo en tabla de finiquito
                    }).ToList();
                #endregion


                listaSummaries.AddRange(from itemF in itemFin
                    where itemF.IdMetodo_Pago <= 0
                    select
                    new NotificationSummary()
                    {
                        Reg = 0,
                        Msg1 =
                            $"El finiquito {itemF.IdFiniquito} no tiene asignado un metodo de pago. {itemF.IdMetodo_Pago}",
                        Msg2 = ""
                    });

                if (itemFiniquitoL.Count > 0)
                {
                    itemFiniquito = itemFiniquitoL[0];
                    
                    //Buscar los bancos y los datos bancarios
                    listaBancos = context.C_Banco_SAT.ToList();
                    var arrayIdEmpleados = itemFiniquitoL.Select(x => x.IdEmpleado).ToArray();

                    listaDatosBancario = (from d in context.DatosBancarios
                        where arrayIdEmpleados.Contains(d.IdEmpleado)
                        select d).ToList();
                }

            }

            if (itemFiniquito == null) return null;

                if (itemFiniquito.PagoEnEfectivo == true)
                {
                    return itemFiniquito;
                }

                string claveBanco = "00";
                string strCuenta = "0";

                SetDatosBancarios(itemFiniquito.IdEmpleado, listaBancos, listaDatosBancario, out claveBanco, out strCuenta);

                itemFiniquito.CveBanco = claveBanco;
                itemFiniquito.NumeroDeCuenta = strCuenta;

               
                return itemFiniquito;
            
        }
       private void SetDatosBancarios(int idEmpleado, List<C_Banco_SAT> listaBancos, List<DatosBancarios> listaDatosBancarios, out string cveBanco, out string strCuenta)
        {
           string noCuenta = "00";
            cveBanco = "00";
            bool setCuenta = false;

            //buscar en datos bancarios el dato bancario activo
            var itemDatosb = listaDatosBancarios.FirstOrDefault(x => x.IdEmpleado == idEmpleado);

            if (itemDatosb != null)
            {
                //Buscamos el banco
                var itemBanco = listaBancos.FirstOrDefault(x => x.IdBanco == itemDatosb.IdBanco);

                if (itemBanco != null)
                {
                    //establece la clave del banco
                    cveBanco = itemBanco.CveSat.ToString().PadLeft(3, '0');
                }

                //creo que esto no se usa
               

                //CUENTA BANCARIA
                if (itemDatosb.CuentaBancaria != null)
                {
                    //Revisa que tenga la longitud correcta
                    if (itemDatosb.CuentaBancaria.Length == 10 || itemDatosb.CuentaBancaria.Length == 11)
                    {
                        noCuenta = itemDatosb.CuentaBancaria;
                        setCuenta = true;
                    }
                }
                //NUMERO DE TARJETA
                if (setCuenta == false)
                {
                    if (itemDatosb.NumeroTarjeta != null)
                    {
                        if (itemDatosb.NumeroTarjeta.Length == 15 || itemDatosb.NumeroTarjeta.Length == 16)
                        {
                            noCuenta = itemDatosb.NumeroTarjeta;
                            setCuenta = true;
                        }
                    }
                }

                //CLABE
                if (setCuenta == false)
                {
                    if (itemDatosb.Clabe != null)
                    {
                        if (itemDatosb.Clabe.Length == 18)
                        {
                            noCuenta = itemDatosb.Clabe;
                        }
                    }
                }
            }

            strCuenta = noCuenta;
        }

        public void ActualizarCancelados(List<CancelarCfdi> uuidsACancelar, int idUsuario)
        {
            if (uuidsACancelar != null)
            {
                using (var context = new RHEntities())
                {
                    int cont = 0;
                    foreach (var itemCancelar in uuidsACancelar)
                    {
                        //buscamos el registro
                        var itemTimbrado =
                            context.NOM_CFDI_Timbrado.FirstOrDefault(
                                x => x.IdTimbrado == itemCancelar.IdTimbrado && x.Cancelado == false);

                        if (itemTimbrado == null) continue;

                        //Actualizamos sus datos
                        itemTimbrado.Cancelado = true;
                        itemTimbrado.UsuarioCancelo = idUsuario;
                        itemTimbrado.FechaCancelacion = DateTime.Now;

                    }

                    if (cont > 0)
                    {
                        context.SaveChanges();
                    }
                }

            }
        }

        public List<NOM_CFDI_Timbrado> GetCfdiTimbradosByArrayIdTimbre(int[] arrayIdTimbre)
        {
            using (var context = new RHEntities())
            {
                var lista = (from tim in context.NOM_CFDI_Timbrado
                             where arrayIdTimbre.Contains(tim.IdTimbrado)
                                   && tim.Cancelado == false
                                   && tim.FolioFiscalUUID != null
                             select tim).ToList();

                return lista;
            }

        }

        public void ActualizarCancelados(int[] arrayIdTimbrado, int idUsuario)
        {
            using (var context = new RHEntities())
            {
                //GET Timbrados Items
                var lista = (from tim in context.NOM_CFDI_Timbrado
                             where arrayIdTimbrado.Contains(tim.IdTimbrado)
                             select tim).ToList();

                int[] arrayIdNominas = lista.Where(x => x.IdNomina != 0).Select(x => x.IdNomina).ToArray();
                int[] arrayIdFiniquito = lista.Where(x => x.IdFiniquito != 0).Select(x => x.IdFiniquito).ToArray();


                var listaNominas = (from nom in context.NOM_Nomina
                                    where arrayIdNominas.Contains(nom.IdNomina)
                                    select nom).ToList();


                var listaFiniquitos = (from fin in context.NOM_Finiquito
                                       where arrayIdFiniquito.Contains(fin.IdFiniquito)
                                       select fin).ToList();

                foreach (var itemLista in lista)
                {
                    itemLista.Cancelado = true;
                    itemLista.FechaCancelacion = DateTime.Now;
                    itemLista.UsuarioCancelo = idUsuario;
                }

                foreach (var itemNom in listaNominas)
                {
                    itemNom.CFDICreado = false;
                }

                foreach (var itemFin in listaFiniquitos)
                {
                    itemFin.CFDICreado = false;
                }

                context.SaveChanges();
            }

        }

        public Cliente GetClienteByIdCliente(int idCliente)
        {
            using (var context = new RHEntities())
            {
                var item = context.Cliente.FirstOrDefault(x => x.IdCliente == idCliente);
                return item;
            }
        }

        //Version 2 Timbrado
        public List<Empresa> GetListaEmpresas()
        {
            using (var context = new RHEntities())
            {
                var lista = context.Empresa.ToList();

                return lista;
            }
        }

        public List<Empleado> GetListaEmpleados(int[] arrayIdEmp)
        {
            using (var context = new RHEntities())
            {
                var lista = (from e in context.Empleado
                    where arrayIdEmp.Contains(e.IdEmpleado)
                    select e).ToList();

                return lista;
            }
        }

        public List<ConceptosNomina> GetListaPercepciones(int[] arraynominas, bool esFiniquito)
        {
            if (esFiniquito)
            {
                using (var context = new RHEntities())
                {
                    var r = (from fd in context.NOM_Finiquito_Detalle
                        join c in context.C_NOM_Conceptos on fd.IdConcepto equals c.IdConcepto
                        //join sat in _ctx.C_NOM_Conceptos_SAT on c.IdConceptoSAT equals sat.IdConceptoSAT
                        where arraynominas.Contains(fd.IdFiniquito) &&
                              c.TipoConcepto == 1 && c.IdTipoOtroPago == 0
                              && fd.Total > 0
                        select new ConceptosNomina()
                        {
                            IdConcepto = fd.IdConcepto,
                            NombreConcepto = c.DescripcionCorta,
                            Importe = fd.Total,
                            Gravado = fd.GravadoISR,
                            Excento = fd.ExentoISR,
                            ClaveSat = c.Clave,
                            IdFiniquito = fd.IdFiniquito,
                            IdNomina = 0
                        }).ToList();

                    return r;
                }
            }
            else
            {


                using (var context = new RHEntities())
                {
                    var r = (from nd in context.NOM_Nomina_Detalle
                        join c in context.C_NOM_Conceptos on nd.IdConcepto equals c.IdConcepto
                        where arraynominas.Contains(nd.IdNomina) &&
                              c.TipoConcepto == 1 && c.IdTipoOtroPago == 0 && nd.Complemento == false
                              && nd.Total > 0
                        select new ConceptosNomina()
                        {
                            IdConcepto = nd.IdConcepto,
                            NombreConcepto = c.DescripcionCorta,
                            Importe = nd.Total,
                            Gravado = nd.GravadoISR,
                            Excento = nd.ExentoISR,
                            ClaveSat = c.Clave,
                            DiasHe = nd.DiasHE,
                            TipoHe = nd.TipoHE,
                            HorasExtrasAplicadas = nd.HorasE,
                            IdNomina = nd.IdNomina,
                            IdFiniquito = 0
                        }).ToList();

                    return r;
                }

            }
        }

        public List<ConceptosNomina> GetListaDeducciones(int[] arraynominas, bool esFiniquito)
        {

            if (esFiniquito)
            {
                using (var context = new RHEntities())
                {
                    var r = (from fd in context.NOM_Finiquito_Detalle
                        join c in context.C_NOM_Conceptos on fd.IdConcepto equals c.IdConcepto
                        where arraynominas.Contains(fd.IdFiniquito) && c.TipoConcepto == 2 && fd.Total > 0
                        select new ConceptosNomina()
                        {
                            IdConcepto = fd.IdConcepto,
                            NombreConcepto = c.DescripcionCorta,
                            Importe = fd.Total,
                            Gravado = fd.GravadoISR,
                            Excento = fd.ExentoISR,
                            ClaveSat = c.Clave,
                            IsLiquidacion = fd.Liq,
                            IdNomina = 0,
                            IdFiniquito = fd.IdFiniquito
                        }).ToList();


                    return r;
                }
            }
            else
            {
                using (var context = new RHEntities())
                {
                    var r = (from nd in context.NOM_Nomina_Detalle
                        join c in context.C_NOM_Conceptos on nd.IdConcepto equals c.IdConcepto
                        where arraynominas.Contains(nd.IdNomina) &&
                              c.TipoConcepto == 2 && nd.Total > 0
                        select new ConceptosNomina()
                        {
                            IdConcepto = nd.IdConcepto,
                            NombreConcepto = c.DescripcionCorta,
                            Importe = nd.Total,
                            Gravado = nd.GravadoISR,
                            Excento = nd.ExentoISR,
                            ClaveSat = c.Clave,
                            IdNomina = nd.IdNomina,
                            IdFiniquito = 0
                        }).ToList();


                    return r;
                }
            }
        }

        public List<ConceptosNomina> GetListaListaOtrosPagos(int[] arraynominas, bool esFiniquito)
        {
            if (esFiniquito)
            {
                using (var context = new RHEntities())
                {
                    var r = (from fd in context.NOM_Finiquito_Detalle
                        join c in context.C_NOM_Conceptos on fd.IdConcepto equals c.IdConcepto
                        where arraynominas.Contains(fd.IdFiniquito) &&
                              c.TipoConcepto == 1 && c.IdTipoOtroPago > 0
                              && fd.Total > 0
                        select new ConceptosNomina()
                        {
                            IdConcepto = fd.IdConcepto,
                            NombreConcepto = c.DescripcionCorta,
                            Importe = fd.Total,
                            Gravado = fd.GravadoISR,
                            Excento = fd.ExentoISR,
                            ClaveSat = c.Clave,
                            IdFiniquito = fd.IdFiniquito,
                            IdNomina = 0
                        }).ToList();

                    return r;
                }
            }
            else
            {
                using (var context = new RHEntities())
                {
                    var r = (from nd in context.NOM_Nomina_Detalle
                        join c in context.C_NOM_Conceptos on nd.IdConcepto equals c.IdConcepto
                        join otroPago in context.C_TipoOtroPago_SAT on c.IdTipoOtroPago equals otroPago.IdTipoOtroPago
                        where arraynominas.Contains(nd.IdNomina) &&
                              c.TipoConcepto == 1 && c.IdTipoOtroPago > 0 && nd.Total > 0
                        select new ConceptosNomina()
                        {
                            IdConcepto = nd.IdConcepto,
                            NombreConcepto = c.DescripcionCorta,
                            Importe = nd.Total,
                            Gravado = nd.GravadoISR,
                            Excento = nd.ExentoISR,
                            ClaveSat = c.Clave,
                            IdTipoOtroPago = c.IdTipoOtroPago,
                            ClaveContable = c.Cuenta_Acredora,
                            //<- confirmar si se debe usar la cuenta acredora o deudora de contabilidad
                            ClaveOtroPago = otroPago.Clave,
                            IdNomina = nd.IdNomina,
                            IdFiniquito = 0
                        }).ToList();

                    return r;
                }
            }
        }

        public List<NOM_Incapacidad> GetListaIncapacidadesGeneral(int[] arraynominas, bool esFiniquito)
        {
            if (esFiniquito)
            {
                using (var context = new RHEntities())
                {
                    var lista = (from i in context.NOM_Incapacidad
                        where arraynominas.Contains(i.IdFiniquito)
                        select i).ToList();
                    return lista;
                }
            }
            else
            {
                using (var context = new RHEntities())
                {
                    var lista = (from i in context.NOM_Incapacidad
                        where arraynominas.Contains(i.IdNomina)
                        select i).ToList();
                    return lista;
                }
            }
        }

        /// <summary>
        /// Guarda la informacion en la Tabla NOM_CFDI_Timbrado
        /// </summary>
        /// <param name="listaXmlGenerados"></param>
        /// <returns></returns>
        public bool InsertXmlToDb(List<NOM_CFDI_Timbrado> listaXmlGenerados)
        {
            using (var context = new RHEntities())
            {
                var result = false;

                context.NOM_CFDI_Timbrado.AddRange(listaXmlGenerados);
                var r = context.SaveChanges();

                if (r > 0)
                    result = true;

                return result;
            }
        }

        /// <summary>
        /// Actualiza el campo XML sin Timbre en las tablas NOM_Nomina o NOM_Finiquito
        /// </summary>
        /// <param name="listaXmlGenerados"></param>
        /// <param name="esFiniquito"></param>
        /// <returns></returns>
        public bool ActualizarXmlToTable(List<NOM_CFDI_Timbrado> listaXmlGenerados, bool esFiniquito)
        {
            bool result = false;
            int r = 0;
            using (var context = new RHEntities())
            {
                if (esFiniquito)
                {
                    foreach (var item in listaXmlGenerados)
                    {
                        var reg = context.NOM_Finiquito.FirstOrDefault(x => x.IdFiniquito == item.IdFiniquito);
                        if (reg != null) reg.XMLSinTimbre = item.XMLSinTimbre;
                    }
                    r = context.SaveChanges();
                }
                else
                {
                    foreach (var item in listaXmlGenerados)
                    {
                        var reg = context.NOM_Nomina.FirstOrDefault(x => x.IdNomina == item.IdNomina);
                        reg.XMLSinTimbre = item.XMLSinTimbre;
                    }
                    r = context.SaveChanges();
                }
            }

            if (r > 0)
                result = true;

            return result;
        }
    }


    /// <summary>
    /// Clase utilizada para obtener los datos de Percepcion o deduccion de una nomina
    /// </summary>
    public class ConceptosNomina
    {
        public int IdConcepto { get; set; }
        public string NombreConcepto { get; set; }
        public decimal Importe { get; set; }
        public decimal Gravado { get; set; }
        public decimal Excento { get; set; }
        public string ClaveSat { get; set; }
        public int IdTipoOtroPago { get; set; }
        public string ClaveContable { get; set; }
        public string ClaveOtroPago { get; set; }
        public bool IsLiquidacion { get; set; }
        //------------------------------------>
        public int? DiasHe { get; set; }
        public int? TipoHe { get; set; }
        public int? HorasExtrasAplicadas { get; set; }
        //-------------------------------------->
        public int IdNomina { get; set; }
        public int IdFiniquito { get; set; }
		
		  public decimal saldo { get; set; }
        public decimal remanente { get; set; }
    }
    public class IndemnizacionData
    {
        public decimal Total { get; set; }
        public int AniosServicios { get; set; }
        public decimal UltimoSueldoMensOrd { get; set; }
        public decimal IngresoAcumulable { get; set; }
        public decimal IngresoNoAcumulable { get; set; }
    }
}
