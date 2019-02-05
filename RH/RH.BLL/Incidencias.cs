using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML;
using System.Xml;
using RH.Entidades;
using System.IO;

using Common.Utils;

using ClosedXML.Excel;
using MoreLinq;


namespace RH.BLL
{
    public class _Incidencias
    {
        RHEntities ctx = null;

        private int IdEmpleado { get; set; }
        private NOM_PeriodosPago Periodo { get; set; }
        private List<Incidencia> IncXEmpleado { get; set; }
        private int NumDias { get; set; }

        public _Incidencias()
        {
            ctx = new RHEntities();
        }

        public List<EmpleadoIncidencias> GetIncidenciasByPeriodo(NOM_PeriodosPago ppago, int[] idEmpleados = null)
        {
            int[] empleadosArray;

            //Generar Array de IdEmpleados
            if (idEmpleados == null)
            {
                //Obtiene el id de todos los empleados que estan asignado al periodo

                empleadosArray = ctx.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == ppago.IdPeriodoPago)
                    .Select(x => x.IdEmpleado)
                    .ToArray();
            }
            else
            {
                //id de empleados especificos
                empleadosArray = idEmpleados;
            }

            //var empleados = ctx.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == ppago.IdPeriodoPago).ToList();

            var lista = new List<EmpleadoIncidencias>();
            Periodo = ppago;
            Empleados emp = new Empleados();

            foreach (var item in empleadosArray)
            {
                IncXEmpleado = new List<Incidencia>();
                IdEmpleado = item;
                //   string nombreEmpleado = emp.GetNombreCompleto(IdEmpleado);
                NumDias = Periodo.DiasPeriodo;

                var empComp = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == item);

                EmpleadoIncidencias empIn = new EmpleadoIncidencias();
                empIn.IdEmpleado = IdEmpleado;

                //empIn.NombreEmpleado = nombreEmpleado;
                empIn.Paterno = empComp.APaterno;
                empIn.Materno = empComp.AMaterno;
                empIn.Nombres = empComp.Nombres;

                var fecha = Periodo.Fecha_Inicio;

                while (fecha <= Periodo.Fecha_Fin)
                {
                    var inc = new Incidencia
                    {
                        Fecha = fecha,
                        TipoIncidencia = "X",
                        SePaga = true
                    };

                    IncXEmpleado.Add(inc);

                    //incrementa la fecha en uno
                    fecha = fecha.AddDays(1);
                }

                //GetDiasFestivos();
                GetDiasDescanso();
                //GetVacaciones();
                //GetPermisos();
                //GetInasistencias();
                //GetIncapacidades();
                GetNuevoIngreso();
                GetBajas();

                empIn.Incidencias = IncXEmpleado;
                empIn.DiasAPagar = NumDias;
                empIn.idPeriodo = Periodo.IdPeriodoPago;
                lista.Add(empIn);
            }
            return lista;
        }
        public List<C_TiposInasistencia> catalagoIna()
        {
            var datos = ctx.C_TiposInasistencia.ToList();
            return datos;
        }
        #region GET INCIDENCIAS - NEW
        public List<EmpleadoIncidencias> GetIncidenciasByPeriodo2(NOM_PeriodosPago ppago, int[] idEmpleados = null)
        {
            int[] empleadosArray;

            //Generar Array de IdEmpleados
            if (idEmpleados == null)
            {
                //Obtiene el id de todos los empleados que estan asignado al periodo
                using (var context = new RHEntities())
                {
                    empleadosArray = context.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == ppago.IdPeriodoPago)
                    .Select(x => x.IdEmpleado)
                    .ToArray();
                }
            }
            else
            {
                //id de empleados especificos
                empleadosArray = idEmpleados;
            }



            var lista = new List<EmpleadoIncidencias>();
            Periodo = ppago;
            Empleados emp = new Empleados();


            //GET - Lista Empleados
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<C_DiasFestivos> listaDiasFestivos = new List<C_DiasFestivos>();
            List<Empleado_Contrato> listaContratos = new List<Empleado_Contrato>();
            List<Vacaciones> listaVacaciones = new List<Vacaciones>();
            List<PeriodoVacaciones> listaPeriodoVacaciones = new List<PeriodoVacaciones>();
            List<Permisos> listaPermisos = new List<Permisos>();
            List<Inasistencias> listaInasistenciasNominas = new List<Inasistencias>();
            List<Inasistencias> listaInasistenciasRH = new List<Inasistencias>();
            List<Incapacidades> listaIncapacidades = new List<Incapacidades>();

            using (var context = new RHEntities())
            {

                listaEmpleados = (from e in context.Empleado
                                  where empleadosArray.Contains(e.IdEmpleado)
                                  select e).ToList();

                listaContratos = (from c in context.Empleado_Contrato
                                  where empleadosArray.Contains(c.IdEmpleado)
                                  select c).ToList();

                // listaDiasFestivos = context.C_DiasFestivos.Where(x => x.Fecha >= Periodo.Fecha_Inicio && x.Fecha <= Periodo.Fecha_Fin).ToList();
                listaDiasFestivos = context.C_DiasFestivos.Where(x => Periodo.Fecha_Inicio >= x.Fecha && Periodo.Fecha_Inicio <= x.Fecha).ToList();



                //Lista Vacaciones
                listaVacaciones = (from v in context.Vacaciones
                                       //where ((v.FechaInicio >= ppago.Fecha_Inicio && v.FechaInicio <= ppago.Fecha_Fin) || (v.FechaFin >= ppago.Fecha_Inicio && v.FechaFin <= ppago.Fecha_Fin))
                                       // where ((ppago.Fecha_Inicio >= v.FechaInicio && ppago.Fecha_Inicio <= v.FechaFin) || (ppago.Fecha_Fin >= v.FechaInicio && ppago.Fecha_Fin <= v.FechaFin))
                                   where ((ppago.Fecha_Inicio >= v.FechaInicio && ppago.Fecha_Inicio <= v.FechaFin) || (ppago.Fecha_Fin >= v.FechaInicio))
                                   select v).ToList();

                //Lista Periodo vacaciones
                listaPeriodoVacaciones = (from pv in context.PeriodoVacaciones
                                              //  join c in listaContratos on pv.IdEmpleado_Contrato equals c.IdContrato
                                          select pv).ToList();


                //Lista Permisos
                listaPermisos = (from per in context.Permisos
                                 where empleadosArray.Contains(per.IdEmpleado)
                                 //&& ((per.FechaInicio >= ppago.Fecha_Inicio && per.FechaInicio <= ppago.Fecha_Fin) || (per.FechaFin >= ppago.Fecha_Inicio && per.FechaFin <= ppago.Fecha_Fin))
                                 //&& ((ppago.Fecha_Inicio >= per.FechaInicio && ppago.Fecha_Inicio <= per.FechaFin)||(ppago.Fecha_Fin >= per.FechaInicio && ppago.Fecha_Fin <= per.FechaFin))
                                 && ((ppago.Fecha_Inicio >= per.FechaInicio && ppago.Fecha_Inicio <= per.FechaFin) || (ppago.Fecha_Fin >= per.FechaInicio))
                                 select per).ToList();


                //ListaInasistencias

                listaInasistenciasNominas = (from inaNom in context.Inasistencias
                                             where empleadosArray.Contains(inaNom.IdEmpleado)
                                             && inaNom.idPeriodo == ppago.IdPeriodoPago
                                              //&& ((inaNom.Fecha >= ppago.Fecha_Inicio && inaNom.Fecha <= ppago.Fecha_Fin) || (inaNom.FechaFin >= ppago.Fecha_Inicio && inaNom.FechaFin <= ppago.Fecha_Fin))
                                              //&& ((ppago.Fecha_Inicio >= inaNom.Fecha && ppago.Fecha_Inicio <= inaNom.Fecha) || (ppago.Fecha_Fin >= inaNom.Fecha && ppago.Fecha_Fin <= inaNom.Fecha))
                                              && ((ppago.Fecha_Inicio >= inaNom.Fecha && ppago.Fecha_Inicio <= inaNom.Fecha) || (ppago.Fecha_Fin >= inaNom.Fecha))
                                             select inaNom).ToList();

                listaInasistenciasRH = (from inaRh in context.Inasistencias
                                        where empleadosArray.Contains(inaRh.IdEmpleado)
                                        && inaRh.idPeriodo == 0
                                        //&& ((inaRh.Fecha >= ppago.Fecha_Inicio && inaRh.Fecha <= ppago.Fecha_Fin) || (inaRh.FechaFin >= ppago.Fecha_Inicio && inaRh.FechaFin <= ppago.Fecha_Fin))
                                        //&& ((ppago.Fecha_Inicio >= inaRh.Fecha && ppago.Fecha_Inicio <= inaRh.Fecha)||(ppago.Fecha_Fin >= inaRh.Fecha && ppago.Fecha_Fin <= inaRh.Fecha))
                                        && ((ppago.Fecha_Inicio >= inaRh.Fecha && ppago.Fecha_Inicio <= inaRh.Fecha) || (ppago.Fecha_Fin >= inaRh.Fecha))
                                        select inaRh).ToList();

                listaIncapacidades = (from inca in context.Incapacidades
                                      where empleadosArray.Contains(inca.IdEmpleado)
                                      //&& ((inca.FechaInicio >= ppago.Fecha_Inicio && inca.FechaInicio <= ppago.Fecha_Fin) || (inca.FechaFin >= ppago.Fecha_Inicio && inca.FechaFin <= ppago.Fecha_Fin))
                                      //&& ((ppago.Fecha_Inicio >= inca.FechaInicio && ppago.Fecha_Inicio <= inca.FechaFin)||(ppago.Fecha_Fin >= inca.FechaInicio && ppago.Fecha_Fin <= inca.FechaFin))
                                      && ((ppago.Fecha_Inicio >= inca.FechaInicio && ppago.Fecha_Inicio <= inca.FechaFin) || (ppago.Fecha_Fin >= inca.FechaInicio))
                                      select inca).ToList();


            }

            foreach (var item in empleadosArray)
            {
                NumDias = Periodo.DiasPeriodo;
                IncXEmpleado = new List<Incidencia>();
                IdEmpleado = item;

                var empComp = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == item);
                //var empComp = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == item);

                EmpleadoIncidencias empIn = new EmpleadoIncidencias();
                empIn.IdEmpleado = IdEmpleado;

                //empIn.NombreEmpleado = nombreEmpleado;
                empIn.Paterno = empComp.APaterno;
                empIn.Materno = empComp.AMaterno;
                empIn.Nombres = empComp.Nombres;

                var fecha = Periodo.Fecha_Inicio;

                while (fecha <= Periodo.Fecha_Fin)
                {
                    var inc = new Incidencia
                    {
                        Fecha = fecha,
                        TipoIncidencia = "X",
                        SePaga = true
                    };

                    IncXEmpleado.Add(inc);

                    //incrementa la fecha en uno
                    fecha = fecha.AddDays(1);
                }

                GetDiasFestivos2(listaDiasFestivos, ppago);

                GetVacaciones2(listaVacaciones, listaPeriodoVacaciones, listaContratos, ppago, item);
                GetPermisos2(listaPermisos, item, ppago);
                GetInasistencias2Rh(listaInasistenciasRH, item, Periodo.DiasPeriodo, ppago);

                GetDiasDescanso2(listaContratos, item);

                GetIncapacidades2(listaIncapacidades, item, ppago);

                GetNuevoIngreso2(listaContratos, item);

                GetDiasDeBajaDelEmpleado(listaContratos, item);

                GetInasistencias2Nominas(listaInasistenciasNominas, item, Periodo.DiasPeriodo, ppago);

                empIn.Incidencias = IncXEmpleado;
                empIn.DiasAPagar = NumDias;
                empIn.idPeriodo = Periodo.IdPeriodoPago;
                lista.Add(empIn);
            }
            return lista;
        }
        #endregion

        private void GetDiasDeBajaDelEmpleado(List<Empleado_Contrato> listaContrato, int idEmpleado)
        {
            var contrato = listaContrato.Where(x => x.IdEmpleado == idEmpleado).OrderByDescending(s => s.IdContrato).FirstOrDefault();

            if (contrato != null)
            {
                if (contrato.FechaBaja != null)
                {

                    if (contrato.FechaBaja <= Periodo.Fecha_Inicio)
                    {
                        //Se marca todo como baja B 
                        foreach (var item in IncXEmpleado)
                        {
                            item.TipoIncidencia = "B";
                            item.SePaga = false;
                        }
                        NumDias = 0;
                    }
                    else
                    {
                        var fechaDeBaja = contrato.FechaBaja;
                        while (fechaDeBaja <= Periodo.Fecha_Fin)
                        {
                            var incidencia = IncXEmpleado.FirstOrDefault(x => x.Fecha == fechaDeBaja);
                            if (incidencia == null) continue;

                            incidencia.TipoIncidencia = "B";
                            if (incidencia.SePaga == true)
                            {
                                incidencia.SePaga = false;
                                NumDias--;
                            }

                            fechaDeBaja = fechaDeBaja.Value.AddDays(1);
                        }
                    }
                }
            }



            if (contrato.FechaAlta >= Periodo.Fecha_Inicio && contrato.FechaAlta <= Periodo.Fecha_Fin)
            {
                var fechaTemporal = Periodo.Fecha_Inicio;
                while (fechaTemporal < contrato.FechaAlta)
                {
                    var inc = IncXEmpleado.FirstOrDefault(x => x.Fecha == fechaTemporal);
                    if (inc == null) continue;

                    inc.TipoIncidencia = "NI";

                    if (inc.SePaga == true)
                    {
                        inc.SePaga = false;
                        NumDias--;
                    }
                    fechaTemporal = fechaTemporal.AddDays(1);
                }
            }
        }

        private void GetDiasFestivos2(List<C_DiasFestivos> lista, NOM_PeriodosPago periodoP)
        {

            foreach (var diaF in lista)
            {
                var dias = Utils.GetDiasByPeriodoRange(diaF.Fecha, diaF.Fecha, periodoP.Fecha_Inicio, periodoP.Fecha_Fin);
                if (dias > 0)
                    SetIncidencias(diaF.Fecha, diaF.Fecha, "DF", diaF.IdDiaFestivo, pagarDia: true);
            }
        }


        private void GetDiasDescanso()
        {
            Empleados emp = new Empleados();
            var contrato = emp.GetUltimoContrato(IdEmpleado);

            foreach (var item in IncXEmpleado)
            {
                if ((int)item.Fecha.DayOfWeek == contrato.DiaDescanso)
                {
                    item.TipoIncidencia = "D";
                }
            }
        }

        private void GetDiasDescanso2(List<Empleado_Contrato> listaContratos, int idEmpleado)
        {

            var contrato = listaContratos.Where(x => x.IdEmpleado == idEmpleado).OrderByDescending(s => s.IdContrato).FirstOrDefault();
            //emp.GetUltimoContrato(IdEmpleado);
            if (contrato == null) return;
            foreach (var item in IncXEmpleado)
            {
                if ((int)item.Fecha.DayOfWeek == contrato.DiaDescanso)
                {
                    item.TipoIncidencia = "D";

                    //La incidencia que tiene en este index del array
                    //no se paga entonces los cambiamos como dia de pago e incrementamos a uno los dias
                    //Si ya lo tiene como dia de pago no se hace nada
                    if (item.SePaga != false) continue;
                    item.SePaga = true;
                    NumDias += 1;
                }
            }
        }


        private void GetNuevoIngreso()
        {
            var emp = new Empleados();
            var contrato = emp.GetUltimoContrato(IdEmpleado);
            if (contrato.FechaAlta >= Periodo.Fecha_Inicio && contrato.FechaAlta <= Periodo.Fecha_Fin)
            {
                var fechaTemporal = Periodo.Fecha_Inicio;
                while (fechaTemporal < contrato.FechaAlta)
                {
                    var inc = IncXEmpleado.Where(x => x.Fecha == fechaTemporal).FirstOrDefault();
                    NumDias--;
                    inc.TipoIncidencia = "NI";
                    fechaTemporal = fechaTemporal.AddDays(1);
                }
            }
        }

        private void GetNuevoIngreso2(List<Empleado_Contrato> listaC, int idEmpleado)
        {
            var contrato = listaC.Where(x => x.IdEmpleado == idEmpleado).OrderByDescending(s => s.IdContrato).FirstOrDefault();


            if (contrato.FechaAlta >= Periodo.Fecha_Inicio && contrato.FechaAlta <= Periodo.Fecha_Fin)
            {
                var fechaTemporal = Periodo.Fecha_Inicio;
                while (fechaTemporal < contrato.FechaAlta)
                {
                    var inc = IncXEmpleado.FirstOrDefault(x => x.Fecha == fechaTemporal);
                    if (inc == null) continue;

                    inc.TipoIncidencia = "NI";

                    if (inc.SePaga == true)
                    {
                        inc.SePaga = false;
                        NumDias--;
                    }



                    fechaTemporal = fechaTemporal.AddDays(1);
                }
            }
        }

        private void GetBajas()
        {
            var emp = new Empleados();
            var contrato = emp.GetUltimoContrato(IdEmpleado);
            if (contrato.FechaBaja >= Periodo.Fecha_Inicio && contrato.FechaBaja <= Periodo.Fecha_Fin)
            {
                while (contrato.FechaBaja <= Periodo.Fecha_Fin)
                {
                    NumDias--;
                    var inc = IncXEmpleado.Where(x => x.Fecha == contrato.FechaBaja).FirstOrDefault();
                    inc.TipoIncidencia = "B";
                    contrato.FechaBaja = contrato.FechaBaja.Value.AddDays(1);
                }
            }
        }

        private void GetBajas2(List<Empleado_Contrato> listaC, int idEmpleado)
        {
            var contrato = listaC.Where(x => x.IdEmpleado == idEmpleado).OrderByDescending(s => s.IdContrato).FirstOrDefault();

            if (contrato.FechaBaja >= Periodo.Fecha_Inicio && contrato.FechaBaja <= Periodo.Fecha_Fin)
            {
                while (contrato.FechaBaja <= Periodo.Fecha_Fin)
                {
                    var inc = IncXEmpleado.FirstOrDefault(x => x.Fecha == contrato.FechaBaja);
                    inc.TipoIncidencia = "B";

                    if (inc.SePaga == true)
                    {
                        inc.SePaga = false;
                        NumDias--;
                    }

                    contrato.FechaBaja = contrato.FechaBaja.Value.AddDays(1);
                }
            }
        }


        //private void GetVacaciones()
        //{
        //    var vacaciones = (from v in ctx.Vacaciones
        //                      join pv in ctx.PeriodoVacaciones on v.IdPeridoVacaciones equals pv.IdPeridoVacaciones
        //                      join con in ctx.Empleado_Contrato on pv.IdEmpleado_Contrato equals con.IdContrato
        //                      where con.IdEmpleado == IdEmpleado &&
        //                      ((v.FechaInicio >= Periodo.Fecha_Inicio && v.FechaInicio <= Periodo.Fecha_Fin) || (v.FechaFin >= Periodo.Fecha_Inicio && v.FechaFin <= Periodo.Fecha_Fin))
        //                      select v
        //            ).ToList();


        //    foreach (var vacacion in vacaciones)
        //    {
        //        SetIncidencias(vacacion.FechaInicio, vacacion.FechaFin, "V", vacacion.Id);
        //    }
        //}

        private void GetVacaciones2(List<Vacaciones> listaV, List<PeriodoVacaciones> listaPV, List<Empleado_Contrato> listaC, NOM_PeriodosPago periodoP, int idEmpleado)
        {

            var listaVac = (from v in listaV
                            join pv in listaPV on v.IdPeridoVacaciones equals pv.IdPeridoVacaciones
                            join con in listaC on pv.IdEmpleado_Contrato equals con.IdContrato
                            where con.IdEmpleado == idEmpleado &&
                           //((v.FechaInicio >= periodoP.Fecha_Inicio && v.FechaInicio <= periodoP.Fecha_Fin) || (v.FechaFin >= periodoP.Fecha_Inicio && v.FechaFin <= periodoP.Fecha_Fin))
                           //((periodoP.Fecha_Inicio >= v.FechaInicio && periodoP.Fecha_Inicio <= v.FechaFin)||(periodoP.Fecha_Fin >= v.FechaInicio && periodoP.Fecha_Fin <= v.FechaFin))
                           ((periodoP.Fecha_Inicio >= v.FechaInicio && periodoP.Fecha_Inicio <= v.FechaFin) || (periodoP.Fecha_Fin >= v.FechaInicio))
                            select v).ToList();


            foreach (var vacacion in listaVac)
            {

                var dias = Utils.GetDiasByPeriodoRange(vacacion.FechaInicio, vacacion.FechaFin, periodoP.Fecha_Inicio, periodoP.Fecha_Fin);

                if (dias > 0)
                    SetIncidencias(vacacion.FechaInicio, vacacion.FechaFin, "V", vacacion.Id, pagarDia: true);
            }
        }

        //private void GetInasistencias()
        //{
        //    var inas = ctx.Inasistencias.Where(x => x.IdEmpleado == IdEmpleado && x.idPeriodo == Periodo.IdPeriodoPago && (
        //   (x.Fecha >= Periodo.Fecha_Inicio && x.Fecha <= Periodo.Fecha_Fin) || (x.FechaFin >= Periodo.Fecha_Inicio && x.FechaFin <= Periodo.Fecha_Fin)
        //   )).ToList();

        //    var inas2 = ctx.Inasistencias.Where(x => x.IdEmpleado == IdEmpleado && x.idPeriodo == 0 && (
        //    (x.Fecha >= Periodo.Fecha_Inicio && x.Fecha <= Periodo.Fecha_Fin) || (x.FechaFin >= Periodo.Fecha_Inicio && x.FechaFin <= Periodo.Fecha_Fin)
        //    )).ToList();

        //    inas.AddRange(inas2);

        //    foreach (var inasistencia in inas)
        //    {
        //        var tipoInasistencia = ctx.C_TiposInasistencia.FirstOrDefault(x => x.IdTipoInasistencia == inasistencia.IdTipoInasistencia);
        //        if (!tipoInasistencia.DerechoPago) NumDias = NumDias - inasistencia.Dias;
        //        SetIncidencias(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, tipoInasistencia.Clave, inasistencia.IdInasistencia);
        //    }
        //}

        private void GetInasistencias2Rh(List<Inasistencias> listaInasistenciasRH, int idEmpleado, int diasDelPeriodo, NOM_PeriodosPago periodoP)
        {
            var inas = listaInasistenciasRH.Where(x => x.IdEmpleado == idEmpleado).ToList();

            foreach (var inasistencia in inas)
            {
                var tipoInasistencia = ctx.C_TiposInasistencia.FirstOrDefault(x => x.IdTipoInasistencia == inasistencia.IdTipoInasistencia);

                if (!tipoInasistencia.DerechoPago)
                {
                    if (inasistencia.FechaFin != null)
                    {
                        var dias = Utils.GetDiasByPeriodoRange(inasistencia.Fecha, inasistencia.FechaFin.Value.Date,
                            periodoP.Fecha_Inicio, periodoP.Fecha_Fin);

                        if (SetIncidenciasInasistencia(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, tipoInasistencia.Clave, inasistencia.IdInasistencia, derechoPago: false))
                        {
                            if (dias > 0)
                                NumDias = NumDias - inasistencia.Dias;
                        }
                        //if (dias > 0)
                        //{
                        //    NumDias = NumDias - inasistencia.Dias;
                        //    SetIncidencias(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, tipoInasistencia.Clave, inasistencia.IdInasistencia, pagarDia: false);
                        //}
                    }
                }
                else
                {
                    //Valida si esa fecha ya la tiene como asistencia
                    // si lo tiene como falta lo cambia a asistencia e incrementa los dias

                    if (SetIncidenciasInasistencia(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, tipoInasistencia.Clave, inasistencia.IdInasistencia, derechoPago: true))
                    {
                        NumDias = NumDias + inasistencia.Dias;
                    }
                }
            }

            //Esto es para el caso que si RH agrego como faltas
            //Pero nomina agrega las mismas fechas pero como asistencias
            if (NumDias > diasDelPeriodo)
            {
                NumDias = diasDelPeriodo;
            }

        }

        private void GetInasistencias2Nominas(List<Inasistencias> listaInasistenciasNomina, int idEmpleado, int diasDelPeriodo, NOM_PeriodosPago periodoP)
        {
            var inas = listaInasistenciasNomina.Where(x => x.IdEmpleado == idEmpleado).ToList();

            foreach (var inasistencia in inas)
            {
                var tipoInasistencia = ctx.C_TiposInasistencia.FirstOrDefault(x => x.IdTipoInasistencia == inasistencia.IdTipoInasistencia);

                if (!tipoInasistencia.DerechoPago)
                {
                    if (inasistencia.FechaFin != null)
                    {
                        var dias = Utils.GetDiasByPeriodoRange(inasistencia.Fecha, inasistencia.FechaFin.Value.Date,
                            periodoP.Fecha_Inicio, periodoP.Fecha_Fin);

                        //if (dias > 0)
                        //{
                        // NumDias = NumDias - inasistencia.Dias;
                        //SetIncidencias(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, tipoInasistencia.Clave, inasistencia.IdInasistencia, pagarDia: false);
                        if (SetIncidenciasInasistencia(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, tipoInasistencia.Clave, inasistencia.IdInasistencia, derechoPago: false))
                        {
                            if (dias > 0)
                                NumDias = NumDias - inasistencia.Dias;
                        }
                        // }
                    }
                }
                else
                {
                    //Valida si esa fecha ya la tiene como asistencia
                    // si lo tiene como falta lo cambia a asistencia e incrementa los dias

                    if (SetIncidenciasInasistencia(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, tipoInasistencia.Clave, inasistencia.IdInasistencia, derechoPago: true))
                    {
                        NumDias = NumDias + inasistencia.Dias;
                    }
                }


            }

            //Esto es para el caso que si RH agrego como faltas
            //Pero nomina agrega las mismas fechas pero como asistencias
            if (NumDias > diasDelPeriodo)
            {
                NumDias = diasDelPeriodo;
            }

        }


        //private void GetPermisos()
        //{
        //    var permisos = ctx.Permisos.Where(x => x.IdEmpleado == IdEmpleado &&
        //    ((x.FechaInicio >= Periodo.Fecha_Inicio && x.FechaInicio <= Periodo.Fecha_Fin) || (x.FechaFin >= Periodo.Fecha_Inicio && x.FechaFin <= Periodo.Fecha_Fin))
        //    ).ToList();

        //    foreach (var permiso in permisos)
        //    {
        //        var tipo = "PC";
        //        if (permiso.ConGoce == false)
        //        {
        //            tipo = "PS";
        //            NumDias = NumDias - (int)permiso.Dias;
        //        }
        //        SetIncidencias(permiso.FechaInicio, permiso.FechaFin, tipo, permiso.IdPermiso);
        //    }
        //}

        private void GetPermisos2(List<Permisos> listaPermisos, int idEmpl, NOM_PeriodosPago periodosPago)
        {
            var fechaInicioPeriodo = periodosPago.Fecha_Inicio;
            var fechaFinPeriodo = periodosPago.Fecha_Fin;
            bool pagarPermiso = false;
            var permisos = listaPermisos.Where(x => x.IdEmpleado == idEmpl).ToList();

            foreach (var permiso in permisos)
            {
                var tipo = "PC";

                if (permiso.ConGoce == false)
                {
                    var fechaInicioPermiso = permiso.FechaInicio;
                    var fechaFinPermiso = permiso.FechaFin;

                    var diasPermisos = Utils.GetDiasByPeriodoRange(fechaInicioPermiso, fechaFinPermiso,
                        fechaInicioPeriodo, fechaFinPeriodo);
                    NumDias -= diasPermisos;

                    tipo = "PS";
                    // NumDias = NumDias - (int)permiso.Dias;
                    pagarPermiso = false;
                }
                else
                {
                    pagarPermiso = true;
                }


                SetIncidencias(permiso.FechaInicio, permiso.FechaFin, tipo, permiso.IdPermiso, pagarDia: pagarPermiso);
            }
        }

        //private void GetIncapacidades()
        //{
        //    var incapacidades = ctx.Incapacidades.Where(x => x.IdEmpleado == IdEmpleado &&
        //        ((x.FechaInicio >= Periodo.Fecha_Inicio && x.FechaInicio <= Periodo.Fecha_Fin) || (x.FechaFin >= Periodo.Fecha_Inicio && x.FechaFin <= Periodo.Fecha_Fin))
        //    ).ToList();


        //    foreach (var incapacidad in incapacidades)
        //    {
        //        if (incapacidad.IdIncapacidadesSat == 1)
        //        {
        //            SetIncidencias(incapacidad.FechaInicio, incapacidad.FechaFin, "IR", incapacidad.Id, isIncapaciodad: true);
        //        }
        //        else if (incapacidad.IdIncapacidadesSat == 2)
        //        {
        //            SetIncidencias(incapacidad.FechaInicio, incapacidad.FechaFin, "IE", incapacidad.Id, isIncapaciodad: true);
        //        }
        //        else
        //        {
        //            SetIncidencias(incapacidad.FechaInicio, incapacidad.FechaFin, "IM", incapacidad.Id, isIncapaciodad: true);
        //        }

        //    }
        //}

        private void GetIncapacidades2(List<Incapacidades> listaIncapacidades, int idEmpleado, NOM_PeriodosPago periodosPago)
        {
            var incapacidades = listaIncapacidades.Where(x => x.IdEmpleado == idEmpleado).ToList();

            foreach (var incapacidad in incapacidades)
            {
                if (incapacidad.IdIncapacidadesSat == 1)
                {
                    var dias = Utils.GetDiasByPeriodoRange(incapacidad.FechaInicio, incapacidad.FechaFin, periodosPago.Fecha_Inicio, periodosPago.Fecha_Fin);

                    if (dias > 0)
                    {
                        NumDias -= dias;

                        SetIncidencias(incapacidad.FechaInicio, incapacidad.FechaFin, "IR", incapacidad.Id, isIncapaciodad: true, pagarDia: false);
                    }
                }
                else if (incapacidad.IdIncapacidadesSat == 2)
                {
                    var dias = Utils.GetDiasByPeriodoRange(incapacidad.FechaInicio, incapacidad.FechaFin, periodosPago.Fecha_Inicio, periodosPago.Fecha_Fin);

                    if (dias > 0)
                    {
                        NumDias -= dias;
                        SetIncidencias(incapacidad.FechaInicio, incapacidad.FechaFin, "IE", incapacidad.Id, isIncapaciodad: true, pagarDia: false);
                    }
                }
                else
                {
                    var dias = Utils.GetDiasByPeriodoRange(incapacidad.FechaInicio, incapacidad.FechaFin, periodosPago.Fecha_Inicio, periodosPago.Fecha_Fin);
                    if (dias > 0)
                    {
                        NumDias -= dias;
                        SetIncidencias(incapacidad.FechaInicio, incapacidad.FechaFin, "IM", incapacidad.Id, isIncapaciodad: true, pagarDia: false);
                    }
                }

            }
        }

        private void SetIncidencias(DateTime FechaIncidencia, DateTime FechaFinIncidencia, string TipoIncidencia, int IdIncidencia, bool pagarDia, bool isIncapaciodad = false)
        {
            bool incrementar = false;

            if (FechaIncidencia < Periodo.Fecha_Inicio)
                FechaIncidencia = Periodo.Fecha_Inicio;

            if (FechaFinIncidencia > Periodo.Fecha_Fin)
                FechaFinIncidencia = Periodo.Fecha_Fin;

            while (FechaIncidencia <= FechaFinIncidencia)
            {
                var incidencia = IncXEmpleado.FirstOrDefault(x => x.Fecha == FechaIncidencia);

                if (incidencia == null) continue;


                incidencia.TipoIncidencia = TipoIncidencia;
                incidencia.IdIncidencia = IdIncidencia;
                incidencia.IsIncapacidad = isIncapaciodad;
                incidencia.SePaga = pagarDia;



                FechaIncidencia = FechaIncidencia.AddDays(1);

            }
        }

        //Usado los para saber si en asistencia tiene que incrementar o no los dias de pago
        private bool SetIncidenciasInasistencia(DateTime FechaIncidencia, DateTime FechaFinIncidencia, string TipoIncidencia, int IdIncidencia, bool derechoPago, bool isIncapaciodad = false)
        {
            bool incrementar = false;


            if (FechaIncidencia < Periodo.Fecha_Inicio)
                FechaIncidencia = Periodo.Fecha_Inicio;

            if (FechaFinIncidencia > Periodo.Fecha_Fin)
                FechaFinIncidencia = Periodo.Fecha_Fin;


            while (FechaIncidencia <= FechaFinIncidencia)
            {
                var incidencia = IncXEmpleado.FirstOrDefault(x => x.Fecha == FechaIncidencia);

                if (!derechoPago)
                {//si antes tenia una de estas incidencias en entonces al pasarlo como asistencia se debe incrementar el dia
                    if (incidencia.TipoIncidencia == "X" || incidencia.TipoIncidencia == "V" || incidencia.TipoIncidencia == "IR" || incidencia.TipoIncidencia == "IE" ||
                        incidencia.TipoIncidencia == "IM" || incidencia.TipoIncidencia == "NI" || incidencia.TipoIncidencia == "PC" || incidencia.TipoIncidencia == "DF" ||
                        incidencia.TipoIncidencia == "HE" || incidencia.TipoIncidencia == "H3" || incidencia.TipoIncidencia == "D")
                    {
                        incrementar = true;//INCREMENTAR LAS FALTAS

                    }
                }
                else
                {
                    //si antes tenia una de estas incidencias en entonces al pasarlo como asistencia se debe incrementar el dia
                    if (incidencia.TipoIncidencia == "B" || incidencia.TipoIncidencia == "FI" ||
                        incidencia.TipoIncidencia == "FA" || incidencia.TipoIncidencia == "PS" ||
                        incidencia.TipoIncidencia == "FJ")
                    {
                        incrementar = true;//INCREMENTAR LAS ASISTENCIAS

                    }
                }


                incidencia.TipoIncidencia = TipoIncidencia;
                incidencia.IdIncidencia = IdIncidencia;
                incidencia.IsIncapacidad = isIncapaciodad;
                incidencia.SePaga = derechoPago;

                FechaIncidencia = FechaIncidencia.AddDays(1);
            }

            return incrementar;
        }




        public void cambiarIncidencias(string[] array, string tipo, int idPeriodo, int IdUser)
        {

            //Validar periodo
            var itemPeriodo = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

            if (itemPeriodo == null) return;

            //Si el periodo ya esta autorizado, ya no se podrá hacer cambios
            if (itemPeriodo.Autorizado == true) return;

            if (array == null || idPeriodo <= 0) return;
            int idTipo = 0;
            foreach (var ar in array)
            {
                string[] newarray = ar.Split(',');
                switch (tipo)
                {
                    case "x":
                        idTipo = 1;
                        break;
                    case "v":
                        idTipo = 2;
                        break;
                    case "ir":
                        idTipo = 3;
                        break;
                    case "ie":
                        idTipo = 4;
                        break;
                    case "im":
                        idTipo = 5;
                        break;
                    case "b":
                        idTipo = 7;
                        break;
                    case "fi":
                        idTipo = 8;
                        break;
                    case "fa":
                        idTipo = 9;
                        break;
                    case "ps":
                        idTipo = 10;
                        break;
                    case "pc":
                        idTipo = 11;
                        break;
                    case "d":
                        idTipo = 15;
                        break;
                    case "fj":
                        idTipo = 16;
                        break;
                    default:
                        idTipo = 1;
                        break;


                }
                if (newarray[0] == "0")
                {
                    int idEmpleado = Convert.ToInt32(newarray[1]);
                    var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == idEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();
                    var id = contrato.IdContrato;

                    var idEmpresaF = 0;

                    if (contrato.IdEmpresaFiscal != null)
                    {
                        if (contrato.IdEmpresaFiscal != 0)
                        {
                            idEmpresaF = contrato.IdEmpresaFiscal.Value;
                        }
                    }

                    var itemNuevo = new Inasistencias
                    {
                        IdEmpleado = Convert.ToInt32(newarray[1]),
                        IdTipoInasistencia = idTipo,
                        Fecha = Convert.ToDateTime(newarray[2]),
                        FechaFin = Convert.ToDateTime(newarray[2]),
                        Dias = 1,
                        xNomina = true,
                        idPeriodo = idPeriodo,
                        IdContrato = contrato.IdContrato,
                        IdEmpresaFiscal = idEmpresaF,
                        IdUsuario = IdUser,
                        FechaReg = DateTime.Now
                    };
                    ctx.Inasistencias.Add(itemNuevo);
                    var t = ctx.SaveChanges();
                }
                else
                {
                    const string sqlQuery = "DELETE Inasistencias WHERE IdInasistencia = @p0";
                    ctx.Database.ExecuteSqlCommand(sqlQuery, newarray[0]);
                    int idEmpleado = Convert.ToInt32(newarray[1]);
                    var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == idEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();

                    var idEmpresaF = 0;

                    if (contrato.IdEmpresaFiscal != null)
                    {
                        if (contrato.IdEmpresaFiscal != 0)
                        {
                            idEmpresaF = contrato.IdEmpresaFiscal.Value;
                        }
                    }

                    var itemNuevo = new Inasistencias
                    {
                        IdEmpleado = Convert.ToInt32(newarray[1]),
                        IdTipoInasistencia = idTipo,
                        Fecha = Convert.ToDateTime(newarray[2]),
                        FechaFin = Convert.ToDateTime(newarray[2]),
                        Dias = 1,
                        xNomina = true,
                        idPeriodo = idPeriodo,
                        IdContrato = contrato.IdContrato,
                        IdEmpresaFiscal = idEmpresaF,
                        IdUsuario = IdUser,
                        FechaReg = DateTime.Now
                    };
                    ctx.Inasistencias.Add(itemNuevo);
                    var t = ctx.SaveChanges();
                }
            }
        }


    }

    public class Incidencia
    {
        public DateTime Fecha { get; set; }
        public string TipoIncidencia { get; set; }
        public int IdIncidencia { get; set; }
        public bool IsIncapacidad { get; set; }
        public bool SePaga { get; set; }
    }
    public class EmpleadoIncidencias
    {
        public List<Incidencia> Incidencias { get; set; }
        public int IdEmpleado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public string NombreEmpleado { get; set; }
        public int DiasAPagar { get; set; }
        public int idPeriodo { get; set; }
    }
}
