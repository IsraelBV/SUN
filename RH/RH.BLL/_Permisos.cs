using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novacode;
using Common.Helpers;
using Common.Enums;
namespace RH.BLL
{
    
    public class _Permisos
    {
        RHEntities ctx = null;
        public _Permisos()
        {
            ctx = new RHEntities();
        }
        public List<Empleado> ObtenerEmpleadosPorSucursal(int sucursal)
        {
            List<Empleado> lista = null;

            lista = ctx.Empleado.Where(s => s.IdSucursal.Equals(sucursal)).ToList();

            return lista;
        }
        public DatosEmpleadoPermiso datosempleado(int id)
        {
            var datos = from emp in ctx.Empleado
                        join emp_cont in ctx.Empleado_Contrato
                         on emp.IdEmpleado equals emp_cont.IdEmpleado
                        where emp.IdEmpleado == id && emp_cont.Status == true
                        select new DatosEmpleadoPermiso
                        {
                            IdEmpleado = emp.IdEmpleado,
                            descanso1 = emp_cont.DiaDescanso,
                            status = emp_cont.Status
                        };
            return datos.FirstOrDefault();
        }

        public bool CrearPermiso(Permisos bn)
        {
            var noti = new Notificaciones();
            var result = false;
            var DatosEmpleado = ctx.Empleado.FirstOrDefault(x => x.IdEmpleado == bn.IdEmpleado);

            var DatosContrato = (from empl_contr in ctx.Empleado_Contrato
                                 where empl_contr.IdEmpleado == DatosEmpleado.IdEmpleado
                                 orderby empl_contr.IdContrato descending
                                 select empl_contr).Take(1).FirstOrDefault();
            var DatosPuesto = ctx.Puesto.FirstOrDefault(x => x.IdPuesto == DatosContrato.IdPuesto);
            var DatosDepartamento = ctx.Departamento.FirstOrDefault(x => x.IdDepartamento == DatosPuesto.IdDepartamento);

                ctx.Permisos.Add(bn);
            _Inasistencias ina = new _Inasistencias();
            var duplicado = ina.RevisarDuplicados(bn.IdEmpleado, bn.FechaInicio, bn.FechaFin);
            if (duplicado == null)
            {
                ctx.SaveChanges();
                noti.Permisos(bn, DatosEmpleado, DatosPuesto, DatosDepartamento);
                return false;
            }else
            {
                return true;
            }
          
        }

        public List<Permisos> HistorialPermiso(int id)
        {
            List<Permisos> lista = null;

            lista = ctx.Permisos.Where(x=> x.IdEmpleado == id).ToList();

            return lista;
            
        }


      public DatosEmpleadoPermiso  GetDatosPlantilla(int id)
        {
            var datos = from perm in ctx.Permisos
                        join emp in ctx.Empleado
                        on perm.IdEmpleado equals emp.IdEmpleado
                        join emp_contrato in ctx.Empleado_Contrato
                        on emp.IdEmpleado equals emp_contrato.IdEmpleado
                        join puesto in ctx.Puesto
                        on emp_contrato.IdPuesto equals puesto.IdPuesto
                        join depa in ctx.Departamento
                        on puesto.IdDepartamento equals depa.IdDepartamento
                        where perm.IdPermiso == id
                        select new DatosEmpleadoPermiso
                        {
                            IdEmpleado = emp.IdEmpleado,
                            Nombre = emp.Nombres,
                            APaterno = emp.APaterno,
                            AMaterno = emp.AMaterno,
                            Depa =depa.Descripcion,
                            _Puesto = puesto.Descripcion,
                            FechaInicio = perm.FechaInicio,
                            FechaFin = perm.FechaFin,
                            FechaPresentarse = perm.Presentarse,
                            Observaciones = perm.Observaciones,
                            PermisoDias = perm.Dias,
                            goce = perm.ConGoce
                        };


          return datos.FirstOrDefault();              
         }
        public DatosEmpleadoPermiso EmpresaEmpleado(int IdEmpleado)
        {
           var datos =  from e in ctx.Empleado
            let c = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault()
            let ef = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaFiscal).Select(x => x.RazonSocial).FirstOrDefault()
            let ec = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault()
            let ea = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaAsimilado).Select(x => x.RazonSocial).FirstOrDefault()
            let es = ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaSindicato).Select(x => x.RazonSocial).FirstOrDefault()
            select new DatosEmpleadoPermiso
            {
                EmpresaFiscal = ef,
                EmpresaComplemento = ec,
                EmpresaAsimilado = ea,
                EmpresaSindicato = es
            };


            return datos.FirstOrDefault();
        }


    }

    public class DatosEmpleadoPermiso
    {
        public int IdEmpleado { get; set; }
        public int? descanso1 { get; set; }      
        public bool status { get; set; }
        public string Nombre { get; set; }
        public string APaterno { get; set; }
        public string AMaterno { get; set; }
        public string Empresa { get; set; }
        public string Depa { get; set; }
        public string _Puesto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaPresentarse { get; set; }
        public string Observaciones { get; set; }
        public int? PermisoDias { get; set; }
        public bool? goce { get; set; }
        public string RazonSocial { get; set; }
        public string EmpresaFiscal { get; set; }
        public string EmpresaComplemento { get; set; }
        public string EmpresaSindicato { get; set; }
        public string EmpresaAsimilado { get; set; }


    }
}
