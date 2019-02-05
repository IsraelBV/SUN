using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Nomina.Procesador.Modelos;
using Common.Enums;
using Common.Utils;
using RH.Entidades.GlobalModel;

namespace Nomina.Procesador.Datos
{

    public class NominasDao
    {
        //private readonly RHEntities _ctx = null;

        public NominasDao()
        {
            //  _ctx = new RHEntities();
        }

        /// <summary>
        /// Metodo que valida el estatus actual del periodo.
        /// Actualiza su estatus a Procesando
        /// </summary>
        /// <param name="idPeriodoPago"></param>
        /// <param name="summaryList"></param>
        /// <returns></returns>
        public bool PeriodoDisponibleSetProcesando(int idPeriodoPago, ref List<NotificationSummary> summaryList)
        {
            using (var context = new RHEntities())
            {
                //Obtener el estatus actual del periodo
                var itemPeriodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodoPago);

                //VAL - Si el periodo esta Autorizado, o esta siendo procesado por otro usuario
                //ya no se podrá procesar la nomina y se suspende la ejecución del procesado
                if (itemPeriodo == null)
                {
                    summaryList.Add(new NotificationSummary() { Reg = idPeriodoPago, Msg1 = "Periodo no se encontró en la BD", Msg2 = "" });
                    return false;
                }


                if (itemPeriodo.Autorizado)
                {
                    summaryList.Add(new NotificationSummary() { Reg = idPeriodoPago, Msg1 = "Periodo ya esta Autorizado. No se puede volver a procesar.", Msg2 = "" });
                    return false;
                }

                if (itemPeriodo.Procesando)
                {
                    summaryList.Add(new NotificationSummary() { Reg = idPeriodoPago, Msg1 = "Periodo esta siendo procesado...", Msg2 = "" });
                    return false;
                }

                //Actualizar el periodo a procesando .
                itemPeriodo.Procesando = true;
                itemPeriodo.Procesado = false;
                itemPeriodo.Error_Procesado = false;
                context.SaveChanges();
                return true;
            }

        }
        public void ActualizarPeriodoProcesado(int idPeriodo, bool procesando, bool procesado, bool errorProcesado = false)
        {
            using (var context = new RHEntities())
            {
                var itemPeriodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);
                if (itemPeriodo != null)
                {
                    itemPeriodo.Error_Procesado = errorProcesado;
                    itemPeriodo.Procesando = procesando;
                    itemPeriodo.Procesado = procesado;
                    context.SaveChanges();
                }
            }

        }
        public NOM_Nomina CrearNomina(int idEjercicio = 0, int idPeriodo = 0, int idEmpleado = 0, decimal sd = 0, decimal sdi = 0, decimal sbc = 0, decimal sReal = 0, int idCliente = 0, int idSucursal = 0, int idMetodoPago = 0, int idContrato = 0, int usuario = 0)
        {
            NOM_Nomina item = new NOM_Nomina()
            {
                IdNomina = 0,
                IdEjercicio = idEjercicio,
                IdPeriodo = idPeriodo,
                IdEmpleado = idEmpleado,
                SD = sd,
                SDI = sdi,
                SDReal = sReal,
                TotalNomina = 0,
                TotalDeducciones = 0,
                TotalComplemento = 0,
                TotalPercepciones = 0,
                IdEmpresaFiscal = 0,
                IdEmpresaSindicato = 0,
                IdEmpresaAsimilado = 0,
                IdEmpresaComplemento = 0,
                IdCliente = idCliente,
                IdSucursal = idSucursal,
                IdMetodo_Pago = idMetodoPago,
                FechaReg = DateTime.Now,
                Dias_Laborados = 0,
                IdUsuario = usuario,
                IdContrato = idContrato,
                SBC = sbc,
                PorcentajeTiempo = 100//capturar desde el modulo de nomina

            };

            using (var context = new RHEntities())
            {
                context.NOM_Nomina.Add(item);

                context.SaveChanges();

                return item;

            }

        }

        public NOM_Nomina CrearNomina(NOM_Nomina itemNomina)
        {
            using (var context = new RHEntities())
            {
                context.NOM_Nomina.Add(itemNomina);
                var r = context.SaveChanges();

                return itemNomina;
            }
        }

        public NOM_Nomina ActualizarNominaDatosProcesado(NOM_Nomina nuevosValores)
        {
            if (nuevosValores == null) return null;

            using (var context = new RHEntities())
            {
                var itemNomina = context.NOM_Nomina.FirstOrDefault(x => x.IdNomina == nuevosValores.IdNomina);

                if (itemNomina == null) return null;

                //Pasamos los datos a la nomina

                itemNomina.Dias_Laborados = nuevosValores.Dias_Laborados;
                itemNomina.IdEmpresaFiscal = nuevosValores.IdEmpresaFiscal;
                itemNomina.IdEmpresaAsimilado = nuevosValores.IdEmpresaAsimilado;
                itemNomina.IdEmpresaComplemento = nuevosValores.IdEmpresaComplemento;
                itemNomina.IdEmpresaSindicato = nuevosValores.IdEmpresaSindicato;

                itemNomina.TotalNomina = nuevosValores.TotalNomina;
                itemNomina.TotalDeducciones = nuevosValores.TotalDeducciones;
                itemNomina.TotalPercepciones = nuevosValores.TotalPercepciones;
                itemNomina.TotalImpuestoSobreNomina = nuevosValores.TotalImpuestoSobreNomina;
                itemNomina.TotalObligaciones = nuevosValores.TotalObligaciones;
                itemNomina.TotalComplemento = nuevosValores.TotalComplemento;
                itemNomina.TotalOtrosPagos = nuevosValores.TotalOtrosPagos;
                itemNomina.SBCotizacionDelProcesado = nuevosValores.SBCotizacionDelProcesado;
                itemNomina.TipoTarifa = nuevosValores.TipoTarifa;
                itemNomina.SubsidioEntregado = nuevosValores.SubsidioEntregado;
                itemNomina.SubsidioCausado = nuevosValores.SubsidioCausado;

                itemNomina.ISRantesSubsidio = nuevosValores.ISRantesSubsidio;
                itemNomina.Prima_Riesgo = nuevosValores.Prima_Riesgo;
                itemNomina.SMGV = nuevosValores.SMGV;
                itemNomina.UMA = nuevosValores.UMA;

                context.SaveChanges();

                return itemNomina;
            }


        }
        public List<NOM_Nomina> GetNominasById(int[] idNominas)
        {
            using (var context = new RHEntities())
            {
                var lista = (from t in context.NOM_Nomina
                             where idNominas.Contains(t.IdNomina)
                             select t).ToList();
                return lista;
            }
        }

        public List<Empleado> GetEmpleadosByArray(int[] arrayEmpleados)
        {
            using (var context = new RHEntities())
            {
                var lista = (from e in context.Empleado
                             where arrayEmpleados.Contains(e.IdEmpleado)
                             select e).ToList();

                return lista;
            }
        }

        public NOM_Finiquito GetFiniquitoById(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_Finiquito.FirstOrDefault(x => x.IdFiniquito == idFiniquito);

                return item;
            }
        }
        public NOM_Finiquito_Complemento GetFiniquitoRealById(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_Finiquito_Complemento.FirstOrDefault(x => x.IdFiniquito == idFiniquito);

                return item;
            }
        }
        public List<NOM_Finiquito_Descuento_Adicional> GetDescuentosFiniquito(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_Finiquito_Descuento_Adicional.Where(x => x.IdPeriodo == idPeriodo && x.TipoConcepto == 2 && x.IsComplemento == true).ToList();

                return item;
            }
        }

        /// <summary>
        /// Metodo retorna la lista de ultimo contrato de cada empleado. 
        /// </summary>
        /// <param name="idEmplados"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<Empleado_Contrato> GetContratoEmpleados(int[] idEmplados, int idSucursal)
        {
            List<Empleado_Contrato> listaContratos = new List<Empleado_Contrato>();

            using (var context = new RHEntities())
            {
                foreach (var itemEmpleado in idEmplados)
                {
                    Empleado_Contrato itemContrato = null;
                    //Buscamos el contrato Activo mas Reciente
                    itemContrato =
                     context.Empleado_Contrato.Where(x => x.IdSucursal == idSucursal && x.IdEmpleado == itemEmpleado && x.Status == true)
                         .OrderByDescending(x => x.IdContrato)
                         .FirstOrDefault();


                    //Si no se encontro el contrato activo
                    //Buscamos el contrato Inactivo mas Reciente
                    if (itemContrato == null)
                    {
                        itemContrato =
                            context.Empleado_Contrato.Where(
                                    x => x.IdSucursal == idSucursal && x.IdEmpleado == itemEmpleado && x.Status == false)
                                .OrderByDescending(x => x.IdContrato)
                                .FirstOrDefault();
                    }


                    if (itemContrato == null) continue;

                    listaContratos.Add(itemContrato);
                }


                if (listaContratos.Count > 0)
                {
                    listaContratos = listaContratos.OrderBy(x => x.IdEmpleado).ToList();
                }

                return listaContratos;

            }
        }
        public List<NOM_Nomina_Ajuste> GetDatosDeAjustes(int[] arrayEmplados, int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista = (from na in context.NOM_Nomina_Ajuste
                             where arrayEmplados.Contains(na.IdEmpleado)
                                   && na.IdPeriodo == idPeriodo
                             select na).ToList();

                return lista;
            }
        }

        public List<NOM_Empleado_PeriodoPago> GetEmpleadoPeriodoPagos(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista = (from ep in context.NOM_Empleado_PeriodoPago
                             where ep.IdPeriodoPago == idPeriodo
                             select ep).ToList();

                return lista;
            }
        }
        public List<NOM_Empleado_Complemento> GetDatosComplementoDelPeriodo(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista = context.NOM_Empleado_Complemento.Where(x => x.IdPeriodo == idPeriodo).ToList();

                return lista;
            }
        }

        public List<NOM_Nomina_Detalle_C> GetDatosComplementoFiscal(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista = context.NOM_Nomina_Detalle_C.Where(x => x.IdPeriodo == idPeriodo).ToList();
                return lista;
            }
        }


        /// <summary>
        /// Metodo retorna el ultimo contrato del empleado.
        /// 
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public static Empleado_Contrato GetContratoEmpleado(int idEmpleado, int idSucursal)
        {
            using (var context = new RHEntities())
            {

                Empleado_Contrato itemContrato = null;
                //Buscamos el contrato Activo mas Reciente
                itemContrato =
                 context.Empleado_Contrato.Where(x => x.IdSucursal == idSucursal && x.IdEmpleado == idEmpleado && x.Status == true)
                     .OrderByDescending(x => x.IdContrato)
                     .FirstOrDefault();


                //Si no se encontro el contrato activo
                //Buscamos el contrato Inactivo mas Reciente
                if (itemContrato == null)
                {
                    itemContrato =
                        context.Empleado_Contrato.Where(
                                x => x.IdSucursal == idSucursal && x.IdEmpleado == idEmpleado && x.Status == false)
                            .OrderByDescending(x => x.IdContrato)
                            .FirstOrDefault();
                }


                //var item =
                //    _ctx.Empleado_Contrato.Where(
                //        x => x.IdEmpleado == idEmpleado && x.Status == true && x.IdSucursal == idSucursal)
                //        .OrderByDescending(x => x.IdContrato).FirstOrDefault();

                //var item =
                //    context.Empleado_Contrato.Where(
                //        x => x.IdEmpleado == idEmpleado && x.IdSucursal == idSucursal)
                //        .OrderByDescending(x => x.IdContrato).FirstOrDefault();


                return itemContrato;

            }



        }
        public List<EmpleadoConceptos> GetConceptosByIdEmpleado(int idEmpleado)
        {

            try
            {

                using (var context = new RHEntities())
                {
                    var lista = (from em in context.NOM_Empleado_Conceptos
                                 join cn in context.C_NOM_Conceptos on em.IdConcepto equals cn.IdConcepto
                                 where em.IdEmpleado == idEmpleado
                                 orderby cn.IdConcepto descending
                                 select new EmpleadoConceptos()
                                 {
                                     IdConcepto = cn.IdConcepto,
                                     TipoConcepto = cn.TipoConcepto,
                                     Descripcion = cn.DescripcionCorta,
                                     IsFormulaFiscal = em.Fiscal,
                                     IsFormulaComplemento = em.Complemento,
                                     //IsComplemento = cn.Complemento,
                                     IsImpuestoSobreNomina = cn.ImpuestoSobreNomina,
                                     IdTipoOtroPago = cn.IdTipoOtroPago
                                 }).ToList();

                    return lista;

                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public List<NOM_Empleado_Complemento> GetDatosConceptosComplemento(int idEmpleado, int idPeriodo)
        //{
        //    using (var context = new RHEntities())
        //    {

        //        return
        //            context.NOM_Empleado_Complemento.Where(
        //                x => x.IdEmpleado == idEmpleado && x.IdPeriodo == idPeriodo).ToList();

        //    }


        //}
        public NOM_Cuotas_IMSS AddCuotasImss(NOM_Cuotas_IMSS item)
        {
            using (var context = new RHEntities())
            {

                context.NOM_Cuotas_IMSS.Add(item);
                context.SaveChanges();

                return item;

            }


        }
        public void AddDetalleNomina(NOM_Nomina_Detalle item)
        {
            using (var context = new RHEntities())
            {

                context.NOM_Nomina_Detalle.Add(item);
                context.SaveChanges();

            }


        }

        public void AddDetalleNomina(NOM_Finiquito_Detalle item)
        {
            using (var context = new RHEntities())
            {
                context.NOM_Finiquito_Detalle.Add(item);
                context.SaveChanges();

            }

        }
        public void AddCalculoAnual(NOM_CalculoAnual item)
        {
            using (var context = new RHEntities())
            {

                context.NOM_CalculoAnual.Add(item);
                context.SaveChanges();

            }


        }

        //}
        //public void AddRangeDetalleNomina(List<NOM_Nomina_Detalle> lista)
        //{
        //    using (var context = new RHEntities())
        //    {
        //        context.NOM_Nomina_Detalle.AddRange(lista);
        //        context.SaveChanges();

        //    }


        //}
        public List<C_NOM_Tabla_IMSS> GeTablaImss()
        {
            using (var context = new RHEntities())
            {
                var lista = context.C_NOM_Tabla_IMSS.ToList();
                return lista;
            }
        }
        public List<NOM_Nomina> GetDatosNominas(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista = (from nom in context.NOM_Nomina
                             where nom.IdPeriodo == idPeriodo
                             select nom).ToList();


                return lista;
            }
        }
        public static List<NOM_Nomina_Detalle> GetDatosDetallesNominas(int[] arrayIdNominas)
        {
            if (arrayIdNominas.Length <= 0) return null;

            using (var context = new RHEntities())
            {
                var lista = (from nomD in context.NOM_Nomina_Detalle
                             where arrayIdNominas.Contains(nomD.IdNomina)
                             select nomD).ToList();
                return lista;
            }
        }
        public List<NOM_Cuotas_IMSS> GetDatosCuotasImss(int[] arrayIdNominas)
        {
            if (arrayIdNominas.Length <= 0) return null;

            using (var context = new RHEntities())
            {
                var lista = (from cuotas in context.NOM_Cuotas_IMSS
                             where arrayIdNominas.Contains(cuotas.IdNomina)
                             select cuotas).ToList();
                return lista;
            }
        }
        public Sucursal GetSucursal(int idSucursal)
        {
            using (var context = new RHEntities())
            {
                var item = context.Sucursal.FirstOrDefault(x => x.IdSucursal == idSucursal);
                return item;
            }
        }

        /// <summary>
        /// Retorna la zona de salario mas reciente y activa
        /// Si existen dos activa retorna el mas reciente.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ZonaSalario GetZonaSalario()
        {
            using (var context = new RHEntities())
            {
                var item = context.ZonaSalario.Where(x => x.Status == true).OrderByDescending(x => x.IdZonaSalario).FirstOrDefault();

                if (item == null)
                {
                    //Valor Default
                    ZonaSalario newItem = new ZonaSalario()
                    {
                        IdZonaSalario = 0,
                        SMG = (decimal)0,
                        UMA = (decimal)0,
                        LimiteMaxCotizacion = 0,
                        Zona = "",
                        Status = false
                    };

                    return newItem;
                }

                return item;

            }


        }
        public ParametrosConfig GetValorParametrosConfig(ParametrosDeConfiguracion configuracion)
        {
            var intNumerador = (int)configuracion;
            var strNumerador = ((ParametrosDeConfiguracion)intNumerador).ToString().ToUpper();

            using (var context = new RHEntities())
            {
                return context.ParametrosConfig.Where(x => x.Clave.Trim().ToUpper() == strNumerador).OrderByDescending(x => x.IdConfig).FirstOrDefault(); ;

            }
        }
        public NOM_Ejercicio_Fiscal GetEjercicioFiscal(int idE)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == idE);

            }

        }
        public Empresa GetEmpresa(int? id)
        {
            using (var context = new RHEntities())
            {
                return context.Empresa.FirstOrDefault(x => x.IdEmpresa == id);

            }
        }
        public C_Estado GetEstadoById(int id)
        {
            using (var context = new RHEntities())
            {
                return context.C_Estado.FirstOrDefault(x => x.IdEstado == id);
            }
        }
        public Empleado GetDatosEmpleado(int? id)
        {
            using (var context = new RHEntities())
            {
                return context.Empleado.FirstOrDefault(x => x.IdEmpleado == id);
            }
        }
        public Departamento GetDepartamentoById(int idDepto)
        {
            using (var context = new RHEntities())
            {
                return context.Departamento.FirstOrDefault(x => x.IdDepartamento == idDepto);
            }
        }
        public Puesto GetPuestoById(int idPuesto)
        {
            using (var context = new RHEntities())
            {
                return context.Puesto.FirstOrDefault(x => x.IdPuesto == idPuesto);
            }
        }

        //public void SaveChange()
        //{
        //    _ctx.SaveChanges();
        //}

        //public void SaveShangeAsync()
        //{
        //    _ctx.SaveChangesAsync();
        //}

        /// <summary>
        /// Retorna un array con los id de nomina de la lista de empleados.
        /// </summary>
        /// <param name="empleadoContratos"></param>
        /// <param name="idPeriodoPago"></param>
        /// <returns></returns>
        public int[] GetNominasIdByEmpleados(int[] empleadoIds, int idPeriodoPago)
        {
            using (var context = new RHEntities())
            {
                if (empleadoIds == null) return null;

                var lista = (from n in context.NOM_Nomina
                             where empleadoIds.Contains(n.IdEmpleado) && n.IdPeriodo == idPeriodoPago
                             && n.CFDICreado == false
                             select n.IdNomina).ToArray();

                return lista;

            }
        }

        public int[] GetEmpleadosIdCfdiGenerados(int[] empleadoIds, int idPeriodoPago)
        {
            using (var context = new RHEntities())
            {
                if (empleadoIds == null) return null;

                var lista = (from n in context.NOM_Nomina
                             where empleadoIds.Contains(n.IdEmpleado) && n.IdPeriodo == idPeriodoPago
                             && n.CFDICreado == true
                             select n.IdEmpleado).ToArray();
                return lista;

            }
        }

        /// <summary>
        /// Elimina los registros de nominas procesas - tablas : [NOM_Nomina], [NOM_Nomina_Detalle] y [NOM_Cuotas_IMSS]
        /// </summary>
        /// <param name="idNominas"></param>
        public static void EliminarNominasProcesadas(int[] idNominas)
        {
            if (idNominas == null) return;

            if (idNominas.Length <= 0) return;

            //eliminar de nominas
            var nominas = string.Join(",", idNominas);


            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_Nomina] WHERE IdNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery1);

                //eliminar de detalle de nomina
                string sqlQuery2 = "DELETE [NOM_Nomina_Detalle] WHERE IdNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery2);

                //eliminar de cuotas imss
                string sqlQuery3 = "DELETE [NOM_Cuotas_IMSS] WHERE IdNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery3);

                //eliminar de cuotas imss
                string sqlQuery4 = "DELETE [NOM_Incapacidad] WHERE IdNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery4);

            }

        }

        public static void EliminarAguinaldosProcesados(int[] idNominas)
        {
            if (idNominas == null) return;

            if (idNominas.Length <= 0) return;

            //eliminar de nominas
            var nominas = string.Join(",", idNominas);


            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_Nomina] WHERE IdNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery1);

                //eliminar de detalle de nomina
                string sqlQuery2 = "DELETE [NOM_Nomina_Detalle] WHERE IdNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery2);

                //eliminar de cuotas imss
                string sqlQuery3 = "DELETE [NOM_Aguinaldo] WHERE IdNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery3);

            }

        }

        public static void EliminarCalculosAnuales(int[] idNominas)
        {
            if (idNominas == null) return;

            if (idNominas.Length <= 0) return;

            //eliminar de nominas
            var nominas = string.Join(",", idNominas);


            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_CalculoAnual] WHERE idNomina in (" + nominas + ")";
                context.Database.ExecuteSqlCommand(sqlQuery1);

               

            }

        }
        public static void EliminarCalculoAnual(int idEmpleado, int periodo)
        {
            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_CalculoAnual] WHERE idEmpleado = " + idEmpleado + " and idPeriodo="+periodo;
                context.Database.ExecuteSqlCommand(sqlQuery1);
            }

        }
        public static void EliminarAplicacionSaldo(int idEmpleado, int periodo)
        {
            using (var context = new RHEntities())
            {
                string sqlQuery1 = "DELETE [NOM_AplicacionSaldo] WHERE idEmpleado = " + idEmpleado + " and idPeriodo=" + periodo;
                context.Database.ExecuteSqlCommand(sqlQuery1);
            }

        }


        public NOM_PeriodosPago GetPeriodoPagoById(int id)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == id);
            }
        }
        public static C_NOM_Tabla_ISR GeTablaIsrByTipoNomina(int tipoNomina, decimal sbc)
        {
            using (var context = new RHEntities())
            {
                return context.C_NOM_Tabla_ISR.FirstOrDefault(
                                    x => x.TipoTarifa == tipoNomina && sbc >= x.Limite_Inferior && sbc <= x.Limite_Superior);

            }
        }
        public static List<C_NOM_Tabla_ISR> GetAllTablaIsr(int tipoTarifa)
        {
            using (var context = new RHEntities())
            {
                return context.C_NOM_Tabla_ISR.Where(x => x.TipoTarifa == tipoTarifa).ToList();

            }



        }
        public static C_NOM_Tabla_Subsidio GetTablaSubsidioByTipoNomina(int tipoNomina, decimal sbc)
        {
            using (var context = new RHEntities())
            {

                return context.C_NOM_Tabla_Subsidio.FirstOrDefault(
                    x => x.Tipo_Tarifa == tipoNomina && sbc >= x.Limite_Inferior && sbc <= x.Limite_Superior);

            }


        }
        public static List<C_NOM_Tabla_Subsidio> GetAllTablaSubsidios(int tipoTarifa)
        {
            using (var context = new RHEntities())
            {
                return context.C_NOM_Tabla_Subsidio.Where(x => x.Tipo_Tarifa == tipoTarifa).ToList();

            }



        }
        public static NOM_Ejercicio_Fiscal GetEjercicioFiscalByIdEjercicio(int id)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.IdEjercicio == id);

            }

        }
        public static NOM_PeriodosPago GetPeriodoById(int id)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == id);
            }
        }
        public static C_FactoresIntegracion GetFactoresIntegracionByAntiguedad(int añosAntiguedad)
        {
            añosAntiguedad = añosAntiguedad == 0 ? 1 : añosAntiguedad;

            using (var context = new RHEntities())
            {
                return context.C_FactoresIntegracion.Where(x => x.Antiguedad == añosAntiguedad).FirstOrDefault();
            }
        }

        public static void GuardarDetallesNomina(List<NOM_Nomina_Detalle> listaDetalles, List<Modelos.Nomina12.NominaIncapacidad> listaIncapacidades, int idNomina)
        {

            using (var context = new RHEntities())
            {
                //GUARDA EL DETALLE DE LA NOMINA
                if (listaDetalles.Count > 0)
                {
                    context.NOM_Nomina_Detalle.AddRange(listaDetalles);
                    context.SaveChanges();
                }

                //GUARDA LOS DATOS DE INCAPACIDAD QUE SE REGISTRARON PARA LA NOMINA ACTUAL
                if (listaIncapacidades == null) return;
                if (listaIncapacidades.Count <= 0) return;
                var lista = (from i in listaIncapacidades
                    select new NOM_Incapacidad()
                    {
                        Id = 0,
                        IdNomina = idNomina,
                        IdFiniquito = 0,
                        DiasIncapacidad = i.DiasIncapacidad,
                        IdTipoIncapacidad = Int32.Parse(i.TipoIncapacidad),
                        Importe = i.ImporteMonetario
                    }).ToList();

                context.NOM_Incapacidad.AddRange(lista);//ABC
                context.SaveChanges();
            }
        }

        public static void GuardarCalculoAnual(List<NOM_CalculoAnual> listaCalculo)
        {

            using (var context = new RHEntities())
            {
                //GUARDA los datos del calculo anual
                if (listaCalculo.Count > 0)
                {
                    context.NOM_CalculoAnual.AddRange(listaCalculo);
                    try
                    {
                        context.SaveChanges();
                    }catch(Exception e) { string sd = e.Message; }
                }
            }
        }
        public static void GuardarAguinaldos(List<NOM_Aguinaldo> listaAguinaldos)
        {
            if (listaAguinaldos.Count > 0)
            {
                using (var context = new RHEntities())
                {
                    context.NOM_Aguinaldo.AddRange(listaAguinaldos);
                    context.SaveChanges();
                }
            }

        }

        public static List<NOM_Empleado_Complemento> GetDataEmpleadoComplemento(int idPeriodo)
        {
            using (var context = new RHEntities())
            {

                var lista = (from t in context.NOM_Empleado_Complemento
                             where t.IdPeriodo == idPeriodo
                             select t).ToList();

                return lista;
            }
        }
        public static List<NOM_Empleado_Asimilado> GetDataEmpleadoAsimilados(int idPeriodo)
        {
            using (var context = new RHEntities())
            {

                var lista = (from t in context.NOM_Empleado_Asimilado
                             where t.IdPeriodo == idPeriodo
                             select t).ToList();

                return lista;
            }
        }
        public static List<NOM_Empleado_Sindicato> GetDataEmpleadoSindicato(int idPeriodo)
        {
            using (var context = new RHEntities())
            {

                var lista = (from t in context.NOM_Empleado_Sindicato
                             where t.IdPeriodo == idPeriodo
                             select t).ToList();

                return lista;
            }
        }
        public static List<int> GuardarNominasComplemento(List<NOM_Nomina> listaNominas)
        {
            List<int> lista = null;
            if (listaNominas.Count > 0)
            {
                using (var context = new RHEntities())
                {
                    context.NOM_Nomina.AddRange(listaNominas);
                    context.SaveChanges();

                    //get id nominas generadas
                    lista = listaNominas.Select(x => x.IdNomina).ToList();

                }

            }

            return lista;
        }
        public List<DetalleNominaComp> GetDetallesNominasCompByIdNomina(int idNomina)
        {
            using (var context = new RHEntities())
            {
                var lista2 = (from dc in context.NOM_Nomina_Detalle_C
                              join conce in context.C_NOM_Conceptos_C on dc.IdConcepto equals conce.IdConceptoC
                              where dc.IdNomina == idNomina
                              select new DetalleNominaComp
                              {
                                  IdDetalle = dc.Id,
                                  IdNomina = dc.IdNomina,
                                  IdConcepto = dc.IdConcepto,
                                  DescripcionConcepto = conce.Descripcion,
                                  Cantidad = dc.Cantidad,
                                  TipoConcepto = conce.TipoConcepto
                              }).ToList();

                return lista2;

                //var lista = context.NOM_Nomina_Detalle_C.Where(x => x.IdNomina == idNomina).ToList();

                //return lista;
                //var lista = (from nd in context.NOM_Nomina_Detalle_C
                //    join cn in context.C_NOM_Conceptos_C on cn.IdConceptoC equals nd.IdConcepto
                //    select nd).toList();

            }
        }

        public List<C_NOM_Conceptos_C> GetConceptosComplemento()
        {
            try
            {
                using (var context = new RHEntities())
                {
                    return context.C_NOM_Conceptos_C.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        public List<C_NOM_Conceptos> GetConceptosFiscales()
        {
            using (var context = new RHEntities())
            {
                var lista = context.C_NOM_Conceptos.ToList();

                return lista;

            }
        }
        public static List<NOM_PeriodosPago> GetIdPeriodosAutorizadosByMes(int mes, int año, int idEmpleado, int idContrato)
        {
            DateTime ini = new DateTime(año, mes, 1);
            var diasdelMes = System.DateTime.DaysInMonth(año, mes);

            DateTime fin = new DateTime(año, mes, diasdelMes);

            using (var context = new RHEntities())
            {
                var listaPeriodos = (from p in context.NOM_PeriodosPago
                                     where p.Fecha_Fin >= ini && p.Fecha_Fin <= fin
                                     && p.Autorizado == true
                                     && p.IdTipoNomina != 11 //finiquito
                                     && p.IdTipoNomina != 12 //aguinaldo
                                     && p.IdTipoNomina != 17 //sindicato ? 16 asimilado 
                                     select p).ToList();

                var arrayPeriodos = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                var filtroPorEmpleado = (from pe in context.NOM_Empleado_PeriodoPago
                    where arrayPeriodos.Contains(pe.IdPeriodoPago)
                          && pe.IdEmpleado == idEmpleado
                    select pe.IdPeriodoPago).ToArray();


                var listaFinal = (from p in listaPeriodos
                    where filtroPorEmpleado.Contains(p.IdPeriodoPago)
                    select p).ToList();



                return listaFinal;
            }

        }
        public static int[] GetIdNominasByPeriodo(int[] periodos, int idEmpleado, int idContrato)
        {
            if (periodos == null) return null;
            if (periodos.Length <= 0) return null;

            using (var context = new RHEntities())
            {
                var arrayNominas = (from n in context.NOM_Nomina
                                    where periodos.Contains(n.IdPeriodo)
                                          && n.IdEmpleado == idEmpleado && n.IdContrato == idContrato
                                    select n.IdNomina).ToArray();

                return arrayNominas;
            }
        }

        public static List<NOM_Nomina> GetNominasByPeriodo(int[] periodos, int idEmpleado, int idContrato)
        {
            if (periodos == null) return null;
            if (periodos.Length <= 0) return null;

            using (var context = new RHEntities())
            {
                var arrayNominas = (from n in context.NOM_Nomina
                                    where periodos.Contains(n.IdPeriodo)
                                          && n.IdEmpleado == idEmpleado && n.IdContrato == idContrato
                                    select n).ToList();

                return arrayNominas;
            }
        }

        public static decimal GetGravadosByNominas(List<NOM_Nomina_Detalle> lista)
        {
            if (lista == null) return 0;
            if (lista.Count <= 0) return 0;

            var sumaGravado = (from nd in lista
                               select nd.GravadoISR).Sum();
            return sumaGravado;

        }

        public static decimal GetGravadosSueldosByNominas(List<NOM_Nomina_Detalle> lista)
        {
            if (lista == null) return 0;
            if (lista.Count <= 0) return 0;

            var sumaGravado = (from nd in lista
                               where nd.IdConcepto == 1//sueldo
                               select nd.GravadoISR).Sum();
            return sumaGravado;

        }

        public static decimal GetSBCByNominas(int[] arrayNominas)
        {
            if (arrayNominas == null) return 0;
            if (arrayNominas.Length <= 0) return 0;

            using (var context = new RHEntities())
            {
                var sumaGravado = (from nd in context.NOM_Nomina
                                   where arrayNominas.Contains(nd.IdNomina)
                                   select nd.SBCotizacionDelProcesado).Sum();
                return sumaGravado;
            }
        }
        public static decimal GetSubsidiosByNominas(List<NOM_Nomina_Detalle> lista)
        {
            if (lista == null) return 0;
            if (lista.Count <= 0) return 0;

            var sumaGravado = (from nd in lista
                               where nd.IdConcepto == 144
                               select nd.Total).Sum();
            return sumaGravado;

        }
        public static decimal GetIsrsByNominas(List<NOM_Nomina_Detalle> lista)
        {
            if (lista == null) return 0;
            if (lista.Count <= 0) return 0;

            var sumaGravado = (from nd in lista
                               where nd.IdConcepto == 43
                               select nd.Total).Sum();
            return sumaGravado;

        }
        public static Empleado_Contrato GetContratoByIdContrato(int idContrato)
        {
            using (var context = new RHEntities())
            {
                var item = context.Empleado_Contrato.FirstOrDefault(x => x.IdContrato == idContrato);

                return item;
            }
        }

        #region INFONAVIT - FONACOT

        public static Empleado_Infonavit GetPrestamoInfonavitByIdContrato(int idContrato)
        {
            using (var context = new RHEntities())
            {
                return context.Empleado_Infonavit.FirstOrDefault(x => x.IdEmpleadoContrato == idContrato && x.Status == true);

                //return _ctx.Prestamos.FirstOrDefault(x => x.IdEmpleadoContrato == idContrato && x.TipoPrestamo == 1 && x.Status == true);

            }
        }

        public static List<Empleado_Fonacot> GetPrestamosFonacotByIdContrato(int idContrato)
        {
            using (var context = new RHEntities())
            {

                return context.Empleado_Fonacot.Where(x => x.IdEmpleadoContrato == idContrato && x.Status == true).ToList();
                //  return _ctx.Prestamos.Where(x => x.IdEmpleadoContrato == idContrato && x.TipoPrestamo == 2 && x.Status == true).ToList();
            }



        }


        #endregion

        #region FINIQUITO - INDEMNIZACION

        public NOM_Finiquito AddFiniquito(NOM_Finiquito item)
        {
            using (var context = new RHEntities())
            {
                context.NOM_Finiquito.Add(item);
                context.SaveChanges();
                return item;

            }



        }

        public NOM_Finiquito UpdateFiniquitoProcesado(NOM_Finiquito itemNuevo)
        {
            if (itemNuevo == null) return null;

            using (var context = new RHEntities())
            {
                var itemFiniquito = context.NOM_Finiquito.FirstOrDefault(x => x.IdFiniquito == itemNuevo.IdFiniquito);

                if (itemFiniquito == null)
                    return null;

                itemFiniquito.IdPeriodo = itemNuevo.IdPeriodo;
                itemFiniquito.IdContrato = itemNuevo.IdContrato;
                itemFiniquito.IdEmpleado = itemNuevo.IdEmpleado;
                itemFiniquito.IdSucursal = itemNuevo.IdSucursal;
                itemFiniquito.IdCliente = itemNuevo.IdCliente;
                itemFiniquito.SalarioMinimo = itemNuevo.SalarioMinimo;
                itemFiniquito.SueldoPrimaAntiguedad = itemNuevo.SueldoPrimaAntiguedad;
                itemFiniquito.FechaAlta = itemNuevo.FechaAlta;
                itemFiniquito.FechaBaja = itemNuevo.FechaBaja;
                itemFiniquito.FechaInicioAguinaldo = itemNuevo.FechaInicioAguinaldo;
                itemFiniquito.FechaInicioVacacion = itemNuevo.FechaInicioVacacion;
                itemFiniquito.DiasLaborados = itemNuevo.DiasLaborados;
                itemFiniquito.DiasVacacionesParaCalculo = itemNuevo.DiasVacacionesParaCalculo;
                itemFiniquito.DiasAguinaldoParaCalculo = itemNuevo.DiasAguinaldoParaCalculo;
                itemFiniquito.AguinaldoCorrespondiente = itemNuevo.AguinaldoCorrespondiente;
                itemFiniquito.VacacionesCorrespondiente = itemNuevo.VacacionesCorrespondiente;
                itemFiniquito.PrimaVacacional = itemNuevo.PrimaVacacional;
                itemFiniquito.SD = itemNuevo.SD;
                itemFiniquito.SDI = itemNuevo.SDI;
                itemFiniquito.SBC = itemNuevo.SBC;
                itemFiniquito.DiasEjercicioFiscal = itemNuevo.DiasEjercicioFiscal;
                itemFiniquito.L_3Meses_SueldoParaCalculo = itemNuevo.L_3Meses_SueldoParaCalculo;
                itemFiniquito.VacacionesPendientes = itemNuevo.VacacionesPendientes;
                itemFiniquito.PrimaVacacionPendiente = itemNuevo.PrimaVacacionPendiente;
                itemFiniquito.DiasSueldoPendiente = itemNuevo.DiasSueldoPendiente;
                itemFiniquito.ProporcionalAguinaldo = itemNuevo.ProporcionalAguinaldo;
                itemFiniquito.ProporcionalVacaciones = itemNuevo.ProporcionalVacaciones;
                itemFiniquito.Proporcional20DiasPorAño = itemNuevo.Proporcional20DiasPorAño;
                itemFiniquito.ProporcionalPrimaAntiguedad = itemNuevo.ProporcionalPrimaAntiguedad;
                itemFiniquito.SUELDO = itemNuevo.SUELDO;
                itemFiniquito.VACACIONES = itemNuevo.VACACIONES;
                itemFiniquito.PRIMA_VACACIONAL = itemNuevo.PRIMA_VACACIONAL;
                itemFiniquito.AGUINDALDO = itemNuevo.AGUINDALDO;
                itemFiniquito.VACACIONES_PENDIENTE = itemNuevo.VACACIONES_PENDIENTE;
                itemFiniquito.PRIMA_VACACIONES_PENDIENTE = itemNuevo.PRIMA_VACACIONES_PENDIENTE;
                itemFiniquito.GRATIFICACION = itemNuevo.GRATIFICACION;
                itemFiniquito.L_3MESES_SUELDO = itemNuevo.L_3MESES_SUELDO;
                itemFiniquito.L_20DIAS_POR_AÑO = itemNuevo.L_20DIAS_POR_AÑO;
                itemFiniquito.PRIMA_ANTIGUEDAD = itemNuevo.PRIMA_ANTIGUEDAD;
                itemFiniquito.COMPENSACION_X_INDEMNIZACION = itemNuevo.COMPENSACION_X_INDEMNIZACION;
                itemFiniquito.Total_Liquidacion = itemNuevo.Total_Liquidacion;
                itemFiniquito.SubTotal_Liquidacion = itemNuevo.SubTotal_Liquidacion;
                itemFiniquito.ISR_Liquidacion = itemNuevo.ISR_Liquidacion;
                itemFiniquito.Subsidio_Liquidacion = itemNuevo.Subsidio_Liquidacion;
                itemFiniquito.SubTotal_Finiquito = itemNuevo.SubTotal_Finiquito;
                itemFiniquito.Total_Finiquito = itemNuevo.Total_Finiquito;
                itemFiniquito.ISR_Finiquito = itemNuevo.ISR_Finiquito;
                itemFiniquito.Subsidio_Finiquito = itemNuevo.Subsidio_Finiquito;
                /**********************************************************************/
                itemFiniquito.TOTAL_total = itemNuevo.TOTAL_total;
                /**********************************************************************/
                itemFiniquito.BaseGravableUltimoSueldo = itemNuevo.BaseGravableUltimoSueldo;
                itemFiniquito.ISR_UltimoSueldo = itemNuevo.ISR_UltimoSueldo;
                itemFiniquito.TasaParaLiquidacion = itemNuevo.TasaParaLiquidacion;
                itemFiniquito.Prima_Riesgo = itemNuevo.Prima_Riesgo;
                itemFiniquito.SubTotal_FiniquitoF = itemNuevo.SubTotal_FiniquitoF;
                itemFiniquito.ISR_FiniquitoF = itemNuevo.ISR_FiniquitoF;
                itemFiniquito.TOTAL_totalF = itemNuevo.TOTAL_totalF;
                itemFiniquito.Total_FiniquitoF = itemNuevo.Total_FiniquitoF;
                itemFiniquito.ISR_LiquidacionF = itemNuevo.ISR_LiquidacionF;
                itemFiniquito.Total_LiquidacionF = itemNuevo.Total_LiquidacionF;
                itemFiniquito.SubTotal_LiquidacionF = itemNuevo.SubTotal_LiquidacionF;
                itemFiniquito.Subsidio_FiniquitoF = itemNuevo.Subsidio_FiniquitoF;
                itemFiniquito.Subsidio_LiquidacionF = itemNuevo.Subsidio_LiquidacionF;
                itemFiniquito.L_AniosServicio = itemNuevo.L_AniosServicio;
                itemFiniquito.L_UltimoSueldoMensualOrd = itemNuevo.L_UltimoSueldoMensualOrd;
                itemFiniquito.L_IngresoAcumulable = itemNuevo.L_IngresoAcumulable;
                itemFiniquito.L_IngresoNoAcumulable = itemNuevo.L_IngresoNoAcumulable;
                itemFiniquito.IdEmpresaFiscal = itemNuevo.IdEmpresaFiscal;
                itemFiniquito.IdEmpresaAsimilado = itemNuevo.IdEmpresaAsimilado;
                itemFiniquito.IdEmpresaComplemento = itemNuevo.IdEmpresaComplemento;
                itemFiniquito.IdMetodo_Pago = itemNuevo.IdMetodo_Pago;
                itemFiniquito.TotalPercepciones = itemNuevo.TotalPercepciones;
                itemFiniquito.TotalDeducciones = itemNuevo.TotalDeducciones;
                itemFiniquito.TotalComplemento = itemNuevo.TotalComplemento;
                itemFiniquito.TotalOtrosPagos = itemNuevo.TotalOtrosPagos;
                itemFiniquito.SubsidioCausado = itemNuevo.SubsidioCausado;
                itemFiniquito.SubsidioEntregado = itemNuevo.SubsidioEntregado;
                itemFiniquito.UMA = itemNuevo.UMA;
                itemFiniquito.EsLiquidacion = itemNuevo.EsLiquidacion;
                itemFiniquito.DescuentosAplicados = itemNuevo.DescuentosAplicados;
                itemFiniquito.TipoTarifa = itemNuevo.TipoTarifa;
                itemFiniquito.PensionAlimenticiaDelTotal = itemNuevo.PensionAlimenticiaDelTotal;
                itemFiniquito.Art174 = itemNuevo.Art174;

                context.SaveChanges();
                return itemFiniquito;



            }
        }

        public NOM_Finiquito_Complemento AddFiniquitoComplemento(NOM_Finiquito_Complemento item)
        {
            using (var context = new RHEntities())
            {
                context.NOM_Finiquito_Complemento.Add(item);
                context.SaveChanges();
                return item;

            }



        }

        public void AddDetalleFiniquito(List<NOM_Finiquito_Detalle> lista)
        {
            using (var context = new RHEntities())
            {
                foreach (var item in lista)
                {
                    if (item.Total > 0)
                        context.NOM_Finiquito_Detalle.Add(item);
                }

                context.SaveChanges();
            }


        }

        public NOM_Finiquito GetFiniquitoByContratoID(int idContrato, int idEmpleado, int idSucursal, int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var item =
                    context.NOM_Finiquito.FirstOrDefault(
                        x => x.IdContrato == idContrato && x.IdEmpleado == idEmpleado && x.IdSucursal == idSucursal && x.IdPeriodo == idPeriodo);
                return item;
            }
        }

        public void EliminarFiniquitoProcesado(int idFiniquito, int idPeriodo)
        {
            if (idFiniquito == 0) return;


            using (var context = new RHEntities())
            {
                //eliminar de nominas
                string sqlQuery1 = "DELETE [NOM_Finiquito] WHERE IdFiniquito in (" + idFiniquito + ")";
                context.Database.ExecuteSqlCommand(sqlQuery1);

                //eliminar de detalle de nomina
                string sqlQuery2 = "DELETE [NOM_Finiquito_Detalle] WHERE IdFiniquito in (" + idFiniquito + ")";
                context.Database.ExecuteSqlCommand(sqlQuery2);



                //elimina complemento del finiquito
                string sqlQuery4 = "DELETE [NOM_Finiquito_Complemento] WHERE IdFiniquito in (" + idFiniquito + ")";
                context.Database.ExecuteSqlCommand(sqlQuery4);

                //eliminar de cuotas imss
                string sqlQuery5 = "DELETE [NOM_Cuotas_IMSS] WHERE IdFiniquito in (" + idFiniquito + ")";
                context.Database.ExecuteSqlCommand(sqlQuery5);

                string sqlQuery6 = "DELETE [NOM_FacturacionF_Finiquito] WHERE IdFiniquito in (" + idFiniquito + ")";
                context.Database.ExecuteSqlCommand(sqlQuery6);
                string sqlQuery7 = "DELETE [NOM_FacturacionC_Finiquito] WHERE IdFiniquito in (" + idFiniquito + ")";
                context.Database.ExecuteSqlCommand(sqlQuery7);
            }




        }

        public List<NOM_Finiquito_Descuento_Adicional> GetDescuentosAdicionalesFiniquito(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var listaDescuento = context.NOM_Finiquito_Descuento_Adicional.Where(x => x.IdPeriodo == idPeriodo && x.TipoConcepto == 2).ToList();

                return listaDescuento;

                //var item2 = listaDescuento == null ? 0 : listaDescuento.Sum(x => x.TotalDescuento);

                // return (double)item2;
            }
        }

        public List<NOM_Finiquito_Detalle> GetDescuentoPorSueldoPendiente(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var lista =
                (from fd in context.NOM_Finiquito_Detalle
                 where fd.IdFiniquito == idFiniquito
                       && (fd.IdConcepto == 51 || fd.IdConcepto == 52 || fd.IdConcepto == 48 || fd.IdConcepto == 42)
                 select fd).ToList();


                return lista;
            }
        }

        public List<NOM_Finiquito_Descuento_Adicional> GetComisionesComplementoFiniquito(int IdPeriodo)
        {
            using (var context = new RHEntities())
            {
                var listaComision = context.NOM_Finiquito_Descuento_Adicional.Where(x => x.IdPeriodo == IdPeriodo && x.TipoConcepto == 1 && x.IsComplemento == true).ToList();

                return listaComision;
            }
        }



        #endregion
        public NOM_Aguinaldo AddAguinaldo(NOM_Aguinaldo item)
        {
            using (var context = new RHEntities())
            {
                context.NOM_Aguinaldo.Add(item);
                context.SaveChanges();
                return item;

            }


        }
        public void AddRangeAguinaldos(List<NOM_Aguinaldo> lista)
        {
            using (var context = new RHEntities())
            {
                context.NOM_Aguinaldo.AddRange(lista);
                context.SaveChanges();

            }
        }

        public List<NOM_Aguinaldo> GetAguinaldosByIdPeriodo(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_Aguinaldo.Where(x => x.IdPeriodo == idPeriodo).ToList();
            }
        }

        #region Facturacion
        public List<NOM_Nomina> ObtenerIdNominaByPeriodo(int idperiodo, int? idemp)
        {
            using (var context = new RHEntities())
            {
                var lista = context.NOM_Nomina.Where(x => x.IdPeriodo == idperiodo && x.IdEmpresaFiscal == idemp).ToList();
                return lista;

            }


        }
        public List<NOM_Nomina> ObtenerIdNominaByPeriodoC(int idperiodo, int? idemp)
        {
            using (var context = new RHEntities())
            {
                var lista = context.NOM_Nomina.Where(x => x.IdPeriodo == idperiodo && x.IdEmpresaComplemento == idemp).ToList();
                return lista;

            }


        }
        public NOM_Cuotas_IMSS ObtenerCuotasIMSSHold(int idNomina)
        {
            using (var context = new RHEntities())
            {
                var rubros = context.NOM_Cuotas_IMSS.FirstOrDefault(x => x.IdNomina == idNomina);
                return rubros;

            }
        }
        public NOM_Cuotas_IMSS ObtenerCuotasIMSS2(List<NOM_Cuotas_IMSS> lista, int idNomina)
        {
            if (lista == null) return null;
            var rubros = lista.FirstOrDefault(x => x.IdNomina == idNomina);
            return rubros;
        }
        public decimal ObtenerInfonavitHold(int idNomina)
        {
            using (var context = new RHEntities())
            {
                decimal totalInfonavit = 0;
                var infona = context.NOM_Nomina_Detalle.Where(x => x.IdNomina == idNomina && x.IdConcepto == 51).FirstOrDefault();
                if (infona == null)
                {
                    totalInfonavit = 0;

                    return totalInfonavit;
                }
                else
                {
                    totalInfonavit = totalInfonavit + infona.Total;

                    return totalInfonavit;
                }

            }
        }
        public decimal ObtenerInfonavit2(List<NOM_Nomina_Detalle> lista, int idNomina)
        {

            decimal totalInfonavit = 0;
            var infona = lista.Where(x => x.IdNomina == idNomina && x.IdConcepto == 51).FirstOrDefault();
            if (infona == null)
            {
                totalInfonavit = 0;

                return totalInfonavit;
            }
            else
            {
                totalInfonavit = totalInfonavit + infona.Total;

                return totalInfonavit;
            }


        }
        public decimal ObtenerFonacotHold(int idNomina)
        {
            using (var context = new RHEntities())
            {
                decimal totalFonacot = 0;
                var fonat = context.NOM_Nomina_Detalle.Where(x => x.IdNomina == idNomina && x.IdConcepto == 52).FirstOrDefault();
                if (fonat == null)
                {
                    totalFonacot = 0;
                    return totalFonacot;
                }
                else
                {
                    totalFonacot = totalFonacot + fonat.Total;
                    return totalFonacot;
                }

            }
        }
        public decimal ObtenerFonacot2(List<NOM_Nomina_Detalle> lista, int idNomina)
        {
            decimal totalFonacot = 0;
            var fonat = lista.Where(x => x.IdNomina == idNomina && x.IdConcepto == 52).FirstOrDefault();
            if (fonat == null)
            {
                totalFonacot = 0;
                return totalFonacot;
            }
            else
            {
                totalFonacot = totalFonacot + fonat.Total;
                return totalFonacot;
            }
        }
        public decimal ObtenerPensionHold(int idNomina)
        {
            using (var context = new RHEntities())
            {
                decimal totalPension = 0;
                var pen = context.NOM_Nomina_Detalle.Where(x => x.IdNomina == idNomina && x.IdConcepto == 48).FirstOrDefault();
                if (pen == null)
                {
                    totalPension = 0;
                    return totalPension;
                }
                else
                {
                    totalPension = totalPension + pen.Total;
                    return totalPension;
                }

            }
        }
        public decimal ObtenerPension2(List<NOM_Nomina_Detalle> lista, int idNomina)
        {
            decimal totalPension = 0;
            var pen = lista.Where(x => x.IdNomina == idNomina && x.IdConcepto == 48).FirstOrDefault();
            if (pen == null)
            {
                totalPension = 0;
                return totalPension;
            }
            else
            {
                totalPension = totalPension + pen.Total;
                return totalPension;
            }
        }
        public decimal PorcentajeServicioHold(int idSucursal)
        {
            using (var context = new RHEntities())
            {
                decimal servicio = 0;
                var porceServicio = context.Sucursal.Where(x => x.IdSucursal == idSucursal).FirstOrDefault();
                servicio = porceServicio.PorcentajeServicio;
                return servicio;
            }

        }
        public decimal PorcentajeServicio2(List<Sucursal> lista, int idSucursal)
        {
            decimal servicio = 0;
            var porceServicio = lista.Where(x => x.IdSucursal == idSucursal).FirstOrDefault();
            servicio = porceServicio.PorcentajeServicio;
            return servicio;

        }
        public decimal PorcentajeIVA()
        {
            using (var context = new RHEntities())
            {
                decimal porcentaje = 0;
                var ivapor = context.ParametrosConfig.Where(x => x.IdConfig == 5).FirstOrDefault();
                porcentaje = ivapor.ValorInt;

                return porcentaje;

            }


        }
        public void CrearFactura(int periodo, int idemp, decimal cuotas, decimal impuesto, decimal infonavit, decimal fonacot, decimal pension, decimal relativos, decimal totalNomina, decimal fiscal, decimal porcentajeIVA)
        {
            using (var context = new RHEntities())
            {
                NOM_Facturacion factura = new NOM_Facturacion();

                var dato = context.NOM_Facturacion.FirstOrDefault(x => x.IdPeriodo == periodo && x.IdEmpresaFi_As == idemp);

                if (dato != null)
                {
                    string sqlQuery1 = "DELETE [NOM_Facturacion] WHERE IdPeriodo in (" + periodo + ") and IdEmpresaFi_As = " + idemp;

                    // var iva = context.ParametrosConfig.FirstOrDefault(x => x.IdConfig == 5).ValorInt;

                    context.Database.ExecuteSqlCommand(sqlQuery1);

                    factura.IdPeriodo = periodo;
                    factura.IdEmpresaFi_As = idemp;

                    factura.F_Cuota_IMSS_Infonavit = cuotas;
                    factura.F_Impuesto_Nomina = impuesto;
                    factura.F_Amortizacion_Infonavit = infonavit;
                    factura.F_Fonacot = fonacot;
                    factura.F_Pension_Alimenticia = pension;
                    factura.F_Relativo = relativos;
                    factura.F_Total_Nomina = totalNomina;
                    factura.F_Total_Fiscal = fiscal;
                    factura.IVA = porcentajeIVA;

                    context.NOM_Facturacion.Add(factura);

                    context.SaveChanges();
                }
                else
                {
                    //  var iva = context.ParametrosConfig.Where(x => x.IdConfig == 5).FirstOrDefault().ValorInt;

                    factura.IdPeriodo = periodo;
                    factura.IdEmpresaFi_As = idemp;
                    factura.F_Cuota_IMSS_Infonavit = cuotas;
                    factura.F_Impuesto_Nomina = impuesto;
                    factura.F_Amortizacion_Infonavit = infonavit;
                    factura.F_Fonacot = fonacot;
                    factura.F_Pension_Alimenticia = pension;
                    factura.F_Relativo = relativos;
                    factura.F_Total_Nomina = totalNomina;
                    factura.F_Total_Fiscal = fiscal;

                    context.NOM_Facturacion.Add(factura);
                    context.SaveChanges();
                }

            }

        }
        public void CrearFacturaComplemento(int periodo, int idempC, decimal percepcionesC, decimal porcentajeServicioC, decimal totalServicioC,
            decimal cuotasIMSSC, decimal impuestoNominaC, decimal relativosC, decimal subtotalC,
            decimal totalIVA, decimal totalCompletoC)
        {
            using (var context = new RHEntities())
            {
                NOM_FacturacionComplemento factura = new NOM_FacturacionComplemento();

                var dato = context.NOM_FacturacionComplemento.FirstOrDefault(x => x.IdPeriodo == periodo && x.IdEmpresaC == idempC);

                if (dato != null)
                {
                    string sqlQuery1 = "DELETE [NOM_FacturacionComplemento] WHERE IdPeriodo in (" + periodo + ") and IdEmpresaC = " + idempC;

                    // var iva = context.ParametrosConfig.FirstOrDefault(x => x.IdConfig == 5).ValorInt;

                    context.Database.ExecuteSqlCommand(sqlQuery1);
                    factura.IdPeriodo = periodo;
                    factura.IdEmpresaC = idempC;
                    factura.C_Porcentaje_Servicio = porcentajeServicioC;
                    factura.C_Total_Servicio = totalServicioC;
                    factura.C_Percepciones = percepcionesC;
                    factura.C_Cuotas_IMSS_Infonavit = cuotasIMSSC;
                    factura.C_Impuesto_Nomina = impuestoNominaC;
                    factura.C_Relativos = relativosC;
                    factura.C_Subtotal = subtotalC;
                    factura.C_Total_IVA = totalIVA;
                    factura.C_Total_Complemento = totalCompletoC;
                    context.NOM_FacturacionComplemento.Add(factura);

                    context.NOM_FacturacionComplemento.Add(factura);

                    context.SaveChanges();
                }
                else
                {
                    //  var iva = context.ParametrosConfig.Where(x => x.IdConfig == 5).FirstOrDefault().ValorInt;
                    factura.IdPeriodo = periodo;
                    factura.IdEmpresaC = idempC;
                    factura.C_Porcentaje_Servicio = porcentajeServicioC;
                    factura.C_Total_Servicio = totalServicioC;
                    factura.C_Percepciones = percepcionesC;
                    factura.C_Cuotas_IMSS_Infonavit = cuotasIMSSC;
                    factura.C_Impuesto_Nomina = impuestoNominaC;
                    factura.C_Relativos = relativosC;
                    factura.C_Subtotal = subtotalC;
                    factura.C_Total_IVA = totalIVA;
                    factura.C_Total_Complemento = totalCompletoC;
                    context.NOM_FacturacionComplemento.Add(factura);

                    context.NOM_FacturacionComplemento.Add(factura);

                    context.SaveChanges();
                }

            }





        }

        #endregion

    }

}
