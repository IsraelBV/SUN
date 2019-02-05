using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Nomina.Procesador.Modelos;
using Nomina.Procesador.Datos;
using RH.Entidades;
using Common.Enums;
using Common.Utils;
using RH.Entidades.GlobalModel;

namespace Nomina.Procesador.Metodos
{
    public static class MFiniquitoIndemnizacion
    {
        static readonly NominasDao _NominasDao = new NominasDao();

        /// <summary>
        /// Metodo que ejecuta una Tarea en un hilo independiente
        /// el metodo del cálculo del finiquito
        /// </summary>
        /// <param name="idPeriodo"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idEmpleado"></param>
        /// <param name="idCliente"></param>
        /// <param name="idSucursal"></param>
        /// <param name="arrayF"></param>
        /// <param name="calcularLiquidacion"></param>
        /// <returns></returns>
        public static Task<int> GenerarFiniquitoIndemnizacion(int idPeriodo, int idEjercicio, int idEmpleado, int idCliente, int idSucursal, ParametrosFiniquitos arrayF, bool calcularLiquidacion, int idUsuario, TotalPersonalizablesFiniquitos totalesPerson = null, bool isArt174 = false)
        {
            var t = Task.Factory.StartNew(() => CalculoFiniquitoIndemnizacion(idPeriodo, idEjercicio, idEmpleado, idCliente, idSucursal, arrayF, calcularLiquidacion, idUsuario, totalesPerson, isArt174));

            return t;
        }

        /// <summary>
        /// Metodo que realiza el Cálculo del finiquito
        /// </summary>
        /// <param name="idPeriodo"></param>
        /// <param name="idEjercicio"></param>
        /// <param name="idEmpleado"></param>
        /// <param name="idSucursal"></param>
        /// <param name="arrayF"></param>
        /// <param name="calcularLiquidacion"></param>
        /// <returns></returns>
        private static int CalculoFiniquitoIndemnizacion(int idPeriodo, int idEjercicio, int idEmpleado, int idCliente, int idSucursal, ParametrosFiniquitos arrayF, bool calcularLiquidacion, int idUsuario, TotalPersonalizablesFiniquitos totalesPerson = null, bool isArt174 = false)
        {
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            bool art96 = false;
            bool art174 = isArt174;
            decimal baseGravable174 = 0;

            try
            {
                //Guardar Detalle del finiquito
                List<NOM_Finiquito_Detalle> listaDetalleFiniquito = new List<NOM_Finiquito_Detalle>();

                #region VARIABLES PERSONALIZADAS

                //Variables Totales Personalizados
                //-1 es para indicarle al metodo que tome el valor default - el que se genera automaticamente
                decimal custom3MesesF = -1;
                decimal custom3MesesC = -1;
                decimal custom20DiasF = -1;
                decimal custom20DiasC = -1;
                decimal customPrimaF = -1;
                decimal customPrimaC = -1;
                decimal customPrimaVacF = -1;
                decimal customPrimaVacC = -1;

                var tipoTarifaUltimoSueldo = (int)Tarifas.Mensual;    // - Se usan para realizar el calculo del ISR/SUBSIDIO
                var tipoTarifaCalculoFiniquito = (int)Tarifas.Mensual;

                tipoTarifaUltimoSueldo = arrayF.TipoTarifa;
                tipoTarifaCalculoFiniquito = arrayF.TipoTarifa;

                if (totalesPerson != null)
                {
                    custom3MesesF = totalesPerson.TotalTresMesesFiscalPersonalizado;
                    custom3MesesC = totalesPerson.TotalTresMesesCompPersonalizado;

                    custom20DiasF = totalesPerson.TotalVeinteDiasFiscalPersonalizado;
                    custom20DiasC = totalesPerson.TotalVienteDiasCompPersonalizado;

                    customPrimaF = totalesPerson.TotalPrimaFiscalPersonalizado;
                    customPrimaC = totalesPerson.TotalPrimaCompPersonalizado;

                    customPrimaVacF = totalesPerson.TotalPrimaVacPersonalizado;
                    customPrimaVacC = totalesPerson.TotalPrimaVacCompPersonalizado;
                }

                //valida que el valor sea numerico
                if (arrayF.MesesSalarioF == null)
                {
                    arrayF.MesesSalarioF = -1;
                }

                if (arrayF.MesesSalarioC == null)
                {
                    arrayF.MesesSalarioC = -1;
                }

                #endregion

                //GET DATA 
                #region GET DATA - DB
                //Se obtiene el contrato actual del empleado
                var contratoEmpleado = NominasDao.GetContratoEmpleado(idEmpleado, idSucursal);
                //Se obtiene la Zona de Salario actual
                var zonaSalario = _NominasDao.GetZonaSalario();
                //obtener los dias del ejercicio fiscal
                var ejercicioFiscal = NominasDao.GetEjercicioFiscalByIdEjercicio(idEjercicio);
                //obtener el periodo del finiquito
                var periodoDelFiniquito = NominasDao.GetPeriodoById(idPeriodo);

                //validar que el contrato sea diferente de null
                if (contratoEmpleado == null || zonaSalario == null || ejercicioFiscal == null)
                {
                    if (contratoEmpleado == null)
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = 0,
                            Msg1 = "El contrato del empleado es null",
                            Msg2 = ""
                        });
                        return 0;
                    }
                    else if (zonaSalario == null)
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = 0,
                            Msg1 = "Lo valores de Zona de Salario es null",
                            Msg2 = ""
                        });
                        return 0;
                    }
                    else if (ejercicioFiscal == null)
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = 0,
                            Msg1 = "El ejercicio fiscal es null",
                            Msg2 = ""
                        });
                        return 0;
                    }
                }


                //Buscamos el Id del finiquito procesado antoeriormente y que no haya sido autorizado
                var idFiniquitoAnterior = _NominasDao.GetFiniquitoByContratoID(contratoEmpleado.IdContrato, idEmpleado, idSucursal, idPeriodo);
                #endregion

                //Si el finiquito fue procesado anteriormente se eliminaran los registros

                #region ELIMINAR FINIQUITO PROCESADO ANTERIOR

                if (idFiniquitoAnterior != null)
                {
                    if (idFiniquitoAnterior.CFDICreado == true)
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = idFiniquitoAnterior.IdFiniquito,
                            Msg1 = "No se puede procesar un finiquito que ya fue creado su cfdi",
                            Msg2 = ""
                        });
                        return 0;
                    }

                    _NominasDao.EliminarFiniquitoProcesado(idFiniquitoAnterior.IdFiniquito, idPeriodo);
                }

                #endregion

                #region CREA UN NUEVO ITEM FINIQUITO

                var itemFiniquitoNuevo = new NOM_Finiquito()
                {
                    IdFiniquito = 0,
                    IdPeriodo = idPeriodo,
                    IdContrato = contratoEmpleado.IdContrato,
                    IdEmpleado = idEmpleado,
                    IdSucursal = idSucursal,
                    IdCliente = idCliente,
                    SalarioMinimo = 0,
                    SueldoPrimaAntiguedad = 0,
                    FechaAlta = DateTime.Now,//fechaAlta,
                    FechaBaja = DateTime.Now,
                    FechaInicioAguinaldo = DateTime.Now,
                    FechaInicioVacacion = DateTime.Now,
                    DiasLaborados = 0,
                    DiasVacacionesParaCalculo = 0,
                    DiasAguinaldoParaCalculo = 0,
                    AguinaldoCorrespondiente = 0,
                    VacacionesCorrespondiente = 0,
                    PrimaVacacional = 0,
                    SD = contratoEmpleado.SD,
                    SDI = contratoEmpleado.SDI,
                    SBC = contratoEmpleado.SBC,
                    DiasEjercicioFiscal = 0,
                    L_3Meses_SueldoParaCalculo = 0,
                    VacacionesPendientes = 0,
                    PrimaVacacionPendiente = 0,
                    DiasSueldoPendiente = 0,
                    ProporcionalAguinaldo = 0,
                    ProporcionalVacaciones = 0,
                    Proporcional20DiasPorAño = 0,
                    ProporcionalPrimaAntiguedad = 0,

                    SUELDO = 0,
                    VACACIONES = 0,
                    PRIMA_VACACIONAL = 0,
                    AGUINDALDO = 0,
                    VACACIONES_PENDIENTE = 0,
                    PRIMA_VACACIONES_PENDIENTE = 0,
                    GRATIFICACION = 0,
                    L_3MESES_SUELDO = 0,
                    L_20DIAS_POR_AÑO = 0,
                    PRIMA_ANTIGUEDAD = 0,
                    COMPENSACION_X_INDEMNIZACION = 0,
                    Total_Liquidacion = 0,
                    SubTotal_Liquidacion = 0,
                    ISR_Liquidacion = 0,
                    Subsidio_Liquidacion = 0,
                    SubTotal_Finiquito = 0,
                    Total_Finiquito = 0,



                    ISR_Finiquito = 0,
                    Subsidio_Finiquito = 0,

                    //#########################################################################################################
                    TOTAL_total = 0,
                    //#########################################################################################################

                    BaseGravableUltimoSueldo = 0,
                    ISR_UltimoSueldo = 0,
                    TasaParaLiquidacion = 0,
                    Prima_Riesgo = 0,
                    SubTotal_FiniquitoF = 0,

                    ISR_FiniquitoF = 0,

                    TOTAL_totalF = 0,
                    Total_FiniquitoF = 0,
                    ISR_LiquidacionF = 0,
                    Total_LiquidacionF = 0,
                    SubTotal_LiquidacionF = 0,

                    Subsidio_FiniquitoF = 0,

                    Subsidio_LiquidacionF = 0,

                    L_AniosServicio = 0,
                    L_UltimoSueldoMensualOrd = 0,
                    L_IngresoAcumulable = 0,
                    L_IngresoNoAcumulable = 0,
                    IdEmpresaFiscal = 0,
                    IdEmpresaAsimilado = 0,
                    IdEmpresaComplemento = 0,
                    IdMetodo_Pago = 0,
                    TotalPercepciones = 0,
                    TotalDeducciones = 0,//antes estaba en 0
                    TotalComplemento = 0,
                    TotalOtrosPagos = 0,//(isrUltimoSueldo.ResultadoIsrOSubsidio*-1),
                    SubsidioCausado = 0,//isrUltimoSueldo.Subsidio,
                    SubsidioEntregado = 0,//(isrUltimoSueldo.ResultadoIsrOSubsidio * -1)
                    UMA = 0,
                    EsLiquidacion = calcularLiquidacion,
                    DescuentosAplicados = 0,
                    TipoTarifa = 0,
                    IdUsuario = idUsuario


                };

                var finiquitoGuardado = _NominasDao.AddFiniquito(itemFiniquitoNuevo);

                #endregion

                #region VARIABLES 
                //VALIDAR LAS FECHAS FISCALES CUANDO SEAN NULL
                //if (contratoEmpleado.FechaIMSS == null || contratoEmpleado.FechaBaja == null) return 0;

                DateTime fechaAltaImss = contratoEmpleado.FechaIMSS == null ? DateTime.Now : contratoEmpleado.FechaIMSS.Value;
                // fechaAltaImss = contratoEmpleado.FechaAlta;//Solicitado por Rofolfon

                //Setea la fecha que viene el array parametros

                if (arrayF.FechaAltaF != null)
                {
                    fechaAltaImss = arrayF.FechaAltaF.Value;
                }


                var fechaAlta = contratoEmpleado.FechaAlta;
                var fechaReal = contratoEmpleado.FechaReal;
                //FECHA BAJA
                //Se toma primero el que viene como parametros
                //Segundo se toma la fecha del contrato
                //Ultimo se queda con la fecha actual
                var fechaBaja = DateTime.Now;

                if (arrayF.FechaBajaF != null)
                {
                    fechaBaja = arrayF.FechaBajaF.Value;
                }
                else if (contratoEmpleado.FechaBaja != null)
                {
                    fechaBaja = contratoEmpleado.FechaBaja.Value;
                }

                if (fechaBaja.Year == 1)
                {
                    fechaBaja = DateTime.Now;
                }


                DateTime fechaAntiguedad = contratoEmpleado.FechaReal; //FechaReal o Antiguedad con el cliente

                // DateTime fecha_Imss = contratoEmpleado.FechaIMSS == null ? contratoEmpleado.FechaAlta : contratoEmpleado.FechaIMSS.Value;


                //Fechas de Aguinaldo y Vacaciones Fiscales
                DateTime fechaInicioAguinaldo = arrayF.FechaAguinaldoF == null ? fechaAltaImss : arrayF.FechaAguinaldoF.Value;
                DateTime fechaInicioVacaciones = arrayF.FechaVacacionesF == null ? fechaAltaImss : arrayF.FechaVacacionesF.Value;

                //Fechas de Aguinaldo y Vacaciones Complemento
                DateTime fechaInicioAguinaldo_C = arrayF.FechaAguinaldoC == null ? fechaAlta : arrayF.FechaAguinaldoC.Value;
                DateTime fechaInicioVacaciones_C = arrayF.FechaVacacionesC == null ? fechaAlta : arrayF.FechaVacacionesC.Value;


                //var tipoTarifaUltimoSueldo = (int)Tarifas.Semanal;    // - Se usan para realizar el calculo del ISR/SUBSIDIO
                //var tipoTarifaCalculoFiniquito = (int)Tarifas.Semanal;

                //SALARIO MINIMO - SALARIO PARA EL CÁLCULO DE ANTIGUEDAD
                decimal salarioMinimo = zonaSalario.SMG; //73.04
                decimal salarioUma = zonaSalario.UMA;

                decimal salarioParaPrimaAntiguedad = salarioUma * 2; //  73.04 * 2 = 146.08//se cambiara a uma
                decimal salarioParaPrimaAntiguedadC = salarioUma * 2; //  73.04 * 2 = 146.08



                if (contratoEmpleado.SD < salarioParaPrimaAntiguedad)
                {
                    salarioParaPrimaAntiguedad = contratoEmpleado.SD;
                }

                if (contratoEmpleado.SalarioReal < salarioParaPrimaAntiguedadC)
                {
                    salarioParaPrimaAntiguedadC = contratoEmpleado.SalarioReal;
                }

                int diasEjercicioFiscal = ejercicioFiscal.Dias;

                int idEmpresaAsimilado = 0;
                int idEmpresaComplemento = 0;
                int idMetodo_Pago = 0;
                int idEmpresaFiscal = 0;
                decimal totalPercepciones = 0;
                decimal totalDeducciones = 0;
                decimal totalComplemento = 0;
                decimal totalOtrosPagos = 0;
                decimal subsidioCausado = 0;
                decimal subsidioEntregado = 0;

                decimal totalExcentoFiniquito = 0;
                decimal totalGravadoFiniquito = 0;

                decimal ingresoAcumulableUltimoSueldo = 0;
                decimal ingresoNoAcumulableUltimoSueldo = 0;

                decimal primaDeRiesgo = 0;
                bool _usarUMAParaCalculo = true;//Para el calculo de cuotas imss
                decimal _valorUMA = 0;

                //Esta pension se obtiene del total resultante del finiquito o liquidacion, y sobre ese total se obtiene la pension
                //esta fue una peticion particularmente de alondra en monterrey
                decimal pensionAlimenticiaDelTotal = 0;
                #endregion

                #region USAR UMA-SM

                //USAR UMA O SMGV para las cuotas imss
                //Si se usara UMA o SMGV
                //B) USAR UMA - DEFAULT TRUE
                var usarUmaConfig = _NominasDao.GetValorParametrosConfig(ParametrosDeConfiguracion.AUMA);
                if (usarUmaConfig != null)
                {
                    if (usarUmaConfig.ValorBool != null)
                        _usarUMAParaCalculo = (bool)usarUmaConfig.ValorBool;
                }

                if (_usarUMAParaCalculo)
                {
                    _valorUMA = zonaSalario.UMA;
                }

                #endregion

                #region DESCUENTOS ADICIONALES ?
                //DESCUENTOS ADICIONALES 

                var listaDescuentos = _NominasDao.GetDescuentosAdicionalesFiniquito(idPeriodo);


                var listaDescuentosComplemento = listaDescuentos.Where(x => x.IsComplemento).ToList();

                var listaDescuentoFiscales = listaDescuentos.Where(x => x.IsComplemento == false).ToList();


                //Se suma el total de los descuentos de complemento
                var descuento = listaDescuentosComplemento.Sum(x => x.TotalDescuento);
                decimal totalDescuentoAdicionalComplemento = 0; // 10,140.00; 
                totalDescuentoAdicionalComplemento = descuento <= 0 ? 0 : descuento;
                decimal descuentoAplicado = 0;

                //Se suma el total descuento por fiscal
                var descuentoF = listaDescuentoFiscales.Sum(x => x.TotalDescuento);
                decimal totalDescuentoAdicionalFiscal = 0;
                totalDescuentoAdicionalFiscal = descuentoF <= 0 ? 0 : descuentoF;

                #endregion

                #region PERCEPCIONES ADICIONALES FISCALES
                //no estara disponible por el momentos : Alondra y Gordolfo dijero que solo complemento
                #endregion

                #region PERCEPCIONES ADICIONALES COMPLEMENTO - COMISIONES

                //Suma de las percepciones adicionales
                decimal comisionesComplemento = 0;

                //Get lista de comisioines
                var listaComisiones = _NominasDao.GetComisionesComplementoFiniquito(idPeriodo);

                //Sumamos el total de esas comisiones
                if (listaComisiones.Count > 0)
                {
                    comisionesComplemento = listaComisiones.Select(x => x.TotalDescuento).Sum();
                }

                //Esto se sumara a la gratificacion


                #endregion


                
                #region CANCUN 1 - BUSCAR LAS NOMINAS ANTERIORES para aplicar art174 art96

                decimal totalGravadoDelMes = 0;
                decimal totalGravadoNominasDelMes = 0;
                decimal totalIsrDelMes = 0;
                decimal totalSubsidioDelMes = 0;
                decimal totalGravadoSueldoMensual = 0;
                decimal totalIsrAntesDeDelMes = 0;
                decimal totalBaseGravabableArt174 = 0;


                ////Obtenemos la fecha del periodo del finiquito
                var fechaFinalPeriodo = periodoDelFiniquito.Fecha_Fin;
                var mesDelPeriodo = fechaFinalPeriodo.Month;
                var año = fechaFinalPeriodo.Year;

                ////Buscar los periodos autorizados que se hayan creado en el mes
                ////Por Indicacion de rodolfo se buscarás la nominas que fueron procesadas en el mes
                ////es decir, se toma la fecha del procesado de la nomina

                var listaPeriodosDelMes = NominasDao.GetIdPeriodosAutorizadosByMes(mesDelPeriodo, año, idEmpleado, contratoEmpleado.IdContrato);
                var arrayPeriodos = listaPeriodosDelMes.Select(x => x.IdPeriodoPago).ToArray();

                ////Buscamos las nominas del colaborador que esten en la lista de periodos anterior
                var arrayNominas = NominasDao.GetIdNominasByPeriodo(arrayPeriodos, idEmpleado, contratoEmpleado.IdContrato);

                //Obtiene la suma de isr antes de subsidio del mes
                // totalIsrAntesDeDelMes = listaNominas.Sum(x => x.ISRantesSubsidio);

                // var arrayNominas = listaNominas.Select(x => x.IdNomina).ToArray();

                ////Por cada nomina buscamos sus detalles para obtener los conceptos gravados
                ////sumamos el total de los gravados

                if (arrayNominas?.Length > 0)
                {
                    var listaDetalleDeNominas = NominasDao.GetDatosDetallesNominas(arrayNominas);

                    totalGravadoDelMes = NominasDao.GetGravadosByNominas(listaDetalleDeNominas);

                    totalGravadoNominasDelMes = NominasDao.GetSBCByNominas(arrayNominas);

                    totalIsrDelMes = NominasDao.GetIsrsByNominas(listaDetalleDeNominas);

                    totalSubsidioDelMes = NominasDao.GetSubsidiosByNominas(listaDetalleDeNominas);

                    totalGravadoSueldoMensual = NominasDao.GetGravadosSueldosByNominas(listaDetalleDeNominas);
                }


                #endregion nominas anteriores - cancun 1
          


                // 1
                #region VARIABLES FISCALES

                decimal diasLaborados = Utils.GetDiasEntreDosFechas(fechaAltaImss, fechaBaja);    // FechaBaja - FechaAlta Imms 
                decimal diasParaAguinaldo = Utils.GetDiasEntreDosFechas(fechaInicioAguinaldo, fechaBaja);// FechaBaja - FechaAlta Imss - condicion ? 
                decimal diasParaVaciones = Utils.GetDiasEntreDosFechas(fechaInicioVacaciones, fechaBaja); // FechaBaja - FechaAlta Imss - condicion ?

                //Datos Factores de Integracion - para obtener los dias de aguinaldo, vacaciones y prima de antiguedad
                //var añosAntiguedad = diasLaborados/diasEjercicioFiscal;
               var añosAntiguedad = fechaBaja.Year - fechaAltaImss.Year;
                añosAntiguedad = añosAntiguedad <= 0 ? 1 : añosAntiguedad;//para poder obtener el factor de integracion

                var antiguedadLaboral = 0;//usado para la liquidacion, considerando que tenga mas de 6 meses laborando
                antiguedadLaboral = Utils.GetAntiguedadLiquidacion(fechaAltaImss, fechaBaja);


                var factorIntegracion = NominasDao.GetFactoresIntegracionByAntiguedad(añosAntiguedad);// para 

                decimal aguinaldo = arrayF.DiasAguinaldoF < 0 ? factorIntegracion.DiasAguinaldo : arrayF.DiasAguinaldoF; // 15.00; // 15 dias
                decimal vacaciones = arrayF.DiasVacCorrespondientesF < 0 ? factorIntegracion.DiasVacaciones : arrayF.DiasVacCorrespondientesF;  //6;


                decimal primaVacacional = (factorIntegracion.PrimaVacacional * 100);//25; //%
                decimal sd = contratoEmpleado.SD;//127.54; //No el integrado

                // se almacena en dias, 90 equivale a los 3 meses, pero hay finiquitos que usan un proporcional ejemplo 82 dias y nos los 3 meses completos


                decimal tresMesesDeSalario = arrayF.MesesSalarioF < 0 ? 90 : arrayF.MesesSalarioF;
                decimal vacacionesPendientes = arrayF.DiasVacacionesPendientesF == 0 ? 0 : arrayF.DiasVacacionesPendientesF;
                decimal porcentajePrimaVacionalPendiente = (factorIntegracion.PrimaVacacional * 100);//25; // %
                decimal diasSueldoPendiente = arrayF.DiasSueldoPendientesF == 0 ? 0 : arrayF.DiasSueldoPendientesF; // COndicional Dias de sueldo pendientes*******

                //Se agrega la personalizacion al % de prima
                if (arrayF.PorcentajePimaVacPendienteF >= 0)
                {
                    porcentajePrimaVacionalPendiente = arrayF.PorcentajePimaVacPendienteF;
                }


                //nuevas variables

                int l_AniosServicio = 0;
                decimal l_UltimoSueldoMensualOrd = 0;
                decimal l_IngresoAcumulable = 0;
                decimal l_IngresoNoAcumulable = 0;

                decimal amortizacionInfornavit = 0;
                decimal amortizacionFonacot = 0;
                decimal amortizacionPension = 0;
                #endregion

                #region VARIABLES COMPLEMENTO

                decimal diasLaborados_C = Utils.GetDiasEntreDosFechas(fechaAntiguedad, fechaBaja);

                decimal diasParaAguinaldo_C = Utils.GetDiasEntreDosFechas(fechaInicioAguinaldo_C, fechaBaja);//210;

                decimal diasParaVaciones_C = Utils.GetDiasEntreDosFechas(fechaInicioVacaciones_C, fechaBaja);//210;

                var añosAntiguedad_C = fechaBaja.Year - fechaAntiguedad.Year;
                añosAntiguedad_C = añosAntiguedad_C <= 0 ? 1 : añosAntiguedad_C;

                var factorIntegracion_C = NominasDao.GetFactoresIntegracionByAntiguedad(añosAntiguedad_C);

                decimal aguinaldo_C = arrayF.DiasAguinaldoC < 0 ? factorIntegracion_C.DiasAguinaldo : arrayF.DiasAguinaldoC;//15;
                                                                                                                            //  double vacaciones_C = factorIntegracion_C.DiasVacaciones;//6;

                decimal vacaciones_C = arrayF.DiasVacCorrespondientesC < 0 ? factorIntegracion.DiasVacaciones : arrayF.DiasVacCorrespondientesC;  //6;

                decimal primaVacacional_C = (factorIntegracion_C.PrimaVacacional * 100);//25; //%
                decimal SDReal = contratoEmpleado.SalarioReal; //250;


                int diasEjercicioFiscal_C = ejercicioFiscal.Dias; // hacer un metodo que retorne los dias del ejercicio fiscal
                decimal tresMesesDeSalario_C = arrayF.MesesSalarioC < 0 ? 90 : arrayF.MesesSalarioC; //
                decimal vacacionesPendientes_C = arrayF.DiasVacacionesPendientesC == 0 ? 0 : arrayF.DiasVacacionesPendientesC;// obtener por parametros Vacaciones Pendientes**********
                decimal porcentajePrimaVacionalpendiente_C = (factorIntegracion_C.PrimaVacacional * 100);//25; // %
                decimal diasSueldoPendiente_C = arrayF.DiasSueldoPendientesC == 0 ? 0 : arrayF.DiasSueldoPendientesC;

                if (arrayF.PorcentajePimaVacPendienteC >= 0)
                {
                    porcentajePrimaVacionalpendiente_C = arrayF.PorcentajePimaVacPendienteC;
                }

                #endregion

                #region DATOS DE LA EMPRESA PARA OBTENER LA PRIMA DE RIESGO
                int? idEmpresa = 0;

                if (contratoEmpleado.IdEmpresaFiscal != null)
                {
                    idEmpresa = contratoEmpleado.IdEmpresaFiscal;
                }
                //else if (contratoEmpleado.IdEmpresaAsimilado != null)
                //{
                //    idEmpresa = contratoEmpleado.IdEmpresaFiscal;
                //}

                if (idEmpresa == null) return 0;
                if (idEmpresa <= 0) return 0;

                var empresa = _NominasDao.GetEmpresa(idEmpresa);

                if (empresa == null) return 0;

                if (empresa.PrimaRiesgo == null) return 0;

                primaDeRiesgo = empresa.PrimaRiesgo.Value;


                #endregion

                // 2
                #region PROPORCIONALES FISCALES -  Regla de 3

                decimal proporcionalAguinaldo = (diasParaAguinaldo * aguinaldo) / diasEjercicioFiscal; //(DiasParaAguinaldo * 15) / 365
                decimal proporcionalVacaciones = (diasParaVaciones * vacaciones) / diasEjercicioFiscal; //(DiasParaVaciones * 6) /365

                //truncate para el manejo de 2 decimales
                proporcionalAguinaldo = Utils.TruncateDecimales(proporcionalAguinaldo);
                proporcionalVacaciones = Utils.TruncateDecimales(proporcionalVacaciones);


                //Prima Antiguedad 12 dias por cada año laborado 
                decimal proporcionalPrimaAntiguedad = (diasLaborados * 12) / diasEjercicioFiscal; //Para Indemnizacion

                // 20 dias de sueldo por cada año laborado 
                decimal proporcional20DiasdeSueldo = (diasLaborados * 20) / diasEjercicioFiscal; //Para Indemnizacion (Pendiente Formulado)

                if (arrayF.VeinteDiasPorAF > -1)
                {
                    proporcional20DiasdeSueldo = arrayF.VeinteDiasPorAF;
                }

                #endregion

                #region PROPORCIONALES COMPLEMENTO  - Regla de 3

                decimal proporcionalAguinaldo_C = Utils.TruncateDecimales((diasParaAguinaldo_C * aguinaldo_C) / diasEjercicioFiscal_C); //(DiasParaAguinaldo * 15) / 365

                decimal proporcionalVacaciones_C = Utils.TruncateDecimales((diasParaVaciones_C * vacaciones_C) / diasEjercicioFiscal_C); //(DiasParaVaciones * 6) /365

                //proporcional de vacaciones tomando la fecha de antiguedad - 12 dias por año
                //var añosAntiguedad = fechaBaja.Year - fechaAntiguedad.Year;
                var proporcionalAntiguedad = añosAntiguedad * 12;

                // se suma la proporcionalAntiguedad con proporcionalVacaciones
                // proporcionalVacaciones_C += proporcionalAntiguedad; //No se aplica para vacaciones


                //Prima Antiguedad 12 dias por cada año laborado 
                decimal proporcionalprimaAntiguedad_C = (diasLaborados_C * 12) / diasEjercicioFiscal_C;


                // 20 dias de sueldo por cada año laborado 
                decimal veinteDiasdeSueldoComplento = (diasLaborados_C * 20) / diasEjercicioFiscal_C;

                if (arrayF.VeinteDiasPorAC > -1)
                {
                    veinteDiasdeSueldoComplento = arrayF.VeinteDiasPorAC;
                }

                #endregion

                // 3
                #region CALCULO DE CONCEPTOS FISCALES

                decimal _SUELDO = Utils.TruncateDecimales(sd * diasSueldoPendiente); //- totalDescuentoAdicional;//se aplica por rubro  //Alondra - se hara el descuento a SUELDO F
                decimal _VACACIONES = Utils.TruncateDecimales(sd * proporcionalVacaciones);
                decimal _PRIMA_VACACIONAL = Utils.TruncateDecimales(_VACACIONES * (primaVacacional / 100));
                decimal _AGUINALDO = Utils.TruncateDecimales(sd * proporcionalAguinaldo);

                decimal multiplodeNoventa2 = arrayF.MesesSalarioF < 0 ? 90 : arrayF.MesesSalarioF;

                decimal _3_MESES_SALARIO = calcularLiquidacion == true ? Utils.TruncateDecimales((sd * tresMesesDeSalario)) : 0;//se queda sd comento rodolfo
                //el cambio solo fue la parte exenta para la liquidacion
             
                if (custom3MesesF >= 0)
                {
                    _3_MESES_SALARIO = custom3MesesF;
                }


                if (customPrimaVacF >= 0)
                {
                    _PRIMA_VACACIONAL = customPrimaVacF;
                }


                decimal _PRIMA_ANTIGUEDAD = calcularLiquidacion == false ? 0 : Utils.TruncateDecimales(proporcionalPrimaAntiguedad * salarioParaPrimaAntiguedad);//primaAntiguedad * SD;//otros con salario minimo
                decimal _VACACIONES_PENDIENTES = Utils.TruncateDecimales(sd * vacacionesPendientes);
                decimal _PRIMA_VACACIONAL_PENDIENTE = Utils.TruncateDecimales(_VACACIONES_PENDIENTES * (porcentajePrimaVacionalPendiente / 100));
                decimal _20_DIAS_POR_AÑO = calcularLiquidacion == true ? Utils.TruncateDecimales((sd * proporcional20DiasdeSueldo)) : 0;

                if (custom20DiasF >= 0)
                {
                    _20_DIAS_POR_AÑO = custom20DiasF;
                }

                if (customPrimaF >= 0)
                {
                    _PRIMA_ANTIGUEDAD = customPrimaF;
                }
                #endregion

                #region CALCULO DE CONCEPTOS _C

                decimal SUELDO_C = Utils.TruncateDecimales((SDReal * diasSueldoPendiente_C));
                decimal VACACIONES_C = Utils.TruncateDecimales(SDReal * proporcionalVacaciones_C);
                decimal _AGUINALDO_C = Utils.TruncateDecimales(SDReal * proporcionalAguinaldo_C);
                decimal PRIMA_VACACIONAL_C = Utils.TruncateDecimales(VACACIONES_C * (primaVacacional_C / 100));
                decimal AGUINALDO_C = Utils.TruncateDecimales(SDReal * proporcionalAguinaldo_C);
                decimal _3_MESES_SALARIO_C = calcularLiquidacion == true ? Utils.TruncateDecimales((SDReal * tresMesesDeSalario_C)) : 0; // calcularLiquidacion == true ? (SDReal * tresMesesDeSalario_C) - totalDescuentoAdicional : 0; // restar descuentos adicionales en caso de liquidacion

                if (custom3MesesC >= 0)
                {
                    _3_MESES_SALARIO_C = custom3MesesC;
                }

                if (customPrimaVacC >= 0)
                {
                    PRIMA_VACACIONAL_C = customPrimaVacC;
                }


                decimal PRIMA_ANTIGUEDAD_C = calcularLiquidacion == false ? 0 : Utils.TruncateDecimales(proporcionalprimaAntiguedad_C * salarioParaPrimaAntiguedadC);
                decimal VACACIONES_PENDIENTES_C = Utils.TruncateDecimales(SDReal * vacacionesPendientes_C);
                decimal PRIMA_VACACIONAL_PENDIENTE_C = Utils.TruncateDecimales(VACACIONES_PENDIENTES_C * (porcentajePrimaVacionalpendiente_C / 100));
                decimal _20DIASPORAÑO_C = calcularLiquidacion == true ? Utils.TruncateDecimales((SDReal * veinteDiasdeSueldoComplento)) : 0;

                if (custom20DiasC >= 0)
                {
                    _20DIASPORAÑO_C = custom20DiasC;
                }
                if (customPrimaC >= 0)
                {
                    PRIMA_ANTIGUEDAD_C = customPrimaC;
                }
                #endregion

                //Suma de las diferencias de los conceptos.   dif = concepto_C - conceptoFiscal

                #region GRATIFICACION - COMPENSACION POR INDEMNIZACION - TOTAL FISCAL Y TOTAL COMPLEMENTO

                //decimal GRATIFICACION = (SUELDO_C - _SUELDO) + (VACACIONES_C - _VACACIONES) +
                //                       (PRIMA_VACACIONAL_C - _PRIMA_VACACIONAL) + (AGUINALDO_C - _AGUINALDO) +
                //                       (VACACIONES_PENDIENTES_C - _VACACIONES_PENDIENTES) +
                //                       (PRIMA_VACACIONAL_PENDIENTE_C - _PRIMA_VACACIONAL_PENDIENTE);


                decimal GRATIFICACION = 0;

                var difSueldo = SUELDO_C - _SUELDO;
                var difVacaciones = VACACIONES_C - _VACACIONES;
                var difPrimaVac = PRIMA_VACACIONAL_C - _PRIMA_VACACIONAL;
                var difAguinaldo = AGUINALDO_C - _AGUINALDO;
                var difVacPendientes = VACACIONES_PENDIENTES_C - _VACACIONES_PENDIENTES;
                var difPrimaVacPendientes = PRIMA_VACACIONAL_PENDIENTE_C - _PRIMA_VACACIONAL_PENDIENTE;

                if (difSueldo > 0)
                    GRATIFICACION += difSueldo;
                if (difVacaciones > 0)
                    GRATIFICACION += difVacaciones;
                if (difPrimaVac > 0)
                    GRATIFICACION += difPrimaVac;
                if (difAguinaldo > 0)
                    GRATIFICACION += difAguinaldo;
                if (difVacPendientes > 0)
                    GRATIFICACION += difVacPendientes;
                if (difPrimaVacPendientes > 0)
                    GRATIFICACION += difPrimaVacPendientes;



                //SUMA  A LA GRATICIACION LA CANTIDA DE COMISIONES ADICIONALES
                GRATIFICACION += comisionesComplemento;

                //decimal COMPENSACIONxINDEMNIZACION = (_3_MESES_SALARIO_C - _3_MESES_SALARIO) +
                //                                    (PRIMA_ANTIGUEDAD_C - _PRIMA_ANTIGUEDAD) +
                //                                    (_20DIASPORAÑO_C - _20_DIAS_POR_AÑO);//? va o no va aqui
                //                                                                         //(PRIMA_VACACIONAL_PENDIENTE_C - _PRIMA_VACACIONAL_PENDIENTE); se quita y se pasa a gratificacion


                decimal COMPENSACIONxINDEMNIZACION = 0;
                var dif3Meses = (_3_MESES_SALARIO_C - _3_MESES_SALARIO);
                var difPrimAntiguedad = (PRIMA_ANTIGUEDAD_C - _PRIMA_ANTIGUEDAD);
                var dif20Dias = (_20DIASPORAÑO_C - _20_DIAS_POR_AÑO);//? va o no va aqui
                                                                     //(PRIMA_VACACIONAL_PENDIENTE_C - _PRIMA_VACACIONAL_PENDIENTE); se quita y se pasa a gratificacion

                if (dif3Meses > 0)
                    COMPENSACIONxINDEMNIZACION += dif3Meses;
                if (difPrimAntiguedad > 0)
                    COMPENSACIONxINDEMNIZACION += difPrimAntiguedad;
                if (dif20Dias > 0)
                    COMPENSACIONxINDEMNIZACION += dif20Dias;


                #endregion

                #region APLICAR DESCUENTOS ADICIONALES COMPLEMENTO

                //1) DESCONTAR A GRATIFICACION
                //1.2) DESCONTAR A COMPENSACION X INDEMNIZACION - EN UNA LIQUIDACION

                Dictionary<int, decimal> dicPercepciones = new Dictionary<int, decimal>();

                dicPercepciones.Add(1, GRATIFICACION);
                if (calcularLiquidacion) { dicPercepciones.Add(2, COMPENSACIONxINDEMNIZACION); }
                dicPercepciones.Add(3, _AGUINALDO);
                dicPercepciones.Add(4, _VACACIONES);
                dicPercepciones.Add(5, _SUELDO);

                //Ordena por cantidades de mayor a menor
                List<KeyValuePair<int, decimal>> sorted = (from kv in dicPercepciones orderby kv.Value descending select kv).ToList();

                //Realiza el descuento pasando por cada rubro
                //y termina si el total a descontar es cero.


                foreach (KeyValuePair<int, decimal> entry in sorted)
                {
                    if (totalDescuentoAdicionalComplemento <= 0) break;//antes guardar el total de descuento

                    //Realizar el descuento 
                    switch (entry.Key)
                    {
                        case 1://GRATIFICACION
                            if (GRATIFICACION >= totalDescuentoAdicionalComplemento)
                            {
                                GRATIFICACION = (GRATIFICACION - totalDescuentoAdicionalComplemento);
                                descuentoAplicado += totalDescuentoAdicionalComplemento;
                                totalDescuentoAdicionalComplemento = 0;
                            }
                            else //se descuenta proporcional 
                            {
                                totalDescuentoAdicionalComplemento = (totalDescuentoAdicionalComplemento - GRATIFICACION);
                                descuentoAplicado += GRATIFICACION;
                                GRATIFICACION = 0;
                            }
                            break;
                        case 2://COMPENSACION X INDEMNIZACION
                            if (COMPENSACIONxINDEMNIZACION >= totalDescuentoAdicionalComplemento)
                            {
                                COMPENSACIONxINDEMNIZACION = (COMPENSACIONxINDEMNIZACION - totalDescuentoAdicionalComplemento);

                                descuentoAplicado += totalDescuentoAdicionalComplemento;
                                totalDescuentoAdicionalComplemento = 0;
                            }
                            else //se descuenta proporcional 
                            {

                                totalDescuentoAdicionalComplemento = (totalDescuentoAdicionalComplemento - COMPENSACIONxINDEMNIZACION);
                                descuentoAplicado += COMPENSACIONxINDEMNIZACION;
                                COMPENSACIONxINDEMNIZACION = 0;
                            }
                            break;
                        case 3://AGUINALDO
                            if (_AGUINALDO >= totalDescuentoAdicionalComplemento)
                            {
                                _AGUINALDO = (_AGUINALDO - totalDescuentoAdicionalComplemento);

                                descuentoAplicado += totalDescuentoAdicionalComplemento;
                                totalDescuentoAdicionalComplemento = 0;
                            }
                            else //se descuenta proporcional 
                            {

                                totalDescuentoAdicionalComplemento = (totalDescuentoAdicionalComplemento - _AGUINALDO);
                                descuentoAplicado += _AGUINALDO;
                                _AGUINALDO = 0;
                            }
                            break;
                        case 4://VACACIONES
                            if (_VACACIONES >= totalDescuentoAdicionalComplemento)
                            {
                                _VACACIONES = (_VACACIONES - totalDescuentoAdicionalComplemento);

                                descuentoAplicado += totalDescuentoAdicionalComplemento;
                                totalDescuentoAdicionalComplemento = 0;
                            }
                            else //se descuenta proporcional 
                            {

                                totalDescuentoAdicionalComplemento = (totalDescuentoAdicionalComplemento - _VACACIONES);
                                descuentoAplicado += _VACACIONES;
                                _VACACIONES = 0;
                            }
                            break;
                        case 5://SUELDOS
                            if (_SUELDO >= totalDescuentoAdicionalComplemento)
                            {
                                _SUELDO = (_SUELDO - totalDescuentoAdicionalComplemento);

                                descuentoAplicado += totalDescuentoAdicionalComplemento;
                                totalDescuentoAdicionalComplemento = 0;
                            }
                            else //se descuenta proporcional 
                            {
                                totalDescuentoAdicionalComplemento = (totalDescuentoAdicionalComplemento - _SUELDO);
                                descuentoAplicado += _SUELDO;
                                _SUELDO = 0;
                            }
                            break;
                    }
                }


                //2) DESCONTAR A AGUINALDO
                //3) DESCONTAR A VACACIONES
                //4) DESCONTAR A SUELDO

                #endregion

                #region TOTALES - F y C 

                decimal TOTAL_FISCAL = (_SUELDO + _VACACIONES + _PRIMA_VACACIONAL + _AGUINALDO + _3_MESES_SALARIO +
                       _PRIMA_ANTIGUEDAD + _VACACIONES_PENDIENTES + _PRIMA_VACACIONAL_PENDIENTE +
                       _20_DIAS_POR_AÑO);

                //Se suma la gratificacion al total del finiquito
                TOTAL_FISCAL += GRATIFICACION;
                TOTAL_FISCAL += COMPENSACIONxINDEMNIZACION;

                decimal TOTAL__C = (SUELDO_C + VACACIONES_C + PRIMA_VACACIONAL_C + AGUINALDO_C + _3_MESES_SALARIO_C +
                                   PRIMA_ANTIGUEDAD_C + VACACIONES_PENDIENTES_C + PRIMA_VACACIONAL_PENDIENTE_C +
                                   _20DIASPORAÑO_C);

                #endregion
                // 4
                #region FISCAL - GRAVADOS Y EXCENTOS DE CONCEPTOS

                // TOPE EXCENTO DE LA PRIMA VACACIONAL Y AGUINALDO
                //##############################################################
                decimal Tope_PrimaVacacional = salarioMinimo * 15;        //tope
                decimal Tope_Aguinaldo = salarioMinimo * 30;              //tope
                //##############################################################

                decimal sueldoGravado = _SUELDO; //se grava al 100%
                decimal VacacionesGravadas = _VACACIONES;//grava al 100%
                decimal PrimaVacacionalExcento = _PRIMA_VACACIONAL <= Tope_PrimaVacacional ? _PRIMA_VACACIONAL : Tope_PrimaVacacional;
                decimal PrimaVacacionaGravado = _PRIMA_VACACIONAL - PrimaVacacionalExcento;
                decimal AguinaldoExcento = _AGUINALDO <= Tope_Aguinaldo ? _AGUINALDO : Tope_Aguinaldo;
                decimal AguinaldoGravado = _AGUINALDO - AguinaldoExcento;
                decimal VacacionesPendienteGravado = _VACACIONES_PENDIENTES;  //grava al 100%
                decimal PrimaVacacionalPendienteExcento = _PRIMA_VACACIONAL_PENDIENTE <= Tope_PrimaVacacional ? _PRIMA_VACACIONAL_PENDIENTE : Tope_PrimaVacacional;
                decimal PrimaVacacionalPendienteGravado = _PRIMA_VACACIONAL_PENDIENTE - PrimaVacacionalPendienteExcento;
                decimal GratificacionGravada = GRATIFICACION;

                if (calcularLiquidacion)
                {
                    //  GratificacionGravada += COMPENSACIONxINDEMNIZACION;// + COMPENSACIONxINDEMNIZACION;
                }




                totalGravadoFiniquito = sueldoGravado + VacacionesGravadas + PrimaVacacionaGravado + AguinaldoGravado +
                                          VacacionesPendienteGravado + PrimaVacacionalPendienteGravado +
                                          GratificacionGravada;

                totalExcentoFiniquito = PrimaVacacionalExcento + AguinaldoExcento + PrimaVacacionalPendienteExcento;

                #endregion

                #region SUBTOTAL FINIQUITO - BASE GRAVABLE FINIQUITO

                // subTotal de Finiquito Fiscal-Complemento
                decimal subTotalFiniquito = _SUELDO + _VACACIONES + _PRIMA_VACACIONAL + _AGUINALDO + _VACACIONES_PENDIENTES +
                                           _PRIMA_VACACIONAL_PENDIENTE + GRATIFICACION;

                //suma cuando sea liquidacion
                if (calcularLiquidacion)
                {
                    //  subTotalFiniquito += COMPENSACIONxINDEMNIZACION;

                }
                // subTotal de Finiquito Fiscal
                decimal subTotalFiniquitoF = _SUELDO + _VACACIONES + _PRIMA_VACACIONAL + _AGUINALDO + _VACACIONES_PENDIENTES +
                                           _PRIMA_VACACIONAL_PENDIENTE;
                // otros casos se suma estos dos (GRATIFICACION + COMPENSACIONxINDEMNIZACION)

                //Base Gravable Finiquito Fiscal-Complemento
                decimal BaseGravableFiniquito = sueldoGravado + VacacionesGravadas + PrimaVacacionaGravado + AguinaldoGravado +
                                               VacacionesPendienteGravado + PrimaVacacionalPendienteGravado +
                                               GratificacionGravada;
                //Base Gravable Finiquito Fiscal
                decimal BaseGravableFiniquitoF = sueldoGravado + VacacionesGravadas + PrimaVacacionaGravado + AguinaldoGravado +
                                             VacacionesPendienteGravado + PrimaVacacionalPendienteGravado;
                #endregion

                // 5
                // CALCULO DE ISR DEL FINIQUITO
                //ISR Finiquito Fiscal-Complemento

                #region SUELDO PENDIENTE

                //GUARDAR DETALLES DEL FINIQUITO
                //_NominasDao.AddDetalleFiniquito(listaDetalleFiniquito);

                //Guardar Cuota IMSS
                //Monterrey captura la cuota imss directamente en el modulo de deducciones adicionales

                //Si el colaborador esta activo se aplica esta linea
                //if (contratoEmpleado.Status == true)
                //{

                //  _SUELDO = (sd * diasSueldoPendiente);

                if (_SUELDO > 0)
                {
                    //MCuotasImss.CuotasImss(zonaSalario.SMG, zonaSalario.UMA, primaDeRiesgo, nomina: null,
                    //    finiquito: itemFiniquito, usarUMA: _usarUMAParaCalculo,
                    //    diasDelPeriodo: periodoDelFiniquito.DiasPeriodo);

                    var itemImss = MCuotasImss.CuotasImss(zonaSalario.SMG, zonaSalario.UMA, primaDeRiesgo, nomina: null, finiquito: finiquitoGuardado, usarUMA: _usarUMAParaCalculo, diasDelPeriodo: (int)diasSueldoPendiente);

                    if (itemImss != null)
                    {
                        totalDescuentoAdicionalFiscal += itemImss.Total;
                    }

                    NOM_Nomina nominaSueldo = new NOM_Nomina();
                    nominaSueldo.IdNomina = finiquitoGuardado.IdFiniquito;
                    nominaSueldo.Dias_Laborados = (int)diasSueldoPendiente;
                    nominaSueldo.IdContrato = finiquitoGuardado.IdContrato;
                    nominaSueldo.SD = finiquitoGuardado.SD;
                    nominaSueldo.SDI = finiquitoGuardado.SDI;
                    nominaSueldo.SBC = finiquitoGuardado.SBC;

                    var itemdetalleInfo = MDeducciones.PrestamoInfonavit(nominaSueldo, periodoDelFiniquito, -1);
                    var itemDetalleFona = MDeducciones.PrestamoFonacot(nominaSueldo, periodoDelFiniquito);

                    // monterrey alondra - La pension alimenticia ya no se va a calcular del sueldo pendiente
                    // sino que se obtendrá del neto -
                    NOM_Nomina_Detalle itemDetallePension = null;
                    //Habilitar Para cancun 2, 
                    itemDetallePension = MDeducciones.PensionAlimenticia(nominaSueldo, contratoEmpleado, false, 0);
                    //mty 1 ya no lo ocupa aqui -- comentar esta linea

                    #region INFONAVIT

                    if (itemdetalleInfo != null)
                    {
                        totalDescuentoAdicionalFiscal += itemdetalleInfo.Total;
                        amortizacionInfornavit = itemdetalleInfo.Total;//usado para la factura

                        var itemInfo = new NOM_Finiquito_Detalle()
                        {
                            Id = 0,
                            IdFiniquito = finiquitoGuardado.IdFiniquito,
                            IdConcepto = itemdetalleInfo.IdConcepto,
                            Total = itemdetalleInfo.Total,
                            GravadoISR = itemdetalleInfo.GravadoISR,
                            ExentoISR = itemdetalleInfo.ExentoISR,
                            IdPrestamo = itemdetalleInfo.IdPrestamo,
                            Liq = false
                        };

                        listaDetalleFiniquito.Add(itemInfo);
                    }
                    #endregion

                    #region FONACOT
                    if (itemDetalleFona != null)
                    {
                        foreach (var itemF in itemDetalleFona)
                        {
                            if (itemF == null) continue;

                            totalDescuentoAdicionalFiscal += itemF.Total;
                            amortizacionFonacot += itemF.Total;//usado para la factura

                            var itemD = new NOM_Finiquito_Detalle()
                            {
                                Id = 0,
                                IdFiniquito = finiquitoGuardado.IdFiniquito,
                                IdConcepto = itemF.IdConcepto,
                                Total = itemF.Total,
                                GravadoISR = itemF.GravadoISR,
                                ExentoISR = itemF.ExentoISR,
                                IdPrestamo = itemF.IdPrestamo,
                                Liq = false
                            };

                            listaDetalleFiniquito.Add(itemD);

                        }
                    }

                    #endregion

                    #region PENSION ALIMENTICIA

                    if (itemDetallePension != null)
                    {
                        totalDescuentoAdicionalFiscal += itemDetallePension.Total;
                        amortizacionPension = itemDetallePension.Total;

                        var itemPension = new NOM_Finiquito_Detalle()
                        {
                            Id = 0,
                            IdFiniquito = finiquitoGuardado.IdFiniquito,
                            IdConcepto = itemDetallePension.IdConcepto,
                            Total = itemDetallePension.Total,
                            GravadoISR = itemDetallePension.GravadoISR,
                            ExentoISR = itemDetallePension.ExentoISR,
                            IdPrestamo = itemDetallePension.IdPrestamo,
                            Liq = false
                        };

                        listaDetalleFiniquito.Add(itemPension);
                    }
                    #endregion

                }
                //}

                #endregion

                #region ISR - SUBSIDIO FINIQUITO
                var diasPeriodo = 15;

                #region CANCUN 3 - se suman lo gravado del mes al gravado del finiquito
                
                //Actualizar la BaseGravable del finiquito sumando la base gravable acumulado del mes <---------------------------|
                //decimal nuevaBaseGravableFiniquito = BaseGravableFiniquito + totalGravadoDelMes;// cun
                //BaseGravableFiniquito = nuevaBaseGravableFiniquito;

                //art174 - 
                decimal holdBaseGravableFiniquito = BaseGravableFiniquito;                
                decimal ingresoGravableInicial = 0;
                IsrSubsidio IsrSubsidioBaseInicial = new IsrSubsidio();


                if (art174)
                {
                    //PRIM VACACIONAL
                    //AGUINALDO
                    //prima dominical
                    //PRIMA VAC PENDIENTE
                    //participacion utilidades
                    //decimal BaseGravableFiniquito = sueldoGravado + VacacionesGravadas + PrimaVacacionaGravado + AguinaldoGravado + VacacionesPendienteGravado + PrimaVacacionalPendienteGravado + GratificacionGravada;

                    totalBaseGravabableArt174 = PrimaVacacionaGravado + PrimaVacacionalPendienteGravado + AguinaldoGravado;

                    ingresoGravableInicial = totalBaseGravabableArt174 + GratificacionGravada + VacacionesGravadas;        

                    BaseGravableFiniquito = ingresoGravableInicial;
                }
                else//96
                {
                    decimal nuevaBase96 = BaseGravableFiniquito + totalGravadoDelMes;
                    BaseGravableFiniquito = nuevaBase96;
                }
                
                #endregion base gravada cancun 3

           



                //EJECUTA EL METODO PARA OBTENER EL ISR 96
                var ISRFiniquito = MNominas.CalculoIsrSubsidioFin(null, BaseGravableFiniquito, sd, diasPeriodo, tipoTarifaCalculoFiniquito);


                #region Cancun 4 - aplicando la regla sumar-restar lo obtenido del mes en isr/subsidio
                
                //APLICAR LA REGLA DE RESTAR el isr o subsidio del acumulado del mes <--------------------------------------------------------------------------|
                if (ISRFiniquito != null)
                {
                    if (art174)
                    {
                        //public static IsrSubsidio CalculoIsrSubsidio174(decimal ingresoGravableInicial, decimal ISRFiniquito., decimal sd, decimal isrMonto174, decimal monto174)
                        ISRFiniquito = MNominas.CalculoIsrSubsidio174(ingresoGravableInicial,sd);
                    }
                    else//art96
                    {
                        if (totalSubsidioDelMes > 0 || totalIsrDelMes > 0)
                        {
                            #region art 96 - Aplicar regla

                            decimal nuevoValor = 0;
                            decimal subMes = 0;
                            decimal isrMes = 0;
                            bool nuevoisrIs = true;
                            //isr:-isr + sub
                            //sub:-sub + isr

                            //PRIMERO OBTENEMOS QUIEN ES MAYOR Y REALIZAMOS LA RESTA
                            //UNO DE LOS DOS SE QUEDARA EN CERO
                            if (totalSubsidioDelMes > totalIsrDelMes)
                            {
                                subMes = totalSubsidioDelMes - totalIsrDelMes;
                                isrMes = 0;
                            }
                            else
                            {
                                isrMes = totalIsrDelMes - totalSubsidioDelMes;
                                subMes = 0;
                            }

                            //SEGUNDO 
                            if (ISRFiniquito.EsISR)
                            {
                                nuevoValor = ISRFiniquito.ResultadoIsrOSubsidio - isrMes + subMes;

                                nuevoisrIs = nuevoValor > 0;
                            }
                            else
                            {
                                nuevoValor = (ISRFiniquito.ResultadoIsrOSubsidio - subMes + isrMes);

                                nuevoisrIs = nuevoValor < 0;// && false;
                            }

                            //SET NUEVO VALORES AL OBJETO
                            if (nuevoValor < 0)
                                nuevoValor = nuevoValor * -1;

                            //actualizamos el objeto con el nuevo valor de resultado
                            ISRFiniquito.ResultadoIsrOSubsidio = nuevoValor;
                            ISRFiniquito.EsISR = nuevoisrIs;

                            #endregion
                        }
                    }

                }
                
                #endregion art96-174 - aplicar regla cancun 4



                if (ISRFiniquito != null)
                    if (!ISRFiniquito.EsISR)
                    {
                        totalOtrosPagos = ISRFiniquito.ResultadoIsrOSubsidio;
                        subsidioCausado = ISRFiniquito.Subsidio;
                        subsidioEntregado = ISRFiniquito.ResultadoIsrOSubsidio;
                    }
                    else
                    {
                        subsidioCausado = ISRFiniquito.Subsidio;
                    }



                //ISR Finiquito Fiscal
                //var ISRFiniquitoF = MNominas.CalculoIsrSubsidioFin(null, BaseGravableFiniquito, sd, diasPeriodo, tipoTarifaCalculoFiniquito);



                #endregion
                //6
                #region CALCULO ULTIMO SUELDO FISCAL - TASA % PARA LA LIQUIDACION

                decimal ultimoSueldo = calcularLiquidacion == false ? 0 : sd * 30;


                var isrUltimoSueldo = MNominas.CalculoIsrSubsidioFin(null, ultimoSueldo, sd, diasPeriodo, tipoTarifaUltimoSueldo);


                //Tasa para Liquidacion - se obtiene un porcentaje %
                decimal tasaParaLiquidacion = 0;
                tasaParaLiquidacion = isrUltimoSueldo == null ? 0 : ((isrUltimoSueldo.IsrAntesDeSub / isrUltimoSueldo.BaseGravable) * 100) / 100;


                tasaParaLiquidacion = Math.Round(tasaParaLiquidacion, 4);

                ingresoNoAcumulableUltimoSueldo = isrUltimoSueldo == null ? 0 : isrUltimoSueldo.Base;
                ingresoAcumulableUltimoSueldo = isrUltimoSueldo == null ? 0 : (ultimoSueldo - ingresoNoAcumulableUltimoSueldo);

                #endregion
                // 7
                #region CALCULO DE LA LIQUIDACION
                //TOPES EXCENTOS DE LOS CONCEPTOS DE LA LIQUIDACION
                //##############################################################
                // (SM * 90) x (años de servicio) 

                decimal multiplodeNoventa = arrayF.MesesSalarioF < 0 ? 90 : arrayF.MesesSalarioF;

                //double proporcion_exentaLiquidacion = (salarioMinimo * 90)* añosAntiguedad;
                decimal proporcion_exentaLiquidacion = Utils.TruncateDecimales((salarioUma * multiplodeNoventa) * antiguedadLaboral);
                //decimal proporcion_exentaLiquidacion_antes = Utils.TruncateDecimales((salarioMinimo * multiplodeNoventa) * antiguedadLaboral); //antes se tenia salario minimo

                decimal grabablePrimaDeAntiguedad = 0;
                decimal porcionExentaPrimaDeAntiguedad = 0;

                //Compensacion x Indemnizacion 100% gravado
                //##############################################################
                if (_PRIMA_ANTIGUEDAD > 0)
                {
                    if (proporcion_exentaLiquidacion <= _PRIMA_ANTIGUEDAD)
                    {
                        grabablePrimaDeAntiguedad = _PRIMA_ANTIGUEDAD - proporcion_exentaLiquidacion;
                        porcionExentaPrimaDeAntiguedad = proporcion_exentaLiquidacion;

                        proporcion_exentaLiquidacion = 0;
                    }
                    else
                    {
                        grabablePrimaDeAntiguedad = 0;
                        porcionExentaPrimaDeAntiguedad = _PRIMA_ANTIGUEDAD;

                        proporcion_exentaLiquidacion -= _PRIMA_ANTIGUEDAD;
                    }
                }
                //cambio el dia 27-08-2018 actualizacion de calculo de indemnizacion
                decimal subTotalIndemnizacion = calcularLiquidacion == false ? 0 : _3_MESES_SALARIO + _20_DIAS_POR_AÑO + COMPENSACIONxINDEMNIZACION;

                //Sub Total de Liquidacion Fiscal-Complemento
                //decimal subTotalIndemnizacion = calcularLiquidacion == false
                //    ? 0
                //    : _3_MESES_SALARIO + _20_DIAS_POR_AÑO + _PRIMA_ANTIGUEDAD + COMPENSACIONxINDEMNIZACION;
                ////Sub Total de Liquidacion Fiscal
                //decimal subTotalIndemnizacionF = calcularLiquidacion == false ? 0 : _3_MESES_SALARIO + _20_DIAS_POR_AÑO + _PRIMA_ANTIGUEDAD;

                //Total Base Gravable Indermizacion  Fiscal-Complemento 
                //decimal totalBaseGravableIndemnizacion = subTotalIndemnizacion - proporcion_exentaLiquidacion;
                decimal totalBaseGravableIndemnizacion = 0;
                ////Total Base Gravable Indermizacion  Fiscal
                //decimal totalBaseGravableIndemnizacionF = subTotalIndemnizacionF - proporcion_exentaLiquidacion;
                if (subTotalIndemnizacion > 0)
                {
                    if (proporcion_exentaLiquidacion != 0)
                    {
                        if (proporcion_exentaLiquidacion <= subTotalIndemnizacion)
                        {
                            totalBaseGravableIndemnizacion = subTotalIndemnizacion - proporcion_exentaLiquidacion;
                        }
                        else
                        {
                            totalBaseGravableIndemnizacion = 0;
                            proporcion_exentaLiquidacion = subTotalIndemnizacion;
                        }
                    }
                    else
                    {
                        totalBaseGravableIndemnizacion = subTotalIndemnizacion;
                    }
                }

                // isr liquidacion fiscal-complemento
                //se suman los grabables de la prima de antiguedad y la indemnizacion y se mul+tiplica por la tasapara liquidacion
                decimal ISR_Liquidacion = Utils.TruncateDecimales((totalBaseGravableIndemnizacion + grabablePrimaDeAntiguedad) * tasaParaLiquidacion);
                ////isr liquidacion Fiscal
                //decimal ISR_LiquidacionF = Utils.TruncateDecimales(totalBaseGravableIndemnizacionF * tasaParaLiquidacion);

                //NUEVOS DATOS DEL SAT
                l_AniosServicio = antiguedadLaboral;//contempla un año si es mayor a 6 meses

                #endregion

                // neto pagar liquidacion Fiscal-COmplemento
                #region NETO A PAGAR
                decimal NetoAPagarLiquidacion = (subTotalIndemnizacion + _PRIMA_ANTIGUEDAD) - (ISR_Liquidacion);//quitar 3 meses quitar 15.61

                // neto pagar liquidacion Fiscal
                //decimal NetoAPagarLiquidacionF = subTotalIndemnizacionF + (ISR_LiquidacionF);

                //neto a pagar finiquito Fiscal-Complemento
                decimal NetoAPagarFiniquito = 0;
                decimal NetoAPagarFiniquitoF = 0;

                if (ISRFiniquito != null)
                {
                    if (ISRFiniquito.EsISR)
                    {
                        NetoAPagarFiniquito = (subTotalFiniquito - (ISRFiniquito.ResultadoIsrOSubsidio));
                    }
                    else
                    {
                        NetoAPagarFiniquito = (subTotalFiniquito + (ISRFiniquito.ResultadoIsrOSubsidio));
                    }

                    //neto a pagar finiquito Fiscal
                   // NetoAPagarFiniquitoF = (subTotalFiniquitoF + (ISRFiniquitoF.ResultadoIsrOSubsidio));

                }
                else
                {
                    NetoAPagarFiniquito = subTotalFiniquito;
                  //  NetoAPagarFiniquitoF = subTotalFiniquitoF;
                }

                #endregion


                #region GUARDAR RESULTADOS

                totalPercepciones = (_SUELDO + _VACACIONES + _PRIMA_VACACIONAL + _AGUINALDO + _VACACIONES_PENDIENTES + _PRIMA_VACACIONAL_PENDIENTE + GRATIFICACION);

                //Guardar Finiquito -> ACTUALIZAR EL FINIQUITO
                #region Finiquito Fiscal
                var itemFiniquito = new NOM_Finiquito()
                {
                    IdFiniquito = finiquitoGuardado.IdFiniquito,
                    IdPeriodo = idPeriodo,
                    IdContrato = contratoEmpleado.IdContrato,
                    IdEmpleado = idEmpleado,
                    IdSucursal = idSucursal,
                    IdCliente = idCliente,
                    SalarioMinimo = salarioMinimo,
                    SueldoPrimaAntiguedad = salarioParaPrimaAntiguedad,
                    FechaAlta = fechaAltaImss,//fechaAlta,
                    FechaBaja = fechaBaja,
                    FechaInicioAguinaldo = fechaInicioAguinaldo,
                    FechaInicioVacacion = fechaInicioVacaciones,
                    DiasLaborados = (int)diasLaborados,
                    DiasVacacionesParaCalculo = (int)diasParaVaciones,
                    DiasAguinaldoParaCalculo = (int)diasParaAguinaldo,
                    AguinaldoCorrespondiente = (int)aguinaldo,
                    VacacionesCorrespondiente = (int)vacaciones,
                    PrimaVacacional = (int)primaVacacional,
                    SD = contratoEmpleado.SD,
                    SDI = contratoEmpleado.SDI,
                    SBC = contratoEmpleado.SBC,
                    DiasEjercicioFiscal = diasEjercicioFiscal,
                    L_3Meses_SueldoParaCalculo = tresMesesDeSalario,
                    VacacionesPendientes = vacacionesPendientes,
                    PrimaVacacionPendiente = porcentajePrimaVacionalPendiente,
                    DiasSueldoPendiente = diasSueldoPendiente,
                    ProporcionalAguinaldo = proporcionalAguinaldo,
                    ProporcionalVacaciones = proporcionalVacaciones,
                    Proporcional20DiasPorAño = proporcional20DiasdeSueldo,
                    ProporcionalPrimaAntiguedad = proporcionalPrimaAntiguedad,

                    SUELDO = _SUELDO,
                    VACACIONES = _VACACIONES,
                    PRIMA_VACACIONAL = _PRIMA_VACACIONAL,
                    AGUINDALDO = _AGUINALDO,
                    VACACIONES_PENDIENTE = _VACACIONES_PENDIENTES,
                    PRIMA_VACACIONES_PENDIENTE = _PRIMA_VACACIONAL_PENDIENTE,
                    GRATIFICACION = GRATIFICACION,
                    L_3MESES_SUELDO = _3_MESES_SALARIO,
                    L_20DIAS_POR_AÑO = _20_DIAS_POR_AÑO,
                    PRIMA_ANTIGUEDAD = _PRIMA_ANTIGUEDAD,
                    COMPENSACION_X_INDEMNIZACION = COMPENSACIONxINDEMNIZACION,
                    Total_Liquidacion = NetoAPagarLiquidacion,
                    SubTotal_Liquidacion = subTotalIndemnizacion + _PRIMA_ANTIGUEDAD,
                    ISR_Liquidacion = ISR_Liquidacion,
                    Subsidio_Liquidacion = 0,
                    SubTotal_Finiquito = subTotalFiniquito,
                    Total_Finiquito = NetoAPagarFiniquito,



                    ISR_Finiquito = (ISRFiniquito == null ? 0 : ISRFiniquito.EsISR ? ISRFiniquito.ResultadoIsrOSubsidio : 0),
                    Subsidio_Finiquito = (ISRFiniquito == null ? 0 : ISRFiniquito.EsISR ? 0 : ISRFiniquito.ResultadoIsrOSubsidio),

                    //#########################################################################################################
                    TOTAL_total = ((NetoAPagarLiquidacion + NetoAPagarFiniquito) - totalDescuentoAdicionalFiscal),
                    //#########################################################################################################    

                    BaseGravableUltimoSueldo = ultimoSueldo,
                    ISR_UltimoSueldo = isrUltimoSueldo == null ? 0 : isrUltimoSueldo.IsrAntesDeSub,
                    TasaParaLiquidacion = tasaParaLiquidacion,
                    Prima_Riesgo = primaDeRiesgo,

                    //########Campos sin usar########
                    SubTotal_FiniquitoF = 0,
                    ISR_FiniquitoF = 0,
                    TOTAL_totalF = 0,
                    Total_FiniquitoF = 0,
                    ISR_LiquidacionF = 0,
                    Total_LiquidacionF = 0,
                    SubTotal_LiquidacionF = 0,
                    Subsidio_FiniquitoF = 0,
                    Subsidio_LiquidacionF = 0,
                    //##############################

                    L_AniosServicio = l_AniosServicio,
                    L_UltimoSueldoMensualOrd = ultimoSueldo,
                    L_IngresoAcumulable = ingresoAcumulableUltimoSueldo,
                    L_IngresoNoAcumulable = ingresoNoAcumulableUltimoSueldo,
                    IdEmpresaFiscal = contratoEmpleado.IdEmpresaFiscal,
                    IdEmpresaAsimilado = contratoEmpleado.IdEmpresaAsimilado,
                    IdEmpresaComplemento = contratoEmpleado.IdEmpresaComplemento,
                    IdMetodo_Pago = contratoEmpleado.FormaPago,
                    TotalPercepciones = totalPercepciones,
                    TotalDeducciones = totalDescuentoAdicionalFiscal,//antes estaba en 0
                    TotalComplemento = 0,
                    TotalOtrosPagos = totalOtrosPagos,//(isrUltimoSueldo.ResultadoIsrOSubsidio*-1),
                    SubsidioCausado = subsidioCausado,//isrUltimoSueldo.Subsidio,
                    SubsidioEntregado = subsidioEntregado,//(isrUltimoSueldo.ResultadoIsrOSubsidio * -1)
                    UMA = _valorUMA,
                    EsLiquidacion = calcularLiquidacion,
                    DescuentosAplicados = descuentoAplicado,
                    TipoTarifa = tipoTarifaCalculoFiniquito,
                    PensionAlimenticiaDelTotal = pensionAlimenticiaDelTotal,
                    Art174 = art174

                };

                // var finiquitoDb = _NominasDao.AddFiniquito(itemFiniquito);
                var finiquitoDb = _NominasDao.UpdateFiniquitoProcesado(itemFiniquito);

                #endregion

                //Guardar Finiquito Complemento
                #region Finiquito Complemento
                NOM_Finiquito_Complemento itemFiniquitoComplemento = new NOM_Finiquito_Complemento()
                {
                    Id = 0,
                    IdFiniquito = finiquitoGuardado.IdFiniquito,
                    FechaAlta = fechaReal,
                    FechaBaja = fechaBaja,
                    FechaInicioAguinaldo = fechaInicioAguinaldo_C,
                    FechaInicioVacacion = fechaInicioVacaciones_C,
                    FechaAntiguedadReal = fechaAntiguedad,
                    Años_Antiguedad = añosAntiguedad_C,
                    DiasLaborados = (int)diasLaborados_C,
                    DiasVacacionesParaCalculo = (int)diasParaVaciones_C,
                    DiasAguinaldoParaCalculo = (int)diasParaAguinaldo_C,
                    AguinaldoCorrespondiente = (int)aguinaldo_C,
                    VacacionesCorrespondientes = (int)vacaciones_C,
                    PrimaVacacional = (int)primaVacacional_C,
                    SDReal = SDReal,
                    DiasEjercicioFiscal = diasEjercicioFiscal_C,
                    L_3Meses_SueldoParaCalculo = tresMesesDeSalario_C,
                    VacacionesPendientes = vacacionesPendientes_C,
                    PrimaVacacionPendiente = porcentajePrimaVacionalpendiente_C,
                    ProporcionalAguinaldo = proporcionalAguinaldo_C,
                    ProporcionalVacaciones = proporcionalVacaciones_C,
                    Proporcional20DiasPorAño = veinteDiasdeSueldoComplento,
                    SUELDO = SUELDO_C,
                    VACACIONES = VACACIONES_C,
                    PRIMA_VACACIONAL = PRIMA_VACACIONAL_C,
                    VACACIONES_PENDIENTE = VACACIONES_PENDIENTES_C,
                    PRIMA_VACACIONES_PENDIENTE = PRIMA_VACACIONAL_PENDIENTE_C,
                    L_3MESES_SUELDO = _3_MESES_SALARIO_C,
                    L_20DIAS_POR_AÑO = _20DIASPORAÑO_C,
                    PRIMA_ANTIGUEDAD = PRIMA_ANTIGUEDAD_C,
                    ProporcionalPrimaAntiguedad = proporcionalprimaAntiguedad_C,
                    AGUINALDO = _AGUINALDO_C,
                    SueldoPendiente = diasSueldoPendiente_C//Componer que pueda recibir decimales

                };

                var finiquitoComplementoDb = _NominasDao.AddFiniquitoComplemento(itemFiniquitoComplemento);
                #endregion


                #region CONCEPTOS QUE IRAN EN EL DETALLE DEL FINIQUITO

                #region SUELDO
                //SUELDO Gravado 100%
                decimal isnSueldo = Utils.ImpuestoSobreNomina(_SUELDO, 0.03M);
                NOM_Finiquito_Detalle itemSueldo = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = finiquitoGuardado.IdFiniquito,
                    IdConcepto = 1,
                    Total = _SUELDO,
                    GravadoISR = sueldoGravado,
                    ExentoISR = 0,
                    IdPrestamo = 0,
                    ImpuestoSobreNomina = isnSueldo

                };

                listaDetalleFiniquito.Add(itemSueldo);
                #endregion

                #region VACACIONES
                //Vacaciones Gravado 100%

                decimal isnVac = Utils.ImpuestoSobreNomina(_VACACIONES, 0.03M);
                NOM_Finiquito_Detalle itemVacaciones = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = itemFiniquito.IdFiniquito,
                    IdConcepto = 148,//33
                    Total = _VACACIONES,
                    GravadoISR = VacacionesGravadas,
                    ExentoISR = 0,
                    IdPrestamo = 0,
                    ImpuestoSobreNomina = isnVac
                };

                listaDetalleFiniquito.Add(itemVacaciones);


                #endregion

                #region PRIMA VACACIONAL

                decimal isnPrimVac = Utils.ImpuestoSobreNomina(_PRIMA_VACACIONAL, 0.03M);
                NOM_Finiquito_Detalle itemPrimaVacacional = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = itemFiniquito.IdFiniquito,
                    IdConcepto = 16,
                    Total = _PRIMA_VACACIONAL,
                    GravadoISR = PrimaVacacionaGravado,
                    ExentoISR = PrimaVacacionalExcento,
                    IdPrestamo = 0,
                    ImpuestoSobreNomina = isnPrimVac
                };

                listaDetalleFiniquito.Add(itemPrimaVacacional);

                #endregion

                #region AGUINDALDO
                decimal isnAguinaldo = Utils.ImpuestoSobreNomina(_AGUINALDO, 0.03M);
                NOM_Finiquito_Detalle itemAguinaldo = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = itemFiniquito.IdFiniquito,
                    IdConcepto = 2,//33
                    Total = _AGUINALDO,
                    GravadoISR = AguinaldoGravado,
                    ExentoISR = AguinaldoExcento,
                    IdPrestamo = 0,
                    ImpuestoSobreNomina = isnAguinaldo
                };
                listaDetalleFiniquito.Add(itemAguinaldo);


                #endregion

                #region VACACIONES PENDIENTES Grava 100%
                NOM_Finiquito_Detalle itemVacacionesPendientes = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = itemFiniquito.IdFiniquito,
                    IdConcepto = 148,//33
                    Total = _VACACIONES_PENDIENTES,
                    GravadoISR = VacacionesPendienteGravado,
                    ExentoISR = 0,
                    IdPrestamo = 0
                };

                listaDetalleFiniquito.Add(itemVacacionesPendientes);

                #endregion

                #region PRIMA VACACION PENDIENTE
                NOM_Finiquito_Detalle itemPrimaVacionesPendientes = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = itemFiniquito.IdFiniquito,
                    IdConcepto = 16,
                    Total = _PRIMA_VACACIONAL_PENDIENTE,
                    GravadoISR = PrimaVacacionalPendienteGravado,
                    ExentoISR = PrimaVacacionalPendienteExcento,
                    IdPrestamo = 0
                };

                listaDetalleFiniquito.Add(itemPrimaVacionesPendientes);


                #endregion

                #region GRATIFICACION Grava 100%
                decimal isnGratificacion = Utils.ImpuestoSobreNomina(GratificacionGravada, 0.03M);
                NOM_Finiquito_Detalle itemGratificacion = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = itemFiniquito.IdFiniquito,
                    IdConcepto = 156,//2
                    Total = GratificacionGravada,
                    GravadoISR = GratificacionGravada,
                    ExentoISR = 0,
                    IdPrestamo = 0,
                    ImpuestoSobreNomina = isnGratificacion
                };

                listaDetalleFiniquito.Add(itemGratificacion);


                #endregion

                #region INDEMNIZACION

                //Prima Antiguedad _PRIMA_ANTIGUEDAD
                if (_PRIMA_ANTIGUEDAD > 0)
                {
                    NOM_Finiquito_Detalle itemIndeminizacion = new NOM_Finiquito_Detalle()
                    {
                        Id = 0,
                        IdFiniquito = itemFiniquito.IdFiniquito,
                        IdConcepto = 17,
                        Total = _PRIMA_ANTIGUEDAD,
                        GravadoISR = grabablePrimaDeAntiguedad,
                        ExentoISR = porcionExentaPrimaDeAntiguedad,
                        IdPrestamo = 0
                    };
                    listaDetalleFiniquito.Add(itemIndeminizacion);
                }

                if (subTotalIndemnizacion > 0)
                {

                    //Hasta el 03 - nov - 2017 se uso este - pero se va a separar en los 3 conceptos - cancun
                    NOM_Finiquito_Detalle itemIndeminizaciones = new NOM_Finiquito_Detalle()
                    {
                        Id = 0,
                        IdFiniquito = itemFiniquito.IdFiniquito,
                        IdConcepto = 20,
                        Total = subTotalIndemnizacion,
                        GravadoISR = totalBaseGravableIndemnizacion,
                        ExentoISR = proporcion_exentaLiquidacion,
                        IdPrestamo = 0
                    };
                    listaDetalleFiniquito.Add(itemIndeminizaciones);

                    
                    ////SEPARAMOS EN TOTAL DE LA INDENMIZACION EN EL DETALLE DEL RECIBO

                    //3 MESES _3_MESES_SALARIO
                    //if (_20_DIAS_POR_AÑO > 0)
                    //{
                    //    NOM_Finiquito_Detalle itemIndeminizacion = new NOM_Finiquito_Detalle()
                    //    {
                    //        Id = 0,
                    //        IdFiniquito = itemFiniquito.IdFiniquito,
                    //        IdConcepto = 20,
                    //        Total = _20_DIAS_POR_AÑO,
                    //        GravadoISR = ,
                    //        ExentoISR = ,
                    //        IdPrestamo = 0
                    //    };
                    //    listaDetalleFiniquito.Add(itemIndeminizacion);
                    //}

                    ////20 dias por año  _20_DIAS_POR_AÑO
                    //if (_3_MESES_SALARIO > 0)
                    //{
                    //    NOM_Finiquito_Detalle itemIndeminizacion = new NOM_Finiquito_Detalle()
                    //    {
                    //        Id = 0,
                    //        IdFiniquito = itemFiniquito.IdFiniquito,
                    //        IdConcepto = 20,
                    //        Total = _3_MESES_SALARIO,
                    //        GravadoISR = ,
                    //        ExentoISR = ,
                    //        IdPrestamo = 0
                    //    };
                    //    listaDetalleFiniquito.Add(itemIndeminizacion);
                    //}



                    ////COMPENSACIONxINDEMNIZACION --gratificacion
                    //if (COMPENSACIONxINDEMNIZACION > 0)
                    //{
                    //    NOM_Finiquito_Detalle itemIndeminizacion = new NOM_Finiquito_Detalle()
                    //    {
                    //        Id = 0,
                    //        IdFiniquito = itemFiniquito.IdFiniquito,
                    //        IdConcepto = 149,
                    //        Total = COMPENSACIONxINDEMNIZACION,
                    //        GravadoISR = ,
                    //        ExentoISR = ,
                    //        IdPrestamo = 0
                    //    };
                    //    listaDetalleFiniquito.Add(itemIndeminizacion);
                    //}



                }

                #endregion

                #region ISR/SUBSIDIO FINIQUITO
                if (ISRFiniquito != null)
                {
                    NOM_Finiquito_Detalle itemSubsidio = new NOM_Finiquito_Detalle()
                    {
                        Id = 0,
                        IdFiniquito = itemFiniquito.IdFiniquito,
                        IdConcepto = ISRFiniquito.EsISR == true ? 43 : 144,
                        Total = ISRFiniquito.ResultadoIsrOSubsidio,
                        GravadoISR = 0,
                        ExentoISR = 0,
                        IdPrestamo = 0,
                        Liq = false
                    };
                    listaDetalleFiniquito.Add(itemSubsidio);
                }
                #endregion

                #region ISR LIQUIDACION
                NOM_Finiquito_Detalle itemISRLiq = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = itemFiniquito.IdFiniquito,
                    IdConcepto = 43,
                    Total = ISR_Liquidacion,
                    GravadoISR = 0,
                    ExentoISR = 0,
                    IdPrestamo = 0,
                    Liq = true
                };
                listaDetalleFiniquito.Add(itemISRLiq);
                #endregion

                #endregion


                //AGREGA LOS CONCEPTO DE DESCUENTO ADICIONAL
                if (listaDescuentoFiscales != null)
                {
                    foreach (var itemDescuento in listaDescuentoFiscales)
                    {
                        var itemDescDetalle = new NOM_Finiquito_Detalle()
                        {
                            Id = 0,
                            IdFiniquito = itemFiniquito.IdFiniquito,
                            IdConcepto = itemDescuento.IdConcepto,
                            Total = itemDescuento.TotalDescuento,
                            GravadoISR = 0,
                            ExentoISR = itemDescuento.TotalDescuento,
                            IdPrestamo = 0,
                            Liq = false
                        };

                        listaDetalleFiniquito.Add(itemDescDetalle);
                    }
                }





                //GUARDAR DETALLES DEL FINIQUITO
                _NominasDao.AddDetalleFiniquito(listaDetalleFiniquito);


                #endregion

                //Captura de Facturacion Finiquito
                //finiquitoDb , finiquitoComplementoDb, contratoEmpleado
                #region Factura Finiquito
                RHEntities ctx = new RHEntities();
                NOM_FacturacionF_Finiquito facturaF = new NOM_FacturacionF_Finiquito();
                var iva = ctx.ParametrosConfig.Where(x => x.IdConfig == 5).FirstOrDefault();
                decimal totalCuotasIMSS = 0;
                decimal totalImpuestoSobreNomina = 0;

                var finiquito = ctx.NOM_Finiquito.Where(x => x.IdFiniquito == finiquitoDb.IdFiniquito).FirstOrDefault();
                var cuotas = ctx.NOM_Cuotas_IMSS.Where(x => x.IdFiniquito == finiquitoDb.IdFiniquito).FirstOrDefault();
                var finiquitoDetalle = ctx.NOM_Finiquito_Detalle.Where(x => x.IdFiniquito == finiquitoDb.IdFiniquito).ToList();

                foreach (var impuesto in finiquitoDetalle)
                {
                    totalImpuestoSobreNomina += impuesto.ImpuestoSobreNomina;
                }

                if (cuotas != null)
                {
                    totalCuotasIMSS = cuotas.TotalObrero + cuotas.TotalPatron;
                }

                facturaF.IdEmpresa_Fis = contratoEmpleado.IdEmpresaFiscal.Value;
                facturaF.IdEmpresa_Com = contratoEmpleado.IdEmpresaComplemento == null ? 0 : contratoEmpleado.IdEmpresaComplemento.Value;
                facturaF.IdPeriodo = idPeriodo;
                facturaF.IVA = iva.ValorInt;
                facturaF.IdFiniquito = finiquitoDb.IdFiniquito;
                facturaF.F_Cuota_IMSS_Infonavit = totalCuotasIMSS;
                facturaF.F_ImpuestoNomina = totalImpuestoSobreNomina;
                facturaF.F_Amortizacion_Infonavit = amortizacionInfornavit;
                facturaF.F_Fonacot = amortizacionFonacot;
                facturaF.F_Pension_Alimenticia = amortizacionPension;
                facturaF.F_Relativo = totalCuotasIMSS + totalImpuestoSobreNomina + amortizacionInfornavit + amortizacionFonacot + amortizacionPension;
                facturaF.F_Total_Nomina = (finiquito.Total_Finiquito + finiquito.Total_Liquidacion) - totalDescuentoAdicionalFiscal;
                facturaF.F_Total_Fiscal = (facturaF.F_Relativo + facturaF.F_Total_Nomina);  //totalCuotasIMSS + totalImpuestoSobreNomina + finiquito.Total_Finiquito;

                ctx.NOM_FacturacionF_Finiquito.Add(facturaF);
                ctx.SaveChanges();
                #endregion

                return itemFiniquito.IdFiniquito;

            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        private static Dictionary<int, double> SortMyDictionaryByValue(Dictionary<int, double> myDictionary)
        {
            List<KeyValuePair<int, double>> tempList = new List<KeyValuePair<int, double>>(myDictionary);

            //li.Sort((a, b) => a.CompareTo(b)); // ascending sort
            //li.Sort((a, b) => -1 * a.CompareTo(b)); // descending sort

            tempList.Sort(delegate (KeyValuePair<int, double> firstPair, KeyValuePair<int, double> secondPair)
            {
                return firstPair.Value.CompareTo(secondPair.Value);
            });

            Dictionary<int, double> mySortedDictionary = new Dictionary<int, double>();
            foreach (KeyValuePair<int, double> pair in tempList)
            {
                mySortedDictionary.Add(pair.Key, pair.Value);
            }

            return mySortedDictionary;
        }
    }
}
