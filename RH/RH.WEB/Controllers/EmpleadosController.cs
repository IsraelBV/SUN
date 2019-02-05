using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using RH.Entidades;
using Common.Enums;
using Common.Helpers;
using FiltrosWeb;
using System.Data;
using SYA.BLL;
using System.Threading.Tasks;

namespace RH.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.Rh)]
    public class EmpleadosController : Controller
    {
        private Empleados ctx = new Empleados();


        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;
            if(sucursal != null)
            {
                //Se obtienen los departamentos y los puestos que tiene la empresa para la recontratación
                Departamentos deptos = new Departamentos();
                Puestos puestos = new Puestos();
                ViewBag.Departamentos = deptos.ObtenerDepartamentosPorEmpresa(sucursal.IdCliente);
                ViewBag.Puestos = puestos.ObtenerPuestosPorEmpresa(sucursal.IdCliente);

                ViewBag.IdSucursal = sucursal.IdSucursal;

                Plantillas pl = new Plantillas();
                ViewBag.Plantillas = pl.GetPlantillasByTipo((int)TipoPlantilla.Contrato, sucursal.IdCliente);

                ViewBag.PlantillasBaja = pl.GetPlantillasByTipo2((int)TipoPlantilla.Baja, sucursal.IdCliente);

                Empresas emp = new Empresas();
                ViewBag.Empresas = emp.GetEmpresasBySucursal(sucursal.IdSucursal);
                ViewBag.Esquemas = emp.GetEsquemas();

                return View();
            } 
            else
            {
                return RedirectToAction("Index", "Home");     
            }
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult EmpleadosActivos(int id)
        {
            var model = ctx.ObtenerEmpleadosPorSucursal(id);
            return PartialView(model);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult EmpleadosInactivos(int id)
        {
            var model = ctx.GetEmpleadosInactivos(id);
            return PartialView(model);
        }

        public string GenerarPlantillaContrato(int idPlantilla, int[] empleados)
        {
            Plantillas p = new Plantillas();
            RHEntities ctx = new RHEntities();
            var tipo = ctx.Plantilla.Where(x => x.Id == idPlantilla).Select(x=>x.Tipo).FirstOrDefault();
            if(tipo == 1)
            {
                return p.FormarPlantilla(idPlantilla, (int)TipoPlantilla.Contrato, empleados);
            }else if (tipo == 7)
            {
                return p.FormarPlantilla(idPlantilla, (int)TipoPlantilla.MovimientoPersonal, empleados);
            }
            else if (tipo == 8)
            {
                return p.FormarPlantilla(idPlantilla, (int)TipoPlantilla.CartaAntiguedad, empleados);
            }
            else if (tipo == 9)
            {
                return p.FormarPlantilla(idPlantilla, (int)TipoPlantilla.GastosMedicosMenores, empleados);
            }
            else if (tipo == 10)
            {
                return p.FormarPlantilla(idPlantilla, (int)TipoPlantilla.Sindicato, empleados);
            }
            
            else
            {
                return "false";
            }
            
        }

        public string GenerarPlantillaBaja(int IdPlantilla, int[] empleados)
        {
            
            
                Plantillas p = new Plantillas();
                return p.FormarPlantilla(IdPlantilla, (int)TipoPlantilla.Baja, empleados);
            
            
        }

        /// <summary>
        /// Descarga la plantilla generada
        /// </summary>
        /// <param name="zipFile"></param>
        /// <returns></returns>
        public FileResult GetPlantilla(string path)
        {
            
            var file = System.IO.File.ReadAllBytes(path);
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "Plantillas.zip");
        }


        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Agregar)]
        public ActionResult NuevoRegistro()
        {

            var sucursal = Session["Sucursal"] as SucursalDatos;
            Clientes cliente = new Clientes();
            var departamentosAsignado = cliente.GetDepartamentosAsignadosBycliente(sucursal.IdCliente);
            ViewBag.DepartamentosAsignados = departamentosAsignado;


            TempData["empleado"] = null;
            TempData["contrato"] = null;
            TempData["config"] = null;
            TempData["dbancarios"] = null;
            return View();
        }


        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Agregar)]
        public PartialViewResult FormDatosPersonales()
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;
            if (sucursal != null)
                ViewBag.Sucursal = sucursal.IdSucursal;
            else
                ViewBag.Sucursal = 0;

            return PartialView();
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult GetEmpleado(int id)
        {
            var model = ctx.GetEmpleadoById(id);
            return PartialView(model);
        }

        [HttpPost]
        public void CrearEmpleado(Empleado model)
        {
            TempData["empleado"] = model;
        }

        public JsonResult UpdateEmpleado(Empleado empleado)
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;
            var idUsuario = SessionHelpers.GetIdUsuario();
            //var result = ctx.UpdateEmpleado(empleado, sucursal.IdSucursal);

            //cambio ya que el id de la sucursal se consigue dentro del siguiente metodo
            var result = ctx.UpdateEmpleado(empleado, idUsuario);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult NewContrato()
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;
            var idCliente = sucursal == null ? 0 : sucursal.IdCliente;

            //Obtener la lista de departamentos que tiene la sucursal
            Departamentos deptos = new Departamentos();
            ViewBag.Departamentos = deptos.ObtenerDepartamentosPorEmpresa(idCliente);

            //Obtener la lista de puestos por empresa
            Puestos puestos = new Puestos();
            ViewBag.Puestos = puestos.ObtenerPuestosPorEmpresa(idCliente);

            //Obtener el factor de integración
            FactoresDeIntegracion fi = new FactoresDeIntegracion();
            ViewBag.Factor = fi.ObtenerFactorUno();


            //Obtener los tipos  de contratos - actualizacion al catalogo del sat
            CatalogosSAT catSat = new CatalogosSAT();
            ViewBag.Contratos = catSat.GetCatalogoTipoContrato();

            //Obtener los tipos de Periodicidades - Actualización al catálogo del sat
            ViewBag.Periodicidades = Cat_Sat.GetPeriodicidadPagos();

            //Obtener los tipos de jornada - Actualización al Catálogo del SAT
            ViewBag.TiposJornada = Cat_Sat.GetTiposJornada();

            //Obtener Métodos de Pago - Actualización al Catálogo del SAT
            ViewBag.MetodosPago = Cat_Sat.GetMetodosPago();

            //Obtener el Tipo de Régimen - Actualización al Catálogo del SAT
            var listaTipoRegimen = Cat_Sat.GetTipoRegimen();
            var listaSLITipoRegimen = listaTipoRegimen.Select(x => new SelectListItem()
            {
                Value = x.IdTipoRegimen.ToString(),
                Text = x.Clave +" - "+ x.Descripcion
                

            }).ToList();
            ViewBag.TipoRegimen = listaSLITipoRegimen;

            Empresas emp = new Empresas();
            ViewBag.Empresas = emp.GetEmpresasBySucursal(sucursal.IdSucursal);

            ViewBag.Esquemas = emp.GetEsquemas();
            //Obtenemos el estado para usarlo en Entidad de Servicio
            var edos = new Estados();
            var lista = edos.GetEstados();
            var listaEstados = lista.Select(x => new SelectListItem()
            {
                Value = x.ClaveEstado.ToString(),
                Text = x.Descripcion,
                Selected = (x.IdEstado == 1)
            }).ToList();
            ViewBag.EstadoLista = listaEstados;
            return PartialView();
        }

        public void CreateContrato(Empleado_Contrato contrato)
        {
            TempData["contrato"] = contrato;
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult GetContrato(int id)
        {
            
            //Obtener el contrato
            var model = ctx.GetUltimoContrato(id);

            var sucursal = Session["Sucursal"] as SucursalDatos;
            var idCliente = sucursal == null ? 0 : sucursal.IdCliente;

            //Obtener la lista de departamentos que tiene la sucursal
            Departamentos deptos = new Departamentos();
            ViewBag.Departamentos = deptos.ObtenerDepartamentosPorEmpresa(idCliente);

            //Obtener la lista de puestos por empresa
            Puestos puestos = new Puestos();
            ViewBag.Puestos = puestos.ObtenerPuestosPorEmpresa(idCliente);


            //Obtener los tipos  de contratos - actualizacion al catalogo del sat
            CatalogosSAT catSat = new CatalogosSAT();
            ViewBag.Contratos = catSat.GetCatalogoTipoContrato();

            //Obtener los tipos de Periodicidades - Actualización al catálogo del sat
            ViewBag.Periodicidades = Cat_Sat.GetPeriodicidadPagos();

            //Obtener los tipos de jornada - Actualización al Catálogo del SAT
            ViewBag.TiposJornada = Cat_Sat.GetTiposJornada();

            //Obtener Métodos de Pago - Actualización al Catálogo del SAT
            ViewBag.MetodosPago = Cat_Sat.GetMetodosPago();

            //Obtener el Tipo de Régimen - Actualización al Catálogo del SAT
            var listaTipoRegimen = Cat_Sat.GetTipoRegimen();
            var listaSLITipoRegimen = listaTipoRegimen.Select(x => new SelectListItem()
            {
                Value = x.IdTipoRegimen.ToString(),
                Text = x.Clave +" - " + x.Descripcion
            }).ToList();

            //Agregamos una opcion inical del array

            var itemNew = new SelectListItem()
            {
                Value = "0",
                Text = "No tiene Tipo Regimen",
                Selected = true
            };
            listaSLITipoRegimen.Insert(0, itemNew);


            ViewBag.TipoRegimen = listaSLITipoRegimen;
            //entidad de servicio
            var edos = new Estados();
            var lista = edos.GetEstados();
            //Creamos una lista de elementos seleccionables vacía 
            var listaEstados = new List<SelectListItem>();
            //Creamos un elementos Select List Item que represente el valor nulo como elemento inicial
            SelectListItem item = new SelectListItem() {Value="",Text="No Asignado"};
            //agregamos el elemento creado a la lista vacia
            listaEstados.Add(item);
            //creamos una segunda lista con los elementos de los estados
            var listaEstados2 = lista.Select(x => new SelectListItem()
            {
                Value = x.ClaveEstado.ToString(),
                Text = x.Descripcion,
                Selected = (x.ClaveEstado == model.EntidadDeServicio )
            }).ToList();
            //concatenamos la lista dos a la primera lista
            listaEstados.AddRange(listaEstados2);
            ViewBag.EstadoLista = listaEstados;
            //Obtener el factor de integración
            FactoresDeIntegracion fi = new FactoresDeIntegracion();
            ViewBag.Factor = fi.ObtenerFactorUno();

            return PartialView(model);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Editar)]
        public JsonResult UpdateContrato(Empleado_Contrato model)
        {
                var idUsuario = SessionHelpers.GetIdUsuario();
                var response = ctx.UpdateContrato2(model, idUsuario);
                return Json(response, JsonRequestBehavior.AllowGet);
            
        }
        [HttpPost]
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Editar)]
        public void ConfigurarEmpresas(int[] Empresas, int IdEmpleado = 0)
        {
            //if (IdEmpleado == 0)
                TempData["config"] = Empresas;
            //else
               // ctx.ConfigurarEmpleadoEmpresas(IdEmpleado, Empresas);
        }
        public ActionResult TerminarProcesoNuevoEmpleado()
        {
            var idUsuario = SessionHelpers.GetIdUsuario();
            var empleado = TempData["empleado"] as Empleado;
                empleado.FechaReg = DateTime.Now;
                empleado.IdUsuarioReg = idUsuario;

            var contrato = TempData["contrato"] as Empleado_Contrato;
                contrato.FechaReg = DateTime.Now;
                contrato.IdUsuarioReg = idUsuario;

            var dBancarios = TempData["dbancarios"] as DatosBancarios;

            var IdEmpleado = ctx.CrearEmpleado(empleado, idUsuario);
            if (IdEmpleado > 0)
            {
                contrato.IdEmpleado = IdEmpleado;
                contrato.IdSucursal = empleado.IdSucursal;
                ctx.CrearContrato(contrato);
                if (dBancarios != null)
                {
                    dBancarios.IdEmpleado = IdEmpleado;
                    ctx.NewDatosBancarios(dBancarios);
                }
                var noti = new BLL.Notificaciones();
                noti.Alta(IdEmpleado,idUsuario);
               // noti.IMSS(contrato);
                //Asignar conceptos Default
                if(empleado != null)
                ConceptosNomina.AsignarConceptosDefaultByEmpleado(empleado.IdSucursal, empleado.IdEmpleado);    
            }
            return RedirectToAction("Index");
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult NewDatosBancarios()
        {
            Bancos bnc = new Bancos();
            ViewBag.Bancos = bnc.GetBancos();
            return PartialView();
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Agregar)]
        public JsonResult NuevaCuentaBancaria(DatosBancarios model)
        {
            if (model.IdEmpleado > 0)
            {
                int IdEmpleado = ctx.NewDatosBancarios(model);
                return Json(IdEmpleado, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["dbancarios"] = model;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Devuelve todos los datos bancarios del empleado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult GetDatosBancarios(int id)
        {
            var model = ctx.GetDatosBancarios(id);
            return PartialView(model);
        }

        /// <summary>
        /// Devuelve una vista parcial de un registro del dato bancario
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult GetDatosBancariosById(int id)
        {
            var model = ctx.GetDatosBancariosById(id);
            Bancos bnc = new Bancos();
            ViewBag.Bancos = bnc.GetBancos();
            return PartialView(model);
        }

        public JsonResult UpdateDatosBancarios(DatosBancarios updated)
        {
            try
            {
                var status = ctx.UpdateDatosBancarios(updated);
                if (status)
                    return Json(updated.IdEmpleado, JsonRequestBehavior.AllowGet);
                else
                    return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch(Exception )
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public ActionResult Detalle(int id)
        {
            ViewBag.IdEmpleado = id;
            var empleado = ctx.GetDatosEmpleado(id);
            return View(empleado);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Eliminar)]
        [HttpPost]
        public ActionResult EliminarRegistro(int[] empleados, DateTime fecha, string MotivoBaja, string ComentarioBaja)
        {
            var idU = SessionHelpers.GetIdUsuario();
            var numRecords = 0;
            foreach (var id in empleados)
            {
                var response = ctx.BajaEmpleado(id, fecha, idU,MotivoBaja,ComentarioBaja);
                numRecords = response ? numRecords + 1 : numRecords;
            }
            return Json(numRecords, JsonRequestBehavior.AllowGet);   
        }

        public ActionResult ReenvioAlerta(int [] empleados) 
        {
            try
            {
                var not = new BLL.Notificaciones();
                foreach (var e in empleados)
                {
                    not.ReenvioAlta(e);
                }
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            
            
        }
        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public PartialViewResult FormRecontratacion()
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;
            

            //Obtener la lista de departamentos que tiene la sucursal
            Departamentos deptos = new Departamentos();
            ViewBag.Departamentos = deptos.ObtenerDepartamentosPorEmpresa(sucursal.IdCliente);

            //Obtener la lista de puestos por empresa
            Puestos puestos = new Puestos();
            ViewBag.Puestos = puestos.ObtenerPuestosPorEmpresa(sucursal.IdCliente);

            //Obtener el factor de integración
            FactoresDeIntegracion fi = new FactoresDeIntegracion();
            ViewBag.Factor = fi.ObtenerFactorUno();

            //Obtener las empresas
            Empresas emp = new Empresas();
            ViewBag.Empresas = emp.GetEmpresasBySucursal(sucursal.IdSucursal);


            //Obtener el ultimo contrato
    

            ViewBag.Esquemas = emp.GetEsquemas();
            return PartialView();
        }

        public PartialViewResult FormCambiosalario()
        {

            //Obtener el factor de integración
            FactoresDeIntegracion fi = new FactoresDeIntegracion();
            ViewBag.Factor = fi.ObtenerFactorUno();

            return PartialView();
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Agregar)]
        public ActionResult Recontratar(Empleado_Contrato model)
        {
            var idUsuario = SessionHelpers.GetIdUsuario();
            var response = ctx.Recontratacion(model, idUsuario);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cambiosalario(int idContrato, DateTime Fechainicio  , decimal SalarioReal =0, decimal SD = 0, decimal SDI = 0)
        {
            var idUsuario = SessionHelpers.GetIdUsuario();
            var response =  ctx.Cambiosalario(idUsuario,idContrato, Fechainicio, Math.Round(SalarioReal,4),Math.Round(SD,4),Math.Round(SDI,4));           
            return Json(idUsuario, JsonRequestBehavior.AllowGet);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Consultar)]
        public ActionResult KardexEmpleado(int empleado)
        {
            KardexEmpleado kardex = new KardexEmpleado();
            var lista = kardex.GetKardexEmpleado(empleado);
            return View(lista);
        }

        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Agregar)]
        public ActionResult ImportacionMasiva()
        {
            var sucursal = Session["Sucursal"] as SucursalDatos;

            Clientes cliente = new Clientes();

            var departamentosAsignado = cliente.GetDepartamentosAsignadosBycliente(sucursal.IdCliente);

            ViewBag.DepartamentosAsignados = departamentosAsignado;

            return View();
        }
        public FileResult DescargarPlantilla() 
        {
            //Obtiene el nombre del usuario
            var id = SessionHelpers.GetIdUsuario();
            ControlUsuario cu = new ControlUsuario();
            var usuario = cu.GetUsuarioById(id);
            var userName = usuario == null ? "Alianza" : usuario.Usuario;

            //Se crea la carpeta temporal
            var path = Server.MapPath("~//Files/ImportacionMasiva/" + userName);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            else
            {
                System.IO.Directory.Delete(path, true);
                System.IO.Directory.CreateDirectory(path);
            }

            //Path del archivo nuevo en la carpeta temporal
            var fileName = Server.MapPath("~//Files/ImportacionMasiva/" + userName + "/ImportacionMasiva.xlsx");

            //Copia la plantilla a la carpeta temporal
            var pathPlantilla = Server.MapPath("~//Files/ImportacionMasiva/Plantilla.xlsx");
            System.IO.File.Copy(pathPlantilla, fileName);


            //se obtiene los datos de la sucursal
            var sucursal = Session["sucursal"] as SucursalDatos;    

            //se genera el archivo
            ImportacionMasivaEmpleados ime = new ImportacionMasivaEmpleados();
            var status = ime.FormarPlantilla(fileName, sucursal.IdCliente, sucursal.IdSucursal, sucursal.Nombre);
            
            //Se lee el archivo
            byte[] fyleBytes = System.IO.File.ReadAllBytes(fileName);

            //Se crea un nuevo nombre de archivo para el usuario
            string newFileName = "Plantilla-Importacion-Masiva.xlsx";

            //se nombra la plantilla de excel 
            var nombreArchivoPlantilla = "Plantilla-"+ sucursal.Nombre + "-" + sucursal.Ciudad+".xlsx";

            //Regresa el archivo
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivoPlantilla);
        }
        
        //public ActionResult ExportarReporte()
        //{
        //    return View();
        //}
        public JsonResult ExportarEmpleados()
        {
            ExportarReporte reporte = new ExportarReporte();
            var idusuario = SessionHelpers.GetIdUsuario();
            var sucursal = Session["sucursal"] as SucursalDatos;
            var ruta = Server.MapPath("~/Files/ExportarEmpleados/");
            var resultado = reporte.CrearExcel(sucursal.IdSucursal, ruta, idusuario, sucursal.Nombre, sucursal.Ciudad);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerContrato(int idEmpleado)
        {
            Empleados contrato = new Empleados();
            var con = contrato.GetUltimoContrato(idEmpleado); 
            return Json(con, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerFactor(string fechaReal, string fechaNomina)
        {
            DateTime date2 = DateTime.Parse(fechaReal);
            DateTime date1 = DateTime.Parse(fechaNomina);

            TimeSpan ts = date1 - date2;
            // Difference in days.
            int differenceInDays = ts.Days;
            int anti = (differenceInDays / 365);

            FactoresDeIntegracion fi = new FactoresDeIntegracion();
            var factor = fi.ObtenerFactorUno(anti==0?1:anti);
            return Json(factor, JsonRequestBehavior.AllowGet);
        }
        public FileResult descargarArchivo(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }

        //public FileResult DescargarReporte()
        //{
        //    //Obtiene el nombre del usuario
        //    var id = SessionHelpers.GetIdUsuario();
        //    ControlUsuario cu = new ControlUsuario();
        //    var usuario = cu.GetUsuarioById(id);
        //    var userName = usuario == null ? "Alianza" : usuario.Usuario;

        //    //Se crea la carpeta temporal
        //    var path = Server.MapPath("~//Files/ExportarReporte/" + userName);
        //    if (!System.IO.Directory.Exists(path))
        //        System.IO.Directory.CreateDirectory(path);
        //    else
        //    {
        //       // System.IO.Directory.Delete(path, true);
        //        System.IO.Directory.CreateDirectory(path);
        //    }

        //    //Path del archivo nuevo en la carpeta temporal
        //    Random random = new Random();
        //    var aleatorio = random.Next();
        //    var fileName = Server.MapPath("~//Files/ExportarReporte/" + userName + "/ExportarReporte_"+ aleatorio + ".xlsx");

        //    //Copia la plantilla a la carpeta temporal
        //    var pathPlantilla = Server.MapPath("/Files/ExportarReporte/Plantilla.xlsx");
        //    if (System.IO.File.Exists(pathPlantilla))
        //    {
        //        System.IO.File.Copy(pathPlantilla, fileName);
        //    }

        //    //se obtiene los datos de la sucursal
        //    var sucursal = Session["sucursal"] as SucursalDatos;

        //    //se genera el archivo
        //    ExportarReporte exp = new ExportarReporte();
        //    var status = exp.FormarPlantilla(sucursal.IdSucursal, fileName, true);

        //    //Se lee el archivo
        //    byte[] fyleBytes = System.IO.File.ReadAllBytes(fileName);

        //    //Se crea un nuevo nombre de archivo para el usuario
        //    string newFileName = "Reporte_Empleados_"+sucursal.Nombre+"_"+sucursal.Ciudad+".xlsx";

        //    //Regresa el archivo
        //    return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, newFileName);
        //}

        //este metodo ya no se utiliza

        //[HttpPost]
        //public PartialViewResult ExUploadFile()
        //{
        //    var file = System.Web.HttpContext.Current.Request.Files["UploadedImage"];
        //    var data = new System.Data.DataTable();
        //    var validation = 1;
        //    if (file != null)
        //    {
        //        var sucursal = Session["Sucursal"] as SucursalDatos;
        //        if (sucursal == null) return PartialView();

        //        ImportacionMasivaEmpleados ctxImp = new ImportacionMasivaEmpleados();
        //        if (!file.FileName.EndsWith(".xls"))
        //            validation = ctxImp.ExValidateExcel(file, sucursal.Nombre);

        //        if (validation == 1)
        //        {
        //            data = ctxImp.ExExcelToDataTable(file);
        //            if (data != null)
        //            {
        //                data = ctxImp.LimpiarTabla(data);

        //                DataView dv = data.DefaultView;
        //                dv.Sort = "Paterno asc";//desc

        //                DataTable sortedDT = dv.ToTable();
        //                data = sortedDT;
        //                TempData["dtEmp"] = data;
        //            }
        //            else validation = 2;
        //        }
        //    }
        //    else
        //        validation = 2;
        //    switch (validation)
        //    {
        //        case 1:
        //            ViewBag.Error = "El archivo está vacío";
        //            break;
        //        case 2:
        //            ViewBag.Error = "El archivo está vacío!";
        //            break;
        //        case 3:
        //            ViewBag.Error = "No es un archivo de excel!";
        //            break;
        //        case 4:
        //            ViewBag.Error = "La plantilla no corresponde al Cliente en el que desea importar los datos!";
        //            break;
        //        default:
        //            break;
        //    }
        //    return PartialView(data);
        //}

        [HttpPost]
        public PartialViewResult UploadFile()
        {         
            var file = Request.Files["UploadedImage"];
            var data = new System.Data.DataTable();
            List<ValidationEmpleado> empleadosValidacion = new List<ValidationEmpleado>();
            String[] warnings = new String[] {
                "",
                "EL ARCHIVO TIENE QUE SER EXCEL",
                "LA PLANTILLA NO CORRESPONDE A LA SUCURSAL EN LA CUAL SE QUIERE IMPORTAR",
                "EL ARCHIVO ESTA VACIO",
                "DOCUMENTO NO VALIDO"
            };
            int validation = 0;

            TempData["dtEmp"] = null;// limpia la variable para que no exista informacion previa guardada

            if (file != null)
            {
                ImportacionMasivaEmpleados ctxImp = new ImportacionMasivaEmpleados();
                var sucursal = Session["Sucursal"] as SucursalDatos;
                //valida que el documento sea un documento de .xls/.xlsx si no devuelve 1 y que el documento le pertenesca a la empresa si no devuelve 2.
                validation = ctxImp.ValidateExcel(file, sucursal.Nombre);

                if (validation == 0)
                {   
                    //se verifica el datatable y los datos se validan/ devuelve una tupla con 3 items 1.-el DartaTable, 2.-la lista de los errore y advertencias, 3.-la lista de los enbezados de las coluimnas que presentan un error
                    var infoDatatable = ctxImp.ExcelToDataTable(file);
                    data = infoDatatable.Item2;
                    
                    if (data.Rows.Count > 0)
                    {                     
                        empleadosValidacion = infoDatatable.Item1;
                        TempData["dtEmp"] = empleadosValidacion.Count == 0 ? data : null;
                    }
                    else
                    {
                        validation = 3;
                    }
                }
            }
            else
            {
                validation = 3;
            }
            var resultado = Tuple.Create(empleadosValidacion, warnings[validation]);

            return PartialView(resultado);
        }


        [AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Agregar)]
        public async Task<JsonResult> UploadRecords()
        {
            var data = (System.Data.DataTable)TempData["dtEmp"];
            //si la variable global que contiene los datos de datatable es diferente a null
            if (data != null)
            {
                var idUsuario = SessionHelpers.GetIdUsuario();
                ImportacionMasivaEmpleados ctxImp = new ImportacionMasivaEmpleados();
                var sucursal = Session["Sucursal"] as SucursalDatos;
                //se verifica si hubo algun error al registrar a los empleados
                var resutadoUploadAsync = await ctxImp.UploadRecordsAsync(data, sucursal.IdSucursal, sucursal.IdCliente, idUsuario);

                if (resutadoUploadAsync)
                {
                    return Json(new
                    {
                        type = "success",
                        msg = "DATOS REGISTRADOS"
                    });
                    //return Json( "success", "DATOS REGISTRADOS" );
                }
                else {
                    return Json(new
                    {
                        type = "danger",
                        msg = "OCURRIÓ UN ERROR AL INSERTAR LOS DATOS"
                    });
                    //return Json("danger","OCURRIO UN ERROR AL INSERTAR LOS DATOS");
                }
            }
            else
            {
                return Json(new
                {
                    type = "danger",
                    msg = "ACCION NO VALIDA"
                });
                //return Json("danger","ACCION NO VALIDA");
            }
        }

        //[AccesoModuloAttribute(Modulo = Modulos.RHEmpleados, Accion = AccionCrud.Agregar)]
        //public ActionResult UploadRecords()
        //{
        //    var data = (System.Data.DataTable)TempData["dtEmp"];
        //    if (data != null)
        //    {
        //        var idUsuario = SessionHelpers.GetIdUsuario();
        //        ImportacionMasivaEmpleados ctxImp = new ImportacionMasivaEmpleados();
        //        var sucursal = Session["Sucursal"] as SucursalDatos;
        //        ctxImp.UploadRecords(data, sucursal.IdSucursal, sucursal.IdCliente, idUsuario);
        //        return RedirectToAction("Index", "Empleados");
        //    }
        //    else {
        //        return RedirectToAction("ImportacionMasiva");
        //    }
        //}


        /// <summary>
        /// Metodo de cambio de Empresa del o los empleados
        /// </summary>
        /// <param name="Empleados"></param>
        /// <param name="IdEmpresaFiscal"></param>
        /// <param name="IdEmpresaComplemento"></param>
        /// <param name="IdEmpresaAsimilado"></param>
        /// <param name="IdEmpresaSindicato"></param>
        /// <returns></returns>
        public JsonResult TransferEmployees(int[] Empleados, int? IdEmpresaFiscal, int? IdEmpresaComplemento, int? IdEmpresaAsimilado, int? IdEmpresaSindicato, string fechaIMSS)
        {
            int i = 0;
            foreach(var IdEmpleado in Empleados)
            {
                var status = ctx.ConfigurarEmpleadoEmpresas(IdEmpleado, IdEmpresaFiscal, IdEmpresaComplemento, IdEmpresaAsimilado, IdEmpresaSindicato,fechaIMSS);
                if (status) i++;
            }
            return Json(i, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetDocumentosEmpleado(int IdEmpleado)
        {
            DocumentosEmpleado de = new DocumentosEmpleado();
            var model = de.GetDocumentosEmpleado(IdEmpleado);
            var tipos = de.GetTiposDocumento();
            var listaTipos = new List<C_DocumentosTipo>();
            foreach(var tipo in tipos)
            { 
                if (!model.Select(x => x.IdTipoDocumento).Contains(tipo.IdTipoDocumento))
                    listaTipos.Add(tipo);
            }
            ViewBag.Tipos = listaTipos;
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UploadDoc(int? IdTipoDocumento = null, int? IdEmpleado = null)
        {
            if (IdTipoDocumento == null)
                return Json(new { status = false, message = "ID TIPO DOCUMENTO NO VALIDO" }, JsonRequestBehavior.AllowGet);
            if (IdEmpleado == null)
                return Json(new { status = false, message = "ID EMPLEADO NULO" }, JsonRequestBehavior.AllowGet);

            var file = System.Web.HttpContext.Current.Request.Files["Document"];
            if (file == null)
                return Json(new { status = false, message = "NO EXISTE EL ARCHIVO" }, JsonRequestBehavior.AllowGet);

            string filename = file.FileName;
            string extension = System.IO.Path.GetExtension(filename).ToUpper();
            if (extension == ".PDF")
            {
                DocumentosEmpleado docs = new DocumentosEmpleado();
                filename = docs.GetNombreTipoDocumento((int)IdTipoDocumento);
                filename = IdEmpleado + " - " + filename + ".PDF";

                DocumentosEmpleados doc = new DocumentosEmpleados();
                doc.IdEmpleado = (int)IdEmpleado;
                doc.NombreDocumento = filename;
                doc.IdTipoDocumento = (int)IdTipoDocumento;
                doc.Status = true;

                var path = Server.MapPath("~/Files/DocumentosEmpleados/");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                filename = path + "/" + filename;
                try
                {
                    file.SaveAs(filename);
                }
                catch (HttpException ex)
                {
                    return Json(new { status = false, message = "Ocurrió un error al guardar el archivo. CODIGO ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
                docs.NewDocument(doc);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { status = false, message = "FORMATO DE ARCHIVO NO VÁLIDO, POR FAVOR INGRESA UN PDF." }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadDocument(int IdDocumento)
        {
            DocumentosEmpleado ctxDocs = new DocumentosEmpleado();
            var documento = ctxDocs.GetDocumentoById(IdDocumento);
            var path = Server.MapPath("~/Files/DocumentosEmpleados/" + documento.NombreDocumento);
            var file = System.IO.File.ReadAllBytes(path);
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, documento.NombreDocumento);
        }

        public JsonResult DeleteDocument(int id)
        {
            DocumentosEmpleado ctsDocs = new DocumentosEmpleado();
            var documento = ctsDocs.GetDocumentoById(id);
            var status = ctsDocs.DeleteDocumento(documento);
            if (!status)
                return Json(new { status = status, message = "Error eliminando el documento en la base de datos" }, JsonRequestBehavior.AllowGet);

            var path = Server.MapPath("~/Files/DocumentosEmpleados/" + documento.NombreDocumento);
            try
            {
                System.IO.File.Delete(path);
            }
            catch(System.IO.DirectoryNotFoundException dirNotFoundEx)
            {
                return Json(new { status = false, message = "Error eliminando el documento del Servidor, COD ERROR:" + dirNotFoundEx.Message }, JsonRequestBehavior.AllowGet);
            }
            catch(System.IO.IOException ex)
            {
                return Json(new { status = false, message = "Error eliminando el documento del Servidor, COD ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch(System.UnauthorizedAccessException ex)
            {
                return Json(new { status = false, message = "Error eliminando el documento del Servidor, COD ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true, message = "El documento se ha eliminado correctamente" }, JsonRequestBehavior.AllowGet);
        }
    }
}