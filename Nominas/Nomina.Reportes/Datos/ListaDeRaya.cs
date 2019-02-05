using Common.Utils;
using RH.BLL;
using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomina.Reportes.Datos
{
   public class ListaDeRaya
    {
        RHEntities _ctx = null;

        public ListaDeRaya()
        {
            _ctx = new RHEntities();
        }

        public List<EmpInci> ContadoresIncidencias(List<EmpleadoIncidencias> lista)
        {
            List<EmpInci> _inci = new List<EmpInci>();

            foreach (var a in lista)
            {
                int contadorDescanso = 0;
                int contadorAsistencia = 0;
                int contadorIR = 0;
                int contadorIE = 0;
                int contadorIM = 0;
                int contadorFJ = 0;
                int contadorFI = 0;
                int contadorFA = 0;
                int contadorV = 0;
                foreach (var b in a.Incidencias)
                {
                    if (b.TipoIncidencia.Trim() == "D")
                    {
                        contadorDescanso = contadorDescanso + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "X")
                    {
                        contadorAsistencia = contadorAsistencia + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "IR")
                    {
                        contadorIR = contadorIR + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "IE")
                    {
                        contadorIE = contadorIE + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "IM")
                    {
                        contadorIM = contadorIM + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "PS")
                    {
                        contadorFJ = contadorFJ + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "FI")
                    {
                        contadorFI = contadorFI + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "FA")
                    {
                        contadorFA = contadorFA + 1;
                    }
                    if (b.TipoIncidencia.Trim() == "V")
                    {
                        contadorV = contadorV + 1;
                    }
                }
                _inci.Add(new EmpInci
                {
                    IdEmpleado = a.IdEmpleado,
                    Descansos = contadorDescanso,
                    Asistencias = contadorAsistencia,
                    IncapacidadesE = contadorIE,
                    IncapacidadesM = contadorIM,
                    IncapacidadesR = contadorIR,
                    FaltasA = contadorFA,
                    FaltasI = contadorFI,
                    _Vacaciones = contadorV,
                    DiasAPagar = a.DiasAPagar

                });

            }
            return _inci;
        }

        public EmpleadoDatosNominaViewModel ListadoEmpleados(int id)
        {
            //var contra =
            //   ctx.Empleado_Contrato.Where(x => x.IdEmpleado == id)
            //       .OrderByDescending(d => d.IdContrato)
            //       .FirstOrDefault();
            var datos = (from e in _ctx.Empleado.AsEnumerable()
                         let c = _ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault()
                         let ps = _ctx.Puesto.Where(x => x.IdPuesto == c.IdPuesto).FirstOrDefault()
                         let dp = _ctx.Departamento.Where(x => x.IdDepartamento == ps.IdDepartamento).FirstOrDefault()
                         let pp = _ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == c.IdPeriodicidadPago).FirstOrDefault()
                         where e.IdEmpleado == id 
                         select new EmpleadoDatosNominaViewModel
                         {
                             IdEmpleado = e.IdEmpleado,
                             Nombres = e.Nombres,
                             Paterno = e.APaterno,
                             Materno = e.AMaterno,
                             Sexo = e.Sexo == "H" ? "Hombre" : "Mujer",
                             CURP = e.CURP,
                             RFC = e.RFC,
                             NSS = e.NSS,
                             UMF = c.UMF,
                             FechaAlta = c.FechaAlta.ToString("dd-MM-yyyy"),
                             FechaReal = c.FechaReal.ToString("dd/MM/yyyy"),
                             ////FechaIMSS = contrato.FechaIMSS==null ? "" : Convert.ToString(contrato.FechaIMSS),
                             FechaIMSS = c.FechaIMSS != null ? c.FechaIMSS.Value.ToString("dd/MM/yyyy") : "",
                             ////FechaVigencia = contrato.Vigencia == null ? "" : Convert.ToString(contrato.Vigencia),
                             FechaVigencia = c.Vigencia != null ? c.Vigencia.Value.ToString("dd/MM/yyyy") : "",
                             DiasContrato = c.DiasContrato,
                             TipoContrato = c.TipoContrato == 1 ? "Permanente" : "Temporal",
                             DiaDescanso = UtilsEmpleados.seleccionarDia(c.DiaDescanso),
                             TipoNomina = pp.Descripcion,
                             TipoPago = UtilsEmpleados.TipoDePago(c.FormaPago),
                             TipoSalario = UtilsEmpleados.TipoSalario(c.TipoSalario),
                             SD = Convert.ToString(c.SD),
                             SDI = Convert.ToString(c.SDI),
                             SalarioReal = Convert.ToString(c.SalarioReal),
                             Puesto = ps.Descripcion,
                             Departamento = dp.Descripcion
                         }
                           ).FirstOrDefault();
            return datos;

        }
        public List<AutorizacionDetalle> conceptosDetallados(int idperiodo, int IdEmpresa)
        {
            var lista = new List<AutorizacionDetalle>();
            var empFiscal = _ctx.NOM_Nomina.Where(x => x.IdEmpresaFiscal == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
            if (empFiscal != null)// si la busqueda es diferente a NULL se condiera como Fiscal y se lleva acabo el querry 
            {
                AutorizacionDetalle autorizar = new AutorizacionDetalle();
                var datos = (from nom in _ctx.NOM_Nomina
                             join nom_det in _ctx.NOM_Nomina_Detalle
                             on nom.IdNomina equals nom_det.IdNomina
                             join concep in _ctx.C_NOM_Conceptos
                             on nom_det.IdConcepto equals concep.IdConcepto
                             join cl in _ctx.ClavesContables
                             on concep.IdConcepto equals  cl.IdConcepto
                             join se in _ctx.Sucursal_Empresa
                             on nom.IdSucursal equals se.IdSucursal
                             where nom.IdPeriodo == idperiodo && nom.IdEmpresaFiscal == IdEmpresa && cl.IdEmpresa == IdEmpresa && se.IdEmpresa == IdEmpresa && se.IdEsquema == 1
                             group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, nom_det.Complemento,cl.Acredora,cl.Deudora ,se.Clave_Contable} into temporal
                             select new
                             {
                                 temporal.Key.IdConcepto,
                                 temporal.Key.Descripcion,
                                 temporal.Key.TipoConcepto,
                                 temporal.Key.Obligatorio,
                                 temporal.Key.Complemento,
                                 Sumatoria = temporal.Sum(p => p.Total),
                                 temporal.Key.Acredora,
                                 temporal.Key.Deudora,
                                 temporal.Key.Clave_Contable


                             }).Distinct().ToList();


                 lista = (from new2 in datos
                             select new AutorizacionDetalle
                             {
                                 IdConcepto = new2.IdConcepto,
                                 NombreConcepto = new2.Descripcion,
                                 TipoConcepto = new2.TipoConcepto,
                                 TotalConcepto = new2.Sumatoria,
                                 Obligacion = new2.Obligatorio,
                                 Complemento = new2.Complemento,
                                 Acredora = new2.Acredora,
                                 Deudora = new2.Deudora,
                                 ClaveCliente = new2.Clave_Contable
                             }).OrderByDescending(x=>x.TipoConcepto).ToList();

                return lista;
            }
            else
            {
                var empAsimilado = _ctx.NOM_Nomina.Where(x => x.IdEmpresaAsimilado == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
                var clave = _ctx.Sucursal_Empresa.Where(X => X.IdSucursal == empAsimilado.IdSucursal && X.IdEmpresa == IdEmpresa).FirstOrDefault();

                if (empAsimilado != null)// si la busqueda es diferente a NULL se condiera como Asimilado y se lleva acabo el querry 
                {
                    AutorizacionDetalle autorizar = new AutorizacionDetalle();
                    var datos = (from nom in _ctx.NOM_Nomina
                                 join nom_det in _ctx.NOM_Nomina_Detalle
                                 on nom.IdNomina equals nom_det.IdNomina
                                 join concep in _ctx.C_NOM_Conceptos
                                 on nom_det.IdConcepto equals concep.IdConcepto
                                 join cl in _ctx.ClavesContables
                                 on concep.IdConcepto equals cl.IdConcepto
                              
                                 where nom.IdPeriodo == idperiodo && nom.IdEmpresaAsimilado == IdEmpresa && cl.IdEmpresa == IdEmpresa 
                                 group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, nom_det.Complemento, cl.Acredora, cl.Deudora } into temporal
                                 select new
                                 {
                                     temporal.Key.IdConcepto,
                                     temporal.Key.Descripcion,
                                     temporal.Key.TipoConcepto,
                                     temporal.Key.Obligatorio,
                                     temporal.Key.Complemento,
                                     Sumatoria = temporal.Sum(p => p.Total),
                                     temporal.Key.Acredora,
                                     temporal.Key.Deudora,
                                     


                                 }).Distinct().ToList();



                    lista = (from new2 in datos
                                 select new AutorizacionDetalle
                                 {
                                     IdConcepto = new2.IdConcepto,
                                     NombreConcepto = new2.Descripcion,
                                     TipoConcepto = new2.TipoConcepto,
                                     TotalConcepto = new2.Sumatoria,
                                     Obligacion = new2.Obligatorio,
                                     Complemento = new2.Complemento,
                                     Acredora = new2.Acredora,
                                     Deudora = new2.Deudora,
                                     ClaveCliente = clave.Clave_Contable
                                 }).ToList();

                    
                }
                return lista;
            }



        }
        public List<AutorizacionDetalle> conceptosDetalladosFiniquito(int idperiodo, int IdEmpresa)
        {
            var lista = new List<AutorizacionDetalle>();
           // var empFiscal = _ctx.NOM_Finiquito.Where(x => x.IdEmpresaFiscal == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
            var empFiscal = _ctx.NOM_Finiquito.Where(x => x.IdEmpresaFiscal == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
            var clave = _ctx.Sucursal_Empresa.Where(X => X.IdSucursal == empFiscal.IdSucursal && X.IdEmpresa == IdEmpresa).FirstOrDefault();
            if (empFiscal != null)// si la busqueda es diferente a NULL se condiera como Fiscal y se lleva acabo el querry 
            {
                AutorizacionDetalle autorizar = new AutorizacionDetalle();
                var datos = (from fin in _ctx.NOM_Finiquito
                             join fin_det in _ctx.NOM_Finiquito_Detalle
                             on fin.IdFiniquito equals fin_det.IdFiniquito
                             join concep in _ctx.C_NOM_Conceptos
                             on fin_det.IdConcepto equals concep.IdConcepto
                             join cl in _ctx.ClavesContables
                             on concep.IdConcepto equals cl.IdConcepto
                          
                             where fin.IdPeriodo == idperiodo && fin.IdEmpresaFiscal == IdEmpresa && cl.IdEmpresa == IdEmpresa 
                             group fin_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, cl.Acredora, cl.Deudora} into temporal
                             select new
                             {
                                 temporal.Key.IdConcepto,
                                 temporal.Key.Descripcion,
                                 temporal.Key.TipoConcepto,
                                 temporal.Key.Obligatorio,                                 
                                 Sumatoria = temporal.Sum(p => p.Total),
                                 temporal.Key.Acredora,
                                 temporal.Key.Deudora,
                                 


                             }).ToList();


                 lista = (from new2 in datos
                             select new AutorizacionDetalle
                             {
                                 IdConcepto = new2.IdConcepto,
                                 NombreConcepto = new2.Descripcion,
                                 TipoConcepto = new2.TipoConcepto,
                                 TotalConcepto = new2.Sumatoria,
                                 Obligacion = new2.Obligatorio,                                 
                                 Acredora = new2.Acredora,
                                 Deudora = new2.Deudora,
                                 ClaveCliente = clave.Clave_Contable
                             }).OrderByDescending(x => x.TipoConcepto).ToList();

                return lista;
            }
            else
            {
                //var empAsimilado = _ctx.NOM_Nomina.Where(x => x.IdEmpresaAsimilado == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
                var empAsimilado = _ctx.NOM_Nomina.Where(x => x.IdEmpresaAsimilado == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
                 clave = _ctx.Sucursal_Empresa.Where(X => X.IdSucursal == empAsimilado.IdSucursal && X.IdEmpresa == IdEmpresa).FirstOrDefault();
                if (empAsimilado != null)// si la busqueda es diferente a NULL se condiera como Asimilado y se lleva acabo el querry 
                {
                    AutorizacionDetalle autorizar = new AutorizacionDetalle();
                    var datos = (from nom in _ctx.NOM_Nomina
                                 join nom_det in _ctx.NOM_Nomina_Detalle
                                 on nom.IdNomina equals nom_det.IdNomina
                                 join concep in _ctx.C_NOM_Conceptos
                                 on nom_det.IdConcepto equals concep.IdConcepto
                                 join cl in _ctx.ClavesContables
                                 on concep.IdConcepto equals cl.IdConcepto

                                 where nom.IdPeriodo == idperiodo && nom.IdEmpresaAsimilado == IdEmpresa && cl.IdEmpresa == IdEmpresa
                                 group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, nom_det.Complemento, cl.Acredora, cl.Deudora } into temporal
                                 select new
                                 {
                                     temporal.Key.IdConcepto,
                                     temporal.Key.Descripcion,
                                     temporal.Key.TipoConcepto,
                                     temporal.Key.Obligatorio,
                                     temporal.Key.Complemento,
                                     Sumatoria = temporal.Sum(p => p.Total),
                                     temporal.Key.Acredora,
                                     temporal.Key.Deudora,



                                 }).Distinct().ToList();



                    lista = (from new2 in datos
                             select new AutorizacionDetalle
                             {
                                 IdConcepto = new2.IdConcepto,
                                 NombreConcepto = new2.Descripcion,
                                 TipoConcepto = new2.TipoConcepto,
                                 TotalConcepto = new2.Sumatoria,
                                 Obligacion = new2.Obligatorio,
                                 Complemento = new2.Complemento,
                                 Acredora = new2.Acredora,
                                 Deudora = new2.Deudora,
                                 ClaveCliente = clave.Clave_Contable
                             }).ToList();


                }
                return lista;
            }



        }
        public List<ListadoColaboradores> conceptosDetalladosByEmpleado(int idperiodo, int IdEmpresa)     
        {
           
            var empFiscal = _ctx.NOM_Nomina.Where(x => x.IdEmpresaFiscal == IdEmpresa && x.IdPeriodo == idperiodo).ToList();
            var listaFinal = new List<ListadoColaboradores>();
            if (empFiscal.Count != 0)// si la busqueda es diferente a NULL se condiera como Fiscal y se lleva acabo el querry 
            {
                foreach (var emp in empFiscal)
                {

                    var item = (from nom in _ctx.NOM_Nomina
                                join detNom in _ctx.NOM_Nomina_Detalle
                                on nom.IdNomina equals detNom.IdNomina
                                join conc in _ctx.C_NOM_Conceptos
                                on detNom.IdConcepto equals conc.IdConcepto
                                join clvCon in _ctx.ClavesContables
                                on conc.IdConcepto equals clvCon.IdConcepto
                                join suc in _ctx.Sucursal_Empresa
                                on nom.IdSucursal equals suc.IdSucursal
                                join e in _ctx.Empleado
                                on nom.IdEmpleado equals e.IdEmpleado
                                where clvCon.IdEmpresa == IdEmpresa && suc.IdEmpresa == IdEmpresa && nom.IdNomina == emp.IdNomina
                                select new AutorizacionDetalle
                                {
                                    IdConcepto = conc.IdConcepto,
                                    NombreConcepto = conc.Descripcion,
                                    TipoConcepto = conc.TipoConcepto,
                                    TotalConcepto = detNom.Total,
                                    Acredora = clvCon.Acredora,
                                    Deudora = clvCon.Deudora,
                                    ClaveCliente = suc.Clave_Contable,
                                    rfc = e.RFC

                                }).ToList();
                    listaFinal.Add(new ListadoColaboradores
                    {
                        listaGeneral = item,
                        IdNomina = emp.IdNomina
                    });
                }

                return listaFinal;
            }
            else
            {
                var empAsimilado = _ctx.NOM_Nomina.Where(x => x.IdEmpresaAsimilado == IdEmpresa && x.IdPeriodo == idperiodo).ToList();
                if (empAsimilado.Count != 0)// si la busqueda es diferente a NULL se condiera como Asimilado y se lleva acabo el querry 
                {

                    foreach (var emp in empAsimilado)
                    {

                        var item = (from nom in _ctx.NOM_Nomina
                                    join detNom in _ctx.NOM_Nomina_Detalle
                                    on nom.IdNomina equals detNom.IdNomina
                                    join conc in _ctx.C_NOM_Conceptos
                                    on detNom.IdConcepto equals conc.IdConcepto
                                    join clvCon in _ctx.ClavesContables
                                    on conc.IdConcepto equals clvCon.IdConcepto
                                    join suc in _ctx.Sucursal_Empresa
                                    on nom.IdSucursal equals suc.IdSucursal
                                    join e in _ctx.Empleado
                                    on nom.IdEmpleado equals e.IdEmpleado
                                    where clvCon.IdEmpresa == IdEmpresa && suc.IdEmpresa == IdEmpresa && nom.IdNomina == emp.IdNomina
                                    select new AutorizacionDetalle
                                    {
                                        IdConcepto = conc.IdConcepto,
                                        NombreConcepto = conc.Descripcion,
                                        TipoConcepto = conc.TipoConcepto,
                                        TotalConcepto = detNom.Total,
                                        Acredora = clvCon.Acredora,
                                        Deudora = clvCon.Deudora,
                                        ClaveCliente = suc.Clave_Contable,
                                        rfc = e.RFC

                                    }).ToList();
                        listaFinal.Add(new ListadoColaboradores
                        {
                            listaGeneral = item,
                            IdNomina = emp.IdNomina
                        });
                    }

               
                }
                return listaFinal;
            }
        }
    }
    public class EmpInci
    {
        public List<Inci> Incidencias { get; set; }
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public int DiasAPagar { get; set; }
        public int Descansos { get; set; }
        public int Asistencias { get; set; }
        public int IncapacidadesR { get; set; }
        public int IncapacidadesE { get; set; }
        public int IncapacidadesM { get; set; }
        public int FaltasI { get; set; }
        public int FaltasA { get; set; }
        public int Faltas { get; set; }
        public int _Vacaciones { get; set; }
    }
    public class Inci
    {
        public string TipoIncidencia { get; set; }

    }
    public class ListadoColaboradores
    {
        public decimal IdNomina { get; set; }
        public List<AutorizacionDetalle> listaGeneral { get; set; }
    }
}
