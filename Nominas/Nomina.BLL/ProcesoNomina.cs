using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using RH.BLL;
using Nomina.Procesador;
using Nomina.Procesador.Metodos;
using Nomina.Procesador.Modelos;
using RH.Entidades.GlobalModel;

namespace Nomina.BLL
{

    public class DetalleNomina
    {
        public int IdDetalleNomina { get; set; }
        public int IdNomina { get; set; }
        public int IdConcepto { get; set; }
        public string DescripcionConcepto { get; set; }
        public int TipoConcepto { get; set; }
        public decimal Total { get; set; }
        public decimal GravaISR { get; set; }
        public decimal ExentoISR { get; set; }
        public decimal GravaIMSS { get; set; }
        public bool Complemento { get; set; }

    }

    public class ProcesoNomina
    {
        private readonly RHEntities ctx = null;

        public ProcesoNomina()
        {
            ctx = new RHEntities();
        }
        public NOM_Cuotas_IMSS GetDetalleSS(int IdNomina)
        {
            return IdNomina <= 0 ? null : ctx.NOM_Cuotas_IMSS.FirstOrDefault(x => x.IdNomina == IdNomina);
        }
        public ProcesoNominaViewModel GetNomina(int IdNomina)
        {
            return (from n in ctx.NOM_Nomina
                    join e in ctx.Empleado on n.IdEmpleado equals e.IdEmpleado
                    where n.IdNomina == IdNomina
                    select new ProcesoNominaViewModel
                    {
                        IdEmpleado = e.IdEmpleado,
                        Empleado = e.APaterno + " " + e.AMaterno + " " + e.Nombres,
                        IdNomina = n.IdNomina,
                        IdPeriodo = n.IdPeriodo,
                        Percepciones = n.TotalPercepciones,
                        Deducciones = n.TotalDeducciones,
                        Total = n.TotalNomina,
                        TotalComplemento = n.TotalComplemento,
                        Sd = n.SD,
                        Sbc = n.SBCotizacionDelProcesado,
                        TipoTarifa = n.TipoTarifa,
                        ImpuestoSobreNomina = n.TotalImpuestoSobreNomina,
                        IsrAntesSubsidio = n.ISRantesSubsidio
                    }).FirstOrDefault();
        }

        public static NOM_Nomina GetNominaById(int idNomina)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_Nomina.FirstOrDefault(x => x.IdNomina == idNomina);

                return item;
            }
        }

        public List<DetalleNomina> GetDetalleNomina(int IdNomina)
        {
            return (from n in ctx.NOM_Nomina
                    join nd in ctx.NOM_Nomina_Detalle on n.IdNomina equals nd.IdNomina
                    join c in ctx.C_NOM_Conceptos on nd.IdConcepto equals c.IdConcepto
                    where n.IdNomina == IdNomina
                    select new DetalleNomina
                    {
                        IdDetalleNomina = nd.Id,
                        IdNomina = n.IdNomina,
                        IdConcepto = c.IdConcepto,
                        DescripcionConcepto = c.DescripcionCorta,
                        TipoConcepto = c.TipoConcepto,
                        Total = nd.Total,
                        GravaISR = nd.GravadoISR,
                        ExentoISR = nd.ExentoISR,
                        GravaIMSS = nd.IntegraIMSS,
                        Complemento = nd.Complemento
                    }
                ).OrderBy(x => x.IdDetalleNomina).ToList();
        }
        //public List<ProcesoNominaViewModel> GetNominasByPeriodo(int idPeriodo)
        //{
        //    return (from n in ctx.NOM_Nomina
        //            join e in ctx.Empleado on n.IdEmpleado equals e.IdEmpleado
        //            where n.IdPeriodo == idPeriodo
        //            select new ProcesoNominaViewModel
        //            {
        //                IdEmpleado = e.IdEmpleado,
        //                Empleado = e.APaterno + " " + e.AMaterno + " " + e.Nombres,
        //                IdNomina = n.IdNomina,
        //                IdPeriodo = idPeriodo,
        //                Percepciones = n.TotalPercepciones,
        //                Deducciones = n.TotalDeducciones,
        //                Total = n.TotalNomina,
        //                TotalComplemento = n.TotalComplemento,
        //                Seleccionado = true
        //            }
        //        ).ToList();
        //}
        public static async Task<string> GetRecibosSinTimbre(int[] listNominas, NOM_PeriodosPago ppago, int idUsuario, string path, bool isCfdi33 = false)
        {

            //Se generan los recibos sin timbrado del sat
            if (listNominas != null)
            {
                PDFCore pcCore = new PDFCore();
                var recibo =
                    await
                        pcCore.GetRecibosSintimbre(listNominas, ppago.IdEjercicio, ppago.IdPeriodoPago, idUsuario, path, isCfdi33);
                return recibo;
            }
            else
            {
                return null;
            }

        }
        public static async Task<string> GetRecibosComplemento(int[] listNominas, NOM_PeriodosPago ppago, int idUsuario, string path, bool incluirDetalles)
        {

            //Se generan los recibos sin timbrado del sat
            if (listNominas != null)
            {
                PDFCore pcCore = new PDFCore();
                var recibo =
                    await
                        pcCore.GetRecibosComplemento(listNominas, ppago.IdEjercicio, ppago, idUsuario, path, incluirDetalles);
                return recibo;
            }
            else
            {
                return null;
            }

        }


        public static async Task<string> GetRecibosComplementoDetalle(int[] idEmpleados, NOM_PeriodosPago ppago, int idUsuario, string path)
        {

            //Se generan los recibos sin timbrado del sat
            if (idEmpleados != null)
            {
                PDFCore pcCore = new PDFCore();
                var recibo =
                    await
                        pcCore.GetRecibosComplementoDetalle(idEmpleados, ppago.IdEjercicio, ppago, idUsuario, path);
                return recibo;
            }
            else
            {
                return null;
            }

        }
        public static async Task<string> GetRecibosFiniquitoSinTimbre(int idFiniquito, NOM_PeriodosPago ppago, int idUsuario, string path, bool isCfdi33 = false)
        {
            //Se generan los recibos sin timbrado del sat
            if (idFiniquito != 0)
            {
                PDFCore pcCore = new PDFCore();
                var recibo =
                    await
                        pcCore.GetRecibosFiniquitoSintimbre(idFiniquito, ppago.IdEjercicio, ppago.IdPeriodoPago, idUsuario, path, isCfdi33);
                return recibo;
            }
            else
            {
                return null;
            }

        }
        public static async Task<List<NotificationSummary>> ProcesarNominaAsync(int[] idEmpleados, NOM_PeriodosPago ppago, int idCliente, int idSucursal, int idUsuario, bool anual, bool calcularConUma = false )
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();

            try
            {
                if (idEmpleados == null || ppago == null || ppago.Autorizado == true) return null;

                NominaCore nc = new NominaCore();
                

                if (ppago.IdTipoNomina == 16)//Si es tipo asimilado
                {
                    summaryList = await nc.ProcesarNominaAsimiladoAsync(idEmpleados, ppago, idCliente, idSucursal, idUsuario, calcularConUma);
                }
                else if (ppago.IdTipoNomina == 17 && ppago.Sindicato) //Si el tipo es sindicato
                {
                    summaryList = await nc.ProcesarNominaSindicatoAsync(idEmpleados, ppago, idCliente, idSucursal, idUsuario, calcularConUma);
                }
                else //Nomina Normal
                {
                    summaryList = await nc.ProcesarNominaAsync(idEmpleados, ppago, idCliente, idSucursal, idUsuario, calcularConUma,anual);
                }


                return summaryList;
            }
            catch (Exception ex)
            {
                summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "? -" + ex.Message, Msg2 = "" });

                return summaryList;
            }

        }
        public static async Task<List<NotificationSummary>> ProcesarAguinaldoAsync(int[] arrayIdEmpleado, string[] faltasCapturadas, bool[] generarPensionAlimenticia, int idPeriodo, int idCliente, int idSucursal, int idUsuario, bool anual)
        {
            try
            {
                //Validaciones
                if (arrayIdEmpleado == null) return null;

                if (idPeriodo <= 0 || idSucursal <= 0) return null;

                NominaCore nc = new NominaCore();
                var ids = await nc.ProcesarAguinaldoAsync(arrayIdEmpleado, faltasCapturadas, generarPensionAlimenticia, idPeriodo, idCliente, idSucursal, idUsuario,anual);
                return ids;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public static async Task<int> ProcesarFiniquitoIndemnizacionAsync(int idPeriodo, int idEjercicio, int idEmpleado, int idCliente, int idSucursal, ParametrosFiniquitos arrayF, int idUsuario, bool calcularLiquidacion = false, TotalPersonalizablesFiniquitos totalesPerson = null, bool isArt174=false)
        {
            try
            {
                NominaCore nc = new NominaCore();
                var id = await nc.ProcesarFiniquitoIndemnizacionAsync(idPeriodo, idEjercicio, idEmpleado, idCliente, idSucursal, arrayF, calcularLiquidacion,idUsuario, totalesPerson, isArt174);

                return id;
            }
            catch (MissingMethodException ex)
            {

                return 0;
            }
        }
        public List<ProcesadoModelo2> GetNominaDatosProcesado(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                //Obtenemos los datos actuales del periodo
                var periodoActual = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                if (periodoActual == null) return null;

                var listaEmpleadosDelPeriodo = (from ep in context.NOM_Empleado_PeriodoPago
                                                join en in context.Empleado on ep.IdEmpleado equals en.IdEmpleado
                                                where ep.IdPeriodoPago == idPeriodo
                                                select en).ToList();

                if (listaEmpleadosDelPeriodo.Count <= 0) return null;

                //SOLO COMPLEMENTO

                #region "SOLO COMPLEMENTO"

                if (periodoActual.SoloComplemento)
                {
                    var arrayIdEmpleado = listaEmpleadosDelPeriodo.Select(x => x.IdEmpleado).ToArray();

                    var listaNominas = (from nom in context.NOM_Nomina
                                        where arrayIdEmpleado.Contains(nom.IdEmpleado)
                                              && nom.IdPeriodo == idPeriodo
                                        select nom).ToList();

                    if (listaNominas.Count > 0)
                    {

                        var listaConNomina = (from emp in listaEmpleadosDelPeriodo
                                              join nom in context.NOM_Nomina on emp.IdEmpleado equals nom.IdEmpleado
                                              where nom.IdPeriodo == idPeriodo
                                              select new ProcesadoModelo2
                                              {
                                                  IdEmpleado = emp.IdEmpleado,
                                                  IdNomina = nom.IdNomina,
                                                  Paterno = emp.APaterno,
                                                  Materno = emp.AMaterno,
                                                  Nombres = emp.Nombres,
                                                  Percepciones = nom.TotalPercepciones,
                                                  Deducciones = nom.TotalDeducciones,
                                                  TotalComplemento = nom.TotalComplemento,
                                                  TotalNomina = nom.TotalNomina,
                                                  Conceptos = null,
                                                  Seleccionado = true,
                                                  Sbc = nom.SBC,
                                                  TipoTarifa = nom.TipoTarifa,
                                                  OtrosPagos = nom.TotalOtrosPagos,
                                                  DiasLaborados = nom.Dias_Laborados
                                              }).ToList();

                        //Agrega los colaboradores que no estaban en el ultimo procesado
                        var arrayIdConNom = listaConNomina.Select(x => x.IdEmpleado).ToArray();

                        //Nueva lista sin nomina
                        var listaSinNomina = (from empSin in listaEmpleadosDelPeriodo
                                              where !arrayIdConNom.Contains(empSin.IdEmpleado)
                                              select new ProcesadoModelo2
                                              {
                                                  IdEmpleado = empSin.IdEmpleado,
                                                  IdNomina = 0,
                                                  Paterno = empSin.APaterno,
                                                  Materno = empSin.AMaterno,
                                                  Nombres = empSin.Nombres,
                                                  Percepciones = 0,
                                                  Deducciones = 0,
                                                  TotalComplemento = 0,
                                                  TotalNomina = 0,
                                                  Conceptos = null,
                                                  Seleccionado = true,
                                                  Sbc = 0,
                                                  TipoTarifa = 0,
                                                  OtrosPagos = 0,
                                                  DiasLaborados = 0
                                              }).ToList();

                        listaConNomina.AddRange(listaSinNomina);

                        return listaConNomina;
                    }
                    else
                    {
                        var lista = (from emp in listaEmpleadosDelPeriodo
                                     select new ProcesadoModelo2
                                     {
                                         IdEmpleado = emp.IdEmpleado,
                                         IdNomina = 0,
                                         Paterno = emp.APaterno,
                                         Materno = emp.AMaterno,
                                         Nombres = emp.Nombres,
                                         Percepciones = 0,
                                         Deducciones = 0,
                                         TotalComplemento = 0,
                                         TotalNomina = 0,
                                         Conceptos = null,
                                         Seleccionado = true,
                                         Sbc = 0,
                                         TipoTarifa = 0,
                                         OtrosPagos = 0,
                                         DiasLaborados = 0

                                     }).ToList();

                        return lista;
                    }


                }

                #endregion


                //A) SI EL PERIODO NO HA SIDO PROCESADO
                if (!periodoActual.Procesado)
                {

                    #region CONSULTA SOLO EMPLEADOS Y CONCEPTOS EN CEROS - NOMINAS SIN PROCESAR 

                    //Obtener la lista de conceptos que tienen los empleados del periodo
                    var listaConceptosDeTodosLosEmpleados = (from ep in listaEmpleadosDelPeriodo
                                                             join ec in context.NOM_Empleado_Conceptos on ep.IdEmpleado equals ec.IdEmpleado
                                                             join con in context.C_NOM_Conceptos on ec.IdConcepto equals con.IdConcepto
                                                             select con).ToList();
                    //Agrupa por IdConcepto
                    var listaGroupConceptos = listaConceptosDeTodosLosEmpleados.GroupBy(x => x.IdConcepto).ToList();
                    //Genera Array de Id de los conceptos
                    var arrayIdConceptos = listaGroupConceptos.Select(x => x.Key).ToArray();
                    //Lista Datos de los conceptos
                    var listaConceptosDatos = GetConceptosByArrarId(arrayIdConceptos);

                    // Crear la lista del Modelo2
                    List<ProcesadoModelo2> listaSinProcesar = new List<ProcesadoModelo2>();

                    //Por cada empleado en la lista
                    foreach (var itemEmpleado in listaEmpleadosDelPeriodo)
                    {
                        //Lista de nomina por empleado
                        ProcesadoModelo2 itemModelo = new ProcesadoModelo2();
                        itemModelo.IdEmpleado = itemEmpleado.IdEmpleado;
                        itemModelo.IdNomina = 0;
                        itemModelo.Paterno = itemEmpleado.APaterno;
                        itemModelo.Materno = itemEmpleado.AMaterno;
                        itemModelo.Nombres = itemEmpleado.Nombres;
                        itemModelo.Percepciones = 0;
                        itemModelo.Deducciones = 0;
                        itemModelo.OtrosPagos = 0;
                        itemModelo.TotalComplemento = 0;
                        itemModelo.TotalNomina = 0;
                        itemModelo.DiasLaborados = 0;

                        List<ConceptosProcesadoModelo> listaConceptosAsignado = new List<ConceptosProcesadoModelo>();

                        //recorremos cada concepto

                        #region Conceptos

                        foreach (var itemConcepto in listaConceptosDatos)
                        {

                            var itemC = new ConceptosProcesadoModelo
                            {
                                IdConcepto = itemConcepto.IdConcepto,
                                IdNomina = 0,
                                NombreConcepto = itemConcepto.DescripcionCorta,
                                TotalConcepto = 0,
                                IsComplemento = itemConcepto.FormulaComplemento,
                                IsObligatorio = itemConcepto.Obligatorio
                            };

                            listaConceptosAsignado.Add(itemC);
                        }

                        #endregion


                        //Agregamos la lista de conceptos al modelo
                        itemModelo.Conceptos = listaConceptosAsignado;
                        //Agregamos el modelo a la lista
                        listaSinProcesar.Add(itemModelo);
                    }

                    #endregion

                    return listaSinProcesar;
                }
                else
                {
                    //B) SI EL PERIODO FUE PROCESADO

                    #region CONSULTA CON DATOS PROCESADOS- NOMINA PROCESADA

                    var listaNomina = (from emp in listaEmpleadosDelPeriodo
                                       join nom in context.NOM_Nomina on emp.IdEmpleado equals nom.IdEmpleado
                                       //join emp in ctx.Empleado on nom.IdEmpleado equals emp.IdEmpleado
                                       join deta in context.NOM_Nomina_Detalle on nom.IdNomina equals deta.IdNomina
                                       join conce in context.C_NOM_Conceptos on deta.IdConcepto equals conce.IdConcepto
                                       where nom.IdPeriodo == idPeriodo                                       
                                       select new ProcesadoModelo
                                       {
                                           IdEmpleado = nom.IdEmpleado,
                                           Paterno = emp.APaterno,
                                           Materno = emp.AMaterno,
                                           Nombres = emp.Nombres,
                                           Percepciones = nom.TotalPercepciones,
                                           Deducciones = nom.TotalDeducciones,
                                           OtrosPagos = nom.TotalOtrosPagos,
                                           TotalComplemento = nom.TotalComplemento,
                                           TotalNomina = nom.TotalNomina,
                                           IdConcepto = deta.IdConcepto,
                                           IdNomina = nom.IdNomina,
                                           NombreConcepto = conce.DescripcionCorta,
                                           TotalConcepto = deta.Total,
                                           IsComplemento = deta.Complemento,
                                           IsObligatorio = conce.Obligatorio,
                                           Seleccionado = true,
                                           Sbc = nom.SBCotizacionDelProcesado,
                                           TipoTarifa = nom.TipoTarifa,
                                           DiasLaborados = nom.Dias_Laborados                                          
                                       }).ToList();


                    //Agrupar Id Conceptos
                    var conceptosDiferentes = listaNomina.GroupBy(x => x.IdConcepto).ToList();

                    //Genera Array de Id de los conceptos
                    var arrayIdConceptos = conceptosDiferentes.Select(x => x.Key).ToArray();
                    //Lista Datos de los conceptos
                    var listaConceptosDatos = GetConceptosByArrarId(arrayIdConceptos);

                    List<ProcesadoModelo2> listaConDetalleNomina = new List<ProcesadoModelo2>();

                    //Por cada empleado en la lista
                    foreach (var itemEmpleado in listaEmpleadosDelPeriodo)
                    {
                        //Lista de nomina por empleado
                        var listaPorEmpleado = listaNomina.Where(x => x.IdEmpleado == itemEmpleado.IdEmpleado).ToList();

                        ProcesadoModelo2 itemModelo = new ProcesadoModelo2();
                        itemModelo.IdEmpleado = itemEmpleado.IdEmpleado;
                        itemModelo.IdNomina = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].IdNomina : 0;
                        itemModelo.Paterno = itemEmpleado.APaterno;
                        itemModelo.Materno = itemEmpleado.AMaterno;
                        itemModelo.Nombres = itemEmpleado.Nombres;
                        itemModelo.Percepciones = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].Percepciones : 0;
                        itemModelo.Deducciones = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].Deducciones : 0;
                        itemModelo.OtrosPagos = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].OtrosPagos : 0;
                        itemModelo.TotalComplemento = listaPorEmpleado.Count > 0
                            ? listaPorEmpleado[0].TotalComplemento
                            : 0;
                        itemModelo.TotalNomina = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].TotalNomina : 0;
                        itemModelo.Sbc = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].Sbc : 0;
                        itemModelo.TipoTarifa = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].TipoTarifa : 0;
                        itemModelo.DiasLaborados = listaPorEmpleado.Count > 0 ? listaPorEmpleado[0].DiasLaborados : 0;

                        List<ConceptosProcesadoModelo> listaConceptos = new List<ConceptosProcesadoModelo>();

                        #region Conceptos

                        
                        foreach (var itemConcepto in listaConceptosDatos)
                        {
                            ProcesadoModelo conceptoEncontrado = null;

                            var listaEncontrada =listaPorEmpleado.Where(x => x.IdConcepto == itemConcepto.IdConcepto).ToList();

                            if (listaEncontrada.Count > 0)
                            {
                                conceptoEncontrado = listaEncontrada.First();

                                //Si el empleado tiene doble concepto asignado
                                //ejemplo dos descuentos de fonacot
                                //Se suman las cantidades
                                if (listaEncontrada.Count > 1)
                                {
                                    conceptoEncontrado.TotalConcepto = listaEncontrada.Sum(x => x.TotalConcepto);
                                }
                            }


                            // var conceptoEncontrado = listaPorEmpleado.FirstOrDefault(x => x.IdConcepto == itemConcepto.IdConcepto);
                            
                            var itemC = new ConceptosProcesadoModelo
                            {
                                IdConcepto = itemConcepto.IdConcepto,
                                IdNomina = conceptoEncontrado?.IdNomina ?? 0,
                                NombreConcepto = itemConcepto.DescripcionCorta,
                                TotalConcepto = conceptoEncontrado?.TotalConcepto ?? 0,
                                IsComplemento = conceptoEncontrado?.IsComplemento ?? false,
                                IsObligatorio = conceptoEncontrado?.IsObligatorio ?? false

                            };

                            listaConceptos.Add(itemC);
                        }

                        #endregion

                        //Agregamos la lista de conceptos al modelo
                        itemModelo.Conceptos = listaConceptos;
                        //Agregamos el modelo a la lista
                        listaConDetalleNomina.Add(itemModelo);
                    }

                    #endregion

                    return listaConDetalleNomina;
                }

            }
        }
        public List<AguinaldoModelo> GetNominaDatosProcesadoAguinaldo(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                //Obtenemos los datos actuales del periodo
                var periodoActual = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                if (periodoActual == null) return null;

                var listaEmpleadosDelPeriodo = (from ep in context.NOM_Empleado_PeriodoPago
                                                join en in context.Empleado on ep.IdEmpleado equals en.IdEmpleado
                                                where ep.IdPeriodoPago == idPeriodo
                                                select en).ToList();

                if (listaEmpleadosDelPeriodo.Count <= 0) return null;

                // Crear la lista del Modelo2
                List<AguinaldoModelo> listaModelo = new List<AguinaldoModelo>();

                //SOLO COMPLEMENTO >--> no hay


                #region CONSULTA LOS PROCESADOS
                
                //foreach (var itemReg in listaAguinaldos)
                //{
                //    var itemEmpleado = listaEmpleadosDelPeriodo.FirstOrDefault(x => x.IdEmpleado == itemReg.IdEmpleado);

                //    if (itemEmpleado == null) continue;

                //    //Lista de aguinaldo por empleado
                //    AguinaldoModelo itemAguinaldo = new AguinaldoModelo()
                //    {
                //        IdAguinaldo = itemReg.IdAguinaldo,
                //        IdNomina = itemReg.IdNomina,
                //        IdEmpleado = itemEmpleado.IdEmpleado,
                //        Paterno = itemEmpleado.APaterno,
                //        Materno = itemEmpleado.AMaterno,
                //        Nombres = itemEmpleado.Nombres,
                //        FechaInicio = itemReg.FechaPrimerDia.Value,
                //        FechaFin = itemReg.FechaUltimoDia.Value,
                //        Proporcion = itemReg.Proporcion,
                //        TotalFaltas = itemReg.TotalFaltas,
                //        Aguinaldo = itemReg.Aguinaldo,
                //        Isr = itemReg.ISR,
                //        Complemento = itemReg.Complemento,
                //        Neto = itemReg.Neto,
                //        Total = itemReg.Total,
                //        Factor = itemReg.Factor
                //    };


                //    listaModelo.Add(itemAguinaldo);
                //}

                #endregion
                
                #region CONSULTA SOLO EMPLEADOS Y CONCEPTOS EN CEROS - NOMINAS SIN PROCESAR 

                //Lista Datos de los conceptos
                //var listaConceptosDatos = GetConceptosByArrarId(arrayIdConceptos);
                int[] arrayConcepto = new[] { 2 };
                var listaConceptosDatos = GetConceptosByArrarId(arrayConcepto);
                
                //Consultamos la informacion procesada en la tabla de Aguinaldo
                var listaAguinaldos = context.NOM_Aguinaldo.Where(x => x.IdPeriodo == idPeriodo).ToList();
                
                //Por cada empleado en la lista
                foreach (var itemEmpleado in listaEmpleadosDelPeriodo)
                {
                    if(itemEmpleado == null) continue;

                    var itemProcesado = listaAguinaldos.FirstOrDefault(x => x.IdEmpleado == itemEmpleado.IdEmpleado);

                    //Lista de aguinaldo por empleado
                    AguinaldoModelo itemAguinaldo = new AguinaldoModelo()
                    {
                        IdAguinaldo = itemProcesado?.IdAguinaldo ?? 0,
                        IdNomina = itemProcesado?.IdNomina ?? 0,
                        IdEmpleado = itemEmpleado.IdEmpleado,
                        Paterno = itemEmpleado.APaterno,
                        Materno = itemEmpleado.AMaterno,
                        Nombres = itemEmpleado.Nombres,
                        FechaInicio = itemProcesado ?.FechaPrimerDia.Value ?? DateTime.Now,
                        FechaFin = itemProcesado ?.FechaUltimoDia.Value ?? periodoActual.Fecha_Fin,
                        DiasTrabajados = itemProcesado?.DiasTrabajados ?? 0,
                        Proporcion = itemProcesado ?.Proporcion ??0,
                        TotalFaltas = itemProcesado ?.TotalFaltas ?? 0,
                        FaltasPersonalizadas = itemProcesado?.FaltasPersonalizadas ?? null,
                        Aguinaldo = itemProcesado ?.Aguinaldo ?? 0,
                        Isr = itemProcesado ?.ISR ?? 0,
                        Complemento = itemProcesado ?.Complemento ?? 0,
                        Neto = itemProcesado ?.Neto ?? 0,
                        Total = itemProcesado ? .Total ?? 0,
                        Factor = itemProcesado ?.Factor ?? 0,
                        GenerarPension = itemProcesado?.CalcularPensionAlimenticia??true,
                        PensionAlimenticia = itemProcesado?.PensionAlimenticia ??0,
                        SaldoAnual = itemProcesado?.SaldoAnual ?? 0
                    };




                    listaModelo.Add(itemAguinaldo);
                }

                #endregion
                
                return listaModelo;


            }
        }
        private List<C_NOM_Conceptos> GetConceptosByArrarId(int[] arrayIdConcetos)
        {
            if (arrayIdConcetos == null) return null;

            using (var context = new RHEntities())
            {
                var listaConceptos = (from c in context.C_NOM_Conceptos
                                      where arrayIdConcetos.Contains(c.IdConcepto)
                                      select c).ToList();

                return listaConceptos;
            }
        }
        public ImpuestoDetalleModel GetImpuestosDetalle(NOM_Nomina itemnomina, decimal baseGravable = 0, decimal SD = 0, int tipoTarifa = 0, int diasPeriodo = 0, int idTipoNomina = 0)
        {

            if (baseGravable <= 0) return null;
            if (tipoTarifa <= 0) return null;
            IsrSubsidio calculoIsrSubsidio = new IsrSubsidio();

            if (idTipoNomina == 16)
            {
                calculoIsrSubsidio = MNominas.CalculoIsrAsimilado(baseGravable, SD, diasPeriodo, 4, incluirTablas: true);
            }
            else
            {
                //   calculoIsrSubsidio = MNominas.CalculoIsrSubsidioFin(itemnomina, itemnomina.SBCotizacionDelProcesado, SD, diasPeriodo, tipoTarifa, incluirTablas: true);
                calculoIsrSubsidio = MNominas.CalculoIsrSubsidio304(itemnomina, itemnomina.SBCotizacionDelProcesado, SD, diasPeriodo, tipoTarifa, incluirTablas: true); // factor
                
            }


            var item = new ImpuestoDetalleModel()
            {
                BaseGravable = calculoIsrSubsidio.BaseGravable,
                BaseGravableMensual = calculoIsrSubsidio.BaseGravableMensual,
                LimiteInferior = calculoIsrSubsidio.LimiteInferior,
                Base = calculoIsrSubsidio.Base,
                Tasa = calculoIsrSubsidio.Tasa,
                Isr = calculoIsrSubsidio.IsrAntesDeSub,
                Subsidio = calculoIsrSubsidio.Subsidio,
                NetoAPagar = calculoIsrSubsidio.NetoAPagar,
                ResultadoIsrOSubsidio = (calculoIsrSubsidio.ResultadoIsrOSubsidio),
                Tablaisr = calculoIsrSubsidio.Tablaisr,
                Tablasubsidio = calculoIsrSubsidio.Tablasubsidio,
                IdTablaIsr = calculoIsrSubsidio.IdTablaIsr,
                IdTablaSubsidio = calculoIsrSubsidio.IdTablaSubsidio,
                EsISR = calculoIsrSubsidio.EsISR,
                Porcentajeisr = 0
            };

            return item;

        }

        public NOM_CalculoAnual GetCalculoDetalle(int id)
        {
            var item = new NOM_CalculoAnual();
            using (var ctx = new RHEntities())
            {
                 item = ctx.NOM_CalculoAnual.Where(x => x.idNomina == id).FirstOrDefault();                    
            }
            if (item == null) { return item = new NOM_CalculoAnual(); }
            

            return item;

        }

        /// <summary>
        /// Obtiene la configuracion de las opciones de las columnas para mostrar
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idSucursal"></param>
        /// <param name="idModulo"></param>
        /// <returns></returns>
        public UserConfig ObtenerConfiguracion(int idUsuario, int idSucursal, int idModulo)
        {
            using (var context = new RHEntities())
            {
                var datos =
                    context.UserConfig
                        .FirstOrDefault(x => x.IdUsuario == idUsuario && x.IdSucursal == idSucursal && x.Modulo == idModulo);
                return datos;
            }
        }
        public bool GuardarConfiguracion(int idUsuario, int idSucursal, int[] visible, int[] oculto, int idModulo)
        {
            string oc;
            string vis;
            if (visible != null)
            {
                vis = string.Join(",", visible);
            }
            else
            {
                vis = "0";
            }
            if (oculto != null)
            {
                oc = string.Join(",", oculto);
            }
            else
            {
                oc = "0";
            }

            var result = false;
            int t = 0;
            var existe = ctx.UserConfig.Where(x => x.IdUsuario == idUsuario && x.IdSucursal == idSucursal && x.Modulo == idModulo).FirstOrDefault();

            if (existe != null)
            {
                const string sqlQuery = "DELETE UserConfig WHERE IdUserConfig = @p0";
                ctx.Database.ExecuteSqlCommand(sqlQuery, existe.IdUserConfig);
                var item = new UserConfig
                {
                    IdUsuario = idUsuario,
                    IdSucursal = idSucursal,
                    ConceptosVisibles = vis,
                    ConceptosOcultos = oc,
                    Modulo = idModulo
                };

                ctx.UserConfig.Add(item);
                t = ctx.SaveChanges();
            }
            else
            {


                var item = new UserConfig
                {
                    IdUsuario = idUsuario,
                    IdSucursal = idSucursal,
                    ConceptosVisibles = vis,
                    ConceptosOcultos = oc,
                    Modulo = idModulo
                };

                ctx.UserConfig.Add(item);
                t = ctx.SaveChanges();
            }

            if (t > 1)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public ZonaSalario GetZonaSalario()
        {
            using (var context = new RHEntities())
            {
                var item = context.ZonaSalario.Where(x => x.Status == true).OrderByDescending(x => x.IdZonaSalario).FirstOrDefault();

                if (item == null)
                {
                    ZonaSalario itZ = new ZonaSalario()
                    {
                        IdZonaSalario = 0,
                        SMG = 0,
                        UMA = 0,
                        Status = false,
                        Vigencia = null
                    };
                }

                return item;
                // return _ctx.ZonaSalario.FirstOrDefault(x => x.IdZonaSalario == id);
            }
        }
        public List<EmpleadoCredito> GetCreditosEmpleadosByPeriodo(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                //Obtener los empleados asignados al periodo
                var listaEmppleados =( from emp in context.NOM_Empleado_PeriodoPago
                                      where emp.IdPeriodoPago == idPeriodo
                                      select emp).ToList();

                var arrayIdEmp = listaEmppleados.Select(x => x.IdEmpleado).ToArray();

                var arrayContratos = (from contra in context.Empleado_Contrato
                                      where arrayIdEmp.Contains(contra.IdEmpleado)
                                      select contra.IdContrato).ToArray();

                //Unir con la tabla Empleados a los que tienen credito
                var listaEmpleadosCreditos = (from cre in context.Empleado_Infonavit
                                              join contra in context.Empleado_Contrato on cre.IdEmpleadoContrato equals contra.IdContrato
                                              join emp in context.Empleado on contra.IdEmpleado equals emp.IdEmpleado
                                              where arrayContratos.Contains(cre.IdEmpleadoContrato)
                                              select new EmpleadoCredito
                                              {
                                                  IdEmpleadoPeriodo = 0,
                                                  IdEmpleado = emp.IdEmpleado,
                                                  IdContrato = contra.IdContrato,
                                                  Nombre = emp.Nombres,
                                                  Paterno = emp.APaterno,
                                                  Materno = emp.AMaterno,
                                                  FechaInicio = cre.FechaInicio,
                                                  FechaSuspension = cre.FechaSuspension,
                                                  DiasADescontar = -1
                                              }).ToList();

                //Asignar los dias a descontar
                foreach (var itemCredito in listaEmpleadosCreditos)
                {
                    var item = listaEmppleados.FirstOrDefault(x => x.IdEmpleado == itemCredito.IdEmpleado);

                    if( item == null) continue;
                    itemCredito.IdEmpleadoPeriodo = item.IdEmp_Periodo;
                    itemCredito.DiasADescontar = item.DiasDescuentoInfonavit;
                }

                return listaEmpleadosCreditos;
            }

        }
        public void ActualizaDiasInfonavitByPeriodo(int idEmpleadoPeriodo, int diasPagos)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_Empleado_PeriodoPago.FirstOrDefault(x => x.IdEmp_Periodo == idEmpleadoPeriodo);

                if (item == null) return;
                item.DiasDescuentoInfonavit = diasPagos;
                context.SaveChanges();
            }
        }

        public NOM_Aguinaldo GetAguinaldoByIdAguinaldo(int idAguinaldo)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_Aguinaldo.FirstOrDefault(x => x.IdAguinaldo == idAguinaldo);
                return item;
            }
        }
    }

    public class ProcesoNominaViewModel
    {
        public int IdNomina { get; set; }
        public int IdPeriodo { get; set; }
        public int IdEmpleado { get; set; }
        public string Empleado { get; set; }
        public bool Seleccionado { get; set; }
        public decimal Percepciones { get; set; }
        public decimal Deducciones { get; set; }
        public decimal TotalComplemento { get; set; }
        public decimal Total { get; set; }
        public decimal Sd { get; set; }
        public decimal Sbc { get; set; }
        public int TipoTarifa { get; set; }
        public decimal ImpuestoSobreNomina { get; set; }
        public decimal IsrAntesSubsidio { get; set; }
    }
    public class ProcesadoModelo
    {
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public decimal Percepciones { get; set; }
        public decimal Deducciones { get; set; }
        public decimal TotalComplemento { get; set; }
        public decimal TotalNomina { get; set; }
        public int IdConcepto { get; set; }
        public int IdNomina { get; set; }
        public string NombreConcepto { get; set; }
        public decimal TotalConcepto { get; set; }
        public bool IsComplemento { get; set; }
        public bool IsObligatorio { get; set; }
        public bool Seleccionado { get; set; }
        public decimal Sbc { get; set; }
        public int TipoTarifa { get; set; }
        public decimal OtrosPagos { get; set; }

        public int DiasLaborados { get; set; }
        public decimal saldo{ get; set; }

    }
    public class ProcesadoModelo2
    {
        public int IdEmpleado { get; set; }
        public int IdNomina { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public decimal Percepciones { get; set; }
        public decimal Deducciones { get; set; }
        public decimal TotalComplemento { get; set; }
        public decimal TotalNomina { get; set; }
        public List<ConceptosProcesadoModelo> Conceptos { get; set; }
        public bool Seleccionado { get; set; }
        public decimal Sbc { get; set; }
        public int TipoTarifa { get; set; }
        public decimal OtrosPagos { get; set; }

        public int DiasLaborados { get; set; }
    }
    public class ConceptosProcesadoModelo
    {
        public int IdConcepto { get; set; }
        public int IdNomina { get; set; }
        public string NombreConcepto { get; set; }
        public decimal TotalConcepto { get; set; }
        public bool IsComplemento { get; set; }
        public bool IsObligatorio { get; set; }
    }
    public class ImpuestoDetalleModel
    {
        public decimal BaseGravable { get; set; }
        public decimal BaseGravableMensual { get; set; }
        public decimal LimiteInferior { get; set; }
        public decimal Base { get; set; }
        public decimal Tasa { get; set; }
        public decimal Isr { get; set; }
        public decimal Subsidio { get; set; }
        public decimal ResultadoIsrOSubsidio { get; set; }
        public decimal NetoAPagar { get; set; }
        public decimal Porcentajeisr { get; set; }
        public bool EsISR { get; set; }
        public int IdTablaIsr { get; set; }
        public int IdTablaSubsidio { get; set; }
        public List<C_NOM_Tabla_ISR> Tablaisr { get; set; }
        public List<C_NOM_Tabla_Subsidio> Tablasubsidio { get; set; }

    }

    public class EmpleadoCredito
    {
        public int IdEmpleadoPeriodo { get; set; }
        public int IdEmpleado { get; set; }
        public int IdContrato { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaSuspension { get; set; }
        public int DiasADescontar { get; set; }
    }
    public class AguinaldoModelo
    {
        public int IdAguinaldo { get; set; }
        public int IdNomina { get; set; }
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasTrabajados { get; set; }
        public decimal Proporcion { get; set; }
        public int TotalFaltas { get; set; }
        public int? FaltasPersonalizadas { get; set; }
        public decimal Aguinaldo { get; set; }
        public decimal Isr { get; set; }
        public decimal Complemento { get; set; }
        public decimal Neto { get; set; }
        public decimal Total { get; set; }
        public decimal Factor { get; set; }
        public bool GenerarPension { get; set; }
        public decimal PensionAlimenticia { get; set; }
        public decimal SaldoAnual { get; set; }

    }
}
