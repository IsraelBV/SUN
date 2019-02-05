using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RH.Entidades;
using Nomina.BLL;
using Common.Helpers;
using Nomina.Reportes;
namespace Nomina.WEB.Controllers
{
    public class AcumuladoController : Controller
    {
        // GET: Acumulado
        public ActionResult Index()
        {

            _Acumulado ac = new _Acumulado();

            ViewBag.ejercicio = ac.ejercicios();
            ViewBag.empresas = ac.empresas();
            ViewBag.sucursales = ac.GetClientes();
            return PartialView();
        }

        public ActionResult periodoEjercicio(int idEjercicio, int IdBimestre, int IdEmpresa, int IdSucursal)
        {
            _Acumulado ac = new _Acumulado();

            if (idEjercicio == 0 && IdBimestre == 0 && IdEmpresa == 0 && IdSucursal == 0)
            {
                var datos = ac.periodos();
                return PartialView(datos);
            }
            else if (IdBimestre == 0 && idEjercicio != 0 && IdEmpresa == 0 && IdSucursal == 0)
            {
                var datos = ac.periodoByEjercicio(idEjercicio);
                return PartialView(datos);
            }
            else if (IdBimestre != 0 && idEjercicio != 0 && IdEmpresa == 0 && IdSucursal == 0)
            {
                var datos = ac.periodoByEjercicioByBimestre(idEjercicio, IdBimestre);
                return PartialView(datos);
            }
            else if (idEjercicio == 0 && IdEmpresa == 0 && IdSucursal == 0)
            {
                var datos = ac.periodoByBimestre(IdBimestre);
                return PartialView(datos);
            }
            else if (IdBimestre == 0 && idEjercicio == 0 && IdEmpresa != 0 && IdSucursal == 0)
            {
                var datos = ac.periodoByEmpresa(IdEmpresa);
                return PartialView(datos);
            }
            else if (IdSucursal != 0)
            {
                var datos = ac.periodoBySucursal(IdSucursal);
                return PartialView(datos);
            }
            else
            {

                return null;
            }

        }

        public JsonResult generarAcumulado(int[] idPeriodo)
        {
            Reporte_Acumulado acu = new Reporte_Acumulado();
            var idusuario = SessionHelpers.GetIdUsuario();
            //int IdSucursal = sucursal.IdSucursal;
            var ruta = Server.MapPath("~/Files/Acumulado");

            var resultado = acu.crearAcumulado(idusuario, ruta, idPeriodo);
            //var file = System.IO.File.ReadAllBytes(resultado);
            //Regresa el archivo
            //return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "Incidencia.xlsx");
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadAcumulado()
        {
            //20, 21 
            int[] idPeriodo =
             {1 ,5   ,8   ,9   ,13  ,23  ,24  ,25  ,26  ,27  ,
            28  ,29  ,30  ,31  ,32  ,33  ,34  ,35  ,36  ,37  ,38  ,
            39  ,40  ,41  ,42  ,43  ,44  ,45  ,46  ,47  ,48  ,49  ,
            50  ,51  ,52  ,53  ,54  ,55  ,56  ,57  ,58  ,59  ,60  ,
            61  ,62  ,63  ,64  ,65  ,66  ,67  ,68  ,69  ,70  ,71  ,
            72  ,73  ,74  ,75  ,76  ,77  ,78  ,79  ,80  ,81  ,82  ,
            83  ,84  ,85  ,86  ,87  ,88  ,89  ,90  ,91  ,92  ,93  ,
            94  ,95  ,96  ,97  ,98  ,99  ,100 ,101 ,102 ,103 ,104 ,
            105 ,106 ,107 ,108 ,109 ,110 ,111 ,112 ,113 ,114 ,115 ,
            116 ,118 ,119 ,120 ,121 ,122 ,124 ,125 ,126 ,127 ,128 ,
            129 ,130 ,131 ,132 ,133 ,134 ,135 ,136 ,137 ,138 ,139 ,
            140 ,142 ,143 ,144 ,145 ,146 ,147 ,148 ,149 ,150 ,151 ,
            152 ,153 ,154 ,156 ,157 ,158 ,159 ,160 ,161 ,162 ,163 ,
            164 ,165 ,167 ,168 ,169 ,170 ,171 ,172 ,173 ,174 ,175 ,
            176 ,177 ,179 ,180 ,181 ,182 ,183 ,184 ,185 ,186 ,187 ,
            188 ,189 ,190 ,192 ,193 ,194 ,195 ,196 ,197 ,198 ,199 ,
            200 ,201 ,202 ,203 ,205 ,206 ,207 ,208 ,209 ,210 ,211 ,
            212 ,213 ,214 ,215 ,216 ,217 ,218 ,219 ,220 ,222 ,223 ,
            224 ,225 ,226 ,227 ,228 ,229 ,230 ,231 ,232 ,233 ,234 ,
            235 ,236 ,237 ,238 ,239 ,240 ,241 ,242 ,243 ,244 ,245 ,
            246 ,247 ,248 ,252 ,254 ,256 ,257 ,258 ,259 ,260 };

            Reporte_Acumulado acu = new Reporte_Acumulado();
            var ruta = Server.MapPath("~/Files/Acumulados");
            var pathDescarga = "/Files/Acumulados/";
            var archivoBytes = acu.GenerarArchivoAcumulado(0, idPeriodo, ruta, pathDescarga);

            //Se crea un nuevo nombre de archivo para el usuario
            string newFileName = "ACUMULADO__1.xlsx";

            if (archivoBytes == null) return null; //new EmptyResult();



            return File(archivoBytes, "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", newFileName);
        }

        //int idEjercicio, DateTime dateInicial, DateTime dateFinal, int idEmpresa = 0, int idCliente = 0

        public JsonResult ReporteAcumulado(int idEjercicio, DateTime? dateInicial, DateTime? dateFinal, int idEmpresa = 0, int idCliente = 0, bool incluirNoAutorizados = false,bool calculoanual = false)
        {
            if (idEjercicio == null || dateInicial == null || dateFinal == null)
            {
                return Json(new { success = false, error = "Debe seleccionar las fechas de Inicio y Fin", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            if (dateFinal < dateInicial)
            {
                return Json(new { success = false, error = "La Fecha final no debe ser menor que la fecha inicial", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            //Metodo que hace la busqueda de los periodos
            //Reporte_Acumulado acu = new Reporte_Acumulado();
            ReportesGenerales rg = new ReportesGenerales();
            //var listaPeriodos = acu.GetPeriodosAutorizados(idEjercicio, dateInicial, dateFinal, idEmpresa, idCliente);
            
            var ruta = Server.MapPath("~/Files/Acumulados");
            var pathDescarga = "/Files/Acumulados/";

            //var archivoAcumulado = acu.GenerarArchivoAcumulado(0, listaPeriodos, ruta, pathDescarga, idEmpresa, idCliente);
            int idUsuario = SessionHelpers.GetIdUsuario();
            //var archivoAcumulado = acu.GenerarArchivoAcumuladoComplete2(idUsuario, ruta, pathDescarga, idEjercicio, dateInicial, dateFinal, idEmpresa, idCliente, incluirNoAutorizados,calculoanual);
            var archivoAcumulado = rg.GenerarArchivoAcumuladoComplete_NUEVO2018(idUsuario, ruta, pathDescarga, idEjercicio, dateInicial, dateFinal, idEmpresa, idCliente, incluirNoAutorizados,calculoanual);

            if (archivoAcumulado == null)
            {
                return Json(new { success = false, error = "No se encontrarón periodos con esos criterios de busqueda", resultPath = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success= true, error="",  resultPath = archivoAcumulado }, JsonRequestBehavior.AllowGet);
        }

        public FileResult descargarPlantilla(string ruta)
        {
            string[] arrayRuta = ruta.Split('\\');
            int count = arrayRuta.Length;
            byte[] fyleBytes = System.IO.File.ReadAllBytes(ruta);
            return File(fyleBytes, System.Net.Mime.MediaTypeNames.Application.Octet, arrayRuta[count - 1]);
        }
    }
}