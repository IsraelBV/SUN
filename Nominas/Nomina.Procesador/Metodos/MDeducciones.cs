using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nomina.Procesador.Modelos;
using Nomina.Procesador.Datos;
using RH.Entidades;

using Common.Utils;
namespace Nomina.Procesador.Metodos
{
    public static class MDeducciones
    {
        static readonly NominasDao _nominasDao = new NominasDao();
        public static void Imss(decimal totalObrero, int idNomina = 0, int idFiniquito = 0)
        {
            //Si el procesado es desde el modulo de procesar nomina
            if (idNomina != 0)
            {
                NOM_Nomina_Detalle nd = new NOM_Nomina_Detalle()
                {
                    Id = 0,
                    IdNomina = idNomina,
                    IdConcepto = 42,
                    Total = totalObrero,
                    GravadoISR = 0,
                    ExentoISR = 0,
                    IntegraIMSS = totalObrero,
                    ExentoIMSS = 0
                };
                _nominasDao.AddDetalleNomina(nd);
            }
            else if (idFiniquito != 0) //si el procesado es desde el modulo de finiquito
            {
                NOM_Finiquito_Detalle fd = new NOM_Finiquito_Detalle()
                {
                    Id = 0,
                    IdFiniquito = idFiniquito,
                    IdConcepto = 42,
                    Total = totalObrero,
                    GravadoISR = 0,
                    ExentoISR = 0,
                    IntegraIMSS = totalObrero
                };
                _nominasDao.AddDetalleNomina(fd);
            }

        }

        /// <summary>
        /// Prestamo Infonavit = 25
        /// </summary>
        /// <param name="nomina"></param>
        /// <param name="periodoPago"></param>
        /// <returns></returns>
        public static NOM_Nomina_Detalle PrestamoInfonavit(NOM_Nomina nomina, NOM_PeriodosPago periodoPago, int dias )
        {
            return CalculoDeInfonavit(nomina, periodoPago, dias);
        }

        /// <summary>
        /// Prestamo Fonacot = 26
        /// </summary>
        /// <param name="nomina"></param>
        /// <returns></returns>
        public static List<NOM_Nomina_Detalle> PrestamoFonacot(NOM_Nomina nomina, NOM_PeriodosPago periodoPago)
        {
            return CalculoDeFonacot(nomina,  periodoPago);
        }

        /// <summary>
        /// Pension ALimenticia = 48
        /// </summary>
        /// <param name="nomina"></param>
        /// <param name="contratoActual"></param>
        /// <param name="isImpuestoSobreNomina"></param>
        /// <param name="porcentaje"></param>
        /// <returns></returns>
        public static NOM_Nomina_Detalle PensionAlimenticia(NOM_Nomina nomina, Empleado_Contrato contratoActual, bool isImpuestoSobreNomina = false, decimal porcentaje = 0)
        {
            decimal porcentajePension = contratoActual.PensionAlimenticiaPorcentaje != null ? (decimal)contratoActual.PensionAlimenticiaPorcentaje.Value : 0;
            var tipoSueldo = contratoActual.PensionAlimenticiaSueldo ?? 0;//Tipo de sueldo SD o SDI
            decimal sueldo = 0;
            decimal pagoPensionAlimenticia = 0;

            switch (tipoSueldo)
            {
                case 1:
                    sueldo = nomina.SD;
                    break;
                case 2:
                    sueldo = nomina.SDI;
                    break;
            }

            if (sueldo > 0 && porcentajePension > 0)
            {
                pagoPensionAlimenticia = ((sueldo * nomina.Dias_Laborados) * (porcentajePension / 100));

              //  GuardarConcepto(nomina.IdNomina, 48, pagoPensionAlimenticia, pagoPensionAlimenticia, impuestoNomina: 0);

                NOM_Nomina_Detalle item = new NOM_Nomina_Detalle()
                {
                    Id = 0,
                    IdNomina = nomina.IdNomina,
                    IdConcepto = 48,
                    Total = Utils.TruncateDecimales(pagoPensionAlimenticia),
                    GravadoISR = Utils.TruncateDecimales(pagoPensionAlimenticia),
                    ExentoISR = 0,
                    IntegraIMSS = 0,
                    ImpuestoSobreNomina = 0,
                    Complemento = false

                };

                return item;
            }

            return null;
        }


        public static NOM_Nomina_Detalle PensionAlimenticiaMonto(decimal monto,  Empleado_Contrato contratoActual)
        {
            decimal porcentajePension = contratoActual.PensionAlimenticiaPorcentaje != null ? (decimal)contratoActual.PensionAlimenticiaPorcentaje.Value : 0;
            var tipoSueldo = contratoActual.PensionAlimenticiaSueldo ?? 0;//Tipo de sueldo SD o SDI
            decimal sueldo = 0;
            decimal pagoPensionAlimenticia = 0;

            if (monto <= 0) return null;
           

            pagoPensionAlimenticia = ((monto) * (porcentajePension / 100));

            NOM_Nomina_Detalle item = new NOM_Nomina_Detalle()
            {
                Id = 0,
                IdNomina = 0,
                IdConcepto = 48,
                Total = Utils.TruncateDecimales(pagoPensionAlimenticia),
                GravadoISR = Utils.TruncateDecimales(pagoPensionAlimenticia),
                ExentoISR = 0,
                IntegraIMSS = 0,
                ImpuestoSobreNomina = 0,
                Complemento = false

            };

            return item;
        }


        #region COMPLEMENTOS
        public static NOM_Nomina_Detalle PensionAlimenticiaComplemento(NOM_Nomina nomina, Empleado_Contrato contratoActual, bool isImpuestoSobreNomina = false, decimal porcentaje = 0)
        {
            decimal porcentajePension = contratoActual.PensionAlimenticiaPorcentaje != null ? (decimal)contratoActual.PensionAlimenticiaPorcentaje.Value : 0;
            var tipoSueldo = contratoActual.PensionAlimenticiaSueldo ?? 0;
            decimal sueldo = nomina.SDReal;
            decimal pagoPensionAlimenticia = 0;

            //switch (tipoSueldo)
            //{
            //    case 1:
            //        sueldo = nomina.SD;
            //        break;
            //    case 2:
            //        sueldo = nomina.SDI;
            //        break;
            //}

            if (sueldo > 0 && porcentajePension > 0)
            {
                pagoPensionAlimenticia = (porcentajePension * sueldo) * (porcentajePension / 100);

                // GuardarConcepto(nomina.IdNomina, 48, pagoPensionAlimenticia, pagoPensionAlimenticia, impuestoNomina: 0, isComplemento: true);

                NOM_Nomina_Detalle item = new NOM_Nomina_Detalle()
                {
                    Id = 0,
                    IdNomina = nomina.IdNomina,
                    IdConcepto = 48,
                    Total = pagoPensionAlimenticia,
                    GravadoISR = pagoPensionAlimenticia,
                    ExentoISR = 0,
                    IntegraIMSS = 0,
                    ImpuestoSobreNomina = 0,
                    Complemento = false

                };

                return item;

            }

            return null;

        }
        #endregion

        /// <summary>
        /// Guarda un registro en detalle de nomina
        /// </summary>
        /// <param name="idNomina"></param>
        /// <param name="idConcepto"></param>
        /// <param name="total"></param>
        /// <param name="gravaIsr"></param>
        /// <param name="excentoIsr"></param>
        /// <param name="integraImss"></param>
        /// <param name="impuestoNomina"></param>
        private static void GuardarConcepto(int idNomina = 0, int idConcepto = 0, decimal total = 0, decimal gravaIsr = 0, decimal excentoIsr = 0, decimal integraImss = 0, decimal impuestoNomina = 0, bool isComplemento = false, int idPrestamo = 0)
        {
            var nd = new NOM_Nomina_Detalle()
            {

                IdNomina = idNomina,
                IdConcepto = idConcepto,
                Total = total,
                GravadoISR = gravaIsr,
                ExentoISR = excentoIsr,
                IntegraIMSS = integraImss,
                ExentoIMSS = 0,
                ImpuestoSobreNomina = impuestoNomina,
                Complemento = isComplemento,
                IdPrestamo = idPrestamo
            };

            _nominasDao.AddDetalleNomina(nd);
        }

        private static NOM_Nomina_Detalle CalculoDeInfonavit(NOM_Nomina nomina, NOM_PeriodosPago periodoPago, int dias = -1)
        {
            decimal totalADescontar = 0;
         //   DateTime fechaServidor = DateTime.Now;
            int diasDeDescuento = 0;

            if (dias >= 0)
            {
                diasDeDescuento = dias;
            }
            else
            {
                diasDeDescuento = nomina.Dias_Laborados;
            }
           

            //1) Obtener el prestamo infonavit de la tabla Prestamos pasando el Id del contrato
            var prestamoInfonavit = NominasDao.GetPrestamoInfonavitByIdContrato(nomina.IdContrato);

            //Infonavit no maneja saldo

            if (prestamoInfonavit == null) return null;
            //Validar Fecha de inicio  del descuento

            if (periodoPago.Fecha_Inicio < prestamoInfonavit.FechaInicio && periodoPago.Fecha_Fin < prestamoInfonavit.FechaInicio) return null;

            //if (fechaServidor >= prestamoInfonavit.FechaInicio)
            
            //Validar Fecha suspension
            //if (prestamoInfonavit.FechaSuspension != null)
            //{
            //    if (prestamoInfonavit.FechaSuspension > fechaServidor)
            //        return null;
            //}

            //Validar Fecha de Suspension del descuentos
            //if (fechaServidor < prestamoInfonavit.FechaSuspension)
            //{
            RH.BLL.Infonavit inf = new RH.BLL.Infonavit();
            if (diasDeDescuento > 0)
            {
                //CALCULO DEL CREDITO -
                var calculo = inf.GetInfonavitById(prestamoInfonavit.Id);
                totalADescontar = diasDeDescuento * calculo.DescuentoDiario;
            }

            //Se guarda como detalle de la nomina
            //GuardarConcepto(nomina.IdNomina, 51, totalADescontar, 0, totalADescontar, 0, 0, false, prestamoInfonavit.Id);

            NOM_Nomina_Detalle item = new NOM_Nomina_Detalle()
            {
                Id = 0,
                IdNomina = nomina.IdNomina,
                IdConcepto = 51,
                Total = Utils.TruncateDecimales(totalADescontar),
                GravadoISR = 0,
                ExentoISR = Utils.TruncateDecimales(totalADescontar),
                IntegraIMSS = 0,
                ImpuestoSobreNomina = 0,
                Complemento = false,
                IdPrestamo = prestamoInfonavit?.Id ?? 0
            };

            return item;

            //}
        }

        private static List<NOM_Nomina_Detalle> CalculoDeFonacot(NOM_Nomina nomina, NOM_PeriodosPago periodoPago)
        {
            decimal totalDescuento = 0;
            DateTime fechaServidor = DateTime.Now;
            List<NOM_Nomina_Detalle> listaFonacot = new List<NOM_Nomina_Detalle>();

            //1) Obtener los creditos fonacot 
            var prestamosFonacot = NominasDao.GetPrestamosFonacotByIdContrato(nomina.IdContrato);

            if (prestamosFonacot.Count > 0)
            {
                //2) Por cada prestamo se obtiene su monto de descuento y se guarda en el detalle de la nomina con el id del Prestamo fonacot
                foreach (var prestamo in prestamosFonacot)
                {
                    //validar Saldo > 0
                    if (prestamo.Saldo <= 0) continue; // nueva iteracion del for

                    //validar Fecha de Inicio del Descuento
                    //if (fechaServidor < prestamo.FechaInicioDescuento) continue;
                    //if (  periodoPago.Fecha_Inicio > prestamo.FechaInicioDescuento || periodoPago.Fecha_Fin > prestamo.FechaInicioDescuento) continue;

                    if (periodoPago.Fecha_Inicio < prestamo.FechaInicioDescuento && periodoPago.Fecha_Fin < prestamo.FechaInicioDescuento) continue;
                    //validar FechaSuspension
                    //if (!(fechaServidor < prestamo.FechaSuspension)) continue;

                    //validar que la retencion no sea mayor que el saldo
                    var cantidadDescuento = prestamo.Retencion > prestamo.Saldo
                        ? prestamo.Saldo
                        : prestamo.Retencion;

                    //Guardar en el detalle de la nomina
                    //GuardarConcepto(nomina.IdNomina, 52, cantidadDescuento, 0, cantidadDescuento, 0, 0,false, prestamo.Id);
                    NOM_Nomina_Detalle item = new NOM_Nomina_Detalle()
                    {
                        Id = 0,
                        IdNomina = nomina.IdNomina,
                        IdConcepto = 52,
                        Total = Utils.TruncateDecimales(cantidadDescuento),
                        GravadoISR = 0,
                        ExentoISR = Utils.TruncateDecimales(cantidadDescuento),
                        IntegraIMSS = 0,
                        ImpuestoSobreNomina = 0,
                        Complemento = false,
                        IdPrestamo = prestamo.Id
                    };

                    //return item;

                    listaFonacot.Add(item);


                    //Guarda el total de todos los prestamos fonacot activos 
                    totalDescuento += cantidadDescuento;
                }


                return listaFonacot;
            }

            return null;
        }
    }
}
