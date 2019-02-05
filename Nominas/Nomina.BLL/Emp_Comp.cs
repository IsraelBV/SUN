using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using RH.BLL;

namespace Nomina.BLL
{
    public class Emp_Comp
    {
        RHEntities _ctx = null;
        public Emp_Comp()
        {
            _ctx = new RHEntities();
        }

        /// <summary>
        /// Actualiza los registros en la tabla de EmpleadoComplemento con base en la configuración de los empleados
        /// </summary>
        /// <param name="IdPeriodoPago"></param>
        public void UpdateEmpleadoComplementoRegistros(int IdPeriodoPago)
        {
            PeriodosPago p = new PeriodosPago();
            var empleados = p.GetIdEmpleados(IdPeriodoPago);
            var records = (from ec in _ctx.NOM_Empleado_Conceptos
                           where empleados.Contains(ec.IdEmpleado) && ec.Complemento == true
                           select ec).ToList(); //obtiene los empleados activos en el periodo de pago

            // obtiene los empleados que ya existian en la tabla de empleado complemento con ese periodo de pago
            var listaComplemento = _ctx.NOM_Empleado_Complemento.Where(x => x.IdPeriodo == IdPeriodoPago).ToList();

            foreach (var record in records)
            {
                var complemento = listaComplemento.FirstOrDefault(x => x.IdEmpleado == record.IdEmpleado && x.IdConcepto == record.IdConcepto);
                if (complemento == null) //si no existe en la lista, lo crea
                {
                    complemento = new NOM_Empleado_Complemento();
                    complemento.IdEmpleado = record.IdEmpleado;
                    complemento.IdConcepto = record.IdConcepto;
                    complemento.IdPeriodo = IdPeriodoPago;
                    _ctx.NOM_Empleado_Complemento.Add(complemento);
                    _ctx.SaveChanges();
                }
                listaComplemento.Remove(complemento);
            }

            _ctx.NOM_Empleado_Complemento.RemoveRange(listaComplemento); //elimina el resto de los registros
            _ctx.SaveChanges();
        }

        /// <summary>
        /// Obtiene los empleados que tengan al menos un concepto complemento activos en el Periodo de Pago
        /// </summary>
        /// <param name="IdPeriodoPago"></param>
        /// <returns></returns>
        public List<DatosEmpleado> GetEmpleadosConConceptosComplemento(int IdPeriodoPago)
        {
            return (from conf in _ctx.NOM_Empleado_Complemento
                    join emp in _ctx.Empleado on conf.IdEmpleado equals emp.IdEmpleado
                    let con = _ctx.Empleado_Contrato.Where(x => x.IdEmpleado == emp.IdEmpleado).OrderByDescending(x => x.IdEmpleado).FirstOrDefault()
                    where conf.IdPeriodo == IdPeriodoPago
                    select new DatosEmpleado
                    {
                        IdEmpleado = emp.IdEmpleado,
                        Nombres = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        SDI = con.SDI,
                        SalarioReal = con.SalarioReal,
                        SD = con.SD
                    }
                    ).Distinct().ToList();
        }

        public List<EmpleadoComplemento> empleadoComplemento(int idPeriodo)
        {

            var idConceptos = (from emp_con in _ctx.NOM_Empleado_Conceptos
                               join emp_per in _ctx.NOM_Empleado_PeriodoPago
                               on emp_con.IdEmpleado equals emp_per.IdEmpleado
                               where emp_per.IdPeriodoPago == idPeriodo && emp_con.Complemento == true
                               select emp_con).ToList();
            var idDiferentes = idConceptos.GroupBy(X => X.IdConcepto).ToList();

            var empleadosConceptos = (from emp in _ctx.Empleado
                                      join periodo_emp in _ctx.NOM_Empleado_PeriodoPago
                                      on emp.IdEmpleado equals periodo_emp.IdEmpleado
                                      where periodo_emp.IdPeriodoPago == idPeriodo
                                      select new EmpleadoComplemento
                                      {
                                          IdEmpleadoComplemento = emp.IdEmpleado,
                                          Nombres = emp.Nombres,
                                          Paterno = emp.APaterno,
                                          Materno = emp.AMaterno


                                      }).ToList();

            var array = idDiferentes.Select(x => x.Key).ToArray();

            var conceptos = (from con in _ctx.C_NOM_Conceptos
                             where array.Contains(con.IdConcepto)
                             select con).ToList();


            foreach (var emp in empleadosConceptos)
            {
                List<conceptosComplemento> conceptosCom = new List<conceptosComplemento>();
                foreach (var idcon in conceptos)
                {
                    conceptosComplemento con = new conceptosComplemento();
                    var consulta = _ctx.NOM_Empleado_Complemento.Where(x => x.IdEmpleado == emp.IdEmpleadoComplemento && x.IdConcepto == idcon.IdConcepto).FirstOrDefault();
                    var asignado = _ctx.NOM_Empleado_Conceptos.Where(x => x.IdEmpleado == emp.IdEmpleadoComplemento && x.IdConcepto == idcon.IdConcepto && x.Complemento == true).FirstOrDefault();
                    if (consulta != null)
                    {
                        con.idConcepto = consulta.IdConcepto;
                        con.cantidad = consulta.Cantidad;
                        con.Descricipcion = idcon.DescripcionCorta;
                        con.conFormula = idcon.FormulaComplemento;
                        if (asignado != null)
                        {
                            con.asignado = true;
                        }
                        else
                        {
                            con.asignado = false;
                        }
                    }
                    else
                    {
                        con.idConcepto = idcon.IdConcepto;
                        con.cantidad = 0;
                        con.Descricipcion = idcon.DescripcionCorta;
                        con.conFormula = idcon.FormulaComplemento;
                        if (asignado != null)
                        {
                            con.asignado = true;
                        }
                        else
                        {
                            con.asignado = false;
                        }
                    }
                    conceptosCom.Add(con);
                }
                emp.listaconceptos = conceptosCom;
            }

            return empleadosConceptos;
        }

        /// <summary>
        /// Obtiene las cantidades de la tabla de Empleado Complemento por empleado
        /// </summary>
        /// <param name="IdEmpleado"></param>
        /// <param name="IdPeriodoPago"></param>
        /// <returns></returns>
        //public List<EmpleadoComplemento> GetEmpleadoComplemento(int IdEmpleado, int IdPeriodoPago)
        //{
        //    return (from ec in _ctx.NOM_Empleado_Complemento
        //     join c in _ctx.C_NOM_Conceptos on ec.IdConcepto equals c.IdConcepto
        //     where ec.IdEmpleado == IdEmpleado && ec.IdPeriodo == IdPeriodoPago
        //     select new EmpleadoComplemento
        //     {
        //         IdEmpleadoComplemento = ec.IdEmpleadoComplemento,
        //         DescripcionComplemento = c.Descripcion,
        //         Cantidad = ec.Cantidad,
        //         IdConcepto = ec.IdConcepto,
        //         IdPeriodo = ec.IdPeriodo,
        //         IdEmpleado = ec.IdEmpleado
        //     }).ToList();
        //}

        /// <summary>
        /// Obtiene las cantidades de los conceptos que tienen en común varios empleados en el periodo de pago activo
        /// </summary>
        /// <param name="idsEmpleado"></param>
        /// <param name="IdPeriodoPago"></param>
        /// <returns></returns>
        public List<C_NOM_Conceptos> GetEmpleadoComplemento(int[] idsEmpleado, int IdPeriodoPago)
        {
            var list = _ctx.NOM_Empleado_Complemento
                .Where(x => x.IdPeriodo == IdPeriodoPago && idsEmpleado.Contains(x.IdEmpleado))
                .GroupBy(x => x.IdConcepto)
                .Select(x => new
                {
                    Cantidad = x.Count(),
                    Concepto = x.Key,
                }).ToList();


            var result = new List<int>();
            foreach (var item in list)
            {
                if (item.Cantidad >= idsEmpleado.Length)
                {
                    result.Add(item.Concepto);
                }
            }

            return _ctx.C_NOM_Conceptos.Where(x => result.Contains(x.IdConcepto)).ToList();
        }

        /// <summary>
        /// Actualiza un registro de la tabla de Empleado Complemento
        /// </summary>
        /// <param name="IdEmpleadoComplemento"></param>
        /// <param name="Cantidad"></param>
        /// <returns></returns>
        public int UpdateEmpleadoComplemento(int IdEmpleadoComplemento, decimal Cantidad)
        {
            var record = _ctx.NOM_Empleado_Complemento.FirstOrDefault(x => x.IdEmpleadoComplemento == IdEmpleadoComplemento);
            if (record != null)
            {
                record.Cantidad = Cantidad;
                var result = _ctx.SaveChanges();
                return result;
            }
            return -1000; //el registro no se encontró
        }

        public int UpdateEmpleadoComplemento(int[] idsEmpleados, int IdConcepto, int IdPeriodo, decimal Cantidad)
        {
            foreach (var id in idsEmpleados)
            {
                var record = _ctx.NOM_Empleado_Complemento.FirstOrDefault(x => x.IdEmpleado == id && x.IdConcepto == IdConcepto && x.IdPeriodo == IdPeriodo);
                if (record != null)
                {
                    record.Cantidad = Cantidad;
                }
            }
            return _ctx.SaveChanges();
        }

         public UserConfig ObtenerConfiguracion(int idUsuario, int idSucursal,int idModulo)
        {
           var datos = _ctx.UserConfig.Where(x => x.IdUsuario == idUsuario && x.IdSucursal == idSucursal && x.Modulo == idModulo).FirstOrDefault();
            return datos;
        }
        public bool GuardarConfiguracion (int idUsuario,int idSucursal,int [] visible, int [] oculto,int idModulo)
        {
            string oc;
            string vis;
            if(visible != null)
            {
                 vis = string.Join(",", visible);
            }
            else
            {
                vis = "0";
            }
            if(oculto != null)
            {
                 oc = string.Join(",", oculto);
            }else
            {
                 oc = "0";
            }
            
            var result= false;
            int t = 0;
            var existe = _ctx.UserConfig.Where(x => x.IdUsuario == idUsuario && x.IdSucursal == idSucursal && x.Modulo == idModulo).FirstOrDefault();

            if (existe != null)
            {
                const string sqlQuery = "DELETE UserConfig WHERE IdUserConfig = @p0";
                _ctx.Database.ExecuteSqlCommand(sqlQuery, existe.IdUserConfig);
                var item = new UserConfig
                {
                    IdUsuario = idUsuario,
                    IdSucursal = idSucursal,
                    ConceptosVisibles = vis,
                    ConceptosOcultos = oc,
                    Modulo = idModulo
                };

                _ctx.UserConfig.Add(item);
                 t = _ctx.SaveChanges();
            }else
            {
               

                var item = new UserConfig
                {
                    IdUsuario = idUsuario,
                    IdSucursal = idSucursal,
                    ConceptosVisibles = vis,
                    ConceptosOcultos = oc,
                    Modulo = idModulo
                };

                _ctx.UserConfig.Add(item);
                 t = _ctx.SaveChanges();
            }
          
            if(t > 1)
            {
                 result = true;
            }else
            {
                 result = false;
            }
            return result;
        }
        public void guardarComplemento(int idEmpleado ,int idPeriodo, int idConcepto, decimal cantidad)
        {
            var dato = _ctx.NOM_Empleado_Complemento.Where(x => x.IdEmpleado == idEmpleado && x.IdPeriodo == idPeriodo && x.IdConcepto == idConcepto).FirstOrDefault();

            if(dato != null)
            {
                const string sqlQuery = "DELETE NOM_Empleado_Complemento WHERE IdEmpleadoComplemento = @p0";
                _ctx.Database.ExecuteSqlCommand(sqlQuery, dato.IdEmpleadoComplemento);
                if(cantidad != 0)
                {
                    var item = new NOM_Empleado_Complemento
                    {
                        IdEmpleado = idEmpleado,
                        IdConcepto = idConcepto,
                        IdPeriodo = idPeriodo,
                        Cantidad = cantidad

                    };
                    _ctx.NOM_Empleado_Complemento.Add(item);
                    _ctx.SaveChanges();
                }
             
               
            }else
            {
                if(cantidad != 0)
                {
                    var item = new NOM_Empleado_Complemento
                    {
                        IdEmpleado = idEmpleado,
                        IdConcepto = idConcepto,
                        IdPeriodo = idPeriodo,
                        Cantidad = cantidad

                    };
                    _ctx.NOM_Empleado_Complemento.Add(item);
                    _ctx.SaveChanges();
                }
               
            }
           
        }
    }
    public class EmpleadoComplemento
    {
        public int IdEmpleadoComplemento { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
      
        public List<conceptosComplemento> listaconceptos { get; set; }

    }

    public class conceptosComplemento
    {
        public int idConcepto { get; set; }
        public string Descricipcion { get; set; }
        public decimal cantidad { get; set; }
        public bool conFormula { get; set; }
        public bool asignado { get; set; }
    }
}
