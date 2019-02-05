using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using System.IO;
using ClosedXML.Excel;
using System.Data;

namespace Nomina.BLL
{
   public class _DatosBancarios
    {
        RHEntities ctx = null;
        public _DatosBancarios()
        {
            ctx = new RHEntities();
        }

        public List<CliSuc> Clientes()
        {
            var datos = (from c in ctx.Cliente
                         join s in ctx.Sucursal
                         on c.IdCliente equals s.IdCliente
                         select new CliSuc
                         {
                             IdCliente = c.IdCliente,
                             NomCliente = c.Nombre,
                             IdSucursal = s.IdSucursal,
                             NomSucursal = s.Ciudad

                         }).OrderBy(x=>x.NomCliente).ToList();
            return datos;
        }

        public List<Empleado> empleados (int idSucursal)
        {
            var datos = ctx.Empleado.Where(x => x.IdSucursal == idSucursal).ToList();
            return datos;
        }

        public EmpleadoBank datosBank (int idEmpleado)
        {
            var datos = ctx.DatosBancarios.Where(x => x.IdEmpleado == idEmpleado).FirstOrDefault();
            var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == idEmpleado).OrderByDescending(x=>x.IdContrato).Select(x => x.FormaPago).FirstOrDefault();
            var metodo = ctx.C_Metodos_Pago.Where(x => x.IdMetodo == contrato).FirstOrDefault();
            EmpleadoBank item = new EmpleadoBank();
            if(datos == null)
            {
                item.IdBanco = 0;
                item.IdEmpleado = idEmpleado;
                item.Banco = "0";
                item.NoSiga = 0;
                item.NoSIga2 = 0;
                item.cuenta = "0";
                item.Tarjeta = "0";
                item.Clabe = "0";
                item.IdMetodoPago = 0;
                item.MetodoPago = "";
                return item;
            }else
            {
                var banco = ctx.C_Banco_SAT.Where(x => x.IdBanco == datos.IdBanco).FirstOrDefault();

                item.IdBanco = banco.IdBanco;
                item.IdEmpleado = idEmpleado;
                item.Banco = banco.Descripcion;
                item.NoSiga = datos.NoSigaF;
                item.NoSIga2 = datos.NoSigaC;
                item.cuenta = datos.CuentaBancaria;
                item.Tarjeta = datos.NumeroTarjeta;
                item.Clabe = datos.Clabe;
                item.IdMetodoPago = contrato;
                item.MetodoPago = metodo.Descripcion;
                return item;
            }
            
        }

        public List<C_Banco_SAT> listBancos()
        {
            var datos = ctx.C_Banco_SAT.ToList();
            return datos;
        }

        public List<C_Metodos_Pago> metodos()
        {
            var datos = ctx.C_Metodos_Pago.ToList();
            return datos;
        }

        public bool guardarDatosBancarios(EmpleadoBank datos)
        {
            var resultado = false;
                try
            {
                const string sqlQuery = "DELETE DatosBancarios WHERE IdEmpleado = @p0";
                ctx.Database.ExecuteSqlCommand(sqlQuery, datos.IdEmpleado);
                var item = new DatosBancarios
                {
                    IdBanco = datos.IdBanco,
                    IdEmpleado = datos.IdEmpleado,
                    NoSigaF = datos.NoSiga,
                    NoSigaC = datos.NoSIga2,
                    CuentaBancaria = datos.cuenta,
                    NumeroTarjeta = datos.Tarjeta,
                    Clabe = datos.Clabe,
                    Status = true
                };
                ctx.DatosBancarios.Add(item);
                var r = ctx.SaveChanges();


                var contrato = ctx.Empleado_Contrato.Where(x => x.IdEmpleado == datos.IdEmpleado).OrderByDescending(x=>x.IdContrato).FirstOrDefault();

                contrato.FormaPago = datos.IdMetodoPago;
                ctx.SaveChanges();

                if (r > 0)
                    resultado = true;
                return resultado;
            }
            catch(Exception e)
            {
                return resultado;
            }
       
        }

      
    }

    public class CliSuc
    {
        public int IdCliente { get; set; }
        public string NomCliente { get; set; }
        public int IdSucursal { get; set; }
        public string NomSucursal { get; set; }
    }
    public class EmpleadoBank
    {
        public int IdBanco { get; set; }
        public int IdEmpleado { get; set; }
        public string Banco { get; set; }
        public int? NoSiga { get; set; }
        public int? NoSIga2 { get; set; }
        public string cuenta { get; set; }
        public string Tarjeta { get; set; }
        public string Clabe { get; set; }
        public string MetodoPago { get; set; }
        public int IdMetodoPago { get; set; }
    }
}
