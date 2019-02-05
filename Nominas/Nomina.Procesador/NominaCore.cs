using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Nomina.Procesador.Metodos;
using Nomina.Procesador.Modelos;
using Nomina.Procesador.webServicePAC;
using System.Diagnostics;
using Common.Enums;
using Common.Utils;
using Nomina.Procesador.Datos;
using Nomina.Procesador.Modelos.Nomina12;
using RH.BLL;
using RH.Entidades;
using RH.Entidades.GlobalModel;
namespace Nomina.Procesador
{
    public class NominaCore
    {

        private string varXML = "<xml>";
        public int[] NominasId { get; set; }
        private readonly NominasDao _nominasDao = new NominasDao();
        private decimal _porcentajeNomina = 0;

        private decimal _totalObligaciones = 0;
        private static bool _usarUMAParaCalculo = true;//Para el calculo de cuotas imss
        private decimal _salarioMinimo = 0;
        private decimal _valorUMA = 0;

        public async Task<List<NotificationSummary>> ProcesarNominaAsync(int[] idEmpleados, NOM_PeriodosPago ppago, int idCliente, int idSucursal, int idUsuario, bool calcularConUma,bool anual)
        {
            bool errorProcesado = false;
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            int idPeriodoPago = ppago.IdPeriodoPago;
            List<int> _nominasGeneradas = new List<int>();//guarda las nominas generadas durante el procesado


            try
            {
                if (idEmpleados.Length <= 0)
                {
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "El array de Id de Empleados esta vacia.", Msg2 = "" });
                    return summaryList; // 
                }

                //Validar si el procesado puede continuar,
                //valida el estatus del periodo
                #region VALIDA EL ESTATUS DEL PERIODO PARA PODER CONTINUAR CON EL PROCESADO

                if (!_nominasDao.PeriodoDisponibleSetProcesando(idPeriodoPago, ref summaryList))
                {
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "Este período no esta disponible para ser procesado.", Msg2 = "" });
                    return summaryList; // 
                }

                #endregion

                #region VALIDA QUE EL EMPLEADO YA TENGA SU NOMINA TIMBRADA EN ESTE PERIODO

                var arrayidEmpleadosCfdiGenerados = _nominasDao.GetEmpleadosIdCfdiGenerados(idEmpleados, ppago.IdPeriodoPago);

                if (arrayidEmpleadosCfdiGenerados.Length > 0)
                {
                    var idEmpSumm = string.Join(",", arrayidEmpleadosCfdiGenerados);

                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = $"Los empleados con id: {idEmpSumm} tienen su nominas timbradas en este periodo. \n No se pueden volver a procesar.", Msg2 = "" });

                    //Filtra los empleados que ya se timbraron
                    var arrayFiltro = Utils.ElimarEnArrarDeOtroArray(idEmpleados, arrayidEmpleadosCfdiGenerados);
                    idEmpleados = arrayFiltro;
                }
                #endregion
                
                List<Empleado_Contrato> listaContratos = null;
                List<NOM_Nomina_Ajuste> listaAjustes = null;
                List<C_NOM_Conceptos> listaConceptosFiscales = null;
                List<NOM_Empleado_PeriodoPago> listaEmpleadoPeriodo = null;

                _Incidencias incidencias = null;
                ZonaSalario zonaSalarial = null;
                List<NOM_Empleado_Complemento> listaDatosComplementosDelPeriodo = null;

                //VARIABLES
                listaContratos = _nominasDao.GetContratoEmpleados(idEmpleados, ppago.IdSucursal);
                listaAjustes = _nominasDao.GetDatosDeAjustes(idEmpleados, ppago.IdPeriodoPago);
                listaEmpleadoPeriodo = _nominasDao.GetEmpleadoPeriodoPagos(ppago.IdPeriodoPago);

                //AJUSTES - obtenemos los conceptos para obtener sus 
                if (listaAjustes.Count > 0)
                {
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = $"Se encontró {listaAjustes.Count} conceptos de ajuste", Msg2 = "" });

                    listaConceptosFiscales = _nominasDao.GetConceptosFiscales();
                }


                //Si es un periodo normal diferente de solo complemento
                if (!ppago.SoloComplemento)
                {
                    incidencias = new _Incidencias();
                    zonaSalarial = _nominasDao.GetZonaSalario(); //Zona de Salario 73.04
                    var impuestoSobreNomina = _nominasDao.GetValorParametrosConfig(ParametrosDeConfiguracion.ISN);//Valor de Impuesto sobre nomina
                    listaDatosComplementosDelPeriodo = _nominasDao.GetDatosComplementoDelPeriodo(ppago.IdPeriodoPago);

                    if (impuestoSobreNomina != null)
                    {
                        _porcentajeNomina = impuestoSobreNomina.ValorDecimal;
                    }

                    //validar diferente de null
                    if (listaContratos.Count <= 0)
                    {
                        summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontrarón contratos para generar las nominas", Msg2 = "" });
                        return summaryList;
                    }

                    if (zonaSalarial == null)
                    {
                        summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontrarón datos de zona Salarial", Msg2 = "" });
                        return summaryList;
                    }

                    if (zonaSalarial.SMG == 0 && zonaSalarial.UMA == 0 && zonaSalarial.Status == false)
                    {
                        summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontrarón datos de zona Salarial", Msg2 = "" });
                        return summaryList;
                    }

                    if (ppago.DiasPeriodo <= 0)
                    {
                        string strPeriodo = "El periodo tiene configurado 0 dias : " + ppago.DiasPeriodo;
                        summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = strPeriodo, Msg2 = "" });
                    }

                    string tipoPeriodoDias = $"Tipo de Nomina: {ppago.IdTipoNomina}. - Días del periodo :{ppago.DiasPeriodo}";
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = tipoPeriodoDias, Msg2 = "" });

                    //Si se usara UMA o SMGV
                    //B) USAR UMA -DEFAULT TRUE
                    var usarUmaConfig = _nominasDao.GetValorParametrosConfig(ParametrosDeConfiguracion.AUMA);
                    if (usarUmaConfig != null)
                    {
                        if (usarUmaConfig.ValorBool != null)
                            _usarUMAParaCalculo = (bool)usarUmaConfig.ValorBool;
                    }

                    //Establece la variable si se usará uma para el cálculo de cuotas imss
                    // _usarUMAParaCalculo = calcularConUMA;

                    _salarioMinimo = zonaSalarial.SMG;

                    if (_usarUMAParaCalculo)
                    {
                        _valorUMA = zonaSalarial.UMA;

                        string usarUma = "Se uso UMA para el cálculo, con el valor = " + _valorUMA;

                        summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = usarUma, Msg2 = "" });
                    }
                }


                //Buscamos los id de la nomina de los empleados, 
                //para eliminar las nominas que hayan sido procesadas con ese id de empleado en el mismo periodo
                #region SE ELIMINA LOS REGISTROS DEL PROCESADO ANTERIOR

                var arrayIdNominas = _nominasDao.GetNominasIdByEmpleados(idEmpleados, ppago.IdPeriodoPago);

                //si la nomina fue procesada anteriormente, se eliminan sus registros, para guardar sus nuevos valores.
                if (arrayIdNominas.Length > 0)
                    NominasDao.EliminarNominasProcesadas(arrayIdNominas);                    



                #endregion

                //EJECUTAR EL METODO SOLO POR COMPLEMENTO
                if (ppago.SoloComplemento)
                {
                    return SoloComplemento(idEmpleados, idCliente, idSucursal, ppago.IdEjercicio, ppago.IdPeriodoPago, idUsuario, listaContratos, ppago);

                    // return summaryList;
                }



                //INCIDENCIAS - Obtener la datos de inasistencias de los colaboradores
                // crea una lista de incidencias acumuladas en el periodo de cada colaborador
                var listaInasistencias = incidencias.GetIncidenciasByPeriodo2(ppago, idEmpleados);
                //Incapacidad Materna, Enfermedad, 
                //Crea un registro de nomina por cada contrato en la lista

                #region PROCESADO DE CADA ELEMENTO EN LA LISTA DE CONTRATOS - 

                //Inicia el proceso de crear la nueva nomina por cada colaborador seleccionado 
                foreach (var empleadoContrato in listaContratos)
                {
                    try
                    {

                        if (empleadoContrato.FechaAlta > ppago.Fecha_Fin)
                        {
                            summaryList.Add(new NotificationSummary()
                            { Reg = empleadoContrato.IdEmpleado, Msg1 = $"El idContrato: {empleadoContrato.IdContrato}, la fecha de alta es mayor que la fecha final del periodo " });
                            continue;
                        }

                        List<NOM_Nomina_Detalle> listaDetalleNomina = new List<NOM_Nomina_Detalle>();

                        #region INICIALIZAR VARIABLES 
                        bool anualOK = false;
                        decimal totalNomina = 0;
                        decimal totalPercepciones = 0;
                        decimal totalDeducciones = 0;
                        decimal totalComplemento = 0;
                        decimal totalOtrosPagos = 0;
                        decimal totalImpuestoSobreNomina = 0;
                        decimal sbcotizacion = 0;
                        //salario base de cotizacion con el que se calculo el ISR/Subsidio <- usado en la vista de detalle de nomina
                        var diasPagos = 0;
                        int inasistenciasDelPeriodo = 0;
                        decimal subsidioEntregado = 0;
                        decimal subsidioCausado = 0;
                        decimal isrAntesDeSubsidio = 0;
                        decimal primaRiesgo = 0;                        
                        _totalObligaciones = 0;
                        int vacacionesDelPeriodo = 0;
                        int diasDescInfonavit = -1;//-1 es el valor default que tomara los dias laborados del periodo para el calculo

                        #endregion

                        #region CREAR EL REGISTRO DE LA NOMINA EN LA BD Y OBTINENE SU ID

                        //Validar que el contrato contenta los datos esperados
                        if (empleadoContrato.IdTipoRegimen <= 0)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = " - El contrato No tiene asignado Tipo de Regimen - "
                            });
                            continue;
                        }

                        if (empleadoContrato.IdTipoJornada <= 0)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = " - El contrato No tiene asignado Tipo de Jornada - "
                            });
                            continue;
                        }

                        if (empleadoContrato.IdContrato <= 0)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = " - El contrato No tiene asignado Tipo Contrato - "
                            });
                            continue;
                        }


                        if (empleadoContrato.IdPeriodicidadPago <= 0)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = " - El contrato No tiene asignado Periodicidad de pago - "
                            });
                            continue;
                        }

                        #region  GET CONCEPTOS ASIGNADOS AL EMPLEADO. FISCALES Y COMPLEMENTO - GET Cantidades de los complementos

                        //Obtiene la lista de conceptos asignafos - Fiscales y Complemento
                        var conceptosAsignados = _nominasDao.GetConceptosByIdEmpleado(empleadoContrato.IdEmpleado);

                        if (conceptosAsignados == null)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdEmpleado,
                                Msg1 = " El colaborador no tiene conceptos asignados."
                            });
                            //mensaje no tiene conceptos asignados
                            continue;

                        }

                        #endregion

                        #region OBTENER LA EMPRESA DE LA QUE SE TOMARÁ LA PRIMA DE RIESGO

                        int? idEmpresa = 0;

                        if (empleadoContrato.IdEmpresaFiscal != null)
                        {
                            idEmpresa = empleadoContrato.IdEmpresaFiscal;
                        }
                        else
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = "El contrato No tiene empresa fiscal asignado",
                                Msg2 = ""
                            });
                            continue;
                        }



                        //GET Empresa que se utilizara para obtener la prima de riesgo para el calculo de la nomina

                        if (idEmpresa <= 0)
                        {
                            //guardar en la lista de mensajes que no tiene empresa fiscal asignado
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = " El contrato No tiene empresa fiscal asignado",
                                Msg2 = ""
                            });
                            continue;
                        }
                        var empresa = _nominasDao.GetEmpresa(idEmpresa);

                        if (empresa == null)
                        {
                            //guardar en la lista de mensajes que no se encontró la empresa
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = "Contrato -No se encontró la empresa con el id: " + idEmpresa,
                                Msg2 = ""
                            });
                            continue;
                        }


                        if (empresa.PrimaRiesgo == null)
                        {
                            //guardar en la lista de mensajes que la prima de riesgo es cero o null
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = $"Contrato - La empresa  {empresa.RazonSocial} no tiene prima de riesgo."
                            });
                            continue;
                        }


                        primaRiesgo = empresa.PrimaRiesgo.Value;

                        #endregion



                        //1) Crear el registro de nomina --------------------------------------------
                        var itemNomina = _nominasDao.CrearNomina(ppago.IdEjercicio, ppago.IdPeriodoPago,
                            empleadoContrato.IdEmpleado, empleadoContrato.SD, empleadoContrato.SDI,
                            empleadoContrato.SBC, empleadoContrato.SalarioReal, idCliente, idSucursal,
                            empleadoContrato.FormaPago, empleadoContrato.IdContrato, idUsuario);

                        // 1.1 se agrega a la lista de nominas creadas
                        _nominasGeneradas.Add(itemNomina.IdNomina);

                        #endregion

                        #region SE OBTIENE EL NUMERO DE INASISTENCIAS DEL EMPLEADO EN EL PERIODO

                        //2) Obtener los dias de pago -----------------------------------------------
                        var diasPagoItem = listaInasistencias.FirstOrDefault(x => x.IdEmpleado == empleadoContrato.IdEmpleado);

                        if (diasPagoItem == null)
                        {
                            //guardar en la lista de mensajes 
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = $" El contrato tiene 0 días de pago. IDE: {empleadoContrato.IdEmpleado}",
                                Msg2 = ""
                            });
                            continue;
                        }



                        //Faltas
                        var faltas =
                            diasPagoItem.Incidencias.Where(
                                    x =>
                                        x.TipoIncidencia.Trim() == "F" || x.TipoIncidencia.Trim() == "FI" ||
                                        x.TipoIncidencia.Trim() == "FJ" || x.TipoIncidencia.Trim() == "FA")
                                .Select(i => i.TipoIncidencia)
                                .ToList()
                                .Count;
                        //Permisos sin goce
                        var permisoSinG =
                            diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "PS")
                                .Select(i => i.TipoIncidencia)
                                .ToList()
                                .Count;

                        //Incapacidad
                    

                        var incapacidadE = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IE").Select(i => i.TipoIncidencia).ToList().Count;

                        var incapacidadM = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IM").Select(i => i.TipoIncidencia).ToList().Count;

                        var incapacidadR = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IR").Select(i => i.TipoIncidencia).ToList().Count;

                        var incapacidadTotalDias = incapacidadE + incapacidadM + incapacidadR;

                        //Vacaciones
                        vacacionesDelPeriodo = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "V")
                            .Select(i => i.TipoIncidencia)
                            .ToList()
                            .Count;


                        if (diasPagoItem?.DiasAPagar <= 0)
                        {
                            //guardar en la lista de mensajes 
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdContrato,
                                Msg1 = " El contrato tiene 0 días de pago",
                                Msg2 = ""
                            });

                            if (permisoSinG > 0) // si tiene dias de pago pero tiene permisos sin goce - PS - se procedera para calcular las cuotas imss
                            {
                                summaryList.Add(new NotificationSummary()
                                {
                                    Reg = empleadoContrato.IdContrato,
                                    Msg1 = $" El contrato tiene {permisoSinG} días de Permiso sin goce - PS",
                                    Msg2 = ""
                                });
                            }
                            else // continuar con el siguiente registro
                            {
                                continue;
                            }

                        }


                        #region "INCAPACIDADES"

                        List<Modelos.Nomina12.NominaIncapacidad> listaIncapacidades = new List<NominaIncapacidad>();

                        if (incapacidadE > 0)
                        {
                            Modelos.Nomina12.NominaIncapacidad itemIE = new NominaIncapacidad()
                            {
                               DiasIncapacidad = incapacidadE,
                               TipoIncapacidad = "2",
                               ImporteMonetario = 0.00M
                            };
                            listaIncapacidades.Add(itemIE);
                        }

                        if (incapacidadM > 0)
                        {
                            Modelos.Nomina12.NominaIncapacidad itemIM = new NominaIncapacidad()
                            {
                                DiasIncapacidad = incapacidadM,
                                TipoIncapacidad = "3",
                                ImporteMonetario = 0.00M
                            };
                            listaIncapacidades.Add(itemIM);
                        }


                        if (incapacidadR > 0)
                        {
                            Modelos.Nomina12.NominaIncapacidad itemIR = new NominaIncapacidad()
                            {
                                DiasIncapacidad = incapacidadR,
                                TipoIncapacidad = "1",
                                ImporteMonetario = 0.00M
                            };
                            listaIncapacidades.Add(itemIR);
                        }

                        //if (incapacidad > 0)
                        //{
                        //    //Lista de incapacidades
                        //    var listaIncapacidades = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IE" || x.TipoIncidencia.Trim() == "IM" || x.TipoIncidencia.Trim() == "IR")
                        //        .Select(i => i.TipoIncidencia).ToList();

                        //    var diasPorIe = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IE").Select(i => i.TipoIncidencia).ToList().Count;
                        //    var diasPorIm = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IM").Select(i => i.TipoIncidencia).ToList().Count;
                        //    var diasPorIr = diasPagoItem.Incidencias.Where(x => x.TipoIncidencia.Trim() == "IR").Select(i => i.TipoIncidencia).ToList().Count;

                        //    if (diasPorIe > 0)
                        //    {
                        //        decimal monto = 0;
                        //        int diasIncapacidad = 0;//Guardarlo en un campo Gravado Excento o IntegraImss

                        //        NOM_Nomina_Detalle itemIncapacidad = new NOM_Nomina_Detalle()
                        //        {
                        //            Id = 0,
                        //            IdNomina = 0,//nomina.IdNomina,
                        //            IdConcepto = 1,
                        //            Total = monto,
                        //            GravadoISR = 0,// Grava o no Grava
                        //            ExentoISR = 0,
                        //            IntegraIMSS = 0,//integra imss
                        //            ImpuestoSobreNomina = 0,//_impuestoSobreNomina,
                        //            Complemento = false
                        //        };
                        //        listaDetalleNomina.Add(itemIncapacidad);
                        //    }

                        //    if (diasPorIm > 0)
                        //    {
                        //        decimal monto = 0;
                        //        int diasIncapacidad = 0;//Guardarlo en un campo Gravado Excento o IntegraImss

                        //        NOM_Nomina_Detalle itemIncapacidad = new NOM_Nomina_Detalle()
                        //        {
                        //            Id = 0,
                        //            IdNomina = 0,//nomina.IdNomina,
                        //            IdConcepto = 1,
                        //            Total = monto,
                        //            GravadoISR = 0,// Grava o no Grava
                        //            ExentoISR = 0,
                        //            IntegraIMSS = 0,//integra imss
                        //            ImpuestoSobreNomina = 0,//_impuestoSobreNomina,
                        //            Complemento = false
                        //        };
                        //        listaDetalleNomina.Add(itemIncapacidad);
                        //    }

                        //    if (diasPorIr > 0)
                        //    {
                        //        decimal monto = 0;
                        //        int diasIncapacidad = 0;//Guardarlo en un campo Gravado Excento o IntegraImss ?????? 

                        //        NOM_Nomina_Detalle itemIncapacidad = new NOM_Nomina_Detalle()
                        //        {
                        //            Id = 0,
                        //            IdNomina = 0,//nomina.IdNomina,
                        //            IdConcepto = 1,
                        //            Total = monto,
                        //            GravadoISR = 0,// Grava o no Grava
                        //            ExentoISR = 0,
                        //            IntegraIMSS = 0,//integra imss
                        //            ImpuestoSobreNomina = 0,//_impuestoSobreNomina,
                        //            Complemento = false
                        //        };
                        //        listaDetalleNomina.Add(itemIncapacidad);
                        //    }
                        //}



                        #endregion

                        //Incapacidades
                        // var incapacidades = diasPagoItem.Incidencias.Where(x => x.IsIncapacidad).Select(i => i.IdIncidencia).ToList().Count();

                        inasistenciasDelPeriodo = (faltas + permisoSinG + incapacidadTotalDias);

                        //Si la incapacidad es <= 3 dias se consideran faltas -
                        //y se suma a los dias de inasistencias del periodo
                        //inasistenciasDelPeriodo += incapacidad;

                        #endregion

                        //SET DIAS DE PAGOS
                        //if (diasPagoItem != null) diasPagos = (diasPagoItem.DiasAPagar - inasistenciasDelPeriodo);
                        if (diasPagoItem != null) diasPagos = (diasPagoItem.DiasAPagar);

                        #region ACTUALIZA EL ITEM NOMINA CON DATOS DE LA EMPRESA - Y DIAS DE PAGOS

                        //2.1 update a nomina
                        itemNomina.Dias_Laborados = diasPagos;
                        itemNomina.IdEmpresaFiscal = empleadoContrato.IdEmpresaFiscal;
                        itemNomina.IdEmpresaAsimilado = empleadoContrato.IdEmpresaAsimilado;
                        itemNomina.IdEmpresaComplemento = empleadoContrato.IdEmpresaComplemento;
                        itemNomina.IdEmpresaSindicato = empleadoContrato.IdEmpresaSindicato;

                        #endregion

                        #region SE EJECUTA EL CALCULO DE LOS CONCEPTOS - FISCALES Y COMPLEMENTO

                        TotalConcepto rTotalImss = new TotalConcepto();
                        NOM_Nomina_Detalle itemDetalenomina = new NOM_Nomina_Detalle();
                        List<NOM_Nomina_Detalle> listaFonacot = new List<NOM_Nomina_Detalle>();
                        //por cada concepto en la lista se realiza el cálculo 
                        //ordenar los conceptos por idConcepto desc
                        decimal vacacionesTotal = 0;

                        //Validar si en la lista de conceptos asignado viene el concepto de vacaciones
                        var itemVac = conceptosAsignados.FirstOrDefault(x => x.IdConcepto == 148); //148 Vacaciones
                                                                                                   //Si la vacacion no esta asignada vacaciones sera 0
                        if (itemVac == null)
                        {
                            vacacionesDelPeriodo = 0;
                        }

                        //buscamos en la lista de 
                        var itemEmpleadoPeriodo =
                            listaEmpleadoPeriodo.FirstOrDefault(x => x.IdEmpleado == empleadoContrato.IdEmpleado);

                        if (itemEmpleadoPeriodo != null)
                        {
                            //
                            diasDescInfonavit = itemEmpleadoPeriodo.DiasDescuentoInfonavit;
                        }


                        foreach (var conceptoEmpleado in conceptosAsignados)
                        {
                            try
                            {
                                #region CONCEPTOS FISCALES

                                if (conceptoEmpleado.IsFormulaFiscal)
                                {
                                    // CUOTAS IMSS
                                    if (conceptoEmpleado.IdConcepto == 42) //cuotas imss
                                    {
                                        rTotalImss = MCuotasImss.CuotasImss(zonaSalarial.SMG, zonaSalarial.UMA,
                                            empresa.PrimaRiesgo.Value, nomina: itemNomina,
                                            usarUMA: _usarUMAParaCalculo, diasDelPeriodo: ppago.DiasPeriodo,
                                            diasIncapacidad: incapacidadTotalDias, permisosSinGoce: permisoSinG);

                                        if (rTotalImss == null) continue;
                                    }
                                    else
                                    {

                                        if (conceptoEmpleado.IdConcepto == 52)//FONACOT
                                        {
                                            listaFonacot = CalcularFonacotASync(itemNomina, ppago,
                                                conceptoEmpleado.IdConcepto);

                                            if (listaFonacot == null) continue;

                                            if (listaFonacot.Count <= 0) continue;

                                            listaDetalleNomina.AddRange(listaFonacot);
                                        }
                                        else
                                        {
                                            //Metodo que ejecuta el cálculo del concepto y se espera un Resultado con los totales del cálculo
                                            itemDetalenomina = CalcularConceptoFiscalAsync(itemNomina, ppago,
                                                empleadoContrato, conceptoEmpleado.IdConcepto,
                                                conceptoEmpleado.IsImpuestoSobreNomina, _porcentajeNomina,
                                                zonaSalarial.SMG, empresa.PrimaRiesgo.Value,
                                                inasistenciasEnElPeriodo: inasistenciasDelPeriodo,
                                                totalVacaciones: vacacionesTotal, diasVacaciones: vacacionesDelPeriodo, diasDescuentoInfonavit: diasDescInfonavit);

                                            //Si el resultado del cálculo es null - se continuará con el siguiente registro
                                            if (itemDetalenomina == null) continue;

                                            //Si el concepto es Vacaciones 
                                            if (conceptoEmpleado.IdConcepto == 148)
                                            {
                                                //guardamos el total que se calculo de vacaciones
                                                vacacionesTotal = itemDetalenomina.Total;
                                            }

                                            listaDetalleNomina.Add(itemDetalenomina);
                                        }

                                    }

                                    #region BLOQUE QUE SUMA AL TOTAL DE  - PERCEPCIONES - DEDUCCIONES - OBLIGACIONES - OTROS PAGOS

                                    switch (conceptoEmpleado.TipoConcepto)
                                    {
                                        case 1: //SUMA PERCEPCIONES - OTROS PAGOS

                                            if (conceptoEmpleado.IdTipoOtroPago <= 0)
                                            // Se suma a total percepciones sin otros pagos
                                            {
                                                totalPercepciones += itemDetalenomina.Total;
                                                sbcotizacion += itemDetalenomina.GravadoISR;
                                                totalImpuestoSobreNomina += itemDetalenomina.ImpuestoSobreNomina;

                                                //Si el concepto es Cuotas Imss - se tomará el total de las obligaciones
                                                if (conceptoEmpleado.IdConcepto == 42)
                                                {
                                                    _totalObligaciones += rTotalImss.TotalObligaciones;
                                                }
                                            }
                                            else if (conceptoEmpleado.IdTipoOtroPago > 0) //Se suma a otros pagos
                                            {
                                                totalOtrosPagos += itemDetalenomina.Total;
                                                totalImpuestoSobreNomina += itemDetalenomina.ImpuestoSobreNomina;
                                            }

                                            break;
                                        case 2: //SUMA DEDUCCIONES
                                            if (conceptoEmpleado.IdConcepto == 42) //imss
                                            {
                                                totalDeducciones += rTotalImss.Total;
                                            }
                                            else if (conceptoEmpleado.IdConcepto == 52)
                                            {
                                                if (listaFonacot != null)
                                                {
                                                    if (listaFonacot.Count > 0)
                                                    {

                                                        var sumafonacot = listaFonacot.Sum(x => x.Total);
                                                        totalDeducciones += sumafonacot;

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                totalDeducciones += itemDetalenomina.Total;
                                            }

                                            break;
                                    }

                                    #endregion

                                }

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                summaryList.Add(new NotificationSummary()
                                {
                                    Reg = empleadoContrato.IdEmpleado,
                                    Msg1 = "linea:663 - ex ? " + ex.Message
                                });
                            }
                        }

                        #endregion

                        #region CONCEPTOS DE AJUSTES

                        var listaAjustePorEmpleado = listaAjustes.Where(x => x.IdEmpleado == empleadoContrato.IdEmpleado).ToList();

                        foreach (var itemAjuste in listaAjustePorEmpleado)
                        {
                            if (itemAjuste == null) continue;
                            if (itemAjuste.Total <= 0) continue;
                            if (itemAjuste.GravadoIsr <= 0 && itemAjuste.ExentoIsr <= 0) continue;

                            var itemConcepto = listaConceptosFiscales.FirstOrDefault(x => x.IdConcepto == itemAjuste.IdConcepto);

                            if (itemConcepto == null) continue;

                            if (itemConcepto.IdTipoOtroPago > 0)
                            {
                                totalOtrosPagos += itemAjuste.Total;
                                sbcotizacion += itemAjuste.GravadoIsr;
                                totalImpuestoSobreNomina += itemAjuste.ImpuestoSobreNomina;
                            }
                            else
                            {
                                switch (itemConcepto.TipoConcepto)
                                {
                                    case 1:
                                        //Tipo 1 - Percepcion - 2 - Deduccion
                                        totalPercepciones += itemAjuste.Total;
                                        sbcotizacion += itemAjuste.GravadoIsr;
                                        totalImpuestoSobreNomina += itemAjuste.ImpuestoSobreNomina;
                                        break;
                                    case 2:
                                        totalDeducciones += itemAjuste.Total;
                                        break;
                                }
                            }





                            var itemDetalle = new NOM_Nomina_Detalle()
                            {
                                Id = 0,
                                IdNomina = itemNomina.IdNomina,
                                IdConcepto = itemAjuste.IdConcepto,
                                Total = itemAjuste.Total,
                                GravadoISR = itemAjuste.GravadoIsr,
                                ExentoISR = itemAjuste.ExentoIsr,
                                IntegraIMSS = itemAjuste.IntegraImss,
                                ExentoIMSS = 0,
                                ImpuestoSobreNomina = itemAjuste.ImpuestoSobreNomina,
                                Complemento = false,
                                IdPrestamo = 0
                            };

                            listaDetalleNomina.Add(itemDetalle);
                        }

                        #endregion

                        #region CALCULO DE ISR - SUBSIDIO ************************************************************
                        //verifica Antiguedad
                        if (empleadoContrato.FechaIMSS != null)
                        {
                            if (anual == true && empleadoContrato.FechaIMSS < new DateTime(DateTime.Now.Year, 1, 1)) { anualOK = true; }
                        }

                        TotalIsrSubsidio totalIsr;
                        var exentoIsr = itemDetalenomina != null ? itemDetalenomina.ExentoISR : 0;

                        if (sbcotizacion <= 0)
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdEmpleado,
                                Msg1 = $"El registro tiene: {sbcotizacion} base de cotizacion"
                            });
                        }

                        if (!listaDetalleNomina.Any())
                        {
                            summaryList.Add(new NotificationSummary()
                            {
                                Reg = empleadoContrato.IdEmpleado,
                                Msg1 = $"No se genero conceptos de nomina. Revise los conceptos asignados"
                            });
                        }



                        if (ppago.Ultimo) //usar La tarifa mensual - usado en cancun por ajuste
                        {
                           
                            totalIsr = MNominas.CalculoDeIsr(itemNomina, sbcotizacion, 5,  ppago.DiasPeriodo, anualOK, ppago.IdEjercicio, exentoIsr);
                        }
                        else
                        {
                            totalIsr = MNominas.CalculoDeIsr(itemNomina, sbcotizacion, ppago.IdTipoNomina, ppago.DiasPeriodo, anualOK, ppago.IdEjercicio, exentoIsr);
                        }

                        isrAntesDeSubsidio = totalIsr.IsrAntesSubsidio;
                        subsidioCausado = totalIsr.SubsidioCausado;// nuevo para guardar el subsidio causado aunque se cobro isr
                                                                   //5.1 sumar el total del subsidio a las persepciones
                                                                   //totalPercepciones += totalIsr.Total;
                                                                   //totalPercepciones = totalIsr.Total < 0 ? (totalPercepciones+0) : (totalPercepciones + totalIsr.Total);


                        //SUMA DEL ISR AL TOTAL DEL DEDUCCIONES Y/O PERCEPCIONES
                        if (totalIsr.IsrCobrado > 0) //Si tiene ISR se suma al total de deduccion
                        {
                            decimal[] isrSaldo= MNominas.VerificaSaldo(totalIsr.IsrCobrado,ppago.IdEjercicio-1,itemNomina,ppago.IdPeriodoPago);


                            //if (anual)
                            //{
                            //    if (isrSaldo[1] > 0)
                            //    {
                            //        totalDeducciones += isrSaldo[1];
                            //    }
                            //    else
                            //    {
                            //        totalDeducciones += isrSaldo[0];
                            //    }
                            //}
                            //else//nomina normal
                            //{
                            //    totalDeducciones += isrSaldo[0];
                            //}


                            if (isrSaldo[1] > 0)//si tiene pago anual
                            {
                                totalDeducciones += isrSaldo[1];
                            }
                            else//nomina normal
                            {
                                totalDeducciones += isrSaldo[0];
                            }


                            totalOtrosPagos += isrSaldo[1];
                        }
                        else //Sino, si tiene Subsidio se suma a las percepciones
                        {
                            // totalPercepciones += totalIsr.Total; //<-- antes se guardaba como total percepciones
                            totalOtrosPagos += totalIsr.SubsidioEntregado;
                            subsidioEntregado = totalIsr.SubsidioEntregado; //Subsidio Entregado al colaborador
                            subsidioCausado = totalIsr.SubsidioCausado;
                            //Subsidio Causado - sin el descuento del ISR
                        }

                        #endregion

                        //6) GUARDAR TOTALES - 
                        //6.1 total nomina
                        totalNomina = totalPercepciones - totalDeducciones + totalOtrosPagos;

                        // 6.2 OBLIGACIONES
                        //MObligaciones.ImpuestoEstatal(totalImpuestoSobreNomina, itemNomina.IdNomina);

                        // se suma al total de obligaciones el total del impuesto sobre nomina
                        _totalObligaciones += totalImpuestoSobreNomina;

                        //COMPLEMENTO

                        #region NUEVO CÁLCULO DE COMPLEMENTO


                        //Busca en la lista de Datos de Complemento lo correspondiente al colaborador
                        //Obtenemos una lista con los concentos de complemento
                        var complementoEmpleado =
                            listaDatosComplementosDelPeriodo.Where(x => x.IdEmpleado == empleadoContrato.IdEmpleado)
                                .ToList();

                        totalComplemento = 0;

                        //valida que la lista no este vacia
                        if (complementoEmpleado?.Count > 0)
                        {
                            //Complemento que se subio en el Layout
                            var sumaTotalComplemento = complementoEmpleado.Select(x => x.Cantidad).Sum();

                            //Regla se aplico en mty por causa de descuento de lado de complemento
                            //donde el complemento se queda menor que el fiscal y eso genera negativo
                            //para no aplicar negativo se pondra cero
                            if (totalNomina > sumaTotalComplemento)
                            {
                                totalComplemento = 0;
                            }
                            else
                            {
                                totalComplemento = (sumaTotalComplemento - totalNomina.TruncateDecimal(2));
                            }
                        }

                        #endregion

                        //6.3 Guardar los totales
                        itemNomina.TotalNomina = totalNomina;
                        itemNomina.TotalDeducciones = totalDeducciones;
                        itemNomina.TotalPercepciones = totalPercepciones;
                        itemNomina.TotalImpuestoSobreNomina = totalImpuestoSobreNomina;
                        itemNomina.TotalObligaciones = _totalObligaciones;
                        itemNomina.TotalComplemento = totalComplemento;
                        itemNomina.TotalOtrosPagos = totalOtrosPagos;
                        itemNomina.SBCotizacionDelProcesado = sbcotizacion;
                        itemNomina.TipoTarifa = ppago.IdTipoNomina;
                        itemNomina.SubsidioEntregado = subsidioEntregado;
                        itemNomina.SubsidioCausado = subsidioCausado;
                        itemNomina.ISRantesSubsidio = isrAntesDeSubsidio;
                        itemNomina.Prima_Riesgo = primaRiesgo;
                        itemNomina.SMGV = _salarioMinimo;
                        itemNomina.UMA = _valorUMA;
                        //GUARDA LOS DATOS DE LA NOMINA
                        itemNomina = _nominasDao.ActualizarNominaDatosProcesado(itemNomina);

                        //GUARDAR EL DETALLE DE LA NOMINA
                        NominasDao.GuardarDetallesNomina(listaDetalleNomina, listaIncapacidades, itemNomina.IdNomina);//Nomina - Normal - incapacidades

                    }
                    catch (Exception ex)
                    {
                        summaryList.Add(new NotificationSummary()
                        {
                            Reg = empleadoContrato.IdContrato,
                            Msg1 = "linea:825 - ex ? " + ex.Message
                        });
                    }

                }

                #endregion

                //Crear XML de las nominas procesadas 

                #region CREA LOS XML DE CADA NOMINA PROCESADA - usado para generar recibo sin timbre

                //TimbradoCore timCore = new TimbradoCore();
                //var nominasId = _nominasGeneradas.ToArray();
                // var rxml = await timCore.GenerarXmlAsync(nominasId, ppago.IdEjercicio, ppago.IdPeriodoPago, idUsuario, true);

                #endregion


                //Proceso de Facturacion 
                #region FACTURA DEL PERIODO PROCESADO

                List<int?> newList = new List<int?>();
                
              //  var idEmpresas = (from lc in listaContratos group lc.IdEmpresaFiscal by lc.IdEmpresaFiscal).ToList();
               // var idEmpresasC = (from lc in listaContratos group lc.IdEmpresaComplemento by lc.IdEmpresaComplemento).ToList();

                //Crear listas de datos
                List<NOM_Cuotas_IMSS> listaCuotasImss = new List<NOM_Cuotas_IMSS>();
                List<NOM_Nomina_Detalle> listaNominaDetalle = new List<NOM_Nomina_Detalle>();
                List<NOM_Nomina> listaNomina = new List<NOM_Nomina>();
                Sucursal itemSucursal = new Sucursal();

                //LLenamos las lista con datos
                listaNomina = _nominasDao.GetDatosNominas(ppago.IdPeriodoPago);
                var arrayIdNominasProcesadas = listaNomina.Select(x => x.IdNomina).ToArray();

                var idEmpresas = listaNomina.Select(x => x.IdEmpresaFiscal).Distinct().ToArray();
                var idEmpresasC = listaNomina.Select(x => x.IdEmpresaComplemento).Distinct().ToArray();

                listaNominaDetalle = NominasDao.GetDatosDetallesNominas(arrayIdNominasProcesadas);//nominasId
                listaCuotasImss = _nominasDao.GetDatosCuotasImss(arrayIdNominasProcesadas);//nominasId
                itemSucursal = _nominasDao.GetSucursal(ppago.IdSucursal);


                if (idEmpresas != null)
                {

                    foreach (var idemp in idEmpresas)
                    {
                        //variables factura fiscal
                        decimal totalCuotasIMSS = 0;
                        decimal totalInfotavit = 0;
                        decimal totalFonacot = 0;
                        decimal totalPension = 0;
                        decimal totalImpuesto = 0;
                        decimal relativo = 0;
                        decimal totalPercep = 0;
                        decimal totalFiscal = 0;
                        decimal porcentajeIVA = 0;

                        //var idnominas = _nominasDao.ObtenerIdNominaByPeriodo(ppago.IdPeriodoPago, idemp.Key);
                        var idnominas = listaNomina.Where(x => x.IdEmpresaFiscal == idemp);

                        //recorremos la lista de nominas obteniendo su id
                        foreach (var id in idnominas)
                        {
                            var cuotas = _nominasDao.ObtenerCuotasIMSS2(listaCuotasImss, id.IdNomina);
                            if (cuotas != null)
                                totalCuotasIMSS = totalCuotasIMSS + cuotas.TotalObrero + cuotas.TotalPatron;

                            var infona = _nominasDao.ObtenerInfonavit2(listaNominaDetalle, id.IdNomina);//nomina detalle
                            totalInfotavit = totalInfotavit + infona;

                            //var fonat = _nominasDao.ObtenerFonacot2(listaNominaDetalle, id.IdNomina); //nomina detalle
                            totalFonacot += _nominasDao.ObtenerFonacot2(listaNominaDetalle, id.IdNomina); //Nomina detalle

                            var pensi = _nominasDao.ObtenerPension2(listaNominaDetalle, id.IdNomina); //nomina detalle
                            totalPension = totalPension + pensi;

                            totalPercep = totalPercep + id.TotalNomina;
                            totalImpuesto = totalImpuesto + id.TotalImpuestoSobreNomina;
                        }
                        //resultado fiscal de la factura            
                        relativo = totalCuotasIMSS + totalFonacot + totalImpuesto + totalInfotavit + totalPension;
                        totalFiscal = relativo + totalPercep;
                        porcentajeIVA = _nominasDao.PorcentajeIVA();

                        int idEmpresaFis = 0;
                        idEmpresaFis = Convert.ToInt32(idemp);
                        if (idEmpresaFis > 0)
                        {
                            _nominasDao.CrearFactura(ppago.IdPeriodoPago, idEmpresaFis, totalCuotasIMSS,
                                totalImpuesto, totalInfotavit,
                                totalFonacot, totalPension, relativo, totalPercep, totalFiscal, porcentajeIVA);
                        }
                    }
                }

                if (idEmpresasC != null)
                {
                    foreach (var idempC in idEmpresasC)
                    {    //varianñes facutara complemento
                        if (idempC != null)
                        {
                            decimal totalPercepcionesC = 0;
                            decimal totalCuotasIMSSC = 0;
                            decimal totalImpuestoC = 0;
                            decimal relativoC = 0;
                            decimal subTotalC = 0;
                            decimal totalIVAC = 0;
                            decimal totalComple = 0;
                            decimal porcentajeServicio = 0;
                            decimal totalServicio = 0;
                            decimal porcentajeIVA = 0;


                            var idnominas = listaNomina.Where(x => x.IdEmpresaComplemento == idempC);
                            //var idnominas = _nominasDao.ObtenerIdNominaByPeriodoC(ppago.IdPeriodoPago, idempC.Key);
                            //recorremos la lista de nominas obteniendo su id
                            foreach (var id in idnominas)
                            {
                                var cuotas = _nominasDao.ObtenerCuotasIMSS2(listaCuotasImss, id.IdNomina);
                                //operaciones complemento 
                                totalPercepcionesC = totalPercepcionesC + id.TotalComplemento;
                                totalCuotasIMSSC = totalCuotasIMSSC + id.TotalCuotasIMSSComplemento;
                                totalImpuestoC = totalImpuestoC + id.TotalImpuestoSobreNomina_Complemento;

                            }
                            //resultado fiscal de la factura            

                            //resultado complemento de la factura
                            porcentajeServicio = itemSucursal.PorcentajeServicio;//  _nominasDao.PorcentajeServicioHold(ppago.IdSucursal);
                            porcentajeServicio = porcentajeServicio / 100;
                            totalServicio = totalPercepcionesC * porcentajeServicio;
                            relativoC = totalImpuestoC + totalCuotasIMSSC;
                            subTotalC = relativoC + totalPercepcionesC + totalServicio;
                            porcentajeIVA = _nominasDao.PorcentajeIVA();
                            totalIVAC = (porcentajeIVA / 100) * subTotalC;
                            totalComple = totalIVAC + subTotalC;

                            _nominasDao.CrearFacturaComplemento(ppago.IdPeriodoPago, Convert.ToInt32(idempC), totalPercepcionesC, porcentajeServicio, totalServicio,
                                totalCuotasIMSSC, totalImpuestoC, relativoC, subTotalC, totalIVAC, totalComple);
                        }
                    }
                }

                #endregion


                return summaryList;
            }
            catch (Exception ex)
            {
                errorProcesado = true;

                summaryList.Add(new NotificationSummary()
                {
                    Reg = 0,
                    Msg1 = "linea:1019 - ex ? " + ex.Message
                });

                return summaryList;
            }
            finally
            {
                //Actualizar el periodo como periodo procesado
                _nominasDao.ActualizarPeriodoProcesado(idPeriodoPago, false, true, errorProcesado);
            }
        }

        /// <summary>
        /// Metodo que realizar el llamado de ejecucion del calculo de los metodos fiscales PERCEPCIONES Y DEDUCCIONES
        /// guarda en la base de datos el resultado.
        /// Retorna los totales del resultado del calculo.
        /// </summary>
        /// <param name="itemNomina"></param>
        /// <param name="periodoPago"></param>
        /// <param name="contrato"></param>
        /// <param name="idConcepto"></param>
        /// <param name="isPorcentajeSobreNomina"></param>
        /// <param name="porcentajeSobreNomina"></param>
        /// <param name="zonaSalario"></param>
        /// <param name="primaRiesgo"></param>
        /// <param name="inasistenciasEnElPeriodo"></param>
        /// <returns></returns>
        private static NOM_Nomina_Detalle CalcularConceptoFiscalAsync(NOM_Nomina itemNomina, NOM_PeriodosPago periodoPago, Empleado_Contrato contrato, int idConcepto, bool isPorcentajeSobreNomina, decimal porcentajeSobreNomina, decimal smg = 0, decimal primaRiesgo = 0, int inasistenciasEnElPeriodo = 0, int diasVacaciones = 0, decimal totalVacaciones = 0, int diasDescuentoInfonavit = -1)
        {
            int diasPeriodo = periodoPago.DiasPeriodo;
            switch (idConcepto)
            {
                case 1: //Sueldo
                    return MPercepciones.Sueldo(itemNomina, diasPeriodo, isPorcentajeSobreNomina, porcentajeSobreNomina, diasVacaciones);
                case 37://ASimilados a Salarios
                    return MPercepciones.AsimiladosASalarios(itemNomina, diasPeriodo, isPorcentajeSobreNomina, porcentajeSobreNomina);
                case 40: //Premios Asistencia
                    return inasistenciasEnElPeriodo > 0 ? null : MPercepciones.PremiosAsistencia(itemNomina, diasPeriodo, isPorcentajeSobreNomina, porcentajeSobreNomina);
                case 8: //Premios Puntualidad
                    return inasistenciasEnElPeriodo > 0 ? null : MPercepciones.PremiosPuntualidad(itemNomina, diasPeriodo, isPorcentajeSobreNomina, porcentajeSobreNomina);
                case 42: //Cuota IMSS
                         //return  MCuotasImss.CuotasImss(zonaSalario, primaRiesgo, nomina: itemNomina);
                case 51: //Prestamo Infonavit
                    return MDeducciones.PrestamoInfonavit(itemNomina, periodoPago, diasDescuentoInfonavit);
                //case 52: //Prestamo fonacot
                //    return null; //return MDeducciones.PrestamoFonacot(itemNomina, periodoPago);
                case 48://Pension Alimenticia
                    return MDeducciones.PensionAlimenticia(itemNomina, contrato);
                case 148://Vacaciones
                    return MPercepciones.Vacaciones(itemNomina, diasVacaciones, isPorcentajeSobreNomina, porcentajeSobreNomina);
                case 16://Prima Vacacional
                    return MPercepciones.PrimaVacaciones(itemNomina, totalVacaciones, smg, isPorcentajeSobreNomina, porcentajeSobreNomina);
                default:
                    return null;
            }
        }

        private static List<NOM_Nomina_Detalle> CalcularFonacotASync(NOM_Nomina itemNomina, NOM_PeriodosPago periodoPago, int idConcepto)
        {
            if (idConcepto == 52)
            {
                return MDeducciones.PrestamoFonacot(itemNomina, periodoPago);
            }
            else
            {
                return null;
            }

        }


        private List<NotificationSummary> SoloComplemento(int[] idEmpleados, int idCliente, int idSucursal, int idEjercicio, int idPeriodo, int idUsuario, List<Empleado_Contrato> listaContratos, NOM_PeriodosPago ppago)
        {
            List<int> nominasCreadas = new List<int>();
            List<NOM_Nomina> listaNominas = new List<NOM_Nomina>();
            List<NotificationSummary> summaryList = new List<NotificationSummary>();


            //Obtiene los datos de complementos de los empleados
            var listaDatosComplemento = NominasDao.GetDataEmpleadoComplemento(idPeriodo);

            summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "Procesado solo complemento", Msg2 = "" });

            //Crea un item de nomina por cada item en el array de idEmpleados
            foreach (var itemIdE in idEmpleados)
            {
                //Busca al colaborador en la lista de contratos
                var itemContrato = listaContratos.FirstOrDefault(x => x.IdEmpleado == itemIdE);

                //Busca los datos de complemento asignados al colaborador
                var complementoEmpleado = listaDatosComplemento.Where(x => x.IdEmpleado == itemIdE).ToList();

                decimal totalComplemento = 0;

                if (complementoEmpleado.Count > 0)
                {
                    //valida que la lista no este vacia
                    if (complementoEmpleado.Count > 0)
                    {
                        var sumaTotalComplemento = complementoEmpleado.Select(x => x.Cantidad).Sum();
                        totalComplemento = sumaTotalComplemento;
                    }
                }
                else
                {

                    summaryList.Add(new NotificationSummary() { Reg = itemIdE, Msg1 = "No se encontraron datos de complemento para empleado", Msg2 = "" });

                }

                if (itemContrato == null) continue;
                //Crea el item de nomina
                NOM_Nomina itemNomina = new NOM_Nomina()
                {
                    IdNomina = 0,
                    IdCliente = idCliente,
                    IdSucursal = idSucursal,
                    IdEjercicio = idEjercicio,
                    IdPeriodo = idPeriodo,
                    IdEmpleado = itemContrato.IdEmpleado,
                    IdContrato = itemContrato.IdContrato,
                    FechaReg = DateTime.Now,
                    IdUsuario = idUsuario,
                    CFDICreado = false,
                    TotalNomina = 0,
                    TotalPercepciones = 0,
                    TotalDeducciones = 0,
                    TotalOtrosPagos = 0,
                    TotalComplemento = totalComplemento,
                    TotalImpuestoSobreNomina = 0,
                    TotalImpuestoSobreNomina_Complemento = 0,
                    TotalObligaciones = 0,
                    TotalCuotasIMSS = 0,
                    TotalCuotasIMSSComplemento = 0,
                    SubsidioCausado = 0,
                    SubsidioEntregado = 0,
                    ISRantesSubsidio = 0,
                    SD = itemContrato.SD,
                    SDI = itemContrato.SDI,
                    SBC = itemContrato.SBC,
                    SDReal = itemContrato.SalarioReal,
                    IdEmpresaFiscal = itemContrato.IdEmpresaFiscal,
                    IdEmpresaComplemento = itemContrato.IdEmpresaComplemento,
                    IdEmpresaAsimilado = itemContrato.IdEmpresaAsimilado,
                    IdEmpresaSindicato = itemContrato.IdEmpresaSindicato,
                    Dias_Laborados = 0,
                    Faltas = 0,
                    Prima_Riesgo = 0,
                    TipoTarifa = 0,
                    SBCotizacionDelProcesado = 0,
                    XMLSinTimbre = null,
                    SMGV = 0,
                    UMA = 0

                };
                //Agrega el item a la lista a guardar
                listaNominas.Add(itemNomina);
            }


            //Guarda la lista de nominas generadas de solo complemento
            nominasCreadas = NominasDao.GuardarNominasComplemento(listaNominas);



            #region FACTURA DEL PERIODO PROCESADO

            var nominasId = nominasCreadas.ToArray();

            List<int?> newList = new List<int?>();

            //foreach (var lc in listaContratos)
            //{
            //    idEmpresas.Add(lc.IdEmpresaFiscal);
            //}

            //var idEmpresas = (from lc in listaContratos group lc.IdEmpresaFiscal by lc.IdEmpresaFiscal).ToList();

            var idEmpresasC = (from lc in listaContratos group lc.IdEmpresaComplemento by lc.IdEmpresaComplemento).ToList();

            //Crear listas de datos
            List<NOM_Cuotas_IMSS> listaCuotasImss = new List<NOM_Cuotas_IMSS>();
            List<NOM_Nomina_Detalle> listaNominaDetalle = new List<NOM_Nomina_Detalle>();
            List<NOM_Nomina> listaNomina = new List<NOM_Nomina>();
            Sucursal itemSucursal = new Sucursal();

            //LLenamos las lista con datos
            listaNomina = _nominasDao.GetDatosNominas(ppago.IdPeriodoPago);
            listaNominaDetalle = NominasDao.GetDatosDetallesNominas(nominasId);
            listaCuotasImss = _nominasDao.GetDatosCuotasImss(nominasId);
            itemSucursal = _nominasDao.GetSucursal(ppago.IdSucursal);


            if (idEmpresasC.Count > 0)
            {
                foreach (var idempC in idEmpresasC)
                {    //varianñes facutara complemento
                    if (idempC.Key != null)
                    {
                        decimal totalPercepcionesC = 0;
                        decimal totalCuotasIMSSC = 0;
                        decimal totalImpuestoC = 0;
                        decimal relativoC = 0;
                        decimal subTotalC = 0;
                        decimal totalIVAC = 0;
                        decimal totalComple = 0;
                        decimal porcentajeServicio = 0;
                        decimal totalServicio = 0;
                        decimal porcentajeIVA = 0;




                        var idnominas = listaNomina.Where(x => x.IdEmpresaComplemento == idempC.Key);
                        //var idnominas = _nominasDao.ObtenerIdNominaByPeriodoC(ppago.IdPeriodoPago, idempC.Key);
                        //recorremos la lista de nominas obteniendo su id
                        foreach (var id in idnominas)
                        {
                            var cuotas = _nominasDao.ObtenerCuotasIMSS2(listaCuotasImss, id.IdNomina);
                            //operaciones complemento 
                            totalPercepcionesC = totalPercepcionesC + id.TotalComplemento;
                            totalCuotasIMSSC = totalCuotasIMSSC + id.TotalCuotasIMSSComplemento;
                            totalImpuestoC = totalImpuestoC + id.TotalImpuestoSobreNomina_Complemento;

                        }
                        //resultado fiscal de la factura            

                        //resultado complemento de la factura
                        porcentajeServicio = itemSucursal.PorcentajeServicio;//  _nominasDao.PorcentajeServicioHold(ppago.IdSucursal);
                        porcentajeServicio = porcentajeServicio / 100;
                        totalServicio = totalPercepcionesC * porcentajeServicio;
                        relativoC = totalImpuestoC + totalCuotasIMSSC;
                        subTotalC = relativoC + totalPercepcionesC + totalServicio;
                        porcentajeIVA = _nominasDao.PorcentajeIVA();
                        totalIVAC = (porcentajeIVA / 100) * subTotalC;
                        totalComple = totalIVAC + subTotalC;

                        _nominasDao.CrearFacturaComplemento(ppago.IdPeriodoPago, Convert.ToInt32(idempC.Key), totalPercepcionesC, porcentajeServicio, totalServicio,
                            totalCuotasIMSSC, totalImpuestoC, relativoC, subTotalC, totalIVAC, totalComple);
                    }
                }
            }

            #endregion



            return summaryList;
        }

        public async Task<List<NotificationSummary>> ProcesarNominaAsimiladoAsync(int[] idEmpleados, NOM_PeriodosPago ppago, int idCliente, int idSucursal, int idUsuario, bool calcularConUMA = false)
        {
            bool errorProcesado = false;
            var summaryList = new List<NotificationSummary>();
            int idPeriodoPago = ppago.IdPeriodoPago;
            decimal porcentajeImpuestoSnomina = (decimal)0.03; //3%
            List<int> _nominasAsimGeneradas = new List<int>();//guarda las nominas generadas
            try
            {

                //Validar si el procesado puede continuar,
                //valida el estatus del periodo
                #region VALIDA EL ESTATUS DEL PERIODO PARA PODER CONTINUAR CON EL PROCESADO

                if (!_nominasDao.PeriodoDisponibleSetProcesando(idPeriodoPago, ref summaryList))
                {
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "Este período no esta disponible para ser procesado.", Msg2 = "" });
                    return summaryList; // retorna la lista vacia
                }

                #endregion


                #region VALIDA QUE EL EMPLEADO YA TENGA SU NOMINA TIMBRADA EN ESTE PERIODO

                var arrayidEmpleadosCfdiGenerados = _nominasDao.GetEmpleadosIdCfdiGenerados(idEmpleados, ppago.IdPeriodoPago);

                if (arrayidEmpleadosCfdiGenerados.Length > 0)
                {
                    var idEmpSumm = string.Join(",", arrayidEmpleadosCfdiGenerados);

                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = $"Los empleados con id: {idEmpSumm} tienen su nominas timbradas en este periodo. \n Ya no se pueden volver a procesar.", Msg2 = "" });

                    //Filtra los empleados que ya se timbraron
                    var arrayFiltro = Utils.ElimarEnArrarDeOtroArray(idEmpleados, arrayidEmpleadosCfdiGenerados);
                    idEmpleados = arrayFiltro;
                }
                #endregion


                List<Empleado_Contrato> listaContratos = null;

                var listaDatosAsimilados = NominasDao.GetDataEmpleadoAsimilados(ppago.IdPeriodoPago);

                if (listaDatosAsimilados.Count <= 0)
                {
                    errorProcesado = true;
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontrarón Datos de Asimilados para generar las nominas", Msg2 = "" });
                    return summaryList;
                }

                //VARIABLES
                listaContratos = _nominasDao.GetContratoEmpleados(idEmpleados, ppago.IdSucursal);

                //validar diferente de null
                if (listaContratos.Count <= 0)
                {
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontrarón contratos para generar las nominas", Msg2 = "" });
                    return summaryList;
                }


                //Buscamos los id de la nomina de los empleados, 
                //para eliminar las nominas que hayan sido procesadas con ese id de empleado en el mismo periodo
                #region SE ELIMINA LOS REGISTROS DEL PROCESADO ANTERIOR



                var arrayIdNominas = _nominasDao.GetNominasIdByEmpleados(idEmpleados, ppago.IdPeriodoPago);

                //si la nomina fue procesada anteriormente, se eliminan sus registros, para guardar sus nuevos valores.
                if (arrayIdNominas.Length > 0)
                    NominasDao.EliminarNominasProcesadas(arrayIdNominas);
                #endregion
                //Incapacidad Materna, Enfermedad, 

                //Obtenemos el % sobre nomina 
                var itemIsn = _nominasDao.GetValorParametrosConfig(ParametrosDeConfiguracion.ISN);

                if (itemIsn != null)
                {
                    porcentajeImpuestoSnomina = itemIsn.ValorDecimal;
                }

                //Crea un registro de nomina por cada contrato en la lista
                #region PROCESADO DE CADA ELEMENTO EN LA LISTA DE CONTRATOS 

                //Inicia el proceso de crear la nueva nomina por cada colaborador seleccionado 
                foreach (var empleadoContrato in listaContratos)
                {
                    try
                    {
                        List<NOM_Nomina_Detalle> listaDetalleNomina = new List<NOM_Nomina_Detalle>();

                        #region INICIALIZAR VARIABLES 

                        decimal totalNomina = 0;
                        decimal totalPercepciones = 0;
                        decimal totalDeducciones = 0;
                        decimal totalComplemento = 0;
                        decimal totalOtrosPagos = 0;
                        decimal totalImpuestoSobreNomina = 0;
                        decimal sbcotizacion = 0; //salario base de cotizacion con el que se calculo el ISR/Subsidio <- usado en la vista de detalle de nomina
                        var diasPagos = ppago.DiasPeriodo;
                        int inasistenciasDelPeriodo = 0;
                        decimal subsidioEntregado = 0;
                        decimal subsidioCausado = 0;
                        decimal isrAntesDeSubsidio = 0;
                        decimal primaRiesgo = 0;
                        _totalObligaciones = 0;
                        int vacacionesDelPeriodo = 0;
                        decimal ingresoGravado = 0;

                        #endregion

                        //Busca los datos de complemento asignados al colaborador
                        var asimiladoEmpleado = listaDatosAsimilados.FirstOrDefault(x => x.IdEmpleado == empleadoContrato.IdEmpleado);

                        if (asimiladoEmpleado == null) continue;

                        ingresoGravado = asimiladoEmpleado.Cantidad;

                        if (ingresoGravado <= 0) continue;

                        #region CREAR EL REGISTRO DE LA NOMINA EN LA BD Y OBTINENE SU ID

                        //Validar que el contrato contenta los datos esperados
                        if (empleadoContrato.IdTipoRegimen <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Tipo de Regimen - " });
                            continue;
                        }

                        if (empleadoContrato.IdTipoJornada <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Tipo de Jornada - " });
                            continue;
                        }

                        if (empleadoContrato.IdContrato <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Tipo Contrato - " });
                            continue;
                        }


                        if (empleadoContrato.IdPeriodicidadPago <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Periodicidad de pago - " });
                            continue;
                        }



                        #endregion

                        #region OBTENER LA EMPRESA DE ASIMILADO

                        int? idEmpresa = 0;

                        if (empleadoContrato.IdEmpresaAsimilado != null)
                        {
                            idEmpresa = empleadoContrato.IdEmpresaAsimilado;
                        }
                        else
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "No tiene empresa Asimilado asignado", Msg2 = "" });
                            continue;
                        }

                        //GET Empresa que se utilizara para obtener la prima de riesgo para el calculo de la nomina

                        if (idEmpresa <= 0)
                        {
                            //guardar en la lista de mensajes que no tiene empresa fiscal asignado
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "No tiene empresa asimilado asignado", Msg2 = "" });
                            continue;
                        }
                        var empresa = _nominasDao.GetEmpresa(idEmpresa);

                        if (empresa == null)
                        {
                            //guardar en la lista de mensajes que no se encontró la empresa
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "No se encontró la empresa con el id: " + idEmpresa, Msg2 = "" });
                            continue;
                        }


                        //if (empresa.PrimaRiesgo == null)
                        //{
                        //    //guardar en la lista de mensajes que la prima de riesgo es cero o null
                        //    summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "La empresa " + empresa.RazonSocial + " no tiene prima de riesgo." });
                        //    continue;
                        //}



                        #endregion

                        //1) Crear el registro de nomina --------------------------------------------
                        var itemNomina = _nominasDao.CrearNomina(ppago.IdEjercicio, ppago.IdPeriodoPago, empleadoContrato.IdEmpleado, empleadoContrato.SD, empleadoContrato.SDI, empleadoContrato.SBC, empleadoContrato.SalarioReal, idCliente, idSucursal, empleadoContrato.FormaPago, empleadoContrato.IdContrato, idUsuario);

                        // 1.1 se agrega a la lista de nominas creadas
                        _nominasAsimGeneradas.Add(itemNomina.IdNomina);

                        //SET DIAS DE PAGOS

                        #region ACTUALIZA EL ITEM NOMINA CON DATOS DE LA EMPRESA - Y DIAS DE PAGOS

                        //2.1 update a nomina
                        itemNomina.Dias_Laborados = diasPagos;//dias el periodo
                        itemNomina.IdEmpresaFiscal = 0;//empleadoContrato.IdEmpresaFiscal;
                        itemNomina.IdEmpresaAsimilado = empleadoContrato.IdEmpresaAsimilado;
                        itemNomina.IdEmpresaComplemento = 0;//empleadoContrato.IdEmpresaComplemento;
                        itemNomina.IdEmpresaSindicato = 0;// empleadoContrato.IdEmpresaSindicato;

                        #endregion

                        #region CALCULO DE ISR -  ************************************************************
                        sbcotizacion = ingresoGravado;
                        var totalIsr = MNominas.CalculoDeIsr(itemNomina, ingresoGravado, ppago.IdTipoNomina, ppago.DiasPeriodo,false,0,0);

                        //Calculo de ISN - 3%
                        totalImpuestoSobreNomina = ingresoGravado * porcentajeImpuestoSnomina;//default 3%

                        //CREA el detalle de la nomina con el concepto de Asimilado
                        NOM_Nomina_Detalle itemDetalleNominaAsimilado = new NOM_Nomina_Detalle
                        {
                            IdNomina = itemNomina.IdNomina,
                            IdConcepto = 37,
                            Total = ingresoGravado,
                            Complemento = false,
                            ExentoIMSS = 0,
                            ExentoISR = 0,
                            GravadoISR = ingresoGravado,
                            Id = 0,
                            IdPrestamo = 0,
                            ImpuestoSobreNomina = totalImpuestoSobreNomina
                        };

                        listaDetalleNomina.Add(itemDetalleNominaAsimilado);
                        #endregion


                        totalDeducciones = totalIsr.IsrCobrado;
                        totalPercepciones = ingresoGravado;
                        totalNomina = ingresoGravado - totalDeducciones;
                        //6.3 Guardar los totales
                        itemNomina.TotalNomina = totalNomina;
                        itemNomina.TotalDeducciones = totalDeducciones;
                        itemNomina.TotalPercepciones = totalPercepciones;
                        itemNomina.TotalImpuestoSobreNomina = totalImpuestoSobreNomina;
                        itemNomina.TotalObligaciones = 0;
                        itemNomina.TotalComplemento = 0;
                        itemNomina.TotalOtrosPagos = 0;
                        itemNomina.SBCotizacionDelProcesado = sbcotizacion;
                        itemNomina.TipoTarifa = ppago.IdTipoNomina;
                        itemNomina.SubsidioEntregado = 0;
                        itemNomina.SubsidioCausado = 0;
                        itemNomina.ISRantesSubsidio = totalIsr.IsrCobrado;
                        itemNomina.Prima_Riesgo = primaRiesgo;
                        itemNomina.SMGV = _salarioMinimo;
                        itemNomina.UMA = _valorUMA;

                        //GUARDA LOS DATOS DE LA NOMINA
                        itemNomina = _nominasDao.ActualizarNominaDatosProcesado(itemNomina);

                        //GUARDAR EL DETALLE DE LA NOMINA
                        NominasDao.GuardarDetallesNomina(listaDetalleNomina,null, 0);//Asimilado no aplica incapacidades
                    }
                    catch (Exception e)
                    {

                    }

                }

                #endregion

                //Crear XML de las nominas procesadas 
                #region CREA LOS XML DE CADA NOMINA PROCESADA - usado para generar recibo sin timbre

                //TimbradoCore timCore = new TimbradoCore();
                var nominasId = _nominasAsimGeneradas.ToArray();
                // var rxml = await timCore.GenerarXmlAsync(nominasId, ppago.IdEjercicio, ppago.IdPeriodoPago, idUsuario, true);

                #endregion

                //Proceso de Facturacion 
                #region FACTURA DEL PERIODO PROCESADO

                List<int?> newList = new List<int?>();

                //foreach (var lc in listaContratos)
                //{
                //    idEmpresas.Add(lc.IdEmpresaFiscal);
                //}

                var idEmpresas = (from lc in listaContratos group lc.IdEmpresaAsimilado by lc.IdEmpresaAsimilado).ToList();

                //  var idEmpresasC = (from lc in listaContratos group lc.IdEmpresaComplemento by lc.IdEmpresaComplemento).ToList();

                //Crear listas de datos
                List<NOM_Cuotas_IMSS> listaCuotasImss = new List<NOM_Cuotas_IMSS>();
                List<NOM_Nomina_Detalle> listaNominaDetalle = new List<NOM_Nomina_Detalle>();
                List<NOM_Nomina> listaNomina = new List<NOM_Nomina>();
                Sucursal itemSucursal = new Sucursal();

                //LLenamos las lista con datos
                listaNomina = _nominasDao.GetDatosNominas(ppago.IdPeriodoPago);
                listaNominaDetalle = NominasDao.GetDatosDetallesNominas(nominasId);
                listaCuotasImss = _nominasDao.GetDatosCuotasImss(nominasId);
                itemSucursal = _nominasDao.GetSucursal(ppago.IdSucursal);


                if (idEmpresas != null)
                {

                    foreach (var idemp in idEmpresas)
                    {
                        //variables factura fiscal
                        decimal totalCuotasIMSS = 0;
                        decimal totalInfotavit = 0;
                        decimal totalFonacot = 0;
                        decimal totalPension = 0;
                        decimal totalImpuesto = 0;
                        decimal relativo = 0;
                        decimal totalPercep = 0;
                        decimal totalFiscal = 0;
                        decimal porcentajeIVA = 0;

                        //var idnominas = _nominasDao.ObtenerIdNominaByPeriodo(ppago.IdPeriodoPago, idemp.Key);
                        var idnominas = listaNomina.Where(x => x.IdEmpresaAsimilado == idemp.Key);

                        //recorremos la lista de nominas obteniendo su id
                        foreach (var id in idnominas)
                        {
                            var cuotas = _nominasDao.ObtenerCuotasIMSS2(listaCuotasImss, id.IdNomina);
                            if (cuotas != null)
                                totalCuotasIMSS = totalCuotasIMSS + cuotas.TotalObrero + cuotas.TotalPatron;

                            var infona = _nominasDao.ObtenerInfonavit2(listaNominaDetalle, id.IdNomina);//nomina detalle
                            totalInfotavit = totalInfotavit + infona;

                            var fonat = _nominasDao.ObtenerFonacot2(listaNominaDetalle, id.IdNomina); //nomina detalle
                            totalFonacot = _nominasDao.ObtenerFonacot2(listaNominaDetalle, id.IdNomina); //Nomina detalle

                            var pensi = _nominasDao.ObtenerPension2(listaNominaDetalle, id.IdNomina); //nomina detalle
                            totalPension = totalPension + fonat;

                            totalPercep = totalPercep + id.TotalNomina;
                            totalImpuesto = totalImpuesto + id.TotalImpuestoSobreNomina;
                        }
                        //resultado fiscal de la factura            
                        relativo = totalCuotasIMSS + totalFonacot + totalImpuesto + totalInfotavit + totalPension;
                        totalFiscal = relativo + totalPercep;
                        porcentajeIVA = _nominasDao.PorcentajeIVA();

                        _nominasDao.CrearFactura(ppago.IdPeriodoPago, Convert.ToInt32(idemp.Key), totalCuotasIMSS, totalImpuesto, totalInfotavit,
                            totalFonacot, totalPension, relativo, totalPercep, totalFiscal, porcentajeIVA);
                    }
                }



                #endregion


                return summaryList;
            }
            catch (Exception ex)
            {
                errorProcesado = true;

                return summaryList;
            }
            finally
            {
                //Actualizar el periodo como periodo procesado

                _nominasDao.ActualizarPeriodoProcesado(idPeriodoPago, false, true, errorProcesado);
            }
        }

        public async Task<List<NotificationSummary>> ProcesarNominaSindicatoAsync(int[] idEmpleados, NOM_PeriodosPago ppago, int idCliente, int idSucursal, int idUsuario, bool calcularConUMA = false)
        {
            bool errorProcesado = false;
            List<NotificationSummary> summaryList = new List<NotificationSummary>();
            int idPeriodoPago = ppago.IdPeriodoPago;
            List<int> _nominasSindGeneradas = new List<int>();//guarda las nominas generadas
            try
            {

                //Validar si el procesado puede continuar,
                //valida el estatus del periodo
                #region VALIDA EL ESTATUS DEL PERIODO PARA PODER CONTINUAR CON EL PROCESADO

                if (!_nominasDao.PeriodoDisponibleSetProcesando(idPeriodoPago, ref summaryList))
                {
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "Este período no esta disponible para ser procesado.", Msg2 = "" });
                    return summaryList; // retorna la lista vacia
                }

                #endregion

                List<Empleado_Contrato> listaContratos = null;

                var listaDatosSindicato = NominasDao.GetDataEmpleadoSindicato(ppago.IdPeriodoPago);

                //VARIABLES
                listaContratos = _nominasDao.GetContratoEmpleados(idEmpleados, ppago.IdSucursal);

                //validar diferente de null
                if (listaContratos.Count <= 0)
                {
                    summaryList.Add(new NotificationSummary() { Reg = 0, Msg1 = "No se encontrarón contratos para generar las nominas", Msg2 = "" });
                    return summaryList;
                }


                //Buscamos los id de la nomina de los empleados, 
                //para eliminar las nominas que hayan sido procesadas con ese id de empleado en el mismo periodo
                #region SE ELIMINA LOS REGISTROS DEL PROCESADO ANTERIOR

                var arrayIdNominas = _nominasDao.GetNominasIdByEmpleados(idEmpleados, ppago.IdPeriodoPago);

                //si la nomina fue procesada anteriormente, se eliminan sus registros, para guardar sus nuevos valores.
                if (arrayIdNominas.Length > 0)
                    NominasDao.EliminarNominasProcesadas(arrayIdNominas);
                #endregion
                //Incapacidad Materna, Enfermedad, 

                //Crea un registro de nomina por cada contrato en la lista
                #region PROCESADO DE CADA ELEMENTO EN LA LISTA DE CONTRATOS 

                //Inicia el proceso de crear la nueva nomina por cada colaborador seleccionado 
                foreach (var empleadoContrato in listaContratos)
                {
                    try
                    {
                        List<NOM_Nomina_Detalle> listaDetalleNomina = new List<NOM_Nomina_Detalle>();

                        #region INICIALIZAR VARIABLES 

                        decimal totalNomina = 0;
                        decimal totalPercepciones = 0;
                        decimal totalDeducciones = 0;
                        decimal totalComplemento = 0;
                        decimal totalOtrosPagos = 0;
                        decimal totalImpuestoSobreNomina = 0;
                        decimal sbcotizacion = 0; //salario base de cotizacion con el que se calculo el ISR/Subsidio <- usado en la vista de detalle de nomina
                        var diasPagos = 0;
                        int inasistenciasDelPeriodo = 0;
                        decimal subsidioEntregado = 0;
                        decimal subsidioCausado = 0;
                        decimal isrAntesDeSubsidio = 0;
                        decimal primaRiesgo = 0;
                        _totalObligaciones = 0;
                        int vacacionesDelPeriodo = 0;
                        decimal ingresoGravado = 0;
                        decimal ingresoExento = 0;
                        decimal ingresoTotal = 0;


                        #endregion

                        //Busca los datos de complemento asignados al colaborador
                        var sindicatoEmpleado = listaDatosSindicato.FirstOrDefault(x => x.IdEmpleado == empleadoContrato.IdEmpleado);

                        if (sindicatoEmpleado == null) continue;

                        ingresoGravado = sindicatoEmpleado.Gravado;
                        ingresoExento = sindicatoEmpleado.Exento;
                        ingresoTotal = sindicatoEmpleado.Total;

                        if (ingresoTotal <= 0) continue;

                        #region CREAR EL REGISTRO DE LA NOMINA EN LA BD Y OBTINENE SU ID

                        //Validar que el contrato contenta los datos esperados
                        if (empleadoContrato.IdTipoRegimen <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Tipo de Regimen - " });
                            continue;
                        }

                        if (empleadoContrato.IdTipoJornada <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Tipo de Jornada - " });
                            continue;
                        }

                        if (empleadoContrato.IdContrato <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Tipo Contrato - " });
                            continue;
                        }


                        if (empleadoContrato.IdPeriodicidadPago <= 0)
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdContrato, Msg1 = " - EL contrato No tiene asignado Periodicidad de pago - " });
                            continue;
                        }



                        #endregion

                        #region OBTENER LA EMPRESA DE ASIMILADO

                        int? idEmpresa = 0;

                        if (empleadoContrato.IdEmpresaSindicato != null)
                        {
                            idEmpresa = empleadoContrato.IdEmpresaSindicato;
                        }
                        else
                        {
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "No tiene empresa Sindicato asignado", Msg2 = "" });
                            continue;
                        }




                        //GET Empresa que se utilizara para obtener la prima de riesgo para el calculo de la nomina

                        if (idEmpresa <= 0)
                        {
                            //guardar en la lista de mensajes que no tiene empresa fiscal asignado
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "No tiene empresa sindicato asignado", Msg2 = "" });
                            continue;
                        }
                        var empresa = _nominasDao.GetEmpresa(idEmpresa);

                        if (empresa == null)
                        {
                            //guardar en la lista de mensajes que no se encontró la empresa
                            summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "No se encontró la empresa con el id: " + idEmpresa, Msg2 = "" });
                            continue;
                        }


                        //if (empresa.PrimaRiesgo == null)
                        //{
                        //    //guardar en la lista de mensajes que la prima de riesgo es cero o null
                        //    summaryList.Add(new NotificationSummary() { Reg = empleadoContrato.IdEmpleado, Msg1 = "La empresa " + empresa.RazonSocial + " no tiene prima de riesgo." });
                        //    continue;
                        //}



                        #endregion

                        //1) Crear el registro de nomina --------------------------------------------
                        var itemNomina = _nominasDao.CrearNomina(ppago.IdEjercicio, ppago.IdPeriodoPago, empleadoContrato.IdEmpleado, empleadoContrato.SD, empleadoContrato.SDI, empleadoContrato.SBC, empleadoContrato.SalarioReal, idCliente, idSucursal, empleadoContrato.FormaPago, empleadoContrato.IdContrato, idUsuario);

                        // 1.1 se agrega a la lista de nominas creadas
                        _nominasSindGeneradas.Add(itemNomina.IdNomina);

                        //SET DIAS DE PAGOS

                        #region ACTUALIZA EL ITEM NOMINA CON DATOS DE LA EMPRESA - Y DIAS DE PAGOS

                        //2.1 update a nomina
                        itemNomina.Dias_Laborados = diasPagos;//dias el periodo
                        itemNomina.IdEmpresaFiscal = 0;//empleadoContrato.IdEmpresaFiscal;
                        itemNomina.IdEmpresaAsimilado = 0; //empleadoContrato.IdEmpresaAsimilado;
                        itemNomina.IdEmpresaComplemento = 0;//empleadoContrato.IdEmpresaComplemento;
                        itemNomina.IdEmpresaSindicato = empleadoContrato.IdEmpresaSindicato;

                        #endregion

                        #region CALCULO DE ISR -  ************************************************************
                        sbcotizacion = ingresoGravado;
                        //var totalIsr = await MNominas.CalculoDeIsr(itemNomina, ingresoGravado, ppago.IdTipoNomina, diasPeriodo: ppago.DiasPeriodo);

                        //CREA el detalle de la nomina con el concepto de Asimilado

                        NOM_Nomina_Detalle itemDetalleNominaSindicato = new NOM_Nomina_Detalle
                        {
                            IdNomina = itemNomina.IdNomina,
                            IdConcepto = 13,
                            Total = ingresoTotal,
                            Complemento = false,
                            ExentoIMSS = 0,
                            ExentoISR = ingresoExento,
                            GravadoISR = 0,// ingresoGravado,
                            Id = 0,
                            IdPrestamo = 0,
                            ImpuestoSobreNomina = 0
                        };

                        listaDetalleNomina.Add(itemDetalleNominaSindicato);
                        #endregion


                        totalDeducciones = 0;//totalIsr.IsrCobrado;
                        totalPercepciones = ingresoExento;
                        totalNomina = ingresoExento;
                        //6.3 Guardar los totales
                        itemNomina.TotalNomina = totalNomina;
                        itemNomina.TotalDeducciones = totalDeducciones;
                        itemNomina.TotalPercepciones = totalPercepciones;
                        itemNomina.TotalImpuestoSobreNomina = 0;
                        itemNomina.TotalObligaciones = 0;
                        itemNomina.TotalComplemento = 0;
                        itemNomina.TotalOtrosPagos = 0;
                        itemNomina.SBCotizacionDelProcesado = sbcotizacion;
                        itemNomina.TipoTarifa = ppago.IdTipoNomina;
                        itemNomina.SubsidioEntregado = 0;
                        itemNomina.SubsidioCausado = 0;
                        itemNomina.ISRantesSubsidio = 0;
                        itemNomina.Prima_Riesgo = primaRiesgo;
                        itemNomina.SMGV = _salarioMinimo;
                        itemNomina.UMA = _valorUMA;

                        //GUARDA LOS DATOS DE LA NOMINA
                        itemNomina = _nominasDao.ActualizarNominaDatosProcesado(itemNomina);

                        //GUARDAR EL DETALLE DE LA NOMINA
                        NominasDao.GuardarDetallesNomina(listaDetalleNomina,null, 0);//sindicato - no aplica incapacidades
                    }
                    catch (Exception e)
                    {

                    }

                }

                #endregion

                //Crear XML de las nominas procesadas 
                #region CREA LOS XML DE CADA NOMINA PROCESADA - usado para generar recibo sin timbre

                //TimbradoCore timCore = new TimbradoCore();
                //var nominasId = _nominasSindGeneradas.ToArray();
                // var rxml = await timCore.GenerarXmlAsync(nominasId, ppago.IdEjercicio, ppago.IdPeriodoPago, idUsuario, true);

                #endregion

                //Proceso de Facturacion 
                #region FACTURA DEL PERIODO PROCESADO

                //List<int?> newList = new List<int?>();



                //var idEmpresas = (from lc in listaContratos group lc.IdEmpresaSindicato by lc.IdEmpresaSindicato).ToList();

                ////Crear listas de datos
                //List<NOM_Cuotas_IMSS> listaCuotasImss = new List<NOM_Cuotas_IMSS>();
                //List<NOM_Nomina_Detalle> listaNominaDetalle = new List<NOM_Nomina_Detalle>();
                //List<NOM_Nomina> listaNomina = new List<NOM_Nomina>();
                //Sucursal itemSucursal = new Sucursal();

                //LLenamos las lista con datos
                //listaNomina = _nominasDao.GetDatosNominas(ppago.IdPeriodoPago);
                //listaNominaDetalle = _nominasDao.GetDatosDetallesNominas(nominasId);
                //listaCuotasImss = _nominasDao.GetDatosCuotasImss(nominasId);
                //itemSucursal = _nominasDao.GetSucursal(ppago.IdSucursal);


                //if (idEmpresas != null)
                //{

                //    foreach (var idemp in idEmpresas)
                //    {
                //        //variables factura fiscal
                //        decimal totalCuotasIMSS = 0;
                //        decimal totalInfotavit = 0;
                //        decimal totalFonacot = 0;
                //        decimal totalPension = 0;
                //        decimal totalImpuesto = 0;
                //        decimal relativo = 0;
                //        decimal totalPercep = 0;
                //        decimal totalFiscal = 0;
                //        decimal porcentajeIVA = 0;

                //        //var idnominas = _nominasDao.ObtenerIdNominaByPeriodo(ppago.IdPeriodoPago, idemp.Key);
                //        var idnominas = listaNomina.Where(x => x.IdEmpresaFiscal == idemp.Key);

                //        //recorremos la lista de nominas obteniendo su id
                //        foreach (var id in idnominas)
                //        {
                //            var cuotas = _nominasDao.ObtenerCuotasIMSS2(listaCuotasImss, id.IdNomina);
                //            if (cuotas != null)
                //                totalCuotasIMSS = totalCuotasIMSS + cuotas.TotalObrero + cuotas.TotalPatron;

                //            var infona = _nominasDao.ObtenerInfonavit2(listaNominaDetalle, id.IdNomina);//nomina detalle
                //            totalInfotavit = totalInfotavit + infona;

                //            var fonat = _nominasDao.ObtenerFonacot2(listaNominaDetalle, id.IdNomina); //nomina detalle
                //            //totalFonacot = _nominasDao.ObtenerFonacot2(listaNominaDetalle, id.IdNomina); //Nomina detalle

                //            //var pensi = _nominasDao.ObtenerPension2(listaNominaDetalle, id.IdNomina); //nomina detalle
                //            totalPension = totalPension + fonat;

                //            totalPercep = totalPercep + id.TotalNomina;
                //            totalImpuesto = totalImpuesto + id.TotalImpuestoSobreNomina;
                //        }
                //        //resultado fiscal de la factura            
                //        //relativo = totalCuotasIMSS + totalFonacot + totalImpuesto + totalInfotavit + totalPension;
                //        //totalFiscal = relativo + totalPercep;
                //        //porcentajeIVA = _nominasDao.PorcentajeIVA();

                //        //_nominasDao.CrearFactura(ppago.IdPeriodoPago, Convert.ToInt32(idemp.Key), totalCuotasIMSS, totalImpuesto, totalInfotavit,totalFonacot, totalPension, relativo, totalPercep, totalFiscal, porcentajeIVA);
                //    }
                //}



                #endregion


                return summaryList;
            }
            catch (Exception ex)
            {
                errorProcesado = true;

                return summaryList;
            }
            finally
            {
                //Actualizar el periodo como periodo procesado

                _nominasDao.ActualizarPeriodoProcesado(idPeriodoPago, false, true, errorProcesado);
            }
        }

        public async Task<int> ProcesarFiniquitoIndemnizacionAsync(int idPeriodo, int idEjercicio, int idEmpleado, int idCliente, int idSucursal, ParametrosFiniquitos arrayF, bool calcularLiquidacion, int idUsuario, TotalPersonalizablesFiniquitos totalesPerson = null, bool isArt174 =false)
        {
            //Obtener el estatus actual del periodo
            var periodoSeleccionado = _nominasDao.GetPeriodoPagoById(idPeriodo);

            //VAL - Si el periodo esta Autorizado, o esta siendo procesado por otro usuario
            //ya no se podrá procesar la nomina y se suspende la ejecución del procesado
            if (periodoSeleccionado.Autorizado || periodoSeleccionado.Procesando) return 0;

            return await MFiniquitoIndemnizacion.GenerarFiniquitoIndemnizacion(idPeriodo, idEjercicio, idEmpleado, idCliente, idSucursal, arrayF, calcularLiquidacion,idUsuario, totalesPerson, isArt174);
        }

        public async Task<List<NotificationSummary>> ProcesarAguinaldoAsync(int[] arrayIdEmpleado, string[] faltasCapturadas, bool[] generarPensionAlimenticia, int idPeriodo, int idCliente, int idSucursal, int idUsuario,bool anual)
        {

            //Obtener el estatus actual del periodo
            var periodoSeleccionado = _nominasDao.GetPeriodoPagoById(idPeriodo);

            //VAL - Si el periodo esta Autorizado, ya no se podrá procesar la nomina
            if (periodoSeleccionado.Autorizado)
                return null;

            //Actualizar el periodo a procesando ...
            periodoSeleccionado.Procesando = true;
            periodoSeleccionado.Procesado = false;
            _nominasDao.ActualizarPeriodoProcesado(idPeriodo, true, false);

            var t = await MAguinaldo.GenerarAguinaldo(arrayIdEmpleado, faltasCapturadas, generarPensionAlimenticia, idPeriodo, idCliente, idSucursal, idUsuario, anual);

            //Actualizar el periodo como periodo procesado
            periodoSeleccionado.Procesando = false;
            periodoSeleccionado.Procesado = true;
            _nominasDao.ActualizarPeriodoProcesado(idPeriodo, false, true);

            return t;
        }

    }
}
