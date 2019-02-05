using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using MoreLinq;
using Common.Utils;
namespace Nomina.Reportes.Datos
{
   public class ReportesDAO
    {
        RHEntities _ctx = null;

        public ReportesDAO()
        {
            _ctx = new RHEntities();
        }

        #region Autorizar Nomina
        //
        //
        //METODOS DEL REPORTE DE AUTORIZACION 
        //

        /// <summary>
        /// Metodo para Traear las Empresas Fiscales de una Sucursal en especifico
        /// </summary>
        /// <param name="idsucursal"></param>
        /// <returns>
        /// Retorna una lista de Empresas Fiscales de la Sucursal
        /// </returns>
        public List<SucursalesEmpresa> ListSucursalEmpresaFiscales(int idsucursal)
        {


            var datos = (from empresa in _ctx.Empresa.AsEnumerable()
                         join sucursal in _ctx.Sucursal_Empresa.AsEnumerable()
                         on empresa.IdEmpresa equals sucursal.IdEmpresa
                         where sucursal.IdSucursal == idsucursal && sucursal.IdEsquema == 1

                         select new SucursalesEmpresa
                         {
                             IdTabla = empresa.IdEmpresa,
                             Nombre = empresa.RazonSocial,
                             ClaveContable = sucursal.Clave_Contable,
                             CP = empresa.CP,
                             Estado = Convert.ToString(_ctx.C_Estado.Where(x => x.IdEstado == empresa.IdEstado).Select(x => x.Descripcion).FirstOrDefault()),
                             Municipio = empresa.Municipio,
                             Colonia = empresa.Colonia,
                             Calle = empresa.Calle,
                             NoExt = empresa.NoExt,
                             RFC = empresa.RFC,
                             RP = empresa.RegistroPatronal
                         }).ToList();
            return datos.DistinctBy(x => x.Nombre).ToList();

        }

        /// <summary>
        /// Metodo para Traear las Empresas Fiscales y complementarias de una Sucursal en especifico
        /// </summary>
        /// <param name="idsucursal"></param>
        /// <returns></returns>
        public List<SucursalesEmpresa> ListaSucursalesEmpresasConComplemento(int idsucursal,NOM_PeriodosPago periodo)
        {
            var datos = (from empresa in _ctx.Empresa.AsEnumerable()
                         join sucursal in _ctx.Sucursal_Empresa.AsEnumerable()
                         on empresa.IdEmpresa equals sucursal.IdEmpresa
                         where sucursal.IdSucursal == idsucursal && sucursal.IdEsquema == 1 || sucursal.IdEsquema == 2 || sucursal.IdEsquema == 4

                         select new SucursalesEmpresa
                         {
                             IdTabla = empresa.IdEmpresa,
                             Nombre = empresa.RazonSocial,
                             ClaveContable = sucursal.Clave_Contable,
                             CP = empresa.CP,
                             Estado = Convert.ToString(_ctx.C_Estado.Where(x => x.IdEstado == empresa.IdEstado).Select(x => x.Descripcion).FirstOrDefault()),
                             Municipio = empresa.Municipio,
                             Colonia = empresa.Colonia,
                             Calle = empresa.Calle,
                             NoExt = empresa.NoExt,
                             RFC = empresa.RFC,
                             RP = empresa.RegistroPatronal
                         }).ToList();
            return datos.DistinctBy(x => x.Nombre).ToList();
        }

        public List<SucursalesEmpresa> ListaSucursalesEmpresasConSindicato(int idsucursal)
        {
            var datos = (from empresa in _ctx.Empresa.AsEnumerable()
                         join sucursal in _ctx.Sucursal_Empresa.AsEnumerable()
                         on empresa.IdEmpresa equals sucursal.IdEmpresa
                         where sucursal.IdSucursal == idsucursal && sucursal.IdEsquema == 1 || sucursal.IdEsquema == 3

                         select new SucursalesEmpresa
                         {
                             IdTabla = empresa.IdEmpresa,
                             Nombre = empresa.RazonSocial,
                             ClaveContable = sucursal.Clave_Contable,
                             CP = empresa.CP,
                             Estado = Convert.ToString(_ctx.C_Estado.Where(x => x.IdEstado == empresa.IdEstado).Select(x => x.Descripcion).FirstOrDefault()),
                             Municipio = empresa.Municipio,
                             Colonia = empresa.Colonia,
                             Calle = empresa.Calle,
                             NoExt = empresa.NoExt,
                             RFC = empresa.RFC,
                             RP = empresa.RegistroPatronal
                         }).ToList();
            return datos.DistinctBy(x => x.Nombre).ToList();
        }

        /// <summary>
        /// Metodo para Traear el listado de los empleados que existen en el periodo
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <returns></returns>
        public List<EmpleadoNomina> GetIdEmpleadosProcesados(int IdPeriodo)
        {
            var datos = (from nom in _ctx.NOM_Nomina
                         join peri in _ctx.NOM_PeriodosPago
                         on nom.IdPeriodo equals peri.IdPeriodoPago
                         where nom.IdPeriodo == IdPeriodo
                         select new EmpleadoNomina
                         {
                             idempleado = nom.IdEmpleado
                         }).ToList();

            return datos;
        }

        /// <summary>
        /// Obtine los Totales de obligacion de la tabla Cuotas IMSS
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <returns></returns>
        public TotalOblicaciones totalesObligaciones(int IdPeriodo)
        {
            decimal totalIMSS = 0;
            var listaNomina = _ctx.NOM_Nomina.Where(x => x.IdPeriodo == IdPeriodo).Select(x => x.IdNomina).ToArray();

            var rubros = (from CIMSS in _ctx.NOM_Cuotas_IMSS
                          where listaNomina.Contains(CIMSS.IdNomina)
                          select CIMSS).ToList();

            var impuestoNomina = (from impuesto in _ctx.NOM_Nomina
                                  where listaNomina.Contains(impuesto.IdNomina)
                                  select impuesto).ToList();
            TotalOblicaciones totales = new TotalOblicaciones();

            totalIMSS = rubros.Sum(x => x.CesantiaVejez_Patron) + rubros.Sum(x => x.Pensionados_Patron) + rubros.Sum(x => x.PrestacionesDinero_Patron) + rubros.Sum(x => x.Cuota_Fija_Patron) + rubros.Sum(x => x.Excedente_Patron) + rubros.Sum(x => x.InvalidezVida_Patron);
            totales.FondoRetiroSAR = rubros.Sum(x => x.SeguroRetiro_Patron);
            totales.ImpuestoEstatal = impuestoNomina.Sum(x => x.TotalImpuestoSobreNomina);
            totales.TotalGuarderia = rubros.Sum(x => x.GuarderiasPrestaciones_Patron);
            totales.Infonavit = rubros.Sum(x => x.Infonavit_Patron);
            totales.RiesgoDeTrabajo = rubros.Sum(x => x.RiesgoTrabajo_Patron);

            return totales;
        }

        /// <summary>
        /// Trae las Sumas totales de Los rubros IMSS por empresa
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <param name="IdEmpresa"></param>
        /// <returns></returns>
        public TotalesRubrosIMSS HistorialCuotas(int IdPeriodo, int IdEmpresa)
        {
            int[] listaNomina;

            var empresaFiscal = _ctx.NOM_Nomina.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaFiscal == IdEmpresa).Select(x => x.IdNomina).ToArray();
            if (empresaFiscal != null)
            {
                listaNomina = empresaFiscal;
                var lista = (from CIMSS in _ctx.NOM_Cuotas_IMSS
                             where listaNomina.Contains(CIMSS.IdNomina)
                             select CIMSS).ToList();
                TotalesRubrosIMSS Totales = new TotalesRubrosIMSS();

                Totales.TotalCuotoFija = lista.Sum(x => x.Cuota_Fija_Patron);

                Totales.TotalExcedentePatron = lista.Sum(x => x.Excedente_Patron);
                Totales.TotalExcedenteobrero = lista.Sum(x => x.Excedente_Obrero);
                Totales.TotalPrestacionesPatron = lista.Sum(x => x.PrestacionesDinero_Patron);
                Totales.TotalPrestacionesObrero = lista.Sum(x => x.PrestacionesDinero_Obrero);
                Totales.TotalPensionadosPatron = lista.Sum(x => x.Pensionados_Patron);
                Totales.TotalPensionadosObrero = lista.Sum(x => x.Pensionados_Obrero);
                Totales.TotalInvalidezVidaPatron = lista.Sum(x => x.InvalidezVida_Patron);
                Totales.TotalInvalidezVidaObrero = lista.Sum(x => x.InvalidezVida_Obrero);
                Totales.TotalGuarderiasPatron = lista.Sum(x => x.GuarderiasPrestaciones_Patron);
                Totales.TotalSeguroRetiroPatron = lista.Sum(x => x.SeguroRetiro_Patron);
                Totales.TotalCenyVejPatron = lista.Sum(x => x.CesantiaVejez_Patron);
                Totales.TotalCenyVejObrero = lista.Sum(x => x.CesantiaVejez_Obrero);
                Totales.TotalInfonavitPatron = lista.Sum(x => x.Infonavit_Patron);
                Totales.TotalRiesgoTrabajoPatron = lista.Sum(x => x.RiesgoTrabajo_Patron);
                return Totales;
            }
            else
            {
                var empresaAsimilados = _ctx.NOM_Nomina.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaAsimilado == IdEmpresa).Select(x => x.IdNomina).ToArray();
                if (empresaAsimilados != null)
                {
                    listaNomina = empresaFiscal;
                    var lista = (from CIMSS in _ctx.NOM_Cuotas_IMSS
                                 where listaNomina.Contains(CIMSS.IdNomina)
                                 select CIMSS).ToList();
                    TotalesRubrosIMSS Totales = new TotalesRubrosIMSS();

                    Totales.TotalCuotoFija = lista.Sum(x => x.Cuota_Fija_Patron);
                    Totales.TotalExcedentePatron = lista.Sum(x => x.Excedente_Patron);
                    Totales.TotalExcedenteobrero = lista.Sum(x => x.Excedente_Obrero);
                    Totales.TotalPrestacionesPatron = lista.Sum(x => x.PrestacionesDinero_Patron);
                    Totales.TotalPrestacionesObrero = lista.Sum(x => x.PrestacionesDinero_Obrero);
                    Totales.TotalPensionadosPatron = lista.Sum(x => x.Pensionados_Patron);
                    Totales.TotalPensionadosObrero = lista.Sum(x => x.Pensionados_Obrero);
                    Totales.TotalInvalidezVidaPatron = lista.Sum(x => x.InvalidezVida_Patron);
                    Totales.TotalInvalidezVidaObrero = lista.Sum(x => x.InvalidezVida_Obrero);
                    Totales.TotalGuarderiasPatron = lista.Sum(x => x.GuarderiasPrestaciones_Patron);
                    Totales.TotalSeguroRetiroPatron = lista.Sum(x => x.SeguroRetiro_Patron);
                    Totales.TotalCenyVejPatron = lista.Sum(x => x.CesantiaVejez_Patron);
                    Totales.TotalCenyVejObrero = lista.Sum(x => x.CesantiaVejez_Obrero);
                    Totales.TotalInfonavitPatron = lista.Sum(x => x.Infonavit_Patron);
                    Totales.TotalRiesgoTrabajoPatron = lista.Sum(x => x.RiesgoTrabajo_Patron);
                    return Totales;
                }
                else
                {
                    return null;
                }

            }

        }
        /// <summary>
        /// Saca la sumatoria de las obligaciones 
        /// </summary>
        /// <param name="idPeriodo"></param>
        /// <returns></returns>
        public Obligaciones obligacionesGenerales(int IdPeriodo,int IdEmpresa)
        {
            var empresaFiscal = _ctx.NOM_Nomina.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaFiscal == IdEmpresa).Select(x => x.IdNomina).ToArray();
            if(empresaFiscal != null)
            {
                var datos = (from nom in _ctx.NOM_Nomina
                             join cuot in _ctx.NOM_Cuotas_IMSS
                             on nom.IdNomina equals cuot.IdNomina
                             where nom.IdPeriodo == IdPeriodo && nom.IdEmpresaFiscal == IdEmpresa
                             select new Obligaciones
                             {
                                 idNomina = nom.IdNomina,
                                 FondoRetiro = cuot.SeguroRetiro_Patron,
                                 ImpuestoNomina = nom.TotalImpuestoSobreNomina,
                                 Guarderia = cuot.GuarderiasPrestaciones_Patron,
                                 IMSSEmpresa = cuot.CesantiaVejez_Patron + cuot.Pensionados_Patron + cuot.Cuota_Fija_Patron + cuot.Excedente_Patron + cuot.InvalidezVida_Patron,
                                 RiesgoTrabajo = cuot.RiesgoTrabajo_Patron

                             }).ToList();

                Obligaciones obliga = new Obligaciones();
                foreach (var d in datos)
                {
                    obliga.FondoRetiro = obliga.FondoRetiro +d.FondoRetiro;
                    obliga.ImpuestoNomina = obliga.ImpuestoNomina + d.ImpuestoNomina;
                    obliga.Guarderia = obliga.Guarderia +d.Guarderia;
                    obliga.IMSSEmpresa = obliga.IMSSEmpresa +d.IMSSEmpresa;
                    obliga.RiesgoTrabajo =obliga.RiesgoTrabajo +d.RiesgoTrabajo;
                }
                return obliga;
            }
            else
            {
                var empresaAsim = _ctx.NOM_Nomina.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaFiscal == IdEmpresa).Select(x => x.IdNomina).ToArray();
                if (empresaAsim != null)
                {
                    var datos = (from nom in _ctx.NOM_Nomina
                                 join cuot in _ctx.NOM_Cuotas_IMSS
                                 on nom.IdNomina equals cuot.IdNomina
                                 where nom.IdPeriodo == IdPeriodo && nom.IdEmpresaFiscal == IdEmpresa
                                 select new Obligaciones
                                 {
                                     idNomina = nom.IdNomina,
                                     FondoRetiro = cuot.SeguroRetiro_Patron,
                                     ImpuestoNomina = nom.TotalImpuestoSobreNomina,
                                     Guarderia = cuot.GuarderiasPrestaciones_Patron,
                                     IMSSEmpresa = cuot.CesantiaVejez_Patron + cuot.Pensionados_Patron + cuot.Cuota_Fija_Patron + cuot.Excedente_Patron + cuot.InvalidezVida_Patron,
                                     RiesgoTrabajo = cuot.RiesgoTrabajo_Patron

                                 }).ToList();

                    Obligaciones obliga = new Obligaciones();
                    foreach (var d in datos)
                    {
                        obliga.FondoRetiro = +d.FondoRetiro;
                        obliga.ImpuestoNomina = +d.ImpuestoNomina;
                        obliga.Guarderia = +d.Guarderia;
                        obliga.IMSSEmpresa = +d.IMSSEmpresa;
                        obliga.RiesgoTrabajo = +d.RiesgoTrabajo;
                    }
                    return obliga;
                }
                else
                {
                    return null;
                }
            }
            
        }

        public List<AutorizacionDetalle> autorizaciondetalleByEmpresa(int idperiodo, int IdEmpresa)
        {

            var empFiscal = _ctx.NOM_Nomina.Where(x => x.IdEmpresaFiscal == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
            if (empFiscal != null)// si la busqueda es diferente a NULL se condiera como Fiscal y se lleva acabo el querry 
            {
                AutorizacionDetalle autorizar = new AutorizacionDetalle();
                var datos = (from nom in _ctx.NOM_Nomina
                             join nom_det in _ctx.NOM_Nomina_Detalle
                             on nom.IdNomina equals nom_det.IdNomina
                             join concep in _ctx.C_NOM_Conceptos
                             on nom_det.IdConcepto equals concep.IdConcepto
                             where nom.IdPeriodo == idperiodo && nom.IdEmpresaFiscal == IdEmpresa
                             group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, nom_det.Complemento } into temporal
                             select new
                             {
                                 temporal.Key.IdConcepto,
                                 temporal.Key.Descripcion,
                                 temporal.Key.TipoConcepto,
                                 temporal.Key.Obligatorio,
                                 temporal.Key.Complemento,
                                 Sumatoria = temporal.Sum(p => p.Total)


                             }).ToList();


                var lista = (from new2 in datos
                             select new AutorizacionDetalle
                             {
                                 IdConcepto = new2.IdConcepto,
                                 NombreConcepto = new2.Descripcion,
                                 TipoConcepto = new2.TipoConcepto,
                                 TotalConcepto = new2.Sumatoria,
                                 Obligacion = new2.Obligatorio,
                                 Complemento = new2.Complemento
                             }).ToList();

                return lista;
            }
            else
            {
                var empAsimilado = _ctx.NOM_Nomina.Where(x => x.IdEmpresaAsimilado == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
                if (empAsimilado != null)// si la busqueda es diferente a NULL se condiera como Asimilado y se lleva acabo el querry 
                {
                    AutorizacionDetalle autorizar = new AutorizacionDetalle();
                    var datos = (from nom in _ctx.NOM_Nomina
                                 join nom_det in _ctx.NOM_Nomina_Detalle
                                 on nom.IdNomina equals nom_det.IdNomina
                                 join concep in _ctx.C_NOM_Conceptos
                                 on nom_det.IdConcepto equals concep.IdConcepto
                                 where nom.IdPeriodo == idperiodo && nom.IdEmpresaAsimilado == IdEmpresa
                                 group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, nom_det.Complemento } into temporal
                                 select new
                                 {
                                     temporal.Key.IdConcepto,
                                     temporal.Key.Descripcion,
                                     temporal.Key.TipoConcepto,
                                     temporal.Key.Obligatorio,
                                     temporal.Key.Complemento,
                                     Sumatoria = temporal.Sum(p => p.Total)


                                 }).ToList();


                    var lista = (from new2 in datos
                                 select new AutorizacionDetalle
                                 {
                                     IdConcepto = new2.IdConcepto,
                                     NombreConcepto = new2.Descripcion,
                                     TipoConcepto = new2.TipoConcepto,
                                     TotalConcepto = new2.Sumatoria,
                                     Obligacion = new2.Obligatorio,
                                     Complemento = new2.Complemento
                                 }).ToList();

                    return lista;
                }
                else
                {
                    var empComplemento = _ctx.NOM_Nomina.Where(x => x.IdEmpresaComplemento == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
                    if (empComplemento != null)// si la busqueda es diferente a NULL se condiera como Complemento y se lleva acabo el querry 
                    {
                        AutorizacionDetalle autorizar = new AutorizacionDetalle();
                        var datos = (from nom in _ctx.NOM_Nomina
                                     join nom_det in _ctx.NOM_Nomina_Detalle
                                     on nom.IdNomina equals nom_det.IdNomina
                                     join concep in _ctx.C_NOM_Conceptos
                                     on nom_det.IdConcepto equals concep.IdConcepto
                                     where nom.IdPeriodo == idperiodo && nom.IdEmpresaComplemento == IdEmpresa
                                     group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, nom_det.Complemento } into temporal
                                     select new
                                     {
                                         temporal.Key.IdConcepto,
                                         temporal.Key.Descripcion,
                                         temporal.Key.TipoConcepto,
                                         temporal.Key.Obligatorio,
                                         temporal.Key.Complemento,
                                         Sumatoria = temporal.Sum(p => p.Total)


                                     }).ToList();


                        var lista = (from new2 in datos
                                     select new AutorizacionDetalle
                                     {
                                         IdConcepto = new2.IdConcepto,
                                         NombreConcepto = new2.Descripcion,
                                         TipoConcepto = new2.TipoConcepto,
                                         TotalConcepto = new2.Sumatoria,
                                         Obligacion = new2.Obligatorio,
                                         Complemento = new2.Complemento
                                     }).ToList();

                        return lista;
                    }
                    else
                    {
                        // Si al final no es ninguno de los otros 3 casos se considera que es una empresa Sindicado 
                        AutorizacionDetalle autorizar = new AutorizacionDetalle();
                        var datos = (from nom in _ctx.NOM_Nomina
                                     join nom_det in _ctx.NOM_Nomina_Detalle
                                     on nom.IdNomina equals nom_det.IdNomina
                                     join concep in _ctx.C_NOM_Conceptos
                                     on nom_det.IdConcepto equals concep.IdConcepto
                                     where nom.IdPeriodo == idperiodo && nom.IdEmpresaSindicato == IdEmpresa
                                     group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, nom_det.Complemento } into temporal
                                     select new
                                     {
                                         temporal.Key.IdConcepto,
                                         temporal.Key.Descripcion,
                                         temporal.Key.TipoConcepto,
                                         temporal.Key.Obligatorio,
                                         temporal.Key.Complemento,
                                         Sumatoria = temporal.Sum(p => p.Total)


                                     }).ToList();


                        var lista = (from new2 in datos
                                     select new AutorizacionDetalle
                                     {
                                         IdConcepto = new2.IdConcepto,
                                         NombreConcepto = new2.Descripcion,
                                         TipoConcepto = new2.TipoConcepto,
                                         TotalConcepto = new2.Sumatoria,
                                         Obligacion = new2.Obligatorio,
                                         Complemento = new2.Complemento
                                     }).ToList();

                        return lista;
                    }

                }
            }



        }
        /// <summary>
        /// Trae las Sumas totales de Los rubros IMSS Finiquito
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <param name="IdEmpresa"></param>
        /// <returns></returns>
        public TotalesRubrosIMSS HistorialCuotasFiniquito(int IdPeriodo, int IdEmpresa)
        {
            int[] listaNomina;

            var empresaFiscal = _ctx.NOM_Finiquito.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaFiscal == IdEmpresa).Select(x => x.IdFiniquito).ToArray();
            if (empresaFiscal != null)
            {
                listaNomina = empresaFiscal;
                var lista = (from CIMSS in _ctx.NOM_Cuotas_IMSS
                             where listaNomina.Contains(CIMSS.IdFiniquito)
                             select CIMSS).ToList();
                TotalesRubrosIMSS Totales = new TotalesRubrosIMSS();

                Totales.TotalCuotoFija = lista.Sum(x => x.Cuota_Fija_Patron);

                Totales.TotalExcedentePatron = lista.Sum(x => x.Excedente_Patron);
                Totales.TotalExcedenteobrero = lista.Sum(x => x.Excedente_Obrero);
                Totales.TotalPrestacionesPatron = lista.Sum(x => x.PrestacionesDinero_Patron);
                Totales.TotalPrestacionesObrero = lista.Sum(x => x.PrestacionesDinero_Obrero);
                Totales.TotalPensionadosPatron = lista.Sum(x => x.Pensionados_Patron);
                Totales.TotalPensionadosObrero = lista.Sum(x => x.Pensionados_Obrero);
                Totales.TotalInvalidezVidaPatron = lista.Sum(x => x.InvalidezVida_Patron);
                Totales.TotalInvalidezVidaObrero = lista.Sum(x => x.InvalidezVida_Obrero);
                Totales.TotalGuarderiasPatron = lista.Sum(x => x.GuarderiasPrestaciones_Patron);
                Totales.TotalSeguroRetiroPatron = lista.Sum(x => x.SeguroRetiro_Patron);
                Totales.TotalCenyVejPatron = lista.Sum(x => x.CesantiaVejez_Patron);
                Totales.TotalCenyVejObrero = lista.Sum(x => x.CesantiaVejez_Obrero);
                Totales.TotalInfonavitPatron = lista.Sum(x => x.Infonavit_Patron);
                Totales.TotalRiesgoTrabajoPatron = lista.Sum(x => x.RiesgoTrabajo_Patron);
                return Totales;
            }
            else
            {
                var empresaAsimilados = _ctx.NOM_Finiquito.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaAsimilado == IdEmpresa).Select(x => x.IdFiniquito).ToArray();
                if (empresaAsimilados != null)
                {
                    listaNomina = empresaFiscal;
                    var lista = (from CIMSS in _ctx.NOM_Cuotas_IMSS
                                 where listaNomina.Contains(CIMSS.IdNomina)
                                 select CIMSS).ToList();
                    TotalesRubrosIMSS Totales = new TotalesRubrosIMSS();

                    Totales.TotalCuotoFija = lista.Sum(x => x.Cuota_Fija_Patron);
                    Totales.TotalExcedentePatron = lista.Sum(x => x.Excedente_Patron);
                    Totales.TotalExcedenteobrero = lista.Sum(x => x.Excedente_Obrero);
                    Totales.TotalPrestacionesPatron = lista.Sum(x => x.PrestacionesDinero_Patron);
                    Totales.TotalPrestacionesObrero = lista.Sum(x => x.PrestacionesDinero_Obrero);
                    Totales.TotalPensionadosPatron = lista.Sum(x => x.Pensionados_Patron);
                    Totales.TotalPensionadosObrero = lista.Sum(x => x.Pensionados_Obrero);
                    Totales.TotalInvalidezVidaPatron = lista.Sum(x => x.InvalidezVida_Patron);
                    Totales.TotalInvalidezVidaObrero = lista.Sum(x => x.InvalidezVida_Obrero);
                    Totales.TotalGuarderiasPatron = lista.Sum(x => x.GuarderiasPrestaciones_Patron);
                    Totales.TotalSeguroRetiroPatron = lista.Sum(x => x.SeguroRetiro_Patron);
                    Totales.TotalCenyVejPatron = lista.Sum(x => x.CesantiaVejez_Patron);
                    Totales.TotalCenyVejObrero = lista.Sum(x => x.CesantiaVejez_Obrero);
                    Totales.TotalInfonavitPatron = lista.Sum(x => x.Infonavit_Patron);
                    Totales.TotalRiesgoTrabajoPatron = lista.Sum(x => x.RiesgoTrabajo_Patron);
                    return Totales;
                }
                else
                {
                    return null;
                }

            }

        }
        /// <summary>
        /// Obtine los Totales de obligacion de la tabla Cuotas IMSS Finiquito
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <returns></returns>
        public TotalOblicaciones totalesObligacionesFiniquito(int IdPeriodo)
        {
            decimal totalIMSS = 0;
            int idFiniquito = _ctx.NOM_Finiquito.Where(x => x.IdPeriodo == IdPeriodo).Select(x => x.IdFiniquito).FirstOrDefault();

            var rubros = (from CIMSS in _ctx.NOM_Cuotas_IMSS
                          where CIMSS.IdFiniquito == idFiniquito
                          select CIMSS).ToList();

            var impuestoNomina = (from fin_det in _ctx.NOM_Finiquito_Detalle
                                  where fin_det.IdFiniquito == idFiniquito
                                  select fin_det).ToList();
            TotalOblicaciones totales = new TotalOblicaciones();

            totalIMSS = rubros.Sum(x => x.CesantiaVejez_Patron) + rubros.Sum(x => x.Pensionados_Patron) + rubros.Sum(x => x.PrestacionesDinero_Patron) + rubros.Sum(x => x.Cuota_Fija_Patron) + rubros.Sum(x => x.Excedente_Patron) + rubros.Sum(x => x.InvalidezVida_Patron);
            totales.FondoRetiroSAR = rubros.Sum(x => x.SeguroRetiro_Patron);
            totales.ImpuestoEstatal = impuestoNomina.Sum(x => x.ImpuestoSobreNomina);
            totales.TotalGuarderia = rubros.Sum(x => x.GuarderiasPrestaciones_Patron);
            totales.Infonavit = rubros.Sum(x => x.Infonavit_Patron);
            totales.RiesgoDeTrabajo = rubros.Sum(x => x.RiesgoTrabajo_Patron);

            return totales;
        }
        /// <summary>
        /// Saca la sumatoria de las obligaciones Finiquito
        /// </summary>
        /// <param name="idPeriodo"></param>
        /// <returns></returns>
        public Obligaciones obligacionesGeneralesFiniquito(int IdPeriodo, int IdEmpresa)
        {
            int idFiniquito = _ctx.NOM_Finiquito.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaFiscal == IdEmpresa).Select(x => x.IdFiniquito).FirstOrDefault();

            var conceptos = _ctx.NOM_Finiquito_Detalle.Where(x => x.IdFiniquito == idFiniquito).ToList();
            var impuesto = conceptos.Sum(x => x.ImpuestoSobreNomina);
            if (idFiniquito != 0)
            {
                var datos = (from fin in _ctx.NOM_Finiquito
                             join cuot in _ctx.NOM_Cuotas_IMSS
                             on fin.IdFiniquito equals cuot.IdFiniquito
                             where fin.IdPeriodo == IdPeriodo && fin.IdEmpresaFiscal == IdEmpresa
                             select new Obligaciones
                             {
                                 idNomina = fin.IdFiniquito,
                                 FondoRetiro = cuot.SeguroRetiro_Patron,
                                 ImpuestoNomina = impuesto,
                                 Guarderia = cuot.GuarderiasPrestaciones_Patron,
                                 IMSSEmpresa = cuot.CesantiaVejez_Patron + cuot.Pensionados_Patron + cuot.Cuota_Fija_Patron + cuot.Excedente_Patron + cuot.InvalidezVida_Patron,
                                 RiesgoTrabajo = cuot.RiesgoTrabajo_Patron

                             }).ToList();

                Obligaciones obliga = new Obligaciones();
                foreach (var d in datos)
                {
                    obliga.FondoRetiro = obliga.FondoRetiro + d.FondoRetiro;
                    obliga.ImpuestoNomina = obliga.ImpuestoNomina + d.ImpuestoNomina;
                    obliga.Guarderia = obliga.Guarderia + d.Guarderia;
                    obliga.IMSSEmpresa = obliga.IMSSEmpresa + d.IMSSEmpresa;
                    obliga.RiesgoTrabajo = obliga.RiesgoTrabajo + d.RiesgoTrabajo;
                }
                return obliga;
            }
            else
            {
                int idFiniquito2 = _ctx.NOM_Finiquito.Where(x => x.IdPeriodo == IdPeriodo && x.IdEmpresaFiscal == IdEmpresa).Select(x => x.IdFiniquito).FirstOrDefault();
                if (idFiniquito2 != 0)
                {
                    var datos = (from nom in _ctx.NOM_Nomina
                                 join cuot in _ctx.NOM_Cuotas_IMSS
                                 on nom.IdNomina equals cuot.IdFiniquito
                                 where nom.IdPeriodo == IdPeriodo && nom.IdEmpresaFiscal == IdEmpresa
                                 select new Obligaciones
                                 {
                                     idNomina = nom.IdNomina,
                                     FondoRetiro = cuot.SeguroRetiro_Patron,
                                     ImpuestoNomina = impuesto,
                                     Guarderia = cuot.GuarderiasPrestaciones_Patron,
                                     IMSSEmpresa = cuot.CesantiaVejez_Patron + cuot.Pensionados_Patron + cuot.Cuota_Fija_Patron + cuot.Excedente_Patron + cuot.InvalidezVida_Patron,
                                     RiesgoTrabajo = cuot.RiesgoTrabajo_Patron

                                 }).ToList();

                    Obligaciones obliga = new Obligaciones();
                    foreach (var d in datos)
                    {
                        obliga.FondoRetiro = +d.FondoRetiro;
                        obliga.ImpuestoNomina = +d.ImpuestoNomina;
                        obliga.Guarderia = +d.Guarderia;
                        obliga.IMSSEmpresa = +d.IMSSEmpresa;
                        obliga.RiesgoTrabajo = +d.RiesgoTrabajo;
                    }
                    return obliga;
                }
                else
                {
                    return null;
                }
            }

        }
        public List<AutorizacionDetalle> autorizaciondetalleByEmpresaFiniquito(int idperiodo, int IdEmpresa)
        {

            var empFiscal = _ctx.NOM_Finiquito.Where(x => x.IdEmpresaFiscal == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
            if (empFiscal != null)// si la busqueda es diferente a NULL se condiera como Fiscal y se lleva acabo el querry 
            {
                AutorizacionDetalle autorizar = new AutorizacionDetalle();
                var datos = (from nom in _ctx.NOM_Finiquito
                             join nom_det in _ctx.NOM_Finiquito_Detalle
                             on nom.IdFiniquito equals nom_det.IdFiniquito
                             join concep in _ctx.C_NOM_Conceptos
                             on nom_det.IdConcepto equals concep.IdConcepto
                             where nom.IdPeriodo == idperiodo && nom.IdEmpresaFiscal == IdEmpresa
                             group nom_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio, } into temporal
                             select new
                             {
                                 temporal.Key.IdConcepto,
                                 temporal.Key.Descripcion,
                                 temporal.Key.TipoConcepto,
                                 temporal.Key.Obligatorio,
                                 
                                 Sumatoria = temporal.Sum(p => p.Total)


                             }).ToList();


                var lista = (from new2 in datos
                             select new AutorizacionDetalle
                             {
                                 IdConcepto = new2.IdConcepto,
                                 NombreConcepto = new2.Descripcion,
                                 TipoConcepto = new2.TipoConcepto,
                                 TotalConcepto = new2.Sumatoria,
                                 Obligacion = new2.Obligatorio,
                                 
                             }).ToList();

                return lista;
            }
            else
            {
                var empAsimilado = _ctx.NOM_Finiquito.Where(x => x.IdEmpresaAsimilado == IdEmpresa && x.IdPeriodo == idperiodo).FirstOrDefault();
                if (empAsimilado != null)// si la busqueda es diferente a NULL se condiera como Asimilado y se lleva acabo el querry 
                {
                    AutorizacionDetalle autorizar = new AutorizacionDetalle();
                    var datos = (from fin in _ctx.NOM_Finiquito
                                 join fin_det in _ctx.NOM_Finiquito_Detalle
                                 on fin.IdFiniquito equals fin_det.IdFiniquito
                                 join concep in _ctx.C_NOM_Conceptos
                                 on fin_det.IdConcepto equals concep.IdConcepto
                                 where fin.IdPeriodo == idperiodo && fin.IdEmpresaAsimilado == IdEmpresa
                                 group fin_det by new { concep.IdConcepto, concep.Descripcion, concep.TipoConcepto, concep.Obligatorio } into temporal
                                 select new
                                 {
                                     temporal.Key.IdConcepto,
                                     temporal.Key.Descripcion,
                                     temporal.Key.TipoConcepto,
                                     temporal.Key.Obligatorio,
                                     
                                     Sumatoria = temporal.Sum(p => p.Total)


                                 }).ToList();


                    var lista = (from new2 in datos
                                 select new AutorizacionDetalle
                                 {
                                     IdConcepto = new2.IdConcepto,
                                     NombreConcepto = new2.Descripcion,
                                     TipoConcepto = new2.TipoConcepto,
                                     TotalConcepto = new2.Sumatoria,
                                     Obligacion = new2.Obligatorio,
                                     
                                 }).ToList();

                    return lista;
                }
                else
                {
                    return null;
                }

            }



        }
        public EmpleadoDatosNominaViewModel DatosEmpleadoViewModel(int id)
        {
            //var contra =
            //   ctx.Empleado_Contrato.Where(x => x.IdEmpleado == id)
            //       .OrderByDescending(d => d.IdContrato)
            //       .FirstOrDefault();
            var datos =(from e in _ctx.Empleado.AsEnumerable()
                       let c = _ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault()
                       let ef = _ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaFiscal).Select(x => x.RazonSocial).FirstOrDefault()
                       let ec = _ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault()
                       let ea = _ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaAsimilado).Select(x => x.RazonSocial).FirstOrDefault()
                       let es = _ctx.Empresa.Where(x => x.IdEmpresa == c.IdEmpresaSindicato).Select(x => x.RazonSocial).FirstOrDefault()
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
                           EmpresaFiscal = ef,
                           EmpresaComplemento = ec,
                           EmpresaAsimilado = ea,
                           EmpresaSindicato = es,
                           Puesto = ps.Descripcion,
                           Departamento = dp.Descripcion
                       }
                           ).FirstOrDefault();
            return datos;

        }

        public NOM_Nomina EmpleadoNominaByPeriodo(int IdEmpleado, int IdPeriodo)
        {
            var resultado = _ctx.NOM_Nomina.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodo == IdPeriodo).FirstOrDefault();
            return resultado;
        }

        public DatosDispersion EmpleadoDispersion(int idPeriodo, int idEmpleado)
        {
            var resultado = (from e in _ctx.Empleado
                             let nom = _ctx.NOM_Nomina.Where(x => x.IdEmpleado == e.IdEmpleado && x.IdPeriodo == idPeriodo).FirstOrDefault()
                             let ec = _ctx.Empleado_Contrato.Where(x => x.IdEmpleado == e.IdEmpleado).OrderByDescending(x => x.IdContrato).FirstOrDefault()
                             let f = _ctx.C_Metodos_Pago.Where(x => x.IdMetodo == ec.FormaPago).FirstOrDefault()
                             where e.IdEmpleado == idEmpleado
                             select new DatosDispersion
                             {
                                 nombres = e.Nombres,
                                 paterno = e.APaterno,
                                 materno = e.AMaterno,
                                 importePagar = nom.TotalNomina,
                                 totalComplemento = nom.TotalComplemento,
                                 MetodoPago = f.Descripcion,
                                 idNomina = nom.IdNomina
                             }

                            ).FirstOrDefault();
            return resultado;
        }

        public DatosDispersion DatosBancarios (int idEmpleado)
        {
            var resultado = (from datosB in _ctx.DatosBancarios
                             join banco in _ctx.C_Banco_SAT
                             on datosB.IdBanco equals banco.IdBanco
                             where datosB.IdEmpleado == idEmpleado
                             select new DatosDispersion
                             {
                                 cuenta = datosB.CuentaBancaria,
                                 nosigaF = datosB.NoSigaF,
                                 nosigaC = datosB.NoSigaC,
                                idBanco = datosB.IdBanco
                             }).FirstOrDefault();

            return resultado;      
        }
        public List<NominaDetalle> EmpleadoNomDetallePercepcion(int IdNomina, bool complemento)
        {
            var detalle = from nd in _ctx.NOM_Nomina_Detalle
                          join nc in _ctx.C_NOM_Conceptos
                          on nd.IdConcepto equals nc.IdConcepto
                          where nd.IdNomina == IdNomina && nc.TipoConcepto == 1
                          select new NominaDetalle
                          {
                              Detalle = nc.Descripcion,
                              Cantidad = nd.Total,
                              complemento = nd.Complemento
                          };
            return detalle.ToList();

        }
        public List<NominaDetalle> EmpleadoNomDetalleDeduccion(int IdNomina)
        {
            var detalle = from nd in _ctx.NOM_Nomina_Detalle
                          join nc in _ctx.C_NOM_Conceptos
                          on nd.IdConcepto equals nc.IdConcepto
                          where nd.IdNomina == IdNomina && nc.TipoConcepto == 2
                          select new NominaDetalle
                          {
                              Detalle = nc.Descripcion,
                              Cantidad = nd.Total
                          };
            return detalle.ToList();

        }
        #endregion
        public List<NominaDetalle> EmpleadoFinDetallePercepcion(int idFiniquito, int idPeriodo)
        {
            var detalle = new List<NominaDetalle>();
            var item = from nd in _ctx.NOM_Finiquito_Detalle
                          join nc in _ctx.C_NOM_Conceptos
                          on nd.IdConcepto equals nc.IdConcepto
                          where nd.IdFiniquito == idFiniquito && nc.TipoConcepto == 1
                          select new NominaDetalle
                          {
                              Detalle = nc.Descripcion,
                              Cantidad = nd.Total,
                              
                          };
            detalle.AddRange(item);

            var item2 = from nd in _ctx.NOM_Finiquito_Descuento_Adicional
                        join nc in _ctx.C_NOM_Conceptos
                        on nd.IdConcepto equals nc.IdConcepto
                        where nd.IdPeriodo == idPeriodo && nc.TipoConcepto == 1 && nd.IsComplemento == false
                        select new NominaDetalle
                        {
                            Detalle = nc.Descripcion,
                            Cantidad = nd.TotalDescuento,

                        };
            detalle.AddRange(item2);
            return detalle.ToList();

        }
        public List<NominaDetalle> EmpleadoFinDetalleDeduccion(int idFiniquito,int idPeriodo)
        {
            var detalle = new List<NominaDetalle>();
            var item = from nd in _ctx.NOM_Finiquito_Detalle
                          join nc in _ctx.C_NOM_Conceptos
                          on nd.IdConcepto equals nc.IdConcepto
                          where nd.IdFiniquito == idFiniquito && nc.TipoConcepto == 2
                          select new NominaDetalle
                          {
                              Detalle = nc.Descripcion,
                              Cantidad = nd.Total
                          };

            detalle.AddRange(item);

            var item2 = from nd in _ctx.NOM_Finiquito_Descuento_Adicional
                        join nc in _ctx.C_NOM_Conceptos
                        on nd.IdConcepto equals nc.IdConcepto
                        where nd.IdPeriodo == idPeriodo && nc.TipoConcepto == 1 && nd.IsComplemento == false
                        select new NominaDetalle
                        {
                            Detalle = nc.Descripcion,
                            Cantidad = nd.TotalDescuento,

                        };
            detalle.AddRange(item2);
            return detalle.ToList();

        }


        /// <summary>
        /// Obtiene listado de datos bancarios de los empleados por empresa  para el reporte de dispersion
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public DatosBancarios datosBancariosByEmpresa(int idEmpleado)
        {
            var datosb = new DatosBancarios {
                Id=0,
                IdEmpleado=idEmpleado,
                IdBanco=1,
                NoSigaF=0,
                NoSigaC=0,
                CuentaBancaria= "0000000000",
                NumeroTarjeta= "0000000000000000",
                Clabe= "000000000000000000",
                ParentezcoBeneficiario="0",
                Status=true
            };
            var ldatosb = new List<DatosBancarios>();
            
            var listado = _ctx.DatosBancarios.Where(x => x.IdEmpleado == idEmpleado).FirstOrDefault();
            if (listado == null)
            {
                _ctx.DatosBancarios.Add(datosb);
                _ctx.SaveChanges();
                listado = datosb;
            }
        

            return listado;
        }
        /// <summary>
        /// Obtiene el listado del detalle para el reporte de finiquito
        /// </summary>
        /// <returns></returns>
        public List<NominaDetalle> detalleFiniquito(int idfiniquito)
        {
            var datos = (from det in _ctx.NOM_Finiquito_Detalle
                         join concept in _ctx.C_NOM_Conceptos
                         on det.IdConcepto equals concept.IdConcepto
                         where det.IdFiniquito == idfiniquito
                         select new NominaDetalle
                         {
                             Detalle = concept.DescripcionCorta,
                             Cantidad = det.Total,
                             tipoConcpeto = concept.TipoConcepto

                         }).ToList();
            return datos;
        }





        public ListadoBancoEmpleados datosBancoEmpleados(int idBanco, List<NOM_Nomina> nominas,int idEmpresa)
        {
            ListadoBancoEmpleados lista = new ListadoBancoEmpleados();
            decimal total = 0;
            List<RegistroEmpleados> registros = new List<RegistroEmpleados>();
            RHEntities ctx = new RHEntities();
            var banco = ctx.C_Banco_SAT.Where(x => x.IdBanco == idBanco).FirstOrDefault();

            lista.IdBanco = banco.IdBanco;
            lista.NombreBanco = banco.Descripcion;
       
            foreach (var emp in nominas)
            {
                RegistroEmpleados item = new RegistroEmpleados();
                var datosbanco = DatosBancarios(emp.IdEmpleado);
                var empleadoRegistro = EmpleadoDispersion(emp.IdPeriodo, emp.IdEmpleado);
                if (datosbanco != null)
                {
                    
                    if(banco.IdBanco == datosbanco.idBanco)
                    {
                        if(idEmpresa == emp.IdEmpresaFiscal || idEmpresa == emp.IdEmpresaAsimilado)
                        {
                            if (emp.TotalNomina > 0)
                            {
                                item.NoSigaF = datosbanco.nosigaF;
                                item.NoSigaC = 0;
                                item.Empleado =  empleadoRegistro.paterno + " " + empleadoRegistro.materno + " " + empleadoRegistro.nombres ;
                                item.Importe = empleadoRegistro.importePagar;
                                item.cuenta = datosbanco.cuenta;
                                item.tipocuenta = empleadoRegistro.MetodoPago;
                                item.MetodoPago = emp.IdMetodo_Pago;
                                item.idnomina = empleadoRegistro.idNomina;
                                var tot = Utils.TruncateDecimales(item.Importe);
                                total = total + tot;
                                registros.Add(item);
                            }
                        }
                        else if(idEmpresa == emp.IdEmpresaComplemento )
                        {
                            if (emp.TotalComplemento > 0)
                            {
                                item.NoSigaF = 0;
                                item.NoSigaC = datosbanco.nosigaC;
                                item.Empleado = empleadoRegistro.paterno + " " + empleadoRegistro.materno + " " + empleadoRegistro.nombres;
                                item.Importe = emp.TotalComplemento;
                                item.cuenta = datosbanco.cuenta;
                                item.tipocuenta = empleadoRegistro.MetodoPago;
                                item.MetodoPago = emp.IdMetodo_Pago;
                                item.idnomina = empleadoRegistro.idNomina;
                                var tot = Utils.TruncateDecimales(item.Importe);
                                total = total + tot;
                                registros.Add(item);
                            }
                        }else if(idEmpresa == emp.IdEmpresaSindicato)
                        {
                            if (emp.TotalComplemento == 0)
                            {
                                item.NoSigaF = 0;
                                item.NoSigaC = datosbanco.nosigaC;
                                item.Empleado = empleadoRegistro.paterno + " " + empleadoRegistro.materno + " " + empleadoRegistro.nombres;
                                item.Importe = emp.TotalNomina;
                                item.cuenta = datosbanco.cuenta;
                                item.tipocuenta = empleadoRegistro.MetodoPago;
                                item.MetodoPago = emp.IdMetodo_Pago;
                                item.idnomina = empleadoRegistro.idNomina;
                                var tot = Utils.TruncateDecimales(item.Importe);
                                total = total + tot;
                                registros.Add(item);
                            }
                        }
                  
                    }
                }
            }
            lista.empleados = registros;
            var totalTruncate = Utils.TruncateDecimales(total);
            lista.Total = totalTruncate;
            return lista;
        }

        

    }
    public class SucursalesEmpresa
    {
        public int IdTabla { get; set; }
        public int IdSucursal { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string ClaveContable { get; set; }
        public string CP { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public string NoExt { get; set; }
        public string Calle { get; set; }
        public string RP { get; set; }
        public string RFC { get; set; }

    }

    public class EmpleadoNomina
    {
        public int idempleado { get; set; }
    }
    public class TotalOblicaciones
    {
        public decimal FondoRetiroSAR { get; set; }
        public decimal ImpuestoEstatal { get; set; }
        public decimal TotalGuarderia { get; set; }
        public decimal TotalIMSS { get; set; }
        public decimal Infonavit { get; set; }
        public decimal RiesgoDeTrabajo { get; set; }
    }
    public class TotalesRubrosIMSS
    {
        public decimal TotalCuotoFija { get; set; }
        public decimal TotalCenyVejPatron { get; set; }
        public decimal TotalCenyVejObrero { get; set; }
        public decimal TotalExcedentePatron { get; set; }
        public decimal TotalExcedenteobrero { get; set; }
        public decimal TotalPrestacionesPatron { get; set; }
        public decimal TotalPrestacionesObrero { get; set; }
        public decimal TotalPensionadosPatron { get; set; }
        public decimal TotalPensionadosObrero { get; set; }
        public decimal TotalInvalidezVidaPatron { get; set; }
        public decimal TotalInvalidezVidaObrero { get; set; }
        public decimal TotalGuarderiasPatron { get; set; }
        public decimal TotalSeguroRetiroPatron { get; set; }
        public decimal TotalInfonavitPatron { get; set; }
        public decimal TotalRiesgoTrabajoPatron { get; set; }
    }
    public class AutorizacionDetalle
    {
        public string NombreConcepto { get; set; }
        public int? TipoConcepto { get; set; }
        public decimal TotalConcepto { get; set; }
        public int IdConcepto { get; set; }
        public bool? Obligacion { get; set; }
        public bool Complemento { get; set; }
        public string Acredora { get; set; }
        public string Deudora { get; set; }
        public string ClaveCliente { get; set; }
        public string rfc { get; set; }
    }

    public class EmpleadoDatosNominaViewModel
    {
        public int IdEmpleado { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Sexo { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public string UMF { get; set; }
        public string FechaAlta { get; set; }
        public string FechaReal { get; set; }
        public string FechaIMSS { get; set; }
        public string FechaVigencia { get; set; }
        public string TipoContrato { get; set; }
        public int? DiasContrato { get; set; }
        public string DiaDescanso { get; set; }

        public string TipoNomina { get; set; }
        public string TipoPago { get; set; }
        public string TipoSalario { get; set; }
        public string SD { get; set; }
        public string SDI { get; set; }
        public string SalarioReal { get; set; }
        public string EmpresaFiscal { get; set; }
        public string EmpresaComplemento { get; set; }
        public string EmpresaSindicato { get; set; }
        public string EmpresaAsimilado { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
    }
    public class NominaDetalle
    {
        public string Detalle { get; set; }
        public decimal Cantidad { get; set; }
        public bool complemento { get; set; }
        public int tipoConcpeto { get; set; }
    }

    public class DatosDispersion
    {
        public string nombres { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public int? nosigaF { get; set; }
        public int? nosigaC { get; set; }
        public decimal importePagar { get; set; }
        public string cuenta { get; set; } 
        public string nombreBanco { get; set; }
        public int idBanco { get; set; }
        public decimal totalComplemento { get; set; }
        public string MetodoPago { get; set; }
        public int idNomina { get; set; }
    }
    public class Obligaciones
    {
        public int idNomina { get; set; }
        public decimal FondoRetiro { get; set; }
        public decimal ImpuestoNomina { get; set; }
        public decimal Guarderia { get; set; }
        public decimal IMSSEmpresa { get; set; }
        public decimal RiesgoTrabajo { get; set; }
    }

    public class ListadoBancoEmpleados
    {
        public int IdBanco { get; set; }
        public string NombreBanco { get; set; }
        public List<RegistroEmpleados> empleados { get; set; }
        public decimal Total { get; set; }
        
    }

    public class RegistroEmpleados
    {
        public int? NoSigaF { get; set; }
        public int? NoSigaC { get; set; }
        public string Empleado { get; set; }
        public decimal Importe { get; set; }
        public string cuenta { get; set; }
        public string tipocuenta { get; set; }
        public int MetodoPago { get; set; }
        public int idnomina { get; set; }
    }
}
