using RH.Entidades;
using RH.Entidades.GlobalModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;
using System.Text.RegularExpressions;

namespace Nomina.BLL
{

    public class _Layout
    {
        //RHEntities ctx = null;

        public _Layout()
        {
            // ctx = new RHEntities();
        }
        public List<LayoutBanco> listaEmpleados(int idPeriodo, int idEmpresa)
        {
            try

            {

                using (var context = new RHEntities())
                {

                    var empleadosDelPeriodo = context.NOM_Empleado_PeriodoPago.Where(x => x.IdPeriodoPago == idPeriodo).ToList();

                    var nombrePeriodo = context.NOM_PeriodosPago.Where(x => x.IdPeriodoPago == idPeriodo).Select(x => x.Descripcion).FirstOrDefault();

                    var listaNominas = (from n in context.NOM_Nomina
                                        where n.IdPeriodo == idPeriodo
                                        select n).ToList();

                    var arrayNomina = listaNominas.Select(x => x.IdNomina).ToArray();
                    var arrayEmpleado = listaNominas.Select(x => x.IdEmpleado).ToArray();
                    var arrayContratos = listaNominas.Select(x => x.IdContrato).ToArray();

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


                    List<LayoutBanco> empleados = new List<LayoutBanco>();

                    foreach (var idemp in empleadosDelPeriodo)
                    {

                        var itemContrato = listaContratos.FirstOrDefault(x => x.IdEmpleado == idemp.IdEmpleado);

                        var empresaC = itemContrato;
                        var empresaS = itemContrato;
                        var empresaA = itemContrato;

                        var dato = (from emp in listaEmpleados
                                    join datosB in listaDatosBancarios
                            on emp.IdEmpleado equals datosB.IdEmpleado
                                    join nomina in listaNominas
                                    on emp.IdEmpleado equals nomina.IdEmpleado
                                    join contrato in listaContratos
                                    on emp.IdEmpleado equals contrato.IdEmpleado
                                    join empresa in listaEmpresas
                                    on contrato.IdEmpresaFiscal equals empresa.IdEmpresa
                                    where
                                    nomina.IdPeriodo == idPeriodo && nomina.IdEmpleado == idemp.IdEmpleado &&
                                    datosB.NoSigaF != 0 && empresa.IdEmpresa == idEmpresa && contrato.FormaPago != 1
                                    /*&& empresa.RazonSocial != null*/
                                    select new LayoutBanco
                                    {
                                        IdEmpleado = emp.IdEmpleado,
                                        NombrePeriodo = nombrePeriodo,
                                        NoSiga1 = datosB.NoSigaF,
                                        NoSiga2 = datosB.NoSigaC,
                                        CuentaBancaria = datosB.CuentaBancaria,
                                        Nombres = emp.Nombres,
                                        Paterno = emp.APaterno,
                                        Materno = emp.AMaterno,
                                        Importe = nomina.TotalNomina,
                                        Generado = false,
                                        NombreEmpresa = empresa.RazonSocial,
                                        NoEmisor = empresa.ClaveEmisora_Banco,
                                        IdEmpresa = empresa.IdEmpresa,
                                        IsComplemento = false,
                                        IdBanco = datosB.IdBanco,
                                    }).FirstOrDefault();
                        if (dato != null)
                        {
                            empleados.Add(dato);
                        }

                        // var empresaC = context.Empleado_Contrato.Where(x => x.IdEmpleado == idemp.IdEmpleado && x.Status == true).FirstOrDefault();

                        if (empresaC != null)
                        {
                            var dato2 = (from emp in listaEmpleados
                                         join datosB in listaDatosBancarios
                                on emp.IdEmpleado equals datosB.IdEmpleado
                                         join nomina in listaNominas
                                         on emp.IdEmpleado equals nomina.IdEmpleado
                                         join contrato in listaContratos
                                         on emp.IdEmpleado equals contrato.IdEmpleado
                                         join empresa in listaEmpresas
                                         on contrato.IdEmpresaComplemento equals empresa.IdEmpresa
                                         where
                                         nomina.IdPeriodo == idPeriodo && nomina.IdEmpleado == idemp.IdEmpleado &&
                                         datosB.NoSigaC != 0 && empresa.IdEmpresa == idEmpresa && contrato.FormaPago != 1
                                         /*&& empresa.RazonSocial != null*/
                                         select new LayoutBanco
                                         {
                                             IdEmpleado = emp.IdEmpleado,
                                             NombrePeriodo = nombrePeriodo,
                                             NoSiga1 = datosB.NoSigaF,
                                             NoSiga2 = datosB.NoSigaC,
                                             CuentaBancaria = datosB.CuentaBancaria,
                                             Nombres = emp.Nombres,
                                             Paterno = emp.APaterno,
                                             Materno = emp.AMaterno,
                                             Importe = nomina.TotalComplemento,
                                             Generado = false,
                                             NombreEmpresa = empresa.RazonSocial,
                                             NoEmisor = empresa.ClaveEmisora_Banco,
                                             IdEmpresa = empresa.IdEmpresa,
                                             IsComplemento = true,
                                             IdBanco = datosB.IdBanco,
                                         }).FirstOrDefault();
                            if (dato2 != null)
                            {
                                empleados.Add(dato2);
                            }
                        }

                        // var empresaS = context.Empleado_Contrato.Where(x => x.IdEmpleado == idemp.IdEmpleado && x.Status == true).FirstOrDefault();

                        if (empresaS != null)
                        {
                            var dato2 = (from emp in listaEmpleados
                                         join datosB in listaDatosBancarios
                                on emp.IdEmpleado equals datosB.IdEmpleado
                                         join nomina in listaNominas
                                         on emp.IdEmpleado equals nomina.IdEmpleado
                                         join contrato in listaContratos
                                         on emp.IdEmpleado equals contrato.IdEmpleado
                                         join empresa in listaEmpresas
                                         on contrato.IdEmpresaSindicato equals empresa.IdEmpresa
                                         where
                                         nomina.IdPeriodo == idPeriodo && nomina.IdEmpleado == idemp.IdEmpleado &&
                                         datosB.NoSigaC != 0 && empresa.IdEmpresa == idEmpresa && contrato.FormaPago != 1
                                         /*&& empresa.RazonSocial != null*/
                                         select new LayoutBanco
                                         {
                                             IdEmpleado = emp.IdEmpleado,
                                             NombrePeriodo = nombrePeriodo,
                                             NoSiga1 = datosB.NoSigaF,
                                             NoSiga2 = datosB.NoSigaC,
                                             CuentaBancaria = datosB.CuentaBancaria,
                                             Nombres = emp.Nombres,
                                             Paterno = emp.APaterno,
                                             Materno = emp.AMaterno,
                                             Importe = nomina.TotalNomina,
                                             Generado = false,
                                             NombreEmpresa = empresa.RazonSocial,
                                             NoEmisor = empresa.ClaveEmisora_Banco,
                                             IdEmpresa = empresa.IdEmpresa,
                                             IsComplemento = true,
                                             IdBanco = datosB.IdBanco,
                                         }).FirstOrDefault();
                            if (dato2 != null)
                            {
                                empleados.Add(dato2);
                            }
                        }

                        //  var empresaA = context.Empleado_Contrato.Where(x => x.IdEmpleado == idemp.IdEmpleado && x.Status == true).FirstOrDefault();

                        if (empresaA != null)
                        {
                            var dato2 = (from emp in listaEmpleados
                                         join datosB in listaDatosBancarios
                                on emp.IdEmpleado equals datosB.IdEmpleado
                                         join nomina in listaNominas
                                         on emp.IdEmpleado equals nomina.IdEmpleado
                                         join contrato in listaContratos
                                         on emp.IdEmpleado equals contrato.IdEmpleado
                                         join empresa in listaEmpresas
                                         on contrato.IdEmpresaAsimilado equals empresa.IdEmpresa
                                         where
                                         nomina.IdPeriodo == idPeriodo && nomina.IdEmpleado == idemp.IdEmpleado &&
                                         datosB.NoSigaF != 0 && empresa.IdEmpresa == idEmpresa && contrato.FormaPago != 1
                                         /*&& empresa.RazonSocial != null*/
                                         select new LayoutBanco
                                         {
                                             IdEmpleado = emp.IdEmpleado,
                                             NombrePeriodo = nombrePeriodo,
                                             NoSiga1 = datosB.NoSigaF,
                                             NoSiga2 = datosB.NoSigaC,
                                             CuentaBancaria = datosB.CuentaBancaria,
                                             Nombres = emp.Nombres,
                                             Paterno = emp.APaterno,
                                             Materno = emp.AMaterno,
                                             Importe = nomina.TotalNomina,
                                             Generado = false,
                                             NombreEmpresa = empresa.RazonSocial,
                                             NoEmisor = empresa.ClaveEmisora_Banco,
                                             IdEmpresa = empresa.IdEmpresa,
                                             IsComplemento = false,
                                             IdBanco = datosB.IdBanco,
                                         }).FirstOrDefault();
                            if (dato2 != null)
                            {
                                empleados.Add(dato2);
                            }
                        }


                    }

                    return empleados;

                }
            }
            catch (Exception es)
            {

                return null;
            }


        }

        public LayoutBanco listaEmpleadosFiniquito(int idPeriodo, int idEmpresa)
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
                            /*&& empresa.RazonSocial != null*/
                            select new LayoutBanco
                            {
                                IdEmpleado = emp.IdEmpleado,
                                NombrePeriodo = nombrePeriodo,
                                NoSiga1 = datosB.NoSigaF,
                                NoSiga2 = datosB.NoSigaC,
                                CuentaBancaria = datosB.CuentaBancaria,
                                Nombres = emp.Nombres,
                                Paterno = emp.APaterno,
                                Materno = emp.AMaterno,
                                Importe = fin.TOTAL_total,
                                Generado = false,
                                NombreEmpresa = empresa.RazonSocial,
                                NoEmisor = empresa.ClaveEmisora_Banco,
                                IdEmpresa = empresa.IdEmpresa,
                                IsComplemento = false
                            }).FirstOrDefault();

                return dato;
            }
        }
        public List<Empresa> empresas(int idSucursal)
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



        public string[] GenerarLayout(string pathTxt, encabezado Encabezado, int idUsuario, List<detallado> Detalle, List<emisoras> emisoras)
        {
            int count = Detalle == null ? 0 : Detalle.Count;

            List<int> emi = new List<int>();

            int j = 0;
            int c = 0;


            decimal importetotal = Utils.TruncateDecimalesAbc(Encabezado.ImporteTotal, 2);

            //string imptotal = Regex.Replace(importetotal.ToString(), @".", "");
            string imptotal = importetotal.ToString().Replace(".", "");
            ////validar que el directorio para el txt este creado, sino se crea
            var newruta = ValidarFolderUsuario(idUsuario, pathTxt);

            var grupo = emisoras.GroupBy(u => u.NoEmisor).Select(grp => grp.FirstOrDefault()).ToList();
            int countEmi = emisoras == null ? 0 : grupo.Count;
            string[] archivoTxt = new string[countEmi];
            foreach (var emisor in grupo)
            {
                //string[] lines = new string[count + 1];
                List<string> lines = new List<string>();
                //int i = 1;

                string encabezadoFinal = Encabezado.TipoRegistro + "" + Encabezado.ClaveServicio + "" + emisor.NoEmisor + "" + Encabezado.Fecha + "" + addCeros((Encabezado.Consecutivo + c), 2) + "" + addCeros(Encabezado.TotalEmpleados, 6) + "" + addCeros(Convert.ToInt32(imptotal), 15)
                    + "" + addCeros(0, 6) + "" + addCeros(0, 15) + "" + addCeros(0, 6) + "" + addCeros(0, 15) + "" + addCeros(0, 6) + "" + 0 + "    " + "" + addCeros(0, 8) + "" + addCeros(0, 10) + "" + addCeros(0, 55);
                archivoTxt[j] = pathTxt + "NI" + emisor.NoEmisor + addCeros((Encabezado.Consecutivo + c), 2) + ".PAG";
                //lines[0] = encabezadoFinal;
                lines.Add(encabezadoFinal);
                foreach (var d in Detalle.Where(x => x.NoEmisor == emisor.NoEmisor))
                {
                    string impDetalle = Extensores.ToCurrencyFormat(d.Importe);

                    string impDGeneral = impDetalle.ToString().Replace(".", "");
                    impDGeneral = impDGeneral.ToString().Replace(",", "");
                    string detallado = d.TipoRegistro + "" + d.fecha + "" + addCeros(d.NoSiga1, 10) + "                                                                                " + "" + addCeros(Convert.ToInt32(impDGeneral), 15) + "" + addCeros(d.Banco, 3)
                        + "" + addCeros(d.TipoCuenta, 2) + "" + addCeros(d.CuentaBancaria, 18) + "0 " + "" + addCeros(0, 8) + "                  ";
                    //lines[i] = detallado;
                    lines.Add(detallado);
                    //i++;
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
