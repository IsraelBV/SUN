using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Utils;
using RH.Entidades;
using MoreLinq;
using ClosedXML.Excel;
using SYA.BLL;

namespace RH.BLL
{
    public class LayoutImss
    {
        //IDSE

        public static string IdseGenerarLayout(int idUsuario, int tipoMovimiento, int empresa, DateTime FechaI, DateTime FechaF, string path, string pathDescarga)
        {
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<Cliente> listaClientes = new List<Cliente>();
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<Empleado_Contrato> listaContratos = new List<Empleado_Contrato>();
            List<Kardex> listaKardex = new List<Kardex>();
            var nombreArchivo = "REINGRESO.txt";
            Empresa itemEmpresa;
            ControlUsuario cu = new ControlUsuario();
            var sucursales = cu.GetSucursalesUsuario(ControlAcceso.GetUsuarioEnSession());

            //var FechaI = new DateTime(fecI.Year, fecI.Month, fecI.Day, 05, 00, 0);
            //var FechaF = new DateTime(fecF.Year, fecF.Month, fecF.Day, 23, 50, 0);

            using (var context = new RHEntities())
            {
                itemEmpresa = context.Empresa.FirstOrDefault(x => x.IdEmpresa == empresa);

                //ALTA-REINGRESO
                switch (tipoMovimiento)
                {
                    case 1:
                        nombreArchivo = "REINGRESO " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";
                        //Buscamos los contratos con fecha de Alta imss  en el rango de fecha
                        listaContratos = (from c in context.Empleado_Contrato
                                          where c.FechaIMSS >= FechaI && c.FechaIMSS <= FechaF
                                               // && c.Status == true && c.BajaIMSS == null //querian que laas altas o bajas aparezcan en ambos reportes
                                                // && c.IdEmpresaFiscal != null
                                                && c.IdEmpresaFiscal == empresa
                                          select c).ToList();
                        break;
                    case 2:
                        nombreArchivo = " BAJA " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";
                        //Buscamos los contratos con fecha de Alta imss  en el rango de fecha
                        listaContratos = (from c in context.Empleado_Contrato
                                          where c.BajaIMSS >= FechaI && c.BajaIMSS <= FechaF
                                                && c.Status == false && c.BajaIMSS != null
                                                // && c.IdEmpresaFiscal != null
                                                && c.IdEmpresaFiscal == empresa
                                          select c).ToList();
                        break;
                    case 3://Cambio Salsario
                        nombreArchivo = "MODIFICACION " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";

                        //Buscamos en Kardex los cambios de salario
                        listaKardex = (from k in context.Kardex
                                       where k.Fecha >= FechaI && k.Fecha <= FechaF
                                             && k.Tipo == (int)TipoKardex.SDI
                                       select k).ToList();

                        //Obtener los idEMpleados
                        var arrayIdEmp = listaKardex.Select(x => x.IdEmpleado).ToList();

                        //Buscamos los cumple imss
                        //var listaCumpleImss = (from ci in context.CumpleIMSS
                        //                      where ci.FechaIMSS >= FechaI && ci.FechaIMSS <= FechaF
                        //                      select ci).ToList();


                        //Buscamos los contratos activos de los empleados
                        listaContratos = (from c in context.Empleado_Contrato
                                          where arrayIdEmp.Contains(c.IdEmpleado)
                                                && c.Status == true
                                          select c).ToList();


                        break;
                }

                //Obtenemos el array de Id empleados
                listaContratos = listaContratos.Where(x=> sucursales.Contains(x.IdSucursal)).ToList();
                var arrayIdEmpleados = listaContratos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

                //listaEmpresas = context.Empresa.ToList();

            }

            string[] lineasArchivo = new string[listaContratos.Count];
            int linea = 0;
            foreach (var itemContrato in listaContratos)
            {
                //var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == itemContrato.IdEmpresaFiscal.Value);
                var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == itemContrato.IdEmpleado);

                string strReg = "";
                switch (tipoMovimiento)
                {
                    case 1:
                        strReg = IdseRegistroAltaReingreso(itemEmpresa, itemEmpleado, itemContrato);
                        break;
                    case 2:
                        strReg = IdseRegistroBaja(itemEmpresa, itemEmpleado, itemContrato);
                        break;
                    case 3:
                        var itemKardex = listaKardex.FirstOrDefault(x => x.IdEmpleado == itemContrato.IdEmpleado);

                        strReg = IdseRegistroCambioSalario(itemEmpresa, itemEmpleado, itemContrato, itemKardex);
                        break;
                }

                lineasArchivo[linea] = strReg;
                linea++;
            }





            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, path);
            var archivoIdse = pathUsuario + nombreArchivo;
            File.WriteAllLines(archivoIdse, lineasArchivo, Encoding.Default);

            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

        }

        private static string IdseRegistroBaja(Empresa emp, Empleado e, Empleado_Contrato con)
        {
            var registro = "                                                                                                                                                                                     ";
            //logitud de la cadena 169

            registro = registro.Insert(0, emp.RegistroPatronal);
            registro = registro.Insert(11, e.NSS);
            registro = registro.Insert(22, e.APaterno);
            registro = registro.Insert(49, e.AMaterno);
            registro = registro.Insert(76, e.Nombres);
            registro = registro.Insert(103, "000000000000000");
            registro = registro.Insert(118, con.BajaIMSS.ToStringOrEmptyDate<DateTime>("ddMMyyyy"));
            registro = registro.Insert(131, "02");//02 es baja
            registro = registro.Insert(133, emp.Guia);
            registro = registro.Insert(138, e.IdEmpleado.ToString());
            registro = registro.Insert(148, "2 ");//Causa Baja segun lo capturado por RH
            registro = registro.Insert(149, "                  ");
            registro = registro.Insert(167, "9");
            registro = registro.TrimEnd();
            return registro;
        }
        private static string IdseRegistroAltaReingreso(Empresa emp, Empleado e, Empleado_Contrato con)
        {
            var registro = "                                                                                                                                                                                     ";
            //logitud de la cadena 169

            registro = registro.Insert(0, emp.RegistroPatronal);
            registro = registro.Insert(11, e.NSS??"NoTieneNSS");
            registro = registro.Insert(22, e.APaterno);
            registro = registro.Insert(49, e.AMaterno);
            registro = registro.Insert(76, e.Nombres);
            registro = registro.Insert(103, con.SBC.ToIdseFormat());
            registro = registro.Insert(109, "      ");
            registro = registro.Insert(115, "1");//El que le asigna RH tipo de contrato
            registro = registro.Insert(116, "0");//El que le asigna RH
            registro = registro.Insert(117, "0");//Semana o jornada reducida
            registro = registro.Insert(118, con.FechaIMSS.ToStringOrEmptyDate<DateTime>("ddMMyyyy"));// cambiar al formato DDMMAAAA
            registro = registro.Insert(126, con.UMF ?? "");//si no tiene ?
            registro = registro.Insert(129, "  ");

            registro = registro.Insert(131, "08");
            registro = registro.Insert(133, emp.Guia);//Guia es por Lugar MTY 34400 CUN 07400

            registro = registro.Insert(138, e.IdEmpleado.ToString());
            registro = registro.Insert(148, " ");
            registro = registro.Insert(149, e.CURP);
            registro = registro.Insert(167, "9");
            registro = registro.TrimEnd();
            return registro;
        }
        private static string IdseRegistroCambioSalario(Empresa emp, Empleado e, Empleado_Contrato con, Kardex kardex)
        {
            var registro = "                                                                                                                                                                                     ";
            //logitud de la cadena 169

            registro = registro.Insert(0, emp.RegistroPatronal);
            registro = registro.Insert(11, e.NSS);
            registro = registro.Insert(22, e.APaterno.ToUpper());
            registro = registro.Insert(49, e.AMaterno.ToUpper());
            registro = registro.Insert(76, e.Nombres.ToUpper());
            registro = registro.Insert(103, con.SDI.ToIdseFormat());
            registro = registro.Insert(109, "      ");
            registro = registro.Insert(115, "7");
            registro = registro.Insert(116, "0");//tipo salario segun rh
            registro = registro.Insert(117, "0");//semna jornada - cristian dijo que por default sera 0
            registro = registro.Insert(118, kardex.Fecha.ToString("ddMMyyyy"));//Cambiar a la fecha de cambios de salario
            registro = registro.Insert(126, "     ");
            registro = registro.Insert(131, "07");//cambio de salario
            registro = registro.Insert(133, emp.Guia);
            registro = registro.Insert(138, e.IdEmpleado.ToString());
            registro = registro.Insert(148, " ");
            registro = registro.Insert(149, e.CURP);
            registro = registro.Insert(167, "9");
            registro = registro.TrimEnd();
            return registro;
        }

        //SUA

        public static string SuaGenerarLayout(int idUsuario, int tipoMovimiento, int empresa, DateTime FechaI, DateTime FechaF, string path, string pathDescarga)
        {
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<Cliente> listaClientes = new List<Cliente>();
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<Empleado_Contrato> listaContratos = new List<Empleado_Contrato>();
            List<Kardex> listaKardex = new List<Kardex>();
            var nombreArchivo = "Aseg.txt";
            Empresa itemEmpresa;
            var itemAbrev = new List<Sucursal_Empresa>();
            ControlUsuario cu = new ControlUsuario();
            var sucursales = cu.GetSucursalesUsuario(ControlAcceso.GetUsuarioEnSession());

            //var FechaI = new DateTime(fecI.Year, fecI.Month, fecI.Day, 05, 00, 0);
            //var FechaF = new DateTime(fecF.Year, fecF.Month, fecF.Day, 23, 50, 0);


            using (var context = new RHEntities())
            {
                itemEmpresa = context.Empresa.FirstOrDefault(x => x.IdEmpresa == empresa);
                itemAbrev = context.Sucursal_Empresa.Where(x => x.IdEmpresa == itemEmpresa.IdEmpresa && x.IdEsquema == 1).ToList();
                //ALTA-REINGRESO
                switch (tipoMovimiento)
                {
                    case 1:
                        nombreArchivo = "Aseg " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";
                        //Buscamos los contratos con fecha de Alta imss  en el rango de fecha
                        listaContratos = (from c in context.Empleado_Contrato
                                          where c.FechaIMSS >= FechaI && c.FechaIMSS <= FechaF
                                                && c.Status == true && c.BajaIMSS == null
                                                // && c.IdEmpresaFiscal != null
                                                && c.IdEmpresaFiscal == empresa && c.IsReingreso == false
                                          select c).ToList();
                        break;
                    case 2://Baja - Reingreso
                        nombreArchivo = "Movt " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";
                        //Buscamos los contratos con fecha de Alta imss  en el rango de fecha
                        listaContratos = (from c in context.Empleado_Contrato
                                          where c.BajaIMSS >= FechaI && c.BajaIMSS <= FechaF
                                                && c.Status == false && c.BajaIMSS != null
                                                // && c.IdEmpresaFiscal != null
                                                && c.IdEmpresaFiscal == empresa
                                          select c).ToList();
                        break;

                }

                //Obtenemos el array de Id empleados
                listaContratos = listaContratos.Where(x => sucursales.Contains(x.IdSucursal)).ToList();
                var arrayIdEmpleados = listaContratos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

                //listaEmpresas = context.Empresa.ToList();

            }

            //string[] lineasArchivo = new string[listaContratos.Count];
            //int linea = 0;
            //foreach (var itemContrato in listaContratos)
            //{
            //var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == itemContrato.IdEmpresaFiscal.Value);
            //var itemEmpleado = listaEmpleados.FirstOrDefault(x => x.IdEmpleado == itemContrato.IdEmpleado);
            //string clave = itemAbrev.Where(x => x.IdEmpresa == itemContrato.IdEmpresaFiscal && x.IdSucursal == itemContrato.IdSucursal).Select(x => x.Clave_Imss).FirstOrDefault();
            //string strReg = "";

            List<string> strReg = new List<string>();
            switch (tipoMovimiento)
            {
                case 1:
                    strReg = SuaAseg(itemEmpresa, listaEmpleados, listaContratos, itemAbrev);
                    var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, path);
                    var archivoIdse = pathUsuario + nombreArchivo;
                    File.WriteAllLines(archivoIdse, strReg);
                    break;
                case 2:
                    strReg = SuaMovt(itemEmpresa, listaEmpleados, listaContratos, itemAbrev);
                    var pathUsuario2 = Utils.ValidarFolderUsuario(idUsuario, path);
                    var archivoSuaMovBaja = pathUsuario2 + nombreArchivo;
                    File.WriteAllLines(archivoSuaMovBaja, strReg);
                    break;
            }

            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;

        }

        private static List<string> SuaAseg(Empresa emp, List<Empleado> empleado, List<Empleado_Contrato> contrato, List<Sucursal_Empresa> itemAbrev)
        {
            List<string> lineasArchivo = new List<string>();
            int linea = 0;
            foreach (var con in contrato)
            {
                var e = empleado.FirstOrDefault(x => x.IdEmpleado == con.IdEmpleado);
                string clave = itemAbrev.Where(x => x.IdEmpresa == con.IdEmpresaFiscal && x.IdSucursal == con.IdSucursal).Select(x => x.Clave_Imss).FirstOrDefault();
                string[] fechaImss = con.FechaIMSS.Value.ToString("dd-MM-yyyy").Split('-');

                string fechaImssFinal = fechaImss[0] + fechaImss[1] + fechaImss[2];
                string nombreCompleto = e.APaterno.Replace("Ñ", "#") + "$" + e.AMaterno.Replace("Ñ", "#") + "$" + e.Nombres.Replace("Ñ", "#");

                var registro = "                                                                                                                                                                   ";
                //longitud de la cadena 164
                registro = registro.Insert(0, emp.RegistroPatronal);
                registro = registro.Insert(11, e.NSS);
                registro = registro.Insert(22, e.RFC);
                registro = registro.Insert(35, e.CURP);
                registro = registro.Insert(53, nombreCompleto);
                registro = registro.Insert(103, "10");
                registro = registro.Insert(105, fechaImssFinal);
                registro = registro.Insert(113, con.SDI.ToSUAFormat());
                registro = registro.Insert(120, clave == null ? "SINCLAVE" : clave);
                registro = registro.Insert(147, "00000000");
                registro = registro.Insert(156, "00000000");
                registro = registro.TrimEnd();
                lineasArchivo.Add(registro);

                //    linea++;
            }

            return lineasArchivo;
        }

        private static List<string> SuaMovt(Empresa emp, List<Empleado> empleado, List<Empleado_Contrato> contrato, List<Sucursal_Empresa> itemAbrev)
        {
            List<string> lineasArchivo = new List<string>();
            foreach (var con in contrato)
            {
                var e = empleado.FirstOrDefault(x => x.IdEmpleado == con.IdEmpleado);
                string clave = itemAbrev.Where(x => x.IdEmpresa == con.IdEmpresaFiscal && x.IdSucursal == con.IdSucursal).Select(x => x.Clave_Imss).FirstOrDefault();
                string[] BajaIMSS = con.BajaIMSS.Value.ToString("dd-MM-yyyy").Split('-');

                string fechaImssFinal = BajaIMSS[0] + BajaIMSS[1] + BajaIMSS[2];
                var registro = "                                            ";
                registro = registro.Insert(0, emp.RegistroPatronal);
                registro = registro.Insert(11, e.NSS);
                registro = registro.Insert(22, "02");
                registro = registro.Insert(24, fechaImssFinal);
                registro = registro.Insert(40, "0000000000");
                registro = registro.TrimEnd();
                lineasArchivo.Add(registro);

            }

            //longitud de la cadena 164


            return lineasArchivo;
        }

        //EXCEL
        public static string ExcelLayout(int idUsuario, int tipoMovimiento, int empresa, DateTime FechaI, DateTime FechaF, string path, string pathDescarga)
        {
            List<Empresa> listaEmpresas = new List<Empresa>();
            List<Cliente> listaClientes = new List<Cliente>();
            List<Empleado> listaEmpleados = new List<Empleado>();
            List<Empleado_Contrato> listaContratos = new List<Empleado_Contrato>();
            List<Kardex> listaKardex = new List<Kardex>();
            List<Sucursal> listaSucursal = new List<Sucursal>();
            List<CumpleIMSS> listaCumpleImsss = new List<CumpleIMSS>();


            List<Empleado> listaEmpleadosParaCumple = new List<Empleado>();
            List<Empleado_Contrato> listaContratosParaCumple = new List<Empleado_Contrato>();

            var nombreArchivo = "REINGRESO.txt";
            Empresa itemEmpresa;

            //var FechaI = new DateTime(fecI.Year, fecI.Month, fecI.Day, 05, 00, 0);
            //var FechaF = new DateTime(fecF.Year, fecF.Month, fecF.Day, 23, 50, 0);


            using (var context = new RHEntities())
            {
                listaClientes = context.Cliente.ToList();
                listaSucursal = context.Sucursal.ToList();


                itemEmpresa = context.Empresa.FirstOrDefault(x => x.IdEmpresa == empresa);

                //ALTA-REINGRESO
                switch (tipoMovimiento)
                {
                    case 1:
                        nombreArchivo = "ALTA " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";
                        //Buscamos los contratos con fecha de Alta imss  en el rango de fecha
                        listaContratos = (from c in context.Empleado_Contrato
                                          where c.FechaIMSS >= FechaI && c.FechaIMSS <= FechaF
                                               // && c.Status == true && c.BajaIMSS == null
                                                // && c.IdEmpresaFiscal != null
                                                && c.IdEmpresaFiscal == empresa
                                          select c).ToList();
                        break;
                    case 2:
                        nombreArchivo = " BAJAS " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";
                        //Buscamos los contratos con fecha de Alta imss  en el rango de fecha
                        listaContratos = (from c in context.Empleado_Contrato
                                          where c.BajaIMSS >= FechaI && c.BajaIMSS <= FechaF
                                               // && c.Status == false && c.BajaIMSS != null
                                                // && c.IdEmpresaFiscal != null
                                                && c.IdEmpresaFiscal == empresa
                                          select c).ToList();
                        break;
                    case 3://Cambio Salsario
                        nombreArchivo = "CAMBIOS SALARIO " + itemEmpresa.RazonSocial.Substring(0, 5) + ".txt";

                        //Buscamos en Kardex los cambios de salario
                        listaKardex = (from k in context.Kardex
                                       where k.Fecha >= FechaI && k.Fecha <= FechaF
                                             && k.Tipo == (int)TipoKardex.SDI
                                       select k).ToList();

                        //Obtener los idEMpleados
                        var arrayIdEmp = listaKardex.Select(x => x.IdEmpleado).ToList();
                        
                        //Buscamos los contratos activos de los empleados
                        listaContratos = (from c in context.Empleado_Contrato
                                          where arrayIdEmp.Contains(c.IdEmpleado)
                                          select c).ToList();

                        listaContratos = listaContratos.Where(x => x.IdEmpresaFiscal == empresa).ToList();


                        //Buscamos los cumple imss
                        listaCumpleImsss = (from ci in context.CumpleIMSS
                                               where ci.FechaRegistro >= FechaI && ci.FechaRegistro <= FechaF
                                               && ci.IdEmpresaEmpleado == empresa
                                            select ci).ToList();


                        var arrayComtratosCumple = listaCumpleImsss.Select(x => x.IdContrato).ToArray();

                        listaContratosParaCumple = (from c in context.Empleado_Contrato
                            where arrayComtratosCumple.Contains(c.IdContrato)
                            select c).ToList();

                        var arrayEmpleadoCumple = listaContratosParaCumple.Select(x => x.IdEmpleado).ToArray();

                        listaEmpleadosParaCumple = (from e in context.Empleado
                            where arrayEmpleadoCumple.Contains(e.IdEmpleado)
                            select e).ToList();


                      

                        break;
                }

                //Obtenemos el array de Id empleados
                var arrayIdEmpleados = listaContratos.Select(x => x.IdEmpleado).ToArray();

                listaEmpleados = (from e in context.Empleado
                                  where arrayIdEmpleados.Contains(e.IdEmpleado)
                                  select e).ToList();

                //listaEmpresas = context.Empresa.ToList();

            }

        

            //var itemEmpresa = listaEmpresas.FirstOrDefault(x => x.IdEmpresa == itemContrato.IdEmpresaFiscal.Value);


            string strReg = "";
            switch (tipoMovimiento)
            {
                case 1:
                      strReg = ExcelRegistroAltaReingreso(itemEmpresa, listaEmpleados, listaContratos, idUsuario, path, pathDescarga, listaClientes, listaSucursal,false);
                    break;
                case 2:
                    strReg = ExcelRegistroAltaReingreso(itemEmpresa, listaEmpleados, listaContratos, idUsuario, path, pathDescarga, listaClientes, listaSucursal, true);
                    break;
                case 3:
                    //  var itemKardex = listaKardex.FirstOrDefault(x => x.IdEmpleado == itemContrato.IdEmpleado);

                       strReg = ExcelRegistroCambioSalario(itemEmpresa, listaEmpleados, listaContratos, idUsuario, path, pathDescarga, listaClientes, listaSucursal,listaKardex, listaCumpleImsss,listaEmpleadosParaCumple, listaContratosParaCumple);

                    break;
            }

            
            //Creamos el folder para guardar el archivo
            //var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, path);
            //var archivoIdse = pathUsuario + nombreArchivo;
            //File.WriteAllLines(archivoIdse, lineasArchivo);

            return strReg;

        }

      
        private static string ExcelRegistroAltaReingreso(Empresa itemEmpresa, List<Empleado> listaEmpleado, List<Empleado_Contrato> listaContratos, int idUsuario, string pathFolder, string pathDescarga, List<Cliente> listaClientes, List<Sucursal> listaSucursal, bool esBaja)
        {
            var registro = "                                                                                                                                                                                     ";
            //logitud de la cadena 169

            var workbook = new XLWorkbook();

            

            #region DOCUMENTO EXCEL - NOMINA ********************************************************

            var strNombreHoja = "ALTAS";

            if (esBaja)
            {
                strNombreHoja = "BAJAS";
            }

            var worksheet = workbook.Worksheets.Add(strNombreHoja);

            worksheet.Cell("B1").Value = itemEmpresa.RazonSocial;
            worksheet.Range("B1:D3").Row(1).Merge();

            worksheet.Cell("B1").Style.Font.SetFontSize(14)
            .Font.SetBold(true)
            .Font.SetFontColor(XLColor.Black);


            worksheet.Cell(3, 1).Value = "IdEmpleado";
            worksheet.Cell(3, 2).Value = "Paterno";
            worksheet.Cell(3, 3).Value = "Materno";
            worksheet.Cell(3, 4).Value = "Nombres";
            worksheet.Cell(3, 5).Value = "Sexo";
            worksheet.Cell(3, 6).Value = "RFC";
            worksheet.Cell(3, 7).Value = "CURP";
            worksheet.Cell(3, 8).Value = "NSS";
            worksheet.Cell(3, 9).Value = "Sd";
            worksheet.Cell(3, 10).Value = "Sdi";
            worksheet.Cell(3, 11).Value = "Sbc";
            worksheet.Cell(3, 12).Value = "Cliente";
            worksheet.Cell(3, 13).Value = esBaja ?"Fecha Baja" :"Fecha Alta" ;
            worksheet.Cell(3, 14).Value = esBaja ?"Baja IMSS" : "Fecha Imss";
            worksheet.Cell(3, 15).Value = esBaja ? "Motivo Baja" : "Fecha Real";
        

            int row = 4;

            //ordenamos el contrato por fecha
            listaContratos = esBaja ? listaContratos.OrderBy(x => x.FechaBaja).ToList() : listaContratos.OrderBy(x => x.IdContrato).ToList();
            

            foreach (var itemContrato in listaContratos)
            {
                var itemEmpleado = listaEmpleado.FirstOrDefault(x => x.IdEmpleado == itemContrato.IdEmpleado);

                if (itemEmpleado == null) continue;

                worksheet.Cell(row, 1).Value = itemEmpleado.IdEmpleado;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 2).Value = itemEmpleado.APaterno;
                worksheet.Cell(row, 3).Value = itemEmpleado.AMaterno;
                worksheet.Cell(row, 4).Value = itemEmpleado.Nombres;
                worksheet.Cell(row, 5).Value = itemEmpleado.Sexo;
                worksheet.Cell(row, 6).Value = itemEmpleado.RFC;
                worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 7).Value = itemEmpleado.CURP;
                worksheet.Cell(row, 8).Value = itemEmpleado.NSS;
                worksheet.Cell(row, 9).Value = itemContrato.SD;
                worksheet.Cell(row, 10).Value = itemContrato.SDI;
                worksheet.Cell(row, 11).Value = itemContrato.SBC;

                var itemSucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemContrato.IdSucursal);
                
                var strCliente = "--";
                if (itemSucursal != null)
                {
                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);
                    if (itemCliente != null)
                    {
                        strCliente = itemCliente.Nombre + " " + itemSucursal.Ciudad;
                    }
                }

                worksheet.Cell(row, 12).Value = strCliente;

                worksheet.Cell(row, 13).Value = esBaja ? itemContrato.FechaBaja : itemContrato.FechaAlta;
                worksheet.Cell(row, 14).Value = esBaja ? itemContrato.BajaIMSS : itemContrato.FechaIMSS;
                worksheet.Cell(row, 15).Value = esBaja ? itemContrato.MotivoBaja : itemContrato.FechaReal.ToString();
              
                row++;
            }

            #region AJUSTES DE COLUMNA
            worksheet.Column(1).AdjustToContents();
            worksheet.Column(2).AdjustToContents();
            worksheet.Column(3).AdjustToContents();
            worksheet.Column(4).AdjustToContents();
            worksheet.Column(5).AdjustToContents();
            worksheet.Column(6).AdjustToContents();
            worksheet.Column(7).AdjustToContents();
            worksheet.Column(8).AdjustToContents();
            worksheet.Column(9).AdjustToContents();
            worksheet.Column(10).AdjustToContents();
            worksheet.Column(11).AdjustToContents();
            worksheet.Column(12).AdjustToContents();
            worksheet.Column(13).AdjustToContents();
            worksheet.Column(14).AdjustToContents();
            worksheet.Column(15).AdjustToContents();

            #endregion

            #region DISEÑO

            worksheet.Range("A3:O3").Style
          .Font.SetFontSize(13)
          .Font.SetBold(true)
          .Font.SetFontColor(XLColor.Black)
          .Fill.SetBackgroundColor(XLColor.YellowGreen)
           .Border.BottomBorder = XLBorderStyleValues.Thick;

            worksheet.SheetView.Freeze(3, 4);

            #endregion




            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "";
            if (esBaja)
            {
                 nombreArchivo = "Bajas "+ itemEmpresa.RazonSocial.Substring(0, 15) + "_.xlsx";
            }
            else
            {
                nombreArchivo = "Altas " + itemEmpresa.RazonSocial.Substring(0, 15) + "_.xlsx";
            }

          

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);
            #endregion

            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;
        }
        private static string ExcelRegistroCambioSalario(Empresa itemEmpresa, List<Empleado> listaEmpleado, List<Empleado_Contrato> listaContratos, int idUsuario, string pathFolder, string pathDescarga, List<Cliente> listaClientes, List<Sucursal> listaSucursal, List<Kardex> listaKardex, List<CumpleIMSS> listaCumpleImss, List<Empleado> listaEmpleadoCumple, List<Empleado_Contrato> listaContratosCumple )
        {
            var registro = "                                                                                                                                                                                     ";
            //logitud de la cadena 169

            var workbook = new XLWorkbook();

            #region DOCUMENTO EXCEL - NOMINA ********************************************************

            var worksheet = workbook.Worksheets.Add("Cambio Salarios");

            worksheet.Cell("B1").Value = itemEmpresa.RazonSocial;
            worksheet.Range("B1:D3").Row(1).Merge();

            worksheet.Cell("B1").Style.Font.SetFontSize(14)
             .Font.SetBold(true)
             .Font.SetFontColor(XLColor.Black);


            worksheet.Cell(3, 1).Value = "IdEmpleado";
            worksheet.Cell(3, 2).Value = "Paterno";
            worksheet.Cell(3, 3).Value = "Materno";
            worksheet.Cell(3, 4).Value = "Nombres";
            worksheet.Cell(3, 5).Value = "NSS";
            worksheet.Cell(3, 6).Value = "Sdi Cambio";
            worksheet.Cell(3, 7).Value = "Fecha Reg";
            worksheet.Cell(3, 8).Value = "Cliente";

            int row = 4;

            //ordenamos la lista por fecha
            listaKardex = listaKardex.OrderBy(x => x.Fecha).ToList();
            foreach (var itemKardex in listaKardex)
            {
                var itemEmpleado = listaEmpleado.FirstOrDefault(x => x.IdEmpleado == itemKardex.IdEmpleado);

                if (itemEmpleado == null ) continue;

                var itemContrato = listaContratos.FirstOrDefault(x => x.IdEmpleado ==itemEmpleado.IdEmpleado);

                if (itemContrato == null) continue;

                worksheet.Cell(row, 1).Value = itemEmpleado.IdEmpleado;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 2).Value = itemEmpleado.APaterno;
                worksheet.Cell(row, 3).Value = itemEmpleado.AMaterno;
                worksheet.Cell(row, 4).Value = itemEmpleado.Nombres;
                worksheet.Cell(row, 5).Value = itemEmpleado.NSS;
                worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 6).Value = itemKardex.ValorNuevo;
                worksheet.Cell(row, 7).Value = itemKardex.Fecha;

                var itemSucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemContrato.IdSucursal);

                var strCliente = "--";
                if (itemSucursal != null)
                {
                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);
                    if (itemCliente != null)
                    {
                        strCliente = itemCliente.Nombre + " " + itemSucursal.Ciudad;
                    }
                }

                worksheet.Cell(row, 8).Value = strCliente;

                row++;
            }


            //CUMPLE IMSS
            row += 2;
            worksheet.Cell(row, 1).Value = "CUMPLE IMSS";

            worksheet.Cell(row, 1).Style.Font.SetFontSize(13)
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.Black);
         

            row++;
            int headerCumple = row;
            #region HEADER CUMPLE IMSS
            worksheet.Cell(row, 1).Value = "IdEmpleado";
            worksheet.Cell(row, 2).Value = "Paterno";
            worksheet.Cell(row, 3).Value = "Materno";
            worksheet.Cell(row, 4).Value = "Nombres";
            worksheet.Cell(row, 5).Value = "NSS";
            worksheet.Cell(row, 6).Value = "Fecha Imss";
            worksheet.Cell(row, 7).Value = "Año";
            worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(row, 8).Value = "FactorIntegracion";
            worksheet.Cell(row, 9).Value = "SDI Anterior";
            worksheet.Cell(row, 10).Value = "SDI Nuevo";
            worksheet.Cell(row, 11).Value = "Fecha Reg";
            worksheet.Cell(row, 12).Value = "Cliente";
            #endregion

            //ordenamos la lista por fecha
            listaCumpleImss = listaCumpleImss.OrderBy(x => x.FechaRegistro).ToList();
            //listaEmpleadoCumple, List<Empleado_Contrato> listaContratosCumple 
            foreach (var itemCumple in listaCumpleImss)
            {
                row++;
                var itemContrato = listaContratosCumple.FirstOrDefault(x => x.IdContrato == itemCumple.IdContrato);
                var itemEmpleado = listaEmpleadoCumple.FirstOrDefault(x => x.IdEmpleado == itemContrato.IdEmpleado);


                worksheet.Cell(row, 1).Value = itemContrato.IdEmpleado;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 2).Value = itemEmpleado.APaterno;
                worksheet.Cell(row, 3).Value = itemEmpleado.AMaterno;
                worksheet.Cell(row, 4).Value = itemEmpleado.Nombres;
                worksheet.Cell(row, 5).Value = itemEmpleado.NSS;
                worksheet.Cell(row, 6).Value = itemCumple.FechaIMSS;
                worksheet.Cell(row, 7).Value = itemCumple.Anio;
                worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 8).Value = itemCumple.FactorIntegracion;
                worksheet.Cell(row, 9).Value = itemCumple.SDIViejo;
                worksheet.Cell(row, 10).Value = itemCumple.SDINuevo;
                worksheet.Cell(row, 11).Value = itemCumple.FechaRegistro;

                var itemSucursal = listaSucursal.FirstOrDefault(x => x.IdSucursal == itemContrato.IdSucursal);

                var strCliente = "--";
                if (itemSucursal != null)
                {
                    var itemCliente = listaClientes.FirstOrDefault(x => x.IdCliente == itemSucursal.IdCliente);
                    if (itemCliente != null)
                    {
                        strCliente = itemCliente.Nombre + " " + itemSucursal.Ciudad;
                    }
                }

                worksheet.Cell(row,12 ).Value = strCliente;

            }


            #region AJUSTES DE COLUMNA
            worksheet.Column(1).AdjustToContents();
            worksheet.Column(2).AdjustToContents();
            worksheet.Column(3).AdjustToContents();
            worksheet.Column(4).AdjustToContents();
            worksheet.Column(5).AdjustToContents();
            worksheet.Column(6).AdjustToContents();
            worksheet.Column(7).AdjustToContents();
            worksheet.Column(8).AdjustToContents();
            worksheet.Column(9).AdjustToContents();
            worksheet.Column(10).AdjustToContents();
            worksheet.Column(11).AdjustToContents();
            worksheet.Column(12).AdjustToContents();


            #endregion

            #region DISEÑO

            worksheet.Range("A3:H3").Style
          .Font.SetFontSize(13)
          .Font.SetBold(true)
          .Font.SetFontColor(XLColor.Black)
          .Fill.SetBackgroundColor(XLColor.YellowGreen)
           .Border.BottomBorder = XLBorderStyleValues.Thick;


            string strFormatHeader = "A"+ headerCumple+":L"+ headerCumple;
           worksheet.Range(strFormatHeader).Style
          .Font.SetFontSize(13)
          .Font.SetBold(true)
          .Font.SetFontColor(XLColor.Black)
          .Fill.SetBackgroundColor(XLColor.LightSteelBlue)
           .Border.BottomBorder = XLBorderStyleValues.Thick;


            worksheet.SheetView.Freeze(3, 4);

            #endregion




            //Creamos el folder para guardar el archivo
            var pathUsuario = Utils.ValidarFolderUsuario(idUsuario, pathFolder);

            string nombreArchivo = "Cambio Salario "+ itemEmpresa.RazonSocial.Substring(0,15)+  "_.xlsx";

            var fileName = pathUsuario + nombreArchivo;
            //Guarda el archivo
            workbook.SaveAs(fileName);
            #endregion

            //  return fileName;
            return pathDescarga + "\\" + idUsuario + "\\" + nombreArchivo;
        }



    }
}
