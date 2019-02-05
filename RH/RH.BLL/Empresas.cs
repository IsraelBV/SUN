using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Common.Enums;
using Novacode;

namespace RH.BLL
{
   public class Empresas
    {
        RHEntities ctx = null;

        public Empresas()
        {
            ctx = new RHEntities();
        }

        public int GetEsquemaEmpresa(int IdEmpresa)
        {
            
            return ctx.Sucursal_Empresa.Where(x => x.IdEmpresa == IdEmpresa).Select(x => x.IdEsquema).FirstOrDefault();
        }

        public List<Empresa> GetEmpresasByGuia()
        {
            return ctx.Empresa.Where(x => x.Guia != null).ToList();
        }

        public Empresa GetEmpresaById(int id)
        {
            return ctx.Empresa.Where(x => x.IdEmpresa == id).FirstOrDefault();
        }

        public List<Empresa> GetRegistrosPatronales()
        {
            List<Empresa> lista = null;

            lista = ctx.Empresa.ToList();

            return lista;
        }

        public List<Empresa> GetAllEmpresas()
        {
            using (var context = new RHEntities())
            {
                return context.Empresa.ToList();
            }
        }

        public static List<Empresa> GetEmpresasFiscales()
        {
            using (var context = new RHEntities())
            {
                var lista = (from emp in context.Empresa
                    where emp.RegistroPatronal != null
                    select emp).ToList();

                return lista;
            }
        }


        public List<Cliente> GetClientesByIdEmpresa(int id)
        {
            using (var context = new RHEntities())
            {
                var arraySucursales =
                    context.Sucursal_Empresa.Where(x => x.IdEmpresa == id).Select(x => x.IdSucursal).ToArray();



                var listaClientes = (from cli in context.Cliente
                    join suc in context.Sucursal on cli.IdCliente equals suc.IdCliente
                    where arraySucursales.Contains(suc.IdSucursal)
                    select cli).ToList();

                return listaClientes;
            }
        }

        public int GetIdByRazonSocial(string razon, int idCliente)
        {
            return ctx.Empresa.Where(x => x.RazonSocial.Equals(razon)).Select(x => x.IdEmpresa).FirstOrDefault();
        }

        public List<RegistroDatos> GetEmpresasBySucursal(int IdSucursal)
        {
            return (from se in ctx.Sucursal_Empresa
                    join emp in ctx.Empresa on se.IdEmpresa equals emp.IdEmpresa
                    where se.IdSucursal == IdSucursal
                    select new RegistroDatos
                    {
                        IdEmpresa = emp.IdEmpresa,
                        RazonSocial = emp.RazonSocial,
                        IdEsquema = se.IdEsquema
                    }
                    ).ToList();
        }

        public List<string> GetEmpresasBySucursal(int IdSucursal, int Esquema)
        {
            return (from se in ctx.Sucursal_Empresa
                    join emp in ctx.Empresa on se.IdEmpresa equals emp.IdEmpresa
                    where se.IdEsquema == Esquema && se.IdSucursal == IdSucursal
                    select emp.RazonSocial
                    ).ToList();
        }
        

        public List<C_Esquema> GetEsquemas()
        {
            return ctx.C_Esquema.ToList();
        }

        public List<C_RegimenFiscal_SAT> Regimen()
        {
            return ctx.C_RegimenFiscal_SAT.ToList();
        }

        public RegistroDatos GetRegistroPatronalById(int? id)
        {
           

            var datos1 = from pat in ctx.Empresa
                         join es in ctx.C_Estado
                         on pat.IdEstado equals es.IdEstado
                      where pat.IdEmpresa == id
            select new RegistroDatos
                        {

                            IdRegistro = pat.IdEmpresa,
                            RegistroPatronal = pat.RegistroPatronal,
                            Guia = pat.Guia,
                            ClaveSeguro = pat.ClaveSeguro,
                            Clase = pat.Clase,
                            PrimaRiesgo = pat.PrimaRiesgo,
                            RFC = pat.RFC,
                            RazonSocial = pat.RazonSocial,
                            CP = pat.CP,
                            Pais = pat.Pais,
                            IdEstado = es.IdEstado,
                            Estado = es.Descripcion,
                            Municipio = pat.Municipio,
                            NoExt = pat.NoExt,
                            Calle = pat.Calle,
                            RegimenFiscal = pat.RegimenFiscal, //probando si aqui debe ir
                            Status = pat.Status,
                            Colonia = pat.Colonia



                        };
                        
            return datos1.FirstOrDefault();
        }

    
        public List<RegistroDatos> GetEstadosByIQueryable()
        {
            

            var datos = from pat in ctx.Empresa
                        join es in ctx.C_Estado
                         on pat.IdEstado equals es.IdEstado

                        select new RegistroDatos
                        {

                            IdRegistro = pat.IdEmpresa,
                            RegistroPatronal = pat.RegistroPatronal,
                            Guia = pat.Guia,
                            ClaveSeguro = pat.ClaveSeguro,
                            Clase = pat.Clase,
                            PrimaRiesgo = pat.PrimaRiesgo,
                            RFC = pat.RFC,
                            RazonSocial = pat.RazonSocial,
                            CP = pat.CP,
                            Pais = pat.Pais,
                            IdEstado = es.IdEstado,
                            Estado = es.Descripcion,
                            Municipio = pat.Municipio,
                            NoExt = pat.NoExt,
                            Calle = pat.Calle,
                            Status = pat.Status,
                            Colonia = pat.Colonia



                        };

            return datos.ToList();
        }

      

        public bool DeleteRegistroPatronal(int id, Empresas bn)
        {
            var result = false;

            var item = ctx.Empresa.FirstOrDefault(x => x.IdEmpresa == id);

            //si no se encontro un registro con ese id,
            // retornamos false 
            if (item == null) return false;

            ctx.Empresa.Attach(item);
            ctx.Empresa.Remove(item);

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        public bool UpdateRegistroPatronal(int id, RegistroDatos bn)
        {
          
            var result = false;

            var item = ctx.Empresa.FirstOrDefault(x => x.IdEmpresa == id);

          
            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;
            item.Clase = bn.Clase;
            item.Guia = bn.Guia;
            item.ClaveSeguro = bn.ClaveSeguro;
            item.RegistroPatronal = bn.RegistroPatronal;
            item.PrimaRiesgo = bn.PrimaRiesgo;
            item.RFC = bn.RFC;
            item.RazonSocial = bn.RazonSocial;
            item.CP = bn.CP;
            item.Pais = bn.Pais;
            item.RegimenFiscal = bn.RegimenFiscal;
           
            item.IdEstado = Convert.ToInt32(bn.Estado);
            item.Municipio = bn.Municipio;
            item.Colonia = bn.Colonia;
            item.NoExt = bn.NoExt;
            item.Calle = bn.Calle;
            item.Status = true;

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }
        public bool ActivarEmpresa(int id)
        {

            var result = false;

            var item = ctx.Empresa.FirstOrDefault(x => x.IdEmpresa == id);


            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;
      
            item.Status = true;

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }
        public bool DesactivarEmpresa(int id)
        {

            var result = false;

            var item = ctx.Empresa.FirstOrDefault(x => x.IdEmpresa == id);


            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;

            item.Status = false;

            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }


        public bool CreateRegistroPatronal(Empresa rp)
        {
            var result = false;

            rp.Status = true;

            ctx.Empresa.Add(rp);
            var r = ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        public string GetRazonSocialById(int? IdEmpresa)
        {
            if (IdEmpresa == null) return "n/a";
            var empresa = ctx.Empresa.FirstOrDefault(x => x.IdEmpresa == IdEmpresa);
            return empresa != null ? empresa.RazonSocial : "n/a";
        }

        //obtener empresas por usuario en la sesion
        public List<Empresa> getEmpresasByUser(SYA_Usuarios usuario)
        {
            var listaEmpresas = new List<Empresa>();

            using (var context = new RHEntities())
            {
                if (usuario.Sucursales != "*")
                {
                    var sucursalesUsuario = usuario.Sucursales.Split(',').Select(Int32.Parse).ToArray();
                    listaEmpresas = (from listEmp in context.Empresa
                                     join sucEmp in context.Sucursal_Empresa on listEmp.IdEmpresa equals sucEmp.IdEmpresa
                                     where sucEmp.IdEsquema == 1 && sucursalesUsuario.Contains(sucEmp.IdSucursal)
                                     select listEmp).ToList();
                }
                else
                {

                    listaEmpresas = context.Empresa.Where(x => x.RegistroPatronal != null).ToList();
                }
            }
            return listaEmpresas;
        }
    }

    public class RegistroDatos
    {
        public int IdRegistro { get; set; }
        public string RegistroPatronal { get; set; }
        public string Guia { get; set; }
        public string ClaveSeguro { get; set; }
        public string Clase { get; set; }
        public decimal? PrimaRiesgo { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string CP { get; set; }
        public int Pais { get; set; }
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string NoExt { get; set; }
        public string Calle { get; set; }
        public bool Status { get; set; }
        public string Colonia { get; set; }
        public string Esquema { get; set; }
        public int IdEsquema { get; set; }
        public int IdEmpresa { get; set; }
        public int RegimenFiscal { get; set; }
    }

    public class Empleado_Empresa_Configuracion
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; }
        public int Esquema { get; set; }
    }
}
