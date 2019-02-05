using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using RH.BLL;

namespace Nomina.BLL
{
    public class AsignacionConceptos
    {
        RHEntities ctx = null;

        public AsignacionConceptos()
        {
            ctx = new RHEntities();
        }

        public List<asignacionclase> busquedaEmplComp(int id, List<int> listIdEmpleados)
        {
            List<asignacionclase> result = new List<asignacionclase>();

            var arrayIdEmpleado = listIdEmpleados.ToArray();

            List<NOM_Empleado_Conceptos> listaEmpleadoConceptos;
            List<Empleado> listaEmpleados;
            List<Empleado_Contrato> listaContratos;


            using (var contexto = new RHEntities())
            {
                listaEmpleadoConceptos = (from ec in contexto.NOM_Empleado_Conceptos
                                          where arrayIdEmpleado.Contains(ec.IdEmpleado)
                                          select ec).ToList();

                listaEmpleados = (from em in contexto.Empleado
                                  where arrayIdEmpleado.Contains(em.IdEmpleado)
                                  select em).ToList();

                listaContratos = (from con in contexto.Empleado_Contrato
                                  where arrayIdEmpleado.Contains(con.IdEmpleado)
                                        && con.Status == true
                                  orderby con.IdContrato descending
                                  select con).ToList();


                foreach (var idemp in listIdEmpleados)
                {
                    asignacionclase item = new asignacionclase();
                    var datos = listaEmpleadoConceptos.FirstOrDefault(x => x.IdConcepto == id && x.IdEmpleado == idemp);

                    var emp = (from e in listaEmpleados
                               join ce in listaContratos
                               on e.IdEmpleado equals ce.IdEmpleado
                               where e.IdEmpleado == idemp
                               orderby ce.IdContrato descending
                               select new DatosEmpleado
                               {
                                   IdEmpleado = e.IdEmpleado,
                                   Nombres = e.Nombres,
                                   Paterno = e.APaterno,
                                   Materno = e.AMaterno,
                                   Status = ce.Status,
                                   idEmpresaFiscal = ce.IdEmpresaFiscal,
                                   idEmpresaAsimilado = ce.IdEmpresaAsimilado,
                                   idEmpresaComplemento = ce.IdEmpresaComplemento,
                                   idEmpresaSindicato = ce.IdEmpresaSindicato

                               }).FirstOrDefault();

                    if(emp == null) continue;
                    if (emp.Status == true)
                    {
                        item.IdC = id;
                        item.IdE = emp.IdEmpleado;
                        item.Nombres = emp.Nombres;
                        item.Paterno = emp.Paterno;
                        item.Materno = emp.Materno;
                        item.statusEmpleado = emp.Status;
                        item.idFiscal = emp.idEmpresaFiscal;
                        item.idAsimilado = emp.idEmpresaAsimilado;
                        item.idSindicato = emp.idEmpresaSindicato;
                        item.idComplemento = emp.idEmpresaComplemento;

                        if (datos != null)
                        {

                            item.status = true;
                            item.isFiscal = datos.Fiscal;
                            item.isComplemento = datos.Complemento;
                        }
                        else
                        {
                            item.status = false;
                            item.isFiscal = false;
                            item.isComplemento = false;
                        }

                        result.Add(item);

                    }
                    else
                    {
                        item.IdC = id;
                        item.IdE = emp.IdEmpleado;
                        item.Nombres = emp.Nombres;
                        item.Paterno = emp.Paterno;
                        item.Materno = emp.Materno;
                        item.statusEmpleado = emp.Status;
                        item.idFiscal = emp.idEmpresaFiscal;
                        item.idAsimilado = emp.idEmpresaAsimilado;
                        item.idSindicato = emp.idEmpresaSindicato;
                        item.idComplemento = emp.idEmpresaComplemento;
                        if (datos != null)
                        {

                            item.status = true;
                            item.isFiscal = datos.Fiscal;
                            item.isComplemento = datos.Complemento;
                        }
                        else
                        {
                            item.status = false;
                            item.isFiscal = false;
                            item.isComplemento = false;
                        }


                        result.Add(item);
                    }

                }
            }






            return result;
        }

        public bool GuardarRegistroConcepto_Empleado(int[] arrayC, List<ConceptoEmpleado> arrayE, int idsucursal)
        {
            if (arrayE == null) return false;

            var longitudC = arrayC.Length;
            foreach (var c in arrayC)
            {
                foreach (var e in arrayE)
                {


                    // busca que exista el registro 
                    var busqueda = ctx.NOM_Empleado_Conceptos.Where(x => x.IdConcepto == c && x.IdEmpleado == e.ide).FirstOrDefault();

                    //si checkvalue existe  
                    if (e.checkvalue == true)
                    {
                        if (busqueda == null)
                        {
                            //insertamos registro 
                            var item = new NOM_Empleado_Conceptos
                            {
                                IdConcepto = c,
                                IdEmpleado = e.ide,

                                IdSucursal = idsucursal,
                                Fiscal = e.isFiscal,
                                Complemento = e.isComplemento
                            };
                            ctx.NOM_Empleado_Conceptos.Add(item);
                            var t = ctx.SaveChanges();
                        }
                        else
                        {
                            //insertamos registro 
                            var item = ctx.NOM_Empleado_Conceptos.Where(x => x.IdConcepto == c && x.IdEmpleado == e.ide).FirstOrDefault();
                            {
                                item.IdConcepto = c;
                                item.IdEmpleado = e.ide;

                                item.IdSucursal = idsucursal;
                                item.Fiscal = e.isFiscal;
                                item.Complemento = e.isComplemento;

                            };

                            var t = ctx.SaveChanges();
                        }
                    }
                    else
                    {
                        //si checkvalue es igual false
                        if (busqueda != null)
                        {
                            //borrar regristo
                            if (longitudC == 1)
                            {
                                const string sqlQuery = "DELETE NOM_Empleado_Conceptos WHERE IdConfiguracion = @p0";
                                ctx.Database.ExecuteSqlCommand(sqlQuery, busqueda.IdConfiguracion);
                            }
                        }
                    }

                }
            }
            return true;
        }

        public List<Concepto> ListadoConcepto()
        {
            List<Concepto> resultado = new List<Concepto>();
            var listado = ctx.C_NOM_Conceptos.Where(x => x.FormulaFiscal != false || x.FormulaComplemento != false).ToList();
            foreach (var a in listado)
            {

                Concepto item = new Concepto();
                item.IdConcepto = a.IdConcepto;
                item.NombreConcepto = a.Descripcion;
                item.TipoConcepto = a.TipoConcepto;

                if (a.FormulaFiscal == true && a.FormulaComplemento == false)
                {
                    item.TipoFormula = 1;
                }
                else if (a.FormulaFiscal == false && a.FormulaComplemento == true)
                {
                    item.TipoFormula = 2;
                }
                else if (a.FormulaFiscal == true && a.FormulaComplemento == true)
                {
                    item.TipoFormula = 3;
                }
                else if (a.FormulaFiscal == false && a.FormulaComplemento == false)
                {
                    item.TipoFormula = 4;
                }
                resultado.Add(item);
            }


            return resultado;

        }

        public Concepto checkConcepto(int idConpceto)
        {
            var concepto = (from con in ctx.C_NOM_Conceptos
                            where con.IdConcepto == idConpceto
                            select new Concepto
                            {
                                IdConcepto = con.IdConcepto,
                                NombreConcepto = con.Descripcion,
                                conFiscal = con.FormulaFiscal,
                                conComplemento = con.FormulaComplemento
                            }).FirstOrDefault();
            return concepto;
        }


    }

    public class asignacionclase
    {
        public int IdC { get; set; }
        public int IdE { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public bool status { get; set; }
        public bool statusEmpleado { get; set; }
        public bool isFiscal { get; set; }
        public bool isComplemento { get; set; }
        public bool FormulaFiscal { get; set; }
        public bool FormulaComplemento { get; set; }
        public int? idFiscal { get; set; }
        public int? idAsimilado { get; set; }
        public int? idComplemento { get; set; }
        public int? idSindicato { get; set; }
    }
    public class ConceptoEmpleado
    {
        public bool checkvalue { get; set; }
        public int ide { get; set; }
        public bool isFiscal { get; set; }
        public bool isComplemento { get; set; }

    }

    public class Concepto
    {
        public int IdConcepto { get; set; }
        public string NombreConcepto { get; set; }
        public int TipoFormula { get; set; }
        public int TipoConcepto { get; set; }
        public bool conFiscal { get; set; }
        public bool conComplemento { get; set; }
    }
}
