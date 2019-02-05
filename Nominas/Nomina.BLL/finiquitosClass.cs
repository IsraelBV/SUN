using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Nomina.Procesador;

namespace Nomina.BLL
{
    public class finiquitosClass
    {
        //RHEntities ctx = null;
        public finiquitosClass()
        {
          //  ctx = new RHEntities();
        }

        public EmpleadoFiniquitos EmpleadoPeriodoById(int idperiodo)
        {
            using (var context = new RHEntities())
            {
                var datos = (from e in context.Empleado
                    join p in context.NOM_Empleado_PeriodoPago
                    on e.IdEmpleado equals p.IdEmpleado
                    where p.IdPeriodoPago == idperiodo
                    select new EmpleadoFiniquitos
                    {
                        idEmpleado = e.IdEmpleado,
                        Nombres = e.Nombres,
                        Paterno = e.APaterno,
                        Materno = e.AMaterno,

                    }
                ).FirstOrDefault();
                return datos;
            }
        }
        public NOM_Finiquito FiniquitoFiscal(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var datos = context.NOM_Finiquito.FirstOrDefault(x => x.IdPeriodo == idPeriodo);
                return datos;
            }
        }

        //public static async Task<int> ProcesarFiniquitoAsync(NOM_PeriodosPago ppago, int idEmpleado, bool calcularLiquitacion)
        //{
        //    try
        //    {

        //        NominaCore nc = new NominaCore();
        //        var finiquito = await nc.ProcesarFiniquitoIndemnizacionAsync(ppago.IdPeriodoPago, ppago.IdEjercicio, idEmpleado, ppago.IdSucursal, calcularLiquitacion);
        //        return finiquito;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}
        public NOM_Finiquito_Complemento FiniquitoComplemento(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var datos = context.NOM_Finiquito_Complemento.Where(x => x.IdFiniquito == idFiniquito).FirstOrDefault();
                return datos;
            }
        }
        public List<finiquitoDetalle> FiniquitoDetalle(int idFiniquito)
        {
            using (var context = new RHEntities())
            {
                var concepto = (from fd in context.NOM_Finiquito_Detalle
                    join c in context.C_NOM_Conceptos
                    on fd.IdConcepto equals c.IdConcepto
                    where fd.IdFiniquito == idFiniquito
                    select new finiquitoDetalle
                    {
                        IdConcepto = c.IdConcepto,
                        Descripcion = c.Descripcion,
                        Total = fd.Total,
                        Exento = fd.ExentoISR,
                        Gravado = fd.GravadoISR,
                        ImpuestoSn = fd.ImpuestoSobreNomina
                    }).ToList();
                return concepto;
            }
        }
        public List<NOM_Finiquito_Descuento_Adicional> DescuentoFiscal_Complemento(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista =
                    context.NOM_Finiquito_Descuento_Adicional.Where(x => x.IdPeriodo == idPeriodo && x.TipoConcepto == 2)
                        .ToList();
                return lista;
            }
        }

        public List<NOM_Finiquito_Descuento_Adicional> DescuentoFiscal_PercepcionesExtas(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista =
                    context.NOM_Finiquito_Descuento_Adicional.Where(x => x.IdPeriodo == idPeriodo && x.TipoConcepto == 1)
                        .ToList();
                return lista;
            }
        }
        public List<NOM_Finiquito_Descuento_Adicional> DescuentoComplemento (int IdPeriodo)
        {
            using (var context = new RHEntities())
            {
                var lista =
                    context.NOM_Finiquito_Descuento_Adicional.Where(x => x.IdPeriodo == IdPeriodo && x.IsComplemento == true)
                        .ToList();
                return lista;
            }
        }
        public List<C_NOM_Conceptos> ListaConceptosDescuentos()
        {
            using (var context = new RHEntities())
            {
                var lista = context.C_NOM_Conceptos.Where(x => x.TipoConcepto == 2).ToList();
                return lista;
            }
            
        }
        public List<C_NOM_Conceptos> ListaConceptosPercepcionesExtras()
        {
            using (var context = new RHEntities())
            {
                var lista = context.C_NOM_Conceptos.Where(x => x.TipoConcepto == 1).ToList();
                return lista;
            }
        }
        public bool GuardarDescuentos (List<NOM_Finiquito_Descuento_Adicional> arrayDes, List<NOM_Finiquito_Descuento_Adicional> arrayDesC,int IdPeriodo)
        {
            using (var context = new RHEntities())
            {
                var itemPeriodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == IdPeriodo);

                if(itemPeriodo == null) return false;

                if(itemPeriodo.Autorizado == true) return false;

                arrayDes.AddRange(arrayDesC);
                const string sqlQuery =
                    "DELETE NOM_Finiquito_Descuento_Adicional WHERE IdPeriodo = @p0 and TipoConcepto = 2 ";
                context.Database.ExecuteSqlCommand(sqlQuery, IdPeriodo);
                foreach (var des in arrayDes)
                {
                    if (des.Descripcion != null && des.TotalDescuento > 0)
                    {
                        var item = new NOM_Finiquito_Descuento_Adicional
                        {
                            IdPeriodo = IdPeriodo,
                            IdConcepto = des.IdConcepto,
                            Descripcion = des.Descripcion,
                            TotalDescuento = des.TotalDescuento,
                            IsComplemento = des.IsComplemento,
                            TipoConcepto = 2

                        };
                        context.NOM_Finiquito_Descuento_Adicional.Add(item);
                    }



                }

                var t = context.SaveChanges();
                return true;
            }
        }


        public bool GuardarOtrasPercepciones(List<NOM_Finiquito_Descuento_Adicional> arrayDes, List<NOM_Finiquito_Descuento_Adicional> arrayDesC, int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var itemPeriodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                if (itemPeriodo == null) return false;

                if (itemPeriodo.Autorizado == true) return false;


                arrayDes.AddRange(arrayDesC);
                const string sqlQuery =
                    "DELETE NOM_Finiquito_Descuento_Adicional WHERE IdPeriodo = @p0 and TipoConcepto = 1";
                context.Database.ExecuteSqlCommand(sqlQuery, idPeriodo);
                foreach (var des in arrayDes)
                {
                    if (des.Descripcion != null && des.TotalDescuento > 0)
                    {
                        var item = new NOM_Finiquito_Descuento_Adicional
                        {
                            IdPeriodo = idPeriodo,
                            IdConcepto = des.IdConcepto,
                            Descripcion = des.Descripcion,
                            TotalDescuento = des.TotalDescuento,
                            IsComplemento = des.IsComplemento,
                            TipoConcepto = 1

                        };
                        context.NOM_Finiquito_Descuento_Adicional.Add(item);
                    }



                }

                var t = context.SaveChanges();
                return true;
            }
        }
        public NOM_FacturacionF_Finiquito facturaF(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var dato = context.NOM_FacturacionF_Finiquito.Where(x => x.IdPeriodo == idPeriodo).FirstOrDefault();

                return dato;
            }
        }
        public NOM_FacturacionC_Finiquito facturaC(int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var dato = context.NOM_FacturacionC_Finiquito.Where(x => x.IdPeriodo == idPeriodo).FirstOrDefault();

                return dato;
            }
        }

        public bool GuardarFacturacion(NOM_FacturacionC_Finiquito factura, int periodo)
        {
            using (var context = new RHEntities())
            {
                var dato =
                    context.NOM_FacturacionC_Finiquito.Where(
                        x => x.IdPeriodo == periodo && x.IdEmpresa_C == factura.IdEmpresa_C).FirstOrDefault();

                if (dato != null)
                {
                    string sqlQuery1 = "DELETE [NOM_FacturacionC_Finiquito] WHERE IdPeriodo in (" + periodo +
                                       ") and IdEmpresa_C=" + dato.IdEmpresa_C;
                    context.Database.ExecuteSqlCommand(sqlQuery1);
                    factura.IdPeriodo = periodo;
                    //factura.IVA = 16;
                    var result = false;
                    context.NOM_FacturacionC_Finiquito.Add(factura);
                    var r = context.SaveChanges();

                    if (r > 0)
                        result = true;

                    return result;
                }
                else
                {


                    factura.IdPeriodo = periodo;

                    context.NOM_FacturacionC_Finiquito.Add(factura);
                    //factura.IVA = 16;
                    var result = false;
                    //ctx.NOM_Facturacion.Add(factura);
                    var r = context.SaveChanges();

                    if (r > 0)
                        result = true;

                    return result;
                }
            }

        }
        public byte[] GetReciboFiscal(int idFiniquito, int idPeriodo, int idUsuario, string pathFolder, bool liquidacion = false)
        {
            PDFCore pc = new PDFCore();

            return pc.ReciboFiniquitoFiscal(idFiniquito, idPeriodo, idUsuario, pathFolder, liquidacion);

        }

        public byte[] GetReciboReal(int idFiniquito, int idPeriodo, int idUsuario, string pathFolder, bool liquidacion = false)
        {
            PDFCore pc = new PDFCore();

            return pc.ReciboFiniquitoReal(idFiniquito, idPeriodo, idUsuario, pathFolder, liquidacion);

        }

        public bool Autorizacion (int idPeriodo)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);
                if (item == null) return false;
                var result = false;

                item.Autorizado = true;
                item.Procesado = true;
                item.Procesando = false;
                item.FechaAutorizacion = DateTime.Now;

                var r2 = context.SaveChanges();

                if (r2 > 0)
                {
                    result = true;
                }

                return result;
            }
        }

    }


    public class EmpleadoFiniquitos
    {
        public int idEmpleado { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
    public class finiquitoDetalle
    {
        public int IdConcepto { get; set; }
        public string Descripcion { get; set; }
        public decimal Total { get; set; }
        public decimal Exento { get; set; }
        public decimal Gravado { get; set; }

        public decimal ImpuestoSn { get; set; }
    }

    
}
