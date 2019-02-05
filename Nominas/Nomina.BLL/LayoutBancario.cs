using System;
using RH.Entidades;
using RH.Entidades.GlobalModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;
using System.IO;

namespace Nomina.BLL
{
    public class LayoutBancario
    {
        //lista de empresas concenientes a el clinete
        public List<Empresa> ListaEmpresas(int idSucursal)
        {
            using (var context = new RHEntities())
            {

                var empresa = (from suc in context.Sucursal
                               join suc_emp in context.Sucursal_Empresa
                               on suc.IdSucursal equals suc_emp.IdSucursal
                               join emp in context.Empresa
                               on suc_emp.IdEmpresa equals emp.IdEmpresa
                               where suc.IdSucursal == idSucursal
                               select emp).ToList();
                return empresa;
            }
        }

        //listado de bancos
        public List<C_Banco_SAT> ListaBancos(int idPeriodo, int idTiponomina)
        {
            using (var context = new RHEntities())
            {
                List<Tuple<int, int>> listaNom = new List<Tuple<int, int>>();
                List<C_Banco_SAT> banco = null;

                if (idTiponomina != 11)
                {
                    var lista = (from n in context.NOM_Nomina
                                 where n.IdPeriodo == idPeriodo
                                 select n).ToList();
                    foreach (var n in lista)
                    {
                        listaNom.Add(Tuple.Create(n.IdEmpleado, n.IdPeriodo));
                    }
                }
                else
                {
                    var lista = (from n in context.NOM_Finiquito
                                 where n.IdPeriodo == idPeriodo
                                 select n).ToList();

                    foreach (var n in lista)
                    {
                        listaNom.Add(Tuple.Create(n.IdEmpleado, n.IdPeriodo));
                    }
                }
                
                    banco = (from nomina in listaNom
                            join datosB in context.DatosBancarios
                            on nomina.Item1 equals datosB.IdEmpleado
                            join banc in context.C_Banco_SAT
                            on datosB.IdBanco equals banc.IdBanco
                            where nomina.Item2 == idPeriodo &&
                            (
                            datosB.NoSigaC != 0 ||
                            datosB.NoSigaF != 0
                            )
                            select banc).Distinct().ToList();
               

                return banco;
            }
        }

        //Devuelve una lista con la informacion necesaria para plasmar las nominas en una tabla
        public List<LayoutBanco> ListaEmpleados(int IdTipoNomina, int idPeriodo, int idEmpresa, int idBanco)
        {
            //try
            //{

                using (var context = new RHEntities())
                {
                    List<Tuple<int, decimal, decimal, int, int?, int?, int>> listaNominas = new List<Tuple<int, decimal, decimal, int, int?, int?, int>>();
                    if (IdTipoNomina != 11)
                    { // en caso de nomina
                        var lista = (from n in context.NOM_Nomina
                                     where n.IdPeriodo == idPeriodo
                                     orderby n.IdEmpleado ascending
                                     select n).ToList();
                        foreach (var n in lista)
                        {
                            listaNominas.Add(Tuple.Create(n.IdEmpleado, n.TotalNomina, n.TotalComplemento, n.IdPeriodo, n.IdEmpresaFiscal, n.IdEmpresaComplemento, n.IdContrato));
                        }
                    }
                    else
                    {//caso de finiquito
                        var item = (from n in context.NOM_Finiquito
                                    where n.IdPeriodo == idPeriodo
                                    select n).FirstOrDefault();
                        listaNominas.Add(Tuple.Create(item.IdEmpleado, item.TOTAL_total, item.TotalComplemento, item.IdPeriodo, item.IdEmpresaFiscal, item.IdEmpresaComplemento, item.IdContrato));
                    }

                    //var nombrePeriodo = context.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).Select(x => x.Descripcion).FirstOrDefault();

                    var arrayContratos = listaNominas.Select(x => x.Item7).ToArray();
                    var arrayEmpleado = listaNominas.Select(x => x.Item1).ToArray();

                    var listaEmpresas = context.Empresa.ToList();

                    var listaEmpleados = (from e in context.Empleado
                                          where arrayEmpleado.Contains(e.IdEmpleado)
                                          select e).ToList();

                    var listaContratos = (from c in context.Empleado_Contrato
                                          where arrayContratos.Contains(c.IdContrato)
                                          select c).ToList();

                    var listaDatosBancarios = (from d in context.DatosBancarios
                                               where arrayEmpleado.Contains(d.IdEmpleado)
                                               select d).ToList();
                    var empresa = (from empresaex in listaEmpresas
                                   where empresaex.IdEmpresa == idEmpresa
                                   select empresaex).FirstOrDefault();

                    bool tipoEmpresa = empresa.RegistroPatronal != null;


                    List<LayoutBanco> empleados = new List<LayoutBanco>();

                        var dato = (from emp in listaEmpleados
                                    join datosB in listaDatosBancarios
                                    on emp.IdEmpleado equals datosB.IdEmpleado
                                    join nomina in listaNominas
                                    on emp.IdEmpleado equals nomina.Item1
                                    join contrato in listaContratos
                                    on emp.IdEmpleado equals contrato.IdEmpleado
                                    where //arrayEmpleado.Contains(nomina.Item1) &&
                                    //nomina.Item4 == idPeriodo &&
                                    (IdTipoNomina != 11 ? (((tipoEmpresa) ? datosB.NoSigaF : datosB.NoSigaC)) : datosB.NoSigaC) != 0 &&
                                    ((tipoEmpresa) ? nomina.Item2 : nomina.Item3) != 0 &&
                                    ((tipoEmpresa) ? nomina.Item5.Value : nomina.Item6.Value) == idEmpresa &&
                                    datosB.IdBanco == idBanco &&
                                    contrato.FormaPago != 1
                                    select new LayoutBanco
                                    {
                                        IdEmpleado = emp.IdEmpleado,
                                        //NombrePeriodo = nombrePeriodo,      
                                        NoSiga = (IdTipoNomina != 11 ? (((tipoEmpresa) ? datosB.NoSigaF : datosB.NoSigaC)) : datosB.NoSigaC),
                                        CuentaBancaria = datosB.CuentaBancaria,
                                        Nombres = emp.Nombres,
                                        Paterno = emp.APaterno,
                                        Materno = emp.AMaterno,
                                        Importe = (tipoEmpresa) ? nomina.Item2 : nomina.Item3,
                                        //Generado = false,
                                        NombreEmpresa = empresa.RazonSocial,
                                        NoEmisor = empresa.ClaveEmisora_Banco,
                                        //IdEmpresa = (tipoEmpresa) ? nomina.Item5.Value : nomina.Item6.Value,
                                        //IdBanco = datosB.IdBanco
                                    }).ToList();
                        if (dato != null)
                        {
                            empleados.AddRange(dato);
                        }
                 
                    return empleados;

                }
            //}
            //catch (Exception es)
            //{

            //    return null;
            //}


        }



        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>METODO DESCONTINUADO<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        //Devuelve una lista con la informacion necesaria para plasmar un finiquito             
        public List<LayoutBanco> ListaEmpleadosFiniquito(int idPeriodo, int idEmpresa)
                {
                    using (var context = new RHEntities())
                    {
                        var empPeriodo =
                            context.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == idPeriodo).FirstOrDefault();
                        var nombrePeriodo =
                            context.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo)
                                .Select(x => x.Descripcion)
                                .FirstOrDefault();


                        var dato = (from emp in context.Empleado
                                    join datosB in context.DatosBancarios
                                    on emp.IdEmpleado equals datosB.IdEmpleado
                                    join fin in context.NOM_Finiquito
                                    on emp.IdEmpleado equals fin.IdEmpleado
                                    join contrato in context.Empleado_Contrato
                                    on emp.IdEmpleado equals contrato.IdEmpleado
                                    join empresa in context.Empresa
                                    on contrato.IdEmpresaFiscal equals empresa.IdEmpresa
                                    where
                                    fin.IdPeriodo == idPeriodo && fin.IdEmpleado == empPeriodo.IdEmpleado && datosB.NoSigaF != 0 &&
                                    empresa.IdEmpresa == idEmpresa
                                    select new LayoutBanco
                                    {
                                        IdEmpleado = emp.IdEmpleado,
                                        NombrePeriodo = nombrePeriodo,
                                        NoSiga = datosB.NoSigaC,
                                        CuentaBancaria = datosB.CuentaBancaria,
                                        Nombres = emp.Nombres,
                                        Paterno = emp.APaterno,
                                        Materno = emp.AMaterno,
                                        Importe = fin.TOTAL_total,
                                        Generado = false,
                                        NombreEmpresa = empresa.RazonSocial,
                                        NoEmisor = empresa.ClaveEmisora_Banco,
                                        IdEmpresa = empresa.IdEmpresa,
                                        IsComplemento = false,
                                        IdBanco = datosB.IdBanco
                                    }).ToList();
                        return dato;
                    }
                }
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        //Devuelve una lista con la informacion de Nominas o finiquito necesaria para crear el layout:
        //    List<int> detalle = lista de ids de los empleados.
        public List<LayoutBanco> ListaEmpleadosSeleccionados(int IdTipoNomina, int idPeriodo, int idEmpresa, List<int> detalle)
        {
            try
            {

                using (var context = new RHEntities())
                {
                    List<Tuple<int, decimal, decimal>> listaNominas = new List<Tuple<int, decimal, decimal>>();

                    if (IdTipoNomina != 11)
                    {
                       var lista = (from n in context.NOM_Nomina
                                        where detalle.Contains(n.IdEmpleado)
                                        && n.IdPeriodo == idPeriodo
                                        select n).ToList();
                        //select Tuple.Create(n.IdEmpleado, n.TotalNomina, n.TotalComplemento)).ToList();
                        //select new Tuple<int,decimal,decimal>(n.IdEmpleado,n.TotalNomina,n.TotalComplemento)).ToList();
                        foreach (var n in lista)
                        {
                            listaNominas.Add(Tuple.Create(n.IdEmpleado, n.TotalNomina, n.TotalComplemento));
                        }
                    }
                    else
                    {
                        var item = (from n in context.NOM_Finiquito
                                        where detalle.Contains(n.IdEmpleado)
                                        && n.IdPeriodo == idPeriodo
                                        select n).FirstOrDefault();
                        //select new Tuple<int, decimal, decimal>(n.IdEmpleado, n.TOTAL_total, n.TotalComplemento)).ToList();
                        listaNominas.Add(Tuple.Create(item.IdEmpleado, item.TOTAL_total, item.TotalComplemento));
                    }               
                   
                    var arrayEmpleado = listaNominas.Select(x => x.Item1).ToArray();

                    var listaEmpleados = (from e in context.Empleado
                                          where arrayEmpleado.Contains(e.IdEmpleado)
                                          select e).ToList();


                    var listaDatosBancarios = (from d in context.DatosBancarios
                                               where arrayEmpleado.Contains(d.IdEmpleado) &&
                                               d.Status == true
                                               select d).ToList();

                    var empresa = (from empresaex in context.Empresa
                                   where empresaex.IdEmpresa == idEmpresa
                                   select empresaex).FirstOrDefault();

                    bool tipoEmpresa = empresa.RegistroPatronal != null;


                    List<LayoutBanco> empleados = new List<LayoutBanco>();

                    var dato = (from emp in listaEmpleados
                                join datosB in listaDatosBancarios
                                on emp.IdEmpleado equals datosB.IdEmpleado
                                join nomina in listaNominas
                                on emp.IdEmpleado equals nomina.Item1
                                orderby (IdTipoNomina != 11 ? (((tipoEmpresa) ? datosB.NoSigaF : datosB.NoSigaC)) : datosB.NoSigaC) ascending
                                select new LayoutBanco
                                {
                                    IdEmpleado = emp.IdEmpleado,
                                    NoSiga = (IdTipoNomina != 11 ?(((tipoEmpresa) ? datosB.NoSigaF : datosB.NoSigaC)):datosB.NoSigaC),
                                    Importe = (tipoEmpresa) ? nomina.Item2 : nomina.Item3,
                                    CuentaBancaria = datosB.CuentaBancaria,
                                    NoEmisor = empresa.ClaveEmisora_Banco
                                }).ToList();
                    if (dato != null)
                    {
                        empleados.AddRange(dato);
                    }

                    return empleados;

                }
            }
            catch (Exception es)
            {

                return null;
            }


        }



        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>METODO DESCONTINUADO<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        //Devuelve una lista con la informacion de un finiquito necesaria para crear el layout:
        //    List<int> detalle = id de el empleado.
        public List<LayoutBanco> ListaEmpleadosSeleccionadosFiniquito(int idPeriodo, int idEmpresa, List<int> detalle)
        {
            using (var context = new RHEntities())
            {
               
                var dato = (from emp in context.Empleado
                            join datosB in context.DatosBancarios
                            on emp.IdEmpleado equals datosB.IdEmpleado
                            join fin in context.NOM_Finiquito
                            on emp.IdEmpleado equals fin.IdEmpleado
                            join contrato in context.Empleado_Contrato
                            on emp.IdEmpleado equals contrato.IdEmpleado
                            join empresa in context.Empresa
                            on contrato.IdEmpresaFiscal equals empresa.IdEmpresa
                            where fin.IdPeriodo == idPeriodo
                            select new LayoutBanco
                            {
                                IdEmpleado = emp.IdEmpleado,
                                NoSiga = datosB.NoSigaC,
                                Importe = fin.TOTAL_total,
                                CuentaBancaria = datosB.CuentaBancaria,
                                NoEmisor = empresa.ClaveEmisora_Banco
                            }).ToList();
                return dato;
            }
        }
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        public string[] GenerarLayout(string pathTxt, encabezado Encabezado, int idUsuario, List<LayoutBanco> Detalle)
        {
            int count = Detalle == null ? 0 : Detalle.Count;

            List<int> emi = new List<int>();

            int j = 0;
            int c = 0;
            
            var importeTotal = Detalle.Sum(x => x.Importe);
            var emisoras = Detalle.Select(x => x.NoEmisor);

            decimal importetotal = Utils.TruncateDecimalesAbc(importeTotal, 2);

            string imptotal = importetotal.ToString().Replace(".", "");
            var newruta = ValidarFolderUsuario(idUsuario, pathTxt);

            var grupo = emisoras.GroupBy(u => u).Select(grp => grp.FirstOrDefault()).ToList();

            int countEmi = emisoras == null ? 0 : grupo.Count;
            string[] archivoTxt = new string[countEmi];
            foreach (var emisor in grupo)
            {
                
                List<string> lines = new List<string>();
               
                string encabezadoFinal = Encabezado.TipoRegistro + "" + Encabezado.ClaveServicio + "" + emisor + "" + Encabezado.Fecha + "" + addCeros((Encabezado.Consecutivo + c), 2) + "" + addCeros(count, 6) + "" + addCeros(Convert.ToInt32(imptotal), 15)+ "" + addCeros(0, 6) + "" + addCeros(0, 15) + "" + addCeros(0, 6) + "" + addCeros(0, 15) + "" + addCeros(0, 6) + "" + 0 + "    " + "" + addCeros(0, 8) + "" + addCeros(0, 10) + "" + addCeros(0, 55);
                archivoTxt[j] = pathTxt + idUsuario+ "\\" + "NI" + emisor + addCeros((Encabezado.Consecutivo + c), 2) + ".PAG";
                
                lines.Add(encabezadoFinal);
                foreach (var d in Detalle.Where(x => x.NoEmisor == emisor))
                {
                    string impDetalle = Extensores.ToCurrencyFormat(d.Importe);

                    string impDGeneral = impDetalle.ToString().Replace(".", "");
                           impDGeneral = impDGeneral.ToString().Replace(",", "");
                    var TipoCuenta = 1;
                    string detallado = Encabezado.registroDetalle + "" + Encabezado.Fecha + "" + addCeros(Int32.Parse(d.NoSiga.ToString()), 10) + "                                                                                " + "" + addCeros(Convert.ToInt32(impDGeneral), 15) + "" + addCeros(Encabezado.Banco, 3) + "" + addCeros(TipoCuenta, 2) + "" + addCeros(Convert.ToInt32(d.CuentaBancaria), 18) + "0 " + "" + addCeros(0, 8) + "                  ";
                    
                    lines.Add(detallado);
                    
                }
                File.WriteAllLines(archivoTxt[j], lines.ToArray());
                j++;
                c++;
            }
            return archivoTxt;            
        }

        private string ValidarFolderUsuario(int idUsuario, string pathFolder)
        {

            //var pathArchivos = @"C:\Sites\Nominas\Nomina.WEB\Files\DownloadRecibos";
            //var pathArchivos = HttpContext.Current.Server.MapPath("~/Files/DownloadRecibos/");
            var pathArchivos = pathFolder;

            if (!Directory.Exists(pathArchivos))
            {
                Directory.CreateDirectory(pathArchivos);
            }

            //Crear folder para el usuario con su id
            string folderUsuario = pathArchivos + "\\" + idUsuario + "\\";
            if (Directory.Exists(folderUsuario))
            {
                //Elimina el contenido del folder
                Array.ForEach(Directory.GetFiles(folderUsuario), File.Delete);

            }
            else
            {
                //Crea el folder con el id del usuario
                Directory.CreateDirectory(folderUsuario);
             }




                    return folderUsuario;

              }

        //agregar ceros especificamente para el layout
        public string addCeros(int n, int length)
        {
            var str = (n > 0 ? n : -n) + "";
            var zeros = "";
            for (var i = length - str.Length; i > 0; i--)
                zeros += "0";
            zeros += str;
            return n >= 0 ? zeros : "-" + zeros;
        }
        
    }
}


