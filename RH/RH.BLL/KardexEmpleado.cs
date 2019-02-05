using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Common.Enums;
using System.Net;
using System.Web.Script.Serialization;
using SYA.BLL;

namespace RH.BLL
{
    public class KardexEmpleado
    {
     //   RHEntities context = null;
        public KardexEmpleado()
        {
           // context = new RHEntities();
        }

        public List<MovimientosViewModel> MovimientosRecientes(DateTime fecha)
        {
            
            ControlUsuario cu = new ControlUsuario();
            int[] sucursales = cu.GetSucursalesUsuario(ControlAcceso.GetUsuarioEnSession());
            using (var context = new RHEntities())
            {
                return (from k in context.Kardex
                    join emp in context.Empleado on k.IdEmpleado equals emp.IdEmpleado
                    join con in context.Empleado_Contrato on emp.IdEmpleado equals con.IdEmpleado
                    join s in context.Sucursal on emp.IdSucursal equals s.IdSucursal
                    join c in context.Cliente on s.IdCliente equals c.IdCliente
                    join e in context.Empresa on con.IdEmpresaFiscal equals e.IdEmpresa
                    where k.Fecha >= fecha && k.Tipo != 5 && k.Tipo != 6 && k.Tipo != 4 && sucursales.Contains(s.IdSucursal   )
                    select new MovimientosViewModel
                    {
                        IdEmpleado = emp.IdEmpleado,
                        Nombres = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        Fecha = k.Fecha,
                        Cliente = c.Nombre,
                        Empresa = e.RazonSocial,
                        IdTipoMovimiento = k.Tipo,

                    }
                ).ToList();
            }
        }

        public List<DatosEmpleado> BuscarMovimientos(DateTime fechaini, DateTime fechafin, int tipo, int Empresa)
        {
            using (var context = new RHEntities())
            {
                return (from k in context.Kardex
                    join emp in context.Empleado on k.IdEmpleado equals emp.IdEmpleado
                    join con in context.Empleado_Contrato on emp.IdEmpleado equals con.IdEmpleado
                    join s in context.Sucursal on emp.IdSucursal equals s.IdSucursal
                    join c in context.Cliente on s.IdCliente equals c.IdCliente
                    where
                    k.Tipo == tipo && (k.Fecha >= fechaini && k.Fecha <= fechafin) && con.IdEmpresaFiscal == Empresa &&
                    con.Status == true
                    select new DatosEmpleado
                    {
                        IdEmpleado = emp.IdEmpleado,
                        NSS = emp.NSS,
                        CURP = emp.CURP,
                        Nombres = emp.Nombres,
                        Paterno = emp.APaterno,
                        Materno = emp.AMaterno,
                        FechaMovimiento = k.Fecha,
                        Cliente = c.Nombre
                    }
                ).ToList();
            }
        }

        public List<Kardex> GetKardexEmpleado(int idEmpleado)
        {
            using (var context = new RHEntities())
            {
                return (from s in context.Kardex
                        where s.IdEmpleado == idEmpleado
                        orderby s.IdKardex descending
                        select s)
                    .ToList();
            }
        }

        public void Alta(int idEmpleado, int idUser)
        {
            using (var context = new RHEntities())
            {
                Kardex kardex = new Kardex();
                kardex.IdEmpleado = idEmpleado;
                kardex.IdUsuario = idUser;
                kardex.Fecha = DateTime.Now;
                kardex.Tipo = (int) TipoKardex.Alta;
                context.Kardex.Add(kardex);
                context.SaveChanges();
            }
        }
        public void AltaByArray(int [] arrayIdEmpleados, int idUser)
        {
            List<Kardex> kardeList = new List<Kardex>(); 
            using (var context = new RHEntities())
            {
                foreach (var idEmpleado in arrayIdEmpleados)
                {
                    Kardex kardex = new Kardex();
                    kardex.IdEmpleado = idEmpleado;
                    kardex.IdUsuario = idUser;
                    kardex.Fecha = DateTime.Now;
                    kardex.Tipo = (int)TipoKardex.Alta;

                    kardeList.Add(kardex);
                }
                context.Kardex.AddRange(kardeList);
                context.SaveChanges();
            }
        }

        public void Baja(int idempleado, int idUser)
        {
            using (var context = new RHEntities())
            {
                Kardex kardex = new Kardex();
                kardex.IdEmpleado = idempleado;
                kardex.IdUsuario = idUser;
                kardex.Fecha = DateTime.Now;
                kardex.Tipo = (int) TipoKardex.Baja;
                context.Kardex.Add(kardex);
                context.SaveChanges();
            }
        }

        public void Recontratacion(int idempleado, int idUser)
        {
            using (var context = new RHEntities())
            {
                Kardex kardex = new Kardex();
                kardex.IdEmpleado = idempleado;
                kardex.IdUsuario = idUser;
                kardex.Fecha = DateTime.Now;
                kardex.Tipo = (int) TipoKardex.Recontratacion;
                context.Kardex.Add(kardex);
                context.SaveChanges();
            }
        }
        public void CambioSalario(int idempleado, int idUser)
        {
            using (var context = new RHEntities())
            {
                Kardex kardex = new Kardex();
                kardex.IdEmpleado = idempleado;
                kardex.IdUsuario = idUser;
                kardex.Fecha = DateTime.Now;
                kardex.Tipo = (int)TipoKardex.Recontratacion;
                context.Kardex.Add(kardex);
                context.SaveChanges();
            }
        }

        public bool Puesto(int idEmpleado, int? puesto, int? puestoNuevo, int idUser)
        {

            bool result = false;
            try
            {
                using (var context = new RHEntities())
                {
                    Kardex kardex = new Kardex();
                    kardex.IdEmpleado = idEmpleado;
                    kardex.Fecha = DateTime.Now;
                    kardex.IdUsuario = idUser;
                    var PuestoAnterior = context.Puesto.FirstOrDefault(x => x.IdPuesto == puesto);
                    var PuestoNuevo = context.Puesto.FirstOrDefault(x => x.IdPuesto == puestoNuevo);
                    kardex.ValorAnterior = PuestoAnterior.Descripcion;
                    kardex.ValorNuevo = PuestoNuevo.Descripcion;
                    kardex.Tipo = (int) TipoKardex.Puesto;
                    context.Kardex.Add(kardex);
                    context.SaveChanges();
                    result = true;
                }

            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public void Salarios(int idEmpleado, decimal? sr, decimal? srNuevo, int tipoSalario, int idUser, string fechai="")
        {
            using (var context = new RHEntities())
            {
                Kardex kardex = new Kardex();
                kardex.IdEmpleado = idEmpleado;
                kardex.Fecha = DateTime.Now;
                kardex.Tipo = tipoSalario;
                kardex.IdUsuario = idUser;
                kardex.ValorAnterior = Convert.ToString(sr);
                kardex.ValorNuevo = Convert.ToString(srNuevo)+"-"+fechai;
                context.Kardex.Add(kardex);
                context.SaveChanges();
            }
        }

        public void RegistroPatronal(int idEmpleado, int? RP, int? RPNuevo, int TipoKardex, int idUser)
        {
            using (var context = new RHEntities())
            {
                Kardex kardex = new Kardex();
                kardex.IdEmpleado = idEmpleado;
                kardex.Fecha = DateTime.Now;
                kardex.Tipo = TipoKardex;
                kardex.IdUsuario = idUser;
                if (RP != null)
                {
                    var RPAnterior = context.Empresa.FirstOrDefault(x => x.IdEmpresa == RP);
                    kardex.ValorAnterior = RPAnterior.RazonSocial;
                }

                if (RPNuevo != null)
                {
                    var RPN = context.Empresa.FirstOrDefault(x => x.IdEmpresa == RPNuevo);
                    kardex.ValorNuevo = RPN.RazonSocial;
                }
                context.Kardex.Add(kardex);
                context.SaveChanges();
            }


        }

        //public void EnviarASlack(Empleado empleado)
        //{
        //    SlackMessage message = new SlackMessage();
        //    message.text = "Baja de Empleado" + empleado.Nombres;
        //    string url = "https://hooks.slack.com/services/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";

        //    var json = new JavaScriptSerializer().Serialize(message);


        //    using(var client = new WebClient())
        //    {
        //        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        //        client.UploadString(url, "POST", json);
        //    }
        //}
                
    }

    public class KardexDatos
    {
        public int IdKardex { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public string ValorAntiguo { get; set; }
        public string ValorNuevo { get; set; }
    }

    public class MovimientosViewModel
    {
        public int IdEmpleado { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string TipoMovimiento { get; set; }
        public int IdTipoMovimiento { get; set; }
    }

    //public class SlackMessage
    //{
    //    public string text { get; set; }
    //    public string username { get; set; }
    //    public string channel { get; set; }
    //}
}
