using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Utils;
using RH.Entidades;
using RH.Entidades.GlobalModel;

namespace Nomina.BLL
{
    public class PeriodosPago
    {
        RHEntities ctx;

        public PeriodosPago()
        {
            ctx = new RHEntities();
        }

        public List<C_PeriodicidadPago_SAT> GetPeriodicidadPagos()
        {
            var lista = ctx.C_PeriodicidadPago_SAT.ToList();

            foreach (var item in lista)
            {
                item.Descripcion = item.IdPeriodicidadPago.ToString().PadLeft(2, '0') + " - " + item.Descripcion;
            }

            return ctx.C_PeriodicidadPago_SAT.ToList();
        }

        public bool PeriodoEnProceso(int IdPeriodoPago)
        {
            var periodo = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == IdPeriodoPago);
            return periodo.Procesando == true ? true : false;
        }

        public NOM_PeriodosPago SetPeriodoEnProceso(int IdPeriodoPago)
        {
            var periodo = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == IdPeriodoPago);
            periodo.Procesando = true;
            ctx.SaveChanges();
            return periodo;
        }

        public NOM_PeriodosPago CerrarPeriodo(int IdPeriodoPago)
        {
            using (var context = new RHEntities())
            {
                var periodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == IdPeriodoPago);

                if (periodo == null) return null;

                periodo.Procesando = false;
                periodo.Procesado = true;
                context.SaveChanges();
                return periodo;
            }
        }

        public bool UpdatePeriodoPago(NOM_PeriodosPago updatedModel)
        {
            //Validar periodo
            var itemPeriodo = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == updatedModel.IdPeriodoPago);

            //Si el periodo ya esta autorizado, ya no se podrá hacer cambios
            if (itemPeriodo.Autorizado == true) return false;

            var old = GetPeriodoPagoById(updatedModel.IdPeriodoPago);
            if (old == null) return false;
            updatedModel.Fecha_Pago = updatedModel.Fecha_Fin;
            ctx.Entry(old).CurrentValues.SetValues(updatedModel);
            var result = ctx.SaveChanges();
            return result > 0 ? true : false;
        }

        public List<NOM_PeriodosPago> GetPeriodosPagoBySucursal(int id)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_PeriodosPago.Where(x => x.IdSucursal == id).ToList();
            }
        }

        public Tuple<List<NOM_PeriodosPago>,int> GetPeriodosPagoBySucursal(int id, int idEjercicio,int idusuario)
        {
            List<NOM_PeriodosPago> periodos = null;
            int departamento = 0;
            using (var context = new RHEntities())
            {
                var usuarios = context.SYA_Usuarios.FirstOrDefault(x => x.IdUsuario == idusuario);
               
                if (idEjercicio <= 0)
                {
                    var aaaa = Utils.GetAñoActual();

                    var ae = context.NOM_Ejercicio_Fiscal.FirstOrDefault(x => x.Anio == aaaa);

                    if (ae != null)
                    {
                        idEjercicio = ae.IdEjercicio;
                    }
                }
                departamento = usuarios.IdDepartamento;

                if (departamento != 5)
                {
                    do {
                        periodos = context.NOM_PeriodosPago.Where(x => x.IdSucursal == id && x.IdEjercicio == idEjercicio).ToList();
                        idEjercicio--;
                    } while (periodos.Count == 0);
                }
                else {
                    do {
                        periodos = context.NOM_PeriodosPago.Where(x => x.IdSucursal == id && x.IdEjercicio == idEjercicio && x.Autorizado == true).ToList();
                        idEjercicio--;
                    } while (periodos.Count == 0);
                }

            }
            return Tuple.Create(periodos, departamento);
        }

        public List<NOM_PeriodosPago> GetPeriodosPagoActivosBySucursal(int id)
        {

            return GetPeriodosPagoBySucursal(id).Where(x => x.Autorizado != true).ToList();
        }

        public List<int> GetIdEmpleados(int IdPeriodo)
        {
            return ctx.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == IdPeriodo).Select(x => x.IdEmpleado).ToList();
        }

        public List<EmpleadoNomina> GetIdEmpleadosProcesados(int IdPeriodo)
        {
            var datos = (from nom in ctx.NOM_Nomina
                         join peri in ctx.NOM_PeriodosPago
                         on nom.IdPeriodo equals peri.IdPeriodoPago
                         where nom.IdPeriodo == IdPeriodo
                         select new EmpleadoNomina
                         {
                             idempleado = nom.IdEmpleado
                         }).ToList();

            return datos;
        }

        public List<Empleado> GetEmpleados(int IdPeriodo)
        {
            using (var context = new RHEntities())
            {
                return (from epp in context.NOM_Empleado_PeriodoPago
                        join e in context.Empleado on epp.IdEmpleado equals e.IdEmpleado
                        where epp.IdPeriodoPago == IdPeriodo
                        select e
                ).ToList();
            }
        }

        public NOM_PeriodosPago GetPeriodoPagoById(int id)
        {
            using (var context = new RHEntities())
            {
                return context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == id);
            }

        }

        public bool CrearPeriodo(NOM_PeriodosPago model)
        {
            using (var context = new RHEntities())
            {
                context.NOM_PeriodosPago.Add(model);
                var result = context.SaveChanges();
                return result > 0 ? true : false;
            }
        }

        public bool UpdatePeriodoPagoEmpleados(int IdPeriodo, int[] empleados = null)
        {
            using (var context = new RHEntities())
            {
                //Validar periodo
                var itemPeriodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == IdPeriodo);

                //Si el periodo ya esta autorizado, ya no se podrá hacer cambios
                if (itemPeriodo.Autorizado == true) return false;

                //Elimina los empleados que tenía configurado anteriormente
                var old = context.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == IdPeriodo).ToList();
                if (old.Count > 0)
                    context.NOM_Empleado_PeriodoPago.RemoveRange(old);

                if (empleados != null)
                {
                    //Agrega empleado por empleado
                    foreach (var item in empleados)
                    {
                        var registro = new NOM_Empleado_PeriodoPago();
                        registro.IdPeriodoPago = IdPeriodo;
                        registro.IdEmpleado = item;
                        registro.DiasDescuentoInfonavit = -1;
                        context.NOM_Empleado_PeriodoPago.Add(registro);
                    }

                }
                return context.SaveChanges() > 0 ? true : false;

            }
        }

        public List<EmpleadoNomina> GetEmpleadoByTipoNomina(int IdPeriodicidadPago, int idSucursal, int rfc)
        {

            using (var context = new RHEntities())
            {
                var listaPeriodiciadad = context.C_PeriodicidadPago_SAT.ToList();

                var listaEmpleadoContrato = IdPeriodicidadPago <= 15 ?
                        context.Empleado_Contrato.Where(x => (x.IdEmpresaFiscal != null || x.IdEmpresaComplemento != null) && x.IdSucursal == idSucursal && x.Status == true).ToList() :
                        (IdPeriodicidadPago == 16 ?
                            context.Empleado_Contrato.Where(x => x.IdEmpresaAsimilado != null && x.IdSucursal == idSucursal && x.Status == true).ToList() :
                            context.Empleado_Contrato.Where(x => x.IdEmpresaSindicato != null && x.IdSucursal == idSucursal && x.Status == true).ToList()
                         );

                var arrayIdsEmpleadoContrato = listaEmpleadoContrato.Select(x => x.IdEmpleado).ToArray();

                var datos = (from emp in context.Empleado
                             where arrayIdsEmpleadoContrato.Contains(emp.IdEmpleado)
                             && emp.IdSucursal == idSucursal
                             && emp.RFCValidadoSAT == rfc
                             select new EmpleadoNomina
                             {
                                 idempleado = emp.IdEmpleado,
                                 Nombres = emp.Nombres,
                                 Paterno = emp.APaterno,
                                 Materno = emp.AMaterno,
                                 RFC = emp.RFC,
                                 Status = emp.Status
                             }).ToList();

                //// version anterior, modificado el 30-08-2018
                //var datos = (from emp in context.Empleado
                //             join con in ctx.Empleado_Contrato on emp.IdEmpleado equals con.IdEmpleado
                //             where emp.IdSucursal == idSucursal /*&& con.IdPeriodicidadPago == IdPeriodicidadPago*/
                //             && emp.RFCValidadoSAT == rfc
                //             && emp.Status == true
                //             //&& con.Status == true
                //             select new EmpleadoNomina
                //             {
                //                 idempleado = emp.IdEmpleado,
                //                 Nombres = emp.Nombres,
                //                 Paterno = emp.APaterno,
                //                 Materno = emp.AMaterno,
                //                 RFC = emp.RFC,
                //                 Status = emp.Status
                //             }).ToList();

                var arrayIdEmpleados = datos.Select(x => x.idempleado).ToArray();

                var contratos = (from c in context.Empleado_Contrato
                                 where arrayIdEmpleados.Contains(c.IdEmpleado)
                                 select c).ToList();


                foreach (var itemEmpleado in datos)
                {
                    var itemContrato =
                        contratos.Where(x => x.IdEmpleado == itemEmpleado.idempleado)
                            .OrderByDescending(x => x.IdContrato)
                            .FirstOrDefault();

                    if (itemContrato == null) continue;

                    var itemPeriodicidad =
                        listaPeriodiciadad.FirstOrDefault(x => x.IdPeriodicidadPago == itemContrato.IdPeriodicidadPago);

                    itemEmpleado.IdPeriodicidadPago = itemContrato.IdPeriodicidadPago;
                    itemEmpleado.PeriodicidadPago = itemPeriodicidad.Descripcion;


                }



                return datos;
            }


        }
        public List<EmpleadoNomina> empleadoFiniquito(int idSucursal, int rfc)
        {
            var datos = (from emp in ctx.Empleado
                         join con in ctx.Empleado_Contrato on emp.IdEmpleado equals con.IdEmpleado
                         where emp.IdSucursal == idSucursal 
                               && emp.RFCValidadoSAT == rfc
                               && con.IdEmpresaFiscal != null
                               && con.IdEmpresaComplemento != null
                         select new EmpleadoNomina
                         {
                             idempleado = emp.IdEmpleado,
                             Nombres = emp.Nombres,
                             Paterno = emp.APaterno,
                             Materno = emp.AMaterno,
                             RFC = emp.RFC,
                             Status = emp.Status

                         }).ToList();
            return datos;
        }
        public List<EmpleadoNomina> empleadoFiniquitoC(int idSucursal, int rfc)
        {
            var datos = (from emp in ctx.Empleado
                         join con in ctx.Empleado_Contrato
                         on emp.IdEmpleado equals con.IdEmpleado
                         where emp.IdSucursal == idSucursal 
                            && emp.RFCValidadoSAT == rfc
                            //&& con.IdEmpresaFiscal != null
                            && con.IdEmpresaComplemento != null
                         select new EmpleadoNomina
                         {
                             idempleado = emp.IdEmpleado,
                             Nombres = emp.Nombres,
                             Paterno = emp.APaterno,
                             Materno = emp.AMaterno,
                             RFC = emp.RFC,
                             Status = emp.Status

                         }).ToList();
            return datos;
        }
        public List<EmpleadoNomina> EmpleadosBaja(int IdPeriodicidadPago, int idSucursal)
        {
            using (var context = new RHEntities())
            {
                var listaPeriodiciadad = context.C_PeriodicidadPago_SAT.ToList();

                var datos = (from emp in ctx.Empleado
                                 //  join con in ctx.Empleado_Contrato
                                 //  on emp.IdEmpleado equals con.IdEmpleado
                             where emp.IdSucursal == idSucursal /*&& con.IdPeriodicidadPago == IdPeriodicidadPago*/ && emp.Status == false
                             select new EmpleadoNomina
                             {
                                 idempleado = emp.IdEmpleado,
                                 Nombres = emp.Nombres,
                                 Paterno = emp.APaterno,
                                 Materno = emp.AMaterno,
                                 RFC = emp.RFC
                             }).ToList();


                var arrayIdEmpleados = datos.Select(x => x.idempleado).ToArray();

                var contratos = (from c in context.Empleado_Contrato
                                 where arrayIdEmpleados.Contains(c.IdEmpleado)
                                 select c).ToList();

                foreach (var itemEmpleado in datos)
                {
                    var itemContrato =
                        contratos.Where(x => x.IdEmpleado == itemEmpleado.idempleado)
                            .OrderByDescending(x => x.IdContrato)
                            .FirstOrDefault();

                    if (itemContrato == null) continue;

                    var itemPeriodicidad =
                        listaPeriodiciadad.FirstOrDefault(x => x.IdPeriodicidadPago == itemContrato.IdPeriodicidadPago);

                    itemEmpleado.IdPeriodicidadPago = itemContrato.IdPeriodicidadPago;
                    itemEmpleado.PeriodicidadPago = itemPeriodicidad.Descripcion;


                }


                return datos;
            }



        }
        public bool guardarPeriodo(int[] arrayE, string[] periodoDatos, int idsucursal, int idCliente, int idUsuario)
        {
            try
            {
                //validacion
                if (periodoDatos == null) return false;

                if (Convert.ToInt32(periodoDatos[1]) == 0) return false;

                var fechafin = Convert.ToDateTime(periodoDatos[4]);
                var anio = fechafin.Year.ToString();
                int mes = fechafin.Month;
                var fechaActual = DateTime.Now;
                double calc = (double)mes / (double)2;
                var bim = (int)Math.Ceiling(calc);
                var esSindicato = false;
                int tipoNomina = 0;

                var ejercicio = ctx.NOM_Ejercicio_Fiscal.Where(x => x.Anio == anio).Select(x => x.IdEjercicio).FirstOrDefault();

                bool soloComplemento = false;
                if (periodoDatos.Length == 9)
                {
                    soloComplemento = Convert.ToBoolean(periodoDatos[8]);
                }

                //Get tipo de Nomina del periodo
                tipoNomina = Convert.ToInt32(periodoDatos[0]);

                if (tipoNomina == 17) //Sindicato
                {
                    esSindicato = true;
                }

                var item = new NOM_PeriodosPago
                {
                    IdTipoNomina = tipoNomina, // Convert.ToInt32(periodoDatos[0]),
                    DiasPeriodo = Convert.ToInt32(periodoDatos[1]),
                    Descripcion = periodoDatos[2],
                    Fecha_Inicio = Convert.ToDateTime(periodoDatos[3]),
                    Fecha_Fin = Convert.ToDateTime(periodoDatos[4]),
                    Fecha_Pago = Convert.ToDateTime(periodoDatos[4]),
                    IdEjercicio = ejercicio,
                    Bimestre = bim,
                    IdSucursal = idsucursal,
                    Ultimo = Convert.ToBoolean(periodoDatos[5]),
                    Especial = Convert.ToBoolean(periodoDatos[6]),
                    IdTipoNominaSat = Convert.ToInt32(periodoDatos[7]),
                    SoloComplemento = soloComplemento,
                    Sindicato = esSindicato,
                    FechaReg = DateTime.Now,
                    IdCliente = idCliente,
                    IdUsuario = idUsuario
                };

                if (tipoNomina == 16) //Asimilado
                {
                    item.IdTipoNominaSat = 2;//E - extraordinaria
                }


                if (item.IdTipoNomina <= 0) return false;

                ctx.NOM_PeriodosPago.Add(item);
                var t = ctx.SaveChanges();

                int idperiodo = item.IdPeriodoPago;
                foreach (var p in arrayE)
                {
                    var peri = new NOM_Empleado_PeriodoPago
                    {
                        IdEmpleado = p,
                        IdPeriodoPago = idperiodo,
                        DiasDescuentoInfonavit = -1
                    };
                    ctx.NOM_Empleado_PeriodoPago.Add(peri);
                    ctx.SaveChanges();
                }


                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public List<EmpleadoNomina> empleadosDetalle(int idPeriodo)
        {
            var datos = (from emp in ctx.Empleado
                         join empPeri in ctx.NOM_Empleado_PeriodoPago
                         on emp.IdEmpleado equals empPeri.IdEmpleado
                         where empPeri.IdPeriodoPago == idPeriodo
                         select new EmpleadoNomina
                         {
                             idempleado = emp.IdEmpleado,
                             Nombres = emp.Nombres,
                             Paterno = emp.APaterno,
                             Materno = emp.AMaterno,
                             RFC = emp.RFC,
                             Status = emp.Status
                         }).ToList();
            return datos;
        }
        /// <summary>
        /// detalle para las nominas normales
        /// </summary>
        /// <param name="idPeriodo"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<EmpleadoNomina> IdempleadosDetalle(int idPeriodo, int idSucursal)
        {
            using (var context = new RHEntities())
            {
                // var periodo = context.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).FirstOrDefault();

                List<EmpleadoNomina> empleados = new List<EmpleadoNomina>();
                var datos = (from emp in context.Empleado
                             join empPeri in context.NOM_Empleado_PeriodoPago
                             on emp.IdEmpleado equals empPeri.IdEmpleado
                             where empPeri.IdPeriodoPago == idPeriodo
                             select emp.IdEmpleado).ToList();

                var datos2 = context.Empleado.Where(x => x.IdSucursal == idSucursal).Select(x => x.IdEmpleado).ToList();

                foreach (var d in datos2)
                {
                    if (!datos.Contains(d))
                    {
                        empleados.AddRange(from emp in context.Empleado
                                               //join ec in ctx.Empleado_Contrato
                                               //on emp.IdEmpleado equals ec.IdEmpleado
                                           where emp.IdSucursal == idSucursal && emp.IdEmpleado == d && emp.Status == true /*&& ec.IdPeriodicidadPago == periodo.IdTipoNomina*/ && emp.RFCValidadoSAT == 1
                                           select new EmpleadoNomina
                                           {
                                               idempleado = emp.IdEmpleado,
                                               Nombres = emp.Nombres,
                                               Paterno = emp.APaterno,
                                               Materno = emp.AMaterno,
                                               RFC = emp.RFC,
                                               Status = emp.Status
                                           });
                    }
                }
                return empleados;
            }

        }
        /// <summary>
        /// Detalle Nomina finiquito
        /// </summary>
        /// <param name="arrayE"></param>
        /// <param name="idPeriodo"></param>
        /// <returns></returns>
        public List<EmpleadoNomina> IdempleadosDetalleFiniquito(int idPeriodo, int idSucursal)
        {
            var periodo = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).FirstOrDefault();
            List<EmpleadoNomina> empleados = new List<EmpleadoNomina>();
            var datos = (from emp in ctx.Empleado
                         join empPeri in ctx.NOM_Empleado_PeriodoPago
                         on emp.IdEmpleado equals empPeri.IdEmpleado
                         where empPeri.IdPeriodoPago == idPeriodo
                         select emp.IdEmpleado).ToList();

            var datos2 = ctx.Empleado.Where(x => x.IdSucursal == idSucursal).Select(x => x.IdEmpleado).ToList();

            foreach (var d in datos2)
            {
                if (!datos.Contains(d))
                {
                    empleados.AddRange(from emp in ctx.Empleado
                                       join ec in ctx.Empleado_Contrato
                                       on emp.IdEmpleado equals ec.IdEmpleado
                                       where emp.IdSucursal == idSucursal && emp.IdEmpleado == d
                                       select new EmpleadoNomina
                                       {
                                           idempleado = emp.IdEmpleado,
                                           Nombres = emp.Nombres,
                                           Paterno = emp.APaterno,
                                           Materno = emp.AMaterno,
                                           RFC = emp.RFC,
                                           Status = emp.Status
                                       });
                }
            }
            return empleados;
        }
        public bool eliminarEmpLista(int[] arrayE, int idPeriodo)
        {
            var resultado = false;
            try
            {
                //Validar periodo
                var itemPeriodo = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                //Si el periodo ya esta autorizado, ya no se podrá hacer cambios
                if (itemPeriodo.Autorizado == true) return false;

                foreach (var e in arrayE)
                {
                    var nomina = ctx.NOM_Nomina.Where(x => x.IdPeriodo == idPeriodo && x.IdEmpleado == e).FirstOrDefault();

                    if (nomina != null)
                    {
                        var nominadetalle = ctx.NOM_Nomina_Detalle.Where(x => x.IdNomina == nomina.IdNomina).ToList();
                        var cuotaimss = ctx.NOM_Cuotas_IMSS.Where(x => x.IdNomina == nomina.IdNomina).FirstOrDefault();
                        if (cuotaimss != null)
                        {
                            const string sqlQuery = "DELETE NOM_Cuotas_IMSS WHERE IdNomina = @p0 and IdFiniquito = @p1";
                            ctx.Database.ExecuteSqlCommand(sqlQuery, cuotaimss.IdNomina, cuotaimss.IdFiniquito);
                        }

                        if (nominadetalle != null)
                        {
                            foreach (var nd in nominadetalle)
                            {
                                const string sqlQuery2 = "DELETE NOM_Nomina_Detalle WHERE IdNomina = @p0 ";
                                ctx.Database.ExecuteSqlCommand(sqlQuery2, nd.Id);
                            }
                        }

                        const string sqlQuery4 = "DELETE NOM_Nomina WHERE IdNomina = @p0 ";
                        ctx.Database.ExecuteSqlCommand(sqlQuery4, nomina.IdNomina);

                    }


                    var complemento = ctx.NOM_Empleado_Complemento.Where(x => x.IdEmpleado == e && x.IdPeriodo == idPeriodo).ToList();

                    if (complemento != null)
                    {
                        foreach (var c in complemento)
                        {
                            const string sqlQuery5 = "DELETE NOM_Empleado_Complemento WHERE IdEmpleadoComplemento = @p0 ";
                            ctx.Database.ExecuteSqlCommand(sqlQuery5, c.IdEmpleadoComplemento);
                        }
                    }

                    const string sqlQuery3 = "DELETE NOM_Empleado_PeriodoPago WHERE IdEmpleado = @p0 and IdPeriodoPago = @p1";
                    ctx.Database.ExecuteSqlCommand(sqlQuery3, e, idPeriodo);
                }
                resultado = true;
            }
            catch (Exception e)
            {
                resultado = false;
            }
            return resultado;
        }

        public bool agregarEmpleadosAPeriodo(int[] arrayE, int idPeriodo)
        {
            var result = false;
            try
            {
                //Validar periodo
                var itemPeriodo = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                //Si el periodo ya esta autorizado, ya no se podrá hacer cambios
                if (itemPeriodo.Autorizado == true) return false;


                foreach (var e in arrayE)
                {
                    var item = new NOM_Empleado_PeriodoPago
                    {
                        IdEmpleado = e,
                        IdPeriodoPago = idPeriodo,
                        DiasDescuentoInfonavit = -1
                    };
                    ctx.NOM_Empleado_PeriodoPago.Add(item);
                    var t = ctx.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public bool cambiarNombre(int idPeriodo, string descripcion)
        {
            //Validar periodo
            var itemPeriodo = ctx.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

            //Si el periodo ya esta autorizado, ya no se podrá hacer cambios
            if (itemPeriodo.Autorizado == true) return false;

            var result = false;
            var item = ctx.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).FirstOrDefault();
            if (item == null) return false;
            item.Descripcion = descripcion;
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        public Task<string> EliminarPeriodoAsync(int idPeriodo)
        {
            //var t = Task.Factory.StartNew(() =>
            //{
            //    var r = EliminarPeriodo(idPeriodo);

            //    return r;
            //});

             var r = EliminarPeriodo(idPeriodo);

            return r;
        }

        private async Task<string> EliminarPeriodo(int idPeriodo)
        {
            var result = false;
            string mensajeRespuesta = null;
            try
            {
                
                using (var context = new RHEntities()) //mtyc
                {
                    var periodo = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == idPeriodo);

                    if (periodo == null) return "No se encontró el periodo";

                    //Si el periodo ya esta autorizado, ya no se podrá hacer cambios
                    if (periodo.Autorizado)
                    {
                        mensajeRespuesta = "No se puede eliminar un periodo autorizado";
                        return mensajeRespuesta;
                    }

                    var listaTimbres =
                        context.NOM_CFDI_Timbrado.Where(x => x.IdPeriodo == idPeriodo && x.FolioFiscalUUID != null)
                            .Select(x => x.IdTimbrado)
                            .ToList();

                    if (listaTimbres.Count > 0)
                    {
                        mensajeRespuesta = "No se puede eliminar un periodo que tiene registros timbrados o cancelados";
                        return mensajeRespuesta;
                    }

                    if (periodo.Autorizado == false)
                    {
                        //si es finiquito
                        // si es nomina, buscamos las nominas generadas 
                        // tablas a eliminar

                        var listaDeNominasId = context.NOM_Nomina.Where(x => x.IdPeriodo == idPeriodo).Select(x => x.IdNomina).ToArray();
                        var listaDeFiniquitosId = context.NOM_Finiquito.Where(x => x.IdPeriodo == idPeriodo).Select(x => x.IdFiniquito).ToArray();
                        //eliminar de nominas
                        var nominasArray = string.Join(",", listaDeNominasId);
                        var finiquitosArray = string.Join(",", listaDeFiniquitosId);

                        if (periodo.IdTipoNomina == 11) //finiquito
                        {
                            //CUOTAS IMSS finiquitos
                            if (finiquitosArray.Length > 0)
                            {
                                string sqlQuery6 = "DELETE [NOM_Cuotas_IMSS] WHERE [IdFiniquito] in (" + finiquitosArray +
                                                   ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery6);

                                // NOM_Finiquito_Complemento
                                string sqlQuery19 = "DELETE [NOM_Finiquito_Complemento] WHERE [IdFiniquito] in (" + finiquitosArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery19);

                                //NOM_Finiquito_Detalle
                                string sqlQuery21 = "DELETE [NOM_Finiquito_Detalle] WHERE [IdFiniquito] in (" + finiquitosArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery21);

                                //NOM_Incapacidad
                                string sqlQuery22 = "DELETE [NOM_Incapacidad] WHERE [IdFiniquito] in (" + finiquitosArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery22);

                                //elimnar finiquitos
                                string sqlQuery2 = "DELETE [NOM_Finiquito] WHERE [IdFiniquito] in (" + finiquitosArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery2);

                            }

                          
                        }
                        else
                        {
                            if (nominasArray.Length > 0)
                            {
                                //Historial de pagos
                                string sqlQuery3 = "DELETE [HistorialPagos] WHERE IdNomina in (" + nominasArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery3);




                                //nominas detalle
                                string sqlQuery4 = "DELETE [NOM_Nomina_Detalle] WHERE IdNomina in (" + nominasArray +
                                                   ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery4);

                                //CUOTAS IMSS - nomina
                                string sqlQuery5 = "DELETE [NOM_Cuotas_IMSS] WHERE IdNomina in (" + nominasArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery5);

                                string sqlQuery23 = "DELETE [NOM_Incapacidad] WHERE IdNomina in (" + nominasArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery23);

                                string sqlQuery1 = "DELETE [NOM_Nomina] WHERE IdNomina in (" + nominasArray + ")";
                                await context.Database.ExecuteSqlCommandAsync(sqlQuery1);
                            }

                        }


                        //Empleado complemento 
                        string sqlQuery7 = "DELETE [NOM_Empleado_Complemento] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery7);

                        //[NOM_Empleado_PeriodoPago]
                        string sqlQuery8 = "DELETE [NOM_Empleado_PeriodoPago] WHERE [IdPeriodoPago] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery8);

                        //Inasistencias
                        string sqlQuery10 = "DELETE [Inasistencias] WHERE [idPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery10);

                        //[NOM_Aguinaldo]
                        string sqlQuery11 = "DELETE [NOM_Aguinaldo] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery11);

                        //NOM_Empleado_Asimilado
                        string sqlQuery12 = "DELETE [NOM_Empleado_Asimilado] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery12);

                        //NOM_Empleado_Sindicato
                        string sqlQuery13 = "DELETE [NOM_Empleado_Sindicato] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery13);

                        //NOM_Facturacion
                        string sqlQuery14 = "DELETE [NOM_Facturacion] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery14);

                        //NOM_FacturacionC_Finiquito
                        string sqlQuery15 = "DELETE [NOM_FacturacionC_Finiquito] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery15);

                        //NOM_FacturacionComplemento
                        string sqlQuery16 = "DELETE [NOM_FacturacionComplemento] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery16);

                        //NOM_FacturacionF_Finiquito
                        string sqlQuery17 = "DELETE [NOM_FacturacionF_Finiquito] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery17);

                        //NOM_FacturacionSindicato
                        string sqlQuery18 = "DELETE [NOM_FacturacionSindicato] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery18);

                        //NOM_Finiquito_Descuento_Adicional
                        string sqlQuery20 = "DELETE [NOM_Finiquito_Descuento_Adicional] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery20);

                        //NOM_Nomina_Ajuste
                        string sqlQuery24 = "DELETE [NOM_Nomina_Ajuste] WHERE [IdPeriodo] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery24);

                        string sqlQuery25 = "DELETE [NOM_CFDI_Timbrado] WHERE [IdPeriodo] in (" + idPeriodo + ") and [FolioFiscalUUID] is null and [FechaCertificacion] is null ";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery25);


                        //[NOM_Empleado_PeriodoPago] -- dejar como el ultimo a eliminarse
                        string sqlQuery9 = "DELETE [NOM_PeriodosPago] WHERE [IdPeriodoPago] in (" + idPeriodo + ")";
                        await context.Database.ExecuteSqlCommandAsync(sqlQuery9);

                        //context.Database.ExecuteSqlCommandAsync(sqlQuery25);
                        //context.Database.ExecuteSqlCommand(sqlQuery25);




                        mensajeRespuesta = "Se eliminó el periodo correctamente";


                    }
                }

               

            }
            catch (Exception e)
            {
                result = true;
                mensajeRespuesta = "ex - ?";
            }
            return mensajeRespuesta;
        }

        public void UpdateCfdiEstatus(NOM_PeriodosPago pp, GenerarCfdiEstatus estatus)
        {
            using (var context = new RHEntities())
            {
                var item = context.NOM_PeriodosPago.FirstOrDefault(x => x.IdPeriodoPago == pp.IdPeriodoPago);
                if (item != null) item.CfdiGenerado = (int)estatus;
                context.SaveChangesAsync();
            }
        }

        public List<PeriodoInfo> GetPeriodoInfo(int idSucursal, int idEjercicio)
        {
            //inicialmente no se usara el ejercicio 
            //hasta que se implemente el filtrado por ejerercio al seleccionar un periodo

            //una lista de periodos de la sucursal
            //una lista de las nominas de todos los periodos del array
            //una lista de timbrados de todos los periodos del array
            List<NOM_PeriodosPago> listaPeriodos = new List<NOM_PeriodosPago>();
            List<int> listaNominas = new List<int>(); //solo id del periodo en las nominas donde total sea > 0
            List<int> listaTimbrados = new List<int>(); //solo id del periodo donde el registro tenga UUID != null
            List<int> listaEmpleados = new List<int>(); //solo id del periodo de la tabla NOM_Empleado_periodo

            #region GET DATA
            using (var context = new RHEntities())
            {
                //aqui se deberá filtrar tambien por id del ejercicio
                listaPeriodos = context.NOM_PeriodosPago.Where(x => x.IdSucursal == idSucursal).ToList();

                var arrayIdPeriodosDeLaSucursal = listaPeriodos.Select(x => x.IdPeriodoPago).ToArray();

                listaEmpleados = (from e in context.NOM_Empleado_PeriodoPago
                                  where arrayIdPeriodosDeLaSucursal.Contains(e.IdPeriodoPago)
                                  select e.IdPeriodoPago).ToList();

                listaNominas = (from n in context.NOM_Nomina
                                where arrayIdPeriodosDeLaSucursal.Contains(n.IdPeriodo)
                                select n.IdPeriodo).ToList();

                var listaFiniquitos = (from n in context.NOM_Finiquito
                                       where arrayIdPeriodosDeLaSucursal.Contains(n.IdPeriodo)
                                       select n.IdPeriodo).ToList();

                if (listaFiniquitos.Count > 0)
                {
                    listaNominas.AddRange(listaFiniquitos);
                }


                //No se contempla los cancelados - tambien se deberán de agregar en la info
                listaTimbrados = (from t in context.NOM_CFDI_Timbrado
                                  where arrayIdPeriodosDeLaSucursal.Contains(t.IdPeriodo)
                                  && t.FolioFiscalUUID != null && t.FechaCertificacion != null
                                  && t.Cancelado == false//osea que no ha sido cancelado
                                  select t.IdPeriodo).ToList();
            }
            #endregion


            List<PeriodoInfo> listaInfos = new List<PeriodoInfo>();
            int numEmpeados = 0;
            int numNominas = 0;
            int numTimbrados = 0;
            int numCancelados = 0;

            //for 
            foreach (var item in listaPeriodos)
            {
                numEmpeados = 0;
                numNominas = 0;
                numTimbrados = 0;
                numCancelados = 0;
                PeriodoInfo itemInfo = new PeriodoInfo();

                //obtener los empleados de periodo
                numEmpeados = listaEmpleados.Count(x => x == item.IdPeriodoPago);
                //obtener las nominas del periodo
                numNominas = listaNominas.Count(x => x == item.IdPeriodoPago);
                //obtener los timbres del periodo
                numTimbrados = listaTimbrados.Count(x => x == item.IdPeriodoPago);

                itemInfo.IdPeriodo = item.IdPeriodoPago;
                itemInfo.EsComplemento = item.SoloComplemento;
                itemInfo.IdSucursal = item.IdSucursal;
                itemInfo.NumEmpleados = numEmpeados;
                itemInfo.NumNominas = numNominas;
                itemInfo.NumTimbrados = numTimbrados;
                itemInfo.NumCancelados = 0;

                listaInfos.Add(itemInfo);
            }


            return listaInfos;

        }
        public List<Tuple<int, string>> GetPeriodosBySucursalTimbrados(int idSucursal, int idEjercicio, int idEmisor)
        {
            using (var context = new RHEntities())
            {
                var listaTimbrados =
                    context.NOM_CFDI_Timbrado.Where(x => x.IdSucursal == idSucursal && x.IdEjercicio == idEjercicio && x.IdEmisor == idEmisor)
                        .Select(x => x.IdPeriodo)
                        .Distinct()
                        .ToArray();

                var listaPeriodos = (from p in context.NOM_PeriodosPago
                    where listaTimbrados.Contains(p.IdPeriodoPago)
                    select new {p.IdPeriodoPago, p.Descripcion}).ToList();

                var listaTupla = listaPeriodos.Select(t => new Tuple<int, string>(t.IdPeriodoPago, t.Descripcion)).ToList();

                return listaTupla;
            }
        }

        public bool SetPeriodoPago(int idPeriodo) {
            using (var context = new RHEntities()) //mtyc
            {
                var result = false;
                var item = context.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).FirstOrDefault();
                if (item == null) return false;
                item.Pagado = true;
                item.FechaPagoBanco = DateTime.Now;
                var r = context.SaveChanges();

                if (r > 0)
                    result = true;

                return result;
            }
        }
    }

    public class EmpleadoNomina
    {
        public EmpleadoNomina()
        {
            idempleado = 0;
            IdPeriodicidadPago = 0;
            PeriodicidadPago = "Default";
        }

        public int idempleado { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string RFC { get; set; }
        public bool Status { get; set; }
        public int IdPeriodicidadPago { get; set; }
        public string PeriodicidadPago { get; set; }
    }
}
