using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Utils;
using Nomina.Procesador.Datos;
using Nomina.Procesador.Modelos;
using RH.Entidades;

namespace Nomina.Procesador.Metodos
{
    public static class MCuotasImss
    {
        static readonly NominasDao _nominasDao = new NominasDao();

        //Variables
        private static decimal _sbc = 0; // Salario Base de Cotizacion
        private static decimal _zonaSalarial = 0;
        private static decimal _smgv = 0;// Salario Minimo General Vigente
        private static int _factorVeces = 3;
        private static decimal _cuotaAdicional = 0;
        private static int _diasLaborados = 0;
        private static decimal _excedente = 0;
        private static decimal _excedentePatron = 0;
        private static decimal _excedenteObrero = 0;
        private static decimal _prestacionesDineroPatron = 0;
        private static decimal _prestacionesDineroObrero = 0;
        private static decimal _cuotaFijaPatron = 0;
        private static decimal _pencionadosBeneficiadosPatron = 0;
        private static decimal _pencionadosBeneficiadosObrero = 0;
        private static decimal _invalidezVidaPatron = 0;
        private static decimal _invalidezVidaObrero = 0;
        private static decimal _guarderiasPatron = 0;
        private static decimal _seguroRetiroPatron = 0;
        private static decimal _cesantiaVejezPatron = 0;
        private static decimal _cesantiaVejezObrero = 0;
        private static decimal _infonavitPatron = 0;
        private static decimal _riesgoTrabajoPatron = 0;
        private static decimal _totalPatron = 0;
        private static decimal _totalObrero = 0;
        private static decimal _SDI = 0;
        private static int _idNomina = 0;
        private static int _idFiniquito = 0;
        private static decimal _SD = 0;
        private static bool _usarUMAParaCalculo = true;
        private static bool relativoPatronal = false;//Relativo Patron = 0 dias laborados de un colaborador que aun esta dado de alta

        public static TotalConcepto CuotasImss(decimal salarioMinimoGeneral, decimal uma, decimal? primaRiesgo, NOM_Nomina nomina = null, NOM_Finiquito finiquito = null, bool usarUMA = true, int diasDelPeriodo = 0,int diasIncapacidad =0, int permisosSinGoce = 0)
        {
            NOM_Cuotas_IMSS resultadoCuotas;
            //Inicializar las variables
         relativoPatronal = false;
         _sbc = 0; // Salario Base de Cotizacion
         _zonaSalarial = 0;
         _smgv = 0;// Salario Minimo General Vigente
         _factorVeces = 3;
         _cuotaAdicional = 0;
         _diasLaborados = 0;
         _excedente = 0;
         _excedentePatron = 0;
         _excedenteObrero = 0;
         _prestacionesDineroPatron = 0;
         _prestacionesDineroObrero = 0;
         _cuotaFijaPatron = 0;
         _pencionadosBeneficiadosPatron = 0;
         _pencionadosBeneficiadosObrero = 0;
         _invalidezVidaPatron = 0;
         _invalidezVidaObrero = 0;
         _guarderiasPatron = 0;
         _seguroRetiroPatron = 0;
         _cesantiaVejezPatron = 0;
         _cesantiaVejezObrero = 0;
         _infonavitPatron = 0;
         _riesgoTrabajoPatron = 0;
         _totalPatron = 0;
         _totalObrero = 0;
         _SDI = 0;
         _idNomina = 0;
         _idFiniquito = 0;
         _SD = 0;

            

        resultadoCuotas = CuotasImss2( salarioMinimoGeneral,  uma, primaRiesgo,  nomina ,  finiquito ,  usarUMA ,  diasDelPeriodo, permisosSinG:permisosSinGoce);

            if(diasDelPeriodo == diasIncapacidad)
            {
                resultadoCuotas.CesantiaVejez_Patron = 0;
                resultadoCuotas.Cuota_Fija_Patron = 0;
                resultadoCuotas.Excedente_Patron = 0;
                resultadoCuotas.GuarderiasPrestaciones_Patron = 0;
                resultadoCuotas.InvalidezVida_Patron = 0;
                resultadoCuotas.Pensionados_Patron = 0;
                resultadoCuotas.PrestacionesDinero_Patron = 0;
                resultadoCuotas.RiesgoTrabajo_Patron = 0;
                resultadoCuotas.TotalPatron = (resultadoCuotas.SeguroRetiro_Patron + resultadoCuotas.Infonavit_Patron);
                resultadoCuotas.TotalObrero = 0;
            }
            //A) Si dias de incapacidad es de 1 a 3 - se toman como faltas
            //queda igual como vienen los datos de la nomina en diasLaborados

            //B) Si dias de incapacidad es > 3 y < a los dias del periodo - el patron paga los dias restantes
            //if(diasIncapacidad >3 && diasIncapacidad < diasDelPeriodo)
            //{
            //    //para calcular nuevamente anexando los dias de incapacidad, 
            //    //y esto es lo que será pagado por el patron

            //    nomina.Dias_Laborados += diasIncapacidad; 

            //    var nuevoCalculo = CuotasImss2(salarioMinimoGeneral, uma, primaRiesgo, nomina, finiquito, usarUMA, diasDelPeriodo);

            //    //Ese nuevo calculo se actualiza el registro solo del lado del patron
            //    resultadoCuotas.TotalPatron = nuevoCalculo.TotalPatron;
            //    resultadoCuotas.Cuota_Fija_Patron = nuevoCalculo.Cuota_Fija_Patron;
            //    resultadoCuotas.Excedente_Patron = nuevoCalculo.Excedente_Patron;
            //    resultadoCuotas.PrestacionesDinero_Patron = nuevoCalculo.PrestacionesDinero_Patron;
            //    resultadoCuotas.Pensionados_Patron = nuevoCalculo.Pensionados_Patron;
            //    resultadoCuotas.InvalidezVida_Patron = nuevoCalculo.InvalidezVida_Patron;
            //    resultadoCuotas.GuarderiasPrestaciones_Patron = nuevoCalculo.GuarderiasPrestaciones_Patron;
            //    resultadoCuotas.SeguroRetiro_Patron = nuevoCalculo.SeguroRetiro_Patron;
            //    resultadoCuotas.CesantiaVejez_Patron = nuevoCalculo.CesantiaVejez_Patron;
            //    resultadoCuotas.Infonavit_Patron = nuevoCalculo.Infonavit_Patron;
            //    resultadoCuotas.RiesgoTrabajo_Patron = nuevoCalculo.RiesgoTrabajo_Patron;
            //}

            //C) Si los dias de incapacidad es los mismos dias que el periodo -  el patro paga toda la cuota imss
            //queda igual, ya que si el dias laborados es cero, todo lo paga el patron como relativoPatronal


            //Guarda los datos en la Tabla Cuotas Imss
            _nominasDao.AddCuotasImss(resultadoCuotas);


            if (relativoPatronal != true)
            {
                _totalObrero = resultadoCuotas.TotalObrero;
                //Guardar el concepto en detalle de la nomina
                MDeducciones.Imss(_totalObrero, idNomina: _idNomina, idFiniquito: _idFiniquito);
            }
            else
            {
                _totalObrero = 0;
            }


            //Objeto utilizado en el metodo del procesado para la sumatoria de la nomina con los demas conceptos
            var totalConcepto = new TotalConcepto
            {
                Total = _totalObrero,
                ImpuestoSobreNomina = 0,
                TotalObligaciones = 0
            };

            return totalConcepto;


        }

        /// <summary>
        /// Metodo que genera las cuotas imss - 
        /// NOM_Nomina - para obetener los datos de la nomina como Id, SD, SDI, dias laborados, 
        /// </summary>
        /// <param name="nomina"></param>
        /// <param name="zsalarial"></param>
        /// <param name="primaRiesgo"></param>
        /// <param name="finiquito"></param>
        private static NOM_Cuotas_IMSS CuotasImss2(decimal salarioMinimoGeneral, decimal uma, decimal? primaRiesgo, NOM_Nomina nomina = null, NOM_Finiquito finiquito = null, bool usarUMA = true, int diasDelPeriodo = 0, int permisosSinG = 0)
        {
            //Relativo Patron = 0 dias laborados de un colaborador que aun esta dado de alta

            //PARAMETROS DE CONFIGURACION
            _usarUMAParaCalculo = usarUMA;

            //A) FACTOR VECES SALARIO MINIMO
            var factorSalarioMinimoGeneralVigente = _nominasDao.GetValorParametrosConfig(ParametrosDeConfiguracion.FSMGV); //para obtener el numero de veces del Salario minimo
            if (factorSalarioMinimoGeneralVigente != null)
            {
                if(factorSalarioMinimoGeneralVigente != null)
                _factorVeces = (int)factorSalarioMinimoGeneralVigente.ValorInt;//al momento del desarrollo es 3
            }                 

            //validacion
            if (nomina == null && finiquito == null) return null;
            //si ambos objetos contienen datos
            //la prioridad es nomina - entonces finiquito seteamos a null
            if (nomina != null)
            {
                finiquito = null;
            }
            
            //Anteriormente se obtenia los dias Laborados del finiquito, pero se cambio a que sean los dias del periodo -> solitud por alondra
            _diasLaborados = nomina?.Dias_Laborados ?? diasDelPeriodo;//finiquito.DiasLaborados;// aqui cambiar por los dias de periodicidad de pago en el contrato - incidencias
            _zonaSalarial = _usarUMAParaCalculo== true ? uma : salarioMinimoGeneral;

            _SDI = nomina?.SDI ?? finiquito.SDI;

            _idNomina = nomina?.IdNomina ?? 0;//si el metodo es ejecutado desde el procesado de la nomina, se toma el idNomina
            _idFiniquito = finiquito?.IdFiniquito ?? 0;//si el metodo es ejecuta desde el procesado del finiquito, se toma el id del finiquito sino se inicializa a cero
            _SD = nomina?.SD ?? finiquito.SD;
            _sbc = nomina?.SBC ?? finiquito.SBC;  // Salario Base de Cotizacion

            var tablaImss = _nominasDao.GeTablaImss();

            //Regla PS - se concidera los Permiso sin Goce para el calculo de las cuotas
            if (nomina != null && permisosSinG > 0)
            {
                //validamos si los dias de PS son igual que los dias del periodo
                //es decir que tiene Permisos sin goce en todo los dias de la nomina

                if (permisosSinG == diasDelPeriodo)
                {
                    _diasLaborados = 0;//se consideran como faltas y el pago de las cuotas las pagará el patron
                }
                else
                {
                    _diasLaborados += permisosSinG;//? si son 15 dias de PS se calculará la parte obrera y patron ?  es decir, lo no absorvera el patron el total?
                }
                
            }


            //Si los dias laborados es igual a cero
            //entonces se toma el dia del periodo, para el cálculo de los relativos que se cobrará al patron
            if (_diasLaborados == 0)
            {
                _diasLaborados = diasDelPeriodo;
                relativoPatronal = true;
            }

          


            //1) Cuota fija
            #region CUOTA FIJA

            //1   Especie - Cuota Fija    20.400  0.000
            var itemCutoaFija = tablaImss.FirstOrDefault(x => x.IdIMSS == 1);

            var cuotaFija = _diasLaborados * _zonaSalarial;

            if (itemCutoaFija != null) _cuotaFijaPatron = cuotaFija * (itemCutoaFija.Patron / 100);

            #endregion

            //Excedentes
            #region EXCEDENTES
            //se busca el registro en la bd para obtener los porcentajes Patron-Obrero
            var itemExcedente = tablaImss.FirstOrDefault(x => x.IdIMSS == 2);

            if (itemExcedente != null)
            {

                _smgv = _zonaSalarial * _factorVeces; // Salario Minimo General Vigente o UMA

            decimal R1P = 0;
            decimal R2O = 0;

                if (_smgv > _sbc)
            {
                    R1P = 0;
            }
            else
            {
                    R1P = ((_sbc - _smgv) * (itemExcedente.Patron / 100)) * _diasLaborados;
                    R2O = ((_sbc - _smgv) * (itemExcedente.Obrero / 100)) * _diasLaborados;

                }
                _excedentePatron = R1P;
                _excedenteObrero = R2O;

            //               decimal R2P = 0;
            //decimal R2O = 0;
           
            //    //Calcular Excedente Patron
            //    R2P = R1P * (itemExcedente.Patron / 100);
            //    _excedentePatron = R2P * _diasLaborados;

            //    if (_excedentePatron < 0)
            //    {
            //        _excedentePatron = 0;
            //    }
            //    //Calcular Exedente Obrero
            //    R2O = R1P * (itemExcedente.Obrero / 100);
            //    _excedenteObrero = R2O * _diasLaborados;
            //    if (_excedenteObrero < 0)
            //    {
            //        _excedenteObrero = 0;
            //    }
            }

            #endregion

            //3) Prestaciones en Dinero
            #region PRESTACIONES EN DINERO

            var itemPrestacionesDinero = tablaImss.FirstOrDefault(x => x.IdIMSS == 3);

            if (itemPrestacionesDinero != null)
            {
                _prestacionesDineroPatron = (_sbc * (itemPrestacionesDinero.Patron / 100)) * _diasLaborados;
                _prestacionesDineroObrero = (_sbc * (itemPrestacionesDinero.Obrero / 100)) * _diasLaborados;
            }

            #endregion

            //4) Pensionados y Beneficiados
            #region PENSIONADOS Y BENEFICIADOS

            var itemPensionados = tablaImss.FirstOrDefault(x => x.IdIMSS == 4);

            if (itemPensionados != null)
            {
                _pencionadosBeneficiadosPatron = (_sbc * (itemPensionados.Patron / 100)) * _diasLaborados;
                _pencionadosBeneficiadosObrero = (_sbc * (itemPensionados.Obrero / 100)) * _diasLaborados;
            }

            #endregion

            //5) Invalidez y Vida
            #region INVALIDEZ Y VIDA

            var itemIvalidez = tablaImss.FirstOrDefault(x => x.IdIMSS == 5);

            if (itemIvalidez != null)
            {
                _invalidezVidaPatron = (_sbc * (itemIvalidez.Patron / 100)) * _diasLaborados;
                _invalidezVidaObrero = (_sbc * (itemIvalidez.Obrero / 100)) * _diasLaborados;
            }

            #endregion

            //6) Guarderia
            #region GUARDERÍA

            var itemGuardería = tablaImss.FirstOrDefault(x => x.IdIMSS == 6);

            if (itemGuardería != null) _guarderiasPatron = (_sbc * (itemGuardería.Patron / 100)) * _diasLaborados;

            #endregion

            //7) Seguro de Retiro
            #region SEGURO DE RETIRO

            var itemSeguro = tablaImss.FirstOrDefault(x => x.IdIMSS == 7);

            if (itemSeguro != null) _seguroRetiroPatron = (_sbc * (itemSeguro.Patron / 100)) * _diasLaborados;

            #endregion

            //8) Cesantía y Vejez
            #region CESANTÍA Y VEJEZ

            var itemCesantia = tablaImss.FirstOrDefault(x => x.IdIMSS == 8);

            if (itemCesantia != null)
            {
                _cesantiaVejezPatron = (_sbc * (itemCesantia.Patron / 100)) * _diasLaborados;
                _cesantiaVejezObrero = (_sbc * (itemCesantia.Obrero / 100)) * _diasLaborados;
            }

            #endregion

            //9) Infonavit
            #region INFONAVIT

            var itemInfonavit = tablaImss.FirstOrDefault(x => x.IdIMSS == 9);

            if (itemInfonavit != null) _infonavitPatron = (_sbc * (itemInfonavit.Patron / 100)) * _diasLaborados;

            #endregion

            //10) Riesgo de Trabajo
            #region RIESGO DE TRABAJO

            var riesgoTrabajo = primaRiesgo ?? 0;

            _riesgoTrabajoPatron = (_sbc * ((decimal)riesgoTrabajo / 100)) * _diasLaborados;

            #endregion

            //Sumatoria
            #region TOTAL PATRON, TOTAL OBRERO
            //Si el Salario Diario es menor o igual al SMG las cuotas del obrero lo pagará el patron
            if (_SD <= salarioMinimoGeneral)
            {
                _totalPatron = _cuotaFijaPatron + _excedentePatron + _prestacionesDineroPatron +
                          _pencionadosBeneficiadosPatron +
                          _invalidezVidaPatron + _guarderiasPatron + _seguroRetiroPatron + _cesantiaVejezPatron +
                          _infonavitPatron + _riesgoTrabajoPatron + _excedenteObrero + _prestacionesDineroObrero + _pencionadosBeneficiadosObrero +
                               _invalidezVidaObrero + _cesantiaVejezObrero;

                _totalObrero = 0;
            }
            else
            {


                _totalObrero = _excedenteObrero + _prestacionesDineroObrero + _pencionadosBeneficiadosObrero +
                               _invalidezVidaObrero + _cesantiaVejezObrero; _totalPatron = _cuotaFijaPatron + _excedentePatron + _prestacionesDineroPatron +
                           _pencionadosBeneficiadosPatron +
                           _invalidezVidaPatron + _guarderiasPatron + _seguroRetiroPatron + _cesantiaVejezPatron +
                           _infonavitPatron + _riesgoTrabajoPatron;

                _totalObrero = _excedenteObrero + _prestacionesDineroObrero + _pencionadosBeneficiadosObrero +
                               _invalidezVidaObrero + _cesantiaVejezObrero;
            }


            #endregion

            //Si el Salario diario es igual o menor al salario minimo 
            // las cuota las paga el patron.
            // O si los dias laborados es cero, pero el colaborar esta activo las cuotas las paga el patron
            if (_SD <= salarioMinimoGeneral || relativoPatronal == true)
            {
                var cuotasImss = new NOM_Cuotas_IMSS()
                {
                    IdCuota = 0,
                    IdNomina = nomina?.IdNomina ?? 0,
                    IdFiniquito = finiquito?.IdFiniquito ?? 0,
                    Cuota_Fija_Patron = Utils.TruncateDecimales(_cuotaFijaPatron),
                    Excedente_Patron = Utils.TruncateDecimales((_excedentePatron + _excedenteObrero)),
                    Excedente_Obrero = 0,
                    PrestacionesDinero_Patron = Utils.TruncateDecimales((_prestacionesDineroPatron + _prestacionesDineroObrero)),
                    PrestacionesDinero_Obrero = 0,
                    Pensionados_Patron = Utils.TruncateDecimales((_pencionadosBeneficiadosPatron + _pencionadosBeneficiadosObrero)),
                    Pensionados_Obrero = 0,
                    InvalidezVida_Patron = Utils.TruncateDecimales((_invalidezVidaPatron + _invalidezVidaObrero)),
                    InvalidezVida_Obrero = 0,
                    GuarderiasPrestaciones_Patron = Utils.TruncateDecimales(_guarderiasPatron),
                    SeguroRetiro_Patron = Utils.TruncateDecimales(_seguroRetiroPatron),
                    CesantiaVejez_Patron = Utils.TruncateDecimales((_cesantiaVejezPatron + _cesantiaVejezObrero)),
                    CesantiaVejez_Obrero = 0,
                    Infonavit_Patron = Utils.TruncateDecimales(_infonavitPatron),
                    RiesgoTrabajo_Patron = Utils.TruncateDecimales(_riesgoTrabajoPatron),
                    TotalPatron = Utils.TruncateDecimales(_totalPatron),
                    TotalObrero = Utils.TruncateDecimales(_totalObrero)
                };
                // _nominasDao.AddCuotasImss(cuotasImss);

                return cuotasImss;
            }
            else
            {
                var cuotasImss = new NOM_Cuotas_IMSS()
                {
                    IdCuota = 0,
                    IdNomina = nomina?.IdNomina ?? 0,
                    IdFiniquito = finiquito?.IdFiniquito ?? 0,
                    Cuota_Fija_Patron = Utils.TruncateDecimales(_cuotaFijaPatron),
                    Excedente_Patron = Utils.TruncateDecimales(_excedentePatron),
                    Excedente_Obrero = Utils.TruncateDecimales(_excedenteObrero),
                    PrestacionesDinero_Patron = Utils.TruncateDecimales(_prestacionesDineroPatron),
                    PrestacionesDinero_Obrero = Utils.TruncateDecimales(_prestacionesDineroObrero),
                    Pensionados_Patron = Utils.TruncateDecimales(_pencionadosBeneficiadosPatron),
                    Pensionados_Obrero = Utils.TruncateDecimales(_pencionadosBeneficiadosObrero),
                    InvalidezVida_Patron = Utils.TruncateDecimales(_invalidezVidaPatron),
                    InvalidezVida_Obrero = Utils.TruncateDecimales(_invalidezVidaObrero),
                    GuarderiasPrestaciones_Patron = Utils.TruncateDecimales(_guarderiasPatron),
                    SeguroRetiro_Patron = Utils.TruncateDecimales(_seguroRetiroPatron),
                    CesantiaVejez_Patron = Utils.TruncateDecimales(_cesantiaVejezPatron),
                    CesantiaVejez_Obrero = Utils.TruncateDecimales(_cesantiaVejezObrero),
                    Infonavit_Patron = Utils.TruncateDecimales(_infonavitPatron),
                    RiesgoTrabajo_Patron = Utils.TruncateDecimales(_riesgoTrabajoPatron),
                    TotalPatron = Utils.TruncateDecimales(_totalPatron),
                    TotalObrero = Utils.TruncateDecimales(_totalObrero)
                };
                //   _nominasDao.AddCuotasImss(cuotasImss);
                return cuotasImss;
            }

            //if (relativoPatronal != true)
            //{
            //    //Guardar el concepto en detalle de la nomina
            //    MDeducciones.Imss(_totalObrero, idNomina: _idNomina, idFiniquito: _idFiniquito);
            //}
            //else
            //{
            //    _totalObrero = 0;
            //}


            ////Obeto utilizado en el metodo del procesado para la sumatoria de la nomina con los demas conceptos
            //var totalConcepto = new TotalConcepto
            //{
            //    Total = _totalObrero,
            //    ImpuestoSobreNomina = 0,
            //    TotalObligaciones = 0
            //};

            //return totalConcepto;


        }


    }
}
