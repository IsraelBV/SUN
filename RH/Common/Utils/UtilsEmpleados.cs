using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class UtilsEmpleados
    {
        /// <summary>
        /// Recibe la descripción del turno y le asigna su identificador
        /// </summary>
        /// <param name="turnDescription"></param>
        /// <returns></returns>
        public static int SeleccionarTurno(string turnDescription)
        {
            switch (turnDescription)
            {
                case "Administrativo":
                    return 1;
                case "Matutino":
                    return 2;
                case "Vespertino":
                    return 3;
                case "Nocturno":
                    return 4;
                default:
                    return 1;
            }
        }
        

        /// <summary>
        /// Regresa la palabra correspondiente al número del turno
        /// </summary>
        /// <param name="Turno"></param>
        /// <returns></returns>
        public static string SeleccionarTurno(int? Turno)
        {
            switch (Turno)
            {
                case 1:
                    return "Administrativo";
                case 2:
                    return "Matutino";
                case 3:
                    return "Vespertino";
                case 4:
                    return "Nocturno";
                default:
                    return "Administrativo";
            }
        }

        /// <summary>
        /// Regresa la palabra correspondiente al número del día de la semana
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string selectDay(int? day)
        {
            switch (day)
            {
                case 1:
                    return "Lunes";
                case 2:
                    return "Martes";
                case 3:
                    return "Miércoles";
                case 4:
                    return "Jueves";
                case 5:
                    return "Viernes";
                case 6:
                    return "Sábado";
                case 7:
                    return "Domingo";
                default:
                    return "n/a";
            }
        }

        /// <summary>
        /// Regresa el número correspondiente al día de la semana
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int selectDay(string day)
        {
            switch (day)
            {
                case "Lunes":
                    return 1;
                case "Martes":
                    return 2;
                case "Miércoles":
                    return 3;
                case "Jueves":
                    return 4;
                case "Viernes":
                    return 5;
                case "Sábado":
                    return 6;
                case "Domingo":
                    return 0;
                default:
                    return 0;
            }
        }

        public static int SeleccionarTipoNomina(string desc_tipo_nom)
        {
            int tipoNomina = 1;
            switch (desc_tipo_nom)
            {
                case "Quincenal":
                    tipoNomina = 1;
                    break;
                case "Semanal":
                    tipoNomina = 2;
                    break;
                case "Mensual":
                    tipoNomina = 3;
                    break;
                case "Catorcenal":
                    tipoNomina = 4;
                    break;
                default:
                    tipoNomina = 1;
                    break;
            }
            return tipoNomina;
        }

        public static int SeleccionarTipoSemana(string desc_tipo_semana)
        {
            int tipoSemana = 1;
            switch (desc_tipo_semana)
            {
                case "Completa":
                    tipoSemana = 1;
                    break;
                case "1día":
                    tipoSemana = 2;
                    break;
                case "2 días":
                    tipoSemana = 3;
                    break;
                case "3 días":
                    tipoSemana = 4;
                    break;
                case "4 días":
                    tipoSemana = 5;
                    break;
                case "5 días":
                    tipoSemana = 6;
                    break;
                case "Menos de 8 horas":
                    tipoSemana = 7;
                    break;
                default:
                    tipoSemana = 1;
                    break;
            }
            return tipoSemana;
        }

        public static string SeleccionarTipoSemanaById(int desc_tipo_semana)
        {
            string tipoSemana = "Completa";
            switch (desc_tipo_semana)
            {
                case 1:
                    tipoSemana = "Completa";
                    break;
                case 2:
                    tipoSemana = "1 día";
                    break;
                case 3:
                    tipoSemana = "2 días";
                    break;
                case 4:
                    tipoSemana = "3 días";
                    break;
                case 5:
                    tipoSemana = "4 días";
                    break;
                case 6:
                    tipoSemana = "5 días";
                    break;
                case 7:
                    tipoSemana = "Menos de 8 horas";
                    break;
                default:
                    tipoSemana = "Completa";
                    break;
            }
            return tipoSemana;
        }

        public static int SeleccionarTipoSalario(string desc_tipo_salario)
        {
            int tipoSalario = 1;
            switch (desc_tipo_salario)
            {
                case "Fijo":
                    tipoSalario = 1;
                    break;
                //case "Mixto":
                //    tipoSalario = 2;
                //    break;
                //case "Variable":
                //    tipoSalario = 3;
                //    break;
                default:
                    tipoSalario = 1;
                    break;
            }
            return tipoSalario;
        }
        
        
        public static int SeleccionarFormaPago(string FormaPago)
        {
            if (FormaPago == null)
                return 3;
            switch (FormaPago.ToUpper())
            {
                case "EFECTIVO":
                    return 1;
                case "CHEQUE":
                    return 2;
                case "TRANSFERENCIA":
                    return 3;
                case "TARJETAS DE CRÉDITO":
                    return 4;
                case "MONEDEROS ELECTRÓNICOS":
                    return 5;
                case "DINERO ELECTRÓNICO":
                    return 6;
                case "TARJETAS DIGITALES":
                    return 7;
                case "VALES DE DESPENSA":
                    return 8;
                case "BIENES":
                    return 9;
                case "SERVICIO":
                    return 10;
                case "POR CUENTA DE TERCERO":
                    return 11;
                case "DACIÓN EN PAGO":
                    return 12;
                case "PAGO POR SUBROGACIÓN":
                    return 13;
                case "PAGO POR CONSIGNACIÓN":
                    return 14;
                case "CONDONACIÓN":
                    return 15;
                case "CANCELACIÓN":
                    return 16;
                case "COMPENSACIÓN":
                    return 17;
                case "NA":
                    return 18;
                case "OTROS":
                    return 19;
                default:
                    return 3;
            }
        }
        public static string SeleccionarFormaPagoById(int? FormaPago)
        {
            if (FormaPago == null)
                return "TRANSFERENCIA";
            switch (FormaPago)
            {
                case 1:
                    return "EFECTIVO";
                case 2:
                    return "CHEQUE";
                case 3:
                    return "TRANSFERENCIA";
                case 4:
                    return "TARJETAS DE CRÉDITO";
                case 5:
                    return "MONEDEROS ELECTRÓNICOS";
                case 6:
                    return "DINERO ELECTRÓNICO";
                case 7:
                    return "TARJETAS DIGITALES";
                case 8:
                    return "VALES DE DESPENSA";
                case 9:
                    return "BIENES";
                case 10:
                    return "SERVICIO";
                case 11:
                    return "POR CUENTA DE TERCERO";
                case 12:
                    return "DACIÓN EN PAGO";
                case 13:
                    return "PAGO POR SUBROGACIÓN";
                case 14:
                    return "PAGO POR CONSIGNACIÓN";
                case 15:
                    return "CONDONACIÓN";
                case 16:
                    return "CANCELACIÓN";
                case 17:
                    return "COMPENSACIÓN";
                case 18:
                    return "NA";
                case 19:
                    return "OTROS";
                default:
                    return "TRANSFERENCIA";
            }
        }
        public static string seleccionarDia(int? day)
        {
            switch (day)
            {
                case 1:
                    return "Lunes";
                case 2:
                    return "Martes";
                case 3:
                    return "Miércoles";
                case 4:
                    return "Jueves";
                case 5:
                    return "Viernes";
                case 6:
                    return "Sábado";
                case 0:
                    return "Domingo";
                default:
                    return "n/a";
            }
        }

        public static string SeleccionaTipoNomina(int tipo_nomina)
        {
            string tipoNomina;
            switch (tipo_nomina)
            {
                case 1:
                    tipoNomina = "Quincenal";
                    break;
                case 2:
                    tipoNomina = "Semanal";
                    break;
                case 3:
                    tipoNomina = "Mensual";
                    break;
                case 4:
                    tipoNomina = "Catorcenal";
                    break;
                default:
                    tipoNomina = "Quincenal";
                    break;
            }
            return tipoNomina;
        }
        public static string TipoDePago(int FormaPago)
        {
            switch (FormaPago)
            {
                case 1:
                    return "Tarjeta";
                case 2:
                    return "Cheque";
                case 3:
                    return "Efectivo";
                case 4:
                    return "SPEI";
                default:
                    return "Tarjeta";
            }
        }
        public static string TipoDePagoInfo(int FormaPago)
        {
            switch (FormaPago)
            {
                case 1:
                    return "Efectivo";
                case 2:
                    return "Cheque";
                case 3:
                    return "Transferencia";
                case 4:
                    return "Tarjetas de Crédito";
                default:
                    return "Tarjeta";
            }
        }

        public static int SeleccionarPeriodicidadDePago(string PPago)
        {
            if (PPago == null)
                return 4;
           
            switch (PPago.ToUpper())
            {
                case "DIARIO":
                    return 1;
                case "SEMANAL":
                    return 2;
                case "CATORCENAL":
                    return 3;
                case "QUINCENAL":
                    return 4;
                case "MENSUAL":
                    return 5;
                case "BIMESTRAL":
                    return 6;
                case "UNIDAD OBRA":
                    return 7;
                case "COMISION":
                    return 8;
                case "PRECIO ALZADA":
                    return 9;
                case "OTRA PERIODICIDAD":
                    return 10;
                case "FINIQUITO":
                    return 11;
                case "AGUINALDO":
                    return 12;
                case "DECENAL":
                    return 14;
                case "DOCENAL":
                    return 15;
                
                default:
                    return 4;
               
            }
        }
        public static string TipoSalario(int desc_tipo_salario)
        {
            string tipoSalario;
            switch (desc_tipo_salario)
            {
                case 1:
                    tipoSalario = "Fijo";
                    break;
                ////case 2:
                ////    tipoSalario = "Mixto";
                ////    break;
                ////case 3:
                ////    tipoSalario = "Variable";
                ////    break;
                default:
                    tipoSalario = "Fijo";
                    break;
            }
            return tipoSalario;
        }
        public static string GetNombreCompletoEmpleado(string Nombre, string Paterno, string Materno)
        {
            return Paterno + " " + Materno + " " + Nombre;
        }

        public static int ValorRiesgoPuesto(string riesgo)
        {
            if (string.IsNullOrEmpty(riesgo)) return 0;

            switch (riesgo.Trim().ToUpper())
            {
                case "I":
                    return 1;
                case "II":
                    return 2;
                case "III":
                    return 3;
                case "IV":
                    return 4;
                case "V":
                    return 5;
                default:
                    return 0;
            }
        }
        
    }
}
