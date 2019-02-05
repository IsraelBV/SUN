using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RH.Entidades;
using System.Diagnostics;
using System.Web;
using Excel;
using System.Data;
using Common.Utils;
using ClosedXML;
using ClosedXML.Excel;

namespace RH.BLL
{
    public class ImportacionMasivaEmpleados
    {
        #region FORMAR PLANTILLA
        public int FormarPlantilla(string fileName, int cliente, int IdSucursal, string nombreCliente)
        {
            using (var wb = new XLWorkbook(fileName))
            {
                var wsMain = wb.Worksheet(1);
                var wsData = wb.Worksheet(2);

                var ps = getPuestos(wsData, cliente);
                insertValidation(ps, 23, wsMain);

                var pp = getPerioPago(wsData, cliente);
                insertValidation(pp, 26, wsMain);

                var mp = getMetodoPago(wsData, cliente);
                insertValidation(mp, 27, wsMain);

                var tj = getTipoJornada(wsData, cliente);
                insertValidation(tj, 42, wsMain);

                var ens = getEntidadServicio(wsData, cliente);
                insertValidation(ens, 44, wsMain);

                var si = getSindicalizado(wsData, cliente);
                insertValidation(si, 45, wsMain);

                var bancos = getBancos(wsData);
                insertValidation(bancos, 41, wsMain);

                var ef = getEmpresas(wsData, IdSucursal, 1, "Fiscales", 4);
                insertValidation(ef, 32, wsMain);

                var ec = getEmpresas(wsData, IdSucursal, 2, "Complemento", 5);
                insertValidation(ec, 33, wsMain);

                var es = getEmpresas(wsData, IdSucursal, 3, "Sindicato", 6);
                insertValidation(es, 34, wsMain);

                var ea = getEmpresas(wsData, IdSucursal, 4, "Asimilado", 7);
                insertValidation(ea, 35, wsMain);

                var pa = GetParentezco(wsData, 13);
                insertValidation(pa, 49, wsMain);


                wb.Properties.Keywords = nombreCliente;

                wb.SaveAs(fileName);
            }
            return 1;
        }

        private void insertValidation(IXLRange range, int column, IXLWorksheet ws)
        {
            if (range != null)
            {
                for (int row = 2; row < 102; row++)
                {
                    ws.Cell(row, column).DataValidation.List(range);
                }
            }
        }

        private IXLRange getPuestos(IXLWorksheet ws, int cliente)
        {
            Puestos ctx_puestos = new Puestos();
            var puestos = ctx_puestos.GetPuestosLista(cliente);
            if (puestos.Count > 0)
            {
                ws.Cell(1, 2).Value = "Puestos";
                var rangePuestos = ws.Cell(2, 2).InsertData(puestos);
                return rangePuestos;
            }
            else
                return null;
        }
        private IXLRange getSindicalizado(IXLWorksheet ws, int cliente)
        {
            ws.Cell(1, 12).Value = "Sindicalizado";
            List<string> respuesta = new List<string>();
            respuesta.Add("SI");
            respuesta.Add("NO");
            var rangePuestos = ws.Cell(2, 12).InsertData(respuesta);

            return rangePuestos;


        }
        //periodicidad de pago
        private IXLRange getPerioPago(IXLWorksheet ws, int cliente)
        {
            var lista_PeriodicidadP = Cat_Sat.GetPeriodicidadPagos();


            if (lista_PeriodicidadP.Count > 0)
            {
                var lista_PeriodicidadPagos = lista_PeriodicidadP.Select(x => x.Descripcion).ToList();
                ws.Cell(1, 8).Value = "Periodicidad de pago";
                var rangePuestos = ws.Cell(2, 8).InsertData(lista_PeriodicidadPagos);
                return rangePuestos;
            }
            else
                return null;
        }


        //método de pago 
        private IXLRange getMetodoPago(IXLWorksheet ws, int cliente)
        {
            var lista_MetodoPago = Cat_Sat.GetMetodosPago();

            if (lista_MetodoPago.Count > 0)
            {
                var lista_MetodoPagos = lista_MetodoPago.Select(x => x.Descripcion).ToList();
                ws.Cell(1, 9).Value = "Método Pago";
                var rangePuestos = ws.Cell(2, 9).InsertData(lista_MetodoPagos);
                return rangePuestos;
            }
            else
                return null;
        }

        //tipo de jornada 
        private IXLRange getTipoJornada(IXLWorksheet ws, int cliente)
        {
            var lista_TipoJornada = Cat_Sat.GetTiposJornada();
            if (lista_TipoJornada.Count > 0)
            {
                var lista_TipoJornadas = lista_TipoJornada.Select(x => x.Descripcion).ToList();
                ws.Cell(1, 10).Value = "Tipo de Jornada";
                var rangePuestos = ws.Cell(2, 10).InsertData(lista_TipoJornadas);
                return rangePuestos;
            }
            else
                return null;
        }

        //
        //entidad de servicio 
        private IXLRange getEntidadServicio(IXLWorksheet ws, int cliente)
        {

            var edos = new Estados();
            var listaEntidadServicio = edos.GetEstados();
            if (listaEntidadServicio.Count > 0)
            {
                var lista_EntidadServicios = listaEntidadServicio.Select(x => x.ClaveEstado).ToList();
                ws.Cell(1, 11).Value = "Entidad de Servicio";
                var rangePuestos = ws.Cell(2, 11).InsertData(lista_EntidadServicios);
                return rangePuestos;
            }
            else
                return null;
        }

        //bancos
        private IXLRange getBancos(IXLWorksheet ws)
        {
            Bancos ctx_bancos = new Bancos();
            var bancos = ctx_bancos.ObtenerBancosDescripcion();
            if (bancos.Count > 0)
            {
                ws.Cell(1, 3).Value = "Bancos";
                var range = ws.Cell(2, 3).InsertData(bancos);
                return range;
            }
            else
                return null;
        }

        private IXLRange GetParentezco(IXLWorksheet ws, int columna)
        {
            List<string> listaParentezco = new List<string>();

            listaParentezco.Add("ESPOSO/A");
            listaParentezco.Add("HIJO/A");
            listaParentezco.Add("PADRE/MADRE");
            listaParentezco.Add("HERMANO/A");
            listaParentezco.Add("TÍO/A");
            listaParentezco.Add("PRIMO/A");
            listaParentezco.Add("SOBRINO/A");
            listaParentezco.Add("ABUELO/A");

            ws.Cell(1, columna).Value = "Parentezco";

            var range = ws.Cell(2, columna).InsertData(listaParentezco);

            return range;
        }

        private IXLRange getEmpresas(IXLWorksheet ws, int IdSucursal, int esquema, string titulo, int columna)
        {
            Empresas bll_empresas = new Empresas();
            var empresas = bll_empresas.GetEmpresasBySucursal(IdSucursal, esquema);
            if (empresas.Count > 0)
            {
                ws.Cell(1, columna).Value = titulo;
                var range = ws.Cell(2, columna).InsertData(empresas);
                return range;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region NUEVO IMPORTAR EMPLEADOS
        public int ValidateExcel(HttpPostedFileBase fileBase, string nombreCliente)
        {
            //si es un documento de excel
            if (fileBase.FileName.EndsWith(".xls") || fileBase.FileName.EndsWith(".xlsx"))
            {
                //Leer las propiedades de la hoja para comprobar que el archivo sea la plantilla de la empresa
                using (XLWorkbook wBook = new XLWorkbook(fileBase.InputStream))
                {
                    var keyword = wBook.Properties.Keywords;
                    //verifica wBook.Properties.Keywords exista
                    if (keyword != null)
                    {
                        keyword = keyword.Trim(';');
                        //si es el nombre del cliente al que se esta subiendo el excel 
                        if (keyword != nombreCliente)
                        {
                            return 2;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else {
                        return 4;
                    }
                }
            }
            else //Archivo no tiene el formato indicado
            {
                return 1;
            }
        }

        public string[] requiredData = {"Nombres", "Paterno", "Materno", "Fecha de Nacimiento", "Sexo", "RFC", "CURP", "Nacionalidad",
                                            "Estado de Origen", "Edo Civil", "Fecha Alta", "Fecha Real", "Tipo Contrato","Puesto", "Turno",
                                            "Descanso", "Periodicidad de Pago", "Método Pago", "SD", "SDI", "SBC", "Salario Real",
                                            "Banco", "Tipo de Jornada", "Tipo Salario",  "Entidad de Servicio", "Sindicalizado"};

        public string[] notRequiredData = {"NSS","Teléfono","Celular","Email","Días Contrato","No Siga Fiscal","No Siga Complemento",
                                            "Cuenta Bancaria","# Tarjeta","Clabe","RFC Beneficiario","CURP Beneficiario"};

        public Tuple<List<ValidationEmpleado>,DataTable> ExcelToDataTable(HttpPostedFileBase fileBase)
        {
            var datosValidacion = new Empleados().GetDatosValidacion();
            List<ValidationEmpleado> ListaEmpledosValidacion = new List<ValidationEmpleado>();

            #region File to Datatable
                Stream stream = fileBase.InputStream;
                IExcelDataReader reader = null;
                if (fileBase.FileName.EndsWith(".xls"))
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                else
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                reader.IsFirstRowAsColumnNames = true;
                var result = reader.AsDataSet();
                reader.Close();
                reader.Dispose();
                reader = null;
                var dataTable = result.Tables[0];
            #endregion

            //RECORRE CADA LINEA DEL EXCEL
            foreach (DataRow row in dataTable.Rows)
            {
                //si cada row del datatable tiene los 3 primeros datos
                if (row[0] == DBNull.Value && row[1] == DBNull.Value && row[2] == DBNull.Value)
                {
                    row.Delete();
                }
                else
                {   
                    //valida la infroamcion de cada row
                    var DataValidacion = ValidateRow(row, datosValidacion);
                    if (DataValidacion != null)
                    {
                        //sidevuelve algun error lo guarda en la lista
                        ListaEmpledosValidacion.Add(
                            new ValidationEmpleado(){
                                Nombre = row[0].ToString() +" "+ row[1].ToString() + " " + row[2].ToString(),
                                NumWar = DataValidacion.Where(x => x.Type == true).Count(),
                                NumError = DataValidacion.Where(x => x.Type == false).Count(),
                                data = DataValidacion
                            }
                        );
                    }
                }
            }
            dataTable.AcceptChanges();
            //ListaEmpledosValidacion = ListaEmpledosValidacion.Count == 0 ? null : ListaEmpledosValidacion;

            return Tuple.Create(ListaEmpledosValidacion, dataTable);
        }

        public List<ValidationData> ValidateRow(DataRow row, List<DatosEmpleadoValidacion> datosValidacion)
        {
            List<ValidationData> ListaValidacionDatos = new List<ValidationData>();
            //convierte en mayusculas las cadenas de texto
            row[5] = row[5].ToString().ToUpper();
            row[6] = row[6].ToString().ToUpper();
            row[46] = row[46] != DBNull.Value? row[46].ToString().ToUpper(): row[46];
            row[47] = row[47] != DBNull.Value? row[47].ToString().ToUpper() : row[47];

            //recorre las celdas de la informacion indispensable para validarlas 
            for (int j = 0; j < requiredData.Length; j++)
            {
                int TipoError = 1;
                string mensaje = "";
                //indice de celda de la informacion en el datatable
                var idx = row.Table.Columns[requiredData[j]].Ordinal + 1;
                //si la celda no esta vacia
                if (row[requiredData[j]] != DBNull.Value && row[requiredData[j]].ToString() != "")
                {
                    //0 REGEX no coincide, 1 datos correctos, 2 repetido en la base de datos, 3 no se eligio una opcion de la lista proporcionada en la plantilla
                    TipoError = ValidateCell(idx, row[requiredData[j]].ToString(), datosValidacion);
                    if (TipoError == 0)
                    {
                        mensaje = "Formato no valido.";
                    }
                    else if (TipoError == 2)
                    {
                        mensaje = "Este dato ya ha sido registrado anteriormente.";
                    }
                    else if (TipoError == 3)
                    {
                        mensaje = "Seleccione una opcion de la lista porporcionada en la plantilla.";
                    }
                }
                else
                {
                    TipoError = 2;
                    mensaje = "Indispensable para registro del empleado.";
                }

                if (TipoError != 1) {
                    ListaValidacionDatos.Add(
                        new ValidationData()
                        {
                            Title = requiredData[j],
                            Type = (TipoError == 0)?true:false,
                            Data = row[requiredData[j]].ToString(),
                            Msg = mensaje
                        }
                    );
                }
            }

            //recorre las celdas de la informacion no indispensable
            for (int j = 0; j < notRequiredData.Length; j++)
            {
                int TipoError = 1;
                string mensaje = "";
                //indice de celda de la informacion en el datatable
                var idx = row.Table.Columns[notRequiredData[j]].Ordinal + 1;
                //si la celda no esta vacia
                if (row[notRequiredData[j]] != DBNull.Value && row[notRequiredData[j]].ToString() != "")
                {
                    //0 REGEX no coincide, 1 datos correctos, 2 repetido en la base de datos, 3 no se eligio una opcion de la lista proporcionada en la plantilla
                    TipoError = ValidateCell(idx, row[notRequiredData[j]].ToString(), datosValidacion);
                    if (TipoError == 0)
                    {
                        mensaje = "Formato no valido.";
                    }
                    else if (TipoError == 2)
                    {
                        mensaje = "Este dato ya ha sido registrado anteriormente.";
                    }
                }

                if (TipoError != 1)
                {
                    ListaValidacionDatos.Add(
                        new ValidationData()
                        {
                            Title = notRequiredData[j],
                            Type = (TipoError == 0) ? true : false,
                            Data = row[notRequiredData[j]].ToString(),
                            Msg = mensaje
                        }
                    );
                }
            }

            return ListaValidacionDatos.Count != 0 ? ListaValidacionDatos : null;
        }

        /// <summary>
        /// RETORNA: 
        ///     0 si REGEX no coincide,
        ///     1 si el dato es correcto, 
        ///     2 si la informacion ya existe en DB
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="value"></param>
        /// <param name="datosValidacion"></param>
        /// <returns></returns>
        public int ValidateCell(int idx, string value, List<DatosEmpleadoValidacion> datosValidacion) {
           
            string RegExp = "";
            //valida que la informacion no este ya registrada y valida las codenas de texto segun el formato deseado
            switch (idx) {
                case 5://verifica que el sexo sea de la lista de la plantilla
                    if (value == "Mujer" || value == "Hombre") {
                        return 1;
                    }
                    else
                        return 3;
                    break;
                case 6://se analiza la compatibilidad del formato de rfc y si ya esta registrada 
                    if (Utils.ValidateData("^[A-z]{4}\\d{6}[A-z\\d]{3}$", value))
                    {
                        if (datosValidacion.Select(x => x.RFC).Contains(value))
                        {
                            return 2;
                        }
                        else
                            return 1;
                    }
                    else
                        return 0;
                    break; 
                case 7://se analiza la compatibilidad del formato de curp y si ya esta registrada
                    if (Utils.ValidateData("^[A-z]{4}\\d{6}[A-z]{6}[A-z\\d]{2}$", value))
                    {
                        if (datosValidacion.Select(x => x.CURP).Contains(value))
                        {
                            return 2;
                        }
                        else
                            return 1;
                    }
                    else
                        return 0;
                    break;
                case 8://se analiza la compatibilidad del formato de nss y si ya esta registrada
                    if (Utils.ValidateData("^\\d{11}$", value))
                    {
                        if (datosValidacion.Select(x => x.NSS).Contains(value))
                        {
                            return 2;
                        }
                        else
                            return 1;
                    }
                    else
                        return 0;
                    break;
                case 13://telefono
                    RegExp = "^\\d{10}$";
                    break;
                case 14://celular
                    RegExp = "^\\d{10}$";
                    break;
                case 15://email
                    RegExp = "^[\\w.-]+@[\\w.-]+\\.[a-zA-Z]{2,4}$";
                    break;
                case 21://dias de contrato
                    RegExp = "^[\\d]+$";
                    break;
                case 36://no siga fiscal 
                    RegExp = "^[\\d]+$";
                    break;
                case 37://no siga complemento
                    RegExp = "^[\\d]+$";
                    break;
                case 38://cuenta bancaria
                    if (Utils.ValidateData("^[\\d]{10}$", value))
                    {
                        if (datosValidacion.Select(x => x.CuentaBancaria).Contains(value))
                        {
                            return 2;
                        }
                        else
                            return 1;
                    }
                    else
                        return 0;
                    break;
                case 39://numero de tarjeta
                    if (Utils.ValidateData("^[\\d]{16}$", value))
                    {
                        if (datosValidacion.Select(x => x.NumTarjeta).Contains(value))
                        {
                            return 2;
                        }
                        else
                            return 1;
                    }
                    else
                        return 0;
                    break;
                case 40://clabe
                    if (Utils.ValidateData("^[\\d]{18}$", value))
                    {
                        if (datosValidacion.Select(x => x.Clabe).Contains(value))
                        {
                            return 2;
                        }
                        else
                            return 1;
                    }
                    else
                        return 0;
                    break;
                case 47:// RFC beneficiario
                    RegExp = "^[A-z]{4}\\d{6}[A-z\\d]{3}$";
                    break;
                case 48:// CURP beneficiario
                    RegExp = "^[A-z]{4}\\d{6}[A-z]{6}[A-z\\d]{2}$";
                    break;
                default:
                    return 1;
                    break;
                   
            }

            if (Utils.ValidateData(RegExp, value))
            {
                return 1;
            }
            else
                return 0;
        }

        public Task<bool> UploadRecordsAsync(DataTable data, int idSucursal, int idCliente, int idUsuario) {
            var t = Task.Factory.StartNew(() => {
                var res = UploadRecords(data, idSucursal, idCliente, idUsuario);
                return res;
            });
            return t;
        }
        //public bool UploadRecords(DataTable data, int idSucursal, int idCliente, int idUsuario)
        //{
        //    Empleados ctx = new Empleados();

        //    foreach (DataRow row in data.Rows)
        //    {
        //        //Datos Personales
        //        Empleado empleado = new Empleado();
        //        empleado.Nombres = row["Nombres"].ToString();
        //        empleado.APaterno = row["Paterno"].ToString();
        //        empleado.AMaterno = row["Materno"].ToString();
        //        empleado.FechaNacimiento = Convert.ToDateTime(row["Fecha de Nacimiento"].ToString());
        //        empleado.Sexo = row["Sexo"].ToString().Equals("Hombre") ? "H" : "M";
        //        empleado.RFC = row["RFC"].ToString().Trim();
        //        empleado.CURP = row["CURP"].ToString().Trim();
        //        empleado.NSS = row["NSS"].ToString();
        //        empleado.Nacionalidad = row["Nacionalidad"].ToString();
        //        empleado.Estado = row["Estado de Origen"].ToString();
        //        empleado.Telefono = row["Teléfono"].ToString();
        //        empleado.Celular = row["Celular"].ToString();
        //        empleado.Email = row["Email"].ToString();
        //        empleado.Direccion = (row["Dirección"] == DBNull.Value) ? "Dirección no proporcionada" : row["Dirección"].ToString();
        //        empleado.IdSucursal = idSucursal;
        //        empleado.EstadoCivil = row["Edo Civil"].ToString();
        //        empleado.Status = true;
        //        empleado.RFCValidadoSAT = 2;

        //        var idEmpleado = ctx.CrearEmpleado(empleado, idUsuario);

        //        if (idEmpleado > 0)
        //        {
        //            //Datos de Contratación
        //            Empleado_Contrato contrato = new Empleado_Contrato();
        //            contrato.IdEmpleado = idEmpleado;
        //            contrato.FechaAlta = Convert.ToDateTime(row["Fecha Alta"].ToString());
        //            contrato.FechaReal = Convert.ToDateTime(row["Fecha Real"].ToString());
        //            if (row["Fecha IMSS"] != DBNull.Value)
        //                contrato.FechaIMSS = Convert.ToDateTime(row["Fecha IMSS"].ToString());
        //            if (row["UMF"] != DBNull.Value)
        //                contrato.UMF = row["UMF"].ToString();
        //            contrato.TipoContrato = row["Tipo Contrato"].ToString().Equals("Temporal") ? 2 : 1;
        //            if (contrato.TipoContrato == 2)
        //            {
        //                contrato.Vigencia = Convert.ToDateTime(row["Vigencia"].ToString());
        //                contrato.DiasContrato = Convert.ToInt32(row["Días Contrato"]);
        //            }
        //            Puestos ctxPuestos = new Puestos();
        //            contrato.IdPuesto = ctxPuestos.ObtenerPuestoPorDescripcion(row["Puesto"].ToString());
        //            contrato.Turno = UtilsEmpleados.SeleccionarTurno(row["Turno"].ToString());
        //            contrato.DiaDescanso = UtilsEmpleados.selectDay(row["Descanso"].ToString());
        //            contrato.IdPeriodicidadPago = UtilsEmpleados.SeleccionarPeriodicidadDePago(row["Periodicidad de pago"].ToString());
        //            contrato.FormaPago = UtilsEmpleados.SeleccionarFormaPago(row["Método Pago"].ToString());
        //            contrato.PagoElectronico = (contrato.FormaPago == 3 || contrato.FormaPago == 4 || contrato.FormaPago == 5 || contrato.FormaPago == 6 || contrato.FormaPago == 7) ? true : false;
        //            contrato.SD = Convert.ToDecimal(row["SD"].ToString());
        //            contrato.SDI = Convert.ToDecimal(row["SDI"].ToString());
        //            contrato.SBC = Convert.ToDecimal(row["SBC"].ToString());
        //            contrato.SalarioReal = Convert.ToDecimal(row["Salario Real"].ToString());
        //            contrato.IdTipoJornada = UtilsEmpleados.SeleccionarTipoNomina(row["Tipo de Jornada"].ToString());
        //            contrato.TipoSalario = UtilsEmpleados.SeleccionarTipoSalario(row["Tipo Salario"].ToString());
        //            contrato.EntidadDeServicio = (row["Entidad de Servicio"].ToString());
        //            contrato.Sindicalizado = (row["Sindicalizado"].ToString().ToUpper().Equals("SI")) ? true : false;
        //            contrato.Status = true;
        //            contrato.IdSucursal = idSucursal;



        //            Empresas ctxRP = new Empresas();
        //            int idEmpresaFiscal = 0;
        //            int idEmpresaAsimilado = 0;

        //            if (row["Empresa Fiscal"] != DBNull.Value)
        //            {
        //                idEmpresaFiscal = ctxRP.GetIdByRazonSocial(row["Empresa Fiscal"].ToString(), idCliente);

        //                contrato.IdEmpresaFiscal = idEmpresaFiscal;
        //            }

        //            if (row["Empresa Complemento"] != DBNull.Value)
        //                contrato.IdEmpresaComplemento = ctxRP.GetIdByRazonSocial(row["Empresa Complemento"].ToString(), idCliente);

        //            if (row["Empresa Sindicato"] != DBNull.Value)
        //                contrato.IdEmpresaSindicato = ctxRP.GetIdByRazonSocial(row["Empresa Sindicato"].ToString(), idCliente);

        //            if (row["Empresa Asimilado"] != DBNull.Value)
        //            {
        //                idEmpresaAsimilado = ctxRP.GetIdByRazonSocial(row["Empresa Asimilado"].ToString(), idCliente);
        //                contrato.IdEmpresaAsimilado = idEmpresaAsimilado;
        //            }

        //            //Tipo Regimen
        //            contrato.IdTipoRegimen = idEmpresaAsimilado > 0 ? 8 : 1;//Asimilado Honorarios sino Sueldo

        //            //Tipo Jornada
        //            contrato.IdTipoJornada = 3;

        //            //Crea el contrato en la base de datos
        //            ctx.CrearContrato(contrato);

        //            DatosBancarios bancarios = new DatosBancarios();
        //            bancarios.IdEmpleado = idEmpleado;
        //            Bancos bllBancos = new Bancos();

        //            if (row["Banco"].ToString().Trim() != "")
        //            {
        //                bancarios.IdBanco = bllBancos.ObtenerIdBancoPorDescripcion(row["Banco"].ToString());
        //            }

        //            int numDatosBancarios = 0;
        //            if (row["No Siga Fiscal"] != DBNull.Value)
        //            {
        //                if (row["No Siga Fiscal"].ToString().Trim() != "")
        //                {
        //                    bancarios.NoSigaF = Convert.ToInt32(row["No Siga Fiscal"]);
        //                    numDatosBancarios++;
        //                }
        //            }
        //            if (row["No Siga Complemento"] != DBNull.Value)
        //            {
        //                if (row["No Siga Complemento"].ToString().Trim() != "")
        //                {
        //                    bancarios.NoSigaC = Convert.ToInt32(row["No Siga Complemento"]);
        //                    numDatosBancarios++;
        //                }
        //            }


        //            if (row["Cuenta Bancaria"] != DBNull.Value)
        //            {
        //                bancarios.CuentaBancaria = row["Cuenta Bancaria"].ToString();
        //                numDatosBancarios++;
        //            }

        //            if (row["# Tarjeta"] != DBNull.Value)
        //            {
        //                bancarios.NumeroTarjeta = row["# Tarjeta"].ToString();
        //                numDatosBancarios++;
        //            }

        //            if (row["Clabe"] != DBNull.Value)
        //            {
        //                bancarios.Clabe = row["Clabe"].ToString();
        //                numDatosBancarios++;
        //            }


        //            #region BENEFICIARIOS

        //            if (row["Nombre Beneficiario"] != DBNull.Value)
        //            {
        //                bancarios.NombreBeneficiario = row["Nombre Beneficiario"].ToString();
        //                numDatosBancarios++;
        //            }

        //            if (row["RFC Beneficiario"] != DBNull.Value)
        //            {
        //                bancarios.RFCBeneficiario = row["RFC Beneficiario"].ToString();
        //                numDatosBancarios++;
        //            }

        //            if (row["CURP Beneficiario"] != DBNull.Value)
        //            {
        //                bancarios.CURPBeneficiario = row["CURP Beneficiario"].ToString();
        //                numDatosBancarios++;
        //            }

        //            if (row["Parentezco Beneficiario"] != DBNull.Value)
        //            {
        //                bancarios.ParentezcoBeneficiario = row["Parentezco Beneficiario"].ToString();
        //                numDatosBancarios++;
        //            }

        //            if (row["Domicilio Beneficiario"] != DBNull.Value)
        //            {
        //                bancarios.DomicilioBeneficiario = row["Domicilio Beneficiario"].ToString();
        //                numDatosBancarios++;
        //            }
        //            #endregion



        //            bancarios.Status = true;

        //            if (numDatosBancarios > 0)
        //                ctx.NewDatosBancarios(bancarios, false);

        //            var noti = new Notificaciones();
        //            noti.Alta(idEmpleado);

        //            //Asignar conceptos Default
        //            ConceptosNomina.AsignarConceptosDefaultByEmpleado(idSucursal, idEmpleado);
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public bool UploadRecords(DataTable data, int idSucursal, int idCliente, int idUsuario)
        {
            try { 
                List<Empleado> listaEmpleados = new List<Empleado>();
                List<Empleado_Contrato> listaContratos = new List<Empleado_Contrato>();
                List<DatosBancarios> listaBancarios = new List<DatosBancarios>();
                List<Puesto> puestos = new List<Puesto>();
                List<Empresa> empresas = new List<Empresa>();
                List<C_Banco_SAT> bancos = new List<C_Banco_SAT>();

                using (var context = new RHEntities())
                {
                    puestos = context.Puesto.ToList();
                    empresas = context.Empresa.ToList();
                    bancos = context.C_Banco_SAT.ToList();
                }

                Empleados ctx = new Empleados();

                #region Regitro de datos personal de empleados
                foreach (DataRow row in data.Rows)
                {
                    //Datos Empleado
                    Empleado empleado = new Empleado();
                    empleado.Nombres = row["Nombres"].ToString();
                    empleado.APaterno = row["Paterno"].ToString();
                    empleado.AMaterno = row["Materno"].ToString();
                    empleado.FechaNacimiento = Convert.ToDateTime(row["Fecha de Nacimiento"].ToString());
                    empleado.Sexo = row["Sexo"].ToString().Equals("Hombre") ? "H" : "M";
                    empleado.RFC = row["RFC"].ToString().Trim();
                    empleado.CURP = row["CURP"].ToString().Trim();
                    empleado.NSS = row["NSS"].ToString();
                    empleado.Nacionalidad = row["Nacionalidad"].ToString();
                    empleado.Estado = row["Estado de Origen"].ToString();
                    empleado.Telefono = row["Teléfono"].ToString();
                    empleado.Celular = row["Celular"].ToString();
                    empleado.Email = row["Email"].ToString();
                    empleado.Direccion = (row["Dirección"] == DBNull.Value) ? "Dirección no proporcionada" : row["Dirección"].ToString();
                    empleado.IdSucursal = idSucursal;
                    empleado.EstadoCivil =  row["Edo Civil"].ToString();
                    empleado.Status = true;
                    empleado.RFCValidadoSAT = 2;
                    empleado.FechaReg = DateTime.Now;
                    empleado.IdUsuarioReg = idUsuario;

                    listaEmpleados.Add(empleado);
                }
                // se registran todos los datos personales del empleado a ecxepcion de los de contrato y bancarios
                ctx.CrearEmpleados(listaEmpleados);
                #endregion

                #region REGISTRO DE CONTRATOS - DATOS BANCARIOS
                foreach (DataRow row in data.Rows)
                {
                    #region DATOS CONTRATO
                    var rfcComparacion = row["RFC"].ToString().Trim();
                    var idEmpleado = listaEmpleados.Where(x => x.RFC == rfcComparacion).Select(x => x.IdEmpleado).FirstOrDefault();

                    if (idEmpleado == 0) continue;

                    //Datos de Contratación
                    Empleado_Contrato contrato = new Empleado_Contrato();

                    contrato.IdEmpleado = idEmpleado;
                    contrato.FechaAlta = Convert.ToDateTime(row["Fecha Alta"].ToString());
                    contrato.FechaReal = Convert.ToDateTime(row["Fecha Real"].ToString());
                    if (row["Fecha IMSS"] != DBNull.Value)
                        contrato.FechaIMSS = Convert.ToDateTime(row["Fecha IMSS"].ToString());
                    if (row["UMF"] != DBNull.Value)
                        contrato.UMF = row["UMF"].ToString();
                    contrato.TipoContrato = row["Tipo Contrato"].ToString().Equals("Temporal") ? 2 : 1;
                    if (contrato.TipoContrato == 2)
                    {
                        contrato.Vigencia = Convert.ToDateTime(row["Vigencia"].ToString());
                        contrato.DiasContrato = Convert.ToInt32(row["Días Contrato"]);
                    }

                    var puestoDatatable = row["Puesto"].ToString();
                    contrato.IdPuesto = puestos.Where(x => x.Descripcion == puestoDatatable).Select(x => x.IdPuesto).FirstOrDefault();
                    contrato.Turno = UtilsEmpleados.SeleccionarTurno(row["Turno"].ToString());
                    contrato.DiaDescanso = UtilsEmpleados.selectDay(row["Descanso"].ToString());
                    contrato.IdPeriodicidadPago = UtilsEmpleados.SeleccionarPeriodicidadDePago(row["Periodicidad de pago"].ToString());
                    contrato.FormaPago = UtilsEmpleados.SeleccionarFormaPago(row["Método Pago"].ToString());
                    contrato.PagoElectronico = (contrato.FormaPago == 3 || contrato.FormaPago == 4 || contrato.FormaPago == 5 || contrato.FormaPago == 6 || contrato.FormaPago == 7) ? true : false;
                    contrato.SD = Convert.ToDecimal(row["SD"].ToString());
                    contrato.SDI = Convert.ToDecimal(row["SDI"].ToString());
                    contrato.SBC = Convert.ToDecimal(row["SBC"].ToString());
                    contrato.SalarioReal = Convert.ToDecimal(row["Salario Real"].ToString());
                    contrato.IdTipoJornada = UtilsEmpleados.SeleccionarTipoNomina(row["Tipo de Jornada"].ToString());
                    contrato.TipoSalario = UtilsEmpleados.SeleccionarTipoSalario(row["Tipo Salario"].ToString());
                    contrato.EntidadDeServicio = (row["Entidad de Servicio"].ToString());
                    contrato.Sindicalizado = (row["Sindicalizado"].ToString().ToUpper().Equals("SI")) ? true : false;
                    contrato.Status = true;
                    contrato.IdSucursal = idSucursal;

                    int idEmpresaAsimilado = 0;

                    if (row["Empresa Fiscal"] != DBNull.Value)
                    {
                        var empresaFiscalDatatable = row["Empresa Fiscal"].ToString();
                        contrato.IdEmpresaFiscal = empresas.Where(x => x.RazonSocial == empresaFiscalDatatable).Select(x => x.IdEmpresa).FirstOrDefault();
                    }

                    if (row["Empresa Complemento"] != DBNull.Value)
                    {
                        var empresaComplementoDatatable = row["Empresa Complemento"].ToString();
                        contrato.IdEmpresaComplemento = empresas.Where(x => x.RazonSocial == empresaComplementoDatatable).Select(x => x.IdEmpresa).FirstOrDefault();
                    }

                    if (row["Empresa Sindicato"] != DBNull.Value)
                    {
                        var empresaSindicatoDatatable = row["Empresa Sindicato"].ToString();
                        contrato.IdEmpresaSindicato = empresas.Where(x => x.RazonSocial == empresaSindicatoDatatable).Select(x => x.IdEmpresa).FirstOrDefault();
                    }

                    if (row["Empresa Asimilado"] != DBNull.Value)
                    {
                        var empresaAsimiladoDatatable = row["Empresa Asimilado"].ToString();
                        idEmpresaAsimilado = empresas.Where(x => x.RazonSocial == empresaAsimiladoDatatable).Select(x => x.IdEmpresa).FirstOrDefault();
                        contrato.IdEmpresaAsimilado = idEmpresaAsimilado;
                    }

                    //Tipo Regimen
                    contrato.IdTipoRegimen = idEmpresaAsimilado > 0 ? 8 : 1;//Asimilado Honorarios sino Sueldo

                    //Tipo Jornada
                    contrato.IdTipoJornada = 3;
                    contrato.FechaReg = DateTime.Now;
                    contrato.IdUsuarioReg = idUsuario;

                    #endregion
                    #region DATOS BANCARIOS

                    DatosBancarios bancarios = new DatosBancarios();
                    bancarios.IdEmpleado = idEmpleado;

                    if (row["Banco"].ToString().Trim() != "")
                    {
                        var bancoDatatable = row["Banco"].ToString();
                        bancarios.IdBanco = bancos.Where(x => x.Descripcion == bancoDatatable).Select(x => x.IdBanco).FirstOrDefault();
                    }

                    if (row["No Siga Fiscal"] != DBNull.Value)
                    {
                        if (row["No Siga Fiscal"].ToString().Trim() != "")
                        {
                            bancarios.NoSigaF = Convert.ToInt32(row["No Siga Fiscal"]);
                        }
                    }
                    if (row["No Siga Complemento"] != DBNull.Value)
                    {
                        if (row["No Siga Complemento"].ToString().Trim() != "")
                        {
                            bancarios.NoSigaC = Convert.ToInt32(row["No Siga Complemento"]);
                        }
                    }

                    if (row["Cuenta Bancaria"] != DBNull.Value)
                    {
                        bancarios.CuentaBancaria = row["Cuenta Bancaria"].ToString();
                    }

                    if (row["# Tarjeta"] != DBNull.Value)
                    {
                        bancarios.NumeroTarjeta = row["# Tarjeta"].ToString();
                    }

                    if (row["Clabe"] != DBNull.Value)
                    {
                        bancarios.Clabe = row["Clabe"].ToString();
                    }

                    #region BENEFICIARIOS

                    if (row["Nombre Beneficiario"] != DBNull.Value)
                    {
                        bancarios.NombreBeneficiario = row["Nombre Beneficiario"].ToString();
                    }

                    if (row["RFC Beneficiario"] != DBNull.Value)
                    {
                        bancarios.RFCBeneficiario = row["RFC Beneficiario"].ToString();
                    }

                    if (row["CURP Beneficiario"] != DBNull.Value)
                    {
                        bancarios.CURPBeneficiario = row["CURP Beneficiario"].ToString();
                    }

                    if (row["Parentezco Beneficiario"] != DBNull.Value)
                    {
                        bancarios.ParentezcoBeneficiario = row["Parentezco Beneficiario"].ToString();
                    }

                    if (row["Domicilio Beneficiario"] != DBNull.Value)
                    {
                        bancarios.DomicilioBeneficiario = row["Domicilio Beneficiario"].ToString();
                    }
                    #endregion

                    bancarios.Status = true;

                    //se agregan a las listas 
                    listaContratos.Add(contrato);
                    listaBancarios.Add(bancarios);

                    #endregion

                }//fin del segundo for

                //se guardan los datos en DB
                ctx.CrearContratosDatosB(listaContratos, listaBancarios);

                #endregion

                var noti = new Notificaciones();
                var arrayIdsEmpleado = listaEmpleados.Select(x => x.IdEmpleado).ToArray();
                //noti.Alta(idEmpleado);
                noti.Alta(arrayIdsEmpleado, idUsuario);

                KardexEmpleado kardex = new KardexEmpleado();
                kardex.AltaByArray(arrayIdsEmpleado, idUsuario);

                //Asignar conceptos Default
                ConceptosNomina.AsignarConceptosDefaultByEmpleados(idSucursal, arrayIdsEmpleado);
                //ConceptosNomina.AsignarConceptosDefaultByEmpleado(idSucursal, idEmpleado);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

    }
}
