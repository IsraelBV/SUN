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
    public static class MNominas
    {
        static readonly NominasDao _nominasDao = new NominasDao();
        static readonly RHDAO _rhDao = new RHDAO();

        //public static Task<TotalIsrSubsidio> CalculoDeIsr(NOM_Nomina nomina, decimal salarioBase, int tipoNomina, int diasPeriodo)
        //{

        //    var t = Task.Factory.StartNew(() =>
        //    {
        //        IsrSubsidio objCalculoSubsidio;

        //        if (tipoNomina == 16) //Asimilado
        //        {
        //            objCalculoSubsidio = CalculoIsrAsimilado(salarioBase, nomina.SD, diasPeriodo, 4);//5 tarifa mensual// se cambio a 4 por peticion de rodolfo y sergio
        //        }
        //        else
        //        {
        //            objCalculoSubsidio = CalculoIsrSubsidio(salarioBase, nomina.SD, diasPeriodo, tipoNomina);
        //        }

        //        if (objCalculoSubsidio != null)
        //        {
        //            //SE crea el objeto total del calculo
        //            var totalConcepto = new TotalIsrSubsidio
        //            {
        //                SubsidioCausado = objCalculoSubsidio.Subsidio,
        //                SubsidioEntregado = 0,
        //                IsrAntesSubsidio = objCalculoSubsidio.Isr,
        //                IsrCobrado = 0
        //            };


        //            //Si Resultado es ISR, Subsidio se guarda en Cero
        //            if (objCalculoSubsidio.EsISR)
        //            {
        //                totalConcepto.IsrCobrado = objCalculoSubsidio.ResultadoIsrOSubsidio;
        //                GuardarConcepto(nomina.IdNomina, 43, objCalculoSubsidio.ResultadoIsrOSubsidio, 0, 0, 0, 0);
        //                GuardarConcepto(nomina.IdNomina, 144, 0, 0, 0, 0, 0);
        //            }
        //            else  //Si Resultado es Subsidio, ISR se guarda en Cero
        //            {
        //                totalConcepto.SubsidioEntregado = objCalculoSubsidio.ResultadoIsrOSubsidio;
        //                GuardarConcepto(nomina.IdNomina, 144, (objCalculoSubsidio.ResultadoIsrOSubsidio), 0, 0, 0, 0);
        //                GuardarConcepto(nomina.IdNomina, 43, 0, 0, 0, 0, 0);
        //            }


        //            return totalConcepto;
        //        }
        //        else
        //        {
        //            var totalConcepto = new TotalIsrSubsidio
        //            {
        //                SubsidioCausado = 0,
        //                SubsidioEntregado = 0,
        //                IsrAntesSubsidio = 0,
        //                IsrCobrado = 0
        //            };

        //            return totalConcepto;
        //        }

        //    });

        //    return t;
        //}


            //usado para el calculo del isr de la nomina - 2 metodo nomina - son 2 porque ultimo se fuerza a que sea la tabla mensual
            //usando para el calculo isr del asimilado - 1

        public static TotalIsrSubsidio CalculoDeIsr(NOM_Nomina nomina, decimal salarioBase, int tipoNomina, int diasPeriodo, bool anualOK, int eje, decimal exento)
        {

            decimal isra = 0, saldo = 0;
            IsrSubsidio objCalculoSubsidio;

            if (tipoNomina == 16) //Asimilado
            {
                objCalculoSubsidio = CalculoIsrAsimilado(salarioBase, nomina.SD, diasPeriodo, 4);//5 tarifa mensual// se cambio a 4 por peticion de rodolfo y sergio
            }
            else
            {
                
               //objCalculoSubsidio = CalculoIsrSubsidioFin(nomina, salarioBase, nomina.SD, diasPeriodo, tipoNomina);
                objCalculoSubsidio = CalculoIsrSubsidio304(nomina, salarioBase, nomina.SD, diasPeriodo, tipoNomina);//factor
               
            }

            if (objCalculoSubsidio != null)
            {
                //SE crea el objeto total del calculo
                var totalConcepto = new TotalIsrSubsidio
                {
                    SubsidioCausado = objCalculoSubsidio.Subsidio, //GetSubsidioCausadoByTipoNomina(tipoNomina,salarioBase),
                    SubsidioEntregado = 0,
                    IsrAntesSubsidio = objCalculoSubsidio.IsrAntesDeSub,
                    IsrCobrado = 0
                };


             
                //Si Resultado es ISR, Subsidio se guarda en Cero
                if (objCalculoSubsidio.EsISR)
                {
                    totalConcepto.IsrCobrado = objCalculoSubsidio.ResultadoIsrOSubsidio;
                    GuardarConcepto(nomina.IdNomina, 43, objCalculoSubsidio.ResultadoIsrOSubsidio, 0, 0, 0, 0);
                    GuardarConcepto(nomina.IdNomina, 144, 0, 0, 0, 0, 0);
                }
                else  //Si Resultado es Subsidio, ISR se guarda en Cero
                {
                    totalConcepto.SubsidioEntregado = objCalculoSubsidio.ResultadoIsrOSubsidio;
                    GuardarConcepto(nomina.IdNomina, 144, (objCalculoSubsidio.ResultadoIsrOSubsidio), 0, 0, 0, 0);
                    GuardarConcepto(nomina.IdNomina, 43, 0, 0, 0, 0, 0);
                }

                if (anualOK)
                {
                    CalculoAnual Canual = new CalculoAnual();
                    NominasDao.EliminarCalculoAnual(nomina.IdEmpleado,nomina.IdPeriodo);
                    Canual = GetCalculoAnual(nomina.IdEmpleado, eje, nomina.IdEmpresaFiscal, salarioBase, objCalculoSubsidio.EsISR ? objCalculoSubsidio.ResultadoIsrOSubsidio : 0, objCalculoSubsidio.Subsidio,exento);
                    isra = Canual.ResultadoIsrOSubsidio > 0 ? Math.Abs(Canual.ResultadoIsrOSubsidio) : 0;
                    saldo = Canual.ResultadoIsrOSubsidio < 0 ? Math.Abs(Canual.ResultadoIsrOSubsidio) : 0;

                    if (isra > 0)
                    {
                        GuardarConcepto(nomina.IdNomina, 154, isra, 0, 0, 0, 0); 
                        totalConcepto.IsrCobrado += isra;
                    }

                    NOM_CalculoAnual itemCalculo = new NOM_CalculoAnual();
                    itemCalculo.id = 0;
                    itemCalculo.idEmpleado = nomina.IdEmpleado;
                    itemCalculo.idEmpresa = (int)nomina.IdEmpresaFiscal;
                    itemCalculo.idEjercicio = eje;
                    itemCalculo.idPeriodo = nomina.IdPeriodo;
                    itemCalculo.idNomina = nomina.IdNomina;
                    itemCalculo.baseGravable = Canual.BaseGravable;
                    itemCalculo.Exento = Canual.Exento;
                    itemCalculo.subCausado = Canual.Subsidio;
                    itemCalculo.isrAntes = Canual.IsrAntesDeSub;
                    itemCalculo.isrPagado = Canual.isrPagado;
                    itemCalculo.isrRetener = isra;
                    itemCalculo.saldoFavor = saldo;                   
                    itemCalculo.status = true;

                    _nominasDao.AddCalculoAnual(itemCalculo);
                }

                return totalConcepto;
            }
            else
            {
                var totalConcepto = new TotalIsrSubsidio
                {
                    SubsidioCausado = 0,
                    SubsidioEntregado = 0,
                    IsrAntesSubsidio = 0,
                    IsrCobrado = 0
                };

                return totalConcepto;
            }

        }

        private static void GuardarConcepto(int idNomina = 0, int idConcepto = 0, decimal total = 0, decimal gravaIsr = 0, decimal excentoIsr = 0, decimal integraImss = 0, decimal impuestoNomina = 0)
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

        /// <summary>
        /// Método que realiza el cálculo del IRS/SUBSIDIO Retorna un objeto "IsrSubsidio" con los valores del cálculo.
        /// tipoTarifa: Diario, semanal, quincenal, etc..
        /// 1.- Usado en Detalle de la nomina 
        /// 2.- Fininiquito - 
        /// 3.- finiquito F
        /// 4.- ultimo sueldo
        /// 5- en este mismo archivo linea 96
        /// </summary>
        /// <param name="nomina"></param>
        /// <param name="baseGravable"></param>
        /// <param name="diasPeriodo"></param>
        /// <param name="tipoTarifa"></param>
        /// <returns></returns>
        public static IsrSubsidio CalculoIsrSubsidioFin(NOM_Nomina nomina,decimal baseGravable, decimal SD, int diasPeriodo, int tipoTarifa, bool incluirTablas = false)
        {
            ////ASimilados a salario
            //if (tipoTarifa == 16) //Asimilado a Salarios - tipo Tarifa viene con el valor de Periodicidad de Pago
            //{
            //    if (diasPeriodo > 15)//Aqui habria que asegurarse de que tarifa quieren aplicar para el calculo
            //    {
            //        tipoTarifa = 5;//Lo cambiamos al tipo de tarifa mensual    
            //    }
            //    else if (diasPeriodo == 15)
            //    {
            //        tipoTarifa = 4; //Lo cambiamos al tipo de tarifa quincenal 
            //    }
            //    else
            //    {
            //        tipoTarifa = 2; //Lo cambiamos al tipo de tarifa semanal 
            //    }
            //}

            if (baseGravable <= 0) return null;
            if (tipoTarifa <= 0) return null;

            bool esISR = false;
            double variableMensual = 30.40;
            double nuevaBaseGravable = 0.00;
            double holdBaseGravable = 0.00;
            decimal subsidio14 = 0;
            decimal isr14 = 0;

            //Regla 14 dias - para nominas catorcenales
            if (diasPeriodo == 14)
            {
                //Se obtiene el sueldo mensual
                nuevaBaseGravable = ((double)SD * variableMensual);
                holdBaseGravable = (double)baseGravable;
                baseGravable = (decimal)nuevaBaseGravable;
                tipoTarifa = 5;//mensual
            }

            List<C_NOM_Tabla_ISR> tablaisrcompleta = null;
            List<C_NOM_Tabla_Subsidio> tablasubsidiocompleta = null;

            var tablaIsr = NominasDao.GeTablaIsrByTipoNomina(tipoTarifa, baseGravable);
            var tablaSubsidio = NominasDao.GetTablaSubsidioByTipoNomina(tipoTarifa, baseGravable);

            if (tablaIsr == null || tablaSubsidio == null) return null;

            if (incluirTablas)
            {
                tablaisrcompleta = NominasDao.GetAllTablaIsr(tipoTarifa);
                tablasubsidiocompleta = NominasDao.GetAllTablaSubsidios(tipoTarifa);
            }

            //1) Buscar el rango de limite inferior
            decimal limiteInferior = tablaIsr.Limite_Inferior;

            //2) Restar Ingreso - Limite Inferior = BASE
            decimal _base = baseGravable - limiteInferior;

            //3) Tomar el porcentaje del Rango %
            decimal porcentaje = tablaIsr.Porcentaje;

            //4) Multiplicar el % por la BASE
            decimal resultado = _base * (porcentaje / 100);

            //5)Tomar la cuota fija del rango
            decimal cuotaFija = tablaIsr.Cuota_Fija;

            //6 Sumar 4) + 5) = ISR
            decimal isr = resultado + cuotaFija;

            // 7) buscar en la tabla de subsidio en que rango esta el Salario Gravable
            decimal subsidioAlEmpleo = tablaSubsidio.Subsidio;

            //7.1 proporcional para el periodo de 14 dias 
            if (diasPeriodo == 14)
            {
                isr14 = (isr / (decimal)variableMensual) * diasPeriodo;
                subsidio14 = (subsidioAlEmpleo / (decimal)variableMensual) * diasPeriodo;
                baseGravable = (decimal)holdBaseGravable;
            }


            //8) Resta del 6) - 7) = ISR o Subsidio
            decimal isrOSubsidio = 0;
            if (isr > subsidioAlEmpleo)
            {
                isrOSubsidio = (isr - subsidioAlEmpleo);

                if (diasPeriodo == 14)
                {
                    isrOSubsidio = (isr14 - subsidio14);
                }
            }
            else
            {
                isrOSubsidio = (subsidioAlEmpleo - isr);

                if (diasPeriodo == 14)
                {
                    isrOSubsidio = (subsidio14 - isr14);
                }
            }


            //9) Neto a pagar Salario Gravable - 8)
            decimal netoPagar = 0;
            esISR = isr > subsidioAlEmpleo;

            if (esISR)
            {
                netoPagar = baseGravable - (isrOSubsidio);
            }
            else
            {
                netoPagar = baseGravable + (isrOSubsidio);
            }

            esISR = isr > subsidioAlEmpleo;

            var item = new IsrSubsidio()
            {

                BaseGravable = baseGravable,
                BaseGravableMensual = (decimal)nuevaBaseGravable,
                LimiteInferior = limiteInferior,
                Base = _base,
                Tasa = porcentaje,
                IsrAntesDeSub = isr,
                Subsidio = subsidioAlEmpleo,
                NetoAPagar = netoPagar,
                ResultadoIsrOSubsidio =Common.Utils.Utils.TruncateDecimales(isrOSubsidio),
                IdTablaIsr = tablaIsr.IdISR,
                IdTablaSubsidio = tablaSubsidio.IdSubsidio,
                Tablaisr = incluirTablas ? tablaisrcompleta : null,
                Tablasubsidio = incluirTablas ? tablasubsidiocompleta : null,
                EsISR = esISR
            };

            return item;
        }

        public static IsrSubsidio CalculoIsrSubsidioAnual( decimal baseGravable)
        {
            int tipoTarifa = 365;
            bool esISR;

            var tablaIsr = NominasDao.GeTablaIsrByTipoNomina(tipoTarifa, baseGravable);
//            var tablaSubsidio = NominasDao.GetTablaSubsidioByTipoNomina(tipoTarifa, baseGravable);

            if (tablaIsr == null ) return null;
         

            //1) Buscar el rango de limite inferior
            decimal limiteInferior = tablaIsr.Limite_Inferior;

            //2) Restar Ingreso - Limite Inferior = BASE
            decimal _base = baseGravable - limiteInferior;

            //3) Tomar el porcentaje del Rango %
            decimal porcentaje = tablaIsr.Porcentaje;

            //4) Multiplicar el % por la BASE
            decimal resultado = _base * (porcentaje / 100);

            //5)Tomar la cuota fija del rango
            decimal cuotaFija = tablaIsr.Cuota_Fija;
            
            //6 Sumar 4) + 5) = ISR
            decimal isrOSubsidio = resultado + cuotaFija;

       

        

            var item = new IsrSubsidio()
            {
                BaseGravable = baseGravable,
                BaseGravableMensual = cuotaFija,
                LimiteInferior = limiteInferior,
                Base = _base,
                Tasa = porcentaje,
                IsrAntesDeSub = isrOSubsidio,
                Subsidio = 0,
                NetoAPagar = resultado,
                ResultadoIsrOSubsidio = Utils.TruncateDecimales(isrOSubsidio)
            };

            return item;
        }

        //Version 2 - 30.4
        //Method Name - CalculoIsrSubsidio304
        //
        public static IsrSubsidio CalculoIsrSubsidio304(NOM_Nomina nomina, decimal baseGravable, decimal SD, int diasPeriodo, int tipoTarifa, bool incluirTablas = false)
        {


            if (baseGravable <= 0) return null;
            //if (tipoTarifa <= 0) return null;

            bool esISR = false;
            decimal factor304 = 30.40M;
            decimal nuevaBaseGravable = 0;
            decimal holdBaseGravable = 0;
            decimal subsidio304 = 0;
            decimal isr304 = 0;

            //Regla 14 dias - para nominas catorcenales***********************************************************
            //version 30.4
            //Se obtiene el sueldo mensual

            //nuevaBaseGravable = (SD * variableMensual);//Obtenemos el sueldo mensual SD * 30.4
            //holdBaseGravable = baseGravable; //Guardamos la baseGravable anterior
            //baseGravable = (decimal)nuevaBaseGravable;


            nuevaBaseGravable = (baseGravable / nomina.Dias_Laborados) * factor304;//nueva base a 30.4
            holdBaseGravable = baseGravable; //Guardamos la baseGravable anterior
            baseGravable = (decimal)nuevaBaseGravable;// establecemos como base de calculo la nueva base encontrada
            //******************************************************************************************************


            tipoTarifa = 5;//mensual


            List<C_NOM_Tabla_ISR> tablaisrcompleta = null;
            List<C_NOM_Tabla_Subsidio> tablasubsidiocompleta = null;

            var tablaIsr = NominasDao.GeTablaIsrByTipoNomina(tipoTarifa, baseGravable);
            var tablaSubsidio = NominasDao.GetTablaSubsidioByTipoNomina(tipoTarifa, baseGravable);

            if (tablaIsr == null || tablaSubsidio == null) return null;

            if (incluirTablas)
            {
                tablaisrcompleta = NominasDao.GetAllTablaIsr(tipoTarifa);
                tablasubsidiocompleta = NominasDao.GetAllTablaSubsidios(tipoTarifa);
            }

            //1) Buscar el rango de limite inferior
            decimal limiteInferior = tablaIsr.Limite_Inferior;

            //2) Restar Ingreso - Limite Inferior = BASE
            decimal _base = baseGravable - limiteInferior;

            //3) Tomar el porcentaje del Rango %
            decimal porcentaje = tablaIsr.Porcentaje;

            //4) Multiplicar el % por la BASE
            decimal resultado = _base * (porcentaje / 100);

            //5)Tomar la cuota fija del rango
            decimal cuotaFija = tablaIsr.Cuota_Fija;

            //6 Sumar 4) + 5) = ISR
            decimal isr = resultado + cuotaFija;

            // 7) buscar en la tabla de subsidio en que rango esta el Salario Gravable
            decimal subsidioAlEmpleo = tablaSubsidio.Subsidio;

            //7.1 proporcional para el periodo de 14 dias 

            //version 30.4
            isr304 = Utils.TruncateDecimales((isr / (decimal)factor304) * nomina.Dias_Laborados);
            subsidio304 = Utils.TruncateDecimales((subsidioAlEmpleo / (decimal)factor304) * nomina.Dias_Laborados);
            baseGravable = (decimal)holdBaseGravable;



            //8) Resta del 6) - 7) = ISR o Subsidio
            decimal isrOSubsidio = 0;
            if (isr > subsidioAlEmpleo)
            {
                isrOSubsidio = (isr - subsidioAlEmpleo);

                //version 30.4
                isrOSubsidio = (isr304 - subsidio304);
                
            }
            else
            {
                isrOSubsidio = (subsidioAlEmpleo - isr);

                //version 30.4
                isrOSubsidio = (subsidio304 - isr304);
                
            }


            //9) Neto a pagar Salario Gravable - 8)
            decimal netoPagar = 0;
            esISR = isr > subsidioAlEmpleo;

            if (esISR)
            {
                netoPagar = baseGravable - (isrOSubsidio);
            }
            else
            {
                netoPagar = baseGravable + (isrOSubsidio);
            }

            esISR = isr > subsidioAlEmpleo;

            var item = new IsrSubsidio()
            {

                BaseGravable = baseGravable,
                BaseGravableMensual = (decimal)nuevaBaseGravable,
                LimiteInferior = limiteInferior,
                Base = _base,
                Tasa = porcentaje,
                IsrAntesDeSub = isr304,
                Subsidio = subsidio304,//subsidioAlEmpleo
                NetoAPagar = Utils.TruncateDecimales(netoPagar),
                ResultadoIsrOSubsidio = Common.Utils.Utils.TruncateDecimales(isrOSubsidio),
                IdTablaIsr = tablaIsr.IdISR,
                IdTablaSubsidio = tablaSubsidio.IdSubsidio,
                Tablaisr = incluirTablas ? tablaisrcompleta : null,
                Tablasubsidio = incluirTablas ? tablasubsidiocompleta : null,
                EsISR = esISR
            };

            return item;
        }

        public static IsrSubsidio CalculoIsrSubsidio304especial(int Dias_Laborados, decimal baseGravable, decimal SD, int diasPeriodo, int tipoTarifa, bool incluirTablas = false)
        {


            if (baseGravable <= 0) return null;
            //if (tipoTarifa <= 0) return null;

            bool esISR = false;
            decimal factor304 = 30.40M;
            decimal nuevaBaseGravable = 0;
            decimal holdBaseGravable = 0;
            decimal subsidio304 = 0;
            decimal isr304 = 0;

            //Regla 14 dias - para nominas catorcenales***********************************************************
            //version 30.4
            //Se obtiene el sueldo mensual

            //nuevaBaseGravable = (SD * variableMensual);//Obtenemos el sueldo mensual SD * 30.4
            //holdBaseGravable = baseGravable; //Guardamos la baseGravable anterior
            //baseGravable = (decimal)nuevaBaseGravable;


            nuevaBaseGravable = (baseGravable / Dias_Laborados) * factor304;//nueva base a 30.4
            holdBaseGravable = baseGravable; //Guardamos la baseGravable anterior
            baseGravable = (decimal)nuevaBaseGravable;// establecemos como base de calculo la nueva base encontrada
            //******************************************************************************************************


            tipoTarifa = 5;//mensual


            List<C_NOM_Tabla_ISR> tablaisrcompleta = null;
            List<C_NOM_Tabla_Subsidio> tablasubsidiocompleta = null;

            var tablaIsr = NominasDao.GeTablaIsrByTipoNomina(tipoTarifa, baseGravable);
            var tablaSubsidio = NominasDao.GetTablaSubsidioByTipoNomina(tipoTarifa, baseGravable);

            if (tablaIsr == null || tablaSubsidio == null) return null;

            if (incluirTablas)
            {
                tablaisrcompleta = NominasDao.GetAllTablaIsr(tipoTarifa);
                tablasubsidiocompleta = NominasDao.GetAllTablaSubsidios(tipoTarifa);
            }

            //1) Buscar el rango de limite inferior
            decimal limiteInferior = tablaIsr.Limite_Inferior;

            //2) Restar Ingreso - Limite Inferior = BASE
            decimal _base = baseGravable - limiteInferior;

            //3) Tomar el porcentaje del Rango %
            decimal porcentaje = tablaIsr.Porcentaje;

            //4) Multiplicar el % por la BASE
            decimal resultado = _base * (porcentaje / 100);

            //5)Tomar la cuota fija del rango
            decimal cuotaFija = tablaIsr.Cuota_Fija;

            //6 Sumar 4) + 5) = ISR
            decimal isr = resultado + cuotaFija;

            // 7) buscar en la tabla de subsidio en que rango esta el Salario Gravable
            decimal subsidioAlEmpleo = tablaSubsidio.Subsidio;

            //7.1 proporcional para el periodo de 14 dias 

            //version 30.4
            isr304 = Utils.TruncateDecimales((isr / (decimal)factor304) * Dias_Laborados);
            subsidio304 = Utils.TruncateDecimales((subsidioAlEmpleo / (decimal)factor304) * Dias_Laborados);
            baseGravable = (decimal)holdBaseGravable;



            //8) Resta del 6) - 7) = ISR o Subsidio
            decimal isrOSubsidio = 0;
            if (isr > subsidioAlEmpleo)
            {
                isrOSubsidio = (isr - subsidioAlEmpleo);

                //version 30.4
                isrOSubsidio = (isr304 - subsidio304);

            }
            else
            {
                isrOSubsidio = (subsidioAlEmpleo - isr);

                //version 30.4
                isrOSubsidio = (subsidio304 - isr304);

            }


            //9) Neto a pagar Salario Gravable - 8)
            decimal netoPagar = 0;
            esISR = isr > subsidioAlEmpleo;

            if (esISR)
            {
                netoPagar = baseGravable - (isrOSubsidio);
            }
            else
            {
                netoPagar = baseGravable + (isrOSubsidio);
            }

            esISR = isr > subsidioAlEmpleo;

            var item = new IsrSubsidio()
            {

                BaseGravable = baseGravable,
                BaseGravableMensual = (decimal)nuevaBaseGravable,
                LimiteInferior = limiteInferior,
                Base = _base,
                Tasa = porcentaje,
                IsrAntesDeSub = isr304,
                Subsidio = subsidio304,//subsidioAlEmpleo
                NetoAPagar = Utils.TruncateDecimales(netoPagar),
                ResultadoIsrOSubsidio = Common.Utils.Utils.TruncateDecimales(isrOSubsidio),
                IdTablaIsr = tablaIsr.IdISR,
                IdTablaSubsidio = tablaSubsidio.IdSubsidio,
                Tablaisr = incluirTablas ? tablaisrcompleta : null,
                Tablasubsidio = incluirTablas ? tablasubsidiocompleta : null,
                EsISR = esISR
            };

            return item;
        }

        public static IsrSubsidio  CalculoIsrSubsidio174(decimal Gravable,decimal sd)
        {
            var porcentaje = 0.00M;
            
       

            // Articulo 1 del 174.
            decimal monto174 = Utils.TruncateDecimales((Gravable / 365) * 30.40M);
            // Se calcula el sueldo diario por 30.4 para saber sueldo mes anterior
            decimal sueldoMensual304 = Utils.TruncateDecimales(30.4M * sd);
            // Articulo 2 del 174. Sacamos la base gravable
            decimal baseGravable174 = monto174 + sueldoMensual304;
            //Se calcula isr de la Base generada en Articulo 2
            var Isrbase174 = CalculoIsrSubsidioFin(null, baseGravable174, sd, 0, 5, false);
            // Se calcula isr generado en sueldo ordinario del mes anterior
            var isrSueldoMensual = Utils.TruncateDecimales(CalculoIsrSubsidioFin(null, sueldoMensual304, sd, 0, 5, false).IsrAntesDeSub);
            // Se calcula diferencia de ISR conforme Articulo 3 del 174
            var r1 = Isrbase174.IsrAntesDeSub - isrSueldoMensual;
            //Calculo porcentaje conforme Articulo 5 del 174
            porcentaje = r1 / monto174;
            // Calculo ISR tomando base gravable inicial * tasa. Articulo 4 del 174
            var resultadoIsr174 = Utils.TruncateDecimales(Gravable * porcentaje);          
                
            if (resultadoIsr174 < 0)
                resultadoIsr174 = resultadoIsr174 * -1;

            var item = new IsrSubsidio()
            {
                BaseGravable = Gravable,
                BaseGravableMensual = (decimal)Gravable,
                LimiteInferior = 0,
                Base = 0,
                Tasa = porcentaje,
                IsrAntesDeSub = resultadoIsr174,
                Subsidio = 0,//subsidioAlEmpleo
                NetoAPagar = 0,
                ResultadoIsrOSubsidio = Utils.TruncateDecimales(resultadoIsr174),
                IdTablaIsr = 0,
                IdTablaSubsidio = 0,
                Tablaisr = null,
                Tablasubsidio = null,
                EsISR = true// Asignamos true debido a que no calculamos subsidio.
            };

            return item;


        }

        public static IsrSubsidio CalculoIsrAsimilado(decimal baseGravable, decimal SD, int diasPeriodo, int tipoTarifa, bool incluirTablas = false)
        {


            if (baseGravable <= 0) return null;
            if (tipoTarifa <= 0) return null;

            bool esISR = false;

            double nuevaBaseGravable = 0.00;

            List<C_NOM_Tabla_ISR> tablaisrcompleta = null;
            List<C_NOM_Tabla_Subsidio> tablasubsidiocompleta = null;

            var tablaIsr = NominasDao.GeTablaIsrByTipoNomina(tipoTarifa, baseGravable);
            //  var tablaSubsidio = NominasDao.GetTablaSubsidioByTipoNomina(tipoTarifa, baseGravable);

            if (tablaIsr == null) return null;

            if (incluirTablas)
            {
                tablaisrcompleta = NominasDao.GetAllTablaIsr(tipoTarifa);
                tablasubsidiocompleta = NominasDao.GetAllTablaSubsidios(tipoTarifa);
            }

            //1) Buscar el rango de limite inferior
            decimal limiteInferior = tablaIsr.Limite_Inferior;

            //2) Restar Ingreso - Limite Inferior = BASE
            decimal _base = baseGravable - limiteInferior;

            //3) Tomar el porcentaje del Rango %
            decimal porcentaje = tablaIsr.Porcentaje;

            //4) Multiplicar el % por la BASE
            decimal resultado = _base * (porcentaje / 100);

            //5)Tomar la cuota fija del rango
            decimal cuotaFija = tablaIsr.Cuota_Fija;

            //6 Sumar 4) + 5) = ISR
            decimal isr = resultado + cuotaFija;

            // 7) buscar en la tabla de subsidio en que rango esta el Salario Gravable
            decimal subsidioAlEmpleo = 0; // tablaSubsidio.Subsidio;


            //8) Resta del 6) - 7) = ISR o Subsidio
            decimal isrOSubsidio = 0;

            //9) Neto a pagar Salario Gravable - 8)
            decimal netoPagar = baseGravable - isr;

            isrOSubsidio = isr;
            esISR = true;

            var item = new IsrSubsidio()
            {
                BaseGravable = baseGravable,
                BaseGravableMensual = (decimal)nuevaBaseGravable,
                LimiteInferior = limiteInferior,
                Base = _base,
                Tasa = porcentaje,
                IsrAntesDeSub = isr,
                Subsidio = subsidioAlEmpleo,
                NetoAPagar = netoPagar,
                ResultadoIsrOSubsidio = Utils.TruncateDecimales(isrOSubsidio),
                IdTablaIsr = tablaIsr.IdISR,
                IdTablaSubsidio = 0,//tablaSubsidio.IdSubsidio,
                Tablaisr = incluirTablas ? tablaisrcompleta : null,
                Tablasubsidio = incluirTablas ? tablasubsidiocompleta : null,
                EsISR = esISR
            };

            return item;
        }

        public static int GetInasistenciasByIdEmpleado(int idEmpleado)
        {
            return 0; //idEmpleado <= 0 ? 0 : _rhDao.GetInasistenciasByIdEmpleado(idEmpleado);
        }

        public static decimal GetSubsidioCausadoByTipoNomina(int tipoNomina, decimal sbc)
        {
            if (tipoNomina == 16) { return 0; }
            var tablaSubsidio = NominasDao.GetTablaSubsidioByTipoNomina(tipoNomina, sbc);

            return tablaSubsidio.Subsidio;
        }

        public static CalculoAnual GetCalculoAnualconPrevision(int idEmpleado, int idEjercicio, decimal sd,decimal sdi, int? idempresa, decimal gravadoextra, decimal uma)
        {
           
            decimal resultadoA = 0;
            int[] ltnom = {2,3,4,5,6};
            var item = new CalculoAnual()
            {

                BaseGravable = 0,
                Cuotafija = 0, 
                LimiteInferior = 0,               
                Base = 0,          
                Tasa =0 ,      
                IsrAntesDeSub = 0,
                Subsidio = 0,
                marginal =0,
                ResultadoIsrOSubsidio = 0,
                isrPagado = 0

            };

            using (var ctx = new RHEntities())
            {
                int tLeft = 0, diasPeriocidad = 0;
                decimal sueldoPeriocidad = 0, sueldoPeriocidadPremio = 0;

                var Anominas = (from n in ctx.NOM_Nomina
                            join cf in ctx.NOM_CFDI_Timbrado on new { n.IdPeriodo, n.IdNomina } equals new { cf.IdPeriodo, cf.IdNomina }
                            where n.IdEmpleado == idEmpleado && n.IdEjercicio == idEjercicio && n.IdEmpresaFiscal == idempresa && n.IdEmpresaFiscal != 0 && cf.Cancelado==false
                            select n).ToList();
                // var Anominas = ctx.NOM_Nomina.Where(x => x.IdEmpleado == idEmpleado && x.IdEjercicio == idEjercicio && x.IdEmpresaFiscal==idempresa && x.IdEmpresaFiscal!=0).ToList();
                if (Anominas.Count > 0)
                {
                    var Lnominas = Anominas.Select(x => x.IdNomina).ToList();

                    var cper = (from n in Anominas
                                join p in ctx.NOM_PeriodosPago on n.IdPeriodo equals p.IdPeriodoPago
                                where ltnom.Contains(p.IdTipoNomina)
                                orderby n.IdPeriodo ascending
                                select p.IdPeriodoPago);

                    if (cper.Count() == 0) { return item = new CalculoAnual(); }

                    int per = cper.Last();

                    //Anominas.Where(x=> x.).Select(x => x.IdPeriodo).Last();
                    DateTime fechafinPeriodo = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == per).Select(x => x.Fecha_Fin).FirstOrDefault();
                    int periodicidad = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == idEmpleado && x.FechaBaja == null).Select(x => x.IdPeriodicidadPago).FirstOrDefault();
                    var TisrpagadoA = ctx.NOM_Nomina_Detalle.Where(x => x.IdConcepto == 43 && Lnominas.Contains(x.IdNomina)).Select(x => x.Total).Sum();
                    var TsaldoAnual = ctx.NOM_Nomina_Detalle.Where(x => x.IdConcepto == 143 && Lnominas.Contains(x.IdNomina)).Select(x => x.Total).DefaultIfEmpty(0).Sum();
                    var TsubcausadoA = ctx.NOM_Nomina.Where(x => Lnominas.Contains(x.IdNomina)).Select(x => x.SubsidioCausado).Sum();
                    var TgravadoA = ctx.NOM_Nomina_Detalle.Where(x => Lnominas.Contains(x.IdNomina)).Select(x => x.GravadoISR).Sum();
                    item.Exento  = ctx.NOM_Nomina_Detalle.Where(x => Lnominas.Contains(x.IdNomina)).Select(x => x.ExentoISR).Sum();
                    var conceptosE   = ctx.NOM_Empleado_Conceptos.Where(x => x.IdEmpleado == idEmpleado);

                    int dQuedan = Utils.GetDaysleft(fechafinPeriodo);
                    
                    // para calculo anual de años pasados
                    if (fechafinPeriodo.Year == DateTime.Now.Year)
                    {
                        bool asist = conceptosE.Any(x => x.IdConcepto == 40);
                        bool punt = conceptosE.Any(x => x.IdConcepto == 8);

                        // Bloque para generar prevision de nomina
                        switch (periodicidad)
                        {
                            case 2:
                                diasPeriocidad = 7;
                                break;
                            case 3:
                                diasPeriocidad = 14;
                                break;
                            case 4:
                                diasPeriocidad = 15;
                                break;
                            case 5:
                                diasPeriocidad = 30;
                                break;
                        }

                        tLeft = dQuedan / diasPeriocidad;
                        sueldoPeriocidadPremio = sdi * diasPeriocidad;
                        sueldoPeriocidad = sd * diasPeriocidad;

                        //lo siguente calcula el valance de los dias que faltan cuando las quincenas son de 16 dias y no de 15
                        //refiriendoce a los meses que tienen 31 dias, ya qye al calcular "sueldoPeriocidad" se toma en cuenta el valor absoluto de la periodicidad (diasPeriocidad).
                        var modbalance = dQuedan % diasPeriocidad;
                        var difgrabado = sd * modbalance;

                        //calculo probicional de aguinaldo para grabado anual

                        //var aguinaldo = sd * 15;
                        //var limiteExentoAguinaldo = uma * 30m;

                        //var grabadoAguinaldo = (aguinaldo > limiteExentoAguinaldo && limiteExentoAguinaldo > 0) ? aguinaldo - limiteExentoAguinaldo : 0;


                        // Verifica si tiene derecho a premio asistencia y puntualidad.
                        if (asist) { sueldoPeriocidad = sueldoPeriocidad + (sueldoPeriocidadPremio * 0.10M); }
                        if (punt) { sueldoPeriocidad = sueldoPeriocidad + (sueldoPeriocidadPremio * 0.10M); }

                        //ISR del sueldo nominal mas sus premios.
                        var isrPeriocidad = CalculoIsrSubsidio304especial(diasPeriocidad, sueldoPeriocidad, sd, 0, Anominas.Select(x => x.TipoTarifa).FirstOrDefault(), false);

                        //calculo especial de isr para los dias extras que faltan cuando los meses tiene 31 dias (simepre por que diciembre tiene 31 dias)                
                        var isrDiasFaltantes = CalculoIsrSubsidio304especial(modbalance, (sd * modbalance), sd, 0, Anominas.Select(x => x.TipoTarifa).FirstOrDefault(), false);

                        // Verificacion si genera ISR o Subsidio.
                        if (isrPeriocidad.EsISR)
                        {
                            decimal isrPrevision = isrPeriocidad.ResultadoIsrOSubsidio * tLeft;
                            //se le suma el proporcional del impuesto de los dias faltantes
                            isrPrevision += isrDiasFaltantes.ResultadoIsrOSubsidio;

                            TisrpagadoA += isrPrevision;
                        }
                        decimal subPrevision = isrPeriocidad.Subsidio * tLeft;
                        //se le suma el proporcional del impuesto de los dias faltantes si existe
                        if (isrDiasFaltantes != null) {
                            subPrevision += isrDiasFaltantes.Subsidio;
                        }

                        TsubcausadoA += subPrevision;

                        TgravadoA = tLeft > 0 ? TgravadoA + ((sueldoPeriocidad * tLeft) + difgrabado /*+ grabadoAguinaldo*/) : TgravadoA;
                        //#######################################################################////
                    }

                    // Anexamos si hay algun gravado que no se tome de las nominas
                    TgravadoA += gravadoextra;
                    // Guardamos valores para presentar en reporte.
                    item.BaseGravable = TgravadoA;
                    item.Subsidio = TsubcausadoA;                    

                    // Se calcula ISR del total gravado con prevision.
                    var isrTgravadoA = CalculoIsrSubsidioAnual(TgravadoA);

                    // Guardamos valores para presentar en reporte.
                    item.Tasa = isrTgravadoA.Tasa;
                    item.Base = isrTgravadoA.Base;
                    item.LimiteInferior = isrTgravadoA.LimiteInferior;
                    item.IsrAntesDeSub = isrTgravadoA.ResultadoIsrOSubsidio;
                    item.Cuotafija = isrTgravadoA.BaseGravableMensual;
                    item.marginal = isrTgravadoA.NetoAPagar;
                    item.isrPagado = TisrpagadoA;
                    //Comparacion en caso de que subsidio causado en el año es mayor al ISR anual no se cobra ni se repone valor al empleado.
                    if (isrTgravadoA.ResultadoIsrOSubsidio < TsubcausadoA) { return item; }


                    resultadoA = isrTgravadoA.ResultadoIsrOSubsidio - TsubcausadoA;

                    resultadoA += TsaldoAnual;
                    resultadoA -= TisrpagadoA;
                    
                    item.ResultadoIsrOSubsidio = resultadoA;  
                }
                /*  var datos = ctx.NOM_Nomina.Where(x=> x.SBCotizacionDelProcesado>0 && x.Dias_Laborados!=0);               
                    datos.ToList().ForEach(c => c.SubsidioCausado = CalculoIsrSubsidio304especial(c.Dias_Laborados, c.SBCotizacionDelProcesado, c.SD, 0, c.TipoTarifa, false).Subsidio);
                    datos.ToList().ForEach(c => c.ISRantesSubsidio = CalculoIsrSubsidio304especial(c.Dias_Laborados, c.SBCotizacionDelProcesado, c.SD, 0, c.TipoTarifa, false).IsrAntesDeSub);
                    ctx.SaveChanges();*/



            }
            return item;
        }

        public static CalculoAnual GetCalculoAnual(int idEmpleado, int idEjercicio, int? idempresa, decimal baseg, decimal isr, decimal sub, decimal exento)
        {
            decimal resultadoA = 0;
            int[] ltnom = { 2, 3, 4, 5, 6 };
            var item = new CalculoAnual()
            {

                BaseGravable = 0,
                Cuotafija = 0,
                LimiteInferior = 0,
                Base = 0,
                Tasa = 0,
                IsrAntesDeSub = 0,
                Subsidio = 0,
                marginal = 0,
                ResultadoIsrOSubsidio = 0,
                isrPagado = 0

            };

            using (var ctx = new RHEntities())
            {
                
                var Anominas = (from n in ctx.NOM_Nomina
                                join cf in ctx.NOM_CFDI_Timbrado on new { n.IdPeriodo, n.IdNomina } equals new { cf.IdPeriodo, cf.IdNomina }
                                where n.IdEmpleado == idEmpleado && n.IdEjercicio == idEjercicio && n.IdEmpresaFiscal == idempresa && n.IdEmpresaFiscal != 0 && cf.Cancelado == false
                                select n).ToList();                
                if (Anominas.Count > 0)
                {
                    var Lnominas = Anominas.Select(x => x.IdNomina).ToList();

                    var cper = (from n in Anominas
                                join p in ctx.NOM_PeriodosPago on n.IdPeriodo equals p.IdPeriodoPago
                                where ltnom.Contains(p.IdTipoNomina)
                                orderby n.IdPeriodo ascending
                                select p.IdPeriodoPago);

                    if (cper.Count() == 0) { return item = new CalculoAnual(); }            
                  
                    var TisrpagadoA = ctx.NOM_Nomina_Detalle.Where(x => x.IdConcepto == 43 && Lnominas.Contains(x.IdNomina)).Select(x => x.Total).Sum();
                    var TsaldoAnual = ctx.NOM_Nomina_Detalle.Where(x => x.IdConcepto == 143 && Lnominas.Contains(x.IdNomina)).Select(x => x.Total).DefaultIfEmpty(0).Sum();
                    var TsubcausadoA = ctx.NOM_Nomina.Where(x => Lnominas.Contains(x.IdNomina)).Select(x => x.SubsidioCausado).Sum();
                    var TgravadoA = ctx.NOM_Nomina_Detalle.Where(x => Lnominas.Contains(x.IdNomina)).Select(x => x.GravadoISR).Sum();
                    var TexentoA = ctx.NOM_Nomina_Detalle.Where(x => Lnominas.Contains(x.IdNomina)).Select(x => x.ExentoISR).Sum();

                    TgravadoA += baseg;
                    TexentoA += exento;
                    TisrpagadoA += isr;
                    TsubcausadoA += sub;                    
                    
                    // Guardamos valores para presentar en reporte.
                    item.BaseGravable = TgravadoA;
                    item.Exento = TexentoA;
                    item.Subsidio = TsubcausadoA;

                    // Se calcula ISR del total gravado con prevision.
                    var isrTgravadoA = CalculoIsrSubsidioAnual(TgravadoA);

                    // Guardamos valores para presentar en reporte.
                    item.Tasa = isrTgravadoA.Tasa;
                    item.Base = isrTgravadoA.Base;
                    item.LimiteInferior = isrTgravadoA.LimiteInferior;
                    item.IsrAntesDeSub = isrTgravadoA.ResultadoIsrOSubsidio;
                    item.Cuotafija = isrTgravadoA.BaseGravableMensual;
                    item.marginal = isrTgravadoA.NetoAPagar;
                    item.isrPagado = TisrpagadoA;
                    //Comparacion en caso de que subsidio causado en el año es mayor al ISR anual no se cobra ni se repone valor al empleado.
                    if (isrTgravadoA.ResultadoIsrOSubsidio < TsubcausadoA) { return item; }


                    resultadoA = isrTgravadoA.ResultadoIsrOSubsidio - TsubcausadoA;

                    resultadoA += TsaldoAnual;
                    resultadoA -= TisrpagadoA;

                    item.ResultadoIsrOSubsidio = resultadoA;
                }        



            }
            return item;
        }

        public static decimal[] VerificaSaldo(decimal isr, int idEjercicio, NOM_Nomina itemnomina, int idperiodo)
        {
            decimal[] dato =  new decimal[2];
            dato[0] = isr;

            using (var ctx = new RHEntities())
            {
                NominasDao.EliminarAplicacionSaldo(itemnomina.IdEmpleado, idperiodo);
                var item = ctx.NOM_CalculoAnual.Where(x => x.idEjercicio == idEjercicio && x.idEmpleado == itemnomina.IdEmpleado).FirstOrDefault();
                if (item != null && item.saldoFavor>0)
                {
                    var aplicado = ctx.NOM_AplicacionSaldo.Where(x => x.idEmpleado == itemnomina.IdEmpleado && x.idCalculo == item.id).Select(x=> x.ISRinicial).DefaultIfEmpty(0).Sum();

                    if (aplicado < item.saldoFavor)
                    {
                        decimal pagado;
                        decimal remanente=0;

                        dato[0] = item.saldoFavor - (aplicado + isr);
                        if (dato[0] < 0)
                        {
                            item.status = false;
                            pagado = isr + dato[0];
                            dato[0] = Math.Abs(dato[0]);                            
                        }
                        else
                        {                            
                            pagado = isr;
                            remanente = item.saldoFavor - (aplicado + isr);
                        }

                        NOM_AplicacionSaldo Osaldo = new NOM_AplicacionSaldo
                        {
                            idEmpleado = itemnomina.IdEmpleado,
                            idPeriodo = idperiodo,
                            idCalculo = item.id,
                            ISRinicial = isr,
                            ISRcobrado = dato[0]
                        };

                        NOM_Nomina_Detalle ODetalle = new NOM_Nomina_Detalle
                        {
                            Id = 0,
                            IdNomina = itemnomina.IdNomina,
                            IdConcepto = 146,
                            Total = pagado,
                            GravadoISR = 0,
                            ExentoISR = 0,
                            IntegraIMSS = 0,
                            ExentoIMSS = 0,
                            ImpuestoSobreNomina = 0,
                            Complemento = false,
                            IdPrestamo = 0,
                            SaldoFavor = item.saldoFavor,
                            Remanente = remanente
                        };

                        dato[1] = pagado;
                        ctx.NOM_AplicacionSaldo.Add(Osaldo);
                        ctx.NOM_Nomina_Detalle.Add(ODetalle);
                        ctx.SaveChanges();
                    }
                }


            }


            return dato;
        }
    }
}
