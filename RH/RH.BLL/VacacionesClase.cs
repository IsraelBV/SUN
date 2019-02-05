using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.BLL
{

    public class VacacionesClase
    {
        RHEntities ctx = null;
        public VacacionesClase()
        {
            ctx = new RHEntities();
        }
        public DatosEmpleadoVacaciones GetDatosById(int id)
        {
            var datos = from emp in ctx.Empleado
                        join emp_cont in ctx.Empleado_Contrato
                         on emp.IdEmpleado equals emp_cont.IdEmpleado
                        where emp.IdEmpleado == id && emp_cont.Status == true
                        select new DatosEmpleadoVacaciones
                        {
                            IdEmpleado = emp.IdEmpleado,
                            IdEmpContrato = emp_cont.IdContrato,
                            Nombres = emp.Nombres,
                            Paterno = emp.APaterno,
                            Materno = emp.AMaterno,
                            Alta = emp_cont.FechaAlta,
                            Real = emp_cont.FechaReal,
                            Descanso1 = emp_cont.DiaDescanso,
                        };
            return datos.FirstOrDefault();
        }


        public List<Empleado> ObtenerEmpleadosPorSucursal(int sucursal)
        {
            List<Empleado> lista = null;

            lista = ctx.Empleado.Where(s => s.IdSucursal.Equals(sucursal)).ToList();

            return lista;
        }

        // agregar dias tomados - y si el periodo esta completo
        public List<DatosEmpleadoVacaciones> GetPeriodoVacaciones(int id)
        {
            var datos = from periodo in ctx.PeriodoVacaciones
                        join emp_cont in ctx.Empleado_Contrato
                         on periodo.IdEmpleado_Contrato equals emp_cont.IdContrato
                        join emp in ctx.Empleado
                        on emp_cont.IdEmpleado equals emp.IdEmpleado
                        where emp_cont.IdContrato == id
                        select new DatosEmpleadoVacaciones
                        {
                            IdEmpleado = emp.IdEmpleado,
                            IdEmpContrato = emp_cont.IdContrato,
                            IdPeriodo = periodo.IdPeridoVacaciones,
                            PerioVaca = periodo.PeridoVacaciones,
                            Dias = periodo.DiasCorresponde
                        };
            return datos.ToList();
        }


        // agregar dias tomados - y si el periodo esta completo
        public List<DatosEmpleadoVacaciones> GetPeriodoVacacionesV2(int idContrato)
        {
            var listaPeriodos = ctx.PeriodoVacaciones.Where(x => x.IdEmpleado_Contrato == idContrato).ToList();

            List < DatosEmpleadoVacaciones > listaDatos = new List<DatosEmpleadoVacaciones>();

            int diasUsados = 0;

            foreach (var item in listaPeriodos)
            {

                diasUsados = sumaDiasTomados(item.IdPeridoVacaciones);

                listaDatos.Add(new DatosEmpleadoVacaciones
                {
                    IdEmpContrato = item.IdEmpleado_Contrato,
                    IdPeriodo =   item.IdPeridoVacaciones,
                    PerioVaca = item.PeridoVacaciones,
                    Dias =  item.DiasCorresponde,
                    DiasTomados = diasUsados,
                    FechaUltimoPeriodo = getFechaUltimoPeriodo(item.IdPeridoVacaciones),
                    periodoCompletado = (diasUsados == item.DiasCorresponde ? true :false)
                });
            }

            //var datos = from periodo in ctx.PeriodoVacaciones
            //            where periodo.IdEmpleado_Contrato == idContrato
            //            let dias = sumaDiasTomados(periodo.IdPeridoVacaciones)
            //            select new DatosEmpleadoVacaciones
            //            {
            //                IdEmpContrato = periodo.IdEmpleado_Contrato,
            //                IdPeriodo = periodo.IdPeridoVacaciones,
            //                PerioVaca = periodo.PeridoVacaciones,
            //                Dias = periodo.DiasCorresponde,
            //                DiasTomados = dias
            //            };
            //return datos.ToList();


            return listaDatos;
        }


        public List<DatosEmpleadoVacaciones> GetVacaciones(int id)
        {
            var datos = from periodo in ctx.PeriodoVacaciones
                        join vaca in ctx.Vacaciones
                         on periodo.IdPeridoVacaciones equals vaca.IdPeridoVacaciones

                        where periodo.IdPeridoVacaciones == id
                        select new DatosEmpleadoVacaciones
                        {
                            DiasTomados = vaca.DiasTomados,
                            status = vaca.Status,
                            Dias = periodo.DiasCorresponde,
                            Inicio = vaca.FechaInicio,
                            Fin = vaca.FechaFin

                        };

            return datos.ToList();
        }


        public DatosEmpleadoVacaciones GetPeriodoVacacionesById(int id)
        {
            var datos = from periodo in ctx.PeriodoVacaciones
                        join emp_contrato in ctx.Empleado_Contrato
                        on periodo.IdEmpleado_Contrato equals emp_contrato.IdContrato
                        where periodo.IdPeridoVacaciones == id
                        select new DatosEmpleadoVacaciones
                        {

                            IdPeriodo = periodo.IdPeridoVacaciones,
                            PerioVaca = periodo.PeridoVacaciones,
                            Dias = periodo.DiasCorresponde,
                            Descanso1 = emp_contrato.DiaDescanso,
                        };
            return datos.FirstOrDefault();
        }

        public bool CrearVacaciones(Vacaciones collection)
        {
            
            var periodo = ctx.PeriodoVacaciones.FirstOrDefault(x => x.IdPeridoVacaciones == collection.IdPeridoVacaciones);
            var datos = from empleado in ctx.Empleado
                        join emp_contr in ctx.Empleado_Contrato
                        on empleado.IdEmpleado equals emp_contr.IdEmpleado
                        join contrato in ctx.Empleado_Contrato
                        on emp_contr.IdContrato equals contrato.IdContrato
                        where emp_contr.IdContrato == periodo.IdEmpleado_Contrato
                        select new DatosEmpleadoVacaciones
                        {
                            IdEmpleado = empleado.IdEmpleado,
                            Nombres = empleado.Nombres,
                            Paterno = empleado.APaterno,
                            Materno = empleado.AMaterno,
                            IdSucursal = empleado.IdSucursal
                        };
            var DatosEmpleado = datos.FirstOrDefault();
            collection.Status = true;

            ctx.Vacaciones.Add(collection);
            _Inasistencias ina = new _Inasistencias();
            var duplicado = ina.RevisarDuplicados(collection.Id, collection.FechaInicio, collection.FechaFin);
            if (duplicado == null)
            {
                try
                {
                    var noti = new Notificaciones();
                    ctx.SaveChanges();
                    noti.Vacaciones(collection, DatosEmpleado, periodo);
                }
                catch(SqlException e)
                {
                    string r = e.Message;

                }
                return false;
            }
            else
            {
                return true;
            }

        }
        public List<DatosEmpleadoVacaciones> Historial(int id)
        {
            var datos = from peri in ctx.PeriodoVacaciones
                        join emp_contrato in ctx.Empleado_Contrato
                        on peri.IdEmpleado_Contrato equals emp_contrato.IdContrato
                        join vacaciones in ctx.Vacaciones
                        on peri.IdPeridoVacaciones equals vacaciones.IdPeridoVacaciones


                        where emp_contrato.IdContrato == id
                        select new DatosEmpleadoVacaciones
                        {
                            IdPeriodo = peri.IdPeridoVacaciones,
                            PerioVaca = peri.PeridoVacaciones,
                            DiasTomados = vacaciones.DiasTomados,
                            trabajo = vacaciones.Trabajado,
                            Inicio = vacaciones.FechaInicio,
                            Fin = vacaciones.FechaFin,
                            Presentarse = vacaciones.Presentarse,
                            observaciones = vacaciones.Observaciones,
                            status = vacaciones.Status,
                            idvacaciones = vacaciones.Id


                        };
            return datos.ToList();
        }

        public List<DatosEmpleadoVacaciones> HistoriaByPeriodo(int id)
        {
            var datos = from vacaciones in ctx.Vacaciones
                        join periodo in ctx.PeriodoVacaciones
                        on vacaciones.IdPeridoVacaciones equals periodo.IdPeridoVacaciones
                        where vacaciones.IdPeridoVacaciones == id
                        select new DatosEmpleadoVacaciones
                        {
                            IdPeriodo = periodo.IdPeridoVacaciones,
                            PerioVaca = periodo.PeridoVacaciones,
                            DiasTomados = vacaciones.DiasTomados,
                            trabajo = vacaciones.Trabajado,
                            Inicio = vacaciones.FechaInicio,
                            Fin = vacaciones.FechaFin,
                            Presentarse = vacaciones.Presentarse,
                            observaciones = vacaciones.Observaciones,
                            status = vacaciones.Status,
                            idvacaciones = vacaciones.Id
                        };
            return datos.ToList();
        }

        public DatosEmpleadoVacaciones ObtenerIdContrato(int id)
        {
            var datos = from emp_contrato in ctx.Empleado_Contrato
                        join emp in ctx.Empleado
                        on emp_contrato.IdEmpleado equals emp.IdEmpleado
                        where emp.IdEmpleado == id && emp_contrato.Status == true
                        select new DatosEmpleadoVacaciones
                        {
                            IdEmpContrato = emp_contrato.IdContrato
                        };
            return datos.FirstOrDefault();
        }

        public List<DatosEmpleadoVacaciones> ObtenerIDPeriodo(int id)
        {
            var datos = from periodo in ctx.PeriodoVacaciones
                        join emp_contrato in ctx.Empleado_Contrato
                        on periodo.IdEmpleado_Contrato equals emp_contrato.IdContrato
                        where emp_contrato.IdContrato == id
                        select new DatosEmpleadoVacaciones
                        {
                            IdPeriodo = periodo.IdPeridoVacaciones,
                            PerioVaca = periodo.PeridoVacaciones

                        };
            return datos.ToList();
        }

        public DatosEmpleadoVacaciones WordDoc(int id)
        {
            var datos = from emp_contrato in ctx.Empleado_Contrato
                        join periodo in ctx.PeriodoVacaciones
                        on emp_contrato.IdContrato equals periodo.IdEmpleado_Contrato
                        join emp in ctx.Empleado
                        on emp_contrato.IdEmpleado equals emp.IdEmpleado
                        join pues in ctx.Puesto
                        on emp_contrato.IdPuesto equals pues.IdPuesto
                        join depa in ctx.Departamento
                        on pues.IdDepartamento equals depa.IdDepartamento
                        join vacas in ctx.Vacaciones
                        on periodo.IdPeridoVacaciones equals vacas.IdPeridoVacaciones

                        where vacas.Id == id

                        select new DatosEmpleadoVacaciones
                        {
                            IdEmpleado = emp.IdEmpleado,
                            idvacaciones = vacas.Id,
                            IdPeriodo = vacas.IdPeridoVacaciones,
                            Nombres = emp.Nombres,
                            Paterno = emp.APaterno,
                            Materno = emp.AMaterno,
                            Depa = depa.Descripcion,
                            _Puesto = pues.Descripcion,
                            Alta = emp_contrato.FechaAlta,
                            PerioVaca = periodo.PeridoVacaciones,
                            Inicio = vacas.FechaInicio,
                            Fin = vacas.FechaFin,
                            Presentarse = vacas.Presentarse,
                            Dias = vacas.DiasTomados,
                            DiasTotales = periodo.DiasCorresponde
                        };
            return datos.FirstOrDefault();
        }

        public Empleado_Contrato EmpresaEmpleado(int IdEmpleado)
        {
            //var datos = from e in ctx.Empleado
            //            let c = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault()
            //            let ef = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaFiscal).Select(x => x.RazonSocial).FirstOrDefault()
            //            let ec = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault()
            //            let ea = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaAsimilado).Select(x => x.RazonSocial).FirstOrDefault()
            //            let es = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaSindicato).Select(x => x.RazonSocial).FirstOrDefault()
            //            select new DatosEmpleadoVacaciones
            //            {
            //                EmpresaFiscal = ef,
            //                EmpresaComplemento = ec,
            //                EmpresaAsimilado = ea,
            //                EmpresaSindicato = es
            //            };
            var datos = (from e in ctx.Empleado
                         join ec in ctx.Empleado_Contrato
                         on e.IdEmpleado equals ec.IdEmpleado
                         where e.IdEmpleado == IdEmpleado
                         select ec);

            return datos.FirstOrDefault();

        }

        public int sumatoria(int id, int idperiodo)
        {
            int suma = 0;
            suma = (from vacas in ctx.Vacaciones
                    where vacas.IdPeridoVacaciones == idperiodo && vacas.Id < id
                    select (int?)vacas.DiasTomados).Sum() ?? 0;
            ;
            return suma;
        }


        private int sumaDiasTomados(int idPeriodoVacacion)
        {
            var suma = (from vacacion in ctx.Vacaciones
                where vacacion.IdPeridoVacaciones == idPeriodoVacacion
                select (int?) vacacion.DiasTomados).Sum() ?? 0;

            return suma;
        }

        private string getFechaUltimoPeriodo(int IdPeridoVac)
        {
            //var item =
            //    ctx.Vacaciones
            //        .OrderByDescending(x => x.IdPeridoVacaciones)
            //        .FirstOrDefault(x => x.IdPeridoVacaciones == IdPeridoVac);
            var item = (from peri in ctx.Vacaciones
                       where peri.IdPeridoVacaciones == IdPeridoVac
                       orderby peri.Id descending
                       select peri).FirstOrDefault();

            if (item == null) return string.Empty;

            var fecha = item.FechaInicio.ToString("d") + " - " + item.FechaFin.ToString("d");

            return fecha;
        }

        public int? DiaDescanso(int id)
        {
            //var datos = ctx.Empleado_Contrato.Where(s => s.IdContrato == id).FirstOrDefault();
            int? datos = (from contrato in ctx.Empleado_Contrato
                          where contrato.IdEmpleado == id && contrato.Status == true
                          select contrato).FirstOrDefault().DiaDescanso;

            return datos;
        }

    }



    public class DatosEmpleadoVacaciones
    {

        public int IdEmpleado { get; set; }
        public int IdEmpContrato { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public DateTime Alta { get; set; }
        public DateTime Real { get; set; }
        public int? Descanso1 { get; set; }
        public string Descanso2 { get; set; }
        public int IdPeriodo { get; set; }
        public string PerioVaca { get; set; }
        public int Dias { get; set; }
        public int DiasTomados { get; set; }
        public int DiasTotales { get; set; }
        public bool trabajo { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public DateTime Presentarse { get; set; }
        public string observaciones { get; set; }
        public string Depa { get; set; }
        public string _Puesto { get; set; }
        public bool status { get; set; }
        public int idvacaciones { get; set; }
        public int sumatoria { get; set; }
        public string RazonSocial { get; set; }
        public int IdSucursal { get; set; }

        public string FechaUltimoPeriodo { get; set; }

        public bool periodoCompletado { get; set; }
        public string EmpresaFiscal { get; set; }
        public string EmpresaComplemento { get; set; }
        public string EmpresaSindicato { get; set; }
        public string EmpresaAsimilado { get; set; }
    }



}
