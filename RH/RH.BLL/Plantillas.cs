using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Common.Helpers;
using System.Web;
using Novacode;
using Common.Utils;
using SYA.BLL;
namespace RH.BLL
{
    public class Plantillas
    {
        readonly RHEntities _ctx;

        public Plantillas()
        {
            _ctx = new RHEntities();
        }

        /// <summary>
        /// Obtiene todas las plantillas activas
        /// </summary>
        /// <returns></returns>
        public List<Plantilla> GetPlantillas()
        {
            return _ctx.Plantilla.Where(x => x.Status == true).ToList();
        }


        /// <summary>
        /// Agregar una nueva plantilla a la bd
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool AgregarPlantilla(Plantilla p)
        {
            var result = false;

            _ctx.Plantilla.Add(p);
            var r = _ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }


        public bool EliminarPlantilla(int idPlantilla)
        {
            var result = false;
            var item = _ctx.Plantilla.FirstOrDefault(x => x.Id == idPlantilla);

            _ctx.Plantilla.Attach(item);
            _ctx.Plantilla.Remove(item);

            var r = _ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;

        }

        public bool EditarPlantilla(Plantilla model)
        {
            var original = _ctx.Plantilla.Where(x => x.Id == model.Id).FirstOrDefault();
            if (original != null)
            {
                original.Clientes = model.Clientes;
                var status = _ctx.SaveChanges();
                return status > 0 ? true : false;
            }
            return false;
        }



        public List<Plantilla> GetPlantillasByTipo(int tipo, int IdCliente)
        {
            var list = _ctx.Plantilla.Where(x => x.Tipo != 2 && x.Tipo != 3 && x.Tipo != 4 && x.Tipo != 5 && x.Tipo != 6).ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Clientes != "*")
                {
                    if (list[i].Clientes != null)
                    {
                        var array = list[i].Clientes.Split(',');
                        var found = Array.IndexOf(array, IdCliente.ToString());
                        if (found == -1)
                        {
                            list.RemoveAt(i);
                        }
                    }
                    else
                    {
                        list.RemoveAt(i);
                    }
                }
            }
            return list;
        }

        public List<Plantilla> GetPlantillasByTipo2(int tipo, int IdCliente)
        {
            var list = _ctx.Plantilla.Where(x => x.Tipo == tipo).ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Clientes != "*")
                {
                    if (list[i].Clientes != null)
                    {
                        var array = list[i].Clientes.Split(',');
                        var found = Array.IndexOf(array, IdCliente.ToString());
                        if (found == -1)
                        {
                            list.RemoveAt(i);
                        }
                    }
                    else
                    {
                        list.RemoveAt(i);
                    }
                }
            }
            return list;
        }
        //public List<Plantilla> GetPlantillas(int tipo, int IdCliente)
        //{
        //    var list = _ctx.Plantilla.Where(x => x.Tipo == tipo).ToList();
        //    for (int i = list.Count - 1; i >= 0; i--)
        //    {
        //        if (list[i].Clientes != "*")
        //        {
        //            if (list[i].Clientes != null)
        //            {
        //                var array = list[i].Clientes.Split(',');
        //                var found = Array.IndexOf(array, IdCliente.ToString());
        //                if (found == -1)
        //                {
        //                    list.RemoveAt(i);
        //                }
        //            }
        //            else
        //            {
        //                list.RemoveAt(i);
        //            }
        //        }
        //    }
        //    return list;
        //}
        public Plantilla GetPlantillaById(int id)
        {
            return _ctx.Plantilla.Where(x => x.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Genera varias plantillas y las guarda en un archivo zip
        /// </summary>
        /// <param name="IdPlantilla"></param>
        /// <param name="TipoPlantilla"></param>
        /// <param name="empleados"></param>
        /// <returns></returns>
        public string FormarPlantilla(int IdPlantilla, int TipoPlantilla, int[] elementos)
        {
            //Obtiene el nombre del usuario 
            var id = SessionHelpers.GetIdUsuario();
            ControlUsuario cu = new ControlUsuario();
            var usuario = cu.GetUsuarioById(id);
            var userName = usuario == null ? "Alianza" : usuario.Usuario;

            var plantilla = GetPlantillaById(IdPlantilla);
            //carpeta donde se encuentra la plantilla original
            var sourcePath = HttpContext.Current.Server.MapPath("~//Files/Plantillas");

            //carpeta donde se guardará la copia
            var targetPath = HttpContext.Current.Server.MapPath("~//Files/Plantillas/" + userName);

            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
            else
            {
                System.IO.Directory.Delete(targetPath, true);
                System.IO.Directory.CreateDirectory(targetPath);
            }

            var sourceFile = System.IO.Path.Combine(sourcePath, plantilla.NombreArchivo);
            var destFile = System.IO.Path.Combine(targetPath, plantilla.NombreArchivo);

            try
            {
                if (System.IO.File.Exists(destFile))
                    System.IO.File.Delete(destFile);
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            catch (System.IO.IOException)
            {
                return null;
            }

            if (destFile != null)
            {
                //Forma una carpeta temporal
                var newPath = FormarCarpetaTemporal(userName);

                //Aquí la porción de código para sustituir palabras
                switch (TipoPlantilla)
                {
                    case 1:
                        //Contratos
                        PlantillaContrato(destFile, newPath, elementos);
                        break;
                    case 2:
                        //Vacaciones
                        PlantillaVacaciones(destFile, newPath, elementos);
                        break;
                    case 3:
                        //Incapacidades
                        break;
                    case 4:
                        //Baja
                        PlantillaBaja(destFile, newPath, elementos);
                        break;
                    case 5:
                        //Permisos
                        PlantillaPermisos(destFile, newPath, elementos);
                        break;
                    case 6:
                        //Préstamos
                    case 7:
                        //Movimiento de Personal
                        PlantillaMovimientoPersonal(destFile, newPath, elementos);
                        break;
                    case 8:
                        //Carta de Antigüedad
                        PlantillaCartaAntiguedad(destFile, newPath, elementos);
                        break;
                    case 9:
                        //Gastos Médicos Menores
                        PlantillaGastosMedicosMenores(destFile, newPath, elementos);
                        break;
                    case 10:
                        //Sindicato
                        PlantillaSindicato(destFile, newPath, elementos);
                        break;
                    default:
                        break;
                }

                //Crea un archivo zip para descargar las plantillas en caso de que sean varias
                var zipFile = HttpContext.Current.Server.MapPath("~//Files/Plantillas/" + userName + "/" + DateTime.Now.ToString("dd-MM-yyyy") + ".zip");
                ZipDirectory(newPath, zipFile);

                return zipFile;
            }

            return null;
        }


        public string FormarCarpetaTemporal(string user)
        {
            var targetPath = HttpContext.Current.Server.MapPath("~//Files/Plantillas/" + user + "/" + DateTime.Now.ToString("dd-MM-yyyy") + "/");
            try
            {
                if (!System.IO.Directory.Exists(targetPath))
                    System.IO.Directory.CreateDirectory(targetPath);
                else
                {
                    System.IO.Directory.Delete(targetPath, true);
                    System.IO.Directory.CreateDirectory(targetPath);
                }
                return targetPath;
            }
            catch (System.IO.IOException )
            {
                return null;
            }
        }

        /// <summary>
        /// Crear un archivo zip de una carpeta
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        public void ZipDirectory(string sourcePath, string destPath)
        {
            try
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(sourcePath, destPath);
            }
            catch (System.IO.IOException )
            {

            }
        }

        public bool ArchivoRegistrado(string nomArchivo)
        {
            bool result = false;

            var item = _ctx.Plantilla.FirstOrDefault(x => x.NombreArchivo == nomArchivo);

            if (item != null)
            {
                result = true;
            }

            return result;
        }

        private void PlantillaContrato(string file, string newPath, int[] empleados)
        {
            Empleados emp = new Empleados();
            Puestos ps = new Puestos();
            //Banco bn = new Banco();

            foreach (var item in empleados)
            {
                using (var document = DocX.Load(file))
                {
                    var cuentab = " ";
                    var descripcionb = " ";
                    var empleado = emp.GetEmpleadoById(item);
                    var contrato = emp.ObtenerContratoEmpleadoPorId(item);
                    var bancos = emp.GetDatosBancariosByIdEmpleado(item);
                    if (bancos != null)
                    {
                        cuentab = bancos.CuentaBancaria;
                        descripcionb = bancos.Descripcion;
                    
                    }
                    var newDoc = newPath + empleado.APaterno +"_"+ empleado.AMaterno +"_"+ empleado.Nombres +  "_.docx";
                    document.ReplaceText("<<Empleado_Paterno>>", empleado.APaterno);
                    document.ReplaceText("<<Empleado_Materno>>", empleado.AMaterno);
                    document.ReplaceText("<<Empleado_Nombres>>", empleado.Nombres);
                    document.ReplaceText("<<Empleado_Nacionalidad>>", empleado.Nacionalidad);
                    var edad = DateTime.Now.Year - empleado.FechaNacimiento.Year;
                    document.ReplaceText("<<Empleado_Edad>>", edad.ToString());
                    document.ReplaceText("<<Empleado_NSS>>", empleado.NSS == null ? " ": empleado.NSS);
                    document.ReplaceText("<<Empleado_EstadoCivil>>", empleado.EstadoCivil);
                    if (empleado.Direccion != null)
                        document.ReplaceText("<<Empleado_Domicilio>>", empleado.Direccion);

                    document.ReplaceText("<<Empleado_CURP>>", empleado.CURP);
                    document.ReplaceText("<<Empleado_RFC>>", empleado.RFC);

                    if (contrato.IdPuesto != null)
                    {
                        var puesto = ps.GetPuesto(contrato.IdPuesto);
                        document.ReplaceText("<<Empleado_Puesto>>", puesto.Descripcion);
                    }
                    document.ReplaceText("<<Empleado_SalarioDiario>>", contrato.SD.ToString());
                    document.ReplaceText("<<Empleado_SalarioDiarioLetras>>", Utils.ConvertCantidadALetras(contrato.SD.ToString()));
                    var periocidad = _ctx.C_PeriodicidadPago_SAT.Where(x => x.IdPeriodicidadPago == contrato.IdPeriodicidadPago).Select(x => x.Descripcion).FirstOrDefault();
                    document.ReplaceText("<<Empleado_TipoDeNomina>>", periocidad);

                    if (cuentab != null)
                    {
                        document.ReplaceText("<<Empleado_CuentaBancaria>>", cuentab);
                    }
                    document.ReplaceText("<<Empleado_BancoDeCuenta>>", descripcionb);
                    document.ReplaceText("<<Empleado_FechaAntiguedad>>", contrato.FechaReal.ToString("dd-MM-yyyy"));
                    document.ReplaceText("<<Empleado_FechaAltaIMSS>>", contrato.FechaIMSS == null ?"sin fecha":contrato.FechaIMSS.Value.ToString("dd-MM-yyyy"));
                    document.ReplaceText("<<Empleado_DiasDeContrato>>", contrato.DiasContrato.ToString());
                    document.ReplaceText("<<Empleado_VenceContrato>>", contrato.Vigencia.ToString());
                   
                    document.SaveAs(newDoc);
                }
            }
        }

        private void PlantillaBaja(string file, string newPath, int[] empleados)
        {
            if (empleados != null)
            {
                Empleados emp = new Empleados();
                Puestos ps = new Puestos();
                foreach (var item in empleados)
                {
                    using (var document = DocX.Load(file))
                    {
                        var empleado = emp.GetEmpleadoById(item);
                        var contrato = emp.GetUltimoContrato(item);
                        var newDoc = newPath + item + ".docx";
                        document.ReplaceText("<<Empleado_Empresa>>", "");
                        document.ReplaceText("<<FechaActual>>", DateTime.Now.ToString("dd-MM-yyyy"));
                        document.ReplaceText("<<Empleado_Nombres>>", empleado.Nombres);
                        document.ReplaceText("<<Empleado_Paterno>>", empleado.APaterno);
                        document.ReplaceText("<<Empleado_Materno>>", empleado.AMaterno);
                        if (contrato.IdPuesto != null)
                        {
                            var puesto = ps.GetPuesto(contrato.IdPuesto);
                            document.ReplaceText("<<Empleado_Puesto>>", puesto.Descripcion);
                        }

                        var BajaImss = contrato.BajaIMSS != null ? contrato.BajaIMSS.Value.ToString("dd-MM-yyyy") : "";
                        document.ReplaceText("<<Empleado_BajaIMSS>>", BajaImss);

                        document.ReplaceText("<<Empleado_FechaReal>>", contrato.FechaReal.ToString("dd-MM-yyyy"));

                        var nss = empleado.NSS != null ? empleado.NSS : "";
                        document.ReplaceText("<<Empleado_NSS>>", nss);

                        var sr = contrato.SalarioReal != null ? contrato.SalarioReal.ToString() : "";
                        document.ReplaceText("<<Empleado_SalarioReal>>", sr);

                        var sdi = contrato.SDI != null ? contrato.SDI.ToString() : "";
                        document.ReplaceText("<<Empleado_SDI>>", sdi);

                        var fechaBaja = contrato.FechaBaja != null ? contrato.FechaBaja.Value.ToString("dd-MM-yyyy") : "";
                        document.ReplaceText("<<Empleado_FechaDeBaja>>", fechaBaja);

                        document.SaveAs(newDoc);

                    }
                }
            }
        }
        public void PlantillaPermisos(string file, string newPath, int[] permiso)
        {
            _Permisos perm = new _Permisos();
            foreach (var item in permiso)
            {
                using (var document = DocX.Load(file))
                {
                    var empresadoc = "";
                    var permisos = perm.GetDatosPlantilla(item);
                    var newDoc = newPath + item + ".docx";
                    var observacion = "";
                    if (permisos.goce == true)
                    {
                        document.ReplaceText("<<Tipo_Permiso1>>", "CON");
                        document.ReplaceText("<<tipo_permiso2>>", "con");
                    } else
                    {
                        document.ReplaceText("<<Tipo_Permiso1>>", "SIN");
                        document.ReplaceText("<<tipo_permiso2>>", "sin");
                    }

                    if (permisos.Observaciones == null)
                    {
                        observacion = "";
                    } else
                    {
                        observacion = permisos.Observaciones;
                    }
                    if (permisos.PermisoDias == 0)
                    {
                        permisos.PermisoDias = 1;
                    }

                    //aqui se define que empresa tiene el empleado 
                    //
                    //
                    
                    DatosEmpleadoPermiso empresa = new DatosEmpleadoPermiso();
                    
                    empresa = perm.EmpresaEmpleado(permisos.IdEmpleado);

                    if (empresa.EmpresaFiscal == null)
                    {
                        if(empresa.EmpresaAsimilado == null)
                        {
                            if (empresa.EmpresaComplemento == null)
                            {
                                empresadoc = empresa.EmpresaSindicato;
                            }
                            else
                            {
                                empresadoc = empresa.EmpresaComplemento;
                            }
                        }else
                        {
                            empresadoc = empresa.EmpresaAsimilado;
                        }
                    }
                    else
                    {
                        empresadoc = empresa.EmpresaFiscal;
                    }



                    document.ReplaceText("<<Empleado_Empresa>>", empresadoc);
                    document.ReplaceText("<<Empleado_FechaActual>>", DateTime.Now.ToString("dd-MM-yyyy"));
                    document.ReplaceText("<<Empleado_Nombres>>", permisos.Nombre);
                    document.ReplaceText("<<Empleado_Paterno>>", permisos.APaterno);
                    document.ReplaceText("<<Empleado_Materno>>", permisos.AMaterno);
                    document.ReplaceText("<<Empleado_Departamento>>", permisos.Depa);
                    document.ReplaceText("<<Empleado_Puesto>>", permisos._Puesto);
                    document.ReplaceText("<<Permisos_Inicio>>", permisos.FechaInicio.ToString("dd-MM-yyyy"));
                    document.ReplaceText("<<Permisos_Fin>>", permisos.FechaFin.ToString("dd-MM-yyyy"));
                    document.ReplaceText("<<Permisos_Presentarse>>", permisos.FechaPresentarse.ToString("dd-MM-yyyy"));
                    document.ReplaceText("<<Permisos_Observacion>>", observacion);
                    document.ReplaceText("<<Permisos_Dias>>", Convert.ToString(permisos.PermisoDias));
                    document.SaveAs(newDoc);
                }


            }
        }
        private void PlantillaVacaciones(string file, string newPath, int[] vacaciones)
        {
   

            var empresadoc = "";
            VacacionesClase vacas = new VacacionesClase();
            foreach (var item in vacaciones)
            {
                using (var documento = DocX.Load(file))
                {
                    
                    
                    Empleado_Contrato empresa = new Empleado_Contrato();
                    var _vacas = vacas.WordDoc(item);
                     empresa = vacas.EmpresaEmpleado(_vacas.IdEmpleado);
                    if (empresa.IdEmpresaFiscal == null || empresa.IdEmpresaFiscal == 0)
                    {
                        if (empresa.IdEmpresaAsimilado == null || empresa.IdEmpresaAsimilado == 0)
                        {
                            if (empresa.IdEmpresaComplemento == null || empresa.IdEmpresaComplemento == 0)
                            {
                                empresadoc = _ctx.Empresa.Where(x => x.IdEmpresa == empresa.IdEmpresaSindicato).Select(x => x.RazonSocial).FirstOrDefault();
                            }
                            else
                            {
                                empresadoc = _ctx.Empresa.Where(x => x.IdEmpresa == empresa.IdEmpresaComplemento).Select(x => x.RazonSocial).FirstOrDefault();
                            }
                        }
                        else
                        {
                            empresadoc = _ctx.Empresa.Where(x => x.IdEmpresa == empresa.IdEmpresaAsimilado).Select(x => x.RazonSocial).FirstOrDefault();
                        }
                    }
                    else
                    {
                        empresadoc = _ctx.Empresa.Where(x=>x.IdEmpresa == empresa.IdEmpresaFiscal).Select(x=>x.RazonSocial).FirstOrDefault();
                    }
                    var nombre = "Constancia_Vacaciones_" + _vacas.Paterno + "_" + _vacas.Materno + "_" + _vacas.Nombres;
                    var newDoc = newPath + nombre + ".docx";
                    int sumatoria = vacas.sumatoria(_vacas.idvacaciones, _vacas.IdPeriodo);
          

                    var dias = Convert.ToString(_vacas.Dias);
                    var anio = Convert.ToString(_vacas.Inicio.Year);
                    var tomados = Convert.ToString(sumatoria);
                    var pendientes = _vacas.DiasTotales - (_vacas.Dias + sumatoria);
                    var DiasPendientes = Convert.ToString(pendientes);

                    documento.ReplaceText("<<Empleado_Empresa>>",empresadoc);
                    documento.ReplaceText("<<Nombres>>", _vacas.Nombres);
                    documento.ReplaceText("<<Paterno>>", _vacas.Paterno);
                    documento.ReplaceText("<<Materno>>", _vacas.Materno);
                    documento.ReplaceText("<<FechaDeAlta>>", _vacas.Alta.ToString("dd/MM/yyyy"));
                    documento.ReplaceText("<<Departamento>>", _vacas.Depa);
                    documento.ReplaceText("<<Puesto>>", _vacas._Puesto);
                    documento.ReplaceText("<<FechaActual>>", DateTime.Today.ToString("dd/MM/yyyy"));
                    documento.ReplaceText("<<Periodo>>", _vacas.PerioVaca);
                    documento.ReplaceText("<<Vacaciones_Anio>>", anio);
                    documento.ReplaceText("<<FechaInicio>>", _vacas.Inicio.ToString("dd/MM/yyyy"));
                    documento.ReplaceText("<<FechaFin>>", _vacas.Fin.ToString("dd/MM/yyyy"));
                    documento.ReplaceText("<<Presentarse>>", _vacas.Presentarse.ToString("dd/MM/yyyy"));
                    documento.ReplaceText("<<Dias>>", dias);
                    documento.ReplaceText("<<DiasTomados>>", tomados);
                    documento.ReplaceText("<<DiasPendientes>>", DiasPendientes);
                    documento.SaveAs(newDoc);
                }


            }
        }
        //Movimiento de Personal
        private void PlantillaMovimientoPersonal(string file, string newPath, int[] empleados)
        {
            
            Empleados emp = new Empleados();
            Puestos ps = new Puestos();
            
            foreach (var item in empleados)
                {
                    using (var document = DocX.Load(file))
                    {
                        var cuentab = " ";
                        var descripcionb = " ";
                        var empleado = emp.GetEmpleadoById(item);
                        var contrato = emp.ObtenerContratoEmpleadoPorId(item);
                        var bancos = emp.GetDatosBancariosByIdEmpleado(item);
                        if (bancos != null)
                        {
                            cuentab = bancos.CuentaBancaria;
                            descripcionb = bancos.Descripcion;

                        }
                        var tipocontrato = _ctx.C_TipoContrato_SAT.Where(x => x.IdTipoContrato == contrato.TipoContrato).FirstOrDefault();
                        var creditoinfonavit = _ctx.Empleado_Infonavit.Where(x => x.IdEmpleadoContrato == contrato.IdContrato).FirstOrDefault();
                    var cliente = (from s in _ctx.Sucursal
                                   join c in _ctx.Cliente
                                   on s.IdCliente equals c.IdCliente
                                   where s.IdSucursal == contrato.IdSucursal
                                   select c.Nombre).FirstOrDefault();
                        decimal sr = contrato.SalarioReal == 0 ? 0 : contrato.SalarioReal;
                        decimal salarioMensual = sr * 30;
                        var newDoc = newPath + empleado.APaterno + "_" + empleado.AMaterno + "_" + empleado.Nombres + "_.docx";
                        document.ReplaceText("<<Empleado_Cliente>>", cliente);
                        document.ReplaceText("<<Empleado_Clave>>", empleado.IdEmpleado.ToString());
                        document.ReplaceText("<<Empleado_Paterno>>", empleado.APaterno);
                        document.ReplaceText("<<Empleado_Materno>>", empleado.AMaterno);
                        document.ReplaceText("<<Empleado_Nombres>>", empleado.Nombres);
                        document.ReplaceText("<<Empleado_CURP>>", empleado.CURP);
                        document.ReplaceText("<<Empleado_RFC>>", empleado.RFC);
                        document.ReplaceText("<<Empleado_NSS>>", empleado.NSS == null ? " " : empleado.NSS);
                        document.ReplaceText("<<Empleado_EstadoCivil>>", empleado.EstadoCivil);
                        document.ReplaceText("<<Empleado_Sexo>>", empleado.Sexo == "H"? "Hombre" :"Mujer");
                        document.ReplaceText("<<Empleado_Domicilio>>", empleado.Direccion);
                        document.ReplaceText("<<Empleado_Entidad>>", contrato.EntidadDeServicio);
                        document.ReplaceText("<<Empleado_UMF>>", contrato.UMF== null ?" ": contrato.UMF);
                        document.ReplaceText("<<Empleado_FechaDeNacimiento>>", empleado.FechaNacimiento.ToString("dd/MM/yyyy"));
                        document.ReplaceText("<<Empleado_EntidadDeNacimiento>>", empleado.Estado);
                        var puesto = ps.GetPuesto(contrato.IdPuesto);
                        document.ReplaceText("<<Empleado_Puesto>>", puesto.Descripcion);
                        document.ReplaceText("<<Empleado_TipoEmpleado>>", tipocontrato.Descripcion);
                       var departamento = (from d in _ctx.Departamento
                                        join p in _ctx.Puesto
                                        on d.IdDepartamento equals p.IdDepartamento
                                        where p.IdPuesto == puesto.IdPuesto
                                        select d.Descripcion).FirstOrDefault();
                        document.ReplaceText("<<Empleado_Departamento>>", departamento);
                        document.ReplaceText("<<Empleado_SalarioMensual>>", salarioMensual.ToString());
                        document.ReplaceText("<<Empleado_FechaAntiguedad>>", contrato.FechaReal.ToString("dd/MM/yyyy"));
                        document.ReplaceText("<<Empleado_SalarioDiario>>", contrato.SD.ToString());
                        document.ReplaceText("<<Empleado_FechaAltaIMSS>>",contrato.FechaIMSS == null ? " ": contrato.FechaIMSS.Value.ToString("dd/MM/yyyy"));
                        document.ReplaceText("<<Empleado_Compensacion>>", contrato.SalarioReal.ToString());
                        document.ReplaceText("<<Empleado_SalarioDiarioIntegrado>>", contrato.SDI.ToString());
                        document.ReplaceText("<<Empleado_NoCreditoInfonavit>>",creditoinfonavit== null ? " ": creditoinfonavit.NumCredito);
                        //document.ReplaceText("<<Empleado_PorcentajeDescuentoInfonavit>>",;
                        document.ReplaceText("<<Empleado_BancoDeCuenta>>", descripcionb);
                        document.ReplaceText("<<Empleado_CuentaBancaria>>", cuentab);
                        document.ReplaceText("<<Empleado_VenceContrato>>", contrato.Vigencia.ToString());

                        document.SaveAs(newDoc);
                    }
                }
        }


        //Carta de Antigüedad
        private void PlantillaCartaAntiguedad(string file, string newPath, int[] empleados)
        {

            Empleados emp = new Empleados();
            foreach (var item in empleados)
            {
                using (var document = DocX.Load(file))
                {
                    var empleado = emp.GetEmpleadoById(item);
                    var contrato = emp.ObtenerContratoEmpleadoPorId(item);
                    var newDoc = newPath + empleado.APaterno + "_" + empleado.AMaterno + "_" + empleado.Nombres + "_.docx";
                    document.ReplaceText("<<Empleado_Paterno>>", empleado.APaterno);
                    document.ReplaceText("<<Empleado_Materno>>", empleado.AMaterno);
                    document.ReplaceText("<<Empleado_Nombres>>", empleado.Nombres);
                    document.ReplaceText("<<Empleado_FechaAntiguedad>>", contrato.FechaReal.ToString("dd/MM/yyyy"));
                    document.SaveAs(newDoc);
                }
            }
        }

            //Gastos Médicos Menores
        private void PlantillaGastosMedicosMenores(string file, string newPath, int[] empleados)
        {

            Empleados emp = new Empleados();
            foreach (var item in empleados)
            {
                using (var document = DocX.Load(file))
                {
                    var empleado = emp.GetEmpleadoById(item);
                    var contrato = emp.ObtenerContratoEmpleadoPorId(item);
                    var newDoc = newPath + empleado.APaterno + "_" + empleado.AMaterno + "_" + empleado.Nombres + "_.docx";
                    document.ReplaceText("<<Empleado_Paterno>>", empleado.APaterno);
                    document.ReplaceText("<<Empleado_Materno>>", empleado.AMaterno);
                    document.ReplaceText("<<Empleado_Nombres>>", empleado.Nombres);
                    document.ReplaceText("<<Empleado_Sexo>>", empleado.Sexo == "H" ? "Hombre" : "Mujer");
                    document.ReplaceText("<<Empleado_FechaDeNacimiento>>", empleado.FechaNacimiento.ToString("dd/MM/yyyy"));
                    document.ReplaceText("<<Empleado_EstadoCivil>>", empleado.EstadoCivil);
                    document.ReplaceText("<<Empleado_Telefono>>", empleado.Telefono== null ? " ":empleado.Telefono);
                    document.ReplaceText("<<Empleado_Celular>>", empleado.Celular== null ? " ":empleado.Celular);
                    document.ReplaceText("<<Empleado_Email>>", empleado.Email== null ? " ":empleado.Email);
                    document.SaveAs(newDoc);
                }
            }
        }
        //Sindicato
        private void PlantillaSindicato(string file, string newPath, int[] empleados)
        {

            Empleados emp = new Empleados();
            Puestos ps = new Puestos();
            foreach (var item in empleados)
            {
                using (var document = DocX.Load(file))
                {
                    var empleado = emp.GetEmpleadoById(item);
                    var contrato = emp.ObtenerContratoEmpleadoPorId(item);
                    var newDoc = newPath + empleado.APaterno + "_" + empleado.AMaterno + "_" + empleado.Nombres + "_.docx";
                    document.ReplaceText("<<Empleado_FechaDeAlta>>", contrato.FechaAlta.ToString("dd/MM/yyyy"));
                    document.ReplaceText("<<Empleado_Paterno>>", empleado.APaterno);
                    document.ReplaceText("<<Empleado_Materno>>", empleado.AMaterno);
                    document.ReplaceText("<<Empleado_Nombres>>", empleado.Nombres);
                    document.ReplaceText("<<Empleado_Domicilio>>", empleado.Direccion);
                    document.ReplaceText("<<Empleado_RFC>>", empleado.RFC);
                    document.ReplaceText("<<Empleado_CURP>>", empleado.CURP);
                    var puesto = ps.GetPuesto(contrato.IdPuesto);
                    document.ReplaceText("<<Empleado_Puesto>>", puesto.Descripcion);


                    document.SaveAs(newDoc);
                }
            }
        }

    }
}
