using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.BLL;
using RH.Entidades;
using Common.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web;
using Common.Utils;
namespace Nomina.BLL
{
    public class AutorizarNomina
    {
        RHEntities ctx = null;

        public AutorizarNomina()
        {
            ctx = new RHEntities();
        }

        public List<AutorizacionDetalle> autorizaciondetalle(int idperiodo)
        {
            AutorizacionDetalle autorizar = new AutorizacionDetalle();
            var datos = (from nom in ctx.NOM_Nomina
                         join nom_det in ctx.NOM_Nomina_Detalle
                         on nom.IdNomina equals nom_det.IdNomina
                         join concep in ctx.C_NOM_Conceptos
                         on nom_det.IdConcepto equals concep.IdConcepto
                         where nom.IdPeriodo == idperiodo
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
        
        public Obligaciones obligacionesGenerales (int idPeriodo)
        {
            decimal fondoR = 0;
            decimal impues = 0;
            decimal guar = 0;
            decimal imss = 0;
            decimal ries = 0;
            decimal infoemp = 0;
            var datos = (from nom in ctx.NOM_Nomina
                         join cuot in ctx.NOM_Cuotas_IMSS
                         on nom.IdNomina equals cuot.IdNomina
                         where nom.IdPeriodo == idPeriodo
                         select new Obligaciones
                         {
                             idNomina = nom.IdNomina,
                             FondoRetiro = cuot.SeguroRetiro_Patron,
                             ImpuestoNomina = nom.TotalImpuestoSobreNomina,
                             Guarderia = cuot.GuarderiasPrestaciones_Patron,
                             IMSSEmpresa = cuot.CesantiaVejez_Patron + cuot.Pensionados_Patron + cuot.Cuota_Fija_Patron + cuot.Excedente_Patron + cuot.InvalidezVida_Patron,
                             RiesgoTrabajo = cuot.RiesgoTrabajo_Patron,
                             InfonavitEmpresa = cuot.Infonavit_Patron
                             
                         }).ToList();

            Obligaciones obliga = new Obligaciones();
            foreach(var d in datos)
            {
                fondoR = fondoR + d.FondoRetiro;
                impues = impues + d.ImpuestoNomina;
                guar =guar + d.Guarderia;
                imss = imss+ d.IMSSEmpresa;
                 ries= ries +d.RiesgoTrabajo;
                infoemp = infoemp + d.InfonavitEmpresa;
            }

            obliga.FondoRetiro = fondoR;
            obliga.ImpuestoNomina = impues ;
            obliga.Guarderia = guar;
            obliga.IMSSEmpresa = imss;
            obliga.RiesgoTrabajo = ries;
            obliga.InfonavitEmpresa = infoemp;

            return obliga;
        }
        public bool Autorizacion(int idperiodo)
        {
            var result = false;
            HistorialPagos historial = new HistorialPagos();
            try
            {
                var nomina = ctx.NOM_Nomina.Where(x => x.IdPeriodo == idperiodo).ToList();
                 foreach (var n in nomina)
                {
                    var detNom = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == n.IdNomina).ToList();
                    foreach(var d in detNom)
                    {
                        using (var Transaction = ctx.Database.BeginTransaction())
                        {
                            try
                            {
                                if (d.IdConcepto == 51)
                                {
                                    historial.IdPrestamo = d.IdPrestamo;
                                    historial.IdNomina = n.IdNomina;
                                    historial.CantidadDescuento = d.Total;
                                    historial.FechaRegistro = DateTime.Now;
                                    historial.TipoPrestamo = 3;
                                    historial.Salario = n.SDI;
                                    historial.DiasDescontados = n.Dias_Laborados;
                                    ctx.HistorialPagos.Add(historial);
                                    //   var guardar = ctx.SaveChanges();
                                }

                                else
                                               if (d.IdConcepto == 52)
                                {
                                    historial.IdPrestamo = d.IdPrestamo;
                                    historial.IdNomina = n.IdNomina;
                                    historial.CantidadDescuento = d.Total;
                                    historial.FechaRegistro = DateTime.Now;
                                    historial.TipoPrestamo = 2;
                                    historial.Salario = n.SDI;
                                    historial.DiasDescontados = n.Dias_Laborados;
                                    ctx.HistorialPagos.Add(historial);
                                    

                                    var descuento = ctx.Empleado_Fonacot.FirstOrDefault(x => x.Id == d.IdPrestamo);

                                    decimal total = 0;
                                    total = descuento.Saldo - d.Total;

                                    descuento.Saldo = total;
                                    var guardarDescuento = ctx.SaveChanges();

                                }

                                var guardar = ctx.SaveChanges();


                                Transaction.Commit();
                            }
                            catch (Exception e)
                            {
                                
                                Transaction.Rollback();
                            }
                        }
                    }
                }

                var item = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idperiodo);
                


                item.Autorizado = true;
                item.FechaAutorizacion = DateTime.Now;
                item.UsuarioAutorizo = SessionHelpers.GetIdUsuario();
                var r2 = ctx.SaveChanges();

                result = true;

                return result;
            }
            catch (Exception e)
            {
                return result;
            }

        }

        public TotalesRubrosIMSS DispersionCuotasIMSS(int IdPeriodo)
        {
            int[] listaNomina;

            var empresaFiscal = ctx.NOM_Nomina.Where(x => x.IdPeriodo == IdPeriodo ).Select(x => x.IdNomina).ToArray();
         
                listaNomina = empresaFiscal;
                var lista = (from CIMSS in ctx.NOM_Cuotas_IMSS
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
        
        public Dispercion totalImpuestoSobreNomina(int idperiodo)
        {
            int[] listaNomina;

            var empresaFiscal = ctx.NOM_Nomina.Where(x => x.IdPeriodo == idperiodo).Select(x => x.IdNomina).ToArray();
            listaNomina = empresaFiscal;
            var lista = (from nom_det in ctx.NOM_Nomina_Detalle
                         where listaNomina.Contains(nom_det.IdNomina)
                         select nom_det).ToList();
            Dispercion disp = new Dispercion();
            disp.totalSobreNomina = lista.Sum(x => x.ImpuestoSobreNomina);

            return disp;
        }

        public Dispercion totalFonacot(int idperiodo)
        {
            int[] listaNomina;

            var empresaFiscal = ctx.NOM_Nomina.Where(x => x.IdPeriodo == idperiodo).Select(x => x.IdNomina).ToArray();
            listaNomina = empresaFiscal;
            var lista = (from nom_det in ctx.NOM_Nomina_Detalle
                         where listaNomina.Contains(nom_det.IdNomina) && nom_det.IdConcepto == 26
                         select nom_det).ToList();
            Dispercion disp = new Dispercion();
            disp.totalFonacot = lista.Sum(x => x.Total);

            return disp;
        }
        public Dispercion totalInfonavit (int idperiodo)
        {
            int[] listaNomina;

            var empresaFiscal = ctx.NOM_Nomina.Where(x => x.IdPeriodo == idperiodo).Select(x => x.IdNomina).ToArray();
            listaNomina = empresaFiscal;
            var lista = (from nom_det in ctx.NOM_Nomina_Detalle
                         where listaNomina.Contains(nom_det.IdNomina) && nom_det.IdConcepto == 25
                         select nom_det).ToList();
            Dispercion disp = new Dispercion();
            disp.totalInfonavit = lista.Sum(x => x.Total);

            return disp;
        }


        public NOM_Facturacion VisualizarFacturacion(int idperiodo)
        {
            var dato = ctx.NOM_Facturacion.Where(x => x.IdPeriodo == idperiodo).FirstOrDefault();
            var iva = ctx.ParametrosConfig.Where(x => x.IdConfig == 5).Select(x => x.ValorInt).FirstOrDefault();
            if (dato == null)
            {
                NOM_Facturacion dato2 = new NOM_Facturacion();
                dato2.IVA = iva;
                dato2.F_Cuota_IMSS_Infonavit = 0;
                dato2.F_Impuesto_Nomina = 0;
                dato2.F_Amortizacion_Infonavit = 0;
                dato2.F_Fonacot = 0;
                dato2.F_Pension_Alimenticia = 0;
                dato2.F_Relativo = 0;
                dato2.F_Total_Nomina = 0;
                dato2.F_Total_Fiscal = 0;
         
                return dato2;
            }
            else
            {
                return dato;
            }
        }
        public NOM_FacturacionComplemento VisualizarFacturacionC(int idperiodo)
        {
            var dato = ctx.NOM_FacturacionComplemento.Where(x => x.IdPeriodo == idperiodo).FirstOrDefault();
            if (dato == null)
            {
                NOM_FacturacionComplemento dato2 = new NOM_FacturacionComplemento();
                dato2.IdFacturaC = 0;
                dato2.C_Percepciones = 0;
                dato2.C_Porcentaje_Servicio = 0;
                dato2.C_Total_Servicio = 0;
                dato2.C_Cuotas_IMSS_Infonavit = 0;
                dato2.C_Impuesto_Nomina = 0;
                dato2.C_Relativos = 0;
                dato2.C_Descuentos = 0;
                dato2.C_Otros = 0;
                dato2.C_Subtotal = 0;
                dato2.C_Total_IVA = 0;
                dato2.C_Total_Complemento = 0;
                return dato2;
            }
            else
            {
                return dato;
            }
            
        }
        public NOM_FacturacionSindicato VisualizarFacturacionS(int idperiodo)
        {
            NOM_FacturacionSindicato dato = new NOM_FacturacionSindicato();
             dato = ctx.NOM_FacturacionSindicato.Where(x => x.IdPeriodo == idperiodo).FirstOrDefault();
            if (dato == null)
            {
                NOM_FacturacionSindicato dato2 = new NOM_FacturacionSindicato();
                dato2.S_Costo_IMSS = 0;
                dato2.S_Cuota_Legado = 0;
                dato2.S_Dif_Montvde = 0;
                dato2.S_IVA_Costo_IMSS = 0;
                dato2.S_IVA_Percepcion_Fiscal = 0;
                dato2.S_IVA_Porcentaje_Nomina = 0;
                dato2.S_Percepcion_Fiscal = 0;
                dato2.S_Percepcion_Sindicato = 0;
                dato2.S_Porcentaje_Comision = 0;
                dato2.S_Total_Comision = 0;
                dato2.S_Total_Porcentaje_Nomina = 0;
                dato2.S_Total_Sindicato = 0;
                return dato2;
            }else
            {
                return dato;
            }
                
        }
        public bool GuardarFacturacion(NOM_FacturacionComplemento factura, int periodo)
        {
            var dato = ctx.NOM_FacturacionComplemento.Where(x => x.IdPeriodo == periodo && x.IdEmpresaC == factura.IdEmpresaC).FirstOrDefault();

            if(dato != null)
            {
                string sqlQuery1 = "DELETE [NOM_FacturacionComplemento] WHERE IdPeriodo in (" + periodo + ") and IdEmpresaC="+dato.IdEmpresaC;
                ctx.Database.ExecuteSqlCommand(sqlQuery1);
                factura.IdPeriodo = periodo;
                //factura.IVA = 16;
                var result = false;
                ctx.NOM_FacturacionComplemento.Add(factura);
                var r = ctx.SaveChanges();

                if (r > 0)
                    result = true;

                return result;
            }else
            {
               

                factura.IdPeriodo = periodo;
                //factura.IVA = 16;
                var result = false;
                //ctx.NOM_Facturacion.Add(factura);
                var r = ctx.SaveChanges();

                if (r > 0)
                    result = true;

                return result;
            }

         
        }

        public bool GuardarFacturacionSindicato(NOM_FacturacionSindicato factura, int periodo,SucursalDatos sucursal)
        {
            
            try
            {
                var idSindicato = ctx.Sucursal_Empresa.Where(x => x.IdSucursal == sucursal.IdSucursal && x.IdEsquema == 3).Select(x => x.IdEmpresa).FirstOrDefault();

                var dato = ctx.NOM_FacturacionSindicato.Where(x => x.IdPeriodo == periodo && x.IdEmpresaS == factura.IdEmpresaS).FirstOrDefault();

                if (dato != null)
                {
                    string sqlQuery1 = "DELETE [NOM_FacturacionComplemento] WHERE IdPeriodo in (" + periodo + ") and IdEmpresaC=" + dato.IdEmpresaS;
                    ctx.Database.ExecuteSqlCommand(sqlQuery1);
                    factura.IdPeriodo = periodo;
                    factura.IdEmpresaS = idSindicato;
                    //factura.IVA = 16;
                    var result = false;
                    ctx.NOM_FacturacionSindicato.Add(factura);
                    var r = ctx.SaveChanges();

                    if (r > 0)
                        result = true;

                    return result;
                }
                else
                {

                    factura.IdEmpresaS = idSindicato;
                    factura.IdPeriodo = periodo;
                    //factura.IVA = 16;
                    var result = false;
                    ctx.NOM_FacturacionSindicato.Add(factura);
                    var r = ctx.SaveChanges();

                    if (r > 0)
                        result = true;

                    return result;
                }
            }
            catch(Exception e)
            {
                return false;
            }
         


        }

    }


    public class AutorizacionDetalle
    {
        public string NombreConcepto { get; set; }
        public int? TipoConcepto { get; set; }
        public decimal TotalConcepto { get; set; }
        public int IdConcepto { get; set; }
        public bool? Obligacion { get; set; }
        public bool Complemento { get; set; }
        

    }
    public class Obligaciones
    {
        public int idNomina { get; set; }
        public decimal FondoRetiro { get; set; }
        public decimal ImpuestoNomina { get; set; }
        public decimal Guarderia { get; set; }
        public decimal IMSSEmpresa { get; set; }
        public decimal RiesgoTrabajo { get; set; }
        public decimal InfonavitEmpresa { get; set; }
        
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

    public class Dispercion
    {
        public decimal totalSobreNomina { get; set; }
        public decimal totalFonacot { get; set; }
        public decimal totalInfonavit { get; set; }
    }

}
