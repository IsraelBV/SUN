using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class Clientes
    {
        private readonly RHEntities _ctx = null;

        public Clientes()
        {
            _ctx = new RHEntities();
        }

        /// <summary>
        /// Retorna todas los clientes, activas o inactivas
        /// </summary>
        /// <returns></returns>
        public List<Cliente> GetClientes()
        {
            List<Cliente> lista = null;

            lista = _ctx.Cliente.ToList();

            return lista;
        }

        public List<Cliente> GetClientesActivos()
        {
            return _ctx.Cliente.Where(x => x.Status == true).ToList();
        }

        public Cliente GetClienteById(int idCliente)
        {
            if (idCliente <= 0) return null;
            return _ctx.Cliente.FirstOrDefault(x => x.IdCliente == idCliente);
        }

        public bool GuardarCliente(Cliente nuevaCliente, List<int> idsDepartamento)
        {
            
            //buscamos un registro que tenga el nombre del nuevo cliente
            var item = _ctx.Cliente.Where(x => x.Nombre == nuevaCliente.Nombre).FirstOrDefault();
            //si el registro buscado es null o no existe entonces agrega el nuevo cliente.
            if (item == null)
            {
                nuevaCliente.Status = true;
                _ctx.Cliente.Add(nuevaCliente);
                var r = _ctx.SaveChanges();

                if (r <= 0) return false;


               // GuardarConfiguracionDepartamento(nuevaCliente.IdCliente, idsDepartamento);

                return true;
            }
            //si el registro ya existe retornamos un falso.
            else
            {
                return false;
            }
       }
        public void ActualizarCliente(Cliente editarCliente, List<int> idsDepartamento,int idCliente)
        {
            var Cliente = _ctx.Cliente.FirstOrDefault(x => x.IdCliente == idCliente);
            if (Cliente != null)
            {
                Cliente.Nombre = editarCliente.Nombre;
                Cliente.Rfc = editarCliente.Rfc;
                Cliente.NodoSubcontratacion = editarCliente.NodoSubcontratacion;

                Cliente.Status = true;

                var re = _ctx.SaveChanges();

                //const string sqlQuery2 = "DELETE Cliente_DEPARTAMENTO WHERE IdCliente = @p0";
                //_ctx.Database.ExecuteSqlCommand(sqlQuery2, idCliente);
                //GuardarConfiguracionDepartamento(editarCliente.IdCliente, idsDepartamento);

            }

        }
 
        private void GuardarConfiguracionDepartamento(int idCliente, List<int> idsDepartamento)
        {
            if (idsDepartamento == null) return;
            foreach (var d in idsDepartamento)
            {
                var item = new Cliente_Departamento()
                {
                    IdCliente = idCliente,
                    IdDepto = d
                };

                _ctx.Cliente_Departamento.Add(item);
                var r = _ctx.SaveChanges();
            }
        }
  
        public List<ClienteDepartamentos> GetClienteDepartamentos(int idCliente)
        {
            var listaDeptos = _ctx.Departamento.ToList();
            var listaEDepto = _ctx.Cliente_Departamento.Where(x => x.IdCliente == idCliente).ToList();
            var array = listaEDepto.Select(x => x.IdDepto).ToArray();

            var listaEd = new List<ClienteDepartamentos>();

            foreach (var rp in listaDeptos)
            {
                var sel = array.Contains(rp.IdDepartamento);

                listaEd.Add(new ClienteDepartamentos { IdDepartamento = rp.IdDepartamento, NombreDepartamento = rp.Descripcion, Seleccionado = sel });
            }


            return listaEd;
        }

        public string GetClienteBySucursal(int idSucursal)
        {

            var item = (from c in _ctx.Cliente
                        join s in _ctx.Sucursal
                        on c.IdCliente equals s.IdCliente
                        where s.IdSucursal == idSucursal
                        select c.Nombre).FirstOrDefault();

            return item;

            //using (var context = new RHEntities())
            //{
            //    var item = context.Sucursal.FirstOrDefault(x => x.IdSucursal == idSucursal);

            //    return item;
            //}


            //return (from s in _ctx.Sucursal
            //        join c in _ctx.Cliente on s.IdCliente equals c.IdCliente
            //        select c
            //        ).FirstOrDefault();
        }

        public int GetIdClienteBySucursal(int idSucursal)
        {
            return (from s in _ctx.Sucursal
                    join c in _ctx.Cliente on s.IdCliente equals c.IdCliente
                    where s.IdSucursal == idSucursal
                    select c.IdCliente).FirstOrDefault();
        }

        public bool ExisteDB(string DB)
        {

            const string sqlQuery = "SELECT name FROM DBO.SYSDATABASES WHERE NAME = @p0";

            var dbName = _ctx.Database.SqlQuery<string>(sqlQuery, DB).FirstOrDefault<string>();

            return dbName != null;  
        }
 
        public List<ClienteDepartamentos> DetallesDepartamentos (int idcliente)
        {
            var datos = from cli_depa in _ctx.Cliente_Departamento
                        join depa in _ctx.Departamento
                        on cli_depa.IdDepto equals depa.IdDepartamento
                        where cli_depa.IdCliente == idcliente
                        select new ClienteDepartamentos
                        {
                            IdDepartamento = depa.IdDepartamento,
                            NombreDepartamento = depa.Descripcion
                        };

                        return datos.ToList();
        }

        public int GetDepartamentosAsignadosBycliente(int idCliente)
        {
            var item = _ctx.Cliente_Departamento.Count(c => c.IdCliente == idCliente);

            return item;
        }


 #region "SUCURSALES"

        public bool GuardarSucursalCliente(Sucursal sucursal, List<int> EmpresaFis, List<int> EmpresaComp, List<int> EmpresaAsim, List<int> EmpresaSin)
        {
            
            _ctx.Sucursal.Add(sucursal);
            var r = _ctx.SaveChanges();

            if (r <= 0) return false;

            GuardarConfiguracionSucursalEmpresa(sucursal.IdSucursal, EmpresaFis,1);
            GuardarConfiguracionSucursalEmpresa(sucursal.IdSucursal, EmpresaComp, 2);
            GuardarConfiguracionSucursalEmpresa(sucursal.IdSucursal, EmpresaSin, 3);
            GuardarConfiguracionSucursalEmpresa(sucursal.IdSucursal, EmpresaAsim, 4);


            return true;
        }
        private void GuardarConfiguracionSucursalEmpresa(int idSucursal, List<int> idsRegistro, int Esquema)
        {
            if (idsRegistro == null) return;

            foreach (var r in idsRegistro)
            {
                var item = new Sucursal_Empresa()
                {
                    IdSucursal = idSucursal,
                    IdEmpresa = r,
                    IdEsquema = Esquema,
                    Status = true
                };
                _ctx.Sucursal_Empresa.Add(item);
            }
            var t = _ctx.SaveChanges();
        }
        public List<SucursalCliente> GetSucursalesByIdCliente(int idCliente)
        {
            var listaSuc = (from s in _ctx.Sucursal
                join e in _ctx.Cliente on s.IdCliente equals e.IdCliente
                join est in _ctx.C_Estado on s.IdEstado equals est.IdEstado
                join z in _ctx.ZonaSalario on s.IdZonaSalario equals z.IdZonaSalario
                where s.IdCliente == idCliente
                select new SucursalCliente
                {
                  IdSucursal = s.IdSucursal,
                  Ciudad = s.Ciudad,
                  Estado = est.Descripcion,
                  ZonaSalario = z.Zona,
                  Status = s.Status,
                  NomCliente = e.Nombre,
                  IdZona = s.IdZonaSalario,
                  IdEstado = s.IdEstado,
                  IdCliente = s.IdCliente,
                  ValorZonaSalario = z.SMG.ToString()
                }).ToList();

            return listaSuc;
        }
        public List<ZonaSalario> GetZonaSalario()
        {
            return _ctx.ZonaSalario.ToList();
        }
        public Sucursal GetSucursalById(int idSucursal)
        {
            if (idSucursal <= 0) return null;
            return _ctx.Sucursal.FirstOrDefault(x => x.IdSucursal == idSucursal);
        }
        public SucursalCliente GetSucursalClienteById(int idSucursal)
        {
            if (idSucursal <= 0) return null;

            var item = (from s in _ctx.Sucursal
                join e in _ctx.Cliente on s.IdCliente equals e.IdCliente
                join est in _ctx.C_Estado on s.IdEstado equals est.IdEstado
                join z in _ctx.ZonaSalario on s.IdZonaSalario equals z.IdZonaSalario
                where s.IdSucursal == idSucursal
                select new SucursalCliente
                {
                    IdSucursal = s.IdSucursal,
                    Ciudad = s.Ciudad,
                    Estado = est.Descripcion,
                    ZonaSalario = z.Zona,
                    Status = s.Status,
                    NomCliente = e.Nombre,
                    IdZona = s.IdZonaSalario,
                    IdEstado = s.IdEstado,
                    IdCliente = s.IdCliente,
                    ValorZonaSalario = z.SMG.ToString()
                }).ToList();



            return item.FirstOrDefault();
        }
        public List<SucursalEmpresas> GetSucursalEmpresaSeleccionada (int IdSucursal, int Tipo, int esquema)
        {
            var listaEr = new List<SucursalEmpresas>();
          if(Tipo == 1)
            {
                var listaRp = _ctx.Empresa.Where(x => x.RegistroPatronal != null).ToList();
                var listaErp = _ctx.Sucursal_Empresa.Where(x => x.IdSucursal == IdSucursal && x.IdEsquema == esquema).ToList();
                var array = listaErp.Select(x => x.IdEmpresa).ToArray();



                foreach (var rp in listaRp)
                {
                    var sel = array.Contains(rp.IdEmpresa);

                    listaEr.Add(new SucursalEmpresas { IdEmpresa = rp.IdEmpresa, NombreEmpresa = rp.RazonSocial, Seleccionado = sel, Esquema = esquema });
                }
                return listaEr;
            }
            else
            {
                var listaRp = _ctx.Empresa.Where(x => x.RegistroPatronal == null).ToList();
                var listaErp = _ctx.Sucursal_Empresa.Where(x => x.IdSucursal == IdSucursal && x.IdEsquema == esquema).ToList();
                var array = listaErp.Select(x => x.IdEmpresa).ToArray();



                foreach (var rp in listaRp)
                {
                    var sel = array.Contains(rp.IdEmpresa);

                    listaEr.Add(new SucursalEmpresas { IdEmpresa = rp.IdEmpresa, NombreEmpresa = rp.RazonSocial, Seleccionado = sel, Esquema = esquema });
                }
                return listaEr;
            }
               
            }
        public List<SucursalEmpresas> GetSucursalEmpresas(int idSucursal)
        {
            var listaEmpresas = _ctx.Empresa.ToList();
            var listaSucursalEmpreas = _ctx.Sucursal_Empresa.Where(x => x.IdSucursal == idSucursal).ToList();
            var array = listaSucursalEmpreas.Select(x => x.IdEmpresa).ToArray();

            var listaEr = new List<SucursalEmpresas>();

            foreach (var rp in listaEmpresas)
            {
                var sel = array.Contains(rp.IdEmpresa);

                listaEr.Add(new SucursalEmpresas { IdEmpresa = rp.IdEmpresa, NombreEmpresa = rp.RazonSocial, Seleccionado = sel });
            }


            return listaEr;
        }
        public void ActualizarSucursal(Sucursal editarSucursal, List<int> Fiscal , List<int> Asimilado , List<int> Sindicato , List<int> Complemento)
        {
            var sucursal = _ctx.Sucursal.FirstOrDefault(x => x.IdSucursal == editarSucursal.IdSucursal);

            if (sucursal != null)
            {
                sucursal.IdCliente = editarSucursal.IdCliente;
                sucursal.Ciudad = editarSucursal.Ciudad;
                sucursal.IdEstado = editarSucursal.IdEstado;
                sucursal.IdZonaSalario = editarSucursal.IdZonaSalario;
                sucursal.Status = editarSucursal.Status;

                var re = _ctx.SaveChanges();

                const string sqlQuery = "DELETE Sucursal_Empresa WHERE IdSucursal = @p0";
                

                _ctx.Database.ExecuteSqlCommand(sqlQuery, editarSucursal.IdSucursal);

                //GuardarConfiguracionSucursalEmpresa(editarSucursal.IdSucursal, idsRegistro);
                GuardarConfiguracionSucursalEmpresa(editarSucursal.IdSucursal, Fiscal, 1);
                GuardarConfiguracionSucursalEmpresa(editarSucursal.IdSucursal, Complemento, 2);
                GuardarConfiguracionSucursalEmpresa(editarSucursal.IdSucursal, Sindicato, 3);
                GuardarConfiguracionSucursalEmpresa(editarSucursal.IdSucursal, Asimilado, 4);

            }
        }

        public List<SucursalEmpresas> DetalleSucursal (int IdSucursal)
        {
            var datos = from emp in _ctx.Empresa
                        join suc_emp in _ctx.Sucursal_Empresa
                        on emp.IdEmpresa equals suc_emp.IdEmpresa
                        where suc_emp.IdSucursal == IdSucursal
                        select new SucursalEmpresas
                        {
                            IdEmpresa = suc_emp.IdEmpresa,
                            NombreEmpresa = emp.RazonSocial,
                            Esquema = suc_emp.IdEsquema

                        };
            return datos.ToList();

        }

        #endregion

        //obtener sucursales por usuario en la sesion
        public List<Cliente> getClientesByUser(SYA_Usuarios usuario)
        {
            var listaClientes = new List<Cliente>();

            using (var context = new RHEntities())
            {
                if (usuario.Sucursales != "*")
                {
                    var sucursalesUsuario = usuario.Sucursales.Split(',').Select(Int32.Parse).ToArray();

                    listaClientes = (from suc in context.Sucursal
                                      join cli in context.Cliente on suc.IdCliente equals cli.IdCliente
                                       where sucursalesUsuario.Contains(suc.IdSucursal)
                                       select cli).ToList();
                }
                else
                {
                    listaClientes = context.Cliente.ToList();
                }
            }
            return listaClientes;
        }
    }


    public class ClienteEmpresas
    {
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public bool Seleccionado { get; set; }
        public int Esquema { get; set; }
    }

    public class ClienteDepartamentos
    {
        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; }
        public bool Seleccionado { get; set; }
    }

    public class SucursalCliente
    {
        public int IdSucursal { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string ZonaSalario { get; set; }
        public bool  Status { get; set; }
        public string NomCliente { get; set; }
        public int IdZona { get; set; }
        public int IdEstado { get; set; }
        public int IdCliente { get; set; }
        public string ValorZonaSalario { get; set; }


    }

    public class SucursalEmpresas
    {
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public bool Seleccionado { get; set; }
        public int Esquema { get; set; }
    }
}
