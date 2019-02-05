using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;
using RH.BLL;
using RH.Entidades;

namespace RH.BLL
{
    public class _Inasistencias
    {
        RHEntities ctx = null;
        public _Inasistencias()
        {
            ctx = new RHEntities();
        }

        public bool DeleteInasistencia(int IdInasistencia)
        {
            var inasistencia = ctx.Inasistencias.FirstOrDefault(x => x.IdInasistencia == IdInasistencia);
            ctx.Inasistencias.Remove(inasistencia);
            var result = ctx.SaveChanges();
            return result > 0 ? true : false;
        }

        public List<Inasistencias> GetDetail(int IdE)
        {
            var lista = ctx.Inasistencias.Where(x => x.IdEmpleado == IdE && x.idPeriodo == 0).ToList();
            var lista2 = ctx.Inasistencias.Where(x => x.IdEmpleado == IdE && x.idPeriodo > 0 && x.xNomina == true).ToList();

            lista.AddRange(lista2);

            // return ctx.Inasistencias.Where(x => x.IdEmpleado == IdEmpleado).ToList();
            return lista;
        }

        public bool NuevaInasistencia(Inasistencias model)
        {
            ctx.Inasistencias.Add(model);
            var result = ctx.SaveChanges();
            return result > 0 ? true : false;
        }

        public List<IncidenciasDuplicadas> CapturaMasiva(List<Inasistencias> lista)
        {
            var listaDuplicados = new List<IncidenciasDuplicadas>();
            foreach (var item in lista)
            {
                var duplicado = RevisarDuplicados(item.IdEmpleado, item.Fecha, item.FechaFin);
                if (duplicado == null)
                    ctx.Inasistencias.Add(item);
                else
                {
                    duplicado.IdEmpleado = item.IdEmpleado;
                    duplicado.FechaIncidencia = item.Fecha;
                    duplicado.FechaFinIncidencia = item.FechaFin;
                    duplicado.Dias = item.Dias;
                    duplicado.IdTipoInasistencia = item.IdTipoInasistencia;
                    listaDuplicados.Add(duplicado);
                }
            }
            ctx.SaveChanges();
            return listaDuplicados;
        }


        public List<IncidenciasDuplicadas> GuardarInasistencias(int[] empleados, string[] fechas, int tipoInasistencias,int IdUser)
        {
            var lista = new List<Inasistencias>();
            var listaDuplicados = new List<IncidenciasDuplicadas>();
            int cont = 0;
            DateTime date = DateTime.Now;
            if (empleados == null || fechas == null) return null;

            foreach (var itemEmpleado in empleados)
            {
                var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == itemEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault();

                if(contrato == null) continue;

                foreach (var itemFecha in fechas)
                {
                    var fechaInasistencia = DateTime.Parse(itemFecha.ToDiaMesAño());

                    //validamos que la fecha a registrar no exista para ese colaborador
                    var duplicado = RevisarDuplicados(itemEmpleado, fechaInasistencia, fechaInasistencia);
                    if (duplicado != null)
                    {
                        duplicado.IdEmpleado = itemEmpleado;
                        duplicado.FechaIncidencia = fechaInasistencia;
                        duplicado.FechaFinIncidencia = fechaInasistencia;
                        duplicado.Dias = 1;
                        duplicado.IdTipoInasistencia = tipoInasistencias;
                        
                        listaDuplicados.Add(duplicado);
                    }
                    else
                    {
                        var idEmpresaF = 0;

                        if (contrato.IdEmpresaFiscal != null)
                        {
                            if (contrato.IdEmpresaFiscal != 0)
                            {
                                idEmpresaF = contrato.IdEmpresaFiscal.Value;
                            }
                        }

                        var inasistencia = new Inasistencias()
                        {
                            IdInasistencia = 0,
                            IdEmpleado = itemEmpleado,
                            Dias = 1,
                            Fecha = fechaInasistencia,
                            FechaFin = fechaInasistencia,
                            IdTipoInasistencia = tipoInasistencias,
                            IdContrato = contrato.IdContrato,
                            IdEmpresaFiscal = idEmpresaF,
                            IdUsuario = IdUser,
                            FechaReg = DateTime.Now
                    };

                        ctx.Inasistencias.Add(inasistencia);
                        cont++;
                    }
                }
            }

            if (cont > 0)
                ctx.SaveChanges();

            return listaDuplicados;

        }


        public int CapturaMasivaSinComprobasion(List<Inasistencias> lista)
        {
            ctx.Inasistencias.AddRange(lista);
            return ctx.SaveChanges();
        }
        public IncidenciasDuplicadas RevisarDuplicados(int IdEmpleado, DateTime Fecha, DateTime? FechaFinal)
        {
            if (FechaFinal == null)
                FechaFinal = Fecha;


            var duplicado = new IncidenciasDuplicadas();
            var emp = new Empleados();
            var contrato = emp.GetUltimoContrato(IdEmpleado);
            //if (contrato.FechaAlta > Fecha)
            if (contrato.FechaReal > Fecha)
            {
                duplicado.Incidencia = "Nuevo Ingreso";
                //duplicado = DateTimeToString(contrato.FechaAlta, contrato.FechaAlta, duplicado);
                duplicado = DateTimeToString(contrato.FechaReal, contrato.FechaReal, duplicado);
                duplicado = SetEmpleado(IdEmpleado, duplicado);
                return duplicado;
            }

            if (contrato.FechaBaja > FechaFinal)
            {
                duplicado.Incidencia = "Baja";
                duplicado = DateTimeToString(contrato.FechaBaja.Value.Date, contrato.FechaBaja.Value.Date, duplicado);
                duplicado = SetEmpleado(IdEmpleado, duplicado);
            }

            var permisos = ctx.Permisos.Where(x => x.IdEmpleado == IdEmpleado && ((x.FechaInicio <= Fecha && x.FechaFin >= Fecha) || (x.FechaInicio <= FechaFinal && x.FechaFin >= FechaFinal))).FirstOrDefault();
            if (permisos != null)
            {
                duplicado.Incidencia = "Permiso";
                duplicado = DateTimeToString(permisos.FechaInicio, permisos.FechaFin, duplicado);
                duplicado = SetEmpleado(IdEmpleado, duplicado);
                return duplicado;
            }

            var incapacidades = ctx.Incapacidades.Where(x => x.IdEmpleado == IdEmpleado && ((x.FechaInicio <= Fecha && x.FechaFin >= Fecha) || (x.FechaInicio <= FechaFinal && x.FechaFin >= FechaFinal))).FirstOrDefault();
            if (incapacidades != null)
            {
                duplicado.Incidencia = "Incapacidad";
                duplicado = DateTimeToString(incapacidades.FechaInicio, incapacidades.FechaFin, duplicado);
                duplicado = SetEmpleado(IdEmpleado, duplicado);
                return duplicado;
            }

            var inasistencia = ctx.Inasistencias.Where(x => x.IdEmpleado == IdEmpleado && (
                (x.Fecha <= Fecha && x.Fecha <= FechaFinal) && (x.FechaFin >= Fecha && x.FechaFin >= FechaFinal)
            )).FirstOrDefault();
            if (inasistencia != null)
            {
                duplicado.Incidencia = "Inasistencia";
                if (inasistencia.FechaFin != null)
                {
                    duplicado = DateTimeToString(inasistencia.Fecha, inasistencia.FechaFin.Value.Date, duplicado);
                }
                else
                {
                    duplicado.Fecha = inasistencia.Fecha.ToString(@"dd \de MMMM");
                }
                duplicado = SetEmpleado(IdEmpleado, duplicado);
                return duplicado;
            }

            var vacacion = (from v in ctx.Vacaciones
                            join pv in ctx.PeriodoVacaciones on v.IdPeridoVacaciones equals pv.IdPeridoVacaciones
                            join c in ctx.Empleado_Contrato on pv.IdEmpleado_Contrato equals c.IdContrato
                            where c.IdEmpleado == IdEmpleado && ((v.FechaInicio <= Fecha && v.FechaFin >= Fecha) || (v.FechaInicio <= FechaFinal && v.FechaFin >= FechaFinal))
                            select v
                            ).FirstOrDefault();
            if (vacacion != null)
            {
                duplicado.Incidencia = "Vacaciones";
                duplicado = DateTimeToString(vacacion.FechaInicio, vacacion.FechaFin, duplicado);
                duplicado = SetEmpleado(IdEmpleado, duplicado);
                return duplicado;
            }

            //var diaFestivo = ctx.DiasFestivos.Where(x => x.Fecha >= Fecha && x.Fecha <= FechaFinal).FirstOrDefault();
            //if(diaFestivo != null)
            //{
            //    duplicado.Incidencia = "Día Festivo";
            //    duplicado.Fecha = diaFestivo.Fecha.ToString(@"dd \de MMMM");
            //    duplicado = SetEmpleado(IdEmpleado, duplicado);
            //    return duplicado;
            //}

            //var diaDescanso = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == IdEmpleado).OrderByDescending(x => x.IdContrato).Select(x => x.DiaDescanso).FirstOrDefault();
            //var fechaTemporal = Fecha;
            //while(fechaTemporal <= FechaFinal)
            //{
            //    if((int)fechaTemporal.DayOfWeek == diaDescanso)
            //    {
            //        duplicado.Incidencia = "Día de Descanso";
            //        duplicado.Fecha = fechaTemporal.ToString(@"dd \de MMMM");
            //        duplicado = SetEmpleado(IdEmpleado, duplicado);
            //        return duplicado;
            //    }
            //    fechaTemporal = fechaTemporal.AddDays(1);
            //}

            return null;
        }

        private IncidenciasDuplicadas DateTimeToString(DateTime fecha, DateTime FechaFin, IncidenciasDuplicadas duplicado)
        {
            if (fecha == FechaFin)
            {
                duplicado.Fecha = fecha.ToString(@"dd \de MMMM");
            }
            else if (fecha.Month == FechaFin.Month)
            {
                duplicado.Fecha = fecha.ToString("dd");
                duplicado.FechaFinal = FechaFin.ToString(@"dd \de MMMM");
            }
            else
            {
                duplicado.Fecha = fecha.ToString("dd/MM");
                duplicado.FechaFinal = FechaFin.ToString("dd/MM");
            }
            return duplicado;
        }

        private IncidenciasDuplicadas SetEmpleado(int IdEmpleado, IncidenciasDuplicadas duplicado)
        {
            Empleados e = new Empleados();
            var empleado = e.GetEmpleadoById(IdEmpleado);
            duplicado.Empleado = empleado.Nombres + " " + empleado.APaterno + " " + empleado.AMaterno;
            return duplicado;
        }
    }

    public class IncidenciasDuplicadas
    {
        public string Incidencia { get; set; }
        public string Fecha { get; set; }
        public string FechaFinal { get; set; }
        public string Empleado { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime FechaIncidencia { get; set; }
        public DateTime? FechaFinIncidencia { get; set; }
        public int Dias { get; set; }
        public int IdTipoInasistencia { get; set; }
    }
}
